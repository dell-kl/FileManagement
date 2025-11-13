using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;
using System.Data;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace SIS_DIAF.Caching
{
    public class CacheCompartida : DbCommandInterceptor
    {

        private readonly IMemoryCache _cache;

        public CacheCompartida(IMemoryCache memoryCache)
        { 
            this._cache = memoryCache;

            if ( this._cache.TryGetValue("tipo", out string tipo))     
                    this._cache.Set("tipo", tipo);
            else
                _cache.Set("tipo", "SELECT");
            
        }

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
    CommandEventData eventData, InterceptionResult<DbDataReader> result,
    CancellationToken cancellationToken = default)
        {
            //capturamos la consulta antes de que se logre enviar a la base de datos. 
            string key = command.CommandText +
                         string.Join(",", command.Parameters.Cast<DbParameter>().Select(p => p.Value));

            //comprobar si el comando se trata de una actualizacion o eliminacion.
            String comandoDML = command.CommandText.Contains("UPDATE") ? "UPDATE" : "SELECT";

            if (_cache.TryGetValue("tipo", out string tipo))
            { 
                if ( tipo.Equals("SELECT") && comandoDML.Equals("SELECT") )
                {
                    //verificar si existe esta clave en la _cache. 
                    if (_cache.TryGetValue(key, out List<Dictionary<string, object>>? cacheEntry))
                    {
                        var table = new DataTable();
                        if (cacheEntry != null && cacheEntry.Any())
                        {
                            foreach (var pair in cacheEntry.First())
                            {
                                table.Columns.Add(pair.Key,
                                    pair.Value is not null && pair.Value?.GetType() != typeof(DBNull)
                                        ? pair.Value.GetType()
                                        : typeof(object));
                            }

                            foreach (var row in cacheEntry)
                            {
                                table.Rows.Add(row.Values.ToArray());
                            }
                        }

                        var reader = table.CreateDataReader();
                        Console.WriteLine("==== READ FROm CACHE ===");
                        return InterceptionResult<DbDataReader>.SuppressWithResult(reader);
                    }
                }
                else if ( comandoDML.Equals("UPDATE") && tipo.Equals("SELECT") ) 
                   _cache.Set("tipo", "UPDATE");
            

            }

            return result;
        }
      
        public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command,
            CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            //este metodo despues de que el comando se ha ejecutado exitosamente y ha devuelto un resultado. 

            var key = command.CommandText + string.Join(",", command.Parameters.Cast<DbParameter>().Select(p => p.Value));

            if ( command.CommandText.Contains("SELECT") )
            {
                if (this._cache.TryGetValue("tipo", out string tipo))
                {
                    if (tipo.Equals("UPDATE"))
                        this._cache.Set("tipo", "SELECT");
                }

                var resultsList = new List<Dictionary<string, object>>();
                if (result.HasRows)
                {
                    while (await result.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < result.FieldCount; i++)
                        {
                            row.TryAdd(result.GetName(i), result.GetValue(i));
                        }

                        resultsList.Add(row);
                    }

                    if (resultsList.Any())
                    {
                        _cache.Set(key, resultsList);
                    }
                }


                result.Close();

                var table = new DataTable();

                //aqui le decimos si solamente contiene uno.
                if (resultsList.Any())
                {

                    foreach (var pair in resultsList.First())
                    {
                        table.Columns.Add(pair.Key,
                            pair.Value is not null && pair.Value?.GetType() != typeof(DBNull)
                                ? pair.Value.GetType()
                                : typeof(object));
                    }

                    foreach (var row in resultsList)
                    {
                        table.Rows.Add(row.Values.ToArray());
                    }
                }

                return table.CreateDataReader();

            }
            else
            {
                _cache.Set("tipo", "UPDATE");
            }

            return result;

        }
    

    }
}

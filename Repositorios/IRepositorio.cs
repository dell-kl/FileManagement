namespace SIS_DIAF.Repositorios
{
    public interface IRepositorio<T> where T : class
    {
        public Task<T> GetxId(long id);
        public Task<T> GetxCod( string c);
        public Task<bool> Create(T x); 
        public Task<bool> Update(T x);
        public Task<bool> Delete(long id);
        public Task<ICollection<T>> Listar();

    }
}

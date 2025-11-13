let indicador = {
    "tipo": 0,
    "entrada": 0
};

document.addEventListener("DOMContentLoaded", (e) => {
    Regms();
});

function Regms()
{
    validarFormularioFiltro();
    botonPermitirFormularioFiltro();
}

function validarFormularioFiltro() {
    let formulario = document.getElementById("formularioBusqueda");
    

    if (formulario) {
        let tipoBusqueda = document.getElementsByClassName("tipoBusqueda");
        let entradaBusqueda = document.getElementsByClassName("entradaBusqueda");

        if (tipoBusqueda.length > 0 && entradaBusqueda.length > 0) {

            tipoBusqueda[0].addEventListener("change", (e) => {
                indicador.tipo = 1;
                botonPermitirFormularioFiltro();
            });

            entradaBusqueda[0].addEventListener("blur", (e) => {
                let valor = e.target.value;

                if (valor.trim().length > 0)
                    indicador.entrada = 1;
                else
                    indicador.entrada = 0;

                botonPermitirFormularioFiltro();
            });

            entradaBusqueda[0].addEventListener("input", (e) => {
                let valor = e.target.value;

                if (valor.trim().length > 0)
                    indicador.entrada = 1;
                else
                    indicador.entrada = 0;

                botonPermitirFormularioFiltro();
            });
        }
    }
}

function botonPermitirFormularioFiltro() {
    let boton = document.querySelector("#formularioBusqueda button");
    
    if (boton) {
        if (indicador.tipo === 1 && indicador.entrada === 1)
            boton.disabled = false;
        else
            boton.disabled = true;
    }

}
document.addEventListener("DOMContentLoaded", (e) => {
    Formularios();
});

let formularioRol = {
    Descripcion: false,
    Estado : false
};

let expresionRegular = {
    descripcion: /^[a-zA-Z]+$/
}

function Formularios() {
    let verificar = window.location.pathname;

    FormularioRol();
    VerificarTipoFormulario();
}


function FormularioRol() {
    /* Capturamos los campos de nuestro formulario */
    const verificarCampos = (e) => {
        let valor = e.target.value;

        if (e.target.name === "descripcion") {

            if (expresionRegular.descripcion.test(valor)) {
                formularioRol.descripcion = true;
                verificarMensajeAlerta("crear", e.target.name, true);
            }
            else {
                formularioRol.descripcion = false;
                verificarMensajeAlerta("crear", e.target.name, false);
            }
        }
        else if (e.target.name === "estado") {
            console.log(valor);

            if (valor === "sin_valor" || valor.length === 0) {
                formularioRol.Estado = false;
                verificarMensajeAlerta("crear", e.target.name, false);
            }
            else {
                formularioRol.Estado = true;
                verificarMensajeAlerta("crear", e.target.name, true);
            }
        }
    };

    let campoDescripcion = document.querySelector("#agregarRol #descripcion");
    campoDescripcion.addEventListener("blur", verificarCampos);

    let campoRol = document.querySelector("#agregarRol select");
    campoRol.addEventListener("blur", verificarCampos);
}


function verificarMensajeAlerta(tipo, campo, estado) {

    if (tipo === "crear") {
        if (campo === "descripcion") {
            let m_c_d = document.getElementById("m-c-d");

            if (estado)
                m_c_d.classList.add("d-none");
            else
                m_c_d.classList.remove("d-none");
        }
        else if (campo === "estado") {
            let m_e_e = document.getElementById("m-e-e");

            if (estado)
                m_e_e.classList.add("d-none");
            else
                m_e_e.classList.remove("d-none");
        }
    }

    BloquearFormulario(tipo);
}


function BloquearFormulario(formulario)
{
    let estado = false;

    if (formularioRol.Descripcion && formulario.Estado)
        estado = true;

    if (formulario === "crear") {
        let botonCrear = document.getElementById("botonRegistrarRol");
        console.log(botonCrear);
        botonCrear.disabled = estado;
    }
    else if (formulario === "actualizar") {
        let botonActualizar = document.querySelectorAll("#botonActualizar");
        botonActualizar.forEach((n) => {
            n.disabled = estado;
        });
    }
}
    
function VerificarTipoFormulario() {
   
    document.getElementById("btn-agregarRol")
        .addEventListener("click", (e) => {
            BloquearFormulario("crear");
        });
}
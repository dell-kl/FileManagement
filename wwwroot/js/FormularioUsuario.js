let formularioValido = {
    "nombre": 0,
    "cedula": 0,
    "email": 0,
    "celular": 0,
    "clave": 0,
    "estado": 0,
    "rol": 0,
    "sucursal": 0
};

let expresionesRegulares = { 
    "nombre": /^[a-zA-Z. ]+$/,
    "cedula": /^[0-9]{10,10}$/,
    "email": /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
    "celular": /^[0-9]{10,10}$/,
    "clave": /^([A-Za-z]|[0-9]|[!@$#]){9,9}$/,
    "estado": /^activo|inactivo$/,
    "rol": /^[0-9]+$/,
    "sucursal" : /^[0-9]+$/
}

document.addEventListener("DOMContentLoaded", (e) => {
    FormUsuario();
});

function FormUsuario()
{
   // ValidarCambioPassword("#formularioCambiarContrasena");
    ValidarCampos("#formularioCrearUsuario");
    ValidarCampos("#formularioEditarUsuario");
}

function ValidarCambioPassword(idFormulario) {
    let password = document.querySelector("input[type=password]");

    let name = password.name;
    let valor = password.value;

    password.addEventListener("input", (e) => {
        name = e.target.name;
        valor = e.target.value;
        desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
    });
    desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
}

function ValidarCampos(idFormulario) {  
    let camposTexto = document.querySelectorAll(`${idFormulario} input`);
    
    desactivarEnvio(desactivar = true, idFormulario);

    camposTexto.forEach(campo => {
        let name = campo.name;
        let valor = campo.value;
        

        campo.addEventListener("input", (e) => {
            name = e.target.name;
            valor = e.target.value;

            desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
            verificarValidezCampos(idFormulario);

        });

        campo.addEventListener("blur", (e) => {
            name = e.target.name;
            valor = e.target.value;

            desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = true);
            verificarValidezCampos(idFormulario);
        })

        desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
        verificarValidezCampos(idFormulario);
    });


    let camposSelect = document.querySelectorAll(`${idFormulario} select`);

    camposSelect.forEach(campo => {
        let name = campo.name;
        let valor = campo.value;

        campo.addEventListener("change", (e) => {
            name = e.target.name;
            valor = e.target.value;

            desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
            verificarValidezCampos(idFormulario);
        });

        desplegarOpcionValidacion(name, valor, idFormulario = idFormulario, evento = false);
        verificarValidezCampos(idFormulario);
    });
}

function desplegarOpcionValidacion(name, valor, idFormulario, evento = false) {
    switch (name) {
        case "nombre":
            validarEntrada(name, valor, "Solo se aceptan el caracter de . y letras", idFormulario);
            break;

        case "cedula":
            validarEntrada(name, valor, "Ingresa un numero de cedula valida", idFormulario);

            if (evento) {
                let resultado = validatCI(valor);

                if (resultado) {
                    formularioValido.cedula = 1;
                    return;
                }
                formularioValido.cedula = 0;
                mostrarMensajeAlerta(name, "Ingresa un numero de cedula valida", true, idFormulario);
            }

            break;

        case "email":
            validarEntrada(name, valor, "Ingresa un email valido", idFormulario);
            break;

        case "celular":
            validarEntrada(name, valor, "Ingresa un numero de celular valido", idFormulario);
            break;

        case "clave":
            validarEntrada(name, valor, "La clave solo acepta Mayúsculas, Minúsculas, números y carácteres !#@$. Debe tener una longitud de 9", idFormulario);
            break;

        case "estado":
            validarEntrada(name, valor, "Escoge un estado para el usuario", idFormulario);
            break;

        case "rol":
            validarEntrada(name, valor, "Escoge un rol para el usuario", idFormulario);
            break;

        case "sucursal":
            validarEntrada(name, valor, "Selecciona la sucursal del usuario", idFormulario);
            break;
    }
}

function mostrarMensajeAlerta(campo, mensaje, tipo, idFormulario) {
    let span = document.querySelector(`${idFormulario} #mensaje-${campo}`);
    span.textContent = mensaje;

    if (tipo) {
        span.classList.add("bg-danger", "fw-bold", "text-white", "p-2", "mt-2", "d-block");
        span.classList.remove("d-none");
        return;
    }

    span.classList.remove("d-block");
    span.classList.add("d-none");
}
function validarEntrada(entrada, valor, mensaje, idFormulario) {
    if (expresionesRegulares[entrada].test(valor)) {
        formularioValido[entrada] = 1;
        mostrarMensajeAlerta(entrada, "", false, idFormulario);
        return true;
    }
    formularioValido[entrada] = 0;
    mostrarMensajeAlerta(entrada, mensaje, true, idFormulario);
    return false;
}

function verificarValidezCampos(idFormulario) {
    let n = 1;

    if (
        formularioValido.nombre === n &&
        formularioValido.cedula === n &&
        formularioValido.email === n &&
        formularioValido.celular === n &&
        formularioValido.clave === n &&
        formularioValido.estado === n &&
        formularioValido.rol === n &&
        formularioValido.sucursal === n
    ) {
        desactivarEnvio(desactivar = false, idFormulario);
    }
    else {
        desactivarEnvio(desactivar = true, idFormulario);
    }
}

function desactivarEnvio(desactivar, idFormulario) {
    let botonRegistrar = document.querySelector(`${idFormulario} #botonEnvioFinal`);

    if (botonRegistrar)
        botonRegistrar.disabled = desactivar;
}   
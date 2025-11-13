document.addEventListener("DOMContentLoaded", (e) => {
    InicioSesion();
});

function InicioSesion() {
    MensajesError();
}

//esto de aqui controla los errores que se genera 
//en los campos del login... 
function MensajesError() {
    let mensajes = document.querySelectorAll(".mensaje");
    mensajes.forEach(mensaje => {
        if (mensaje.textContent.trim() === "") {
            mensaje.classList.remove("error_mensaje");
        }
    });
}
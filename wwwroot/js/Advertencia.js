const formArch = document.querySelectorAll("#form-archivo_subido");
const test = document.getElementById('test');
formArch.forEach((form) => {

    form.addEventListener('submit', (e) => {
        e.preventDefault();
        Swal.fire({
            title: "Advertencia?",
            text: "Si sube el archivo, se remplazara el que esta actualmente",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Continuar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                form.submit();
            }
        });
    });
})

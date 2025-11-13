

const input = document.getElementById('input-ci-ec');
const form = document.getElementsByTagName('form');
var isValidate = undefined;
const valtextNumber = (event) => {

    event.target.value = event.target.value.replace(/\D/g, '');
    if (event.target.value.length > 10) {
        event.target.value = event.target.value.slice(0, 10);
    }
}

const validatCI = (cedula) => {

    //Preguntamos si la cedula consta de 10 digitos
    if (cedula.length == 10) {

        //Obtenemos el digito de la region que sonlos dos primeros digitos
        var digito_region = cedula.substring(0, 2);

        //Pregunto si la region existe ecuador se divide en 24 regiones
        if (digito_region >= 1 && digito_region <= 24) {

            // Extraigo el ultimo digito
            var ultimo_digito = cedula.substring(9, 10);

            //Agrupo todos los pares y los sumo
            var pares = parseInt(cedula.substring(1, 2)) + parseInt(cedula.substring(3, 4)) + parseInt(cedula.substring(5, 6)) + parseInt(cedula.substring(7, 8));

            //Agrupo los impares, los multiplico por un factor de 2, si la resultante es > que 9 le restamos el 9 a la resultante
            var numero1 = cedula.substring(0, 1);
            var numero1 = (numero1 * 2);
            if (numero1 > 9) { var numero1 = (numero1 - 9); }

            var numero3 = cedula.substring(2, 3);
            var numero3 = (numero3 * 2);
            if (numero3 > 9) { var numero3 = (numero3 - 9); }

            var numero5 = cedula.substring(4, 5);
            var numero5 = (numero5 * 2);
            if (numero5 > 9) { var numero5 = (numero5 - 9); }

            var numero7 = cedula.substring(6, 7);
            var numero7 = (numero7 * 2);
            if (numero7 > 9) { var numero7 = (numero7 - 9); }

            var numero9 = cedula.substring(8, 9);
            var numero9 = (numero9 * 2);
            if (numero9 > 9) { var numero9 = (numero9 - 9); }

            var impares = numero1 + numero3 + numero5 + numero7 + numero9;

            //Suma total
            var suma_total = (pares + impares);

            //extraemos el primero digito
            var primer_digito_suma = String(suma_total).substring(0, 1);

            //Obtenemos la decena inmediata
            var decena = (parseInt(primer_digito_suma) + 1) * 10;

            //Obtenemos la resta de la decena inmediata - la suma_total esto nos da el digito validador
            var digito_validador = decena - suma_total;

            //Si el digito validador es = a 10 toma el valor de 0
            if (digito_validador == 10)
                var digito_validador = 0;

            //Validamos que el digito validador sea igual al de la cedula
            if (digito_validador == ultimo_digito) {
                isValidate = true
            } else {
                isValidate = false
                new swal({
                    title: "Error Cedula",
                    text: "La cedula: " + cedula + " es incorrecta",
                    icon: "error",
                    button: "Cerrar"
                });
            }

        } else {
            // imprimimos en consola si la region no pertenece
            isValidate = false
            new swal({
                title: "Error Cedula",
                text: "Esta cedula no pertenece a ninguna region",
                icon: "error",
                button: "Cerrar"
            });
        }
    } else {
        //imprimimos en consola si la cedula tiene mas o menos de 10 digitos
        isValidate = false
        new swal({
            title: "Error Cedula",
            text: "Esta cedula tiene menos de 10 Digitos",
            icon: "error",
            button: "Cerrar"
        });

    }

    return isValidate;
}

const submitControl = (event) => {
    if (event.target[0].value.length == 0) {
        swal({
            title: "Cedula vacia",
            text: "Porfavor ingrese su cedula",
            icon: "error",
            button: "Cerrar"
        });
        event.preventDefault();
        return;
    }
    if (event.target[1].value.length == 0) {
        swal({
            title: "Sin contraseña",
            text: "Porfavor ingrese su contraseña",
            icon: "error",
            button: "Cerrar"
        });
        event.preventDefault();
        return;
    } 
    if (isValidate == undefined) {

        validatCI(event.target[0].value)
    }
    if (!isValidate) {
        event.preventDefault();
        return;
    }
}

if (input) {

    input.addEventListener('input', valtextNumber);
    input.addEventListener('change', (event) => {
        const cedula = event.target.value;
        validatCI(cedula)
    });

    if (form) { form[0].addEventListener('submit', submitControl); }
}

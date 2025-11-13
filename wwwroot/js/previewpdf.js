boton_prev = document.querySelectorAll('#boton-vista')
prev = document.createElement('embed')
console.log(prev)
boton_prev.forEach((btn) => {
    btn.addEventListener('click', () => {
        cuerpo = document.querySelector('#modal-body')
        prev.className = 'preview-pdf'
        const url = new URL(prev.baseURI);
        prev.src = url.origin + '/' + btn.value
        const d = btn.value.split('\\')
        document.getElementById('tituloModalLabel').innerHTML = d[d.length - 1]
        cuerpo.append(prev)
    })
})
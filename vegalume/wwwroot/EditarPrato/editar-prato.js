document.querySelectorAll('.numerico').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

document.getElementById('frm-editar-prato').addEventListener('submit', function (e){
    alert("Prato atualizado com sucesso!");
})
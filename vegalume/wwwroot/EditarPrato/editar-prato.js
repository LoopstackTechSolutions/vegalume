document.querySelectorAll('.numerico').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

document.getElementById('frm-cadastro').addEventListener('submit', function (e){
    const senha = document.getElementById('txtSenha').value;
    const telefone = document.getElementById('txtTelefone');

    const rawTelefone = telefone.value.replace(/\D/g, '');

    if(rawTelefone.length !== 11 && rawTelefone.length !== 10){
        alert('Telefone inválido!');
        e.preventDefault();
        return;
    }

    if(senha.length < 5){
        alert('Senha deve ser maior que 4 caracteres!')
        e.preventDefault();
        return;
    }

    telefone.value = rawTelefone;
})
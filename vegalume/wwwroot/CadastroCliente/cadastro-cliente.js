document.querySelectorAll('.numerico').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

const phoneInput = document.querySelector('.telefone');
phoneInput.addEventListener('input', function () {
    let digits = this.value.replace(/\D/g, '').slice(0, 11);
    let formatted = '';

    if (digits.length > 0) formatted += '(' + digits.slice(0, 2);
    if (digits.length >= 3) formatted += ')' + digits.slice(2, 7);
    if (digits.length >= 8) formatted += '-' + digits.slice(7);

    this.value = formatted;
});

document.getElementById('frm-cadastro').addEventListener('submit', async function (e) {
    e.preventDefault();

    const email = document.getElementById('txtEmail').value;
    let emailOk = true;

    try {
        const response = await fetch('/Cliente/TodosClientes');
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }

        const data = await response.json();

        data.forEach(cliente => {
            if (email === cliente.email) {
                emailOk = false;
            }
        });

        if (!emailOk) {
            alert('Email já está em uso!');
            document.getElementById('txtEmail').focus();
            return;
        }

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro ao verificar o email. Tente novamente.');
        return;
    }

    const senha = document.getElementById('txtSenha').value;
    const telefone = document.getElementById('txtTelefone');
    const rawTelefone = telefone.value.replace(/\D/g, '');

    if (rawTelefone.length !== 11 && rawTelefone.length !== 10) {
        alert('Telefone inválido!');
        document.getElementById('txtTelefone').focus();
        return;
    }

    if (senha.length < 5) {
        alert('Senha deve ser maior que 4 caracteres!');
        document.getElementById('txtSenha').focus();
        return;
    }

    telefone.value = rawTelefone;
    alert('Cadastrado com sucesso!');
    this.submit();
});
const btn = document.getElementById('scroll-top');
btn.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
});

fetch('/Cliente/ObterCliente')
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        return response.json();
    })
    .then(cliente => {
        document.getElementById('txtNome').value = cliente.nome;
        document.getElementById('txtSenha').value = cliente.senha;
        telefone = cliente.telefone;
        telefone = telefone.toString().replace(/^(\d{2})(\d{5})(\d{4})$/, '($1)$2-$3');
        document.getElementById('txtTelefone').value = telefone;
    })
    .catch(error => {
        console.error('Fetch error:', error);
    })

document.querySelectorAll('.numerico').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

const phoneInput = document.querySelector('.telefone');
phoneInput.addEventListener('input', function () {
    let digits = this.value.replace(/\D/g, '').slice(0, 11); // Keep only 11 digits
    let formatted = '';

    if (digits.length > 0) formatted += '(' + digits.slice(0, 2);
    if (digits.length >= 3) formatted += ')' + digits.slice(2, 7);
    if (digits.length >= 8) formatted += '-' + digits.slice(7);

    this.value = formatted;
});

const shortDateInput = document.querySelector('.data');
shortDateInput.addEventListener('input', function () {
    let digits = this.value.replace(/\D/g, '').slice(0, 4); // Max 4 digits
    let formatted = '';

    if (digits.length > 2) {
        formatted = digits.slice(0, 2) + '/' + digits.slice(2);
    } else {
        formatted = digits;
    }

    this.value = formatted;
});

const cardInput = document.querySelector('.cartao');
cardInput.addEventListener('input', function () {
    let digits = this.value.replace(/\D/g, '').slice(0, 16); // Max 16 digits
    let formatted = '';

    for (let i = 0; i < digits.length; i++) {
        if (i > 0 && i % 4 === 0) formatted += ' ';
        formatted += digits[i];
    }

    this.value = formatted;
});

document.getElementById('frm-adicionar-cartao').addEventListener('submit', function (e) {
    const cartao = document.getElementById('txtNCartao');
    const validade = document.getElementById('txtValidade');
    const cvv = document.getElementById('txtCVV').value;

    const rawCartao = cartao.value.replace(/\D/g, '');
    const rawValidade = validade.value.replace(/\D/g, '');

    if (rawCartao.length !== 16) {
        alert('N° de cartão inválido!');
        e.preventDefault();
        return;
    }

    const mes = parseInt(rawValidade.slice(0,2));
    const ano = parseInt(rawValidade.slice(-2));

    if (rawValidade.length !== 4 || mes > 12) {
        alert('Data de validade inválida!');
        e.preventDefault();
        return;
    }

    if(ano < (new Date().getFullYear() % 100)){
        alert('Cartão vencido!');
        e.preventDefault;
        return;
    }

    if (cvv.length !== 3) {
        alert('CVV inválido!');
        e.preventDefault();
        return;
    }

    card.value = rawCard;
    exp.value = rawExp;

});

document.getElementById('frm-dados-cadastrais').addEventListener('submit', function (e){
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
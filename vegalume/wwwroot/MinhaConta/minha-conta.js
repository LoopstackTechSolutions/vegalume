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

function Capitalizar(str) {
    return str
        .toLowerCase()
        .split(' ')
        .filter(word => word.trim().length > 0)
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}

fetch('/Cliente/TodosEnderecos')
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        return response.json();
    })
    .then(data => {
        const enderecos = document.getElementById("enderecos");

        if (data.length === 0) {
            document.getElementById("adicione-endereco").style.marginTop = 0;
        }
        else {
                data.forEach(endereco => {
                const linhaEndereco = document.createElement('div');
                linhaEndereco.classList.add("linha-endereco");
                enderecos.appendChild(linhaEndereco);

                const rua = Capitalizar(endereco.rua);
                const numero = endereco.numero;
                const bairro = Capitalizar(endereco.bairro);
                const cidade = Capitalizar(endereco.cidade);
                const estado = endereco.estado;

                const logradouro = document.createElement('div');
                logradouro.classList.add("logradouro");
                logradouro.textContent = rua + ", " + numero + " - " + bairro + ", " + cidade + " - " + estado;
                linhaEndereco.appendChild(logradouro);

                const a = document.createElement('a');
                a.href = ""; // TO-DO
                linhaEndereco.appendChild(a);

                const trashcan = document.createElement('img');
                trashcan.src = "../Imagens/icons8-trash-250.png"
                a.appendChild(trashcan);

            })
        }
    })
    .catch(error => {
        console.error('Fetch error:', error);
    })

fetch('/Cliente/TodosCartoes')
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        return response.json();
    })
    .then(data => {
        console.log("Total cartoes:", data.length);
        const cartoes = document.getElementById('cartoes');

        if (data.length === 0) {
            document.getElementById("adicione-cartao").style.marginTop = 0;
        }
        else {
            data.forEach(cartao => {
                const linhaCartao = document.createElement('div');
                linhaCartao.classList.add('linha-cartao');
                cartoes.appendChild(linhaCartao);

                const bandeira = Capitalizar(cartao.bandeira);
                const modalidade = cartao.modalidade;
                const nomeTitular = cartao.nomeTitular.toUpperCase();
                const numeroCartao = String(cartao.numeroCartao).slice(-4);

                const detalhesCartao = document.createElement('div');
                detalhesCartao.classList.add('detalhes-cartao');
                detalhesCartao.textContent = bandeira + " (" + modalidade + ") - " + nomeTitular + " - **** " + numeroCartao;
                linhaCartao.appendChild(detalhesCartao);

                const a = document.createElement('a');
                a.href = ""; // TO-DO
                linhaCartao.appendChild(a);

                const trashcan = document.createElement('img');
                trashcan.src = "../Imagens/icons8-trash-250.png"
                a.appendChild(trashcan);
            })
        }
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

document.getElementById('frm-adicionar-endereco').addEventListener('submit', function (e) {
    e.preventDefault();
    alert('Endereço cadastrado!');
    setTimeout(() => this.submit(), 0);
});

document.getElementById('frm-adicionar-cartao').addEventListener('submit', function (e) {
    e.preventDefault();

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

    if (rawValidade.length !== 4) {
        alert('Data de validade inválida!');
        e.preventDefault();
        return;
    }

    const mes = parseInt(rawValidade.slice(0, 2));
    const ano = parseInt(rawValidade.slice(2));

    if (mes < 1 || mes > 12) {
        alert('Mês inválido!');
        e.preventDefault();
        return;
    }

    const currentYear = new Date().getFullYear() % 100;
    if (ano < currentYear) {
        alert('Cartão vencido!');
        e.preventDefault();
        return;
    }

    if (cvv.length !== 3) {
        alert('CVV inválido!');
        e.preventDefault();
        return;
    }

    const fullYear = 2000 + ano;
    const monthPadded = mes.toString().padStart(2, '0');
    const formattedDate = `${fullYear}-${monthPadded}-01`;

    cartao.value = rawCartao;
    validade.value = formattedDate;

    alert('Cartão cadastrado!');
    setTimeout(() => this.submit(), 0);
});

document.getElementById('frm-dados-cadastrais').addEventListener('submit', function (e){
    e.preventDefault();

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

    alert('Dados alterados!');
    setTimeout(() => this.submit(), 0);
})
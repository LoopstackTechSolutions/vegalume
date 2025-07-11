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
                linhaEndereco.setAttribute('data-idendereco', endereco.idEndereco);
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

                const a = document.createElement('a');;
                const idendereco = linhaEndereco.dataset.idendereco;
                a.href = "#";
                a.classList.add("excluir-endereco");
                a.setAttribute('data-id', idendereco);
                linhaEndereco.appendChild(a);

                a.addEventListener('click', function (e) {
                    e.preventDefault();

                    const id = this.dataset.id;

                    const confirmed = confirm("Tem certeza que deseja excluir este endereço?");
                    if (!confirmed) return;

                    fetch('/Cliente/ExcluirEndereco', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        },
                        body: `idEndereco=${encodeURIComponent(id)}`
                    })
                        .then(response => {
                            if (response.ok) {
                                alert('Endereço excluído com sucesso!');
                                window.location.reload();
                                window.location.href = '/Home/MinhaConta#salvar';
                            } else {
                                throw new Error('Erro ao excluir endereço.');
                            }
                        })
                        .catch(error => {
                            console.error('Erro:', error);
                        });
                });

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
        const cartoes = document.getElementById('cartoes');

        if (data.length === 0) {
            document.getElementById("adicione-cartao").style.marginTop = 0;
        } else {
            data.forEach(cartao => {
                const linhaCartao = document.createElement('div');
                linhaCartao.classList.add('linha-cartao');
                linhaCartao.setAttribute('data-idcartao', cartao.idCartao);
                cartoes.appendChild(linhaCartao);

                const bandeira = Capitalizar(cartao.bandeira);
                const modalidade = cartao.modalidade;
                const nomeTitular = cartao.nomeTitular.toUpperCase();
                const numeroCartao = String(cartao.numeroCartao).slice(-4);

                const detalhesCartao = document.createElement('div');
                detalhesCartao.classList.add('detalhes-cartao');
                detalhesCartao.textContent = `${bandeira} (${modalidade}) - ${nomeTitular} - **** ${numeroCartao}`;
                linhaCartao.appendChild(detalhesCartao);

                const a = document.createElement('a');
                a.href = "#";
                a.classList.add("excluir-cartao");
                a.setAttribute('data-id', cartao.idCartao);
                linhaCartao.appendChild(a);

                a.addEventListener('click', function (e) {
                    e.preventDefault();

                    console.log("Link clicked:", e.currentTarget);
                    console.log("data-id attribute:", e.currentTarget.getAttribute("data-id"));
                    console.log("dataset.id:", e.currentTarget.dataset.id);

                    const id = e.currentTarget.dataset.id;

                    const confirmed = confirm("Tem certeza que deseja excluir este cartão?");
                    if (!confirmed) return;

                    fetch('/Cliente/ExcluirCartao', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        },
                        body: `idCartao=${encodeURIComponent(id)}`
                    })
                        .then(response => {
                            if (response.ok) {
                                alert('Cartão excluído com sucesso!');
                                window.location.reload();
                                window.location.href = '/Home/MinhaConta#adicionar-cartao'; // no reload needed before
                            } else {
                                throw new Error('Erro ao excluir cartão.');
                            }
                        })
                        .catch(error => {
                            console.error('Erro:', error);
                        });
                });

                const trashcan = document.createElement('img');
                trashcan.src = "../Imagens/icons8-trash-250.png";
                a.appendChild(trashcan);
            });
        }
    })
    .catch(error => {
        console.error('Fetch error:', error);
    });

(async () => {
    try {
        const idCliente = document.getElementById("meus-pedidos").dataset.id;

        const response = await fetch(`/Pedido/TodosPedidosPorCliente?idCliente=${encodeURIComponent(idCliente)}`);
        if (!response.ok) throw new Error('Erro de conexão ao obter pedidos.');

        const pedidos = await response.json();

        const table = document.querySelector("#table-pedidos");
        const primeiro = document.querySelector('#primeiro-pedido');

        if (pedidos.length === 0) {
            table.style.display = "none";
            primeiro.style.display = "block";
        }
        else {
            table.style.display = "table";
            primeiro.style.display = "none";

            const tbody = document.querySelector("#table-pedidos tbody");
            tbody.innerHTML = "";

            for (const pedido of pedidos) {
                const pratosResponse = await fetch(`/Prato/TodosPratosPorPedido?idPedido=${encodeURIComponent(pedido.idPedido)}`);
                if (!pratosResponse.ok) throw new Error('Erro de conexão ao obter pratos.');

                const pratosData = await pratosResponse.json();

                let pratos = "";
                for (const prato of pratosData) {
                    pratos += `${prato.nomePrato} (${prato.qtd})<br>`;
                }

                const date = new Date(pedido.dataHoraPedido);
                const day = String(date.getDate()).padStart(2, '0');
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const year = String(date.getFullYear()).slice(-2);
                const dataFormatada = `${day}/${month}/${year}`;

                tbody.insertAdjacentHTML("afterbegin", `
        <tr class="linha-pedido">
          <td>${pedido.idPedido}</td>
          <td>${dataFormatada}</td>
          <td>${pratos}</td>
          <td>$${pedido.valorTotal},00</td>
          <td>${Capitalizar(pedido.statusPedido)}</td>
          <td class="td-img">
            <a href="/Pedido/AcompanharPedido?idPedido=${pedido.idPedido}" >
            <img src="/Imagens/icons8-eye-100.png"/>
            </a>
          </td>
        </tr>`);
            }
        }
    } catch (error) {
        console.error(error);
    }
})();



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

const shortDateInput = document.querySelector('.data');
shortDateInput.addEventListener('input', function () {
    let digits = this.value.replace(/\D/g, '').slice(0, 4);
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
    let digits = this.value.replace(/\D/g, '').slice(0, 16);
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

document.getElementById('frm-dados-cadastrais').addEventListener('submit', function (e) {
    e.preventDefault();

    const senha = document.getElementById('txtSenha').value;
    const telefone = document.getElementById('txtTelefone');

    const rawTelefone = telefone.value.replace(/\D/g, '');

    if (rawTelefone.length !== 11 && rawTelefone.length !== 10) {
        alert('Telefone inválido!');
        e.preventDefault();
        return;
    }

    if (senha.length < 5) {
        alert('Senha deve ser maior que 4 caracteres!')
        e.preventDefault();
        return;
    }

    telefone.value = rawTelefone;

    alert('Dados alterados!');
    setTimeout(() => this.submit(), 0);
})
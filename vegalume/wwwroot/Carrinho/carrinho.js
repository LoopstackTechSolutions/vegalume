async function diminuirQtd(prato, preco, id) {
    console.log(id);

    preco = parseFloat(preco);
    var qtd = parseInt(document.getElementById(`qtd-${prato}`).value);

    if (qtd > 1) {
        qtd--;

        try {
            const response = await fetch(`/Prato/DiminuirQuantidade?id=${encodeURIComponent(id)}`);
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
            return;
        }

        AtualizarCarrinho();
    }
    else {
        if (confirm('Remover prato do carrinho?')) {
            try {
                const response = await fetch(`/Prato/RemoverDoCarrinho?id=${encodeURIComponent(id)}`);
                if (!response.ok) {
                    throw new Error('Erro de conexão.');
                }
            }
            catch (error) {
                console.error('Fetch error:', error);
                alert('Erro. Tente novamente.');
                return;
            }

            const div = document.getElementById(`div-${id}`);
            div.style.display = "none";
        }
    }

}

async function aumentarQtd(prato, preco, id) {
    preco = parseFloat(preco);
    var qtd = parseInt(document.getElementById(`qtd-${prato}`).value);

    if (qtd < 10) {
        qtd++;

        try {
            const response = await fetch(`/Prato/AumentarQuantidade?id=${encodeURIComponent(id)}`);
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
            return;
        }

        AtualizarCarrinho();
    }
}

async function AtualizarCarrinho() {
    try {
        const response = await fetch("/Prato/ObterCarrinho");
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        const data = await response.json();

        const pratos = document.getElementById("pratos");
        pratos.innerHTML = "";
        var totalCarrinho = 0;

        for (const pratoCarrinho of data) {
            try {
                const pratoResponse = await fetch(`/Prato/ObterPratoPeloId?idPrato=${encodeURIComponent(pratoCarrinho.id)}`);
                if (!pratoResponse.ok) {
                    throw new Error('Erro de conexão.');
                }

                const pratoData = await pratoResponse.json();

                const anotacoes = pratoCarrinho.anotacoes == null ? "" : pratoCarrinho.anotacoes;

                pratos.innerHTML += `<div id="div-${pratoCarrinho.id}">
                <div id="nome-prato">${pratoData.nomePrato}</div>
                <div id="anotacoes-prato">${anotacoes}</div>
                <div id="qtd-total">
                    <div>
                        <button type="button" onclick="diminuirQtd('${pratoData.nomePrato}','${pratoData.precoPrato}', '${pratoCarrinho.id}')">-</button>
                        <input type="text" class="qtd" id="qtd-${pratoData.nomePrato}" value="${pratoCarrinho.qtd}"/>
                        <button type="button" onclick="aumentarQtd('${pratoData.nomePrato}','${pratoData.precoPrato}', '${pratoCarrinho.id}')">+</button>
                    </div>
                    <div id="total-${pratoData.nomePrato}">$ ${(pratoCarrinho.qtd * pratoData.precoPrato).toFixed(0)},00</div>
                </div>
                </div>`;

                totalCarrinho += pratoCarrinho.qtd * pratoData.precoPrato;

            } catch (error) {
                console.error('Fetch error:', error);
                alert('Erro. Tente novamente.');
                return;
            }
        }

        totalCarrinho += 7;
        document.getElementById("total-carrinho").textContent = `$ ${totalCarrinho.toFixed(0)},00`;

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }
};

AtualizarCarrinho();

const id = document.getElementById("main").dataset.id;

(async function () {

    try {
        const response = await fetch(`/Cliente/TodosCartoes`);
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        const data = await response.json();

        const pagamentos = document.getElementById('selPagamento');

        for (const cartao of data) {
            pagamentos.innerHTML += `<option value="${cartao.idCartao}">(${cartao.bandeira}) ${cartao.modalidade} - 
                **** ${String(cartao.numeroCartao).slice(-4)}</option>`;
        }

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }

    try {
        const response = await fetch(`/Cliente/TodosEnderecos`);
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        const data = await response.json();

        const enderecos = document.getElementById('selEndereco');

        for (const endereco of data) {
            enderecos.innerHTML += `<option value="${endereco.idEndereco}">${endereco.rua}, ${endereco.numero} - ${endereco.bairro}, 
                ${endereco.cidade} - ${endereco.estado}</option>`;
        }

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }
})();

const limparCarrinho = document.getElementById("limpar-carrinho");
limparCarrinho.addEventListener('click', async function (e) {
    e.preventDefault();

    if (confirm("Limpar Carrinho?")) {
        try {
            const response = await fetch('/Cliente/LimparCarrinho');

            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }

            const data = await response.json();

            if (data.success) {
                alert("Carrinho limpo!");
                window.location.href = "/Home/Index#nosso-cardapio";
            } else {
                alert("Falha ao limpar o carrinho.");
            }

        } catch (error) {
            console.error('Fetch error:', error);
            alert("Erro ao limpar carrinho. Tente novamente.");
        }
    }
});

const form = document.getElementById("main");
form.addEventListener('submit', async function (e) {
    e.preventDefault();
    console.log("Form submitted");

    const valorTotalDiv = document.getElementById("total-carrinho");
    let text = valorTotalDiv.textContent || valorTotalDiv.innerText;
    text = text.replace(/[^\d,.-]/g, '');
    text = text.replace(',', '.');
    const valorTotal = parseFloat(text);

    const idEndereco = document.getElementById("selEndereco").value;

    const pagamento = document.getElementById("selPagamento").value;

    console.log(pagamento);

    const formaPagamento = pagamento === "pix" ? "pix"
        : (pagamento === "dinheiro" ? "dinheiro"
            : "cartao");

    console.log(formaPagamento);

    const idCartao = (pagamento === "pix" ||
        pagamento === "dinheiro") ?
        null : pagamento;

    try {
        const response = await fetch('/Pedido/FazerPedido',
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `valorTotal=${encodeURIComponent(valorTotal)}&idEndereco=${encodeURIComponent(idEndereco)}&formaPagamento=${encodeURIComponent(formaPagamento)}&idCartao=${encodeURIComponent(idCartao)}`
            });
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }

        const data = await response.json();

        alert("Pedido realizado com sucesso!");
        window.location.href = `/Pedido/AcompanharPedido?idPedido=${data.idPedido}`;

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }
})
async function diminuirQtd(prato, preco, id) {
    console.log(id);

    preco = parseFloat(preco);
    var qtd = parseInt(document.getElementById(`qtd-${prato}`).value);

    if (qtd > 1) {
        qtd--;
        document.getElementById(`qtd-${prato}`).value = qtd;
        document.getElementById(`total-${prato}`).textContent = "$ " + (qtd * preco).toFixed(0) + ",00";
    }
    else {
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

function aumentarQtd(prato, preco) {
    preco = parseFloat(preco);
    var qtd = parseInt(document.getElementById(`qtd-${prato}`).value);

    if (qtd < 10)
        qtd++;

    document.getElementById(`qtd-${prato}`).value = qtd;
    document.getElementById(`total-${prato}`).textContent = "$ " + (qtd * preco).toFixed(0) + ",00";
}

(async function () {
    try {
        const response = await fetch("/Prato/ObterCarrinho");
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        const data = await response.json();

        const pratos = document.getElementById("pratos");

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
            } catch (error) {
                console.error('Fetch error:', error);
                alert('Erro. Tente novamente.');
                return;
            }
        }
    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }
})();

const id = document.getElementById("main").dataset.id;

(async function () {
    try {
        const response = await fetch(`/Cliente/ObterClientePeloId?IdCliente=${encodeURIComponent(id)}`);
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        const data = await response.json();

        const idCliente = data.idcliente;

        try {
            const response = await fetch(`/Cliente/TodosCartoes`);
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
            const data = await response.json();

            const pagamentos = document.getElementById('selPagamento');

            for (const cartao of data) {
                pagamentos.innerHTML += `<option>(${cartao.bandeira}) ${cartao.modalidade} - 
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
                enderecos.innerHTML += `<option>${endereco.rua}, ${endereco.numero} - ${endereco.bairro}, 
                ${endereco.cidade} - ${endereco.estado}</option>`;
            }

        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
        }

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
    }
})();

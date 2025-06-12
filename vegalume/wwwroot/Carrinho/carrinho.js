function diminuirQtd(prato, preco) {
    preco = parseFloat(preco); 
    var qtd = parseInt(document.getElementById(`qtd-${prato}`).value);

    if (qtd > 1)
        qtd--;

    document.getElementById(`qtd-${prato}`).value = qtd;
    document.getElementById(`total-${prato}`).textContent = "$ " + (qtd * preco).toFixed(0) + ",00";
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
                console.log(pratoData);

                const anotacoes = pratoCarrinho.anotacoes == null ? "" : pratoCarrinho.anotacoes;

                pratos.innerHTML += `<div>
                <div id="nome-prato">${pratoData.nomePrato}</div>
                <div id="anotacoes-prato">${anotacoes}</div>
                <div id="qtd-total">
                    <div>
                        <button type="button" onclick="diminuirQtd('${pratoData.nomePrato}','${pratoData.precoPrato}')">-</button>
                        <input type="text" class="qtd" id="qtd-${pratoData.nomePrato}" value="${pratoCarrinho.qtd}" />
                        <button type="button" onclick="aumentarQtd('${pratoData.nomePrato}','${pratoData.precoPrato}')">+</button>
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

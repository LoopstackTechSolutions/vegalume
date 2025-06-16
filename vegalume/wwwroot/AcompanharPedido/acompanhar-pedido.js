const idPedido = document.getElementById("main").dataset.id;

let intervaloProgresso;
let intervaloFetch;

function atualizarStatus(status) {
    if (intervaloProgresso) {
        clearInterval(intervaloProgresso);
    }

    let progresso = 0;

    function mostrarProgresso() {
        document.getElementById("icone").style.display = "none";
        document.getElementById("barras").style.display = "flex";
        document.getElementById("hora-entrega").style.display = "flex";
    }

    function esconderProgresso() {
        document.getElementById("icone").style.display = "block";
        document.getElementById("barras").style.display = "none";
        document.getElementById("hora-entrega").style.display = "none";
    }

    const cancelar = document.getElementById("cancelar");
    cancelar.style.display = "none";

    const fraseStatus = document.getElementById("frase-status");
    const icone = document.getElementById("icone");

    switch (status) {
        case "espera":
            comecarProgresso(`progresso-espera`);
            mostrarProgresso();
            cancelar.style.display = "block";
            fraseStatus.textContent = "Aguardando confirmação do restaurante";
            break;

        case "preparacao":
            document.getElementById("progresso-espera").style.width = '100%';
            comecarProgresso(`progresso-preparacao`);
            mostrarProgresso();
            fraseStatus.textContent = "Seu pedido está sendo preparado";
            break;

        case "transito":
            document.getElementById("progresso-espera").style.width = '100%';
            document.getElementById("progresso-preparacao").style.width = '100%';
            comecarProgresso(`progresso-transito`);
            mostrarProgresso();
            fraseStatus.textContent = "Seu pedido está indo até você";
            break;

        case "entregue":
        case "cancelado":
            clearInterval(intervaloProgresso);
            clearInterval(intervaloFetch);
            esconderProgresso();
            icone.src = (status === "entregue")
                ? "/Imagens/icons8-yes-100-green.png"
                : "/Imagens/icons8-no-100-green.png";
            fraseStatus.textContent = (status === "entregue")
                ? "Seu pedido foi entregue"
                : "Seu pedido foi cancelado";
            break;

        default:
            console.warn(`Status '${status}' não reconhecido.`);
            break;
    }

    function comecarProgresso(id) {
        const barraProgresso = document.getElementById(id);
        intervaloProgresso = setInterval(() => {
            progresso = (progresso + 1) % 101;
            barraProgresso.style.width = progresso + '%';
        }, 25);
    }
}

function Capitalizar(str) {
    return str
        .toLowerCase()
        .split(' ')
        .filter(word => word.trim().length > 0)
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}

function adicionarMinutos(dataString, minutos) {
    const data = new Date(dataString);
    const novaData = new Date(data.getTime() + minutos * 60000);
    const horas = novaData.getHours().toString().padStart(2, '0');
    const minutosFormatados = novaData.getMinutes().toString().padStart(2, '0');
    return `${horas}:${minutosFormatados}`;
}

async function atualizarPedido() {
    try {
        const response = await fetch(`/Pedido/ObterPedidoPeloId?idPedido=${encodeURIComponent(idPedido)}`);
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }

        const pedido = await response.json();

        atualizarStatus(pedido.statusPedido);

        document.getElementById("total-div").textContent = `$ ${pedido.valorTotal},00`;

        document.getElementById("hora-entrega-div").textContent = `${adicionarMinutos(pedido.dataHoraPedido, 40)} - 
                                                                   ${adicionarMinutos(pedido.dataHoraPedido, 80)}`;

        document.getElementById("pratos").innerHTML = "";
        try {
            const response = await fetch(`/Prato/TodosPratosPorPedido?idPedido=${encodeURIComponent(idPedido)}`);
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }

            const pratos = await response.json();

            for (var prato of pratos) {
                const nomePrato = prato.nomePrato;
                const qtd = prato.qtd;
                const precoPrato = parseFloat(prato.precoUnitario);

                document.getElementById("pratos").innerHTML += `
                <div class="prato">
                    <div class="nome">${nomePrato} (${qtd})</div>
                    <div class="total">$ ${precoPrato.toFixed(0)},00</div>
                </div>`;
            }

        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
            clearInterval(intervaloFetch);
        }

        try {
            const response = await fetch(`/Pedido/ObterEnderecoPeloId?idEndereco=${encodeURIComponent(pedido.idEndereco)}`);
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }

            const endereco = await response.json();

            document.getElementById("endereco-div").textContent = `${endereco.rua}, ${endereco.numero} - 
            ${endereco.bairro}, ${endereco.cidade} - ${endereco.estado}`;

        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
            clearInterval(intervaloFetch);
        }

        if (pedido.formaPagamento === "pix" || pedido.formaPagamento === "dinheiro")
            document.getElementById("pagamento-div").textContent = Capitalizar(pedido.formaPagamento);
        else {

            try {
                const response = await fetch(`/Pedido/ObterCartaoPeloId?idCartao=${encodeURIComponent(pedido.idCartao)}`);
                if (!response.ok) {
                    throw new Error('Erro de conexão.');
                }

                const cartao = await response.json();

                document.getElementById("pagamento-div").textContent = `${Capitalizar(cartao.bandeira)}
                                                                    (${Capitalizar(cartao.modalidade)}) - 
                                                                    ${cartao.nomeTitular.toUpperCase()} - 
                                                                    **** ${String(cartao.numeroCartao).slice(-4)}`;

            } catch (error) {
                console.error('Fetch error:', error);
                alert('Erro. Tente novamente.');
                clearInterval(intervaloFetch);
            }
        }

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
        clearInterval(intervaloFetch);
    }
}

atualizarPedido();
intervaloFetch = setInterval(atualizarPedido, 60000);


document.getElementById("cancelar").addEventListener('click', async function (e) {
    if (confirm("Cancelar pedido?")) {
        try {
            const params = new URLSearchParams();
            params.append("idPedido", idPedido);
            params.append("rm", "");

            const response = await fetch(`/Pedido/CancelarPedido`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: params.toString()
            });

            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }

            alert("Pedido Cancelado.");
            window.location.href = "/Home/Index";

        } catch (error) {
            console.error('Fetch error:', error);
            alert('Erro. Tente novamente.');
            clearInterval(intervaloFetch);
        }
    }
});

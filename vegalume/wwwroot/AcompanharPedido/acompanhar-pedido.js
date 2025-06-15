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

    const fraseStatus = document.getElementById("frase-status");

    switch (status) {
        case "espera":
            comecarProgresso(`progresso-espera`);
            mostrarProgresso();
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

async function atualizarPedido() {
    console.log("new fetch");
    try {
        const response = await fetch(`/Pedido/ObterPedidoPeloId?idPedido=${idPedido}`);
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }

        const pedido = await response.json();
        console.log(pedido.statusPedido);

        atualizarStatus(pedido.statusPedido);

    } catch (error) {
        console.error('Fetch error:', error);
        alert('Erro. Tente novamente.');
        clearInterval(intervaloFetch);
    }
}

atualizarPedido();
intervaloFetch = setInterval(atualizarPedido, 60000);
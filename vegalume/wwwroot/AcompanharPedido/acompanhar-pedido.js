let interval;

function animarProgresso(status) {
    const barraProgresso = document.getElementById(`progresso-${status}`);
    let progresso = 0;
    interval = setInterval(function () {

        progresso += 1;
        if (progresso > 100) {
            progresso = 0;
        }
        barraProgresso.style.width = progresso + '%';
    }, 25)
}

animarProgresso("espera");
const elogios = [
    "Ótima escolha!",
    "Delicioso, como sempre!",
    "Escolha de mestre!",
    "De dar água na boca!",
    "Carinho em forma de prato!",
    "Vai te surpreender!"
];

const elogio = elogios[Math.floor(Math.random() * elogios.length)];
document.getElementById("titulo").textContent = elogio;

function diminuirQtd() {
    var qtd = parseInt(document.getElementById("qtd").value);

    if (qtd > 1)
        qtd--;

    document.getElementById("qtd").value = qtd;
}

function aumentarQtd() {
    var qtd = parseInt(document.getElementById("qtd").value);

    if (qtd < 10)
        qtd++;

    document.getElementById("qtd").value = qtd;
}

document.querySelectorAll(".qtd-button").forEach(button => {
    button.addEventListener("click", () => {
        const qtd = parseInt(document.getElementById("qtd").value);
        const total = parseInt(document.getElementById("total").dataset.precoBase) * qtd;
        document.getElementById("total").textContent = "R$ " + total + ",00";
    });
});
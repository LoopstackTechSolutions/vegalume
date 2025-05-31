function revelaEsconde(conteudo) {
    if (conteudo === "localizacao") {
        document.getElementById("p-localizacao").classList.toggle("mostrar");
        document.getElementById("btn-localizacao").classList.toggle("menos");
    }
    if (conteudo === "horarios") {
        document.getElementById("p-horarios").classList.toggle("mostrar");
        document.getElementById("btn-horarios").classList.toggle("menos");
    }
    if (conteudo === "contatos") {
        document.getElementById("p-contatos").classList.toggle("mostrar");
        document.getElementById("btn-contatos").classList.toggle("menos");
    }
}

const btn = document.getElementById('scroll-top');
btn.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
});

fetch('/Prato/TodosPratos')
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        return response.json();
    })
    .then(data => {
        const cardapio = document.getElementById("cardapio");

        data.forEach(prato => {
            const div = document.createElement('div');
            div.classList.add("prato");
            cardapio.appendChild(div);

            const aside = document.createElement('aside');
            div.appendChild(aside);

            const nomePrato = document.createElement('p');
            nomePrato.textContent = prato.nomePrato;
            nomePrato.classList.add("nome-prato");
            aside.appendChild(nomePrato);

            const descricaoPrato = document.createElement("p");
            descricaoPrato.textContent = prato.descricaoPrato;
            descricaoPrato.classList.add("descricao-prato");
            aside.appendChild(descricaoPrato);

            const preco = document.createElement("p");
            preco.textContent = "$" + prato.precoPrato.toFixed(0);
            preco.classList.add("preco");
            div.appendChild(preco);

        })
    })
    .catch(error => {
        console.error('Fetch error:', error);
    })
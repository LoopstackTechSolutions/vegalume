fetch('/Pedido/TodosPedidosPorStatus?status=espera')
    .then(response => {
        if (!response.ok) {
            throw new Error('Erro de conexão.');
        }
        return response.json();
    })
    .then(data => {
        const tableEspera = document.getElementById('table-espera');

        data.forEach(pedido => {
            const tr = document.createElement('tr');
            tableEspera.appendChild(tr);

            const numero = document.createElement('td');
            numero.classList.add('numero');
            numero.textContent = "N° " + pedido.idPedido;
            tr.appendChild(numero);

            const pratos = document.createElement('td');
            pratos.classList.add('pratos');
            fetch(`/Prato/TodosPratosPorPedido?idPedido=${pedido.idPedido}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Erro ao buscar pratos');
                    }
                    return response.json();
                })
                .then(data => {
                    data.forEach(prato => {
                        let anotacoes = prato.anotacoes === "" ? "" : ("*" + prato.anotacoes + "*");
                        pratos.innerHTML += `${prato.nomePrato} (${prato.qtd}) <strong>${anotacoes}</strong><br>`;
                    });
                })
                .catch(error => {
                    console.error('Erro:', error);
                });
            tr.appendChild(pratos);

            const cliente = document.createElement('td');
            cliente.classList.add('cliente');
            fetch(`/Cliente/ObterClientePeloId?idCliente=${pedido.idCliente}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Erro ao buscar cliente');
                    }
                    return response.json();
                })
                .then(data => {
                    cliente.textContent = data.nome;
                })
                .catch(error => {
                    console.error('Erro:', error);
                });
            tr.appendChild(cliente);

            const img = document.createElement('td');
            img.classList.add('img-td');
            tr.appendChild(img)

            const yes = document.createElement('img');
            yes.src = "../Imagens/icons8-yes-100.png";
            img.appendChild(yes);

            const no = document.createElement('img');
            no.src = "../Imagens/icons8-no-100.png";
            img.appendChild(no);
        });
    })
    .catch (error => {
    console.error('Fetch error:', error);
});
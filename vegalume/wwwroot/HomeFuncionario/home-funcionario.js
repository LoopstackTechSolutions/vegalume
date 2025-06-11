function fetchPedidos(status) {
    fetch(`/Pedido/TodosPedidosPorStatus?status=${status}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
            return response.json();
        })
        .then(data => {
            if (data.length === 0)
                document.getElementById("vazio-" + status).style.display = "flex";
            else {
                document.getElementById("vazio-" + status).style.display = "none";
                const table = document.getElementById('table-' + status);
                table.innerHTML = '';

                data.forEach(pedido => {
                    const tr = document.createElement('tr');
                    table.appendChild(tr);

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

                    const aYes = document.createElement('a');
                    aYes.dataset.idPedido = pedido.idPedido;
                    img.appendChild(aYes)
                    const yes = document.createElement('img');
                    yes.src = "../Imagens/icons8-yes-100.png";
                    aYes.appendChild(yes);

                    aYes.addEventListener('click', function (e) {
                        e.preventDefault();

                        const idPedido = this.dataset.idPedido;
                        const rm = document.getElementById('main').dataset.rm;

                        fetch('/Pedido/AvancarPedido', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            body: `idPedido=${encodeURIComponent(idPedido)}&statusAtual=${encodeURIComponent(status)}&rm=${encodeURIComponent(rm)}`
                        })
                            .then(response => {
                                if (response.ok) {
                                    window.location.reload();
                                } else {
                                    throw new Error('Erro.');
                                }
                            })
                            .catch(error => {
                                console.error('Erro:', error);
                            });
                    });

                    const aNo = document.createElement('a');
                    aNo.dataset.idPedido = pedido.idPedido;
                    img.appendChild(aNo)
                    const no = document.createElement('img');
                    no.src = "../Imagens/icons8-no-100.png";
                    aNo.appendChild(no);

                    aNo.addEventListener('click', function (e) {
                        e.preventDefault();

                        const idPedido = this.dataset.idPedido;
                        const rm = document.getElementById('main').dataset.rm;

                        const confirmed = confirm("Tem certeza que deseja cancelar este pedido?");
                        if (!confirmed) return;

                        fetch('/Pedido/CancelarPedido', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            body: `idPedido=${encodeURIComponent(idPedido)}&rm=${encodeURIComponent(rm)}`
                        })
                            .then(response => {
                                if (response.ok) {
                                    alert('Pedido cancelado.');
                                    window.location.reload();
                                } else {
                                    throw new Error('Erro ao cancelar pedido.');
                                }
                            })
                            .catch(error => {
                                console.error('Erro:', error);
                            });
                    });
                });
            }
        })

        .catch(error => {
            console.error('Fetch error:', error);
        });
}

function updatePedidos() {
    fetchPedidos("espera");
    fetchPedidos("preparacao");
    fetchPedidos("transito");
}

updatePedidos();

setInterval(updatePedidos, 30000);
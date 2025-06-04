const txtFiltro = document.getElementById("filtro");

function atualizarClientes() {
    const filtro = txtFiltro.value.trim();

    fetch(`/Cliente/FiltrarClientes?filtro=${encodeURIComponent(filtro)}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
            return response.json();
        })
        .then(data => {
            const table = document.getElementById('clientes');

            if (data.length === 0) {
                document.getElementById("vazio").style.display = "flex";
                table.style.display = "none";
                table.innerHTML = `<tr>
                                            <th class="id">Id</th>
                                            <th class="nome">Nome</th>
                                            <th class="email">Email</th>
                                            <th class="telefone">Telefone</th>
                                            <th class="pedidos">Pedidos</th>
                                       </tr>`;
            }
            else {
                document.getElementById("vazio").style.display = "none";
                table.style.display = "table";
                
                table.innerHTML = `<tr>
                                            <th class="id">Id</th>
                                            <th class="nome">Nome</th>
                                            <th class="email">Email</th>
                                            <th class="telefone">Telefone</th>
                                            <th class="pedidos">Pedidos</th>
                                       </tr>`;

                data.forEach(cliente => {
                    const tr = document.createElement('tr');
                    table.appendChild(tr);

                    const id = document.createElement('td');
                    id.textContent = cliente.idCliente;
                    id.classList.add("id");
                    tr.appendChild(id);

                    const nome = document.createElement('td');
                    nome.textContent = cliente.nome;
                    nome.classList.add("nome");
                    tr.appendChild(nome);

                    const email = document.createElement('td');
                    email.textContent = cliente.email;
                    email.classList.add("email");
                    tr.appendChild(email);

                    const telefone = document.createElement('td');
                    let telefoneFormatado = cliente.telefone.toString().replace(/^(\d{2})(\d{4,5})(\d{4})$/, '($1)$2-$3');
                    telefone.textContent = telefoneFormatado;
                    telefone.classList.add("telefone");
                    tr.appendChild(telefone);

                    const pedidos = document.createElement('td');
                    // TO-DO fetch
                    pedidos.classList.add("pedidos");
                    tr.appendChild(pedidos);
                })
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
        })
}

txtFiltro.addEventListener('input', function (e) {
    e.preventDefault();

    atualizarClientes();
});

atualizarClientes();                   
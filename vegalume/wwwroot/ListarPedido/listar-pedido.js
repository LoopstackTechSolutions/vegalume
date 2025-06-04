const txtFiltro = document.getElementById("txtFiltro");

async function atualizarPedidos() {
    const filtro = txtFiltro.value.trim();

    try {
        const response = await fetch(`/Pedido/FiltrarPedidos?filtro=${encodeURIComponent(filtro)}`);
        if (!response.ok) throw new Error('Erro de conexão.');

        const data = await response.json();
        const table = document.getElementById('pedidos');

        const headerHTML = `
            <tr>
                <th class="numero">N°</th>
                <th class="pratos">Pratos</th>
                <th class="valor-total">Valor Total</th>
                <th class="cliente">Cliente</th>
                <th class="funcionario">Funcionário</th>
                <th class="data">Data</th>
                <th class="status">Status</th>
            </tr>
        `;

        table.innerHTML = headerHTML;

        if (data.length === 0) {
            document.getElementById("nenhum").style.display = "flex";
            table.style.display = "none";
        } else {
            document.getElementById("nenhum").style.display = "none";
            table.style.display = "table";

            for (const pedido of data) {

                let cliente;
                try {
                    const clienteResponse = await fetch(`/Cliente/ObterClientePeloId?idCliente=${encodeURIComponent(pedido.idCliente)}`);
                    if (clienteResponse.ok) {
                        cliente = await clienteResponse.json();
                    }
                } catch (error) {
                    console.error('Erro ao buscar cliente:', error);
                }

                let funcionario;
                try {
                    const funcionarioResponse = await fetch(`/Funcionario/ObterFuncionarioPeloRm?rm=${encodeURIComponent(pedido.rm)}`);
                    if (funcionarioResponse.ok) {
                        funcionario = await funcionarioResponse.json();
                        funcionario = funcionario == null ? "" : funcionario.nome;
                    }
                } catch (error) {
                    console.error('Erro ao buscar funcionário:', error);
                }

                let pratos = "";
                try {
                    const pratosResponse = await fetch(`/Prato/TodosPratosPorPedido?idPedido=${encodeURIComponent(pedido.idPedido)}`);
                    if (pratosResponse.ok) {
                        let listaPratos = await pratosResponse.json();
                        for (const prato of listaPratos) {
                            pratos += `${prato.nomePrato} (${prato.qtd})<br>`;
                        }
                    }
                } catch (error) {
                    console.error('Erro ao buscar pratos:', error);
                }

                let dataFormatada = new Date(pedido.dataHoraPedido).toLocaleDateString('pt-BR');
                dataFormatada = dataFormatada.slice(0, -4) + dataFormatada.slice(-2);

                const status = pedido.statusPedido == "espera" ? "Espera" :
                    (pedido.statusPedido == "preparacao" ? "Preparação" :
                        (pedido.statusPedido == "transito" ? "Trânsito" :
                            (pedido.statusPedido == "entregue" ? "Entregue" : "Cancelado")));

                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td class="numero">${pedido.idPedido}</td>
                    <td class="pratos">${pratos}</td>
                    <td class="valor-total">$${pedido.valorTotal}</td>
                    <td class="cliente">${cliente.nome}</td>
                    <td class="funcionario">${funcionario}</td>
                    <td class="data">${dataFormatada}</td>
                    <td class="status">${status}</td>
                `;
                table.appendChild(tr);
            }
        }
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

txtFiltro.addEventListener('input', function (e) {
    e.preventDefault();
    atualizarPedidos();
});

atualizarPedidos();

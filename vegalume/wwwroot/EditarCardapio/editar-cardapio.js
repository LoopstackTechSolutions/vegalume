document.querySelectorAll('.numerico').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

document.querySelectorAll('.float').forEach(input => {
    input.addEventListener('input', function () {
        if (this.classList.contains('phone-mask')) return;

        this.value = this.value.replace(/\./g, ',');

        this.value = this.value.replace(/[^0-9,]/g, '');

        const parts = this.value.split(',');
        parts[0] = parts[0].replace(/,/g, '');

        if (parts.length > 1) {
            parts[1] = parts[1].substring(0, 2);
            this.value = parts[0] + ',' + parts[1];
        } else {
            this.value = parts[0];
        }
    });
});

function fetchPratos(status) {
    const statusBool = status == "ativos" ? 1 : 0;
    const icon = status == "ativos" ? "hide" : "eye";

    fetch(`/Prato/TodosPratosPorStatus?status=${statusBool}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro de conexão.');
            }
            return response.json();
        })
        .then(data => {
            const table = document.getElementById('table-' + status);
            table.innerHTML = `<tr>
                <th class="id">Id</th>
                <th class="nome">Nome</th>
                <th class="descricao">Descrição</th>
                <th class="preco">Preço</th>
            </tr>`;

            if (data.length === 0) {
                document.getElementById("vazio-" + status).style.display = "flex";
                table.style.display = "none";
            }
            else {
                document.getElementById("vazio-" + status).style.display = "none";
                table.style.display = "table";

                data.forEach(prato => {
                    table.innerHTML += `<tr>
                    <td class="id">${prato.idPrato}</td>
                    <td class="nome">${prato.nomePrato}</td>
                    <td class="descricao">${prato.descricaoPrato}</td>
                    <td class="preco">$${Number(prato.precoPrato).toFixed(0)}</td>
                    <td class="img-td">
                        <a class="a-edit" data-id="${prato.idPrato}"><img src="/Imagens/icons8-edit-100.png"></a>
                        <a class="a-switch" data-id="${prato.idPrato}"><img src="/Imagens/icons8-${icon}-100.png"></a>
                    </td>
                </tr>`
                })

                document.querySelectorAll(".a-switch").forEach(a => {
                    a.addEventListener('click', function (e) {
                        e.preventDefault();
                        const id = this.getAttribute('data-id');

                        fetch('/Prato/TrocarStatus', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            body: `idPrato=${encodeURIComponent(id)}`
                        })
                            .then(response => {
                                if (response.ok) {
                                    updatePratos();
                                } else {
                                    throw new Error('Erro.');
                                }
                            })
                            .catch(error => {
                                console.error('Erro:', error);
                            });
                    })
                })

                document.querySelectorAll(".a-edit").forEach(a => {
                    a.addEventListener('click', function (e) {
                        e.preventDefault();

                        const id = this.getAttribute('data-id');
                        window.location.href = `/Prato/EditarPrato?idPrato=${encodeURIComponent(id)}`;
                    });
                });
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
}

function updatePratos() {
    fetchPratos("ativos");
    fetchPratos("ocultos");
}

updatePratos();

const form = document.getElementById("frm-adicionar-prato");
form.addEventListener('submit', function (e) {
    alert("Prato cadastrado com sucesso! Foi adicionado aos Pratos Ocultos.");
});
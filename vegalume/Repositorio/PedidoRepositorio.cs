using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Runtime.ConstrainedExecution;
using vegalume.Models;


namespace vegalume.Repositorio
{
    public class PedidoRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL")!;

        public int FazerPedido(int? idCliente, decimal valorTotal, int idEndereco, string formaPagamento, int? idCartao)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            var cmd = new MySqlCommand(@"insert into tb_pedido (idEndereco, idCliente, formaPagamento, idCartao, valorTotal)
                                        values (@idEndereco, @idCliente, @formaPagamento, @idCartao, @valorTotal);
                                        select last_insert_id();", conexao);

            cmd.Parameters.AddWithValue("@idEndereco", idEndereco);
            cmd.Parameters.AddWithValue("@idCliente", (object?)idCliente ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@formaPagamento", formaPagamento);
            cmd.Parameters.AddWithValue("@idCartao", (object?)idCartao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@valorTotal", valorTotal);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AdicionarPratoAoPedido(int idPedido, int idPrato, int qtd, string anotacoes)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            var cmd = new MySqlCommand(@"insert into tb_prato_pedido (idPrato, idPedido, qtd, detalhesPedido)
                                        values (@idPrato, @idPedido, @qtd, @detalhesPedido)", conexao);

            cmd.Parameters.AddWithValue("@idPedido", idPedido);
            cmd.Parameters.AddWithValue("@idPrato", idPrato);
            cmd.Parameters.AddWithValue("@qtd", qtd);
            cmd.Parameters.AddWithValue("@detalhesPedido", anotacoes);

            cmd.ExecuteNonQuery();
        }

        public Pedido? ObterPedidoPeloId(int idPedido)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"select * from tb_pedido where idpedido = @idPedido;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idPedido", idPedido);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Pedido
                            {
                                idPedido = reader.GetInt32("idpedido"),
                                statusPedido = reader.GetString("statuspedido"),
                                dataHoraPedido = reader.GetDateTime("datahorapedido"),
                                valorTotal = reader.GetDecimal("valortotal"),
                                idCliente = reader.GetInt32("idcliente"),
                                idEndereco = reader.GetInt32("idendereco"),
                                formaPagamento = reader.GetString("formapagamento"),
                                idCartao = reader.IsDBNull(reader.GetOrdinal("idcartao"))
                                    ? (int?)null : reader.GetInt32("idcartao")
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public IEnumerable<Pedido> TodosPedidosPorStatus(string statusPedido)
        {
            List<Pedido> lista = new List<Pedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"select * from tb_pedido where statuspedido = @statusPedido;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@statusPedido", statusPedido);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pedido
                            {
                                idPedido = reader.GetInt32("idpedido"),
                                dataHoraPedido = reader.GetDateTime("datahorapedido"),
                                valorTotal = reader.GetDecimal("valortotal"),
                                idCliente = reader.GetInt32("idcliente"),
                                idEndereco = reader.GetInt32("idendereco"),
                                formaPagamento = reader.GetString("formaPagamento"),
                                idCartao = reader.IsDBNull(reader.GetOrdinal("idcartao"))
                                ? (int?)null : reader.GetInt32("idcartao")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public IEnumerable<Pedido> TodosPedidosPorCliente(int idCliente)
        {
            List<Pedido> lista = new List<Pedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"select * from tb_pedido where idcliente = @idCliente order by idpedido asc;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pedido
                            {
                                idPedido = reader.GetInt32("idpedido"),
                                dataHoraPedido = reader.GetDateTime("datahorapedido"),
                                statusPedido = reader.GetString("statuspedido"),
                                valorTotal = reader.GetDecimal("valortotal"),
                                idCliente = reader.GetInt32("idcliente"),
                                idEndereco = reader.GetInt32("idendereco"),
                                formaPagamento = reader.GetString("formaPagamento"),
                                idCartao = reader.IsDBNull(reader.GetOrdinal("idcartao"))
                                ? (int?)null : reader.GetInt32("idcartao")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public IEnumerable<Pedido> FiltrarPedidos(string filtro)
        {
            List<Pedido> lista = new List<Pedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                string query;

                if (string.IsNullOrEmpty(filtro))
                    query = @"SELECT distinct pe.* FROM tb_pedido pe ORDER BY pe.idpedido DESC;";
                else
                {
                    query = @"SELECT distinct pe.* FROM tb_pedido pe 
                                left join tb_cliente c on c.idcliente = pe.idcliente 
                                left join tb_funcionario f on f.rm = pe.rm 
                                left join tb_prato_pedido pp on pp.idpedido = pe.idpedido 
                                left join tb_prato pr on pr.idprato = pp.idprato 
                                WHERE COALESCE(c.nome, '') LIKE CONCAT('%', @filtro, '%') 
                                OR COALESCE(f.nome, '') LIKE CONCAT('%', @filtro, '%') 
                                OR COALESCE(pr.nomeprato, '') LIKE CONCAT('%', @filtro, '%') 
                                OR COALESCE(pe.statuspedido, '') LIKE CONCAT('%', @filtro, '%')
                                OR COALESCE(CAST(pe.idpedido AS CHAR), '') LIKE CONCAT('%', @filtro, '%') 
                                OR DATE_FORMAT(pe.datahorapedido, '%d/%m/%Y') = @filtro
                                order by pe.idpedido desc;";
                }

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@filtro", filtro);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pedido
                            {
                                idPedido = reader.GetInt32("idpedido"),
                                dataHoraPedido = reader.GetDateTime("datahorapedido"),
                                valorTotal = reader.GetDecimal("valortotal"),
                                idCliente = reader.GetInt32("idcliente"),
                                rm = reader.IsDBNull(reader.GetOrdinal("rm"))
                                ? (int?)null : reader.GetInt32("rm"),
                                idEndereco = reader.GetInt32("idendereco"),
                                statusPedido = reader.GetString("statuspedido"),
                                formaPagamento = reader.GetString("formaPagamento"),
                                idCartao = reader.IsDBNull(reader.GetOrdinal("idcartao"))
                                ? (int?)null : reader.GetInt32("idcartao")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void CancelarPedido(int idPedido, int rm)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tb_pedido set statuspedido='cancelado', rm=@rm " +
                                                    "where idpedido = @idPedido; ", conexao);

                cmd.Parameters.AddWithValue("@idPedido", idPedido);
                cmd.Parameters.AddWithValue("@rm", rm);

                cmd.ExecuteNonQuery();
            }
        }

        public void AvancarPedido(int idPedido, string statusAtual, int rm)
        {
            string proxStatus = statusAtual == "espera" ? "preparacao" :
                (statusAtual == "preparacao" ? "transito" : "entregue");
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tb_pedido set statuspedido=@proxStatus, rm=@rm " +
                                                    "where idpedido = @idPedido; ", conexao);

                cmd.Parameters.AddWithValue("@idPedido", idPedido);
                cmd.Parameters.AddWithValue("@proxStatus", proxStatus);
                cmd.Parameters.AddWithValue("@rm", rm);

                cmd.ExecuteNonQuery();
            }
        }
    }
}

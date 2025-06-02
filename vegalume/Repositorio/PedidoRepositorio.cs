using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Runtime.ConstrainedExecution;
using vegalume.Models;


namespace vegalume.Repositorio
{
    public class PedidoRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");

        /*public void Cadastrar(Pedido pedido)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_pedido (statusPagamento, rm, cep, idCliente) values (@statusPagamento, @rm, @cep, @idCliente)", conexao); 
                                                                                                                                                
                cmd.Parameters.Add("@statusPagamento", MySqlDbType.Bit).Value = pedido.statusPagamento;//VERIFICAR SE É BIT OU BOOL
                cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = pedido.rm; // ALTERAR O TIPO DE DADO NO BANCO DE DADOS,AQUI E NOS OUTROS REPOSITÓRIOS ??? testar
                cmd.Parameters.Add("@cep", MySqlDbType.Decimal).Value = pedido.cep;
                cmd.Parameters.Add("@idCliente", MySqlDbType.Int64).Value = pedido.idCliente;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }

        }

        public bool Atualizar(Pedido pedido)
        {
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();
                    MySqlCommand cmd = new MySqlCommand("Update tb_pedido set statusPagamento=@statusPagamento, rm=@rm, cep=@cep, idCliente=@idCliente " + " where idPedido=@idPedido ", conexao);
                    cmd.Parameters.Add("@idPedido", MySqlDbType.Int32).Value = pedido.idPedido;
                    cmd.Parameters.Add("@statusPagamento", MySqlDbType.Bit).Value = pedido.statusPagamento;//VERIFICAR SE É BIT OU BOOL
                    cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = pedido.rm; // ALTERAR O TIPO DE DADO NO BANCO DE DADOS E AQUI E NOS OUTROS REPOSITÓRIOS
                    cmd.Parameters.Add("@cep", MySqlDbType.Decimal).Value = pedido.cep;
                    cmd.Parameters.Add("@idCliente", MySqlDbType.Int64).Value = pedido.idCliente;
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0;

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar pedido: {ex.Message}");
                return false;

            }
        }

        public IEnumerable<Pedido> TodosPedidos()
        {
            List<Pedido> Pedidolist = new List<Pedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_pedido", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    Pedidolist.Add(
                                new Pedido
                                {
                                    idPedido = (int)(dr["idPedido"]),
                                    statusPagamento = ((bool)dr["statusPagamento"]),
                                    rm = (int)dr["rm"],//idem
                                    cep = ((int)dr["cep"]),//DEIXO ASSIM OU mudo para ConvertToDecimal
                                    idCliente = (int)(dr["idCliente"]),// idem
                                });
                }
                return Pedidolist;
            }
        }*/

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
                                idCartao = reader.IsDBNull(reader.GetOrdinal("idcartao")) 
                                ? (int?)null : reader.GetInt32("idcartao")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        /*public Pedido ObterPedido(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_pedido where idPedido=@id ", conexao);

                cmd.Parameters.AddWithValue("@id", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataReader dr;
                Pedido pedido = new Pedido();


                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    pedido.idPedido = (int)(dr["idPedido"]);
                    pedido.statusPagamento = ((bool)dr["statusPagamento"]);
                    pedido.rm = Convert.ToInt32(dr["rm"]);//idem
                    pedido.cep = ((int)dr["cep"]);//DEIXO ASSIM OU mudo para ConvertToDecimal
                    pedido.idCliente = (int)(dr["idCliente"]);         
                }
                return pedido;
            }
        }*/

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

        public void AvancarPedido(int idPedido, string statusAtual)
        {
            string proxStatus = statusAtual == "espera" ? "preparacao" : 
                (statusAtual == "preparacao" ? "transito" : "entregue");
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("update tb_pedido set statuspedido=@proxStatus " +
                                                    "where idpedido = @idPedido; ", conexao);

                cmd.Parameters.AddWithValue("@idPedido", idPedido);
                cmd.Parameters.AddWithValue("@proxStatus", proxStatus);

                cmd.ExecuteNonQuery();
            }
        }
    }
}

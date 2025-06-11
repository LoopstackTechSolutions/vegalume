using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Xml;
using vegalume.Models;


namespace vegalume.Repositorio
{
    public class PratoRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");

        public void Cadastrar(Prato prato)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_prato (nomePrato, precoPrato, descricaoPrato, " +
                    "valorCalorico, peso, pessoasServidas, statusPrato) values (@nomePrato, @precoPrato, " +
                    "@descricaoPrato, @valorCalorico, @peso, @pessoasServidas, 0)", conexao);

                cmd.Parameters.Add("@nomePrato", MySqlDbType.VarChar).Value = prato.nomePrato;
                cmd.Parameters.Add("@precoPrato", MySqlDbType.Decimal).Value = prato.precoPrato;
                cmd.Parameters.Add("@descricaoPrato", MySqlDbType.VarChar).Value = prato.descricaoPrato;
                cmd.Parameters.Add("@valorCalorico", MySqlDbType.Int64).Value = prato.valorCalorico;
                cmd.Parameters.Add("@peso", MySqlDbType.Decimal).Value = prato.peso;
                cmd.Parameters.Add("@pessoasServidas", MySqlDbType.Int32).Value = prato.pessoasServidas;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public bool Atualizar(Prato prato)
        {
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();
                    MySqlCommand cmd = new MySqlCommand("Update tb_prato set nomePrato=@nomePrato, precoPrato=@precoPrato, " +
                        "descricaoPrato=@descricaoPrato, valorCalorico=@valorCalorico, peso=@peso, " +
                        "pessoasServidas=@pessoasServidas, statusPrato=@statusPrato " + " where idPrato=@id ", conexao);
                    cmd.Parameters.Add("@idPrato", MySqlDbType.Int64).Value = prato.idPrato;
                    cmd.Parameters.Add("@nomePrato", MySqlDbType.VarChar).Value = prato.nomePrato;
                    cmd.Parameters.Add("@precoPrato", MySqlDbType.Decimal).Value = prato.precoPrato;
                    cmd.Parameters.Add("@descricaoPrato", MySqlDbType.VarChar).Value = prato.descricaoPrato;
                    cmd.Parameters.Add("@valorCalorico", MySqlDbType.Int64).Value = prato.valorCalorico;
                    cmd.Parameters.Add("@peso", MySqlDbType.Decimal).Value = prato.peso;
                    cmd.Parameters.Add("@pessoasServidas", MySqlDbType.Int32).Value = prato.pessoasServidas;
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0;

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar prato: {ex.Message}");
                return false;

            }
        }

        public IEnumerable<Prato> TodosPratos()
        {
            List<Prato> lista = new List<Prato>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_prato", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    lista.Add(
                                new Prato
                                {
                                    idPrato = (int)dr["idPrato"],
                                    nomePrato = (string)dr["nomePrato"],
                                    precoPrato = Convert.ToSingle(dr["precoPrato"]),
                                    descricaoPrato = (string)dr["descricaoPrato"],
                                    valorCalorico = (int)dr["valorCalorico"],
                                    peso = (int)dr["peso"],
                                    pessoasServidas = Convert.ToInt32(dr["pessoasServidas"]),
                                    statusPrato = (bool)dr["statusPrato"]
                                });
                }
                return lista;
            }
        }

        public IEnumerable<Prato> TodosPratosPorStatus(short status)
        {
            List<Prato> lista = new List<Prato>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_prato where statusPrato = @statusPrato", conexao);

                cmd.Parameters.AddWithValue("@statusPrato", status);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    lista.Add(
                                new Prato
                                {
                                    idPrato = (int)dr["idPrato"],
                                    nomePrato = (string)dr["nomePrato"],
                                    precoPrato = Convert.ToSingle(dr["precoPrato"]),
                                    descricaoPrato = (string)dr["descricaoPrato"],
                                    valorCalorico = (int)dr["valorCalorico"],
                                    peso = (int)dr["peso"],
                                    pessoasServidas = Convert.ToInt32(dr["pessoasServidas"]),
                                    statusPrato = Convert.ToUInt64(dr["statusPrato"]) != 0
                                });
                }
                return lista;
            }
        }

        public IEnumerable<PratoPedido> TodosPratosPorPedido(int idPedido)
        {
            List<PratoPedido> lista = new List<PratoPedido>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"select pr.nomeprato, pp.qtd, pp.detalhespedido from tb_prato_pedido pp " +
                                "inner join tb_prato pr on pr.idprato = pp.idprato " +
                                "where pp.idpedido = @idPedido;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idPedido", idPedido);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new PratoPedido
                            {
                                nomePrato = reader.GetString("nomeprato"),
                                qtd = reader.GetInt32("qtd"),
                                anotacoes = reader.IsDBNull(reader.GetOrdinal("detalhespedido"))
                                ? "" : reader.GetString("detalhespedido")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Prato ObterPratoPeloId(int idPrato)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_prato where idPrato=@id ", conexao);

                cmd.Parameters.AddWithValue("@id", idPrato);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataReader dr;
                Prato prato = new Prato();


                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    prato.idPrato = (int)dr["idPrato"];
                    prato.nomePrato = (string)dr["nomePrato"];
                    prato.precoPrato = Convert.ToSingle(dr["precoPrato"]);
                    prato.descricaoPrato = (string)dr["descricaoPrato"];
                    prato.valorCalorico = (int)dr["valorCalorico"];
                    prato.peso = (int)dr["peso"];
                    prato.pessoasServidas = Convert.ToInt32(dr["pessoasServidas"]);
                    prato.statusPrato = Convert.ToUInt64(dr["statusPrato"]) != 0;

                }
                return prato;
            }
        }

        public void TrocarStatus(int idPrato)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select statusPrato from tb_prato where idPrato=@idPrato", conexao);

                cmd.Parameters.AddWithValue("@idPrato", idPrato);

                short statusAtual = Convert.ToInt16(cmd.ExecuteScalar());
                short proxStatus = (short)(statusAtual == 1 ? 0 : 1);

                MySqlCommand cmd2 = new MySqlCommand("update tb_prato set statusPrato=@statusPrato " +
                    "where idPrato=@idPrato", conexao);

                cmd2.Parameters.AddWithValue("@idPrato", idPrato);
                cmd2.Parameters.AddWithValue("@statusPrato", proxStatus);

                cmd2.ExecuteNonQuery();
            }
        }
    }
}

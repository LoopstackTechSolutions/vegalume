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
                MySqlCommand cmd = new MySqlCommand("insert into tb_prato (nomePrato, precoPrato, descricaoPrato) values (@nomePrato, @precoPrato, @descricaoPrato)", conexao);
                                                                                                                                                 
                cmd.Parameters.Add("@nomePrato", MySqlDbType.VarChar).Value = prato.nomePrato;
                cmd.Parameters.Add("@precoPrato", MySqlDbType.Decimal).Value = prato.precoPrato;
                cmd.Parameters.Add("@descricaoPrato", MySqlDbType.VarChar).Value = prato.descricaoPrato;
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
                    MySqlCommand cmd = new MySqlCommand("Update tb_prato set nomePrato=@nomePrato, precoPrato=@precoPrato, descricaoPrato=@descricaoPrato " + " where idPrato=@id ", conexao);
                    cmd.Parameters.Add("@idPrato", MySqlDbType.Int64).Value = prato.idPrato;
                    cmd.Parameters.Add("@nomePrato", MySqlDbType.VarChar).Value = prato.nomePrato;
                    cmd.Parameters.Add("@precoPrato", MySqlDbType.Decimal).Value = prato.precoPrato;
                    cmd.Parameters.Add("@descricaoPrato", MySqlDbType.VarChar).Value = prato.descricaoPrato;
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
            List<Prato> Pratolist = new List<Prato>();

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
                    Pratolist.Add(
                                new Prato
                                {
                                    idPrato = Convert.ToInt64(dr["idPrato"]), 
                                    nomePrato = ((string)dr["nomePrato"]),
                                    precoPrato = ((float)dr["precoPrato"]),
                                    descricaoPrato = ((string)dr["descricaoPrato"]),
                                });
                }
                return Pratolist;
            }
        }

        public Prato ObterPrato(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_prato where idPrato=@id ", conexao);

                cmd.Parameters.AddWithValue("@id", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataReader dr;
                Prato prato = new Prato();


                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    prato.idPrato = Convert.ToInt64(dr["idPrato"]);
                    prato.nomePrato = (string)(dr["nomePrato"]); 
                    prato.precoPrato = (float)(dr["precoPrato"]); 
                    prato.descricaoPrato = (string)(dr["descricaoPrato"]);

                }
                return prato;
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tb_prato where idPrato=@id", conexao);

                cmd.Parameters.AddWithValue("@id", Id);

                int i = cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }
    }
}

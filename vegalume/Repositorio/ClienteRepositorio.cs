using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Xml;
using vegalume.Models;


namespace vegalume.Repositorio
{
    public class ClienteRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");


        public void Cadastrar(Cliente cliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_cliente (nome, senha, telefone, email) values (@nome, @senha, @telefone, @email)", conexao); // @: PARAMETRO
                                                                                                                                                 // Adiciona um parâmetro para o nome, definindo seu tipo e valor
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = cliente.nome;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = cliente.senha;
                cmd.Parameters.Add("@telefone", MySqlDbType.Decimal).Value = cliente.telefone;
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = cliente.email;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public bool Atualizar(Cliente cliente)
        {
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();
                    MySqlCommand cmd = new MySqlCommand("Update tb_cliente set nome=@nome, telefone=@telefone, email=@email " + " where idCliente=@id ", conexao);
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = cliente.idCliente;
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = cliente.nome;
                    cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = cliente.senha;
                    cmd.Parameters.Add("@telefone", MySqlDbType.Decimal).Value = cliente.telefone;
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = cliente.email;
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0;

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar cliente: {ex.Message}");
                return false;

            }
        }

        public IEnumerable<Cliente> TodosClientes()
        {
            List<Cliente> Clientlist = new List<Cliente>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_cliente", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    Clientlist.Add(
                                new Cliente
                                {
                                    idCliente = (int)dr["idCliente"],
                                    nome = (string)dr["nome"], 
                                    senha = (string)dr["senha"],
                                    telefone = (long)dr["telefone"],
                                    email = (string)dr["email"],
                                });
                }
                return Clientlist;
            }
        }

        public Cliente ObterClientePeloId(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_cliente where idCliente=@id ", conexao);

                cmd.Parameters.AddWithValue("@id", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataReader dr;
                Cliente cliente = new Cliente();

                
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    cliente.idCliente = (int)dr["idCliente"];
                    cliente.nome = (string)dr["nome"];
                    cliente.senha = (string)dr["senha"];
                    cliente.telefone = (long)dr["telefone"];
                    cliente.email = (string)dr["email"];
                }
                
                return cliente;
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tb_cliente where idCliente=@id", conexao);

                cmd.Parameters.AddWithValue("@id", Id);

                int i = cmd.ExecuteNonQuery();

                conexao.Close(); 
            }
        }
    }
}

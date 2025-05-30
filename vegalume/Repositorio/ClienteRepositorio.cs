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

        public Cliente ObterClientePeloId(int? Id)
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

        public IEnumerable<Endereco> TodosEnderecos(int? idCliente)
        {
            var lista = new List<Endereco>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"SELECT e.rua, e.numero, e.bairro, e.cidade, e.estado FROM tb_cliente c INNER JOIN tb_endereco e ON e.idcliente = c.idcliente WHERE c.idcliente = @idCliente;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Endereco
                            {
                                rua = reader.GetString("rua"),
                                numero = reader.GetInt16("numero"),
                                bairro = reader.GetString("bairro"),
                                cidade = reader.GetString("cidade"),
                                estado = reader.GetString("estado")
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public void CadastrarEndereco(Endereco endereco, int? idCliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_endereco (rua, numero, bairro, cidade, estado, idcliente)" +
                                                    " values (@rua, @numero, @bairro, @cidade, @estado, @idcliente)", conexao);
                                                                                                                                                                 // Adiciona um parâmetro para o nome, definindo seu tipo e valor
                cmd.Parameters.Add("@rua", MySqlDbType.VarChar).Value = endereco.rua;
                cmd.Parameters.Add("@numero", MySqlDbType.Int16).Value = endereco.numero;
                cmd.Parameters.Add("@bairro", MySqlDbType.VarChar).Value = endereco.bairro;
                cmd.Parameters.Add("@cidade", MySqlDbType.VarChar).Value = endereco.cidade;
                cmd.Parameters.Add("@estado", MySqlDbType.VarChar).Value = endereco.estado;
                cmd.Parameters.Add("@idcliente", MySqlDbType.Int32).Value = idCliente;
                cmd.ExecuteNonQuery();
                conexao.Close();
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

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

        public void CadastrarCliente(Cliente cliente)
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

        public void EditarCliente(Cliente cliente, int? id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("Update tb_cliente set nome=@nome, telefone=@telefone, senha=@senha " + " where idCliente=@id ", conexao);
                
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = cliente.nome;
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = cliente.senha;
                cmd.Parameters.Add("@telefone", MySqlDbType.Decimal).Value = cliente.telefone;
                cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = id;

                cmd.ExecuteNonQuery();
                conexao.Close();
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

        public Cliente ObterClientePeloEmail(string? email)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_cliente where email=@email ", conexao);

                cmd.Parameters.AddWithValue("@email", email);

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

                string query = @"SELECT e.idendereco, e.rua, e.numero, e.bairro, e.cidade, e.estado FROM tb_cliente c " +
                    "INNER JOIN tb_endereco e ON e.idcliente = c.idcliente WHERE c.idcliente = @idCliente;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Endereco
                            {
                                idEndereco = reader.GetInt32("idendereco"),
                                rua = reader.GetString("rua"),
                                numero = reader.GetInt32("numero"),
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

        public IEnumerable<Cartao> TodosCartoes(int? idCliente)
        {
            var lista = new List<Cartao>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string query = @"select ca.idcartao, ca.bandeira, ca.modalidade, ca.nometitular, ca.numerocartao from tb_cliente cl
                                inner join tb_cartao ca on ca.idcliente = cl.idcliente
                                where cl.idcliente = @idCliente;";

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var modalidadeBool = reader.GetFieldValue<bool>("modalidade");
                            string modalidade = modalidadeBool ? "crédito" : "débito";
                            lista.Add(new Cartao
                            {
                                idCartao = reader.GetInt32("idcartao"),
                                bandeira = reader.GetString("bandeira"),
                                modalidade = modalidade,
                                nomeTitular = reader.GetString("nometitular"),
                                numeroCartao = reader.GetInt64("numerocartao")
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

                cmd.Parameters.Add("@rua", MySqlDbType.VarChar).Value = endereco.rua;
                cmd.Parameters.Add("@numero", MySqlDbType.Int32).Value = endereco.numero;
                cmd.Parameters.Add("@bairro", MySqlDbType.VarChar).Value = endereco.bairro;
                cmd.Parameters.Add("@cidade", MySqlDbType.VarChar).Value = endereco.cidade;
                cmd.Parameters.Add("@estado", MySqlDbType.VarChar).Value = endereco.estado;
                cmd.Parameters.Add("@idcliente", MySqlDbType.Int32).Value = idCliente;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void CadastrarCartao(Cartao cartao, int? idCliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_cartao values " +
                    "(null, @numeroCartao, @nomeTitular, @dataValidade, @cvv, @modalidade, @bandeira, @idCliente)", conexao);

                cmd.Parameters.Add("@numeroCartao", MySqlDbType.Int64).Value = cartao.numeroCartao;
                cmd.Parameters.Add("@nomeTitular", MySqlDbType.VarChar).Value = cartao.nomeTitular;
                cmd.Parameters.Add("@dataValidade", MySqlDbType.Date).Value = cartao.dataValidade;
                cmd.Parameters.Add("@cvv", MySqlDbType.Int32).Value = cartao.cvv;

                int modalidadeBool = cartao.modalidade == "crédito" ? 1 : 0;
                cmd.Parameters.Add("@modalidade", MySqlDbType.Bit).Value = modalidadeBool;

                cmd.Parameters.Add("@bandeira", MySqlDbType.VarChar).Value = cartao.bandeira;
                cmd.Parameters.Add("@idCliente", MySqlDbType.Int32).Value = idCliente;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void ExcluirEndereco(int idEndereco)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update tb_endereco set idcliente = null where idEndereco = @idEndereco;", conexao);

                cmd.Parameters.Add("@idEndereco", MySqlDbType.Int32).Value = idEndereco;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void ExcluirCartao(int idCartao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update tb_cartao set idcliente = null where idCartao = @idCartao;", conexao);

                cmd.Parameters.Add("@idCartao", MySqlDbType.Int32).Value = idCartao;

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

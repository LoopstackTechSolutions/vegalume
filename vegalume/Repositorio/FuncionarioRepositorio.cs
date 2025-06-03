using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using vegalume.Models;
using System.Data;
using System.Xml;


namespace vegalume.Repositorio
{
    public class FuncionarioRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");

        public void Cadastrar(Funcionario funcionario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tb_funcionario (nome, senha, telefone, email) values (@nome, @senha, @telefone, @email)", conexao); // @: PARAMETRO
                                                                                                                                                 // Adiciona um parâmetro para o nome, definindo seu tipo e valor
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = funcionario.nome;
                
                cmd.Parameters.Add("@senha", MySqlDbType.VarChar).Value = funcionario.senha;

                cmd.Parameters.Add("@telefone", MySqlDbType.Decimal).Value = funcionario.telefone;
                
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = funcionario.email;
                
                cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }

        public bool Atualizar(Funcionario funcionario)
        {
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();
                    MySqlCommand cmd = new MySqlCommand("Update tb_funcionario set nome=@nome, senha=@senha, telefone=@telefone, email=@email " + " where rm=@rm ", conexao);
                    cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = funcionario.rm;
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = funcionario.nome;
                    cmd.Parameters.Add("@telefone", MySqlDbType.VarChar).Value = funcionario.telefone;
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = funcionario.email;
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0; 

                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar funcionario: {ex.Message}");
                return false;

            }
        }

        public IEnumerable<Funcionario> TodosFuncionarios()
        {
            List<Funcionario> Funcionariolist = new List<Funcionario>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_funcionario", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    Funcionariolist.Add(
                                new Funcionario
                                {
                                    rm = Convert.ToInt32(dr["rm"]), 
                                    nome = ((string)dr["nome"]), 
                                    senha = ((string)dr["senha"]),
                                    telefone = Convert.ToInt64(dr["telefone"]),//CONFIRMAR SE DEIXA DECIMAL MESMO OU INT
                                    email = ((string)dr["email"]), 
                                });
                }
                return Funcionariolist;
            }
        }

        public Funcionario ObterFuncionarioPeloEmail(string email)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_funcionario where email=@email", conexao);
                cmd.Parameters.AddWithValue("@email", email);

                using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr.Read())
                    {
                        return new Funcionario
                        {
                            rm = Convert.ToInt32(dr["rm"]),
                            nome = dr["nome"].ToString(),
                            senha = dr["senha"].ToString(),
                            telefone = Convert.ToInt64(dr["telefone"]),
                            email = dr["email"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public Funcionario ObterFuncionarioPeloRm(int? rm)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from tb_funcionario where rm=@rm", conexao);
                cmd.Parameters.AddWithValue("@rm", rm);

                using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr.Read())
                    {
                        return new Funcionario
                        {
                            rm = Convert.ToInt32(dr["rm"]),
                            nome = dr["nome"].ToString(),
                            senha = dr["senha"].ToString(),
                            telefone = Convert.ToInt64(dr["telefone"]),
                            email = dr["email"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void Excluir(int Rm)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from tb_funcionario where rm=@rm", conexao);

                cmd.Parameters.AddWithValue("@rm", Rm);

                int i = cmd.ExecuteNonQuery();

                conexao.Close(); 
            }
        }
    }
}

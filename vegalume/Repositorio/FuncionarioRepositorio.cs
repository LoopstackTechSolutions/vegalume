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
                MySqlCommand cmd = new MySqlCommand("insert into funcionario (nome, senha, telefone, email) values (@nome, @senha, @telefone, @email)", conexao); // @: PARAMETRO
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
                    MySqlCommand cmd = new MySqlCommand("Update funcionario set nome=@nome, senha=@senha, telefone=@telefone, email=@email " + " where rm=@rm ", conexao);
                    // Adiciona um parâmetro para o código do funcionario a ser atualizado, definindo seu tipo e valor
                    cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = funcionario.rm;
                    // Adiciona um parâmetro para o novo nome, definindo seu tipo e valor
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = funcionario.nome;
                    // Adiciona um parâmetro para o novo telefone, definindo seu tipo e valor
                    cmd.Parameters.Add("@telefone", MySqlDbType.VarChar).Value = funcionario.telefone;
                    // Adiciona um parâmetro para o novo email, definindo seu tipo e valor
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = funcionario.email;
                    // Executa o comando SQL de atualização e retorna o número de linhas afetadas
                    //executa e verifica se a alteração foi realizada
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0; // Retorna true se ao menos uma linha foi atualizada

                }
            }
            catch (MySqlException ex)
            {
                // Logar a exceção (usar um framework de logging como NLog ou Serilog)
                Console.WriteLine($"Erro ao atualizar funcionario: {ex.Message}");
                return false; // Retorna false em caso de erro

            }
        }

        // Método para listar todos os funcionarios do banco de dados
        public IEnumerable<Funcionario> TodosFuncionarios()
        {
            // Cria uma nova lista para armazenar os objetos Funcionario
            List<Funcionario> Funcionariolist = new List<Funcionario>();

            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para selecionar todos os registros da tabela 'funcionario'
                MySqlCommand cmd = new MySqlCommand("SELECT * from funcionario", conexao);

                // Cria um adaptador de dados para preencher um DataTable com os resultados da consulta
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                // Cria um novo DataTable
                DataTable dt = new DataTable();
                // metodo fill- Preenche o DataTable com os dados retornados pela consulta
                da.Fill(dt);
                // Fecha explicitamente a conexão com o banco de dados 
                conexao.Close();

                // interage sobre cada linha (DataRow) do DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    // Cria um novo objeto Funcionario e preenche suas propriedades com os valores da linha atual
                    Funcionariolist.Add(
                                new Funcionario
                                {
                                    rm = Convert.ToInt32(dr["rm"]), // Converte o valor da coluna "@rm" para inteiro
                                    nome = ((string)dr["nome"]), // Converte o valor da coluna "nome" para string
                                    senha = ((string)dr["senha"]),
                                    telefone = ((int)dr["telefone"]), // Converte o valor da coluna "telefone" para string
                                    email = ((string)dr["email"]), // Converte o valor da coluna "email" para string
                                });
                }
                // Retorna a lista de todos os funcionarios
                return Funcionariolist;
            }
        }

        // Método para buscar um funcionario específico pelo seu código (Rm)
        public Funcionario ObterFuncionario(int Rm)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para selecionar um registro da tabela 'funcionario' com base no código
                MySqlCommand cmd = new MySqlCommand("SELECT * from funcionario where rm=@rm ", conexao);

                // Adiciona um parâmetro para o código a ser buscado, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@rm", Rm);

                // Cria um adaptador de dados (não utilizado diretamente para ExecuteReader)
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                // Declara um leitor de dados do MySQL
                MySqlDataReader dr;
                // Cria um novo objeto Funcionario para armazenar os resultados
                Funcionario funcionario = new Funcionario();

                /* Executa o comando SQL e retorna um objeto MySqlDataReader para ler os resultados
                CommandBehavior.CloseConnection garante que a conexão seja fechada quando o DataReader for fechado*/

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                // Lê os resultados linha por linha
                while (dr.Read())
                {
                    // Preenche as propriedades do objeto Funcionario com os valores da linha atual
                    funcionario.rm = Convert.ToInt32(dr["rm"]);//propriedade Rm e convertendo para int
                    funcionario.nome = (string)(dr["nome"]); // propriedade Nome e passando string
                    funcionario.senha = (string)(dr["senha"]);
                    funcionario.telefone = (int)(dr["telefone"]); //propriedade telefone e passando string
                    funcionario.email = (string)(dr["email"]); //propriedade email e passando string
                }
                // Retorna o objeto Funcionario encontrado (ou um objeto com valores padrão se não encontrado)
                return funcionario;
            }
        }


        // Método para excluir um funcionario do banco de dados pelo seu código (ID)
        public void Excluir(int Rm)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();

                // Cria um novo comando SQL para deletar um registro da tabela 'funcionario' com base no código
                MySqlCommand cmd = new MySqlCommand("delete from funcionario where rm=@rm", conexao);

                // Adiciona um parâmetro para o código a ser excluído, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@rm", Rm);

                // Executa o comando SQL de exclusão e retorna o número de linhas afetadas
                int i = cmd.ExecuteNonQuery();

                conexao.Close(); // Fecha  a conexão com o banco de dados
            }
        }
    }
}

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

        public void Cadastrar(Pedido pedido)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into pedido (statusPagamento, rm, cep, idCliente) values (@statusPagamento, @rm, @cep, @idCliente)", conexao); // @: PARAMETRO
                                                                                                                                                 // Adiciona um parâmetro para o nome, definindo seu tipo e valor
                cmd.Parameters.Add("@statusPagamento", MySqlDbType.Bit).Value = pedido.statusPagamento;
                cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = pedido.rm; // ALTERAR O TIPO DE DADO NO BANCO DE DADOS E AQUI
                cmd.Parameters.Add("@cep", MySqlDbType.Decimal).Value = pedido.cep;
                cmd.Parameters.Add("@idCliente", MySqlDbType.Int32).Value = pedido.idCliente;
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
                    MySqlCommand cmd = new MySqlCommand("Update pedido set statusPagamento=@statusPagamento, rm=@rm, cep=@cep, idCliente=@idCliente " + " where idPedido=@idPedido ", conexao);
                    cmd.Parameters.Add("@idPedido", MySqlDbType.Int32).Value = pedido.idPedido;
                    cmd.Parameters.Add("@statusPagamento", MySqlDbType.Bit).Value = pedido.statusPagamento;
                    cmd.Parameters.Add("@rm", MySqlDbType.Int32).Value = pedido.rm; // ALTERAR O TIPO DE DADO NO BANCO DE DADOS E AQUI
                    cmd.Parameters.Add("@cep", MySqlDbType.Decimal).Value = pedido.cep;
                    cmd.Parameters.Add("@idCliente", MySqlDbType.Int32).Value = pedido.idCliente;
                    int linhasAfetadas = cmd.ExecuteNonQuery();

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
                MySqlCommand cmd = new MySqlCommand("SELECT * from pedido", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    Pedidolist.Add(
                                new Pedido
                                {
                                    idPedido = Convert.ToInt32(dr["idPedido"]),
                                    statusPagamento = ((bool)dr["statusPagamento"]),
                                    rm = ((Int32)dr["rm"]),
                                    cep = ((decimal)dr["cep"]),
                                    idCliente = ((string)dr["idCliente"]),
                                });
                }
                // Retorna a lista de todos os pedidos
                return Clientlist;
            }
        }

        // Método para buscar um pedido específico pelo seu código (Id)
        public Pedido ObterPedido(int Id)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para selecionar um registro da tabela 'pedido' com base no código
                MySqlCommand cmd = new MySqlCommand("SELECT * from pedido where idPedido=@id ", conexao);

                // Adiciona um parâmetro para o código a ser buscado, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@id", Id);

                // Cria um adaptador de dados (não utilizado diretamente para ExecuteReader)
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                // Declara um leitor de dados do MySQL
                MySqlDataReader dr;
                // Cria um novo objeto Pedido para armazenar os resultados
                Pedido pedido = new Pedido();

                /* Executa o comando SQL e retorna um objeto MySqlDataReader para ler os resultados
                CommandBehavior.CloseConnection garante que a conexão seja fechada quando o DataReader for fechado*/

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                // Lê os resultados linha por linha
                while (dr.Read())
                {
                    // Preenche as propriedades do objeto Pedido com os valores da linha atual
                    pedido.idPedido = Convert.ToInt32(dr["idPedido"]);//propriedade Id e convertendo para int
                    pedido.nome = (string)(dr["nome"]); // propriedade Nome e passando string
                    pedido.telefone = (string)(dr["telefone"]); //propriedade telefone e passando string
                    pedido.email = (string)(dr["email"]); //propriedade email e passando string
                }
                // Retorna o objeto Pedido encontrado (ou um objeto com valores padrão se não encontrado)
                return pedido;
            }
        }


        // Método para excluir um pedido do banco de dados pelo seu código (ID)
        public void Excluir(int Id)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();

                // Cria um novo comando SQL para deletar um registro da tabela 'pedido' com base no código
                MySqlCommand cmd = new MySqlCommand("delete from pedido where idPedido=@id", conexao);

                // Adiciona um parâmetro para o código a ser excluído, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@id", Id);

                // Executa o comando SQL de exclusão e retorna o número de linhas afetadas
                int i = cmd.ExecuteNonQuery();

                conexao.Close(); // Fecha  a conexão com o banco de dados
            }
        }
    }
}

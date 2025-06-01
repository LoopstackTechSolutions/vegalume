namespace vegalume.Models
{

    public class Funcionario
    {
        public int rm { get; set; }
        public string? nome { get; set; }
        public string? senha { get; set; }
        public long? telefone { get; set; }
        public string? email { get; set; }
        public List<Funcionario>? ListaFuncionario { get; set; }

    }
} 
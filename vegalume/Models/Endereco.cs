namespace vegalume.Models
{

    public class Endereco
    {
        public int idEndereco { get; set; }
        public string? cidade { get; set; }
        public string? bairro { get; set; }
        public string? rua { get; set; }
        public int? numero { get; set; }
        public string? estado { get; set; }
        public int? idCliente { get; set; }
    }
}
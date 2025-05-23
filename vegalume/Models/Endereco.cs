namespace vegalume.Models
{

    public class Endereco
    {
        public decimal? cep { get; set; }
        public string? cidade { get; set; }
        public string? bairro { get; set; }
        public string? rua { get; set; }
        public int? numero { get; set; }
        public List<Endereco>? ListaEndereco { get; set; }

    }
}
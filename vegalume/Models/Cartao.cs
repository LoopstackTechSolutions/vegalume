namespace vegalume.Models
{

    public class Cartao
    {
        public int idCartao { get; set; }
        public long? numeroCartao { get; set; }
        public string? nomeTitular { get; set; }
        public string? dataValidade { get; set; }
        public short? cvv { get; set; }
        public string? bandeira { get; set; }
        public string? modalidade { get; set; }
    }
}
namespace vegalume.Models
{

    public class Cartao
    {
        public decimal? numeroCartao { get; set; }
        public string? nomeTitular { get; set; }
        public DateTime? dataValidade { get; set; }
        public decimal? cvv { get; set; }
        public List<Cartao>? ListaCartao { get; set; }

    }
}
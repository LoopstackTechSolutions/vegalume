namespace vegalume.Models
{

    public class Pagamento
    {
        public int idPagamento { get; set; }
        public DateTime? dataHoraPagamento { get; set; }
        public decimal numeroCartao { get; set; }
        public int idPedido { get; set; }
        public List<Pagamento>? ListaPagamento { get; set; }

    }
}
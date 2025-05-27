using Google.Protobuf.WellKnownTypes;

namespace vegalume.Models
{

    public class Pedido
    {
        public int idPedido { get; set; }
        public bool? statusPagamento { get; set; }
        public DateTime ? dataHoraPedido { get; set; }//NÃO SEI QUAL COLOCAR NO LUGAR DO DATETIME
        public int? rm { get; set; }
        public int? cep { get; set; }
        public int? idCliente { get; set; }
        public List<Pedido>? ListaPedido { get; set; }

    }
}
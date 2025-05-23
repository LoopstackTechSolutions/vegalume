using Google.Protobuf.WellKnownTypes;

namespace vegalume.Models
{

    public class Pedido
    {
        public int idPedido { get; set; }
        public bool? statusPagamento { get; set; }
        public DateTime ? dataHoraPedido { get; set; }//NÃO SEI QUAL COLOCAR NO LUGAR DO DATETIME
        public string? rm { get; set; }
        public string? cep { get; set; }
        public string? idCliente { get; set; }
        public List<Prato>? ListaPrato { get; set; }

    }
}
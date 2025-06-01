using Google.Protobuf.WellKnownTypes;

namespace vegalume.Models
{

    public class Pedido
    {
        public int idPedido { get; set; }
        public string? statusPedido { get; set; }
        public DateTime ? dataHoraPedido { get; set; }
        public int? rm { get; set; }
        public int? idEndereco { get; set; }
        public int? idCliente { get; set; }
        public long? idCartao { get; set; }
    }
}
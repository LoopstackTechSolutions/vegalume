namespace vegalume.Models
{

    public class Cliente
    {
        public int idCliente { get; set; }
        public string? nome { get; set; }
        public string? senha { get; set; }
        public long? telefone { get; set; }
        public string? email { get; set; }
        public List<Cliente>? ListaCliente { get; set; }

    }
}
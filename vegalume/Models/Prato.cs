namespace vegalume.Models
{

    public class Prato
    {
        public int idPrato { get; set; } 
        public string? nomePrato { get; set; }
        public float? precoPrato { get; set; }
        public string? descricaoPrato { get; set; }
        public int? valorCalorico { get; set; }
        public int? peso { get; set; }
        public int? pessoasServidas { get; set; }
        public List<Prato>? ListaPrato { get; set; }

    }
}
namespace HqCatalog.Business.Models
{
    public class HQFavorita
    {
        public string UsuarioId { get; set; } // ID do usuário no Identity
        public int HQId { get; set; } // 🔹 Chave estrangeira para HQ

        // 🔹 Relacionamento com HQ
        public virtual Hq HQ { get; set; }
    }
}

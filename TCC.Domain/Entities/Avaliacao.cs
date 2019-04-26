namespace TCC.Domain.Entities
{
    public class Avaliacao
    {
        public int AvaliacaoId { get; set; }

        public int Nota { get; set; }

        public string Comentario { get; set; }

        public int UsuarioAvaliadorId { get; set; }

        public int UsuarioAvaliadoId { get; set; }

        public int LocacaoId { get; set; }

        public virtual Usuario UsuarioAvaliador { get; set; }

        public virtual Usuario UsuarioAvaliado { get; set; }

        public virtual Locacao Locacao { get; set; }
    }
}

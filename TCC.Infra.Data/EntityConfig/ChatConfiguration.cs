using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class ChatConfiguration : EntityTypeConfiguration<Chat>
    {
        public ChatConfiguration()
        {
            HasKey(c => c.ChatId);

            HasRequired(c => c.UsuarioLocador)
                .WithMany()
                .HasForeignKey(c => c.UsuarioLocadorId);

            HasRequired(c => c.UsuarioLocatario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioLocatarioId);
        }
    }
}

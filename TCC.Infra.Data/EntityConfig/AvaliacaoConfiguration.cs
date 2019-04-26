using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class AvaliacaoConfiguration : EntityTypeConfiguration<Avaliacao>
    {
        public AvaliacaoConfiguration()
        {
            HasKey(a => a.AvaliacaoId);

            Property(a => a.Nota)
                .IsRequired();

            Property(a => a.Comentario)
                .IsOptional()
                .HasMaxLength(250); 

            HasRequired(a => a.UsuarioAvaliador)
                .WithMany()
                .HasForeignKey(a => a.UsuarioAvaliadorId);

            HasRequired(a => a.UsuarioAvaliado)
                .WithMany()
                .HasForeignKey(a => a.UsuarioAvaliadoId);

            HasRequired(a => a.Locacao)
                .WithMany()
                .HasForeignKey(a => a.LocacaoId);
        }
    }
}

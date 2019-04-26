using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class FotoConfiguration : EntityTypeConfiguration<Foto>
    {
        public FotoConfiguration()
        {
            HasKey(f => f.FotoId);

            Property(f => f.Tipo)
                .IsRequired();

            Property(f => f.Imagem)
                .IsRequired();

            Property(f => f.VagaId)
                .IsOptional();

            Property(f => f.EventoId)
                .IsOptional();
        }
    }
}

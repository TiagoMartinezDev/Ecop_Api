using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOP.AccesoDatos.Data.Configurations
{
    public class ClienteConfig : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Apellido)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NroDocumento)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.Telefono)
                .HasMaxLength(20);

            builder.Ignore(x => x.NombreCompleto);

            builder.HasIndex(x => new { x.TipoDocumentoId, x.NroDocumento })
                .IsUnique();

            builder.HasOne(x => x.TipoDocumento)
                .WithMany(t => t.Clientes)
                .HasForeignKey(x => x.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

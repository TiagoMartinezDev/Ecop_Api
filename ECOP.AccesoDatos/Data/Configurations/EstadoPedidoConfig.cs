using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOP.AccesoDatos.Data.Configurations
{
    public class EstadoPedidoConfig : IEntityTypeConfiguration<EstadoPedido>
    {
        public void Configure(EntityTypeBuilder<EstadoPedido> builder)
        {
            builder.ToTable("EstadosPedido");

            builder.Property(x => x.Codigo)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Codigo)
                .IsUnique();
        }
    }
}

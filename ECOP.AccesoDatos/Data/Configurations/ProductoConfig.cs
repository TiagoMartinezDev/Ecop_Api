using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOP.AccesoDatos.Data.Configurations
{
    public class ProductoConfig : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos");

            builder.Property(x => x.Codigo)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.PrecioUnitario)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasIndex(x => x.Codigo)
                .IsUnique();

            builder.HasOne(x => x.UnidadMedida)
                .WithMany(u => u.Productos)
                .HasForeignKey(x => x.UnidadMedidaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

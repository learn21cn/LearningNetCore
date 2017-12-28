using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N3.EF.Entities
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {      
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(51);
            builder.Property(x => x.Amount).HasColumnType("decimal(8,2)");
            builder.HasOne(x => x.ProductCatogary).WithMany(x => x.Products).HasForeignKey(x => x.CatogaryID)
               .OnDelete(DeleteBehavior.Cascade);


        }
    }
}

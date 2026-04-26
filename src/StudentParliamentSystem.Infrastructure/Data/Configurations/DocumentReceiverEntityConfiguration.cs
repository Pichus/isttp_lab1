using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class DocumentReceiverEntityConfiguration : IEntityTypeConfiguration<DocumentReceiver>
{
    public void Configure(EntityTypeBuilder<DocumentReceiver> builder)
    {
        builder.ToTable("document_receivers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Position).IsRequired().HasMaxLength(255);
        builder.Property(x => x.FullTitle).IsRequired().HasMaxLength(500);
        builder.Property(x => x.IsDefault).IsRequired().HasDefaultValue(false);
    }
}

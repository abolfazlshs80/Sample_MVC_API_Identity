using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleDomain.Database.Models;

namespace SampleAPI_APP.Database.Configuration
{
    public class TokenBlacklistConfiguration: IEntityTypeConfiguration<TokenBlacklist>
    {
        public void Configure(EntityTypeBuilder<TokenBlacklist> builder)
        {
            
            builder
                  .Property<DateTime>("CreateDate")
                .HasDefaultValueSql("GETDATE()");
            builder
                .Property<string>("CreateBy")
                .ValueGeneratedOnUpdate();
            builder
                         .Property<DateTime>("UpdateDate")
                .HasDefaultValueSql("GETDATE()");





        }
    }
}

using MyClubLib.Models;
using System.Data.Entity.ModelConfiguration;

public class MemberConfiguration : EntityTypeConfiguration<Member>
{
    public MemberConfiguration()
    {
        // Configure primary key
        HasKey(m => m.MemberId);

        // Configure required properties
        Property(m => m.MemberName).IsRequired().HasMaxLength(100);
        Property(m => m.RegistrationDate).IsRequired();

    }
}

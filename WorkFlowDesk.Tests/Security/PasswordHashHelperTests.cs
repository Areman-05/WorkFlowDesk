using WorkFlowDesk.Common.Security;

namespace WorkFlowDesk.Tests.Security;

public class PasswordHashHelperTests
{
    [Fact]
    public void HashPassword_GeneraFormatoPbkdf2()
    {
        var hash = PasswordHashHelper.HashPassword("Admin123");

        Assert.StartsWith("PBKDF2.", hash);
        Assert.True(PasswordHashHelper.VerifyPassword("Admin123", hash));
        Assert.False(PasswordHashHelper.VerifyPassword("wrong", hash));
    }

    [Fact]
    public void VerifyPassword_AceptaHashLegacySha256()
    {
        const string legacyHash = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

        Assert.True(PasswordHashHelper.IsLegacyHash(legacyHash));
        Assert.True(PasswordHashHelper.VerifyPassword("password", legacyHash));
    }
}

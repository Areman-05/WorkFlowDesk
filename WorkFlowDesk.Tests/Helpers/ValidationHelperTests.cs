using WorkFlowDesk.Common.Helpers;

namespace WorkFlowDesk.Tests.Helpers;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("", false)]
    public void IsValidEmail_valida_formato(string email, bool esperado)
    {
        Assert.Equal(esperado, ValidationHelper.IsValidEmail(email));
    }

    [Theory]
    [InlineData("612345678", true)]
    [InlineData("12345", false)]
    [InlineData("abc", false)]
    public void IsValidPhone_requiere_al_menos_9_digitos(string phone, bool esperado)
    {
        Assert.Equal(esperado, ValidationHelper.IsValidPhone(phone));
    }

    [Theory]
    [InlineData("123456", true)]
    [InlineData("12345", false)]
    public void IsValidPassword_requiere_minimo_6_caracteres(string password, bool esperado)
    {
        Assert.Equal(esperado, ValidationHelper.IsValidPassword(password));
    }
}

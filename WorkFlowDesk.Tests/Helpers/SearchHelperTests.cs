using WorkFlowDesk.Common.Helpers;

namespace WorkFlowDesk.Tests.Helpers;

public class SearchHelperTests
{
    private sealed class Item
    {
        public string Nombre { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
    }

    [Fact]
    public void FilterByText_FiltraPorMultiplesCampos()
    {
        var items = new List<Item>
        {
            new() { Nombre = "Proyecto Alpha", Descripcion = "Cliente A" },
            new() { Nombre = "Beta", Descripcion = "Otro" }
        };

        var resultado = SearchHelper.FilterByText(items, "alpha", i => i.Nombre, i => i.Descripcion).ToList();

        Assert.Single(resultado);
        Assert.Equal("Proyecto Alpha", resultado[0].Nombre);
    }
}

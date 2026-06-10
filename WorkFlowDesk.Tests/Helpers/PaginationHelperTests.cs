using WorkFlowDesk.Common.Helpers;

namespace WorkFlowDesk.Tests.Helpers;

public class PaginationHelperTests
{
    [Fact]
    public void Aplicar_RetornaSoloElementosDeLaPagina()
    {
        var paginacion = new PaginationHelper { TamañoPagina = 2 };
        var datos = Enumerable.Range(1, 5).ToList();

        paginacion.PaginaActual = 2;
        var pagina = paginacion.Aplicar(datos).ToList();

        Assert.Equal(2, pagina.Count);
        Assert.Equal(3, pagina[0]);
        Assert.Equal(4, pagina[1]);
        Assert.Equal("Página 2 de 3 (5 registros)", paginacion.Resumen);
    }
}

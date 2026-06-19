namespace WorkFlowDesk.Common.Services;

/// <summary>Ciudades disponibles para autocompletado de ubicación.</summary>
public static class CityCatalog
{
    private static readonly string[] Cities =
    [
        "A Coruña, España", "Albacete, España", "Alcalá de Henares, España", "Alicante, España",
        "Almería, España", "Amsterdam, Países Bajos", "Andorra la Vella, Andorra", "Antwerp, Bélgica",
        "Badajoz, España", "Barcelona, España", "Bilbao, España", "Birmingham, Reino Unido",
        "Bologna, Italia", "Bordeaux, Francia", "Bruselas, Bélgica", "Burgos, España",
        "Cádiz, España", "Cartagena, España", "Castellón de la Plana, España", "Córdoba, España",
        "Dublin, Irlanda", "Edinburgh, Reino Unido", "Florencia, Italia", "Frankfurt, Alemania",
        "Gijón, España", "Ginebra, Suiza", "Granada, España", "Hamburgo, Alemania",
        "Huelva, España", "Jaén, España", "Las Palmas de Gran Canaria, España", "León, España",
        "Lille, Francia", "Lisboa, Portugal", "Logroño, España", "Londres, Reino Unido",
        "Lyon, Francia", "Madrid, España", "Málaga, España", "Manchester, Reino Unido",
        "Marbella, España", "Marsella, Francia", "Milán, Italia", "Montpellier, Francia",
        "Murcia, España", "Múnich, Alemania", "Nápoles, Italia", "Nice, Francia",
        "Oporto, Portugal", "Oslo, Noruega", "Oviedo, España", "Palma de Mallorca, España",
        "Pamplona, España", "París, Francia", "Praga, República Checa", "Roma, Italia",
        "Salamanca, España", "San Sebastián, España", "Santander, España", "Santiago de Compostela, España",
        "Sevilla, España", "Stockholm, Suecia", "Tarragona, España", "Toledo, España",
        "Torino, Italia", "Valencia, España", "Valladolid, España", "Varsovia, Polonia",
        "Vienna, Austria", "Vigo, España", "Vitoria-Gasteiz, España", "Zaragoza, España",
        "Zurich, Suiza"
    ];

    /// <summary>Busca ciudades que coincidan con el texto introducido.</summary>
    public static IReadOnlyList<string> Search(string query, int maxResults = 8)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<string>();

        var term = query.Trim();
        return Cities
            .Where(c => c.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.StartsWith(term, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(c => c.Length)
            .Take(maxResults)
            .ToList();
    }
}

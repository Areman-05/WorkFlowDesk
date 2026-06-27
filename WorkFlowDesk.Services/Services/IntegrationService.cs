using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class IntegrationService : IIntegrationService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    public Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo)
    {
        try
        {
            var uri = $"mailto:{Uri.EscapeDataString(destinatario)}?subject={Uri.EscapeDataString(asunto)}&body={Uri.EscapeDataString(cuerpo)}";
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> EnviarWebhookAsync(string url, string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        try
        {
            using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            var response = await Http.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> NotificarSlackAsync(string mensaje)
    {
        var path = Path.Combine(DatabasePaths.GetDataDirectory(), "integrations.json");
        if (!File.Exists(path))
            return false;

        try
        {
            var json = await File.ReadAllTextAsync(path);
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("SlackWebhookUrl", out var urlProp))
                return false;

            var url = urlProp.GetString();
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var payload = JsonSerializer.Serialize(new { text = mensaje });
            return await EnviarWebhookAsync(url, payload);
        }
        catch
        {
            return false;
        }
    }
}

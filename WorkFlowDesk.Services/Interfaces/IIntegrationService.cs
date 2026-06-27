namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Integraciones externas (email, webhooks).</summary>
public interface IIntegrationService
{
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo);
    Task<bool> EnviarWebhookAsync(string url, string payloadJson);
    Task<bool> NotificarSlackAsync(string mensaje);
}

namespace WorkFlowDesk.Services.Interfaces;

public interface IDatabaseInitializationService
{
    Task InitializeAsync();
    Task SeedDataAsync();
}

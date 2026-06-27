using WorkFlowDesk.Common.Services;

namespace WorkFlowDesk.Tests.Services;

public class SessionPersistenceServiceTests : IDisposable
{
    private readonly string _tempDir;

    public SessionPersistenceServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "wfd-session-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
        SessionPersistenceService.StorageDirectoryOverride = _tempDir;
        SessionPersistenceService.Clear();
    }

    public void Dispose()
    {
        SessionPersistenceService.Clear();
        SessionPersistenceService.StorageDirectoryOverride = null;
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Save_y_TryGetValidSession_recuperan_usuario()
    {
        SessionPersistenceService.Save(42);

        Assert.True(SessionPersistenceService.TryGetValidSession(out var userId));
        Assert.Equal(42, userId);
    }

    [Fact]
    public void Clear_elimina_sesion_persistida()
    {
        SessionPersistenceService.Save(7);
        SessionPersistenceService.Clear();

        Assert.False(SessionPersistenceService.TryGetValidSession(out _));
    }

    [Fact]
    public void TryGetValidSession_caduca_tras_MaxIdleTime()
    {
        var staleUtc = DateTime.UtcNow.Subtract(SessionPersistenceService.MaxIdleTime.Add(TimeSpan.FromHours(1)));
        var path = Path.Combine(_tempDir, "session.json");
        File.WriteAllText(path,
            $$"""{"UserId":3,"LastActiveUtc":"{{staleUtc:O}}"}""");

        Assert.False(SessionPersistenceService.TryGetValidSession(out _));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Touch_actualiza_marca_de_actividad()
    {
        SessionPersistenceService.Save(5);
        var path = Path.Combine(_tempDir, "session.json");
        var before = File.ReadAllText(path);

        Thread.Sleep(20);
        SessionPersistenceService.Touch(5);

        Assert.NotEqual(before, File.ReadAllText(path));
        Assert.True(SessionPersistenceService.TryGetValidSession(out var userId));
        Assert.Equal(5, userId);
    }
}

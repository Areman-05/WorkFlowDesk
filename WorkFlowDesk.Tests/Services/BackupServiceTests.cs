using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class BackupServiceTests
{
    [Fact]
    public async Task RestoreBackupAsync_devuelve_false_si_no_existe_archivo()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new BackupService(context);

        var ok = await service.RestoreBackupAsync(Path.Combine(Path.GetTempPath(), "no-existe-" + Guid.NewGuid() + ".db"));

        Assert.False(ok);
    }
}

using Data.Contexts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddDbContext<DataContext>(x => x.UseSqlServer("Server=tcp:sql-server-silicon.database.windows.net,1433;Initial Catalog=silicon_database;Persist Security Info=False;User ID=sqlAdmin;Password=Blessme!1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
	})
	.Build();

host.Run();

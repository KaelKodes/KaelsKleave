using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;

namespace KaelKodes.KaelsKleave;

public class KaelsKleaveConfigurator : IServiceConfigurator
{
    public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
    {
        serviceCollection.Configure<KaelsKleaveConfig>(
            openModStartupContext.Configuration.GetSection("KaelsKleave"));
        serviceCollection.AddSingleton<CleaveListener>();
    }
}

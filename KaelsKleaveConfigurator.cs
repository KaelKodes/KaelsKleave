using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.Core.Ioc.Extensions;

namespace KaelKodes.KaelsKleave;

public class KaelsKleaveConfigurator : IPluginContainerConfigurator
{
    public void ConfigureContainer(IPluginServiceConfigurationContext context)
    {
        var services = new ServiceCollection();
        services.Configure<KaelsKleaveConfig>(context.Configuration.GetSection("KaelsKleave"));
        context.ContainerBuilder.PopulateServices(services);
    }
}

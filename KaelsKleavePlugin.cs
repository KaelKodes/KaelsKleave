using System;
using Microsoft.Extensions.Logging;
using OpenMod.Unturned.Plugins;

namespace KaelKodes.KaelsKleave;

public class KaelsKleavePlugin : OpenModUnturnedPlugin
{
    public KaelsKleavePlugin(IServiceProvider serviceProvider, ILogger<KaelsKleavePlugin> logger)
        : base(serviceProvider)
    {
        logger.LogInformation("Kael's Kleave v{Version} is ready.", typeof(KaelsKleavePlugin).Assembly.GetName().Version);
    }
}

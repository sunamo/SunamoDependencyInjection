using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SunamoCl.SunamoCmd;
using SunamoCl.SunamoCmd.Args;

partial class Program
{

    const string appName = "";

    static ServiceCollection services = new();
    static ServiceProvider provider;
    static ILogger logger;

    static Program()
    {
        CmdBootStrap.AddILogger(services, true, null, appName);



        provider = services.BuildServiceProvider();

        logger = provider.GetService<ILogger>() ?? throw new ServiceNotFoundException(nameof(ILogger));
    }

    static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }

    static async Task MainAsync(String[] args)
    {
        var runnedAction = await CmdBootStrap.RunWithRunArgs(new RunArgs
        {
            AddGroupOfActions = AddGroupOfActions,
            Args = args,
            AskUserIfRelease = true,
            RunInDebugAsync = RunInDebugAsync,
            ServiceCollection = services,
            IsDebug =
#if DEBUG
            true
#else
false
#endif
        });

        Console.WriteLine("Finished: " + runnedAction);
        Console.ReadLine();
    }

    static async Task RunInDebugAsync()
    {
        await Task.Delay(1);
    }
}
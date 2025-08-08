;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SunamoCl.SunamoCmd;
using SunamoCl.SunamoCmd.Args;

partial class Program
{

    const string appName = "";

    static ServiceCollection Services = new();
    static ServiceProvider Provider;
    static ILogger logger;

    static Program()
    {
        CmdBootStrap.AddILogger(Services, true, null, appName);



        Provider = Services.BuildServiceProvider();

        logger = Provider.GetService<ILogger>() ?? throw new ServiceNotFoundException(nameof(ILogger));
    }

    static void Main()
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
            ServiceCollection = Services,
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
// variables names: ok

using SunamoCl;

partial class Program
{
    private static Dictionary<string, Func<Task<Dictionary<string, object>>>> AddGroupOfActions()
    {
        Dictionary<string, Func<Task<Dictionary<string, object>>>> groupsOfActions = new()
        {
            { "Other", Other }, // 1
        };

        return groupsOfActions;
    }

    static async Task<Dictionary<string, object>> Other()
    {
        var actions = OtherActions();

        // EN: CL.perform was removed in newer version of SunamoCl
        // CZ: CL.perform byl odstraněn v novější verzi SunamoCl
        await CLActions.PerformActionAsync(actions);

        return actions;
    }

    private static Dictionary<string, object> OtherActions()
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>();
        Dictionary<string, Func<Task>> actionsAsync = new Dictionary<string, Func<Task>>();

        return CLActions.MergeActions(actions, actionsAsync);
    }
}


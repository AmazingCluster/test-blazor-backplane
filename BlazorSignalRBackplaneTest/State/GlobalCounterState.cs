using Fluxor;

namespace BlazorSignalRBackplaneTest.State;

[FeatureState]
public class GlobalCounterState
{
    public int Count { get; init; } = 0;
}

public class IncrementGlobalCounterAction
{
    public int Increment { get; init; } = 1;
}

public partial class Reducers
{
    [ReducerMethod]
    public static GlobalCounterState ReduceIncrementGlobalCounterAction(GlobalCounterState state, IncrementGlobalCounterAction action)
    {
        return new GlobalCounterState { Count = state.Count + action.Increment };
    }
}

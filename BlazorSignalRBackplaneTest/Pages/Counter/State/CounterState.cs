using Fluxor;

namespace BlazorSignalRBackplaneTest.Pages.Counter.State
{
    [FeatureState]
    public class CounterState
    {
        public int Count { get; init; } = 0;
    }

    public class IncrementCounterAction
    {
        public int Increment { get; init; } = 1;
    }

    public partial class Reducers
    {
        [ReducerMethod]
        public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action)
        {
            return new CounterState { Count = state.Count + action.Increment };
        }
    }
}

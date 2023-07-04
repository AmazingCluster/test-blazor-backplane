using BlazorSignalRBackplaneTest.Pages.Counter.State;
using BlazorSignalRBackplaneTest.State;

using Fluxor;
using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;

namespace BlazorSignalRBackplaneTest.Pages.Counter
{
    public partial class Counter : FluxorComponent
    {
        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        [Inject]
        public IState<GlobalCounterState> GlobalState { get; set; } = default!;

        [Inject]
        public IState<CounterState> State { get; set; } = default!;

        private void IncrementCount(int increment)
        {
            Dispatcher.Dispatch(new IncrementCounterAction { Increment = increment });
        }

        private void IncrementGlobalCount(int increment)
        {
            Dispatcher.Dispatch(new IncrementGlobalCounterAction { Increment = increment });
        }
    }
}

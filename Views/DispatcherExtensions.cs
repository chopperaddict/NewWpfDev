using System;
using System . Runtime . CompilerServices;
using System . Windows . Threading;

namespace NewWpfDev . Views {
    public static class DispatcherExtns {
        public static SwitchToUiAwaitable SwitchToUi ( this Dispatcher dispatcher ) {
            return new SwitchToUiAwaitable ( dispatcher );
        }

        public struct SwitchToUiAwaitable : INotifyCompletion {
            private readonly Dispatcher _dispatcher;

            public SwitchToUiAwaitable ( Dispatcher dispatcher ) {
                _dispatcher = dispatcher;
            }

            public SwitchToUiAwaitable GetAwaiter ( ) {
                return this;
            }

            public void GetResult ( ) {
            }

            public bool IsCompleted => _dispatcher . CheckAccess ( );

            public void OnCompleted ( Action continuation ) {
                _dispatcher . BeginInvoke ( continuation );
            }
        }
    }

} // End namespace

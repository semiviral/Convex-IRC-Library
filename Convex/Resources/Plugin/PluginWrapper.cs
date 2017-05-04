﻿#region usings

using System;
using System.Threading.Tasks;
using Convex.Types.Events;
using Serilog;

#endregion

namespace Convex.Resources.Plugin {
    public class PluginWrapper {
        private readonly AsyncEvent<Func<SimpleMessageEventArgs, Task>> simpleMessagedEvent = new AsyncEvent<Func<SimpleMessageEventArgs, Task>>();

        private readonly AsyncEvent<Func<EventArgs, Task>> terminatedEvent = new AsyncEvent<Func<EventArgs, Task>>();
        public PluginHost Host;

        public event Func<EventArgs, Task> Terminated {
            add { terminatedEvent.Add(value); }
            remove { terminatedEvent.Remove(value); }
        }

        public event Func<SimpleMessageEventArgs, Task> SimpleMessaged {
            add { simpleMessagedEvent.Add(value); }
            remove { simpleMessagedEvent.Remove(value); }
        }

        private async Task Callback(ActionEventArgs e) {
            switch (e.ActionType) {
                case PluginActionType.SignalTerminate:
                    await terminatedEvent.InvokeAsync(EventArgs.Empty);
                    break;
                case PluginActionType.RegisterMethod:
                    if (!(e.Result is MethodRegistrar))
                        break;

                    Host.RegisterMethod((MethodRegistrar)e.Result);
                    break;
                case PluginActionType.SendMessage:
                    if (!(e.Result is SimpleMessageEventArgs))
                        break;

                    await simpleMessagedEvent.InvokeAsync((SimpleMessageEventArgs)e.Result);
                    break;
                case PluginActionType.Log:
                    if (!(e.Result is string))
                        break;

                    Log.Information((string)e.Result);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Initialise() {
            if (Host != null)
                return;

            Host = new PluginHost();
            Host.PluginCallback += Callback;
            Host.LoadPlugins();
        }
    }
}
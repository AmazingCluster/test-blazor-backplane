# Blazor Server RR LB PoC

Blazor Server uses sticky sessions by default. This becomes a problem when combining Blazor Server with .NET MVC in a RR Loadbalanced environment. Also, when running Blazor Server only, you cannot deploy on demand as a terminating pod disconnects the SignalR connection you'd lose all session state. 

To work around this, you'd need to do the following:

#### 1. Disable all transport methods except for Websockets
This is necessary as we need to stay connected to the same server. If the fallbacks are enabled and the websocket connection is lost, we would get send to another server. This is only required in a RR LB environment.

This is done in both the startup & the _Host.cshtml where the Blazor scripts are added.

#### 2. Use Redux pattern for all user-provided state & save this state locally
Normally, state is stored server-side. Restarting the application would cause errors as Blazor cannot restore the state. By saving the state to the browser's session storage you are always able to restore the session and let the user resume. This is only required for user-provided state. Other state can just be requested again

*See the `State` folders*

*Note. This is only necessary if you want to deploy/release on demand.*

#### 3. Encrypt session state
This is not actually necessary but good practice in my opinion. Once you store the state in the browser's storage, you can no longer trust its contents. By encrypting it, however, we don't have to constantly validate the state.

Hashing the session storage keys might also be good practice to not expose the namespaces in the storage, which it will do as the default is using the type's full name.

*See `SessionStateStorage`*

*This can be done by using the `ProtectedSessionStorage`* class provided by .NET
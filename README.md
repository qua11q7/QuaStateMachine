# QuaStateMachine
QuaStateMachine framework allows you to convert state machine diagram to code. It supports orthogonal and inner states. There are more than 10 samples in the QuaStateMachineSamples project. Some of the samples are shown below to introduce how to use QuaStateMachine.

## Stopwatch Sample

Stopwatch state machine diagram is shown below:

[![1_Stopwatch.jpg](https://s33.postimg.cc/gdwgr7dvz/1_Stopwatch.jpg)](https://postimg.cc/image/y3y5c8rgr/)

In this sample diagram, there are 3 *States* and 2 *Signals*:
#####States
- Active
- Stopped
- Running

#####Signals
- Reset
- StartStop

It should be noted that **Stopped** and **Running** states are inner states of the **Active** state.

All of the states are connected to each other with *Transitions*. Transitions can be triggered via emitting the corresponding *Signal*. In the diagram above, 5 transitions can be seen. Two of the transitions indicate that **Active** and **Stopped** states should be the initial state of their corresponding level. There are two transitions between **Stopped** and **Running** states, one leading from **Stopped** to **Running** and the other leading from **Running** to **Stopped**. These two different transitions are connected to the same signal, **StartStop**. If the current state is **Stopped** and the **StartStop** signal is emitted, state machine will change its state to **Running** state. If the current state is **Started** and the **StartStop** signal is emitted, state machine then will change its state to **Stopped** state.  There cannot be a transition without a connected signal. 

Stopwatch state machine will be implemented 4 different way using QuaStateMachine framework. The preferred and easist way to implement the state machine is the fourth implementation.

### First Implementation

This implementation is the most general implementation of the state machine. Transitions must be created by hand and all state, transition and signal names are string.

```csharp
    // Declare class-wide gloabal variables
	StateMachine smStopwatch;
    ISignal sigReset;
    ISignal sigStartStop;
```

StateMachine class will contain all the information of the state diagram. Creation of state, transitions and signals are done by using this class. Created states, transitions and signals are returned as interfaces. Signals are most used object in the framework, so their corresponding objects are declared in the global scope of the class. 

```csharp
void Initialize() {
	// Instantiate a new StateMachine object.
    smStopwatch = new StateMachine();

	// Variables to cache the created state objects.
    IState sActive;
    IState sStopped;
    IState sRunning;

	// TryCreateState method will create a IState object with given name.
	// It returns boolean value to indicate if the creation is successfull or not.
	// Created object can be retrieved with an out parameter.
	// If another state name is supplied as a second parameter, created state will be a child state.
    smStopwatch.TryCreateState("sActive", out sActive);
    smStopwatch.TryCreateState("sStopped", sActive, out sStopped);
    smStopwatch.TryCreateState("sRunning", sActive, out sRunning);

	// CreateTransition method will create a ITransition object and return it.
	// First parameter is the name of the transition, second parameter is the source state and the third parameter is the target state of the transition.
    smStopwatch.CreateTransition("sActive loop", sActive, sActive);
    smStopwatch.CreateTransition("sStopped to sRunning", sStopped, sRunning);
    smStopwatch.CreateTransition("sRunning to sStopped", sRunning, sStopped);

	// ConnectSignal method is used to create ISignal objects.
	// Created ISignal object can be retrieved with an out parameter.
	// To create a signal, a signal name and a transition name must be supplied.
	// If a signal has already created and will be connected to another transition, out parameter can be omitted.
    smStopwatch.ConnectSignal("sigReset", "sActive loop", out sigReset);
    smStopwatch.ConnectSignal("sigStartStop", "sStopped to sRunning", out sigStartStop);
    smStopwatch.ConnectSignal(sigStartStop, "sRunning to sStopped");

	// SetInitialState indicates the starting state of the diagram.
	// Second parameter indicates that the first parameter is the inner state of the second parameter.
	// When the state machine enters the parent state, this inner state will be the initial state of its level.
    smStopwatch.SetInitialState(sActive);
    smStopwatch.SetInitialState(sStopped, sActive);

	// Subscribing to IState interface's events.
    sActive.OnStateEnter += SActive_OnStateEnter;
    sActive.OnStateLeave += SActive_OnStateLeave;
    sStopped.OnStateEnter += SStopped_OnStateEnter;
    sStopped.OnStateLeave += SStopped_OnStateLeave;
    sRunning.OnStateEnter += SRunning_OnStateEnter;
    sRunning.OnStateLeave += SRunning_OnStateLeave;
	
	// Start the state machine.
	smStopwatch.Initialize();
}
```

State machine will change its state when a signal is emitted. To emit a signal, call **Emit()** method on an ISignal interface object.

### Second Implementation

In this implementation, instead of using strings as names, enum values will be used. Generic implementation of StateMachine class allows you to use any object as name parameter.

```csharp
// Craete enums to use as State, Transition and Signal names.
enum States {
    Active,
    Stopped,
    Running
}

enum Transitions {
    ActiveLoop,
    Stopped2Runnig,
    Running2Stopped
}

enum Signals {
    Reset,
    StartStop
}
```
```csharp
// Declare class-wide gloabal variables
StateMachine<States, Transitions, Signals> smStopwatch;
ISignal sigReset;
ISignal sigStartStop;

void Initialize() {
	// In this initialization, instead of using string as a name parameter,
	// we will be using corresponding enums.
    smStopwatch = new StateMachine<States, Transitions, Signals>();

    IState sActive;
    IState sStopped;
    IState sRunning;

    smStopwatch.TryCreateState(States.Active, out sActive);
    smStopwatch.TryCreateState(States.Stopped, States.Active, out sStopped);
    smStopwatch.TryCreateState(States.Running, States.Active, out sRunning);

    smStopwatch.CreateTransition(Transitions.ActiveLoop, States.Active, States.Active);
    smStopwatch.CreateTransition(Transitions.Stopped2Runnig, States.Stopped, States.Running);
    smStopwatch.CreateTransition(Transitions.Running2Stopped, States.Running, States.Stopped);

    smStopwatch.ConnectSignal(Signals.Reset, Transitions.ActiveLoop, out sigReset);
    smStopwatch.ConnectSignal(Signals.StartStop, Transitions.Stopped2Runnig, out sigStartStop);
    smStopwatch.ConnectSignal(Signals.StartStop, Transitions.Running2Stopped);

    smStopwatch.SetInitialState(States.Active);
    smStopwatch.SetInitialState(States.Stopped, States.Active);

    sActive.OnStateEnter += SActive_OnStateEnter;
    sActive.OnStateLeave += SActive_OnStateLeave;
    sStopped.OnStateEnter += SStopped_OnStateEnter;
    sStopped.OnStateLeave += SStopped_OnStateLeave;
    sRunning.OnStateEnter += SRunning_OnStateEnter;
    sRunning.OnStateLeave += SRunning_OnStateLeave;
	
	smStopwatch.Initialize();
}
```

### Third Implementation

In this implementation, we will be using a helper class to generate our state machine diagram. This helper class can be found in **QuaStateMachine.Creator** namespace.

```csharp
// Declare class-wide gloabal variables
StateMachine<States, Transitions, Signals> smStopwatch;
ChainCreator<States, Transitions, Signals> creator;
ISignal sigReset;
ISignal sigStartStop;

void Initialize() {
    smStopwatch = new StateMachine<States, Transitions, Signals>();
    creator = new ChainCreator<States, Transitions, Signals>(smStopwatch);

    creator
        .CreateState(States.Active)
        .OnStateEnter(SActive_OnStateEnter)
        .OnStateLeave(SActive_OnStateLeave)
        .CreateInnerState(States.Stopped, States.Active)
        .OnStateEnter(SStopped_OnStateEnter)
        .OnStateLeave(SStopped_OnStateLeave)
        .CreateInnerState(States.Running, States.Active)
        .OnStateEnter(SRunning_OnStateEnter)
        .OnStateLeave(SRunning_OnStateLeave)
        .CreateTransition(Transitions.ActiveLoop, States.Active, States.Active)
        .CreateTransition(Transitions.Stopped2Runnig, States.Stopped, States.Running)
        .CreateTransition(Transitions.Running2Stopped, States.Running, States.Stopped)
        .CreateSignal(Signals.Reset, Transitions.ActiveLoop, out sigReset)
        .CreateSignal(Signals.StartStop, Transitions.Stopped2Runnig, out sigStartStop)
        .ConnectSignal(Signals.StartStop, Transitions.Running2Stopped)
        .SetInitialState(States.Active)
        .SetInitialInnerState(States.Stopped, States.Active);
}
```

This implementation allows you to create your state machine diagram without the need of caching the IState and ITransition objects.

### Fourth Implementation

This implementation shows the most basic and easist way of creating a state machine diagram. It completely eliminates the necessary creation of transitions. Transitions are automatically created in the background and can be retrieved if desired.

```csharp
public enum States {
    Active,
    Stopped,
    Running
}

public enum Signals {
    Reset,
    StartStop
}

StateMachine<States, Signals> SM;

private void Initialize() {
    SM = new StateMachine<States, Signals>();
            
    // Get the active IState object to subscribe to OnStateEnter and OnStateLeave events.
    IState activeState = SM.CreateState(States.Active);
    // We want to only subscribe to OnStateEnter event of Stopped state. We can write an anonymous function such as:
    SM.CreateState(States.Stopped, States.Active).OnStateEnter += () => { stopwatch?.Stop(); Console.WriteLine("Elapsed time: " + stopwatch?.ElapsedMilliseconds.ToString()); };
    // We also only want to listen OnStateEnter event of Running state. 
    SM.CreateState(States.Running, States.Active).OnStateEnter += () => { stopwatch?.Start(); };
            
    // Subscribe to activeState's event with anonymous functions.
    activeState.OnStateEnter += () => { stopwatch = stopwatch ?? new Stopwatch(); };
    activeState.OnStateLeave += () => { stopwatch?.Reset(); };

    // We do not need to get the signal objects if we don't want to. We can emit the signal with the Signals enum.
    SM.ConnectSignal(Signals.Reset, States.Active, States.Active, out ISignal resetSignal);
    SM.ConnectSignal(Signals.StartStop, States.Stopped, States.Running, out ISignal startStopSignal);
    SM.ConnectSignal(Signals.StartStop, States.Running, States.Stopped);

    SM.SetInitialState(States.Active);
    SM.SetInitialState(States.Stopped, States.Active);

    SM.OnStateChangedGeneric += SM_OnStateChanged;
	
	SM.Initialize();
}

private void SM_OnStateChanged(IState<States> priorState, IState<States> formerState) {
    Console.WriteLine(priorState.Name + " --> " + formerState.Name);

    // All logic can be implemented here as well. We just need to watch prior and former states.
}

// Another way of emitting signals.
public void EmitSignal(Signals signal) {
    SM.GetSignalByName(signal).Emit();
}
```

## Keyboard Sample

In this sample, orthogonal states usage will be shown. Keyboard samples state machine diagram is given below.

[![3_Keyboard.jpg](https://s33.postimg.cc/jnk2p33vj/3_Keyboard.jpg)](https://postimg.cc/image/8b7h7av6j/)

It can be seen from the diagram above, there are 7 states and 3 signals. Orthogonal regions are shown as dashed lines. Each orthogonal region has its own inital state. This diagrams implementation will be given with the transitionless creation of state machine.

###Keyboard Implementation

```csharp
StateMachine<States, Signals> SM;

private void Initialize() {
    SM = new StateMachine<States, Signals>();

	// After specifying which parent this state belongs to, you can also specify which orthogonal region of the parent this inner state is in.
	// Orthogonal region index starts from 0 and if not specified otherwise, it will be taken as 0.
    SM.CreateState(States.Active);
    SM.CreateState(States.NumLockOff, States.Active);
    SM.CreateState(States.NumLockOn, States.Active);
    SM.CreateState(States.ScrollLockOff, States.Active, 1);
    SM.CreateState(States.ScrollLockOn, States.Active, 1);
    SM.CreateState(States.CapsLockOff, States.Active, 2);
    SM.CreateState(States.CapsLockOn, States.Active, 2);

    SM.ConnectSignal(Signals.NumLock, States.NumLockOff, States.NumLockOn);
    SM.ConnectSignal(Signals.NumLock, States.NumLockOn, States.NumLockOff);
    SM.ConnectSignal(Signals.ScrollLock, States.ScrollLockOff, States.ScrollLockOn);
    SM.ConnectSignal(Signals.ScrollLock, States.ScrollLockOn, States.ScrollLockOff);
    SM.ConnectSignal(Signals.CapsLock, States.CapsLockOff, States.CapsLockOn);
    SM.ConnectSignal(Signals.CapsLock, States.CapsLockOn, States.CapsLockOff);

	// Orthogonal region index must be given when an initial inner state is being set.
	// If not given, orthogonal region index will be taken as 0.
    SM.SetInitialState(States.Active);
    SM.SetInitialState(States.NumLockOff, States.Active);
    SM.SetInitialState(States.ScrollLockOff, States.Active, 1);
    SM.SetInitialState(States.CapsLockOff, States.Active, 2);

	SM.Initialize();
}
```











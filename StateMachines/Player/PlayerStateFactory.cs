using System.Collections.Generic;

enum PlayerStates
{
    grounded,
    jump,
    fall,
    land,
    climbLedge,
    climbObject,
    idle,
    walk,
    run,
    balance,
}

public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        // root states
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.jump] = new PlayerJumpState(_context, this);
        _states[PlayerStates.fall] = new PlayerFallState(_context, this);
        _states[PlayerStates.land] = new PlayerLandState(_context, this);
        _states[PlayerStates.climbLedge] = new PlayerClimbLedgeState(_context, this);
        _states[PlayerStates.climbObject] = new PlayerClimbObjectState(_context, this);

        // sub states
        _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.run] = new PlayerRunState(_context, this);
        // _states[PlayerStates.balance] = new PlayerBalanceState(_context, this);
        

    }

    // root states
    public PlayerBaseState Grounded() { return _states[PlayerStates.grounded]; }
    public PlayerBaseState Jump() { return _states[PlayerStates.jump]; }
    public PlayerBaseState Fall() { return _states[PlayerStates.fall]; }
    public PlayerBaseState Land() { return _states[PlayerStates.land]; }
    public PlayerBaseState ClimbLedge() { return _states[PlayerStates.climbLedge]; }
    public PlayerBaseState ClimbObject() { return _states[PlayerStates.climbObject]; }

    // sub states
    public PlayerBaseState Idle() { return _states[PlayerStates.idle]; }
    public PlayerBaseState Walk() { return _states[PlayerStates.walk]; }
    public PlayerBaseState Run() { return _states[PlayerStates.run]; }
    public PlayerBaseState Balance() { return _states[PlayerStates.balance]; }
}

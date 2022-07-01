﻿namespace mark.davison.finance.common.client.State;

public interface IStateStore
{
    StateInstance<TState> GetState<TState>() where TState : class, IState, new();

    void SetState<TState>(TState state) where TState : class, IState, new();

    void Reset();
}

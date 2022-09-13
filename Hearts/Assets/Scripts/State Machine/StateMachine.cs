﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this in the awake of the class using stat machine
///_stateMachine.SetState(defaultState);
///
///void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(from: to, to: from, condition);
/// </summary>

[System.Serializable]
public class StateMachine
{
    [SerializeField] private IState _currentState;
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>(capacity: 0);

    public void Tick()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            SetState(transition.To);
        }

        _currentState.Tick();
    }

    public void SetState(IState state)
    {
        if (state == _currentState)
            return;

        _currentState?.OnExit();
        _currentState = state;


        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);

        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        _currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        
        if(_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;

        }

        transitions.Add(item: new Transition(to, predicate));

    }

    private class Transition
    {
        public Func<bool> Condition { get; }

        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        _anyTransitions.Add(item: new Transition(state, predicate));
    }

    private Transition GetTransition()
    {
        foreach(var transition in _anyTransitions)
            if(transition.Condition())
                return transition;
        
        foreach(var transition in _currentTransitions)
            if(transition.Condition())
                return transition;

        return null;
            
        
    }


}

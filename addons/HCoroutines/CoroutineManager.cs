namespace HCoroutines;

using System;
using System.Collections.Generic;
using Godot;

public partial class CoroutineManager : Node {
    public static CoroutineManager Instance { get; private set; }
    public float DeltaTime { get; private set; }
    public double DeltaTimeDouble { get; private set; }

    private bool _isIteratingActiveCoroutines;
    private readonly HashSet<CoroutineBase> _activeCoroutines = [];
    private readonly HashSet<CoroutineBase> _coroutinesToDeactivate = [];
    private readonly HashSet<CoroutineBase> _coroutinesToActivate = [];

    public void StartCoroutine(CoroutineBase coroutine) {
        coroutine.Manager = this;
        coroutine.OnEnter();
    }

    public void ActivateCoroutine(CoroutineBase coroutine) {
        if (_isIteratingActiveCoroutines) {
            _coroutinesToActivate.Add(coroutine);
            _coroutinesToDeactivate.Remove(coroutine);
        } else {
            _activeCoroutines.Add(coroutine);
        }
    }

    public void DeactivateCoroutine(CoroutineBase coroutine) {
        if (_isIteratingActiveCoroutines) {
            _coroutinesToDeactivate.Add(coroutine);
            _coroutinesToActivate.Remove(coroutine);
        } else {
            _activeCoroutines.Remove(coroutine);
        }
    }

    public override void _EnterTree() {
        Instance = this;
    }

    public override void _Process(double delta) {
        DeltaTimeDouble = delta;
        DeltaTime = (float)delta;

        _isIteratingActiveCoroutines = true;

        foreach (CoroutineBase coroutine in _activeCoroutines) {
            if (coroutine.IsAlive && coroutine.IsPlaying) {
                try {
                    coroutine.Update();
                } catch (Exception e) {
                    GD.PrintErr(e.ToString());
                }
            }
        }

        _isIteratingActiveCoroutines = false;

        foreach (CoroutineBase coroutine in _coroutinesToActivate) {
            _activeCoroutines.Add(coroutine);
        }
        _coroutinesToActivate.Clear();

        foreach (CoroutineBase coroutine in _coroutinesToDeactivate) {
            _activeCoroutines.Remove(coroutine);
        }
        _coroutinesToDeactivate.Clear();
    }
}

using System;

namespace TinyVanguard.Session
{
    public readonly struct InputMapActivation
    {
        public InputMapActivation(bool gameplay, bool system, bool ui)
        {
            Gameplay = gameplay;
            System = system;
            UI = ui;
        }

        public bool Gameplay { get; }
        public bool System { get; }
        public bool UI { get; }
    }

    public static class GameplayInputPolicy
    {
        public static InputMapActivation GetActivation(GameSessionState state)
        {
            return state switch
            {
                GameSessionState.Playing => new InputMapActivation(true, true, false),
                GameSessionState.Paused => new InputMapActivation(false, true, true),
                GameSessionState.Victory => new InputMapActivation(false, true, true),
                GameSessionState.Defeat => new InputMapActivation(false, true, true),
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        public static bool TryGetPauseToggleState(
            GameSessionState current,
            out GameSessionState next)
        {
            switch (current)
            {
                case GameSessionState.Playing:
                    next = GameSessionState.Paused;
                    return true;
                case GameSessionState.Paused:
                    next = GameSessionState.Playing;
                    return true;
                case GameSessionState.Victory:
                case GameSessionState.Defeat:
                    next = current;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(current), current, null);
            }
        }
    }
}

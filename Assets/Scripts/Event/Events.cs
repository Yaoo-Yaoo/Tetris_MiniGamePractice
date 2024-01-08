using System;

namespace Tetris.Event
{
    public class InputEvent<T>
    {
        private event Action<T> action;

        public void Register(Action<T> action)
        {
            this.action += action;
        }

        public void UnRegister(Action<T> action)
        {
            this.action -= action;
        }

        public void Trigger(T arg)
        {
            action?.Invoke(arg);
        }
    }

    public class DataEvent<T>
    {
        private event Action<T> action;

        public void Register(Action<T> action)
        {
            this.action += action;
        }

        public void UnRegister(Action<T> action)
        {
            this.action -= action;
        }

        public void Trigger(T arg)
        {
            action?.Invoke(arg);
        }
    }
}

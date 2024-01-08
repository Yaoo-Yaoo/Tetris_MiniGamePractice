using Tetris.Event;

namespace Tetris
{
    public class EventManager
    {
        public InputEvent<E_Input_Type> onReceiveInput;
        public DataEvent<int> onGameScoreChanged;

        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null) _instance = new EventManager();
                return _instance;
            }
        }
        private EventManager()
        {
            onReceiveInput = new InputEvent<E_Input_Type>();
            onGameScoreChanged = new DataEvent<int>();
        }
    }
}

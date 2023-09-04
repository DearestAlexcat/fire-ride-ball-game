using UnityEngine;

namespace Client
{ 
    public class UI : MonoBehaviour
    {
        // World screen. For blurring.
        [field: SerializeField] public GameScreenWorld GameScreenWorld { get; set; }

        // Screen space Camera
        [field: SerializeField] public GameScreen GameScreen { get; set; }
        [field: SerializeField] public MainScreen MainScreen { get; set; }
        [field: SerializeField] public RestartScreen RestartScreen { get; set; }
        [field: SerializeField] public PauseScreen PauseScreen { get; set; }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CloseAll()
        {
            GameScreen.Show(false);
            MainScreen.Show(false);
            RestartScreen.Show(false);
            PauseScreen.Show(false);
        }
    }
}
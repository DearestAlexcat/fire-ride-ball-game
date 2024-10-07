using UnityEngine;

namespace Client
{
    static class GameInitialization
    {
        public static void FullInit()
        {
            InitializeUI();
        }

        public static void InitializeUI()
        {
            var configuration = Service<StaticData>.Get();

            var ui = Service<UI>.Get();

            if (ui == null)
            {
                ui = Object.Instantiate(configuration.UI);
                Service<UI>.Set(ui);
            }
        }
    }
}
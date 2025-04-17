#if IL2CPP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#elif (MONO)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#endif
using MelonLoader;
using System.Collections.Generic;
using SOE;

[assembly: MelonInfo(typeof(MyMod), "Silk Road App", "1.0.0", "Akermi")]
[assembly: MelonGame("TVGS", "Schedule I")]


namespace SOE
{
    public class MyMod : MelonMod
    {
        private bool _isInGame;
        private MyApp _app;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            bool nowInGame = sceneName != null && sceneName.Contains("Main");

            if (!_isInGame && nowInGame)
            {
                LoggerInstance.Msg("Entering game scene: " + sceneName);
                _app = new MyApp();
                _app.Init(LoggerInstance);
            }
            else if (_isInGame && !nowInGame)
            {
                LoggerInstance.Msg("Exiting game scene.");
                _app = null;
            }

            _isInGame = nowInGame;
        }
    }
}
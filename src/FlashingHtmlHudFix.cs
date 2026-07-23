using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace FlashingHtmlHudFix
{
    [MinimumApiVersion(247)]
    public partial class FlashingHtmlHudFix : BasePlugin
    {
        public override string ModuleName => "FlashingHtmlHudFix";
        public override string ModuleVersion => "1.1";
        public override string ModuleAuthor => "Deana https://x.com/girlglock_/";
        public override string ModuleDescription => "A Plugin that fixes Html Hud.";

        private CCSGameRules? _gameRules;
        private bool _gameRulesInitialized;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnTick>(OnTick);
            RegisterListener<Listeners.OnMapStart>(OnMapStartHandler);

            if (hotReload)
            {
                InitializeGameRules();
            }
        }

        private void OnMapStartHandler(string mapName)
        {
            _gameRules = null;
            _gameRulesInitialized = false;
        }

        private void InitializeGameRules()
        {
            if (_gameRulesInitialized) return;
            
            var gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
            _gameRules = gameRulesProxy?.GameRules;
            _gameRulesInitialized = _gameRules != null;
        }

        private void OnTick()
        {
            if (!_gameRulesInitialized)
            {
                InitializeGameRules();
                return;
            }

            if (_gameRules != null)
            {
                _gameRules.GameRestart = _gameRules.RestartRoundTime < Server.CurrentTime;
            }
        }
    }
}

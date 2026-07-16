using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using System.Linq;

namespace FlashingHtmlHudFix
{
    [MinimumApiVersion(247)]
    public partial class FlashingHtmlHudFix : BasePlugin
    {
        public override string ModuleName => "FlashingHtmlHudFix";
        public override string ModuleVersion => "1.2_Toggleable";
        public override string ModuleAuthor => "Deana (Modified for LiteMatch)";
        public override string ModuleDescription => "A Plugin that fixes Html Hud. (Toggleable Switch)";

        private CCSGameRules? _gameRules;
        private bool _gameRulesInitialized;

        // 【新增】：防閃機制的總開關，預設為「關閉 (false)」
        public bool IsFixActive = false;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnTick>(OnTick);
            RegisterListener<Listeners.OnMapStart>(OnMapStartHandler);

            // 【新增】：註冊一個給 LiteMatchManager 呼叫的遙控器指令
            AddCommand("css_hud_fix_toggle", "Toggle HTML fix", OnToggleCommand);

            if (hotReload)
            {
                InitializeGameRules();
            }
        }

        private void OnToggleCommand(CCSPlayerController? player, CommandInfo info)
        {
            // 限制只能由伺服器後台呼叫，玩家不能用
            if (player != null) return; 

            if (info.ArgCount >= 2)
            {
                IsFixActive = info.GetArg(1) == "1";
            }
        }

        private void OnMapStartHandler(string mapName)
        {
            _gameRules = null;
            _gameRulesInitialized = false;
            IsFixActive = false; // 換地圖時自動重置開關
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
            // 【核心改動】：如果開關沒開，就直接跳出，把畫面控制權還給官方！
            if (!IsFixActive) return;

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

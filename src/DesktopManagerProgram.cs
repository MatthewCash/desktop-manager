using System;

namespace DesktopManager {
    static class Program {
        static void Main() {
            Config.LoadConfig();

            Keybinds.RegisterKeybinds();

            Config.TaskbarConfig primaryTaskbar;
            if (Config.GetConfig().taskbars.TryGetValue("primary", out primaryTaskbar)) {
                Taskbar taskbar = new Taskbar(
                    true,
                    null,
                    (WindowAccentState.AccentState) primaryTaskbar.accentState,
                    primaryTaskbar.hideStart,
                    primaryTaskbar.clockToStart
                );
                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            Config.TaskbarConfig secondaryTaskbar;
            if (Config.GetConfig().taskbars.TryGetValue("secondary", out secondaryTaskbar)) {
                var sPos = secondaryTaskbar.position;
                var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(sPos[0], sPos[1], sPos[2], sPos[3]);
                Taskbar taskbar = new Taskbar(
                    false,
                    secondaryTaskbarPosition,
                    (WindowAccentState.AccentState) secondaryTaskbar.accentState,
                    secondaryTaskbar.hideStart,
                    secondaryTaskbar.clockToStart
                );
                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            foreach (var item in Config.GetConfig().monitors) {
                MonitorPosition.SetMonitorPosition(item.Key, item.Value[0], item.Value[1]);
            }

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}

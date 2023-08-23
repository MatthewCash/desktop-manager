using System;

namespace DesktopManager {
    static class Program {
        static void Main() {
            Config.LoadConfig();
            var taskbarConfig = Config.GetConfig().taskbars;

            if (taskbarConfig.TryGetValue("primary", out var primaryTaskbar)) {
                Taskbar taskbar = new(
                    true,
                    null,
                    (WindowAccentState.AccentState) primaryTaskbar.accentState,
                    primaryTaskbar.hideStart,
                    primaryTaskbar.clockToStart
                );
                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            if (taskbarConfig.TryGetValue("secondary", out var secondaryTaskbar)) {
                var sPos = secondaryTaskbar.position;
                var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(sPos[0], sPos[1], sPos[2], sPos[3]);
                Taskbar taskbar = new(
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

            Keybinds.handlers.AddRange(new Keybinds.IKeybindHandler[] {
                new WindowManagement(),
                new VoicemeeterEq()
            });
            Keybinds.RegisterKeybinds();

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}

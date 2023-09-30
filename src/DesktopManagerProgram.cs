using System;

namespace DesktopManager {
    static class Program {
        static void Main() {
            Config.LoadConfig();
            var taskbarConfig = Config.GetConfig().Taskbars;

            if (taskbarConfig.TryGetValue("primary", out var primaryTaskbar)) {
                Taskbar taskbar = new(
                    true,
                    null,
                    (WindowAccentState.AccentState) primaryTaskbar.AccentState,
                    primaryTaskbar.HideStart,
                    primaryTaskbar.ClockToStart
                );
                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            if (taskbarConfig.TryGetValue("secondary", out var secondaryTaskbar)) {
                var sPos = secondaryTaskbar.Position;
                var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(sPos[0], sPos[1], sPos[2], sPos[3]);
                Taskbar taskbar = new(
                    false,
                    secondaryTaskbarPosition,
                    (WindowAccentState.AccentState) secondaryTaskbar.AccentState,
                    secondaryTaskbar.HideStart,
                    secondaryTaskbar.ClockToStart
                );
                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            foreach (var item in Config.GetConfig().Monitors) {
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

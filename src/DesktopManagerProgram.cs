using System;

namespace DesktopManager {
    static class Program {
        static void Main(string[] args) {
            Config.LoadConfig();

            if (args.Length > 0 && args[0] == "--print-monitors") {
                MonitorPosition.PrintMonitors();
            }

            var taskbarConfigs = Config.GetConfig().Taskbars;

            foreach (var taskbarConfig in taskbarConfigs) {
                var position = taskbarConfig.Position is null ? null : new TaskbarPosition.TaskbarRect(taskbarConfig.Position);

                Taskbar taskbar = new(
                    taskbarConfig.MonitorIndex == -1,
                    (uint) taskbarConfig.MonitorIndex,
                    position,
                    (WindowAccentState.AccentState) taskbarConfig.AccentState,
                    taskbarConfig.HideStart,
                    taskbarConfig.ClockToStart
                );

                taskbar.FixTaskbar();
                taskbar.RegisterEvents();
            }

            foreach ((uint key, var position) in Config.GetConfig().Monitors) {
                MonitorPosition.SetMonitorPosition(key, position[0], position[1]);
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

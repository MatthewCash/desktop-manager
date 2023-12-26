using System;
using System.Threading;

namespace DesktopManager {
    static class Program {
        static void Main(string[] args) {
            Config.LoadConfig();

            if (args.Length > 0 && args[0] == "--print-monitors") {
                Console.WriteLine(string.Join('\n', MonitorPosition.GetMonitors()));
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

            MonitorPosition.SetAllMonitorPositions();

            Keybinds.Handlers.AddRange(new Keybinds.IKeybindHandler[] {
                new WindowManagement(),
                new VoicemeeterEq()
            });
            Keybinds.RegisterKeybinds();

            Thread trayIconThread = new(TrayIcon.Create);
            trayIconThread.Start();

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}

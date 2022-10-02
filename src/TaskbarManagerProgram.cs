using System;

namespace TaskBarManager {
    static class Program {
        static void Main() {
            Config.LoadConfig();

            Taskbar mainTaskbar = new Taskbar(true, null, TaskbarAccent.AccentState.ACCENT_DISABLED);
            mainTaskbar.FixTaskbar();
            mainTaskbar.RegisterEvents();

            var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(2560, 1400, 1913, 40);
            Taskbar secondaryTaskbar = new Taskbar(false, secondaryTaskbarPosition, TaskbarAccent.AccentState.ACCENT_ENABLE_TRANSPARANT);
            secondaryTaskbar.FixTaskbar();
            secondaryTaskbar.RegisterEvents();

            // MonitorPosition.GetMonitors(); // This prints out monitors and their ids to the console
            MonitorPosition.SetMonitorPosition(1, 2560, 1440 - 2160);

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}
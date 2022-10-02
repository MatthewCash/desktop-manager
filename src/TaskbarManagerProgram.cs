using System;

namespace TaskBarManager {
    static class Program {
        static void Main() {
            Config.LoadConfig();

            var taskbarConfig = Config.GetConfig().taskbars;

            Taskbar mainTaskbar = new Taskbar(true, null, (TaskbarAccent.AccentState) taskbarConfig.primaryAccentState);
            mainTaskbar.FixTaskbar();
            mainTaskbar.RegisterEvents();

            var sPos = taskbarConfig.secondaryPosition;
            var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(sPos[0], sPos[1], sPos[2], sPos[3]);
            Taskbar secondaryTaskbar = new Taskbar(false, secondaryTaskbarPosition, (TaskbarAccent.AccentState) taskbarConfig.secondaryAccentState);
            secondaryTaskbar.FixTaskbar();
            secondaryTaskbar.RegisterEvents();
            
            foreach(var item in Config.GetConfig().monitors) {
                MonitorPosition.SetMonitorPosition(item.Key, item.Value[0], item.Value[1]);
            }

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}
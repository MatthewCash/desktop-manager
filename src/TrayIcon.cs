using System;
using System.Drawing;
using System.Windows.Forms;

static class TrayIcon {
    private static NotifyIcon trayIcon;

    public static void Create() {
        trayIcon = new() {
            Icon = SystemIcons.Application, // TODO: get an actual icon?
            Text = "Desktop Manager",
            Visible = true,
            ContextMenuStrip = new()
        };
        var items = trayIcon.ContextMenuStrip.Items;

        items.Add(new ToolStripMenuItem("Reload Config", null, ReloadConfig));
        items.Add(new ToolStripMenuItem("Print Monitors", null, PrintMonitors));
        items.Add(new ToolStripSeparator());
        items.Add(new ToolStripMenuItem("Fix Taskbars", null, FixTaskbars));
        items.Add(new ToolStripMenuItem("Reposition Monitors", null, RepositionMonitors));
        items.Add(new ToolStripMenuItem("Reconnect VoiceMeeter", null, ConnectVoicemeeter));
        items.Add(new ToolStripSeparator());
        items.Add(new ToolStripMenuItem("Exit", null, Exit));

        Application.Run();
    }

    private static void ReloadConfig(object sender, EventArgs e) {
        Config.LoadConfig();
    }

    private static void PrintMonitors(object sender, EventArgs e) {
        MessageBox.Show(
            string.Join('\n', MonitorPosition.GetMonitors()),
            "Monitors"
        );
    }

    private static void FixTaskbars(object sender, EventArgs e) {
        Taskbar.FixAllTaskbars();
    }

    private static void RepositionMonitors(object sender, EventArgs e) {
        MonitorPosition.SetAllMonitorPositions();
    }

    private static void ConnectVoicemeeter(object sender, EventArgs e) {
        VoicemeeterEq.ConnectVoicemeeter();
    }

    private static void Exit(object sender, EventArgs e) {
        trayIcon.Visible = false;

        Application.Exit();
    }
}

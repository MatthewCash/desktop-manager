using System.Windows.Forms;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;

class VoicemeeterEq : Keybinds.IKeybindHandler {
    private static RemoteApiWrapper vmrApi;

    public VoicemeeterEq() {
        ConnectVoicemeeter();
    }

    public static void ConnectVoicemeeter() {
        vmrApi = new RemoteApiWrapper(PathHelper.GetDllPath());
        vmrApi.Login();
    }

    private static void SetStripEQGains(float gain1, float gain2, float gain3) {
        uint mediaStripId = Config.GetConfig().MediaStripId;

        vmrApi.SetParameter($"Strip[{mediaStripId}].EQGain1", gain1);
        vmrApi.SetParameter($"Strip[{mediaStripId}].EQGain2", gain2);
        vmrApi.SetParameter($"Strip[{mediaStripId}].EQGain3", gain3);
    }

    public void KeyDown(object sender, KeyEventArgs e) {
        if (!Keybinds.TrackedKeyStates[Keys.CapsLock]) return;

        if (e.KeyCode == Keys.D1) {
            SetStripEQGains(12, -12, -12);
            e.Handled = true;
        } else if (e.KeyCode == Keys.D2) {
            SetStripEQGains(-12, 12, 12);
            e.Handled = true;
        } else if (e.KeyCode == Keys.D3) {
            SetStripEQGains(0, 0, 0);
            e.Handled = true;
        }
    }
}

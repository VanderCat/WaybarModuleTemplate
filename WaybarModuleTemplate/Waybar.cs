using System.Runtime.InteropServices;
using System.Text;

namespace WaybarTest;

public unsafe class Waybar {
    /// <summary>
    /// Waybar CFFI object pointer
    /// </summary>
    private IntPtr Object;

    /// <summary>
    /// Waybar version string
    /// </summary>
    public string WaybarVersion;
    private delegate*unmanaged[Cdecl]<IntPtr, IntPtr> _getRootWidget;
    private delegate*unmanaged[Cdecl]<IntPtr, void> _queueUpdate;

    /// <summary>
    /// The waybar widget allocated for this module
    /// </summary>
    public Gtk.Container RootWidget {
        get {
            var handle = _getRootWidget(Object);
            return new Gtk.Container(_getRootWidget(Object));
        }
    }

    public void QueueUpdate() {
        _queueUpdate(Object);
    }

    public Waybar(Interop.InitInfo* info) {
        Object = info->Object;
        WaybarVersion = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(info->WaybarVersion));
        _getRootWidget = info->GetRootWidget;
        _queueUpdate = info->QueueUpdate;
    }
}
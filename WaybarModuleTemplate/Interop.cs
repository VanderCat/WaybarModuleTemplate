using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WaybarTest;

public unsafe class Interop {
    [StructLayout(LayoutKind.Sequential)]
    public struct InitInfo {
        public IntPtr Object;
        public byte* WaybarVersion;
        public delegate*unmanaged[Cdecl]<IntPtr, IntPtr> GetRootWidget;
        public delegate*unmanaged[Cdecl]<IntPtr, void> QueueUpdate;
    }

    public struct ConfigEntry {
        /// <summary>
        /// Entry key
        /// </summary>
        public byte* Key;
        /// <summary>
        /// Entry value
        /// </summary>
        /// <remarks>
        /// <para>
        /// In ABI version 1, this may be either a bare string if the value is a
        /// string, or the JSON representation of any other JSON object as a string.
        /// </para>
        /// <para>
        /// From ABI version 2 onwards, this is always the JSON representation of the
        /// value as a string.
        /// </para>
        /// </remarks>
        public byte* Value;
    }
    
    
    [UnmanagedCallersOnly(EntryPoint = "wbcffi_init", CallConvs = [typeof(CallConvCdecl)])]
    public static IntPtr Init(InitInfo* initInfo, ConfigEntry* configEntries, nint configEntriesLen) {

        var waybar = new Waybar(initInfo);
        var config = new Dictionary<string, string>();
        for (var i = 0; i < configEntriesLen; i++) {
            var curCfg = configEntries[i];
            var key = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(curCfg.Key));
            var value = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(curCfg.Value));
            config[key] = value;

        }
        var module = new WaybarModule();
        var pin = GCHandle.Alloc(module);
        module.Init(waybar, config);
        var array = new char[15];
        

        return GCHandle.ToIntPtr(pin);
    }

    /// Module deinit/delete function, called when Waybar is closed or when the module is removed
    ///
    /// MANDATORY CFFI function
    ///
    /// @param instance Module instance data (as returned by `wbcffi_init`)
    [UnmanagedCallersOnly(EntryPoint = "wbcffi_deinit", CallConvs = [typeof(CallConvCdecl)])]
    public static void Deinit(IntPtr instance) {
        var handle = GCHandle.FromIntPtr(instance);
        if (handle.Target is IDisposable disposable)
            disposable.Dispose();
        handle.Free();
    }
    
    [UnmanagedCallersOnly(EntryPoint = "wbcffi_update", CallConvs = [typeof(CallConvCdecl)])]
    public static void Update(IntPtr instance) {
        var handle = GCHandle.FromIntPtr(instance);
        if (handle.Target is IWaybarModule module)
            module.Update();
    }
    
    [UnmanagedCallersOnly(EntryPoint = "wbcffi_refresh", CallConvs = [typeof(CallConvCdecl)])]
    public static void Refresh(IntPtr instance, int signal) {
        var handle = GCHandle.FromIntPtr(instance);
        if (handle.Target is IWaybarModule module)
            module.Refresh(signal);
    }

    /// Called on module action (see
    /// https://github.com/Alexays/Waybar/wiki/Configuration#module-actions-config)
    ///
    /// Optional CFFI function
    ///
    /// @param instance Module instance data (as returned by `wbcffi_init`)
    /// @param action_name Action name
    [UnmanagedCallersOnly(EntryPoint = "wbcffi_doaction", CallConvs = [typeof(CallConvCdecl)])]
    public static void DoAction(IntPtr instance, byte* actionName) {
        var handle = GCHandle.FromIntPtr(instance);
        if (handle.Target is IWaybarModule module)
            module.DoAction(Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(actionName)));
    }
}
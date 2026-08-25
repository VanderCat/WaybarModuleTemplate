namespace WaybarTest;

public interface IWaybarModule {
    /// <summary>
    /// Module init/new function, called on module instantiation
    /// </summary>
    /// <param name="initInfo">Waybar module information</param>
    /// <param name="config">Flat representation of the module JSON config.</param>
    public void Init(Waybar initInfo, IDictionary<string, string> config);

    /// <summary>
    /// Called from the GTK main event loop, to update the UI
    /// </summary>
    public void Update() { }
    
    /// <summary>
    /// Called when Waybar receives a POSIX signal and forwards it to each module
    /// </summary>
    public void Refresh(int signal) {}
    
    /// <summary>
    /// Called on module action
    /// </summary>
    /// <see href="https://github.com/Alexays/Waybar/wiki/Configuration#module-actions-config"/>
    /// <param name="actionName"></param>
    public void DoAction(string actionName) {}
}
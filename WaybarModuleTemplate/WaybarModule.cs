using System.Text;

namespace WaybarTest;

public class WaybarModule : IWaybarModule, IDisposable {
    public Waybar Module;
    private Gtk.Box _container;
    private Gtk.Button _button;
    public static int InstanceCount = 0;
    public void Init(Waybar initInfo, IDictionary<string, string> config) {
        Console.WriteLine("cffi_example: init config");
        foreach (var kv in config)
            Console.WriteLine("  {0} = {1}", kv.Key, kv.Value);
        
        // Allocate the instance object
        Module = initInfo;

        var root = initInfo.RootWidget;

        // Add a container for displaying the next widgets
        _container = new Gtk.Box(Gtk.Orientation.Horizontal, 5);
        root.Add(_container);
        
        // Add a label
        var label = new Gtk.Label("[Example C# Module:");
        _container.Add(label);

        // Add a button
        _button = Gtk.Button.NewWithLabel("click me !");
        _button.Clicked += (sender, args) => {
            if (sender is Gtk.Button button) {
                var sb = new StringBuilder();
                sb.Append("Dice throw result: ");
                sb.Append(Random.Shared.NextDouble() % 6 + 1);
                button.Label = sb.ToString();
            }
        };
        _container.Add(_button);

        // Add a label
        label = new Gtk.Label("]");
        _container.Add(label);
        
        Console.WriteLine("cffi_example inst={0}: init success ! ({1} total instances)", this, ++InstanceCount);
        // var win = new Gtk.Window();
        // win.Nya();
    }

    public void Refresh(int signal) {
        Console.WriteLine("cffi_example inst={0}: Received refresh signal {1}", this, signal);
    }

    public void Update() {
        Console.WriteLine("cffi_example inst={0}: Update request", this);
    }

    public void DoAction(string actionName) {
        Console.WriteLine("cffi_example inst={0}: doAction({1})\n", this, actionName);
    }

    public void Dispose() {
        Console.WriteLine("cffi_example inst={0}: dispose", this);
    }
}
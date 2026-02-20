using CommandSystem;
using LabApi.Features.Permissions;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;

namespace ProjectMER.Commands.Map;

public class Rebuild : ICommand
{
    public string Command => "rebuild";

    public string[] Aliases => ["rb"];

    public string Description => "Rebuilds a map";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission($"mpr.{Command}"))
        {
            response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "You need to provide a map name!";
            return false;
        }
        
        string mapName = arguments.At(0);
        
        if (!MapUtils.LoadedMaps.TryGetValue(mapName, out MapSchematic map) && !MapUtils.TryGetMapData(mapName, out map)) // Map is already loaded
        {
            response = $"Map named {arguments.At(0)} doesn't exist!";
            return false;
        }
        
        map.Rebuild();

        response = $"Map named {arguments.At(0)} has been successfully rebuild!";
        return true;
    }
}
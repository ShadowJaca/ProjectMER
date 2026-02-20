using Exiled.API.Features;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Lockers;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace ProjectMER.Features.Serializable;

public class MapSchematic
{
	public MapSchematic() { }

	public MapSchematic(string mapName)
	{
		Name = mapName;
	}

	public string Name;

	public bool IsDirty;

	public Dictionary<string, SerializablePrimitive> Primitives { get; set; } = [];

	public Dictionary<string, SerializableLight> Lights { get; set; } = [];

	public Dictionary<string, SerializableDoor> Doors { get; set; } = [];

	public Dictionary<string, SerializableWorkstation> Workstations { get; set; } = [];

	public Dictionary<string, SerializableItemSpawnpoint> ItemSpawnpoints { get; set; } = [];

	public Dictionary<string, SerializablePlayerSpawnpoint> PlayerSpawnpoints { get; set; } = [];

	public Dictionary<string, SerializableCapybara> Capybaras { get; set; } = [];

	public Dictionary<string, SerializableText> Texts { get; set; } = [];

	public Dictionary<string, SerializableInteractable> Interactables { get; set; } = [];

	public Dictionary<string, SerializableScp079Camera> Scp079Cameras { get; set; } = [];

	public Dictionary<string, SerializableShootingTarget> ShootingTargets { get; set; } = [];

	public Dictionary<string, SerializableSchematic> Schematics { get; set; } = [];

	public Dictionary<string, SerializableTeleport> Teleports { get; set; } = [];

	public Dictionary<string, SerializableLocker> Lockers { get; set; } = [];

	public Dictionary<string, SerializableWaypoint> Waypoints { get; set; } = [];

	public List<MapEditorObject> SpawnedObjects = [];

	public MapSchematic Merge(MapSchematic other)
	{
		Primitives.AddRange(other.Primitives);
		Lights.AddRange(other.Lights);
		Doors.AddRange(other.Doors);
		Workstations.AddRange(other.Workstations);
		ItemSpawnpoints.AddRange(other.ItemSpawnpoints);
		PlayerSpawnpoints.AddRange(other.PlayerSpawnpoints);
		Capybaras.AddRange(other.Capybaras);
		Texts.AddRange(other.Texts);
		Interactables.AddRange(other.Interactables);
		Schematics.AddRange(other.Schematics);
		Scp079Cameras.AddRange(other.Scp079Cameras);
		ShootingTargets.AddRange(other.ShootingTargets);
		Teleports.AddRange(other.Teleports);
		Lockers.AddRange(other.Lockers);
		Waypoints.AddRange(other.Waypoints);

		return this;
	}

	public void Reload()
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects)
			mapEditorObject.Destroy();

		SpawnedObjects.Clear();

		Primitives.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Lights.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Doors.ForEach(kVP =>
		{
			Door? vanillaDoor = Door.Get(kVP.Key);
			if (vanillaDoor != null)
			{
				kVP.Value.SetupDoor(vanillaDoor.Base);
				return;
			}

			SpawnObject(kVP.Key, kVP.Value);
		});
		Workstations.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		ItemSpawnpoints.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		PlayerSpawnpoints.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Capybaras.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Texts.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Interactables.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Schematics.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Scp079Cameras.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		ShootingTargets.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Teleports.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Lockers.ForEach(kVP =>
		{
			kVP.Value._prevType = kVP.Value.LockerType;
			SpawnObject(kVP.Key, kVP.Value);
		});
		Waypoints.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
	}

	public void SpawnObject<T>(string id, T serializableObject) where T : SerializableObject
	{
		List<Exiled.API.Features.Room> rooms = serializableObject.GetRooms();
		foreach (Exiled.API.Features.Room room in rooms)
		{
			if (serializableObject.Index < 0 || serializableObject.Index == room.GetRoomIndex())
			{
				GameObject? gameObject = serializableObject.SpawnOrUpdateObject(room);
				if (gameObject == null)
					continue;

				MapEditorObject mapEditorObject = gameObject.AddComponent<MapEditorObject>().Init(serializableObject, Name, id, room);
				SpawnedObjects.Add(mapEditorObject);
			}
		}

		ListPool<Exiled.API.Features.Room>.Shared.Return(rooms);
	}

	public void DestroyObject(string id)
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects.ToList())
		{
			if (mapEditorObject.Id != id)
				continue;

			SpawnedObjects.Remove(mapEditorObject);
			mapEditorObject.Destroy();
		}
	}

	public bool TryAddElement<T>(string id, T serializableObject) where T : SerializableObject
	{
		bool dirtyPrevValue = IsDirty;
		IsDirty = true;

		if (Primitives.TryAdd(id, serializableObject))
			return true;

		if (Lights.TryAdd(id, serializableObject))
			return true;

		if (Doors.TryAdd(id, serializableObject))
			return true;

		if (Workstations.TryAdd(id, serializableObject))
			return true;

		if (ItemSpawnpoints.TryAdd(id, serializableObject))
			return true;

		if (PlayerSpawnpoints.TryAdd(id, serializableObject))
			return true;

		if (Capybaras.TryAdd(id, serializableObject))
			return true;

		if (Texts.TryAdd(id, serializableObject))
			return true;

		if (Interactables.TryAdd(id, serializableObject))
			return true;

		if (Schematics.TryAdd(id, serializableObject))
			return true;

		if (Scp079Cameras.TryAdd(id, serializableObject))
			return true;

		if (ShootingTargets.TryAdd(id, serializableObject))
			return true;

		if (Teleports.TryAdd(id, serializableObject))
			return true;

		if (Lockers.TryAdd(id, serializableObject))
			return true;

		if (Waypoints.TryAdd(id, serializableObject))
			return true;

		IsDirty = dirtyPrevValue;
		return false;
	}

	public bool TryRemoveElement(string id)
	{
		bool dirtyPrevValue = IsDirty;
		IsDirty = true;

		if (Primitives.Remove(id))
			return true;

		if (Lights.Remove(id))
			return true;

		if (Doors.Remove(id))
			return true;

		if (Workstations.Remove(id))
			return true;

		if (ItemSpawnpoints.Remove(id))
			return true;

		if (PlayerSpawnpoints.Remove(id))
			return true;

		if (Capybaras.Remove(id))
			return true;

		if (Texts.Remove(id))
			return true;

		if (Interactables.Remove(id))
			return true;

		if (Schematics.Remove(id))
			return true;

		if (Scp079Cameras.Remove(id))
			return true;

		if (ShootingTargets.Remove(id))
			return true;

		if (Teleports.Remove(id))
			return true;

		if (Lockers.Remove(id))
			return true;

		if (Waypoints.Remove(id))
			return true;

		IsDirty = dirtyPrevValue;
		return false;
	}

	public void Rebuild()
	{
		string originalName = Name;
		foreach (string mapName in MapUtils.LoadedMaps.Keys.ToList())
		{
			MapUtils.UnloadMap(mapName);
		}

		string yaml = YamlParser.Serializer.Serialize(this);
		MapSchematic rebuiltMap = YamlParser.Deserializer.Deserialize<MapSchematic>(yaml);

		rebuiltMap.Name = $"{originalName}_Rebuilt";

		foreach (var obj in rebuiltMap.Primitives.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Lights.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Doors.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Workstations.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.ItemSpawnpoints.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.PlayerSpawnpoints.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Capybaras.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Texts.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Interactables.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Schematics.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Scp079Cameras.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.ShootingTargets.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Teleports.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Lockers.Values) obj.Rebuild();
		foreach (var obj in rebuiltMap.Waypoints.Values) obj.Rebuild();

		string path = Path.Combine(ProjectMER.MapsDir, $"{rebuiltMap.Name}.yml");
		File.WriteAllText(path, YamlParser.Serializer.Serialize(rebuiltMap));
	}
}

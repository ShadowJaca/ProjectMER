using Exiled.API.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using NorthwoodLib.Pools;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Extensions;

public static class RoomExtensions
{
	public static Exiled.API.Features.Room GetRoomAtPosition(Vector3 position)
	{
		Exiled.API.Features.Room room = Exiled.API.Features.Room.Get(position);
		return room ?? Exiled.API.Features.Room.List.First(x => x.Identifier != null && x.Type == RoomType.Surface);
	}

	public static string GetRoomStringId(this Exiled.API.Features.Room room) => $"{room.Zone}_{room.RoomShape}_{room.Name}";

	public static List<Exiled.API.Features.Room> GetRooms(this SerializableObject serializableObject)
	{
		string[] split = serializableObject.Room.Split('_');
		if (split.Length != 3)
			return ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Type == RoomType.Surface));

		FacilityZone facilityZone = (FacilityZone)Enum.Parse(typeof(FacilityZone), split[0], true);
		RoomShape roomShape = (RoomShape)Enum.Parse(typeof(RoomShape), split[1], true);
		RoomName roomName = (RoomName)Enum.Parse(typeof(RoomName), split[2], true);
		return ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Identifier.Zone == facilityZone && x.RoomShape == roomShape && x.RoomName == roomName));
	}

	public static int GetRoomIndex(this Exiled.API.Features.Room room)
	{
		List<Exiled.API.Features.Room> list = ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Zone == room.Zone && x.RoomShape == room.RoomShape && x.Name == room.Name));
		int index = list.IndexOf(room);
		ListPool<Exiled.API.Features.Room>.Shared.Return(list);
		return index;
	}

	public static Vector3 GetAbsolutePosition(this Exiled.API.Features.Room? room, Vector3 position)
	{
		if (room is null || room.Type == RoomType.Surface)
			return position;

		return room.Transform.TransformPoint(position);
	}

	public static Quaternion GetAbsoluteRotation(this Exiled.API.Features.Room? room, Vector3 eulerAngles)
	{
		if (room is null || room.Type == RoomType.Surface)
			return Quaternion.Euler(eulerAngles);

		return room.Transform.rotation * Quaternion.Euler(eulerAngles);
	}
}

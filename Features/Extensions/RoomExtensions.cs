using Exiled.API.Enums;
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

	public static string GetRoomStringId(this Exiled.API.Features.Room room) => $"{room.Zone}_{room.RoomShape}_{room.Type}";

	public static List<Exiled.API.Features.Room> GetRooms(this SerializableObject serializableObject)
	{
		string[] split = serializableObject.Room.Split('_');
		if (split.Length != 3)
			return ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Type == RoomType.Surface));

		FacilityZone facilityZone = (FacilityZone)Enum.Parse(typeof(FacilityZone), split[0], true);
		RoomShape roomShape = (RoomShape)Enum.Parse(typeof(RoomShape), split[1], true);
		RoomType roomType = (RoomType)Enum.Parse(typeof(RoomType), split[2], true);
		return ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Identifier.Zone == facilityZone && x.RoomShape == roomShape && x.Type == roomType));
	}

	public static int GetRoomIndex(this Exiled.API.Features.Room room)
	{
		List<Exiled.API.Features.Room> list = ListPool<Exiled.API.Features.Room>.Shared.Rent(Exiled.API.Features.Room.List.Where(x => x.Identifier != null && x.Zone == room.Zone && x.RoomShape == room.RoomShape && x.Type == room.Type));
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
	
	
	[Obsolete("Use GetRoomAtPosition instead")]
	public static LabApi.Features.Wrappers.Room LabApiGetRoomAtPosition(Vector3 position) => LabApi.Features.Wrappers.Room.TryGetRoomAtPosition(position, out LabApi.Features.Wrappers.Room? room) ? room : LabApi.Features.Wrappers.Room.List.First(x => x.Base != null && x.Name == RoomName.Outside);

	[Obsolete("Use GetRoomStringId instead")]
	public static string LabApiGetRoomStringId(this LabApi.Features.Wrappers.Room room) => $"{room.Zone}_{room.Shape}_{room.Name}";

	[Obsolete("Use GetRooms instead")]
	public static List<LabApi.Features.Wrappers.Room> LabApiGetRooms(this SerializableObject serializableObject)
	{
		string[] split = serializableObject.Room.Split('_');
		if (split.Length != 3)
			return ListPool<LabApi.Features.Wrappers.Room>.Shared.Rent(LabApi.Features.Wrappers.Room.List.Where(x => x.Base != null && x.Name == RoomName.Outside));

		FacilityZone facilityZone = (FacilityZone)Enum.Parse(typeof(FacilityZone), split[0], true);
		RoomShape roomShape = (RoomShape)Enum.Parse(typeof(RoomShape), split[1], true);
		RoomName roomName = (RoomName)Enum.Parse(typeof(RoomName), split[2], true);

		return ListPool<LabApi.Features.Wrappers.Room>.Shared.Rent(LabApi.Features.Wrappers.Room.List.Where(x => x.Base != null && x.Zone == facilityZone && x.Shape == roomShape && x.Name == roomName));
	}

	[Obsolete("Use GetRoomIndex instead")]
	public static int LabApiGetRoomIndex(this LabApi.Features.Wrappers.Room room)
	{
		List<LabApi.Features.Wrappers.Room> list = ListPool<LabApi.Features.Wrappers.Room>.Shared.Rent(LabApi.Features.Wrappers.Room.List.Where(x => x.Base != null && x.Zone == room.Zone && x.Shape == room.Shape && x.Name == room.Name));
		int index = list.IndexOf(room);
		ListPool<LabApi.Features.Wrappers.Room>.Shared.Return(list);
		return index;
	}

	[Obsolete("Use GetAbsolutePosition instead")]
	public static Vector3 LabApiGetAbsolutePosition(this LabApi.Features.Wrappers.Room? room, Vector3 position)
	{
		if (room is null || room.Name == RoomName.Outside)
			return position;

		return room.Transform.TransformPoint(position);
	}

	[Obsolete("Use GetAbsoluteRotation instead")]
	public static Quaternion LabApiGetAbsoluteRotation(this LabApi.Features.Wrappers.Room? room, Vector3 eulerAngles)
	{
		if (room is null || room.Name == RoomName.Outside)
			return Quaternion.Euler(eulerAngles);

		return room.Transform.rotation * Quaternion.Euler(eulerAngles);
	}
}

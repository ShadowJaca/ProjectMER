using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializablePrefab: SerializableObject
{
    public PrefabType PrefabType { get; set; } = PrefabType.SimpleBoxesOpenConnector;
    
	public override GameObject? SpawnOrUpdateObject(Exiled.API.Features.Room? room = null, GameObject? instance = null)
	{
		GameObject prefab;
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		
		if (RequiresReloading && instance != null)
		{
			MapEditorObject mapEditorObject = instance.GetComponent<MapEditorObject>();
			prefab = UnityEngine.Object.Instantiate(Prefab, position, rotation);
			prefab.AddComponent<MapEditorObject>().Init(this, mapEditorObject.MapName, mapEditorObject.Id, mapEditorObject.Room);
		}
		
		else if (instance == null)
			prefab = UnityEngine.Object.Instantiate(Prefab, position, rotation);

		else
		{
			prefab = instance;
		}
		

		_prevIndex = Index;

		prefab.transform.SetPositionAndRotation(position, rotation);
		prefab.transform.localScale = Scale;

		_prevType = PrefabType;
		NetworkServer.UnSpawn(prefab.gameObject);
		NetworkServer.Spawn(prefab.gameObject);
		

		return prefab.gameObject;
	}



	private GameObject Prefab
	{
		get
		{
			GameObject prefab = PrefabType switch
			{
				PrefabType.SimpleBoxesOpenConnector => PrefabManager.SimpleBoxesOpenConnector,
				PrefabType.PipesShortOpenConnector => PrefabManager.PipesShortOpenConnector,
				PrefabType.BoxesLadderOpenConnector => PrefabManager.BoxesLadderOpenConnector,
				PrefabType.TankSupportedShelfOpenConnector => PrefabManager.TankSupportedShelfOpenConnector,
				PrefabType.AngledFencesOpenConnector => PrefabManager.AngledFencesOpenConnector,
				PrefabType.HugeOrangePipesOpenConnector => PrefabManager.HugeOrangePipesOpenConnector,
				PrefabType.PipesLongOpenConnector => PrefabManager.PipesLongOpenConnector,
				PrefabType.Sinkhole => PrefabManager.Sinkhole,
				PrefabType.TantrumObjBrownCandy => PrefabManager.TantrumObjBrownCandy,
				_ => throw new InvalidOperationException(),
			};

			return prefab;
		}
	}

	public override bool RequiresReloading => _prevType != PrefabType;

	internal PrefabType _prevType = PrefabType.SimpleBoxesOpenConnector;
	
}
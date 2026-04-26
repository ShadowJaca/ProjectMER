using System;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class TeleportObject : MonoBehaviour
{
	private void Start()
	{
		_mapEditorObject = GetComponent<MapEditorObject>();
		Base = (SerializableTeleport)_mapEditorObject.Base;
	}

	public SerializableTeleport Base;
	private MapEditorObject _mapEditorObject;
	[Obsolete("Use NextTimeUseForPlayer instead")]
	public DateTime NextTimeUse;
	public Dictionary<Player, DateTime> NextTimeUseForPlayer = new Dictionary<Player, DateTime>();

	public TeleportObject? GetRandomTarget()
	{
		string targetId = Base.Targets.RandomItem();

		foreach (TeleportObject teleportObject in FindObjectsByType<TeleportObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (teleportObject._mapEditorObject.Id != targetId)
				continue;

			return teleportObject;
		}

		return null;
	}

	public void OnTriggerEnter(Collider other)
	{
		Player? player = Player.Get(other.gameObject);
		if (player is null)
			return;

		if (NextTimeUseForPlayer.TryGetValue(player, out DateTime nextTimeUse) && nextTimeUse > DateTime.Now)
			return;

		TeleportObject? target = GetRandomTarget();
		if (target == null)
			return;

		DateTime dateTime = DateTime.Now.AddSeconds(Base.Cooldown);
		NextTimeUseForPlayer[player] = dateTime;
		target.NextTimeUseForPlayer[player] = dateTime;

		player.Position = target.gameObject.transform.position;
		player.LookRotation = target.gameObject.transform.eulerAngles;
	}
}

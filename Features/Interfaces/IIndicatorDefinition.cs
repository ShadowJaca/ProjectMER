using LabApi.Features.Wrappers;
using UnityEngine;

namespace ProjectMER.Features.Interfaces;

public interface IIndicatorDefinition
{
	public GameObject SpawnOrUpdateIndicator(Exiled.API.Features.Room room, GameObject? instance = null);
}

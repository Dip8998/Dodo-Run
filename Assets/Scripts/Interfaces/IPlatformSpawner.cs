using UnityEngine;

public interface IPlatformSpawner
{
    DodoRun.Platform.PlatformController CreatePlatform(Vector3 spawnPos);
}
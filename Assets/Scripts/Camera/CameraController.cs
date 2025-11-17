using UnityEngine;
using Unity.Cinemachine;
using DodoRun.Main;

namespace DodoRun.Camera
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineCamera playerCamera;

        private void Awake()
        {
            playerCamera = GetComponent<CinemachineCamera>();
            GameService.Instance.EventService.OnPlayerSpawned.AddListner(BindToPlayer);
        }

        private void OnDestroy() => GameService.Instance?.EventService?.OnPlayerSpawned?.RemoveListner(BindToPlayer);

        private void BindToPlayer(Transform player)
        {
            var target = playerCamera.Target;
            target.TrackingTarget = player;
            target.LookAtTarget = player;
            playerCamera.Target = target;
        }
    }
}

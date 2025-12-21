using DodoRun.Main;
using UnityEngine;

namespace DodoRun.Camera
{
    public sealed class CameraView : MonoBehaviour
    {
        private CameraController controller;

        [SerializeField] private Vector3 offset;
        [SerializeField] private float followSmooth;
        [SerializeField] private float rotationSmooth;

        private void Awake()
        {
            controller = new CameraController(
                transform, offset, followSmooth, rotationSmooth
            );

            GameService.Instance.EventService.OnPlayerSpawned
                .AddListner(controller.Bind);
        }

        private void LateUpdate() => controller.Tick();
    }
}

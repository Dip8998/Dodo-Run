using UnityEngine;
using DodoRun.Main;

namespace DodoRun.Camera
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 playerOffset = new(0f, 2.2f, -4.8f);
        [SerializeField] private float followSmoothness = 0.08f;
        [SerializeField] private float rotationSmoothness = 8f;

        private Transform target;
        private Transform cachedTransform;
        private Vector3 velocity;
        private Quaternion forwardRotation;

        private void Awake()
        {
            cachedTransform = transform;
            forwardRotation = Quaternion.LookRotation(Vector3.forward);

            GameService.Instance.EventService.OnPlayerSpawned.AddListner(Bind);
        }

        private void OnDestroy()
        {
            GameService.Instance?.EventService?.OnPlayerSpawned.RemoveListner(Bind);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            cachedTransform.position = Vector3.SmoothDamp(
                cachedTransform.position,
                target.position + playerOffset,
                ref velocity,
                followSmoothness
            );

            cachedTransform.rotation = Quaternion.Slerp(
                cachedTransform.rotation,
                forwardRotation,
                rotationSmoothness * Time.deltaTime
            );
        }

        private void Bind(Transform player) => target = player;
    }
}

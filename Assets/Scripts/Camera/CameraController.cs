using UnityEngine;
using DodoRun.Main;

namespace DodoRun.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 playerOffset = new Vector3(0f, 2.2f, -4.8f);
        [SerializeField] private float followSmoothness = 0.08f;
        [SerializeField] private float rotationSmoothness = 8f;

        private Transform currentTarget;
        private Transform cachedTransform;

        private Vector3 velocity = Vector3.zero;
        private Quaternion forwardLookRotation;

        private void Awake()
        {
            cachedTransform = transform;
            forwardLookRotation = Quaternion.LookRotation(Vector3.forward);

            GameService.Instance.EventService.OnPlayerSpawned.AddListner(BindToPlayer);
        }

        private void OnDestroy()
        {
            GameService.Instance?.EventService?.OnPlayerSpawned?.RemoveListner(BindToPlayer);
        }

        private void LateUpdate()
        {
            if (currentTarget == null) return;

            Vector3 targetPos = currentTarget.position + playerOffset;
            cachedTransform.position = Vector3.SmoothDamp(
                cachedTransform.position,
                targetPos,
                ref velocity,
                followSmoothness
            );

            cachedTransform.rotation = Quaternion.Slerp(
                cachedTransform.rotation,
                forwardLookRotation,
                rotationSmoothness * Time.deltaTime
            );
        }

        private void BindToPlayer(Transform player)
        {
            currentTarget = player;
        }
    }
}

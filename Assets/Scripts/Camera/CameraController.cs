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
        private Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            GameService.Instance.EventService.OnPlayerSpawned.AddListner(BindToPlayer);
        }

        private void OnDestroy() =>
            GameService.Instance?.EventService?.OnPlayerSpawned?.RemoveListner(BindToPlayer);

        private void LateUpdate()
        {
            if (currentTarget == null) return;

            Vector3 desiredPos = currentTarget.position + playerOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, followSmoothness);

            Quaternion desiredRot = Quaternion.LookRotation(Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationSmoothness * Time.deltaTime);
        }

        private void BindToPlayer(Transform player)
        {
            currentTarget = player;
        }
    }
}

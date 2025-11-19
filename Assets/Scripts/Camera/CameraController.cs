using UnityEngine;
using DodoRun.Main;

namespace DodoRun.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 playerOffset;
        [SerializeField] private float followSmoothness;
        [SerializeField] private float roatationSmoothness;

        private Transform currentTarget;
        private Vector3 velocity = Vector3.zero;


        private void Awake()
        {
            GameService.Instance.EventService.OnPlayerSpawned.AddListner(BindToPlayer);
        }

        private void OnDestroy() => GameService.Instance?.EventService?.OnPlayerSpawned?.RemoveListner(BindToPlayer);

        private void LateUpdate()
        {
            if(currentTarget == null) return;

            Vector3 desiredPos = currentTarget.position + playerOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, followSmoothness);

            Quaternion desiredRot = Quaternion.LookRotation(currentTarget.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, roatationSmoothness * Time.deltaTime);
        }

        private void BindToPlayer(Transform player)
        {
            currentTarget = player;
        }
    }
}

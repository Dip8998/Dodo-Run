using UnityEngine;

namespace DodoRun.Camera
{
    public sealed class CameraController
    {
        private readonly Transform camera;
        private Transform target;
        private Vector3 velocity;
        private readonly Vector3 offset;
        private readonly float followSmooth;
        private readonly float rotationSmooth;
        private readonly Quaternion forwardRotation;

        public CameraController(
            Transform camera,
            Vector3 offset,
            float followSmooth,
            float rotationSmooth)
        {
            this.camera = camera;
            this.offset = offset;
            this.followSmooth = followSmooth;
            this.rotationSmooth = rotationSmooth;
            forwardRotation = Quaternion.LookRotation(Vector3.forward);
        }

        public void Bind(Transform target) => this.target = target;

        public void Tick()
        {
            if (target == null) return;

            camera.position = Vector3.SmoothDamp(
                camera.position,
                target.position + offset,
                ref velocity,
                followSmooth
            );

            camera.rotation = Quaternion.Slerp(
                camera.rotation,
                forwardRotation,
                rotationSmooth * Time.deltaTime
            );
        }
    }
}

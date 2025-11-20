using DodoRun.Main;
using UnityEngine;
using UnityEngine.UIElements;

namespace DodoRun.Platform
{
    public class PlatformController
    {
        private PlatformScriptableObject platformData;
        public PlatformView platformView {get; private set;}
        private Rigidbody rigidbody;
        private bool hasSpawnedNext = false;
        private bool isDestroyed = false;

        public PlatformController(PlatformScriptableObject platformScriptableObject, Vector3 spawnPos)
        {
            platformData = platformScriptableObject;
            SetupView(spawnPos);
        }

        private void SetupView(Vector3 spawnPos)
        {
            platformView = Object.Instantiate(platformData.Platform, spawnPos, Quaternion.identity);
            rigidbody = platformView.GetComponent<Rigidbody>();
            platformView.SetController(this);
        }

        public void UpdatePlatform()
        {
            if (isDestroyed || rigidbody == null) return;
            platformView.transform.position += new Vector3(0, 0, -platformData.MoveSpeed * Time.deltaTime);
        }

        public void ResetPlatform(Vector3 spawnPos)
        {
            isDestroyed = false;
            hasSpawnedNext = false;

            Collider col = platformView.GetComponent<Collider>();
            col.enabled = false;

            platformView.transform.position = spawnPos;

            platformView.gameObject.SetActive(true);

            col.enabled = true;
        }

        public void HandleCollision(Collider collider)
        {
            if (collider.gameObject.CompareTag("Create") && !hasSpawnedNext)
            {
                hasSpawnedNext = true;

                float length = platformData.PlatformLength;

                Vector3 spawnPos = new Vector3(
                    platformView.transform.position.x,
                    platformView.transform.position.y,
                    platformView.transform.position.z + length - 0.2f
                    );

                PlatformController controller = GameService.Instance.PlatformService.CreatePlatform(spawnPos);
            }   

            if (collider.gameObject.CompareTag("Destroy"))
            {
                isDestroyed = true;
                GameService.Instance.PlatformService.ReturnPlatformToPool(this);
                platformView.gameObject.SetActive(false);
                return;
            }
        }
    }
}

using UnityEngine;

namespace DodoRun.Shared
{
    public static class WorldMover
    {
        public static void Move(Transform t, float speed)
        {
            t.position += Vector3.back * speed * Time.deltaTime;
        }
    }
}

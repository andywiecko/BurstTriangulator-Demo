using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.BurstTriangulator.Demo
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        private Camera camera;

        [SerializeField]
        private float2 minMaxSize = new(0.01f, 1);

        [SerializeField]
        private float wheelSpeed = 10;

        [SerializeField]
        private float moveSpeed = 1;

        [SerializeField]
        private float speedup = 3;

        [SerializeField]
        private float2 xrange = new(0.05f, 0.9f);

        [SerializeField]
        private float2 yrange = new(-0.7f, -0.3f);

        float size;
        float3 x0, home;

        private void Start()
        {
            camera = GetComponent<Camera>();
            home = camera.transform.position;
            size = camera.orthographicSize;
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            var dm = Input.GetAxis("Mouse ScrollWheel");
            dm = dm > 0 ? 1 : dm < 0 ? -1 : 0;
            if (dm != 0)
            {
                size = math.clamp(camera.orthographicSize + wheelSpeed * dm * dt, minMaxSize.x, minMaxSize.y);
                camera.orthographicSize = size;
            }

            if (Input.GetMouseButtonDown(2))
            {
                x0 = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                var x1 = (float3)Input.mousePosition;
                var n = math.normalizesafe(x1 - x0);
                var speed = moveSpeed * size * (Input.GetKey(KeyCode.LeftShift) ? speedup : 1);
                var p = camera.transform.position + (Vector3)(n * speed * dt);

                p.x = p.x < xrange.x ? xrange.x : p.x;
                p.x = p.x > xrange.y ? xrange.y : p.x;
                p.y = p.y < yrange.x ? yrange.x : p.y;
                p.y = p.y > yrange.y ? yrange.y : p.y;

                camera.transform.position = p;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                camera.transform.position = home;
            }
        }
    }
}
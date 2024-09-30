using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.BurstTriangulator.Demo
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TriangulatorComponent : MonoBehaviour
    {
        public TriangulationSettings Settings => triangulator.Settings;
        public OutputData<double2> Output => triangulator.Output;

        private NativeArray<double2> positions;
        private NativeArray<int> constraints;
        private Triangulator triangulator;
        private Mesh mesh;

        public void Start()
        {
            positions = new(Lake.Points, Allocator.Persistent);
            constraints = new(Lake.Constraints, Allocator.Persistent);
            triangulator = new Triangulator(Allocator.Persistent)
            {
                Input = { Positions = positions, ConstraintEdges = constraints },
            };

            GetComponent<MeshFilter>().mesh = mesh = new Mesh();

            Run();
        }

        public void OnDestroy()
        {
            positions.Dispose();
            constraints.Dispose();
            triangulator.Dispose();
        }

        public void Run()
        {
            triangulator.Run();

            var count = triangulator.Output.Triangles.Length;
            var vertices = new NativeArray<float3>(count, Allocator.Persistent);
            var bar = new NativeArray<float3>(count, Allocator.Persistent);
            var triangles = new NativeArray<int>(count, Allocator.Persistent);

            new CreateMeshDataJob
            {
                triangles = triangulator.Output.Triangles.AsReadOnly(),
                positions = triangulator.Output.Positions.AsReadOnly(),
                vertices = vertices,
                meshTriangles = triangles,
                bar = bar
            }.Run();

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetUVs(1, bar);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();

            bar.Dispose();
            vertices.Dispose();
            triangles.Dispose();
        }

        [BurstCompile]
        private struct CreateMeshDataJob : IJob
        {
            public NativeArray<int>.ReadOnly triangles;
            public NativeArray<double2>.ReadOnly positions;

            public NativeArray<float3> vertices, bar;
            public NativeArray<int> meshTriangles;

            public void Execute()
            {
                for (int i = 0; i < triangles.Length; i++)
                {
                    vertices[i] = new((float2)positions[triangles[i]], 0);
                    meshTriangles[i] = i;
                }

                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    bar[3 * i + 0] = new(1, 0, 0);
                    bar[3 * i + 1] = new(0, 1, 0);
                    bar[3 * i + 2] = new(0, 0, 1);
                }
            }
        }
    }
}
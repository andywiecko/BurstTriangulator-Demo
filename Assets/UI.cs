using UnityEngine;
using UnityEngine.UIElements;

namespace andywiecko.BurstTriangulator.Demo
{
    [RequireComponent(typeof(UIDocument))]
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private TriangulatorComponent target = default;

        private Label trianglesCount, pointsCount;

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            var refine = root.Q<Toggle>("refine");
            var area = root.Q<Slider>("area");
            var angle = root.Q<Slider>("angle");

            target.Settings.RefinementThresholds.Area = area.value * area.value;
            target.Settings.RefinementThresholds.Angle = angle.value;

            area.SetEnabled(refine.value);
            angle.SetEnabled(refine.value);
            refine.RegisterValueChangedCallback(e =>
            {
                area.SetEnabled(e.newValue);
                angle.SetEnabled(e.newValue);
                target.Settings.RefineMesh = e.newValue;
                Run();
            });

            root.Q<Toggle>("holes").RegisterValueChangedCallback(e =>
            {
                target.Settings.AutoHolesAndBoundary = e.newValue;
                Run();
            });

            area.RegisterValueChangedCallback(e =>
            {
                var v = e.newValue;
                target.Settings.RefinementThresholds.Area = v * v;
                Run();
            });

            angle.RegisterValueChangedCallback(e =>
            {
                target.Settings.RefinementThresholds.Angle = e.newValue;
                Run();
            });

            trianglesCount = root.Q<VisualElement>("stats-triangles").Q<Label>("count");
            trianglesCount.text = (target.Output.Triangles.Length / 3).ToString();
            pointsCount = root.Q<VisualElement>("stats-points").Q<Label>("count");
            pointsCount.text = target.Output.Positions.Length.ToString();
        }

        private void Run()
        {
            target.Run();
            trianglesCount.text = (target.Output.Triangles.Length / 3).ToString();
            pointsCount.text = target.Output.Positions.Length.ToString();
        }
    }
}
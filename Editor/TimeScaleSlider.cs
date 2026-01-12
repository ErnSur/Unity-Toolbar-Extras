namespace QuickEye.ToolbarExtras
{
    using UnityEditor.Toolbars;
    using UnityEngine;

    public static class TimeScaleSlider
    {
        private const string Path = "Play Mode/Time Scale";
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 2f;
        private const float Unit = 1f / 100f;

        [MainToolbarElement(Path, defaultDockPosition = MainToolbarDockPosition.Middle, menuPriority = 0)]
        public static MainToolbarElement TimeSlider()
        {
            var content = new MainToolbarContent("Time Scale", "Time Scale");

            MainToolbarSlider slider = new(
                content,
                Time.timeScale / Unit,
                MinTimeScale / Unit,
                MaxTimeScale / Unit,
                OnSliderValueChanged,
                true
            );

            slider.populateContextMenu = static menu =>
            {
                menu.ClearItems();

                menu.InsertAction(
                    0,
                    "Reset",
                    (action) =>
                    {
                        Time.timeScale = 1f;
                        MainToolbar.Refresh(Path);
                    }
                );
            };

            return slider;
        }

        private static void OnSliderValueChanged(float newValue)
        {
            Time.timeScale = newValue * Unit;
        }
    }
}
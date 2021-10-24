using System.Collections.Generic;
using System.Linq;
using Domain.Core.ColorSchemes;
using Pastel;
using Domain.Core.Scenes;

namespace DynamicEditor.Core
{
    public sealed class DeveloperMonitor
    {
        public IColorScheme ColorScheme { get; set; }
        private IEnumerable<string> _monitor;
        private readonly IScene _scene;
        private int _topOffset;
        private int _positionFromTop;
        private int _positionFromLeft;
        private ulong _renderTime;
        private ulong _renderedFrames;
        private ulong _renderTimeAcc;
        private int _avgRenderTime;
        private const int Padding = 1;
        private const int AvgFramesCount = 5;
        private const string StartPointer = "$->";
        private const string EndPointer = "<-$";

        public DeveloperMonitor(int topOffset, int positionFromTop,
            int positionFromLeft, IScene scene, IColorScheme colorScheme)
        {
            ColorScheme = colorScheme;
            _monitor = new List<string>();
            _scene = scene;
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = 0;
            _avgRenderTime = 0;
            _renderedFrames = 0;
            _renderTimeAcc = 0;
        }

        public void TurnOn()
        {
            _scene.OnSceneUpdated += AddMonitor;
            _scene.OnSceneUpdated += ColorizeMonitor;
        }

        public void TurnOff()
        {
            _scene.OnSceneUpdated -= AddMonitor;
            _scene.OnSceneUpdated -= ColorizeMonitor;
        }

        public void Update(int topOffset, int positionFromTop, int positionFromLeft, ulong renderTime)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            
            _renderTime = renderTime;
            _renderTimeAcc += renderTime;
            _renderedFrames++;
            
            if (_renderedFrames == AvgFramesCount)
            {
                _avgRenderTime = (int) _renderTimeAcc / AvgFramesCount;
                _renderTimeAcc = 0;
                _renderedFrames = 0;
            }
        }

        private void AddMonitor(List<string> sceneContent)
        {
            SetMonitor();
            var index = Padding;

            foreach (var monitorLine in _monitor)
            {
                var sceneLine = sceneContent[index];
                var right = sceneLine.Length - monitorLine.Length + StartPointer.Length + EndPointer.Length;

                sceneContent[index] = sceneLine[..right] + monitorLine;
                index++;
            }
        }

        private void SetMonitor()
        {
            var monitor = new List<string>
            {
                $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]",
                $" Left: [ pos: {_positionFromLeft} ]",
                $" Render [ avg: {_avgRenderTime}ms; last: {_renderTime}ms ]"
            };

            var longestLine = monitor
                .OrderByDescending(l => l.Length)
                .ToArray()[0];

            string GetSpacesForShorterLine(string longestLine, string line)
                => new(' ', longestLine.Length - line.Length + 1);

            string GroupLine(string line)
                => StartPointer + line + GetSpacesForShorterLine(longestLine, line) + EndPointer;

            _monitor = monitor.Select(GroupLine);
        }

        private void ColorizeMonitor(List<string> sceneContent)
        {
            var start = Padding;
            var end = _monitor.Count() + start;

            for (var i = start; i < end; i++)
            {
                var startPointer = "$->";
                var endPointer = "<-$";
                var line = sceneContent[i];
                var parts = line.Split(startPointer);
                var result = parts[0] + string.Join("", parts[1..]
                    .Select(l => l
                        .Replace(endPointer, string.Empty)
                        .Pastel(ColorScheme.DeveloperMonitorForeground)
                        .PastelBg(ColorScheme.DeveloperMonitorBackground)));

                sceneContent[i] = result;
            }
        }
    }
}
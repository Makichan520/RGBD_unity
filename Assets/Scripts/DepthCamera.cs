using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Perception.GroundTruth;
using Unity.Simulation;


namespace Perc6d
{
    public class DepthCamera: MonoBehaviour
    {
#if UNITY_2019_3_OR_NEWER
        public NameGenerator       _nameGenerator;
#endif
        public CaptureImageEncoder.ImageFormat _imageFormat = CaptureImageEncoder.ImageFormat.Jpg;
        public float               _screenCaptureInterval = 1.0f;
        public GraphicsFormat      _format = GraphicsFormat.R8G8B8A8_UNorm;

        float                      _elapsedTime;
        string                     _baseDirectory;
        public Camera              _camera;

        public CaptureTriggerMode captureTriggerMode = CaptureTriggerMode.Scheduled;

        private bool _shouldCapture = false;
        
        void Start()
        {
            _baseDirectory = Manager.Instance.GetDirectoryFor(DataCapturePaths.ScreenCapture);
            if (_camera != null && _camera.depthTextureMode == DepthTextureMode.None)
                _camera.depthTextureMode = DepthTextureMode.Depth;
        }
        
        void LateUpdate()
        {   
            _elapsedTime += Time.deltaTime;
            if (captureTriggerMode.Equals(CaptureTriggerMode.Scheduled) && _elapsedTime > _screenCaptureInterval)
            {
                _elapsedTime -= _screenCaptureInterval;

                CaptureFrame();
            } else if (captureTriggerMode.Equals(CaptureTriggerMode.Manual) && _shouldCapture)
            {
                _shouldCapture = false;
                CaptureFrame();
            }
        }

        private void CaptureFrame()
        {
            if (Application.isBatchMode && _camera.targetTexture == null)
            {
                _camera.targetTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, _format);
            }

            string path = "";
#if UNITY_2019_3_OR_NEWER
            if (_nameGenerator != null)
                path = _nameGenerator.Generate(Path.Combine(_baseDirectory,
                    $"{_camera.name}.{_imageFormat.ToString().ToLower()}"));
            else
#endif
                path = Path.Combine(_baseDirectory,
                    _camera.name + "_depth_" + Time.frameCount + "." + _imageFormat.ToString().ToLower());

            CaptureCamera.CaptureDepthToFile
            (
                _camera,
                _format,
                path,
                _imageFormat
            );

            if (!_camera.enabled)
                _camera.Render();

        }

        void OnValidate()
        {
            // Automatically add the camera component if there is one on this game object.
            if (_camera == null)
                _camera = GetComponent<Camera>();
        }
        
        public void RequestCapture()
        {
            if (captureTriggerMode.Equals(CaptureTriggerMode.Manual))
            {
                _shouldCapture = true;
            }
            else
            {
                Debug.LogError($"{nameof(RequestCapture)} can only be used if the camera is in " +
                               $"{nameof(CaptureTriggerMode.Manual)} capture mode.");
            }
        }
        
    }
}

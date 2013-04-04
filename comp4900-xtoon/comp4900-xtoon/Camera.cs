using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace comp4900_xtoon
{
    class Camera
    {
        public Vector3 Position;
        public Vector3 LookAt;
        private Vector3 _upVector;

        private Matrix _rotationMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private float _cameraYaw;
        private float _cameraRoll;
        private float _cameraPitch;
        private float _aspectRatio;

        public const int NEAR_CLIPPING_DISTANCE = 100;
        public const int FAR_CLIPPING_DISTANCE = 10000;

        public Camera(Viewport viewport)
        {
            _aspectRatio = (float)viewport.Width / (float)viewport.Height;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, 
                                                                    NEAR_CLIPPING_DISTANCE, FAR_CLIPPING_DISTANCE);
            Position = new Vector3(3000, 3000, 0);
            LookAt = new Vector3(0, 1500, 0);
            _upVector = Vector3.Up;

            _cameraYaw = 1.0f;
            _cameraRoll = 0.0f;
            _cameraPitch = 0.0f;
        }

        public float CameraYaw
        {
            get { return _cameraYaw; }
            set { _cameraYaw = value; }
        }

        public float CameraRoll
        {
            get { return _cameraRoll; }
            set { _cameraRoll = value; }
        }

        public float CameraPitch
        {
            get { return _cameraPitch; }
            set { _cameraPitch = value; }
        }

        public Matrix RotationMatrix
        {
            get { return _rotationMatrix; }
        }

        public Matrix ViewMatrix
        {
            get { return _viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }

        public void Update()
        {
            _rotationMatrix = Matrix.CreateRotationY(_cameraYaw) * Matrix.CreateRotationX(_cameraRoll) * Matrix.CreateRotationZ(_cameraPitch);
            _viewMatrix = Matrix.CreateLookAt(Position, LookAt, _upVector);
        }
    }
}

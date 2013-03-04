using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace comp4900_xtoon
{
    class Camera
    {
        public Vector3 Position;
        private Vector3 _lookAt;
        private Vector3 _upVector;

        private Matrix _rotationMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private float _cameraYaw;
        private float _aspectRatio;

        private const int NEAR_CLIPPING_DISTANCE = 1000;
        private const int FAR_CLIPPING_DISTANCE = 10000;

        public Camera(Viewport viewport)
        {
            _aspectRatio = (float)viewport.Width / (float)viewport.Height;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, 
                                                                    NEAR_CLIPPING_DISTANCE, FAR_CLIPPING_DISTANCE);
            Position = new Vector3(3000, 3000, 0);
            _lookAt = new Vector3(0, 1500, 0);
            _upVector = Vector3.Up;

            _cameraYaw = 1.0f;
        }

        public float CameraYaw
        {
            get { return _cameraYaw; }
            set { _cameraYaw = value; }
        } 

        public Vector3 LookAt
        {
            get { return _lookAt; }
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
            _rotationMatrix = Matrix.CreateRotationY(_cameraYaw);
            //Vector3 transformedReference = Vector3.Transform(new Vector3(0, 0, -1), _rotationMatrix);
            //_lookAt = Position + transformedReference;
            _viewMatrix = Matrix.CreateLookAt(Position, _lookAt, _upVector);
        }
    }
}

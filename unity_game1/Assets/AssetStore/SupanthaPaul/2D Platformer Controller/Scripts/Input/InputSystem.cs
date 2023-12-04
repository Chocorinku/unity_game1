using UnityEngine;

namespace SupanthaPaul {
    public class InputSystem : MonoBehaviour {

        public FloatingJoystick FloatingJoystick;
        static readonly string HorizontalInput = "Horizontal";
        static readonly string JumpInput = "Jump";
        static readonly string DashInput = "Dash";
        static readonly string VerticalInput = "Vertical";
        public PlayerController plyerController;
        public float moveValue = 0.4f;

        private void Start() {
            
            if (plyerController == null) {
                Debug.Log("plyerController is null");
                plyerController = GameObject.FindWithTag(Tags.Player).GetComponent<PlayerController>();
            } 
                 
        }

        void Update() {
            float horizontalInput = Input.GetAxisRaw(HorizontalInput);
            float verticalInput = Input.GetAxisRaw(VerticalInput);

            if (horizontalInput != 0f || Mathf.Abs(FloatingJoystick.Horizontal) > moveValue) {
                plyerController.SetMoveInput(horizontalInput != 0f ? horizontalInput : Mathf.Sign(FloatingJoystick.Horizontal));
            } else {
                plyerController.SetMoveInput(0f);
            }

            if (verticalInput != 0f || Mathf.Abs(FloatingJoystick.Vertical) > moveValue) {
                plyerController.SetUpDownInput(verticalInput != 0f ? verticalInput : FloatingJoystick.Vertical);
            } else {
                plyerController.SetUpDownInput(0f);
            }
        }

        public static float HorizontalRaw() {
            return Input.GetAxisRaw(HorizontalInput);
        }

        public static bool Jump() {
            return Input.GetButtonDown(JumpInput);
        }

        public static bool Dash() {
            return Input.GetButtonDown(DashInput);
        }

        public static float VerticalRaw() {
            return Input.GetAxisRaw(VerticalInput);
        }
    }
}

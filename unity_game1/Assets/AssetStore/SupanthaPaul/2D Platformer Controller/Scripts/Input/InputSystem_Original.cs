using UnityEngine;

namespace SupanthaPaul {
    public class InputSystem_Original : MonoBehaviour {
        // input string caching
        static readonly string HorizontalInput = "Horizontal";
        static readonly string JumpInput = "Jump";
        static readonly string DashInput = "Dash";
        static readonly string VerticalInput = "Vertical";

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

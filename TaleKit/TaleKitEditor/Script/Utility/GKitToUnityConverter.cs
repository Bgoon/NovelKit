using GKitForWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UColor = UnityEngine.Color;
using UVector2 = UnityEngine.Vector2;

namespace TaleKitEditor.Utility {
	public static class GKitToUnityConverter {

	public static Vector2 ToVector2(UVector2 value) {
			return new Vector2(value.x, value.y);
	}
		public static UVector2 ToUVector2(this Vector2 value) {
			return new UVector2(value.x, value.y);
		}

		public static UColor ToUColor(this Color color) {
			return new UColor(ByteToValue01(color.R), ByteToValue01(color.G), ByteToValue01(color.B), ByteToValue01(color.A));
		}
		public static Color ToColor(this UColor color) {
			return Color.FromArgb(Value01ToByte(color.a), Value01ToByte(color.r), Value01ToByte(color.g), Value01ToByte(color.b));
		}


		private static float ByteToValue01(byte value) {
			return value / 255f;
		}
		public static byte Value01ToByte(float value) {
			return (byte)(value * 255f);
		}
	}
}

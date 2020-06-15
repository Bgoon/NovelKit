using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using GKitForWPF;
using GKitForWPF;

namespace TaleKitEditor.Resources.Shader {
	public class Shader_ColorEditor_H : ShaderEffect {

		private static PixelShader pixelShader;

		static Shader_ColorEditor_H() {
			pixelShader = new PixelShader() {
				UriSource = GResourceUtility.GetUri("Resources/Shader/ColorEditor_H.ps"),
			};
		}
		public Shader_ColorEditor_H() {
			this.PixelShader = pixelShader;
		}
	}
}

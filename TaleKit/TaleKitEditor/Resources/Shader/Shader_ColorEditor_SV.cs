using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using GKit;
using GKit.WPF;

namespace TaleKitEditor.Resources.Shader {
	public class Shader_ColorEditor_SV : ShaderEffect {
		public static readonly DependencyProperty HueProperty = DependencyProperty.Register(nameof(Hue), typeof(double), typeof(Shader_ColorEditor_SV), new UIPropertyMetadata(0d, PixelShaderConstantCallback(1)));

		public double Hue {
			get { 
				return (double)GetValue(HueProperty); 
			} set {
				SetValue(HueProperty, value); 
			}
		}

		private static PixelShader pixelShader;

		static Shader_ColorEditor_SV() {
			pixelShader = new PixelShader() {
				UriSource = GResourceUtility.GetUri("Resources/Shader/ColorEditor_SV.ps"),
			};
		}
		public Shader_ColorEditor_SV() {
			this.PixelShader = pixelShader;

			UpdateShaderValue(HueProperty);
		}
	}
}

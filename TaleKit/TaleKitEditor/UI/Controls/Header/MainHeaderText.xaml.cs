﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaleKitEditor.UI.Controls {
	/// <summary>
	/// MainHeaderText.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainHeaderText : UserControl {
		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(nameof(Text), typeof(string), typeof(MainHeaderText), new PropertyMetadata("Header"));

		public string Text {
			get {
				return (string)GetValue(TextProperty);
			}
			set {
				SetValue(TextProperty, value);
			}
		}

		public MainHeaderText() {
			InitializeComponent();
		}
	}
}

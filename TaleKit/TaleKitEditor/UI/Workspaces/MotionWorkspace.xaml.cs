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
using GKitForWPF;
using GKitForWPF;
using GKitForWPF.Resources;

namespace TaleKitEditor.UI.Workspaces {
	/// <summary>
	/// MotionEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MotionWorkspace : UserControl {
		public MotionWorkspace() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			Init();
		}
		private void Init() {
			EditorContext.CreateFile();
		}
	}
}

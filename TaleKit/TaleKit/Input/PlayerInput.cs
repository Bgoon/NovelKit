using GKit.XInput;
using GKitForUnity;
using System;
using System.Collections.Generic;

namespace TaleKit {
	public static class PlayerInput {
		public class InputKey {
			public readonly List<WinKey> MainKeyList;
			public readonly List<Func<bool>> CustomKeyList;

			public InputKey(params WinKey[] mainKeys) {
				MainKeyList = new List<WinKey>();
				MainKeyList.AddRange(mainKeys);

				CustomKeyList = new List<Func<bool>>();
			}

			public bool Down {
				get; private set;
			}
			public bool Hold {
				get; private set;
			}
			public bool Up {
				get; private set;
			}

			public void UpdateState() {
				this.Down = false;
				this.Up = false;

				bool hold = false;
				XInput.Update();
				foreach (WinKey mainKey in MainKeyList) {
					if (KeyInput.GetKeyHold(mainKey)) {
						hold = true;
						break;
					}
				}
				if (!hold) {
					foreach (Func<bool> customKey in CustomKeyList) {
						if (customKey()) {
							hold = true;
							break;
						}
					}
				}

				if (this.Hold) {
					if (!hold) {
						Up = true;
					}
				} else {
					if (hold) {
						Down = true;
					}
				}
				Hold = hold;
			}
		}

		private static readonly InputKey[] AllKeys;

		//Global Control
		public static readonly InputKey Confirm;
		public static readonly InputKey Cancel;
		public static readonly InputKey Menu;
		public static readonly InputKey MoveLeft;
		public static readonly InputKey MoveRight;
		public static readonly InputKey MoveDown;
		public static readonly InputKey MoveUp;

		//Talk Control
		public static readonly InputKey NextScript;
		public static readonly InputKey BackLog;

		//KeyInputs
		//	확인
		//		PC : Z
		//		XBox : A
		//
		//	취소
		//		PC : X
		//		XBox : B
		//
		//	메뉴
		//		PC : ESC
		//		XBox : Start
		//
		//	상하좌우
		//		PC : Arrow 4키
		//		XBox : DPad
		//
		//	백로그 보기
		//		PC : Tab
		//		XBox : Y


		static PlayerInput() {
			Confirm = new InputKey(WinKey.Z, WinKey.Return);
			Confirm.CustomKeyList.Add(() => { return XInput.FirstPlayer.A.Down; });

			Cancel = new InputKey(WinKey.X, WinKey.Escape);
			Cancel.CustomKeyList.Add(() => { return XInput.FirstPlayer.B.Down; });

			Menu = new InputKey(WinKey.Escape);
			Menu.CustomKeyList.Add(() => { return XInput.FirstPlayer.Start.Down; });

			MoveLeft = new InputKey(WinKey.LeftArrow);
			MoveLeft.CustomKeyList.Add(() => { return XInput.FirstPlayer.DPad_Left.Down; });

			MoveRight = new InputKey(WinKey.RightArrow);
			MoveRight.CustomKeyList.Add(() => { return XInput.FirstPlayer.DPad_Right.Down; });

			MoveDown = new InputKey(WinKey.DownArrow);
			MoveDown.CustomKeyList.Add(() => { return XInput.FirstPlayer.DPad_Down.Down; });

			MoveUp = new InputKey(WinKey.UpArrow);
			MoveUp.CustomKeyList.Add(() => { return XInput.FirstPlayer.DPad_Up.Down; });

			NextScript = new InputKey(WinKey.Z, WinKey.Space, WinKey.Return);
			NextScript.CustomKeyList.Add(() => { return XInput.FirstPlayer.A.Down; });

			BackLog = new InputKey(WinKey.Tab);
			NextScript.CustomKeyList.Add(() => { return XInput.FirstPlayer.Y.Down; });


			AllKeys = new InputKey[] {
				Confirm,
				Cancel,
				Menu,
				MoveLeft,
				MoveRight,
				MoveDown,
				MoveUp,
				NextScript,
				BackLog,
			};
		}

		public static void UpdateStates() {
			foreach (InputKey inputSet in AllKeys) {
				inputSet.UpdateState();
			}
		}
	}
}

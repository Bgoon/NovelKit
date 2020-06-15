using System;

namespace TaleKit {
	public abstract class EventBase {
		public bool IsComplete {
			get; protected set;
		}
		public event Action OnCompleteOnce;

		public abstract void OnStart();
		protected void Complete() {
			IsComplete = true;
			OnCompleteOnce?.Invoke();
			OnCompleteOnce = null;
		}
	}
}

using System;

namespace UnityUtilities.Management {
	public class MonolithicEvent {
		public delegate void MonolithicEventHandler();
		protected event MonolithicEventHandler Event;
		protected object _lock = new object();

		public bool Fired { get; protected set; }

		public static MonolithicEvent operator +(MonolithicEvent m1, MonolithicEventHandler handler) {
			m1.Subscribe(handler);
			return m1;
		}
		
		protected void Subscribe(MonolithicEventHandler handler) {
			//lock (_lock) {
				if (Fired)
					handler();
				else
					Event += handler;
			//}
		}

		public void Subscribe(Action handler) {
			Subscribe(new MonolithicEventHandler(handler));
		}

		protected void Unsubscribe(MonolithicEventHandler handler) {
			//lock (_lock) {
				Event -= handler;
			//}
		}

		public void Unsubscribe(Action handler) {
			Unsubscribe(new MonolithicEventHandler(handler));
		}

		public void Fire() {
			//lock (_lock) {
				if (Fired)
					return;
				Fired = true;
				Event?.Invoke();
			//}
		}

		public void Reset(bool purgeHandlers = false) {
			//lock(_lock)
				Fired = false;
				if(purgeHandlers)
					Event = null;
		}
	}
}
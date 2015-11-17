using System;


namespace A3Utility.Deferred4Unity {
    public class Promise {
        protected Action<bool> action = (result) => {; };


        public Promise() : this(r => {; }) {; }
        public Promise(Action<bool> promise) {
            if(promise == null) { throw new ArgumentNullException("promise", "Promise cannot be null."); }

            this.action = promise;
        }

        public void Fire(bool result) { this.action(result); }

        public static implicit operator Action<bool>(Promise promise) { return (promise == null ? null : promise.action); }
        public static implicit operator Promise(Action<bool> promise) { return new Promise { action = promise }; }
    }

    public class Promise<T> {
        protected Action<bool, T> action = (result, item) => {; };


        public Promise() : this((r, i) => {; }) {; }
        public Promise(Action<bool, T> promise) {
            if(promise == null) { throw new ArgumentNullException("promise", "Promise cannot be null."); }

            this.action = promise;
        }

        public void Fire(bool result, T item) { this.action(result, item); }

        public static implicit operator Action<bool, T>(Promise<T> promise) { return (promise == null ? null : promise.action); }
        public static implicit operator Promise<T>(Action<bool, T> promise) { return new Promise<T> { action = promise }; }
    }
}

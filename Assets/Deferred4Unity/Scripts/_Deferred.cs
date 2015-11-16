using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace A3Utility.Deferred4Unity {
    public class When<T> {
        protected List<Deferred<T>> deferreds = new List<Deferred<T>>();

        protected Action<T[]> done = (loop) => { ; };
        protected Action<T[]> fail = (loop) => { ; };
        protected Action<bool, T[]> finish = (flag, loop) => { ; };
        protected List<T> items = new List<T>();
        protected List<bool> results = new List<bool>();


        public When(Deferred<T> deferred, params Deferred<T>[] others) {
            this.deferreds.Add(deferred);
            foreach(var deff in others) { this.deferreds.Add(deff); }

            this.SetDeferreds();
        }

        public When(When<T> when, params When<T>[] others) {
            this.deferreds.AddRange(when.deferreds);
            foreach(var wh in others) { this.deferreds.AddRange(wh.deferreds); }

            this.SetDeferreds();
        }

        public When<T> Done(Action<T[]> action) {
            this.done = action;
            return this;
        }

        public When<T> Fail(Action<T[]> action) {
            this.fail = action;
            return this;
        }

        public When<T> Finish(Action<bool, T[]> action) {
            this.finish = action;
            return this;
        }

        private void Check() {
            if(this.results.Count < this.deferreds.Count) { return; }

            var items = this.items.ToArray();
            var result = false;
            if(this.results.All(e => e)) {
                this.done(items);
                result = true;
            }
            else {
                this.fail(items);
                result = false;
            }

            this.finish(result, items);
        }

        public void SetDeferreds() {
            this.deferreds.ForEach(e => {
                e.Finish((result, item) => {
                    this.results.Add(result);
                    this.items.Add(item);

                    this.Check();
                });
            });
        }
    }

    public class When {
        protected List<Deferred> deferreds = new List<Deferred>();
        protected Action done = () => { ; };
        protected Action fail = () => { ; };
        protected Action<bool> finish = (flag) => { ; };
        protected List<bool> results = new List<bool>();


        public When(Deferred deferred, params Deferred[] others) {
            this.deferreds.Add(deferred);
            foreach(var deff in others) { this.deferreds.Add(deff); }

            this.SetDeferreds();
        }

        public When(When when, params When[] others) {
            this.deferreds.AddRange(when.deferreds);
            foreach(var wh in others) { this.deferreds.AddRange(wh.deferreds); }

            this.SetDeferreds();
        }

        public When Done(Action action) {
            this.done = action;
            return this;
        }

        public When Fail(Action action) {
            this.fail = action;
            return this;
        }

        public When Finish(Action<bool> action) {
            this.finish = action;
            return this;
        }

        private void Check() {
            if(this.results.Count < this.deferreds.Count) { return; }

            var result = false;
            if(this.results.All(e => e)) {
                this.done();
                result = true;
            }
            else {
                this.fail();
                result = false;
            }

            this.finish(result);
        }

        private void SetDeferreds() {
            this.deferreds.ForEach(e => {
                e.Finish(result => {
                    this.results.Add(result);
                    this.Check();
                });
            });
        }
    }

    public class Promise<T> {
        private Action<bool, T> value = null;


        public Promise() : this((result, item) => { ; }) { ; }
        public Promise(Action<bool, T> promise) { this.value = promise; }

        public void Fire(bool result, T item) { this.value(result, item); }

        public static implicit operator Action<bool, T>(Promise<T> promise) { return (promise == null ? null : promise.value); }
        public static implicit operator Promise<T>(Action<bool, T> promise) { return new Promise<T> { value = promise }; }
    }

    public class Promise {
        private Action<bool> value = null;


        public Promise() : this((result) => { ; }) { ; }
        public Promise(Action<bool> promise) { this.value = promise; }

        public void Fire(bool result) { this.value(result); }
        public static implicit operator Action<bool>(Promise promise) { return (promise == null ? null : promise.value); }
        public static implicit operator Promise(Action<bool> promise) { return new Promise { value = promise }; }
    }

    public class Deferred<T> {
        protected Action<T> done = (loop) => { ; };
        protected Action<T> fail = (loop) => { ; };
        protected Action<bool, T> finish = (flag, loop) => { ; };


        public Deferred(out Promise<T> promise) {
            Action<bool, T> action = (isDone, result) => { this.Promise(isDone, result); };
            promise = action;
        }

        public Deferred(out Action<bool, T> promise) {
            promise = (isDone, result) => { this.Promise(isDone, result); };
        }

        public Deferred<T> Done(Action<T> action) {
            this.done = action;
            return this;
        }

        public Deferred<T> Fail(Action<T> action) {
            this.fail = action;
            return this;
        }

        public Deferred<T> Finish(Action<bool, T> action) {
            this.finish = action;
            return this;
        }

        protected void Promise(bool isDone, T result) {
            if(isDone) { this.done(result); }
            else { this.fail(result); }

            this.finish(isDone, result);
        }
    }

    public class Deferred {
        protected Action done = () => { ; };
        protected Action fail = () => { ; };
        protected Action<bool> finish = (flag) => { ; };


        public Deferred(out Promise promise) {
            Action<bool> action = (bool isDone) => { this.Promise(isDone); };
            promise = action;
        }

        public Deferred(out Action<bool> promise) {
            promise = (isDone) => { this.Promise(isDone); };
        }

        public Deferred Done(Action action) {
            this.done = action;
            return this;
        }

        public Deferred Fail(Action action) {
            this.fail = action;
            return this;
        }

        public Deferred Finish(Action<bool> action) {
            this.finish = action;
            return this;
        }

        protected void Promise(bool isDone) {
            if(isDone) { this.done(); }
            else { this.fail(); }

            this.finish(isDone);
        }
    }
}

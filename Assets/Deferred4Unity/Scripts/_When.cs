using System.Collections.Generic;
using System.Linq;
using System;


namespace A3Utility.Deferred4Unity {
    public class When {
        protected List<Deferred> deferreds = new List<Deferred>();
        protected List<bool> results = new List<bool>();
        protected Action done = () => {; };
        protected Action fail = () => {; };
        protected Action<bool> finish = (result) => {; };


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
        
        protected void Check() {
            if(this.results.Count < this.deferreds.Count) { return; }

            var result = false;
            if(this.results.All(r => r)) {
                this.done();
                result = true;
            }
            else {
                this.fail();
                result = false;
            }

            this.finish(result);
        }

        protected void SetDeferreds() {
            this.deferreds.ForEach(e => {
                e.Finish(r => {
                    this.results.Add(r);

                });
            });
        }
    }

    public class When<T> {
        protected class Result {
            public bool result = false;
            public T item = default(T);


            public Result(bool result, T item) {
                this.result = result;
                this.item = item;
            }
        }

        protected List<Deferred<T>> deferreds = new List<Deferred<T>>();
        protected List<Result> results = new List<Result>();
        protected Action<List<T>> done = (items) => {; };
        protected Action<List<T>> fail = (items) => {; };
        protected Action<bool, List<T>> finish = (result, items) => {; };


        public When(Deferred<T> deferred, params Deferred<T>[] others) {
            this.deferreds.Add(deferred);
            foreach(var deff in others) { this.deferreds.Add(deff); }

            this.SetDeferreds();
        }

        public When(When<T> when, params When<T>[] others) {
            this.deferreds.AddRange(when.deferreds);
            foreach(var deff in others) { this.deferreds.AddRange(deff.deferreds); }

            this.SetDeferreds();
        }

        public When<T> Done(Action<List<T>> action) {
            this.done = action;
            return this;
        }

        public When<T> Fail(Action<List<T>> action) {
            this.fail = action;
            return this;
        }

        public When<T> Finish(Action<bool, List<T>> action) {
            this.finish = action;
            return this;
        }

        protected void Check() {
            if(this.results.Count < this.deferreds.Count) { return; }

            var items = this.results.ConvertAll(r => r.item);
            var result = false;

            if(this.results.All(r => r.result)) {
                this.done(items);
                result = true;
            }
            else {
                this.fail(items);
                result = false;
            }

            this.finish(result, items);
        }

        protected void SetDeferreds() {
            this.deferreds.ForEach(e => {
                e.Finish((result, item) => {
                    this.results.Add(new Result(result, item));
                    this.Check();
                });
            });
        }
    }
}

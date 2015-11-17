using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace A3Utility.Deferred4Unity {
    public class Deferred {
        protected Action done = () => {; };
        protected Action fail = () => {; };
        protected Action<bool> finish = (result) => {; };


        public Deferred(out Promise promise) {
            promise = new Promise(result => this.Promise(result));
        }

        public Deferred(out Action<bool> promise) {
            promise = result => this.Promise(result);
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

        protected void Promise(bool result) {
            if(result) { this.done(); }
            else { this.fail(); }

            this.finish(result);
        }
    }

    public class Deferred<T> {
        protected Action<T> done = (item) => {; };
        protected Action<T> fail = (item) => {; };
        protected Action<bool, T> finish = (result, item) => {; };


        public Deferred(out Promise<T> promise) {
            promise = new Promise<T>((result, item) => this.Promise(result, item));
        }

        public Deferred(out Action<bool, T> promise) {
            promise = (result, item) => this.Promise(result, item);
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

        protected void Promise(bool result, T item) {
            if(result) { this.done(item); }
            else { this.fail(item); }

            this.finish(result, item);
        }
    }
}

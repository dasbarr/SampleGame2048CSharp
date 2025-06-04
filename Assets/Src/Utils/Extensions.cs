using System;
using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine.InputSystem;

namespace SampleGame2048 {
    
    /// <summary>
    /// Various extension methods.
    /// </summary>
    public static class Extensions {
        
        /// <summary>
        /// Random source for list shuffling.
        /// </summary>
        private static readonly Random sShuffleRng = new Random();
        /// <summary>
        /// Creates a new list, shuffles and returns it. Source list will remain untouched.
        /// Source: https://stackoverflow.com/a/1262619
        /// </summary>
        /// <param name="list">List to shuffle.</param>
        /// <typeparam name="T">List element type.</typeparam>
        /// <returns>A new created and shuffled list.</returns>
        public static IList<T> Shuffle<T>(this IList<T> list) {
            var shuffledList = new List<T>(list);
            
            var n = shuffledList.Count;
            while (n > 1) {
                n--;
                var k = sShuffleRng.Next(n + 1);
                var value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            return shuffledList;
        }
        
        /// <summary>
        /// Performs an action on each element of the sequence.
        /// </summary>
        /// <param name="source">Source sequence.</param>
        /// <param name="action">Action that will be performed on each sequence element.</param>
        /// <typeparam name="T">Sequence element type.</typeparam>
        /// <returns>Source sequence.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach(var item in source) {
                action(item);
            }

            return source;
        }

        /// <summary>
        /// Converts input action's 'Performed' event to the observable with appropriate move.
        /// </summary>
        /// <param name="inputAction">
        /// Input action to convert. Should have the same name as one of the Move enum values.
        /// </param>
        /// <returns>Observable related to that input action.</returns>
        public static Observable<Move> ToMoveObservable(this InputAction inputAction)
            => Observable.FromEvent<InputAction.CallbackContext>(
                    h => inputAction.performed += h,
                    h => inputAction.performed -= h
               )
               .Select(_ => Enum.Parse<Move>(inputAction.name));

        /// <summary>
        /// Subscribes a text label to the observable. Label's text will be set to the string representation
        /// of the received data.
        /// </summary>
        /// <param name="source">Observable to subscribe to.</param>
        /// <param name="textLabel">Text label that will be updated on data receiving.</param>
        /// <returns>Subscription disposable.</returns>
        public static IDisposable Subscribe(this Observable<int> source, TMP_Text textLabel) {
            return source.Subscribe(x => textLabel.text = x.ToString());
        }
        
        /// <summary>
        /// Subscribes a recative property to the observable. Property's value will be set to the received data.
        /// </summary>
        /// <param name="source">Observable to subscribe to.</param>
        /// <param name="property">Reactive property that will be updated on data receiving.</param>
        /// <typeparam name="T">Received data type.</typeparam>
        /// <returns>Subscription disposable.</returns>
        public static IDisposable Subscribe<T>(this Observable<T> source, ReactiveProperty<T> property) {
            return source.Subscribe(x => property.Value = x);
        }

        /// <summary>
        /// Converts a HashSet to the readonly version.
        /// </summary>
        /// <param name="source">HashSet to convert to.</param>
        /// <typeparam name="T">HashSet's item type.</typeparam>
        /// <returns>Readonly version of the source HashSet.</returns>
        public static ReadOnlyHashSet<T> AsReadOnly<T>(this HashSet<T> source) => new ReadOnlyHashSet<T>(source);
    }
}

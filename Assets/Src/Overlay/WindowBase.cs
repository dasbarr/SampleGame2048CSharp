using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace SampleGame2048 {
    
    /// <summary>
    /// Base class for all game windows.
    /// </summary>
    public abstract class WindowBase : MonoBehaviour {
        
        //-------------------------------------------------------------
        // Nested
        //-------------------------------------------------------------

        [Serializable]
        public class Settings {
            [Tooltip("Duration of window show/hide animation (in seconds)")]
            [field: SerializeField]
            public float ShowHideAnimationDuration { get; private set; } = 0.4f;

            [Tooltip("Hidden window should have a different Y position for better looking show/hide animation")]
            [field: SerializeField]
            public int HiddenYPosition { get; private set; } = -20;
        }
        
        //-------------------------------------------------------------
        // Dependencies
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class constants
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class variables
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Class methods
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Constructor/finalizer
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Variables
        //-------------------------------------------------------------

        private Settings _settings;

        /// <summary>
        /// Contains window's 'show' or 'hide' animation.
        /// </summary>
        private Sequence _showHideAnimation;
        
        //-------------------------------------------------------------
        // Properties
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Private properties serialized for Unity
        //-------------------------------------------------------------
        
        /// <summary>
        /// Canvas group for changing window opacity during the show/hide animation.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        /// <summary>
        /// Actual window (above the 'shadow overlay').
        /// </summary>
        [SerializeField]
        private Transform _window;
        
        //-------------------------------------------------------------
        // Events
        //-------------------------------------------------------------
        
        //-------------------------------------------------------------
        // Public methods
        //-------------------------------------------------------------

        [Inject]
        public void Construct(Settings settings) {
            _settings = settings;
        }
        
        /// <summary>
        /// Shows the window with animation.
        /// </summary>
        public void Show() {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
            
            // pause all ingame amimations when the window is shown
            Time.timeScale = 0;

            ChangeVisibility(1, true);
        }
        
        //-------------------------------------------------------------
        // Protected methods
        //-------------------------------------------------------------

        /// <summary>
        /// Hides the window.
        /// </summary>
        /// <param name="withAnimation">If true, hide animation will be used. Otherwise window will be hidden instantly.</param>
        protected void Hide(bool withAnimation = true) {
            // window should be non-interactable when hidden
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
            
            // resume all ingame animations
            Time.timeScale = 1;
            
            ChangeVisibility(0, withAnimation);
        }
        
        //-------------------------------------------------------------
        // Private methods
        //-------------------------------------------------------------

        /// <summary>
        /// Changes window visibility.
        /// </summary>
        /// <param name="targetAlpha">Alpha parameter of the window's CanvasGroup will have that value after visibility change.</param>
        /// <param name="withAnimation">If true, animation will be used. Otherwise visibility willl be changed instantly.</param>
        private void ChangeVisibility(float targetAlpha, bool withAnimation) {
            // get rid of the previous animation (if any)
            _showHideAnimation?.Kill();
            _showHideAnimation = null;
            
            // move window to the new position as a part of an animation
            var targetLocalPosition = new Vector3(
                0,
                _settings.HiddenYPosition * (1 - targetAlpha)
            );

            if (withAnimation) {
                // visibility change can be started from any current alpha value, recalculate animation duration accordingly
                var animationPercentToProceed = Math.Abs(targetAlpha - _canvasGroup.alpha);
                var animationDuration = _settings.ShowHideAnimationDuration * animationPercentToProceed;
                
                // prepare the animation
                _showHideAnimation = DOTween.Sequence();
                _showHideAnimation.Insert(
                    0,
                    _canvasGroup.DOFade(targetAlpha, animationDuration).SetEase(Ease.OutSine)
                );
                _showHideAnimation.Insert(
                    0,
                    _window.DOLocalMove(targetLocalPosition, animationDuration).SetEase(Ease.OutSine)
                );

                // animate
                _showHideAnimation
                    .OnComplete(() => _showHideAnimation = null) // remove animation after completion
                    .SetUpdate(true) // window animation should ignore all other animation pausing 
                    .Play();
            }
            else {
                _canvasGroup.alpha = targetAlpha;
                _window.localPosition = targetLocalPosition;
            }
        }
        
        //-------------------------------------------------------------
        // Unity methods
        //-------------------------------------------------------------

        private void Awake() {
            // window should be hidden by default
            Hide(false);
        }
    }
}

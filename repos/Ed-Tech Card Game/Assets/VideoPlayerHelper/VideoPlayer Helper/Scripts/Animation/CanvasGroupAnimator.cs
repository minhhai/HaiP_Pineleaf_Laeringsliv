using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.VideoHelper.Animation
{
    /// <summary>
    /// Animates <see cref="CanvasGroup.alpha"/>.
    /// </summary>
    public class CanvasGroupAnimator : AnimationCurveAnimator
    {
        public CanvasGroup Group;

        public void OnPlay(PointerEventData eventData)
        {
            Animate(In, InDuration, x => Group.alpha = x);
        }

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    Animate(Out, OutDuration, x => Group.alpha = x);
        //}
    }
}

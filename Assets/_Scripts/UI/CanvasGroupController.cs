using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class CanvasGroupController : AOsCore.UI.CanvasGroupController
    {
        public CanvasGroup CanvasGroup { get { return _group; } set { _group = value; } }

        protected override void FadeInAnimation(float animationTime)
        {
            _group.DOFade(0f, animationTime).SetUpdate(true);
        }

        protected override void FadeOutAnimation(float animationTime)
        {
            _group.DOFade(1f, animationTime).SetUpdate(true);
        }
    }
}


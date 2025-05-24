namespace Framework.GUI
{
    using UnityEngine;

    public class UIBase : MonoBehaviour
    {
        private RectTransform _rect;
        public RectTransform Rect
        {
            get
            {
                if (_rect == null) _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }
        public virtual void ShowUI()
        {
            gameObject.Show();
            OnShow();
        }
        public virtual void HideUI()
        {
            gameObject.Hide();
            OnHide();
        }

        protected virtual void OnShow()
        {
            GameEventManager.Instance.StartListening(EventName.OnDataUpdated, OnDataUpdate);
            OnDataUpdate();
        }

        protected virtual void OnHide()
        {
            GameEventManager.Instance.StopListening(EventName.OnDataUpdated, OnDataUpdate);
        }

        protected virtual void OnDataUpdate(object obj = null)
        {

        }
    }
}
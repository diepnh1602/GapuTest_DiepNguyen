namespace Framework.GUI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    public class GUIManager : Singleton<GUIManager>
    {
        [SerializeField] private List<UIBase> uis = new List<UIBase>();
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            uis = canvas.GetComponentsInChildren<UIBase>(true).ToList();

            HideAll();
        }
        public static T ShowOnlyUI<T>() where T : UIBase
        {
            HideAll();
            return ShowUI<T>();
        }

        public static void HideAll()
        {
            foreach (var ui in Instance.uis)
            {
                ui.Hide();
            }
        }

        public static T ShowUI<T>() where T : UIBase
        {
            T ui = GetUI<T>();
            if (ui == null)
            {
                //var prefab = ResourceManager.Instance.GetUIPrefab<T>();
                //if(prefab != null)
                //{
                //    ui = Instantiate(prefab, Instance.canvas.transform);
                //    Instance.UI_Openeds.Add(ui);
                //}
                //else
                //{
                //    return null;
                //}
            }
            ui.ShowUI();
            ui.Rect.localScale = Vector3.one;
            ui.Rect.SetAsLastSibling();
            return ui;
        }

        public static T GetUI<T>() where T : UIBase
        {
            return (T)Instance.uis.Find(x => x as T);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Application;


namespace Application
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textAction;
        [SerializeField] Button button;
        [SerializeField] GameObject selectedGameObject;

        private BaseAction baseAction;
        private UnitActionSystem unitActionSystem;

        private void Awake()
        {
            unitActionSystem = GameManager.Instance.Get<UnitActionSystem>();
        }

        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            textAction.text = baseAction.GetNameAction().ToUpper();

            button.onClick.AddListener(() =>
            {
                unitActionSystem.SetSelectedAction(baseAction);
            });
        }

        public void UpdateSelectedVisual()
        {
            BaseAction selectedAction = unitActionSystem.GetSelectedAction();
            selectedGameObject.SetActive(selectedAction == baseAction);
        }
    }
}

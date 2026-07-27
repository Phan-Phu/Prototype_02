using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textAction;
    [SerializeField] Button button;
    [SerializeField] GameObject selectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textAction.text = baseAction.GetNameAction().ToUpper();

        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedAction == baseAction);
    }
}

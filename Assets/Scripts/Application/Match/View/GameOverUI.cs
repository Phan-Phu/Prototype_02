using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Application
{
    // Temporary game-over screen, built in code so it needs no prefab: a full-screen overlay
    // (which also blocks clicks on the game underneath) with the result and two buttons.
    public class GameOverUI : MonoBehaviour
    {
        private GameObject panelGameObject;
        private TextMeshProUGUI resultText;

        private void Start()
        {
            BuildUI();
            panelGameObject.SetActive(false);

            EventManager.AddListener<GameOverEvent>(OnGameOverEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<GameOverEvent>(OnGameOverEvent);
        }

        private void OnGameOverEvent(GameOverEvent @event)
        {
            resultText.text = @event.IsPlayerWin ? "VICTORY" : "DEFEAT";
            resultText.color = @event.IsPlayerWin ? new Color(0.4f, 1f, 0.4f) : new Color(1f, 0.4f, 0.4f);
            panelGameObject.SetActive(true);
        }

        private void OnRestartButtonClicked()
        {
            MatchSystem.Instance.RestartMatch();
        }

        private void OnMainMenuButtonClicked()
        {
            MatchSystem.Instance.ReturnToMainMenu();
        }

        private void BuildUI()
        {
            GameObject canvasGameObject = new GameObject("GameOverCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGameObject.transform.SetParent(transform, false);

            Canvas canvas = canvasGameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler canvasScaler = canvasGameObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            panelGameObject = CreateUIObject("Panel", canvasGameObject.transform);
            RectTransform panelRect = panelGameObject.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelGameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            resultText = CreateText("ResultText", panelGameObject.transform, "", 120);
            resultText.rectTransform.anchoredPosition = new Vector2(0, 160);
            resultText.rectTransform.sizeDelta = new Vector2(1000, 160);

            CreateButton("RestartButton", panelGameObject.transform, "Restart", new Vector2(0, -40), OnRestartButtonClicked);
            CreateButton("MainMenuButton", panelGameObject.transform, "Main Menu", new Vector2(0, -160), OnMainMenuButtonClicked);
        }

        private GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject uiGameObject = new GameObject(name, typeof(RectTransform));
            uiGameObject.transform.SetParent(parent, false);
            return uiGameObject;
        }

        private TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize)
        {
            TextMeshProUGUI textMesh = CreateUIObject(name, parent).AddComponent<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.raycastTarget = false;
            return textMesh;
        }

        private void CreateButton(string name, Transform parent, string label, Vector2 anchoredPosition, UnityAction onClick)
        {
            GameObject buttonGameObject = CreateUIObject(name, parent);
            RectTransform buttonRect = buttonGameObject.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = anchoredPosition;
            buttonRect.sizeDelta = new Vector2(360, 90);

            Image buttonImage = buttonGameObject.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            Button button = buttonGameObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(onClick);

            TextMeshProUGUI labelText = CreateText("Label", buttonGameObject.transform, label, 48);
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
        }
    }
}

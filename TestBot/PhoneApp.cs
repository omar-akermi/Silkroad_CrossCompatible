#if IL2CPP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#elif MONO
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#endif

using MelonLoader;

namespace SOE
{
    public class MyApp : PhoneApp
    {
        protected override string AppName => "Silkroad";
        protected override string AppTitle => "Silkroad";
        protected override string IconLabel => "Silkroad";
        protected override string IconFileName => "SilkroadIcon.png";

        private RectTransform questListContainer;
        private Text questTitle;
        private Text questTask;
        private Text questReward;
        private Text deliveryStatus;
        private Button acceptButton;

        protected override void BuildUI(GameObject container)
        {
            Transform root = container.transform;
            MelonLogger.Msg("📱 Building Silk Road UI...");

            GameObject bg = UIFactory.Panel("SilkRoad_Background", root, Color.black, fullAnchor: true);
            MelonLogger.Msg("✅ Main panel created");

            // Top bar
            GameObject topBar = UIFactory.Panel("TopBar", bg.transform, new Color(0.15f, 0.15f, 0.15f), new Vector2(0f, 0.93f), new Vector2(1f, 1f));
            UIFactory.Text("AppTitle", "Silk Road", topBar.transform, 26, TextAnchor.MiddleCenter, FontStyle.Bold);

            // LEFT PANEL: Quest list
            GameObject leftPanel = UIFactory.Panel("QuestListPanel", bg.transform, new Color(0.1f, 0.1f, 0.1f), new Vector2(0f, 0f), new Vector2(0.5f, 0.93f));

            GameObject scrollViewGO = new GameObject("ScrollView");
            scrollViewGO.transform.SetParent(leftPanel.transform, false);
            RectTransform scrollRectRT = scrollViewGO.AddComponent<RectTransform>();
            scrollRectRT.anchorMin = Vector2.zero;
            scrollRectRT.anchorMax = Vector2.one;
            scrollRectRT.offsetMin = Vector2.zero;
            scrollRectRT.offsetMax = Vector2.zero;

            ScrollRect scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollViewGO.transform, false);
            RectTransform viewportRT = viewport.AddComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.offsetMin = Vector2.zero;
            viewportRT.offsetMax = Vector2.zero;
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.02f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            scrollRect.viewport = viewportRT;

            GameObject content = new GameObject("QuestListContent");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRT = content.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1f);
            contentRT.anchorMax = new Vector2(1f, 1f);
            contentRT.pivot = new Vector2(0.5f, 1f);
            contentRT.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRT;
            questListContainer = contentRT;

            // RIGHT PANEL: Quest details
            GameObject rightPanel = UIFactory.Panel("QuestDetailPanel", bg.transform, new Color(0.12f, 0.12f, 0.12f), new Vector2(0.5f, 0f), new Vector2(1f, 0.93f));
            VerticalLayoutGroup rightLayout = rightPanel.AddComponent<VerticalLayoutGroup>();
            rightLayout.spacing = 12;
            rightLayout.padding = new RectOffset(10, 10, 10, 10);

            questTitle = UIFactory.Text("QuestTitle", "Select a quest", rightPanel.transform, 22, TextAnchor.UpperLeft, FontStyle.Bold);
            questTask = UIFactory.Text("QuestTask", "Task: --", rightPanel.transform, 18);
            questReward = UIFactory.Text("QuestReward", "Reward: --", rightPanel.transform, 18);
            deliveryStatus = UIFactory.Text("DeliveryStatus", "", rightPanel.transform, 16, TextAnchor.UpperLeft);

            GameObject acceptGO = UIFactory.Button("AcceptButton", "Accept Delivery", rightPanel.transform, new Color(0.2f, 0.6f, 0.2f));
            acceptButton = acceptGO.GetComponent<Button>();

            MelonLogger.Msg("✅ Silk Road UI finished.");

            LoadQuests();
        }

        private void LoadQuests()
        {
            MelonLogger.Msg("📦 Loading quest data (placeholder).");
        }
    }
}

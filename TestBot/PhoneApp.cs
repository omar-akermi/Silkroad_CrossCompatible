#if IL2CPP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Il2CppScheduleOne.Product;

#else
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ScheduleOne.Product;

using System.Collections.Generic;
#endif

using MelonLoader;
using System.Collections.Generic;

namespace SOE
{
    public class MyApp : PhoneApp
    {
        protected override string AppName => "Silkroad";
        protected override string AppTitle => "Silkroad";
        protected override string IconLabel => "Silkroad";
        protected override string IconFileName => "SilkroadIcon.png";

        private List<QuestData> quests;
        private RectTransform questListContainer;
        private Text questTitle;
        private Text questTask;
        private Text questReward;
        private Text deliveryStatus;
        private Button acceptButton;

        protected override void BuildUI(GameObject container)
        {
            Transform root = container.transform;
            MelonLogger.Msg("\ud83d\udcf1 Building Silk Road UI...");

            GameObject bg = UIFactory.Panel("SilkRoad_Background", root, Color.black, fullAnchor: true);
            MelonLogger.Msg("\u2705 Main panel created");

            GameObject topBar = UIFactory.Panel("TopBar", bg.transform, new Color(0.15f, 0.15f, 0.15f), new Vector2(0f, 0.93f), new Vector2(1f, 1f));
            UIFactory.Text("AppTitle", "Silk Road", topBar.transform, 26, TextAnchor.MiddleCenter, FontStyle.Bold);

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

            MelonLogger.Msg("\u2705 Silk Road UI finished.");
            LoadQuests();
        }

        public void LoadQuests()
        {
            if (quests == null)
                quests = new List<QuestData>();

            MelonLogger.Msg("🧠 LoadQuests() with 4 random products");
            quests.Clear();
            System.Random rng = new System.Random();

#if IL2CPP
            var allProducts = ProductManager.Instance.AllProducts;
            List<ProductDefinition> filtered = new List<ProductDefinition>();

            for (int i = 0; i < allProducts.Count; i++)
            {
                var def = allProducts[i];
                if (def != null && !string.IsNullOrEmpty(def.name) && def.Price > 0f)
                {
                    filtered.Add(def);
                }
            }
#else
            var allProducts = ProductManager.Instance.AllProducts;
            List<ProductDefinition> filtered = new List<ProductDefinition>();
            foreach (var def in allProducts)
            {
                if (def != null && !string.IsNullOrEmpty(def.name) && def.Price > 0f)
                {
                    filtered.Add(def);
                }
            }
#endif

            List<ProductDefinition> finalList = new List<ProductDefinition>();
            int count = Mathf.Min(filtered.Count, 4);
            while (finalList.Count < count)
            {
                int index = rng.Next(0, filtered.Count);
                var pick = filtered[index];
                if (!finalList.Contains(pick))
                {
                    finalList.Add(pick);
                }
            }

            foreach (var def in finalList)
            {
                int bricks = rng.Next(10, 20);
                int baseReward = Mathf.RoundToInt(def.Price * 25f * bricks);
                int bonus = UnityEngine.Random.Range(100, 301) * bricks;
                int reward = baseReward + bonus;

                quests.Add(new QuestData
                {
                    Title = $"{def.Name} Delivery",
                    Task = $"Deliver {bricks}x {def.Name} Bricks to the stash.",
                    Reward = reward,
                    ProductID = def.name,
                    AmountRequired = (uint)bricks,
                    TargetObjectName = "GreenTent"
                });
            }

            MelonLogger.Msg($"\ud83d\udce6 Generated {quests.Count} randomized quests.");
            RefreshQuestList();
        }


        private void RefreshQuestList()
        {
            MelonLogger.Msg("🔁 Refreshing quest list...");

            foreach (Transform child in questListContainer)
                GameObject.Destroy(child.gameObject);

            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];

                MelonLogger.Msg($"🆕 Creating icon for quest: {quest.Title}");

                // Find product definition
                ProductDefinition product = null;
                var allProducts = ProductManager.Instance.AllProducts;
                for (int j = 0; j < allProducts.Count; j++)
                {
                    var def = allProducts[j];
                    if (def != null && def.name == quest.ProductID)
                    {
                        product = def;
                        break;
                    }
                }

                if (product == null)
                {
                    MelonLogger.Warning($"❌ Product not found: {quest.ProductID}");
                    continue;
                }

                Sprite iconSprite = product.Icon;
                if (iconSprite == null)
                {
                    MelonLogger.Warning($"⚠️ No icon for {product.name}");
                    continue;
                }

                // Row container
                GameObject row = new GameObject("QuestRow_" + quest.Title);
                row.AddComponent<RectTransform>();
                Image rowBg = row.AddComponent<Image>();
                rowBg.color = new Color(0.15f, 0.15f, 0.15f);
                rowBg.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
                rowBg.type = Image.Type.Sliced;
                row.transform.SetParent(questListContainer, false);


                HorizontalLayoutGroup hLayout = row.AddComponent<HorizontalLayoutGroup>();
                hLayout.spacing = 10;
                hLayout.padding = new RectOffset(50, 10, 5, 5);
                hLayout.childAlignment = TextAnchor.MiddleLeft;
                hLayout.childForceExpandHeight = false;
                hLayout.childForceExpandWidth = false;

                LayoutElement rowLE = row.AddComponent<LayoutElement>();
                rowLE.preferredHeight = 90f;
                rowLE.minHeight = 90f;

                // --- ICON PANEL ---
                GameObject iconGO = new GameObject("QuestIcon_" + quest.Title);
                iconGO.transform.SetParent(row.transform, false);
                RectTransform rt = iconGO.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(90f, 90f);

                Image bg = iconGO.AddComponent<Image>();
                bg.color = new Color(0.15f, 0.15f, 0.15f);
                bg.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
                bg.type = Image.Type.Sliced;

                LayoutElement le = iconGO.AddComponent<LayoutElement>();
                le.preferredHeight = 90f;
                le.minHeight = 90f;
                le.preferredWidth = 90f;

                GameObject icon = new GameObject("Icon");
                icon.AddComponent<RectTransform>();
                icon.AddComponent<Image>(); icon.transform.SetParent(iconGO.transform, false);
                RectTransform iconRT = icon.GetComponent<RectTransform>();
                iconRT.anchorMin = new Vector2(0.15f, 0.15f);
                iconRT.anchorMax = new Vector2(0.85f, 0.85f);
                iconRT.offsetMin = Vector2.zero;
                iconRT.offsetMax = Vector2.zero;

                Image iconImage = icon.GetComponent<Image>();
                iconImage.sprite = iconSprite;
                iconImage.preserveAspect = true;
                iconImage.color = Color.white;

                Outline outline = icon.AddComponent<Outline>();
                outline.effectColor = new Color(1f, 1f, 1f, 0.15f);
                outline.effectDistance = new Vector2(1.5f, -1.5f);

                // IL2CPP-safe button click
                Button btn = iconGO.AddComponent<Button>();
                btn.targetGraphic = bg;

                QuestData clickedQuest = quest; // capture loop variable properly
                void Select() => OnSelectQuest(clickedQuest);
                btn.onClick.AddListener((UnityEngine.Events.UnityAction)Select);

                // --- TEXT PANEL ---
                GameObject textPanel = new GameObject("QuestText");
                textPanel.AddComponent<RectTransform>();
                textPanel.transform.SetParent(row.transform, false);

                RectTransform textRT = textPanel.GetComponent<RectTransform>();
                textRT.sizeDelta = new Vector2(280f, 90f);

                VerticalLayoutGroup vLayout = textPanel.AddComponent<VerticalLayoutGroup>();
                vLayout.spacing = 4;
                vLayout.childAlignment = TextAnchor.MiddleLeft;
                vLayout.childControlHeight = true;
                vLayout.childControlWidth = true;
                vLayout.childForceExpandWidth = false;

                LayoutElement textLE = textPanel.AddComponent<LayoutElement>();
                textLE.minWidth = 200f;
                textLE.flexibleWidth = 1;

                // Title
                UIFactory.Text("QuestTitle", quest.Title, textPanel.transform, 16, TextAnchor.MiddleLeft, FontStyle.Bold);

                // Mafia label
                string mafiaLabel = "Client: Unknown";
                if (product is WeedDefinition) mafiaLabel = "Client: German Mafia";
                else if (product is CocaineDefinition) mafiaLabel = "Client: Canadian Mafia";
                else if (product is MethDefinition) mafiaLabel = "Client: Russian Mafia";

                UIFactory.Text("QuestClient", mafiaLabel, textPanel.transform, 14, TextAnchor.UpperLeft);
            }

            MelonLogger.Msg($"✅ Displayed {quests.Count} quests using in-game product icons.");
        }

        private void OnSelectQuest(QuestData quest)
        {
            MelonLogger.Msg($"\ud83c\udfaf Selected quest: {quest.Title}");
            questTitle.text = quest.Title;
            questTask.text = $"Task: {quest.Task}";
            questReward.text = $"Reward: ${quest.Reward}";

            acceptButton.GetComponentInChildren<Text>().text = "Accept Delivery";
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.interactable = true;

            void OnClick() => AcceptQuest(quest);
            acceptButton.onClick.AddListener((UnityAction)OnClick);
        }

        private void AcceptQuest(QuestData quest)
        {
            MelonLogger.Msg($"\ud83d\ude80 Accepting quest: {quest.Title}");
            deliveryStatus.text = "\ud83d\ude9a Delivery Active";
            acceptButton.interactable = false;
        }
    }
}

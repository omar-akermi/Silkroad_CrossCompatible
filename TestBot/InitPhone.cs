#if IL2CPP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#elif (MONO)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#endif
using System.Collections;
using System.IO;
using System.Linq;
using MelonLoader;
using Object = UnityEngine.Object;
using MelonLoader.Utils;

namespace SOE
{
    public abstract class PhoneApp
    {
        protected GameObject Player;
        protected GameObject AppPanel;
        protected bool AppCreated;
        protected bool IconModified;
        protected bool InitializationStarted;

        protected abstract string AppName { get; }
        protected abstract string AppTitle { get; }
        protected abstract string IconLabel { get; }
        protected abstract string IconFileName { get; }

        protected abstract void BuildUI(GameObject container);

        public void Init(MelonLogger.Instance logger)
        {
            MelonCoroutines.Start(DelayedInit(logger));
        }

        private IEnumerator DelayedInit(MelonLogger.Instance logger)
        {
            yield return new WaitForSeconds(5f);

            Player = GameObject.Find("Player_Local");
            if (Player == null)
            {
                logger.Error("Player_Local not found.");
                yield break;
            }

            GameObject appsCanvas = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas");
            if (appsCanvas == null)
            {
                logger.Error("AppsCanvas not found.");
                yield break;
            }

            Transform existingApp = appsCanvas.transform.Find(AppName);
            if (existingApp != null)
            {
                AppPanel = existingApp.gameObject;
                SetupExistingAppPanel(AppPanel, logger);
            }
            else
            {
                Transform templateApp = appsCanvas.transform.Find("ProductManagerApp");
                if (templateApp == null)
                {
                    logger.Error("Template ProductManagerApp not found.");
                    yield break;
                }

                AppPanel = Object.Instantiate(templateApp.gameObject, appsCanvas.transform);
                AppPanel.name = AppName;

                Transform containerTransform = AppPanel.transform.Find("Container");
                if (containerTransform != null)
                {
                    GameObject container = containerTransform.gameObject;
                    ClearContainer(container);
                    BuildUI(container);
                }

                AppCreated = true;
            }

            AppPanel.SetActive(false);

            if (!IconModified)
            {
                IconModified = ModifyAppIcon(IconLabel, IconFileName, logger);
                if (IconModified)
                    logger.Msg("Icon modified.");
            }
        }

        private void SetupExistingAppPanel(GameObject panel, MelonLogger.Instance logger)
        {
            Transform containerTransform = panel.transform.Find("Container");
            if (containerTransform != null)
            {
                GameObject container = containerTransform.gameObject;
                if (container.transform.childCount < 2)
                {
                    ClearContainer(container);
                    BuildUI(container);
                }
            }

            AppCreated = true;
        }

        private void ClearContainer(GameObject container)
        {
            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(container.transform.GetChild(i).gameObject);
            }
        }

        private bool ModifyAppIcon(string labelText, string fileName, MelonLogger.Instance logger)
        {
            GameObject parent = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons/");
            if (parent == null)
            {
                logger?.Error("AppIcons not found.");
                return false;
            }

            Transform lastIcon = parent.transform.childCount > 0 ? parent.transform.GetChild(parent.transform.childCount - 1) : null;
            if (lastIcon == null)
            {
                logger?.Error("No icon found to clone.");
                return false;
            }

            GameObject iconObj = lastIcon.gameObject;
            iconObj.name = AppName;

            Transform labelTransform = iconObj.transform.Find("Label");
            Text label = labelTransform?.GetComponent<Text>();
            if (label != null) label.text = labelText;

            return ChangeAppIconImage(iconObj, fileName, logger);
        }

        private bool ChangeAppIconImage(GameObject iconObj, string filename, MelonLogger.Instance logger)
        {
            Transform imageTransform = iconObj.transform.Find("Mask/Image");
            Image image = imageTransform?.GetComponent<Image>();
            if (image == null)
            {
                logger?.Error("Image component not found in icon.");
                return false;
            }

            string path = Path.Combine(MelonEnvironment.UserDataDirectory, filename);
            if (!File.Exists(path))
            {
                logger?.Error("Icon file not found: " + path);
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                if (ImageConversion.LoadImage(tex, bytes))
                {
                    image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    return true;
                }
                Object.Destroy(tex);
            }
            catch (System.Exception e)
            {
                logger?.Error("Failed to load image: " + e.Message);
            }

            return false;
        }
    }
}

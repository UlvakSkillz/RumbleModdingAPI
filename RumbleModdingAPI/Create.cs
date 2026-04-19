//Is there more I can supply here?

using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppTMPro;
using UnityEngine;

namespace RumbleModdingAPI.RMAPI
{
    public class Create
    {
        public static GameObject newTextGameObject;
        public static GameObject newButtonGameObject;

        /// <summary>
        /// Creates and returns a new Text GameObject
        /// </summary>
        public static GameObject NewText()
        {
            GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
            newTextGO.SetActive(true);
            newTextGO.GetComponent<TextMeshPro>().autoSizeTextContainer = true;
            return newTextGO;
        }

        /// <summary>
        /// Creates and returns a new Text GameObject with the specified Text, size, color, position, and rotation
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textSize"></param>
        /// <param name="textColor"></param>
        /// <param name="textPosition"></param>
        /// <param name="textRotation"></param>
        public static GameObject NewText(string text, float textSize, Color textColor, Vector3 textPosition, Quaternion textRotation)
        {
            GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
            newTextGO.SetActive(true);
            TextMeshPro tmp = newTextGO.GetComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = textSize;
            tmp.color = textColor;
            tmp.autoSizeTextContainer = true;
            newTextGO.transform.position = textPosition;
            newTextGO.transform.rotation = textRotation;
            return newTextGO;
        }

        /// <summary>
        /// Creates and returns a new Button
        /// </summary>
        public static GameObject NewButton()
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.SetActive(true);
            return newButtonGO;
        }

        /// <summary>
        /// Creates and returns a new Button at the Specified Position/Rotation
        /// </summary>
        /// <param name="buttonPosition"></param>
        /// <param name="buttonRotation"></param>
        public static GameObject NewButton(Vector3 buttonPosition, Quaternion buttonRotation)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.transform.position = buttonPosition;
            newButtonGO.transform.rotation = buttonRotation;
            newButtonGO.SetActive(true);
            return newButtonGO;
        }

        /// <summary>
        /// Creates and returns a new Button with a Listener of the supplied Action
        /// </summary>
        /// <param name="action"></param>
        public static GameObject NewButton(Action action)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.SetActive(true);
            newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
            return newButtonGO;
        }

        /// <summary>
        /// Creates and returns a new Button at the Specified Position/Rotation with a Listener of the supplied Action
        /// </summary>
        /// <param name="buttonPosition"></param>
        /// <param name="buttonRotation"></param>
        /// <param name="action"></param>
        public static GameObject NewButton(Vector3 buttonPosition, Quaternion buttonRotation, Action action)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.transform.position = buttonPosition;
            newButtonGO.transform.rotation = buttonRotation;
            newButtonGO.SetActive(true);
            newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
            return newButtonGO;
        }

        internal static void SetupAPIItems()
        {
            if (RumbleModdingAPI.parentAPIItems == null)
            {
                RumbleModdingAPI.parentAPIItems = new GameObject();
                RumbleModdingAPI.parentAPIItems.name = "APIItems";
                GameObject.DontDestroyOnLoad(RumbleModdingAPI.parentAPIItems);
            }
            if (newTextGameObject == null)
            {
                newTextGameObject = GameObject.Instantiate(RMAPI.GameObjects.Gym.INTERACTABLES.Leaderboard.PlayerTags.HighscoreTag.Nr.GetGameObject());
                TextMeshPro tmp = RMAPI.Create.newTextGameObject.GetComponent<TextMeshPro>();
                newTextGameObject.name = "NewTextGameObject";
                tmp.text = "new Text";
                tmp.color = Color.black;
                newTextGameObject.SetActive(false);
                newTextGameObject.transform.parent = RumbleModdingAPI.parentAPIItems.transform;
            }
            if (newButtonGameObject == null)
            {
                newButtonGameObject = GameObject.Instantiate(RMAPI.GameObjects.Gym.TUTORIAL.Statictutorials.RUMBLEStarterGuide.NextPageButton.InteractionButton.GetGameObject());
                newButtonGameObject.name = "newButton";
                newButtonGameObject.SetActive(false);
                newButtonGameObject.transform.parent = RumbleModdingAPI.parentAPIItems.transform;
            }
        }
    }
}

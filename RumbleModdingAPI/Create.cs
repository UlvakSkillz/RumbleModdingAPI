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
        public static GameObject NewText()
        {
            GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
            newTextGO.SetActive(true);
            newTextGO.GetComponent<TextMeshPro>().autoSizeTextContainer = true;
            return newTextGO;
        }

        public static GameObject NewText(string text, float textSize, Color textColor, UnityEngine.Vector3 textPosition, Quaternion textRotation)
        {
            GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
            newTextGO.SetActive(true);
            TextMeshPro tmp = newTextGO.GetComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = textSize;
            tmp.color = textColor;
            tmp.autoSizeTextContainer = true;
            newTextGameObject.transform.position = textPosition;
            newTextGameObject.transform.rotation = textRotation;
            return newTextGO;
        }

        public static GameObject NewButton()
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.SetActive(true);
            return newButtonGO;
        }

        public static GameObject NewButton(UnityEngine.Vector3 buttonPosition, Quaternion buttonRotation)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.transform.position = buttonPosition;
            newButtonGO.transform.rotation = buttonRotation;
            newButtonGO.SetActive(true);
            return newButtonGO;
        }

        public static GameObject NewButton(Action action)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.SetActive(true);
            newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
            return newButtonGO;
        }

        public static GameObject NewButton(UnityEngine.Vector3 buttonPosition, Quaternion buttonRotation, Action action)
        {
            GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
            newButtonGO.transform.position = buttonPosition;
            newButtonGO.transform.rotation = buttonRotation;
            newButtonGO.SetActive(true);
            newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
            return newButtonGO;
        }
    }
}

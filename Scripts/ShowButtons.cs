using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowButtons : MonoBehaviour
{
    public GameObject buttonsParent;
    public bool useCanvasGroup = false; 
    public float fadeSpeed = 10f;

    private RectTransform rect;
    private Canvas parentCanvas;
    private Camera uiCamera;
    private CanvasGroup canvasGroup;
    private bool isInside = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect == null) Debug.LogError("[UIAreaShowButtonsRobust] unavaliable");

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null) Debug.LogError("[UIAreaShowButtonsRobust] no parents");

        if (parentCanvas != null)
        {
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                uiCamera = null;
            else
                uiCamera = parentCanvas.worldCamera;
        }

        if (buttonsParent == null)
        {
            buttonsParent = gameObject;
        }

        if (useCanvasGroup)
        {
            canvasGroup = buttonsParent.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = buttonsParent.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            SetChildrenActive(buttonsParent, false);
        }
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        bool nowInside = RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos, uiCamera);

        if (useCanvasGroup)
        {
            float target = nowInside ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
            canvasGroup.blocksRaycasts = canvasGroup.alpha > 0.1f;
        }
        else
        {
            if (nowInside != isInside)
            {
                SetChildrenActive(buttonsParent, nowInside);
            }
        }

        isInside = nowInside;
    }

    private void SetChildrenActive(GameObject parent, bool active)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i).gameObject;
            child.SetActive(active);
        }
    }

    void OnDrawGizmosSelected()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Gizmos.color = Color.cyan;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class RedactionHandler : MonoBehaviour
{
    public GameObject RedactionGameObject;

    List<GameObject> RedactionObjects;

    Redactions Redactions;

    Vector2 localPoint;

    int startPoint;

    public void OnMouseLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Started!");
            startPoint = (int)localPoint.x;
        }

        if (context.canceled)
        {
            Debug.Log($"Cancelled! {startPoint} {(int)localPoint.x}");
            Redactions.AddSection(Section.SectionByEnd(startPoint, (int)localPoint.x));
            foreach (Section section in Redactions.Sections)
            {
                Debug.Log(section);
            }

            DrawSections();
        }
    }

    public void OnMouseRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            List<Section> itemsToRemove = new List<Section>();

            foreach (Section section in Redactions.Sections)
            {
                if (localPoint.x > section.Start && localPoint.x < section.End)
                {
                    itemsToRemove.Add(section);
                }
            }

            foreach (Section section in itemsToRemove)
            {
                Debug.Log("REMOVE REMOVE");
                Redactions.RemoveSection(section);
            }

            DrawSections();
        }
    }

    public void OnPositionUpdate(InputAction.CallbackContext context)
    {
        Vector2 screenPoint = context.ReadValue<Vector2>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, Camera.main, out Vector2 output))
        {
            output += rectTransform.sizeDelta / 2;
            localPoint = output;
        }
        // Debug.Log(localPoint);
        // Vector3 mousePosition = context.ReadValue<Vector2>();
        // Debug.Log(mousePosition);

    }

    void Awake()
    {
        RedactionObjects = new List<GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Redactions = new Redactions();
        Redactions.AddSection(new Section(15, 300));

        DrawSections();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DrawSections()
    {
        ClearSections();

        foreach (Section section in Redactions.Sections)
        {
            DrawSection(section);
        }
    }

    void DrawSection(Section section)
    {
        GameObject instance = Instantiate(RedactionGameObject, this.transform);
        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, section.Length);
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0);
        rectTransform.anchoredPosition = new Vector2(section.Start, 0);

        RedactionObjects.Add(instance);
    }

    void ClearSections()
    {
        foreach (GameObject obj in RedactionObjects)
        {
            Destroy(obj);
        }
        RedactionObjects.Clear();
    }
}

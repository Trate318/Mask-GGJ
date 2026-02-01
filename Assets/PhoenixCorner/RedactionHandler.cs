using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class RedactionHandler : MonoBehaviour
{
    [SerializeField] private InteractableScreen interactableScreen;
    public GameObject RedactionGameObject;

    public RedactionHandler goalHandler;

    List<GameObject> RedactionObjects;

    Redactions Redactions;

    int MaxLength = 150;

    Vector2 localPoint;

    int startPoint;

    public void OnMouseLeft(InputAction.CallbackContext context)
    {
        if (PanningCamera.CurrentPoint != PanningCamera.LookingAt.Computer) return;
        if (context.started)
        {
            Debug.Log("Started!");
            startPoint = (int)localPoint.x;
        }

        if (context.canceled)
        {
            Debug.Log($"Cancelled! {startPoint} {(int)localPoint.x}");
            Section newSection = Section.SectionByEnd(startPoint, (int)localPoint.x);
            newSection.Clamp(MaxLength);

            if (newSection.Length > 0)
            {
                Redactions.AddSection(newSection);
            }

            // Debug.Log($"DIFFERENCE { Redactions.ORLengthDifference(goalHandler.Redactions) }");

            DrawSections();
        }
    }

    public void OnMouseRight(InputAction.CallbackContext context)
    {
        if (PanningCamera.CurrentPoint != PanningCamera.LookingAt.Computer) return;
        
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
                Redactions.RemoveSection(section);
            }

            DrawSections();

            // if (goalHandler != null)
            // {
            //     Redactions result = Redactions.OR(goalHandler.Redactions);

            //     goalHandler.Redactions = result;
            //     goalHandler.DrawSections();
            // }
        }
    }

    public void OnPositionUpdateHELLO(InputAction.CallbackContext context)
    {

        Debug.Log(context.ReadValue<Vector2>());
        Vector2 screenPoint = interactableScreen.normalizedPoint;
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, 
                interactableScreen.screenCamera, out Vector2 output))
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

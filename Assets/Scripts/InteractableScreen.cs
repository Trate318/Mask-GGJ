using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableScreen : MonoBehaviour
{
    List<GameObject> dragTargets = new List<GameObject>();

    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private Camera screenCamera;
    [SerializeField] private GraphicRaycaster screenCaster;

    private RectTransform canvasRectTransform;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private float lerpSpeed = 20;

    private void Awake()
    {
        canvasRectTransform = cursorRectTransform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (PanningCamera.CurrentPoint != PanningCamera.LookingAt.Computer || PanningCamera.ChangingView) {return;}
        
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(mouseRay, out RaycastHit hit, 20, raycastMask, QueryTriggerInteraction.Ignore))
        {
            OnCursorInput(hit.textureCoord);
            DrawCursor(hit.textureCoord);
        }
    }

    private void DrawCursor(Vector2 normalisedPosition)
    {
        Cursor.visible = false;
        Vector3 mousePosition = new Vector3(
            normalisedPosition.x * screenCamera.activeTexture.width,
            normalisedPosition.y * screenCamera.activeTexture.height, 0);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform,
                mousePosition,
                screenCamera,
                out var localPoint))
        {
            Vector3 target = Vector3.Lerp(cursorRectTransform.anchoredPosition, localPoint, Time.deltaTime * lerpSpeed);
            cursorRectTransform.anchoredPosition = target;
            
        }
    }
    
    private void OnCursorInput(Vector2 normalisedPosition)
    {
        Vector3 mousePosition = new Vector3(
            normalisedPosition.x * screenCamera.activeTexture.width,
            normalisedPosition.y * screenCamera.activeTexture.height, 0);

        // construct our pointer event
        PointerEventData mouseEvent = new PointerEventData(EventSystem.current);
        mouseEvent.position = mousePosition;

        // perform a raycast using the graphics raycaster
        List<RaycastResult> results = new List<RaycastResult>();
        screenCaster.Raycast(mouseEvent, results);

        bool mouseDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseUp = Mouse.current.leftButton.wasReleasedThisFrame;
        bool mouseHeld = Mouse.current.leftButton.isPressed;

        // send through end drag events as needed
        if (mouseUp)
        {
            foreach(var target in dragTargets)
            {
                if (ExecuteEvents.Execute(target, mouseEvent, ExecuteEvents.endDragHandler))
                    break;
            }
            dragTargets.Clear();
        }

        // process the raycast results
        foreach(var result in results)
        {
            // setup the new event data
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            eventData.pointerCurrentRaycast = eventData.pointerPressRaycast = result;

            // is the mouse down?
            if (mouseHeld)
                eventData.button = PointerEventData.InputButton.Left;

            var slider = result.gameObject.GetComponentInParent<UnityEngine.UI.Slider>();

            // potentially new drag targets?
            if (mouseDown)
            {
                if (ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.beginDragHandler))
                    dragTargets.Add(result.gameObject);

                if (slider != null)
                {
                    slider.OnInitializePotentialDrag(eventData);

                    if (!dragTargets.Contains(result.gameObject))
                        dragTargets.Add(result.gameObject);
                }
            } // need to update drag target
            else if (dragTargets.Contains(result.gameObject))
            {
                eventData.dragging = true;
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.dragHandler);
                if (slider != null)
                {
                    slider.OnDrag(eventData);
                }
            }

            // send a mouse down event?
            if (mouseDown)
            {
                if (ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerDownHandler))
                    break;
            } // send a mouse up event?
            else if (mouseUp)
            {
                bool didRun = ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerUpHandler);
                didRun |= ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerClickHandler);

                if (didRun)
                    break;
            }
        }
    }
}
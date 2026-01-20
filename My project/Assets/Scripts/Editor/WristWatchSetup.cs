using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Hands;
using TMPro;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

/// <summary>
/// Editor utility to create the complete WristWatch scene setup with one click.
/// Access via menu: GameObject > XR > Complete Wrist Watch Scene Setup
/// </summary>
public class WristWatchSetup : Editor
{
    [MenuItem("GameObject/XR/Complete Wrist Watch Scene Setup", false, 10)]
    public static void CreateCompleteSetup()
    {
        Undo.SetCurrentGroupName("Create Complete Wrist Watch Setup");
        int undoGroup = Undo.GetCurrentGroup();

        // Step 1: Find or create XR Origin
        XROrigin xrOrigin = FindOrCreateXROrigin();
        if (xrOrigin == null)
        {
            Debug.LogError("Failed to create XR Origin. Please add XR Origin manually via GameObject > XR > XR Origin (VR)");
            return;
        }

        // Step 2: Configure camera for passthrough (transparent background)
        Camera mainCamera = ConfigureCamera(xrOrigin);

        // Step 3: Create hand tracking events
        XRHandTrackingEvents handTrackingEvents = CreateHandTrackingEvents(xrOrigin.transform);

        // Step 4: Create wrist watch with all components
        GameObject wristWatch = CreateWristWatch(xrOrigin.transform, handTrackingEvents, mainCamera?.transform);

        Undo.CollapseUndoOperations(undoGroup);

        Selection.activeGameObject = wristWatch;

        Debug.Log(
            "<color=green><b>Wrist Watch Setup Complete!</b></color>\n\n" +
            "Scene hierarchy created:\n" +
            "  XR Origin\n" +
            "    ├── Camera Offset\n" +
            "    │   └── Main Camera (configured for passthrough)\n" +
            "    ├── LeftHandTracking (XRHandTrackingEvents)\n" +
            "    └── WristWatch (fully wired)\n\n" +
            "<b>ONLY MANUAL STEP:</b> Enable passthrough in Project Settings:\n" +
            "  Edit > Project Settings > XR Plug-in Management > OpenXR > Android tab\n" +
            "  Enable: \"Meta Quest: Camera (Passthrough)\"\n\n" +
            "<b>For VR Mode:</b> Enable 'Require Palm Up' in WristTracker component"
        );
    }

    private static XROrigin FindOrCreateXROrigin()
    {
        // Try to find existing XR Origin
        XROrigin existingOrigin = Object.FindFirstObjectByType<XROrigin>();
        if (existingOrigin != null)
        {
            Debug.Log("Found existing XR Origin, using it.");
            return existingOrigin;
        }

        // Create new XR Origin
        GameObject xrOriginObj = new GameObject("XR Origin");
        Undo.RegisterCreatedObjectUndo(xrOriginObj, "Create XR Origin");

        XROrigin xrOrigin = xrOriginObj.AddComponent<XROrigin>();

        // Create Camera Offset
        GameObject cameraOffset = new GameObject("Camera Offset");
        cameraOffset.transform.SetParent(xrOriginObj.transform, false);
        Undo.RegisterCreatedObjectUndo(cameraOffset, "Create Camera Offset");

        // Create Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        cameraObj.transform.SetParent(cameraOffset.transform, false);
        Undo.RegisterCreatedObjectUndo(cameraObj, "Create Main Camera");

        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.AddComponent<AudioListener>();

        // Configure XR Origin
        SerializedObject originSO = new SerializedObject(xrOrigin);
        originSO.FindProperty("m_CameraFloorOffsetObject").objectReferenceValue = cameraOffset;
        originSO.FindProperty("m_Camera").objectReferenceValue = camera;
        originSO.ApplyModifiedProperties();

        // Delete default Main Camera if exists
        GameObject defaultCamera = GameObject.Find("Main Camera");
        if (defaultCamera != null && defaultCamera != cameraObj)
        {
            Undo.DestroyObjectImmediate(defaultCamera);
        }

        return xrOrigin;
    }

    private static Camera ConfigureCamera(XROrigin xrOrigin)
    {
        Camera camera = xrOrigin.Camera;
        if (camera == null)
        {
            camera = Camera.main;
        }

        if (camera != null)
        {
            // Configure for passthrough AR (transparent background)
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0); // Fully transparent
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 1000f;

            EditorUtility.SetDirty(camera);
            Debug.Log("Camera configured for passthrough (transparent background)");
        }

        return camera;
    }

    private static XRHandTrackingEvents CreateHandTrackingEvents(Transform parent)
    {
        // Check if already exists
        XRHandTrackingEvents existing = parent.GetComponentInChildren<XRHandTrackingEvents>();
        if (existing != null)
        {
            Debug.Log("Found existing XRHandTrackingEvents, using it.");
            return existing;
        }

        GameObject handTracking = new GameObject("LeftHandTracking");
        handTracking.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(handTracking, "Create Left Hand Tracking");

        XRHandTrackingEvents events = handTracking.AddComponent<XRHandTrackingEvents>();

        // Set to left hand
        SerializedObject so = new SerializedObject(events);
        so.FindProperty("m_Handedness").enumValueIndex = 0; // 0 = Left
        so.ApplyModifiedProperties();

        return events;
    }

    private static GameObject CreateWristWatch(Transform parent, XRHandTrackingEvents handTrackingEvents, Transform headTransform)
    {
        // Check if already exists
        WristTracker existing = parent.GetComponentInChildren<WristTracker>();
        if (existing != null)
        {
            Debug.Log("Found existing WristWatch, skipping creation.");
            return existing.gameObject;
        }

        // Create main WristWatch container
        GameObject wristWatch = new GameObject("WristWatch");
        wristWatch.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(wristWatch, "Create Wrist Watch");

        // Add WristTracker component
        WristTracker wristTracker = wristWatch.AddComponent<WristTracker>();

        // Create Watch Canvas
        GameObject watchCanvas = CreateWatchCanvas(wristWatch.transform);

        // Create tracking lost panel
        GameObject trackingLostPanel = CreateTrackingLostPanel(wristWatch.transform);

        // Add TrackingLostHandler
        TrackingLostHandler trackingLostHandler = wristWatch.AddComponent<TrackingLostHandler>();

        // Wire up ALL serialized fields
        SerializedObject trackerSO = new SerializedObject(wristTracker);
        trackerSO.FindProperty("handTrackingEvents").objectReferenceValue = handTrackingEvents;
        trackerSO.FindProperty("watchTransform").objectReferenceValue = watchCanvas.transform;
        if (headTransform != null)
        {
            trackerSO.FindProperty("headTransform").objectReferenceValue = headTransform;
        }
        trackerSO.ApplyModifiedProperties();

        SerializedObject handlerSO = new SerializedObject(trackingLostHandler);
        handlerSO.FindProperty("watchVisuals").objectReferenceValue = watchCanvas;
        handlerSO.FindProperty("trackingLostPanel").objectReferenceValue = trackingLostPanel;
        handlerSO.ApplyModifiedProperties();

        // Connect tracking events
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            wristTracker.onTrackingAcquired,
            trackingLostHandler.OnTrackingAcquired
        );
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            wristTracker.onTrackingLost,
            trackingLostHandler.OnTrackingLost
        );

        // Connect palm visibility events (for VR mode)
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            wristTracker.onWatchBecameVisible,
            trackingLostHandler.OnWatchBecameVisible
        );
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            wristTracker.onWatchBecameHidden,
            trackingLostHandler.OnWatchBecameHidden
        );

        return wristWatch;
    }

    private static GameObject CreateWatchCanvas(Transform parent)
    {
        GameObject canvasObj = new GameObject("WatchCanvas");
        canvasObj.transform.SetParent(parent, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100f;

        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.AddComponent<CanvasGroup>();

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 200);
        canvasRect.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        // Create background
        CreateWatchBackground(canvasObj.transform);

        // Create time text
        GameObject timeText = CreateTimeText(canvasObj.transform);

        // Create date text
        GameObject dateText = CreateDateText(canvasObj.transform);

        // Add TimeDisplay component
        TimeDisplay timeDisplay = canvasObj.AddComponent<TimeDisplay>();
        SerializedObject timeSO = new SerializedObject(timeDisplay);
        timeSO.FindProperty("timeText").objectReferenceValue = timeText.GetComponent<TextMeshProUGUI>();
        timeSO.FindProperty("dateText").objectReferenceValue = dateText.GetComponent<TextMeshProUGUI>();
        timeSO.ApplyModifiedProperties();

        return canvasObj;
    }

    private static GameObject CreateWatchBackground(Transform parent)
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(parent, false);

        Image image = bgObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        RectTransform rect = bgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return bgObj;
    }

    private static GameObject CreateTimeText(Transform parent)
    {
        GameObject textObj = new GameObject("TimeText");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "12:00 PM";
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.4f);
        rect.anchorMax = new Vector2(1, 0.8f);
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, 0);

        return textObj;
    }

    private static GameObject CreateDateText(Transform parent)
    {
        GameObject textObj = new GameObject("DateText");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Mon, Jan 19";
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.15f);
        rect.anchorMax = new Vector2(1, 0.4f);
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, 0);

        return textObj;
    }

    private static GameObject CreateTrackingLostPanel(Transform parent)
    {
        GameObject panelObj = new GameObject("TrackingLostPanel");
        panelObj.transform.SetParent(parent, false);

        Canvas canvas = panelObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = panelObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100f;

        panelObj.AddComponent<GraphicRaycaster>();
        panelObj.AddComponent<CanvasGroup>();

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 100);
        panelRect.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        panelRect.localPosition = new Vector3(0, 0, 0.5f);

        // Create background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(panelObj.transform, false);

        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Create message text
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(panelObj.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Move your left hand into view";
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20, 20);
        textRect.offsetMax = new Vector2(-20, -20);

        return panelObj;
    }
}

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SceneResultTransition : MonoBehaviour
{
    [SerializeField] private float resultZoomSize = 2f;
    [SerializeField] private float zoomDuration = 0.7f;
    [SerializeField] private float resultDuration = 1.1f;
    [SerializeField] private float textVerticalOffset = -1.2f;
    [SerializeField] private AnimationCurve easing = null;

    private Camera transitionCamera;
    private Coroutine transitionRoutine;

    void Awake()
    {
        transitionCamera = GetComponent<Camera>();

        if (easing == null || easing.length == 0)
            easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }

    public void Play(Transform player, bool victory, int nextSceneIndex)
    {
        if (transitionRoutine != null || player == null)
            return;

        transitionRoutine = StartCoroutine(PlayRoutine(player, victory, nextSceneIndex));
    }

    private IEnumerator PlayRoutine(Transform player, bool victory, int nextSceneIndex)
    {
        DisableGameplayInput();

        float originalSize = transitionCamera.orthographicSize;
        Vector3 originalPosition = transitionCamera.transform.position;
        Vector3 targetPosition = transitionCamera.transform.position;
        targetPosition.x = player.position.x;
        targetPosition.y = player.position.y;

        GameObject resultObject = CreateResultText(player, victory);

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / zoomDuration);
            float easedProgress = easing.Evaluate(progress);

            transitionCamera.transform.position = Vector3.Lerp(
                originalPosition,
                targetPosition,
                easedProgress);
            transitionCamera.orthographicSize = Mathf.Lerp(
                originalSize,
                resultZoomSize,
                easedProgress);

            yield return null;
        }

        transitionCamera.transform.position = targetPosition;
        transitionCamera.orthographicSize = resultZoomSize;
        yield return new WaitForSecondsRealtime(resultDuration);

        if (resultObject != null)
            Destroy(resultObject);

        if (GameStatManager.health <= 0)
            GameStatManager.restartGame();
        else
            GameStatManager.LoadScene(nextSceneIndex);
    }

    private GameObject CreateResultText(Transform player, bool victory)
    {
        GameObject resultObject = new GameObject("SceneResult");
        resultObject.transform.position = player.position + Vector3.up * textVerticalOffset;

        TextMesh text = resultObject.AddComponent<TextMesh>();
        text.text = victory ? "VICTORY" : "DEFEAT";
        text.anchor = TextAnchor.UpperCenter;
        text.alignment = TextAlignment.Center;
        text.fontSize = 48;
        text.characterSize = 0.12f;
        text.color = victory ? new Color(0.25f, 1f, 0.35f) : new Color(1f, 0.25f, 0.25f);
        text.fontStyle = FontStyle.Bold;
        return resultObject;
    }

    private void DisableGameplayInput()
    {
        foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(
                     FindObjectsInactive.Exclude,
                     FindObjectsSortMode.None))
        {
            if (behaviour is BHPlayerMovement playerMovement)
                playerMovement.SetMovementEnabled(false);
            else if (behaviour is GameManager || behaviour is BulletHellManager)
                behaviour.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StoryController : MonoBehaviour
{
    static public StoryController Instance;

    [Header("Story Sequence")]
    [SerializeField] List<StoryStep> _steps = new List<StoryStep>();

    [Header("Components")]
    [SerializeField] TextMeshProUGUI _messageText;

    bool _hasSoldOre = false;
    public bool BeganEndGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }

        _messageText.SetText("");
    }

    private void Start()
    {
        StartCoroutine(StartStory());
    }

    IEnumerator StartStory()
    {
        yield return new WaitForSeconds(1f);

        PlayNextStoryStep();

        yield break;
    }

    public void PlayNextStoryStep()
    {
        bool stepsRemain = false;

        for (int i = 0; i < _steps.Count; i++)
        {
            StoryStep step = _steps[i];
            if (step.Completed)
                continue;

            stepsRemain = true;
            string message = "";
            switch (step.FontSize)
            {
                case FontSize.Large:
                    message += $"<size={_messageText.fontSize * 1.5f}>";
                    message += step.Message;
                    message += "</size>";
                    break;

                case FontSize.ExtraLarge:
                    message += $"<size={_messageText.fontSize * 2f}>";
                    message += step.Message;
                    message += "</size>";
                    break;

                default:
                    message += step.Message;
                    break;
            }

            step.Completed = true;
            DisplayText(message);
            break;
        }

        if (!stepsRemain)
        {
            Debug.Log("No more story");
        }
    }

    public void DisplayStorageFullWarning()
    {
        StartCoroutine(DoText("Storage full. Return to the shop."));
    }

    public void DisplayCoinsGained(int coinsGained)
    {
        System.Action onComplete = null;
        if (!_hasSoldOre)
            onComplete = () => StartCoroutine(DoText("Upgrade your machine."));

        StartCoroutine(DoText($"+ ${coinsGained}", onComplete));

        _hasSoldOre = true;
    }

    public void TriggerEndGame()
    {
        if (BeganEndGame)
            return;

        BeganEndGame = true;
        StartCoroutine(StartEndGameSequence());
    }

    IEnumerator StartEndGameSequence()
    {
        Debug.Log("Start end game sequence");
        float waitEach = 4f;

        GameController.Instance.HomeBase.transform.DOScale(Vector3.zero, waitEach);
        GameController.Instance.MineMachine.ShowUpgradesDisplay(false);

        yield return new WaitForSeconds(waitEach);
        DisplayText($"You found it...");

        yield return new WaitForSeconds(waitEach);
        DisplayText($"Let me repay you for destroying my land...");

        yield return new WaitForSeconds(waitEach);
        CameraController.Instance.Camera.DOShakePosition(waitEach, 1);
        // START FIGHT
        GameController.Instance.EvilTree.FightPlayer();

        yield break;
    }

    public void DisplayText(string text)
    {
        _messageText.DOPause();
        _messageText.DOKill();
        StopCoroutine(DoText(""));
        StartCoroutine(DoText(text));
    }

    IEnumerator DoText(string text, System.Action onCompleteCb = null)
    {
        float writeDuration = 0.9f;
        float holdDuration = 3f;

        _messageText.DOFade(0.8f, 0.15f);
        _messageText.DOText(text, writeDuration/*, scrambleMode: ScrambleMode.All*/);

        yield return new WaitForSeconds(holdDuration);
        //_messageText.DOText("", writeDuration);
        _messageText.DOFade(0f, writeDuration - 0.2f);

        yield return new WaitForSeconds(0.5f);
        _messageText.SetText("");

        onCompleteCb?.Invoke();

        yield break;
    }
}

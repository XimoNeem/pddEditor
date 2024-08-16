using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLayer_Window : WindowController
{
    [SerializeField] private TMP_Text _debugText;
    [SerializeField] private float _messageDisplayTime = 5f;

    private Queue<MessageData> _messageQueue = new Queue<MessageData>();
    private Coroutine _messageCoroutine;

    private struct MessageData
    {
        public string message;
        public Color color;

        public MessageData(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }
    }

    void Start()
    {
        _debugText.text = "";
        _messageCoroutine = StartCoroutine(DisplayMessages());
    }

    public void LogMessage(string message, Color color)
    {
        _messageQueue.Enqueue(new MessageData(message, color));

        _debugText.text = "";

        foreach (var item in _messageQueue)
        {
            MessageData messageData = item;
            _debugText.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(messageData.color)}>{messageData.message}</color>\n";
        }
    }

    private IEnumerator DisplayMessages()
    {
        while (true)
        {
            _debugText.text = "";

            foreach (var item in _messageQueue)
            {
                MessageData messageData = item;
                _debugText.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(messageData.color)}>{messageData.message}</color>\n";
            }

            if (_messageQueue.Count > 0) { _messageQueue.Dequeue(); }

            yield return new WaitForSeconds(_messageDisplayTime);
        }
    }
}

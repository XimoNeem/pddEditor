using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scene_Ticket : Ticket, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage Preview;
    public TMP_Text Name;
    public TMP_Text Path;
    public TMP_Text CreatonDate;
    public TMP_Text EditDate;
    public Button OpenButton;
    public Button DeleteButton;
    public Button CloneButton;
    public GameObject ButtonsContent;

    private Color _backgroundColor;
    private Image _image;

    private void Start()
    {
        _image = this.GetComponent<Image>();
        _backgroundColor = _image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverEnds();
    }

    public void OnHover()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(Color.Lerp(Color.black, Color.white, 0.75f)));
        ButtonsContent.SetActive(true);
    }

    public void OnHoverEnds()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(_backgroundColor));
        ButtonsContent.SetActive(false);
    }

    private IEnumerator ChangeColor(Color b)
    {
        float t = 0;
        while (t < 1)
        {
            _image.color = Color.Lerp(_image.color, b, t);
            t += Time.fixedDeltaTime;
            yield return null;
        }
    }
}

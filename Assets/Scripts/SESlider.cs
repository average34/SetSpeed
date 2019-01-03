using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Slider))]
public class SESlider : MonoBehaviour, IEndDragHandler,IPointerClickHandler


{
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)

    {
        AudioManager.Instance.PlaySE("イィイイイヤッホォオオオオウ！！！！", 1);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySE("イィイイイヤッホォオオオオウ！！！！", 1);
    }

    // Use this for initialization
    void Start()
    {
        GetComponent<Slider>().value = AudioManager.Instance.AttachSESource_ch1.volume;


    }

    // Update is called once per frame
    void Update()
    {

    }
}

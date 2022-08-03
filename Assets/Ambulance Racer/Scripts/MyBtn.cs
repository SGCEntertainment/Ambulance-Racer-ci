using UnityEngine.EventSystems;
using UnityEngine;

public class MyBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    enum Type { direction, stopping }

    bool IsPressing;
    [SerializeField] int dir;
    [SerializeField] Type type;

    private void OnDisable()
    {
        IsPressing = false;
    }

    private void Update()
    {
        if(!IsPressing)
        {
            return;
        }

        switch(type)
        {
            case Type.direction: Manager.Instance.MovePlayer(dir); ; break;
            case Type.stopping: Manager.Instance.Stopping(true); ; break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPressing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPressing = false;
        if(type == Type.stopping)
        {
            Manager.Instance.Stopping(false);
        }
    }
}

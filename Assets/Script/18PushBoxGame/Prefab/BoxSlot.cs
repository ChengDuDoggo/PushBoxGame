using UnityEngine;
public enum SlotType
{
    Null,//空格子
    Wall,//墙格子
    Road,//路格子
    Target,//目标格子
}
public class BoxSlot : MonoBehaviour
{
    public SlotType BoxSlotType;
    [HideInInspector]
    public Vector2Int BoxSlotCoordinates;//格子坐标
    [HideInInspector]
    public bool IsHaveBox = false;//当前格子是否放着箱子
    [HideInInspector]
    public bool IsHavePlayer = false;//当前格子是否站着玩家
    [HideInInspector]
    public bool IsFinishTargetSlot = false;//当前格子是否是完成状态(箱子推进了目标格子)
    private void Awake()
    {
        if (transform.childCount > 0)
        {
            if (GetComponentInChildren<Player>())
            {
                IsHavePlayer = true;
            }
            if (GetComponentInChildren<Box>())
            {
                IsHaveBox = true;
            }
        }
        if (IsHaveBox && BoxSlotType == SlotType.Target)
        {
            IsFinishTargetSlot = true;
        }
    }
}
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int PlayerCoordinates;//玩家当前坐标
    private PushBoxGamePage gameRoot;//游戏根节点
    private BoxSlot currParentBoxSlot;//当前玩家所在的父格子对象
    private BoxSlot targetBoxSlot;//玩家移动的下一个目标格子
    private BoxSlot targetTargetSlot;//玩家移动的下下一个目标格子
    private void Start()
    {
        gameRoot = GetComponentInParent<PushBoxGamePage>();
        gameRoot.UpArrowBtn.onClick.AddListener(() => Move(-1, -2, 0, 0));
        gameRoot.DownArrowBtn.onClick.AddListener(() => Move(1, 2, 0, 0));
        gameRoot.LeftArrowBtn.onClick.AddListener(() => Move(0, 0, -1, -2));
        gameRoot.RightArrowBtn.onClick.AddListener(() => Move(0, 0, 1, 2));
        currParentBoxSlot = GetComponentInParent<BoxSlot>();
        PlayerCoordinates = currParentBoxSlot.BoxSlotCoordinates;
    }
    private void OnDisable()
    {
        gameRoot.UpArrowBtn.onClick.RemoveAllListeners();
        gameRoot.DownArrowBtn.onClick.RemoveAllListeners();
        gameRoot.LeftArrowBtn.onClick.RemoveAllListeners();
        gameRoot.RightArrowBtn.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// 玩家实际移动函数
    /// </summary>
    /// <param name="x1Coordinates">下一个格子X轴变化参数</param>
    /// <param name="x2Coordinates">下下一个格子X轴变化参数</param>
    /// <param name="y1Coordinates">下一个格子Y轴变化参数</param>
    /// <param name="y2Coordinates">下下一个格子Y轴变化参数</param>
    private void Move(int x1Coordinates, int x2Coordinates, int y1Coordinates, int y2Coordinates)
    {
        //保存当前状态
        gameRoot.SaveGameState();
        //拿到下一个目标格子坐标
        Vector2Int targetCoordinates = new Vector2Int(PlayerCoordinates.x + x1Coordinates, PlayerCoordinates.y + y1Coordinates);
        //拿到下下一个目标格子坐标
        Vector2Int targetTargetCoordinates = new Vector2Int(PlayerCoordinates.x + x2Coordinates, PlayerCoordinates.y + y2Coordinates);
        foreach (BoxSlot boxSlot in gameRoot.currentLevelAllBoxSlots)
        {
            if (boxSlot.BoxSlotCoordinates == targetCoordinates)
            {
                //拿到下一个格子
                targetBoxSlot = boxSlot;
                if (targetBoxSlot.BoxSlotType == SlotType.Road || targetBoxSlot.BoxSlotType == SlotType.Target)
                {
                    if (targetBoxSlot.IsHaveBox)//如果下一个格子上有箱子
                    {
                        foreach (BoxSlot boxBoxSlot in gameRoot.currentLevelAllBoxSlots)
                        {
                            if (boxBoxSlot.BoxSlotCoordinates == targetTargetCoordinates)
                            {
                                //拿到下下一个格子
                                targetTargetSlot = boxBoxSlot;
                                if (targetTargetSlot.BoxSlotType == SlotType.Road || targetTargetSlot.BoxSlotType == SlotType.Target)
                                {
                                    if (targetTargetSlot.IsHaveBox)
                                    {
                                        return;
                                    }
                                    //得到下一个格子上的箱子
                                    Box box = targetBoxSlot.GetComponentInChildren<Box>();
                                    box.transform.SetParent(targetTargetSlot.transform);
                                    box.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                                    targetBoxSlot.IsHaveBox = false;
                                    targetBoxSlot.IsFinishTargetSlot = false;
                                    targetTargetSlot.IsHaveBox = true;
                                    if (targetTargetSlot.BoxSlotType == SlotType.Target)
                                    {
                                        targetTargetSlot.IsFinishTargetSlot = true;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    currParentBoxSlot.IsHavePlayer = false;
                    targetBoxSlot.IsHavePlayer = true;
                    transform.SetParent(targetBoxSlot.transform);
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    PlayerCoordinates = boxSlot.BoxSlotCoordinates;
                    currParentBoxSlot = GetComponentInParent<BoxSlot>();
                }
            }
        }
        gameRoot.JudgmentPass();
    }
}
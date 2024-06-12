using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushBoxGamePage : MonoBehaviour
{
    public GameObject BoxPrefab;//箱子预制体
    public GameObject PlayerPrefab;//小人儿预制体
    public Sprite[] SlotImgs;//格子图片合集(0墙、1路和2过关点)
    public List<GridLayoutGroup> Levels = new List<GridLayoutGroup>();//所有关卡
    public Button UpArrowBtn;
    public Button DownArrowBtn;
    public Button LeftArrowBtn;
    public Button RightArrowBtn;
    public Button RestartBtn;
    public Button BackStepBtn;
    public Text TimeText;
    public Text FinishNumText;
    [HideInInspector]
    public BoxSlot[] currentLevelAllBoxSlots;//当前关卡所有BoxSlot
    private readonly List<BoxSlot> currentLevelAllTargetBoxSlots = new List<BoxSlot>();//当前关卡的所有箱子终点BoxSlot
    private int NextLevelIndex;//下一关卡索引
    private int RowsNumOfSlot;//一排格子数
    private Player player;//控制的小人
    private Stack<GameState> historyStack = new Stack<GameState>();//游戏历史状态记录
    private GameState initialState;//游戏初始状态记录
    private bool hasUndoed = false;//判断是否已经使用了撤销功能
    private void Start()
    {
        RestartBtn.onClick.AddListener(RestartLevel);
        BackStepBtn.onClick.AddListener(Undo);
        NextLevelIndex = 0;
        LoadLevel();
    }
    /// <summary>
    /// 加载关卡
    /// </summary>
    /// <param name="levelIndex">需要加载的关卡索引</param>
    private void LoadLevel()
    {
        if (NextLevelIndex < Levels.Count)
        {
            foreach (GridLayoutGroup level in Levels)
            {
                if (level.gameObject.activeSelf)
                {
                    level.gameObject.SetActive(false);
                }
            }
            Levels[NextLevelIndex].gameObject.SetActive(true);
            RowsNumOfSlot = Levels[NextLevelIndex].constraintCount;
            currentLevelAllBoxSlots = Levels[NextLevelIndex].GetComponentsInChildren<BoxSlot>();
            player = GetComponentInChildren<Player>();
            currentLevelAllTargetBoxSlots.Clear();
            //为每一个BoxSlot赋值坐标和图片
            int x = 0;
            int y = 0;
            foreach (BoxSlot boxSlot in currentLevelAllBoxSlots)
            {
                //赋值图片
                boxSlot.GetComponent<Image>().sprite = boxSlot.BoxSlotType switch
                {
                    SlotType.Null => null,
                    SlotType.Wall => SlotImgs[0],
                    SlotType.Road => SlotImgs[1],
                    SlotType.Target => SlotImgs[2],
                    _ => null,
                };
                //赋值坐标
                boxSlot.BoxSlotCoordinates.x = x;
                boxSlot.BoxSlotCoordinates.y = y;
                if (y < RowsNumOfSlot - 1)
                {
                    y++;
                }
                else
                {
                    x++;
                    y = 0;
                }
                if (boxSlot.BoxSlotType == SlotType.Target)
                {
                    currentLevelAllTargetBoxSlots.Add(boxSlot);
                }
            }
            NextLevelIndex++;
            //保存游戏初始状态数据
            initialState = new GameState(currentLevelAllBoxSlots, player);
            JudgmentPass();//当加载完一个关卡后判断一下是否通关
        }
        else//已经是最后一关
        {
            Debug.Log("游戏通关");
            NextLevelIndex = 0;
            currentLevelAllTargetBoxSlots.Clear();
        }
        //关卡加载后重置历史记录和撤销标志
        historyStack.Clear();
        hasUndoed = false;
        BackStepBtn.interactable = true;
    }
    //判断当前关卡是否通过
    public void JudgmentPass()
    {
        int finishNum = 0;
        foreach (BoxSlot targetBoxSlot in currentLevelAllTargetBoxSlots)
        {
            if (targetBoxSlot.IsHaveBox)
            {
                finishNum++;
            }
        }
        FinishNumText.text = $"已完成：{finishNum}/{currentLevelAllTargetBoxSlots.Count}";
        foreach (BoxSlot targetBoxSlot in currentLevelAllTargetBoxSlots)
        {
            if (!targetBoxSlot.IsHaveBox)
            {
                return;
            }
        }
        LoadLevel();//进入下一关
    }
    /// <summary>
    /// 撤销
    /// </summary>
    public void Undo()
    {
        if (historyStack.Count > 0 && !hasUndoed)
        {
            GameState previousState = historyStack.Pop();//取出栈中最顶部的游戏记录信息
            RestoreGameState(previousState);
            JudgmentPass();
            hasUndoed = true;//设置撤销标志
            BackStepBtn.interactable = false;
        }
    }
    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public void RestartLevel()
    {
        historyStack.Clear();
        hasUndoed = false;
        BackStepBtn.interactable = true;
        RestoreGameState(initialState);
        JudgmentPass();
    }
    /// <summary>
    /// 保存当前游戏状态信息
    /// </summary>
    public void SaveGameState()
    {
        GameState gameState = new GameState(currentLevelAllBoxSlots, player);
        historyStack.Push(gameState);//将保存的当前游戏信息记录压入栈中
    }
    /// <summary>
    /// 加载游戏记录
    /// </summary>
    /// <param name="gameState">游戏记录信息</param>
    private void RestoreGameState(GameState gameState)
    {
        //清除原有数据
        foreach (var slot in currentLevelAllBoxSlots)
        {
            if (slot.GetComponentInChildren<Box>())
            {
                Destroy(slot.GetComponentInChildren<Box>().gameObject);
            }
            if (slot.GetComponentInChildren<Player>())
            {
                Destroy(slot.GetComponentInChildren<Player>().gameObject);
            }
        }
        //加载输入的游戏记录信息数据
        foreach (SlotState slotState in gameState.slotStates)
        {
            BoxSlot slot = currentLevelAllBoxSlots[slotState.index];
            slot.IsHaveBox = slotState.isHaveBox;
            slot.IsHavePlayer = slotState.isHavePlayer;
            slot.IsFinishTargetSlot = slotState.isFinishTargetSlot;
            if (slotState.isHaveBox)
            {
                Box box = Instantiate(BoxPrefab.GetComponent<Box>(), slot.transform);
                box.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
            if (slotState.isHavePlayer)
            {
                Player player = Instantiate(PlayerPrefab.GetComponent<Player>(), slot.transform);
                player.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                player.PlayerCoordinates = slot.BoxSlotCoordinates;
            }
        }
    }
}
//记录游戏状态信息类
[System.Serializable]
public class GameState
{
    public List<SlotState> slotStates;
    public Vector2Int playerCoordinates;
    public GameState(BoxSlot[] slots, Player player)
    {
        slotStates = new List<SlotState>();
        for (int i = 0; i < slots.Length; i++)
        {
            slotStates.Add(new SlotState
            {
                index = i,
                isHaveBox = slots[i].IsHaveBox,
                isHavePlayer = slots[i].IsHavePlayer,
                isFinishTargetSlot = slots[i].IsFinishTargetSlot,
            });
        }
        playerCoordinates = player.PlayerCoordinates;
    }
}
//记录格子状态信息类
[System.Serializable]
public class SlotState
{
    public int index;
    public bool isHaveBox;
    public bool isHavePlayer;
    public bool isFinishTargetSlot;
}
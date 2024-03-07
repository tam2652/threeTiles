using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    public Transform selectBar;
    //public List<Tile> matchedBlocks { get; private set; } = new List<Tile>();
    public List<Tile> tileList = new List<Tile>();
    public List<Tile> barBlockList = new List<Tile>(); // ds cac block o select bar
    public List<Tile> selectBarBlockList = new List<Tile>();
    public GameObject moveBackButtonPrefab;
    public AudioSource MenuAudio;
    public AudioClip touchInBlock;
    public AudioClip match3;
    public LevelMenu levelMenu;

    public int levelNumber;
    private void Start()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        tileList.AddRange(tiles);
    
        foreach (Tile tile in tileList)
        {

            Button button = tile.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => MoveTileToSelectBar(tile, button));
            }
            tile.SetTileList(tileList);
        }
        UpdateBlockInteractionStatus(tileList);
        shuffleBlockList();
    }

    private void Update()
    {
        UpdateBlockInteractionStatus(tileList);
    }


    private void MoveTileToSelectBar(Tile tile, Button button)
    {
        if (selectBar.childCount == 7)
        {
            
            return;
        }

        selectBarBlockList.Add(tile);

        // Tìm index để chèn block vào selectBar
        int insertIndex = barBlockList.Count;
        for (int i = 0; i < barBlockList.Count; i++)
        {
            if (barBlockList[i].BlockType == tile.BlockType)
            {
                insertIndex = i + 1;
            }
        }

        button.transform.SetParent(selectBar);
        button.transform.SetSiblingIndex(insertIndex);
        barBlockList.Insert(insertIndex, tile);

        Collider2D collider = tile.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        tileList.Remove(tile);

        // Hiệu ứng rơi xuống select bar
        RectTransform tileTransform = tile.GetComponent<RectTransform>();
        Vector3 startPosition = tileTransform.position;

        // Tính toán endPosition dựa trên insertIndex
        Vector3 endPosition = selectBar.position;
        if (insertIndex > 0)
        {
            Tile previousBlock = barBlockList[insertIndex - 1];
            RectTransform previousTileTransform = previousBlock.GetComponent<RectTransform>();
            float offset = 50f;
            endPosition = new Vector3(previousTileTransform.position.x + previousTileTransform.rect.width + offset, previousTileTransform.position.y, previousTileTransform.position.z);
        }

        tileTransform.SetParent(selectBar);
        float duration = 0.2f;
        tileTransform.DOMove(endPosition, duration).OnComplete(() =>
        {
            // Kích hoạt lại layout của selectBar sau khi block đã rơi xuống
            selectBar.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            selectBar.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
            float delay = 0.255f;
            Invoke("CheckMatchingBlocks", delay);
        });
        if (MenuAudio && touchInBlock)
        {
            MenuAudio.PlayOneShot(touchInBlock);
        }
    }

    public void moveBack(Tile tile, Button button)
    {
        // Kiểm tra xem ô gạch có nằm trong thanh chọn hay không
        if (selectBarBlockList.Contains(tile))
        {
            // Kiểm tra xem ô gạch có phải là ô gạch vừa được thêm vào selectBarBlockList (chọn để đưa trở lại) hay không
            if (selectBarBlockList[selectBarBlockList.Count - 1] == tile)
            {
                // Loại bỏ ô gạch khỏi danh sách trong thanh chọn
                selectBarBlockList.Remove(tile);

                // Xóa nút khỏi thanh chọn
                button.transform.SetParent(null);

                // Đặt lại vị trí ban đầu cho ô gạch

                // Tìm vị trí ban đầu của ô gạch
                int initialIndex = tileList.IndexOf(tile);
                RectTransform initialTileTransform = tile.GetComponent<RectTransform>();
                Vector3 initialPosition = initialTileTransform.anchoredPosition;

                // Di chuyển ô gạch về vị trí ban đầu
                tile.transform.SetParent(null);
                tile.transform.SetSiblingIndex(initialIndex);
                tile.GetComponent<RectTransform>().anchoredPosition = initialPosition;
                barBlockList.Insert(initialIndex, tile);

                Collider2D collider = tile.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }
                tileList.Add(tile);

                // Kích hoạt lại layout của thanh chọn sau khi ô gạch đã di chuyển trở lại
                selectBar.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
                selectBar.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();

                float delay = 0.255f;
                Invoke("CheckMatchingBlocks", delay);
            }
        }
    }

    public void MoveBackButtonClick()
    {
        // Kiểm tra xem có nút nào đang được chọn trong thanh chọn hay không
        if (selectBarBlockList.Count > 0)
        {
            // Lấy tile và button từ thanh chọn (selectBarBlockList)
            Tile tileToMoveBack = selectBarBlockList[selectBarBlockList.Count - 1];
            Button buttonToMoveBack = tileToMoveBack.GetComponentInChildren<Button>();

            // Gọi phương thức moveBack() và truyền tile và button vào.
            moveBack(tileToMoveBack, buttonToMoveBack);
            //CheckMatchingBlocks();
        }
    }

    public bool CompareBlockTypes(Tile block1, Tile block2)
    {
        return block1.BlockType == block2.BlockType;
    }

    private void CheckMatchingBlocks()
    {
        List<Tile> matchedBlocks = new List<Tile>();

        for (int i = 0; i < barBlockList.Count; i++)
        {
            if (matchedBlocks.Contains(barBlockList[i]))
                continue;

            matchedBlocks.Add(barBlockList[i]);

            for (int j = i + 1; j < barBlockList.Count; j++)
            {
                if (CompareBlockTypes(barBlockList[i], barBlockList[j]))
                {
                    matchedBlocks.Add(barBlockList[j]);
                }
            }

            if (matchedBlocks.Count == 3)
            {
                foreach (Tile matchedBlock in matchedBlocks)
                {
                    barBlockList.Remove(matchedBlock);
                    Button button = matchedBlock.GetComponentInChildren<Button>();
                    if (button != null)
                    {
                        Destroy(button.gameObject);
                        if(MenuAudio && match3)
                        {
                            MenuAudio.PlayOneShot(match3);
                        }
                    }
                }
                Debug.Log("Các block bị loại bỏ");
                break;
            }

            matchedBlocks.Clear();
        }

        if (barBlockList.Count == 7)
        {
            string lose = "Lose";
            PlayerPrefs.SetInt("inLevel", levelNumber);
            SceneManager.LoadScene(lose);
            Debug.Log("YOU ARE LOSER");
        }
        if (barBlockList.Count == 0 && tileList.Count == 0)
        {
            UnlockNewLevel();
            PlayerPrefs.SetInt("inLevel", levelNumber);
            levelNumber++;
            Debug.Log("YOU WIN!");
            string win = "Win";
            SceneManager.LoadScene(win);
        }
    }

    public void shuffleBlockList()
    {
        int numBlocks = tileList.Count;

        List<Vector2> initialPositions = new List<Vector2>();
        foreach (Tile tile in tileList)
        {
            initialPositions.Add(tile.GetComponent<RectTransform>().anchoredPosition);
        }
        // Tạo một danh sách chỉ chứa thứ tự các chỉ số từ 0 đến numBlocks - 1
        List<int> indices = new List<int>();
        for (int i = 0; i < numBlocks; i++)
        {
            indices.Add(i);
        }

        // Xáo trộn thứ tự các chỉ số trong danh sách indices
        for (int i = 0; i < numBlocks - 1; i++)
        {
            int randomIndex = Random.Range(i, numBlocks);
            int tempIndex = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = tempIndex;
        }

        // Tạo danh sách tạm thời để lưu các Tile đã được xáo trộn
        List<Tile> shuffledTiles = new List<Tile>();

        // Tạo danh sách shuffledTiles theo thứ tự đã xáo trộn
        for (int i = 0; i < numBlocks; i++)
        {
            int index = indices[i];
            shuffledTiles.Add(tileList[index]);
        }

        // Cập nhật danh sách tileList bằng danh sách shuffledTiles
        for (int i = 0; i < numBlocks; i++)
        {
            tileList[i] = shuffledTiles[i];
            tileList[i].transform.SetSiblingIndex(i);
            tileList[i].GetComponent<RectTransform>().anchoredPosition = initialPositions[i];
            //tileList[i].transform.SetSiblingIndex(i);
        }
    }
    public void UpdateBlockInteractionStatus(List<Tile> initialTiles)   
    {
        foreach (Tile tile in initialTiles)
        {
            bool isSelectBarBlock = selectBarBlockList.Contains(tile);
            if (!isSelectBarBlock)
            {
                bool isOverlapped = tile.CheckTouching(initialTiles);

                Button button = tile.GetComponentInChildren<Button>();
                if (button != null)
                {
                    button.interactable = !isOverlapped;
                }
            }
        }
    }
    void UnlockNewLevel()
    {
        
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex01"))
        {
            PlayerPrefs.SetInt("ReachedIndex01", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel01", PlayerPrefs.GetInt("UnlockedLevel01", 1)  + 1);
            //PlayerPrefs.Save();
            Debug.Log("hihi haha " + PlayerPrefs.GetInt("UnlockedLevel01", 1));
            
        }
    }
}
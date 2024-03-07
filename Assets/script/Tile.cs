using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer maskRenderer;
    public ContactFilter2D contactFilter;
    public Collider2D catCollider;
    private List<Tile> tilesList;
    public int BlockType;

    public void SetTileList(List<Tile> tiles)
    {
        tilesList = tiles;
    }

    public bool CheckTouching(List<Tile> tileList)
    {
        int currentIndex = transform.GetSiblingIndex();

        foreach (Tile otherTile in tileList)
        {
            int otherIndex = otherTile.transform.GetSiblingIndex();

            if (this != otherTile && currentIndex < otherIndex && GetComponent<Collider2D>().bounds.Intersects(otherTile.GetComponent<Collider2D>().bounds))
            {
                return true; // Trả về true nếu block đang xét bị chồng lấn từ phía trên
            }
        }

        return false; // Trả về false nếu không có chồng lấn
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

    public Transform prefabCell;

    List<SpriteRenderer> container = new List<SpriteRenderer>();

    public Color color;

    float startX = -12;
    float endX   =  37;

    float startY =  5;
    float endY   = -20;

    float cellSize = 0.3f;

    void Start() {
        InstantiateCell();
    }

    void Update() {
        for(int i = 0; i < container.Count; i++) {
            container[i].color = color;
        }
    }

    void InstantiateCell() {

        float x = cellSize * startX;
        float y = cellSize * startY;
        
        for(int j = (int)startY; j > endY; j--) {
            for(int i = (int)startX; i < endX; i++) {
                Vector2 position = new Vector2(x + i * cellSize, y + cellSize * j);
                SpriteRenderer cell = Instantiate(prefabCell,
                                                  position, 
                                                  Quaternion.identity).GetComponent<SpriteRenderer>();
                container.Add(cell);
            }
        }
    }

}

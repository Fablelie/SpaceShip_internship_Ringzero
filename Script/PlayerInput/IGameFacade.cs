using UnityEngine;
using System.Collections;

public interface IGameFacade
{

    Vector3 GetRotation();
    void SetRotation(Vector3 euler);
    int GetScore();
    void SetScore(int score);
    Color GetPlayerColor();
    void SetPlayerColor(Color color);

}

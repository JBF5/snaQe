using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePiece : MonoBehaviour
{

    public abstract void PreGameStep();
    public abstract void GameStep();
    public abstract void PostGameStep();
    public abstract void GameOver();

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public int x, y;

    public Board board;

    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    };

    public type pieceType;


    public void Setup(int x_, int y_, Board board_)
    {
        x = x_;
        y = y_;
        board = board_;

        transform.localScale = Vector3.one * 0.35f;
        transform.DOScale(Vector3.one, 0.35f);

    }

    public void Move(int destinoX, int destinoY)
    {
        transform.DOMove(new Vector3(destinoX, destinoY, -5), 0.5f).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            x = destinoX;
            y = destinoY;
        };

    }

    public void Remove(bool animated)
    {
        if (animated)
        {
            transform.DORotate(Vector3(0,0,-120f), 0.12f);

            transform.DOScale(Vector3.one * 1.2f, 0.085f).onComplete = () =>
            {
                transform.DOScale(Vector3.zero, 0.1f).onComplete = () =>
                {
                    Destroy(gameObject);
                };
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Test move")]
    public void MoveTest()
    {
        Move(0, 0);
    }

}

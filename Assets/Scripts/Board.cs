using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Board : MonoBehaviour
{

    public int width;
    public int height;
    public float timeBetweenPieces = 0.005f;

    public GameObject tileObject;

    public float cameraSizeOffset;
    public float cameraVerticalOffset;

    public GameObject[] availablePieces;


    Tile[,] Tiles;

    Piece[,] Pieces;

    Tile startTile;
    Tile endTile;

    bool swappingPieces = false;



    // Start is called before the first frame update
    void Start()
    {
        Tiles = new Tile[width, height];
        Pieces = new Piece[width, height];

        SetUpBoard();
        PositionCamera();
        StartCoroutine(SetupPieces());
    }


    private void SetUpBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var o = Instantiate(tileObject, new Vector3(i, j, -5), Quaternion.identity);
                o.transform.parent = transform;
                Tiles[i, j] = o.GetComponent<Tile>();
                Tiles[i, j].Setup(i, j, this);
            }
        }
    }

    private void PositionCamera()
    {
        float newPositionX = (float)width / 2f;
        float newPositionY = (float)height / 2f;

        Camera.main.transform.position = new Vector3(newPositionX - 0.5f, newPositionY - 0.5f + cameraVerticalOffset, -10);

        float horizontal = width + 1;
        float vertical = (height / 2f) + 1;

        Camera.main.orthographicSize = horizontal > vertical ? horizontal + cameraSizeOffset : vertical + cameraSizeOffset;

    }

    public Piece CreatePieceAt(int x, int y)
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];

        var o = Instantiate(selectedPiece, new Vector3(x, y+1, -5), Quaternion.identity);
        o.transform.parent = transform;
        Pieces[x, y] = o.GetComponent<Piece>();
        Pieces[x, y].Setup(x, y, this);

        Pieces[x, y].Move(x,y);
        

        return Pieces[x, y];
    }

    public void ClearPieceAt(int posx, int posy)
    {

        //Destroy(Pieces[posx, posy].gameObject);
        Pieces[posx, posy].Remove(true);
        Pieces[posx, posy] = null;

    }

    private IEnumerator SetupPieces()
    {
        int maxIteration = 50;
        int currentIteration = 0;


        for (int i = 0; i < width; i++)
        {
            currentIteration = 0;
            for (int j = 0; j < height; j++)
            {
                yield return new WaitForSeconds(timeBetweenPieces);
                if (Pieces[i, j] == null)
                {
                    var newPiece = CreatePieceAt(i, j);

                    while (HasPreviousMatches(i, j))
                    {
                        ClearPieceAt(i, j);
                        newPiece = CreatePieceAt(i, j);
                        currentIteration++;
                        if (currentIteration > maxIteration)
                        {
                            break;
                        }
                    }
                }

            }
        }

        yield return null;
    }

    public void TileDown(Tile tile_)
    {
        if (!swappingPieces)
        {
            startTile = tile_;
        }

    }


    public void TileOver(Tile tile_)
    {
        if (!swappingPieces)
        {
            endTile = tile_;
        }
    }



    public void TileUp(Tile tile_)
    {
        if (!swappingPieces)
        {
            if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))
            {
                StartCoroutine(SwapTiles());
            }
        }

    }

    IEnumerator SwapTiles()
    {
        swappingPieces = true;
        var StartPiece = Pieces[startTile.x, startTile.y];

        var EndPiece = Pieces[endTile.x, endTile.y];

        StartPiece.Move(endTile.x, endTile.y);
        EndPiece.Move(startTile.x, startTile.y);


        Pieces[startTile.x, startTile.y] = EndPiece;
        Pieces[endTile.x, endTile.y] = StartPiece;

        yield return new WaitForSeconds(0.6f);



        var startMatches = GetMatchByPiece(startTile.x, startTile.y, 3);
        var endMatches = GetMatchByPiece(endTile.x, endTile.y, 3);

        var allMatches = startMatches.Union(endMatches).ToList();





        /*
                //bool foundMatch = false;
                startMatches.ForEach(piece =>
                {
                    foundMatch = true;
                    ClearPieceAt(piece.x, piece.y);
                    //Pieces[piece.x, piece.y] = null;
                    //Destroy(piece.gameObject);
                });


                endMatches.ForEach(piece =>
                {
                    foundMatch = true;
                    ClearPieceAt(piece.x, piece.y);
                    //Pieces[piece.x, piece.y] = null;
                    //Destroy(piece.gameObject);
                });*/


        if (allMatches.Count == 0)
        {
            StartPiece.Move(startTile.x, startTile.y);
            EndPiece.Move(endTile.x, endTile.y);

            Pieces[startTile.x, startTile.y] = StartPiece;
            Pieces[endTile.x, endTile.y] = EndPiece;

        }
        else
        {
            ClearPieces(allMatches);
        }

        startTile = null;
        endTile = null;

        swappingPieces = false;

        yield return null;


    }


    public void ClearPieces(List<Piece> piecesToClear)
    {
        piecesToClear.ForEach(piece =>
        {
            ClearPieceAt(piece.x, piece.y);
        });

        List<int> columns = GetColumns(piecesToClear);

        List<Piece> collapsedPieces = CollapseColumns(columns, 0.3f);

        FindMatchesRecursively(collapsedPieces);

    }


    public List<int> GetColumns(List<Piece> piecesToClear)
    {
        var result = new List<int>();

        piecesToClear.ForEach(piece =>
        {
            if (!result.Contains(piece.x))
            {
                result.Add(piece.x);
            }
        });


        return result;
    }


    public List<Piece> CollapseColumns(List<int> columns, float timeToCollapse)
    {
        List<Piece> movingPieces = new List<Piece>();
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];

            for (int y = 0; y < height; y++)
            {
                if (Pieces[column, y] == null)
                {
                    for (int yplus = y + 1; yplus < height; yplus++)
                    {
                        if (Pieces[column, yplus] != null)
                        {
                            Pieces[column, yplus].Move(column, y);
                            Pieces[column, y] = Pieces[column, yplus];

                            if (!movingPieces.Contains(Pieces[column, y]))
                            {
                                movingPieces.Add(Pieces[column, y]);
                            }

                            Pieces[column, yplus] = null;
                            break;
                        }
                    }
                }
            }
        }

        return movingPieces;
    }



    public void FindMatchesRecursively(List<Piece> collapsedPieces)
    {
        StartCoroutine(FindMatchesRecursivelyCoroutine(collapsedPieces));
    }

    IEnumerator FindMatchesRecursivelyCoroutine(List<Piece> collapsedPieces)
    {
        yield return new WaitForSeconds(1f);

        List<Piece> newMatches = new List<Piece>();

        collapsedPieces.ForEach(piece =>
        {
            var matches = GetMatchByPiece(piece.x, piece.y, 3);

            if (matches != null)
            {
                newMatches = newMatches.Union(matches).ToList();

                ClearPieces(newMatches);
            }
        });

        if (newMatches.Count > 0)
        {
            var collapsedPieces2 = CollapseColumns(GetColumns(newMatches), 0.3f);
            FindMatchesRecursively(collapsedPieces2);
        }
        else
        {

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SetupPieces());
            swappingPieces = false;
        }

        yield return null;
    }


    public bool IsCloseTo(Tile start, Tile end)
    {
        if (Math.Abs(start.x - end.x) == 1 && start.y == end.y)
        {
            return true;
        }
        else if (Math.Abs(start.y - end.y) == 1 && start.x == end.x)
        {
            return true;
        }

        return false;
    }

    public bool HasPreviousMatches(int xpos, int ypos)
    {
        var downMatches = GetMatchByDirection(xpos, ypos, new Vector2(0, -1), 2);
        var leftMatches = GetMatchByDirection(xpos, ypos, new Vector2(-1, 0), 2);


        if (downMatches == null)
        {
            downMatches = new List<Piece>();
        }


        if (leftMatches == null)
        {
            leftMatches = new List<Piece>();
        }

        return (downMatches.Count > 0 || leftMatches.Count > 0);


    }


    public List<Piece> GetMatchByDirection(int xpos, int ypos, Vector2 direction, int minPieces = 3)
    {
        var matches = new List<Piece>();
        Piece startPiece = Pieces[xpos, ypos];
        matches.Add(startPiece);

        int nextX, nextY;
        int maxVal = width > height ? width : height;

        for (int i = 1; i < maxVal; i++)
        {
            nextX = xpos + ((int)direction.x * i);
            nextY = ypos + ((int)direction.y * i);

            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {

                Piece nextPiece = Pieces[nextX, nextY];

                if (nextPiece != null && nextPiece.pieceType == startPiece.pieceType)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minPieces)
        {
            return matches;
        }

        return null;

    }



    public List<Piece> GetMatchByPiece(int xpos, int ypos, int minPieces = 3)
    {
        var upMatches = GetMatchByDirection(xpos, ypos, new Vector2(0, 1), 2);
        var downMatches = GetMatchByDirection(xpos, ypos, new Vector2(0, -1), 2);
        var rigthMatches = GetMatchByDirection(xpos, ypos, new Vector2(1, 0), 2);
        var leftMatches = GetMatchByDirection(xpos, ypos, new Vector2(-1, 0), 2);


        if (upMatches == null)
        {
            upMatches = new List<Piece>();
        }

        if (downMatches == null)
        {
            downMatches = new List<Piece>();
        }

        if (rigthMatches == null)
        {
            rigthMatches = new List<Piece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<Piece>();
        }

        var verticalMatches = upMatches.Union(downMatches).ToList();
        var horizontalMatches = leftMatches.Union(rigthMatches).ToList();

        var foundMatches = new List<Piece>();

        if (verticalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(verticalMatches).ToList();
        }

        if (horizontalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(horizontalMatches).ToList();
        }

        return foundMatches;

    }

}

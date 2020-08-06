using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileLine
{
    private bool isVertical;
    private int lineIndex;
    private int startIndex;
    private int endIndex;

    public TileLine(bool _isVertical, int _lineIndex, int _startIndex, int _endIndex)
    {
        this.isVertical = _isVertical;
        this.lineIndex = _lineIndex;
        this.startIndex = _startIndex;
        this.endIndex = _endIndex;
    }

    public bool[] GetUseWidthTile(bool[] refillArray)
    {
        if (isVertical)
        {
            refillArray[lineIndex] = true;
        }
        else
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                refillArray[i] = true;
            }
        }
        return refillArray;
    }

    public Vector2Int[] GetLinePoints()
    {
        Vector2Int[] points = new Vector2Int[endIndex - startIndex + 1];
        if(isVertical)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                points[i - startIndex].x = lineIndex;
                points[i - startIndex].y = i;
            }
        }
        else
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                points[i - startIndex].x = i;
                points[i = startIndex].y = lineIndex;
            }
        }

        return points;
    }
}

public class MatchManager : MonoBehaviour
{
    private int[,] tileArray;

    //전체 타일 랜덤 생성 함수
    //최초 시작시와 매칭 할 수 있는게 없을 경우 호출
    public void MakeAllTile(ref int[,] _tileArray)
    {

    }

    //모든 타일에 대해 매칭 판별 함수
    //매칭에 해당하는 모든 리스트 반환
    public List<TileLine> CheckAllMatch(int[,] _tileArray)
    {
        List<TileLine> matchLines = new List<TileLine>();
        // 좌 -> 우
        matchLines.AddRange(CheckLineMatchAll(HEIGHT, WIDTH, false, new Vector2Int(1, 0)));
        // 상 -> 하
        matchLines.AddRange(CheckLineMatchAll(WIDTH, HEIGHT, true, new Vector2Int(0, 1)));

        return null;
    }
    
    //count가 2이상일 경우 3match로 판단
    private List<TileLine> CheckLineMatchAll(int line1, int line2, bool isVertical, Vector2Int checkPos)
    {
        List<TileLine> matchLines = new List<TileLine>();

        for (int i = 0; i < line1; i++)
        {
            for (int j = 0; j < line2; j++)
            {
                int count = (isVertical) ? CheckLineMatchFromOneTile(i, j, checkPos) : CheckLineMatchFromOneTile(j, i, checkPos);
                if(count >= 2)
                {
                    TileLine line = new TileLine(isVertical, i, j, j + count);
                    matchLines.Add(line);
                }

                j += count;
            }
        }

        return matchLines;
    }

    //기준 타일로부터 방향(_checkPos)에 같은 타일이 몇 개 있는지 반환
    private int CheckLineMatchFromOneTile(int _x, int _y, Vector2Int _checkPos)
    {
        int deltaX = _checkPos.x + _x;
        int deltaY = _checkPos.y + _y;
        int count = 0;
        while (false)
        {
            if(tileArray[_y, _x] == tileArray[deltaY, deltaX])
            {
                count++;
            }
            else
            {
                return count;
            }
            deltaX += _checkPos.x;
            deltaY += _checkPos.y;
        }
        return count;
    }

    //매칭된 타일 제거 함수
    public void RemoveMatchTile(ref int[,] _tileArray, List<TileLine> _tileLines)
    {
        foreach (TileLine tile in _tileLines)
        {
            Vector2Int[] points = tile.GetLinePoints();
            for (int i = 0; i < points.Length; i++)
            {
                _tileArray[points[i].y, points[i].x] = -1;
            }
        }
    }

    

    //제거된 타일의 빈공간 채우는 함수
    public void RefillTile(ref int[,] _tileArray, List<TileLine> _tileLines)
    {
        //리필 할 영역(세로라인) 표시
        bool[] refillArray = new bool[WIDTH];
        foreach(TileLine tile in _tileLines)
        {
            refillArray = tile.GetUseWidthTile(refillArray);
        }
        //리필 영역에 위에 있는 타일 내려 빈공간 채우고, 채우고 남은 최상단 빈 공간 갯수를 저장
        int[] spaceArray = new int[WIDTH];
        for (int i = 0; i < WIDTH; i++)
        {
            if(refillArray[i])
            {
                int space = 0;
                for (int j = HEIGHT; j >= 0; j--)
                {
                    if(_tileArray[j, i] == -1)
                    {
                        space++;
                    }
                    else
                    {
                        if(space > 0)
                        {
                            _tileArray[j + space, i] = _tileArray[j, i];
                            _tileArray[j, i] = -1;
                        }
                    }
                }
                spaceArray[i] = space;
            }
        }
        //최상단 빈 공간에 타일 새로 채운다
        for (int i = 0; i < WIDTH; i++)
        {
            if(refillArray[i])
            {
                for (int j = 0; j < spaceArray[i]; j++)
                {
                    if(_tileArray[j, i] == -1)
                    {
                        int index = Random.Range(0, 5);
                        _tileArray[j, i] = index;
                    }
                }
            }
        }
    }

    //채워진 타일이 매칭 가능한 패널이 존재하는지 확인.
    //존재하지 않는다면 타일 생성 다시
    public bool CheckCanMatch(int[,] _tileArray)
    {
        return false;
    }

    //특정 위치에 있는 타일에 대해 4방향으로 매칭이 있는지 확인하는 함수.
    //유저가 swap시 swap된 두 타일에 대해 사용한다
    public List<TileLine> CheckOneMatch(int[,] _tileArray, int _x, int _y)
    {
        return null;
    }

    const int WIDTH = 9;
    const int HEIGHT = 9;

    int[,] tileGrid = new int[HEIGHT, WIDTH];

    private void Start()
    {
        do
        {
            //최초 전체 타일 생성
            //타일에 match 할 수 있는게 없다면 다시 타일 전체 생성
            MakeAllTile(ref tileGrid);

            bool isMatch = true;
            while (isMatch)
            {
                //전체 타일 중 match 되는 타일 판별
                List<TileLine> matchLine = CheckAllMatch(tileGrid);

                //match가 있다면 제거하고 타일 리필한 다음 다시 전체 match 판별 진행
                if (matchLine.Count > 0)
                {
                    isMatch = true;
                    RemoveMatchTile(ref tileGrid, matchLine);

                    RefillTile(ref tileGrid, matchLine);
                }
                else
                {
                    isMatch = false;
                }
            }
        } while (!CheckCanMatch(tileGrid));
    }
}

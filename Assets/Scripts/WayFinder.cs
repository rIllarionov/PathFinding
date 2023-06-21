using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayFinder : MonoBehaviour
{
    private Tile[,] _tiles;
    private int[,] _map;
    private Vector2Int _startPosition;
    private static Stack<Coordinates> _way = new Stack<Coordinates>();

    private bool _wayIsFind;
    private List<Vector2Int> _trueWay;


    public List<Vector2Int> GetWay()
    {
        return _trueWay;
    }

    public bool WayIsFind()
    {
        return _wayIsFind;
    }
    public void TakeTiles(Tile[,] tiles)
    {
        _tiles = tiles;
        _map = new int[_tiles.GetLength(0), _tiles.GetLength(1)];

        CreateMap();
    }

    public Vector2Int GetStartPosition()
    {
        return _startPosition;
    }

    public void SetStartPosition(Vector2Int startPosition)
    {
        _startPosition = startPosition;
    }

    public void TryFindWayTo(Vector2Int point)
    {
        _wayIsFind = false;
        ResetWay();
        
        if (point.x < 0 || point.x > 9)
        {
            return;
        }

        if (point.y < 0 || point.y > 9)
        {
            return;
        }
        
        var cell = _map[point.x, point.y];

        if (cell == 0)
        {
            StartWayFinding(point);
        }
    }

    private void StartWayFinding(Vector2Int point)
    {
        var map = CopyMap();
        
        var startPosition = new Coordinates(_startPosition.x+1,_startPosition.y+1);
        var finish = new Coordinates(point.x+1,point.y+1);
        
        var step = 1;
        var start = 1;
        var end = 1;
        var numberStep = 1;

        Queue<Coordinates> points = new Queue<Coordinates>();
        
        points.Enqueue(startPosition);

        var isItFinish = false;

        while (true)
        {
            
            for (int i = start; i <= end; i++)
            {
                var currentPoint = points.Dequeue();
                
                var x = currentPoint.X;
                var y = currentPoint.Y;

                if (map[x + 1, y] == 0)
                {
                    map[x + 1, y] = step;
                    points.Enqueue(new Coordinates(x + 1, y,currentPoint));
                    numberStep++;
                }

                if (map[x - 1, y] == 0)
                {
                    map[x - 1, y] = step;
                    points.Enqueue(new Coordinates(x - 1, y,currentPoint));
                    numberStep++;
                }

                if (map[x, y + 1] == 0)
                {
                    map[x, y + 1] = step;
                    points.Enqueue(new Coordinates(x, y + 1,currentPoint));
                    numberStep++;
                }

                if (map[x, y - 1] == 0)
                {
                    map[x, y - 1] = step;
                    points.Enqueue(new Coordinates(x, y - 1,currentPoint));
                    numberStep++;
                }
                
                if (currentPoint.X == finish.X &&
                    currentPoint.Y == finish.Y)
                {
                    isItFinish = true;
                    
                    _way.Push(currentPoint);
                    break;
                }

            }

            if (isItFinish)
            {
                // заполняем стек верными координатами
                var node = _way.Peek().Previous;
                
                while (node != null)
                {
                    _way.Push(node);
                    node = node.Previous;
                }

                break;
            }
            
            if (points.Count==0)
            {
                isItFinish = false;
                break;
            }

            step++;
            start = end + 1;
            end = numberStep;
        }

        if (isItFinish)
        {
            TakeWayCoordinates();
        }
    }
    
    private void TakeWayCoordinates()
    {
        _trueWay = new List<Vector2Int>();
        
        while (_way.Count > 0)
        {
            var current = _way.Pop();
            _trueWay.Add(new Vector2Int(current.X-1,current.Y-1));
        }

        LightWay(_trueWay);

    }

    private void LightWay(List<Vector2Int> way)
    {
        for (int i = 0; i < way.Count; i++)
        {
            var tileAddress = way[i];
            
            _tiles[tileAddress.x,tileAddress.y].SetColor(true);
        }

        _wayIsFind = true;
    }

    private void ResetWay()
    {
        foreach (var tile in _tiles)
        {
            if (tile!=null)
            {
                tile.ResetColor();
            }
        }
    }

    private void CreateMap()
    {
        for (int i = 0; i < _tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _tiles.GetLength(1); j++)
            {
                if (_tiles[i, j] != null && _tiles[i, j].gameObject.name == "SimpleTile(Clone)")
                {
                    _map[i, j] = 0;
                }
                else
                {
                    _map[i, j] = -1;
                }
            }
        }

        _startPosition = SetStartPosition();
    }

    private int[,] CopyMap()
    {
        int[,] array = new int[_map.GetLength(0) + 2, _map.GetLength(1) + 2];

        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = -1;
            }
        }

        for (int i = 1; i < array.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < array.GetLength(1) - 1; j++)
            {
                array[i, j] = _map[i - 1, j - 1];
            }
        }

        return array;
    }

    private Vector2Int SetStartPosition()
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if (_map[i, j] == 0)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }
}

public class Coordinates
{
    public int X;
    public int Y;
    public Coordinates Previous;

    public Coordinates(int x, int y, Coordinates previous = null)
    {
        X = x;
        Y = y;
        Previous = previous;
    }
}
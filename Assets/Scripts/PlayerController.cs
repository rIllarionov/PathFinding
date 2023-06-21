using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private WayFinder _wayFinder;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private MapIndexProvider _indexProvider;
    [SerializeField] private float _moveSpeed = 1f;

    private GameObject _player;

    private bool _playerOnTheGround;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private Animator _animator;

    public void SetPlayer()
    {
        Vector2Int position = _wayFinder.GetStartPosition();

        if (position.x == -1)
        {
            return;
        }

        var tilePosition = _indexProvider.GetTilePosition(position);
        tilePosition.y = 1;
        _player = Instantiate(_playerPrefab, tilePosition, Quaternion.identity);
        _animator = _player.GetComponent<Animator>();
        _playerOnTheGround = true;
    }

    private void Update()
    {
        if (_playerOnTheGround)
        {
            CheckCellsAvailable();
        }

        if (_wayFinder.WayIsFind() && Input.GetMouseButtonDown(0))
        {
            PlayerGo(_wayFinder.GetWay());
        }
    }

    private void PlayerGo(List <Vector2Int> way)
    {
        _animator.SetBool(IsMoving, true);
        
        StartCoroutine(Moving(way));
    }
    
    private IEnumerator Moving(List<Vector2Int> way)
    {
        for (int currentNextPoint = 0; currentNextPoint < way.Count; currentNextPoint++)
        {
            var nextPosition = _indexProvider.GetTilePosition(way[currentNextPoint]);
            nextPosition.y = 1;

            _player.transform.LookAt(nextPosition);

            while (_player.transform.position != nextPosition)
            {
                _player.transform.position = Vector3.MoveTowards(_player.transform.position, nextPosition, _moveSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }

        _animator.SetBool(IsMoving, false);
        _wayFinder.SetStartPosition(way[^1]);
    }

    private void CheckCellsAvailable()
    {
        //взять позицию мышки экранных координатах
        //перевести в мировые
        //затем в локальные координаты карты и понять какая клетка соответствует мышке.
        //если в клетку можно добраться то расчитать маршрут

        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out var hitInfo))
        {
            // Получаем индекс и позицию тайла по позиции курсора
            var tileIndex = _indexProvider.GetIndex(hitInfo.point);
            _wayFinder.TryFindWayTo(tileIndex);
        }
    }
}
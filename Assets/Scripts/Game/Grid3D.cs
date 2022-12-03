using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace ArkadiumTest.Game
{
    public class Grid3D : MonoBehaviour
    {
        [SerializeField]
        private GridPointerComponent _pointerComponent;

        [SerializeField]
        private Vector3Int _dimensions;

        [SerializeField]
        private Transform _selectionMarker;
        private Vector3Int? _selected = null;

        [SerializeField]
        private List<Material> _materials;

        [SerializeField]
        private List<Transform> _transforms;

        [SerializeField]
        private List<Vector3Int> _coordinates;

        private Dictionary<Transform, Vector3Int> _coordinateTable;
        private Dictionary<Vector3Int, Transform> _transformTable;
        private Dictionary<Vector3Int, int> _symbolTable;

        private Action _onScore;
        private Action _onWin;

        private void Awake()
        {
            if (_transforms.Count % 2 != 0)
                throw new ArgumentOutOfRangeException("_transforms");

            _coordinateTable = new Dictionary<Transform, Vector3Int>(_transforms.Count);
            _transformTable = new Dictionary<Vector3Int, Transform>(_transforms.Count);
            _symbolTable = new Dictionary<Vector3Int, int>(_transforms.Count);

            for (int i = 0; i < _transforms.Count; i++)
            {
                _coordinateTable.Add(_transforms[i], _coordinates[i]);
                _transformTable.Add(_coordinates[i], _transforms[i]);
            }


            //Shuffle
            Vector3Int[] coordinates = _coordinates.ToArray();
            var random = new Random();
            coordinates = coordinates.OrderBy((x) => random.Next()).ToArray();
            Queue<Vector3Int> queue = new Queue<Vector3Int>(coordinates);

            Queue<Material> materials = new Queue<Material>(_materials);

            //Assign symbol
            int[] symbols = new int[_transforms.Count];
            while (queue.TryDequeue(out Vector3Int first) && queue.TryDequeue(out Vector3Int second))
            {
                Material material = materials.Dequeue();

                _transformTable[first].GetComponent<MeshRenderer>().material = _transformTable[second].GetComponent<MeshRenderer>().material = material;

                int symbol = _materials.IndexOf(material);
                _symbolTable.Add(first, symbol);
                _symbolTable.Add(second, symbol);

                materials.Enqueue(material);
            }

            _pointerComponent.enabled = false;
            _pointerComponent.RegisterOnClick(Select);

            _transforms = null;
            _coordinates = null;
            _materials = null;
        }

        public void StartGame(Action onScore, Action onWin)
        {
            _onScore = onScore;
            _onWin = onWin;
            _pointerComponent.enabled = true;
        }

        public void Stop()
        {
            _pointerComponent.enabled = false;
        }

        private void Select(Transform slot)
        {
            if (_coordinateTable.ContainsKey(slot))
            {
                Vector3Int selected = _coordinateTable[slot];
                if (!CanSelect(selected))
                    return;

                if (_selected.HasValue && _selected.Value != selected && _symbolTable[_selected.Value] == _symbolTable[selected])
                {
                    Transform other = _transformTable[_selected.Value];

                    _coordinateTable.Remove(slot);
                    _transformTable.Remove(selected);
                    _symbolTable.Remove(selected);

                    _coordinateTable.Remove(other);
                    _transformTable.Remove(_selected.Value);
                    _symbolTable.Remove(_selected.Value);

                    Destroy(slot.gameObject);
                    Destroy(other.gameObject);

                    _selected = null;
                    _selectionMarker.gameObject.SetActive(false);

                    _onScore.Invoke();

                    if (_coordinateTable.Count == 0)
                        _onWin.Invoke();
                }
                else
                {
                    _selected = selected;
                    _selectionMarker.position = slot.position;
                    _selectionMarker.gameObject.SetActive(true);
                }

            }
#if UNITY_EDITOR
            else
                Debug.LogWarning($"Unknown object selected: {slot.name}.", slot);
#endif
        }

        private bool CanSelect(Vector3Int coordinates)
        {
            Vector3Int left = coordinates + Vector3Int.left;
            Vector3Int right = coordinates + Vector3Int.right;

            if (!_transformTable.ContainsKey(left) || !_transformTable.ContainsKey(right))
            {
                Vector3Int forward = coordinates + Vector3Int.forward;
                Vector3Int back = coordinates + Vector3Int.back;

                return !_transformTable.ContainsKey(forward) || !_transformTable.ContainsKey(back);
            }
            else
                return false;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            _dimensions.x = Mathf.Max(1, _dimensions.x);
            _dimensions.y = Mathf.Max(1, _dimensions.y);
            _dimensions.z = Mathf.Max(1, _dimensions.z);

            _pointerComponent = GetComponent<GridPointerComponent>();
            if (_pointerComponent == null)
                _pointerComponent = gameObject.AddComponent<GridPointerComponent>();
        }
#endif
    }
}
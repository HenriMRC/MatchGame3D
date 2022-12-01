using ArkadiumTest.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace ArkadiumTest.Logic
{
    public class Grid3D : MonoBehaviour
    {
        [SerializeField]
        private Vector3Int _dimensions;

        [SerializeField]
        private Transform _selectionMarker;
        private int _selected = -1;


        [SerializeField]
        private List<Material> _materials;

        [SerializeField]
        private List<Transform> _transforms;

        private List<int> _symbols;
        private List<Vector3Int> _positions;

        private void Awake()
        {
            if (_transforms.Count % 2 != 0)
                throw new ArgumentOutOfRangeException("_transforms");

            //Shuffle
            Transform[] transforms = _transforms.ToArray();
            var random = new Random();
            transforms = transforms.OrderBy((x) => random.Next()).ToArray();
            Queue<Transform> queue = new Queue<Transform>(transforms);
            
            Queue<Material> materials = new Queue<Material>(_materials);

            //Assign symbol
            int []symbols = new int[_transforms.Count];
            while (queue.TryDequeue(out Transform first) && queue.TryDequeue(out Transform second))
            {
                Material material = materials.Dequeue();
                first.GetComponent<MeshRenderer>().material = second.GetComponent<MeshRenderer>().material = material;

                int symbol = _materials.IndexOf(material);
                symbols[_transforms.IndexOf(first)] = symbol;
                symbols[_transforms.IndexOf(second)] = symbol;

                materials.Enqueue(material);
            }

            _symbols = new List<int>(symbols);

            //Calculate positions
            _positions = new List<Vector3Int>(_transforms.Count);
            for (int i = 0; i < _transforms.Count; i++)
            {
                Vector3Int position = new Vector3Int(i % _dimensions.x, (i % (_dimensions.x * _dimensions.y)) / _dimensions.x, i / (_dimensions.x * _dimensions.y));
                _positions.Add(position);
            }

            GetComponent<GridMovement>().RegisterOnClick(Select);
        }

        private void Select(Transform slot)
        {
            int newSelected = _transforms.IndexOf(slot);

            if (newSelected < 0)
            {
                _selectionMarker.gameObject.SetActive(false);
                _selected = -1;
            }
            else if (newSelected != _selected)
            {
                if (_selected == -1 || _symbols[newSelected] != _symbols[_selected])
                {
                    _selected = newSelected;
                    _selectionMarker.position = slot.position;
                    _selectionMarker.gameObject.SetActive(true);
                }
                else
                {
                    //TODO: Verify adjacency
                    int first = Mathf.Max(newSelected, _selected);
                    int second = Mathf.Min(newSelected, _selected);

                    Destroy(_transforms[first].gameObject);
                    Destroy(_transforms[second].gameObject);

                    _transforms.RemoveAt(first);
                    _transforms.RemoveAt(second);

                    _symbols.RemoveAt(first);
                    _symbols.RemoveAt(second);

                    _positions.RemoveAt(first);
                    _positions.RemoveAt(second);

                    _selected = -1;
                    _selectionMarker.gameObject.SetActive(false);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _dimensions.x = Mathf.Max(1, _dimensions.x);
            _dimensions.y = Mathf.Max(1, _dimensions.y);
            _dimensions.z = Mathf.Max(1, _dimensions.z);
        }
#endif
    }
}

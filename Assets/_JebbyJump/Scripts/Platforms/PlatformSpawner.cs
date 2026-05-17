using System;
using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.Obstacles;
using JebbyJump.Platforms;
using UnityEngine;

namespace JebbyJump.Level
{
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField] private LevelConfig _config;
        [SerializeField] private GameObject _platformPrefab;
        [SerializeField] private GameObject _cactusPrefab;

        public event Action CactusHit;

        private readonly List<GameObject> _spawnedPlatforms = new List<GameObject>();
        private Transform _container;

        private void Awake()
        {
            if (_config == null) Debug.LogError("[PlatformSpawner] LevelConfig not assigned.", this);
            if (_platformPrefab == null) Debug.LogError("[PlatformSpawner] Platform prefab not assigned.", this);

            var containerGO = new GameObject("GeneratedPlatforms");
            _container = containerGO.transform;
        }

        public void SpawnPlatforms(IReadOnlyList<PlatformColor> sequence)
        {
            if (_config == null) { Debug.LogError("[PlatformSpawner] Cannot spawn - config not assigned.", this); return; }
            if (_platformPrefab == null) { Debug.LogError("[PlatformSpawner] Cannot spawn - prefab not assigned.", this); return; }
            if (sequence == null || sequence.Count == 0) { Debug.LogError("[PlatformSpawner] Cannot spawn - sequence is null or empty.", this); return; }

            foreach (var p in _spawnedPlatforms)
                if (p != null) Destroy(p);
            _spawnedPlatforms.Clear();

            for (int row = 0; row < sequence.Count; row++)
            {
                float rowY = _config.RowStartY + row * _config.RowVerticalSpacing;
                PlatformColor[] colors = BuildRowColors(sequence[row], _config.PlatformsPerRow, _config.AvailableColors);
                Vector3[] positions = GetRowPositions(rowY, _config.PlatformsPerRow, _config.RowHorizontalSpread);

                var distractors = new List<GameObject>();
                for (int i = 0; i < _config.PlatformsPerRow; i++)
                {
                    GameObject go = Instantiate(_platformPrefab, positions[i], Quaternion.identity, _container);
                    go.name = "Platform_Row" + row + "_" + colors[i];
                    go.transform.localScale = new Vector3(_config.PlatformWidth, _config.PlatformHeight, 1f);

                    Platform platform = go.GetComponent<Platform>();
                    if (platform == null)
                        Debug.LogError("[PlatformSpawner] Prefab has no Platform component.", go);
                    else
                        platform.Initialize(colors[i], row);

                    _spawnedPlatforms.Add(go);

                    if (colors[i] != sequence[row])
                        distractors.Add(go);
                }

                TrySpawnCactus(distractors);
            }

            Debug.Log("[PlatformSpawner] Spawned " + sequence.Count + " rows x " + _config.PlatformsPerRow + " platforms.");
        }

        private void TrySpawnCactus(List<GameObject> distractors)
        {
            if (_cactusPrefab == null || _config.CactusSpawnChance <= 0f || distractors.Count == 0) return;
            if (UnityEngine.Random.value >= _config.CactusSpawnChance) return;

            var target = distractors[UnityEngine.Random.Range(0, distractors.Count)];
            TrySpawnCactusOnPlatform(target);
        }

        private void TrySpawnCactusOnPlatform(GameObject targetPlatform)
        {
            var targetCollider = targetPlatform.GetComponent<Collider2D>();
            if (targetCollider == null)
            {
                Debug.LogError("[PlatformSpawner] Target platform has no Collider2D.", targetPlatform);
                return;
            }

            // Spawn under container, not under the scaled platform, to avoid inheriting non-uniform scale.
            var cactusGO = Instantiate(_cactusPrefab, _container);
            cactusGO.name = "Cactus_" + targetPlatform.name;

            var cactusCollider = cactusGO.GetComponent<Collider2D>();
            Bounds platformBounds = targetCollider.bounds;

            float cactusHalfWidth  = cactusCollider != null ? cactusCollider.bounds.extents.x : 0.2f;
            float cactusHalfHeight = cactusCollider != null ? cactusCollider.bounds.extents.y : 0.4f;
            const float edgeInset = 0.1f;

            cactusGO.transform.position = new Vector3(
                platformBounds.max.x - cactusHalfWidth - edgeInset,
                platformBounds.max.y + cactusHalfHeight,
                0f
            );

            var cactus = cactusGO.GetComponent<CactusObstacle>();
            if (cactus != null)
                cactus.PlayerHit += () => CactusHit?.Invoke();
        }

        private static PlatformColor[] BuildRowColors(PlatformColor correct, int count, PlatformColor[] available)
        {
            PlatformColor[] colors = new PlatformColor[count];
            colors[0] = correct;

            List<PlatformColor> distractors = new List<PlatformColor>();
            foreach (PlatformColor c in available)
                if (c != correct) distractors.Add(c);

            for (int i = 1; i < count; i++)
            {
                colors[i] = distractors.Count > 0
                    ? distractors[UnityEngine.Random.Range(0, distractors.Count)]
                    : available[UnityEngine.Random.Range(0, available.Length)];
            }

            for (int i = count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                PlatformColor tmp = colors[i];
                colors[i] = colors[j];
                colors[j] = tmp;
            }
            return colors;
        }

        private static Vector3[] GetRowPositions(float y, int count, float spread)
        {
            Vector3[] positions = new Vector3[count];
            if (count == 1)
            {
                positions[0] = new Vector3(0f, y, 0f);
                return positions;
            }
            float step = spread / (count - 1);
            for (int i = 0; i < count; i++)
                positions[i] = new Vector3(-spread / 2f + i * step, y, 0f);
            return positions;
        }
    }
}

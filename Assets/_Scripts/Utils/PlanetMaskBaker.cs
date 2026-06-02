using System.Collections;
using UnityEngine;

/// <summary>
/// PlanetMaskBaker
/// - Produces an equirectangular Texture2D (width x height)
///   R = continent mask (0..1)
///   G = flora mask (0..1)
///   B = elevation/noise (0..1)
/// - Assign the resulting texture to a material property (e.g. _PlanetMask)
/// - Run in editor or at runtime. Uses a coroutine to avoid frame hitches.
/// </summary>
[ExecuteAlways]
public class PlanetMaskBaker : MonoBehaviour
{
    [Header("Output")]
    public int width = 2048;
    public int height = 1024;
    public string materialTextureProperty = "_PlanetMask";
    public Material targetMaterial;

    [Header("Noise / Continent")]
    public int seed = 12345;
    public float baseScale = 1.0f;
    public int octaves = 5;
    [Range(0f, 1f)] public float continentThreshold = 0.45f;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    [Header("Flora")]
    [Range(0f, 1f)] public float globalFlora = 0.6f;
    public float floraNoiseScale = 6f;
    public float floraLatitudeBias = 1.0f; // >1 reduces flora near poles

    [Header("Runtime")]
    public bool autoBakeOnStart = true;
    public bool generateMipmaps = true;

    Texture2D maskTexture;
    System.Random rng;
    Vector2[] octaveOffsets;

    void Start()
    {
        if (autoBakeOnStart)
            StartBake();
    }

    [ContextMenu("Start Bake")]
    public void StartBake()
    {
        StopAllCoroutines();
        rng = new System.Random(seed);
        octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2((float)rng.NextDouble() * 10000f, (float)rng.NextDouble() * 10000f);
        }
        maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, generateMipmaps);
        maskTexture.wrapMode = TextureWrapMode.Repeat;
        maskTexture.filterMode = FilterMode.Bilinear;
        StartCoroutine(BakeCoroutine());
    }

    IEnumerator BakeCoroutine()
    {
        float invW = 1f / (width - 1);
        float invH = 1f / (height - 1);

        for (int y = 0; y < height; y++)
        {
            float v = y * invH; // 0..1
            float lat = (v - 0.5f) * Mathf.PI; // -pi/2 .. pi/2

            for (int x = 0; x < width; x++)
            {
                float u = x * invW; // 0..1
                float lon = u * Mathf.PI * 2f; // 0..2pi

                // Convert lat/lon to unit sphere direction
                float cosLat = Mathf.Cos(lat);
                Vector3 dir = new Vector3(
                    cosLat * Mathf.Cos(lon),
                    Mathf.Sin(lat),
                    cosLat * Mathf.Sin(lon)
                );

                // Sample layered noise (3D-ish via plane combos)
                float elevation = SampleLayeredNoise(dir);

                // Continent mask (binary-ish but smoothed)
                float continent = Mathf.SmoothStep(continentThreshold - 0.05f, continentThreshold + 0.05f, elevation);

                // Flora mask: only on land, modulated by globalFlora and a flora noise + latitude bias
                float floraNoise = SampleSimpleNoise(dir * floraNoiseScale + new Vector3(100, 100, 100));
                float latFactor = 1f - Mathf.Pow(Mathf.Abs(dir.y), floraLatitudeBias); // favors equator
                float flora = continent * globalFlora * Mathf.Clamp01(floraNoise) * latFactor;

                // Pack into color channels
                Color outCol = new Color(continent, flora, elevation, 1f);
                maskTexture.SetPixel(x, y, outCol);
            }

            // yield every row to keep UI responsive
            if (y % 8 == 0) yield return null;
        }

        maskTexture.Apply(generateMipmaps, false);

        // Assign to material
        if (targetMaterial != null && !string.IsNullOrEmpty(materialTextureProperty))
        {
            targetMaterial.SetTexture(materialTextureProperty, maskTexture);
        }

        Debug.Log($"PlanetMaskBaker: Bake complete ({width}x{height}).");
    }

    // Layered noise using 2D Perlin on different planes to approximate 3D noise
    float SampleLayeredNoise(Vector3 dir)
    {
        float amplitude = 1f;
        float frequency = baseScale;
        float noiseSum = 0f;
        float amplitudeSum = 0f;

        for (int i = 0; i < octaves; i++)
        {
            Vector2 offset = octaveOffsets[i];
            // sample three planar Perlin calls and average to approximate 3D
            float n1 = Mathf.PerlinNoise(dir.x * frequency + offset.x, dir.y * frequency + offset.y);
            float n2 = Mathf.PerlinNoise(dir.y * frequency + offset.x, dir.z * frequency + offset.y);
            float n3 = Mathf.PerlinNoise(dir.z * frequency + offset.x, dir.x * frequency + offset.y);
            float n = (n1 + n2 + n3) / 3f;

            noiseSum += n * amplitude;
            amplitudeSum += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return Mathf.Clamp01(noiseSum / amplitudeSum);
    }

    // Simple single octave noise helper (same planar trick)
    float SampleSimpleNoise(Vector3 dir)
    {
        float n1 = Mathf.PerlinNoise(dir.x, dir.y);
        float n2 = Mathf.PerlinNoise(dir.y, dir.z);
        float n3 = Mathf.PerlinNoise(dir.z, dir.x);
        return (n1 + n2 + n3) / 3f;
    }

    // Optional: expose the generated texture for other scripts
    public Texture2D GetMaskTexture()
    {
        return maskTexture;
    }
}

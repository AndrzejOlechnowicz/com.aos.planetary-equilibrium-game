using System;
using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// PlanetHeightBaker
/// - Produces a normalized equirectangular heightmap (0..1)
/// - Two-pass: sample noise into float buffer, compute min/max, remap, write Texture2D
/// - Assigns texture to material property (default "_HeightMap")
/// </summary>
[ExecuteAlways]
public class PlanetHeightBaker : MonoBehaviour
{
    public event Action OnBakeCompleted;

    [Header("Output")]
    public int width = 2048;
    public int height = 1024;
    public string materialTextureProperty = "_HeightMap";
    public Material targetMaterial;

    [Header("Noise")]
    public int seed = 12345;
    public float baseScale = 1.0f;
    public int octaves = 5;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    [Header("Contrast / Terrain")]
    [Range(0.5f, 3f)] public float elevationContrast = 1.6f;
    [Range(-0.2f, 0.2f)] public float elevationBias = -0.05f;

    [Header("Runtime")]
    public bool autoBakeOnStart = true;
    public bool generateMipmaps = true;
    public float bakeProgress;

    Texture2D heightTexture;
    System.Random rng;
    Vector2[] octaveOffsets;
    float[] sampleBuffer; // width*height floats


    private void OnDestroy()
    {
        if (heightTexture != null)
        {
            Destroy(heightTexture);
        }
    }

    public async void StartBakeAsync()
    {
        rng = new System.Random(seed);
        octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
            octaveOffsets[i] = new Vector2((float)rng.NextDouble() * 10000f, (float)rng.NextDouble() * 10000f);

        heightTexture = new Texture2D(width, height, TextureFormat.RGBA32, generateMipmaps);
        heightTexture.wrapMode = TextureWrapMode.Repeat;
        heightTexture.filterMode = FilterMode.Bilinear;

        sampleBuffer = new float[width * height];

        await BakeHeightTextureAsync();
    }

    public async Awaitable BakeHeightTextureAsync()
    {
        int w = width, h = height;
        float invW = 1f / (w - 1);
        float invH = 1f / (h - 1);

        // First pass: sample noise into buffer and apply bias/contrast
        float minV = float.MaxValue;
        float maxV = float.MinValue;

        await Awaitable.BackgroundThreadAsync();

        var pixels = new Color32[width * height];

        for (int y = 0; y < h; y++)
        {
            float v = y * invH;
            float lat = (v - 0.5f) * Mathf.PI;

            for (int x = 0; x < w; x++)
            {
                float u = x * invW;
                float lon = u * Mathf.PI * 2f;

                float cosLat = Mathf.Cos(lat);
                Vector3 dir = new Vector3(
                    cosLat * Mathf.Cos(lon),
                    Mathf.Sin(lat),
                    cosLat * Mathf.Sin(lon)
                );

                float raw = SampleLayeredNoise(dir);

                // bias + contrast
                float elev = Mathf.Clamp01(Mathf.Pow(Mathf.Clamp01(raw + elevationBias), elevationContrast));

                int idx = y * w + x;
                sampleBuffer[idx] = elev;
                if (elev < minV) minV = elev;
                if (elev > maxV) maxV = elev;
            }
        }

        // Avoid degenerate case
        if (maxV - minV < 1e-5f)
        {
            minV = 0f;
            maxV = 1f;
        }

        // Second pass: remap to 0..1 and write texture
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = y * w + x;
                float v = (sampleBuffer[idx] - minV) / (maxV - minV);
                v = Mathf.Clamp01(v);

                byte value = (byte)(v * 255);
                Color32 color = new Color32(value, value, value, 255);
                pixels[y * w + x] = color;
            }

        }

        await Awaitable.MainThreadAsync();
        destroyCancellationToken.ThrowIfCancellationRequested();

        heightTexture.SetPixels32(pixels);
        heightTexture.Apply(generateMipmaps, false);

        if (targetMaterial != null && !string.IsNullOrEmpty(materialTextureProperty))
            targetMaterial.SetTexture(materialTextureProperty, heightTexture);


        Debug.Log($"PlanetHeightBaker_Normalized: Bake complete ({width}x{height}). min={minV:F4} max={maxV:F4}");
        OnBakeCompleted?.Invoke();
    }

    float SampleLayeredNoise(Vector3 dir)
    {
        float amplitude = 1f;
        float frequency = baseScale;
        float noiseSum = 0f;
        float amplitudeSum = 0f;

        for (int i = 0; i < octaves; i++)
        {
            Vector2 off = octaveOffsets[i];
            float n1 = Mathf.PerlinNoise(dir.x * frequency + off.x, dir.y * frequency + off.y);
            float n2 = Mathf.PerlinNoise(dir.y * frequency + off.x, dir.z * frequency + off.y);
            float n3 = Mathf.PerlinNoise(dir.z * frequency + off.x, dir.x * frequency + off.y);
            float n = (n1 + n2 + n3) / 3f;

            noiseSum += n * amplitude;
            amplitudeSum += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return Mathf.Clamp01(noiseSum / amplitudeSum);
    }

    // expose texture
    public Texture2D GetHeightTexture() => heightTexture;
}

using UnityEditor;
using UnityEngine;
using UnityVolumeRendering;

public class CreateTexture3DEditoerScript : MonoBehaviour
{
    [MenuItem("CreateExamples/3DTexture")]
    static void CreateTexture3D()
    {
        // Configure the texture
        int size = 32;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        float inverseResolution = 1.0f / (size - 1.0f);
        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    colors[x + yOffset + zOffset] = new Color(x * inverseResolution,
                        y * inverseResolution, z * inverseResolution, 1.0f);
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();        

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/Example3DTexture.asset");
    }
    [MenuItem("CreateExamples/VolumeDataset")]
    static void CreateVolumeDataset()
    {
        Debug.Log("CreateVolumeDataset");

        AssetDatabase.CreateAsset(GenerateVolumeData(), "Assets/VolumeDataset_0.asset");
    }
    private static VolumeDataset GenerateVolumeData()
    {
        var DimX = 16;//Dataset.dimX;
        var DimY = 16;//Dataset.dimY;
        var DimZ = 16;//Dataset.dimZ;
        VolumeDataset dataset = ScriptableObject.CreateInstance<VolumeDataset>();
        dataset.datasetName = $"generatedData_{DimX}*{DimY}*{DimZ}";
        dataset.filePath = string.Empty;
        dataset.dimX = DimX;
        dataset.dimY = DimY;
        dataset.dimZ = DimZ;

        int uDimension = DimX * DimY * DimZ;
        dataset.data = new float[uDimension];

        var maxValue = DimX * DimX * 0.25f + DimZ * DimZ * 0.25f;
        for (var y = 0; y < DimY; y++)
        {
            for (var x = 0; x < DimX; x++)
            {
                for (var z = 0; z < DimZ; z++)
                {
                    var v = ((x - DimX / 2) * (x - DimX / 2) + (z - DimZ / 2) * (z - DimZ / 2)) / maxValue;
                    dataset.data[y * DimX * DimZ + x * DimZ + z] = v > 0.25f ? 0f : 1f;
                }
            }
        }
        Debug.Log("Loaded dataset in range: " + dataset.GetMinDataValue() + "  -  " + dataset.GetMaxDataValue());
        dataset.FixDimensions();
        dataset.rotation = Quaternion.Euler(-90.0f, -90.0f, 0.0f);
        return dataset;
    }
}
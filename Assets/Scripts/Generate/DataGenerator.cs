using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityVolumeRendering
{
    public class DataGenerator : MonoBehaviour
    {
        public float GrainSize = 0.01f;
        public int DimX;
        public int DimY;
        public int DimZ;


        private void Start()
        {
            VolumeObjectFactory.CreateObject(GenerateVolumeData());
        }

        private VolumeDataset GenerateVolumeData()
        {
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
}
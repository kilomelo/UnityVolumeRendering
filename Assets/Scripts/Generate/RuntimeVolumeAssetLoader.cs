using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityVolumeRendering;

public class RuntimeVolumeAssetLoader : MonoBehaviour
{
    public VolumeDataset Dataset;
    private Texture3D _tex3D;
    private NativeArray<byte> _colorBuffer;
    // Start is called before the first frame update
    void Start()
    {
        if (null != Dataset)
        {
            // EditData();
            var obj = VolumeObjectFactory.CreateObject(Dataset);
            var meshRenderer = obj.meshRenderer;
            Debug.Log($"MeshRenderer: {meshRenderer}");
            _tex3D = meshRenderer.material.GetTexture("_DataTex") as Texture3D;
            _colorBuffer = _tex3D.GetPixelData<byte>(0);
            Debug.Log($"tex3D: {_tex3D}");
            // VolumeObjectFactory.CreateObject(GenerateVolumeData());
            StartCoroutine(UpdateTex3D());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator UpdateTex3D()
    {
        int uDimension = Dataset.dimX * Dataset.dimY * Dataset.dimZ;

        for (var i = 0; i < uDimension; i++)
        {
            if (null == _tex3D) break;
            yield return new WaitForSeconds(.1f);
            Debug.Log($"color[{i}] = {_colorBuffer[i]}");
            // Dataset
            // _tex3D
        }
    }

    // private void EditData()
    // {
    //     Debug.Log("RuntimeVolumeAssetLoader.EditData");
    //     var maxValue = Dataset.dimX * 0.25f + Dataset.dimZ * 0.25f;
    //     for (var y = 0; y < Dataset.dimY; y++)
    //     {
    //         for (var x = 0; x < Dataset.dimX; x++)
    //         {
    //             for (var z = 0; z < Dataset.dimZ; z++)
    //             {
    //                 var v = ((x - Dataset.dimX / 2) * (x - Dataset.dimX / 2) + (z - Dataset.dimZ / 2) * (z - Dataset.dimZ / 2)) / maxValue;
    //                 Dataset.data[y * Dataset.dimX * Dataset.dimZ + x * Dataset.dimZ + z] = v > 0.25f ? 0f : 1f;
    //             }
    //         }
    //     }
    //     Dataset.FixDimensions();
    //     Dataset.rotation = Quaternion.Euler(-90.0f, -90.0f, 0.0f);
    // }
}

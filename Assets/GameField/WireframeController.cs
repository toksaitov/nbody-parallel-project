using System.Collections.Generic;

using UnityEngine;

public class WireframeController : MonoBehaviour {
    [HideInInspector]
    public List<PlanetController> Planets;

    private Texture2D bodyDataTexture;

    private int bodyDataTexturePropertyID;
    private int bodyCountProperyID;

    private Material material;

    void Start () {
        Planets =
            new List<PlanetController> ();

        bodyDataTexturePropertyID =
            Shader.PropertyToID ("_BodyDataTex");
        bodyCountProperyID =
            Shader.PropertyToID ("_BodyCount");

        material =
            GetComponent<Renderer> ().material;

        RecreateBodyDataTexture ();
        UpdateBodyDataTextureData ();
    }

    void Update () {
        if (Planets.Count > bodyDataTexture.width * bodyDataTexture.height) {
            RecreateBodyDataTexture ();
        }

        UpdateBodyDataTextureData ();
    }

    private void RecreateBodyDataTexture () {
        const int TextureSizeLimit =
            2048;

        int planetCount =
            Planets.Count;

        int textureWidth;
        int textureHeight;

        int bodyCountByColumns = planetCount;
        if (bodyCountByColumns > TextureSizeLimit) {
            Debug.Assert (
                planetCount < TextureSizeLimit * TextureSizeLimit
            );

            bodyCountByColumns =
                TextureSizeLimit;
            textureWidth =
                bodyCountByColumns;
            textureHeight =
                bodyCountByColumns;
        } else {
            // generate a square power-of-two texture as GPUs like square power-of-two textures
            textureWidth = 2;
            while (textureWidth < bodyCountByColumns) {
                textureWidth *= 2;
            }
            textureHeight = 2;
            while (textureHeight < bodyCountByColumns) { // yes, by column to make it square
                textureHeight *= 2;
            }
        }

        bool mipmaps =
            false;

        bodyDataTexture =
            new Texture2D (
                textureWidth,
                textureHeight,
                TextureFormat.RGBAFloat,
                mipmaps
            );
        bodyDataTexture.name =
            "BodyDataTexture";
        bodyDataTexture.filterMode =
            FilterMode.Point;
        bodyDataTexture.wrapMode =
            TextureWrapMode.Clamp;
        bodyDataTexture.anisoLevel =
            0;

        Color [] data =
            new Color [textureHeight * textureWidth];

        for (int y = 0; y < bodyDataTexture.height; ++y) {
            for (int x = 0; x < bodyDataTexture.width; ++x) {
                data [x + y * textureWidth] =
                    Color.black;
            }
        }
        bodyDataTexture.SetPixels (data);
        bodyDataTexture.Apply ();

        material.SetTexture (
            bodyDataTexturePropertyID,
            bodyDataTexture
        );
    }

    private void UpdateBodyDataTextureData () {
        int planetCount = Planets.Count;
        if (planetCount > 0) {
            int textureWidth =
                bodyDataTexture.height;
            int textureHeight =
                bodyDataTexture.width;

            Color [] data =
                new Color [textureHeight * textureWidth];

            int i = 0;
            for (int y = 0; y < textureHeight; ++y) {
                for (int x = 0; x < textureWidth; ++x) {
                    if (i < Planets.Count) {
                        PlanetController planet =
                            Planets [i++];

                        Vector3 planetPosition =
                            planet.transform.localPosition;
                        float planetMass =
                            planet.Mass;

                        Color positionAsColor =
                            new Color (
                                planetPosition.x,
                                planetPosition.y,
                                planetPosition.z,
                                planetMass
                            );

                        data [x + y * textureWidth] =
                            positionAsColor;
                    } else {
                        data [x + y * textureWidth] =
                            Color.black;
                    }
                }
            }
            bodyDataTexture.SetPixels (data);
            bodyDataTexture.Apply ();

            material.SetInt (
                bodyCountProperyID,
                planetCount
            );
        }
    }
}


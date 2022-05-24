using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Lip;

public class LipTrackingBehaviour : MonoBehaviour
{
    [SerializeField] private List<LipShapeTable_v2> lipShapeTables;

    private Dictionary<LipShape_v2, float> _lipWeightings;
    
    private void Start()
    {
        if (!SRanipal_Lip_Framework.Instance.EnableLip)
        {
            enabled = false;
            return;
        }
        SetLipShapeTables(lipShapeTables);
    }

    // Update is called once per frame
    private void Update()
    {                    
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) return;

        SRanipal_Lip_v2.GetLipWeightings(out _lipWeightings);
    }

    public void SetLipShapeTables(List<LipShapeTable_v2> lipTables)
    {                    
        bool valid = true;
        if (lipTables == null)
        {
            valid = false;
        }
        else
        {
            for (int table = 0; table < lipTables.Count; ++table)
            {
                if (lipTables[table].skinnedMeshRenderer == null)
                {
                    valid = false;
                    break;
                }
                for (int shape = 0; shape < lipTables[table].lipShapes.Length; ++shape)
                {
                    LipShape_v2 lipShape = lipTables[table].lipShapes[shape];
                    if (lipShape > LipShape_v2.Max || lipShape < 0)
                    {
                        valid = false;
                        break;
                    }
                }
            }
        }
        if (valid)
            lipShapeTables = lipTables;
    }

    public void UpdateLipShapes(Dictionary<LipShape_v2, float> lipWeights)
    {
        foreach (var table in lipShapeTables)
            RenderModelLipShape(table, lipWeights);
    }

    private void RenderModelLipShape(LipShapeTable_v2 lipShapeTable, Dictionary<LipShape_v2, float> weighting)
    {                    
        for (int i = 0; i < lipShapeTable.lipShapes.Length; i++)
        {
            int targetIndex = (int)lipShapeTable.lipShapes[i];
            if (targetIndex > (int)LipShape_v2.Max || targetIndex < 0) continue;
            lipShapeTable.skinnedMeshRenderer.SetBlendShapeWeight(i, weighting[(LipShape_v2)targetIndex] * 100);
        }
    }

    public Dictionary<LipShape_v2, float> GetLipWeightsDict()
    {
        return _lipWeightings;
    }
}

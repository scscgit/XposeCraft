using System;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.GameInternal;

namespace XposeCraft.Core.Faction
{
    public class FactionColorChange : MonoBehaviour
    {
        public Renderer[] MaterialRenderer;

        private Material[][] _originalMaterials;
        private UnitController _unitController;
        private BuildingController _buildingController;
        private VisionReceiver _visionReceiver;

        private void Initialize()
        {
            _unitController = GetComponent<UnitController>();
            _buildingController = GetComponent<BuildingController>();
            if (_unitController == null && _buildingController == null)
            {
                throw new Exception(typeof(FactionColorChange) + " cannot find "
                                    + typeof(UnitController) + " or " + typeof(BuildingController));
            }
            _visionReceiver = GetComponent<VisionReceiver>();
        }

        private void OnEnable()
        {
            if (_visionReceiver != null)
            {
                // This has to be executed after Vision Receiver, effectively as a callback
                return;
            }
            ChangeColors();
        }

        public void ChangeColors()
        {
            if (_unitController == null && _buildingController == null)
            {
                Initialize();
            }
            var factionIndex = _unitController != null
                ? _unitController.FactionIndex
                : _buildingController.FactionIndex;
            // Only colors of enemies will be changed if that is the current setting
            if (GameManager.Instance.NoColorChangeMyFaction && factionIndex == 0)
                // Alternative condition could handle the current GUI player, but the old colors wouldn't return
                //&& factionIndex == GameManager.Instance.GuiPlayer.FactionIndex)
            {
                return;
            }
            _originalMaterials = new Material[MaterialRenderer.Length][];
            // Duplicate materials and set colors based on factions
            for (var materialsIndex = 0; materialsIndex < MaterialRenderer.Length; materialsIndex++)
            {
                var materials = MaterialRenderer[materialsIndex];
                _originalMaterials[materialsIndex] = new Material[materials.materials.Length];
                for (var materialIndex = 0; materialIndex < _originalMaterials[materialsIndex].Length; materialIndex++)
                {
                    _originalMaterials[materialsIndex][materialIndex] = materials.materials[materialIndex];
                    //var materialCopy = Instantiate(materials.materials[materialIndex]);
                    // Modifying any material in materials will change the appearance of only that object.
                    materials.materials[materialIndex].color = GameManager.Instance.Factions[factionIndex].Color;
                    //materials.materials[materialIndex] = materialCopy;
                }
            }
        }

        private void OnDisable()
        {
            // OnDisable is not called if VisionReceiver controls the lifecycle
            if (_visionReceiver != null)
            {
                return;
            }
            ReturnColors();
        }

        private void ReturnColors()
        {
            for (var materialsIndex = 0; materialsIndex < MaterialRenderer.Length; materialsIndex++)
            {
                // If a material isn't saved, don't rollback it
                if (_originalMaterials[materialsIndex] == null)
                {
                    continue;
                }
                var materials = MaterialRenderer[materialsIndex];
                for (var materialIndex = 0;
                    materialIndex < materials.materials.Length;
                    materialIndex++)
                {
                    Destroy(materials.materials[materialIndex]);
                    materials.materials[materialIndex] = _originalMaterials[materialsIndex][materialIndex];
                }
            }
        }
    }
}

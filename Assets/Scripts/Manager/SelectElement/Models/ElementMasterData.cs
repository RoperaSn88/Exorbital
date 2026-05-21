using System.Collections.Generic;
using UnityEngine;

namespace Manager.SelectElement.Models
{
    [CreateAssetMenu(fileName = "ElementMasterData", menuName = "SelectElement/Models/ElementMasterData")]
    public class ElementMasterData: ScriptableObject
    {

        [SerializeField]
        private ElementModel[] _elements;
        public IReadOnlyList<ElementModel> Elements => _elements;
        
    }
}
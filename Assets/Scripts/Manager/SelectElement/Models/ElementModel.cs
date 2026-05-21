using System;
using UnityEngine;

namespace Manager.SelectElement.Models
{
    [System.Serializable]
    public class ElementModel
    {
        [SerializeField]
        private ElementKinds _elementKind;
        public ElementKinds ElementKind => _elementKind;
        
        [SerializeField]
        private string _elementName;
        public String ElementName => _elementName;
        
        [SerializeField,TextArea]
        private string _elementDescription;
        public String ElementDescription => _elementDescription;
        
    }
}
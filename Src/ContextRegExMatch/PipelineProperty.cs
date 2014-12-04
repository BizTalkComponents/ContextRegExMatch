using System.ComponentModel;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;

namespace BizTalkComponents.ContextRegExMatch
{
    public class PipelineProperty
    {
        [Browsable(true)]
        private readonly string _id;
        private string _displayName;
        private string _description;
        
        public string Value;

        public PipelineProperty(string id, string displayDisplayName, string description)
        {
            _displayName = displayDisplayName;
            _description = description;
            _id = id;
        }

        public string Load(IPropertyBag pgh)
        {
            Value = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(pgh, _id), string.Empty);

            return Value;
        }

        public void Save(IPropertyBag pgh)
        {
            PropertyBagHelper.WritePropertyBag(pgh, _id, Value);
        }
    }
}

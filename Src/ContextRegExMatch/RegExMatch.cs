using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

namespace BizTalkComponents.ContextRegExMatch
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [Guid("E0105181-E664-4770-B20B-6EB5AF77DBA0")]

    public partial class RegExMatchComponent : IBaseComponent, Microsoft.BizTalk.Component.Interop.IComponent, IComponentUI, IPersistPropertyBag
    {
        [DisplayName("Pattern To Match")]
        [Description("The RegEx pattern used to match the specific context value against.")]
        public string PatternToMatch { get; set; }
        private const string PatternToMatchPropertyName = "PatternToMatch";

        [DisplayName("Context Property To Match")]
        [Description("Namespace and value of the context property that the match should execute against. Should be in format 'http://foo.bar#value'.")]
        public string ContextNamespaceToMatch { get; set; }
        private const string ContextNamespaceToMatchPropertyName = "ContextNamespaceToMatch";

        [DisplayName("Context Property To Set")]
        [Description("Namespace and value of the context property that should be used to the set the value. Should be in format 'http://foo.bar#value'.")]
        public string ContextNamespaceToSet { get; set; }
        private const string ContextNamespaceToSetPropertyName = "ContextNamespaceToSet";

        [DisplayName("Value To Set")]
        [Description("The value to set if the match i successful")]
        public string ValueToSet { get; set; }
        private const string ValueToSetPropertyName = "ValueToSet";
        
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            if (string.IsNullOrEmpty(PatternToMatch))
                PatternToMatch = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, PatternToMatchPropertyName), string.Empty);
            

            if (string.IsNullOrEmpty(ValueToSet))
                ValueToSet = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ValueToSetPropertyName), string.Empty);
            

            if (string.IsNullOrEmpty(ContextNamespaceToMatch))
                ContextNamespaceToMatch = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ContextNamespaceToMatchPropertyName), string.Empty);
            

            if (string.IsNullOrEmpty(ContextNamespaceToSet))
                ContextNamespaceToSet = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ContextNamespaceToMatchPropertyName), string.Empty);
            
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, PatternToMatchPropertyName, PatternToMatch);
            PropertyBagHelper.WritePropertyBag(propertyBag, ValueToSetPropertyName, ValueToSet);
            PropertyBagHelper.WritePropertyBag(propertyBag, ContextNamespaceToMatchPropertyName, ContextNamespaceToMatch);
            PropertyBagHelper.WritePropertyBag(propertyBag, ContextNamespaceToSetPropertyName, ContextNamespaceToSet);
        }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if(string.IsNullOrEmpty(PatternToMatch))
                throw new ArgumentException("Pattern property to use for RegEx match cannot be null");

            if (string.IsNullOrEmpty(ContextNamespaceToMatch))
                throw new ArgumentException("Context property to match cannot be null");

            if (string.IsNullOrEmpty(ContextNamespaceToSet))
                throw new ArgumentException("Context property to set cannot be null");
            
            if (string.IsNullOrEmpty(ValueToSet))
                throw new ArgumentException("Value to set cannot be null");

            var contextNamespaceToSetProperty = new ContextProperty(ContextNamespaceToSet);
            var contextNamespaceToMatchProperty = new ContextProperty(ContextNamespaceToMatch);

            pInMsg.BodyPart.Data = ReadStreamToEndAndSeekToBeginning(pInMsg.BodyPart.Data, true, 1048576);

            var regex = new Regex(PatternToMatch);

            string value;
            var result = pInMsg.Context.TryRead(contextNamespaceToMatchProperty, out value);

            if (!result)
                throw new ArgumentException("Context property to use for RegEx match can be null");

            if (regex.IsMatch(value))
                pInMsg.Context.Promote(contextNamespaceToSetProperty.PropertyName, contextNamespaceToSetProperty.PropertyNamespace, ValueToSet);

            return pInMsg;
        }

        Stream ReadStreamToEndAndSeekToBeginning(Stream data, bool seekToBeginning, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            Stream outputStream = data;

            outputStream = new ReadOnlySeekableStream(outputStream);

            while (0 != outputStream.Read(buffer, 0, buffer.Length)) { }

            if (seekToBeginning)
                outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }
    }
}

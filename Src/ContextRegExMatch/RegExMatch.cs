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
    [ComponentCategory(CategoryTypes.CATID_Receiver)]
    [Guid("E0105181-E664-4770-B20B-6EB5AF77DBA0")]

    public partial class RegExMatchComponent : IBaseComponent, Microsoft.BizTalk.Component.Interop.IComponent, IComponentUI, IPersistPropertyBag
    {
        [RequiredRuntime]
        [DisplayName("Pattern To Match")]
        [Description("The RegEx pattern used to match the specific context value against.")]
        public string PatternToMatch { get; set; }
        private const string PatternToMatchPropertyId = "PatternToMatch";

        [RequiredRuntime]
        [DisplayName("Context Property To Match")]
        [Description("Namespace and value of the context property that the match should execute against. Should be in format 'http://foo.bar#value'.")]
        public string ContextNamespaceToMatch { get; set; }
        private const string ContextNamespaceToMatchPropertyId = "ContextNamespaceToMatch";

        [RequiredRuntime]
        [DisplayName("Context Property To Set")]
        [Description("Namespace and value of the context property that should be used to the set the value. Should be in format 'http://foo.bar#value'.")]
        public string ContextNamespaceToSet { get; set; }
        private const string ContextNamespaceToSetPropertyId = "ContextNamespaceToSet";

        [RequiredRuntime]
        [DisplayName("Value To Set")]
        [Description("The value to set if the match i successful")]
        public string ValueToSet { get; set; }
        private const string ValueToSetPropertyId = "ValueToSet";

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            PatternToMatch = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, PatternToMatchPropertyId), string.Empty);
            ValueToSet = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ValueToSetPropertyId), string.Empty);
            ContextNamespaceToMatch = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ContextNamespaceToMatchPropertyId), string.Empty);
            ContextNamespaceToSet = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, ContextNamespaceToMatchPropertyId), string.Empty);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, PatternToMatchPropertyId, PatternToMatch);
            PropertyBagHelper.WritePropertyBag(propertyBag, ValueToSetPropertyId, ValueToSet);
            PropertyBagHelper.WritePropertyBag(propertyBag, ContextNamespaceToMatchPropertyId, ContextNamespaceToMatch);
            PropertyBagHelper.WritePropertyBag(propertyBag, ContextNamespaceToSetPropertyId, ContextNamespaceToSet);
        }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            var contextNamespaceToSetProperty = new ContextProperty(ContextNamespaceToSet);
            var contextNamespaceToMatchProperty = new ContextProperty(ContextNamespaceToMatch);

            pInMsg.BodyPart.Data = ReadStreamToEndAndSeekToBeginning(pInMsg.BodyPart.Data, true, 1048576);

            var regex = new Regex(PatternToMatch);

            string value;
            var result = pInMsg.Context.TryRead(contextNamespaceToMatchProperty, out value);

            if (!result)
                throw new ArgumentException("Context property value to use for RegEx match can be null");

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

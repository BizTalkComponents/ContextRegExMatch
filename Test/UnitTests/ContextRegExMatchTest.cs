using System;
using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;

namespace BizTalkComponents.PipelineComponents.ContextRegExMatch.Tests.UnitTests
{
    [TestClass]
    public class ContextRegExMatchTest
    {
        [TestMethod]
        public void TestMatchPatterSimple()
        {
            const string contextNamespaceToSet = "http://acmeset.com/prop#test";
            const string contextNamespaceToMatch = "http://acmematch.com/prop#test";

            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Promote(contextNamespaceToMatch.Split('#')[1], contextNamespaceToMatch.Split('#')[0], "752368");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent component = new RegExMatchComponent();

            ((RegExMatchComponent)component).ContextNamespaceToSet = contextNamespaceToSet;
            ((RegExMatchComponent)component).ContextNamespaceToMatch = contextNamespaceToMatch;
            ((RegExMatchComponent)component).PatternToMatch = "^.{0}75";
            ((RegExMatchComponent)component).ValueToSet = "OK";

            sendPipeline.AddComponent(component, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsTrue(result.Context.Read(contextNamespaceToSet.Split('#')[1], contextNamespaceToSet.Split('#')[0]).ToString() == "OK");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMissingPropertyToMatch()
        {
            const string contextNamespaceToSet = "http://acmeset.com/prop#test";
            const string contextNamespaceToMatch = "http://acmematch.com/prop#test";

            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Promote(contextNamespaceToMatch.Split('#')[1], contextNamespaceToMatch.Split('#')[0], "752368");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent component = new RegExMatchComponent();

            ((RegExMatchComponent)component).ContextNamespaceToSet = contextNamespaceToSet;
            //((RegExMatchComponent)component).ContextNamespaceToMatch = contextNamespaceToMatch;
            ((RegExMatchComponent)component).PatternToMatch = "^.{0}75";
            ((RegExMatchComponent)component).ValueToSet = "OK";

            sendPipeline.AddComponent(component, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsTrue(result.Context.Read("test", "http://XXX.com/prop").ToString() == "OK");
        }

        [TestMethod]
        public void TestMatchPatterSequentialComponent()
        {
            const string firstContextNamespaceToSet = "http://acmeset1.com/prop#test";
            const string secondContextNamespaceToSet = "http://acmeset2.com/prop#test";
            const string contextNamespaceToMatch = "http://acmematch.com/prop#test";

            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Promote(contextNamespaceToMatch.Split('#')[1], contextNamespaceToMatch.Split('#')[0], "752368");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent firstComponent = new RegExMatchComponent();
            IBaseComponent secondComponent = new RegExMatchComponent();

            ((RegExMatchComponent)firstComponent).ContextNamespaceToSet = firstContextNamespaceToSet;
            ((RegExMatchComponent)firstComponent).ContextNamespaceToMatch = contextNamespaceToMatch;
            ((RegExMatchComponent)firstComponent).PatternToMatch = "^.{2}23";
            ((RegExMatchComponent)firstComponent).ValueToSet = "OK1";

            ((RegExMatchComponent)secondComponent).ContextNamespaceToSet = secondContextNamespaceToSet;
            ((RegExMatchComponent)secondComponent).ContextNamespaceToMatch = contextNamespaceToMatch;
            ((RegExMatchComponent)secondComponent).PatternToMatch = "^.{4}68";
            ((RegExMatchComponent)secondComponent).ValueToSet = "OK2";

            sendPipeline.AddComponent(firstComponent, PipelineStage.Encode);
            sendPipeline.AddComponent(secondComponent, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsTrue(result.Context.Read(firstContextNamespaceToSet.Split('#')[1], firstContextNamespaceToSet.Split('#')[0]).ToString() == "OK1");
            Assert.IsTrue(result.Context.Read(secondContextNamespaceToSet.Split('#')[1], secondContextNamespaceToSet.Split('#')[0]).ToString() == "OK2");
        }

        [TestMethod]
        public void TestNoMatchPattern()
        {
            const string contextNamespaceToSet = "http://acmeset.com/prop#test";
            const string contextNamespaceToMatch = "http://acmematch.com/prop#test";

            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Promote(contextNamespaceToMatch.Split('#')[1], contextNamespaceToMatch.Split('#')[0], "752368");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent component = new RegExMatchComponent();
            ((RegExMatchComponent)component).ContextNamespaceToSet = contextNamespaceToSet;
            ((RegExMatchComponent)component).ContextNamespaceToMatch = contextNamespaceToMatch;
            ((RegExMatchComponent)component).PatternToMatch = "^.{2}75";
            ((RegExMatchComponent)component).ValueToSet = "OK";

            sendPipeline.AddComponent(component, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsNull(result.Context.Read(contextNamespaceToSet.Split('#')[1], contextNamespaceToSet.Split('#')[0]));
        }
    }
}

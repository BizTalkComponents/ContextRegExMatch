using System;

namespace BizTalkComponents.PipelineComponents.ContextRegExMatch
{
    public partial class RegExMatch
    {
        public string Name { get { return "Context RegEx Match Component"; } }
        public string Version { get { return "0.1"; } }
        public string Description { get { return @"Set a context value if a pattern is matched in the selected context"; } }

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("A10D1D10-F3BD-411D-8B5A-DCA90F2BB992");
        }

        public void InitNew()
        {

        }

        public IntPtr Icon { get { return IntPtr.Zero; } }

        public System.Collections.IEnumerator Validate(object obj)
        {
            return null;
        }
    }
}

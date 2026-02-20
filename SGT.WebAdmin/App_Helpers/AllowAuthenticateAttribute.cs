using System;

namespace SGT.WebAdmin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AllowAuthenticateAttribute : Attribute
    {
        public AllowAuthenticateAttribute() { }
    }
}
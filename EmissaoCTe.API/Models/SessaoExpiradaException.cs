using System;

namespace EmissaoCTe.API.Models
{
    [Serializable]
    public class SessaoExpiradaException : Exception
    {
        public bool SessaoExpirada;

        public SessaoExpiradaException(string mensagem)
            : base(mensagem)
        {
            this.SessaoExpirada = true;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
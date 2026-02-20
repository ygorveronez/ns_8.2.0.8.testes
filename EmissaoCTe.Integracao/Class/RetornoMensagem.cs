using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [MessageContract]
    public class RetornoMensagem<T>
    {
        [MessageBodyMember]
        public Retorno<T> Retorno { get; set; }
    }
}
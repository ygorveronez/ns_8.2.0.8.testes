using System.ServiceModel;


namespace EmissaoCTe.Integracao
{
    [MessageContract]
    public class Arquivo
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public int CodigoEmpresaPai;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream;
    }
}
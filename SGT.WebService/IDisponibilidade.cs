using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDisponibilidade" in both code and config file together.
    [ServiceContract]
    public interface IDisponibilidade
    {
        [OperationContract]
        Retorno<bool> Testar();

        [OperationContract]
        Retorno<string> Versao();
    }
}

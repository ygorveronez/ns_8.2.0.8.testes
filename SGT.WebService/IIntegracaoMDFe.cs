using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IIntegracaoMDFe" in both code and config file together.
    [ServiceContract]
    public interface IIntegracaoMDFe
    {
        [OperationContract]
        Retorno<bool> IntegrarMDFeAutorizado(Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle);

        [OperationContract]
        Retorno<bool> IntegrarEventoInclusaoMotorista(Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle);
    }
}

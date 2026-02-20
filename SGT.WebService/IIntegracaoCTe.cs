using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IIntegracaoCTe" in both code and config file together.
    [ServiceContract]
    public interface IIntegracaoCTe
    {
        [OperationContract]
        Retorno<bool> IntegrarCteAverbado(Dominio.ObjetosDeValor.WebService.CTe.AverbacaoOracle averbacaoOracle);

        [OperationContract]
        Retorno<bool> IntegrarCTeAutorizado(Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle);

        [OperationContract]
        Retorno<bool> IntegrarCartaCorrecaoAutorizada(Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle);

        [OperationContract]
        Retorno<bool> IntegrarLogEnvioEmail(Dominio.ObjetosDeValor.WebService.CTe.LogEnvioEmail email);
    }
}

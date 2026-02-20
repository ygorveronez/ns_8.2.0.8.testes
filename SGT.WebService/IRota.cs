using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRota" in both code and config file together.
    [ServiceContract]
    public interface IRota
    {
        [OperationContract]
        Retorno<int> AdicionarRota(Dominio.ObjetosDeValor.WebService.Rota.Rota rotaIntegracao);
    }
}

using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOrdemServico" in both code and config file together.
    [ServiceContract]
    public interface IOrdemServico
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>> ConsultarOrdemServicoPendente(int? inicio, int? limite, int? codigoEmpresa);

        [OperationContract]
        Retorno<bool> RecebimentoDeOrdemServico(int? protocolo, string imagemBase64, string dataExecucao, string observacao, List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens> servicosOrdem);

    }
}

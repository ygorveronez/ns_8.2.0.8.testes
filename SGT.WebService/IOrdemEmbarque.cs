using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOrdemEmbarque" in both code and config file together.
    [ServiceContract]
    public interface IOrdemEmbarque
    {
        [OperationContract]
        Retorno<bool> AtualizarOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueRetorno ordemEmbarque);

        [OperationContract]
        Retorno<bool> AtualizarSituacaoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueSituacaoRetorno ordemEmbarqueSituacaoRetorno);

        [OperationContract]
        Retorno<bool> ConfirmarCancelamentoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoOrdemEmbarqueRequest request);

        [OperationContract]
        Retorno<bool> ConfirmarTrocaPedidoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoTrocaPedidoOrdemEmbarqueRequest request);
    }
}

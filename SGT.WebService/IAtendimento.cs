using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAtendimento" in both code and config file together.
    [ServiceContract]
    public interface IAtendimento
    {
        [OperationContract]
        Retorno<bool> AtualizarStatusDevolucao(Dominio.ObjetosDeValor.WebService.Atendimento.AtualizarStatusDevolucao atualizarStatusDevolucao);

        [OperationContract]
        Retorno<bool> AdicionarAtendimento(Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento adicionarAtendimento);
    }
}

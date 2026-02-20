using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPallet" in both code and config file together.
    [ServiceContract]
    public interface IPallet
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisComConfirmacaoRecebimentoPallet(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoNotaFiscalComConfirmacaoRecebimentoPallet(int protocoloNFe);

        [OperationContract]
        Retorno<bool> MovimentacaoPallet(Dominio.ObjetosDeValor.WebService.Pallet.MovimentacaoPallet movimentacaoPallet);

        [OperationContract]
        Retorno<bool> RetornoContestacaoPallet(Dominio.ObjetosDeValor.WebService.Pallet.RetornoContestacao retornoContestacao);
    }
}

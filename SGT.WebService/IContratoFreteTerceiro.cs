using Dominio.ObjetosDeValor.Embarcador.Terceiros;
using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceContract]
    public interface IContratoFreteTerceiro
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>> BuscarContratosFretePendentesIntegracao(string dataInicial, string dataFinal, int? inicio, int? quantidadeRegistros);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoContratoFrete(List<int> protocolos);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento> AutorizarPagamento(AutorizacaoPagamento autorizacaoPagamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT> EncerrarCIOTPeloContratoTerfeito(int? protocoloContratoTerceiro);
    }
}

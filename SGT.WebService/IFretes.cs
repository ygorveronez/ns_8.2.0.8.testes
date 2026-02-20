using System.Collections.Generic;
using CoreWCF;
using System.Threading.Tasks;

namespace SGT.WebService
{
    [ServiceContract]
    public interface IFretes
    {
        [OperationContract]
        Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> CalcularFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete);

        [OperationContract]
        Task<Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>>> ObterCotacao(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao);

        [OperationContract]
        Retorno<bool> SolicitarRecalculoFrete(Dominio.ObjetosDeValor.WebService.Frete.SolicitacaoRecalculoFrete solicitacaoRecalculoFrete);
    }
}

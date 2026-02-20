using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas.Estrategias;

public class GerarCarregamentoComRedespachoStrategy : IStrategyQuebraRequest
{
    #region Métodos Públicos

    public List<RequestSubtarefa> QuebrarRequestTarefa(ContextoEtapa contexto)
    {
        Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento =
            contexto.RequestDoc.Dados.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();

        List<RequestSubtarefa> retorno = new List<RequestSubtarefa>();
        int ordem = 0;

        retorno.Add(new RequestSubtarefa
        {
            TarefaId = contexto.TarefaId,
            Dados = carregamento.ToBsonDocument(),
            Ordem = ordem++
        });

        if (carregamento.CarregamentosRedespacho != null && carregamento.CarregamentosRedespacho.Count > 0)
        {
            foreach (var carregamentoRedespacho in carregamento.CarregamentosRedespacho)
            {
                retorno.Add(new RequestSubtarefa
                {
                    TarefaId = contexto.TarefaId,
                    Dados = carregamentoRedespacho.ToBsonDocument(),
                    Ordem = ordem++
                });
            }
        }

        return retorno;
    }

    #endregion Métodos Públicos
}

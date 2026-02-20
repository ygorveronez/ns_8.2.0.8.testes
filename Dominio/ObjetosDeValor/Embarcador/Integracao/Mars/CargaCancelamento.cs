using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class CargaCancelamento
    {
        public CargaCancelamento(Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento)
        {
            Detalhes = new DetalhesCargaCancelamento()
            {
                Atividade = DefinirCodigoAtividade((Enumeradores.AtividadeFilial)cargaCancelamento.Carga.Filial.Atividade.Codigo),
                CodigoIntegracaoFilial = cargaCancelamento.Carga.Filial?.CodigoFilialEmbarcador,
                CodigoCargaEmbarcador = cargaCancelamento.Carga.CodigoCargaEmbarcador,
                ProtocoloCarga = cargaCancelamento.Carga.Codigo.ToString(),
                CodigoIntegracaoTransportador = cargaCancelamento.Carga.Empresa?.CodigoIntegracao,
                DataEnvioCancelamentoPlanejado = cargaCancelamento.DataEnvioCancelamento.HasValue 
                    ? cargaCancelamento.DataEnvioCancelamento.Value.ToString() 
                    : DateTime.Now.ToString(),
                DataCancelamentoPlanejado = cargaCancelamento.DataCancelamento.ToString(),
                DataEnvioCancelamentoEfetuado = cargaCancelamento.DataEnvioCancelamento.HasValue
                    ? cargaCancelamento.DataEnvioCancelamento.Value.ToString()
                    : DateTime.Now.ToString(),
                DataCancelamentoEfetuado = cargaCancelamento.DataCancelamento.ToString(),
                MotivoCancelamento = cargaCancelamento.MotivoCancelamento,
                Pedidos = cargaCancelamento.Carga.Pedidos.Select(x => new PedidoCancelamento
                {
                    NumeroPedido = x.Pedido.Numero.ToString(),
                    ProtocoloPedido = x.Pedido.Codigo.ToString(),
                }).ToList()
            };
        }

        [JsonProperty("supplyChainEventDetails")]
        public DetalhesCargaCancelamento Detalhes { get; set; }

        private static string DefinirCodigoAtividade(Enumeradores.AtividadeFilial enumAtividade)
        {
            string retorno;

            if (enumAtividade == Enumeradores.AtividadeFilial.Industrial ||
                Enumeradores.AtividadeFilialHelper.ObterDescricao(enumAtividade) == Localization.Resources.Enumeradores.Atividade.Industrial)
                retorno = "148";
            else if (enumAtividade == Enumeradores.AtividadeFilial.Comercial ||
                Enumeradores.AtividadeFilialHelper.ObterDescricao(enumAtividade) == Localization.Resources.Enumeradores.Atividade.Comercial)
                retorno = "151";
            else
                retorno = enumAtividade.ToString();

            return retorno;
        }
    }
}
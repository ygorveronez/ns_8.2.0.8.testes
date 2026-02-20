using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargasComInteresseTransportadorTerceiro : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro>
    {
        #region Construtores

        public ConsultaCargasComInteresseTransportadorTerceiro() : base(tabela: "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR as CargaJanelaCarregamentoTranportador") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" JOIN T_CARGA_JANELA_CARREGAMENTO AS CargaJanelaCarregamento ON CargaJanelaCarregamento.CJC_CODIGO = CargaJanelaCarregamentoTranportador.CJC_CODIGO");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA AS Carga ON Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO ");
        }


        private void SetarJoinsCentroCarregamento(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);

            if (!joins.Contains(" CentroCarregamento "))
                joins.Append(" JOIN T_CENTRO_CARREGAMENTO AS CentroCarregamento ON CentroCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO ");

        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" JOIN T_VEICULO AS Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");

        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" JOIN T_CARGA_DADOS_SUMARIZADOS AS CargaDadosSumarizados ON CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");

        }

        private void SetarJoinsFilaCarregamentoVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" FilaCarregamentoVeiculo "))
                joins.Append(" JOIN T_FILA_CARREGAMENTO_VEICULO AS FilaCarregamentoVeiculo ON FilaCarregamentoVeiculo.CAR_CODIGO = Carga.CAR_CODIGO ");

          
        }

        private void SetarJoinsFilaCarregamentoVeiculoHistorico(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoVeiculo(joins);

            if (!joins.Contains(" FilaCarregamentoVeiculoHistorico "))

                joins.Append(" LEFT JOIN T_FILA_CARREGAMENTO_VEICULO_HISTORICO AS FilaCarregamentoVeiculoHistorico ON FilaCarregamentoVeiculoHistorico.FLV_CODIGO = FilaCarregamentoVeiculo.FLV_CODIGO AND (FilaCarregamentoVeiculoHistorico.FMP_CODIGO IS NOT NULL OR FilaCarregamentoVeiculoHistorico.FMR_CODIGO IS NOT NULL OR FilaCarregamentoVeiculoHistorico.FMS_CODIGO IS NOT NULL) " +
                    " LEFT JOIN T_FILA_CARREGAMENTO_MOTIVO_ALTERACAO_POSICAO AS FilaCarregamentoMotivoAlteracaoPosicao ON FilaCarregamentoMotivoAlteracaoPosicao.FMP_CODIGO = FilaCarregamentoVeiculoHistorico.FMP_CODIGO " +
                    "  LEFT JOIN T_FILA_CARREGAMENTO_MOTIVO_RETIRADA AS FilaCarregamentoMotivoRetirada ON FilaCarregamentoMotivoRetirada.FMR_CODIGO = FilaCarregamentoVeiculoHistorico.FMR_CODIGO" +
                    "  LEFT JOIN T_FILA_CARREGAMENTO_MOTIVO_SELECAO_MOTORISTA_FORA_ORDEM AS FilaCarregamentoMotivoSelecaoForaOrdem ON FilaCarregamentoMotivoSelecaoForaOrdem.FMS_CODIGO = FilaCarregamentoVeiculoHistorico.FMS_CODIGO");

            
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtroPesquisa)
        {
            switch (propriedade)
            {
                case "DescricaoCentroCarregamento":
                    if (!select.Contains(" DescricaoCentroCarregamento, "))
                    {
                        select.Append(" CentroCarregamento.CEC_DESCRICAO AS DescricaoCentroCarregamento, ");
                        groupBy.Append(" CentroCarregamento.CEC_DESCRICAO, ");

                        SetarJoinsCentroCarregamento(joins);
                    }
                    break;
                case "DescricaoCarga":
                    if (!select.Contains(" DescricaoCarga, "))
                    {
                        select.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR AS DescricaoCarga, ");
                        groupBy.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "Situacao":
                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append(" CargaJanelaCarregamentoTranportador.JCT_SITUACAO AS Situacao, ");
                        groupBy.Append(" CargaJanelaCarregamentoTranportador.JCT_SITUACAO, ");
                    }
                    break;
                case "DataCriacaoCargaFormatada":
                case "DataCriacaoCarga":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append(" Carga.CAR_DATA_CRIACAO AS DataCriacaoCarga, ");
                        groupBy.Append(" Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DescricaoVeiculo":
                    if (!select.Contains(" DescricaoVeiculo, "))
                    {
                        select.Append(" Veiculo.VEI_PLACA AS DescricaoVeiculo, ");
                        groupBy.Append(" Veiculo.VEI_PLACA , ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append(" CargaDadosSumarizados.CDS_ORIGENS AS Origem, ");
                        groupBy.Append(" CargaDadosSumarizados.CDS_ORIGENS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append(" CargaDadosSumarizados.CDS_DESTINOS AS Destino, ");
                        groupBy.Append(" CargaDadosSumarizados.CDS_DESTINOS , ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("FilaCarregamentoVeiculo.FLV_POSICAO AS Posicao, ");
                        groupBy.Append(" FilaCarregamentoVeiculo.FLV_POSICAO, ");

                        SetarJoinsFilaCarregamentoVeiculo(joins);
                    }
                    break;

                case "RealizouACarga":
                    if (!select.Contains(" RealizouACarga, "))
                    {
                        select.Append(" (CASE WHEN EXISTS(SELECT 1 FROM T_CARGA AS CargaTerceiro WHERE CargaTerceiro.CAR_CODIGO = Carga.CAR_CODIGO AND CargaTerceiro.CLI_CGCCPF_TERCEIRO = Carga.CLI_CGCCPF_TERCEIRO AND CargaTerceiro.CAR_SITUACAO = 11) THEN 'Sim' ELSE 'Não' END) AS RealizouACarga,");
                        groupBy.Append(" Carga.CLI_CGCCPF_TERCEIRO, Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Motivo":
                    if (!select.Contains(" Motivo, "))
                    {
                        select.Append(" (CASE WHEN FilaCarregamentoVeiculoHistorico.FMP_CODIGO IS NOT NULL THEN FilaCarregamentoMotivoAlteracaoPosicao.FMP_DESCRICAO WHEN FilaCarregamentoVeiculoHistorico.FMR_CODIGO IS NOT NULL THEN FilaCarregamentoMotivoRetirada.FMR_DESCRICAO WHEN FilaCarregamentoVeiculoHistorico.FMS_CODIGO IS NOT NULL THEN FilaCarregamentoMotivoSelecaoForaOrdem.FMS_DESCRICAO END) AS Motivo, ");
                        groupBy.Append(" FilaCarregamentoVeiculoHistorico.FMP_CODIGO, FilaCarregamentoVeiculoHistorico.FMS_CODIGO, FilaCarregamentoVeiculoHistorico.FMR_CODIGO, FilaCarregamentoMotivoAlteracaoPosicao.FMP_DESCRICAO,FilaCarregamentoMotivoRetirada.FMR_DESCRICAO,FilaCarregamentoMotivoSelecaoForaOrdem.FMS_DESCRICAO,  ");

                        SetarJoinsFilaCarregamentoVeiculoHistorico(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" AND Carga.CAR_CODIGO =  {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" AND Carga.CAR_VEICULO =  {filtrosPesquisa.CodigoVeiculo}");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where.Append($" AND CargaJanelaCarregamento.CEC_CODIGO =  {filtrosPesquisa.CodigoCentroCarregamento}");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" AND CAST(Carga.CAR_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" AND CargaJanelaCarregamentoTranportador.JCT_SITUACAO  = {(int)filtrosPesquisa.Situacao}");

            if (filtrosPesquisa.CodigoClienteTerceiro > 0)
                where.Append($" AND CargaJanelaCarregamentoTranportador.CLI_CGCCPF_TERCEIRO  = {(long)filtrosPesquisa.CodigoClienteTerceiro}");


        }

        #endregion
    }
}

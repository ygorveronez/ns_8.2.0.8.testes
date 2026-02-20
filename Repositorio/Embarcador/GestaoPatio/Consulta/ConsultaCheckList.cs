using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    sealed class ConsultaCheckList : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList>
    {
        #region Construtores

        public ConsultaCheckList() : base(tabela: "T_CHECK_LIST_CARGA as CheckList") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CheckList.CAR_CODIGO ");
        }

        private void SetarJoinsFluxoGestaoPatio(StringBuilder joins)
        {
            if (!joins.Contains(" FluxoGestaoPatio "))
                joins.Append(" left join T_FLUXO_GESTAO_PATIO FluxoGestaoPatio on FluxoGestaoPatio.FGP_CODIGO = CheckList.FGP_CODIGO ");
        }

        private void SetarJoinsPreCarga(StringBuilder joins)
        {
            if (!joins.Contains(" PreCarga "))
                joins.Append(" left join T_PRE_CARGA PreCarga on PreCarga.PCA_CODIGO = CheckList.PCA_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("CheckList.CLC_CODIGO as Codigo, ");
                        groupBy.Append("CheckList.CLC_CODIGO, ");
                    }
                    break;

                case "Data":
                case "DataFormatada":
                    if (!select.Contains(" Data,"))
                    {
                        select.Append("CheckList.CLC_DATA_ABERTURA as Data, ");
                        groupBy.Append("CheckList.CLC_DATA_ABERTURA, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.Append("Transportador.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa,"))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ObservacoesGerais":
                    if (!select.Contains(" ObservacoesGerais,"))
                    {
                        select.Append("CheckList.CLC_OBSERVACAO as ObservacoesGerais, ");
                        groupBy.Append("CheckList.CLC_OBSERVACAO, ");
                    }
                    break;

                case "Status":
                case "StatusDescricao":
                    if (!select.Contains(" Status,"))
                    {
                        select.Append("CheckList.CLC_SITUACAO as Status, ");
                        groupBy.Append("CheckList.CLC_SITUACAO, ");
                    }
                    break;

                case "StatusChecklistAnteriorFormatada":
                    if (!select.Contains(" StatusChecklistAnterior,"))
                    {
                        select.Append(@$"(SELECT TOP 1 ChecklistHistorico.CCH_SITUACAO
                                        FROM T_CHECKLIST_CARGA_HISTORICO ChecklistHistorico
                                        WHERE ChecklistHistorico.CLC_CODIGO = Checklist.CLC_CODIGO
                                            AND ChecklistHistorico.CCH_SITUACAO = {(int)SituacaoCheckList.Rejeitado}) as StatusChecklistAnterior, "); // SQL-INJECTION-SAFE
                    }
                    break;

                case "MotivoReprovaChecklistAnterior":
                    if (!select.Contains(" MotivoReprovaChecklistAnterior,"))
                    {
                        select.Append(@$"(SELECT TOP 1 ChecklistHistorico.CCH_OBSERVACAO
                                        FROM T_CHECKLIST_CARGA_HISTORICO ChecklistHistorico
                                        WHERE ChecklistHistorico.CLC_CODIGO = Checklist.CLC_CODIGO
                                            AND ChecklistHistorico.CCH_SITUACAO = {(int)SituacaoCheckList.Rejeitado}) as MotivoReprovaChecklistAnterior, ");// SQL-INJECTION-SAFE
                    }
                    break;

                case "DataChecklistAnteriorFormatada":
                    if (!select.Contains(" DataChecklistAnterior,"))
                    {
                        select.Append(@$"(SELECT TOP 1 ChecklistHistorico.CCH_DATA
                                        FROM T_CHECKLIST_CARGA_HISTORICO ChecklistHistorico
                                        WHERE ChecklistHistorico.CLC_CODIGO = Checklist.CLC_CODIGO
                                            AND ChecklistHistorico.CCH_SITUACAO = {(int)SituacaoCheckList.Rejeitado}) as DataChecklistAnterior, "); // SQL-INJECTION-SAFE
                    }
                    break;

                case "DataChecklistAtualFormatada":
                    if (!select.Contains(" DataChecklistAtual,"))
                    {
                        select.Append(@$"(SELECT TOP 1 ChecklistHistorico.CCH_DATA
                                        FROM T_CHECKLIST_CARGA_HISTORICO ChecklistHistorico
                                        WHERE ChecklistHistorico.CLC_CODIGO = Checklist.CLC_CODIGO
                                            AND ChecklistHistorico.CCH_SITUACAO = {(int)SituacaoCheckList.Finalizado}) as DataChecklistAtual, "); // SQL-INJECTION-SAFE
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" and ( ");
            where.Append($"        (Carga.CAR_OCULTAR_NO_PATIO = 0 and Carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada})) or");
            where.Append($"        (Carga.CAR_CODIGO is null and PreCarga.PCA_SITUACAO <> {(int)SituacaoPreCarga.Cancelada})");
            where.Append("     ) ");
            where.Append(" and Carga.CAR_CARGA_FECHADA = 1 ");

            SetarJoinsFluxoGestaoPatio(joins);
            SetarJoinsCarga(joins);
            SetarJoinsPreCarga(joins);

            if (filtrosPesquisa.CodigoFilial > 0)
                where.Append($" and FluxoGestaoPatio.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CheckList.CLC_DATA_ABERTURA >= '{filtrosPesquisa.DataInicial.Value.Date.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CheckList.CLC_DATA_ABERTURA <= '{filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}'");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" and CheckList.CLC_SITUACAO = {(int)filtrosPesquisa.Situacao.Value}");
        }

        #endregion
    }
}

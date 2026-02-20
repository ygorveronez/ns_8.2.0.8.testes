using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaAlteracaoFrete : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete>
    {
        #region Construtores

        public ConsultaAlteracaoFrete() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeiculo on ModeloVeiculo.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsOperadorCarga(StringBuilder joins)
        {
            if (!joins.Contains(" OperadorCarga "))
                joins.Append(" left join T_FUNCIONARIO OperadorCarga on OperadorCarga.FUN_CODIGO = Carga.CAR_OPERADOR ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append(" left join T_ROTA_FRETE Rota on Rota.ROF_CODIGO = Carga.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCargaAgrupada(StringBuilder joins)
        {
            if (!joins.Contains(" CargaAgrupada "))
                joins.Append(" left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO = Carga.CAR_CODIGO_AGRUPAMENTO ");
        }

        private void SetarJoinsMotivoSolicitacaoFrete(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoSolicitacaoFrete "))
                joins.Append(" left join T_MOTIVO_SOLICITACAO_FRETE MotivoSolicitacaoFrete on MotivoSolicitacaoFrete.MSF_CODIGO = Carga.MSF_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CNPJFilialFormatado":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Carga.CAR_CODIGO Codigo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "CodigosAgrupados":
                    if (!select.Contains(" CodigosAgrupados, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + _codigoAgrupados.CAR_CODIGO_CARGA_AGRUPADO ");
                        select.Append("      from T_CARGA_CODIGOS_AGRUPADOS _codigoAgrupados ");
                        select.Append("     where _codigoAgrupados.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CodigosAgrupados, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarregamento, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo, "))
                    {
                        select.Append("ModeloVeiculo.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeiculo.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "Negociacao":
                    SetarSelect("TipoFreteEscolhido", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "OperadorCarga":
                    if (!select.Contains(" OperadorCarga, "))
                    {
                        select.Append("OperadorCarga.FUN_NOME OperadorCarga, ");
                        groupBy.Append("OperadorCarga.FUN_NOME, ");

                        SetarJoinsOperadorCarga(joins);
                    }
                    break;

                case "PercentualDiferenca":
                    SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "Rota":
                    if (!select.Contains(" Rota, "))
                    {
                        select.Append("Rota.ROF_DESCRICAO Rota, ");
                        groupBy.Append("Rota.ROF_DESCRICAO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "SituacaoAlteracaoFreteFormatada":
                    if (!select.Contains(" SituacaoAlteracaoFrete, "))
                    {
                        select.Append("Carga.CAR_SITUACAO_ALTERACAO_FRETE_CARGA SituacaoAlteracaoFrete, ");
                        groupBy.Append("Carga.CAR_SITUACAO_ALTERACAO_FRETE_CARGA, ");
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "TipoFreteEscolhido":
                    if (!select.Contains(" TipoFreteEscolhido, "))
                    {
                        select.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO TipoFreteEscolhido, ");
                        groupBy.Append("Carga.CAR_TIPO_FRETE_ESCOLHIDO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "ValorAcrescimo":
                    if (!select.Contains(" ValorAcrescimo, "))
                    {
                        select.Append(@"(SELECT SUM(CCF_VALOR_COMPONENTE) FROM T_CARGA_COMPONENTES_FRETE Componente
                                            WHERE Componente.CAR_CODIGO = Carga.CAR_CODIGO AND Componente.CCF_VALOR_COMPONENTE < 0) ValorAcrescimo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE) ValorFrete, ");
                    break;

                case "ValorTabelaFrete":
                    if (!select.Contains(" ValorTabelaFrete, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE) ValorTabelaFrete, ");
                    break;

                case "ValorTabela":
                    if (!filtroPesquisa.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga)
                        SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    else
                    {
                        SetarSelect("ValorTabelaFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                        SetarSelect("AliquotaICMS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                        SetarSelect("AliquotaISS", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                        SetarSelect("ValorTotalComponentes", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;

                case "AliquotaICMS":
                    if (!select.Contains(" AliquotaICMS, "))
                    {
                        select.Append(@"(SELECT AVG(PED_PERCENTUAL_ALICOTA) FROM T_CARGA_PEDIDO CargaPedido
                                             WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) AliquotaICMS, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS, "))
                    {
                        select.Append(@"(SELECT AVG(PED_PERCENTUAL_ALICOTA_ISS) FROM T_CARGA_PEDIDO CargaPedido
                                             WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) AliquotaISS, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorTotalComponentes":
                    if (!select.Contains(" ValorTotalComponentes, "))
                    {
                        select.Append(@"(SELECT SUM(CCF_VALOR_COMPONENTE) FROM T_CARGA_COMPONENTES_FRETE Componente
                                            WHERE Componente.CCF_TIPO_COMPONENTE_FRETE <> 1 AND Componente.CCF_TIPO_COMPONENTE_FRETE <> 6
                                             AND Componente.CAR_CODIGO = Carga.CAR_CODIGO) ValorTotalComponentes, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        select.Append("( ");
                        select.Append("    (select _veiculo.VEI_PLACA from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + _veiculo.VEI_PLACA ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga ");
                        select.Append("          join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("         where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") Veiculos, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "MotivoSolicitacaoFrete":
                    if (!select.Contains(" MotivoSolicitacaoFrete, "))
                    {
                        select.Append("MotivoSolicitacaoFrete.MSF_DESCRICAO MotivoSolicitacaoFrete, ");
                        groupBy.Append("MotivoSolicitacaoFrete.MSF_DESCRICAO, ");

                        SetarJoinsMotivoSolicitacaoFrete(joins);
                    }
                    break;

                case "StatusTituloCTe":
                    if (!select.Contains(" StatusTituloCTe, "))
                    {
                        select.Append(@"(CASE WHEN (SELECT COUNT(1) FROM T_CARGA_CTE cargaCTe
                                             inner join T_CTE cte on cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                             inner join T_TITULO titulo on titulo.TIT_CODIGO = cte.TIT_CODIGO
                                             where titulo.TIT_STATUS = 3 and cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO) > 0 THEN 'Pago' ELSE 'Em Aberto' END) StatusTituloCTe, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "ValorFreteNegociado":
                    if (!select.Contains(" ValorFreteNegociado, "))
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_NEGOCIADO) ValorFreteNegociado, ");
                    break;

                case "ValorComplementoFrete":
                    if (!select.Contains(" ValorComplementoFrete, "))
                    {
                        select.Append(@"(SELECT SUM(CCF_VALOR_COMPLEMENTO) FROM T_CARGA_COMPLEMENTO_FRETE complemento
                                             WHERE complemento.CAR_CODIGO = Carga.CAR_CODIGO) ValorComplementoFrete, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DiferencaValorPagoTabela":
                    SetarSelect("ValorTabela", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorFreteNegociado", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorComplementoFrete", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "UsuarioAprovacao":
                    if (!select.Contains(" UsuarioAprovacao, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Aprovador.FUN_NOME
                                          FROM T_AUTORIZACAO_ALCADA_CARGA Aprovacao
                                          JOIN T_FUNCIONARIO Aprovador on Aprovador.FUN_CODIGO = Aprovacao.FUN_CODIGO
                                         WHERE Aprovacao.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) AS UsuarioAprovacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(" and Carga.CAR_CARGA_FECHADA = 1  and Carga.CAR_CARGA_DE_PRE_CARGA = 0 "); //((Carga.CAR_CARGA_FECHADA = 1 and Carga.CAR_CARGA_AGRUPADA = 0 ) or (Carga.CAR_CARGA_FECHADA = 0 and Carga.CAR_CODIGO_AGRUPAMENTO is not null))

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                where.Append($" and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                where.Append($" and Carga.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicularCarga}");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and Carga.CAR_OPERADOR = {filtrosPesquisa.CodigoOperador}");

            if (filtrosPesquisa.SituacaoAlteracaoFrete.HasValue)
                where.Append($" and Carga.CAR_SITUACAO_ALTERACAO_FRETE_CARGA = {filtrosPesquisa.SituacaoAlteracaoFrete.Value.ToString("d")}");

            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0)
            {
                string situacoes = String.Join(", ", from situacao in filtrosPesquisa.Situacoes select situacao.ToString("d"));

                where.Append($" and Carga.CAR_SITUACAO IN ({situacoes})");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append(" and ( ");
                where.Append($"        Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or ");
                where.Append("         Carga.CAR_CODIGO in ( ");
                where.Append("             select _cargaveiculos.CAR_CODIGO ");
                where.Append("               from T_CARGA_VEICULOS_VINCULADOS _cargaveiculos ");
                where.Append($"             WHERE _cargaveiculos.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");
                where.Append("         ) ");
                where.Append("     )");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                where.Append($" and (Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' or CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}')");

                SetarJoinsCargaAgrupada(joins);
            }
        }

        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    sealed class ConsultaPagamentoMotoristaTMS : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS>
    {
        #region Construtores

        public ConsultaPagamentoMotoristaTMS() : base(tabela: "T_PAGAMENTO_MOTORISTA_TMS as PagamentoMotoristaTMS") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = PagamentoMotoristaTMS.CAR_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append("left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = PagamentoMotoristaTMS.FUN_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            if (!joins.Contains(" Operador "))
                joins.Append("left join T_FUNCIONARIO Operador on Operador.FUN_CODIGO = PagamentoMotoristaTMS.FUN_CODIGO ");
        }

        private void SetarJoinsTipoPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" PagamentoMotoristaTipo "))
                joins.Append("left join T_PAGAMENTO_MOTORISTA_TIPO PagamentoMotoristaTipo on PagamentoMotoristaTipo.PMT_CODIGO = PagamentoMotoristaTMS.PMT_CODIGO ");
        }

        private void SetarJoinsPlanoDeContaDebito(StringBuilder joins)
        {
            if (!joins.Contains(" PlanoDeContaDebito "))
                joins.Append("left join T_PLANO_DE_CONTA PlanoDeContaDebito on PlanoDeContaDebito.PLA_CODIGO = PagamentoMotoristaTMS.PLA_CODIGO_DEBITO ");
        }

        private void SetarJoinsPlanoDeContaCredito(StringBuilder joins)
        {
            if (!joins.Contains(" PlanoDeContaCredito "))
                joins.Append("left join T_PLANO_DE_CONTA PlanoDeContaCredito on PlanoDeContaCredito.PLA_CODIGO = PagamentoMotoristaTMS.PLA_CODIGO_CREDITO ");
        }

        private void SetarJoinsAcertoViagem(StringBuilder joins)
        {
            if (!joins.Contains(" AcertoViagem "))
                joins.Append("left join T_ACERTO_DE_VIAGEM AcertoViagem on AcertoViagem.ACV_CODIGO in (select AcertoAdiantamento.ACV_CODIGO from T_ACERTO_ADIANTAMENTO AcertoAdiantamento where AcertoAdiantamento.PAM_CODIGO = PagamentoMotoristaTMS.PAM_CODIGO) and AcertoViagem.ACV_SITUACAO <> 3");
        }

        private void SetarJoinsFavorecido(StringBuilder joins)
        {
            if (!joins.Contains(" Favorecido "))
                joins.Append("left join T_CLIENTE Favorecido on Favorecido.CLI_CGCCPF = PagamentoMotoristaTMS.CLI_CGCCPF_TITULO_PAGAR");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtroPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroPagamento":
                    if (!select.Contains(" NumeroPagamento, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_NUMERO as NumeroPagamento, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_NUMERO, ");
                    }
                    break;

                case "Acerto":
                    if (!select.Contains(" Acerto, "))
                    {
                        select.Append("AcertoViagem.ACV_NUMERO as Acerto, ");
                        groupBy.Append("AcertoViagem.ACV_NUMERO, ");

                        SetarJoinsAcertoViagem(joins);
                    }
                    break;

                case "Situacao":
                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_SITUACAO as Situacao, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_SITUACAO, ");
                    }
                    break;

                case "Etapa":
                case "EtapaDescricao":
                    if (!select.Contains(" Etapa, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_ETAPA as Etapa, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_ETAPA, ");
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "CPFMotorista":
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        select.Append("Motorista.FUN_CPF as CPFMotorista, ");
                        groupBy.Append("Motorista.FUN_CPF, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR as Valor, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR, ");
                    }
                    break;

                case "DataPagamento":
                case "DataPagamentoFormatada":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_DATA_PAGAMENTO as DataPagamento, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_DATA_PAGAMENTO, ");
                    }
                    break;

                case "TipoPagamento":
                case "TipoPagamentoDescricao":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select.Append("PagamentoMotoristaTipo.PMT_DESCRICAO as TipoPagamento, ");
                        groupBy.Append("PagamentoMotoristaTipo.PMT_DESCRICAO, ");

                        SetarJoinsTipoPagamento(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_OBSERVACAO as Observacao, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_OBSERVACAO, ");
                    }
                    break;

                case "PlanoEntrada":
                    if (!select.Contains(" PlanoEntrada, "))
                    {
                        select.Append("PlanoDeContaCredito.PLA_DESCRICAO as PlanoEntrada, ");
                        groupBy.Append("PlanoDeContaCredito.PLA_DESCRICAO, ");

                        SetarJoinsPlanoDeContaCredito(joins);
                    }
                    break;

                case "PlanoSaida":
                    if (!select.Contains(" PlanoSaida, "))
                    {
                        select.Append("PlanoDeContaDebito.PLA_DESCRICAO as PlanoSaida, ");
                        groupBy.Append("PlanoDeContaDebito.PLA_DESCRICAO, ");

                        SetarJoinsPlanoDeContaDebito(joins);
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        select.Append("Operador.FUN_NOME as Operador, ");
                        groupBy.Append("Operador.FUN_NOME, ");

                        SetarJoinsOperador(joins);
                    }
                    break;

                case "Moeda":
                case "MoedaDescricao":
                    if (!select.Contains(" Moeda, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_MOEDA_COTACAO_BANCO_CENTRAL as Moeda, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_MOEDA_COTACAO_BANCO_CENTRAL, ");
                    }
                    break;

                case "ValorMoeda":
                    if (!select.Contains(" ValorMoeda, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_MOEDA_COTACAO as ValorMoeda, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_MOEDA_COTACAO, ");
                    }
                    break;

                case "ValorOriginalMoeda":
                    if (!select.Contains(" ValorOriginalMoeda, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA as ValorOriginalMoeda, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA, ");
                    }
                    break;

                case "Favorecido":
                    if (!select.Contains(" Favorecido, "))
                    {
                        select.Append("Favorecido.CLI_NOME as Favorecido, ");
                        groupBy.Append("Favorecido.CLI_NOME, ");

                        SetarJoinsFavorecido(joins);
                    }
                    break;

                case "DataEfetivacao":
                case "DataEfetivacaoFormatada":
                    if (!select.Contains(" DataEfetivacao, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_DATA_EFETIVACAO as DataEfetivacao, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_DATA_EFETIVACAO, ");
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + tomador.PCT_NOME
                                            FROM T_CARGA_CTE cargaCTe
                                            JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                            JOIN T_CTE_PARTICIPANTE tomador ON tomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                            WHERE cargaCTe.CAR_CODIGO = PagamentoMotoristaTMS.CAR_CODIGO FOR XML PATH('')), 3, 1000) Tomador, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.CAR_CODIGO,"))
                            groupBy.Append("PagamentoMotoristaTMS.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS varchar(20))
                                            FROM T_CARGA_CTE cargaCTe
                                            JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                            WHERE cargaCTe.CCC_CODIGO is null and cargaCTe.CAR_CODIGO = PagamentoMotoristaTMS.CAR_CODIGO FOR XML PATH('')), 3, 1000) NumeroDocumento,");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.CAR_CODIGO,"))
                            groupBy.Append("PagamentoMotoristaTMS.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoComplementar":
                    if (!select.Contains(" NumeroDocumentoComplementar, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS varchar(20))
                                            FROM T_CARGA_CTE cargaCTe
                                            JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                            WHERE cargaCTe.CCC_CODIGO is not null and cargaCTe.CAR_CODIGO = PagamentoMotoristaTMS.CAR_CODIGO FOR XML PATH('')), 3, 1000) NumeroDocumentoComplementar,  ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.CAR_CODIGO,"))
                            groupBy.Append("PagamentoMotoristaTMS.CAR_CODIGO, ");
                    }
                    break;

                case "CNPJTomadorFormatado":
                    if (!select.Contains(" CNPJTomador, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + tomador.PCT_CPF_CNPJ
                                            FROM T_CARGA_CTE cargaCTe
                                            JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                            JOIN T_CTE_PARTICIPANTE tomador ON tomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                            WHERE cargaCTe.CAR_CODIGO = PagamentoMotoristaTMS.CAR_CODIGO FOR XML PATH('')), 3, 1000) CNPJTomador, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.CAR_CODIGO,"))
                            groupBy.Append("PagamentoMotoristaTMS.CAR_CODIGO, ");
                    }
                    break;

                case "ValorIRRF":
                    if (!select.Contains(" ValorIRRF, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_IRRF as ValorIRRF, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_IRRF, ");
                    }
                    break;

                case "ValorINSS":
                    if (!select.Contains(" ValorINSS "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_INSS as ValorINSS, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_INSS, ");
                    }
                    break;

                case "ValorSEST":
                    if (!select.Contains(" ValorSEST, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_SEST as ValorSEST, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_SEST, ");
                    }
                    break;

                case "ValorSENAT":
                    if (!select.Contains(" ValorSENAT, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_VALOR_SENAT as ValorSENAT, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_SENAT, ");
                    }
                    break;

                case "Data":
                case "DataLancamentoFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("PagamentoMotoristaTMS.PAM_DATA as Data, ");
                        groupBy.Append("PagamentoMotoristaTMS.PAM_DATA, ");
                    }
                    break;

                case "ValorLiquido":
                    if (!select.Contains(" ValorLiquido, "))
                    {
                        select.Append("(COALESCE(PagamentoMotoristaTMS.PAM_VALOR, 0) - COALESCE(PagamentoMotoristaTMS.PAM_VALOR_IRRF, 0) - COALESCE(PagamentoMotoristaTMS.PAM_VALOR_INSS, 0) - COALESCE(PagamentoMotoristaTMS.PAM_VALOR_SEST, 0) - COALESCE(PagamentoMotoristaTMS.PAM_VALOR_SENAT, 0)) as ValorLiquido, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.PAM_VALOR, "))
                            groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.PAM_VALOR_IRRF, "))
                            groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_IRRF, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.PAM_VALOR_INSS, "))
                            groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_INSS, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.PAM_VALOR_SEST, "))
                            groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_SEST, ");

                        if (!groupBy.Contains("PagamentoMotoristaTMS.PAM_VALOR_SENAT, "))
                            groupBy.Append("PagamentoMotoristaTMS.PAM_VALOR_SENAT, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" and PagamentoMotoristaTMS.FUN_CODIGO_MOTORISTA = {filtrosPesquisa.CodigoMotorista}");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and PagamentoMotoristaTMS.FUN_CODIGO = {filtrosPesquisa.CodigoOperador}");

            if (filtrosPesquisa.CodigosTipoPagamento.Count > 0)
            {
                SetarJoinsTipoPagamento(joins);

                where.Append($" and PagamentoMotoristaTipo.PMT_CODIGO in ( {string.Join(", ", filtrosPesquisa.CodigosTipoPagamento)})");
            }

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CAST(PagamentoMotoristaTMS.PAM_DATA_PAGAMENTO AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CAST(PagamentoMotoristaTMS.PAM_DATA_PAGAMENTO AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.Situacao != SituacaoPagamentoMotorista.Todas)
                where.Append($" and PagamentoMotoristaTMS.PAM_SITUACAO = {(int)filtrosPesquisa.Situacao}");

            if (filtrosPesquisa.Etapa > 0)
                where.Append($" and PagamentoMotoristaTMS.PAM_ETAPA = {(int)filtrosPesquisa.Etapa}");

            if (filtrosPesquisa.NumeroPagamento > 0)
                where.Append($" and PagamentoMotoristaTMS.PAM_NUMERO = {filtrosPesquisa.NumeroPagamento}");

            if (filtrosPesquisa.NumeroCarga > 0)
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");

            if (filtrosPesquisa.NumeroDocumento > 0)
                where.Append($" and PagamentoMotoristaTMS.CAR_CODIGO IN (SELECT cargaCTe.CAR_CODIGO FROM T_CARGA_CTE cargaCTe JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO where cte.CON_NUM = {filtrosPesquisa.NumeroDocumento})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Tomador > 0)
                where.Append($" and PagamentoMotoristaTMS.CAR_CODIGO IN (SELECT cargaCTe.CAR_CODIGO FROM T_CARGA_CTE cargaCTe JOIN T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO JOIN T_CTE_PARTICIPANTE tomador ON tomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE where tomador.CLI_CODIGO = {filtrosPesquisa.Tomador})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CpfCnpjFavorecido > 0)
                where.Append($" and Favorecido.CLI_CGCCPF = '{filtrosPesquisa.CpfCnpjFavorecido}'");

            if (filtrosPesquisa.PagamentosSemAcertoViagem)
            {
                SetarJoinsAcertoViagem(joins);

                where.Append(" and AcertoViagem.ACV_CODIGO is null");
            }

            if (filtrosPesquisa.DataEfetivacaoInicial.HasValue)
                where.Append($" and CAST(PagamentoMotoristaTMS.PAM_DATA_EFETIVACAO AS DATE) >= '{filtrosPesquisa.DataEfetivacaoInicial.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataEfetivacaoFinal.HasValue)
                where.Append($" and CAST(PagamentoMotoristaTMS.PAM_DATA_EFETIVACAO AS DATE) <= '{filtrosPesquisa.DataEfetivacaoFinal.Value.ToString(datePattern)}'");
        }

        #endregion
    }
}

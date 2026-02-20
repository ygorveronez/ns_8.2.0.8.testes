using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaContratoFreteAcrescimoDesconto : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto>
    {
        #region Construtores

        public ConsultaContratoFreteAcrescimoDesconto() : base(tabela: "T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO as acrecimoDesconto ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsAcrescimoDescontoIntegracao(StringBuilder joins)
        {
            if (!joins.Contains(" Integracao "))
                joins.Append("left join T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_INTEGRACAO Integracao on Integracao.CAD_CODIGO = acrecimoDesconto.CAD_CODIGO ");
        }

        private void SetarJoinsContratoFreteTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" contrato "))
                joins.Append(" inner join T_CONTRATO_FRETE_TERCEIRO contrato on contrato.CFT_CODIGO = acrecimoDesconto.CFT_CODIGO ");
        }

        private void SetarJoinsCargaCIOT(StringBuilder joins)
        {
            if (!joins.Contains(" cargaCIOT ")) 
            {
                SetarJoinsContratoFreteTerceiro(joins);
                joins.Append(" inner join T_CARGA_CIOT cargaCIOT on cargaCIOT.CFT_CODIGO = contrato.CFT_CODIGO ");
            }
                
        }

        private void SetarJoinsCIOT(StringBuilder joins)
        {
            if (!joins.Contains(" ciot "))
            {
                SetarJoinsCargaCIOT(joins);
                joins.Append(" inner join T_CIOT ciot on ciot.CIO_CODIGO = cargaCIOT.CIO_CODIGO ");
            }
                
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" carga "))
            {
                SetarJoinsCIOT(joins);
                joins.Append(" inner join T_CARGA carga on carga.CAR_CODIGO = cargaCIOT.CAR_CODIGO ");
            }
                
        }

        private void SetarJoinsJustificativa(StringBuilder joins)
        {
            if (!joins.Contains(" justificativa "))
                joins.Append(" inner join T_JUSTIFICATIVA justificativa on justificativa.JUS_CODIGO = acrecimoDesconto.JUS_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" motorista "))
            {
                SetarJoinsCIOT(joins);
                joins.Append(" left join T_FUNCIONARIO motorista on motorista.FUN_CODIGO = ciot.FUN_CODIGO ");
            }
                
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            if (!joins.Contains(" operador "))
            {
                SetarJoinsCIOT(joins);
                joins.Append(" left join T_FUNCIONARIO operador on operador.FUN_CODIGO = acrecimoDesconto.FUN_CODIGO ");
            }

        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumCiot":
                    if (!select.Contains(" NumCiot, "))
                    {
                        select.Append("ciot.CIO_NUMERO NumCiot, ");
                        groupBy.Append("ciot.CIO_NUMERO, ");

                        SetarJoinsCIOT(joins);
                    }
                    break;

                case "NumContratoFrete":
                    if (!select.Contains(" NumContratoFrete, "))
                    {
                        select.Append("Contrato.CFT_NUMERO_CONTRATO NumContratoFrete, ");
                        groupBy.Append("Contrato.CFT_NUMERO_CONTRATO, ");

                        SetarJoinsContratoFreteTerceiro(joins);

                    }
                    break;

                case "NumCarga":
                    if (!select.Contains(" NumCarga, "))
                    {
                        select.Append("carga.CAR_CODIGO_CARGA_EMBARCADOR NumCarga, ");
                        groupBy.Append("carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("justificativa.JUS_DESCRICAO Justificativa, ");
                        groupBy.Append("justificativa.JUS_DESCRICAO, ");

                        SetarJoinsJustificativa(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("acrecimoDesconto.CAD_VALOR Valor, ");
                        groupBy.Append("acrecimoDesconto.CAD_VALOR, ");


                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("acrecimoDesconto.CAD_OBSERVACAO Observacao, ");
                        groupBy.Append("acrecimoDesconto.CAD_OBSERVACAO, ");


                    }
                    break;

                case "DataLancamentoFormatada":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select.Append("acrecimoDesconto.CAD_DATA DataLancamento, ");
                        groupBy.Append("acrecimoDesconto.CAD_DATA, ");


                    }
                    break;
            
                case "SituacaoFormatada":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("acrecimoDesconto.CAD_SITUACAO Situacao, ");
                        groupBy.Append("acrecimoDesconto.CAD_SITUACAO, ");


                    }
                    break;

                case "RetornoIntegracaoFormatada":
                    if (!select.Contains(" RetornoIntegracao, "))
                    {
                        select.Append("integracao.INT_SITUACAO_INTEGRACAO RetornoIntegracao, ");
                        groupBy.Append("integracao.INT_SITUACAO_INTEGRACAO, ");

                        SetarJoinsAcrescimoDescontoIntegracao(joins);
                    }
                    break;

                case "CPFAutonomo":
                    if (!select.Contains(" CPFAutonomo, "))
                    {
                        select.Append("motorista.FUN_CPF CPFAutonomo, ");
                        groupBy.Append("motorista.FUN_CPF, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "NomeAutonomo":
                    if (!select.Contains(" NomeAutonomo, "))
                    {
                        select.Append("motorista.FUN_NOME NomeAutonomo, ");
                        groupBy.Append("motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "OperadorSolicitante":
                    if (!select.Contains(" OperadorSolicitante, "))
                    {
                        select.Append("operador.FUN_NOME OperadorSolicitante, ");
                        groupBy.Append("operador.FUN_NOME, ");
                        SetarJoinsOperador(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoFreteAcrescimoDesconto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCarga(joins);
            SetarJoinsJustificativa(joins);
            SetarJoinsCIOT(joins);
            SetarJoinsContratoFreteTerceiro(joins);
            SetarJoinsMotorista(joins);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append(" and CAST(acrecimoDesconto.CAD_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append(" and CAST(acrecimoDesconto.CAD_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'");


            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinsCarga(joins);
            }


            if (filtrosPesquisa.Justificativa>0 )
            {
                where.Append($" and justificativa.JUS_CODIGO = '{filtrosPesquisa.Justificativa}'");

                SetarJoinsJustificativa(joins);
            }

    
            if (filtrosPesquisa.NumCiot > 0)
            {
                where.Append($" and ciot.CIO_CODIGO = '{filtrosPesquisa.NumCiot}'");

                SetarJoinsCIOT(joins);
            }


            if (filtrosPesquisa.NumContratoFrete> 0)
            {
                where.Append($" and contrato.CFT_CODIGO = '{filtrosPesquisa.NumContratoFrete}'");

                SetarJoinsContratoFreteTerceiro(joins);
            }

            if (filtrosPesquisa.NumCarga > 0)
            {
                where.Append(" and carga.CAR_CODIGO = '" + filtrosPesquisa.NumCarga + "'");

                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Terceiro) && filtrosPesquisa.Terceiro != "0")
            {
                where.Append($" and motorista.FUN_CPF = '" + filtrosPesquisa.Terceiro + "'");

                SetarJoinsMotorista(joins);
            }
            
          

        }

        #endregion
    }
}

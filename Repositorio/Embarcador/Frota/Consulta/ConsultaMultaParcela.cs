using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaMultaParcela : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela>
    {
        #region Construtores

        public ConsultaMultaParcela() : base(tabela: "T_INFRACAO as Infracao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsInfracaoParcela(StringBuilder joins)
        {
            if (!joins.Contains(" InfracaoParcela "))
                joins.Append(" join T_INFRACAO_PARCELA InfracaoParcela on InfracaoParcela.INF_CODIGO = Infracao.INF_CODIGO ");
        }

        private void SetarJoinsTipoInfracao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoInfracao "))
                joins.Append(" left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ");
        }

        private void SetarJoinsInfracaoTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" InfracaoTitulo "))
                joins.Append(" left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" Titulo "))
                joins.Append(" left join T_TITULO Titulo on Titulo.TIT_CODIGO = Infracao.TIT_CODIGO ");
        }

        private void SetarJoinsInfracaoPagar(StringBuilder joins)
        {
            if (!joins.Contains(" InfracaoPagar "))
                joins.Append(" left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ");
        }

        private void SetarJoinsClienteFornecedorPagar(StringBuilder joins)
        {
            SetarJoinsInfracaoPagar(joins);

            if (!joins.Contains(" ClienteFornecedorPagar "))
                joins.Append(" left join T_CLIENTE ClienteFornecedorPagar on ClienteFornecedorPagar.CLI_CGCCPF = InfracaoPagar.CLI_CGCCPF ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Infracao.VEI_CODIGO ");
        }

        private void SetarJoinsCidade(StringBuilder joins)
        {
            if (!joins.Contains(" Cidade "))
                joins.Append(" left join T_LOCALIDADES Cidade on Cidade.LOC_CODIGO = Infracao.LOC_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append(" left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append(" left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = Infracao.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("InfracaoParcela.IFP_CODIGO Codigo, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "NumeroAtuacao":
                    if (!select.Contains(" NumeroAtuacao, "))
                    {
                        select.Append("Infracao.INF_NUMERO_ATUACAO NumeroAtuacao, ");
                    }
                    break;

                case "NumeroMulta":
                    if (!select.Contains(" NumeroMulta, "))
                    {
                        select.Append("Infracao.INF_NUMERO NumeroMulta, ");
                    }
                    break;

                case "DescricaoDataMulta":
                    if (!select.Contains(" DataMulta, "))
                    {
                        select.Append("Infracao.INF_DATA DataMulta, ");
                    }
                    break;

                case "DescricaoPagoPor":
                    if (!select.Contains(" PagoPor, "))
                    {
                        select.Append("Infracao.INF_RESPONSAVEL_PAGAMENTO_INFRACAO PagoPor, ");
                    }
                    break;

                case "LocalInfracao":
                    if (!select.Contains(" LocalInfracao, "))
                    {
                        select.Append("Infracao.INF_LOCAL LocalInfracao, ");
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("Cidade.LOC_DESCRICAO Cidade, ");

                        SetarJoinsCidade(joins);
                    }
                    break;

                case "TipoInfracao":
                    if (!select.Contains(" TipoInfracao, "))
                    {
                        select.Append("TipoInfracao.TIN_DESCRICAO TipoInfracao, ");

                        SetarJoinsTipoInfracao(joins);
                    }
                    break;

                case "DescricaoNivel":
                    if (!select.Contains(" Nivel, "))
                    {
                        select.Append("TipoInfracao.TIN_NIVEL Nivel, ");

                        SetarJoinsTipoInfracao(joins);
                    }
                    break;

                case "ValorTipoInfracao":
                    if (!select.Contains(" ValorTipoInfracao, "))
                    {
                        select.Append("TipoInfracao.TIN_VALOR ValorTipoInfracao, ");

                        SetarJoinsTipoInfracao(joins);
                    }
                    break;

                case "PontosTipoInfracao":
                    if (!select.Contains(" PontosTipoInfracao, "))
                    {
                        select.Append("TipoInfracao.TIN_PONTOS PontosTipoInfracao, ");

                        SetarJoinsTipoInfracao(joins);
                    }
                    break;

                case "ReducaoComissao":
                    if (!select.Contains(" ReducaoComissao, "))
                    {
                        select.Append("TipoInfracao.TIN_PERCENTUAL_REDUCAO_COMISSAO_MOTORISTA ReducaoComissao, ");

                        SetarJoinsTipoInfracao(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME Motorista, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        select.Append("Pessoa.CLI_NOME Pessoa, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "DescricaoVencimento":
                    if (!select.Contains(" Vencimento, "))
                    {
                        select.Append("InfracaoParcela.IFP_DATA_VENCIMENTO Vencimento, ");

                        SetarJoinsInfracaoParcela(joins);
                    }
                    break;

                case "DescricaoVencimentoPagar":
                    if (!select.Contains(" VencimentoPagar, "))
                    {
                        select.Append("InfracaoPagar.ITE_DATA_VENCIMENTO VencimentoPagar, ");

                        SetarJoinsInfracaoPagar(joins);
                    }
                    break;

                case "FornecedorPagar":
                    if (!select.Contains(" FornecedorPagar, "))
                    {
                        select.Append("ClienteFornecedorPagar.CLI_NOME FornecedorPagar, ");

                        SetarJoinsClienteFornecedorPagar(joins);
                    }
                    break;

                case "DescricaoCompensacao":
                    if (!select.Contains(" Compensacao, "))
                    {
                        select.Append("InfracaoTitulo.IFT_DATA_COMPENSACAO Compensacao, ");

                        SetarJoinsInfracaoTitulo(joins);
                    }
                    break;

                case "ValorAteVencimento":
                    if (!select.Contains(" ValorAteVencimento, "))
                    {
                        select.Append("InfracaoParcela.IFP_VALOR ValorAteVencimento, ");

                        SetarJoinsInfracaoParcela(joins);
                    }
                    break;

                case "ValorAposVencimento":
                    if (!select.Contains(" ValorAposVencimento, "))
                    {
                        select.Append("InfracaoParcela.IFP_VALOR_APOS_VENCIMENTO ValorAposVencimento, ");

                        SetarJoinsInfracaoParcela(joins);
                    }
                    break;

                case "CodigoTitulo":
                    if (!select.Contains(" CodigoTitulo, "))
                    {
                        select.Append("Titulo.TIT_CODIGO CodigoTitulo, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DescricaoSituacaoTitulo":
                    if (!select.Contains(" SituacaoTitulo, "))
                    {
                        select.Append("Titulo.TIT_STATUS SituacaoTitulo, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "SaldoTitulo":
                    if (!select.Contains(" SaldoTitulo, "))
                    {
                        select.Append("Titulo.TIT_VALOR_PENDENTE SaldoTitulo, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DescricaoStatusMulta":
                    if (!select.Contains(" StatusMulta, "))
                    {
                        select.Append("Infracao.INF_SITUACAO StatusMulta, ");
                    }
                    break;

                case "NumeroMatriculaMotorista":
                    if (!select.Contains(" NumeroMatriculaMotorista, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_MATRICULA NumeroMatriculaMotorista, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "NumeroParcela":
                    if (!select.Contains(" NumeroParcela, "))
                    {
                        select.Append("InfracaoParcela.IFP_PARCELA NumeroParcela, ");

                        SetarJoinsInfracaoParcela(joins);
                    }
                    break;

                case "TituloParcela":
                    if (!select.Contains(" TituloParcela, "))
                    {
                        select.Append("InfracaoParcela.TIT_CODIGO TituloParcela, ");

                        SetarJoinsInfracaoParcela(joins);
                    }
                    break;

                case "CodigoIntegracaoMotorista":
                    if (!select.Contains(" CodigoIntegracaoMotorista,"))
                    {
                        select.Append("Motorista.FUN_CODIGO_INTEGRACAO CodigoIntegracaoMotorista, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsInfracaoParcela(joins);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append(" and Infracao.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString());

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoTipoInfracao))
            {
                where.Append($" and TipoInfracao.TIN_TIPO = {filtrosPesquisa.TipoTipoInfracao} ");

                SetarJoinsTipoInfracao(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NivelInfracao))
            {
                where.Append($" and TipoInfracao.TIN_NIVEL = {filtrosPesquisa.NivelInfracao} ");

                SetarJoinsTipoInfracao(joins);
            }

            if (filtrosPesquisa.NumeroMulta > 0)
                where.Append(" and Infracao.INF_NUMERO = " + filtrosPesquisa.NumeroMulta.ToString());

            if (filtrosPesquisa.CodigoCidade > 0)
                where.Append(" and Infracao.LOC_CODIGO = " + filtrosPesquisa.CodigoCidade.ToString());

            if (filtrosPesquisa.CodigosTipoInfracoes.Count > 0)
                where.Append($" and Infracao.TIN_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosTipoInfracoes)}) ");

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append(" and Infracao.FUN_CODIGO_MOTORISTA = " + filtrosPesquisa.CodigoMotorista.ToString());

            if (filtrosPesquisa.CodigoTitulo > 0)
                where.Append(" and Infracao.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo.ToString());

            if (filtrosPesquisa.CnpjPessoa > 0)
                where.Append(" and Infracao.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString());

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAtuacao))
                where.Append(" and Infracao.INF_NUMERO_ATUACAO = '" + filtrosPesquisa.NumeroAtuacao + "'");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.DataLancamentoInicial != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA_LANCAMENTO AS DATE) >= '" + filtrosPesquisa.DataLancamentoInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataLancamentoFinal != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA_LANCAMENTO AS DATE) <= '" + filtrosPesquisa.DataLancamentoFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
            {
                where.Append(" and CAST(InfracaoParcela.IFP_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicial.ToString(pattern) + "'");

                SetarJoinsInfracaoParcela(joins);
            }

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                where.Append(" and CAST(InfracaoParcela.IFP_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinal.ToString(pattern) + "'");

                SetarJoinsInfracaoParcela(joins);
            }

            if (filtrosPesquisa.DataVencimentoInicialPagar != DateTime.MinValue)
            {
                where.Append(" and CAST(InfracaoPagar.ITE_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicialPagar.ToString(pattern) + "'");

                SetarJoinsInfracaoPagar(joins);
            }

            if (filtrosPesquisa.DataVencimentoFinalPagar != DateTime.MinValue)
            {
                where.Append(" and CAST(InfracaoPagar.ITE_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinalPagar.ToString(pattern) + "'");

                SetarJoinsInfracaoPagar(joins);
            }

            if (filtrosPesquisa.CnpjFornecedorPagar > 0)
            {
                where.Append(" and InfracaoPagar.CLI_CGCCPF = " + filtrosPesquisa.CnpjFornecedorPagar.ToString());

                SetarJoinsInfracaoPagar(joins);
            }

            if ((int)filtrosPesquisa.PagoPor > 0)
                where.Append(" and Infracao.INF_RESPONSAVEL_PAGAMENTO_INFRACAO = " + ((int)filtrosPesquisa.PagoPor).ToString());

            if ((int)filtrosPesquisa.StatusMulta > 0)
                where.Append(" and Infracao.INF_SITUACAO = " + ((int)filtrosPesquisa.StatusMulta).ToString());

            if (filtrosPesquisa.DataLimiteInicial != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA_LIMITE_INDICACAO_CONDUTOR AS DATE) >= '" + filtrosPesquisa.DataLimiteInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataLimiteFinal != DateTime.MinValue)
                where.Append(" and CAST(Infracao.INF_DATA_LIMITE_INDICACAO_CONDUTOR AS DATE) <= '" + filtrosPesquisa.DataLimiteFinal.ToString(pattern) + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoOcorrenciaInfracao))
                where.Append($" and Infracao.INF_TIPO_OCORRENCIA_INFRACAO = {filtrosPesquisa.TipoOcorrenciaInfracao} ");
        }

        #endregion
    }
}

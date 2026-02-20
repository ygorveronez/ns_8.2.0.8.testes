using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaProspeccao : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao>
    {
        #region Construtores

        public ConsultaProspeccao() : base(tabela: "T_PROSPECCAO as Prospeccao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Prospeccao.FUN_CODIGO ");
        }

        private void SetarJoinsProdutoProspect(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoProspect "))
                joins.Append(" left join T_PROSPECCAO_PRODUTO ProdutoProspect on ProdutoProspect.PP_CODIGO = Prospeccao.PP_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" left join T_CLIENTE_PROSPECT Cliente on Cliente.CPR_CODIGO = Prospeccao.CPR_CODIGO ");
        }

        private void SetarJoinsCidade(StringBuilder joins)
        {
            if (!joins.Contains(" Cidade "))
                joins.Append(" left join T_LOCALIDADES Cidade on Cidade.LOC_CODIGO = Prospeccao.LOC_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Prospeccao.EMP_CODIGO ");
        }

        private void SetarJoinsOrigemContatoClienteProspect(StringBuilder joins)
        {
            if (!joins.Contains(" OrigemContatoClienteProspect "))
                joins.Append(" left join T_PROSPECCAO_ORIGEM_CLIENTE OrigemContatoClienteProspect on OrigemContatoClienteProspect.OCC_CODIGO = Prospeccao.OCC_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtroPesquisa)
        {
            switch (propriedade)
            {
                case "DataLancamentoFormatada":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select.Append("Prospeccao.PRO_DATA_LANCAMENTO as DataLancamento, ");
                        groupBy.Append("Prospeccao.PRO_DATA_LANCAMENTO, ");
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Usuario.FUN_NOME as Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        select.Append("ProdutoProspect.PP_DESCRICAO as Produto, ");
                        groupBy.Append("ProdutoProspect.PP_DESCRICAO, ");

                        SetarJoinsProdutoProspect(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Cliente.CPR_NOME as Cliente, ");
                        groupBy.Append("Cliente.CPR_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "CNPJFormatado":
                    if (!select.Contains(" CNPJ, "))
                    {
                        select.Append("Prospeccao.PRO_CNPJ as CNPJ, ");
                        groupBy.Append("Prospeccao.PRO_CNPJ, ");
                    }
                    break;

                case "Contato":
                    if (!select.Contains(" Contato, "))
                    {
                        select.Append("Prospeccao.PRO_CONTATO as Contato, ");
                        groupBy.Append("Prospeccao.PRO_CONTATO, ");
                    }
                    break;

                case "Email":
                    if (!select.Contains(" Email, "))
                    {
                        select.Append("Prospeccao.PRO_EMAIL as Email, ");
                        groupBy.Append("Prospeccao.PRO_EMAIL, ");
                    }
                    break;

                case "Telefone":
                    if (!select.Contains(" Telefone, "))
                    {
                        select.Append("Prospeccao.PRO_TELEFONE as Telefone, ");
                        groupBy.Append("Prospeccao.PRO_TELEFONE, ");
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("Cidade.LOC_DESCRICAO + ' - ' + Cidade.UF_SIGLA as Cidade, ");
                        groupBy.Append("Cidade.LOC_DESCRICAO, Cidade.UF_SIGLA, ");

                        SetarJoinsCidade(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("Prospeccao.PRO_VALOR as Valor, ");
                        groupBy.Append("Prospeccao.PRO_VALOR, ");
                    }
                    break;

                case "DescricaoTipoContato":
                    if (!select.Contains(" TipoContato, "))
                    {
                        select.Append("Prospeccao.PRO_TIPO_CONTATO as TipoContato, ");
                        groupBy.Append("Prospeccao.PRO_TIPO_CONTATO, ");
                    }
                    break;

                case "OrigemContato":
                    if (!select.Contains(" OrigemContato, "))
                    {
                        select.Append("OrigemContatoClienteProspect.OCC_DESCRICAO as OrigemContato, ");
                        groupBy.Append("OrigemContatoClienteProspect.OCC_DESCRICAO, ");

                        SetarJoinsOrigemContatoClienteProspect(joins);
                    }
                    break;

                case "DescricaoSatisfacao":
                    if (!select.Contains(" Satisfacao, "))
                    {
                        select.Append("Prospeccao.ATE_SATISFACAO as Satisfacao, ");
                        groupBy.Append("Prospeccao.ATE_SATISFACAO, ");
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("Prospeccao.PRO_SITUACAO as Situacao, ");
                        groupBy.Append("Prospeccao.PRO_SITUACAO, ");
                    }
                    break;

                case "DataRetornoFormatada":
                    if (!select.Contains(" DataRetorno, "))
                    {
                        select.Append("Prospeccao.PRO_DATA_RETORNO as DataRetorno, ");
                        groupBy.Append("Prospeccao.PRO_DATA_RETORNO, ");
                    }
                    break;

                case "DescricaoFaturado":
                    if (!select.Contains(" Faturado, "))
                    {
                        select.Append("Prospeccao.PRO_FATURADO as Faturado, ");
                        groupBy.Append("Prospeccao.PRO_FATURADO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                where.Append(" AND Empresa.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa);
                SetarJoinsEmpresa(joins);
            }

            if (filtrosPesquisa.DataLancamentoInicial != DateTime.MinValue)
                where.Append(" and CAST(Prospeccao.PRO_DATA_LANCAMENTO AS DATE) >= '" + filtrosPesquisa.DataLancamentoInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataLancamentoFinal != DateTime.MinValue)
                where.Append(" and CAST(Prospeccao.PRO_DATA_LANCAMENTO AS DATE) <= '" + filtrosPesquisa.DataLancamentoFinal.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataRetornoInicial != DateTime.MinValue)
                where.Append(" and CAST(Prospeccao.PRO_DATA_RETORNO AS DATE) >= '" + filtrosPesquisa.DataRetornoInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataRetornoFinal != DateTime.MinValue)
                where.Append(" and CAST(Prospeccao.PRO_DATA_RETORNO AS DATE) <= '" + filtrosPesquisa.DataRetornoFinal.ToString(pattern) + "' ");

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                where.Append(" AND Usuario.FUN_CODIGO = " + filtrosPesquisa.CodigoUsuario);
                SetarJoinsUsuario(joins);
            }

            if (filtrosPesquisa.CodigoCidade > 0)
            {
                where.Append(" AND Cidade.LOC_CODIGO = " + filtrosPesquisa.CodigoCidade);
                SetarJoinsCidade(joins);
            }

            if (filtrosPesquisa.CodigoCliente > 0)
            {
                where.Append(" AND Cliente.CPR_CODIGO = " + filtrosPesquisa.CodigoCliente);
                SetarJoinsCliente(joins);
            }

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                where.Append(" AND ProdutoProspect.PP_CODIGO = " + filtrosPesquisa.CodigoProduto);
                SetarJoinsProdutoProspect(joins);
            }

            if (filtrosPesquisa.CodigoOrigemContato > 0)
            {
                where.Append(" AND OrigemContatoClienteProspect.OCC_CODIGO = " + filtrosPesquisa.CodigoOrigemContato);
                SetarJoinsOrigemContatoClienteProspect(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CNPJ))
                where.Append(" AND Prospeccao.PRO_CNPJ = '" + filtrosPesquisa.CNPJ + "'");

            if (filtrosPesquisa.Faturado.HasValue)
                where.Append(" AND Prospeccao.PRO_FATURADO = " + (filtrosPesquisa.Faturado.Value ? "1" : "0"));

            if (filtrosPesquisa.TipoContato.HasValue)
                where.Append(" AND Prospeccao.PRO_TIPO_CONTATO = " + filtrosPesquisa.TipoContato.Value.ToString("d"));

            if (filtrosPesquisa.Satisfacao.HasValue)
                where.Append(" AND Prospeccao.ATE_SATISFACAO = " + filtrosPesquisa.Satisfacao.Value.ToString("d"));

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append(" AND Prospeccao.PRO_SITUACAO = " + filtrosPesquisa.Situacao.Value.ToString("d"));
        }

        #endregion
    }
}
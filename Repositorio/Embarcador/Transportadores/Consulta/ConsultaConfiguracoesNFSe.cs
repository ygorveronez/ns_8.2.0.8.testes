using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Transportadores
{
    sealed class ConsultaConfiguracoesNFSe : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe>
    {
        #region Construtores

        public ConsultaConfiguracoesNFSe() : base(tabela: "T_EMPRESA_CONFIGURACAO_NFSE as ConfiguracaoNFSe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsLocalidadePrestacaoServico(StringBuilder joins)
        {
            if (!joins.Contains(" LocalidadePrestacaoServico "))
                joins.Append(" left join T_LOCALIDADES LocalidadePrestacaoServico on ConfiguracaoNFSe.LOC_PRESTACAO_CODIGO = LocalidadePrestacaoServico.LOC_CODIGO ");
        }
        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on ConfiguracaoNFSe.EMP_CODIGO = Transportador.EMP_CODIGO ");
        }
        private void SetarJoinsSerie(StringBuilder joins)
        {
            if (!joins.Contains(" Serie "))
                joins.Append(" left join T_EMPRESA_SERIE Serie on ConfiguracaoNFSe.COF_SERIE_NFSE = Serie.ESE_CODIGO ");
        }

        private void SetarJoinsServico(StringBuilder joins)
        {
            if (!joins.Contains(" Servico "))
                joins.Append(" left join T_NFSE_SERVICO Servico on ConfiguracaoNFSe.SER_CODIGO = Servico.SER_CODIGO ");
        }

        private void SetarJoinsNatureza(StringBuilder joins)
        {
            if (!joins.Contains(" Natureza "))
                joins.Append(" left join T_NFSE_NATUREZA Natureza on ConfiguracaoNFSe.NAN_CODIGO = Natureza.NAN_CODIGO ");
        }
        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on ConfiguracaoNFSe.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinsLocalidadeTomador(StringBuilder joins)
        {
            if (!joins.Contains(" LocalidadeTomador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTomador on ConfiguracaoNFSe.LOC_CODIGO_TOMADOR = LocalidadeTomador.LOC_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtroPesquisa)
        {

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("ConfiguracaoNFSe.ECN_CODIGO as Codigo, ");
                        groupBy.Append("ConfiguracaoNFSe.ECN_CODIGO, ");
                    }
                    break;

                case "LocalidadePrestacaoServico":
                    if (!select.Contains(" LocalidadePrestacaoServico,"))
                    {
                        select.Append("LocalidadePrestacaoServico.LOC_DESCRICAO as LocalidadePrestacaoServico, ");
                        groupBy.Append("LocalidadePrestacaoServico.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadePrestacaoServico(joins);
                    }
                    break;

                case "UFLocalidadePrestacaoServico":
                    if (!select.Contains(" UFLocalidadePrestacaoServico,"))
                    {
                        select.Append("LocalidadePrestacaoServico.UF_SIGLA as UFLocalidadePrestacaoServico, ");
                        groupBy.Append("LocalidadePrestacaoServico.UF_SIGLA, ");

                        SetarJoinsLocalidadePrestacaoServico(joins);
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie,"))
                    {
                        select.Append("Serie.ESE_NUMERO as Serie, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "SerieRPS":
                    if (!select.Contains(" SerieRPS,"))
                    {
                        select.Append("ConfiguracaoNFSe.ECN_SERIE_RPS as SerieRPS, ");
                        groupBy.Append("ConfiguracaoNFSe.ECN_SERIE_RPS, ");
                    }
                    break;

                case "Servico":
                    if (!select.Contains(" Servico,"))
                    {
                        select.Append("Servico.SER_DESCRICAO as Servico, ");
                        groupBy.Append("Servico.SER_DESCRICAO, ");

                        SetarJoinsServico(joins);
                    }
                    break;

                case "CodigoServico":
                    if (!select.Contains(" CodigoServico,"))
                    {
                        select.Append("Servico.SER_NUMERO as CodigoServico, ");
                        groupBy.Append("Servico.SER_NUMERO, ");

                        SetarJoinsServico(joins);
                    }
                    break;

                case "Natureza":
                    if (!select.Contains(" Natureza,"))
                    {
                        select.Append("Natureza.NAN_DESCRICAO as Natureza, ");
                        groupBy.Append("Natureza.NAN_DESCRICAO, ");

                        SetarJoinsNatureza(joins);
                    }
                    break;

                case "Aliquota":
                    if (!select.Contains(" Aliquota,"))
                    {
                        select.Append("ConfiguracaoNFSe.ECN_ALIQUOTA_ISS as Aliquota, ");
                        groupBy.Append("ConfiguracaoNFSe.ECN_ALIQUOTA_ISS, ");
                    }
                    break;

                case "Retencao":
                    if (!select.Contains(" Retencao,"))
                    {
                        select.Append("ConfiguracaoNFSe.ECN_RETENCAO_ISS as Retencao, ");
                        groupBy.Append("ConfiguracaoNFSe.ECN_RETENCAO_ISS, ");
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

                case "IncluirValorISSBaseCalculo":
                case "IncluirValorISSBaseCalculoFormatado":
                    if (!select.Contains(" IncluirValorISSBaseCalculo,"))
                    {
                        select.Append("ConfiguracaoNFSe.ECN_INCLUIR_ISS_BASE_CALCULO as IncluirValorISSBaseCalculo, ");
                        groupBy.Append("ConfiguracaoNFSe.ECN_INCLUIR_ISS_BASE_CALCULO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Transportador.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                SetarJoinsTransportador(joins);
            }

            if (filtrosPesquisa.CodigoLocalidadePrestacaoServico > 0)
            {
                where.Append($" and LocalidadePrestacaoServico.LOC_CODIGO = {filtrosPesquisa.CodigoLocalidadePrestacaoServico}");

                SetarJoinsLocalidadePrestacaoServico(joins);
            }

            if (filtrosPesquisa.CodigoServico > 0)
            {
                where.Append($" and Servico.SER_CODIGO = {filtrosPesquisa.CodigoServico}");

                SetarJoinsServico(joins);
            }

            if (filtrosPesquisa.CPFCNPJClienteTomador > 0d)
            {
                where.Append($" and ConfiguracaoNFSe.CLI_CGCCPF = {filtrosPesquisa.CPFCNPJClienteTomador}");
            }

            if (filtrosPesquisa.CodigoGrupoTomador > 0)
            {
                where.Append($" and ConfiguracaoNFSe.GRP_TOMADOR = {filtrosPesquisa.CodigoGrupoTomador}");
            }

            if (filtrosPesquisa.CodigoLocalidadeTomador > 0)
            {
                where.Append($" and ConfiguracaoNFSe.LOC_CODIGO_TOMADOR = {filtrosPesquisa.CodigoLocalidadeTomador}");
            }

            if (filtrosPesquisa.CodigoUFTomador > 0)
            {
                where.Append($" and LocalidadeTomador.UF_SIGLA = {filtrosPesquisa.CodigoUFTomador}");

                SetarJoinsLocalidadeTomador(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($" and ConfiguracaoNFSe.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

                SetarJoinsTipoOperacao(joins);
            }

        }

    }

}

using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ConsultaExtracaoMassivaTabelaFreteCliente : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente>
    {
        #region Propriedades

        private const string COLUNA_JOIN_PARAMETROBASECALCULO = "#COLUNA_JOIN_PARAMETROBASECALCULO#";
        private const string BLOQUEIO_PARAMETROBASECALCULO_ATUAL = "#BLOQUEIO_PARAMETROBASECALCULO_ATUAL#";
        private const string FILTRO_PARAMETROBASECALCULO_ATUAL = "#FILTRO_PARAMETROBASECALCULO#";
        private const string VALOR_COLUNA_EXCLUSAO = "#COLUNA_EXCLUSAO#";

        #endregion

        #region Construtores

        public ConsultaExtracaoMassivaTabelaFreteCliente() : base("")
        {

        }

        #endregion

        #region Métodos Privados 

        private string SetarTabela(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtroPesquisa)
        {
            StringBuilder sbTabela = new StringBuilder();
            sbTabela.Append($@"(SELECT TFC_CODIGO
                                     , TBF_CODIGO
                                     , CASE WHEN T_TABELA_FRETE_CLIENTE.TFC_TABELA_ORIGINARIA IS NULL THEN 
				                        (SELECT MAX(TFC_CODIGO) FROM T_TABELA_FRETE_CLIENTE Auxiliar WHERE Auxiliar.TFC_TABELA_ORIGINARIA = T_TABELA_FRETE_CLIENTE.TFC_CODIGO AND Auxiliar.TBF_CODIGO = T_TABELA_FRETE_CLIENTE.TBF_CODIGO)
			                           ELSE
				                        ISNULL((SELECT MAX(TFC_CODIGO) FROM T_TABELA_FRETE_CLIENTE Auxiliar WHERE Auxiliar.TFC_TABELA_ORIGINARIA = T_TABELA_FRETE_CLIENTE.TFC_TABELA_ORIGINARIA AND Auxiliar.TBF_CODIGO = T_TABELA_FRETE_CLIENTE.TBF_CODIGO AND Auxiliar.TFC_CODIGO < T_TABELA_FRETE_CLIENTE.TFC_CODIGO), 0)
		                               END AS TFC_CODIGO_ANTERIOR
                                     , {(filtroPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores > 0 ? "0 AS HIO_CODIGO" : @"
                                       CASE WHEN T_TABELA_FRETE_CLIENTE.TBF_DATA_HISTORICO_ALTERACAO IS NULL THEN 
                                        (SELECT MIN(HIO_CODIGO) FROM T_HISTORICO_OBJETO WHERE HIO_OBJETO = 'TabelaFreteCliente' AND HIO_ACAO IN (0, 1, 3) AND HIO_CODIGO_OBJETO = ISNULL(TFC_TABELA_ORIGINARIA, TFC_CODIGO))
			                           ELSE 
                                        (SELECT MIN(HIO_CODIGO) FROM T_HISTORICO_OBJETO WHERE HIO_OBJETO = 'TabelaFreteCliente' AND HIO_ACAO IN (0, 1, 3) AND HIO_CODIGO_OBJETO = ISNULL(TFC_TABELA_ORIGINARIA, TFC_CODIGO) AND HIO_DATA > TBF_DATA_HISTORICO_ALTERACAO)
		                               END AS HIO_CODIGO")}
                                  FROM T_TABELA_FRETE_CLIENTE 
                                 WHERE TBF_CODIGO in ({string.Join(",", filtroPesquisa.CodigosTabelasFrete)})
                                   AND {(filtroPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores > 0 ? "TFC_CODIGO = " + filtroPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores : " 0 = 0")}
                               ) AS HistoricoTabelaFreteCliente");

            return sbTabela.ToString();
        }

        private void SetarJoins(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtroPesquisa, StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFreteCliente "))
                joins.Append(@" JOIN T_TABELA_FRETE_CLIENTE TabelaFreteCliente ON TabelaFreteCliente.TFC_CODIGO = HistoricoTabelaFreteCliente.TFC_CODIGO");

            if (filtroPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores > 0)
                return;

            if (!joins.Contains(" HistoricoObjetoTabelaFreteCliente "))
                joins.Append(@" JOIN T_HISTORICO_OBJETO HistoricoObjetoTabelaFreteCliente ON HistoricoObjetoTabelaFreteCliente.HIO_CODIGO = HistoricoTabelaFreteCliente.HIO_CODIGO");
        }

        private void SetarJoinsUsuarioAlteracao(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioAlteracao "))
                joins.Append(@" JOIN T_FUNCIONARIO UsuarioAlteracao ON UsuarioAlteracao.FUN_CODIGO = HistoricoObjetoTabelaFreteCliente.FUN_CODIGO");
        }

        private void SetarJoinsTabelaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFrete "))
                joins.Append(@" JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = HistoricoTabelaFreteCliente.TBF_CODIGO");
        }

        private void SetarJoinsTabelaFreteVigencia(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFreteVigencia "))
                joins.Append(@" JOIN T_TABELA_FRETE_VIGENCIA TabelaFreteVigencia ON TabelaFreteVigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO");
        }

        private void SetarJoinsParametroBaseCalculo(StringBuilder joins)
        {
            if (!joins.Contains(" ParametroBaseCalculo "))
                joins.Append(@$" LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBaseCalculo ON ParametroBaseCalculo.TFC_CODIGO = HistoricoTabelaFreteCliente.{COLUNA_JOIN_PARAMETROBASECALCULO}");

            if (!joins.Contains(" ParametroBaseCalculoItem "))
                joins.Append(@" LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ParametroBaseCalculoItem ON ParametroBaseCalculoItem.TBC_CODIGO = ParametroBaseCalculo.TBC_CODIGO");

            if (!joins.Contains(" ParametroBaseCalculoAtual "))
                joins.Append(@$" LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBaseCalculoAtual ON {BLOQUEIO_PARAMETROBASECALCULO_ATUAL} AND ParametroBaseCalculoAtual.TFC_CODIGO = HistoricoTabelaFreteCliente.TFC_CODIGO AND ParametroBaseCalculoAtual.TBC_CODIGO_OBJETO = ParametroBaseCalculo.TBC_CODIGO_OBJETO");

            if (!joins.Contains(" ParametroBaseCalculoItemAtual "))
                joins.Append(@$" LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ParametroBaseCalculoItemAtual ON {BLOQUEIO_PARAMETROBASECALCULO_ATUAL} AND ParametroBaseCalculoItemAtual.TBC_CODIGO = ParametroBaseCalculoAtual.TBC_CODIGO AND ParametroBaseCalculoItemAtual.TPI_TIPO_OBJETO = ParametroBaseCalculoItem.TPI_TIPO_OBJETO AND ParametroBaseCalculoItemAtual.TPI_CODIGO_OBJETO = ParametroBaseCalculoItem.TPI_CODIGO_OBJETO");
        }

        private void SetarGroupBy(StringBuilder groupBy)
        {
            if (!groupBy.Contains("TabelaFreteCliente.TFC_TABELA_ORIGINARIA, "))
                groupBy.Append("TabelaFreteCliente.TFC_TABELA_ORIGINARIA, ");

            if (!groupBy.Contains("TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, "))
                groupBy.Append("TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, ");

            if (!groupBy.Contains("HistoricoObjetoTabelaFreteCliente.HIO_DATA, "))
                groupBy.Append("HistoricoObjetoTabelaFreteCliente.HIO_DATA, ");
        }

        private void SetarColunasOrderBy(StringBuilder select, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, StringBuilder orderBy)
        {
            if (parametrosConsulta != null && !string.IsNullOrEmpty(parametrosConsulta.PropriedadeOrdenar) && select.Contains(parametrosConsulta.PropriedadeOrdenar))
                orderBy.Append(", ");

            if (parametrosConsulta != null && !string.IsNullOrEmpty(parametrosConsulta.PropriedadeOrdenar) && !parametrosConsulta.PropriedadeOrdenar.Contains("DataAlteracao"))
                orderBy.Append(@" CASE WHEN TabelaFreteCliente.TFC_TABELA_ORIGINARIA IS NULL THEN ISNULL(TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, HistoricoObjetoTabelaFreteCliente.HIO_DATA)
                                       ELSE HistoricoObjetoTabelaFreteCliente.HIO_DATA 
                                  END");
        }

        private void SetarSelectPropriedadesParametros(StringBuilder select, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtroPesquisa, StringBuilder joins, StringBuilder groupBy)
        {
            if (!select.Contains(" ValorItemDepois,"))
            {
                Dictionary<string, string> camposValorItem = new Dictionary<string, string>()
                {
                    { "TabelaFrete.TBF_CODIGO", "CodigoTabelaFrete" },
                    { "TabelaFrete.TBF_PARAMETRO_BASE", "TipoParametroBaseTabelaFrete" },
                    { "TabelaFreteVigencia.TFV_DATA_INICIAL", "DataHoraInicialVigenciaDepois" },
                    { "TabelaFreteVigencia.TFV_DATA_FINAL", "DataHoraFinalVigenciaDepois" },
                    { "TabelaFreteCliente.TFC_DESCRICAO_ORIGEM", "DescricaoOrigemDepois" },
                    { "TabelaFreteCliente.TFC_DESCRICAO_DESTINO", "DescricaoDestinoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_MINIMO_GARANTIDO", "ValorMinimoGarantidoParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_ENTREGA_EXCEDENTE", "ValorEntregaExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_PALLET_EXCEDENTE", "ValorPalletExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE", "ValorQuilometragemParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_PESO_EXCEDENTE", "ValorPesoExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_AJUDANTE_EXCEDENTE", "ValorAjudanteExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_MAXIMO", "ValorMaximoParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_PERCENTUAL_PAGAMENTO_AGREGADO", "PercentualPagamentoAgregadoParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_BASE", "ValorBaseParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_HORA_EXCEDENTE", "ValorHoraExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_VALOR_PACOTE_EXCEDENTE", "ValorPacoteExcedenteParametroBaseCalculoDepois" },
                    { "ParametroBaseCalculo.TBC_CODIGO_OBJETO", "CodigoObjetoParametroBaseCalculo" },
                    { "ParametroBaseCalculoItem.TPI_TIPO_VALOR", "TipoValorItem" },
                    { "ParametroBaseCalculoItem.TPI_TIPO_OBJETO", "TipoParametroObjetoItem" },
                    { "ParametroBaseCalculoItem.TPI_CODIGO_OBJETO", "CodigoObjetoItem" },
                    { "ParametroBaseCalculoItem.TPI_VALOR", "ValorItemDepois" },
                    { "ParametroBaseCalculoItem.TPI_CODIGO", "CodigoItem" }
                };

                if (filtroPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores == 0)
                    camposValorItem.Add("HistoricoObjetoTabelaFreteCliente.HIO_DESCRICAO_ACAO", "DescricaoAcaoAuditoria");

                foreach (var campo in camposValorItem)
                    AdicionarSelectComGroupBy(campo.Key, campo.Value, select, groupBy);

                SetarJoinsTabelaFrete(joins);
                SetarJoinsParametroBaseCalculo(joins);
                SetarJoinsTabelaFreteVigencia(joins);
            }

            if (!select.Contains(" CodigoTabelaFreteCliente,"))
            {
                select.Append("TabelaFreteCliente.TFC_CODIGO as CodigoTabelaFreteCliente, ");
                groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
            }

            if (!select.Contains(" CodigoTabelaFreteClienteOriginal,"))
            {
                select.Append("TabelaFreteCliente.TFC_TABELA_ORIGINARIA as CodigoTabelaFreteClienteOriginal, ");
                groupBy.Append("TabelaFreteCliente.TFC_TABELA_ORIGINARIA, ");
            }
        }

        private void AdicionarSelectComGroupBy(string campo, string alias, StringBuilder select, StringBuilder groupBy)
        {
            if (!select.Contains($" {alias},"))
                select.Append($"{campo} {alias}, ");

            if (!groupBy.Contains($"{campo}, "))
                groupBy.Append($"{campo}, ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtroPesquisa)
        {
            if (ObterPropriedadesParametros().Contains(propriedade))
            {
                SetarSelectPropriedadesParametros(select, filtroPesquisa, joins, groupBy);
            }

            switch (propriedade)
            {
                case "CodigoTabelaFreteCliente":
                    if (!select.Contains(" CodigoTabelaFreteCliente,"))
                    {
                        select.Append("TabelaFreteCliente.TFC_CODIGO as CodigoTabelaFreteCliente, ");
                        groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                    }
                    break;
                case "DescricaoDataAlteracao":
                case "DataAlteracao":
                    if (!select.Contains(" DataAlteracao,"))
                    {
                        select.Append(@"CASE 
                                        WHEN TabelaFreteCliente.TFC_TABELA_ORIGINARIA IS NULL THEN ISNULL(TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, HistoricoObjetoTabelaFreteCliente.HIO_DATA)
                                        ELSE HistoricoObjetoTabelaFreteCliente.HIO_DATA 
                                        END AS DataAlteracao, ");
                    }
                    break;
                case "UsuarioAlteracao":
                    if (!select.Contains(" UsuarioAlteracao,"))
                    {
                        select.Append("UsuarioAlteracao.FUN_NOME as UsuarioAlteracao, ");
                        groupBy.Append("UsuarioAlteracao.FUN_NOME, ");

                        SetarJoinsUsuarioAlteracao(joins);
                    }
                    break;
                case "DescricaoTabelaFrete":
                    if (!select.Contains(" DescricaoTabelaFrete,"))
                    {
                        select.Append(@"TabelaFrete.TBF_DESCRICAO DescricaoTabelaFrete, ");
                        groupBy.Append("TabelaFrete.TBF_DESCRICAO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;
                case "DescricaoSituacaoTabelaFrete":
                    if (!select.Contains(" SituacaoTabelaFrete,"))
                    {
                        select.Append(@"TabelaFrete.TBF_ATIVO SituacaoTabelaFrete, ");
                        groupBy.Append("TabelaFrete.TBF_ATIVO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;
                case "AprovadorTabelaFreteCliente":
                    if (!select.Contains(" AprovadorTabelaFreteCliente,"))
                    {
                        select.Append($@"(SELECT TOP 1 Funcionario.FUN_NOME
                                            FROM T_AUTORIZACAO_ALCADA_TABELA_FRETE_ALTERACAO AutorizacaoAlcadaTabelaFreteAlteracao 
                                            JOIN T_TABELA_FRETE_CLIENTE_ALTERACAO TabelaFreteClienteAlteracao ON TabelaFreteClienteAlteracao.TFA_CODIGO = AutorizacaoAlcadaTabelaFreteAlteracao.TFA_CODIGO
                                            JOIN T_TABELA_FRETE_ALTERACAO TabelaFreteAlteracao ON TabelaFreteAlteracao.TFA_CODIGO = TabelaFreteClienteAlteracao.TFA_CODIGO
                                            JOIN T_FUNCIONARIO Funcionario ON Funcionario.FUN_CODIGO = AutorizacaoAlcadaTabelaFreteAlteracao.FUN_CODIGO
                                           WHERE TabelaFreteClienteAlteracao.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                                             AND TabelaFreteAlteracao.TFA_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete.Aprovada}
                                             AND AutorizacaoAlcadaTabelaFreteAlteracao.AAL_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada}
                                             AND AutorizacaoAlcadaTabelaFreteAlteracao.AAL_DATA IS NOT NULL
                                             AND TabelaFreteClienteAlteracao.TCA_DATA_ALTERACAO BETWEEN DATEADD(SECOND, -(60 - DATEPART(SECOND, HistoricoObjetoTabelaFreteCliente.HIO_DATA)), HistoricoObjetoTabelaFreteCliente.HIO_DATA) AND DATEADD(SECOND, 60, HistoricoObjetoTabelaFreteCliente.HIO_DATA)
                                        ORDER BY TabelaFreteClienteAlteracao.TCA_DATA_ALTERACAO ASC
                                               , AutorizacaoAlcadaTabelaFreteAlteracao.AAL_DATA DESC) AprovadorTabelaFreteCliente, ");

                        if (!groupBy.Contains("TabelaFreteCliente.TFC_CODIGO,"))
                            groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                    }
                    break;
            }
        }

        protected override SQLDinamico ObterSql(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarJoins(filtrosPesquisa, joins);

            if (filtrosPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores == 0)
                foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                    SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            else
                SetarSelectPropriedadesParametros(select, filtrosPesquisa, joins, groupBy);

            SetarOrderBy(parametrosConsulta, select, orderBy, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            SetarColunasOrderBy(select, parametrosConsulta, orderBy);
            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            if (filtrosPesquisa.CodigoTabelaFreteClientePesquisaParametrosAnteriores == 0)
                SetarGroupBy(groupBy);

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append("select 1 as counter");
            else
                sql.Append($"select {(_somenteRegistrosDistintos ? "distinct " : "")}{(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")}, CAST({VALOR_COLUNA_EXCLUSAO} AS BIT) as Exclusao "); 

            sql.Append($" from {SetarTabela(filtrosPesquisa)} ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append($" where {condicoes.Substring(4)} AND {FILTRO_PARAMETROBASECALCULO_ATUAL} ");

            if (agrupamentos.Length > 0)
                sql.Append($" group by {agrupamentos.Substring(0, agrupamentos.Length - 1)} ");

            if (sql.IndexOf(COLUNA_JOIN_PARAMETROBASECALCULO) > 0)
            {
                string sqlExclusao = sql.ToString();
                sqlExclusao = sqlExclusao.Replace(COLUNA_JOIN_PARAMETROBASECALCULO, "TFC_CODIGO_ANTERIOR");
                sqlExclusao = sqlExclusao.Replace(BLOQUEIO_PARAMETROBASECALCULO_ATUAL, "1 = 1");
                sqlExclusao = sqlExclusao.Replace(FILTRO_PARAMETROBASECALCULO_ATUAL, "ParametroBaseCalculo.TBC_CODIGO IS NOT NULL AND ParametroBaseCalculoItem.TPI_CODIGO IS NOT NULL AND ParametroBaseCalculoItemAtual.TPI_CODIGO IS NULL");
                sqlExclusao = sqlExclusao.Replace(VALOR_COLUNA_EXCLUSAO, "1");

                sql.Replace(COLUNA_JOIN_PARAMETROBASECALCULO, "TFC_CODIGO");
                sql.Replace(BLOQUEIO_PARAMETROBASECALCULO_ATUAL, "0 = 1");
                sql.Replace(FILTRO_PARAMETROBASECALCULO_ATUAL, "1 = 1");
                sql.Replace(VALOR_COLUNA_EXCLUSAO, "0");

                sql.Append($" UNION ALL {sqlExclusao}");
            }

            if (somenteContarNumeroRegistros)
            {
                sql.Insert(0, "select distinct(count(0) over ()) from (");
                sql.Append(") as contador");
            }
            else
            {
                sql.Append($" order by {(orderBy.Length > 0 ? orderBy.ToString() : "1 asc")}");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), null);
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataInicialAlteracao.HasValue)
                where.Append($" AND CONVERT(DATE, HistoricoObjetoTabelaFreteCliente.HIO_DATA) >= '{filtrosPesquisa.DataInicialAlteracao.Value.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinalAlteracao.HasValue)
                where.Append($" AND CONVERT(DATE, HistoricoObjetoTabelaFreteCliente.HIO_DATA) <= '{filtrosPesquisa.DataFinalAlteracao.Value.ToString("yyyy-MM-dd")}'");
        }

        #endregion

        #region Métodos Públicos

        public static List<string> ObterPropriedadesParametros()
        {
            return new List<string>()
            {
                "TipoParametroBaseTabelaFrete",
                "DescricaoTipoParametroBaseTabelaFrete",
                "CodigoObjetoParametroBaseCalculo",
                "DescricaoObjetoParametroBaseCalculo",
                "DataInicialVigenciaAntes",
                "DataInicialVigenciaDepois",
                "DataFinalVigenciaAntes",
                "DataFinalVigenciaDepois",
                "DescricaoOrigemAntes",
                "DescricaoOrigemDepois",
                "DescricaoDestinoAntes",
                "DescricaoDestinoDepois",
                "DescricaoValorMinimoGarantidoParametroBaseCalculoAntes",
                "ValorMinimoGarantidoParametroBaseCalculoDepois",
                "DescricaoValorMinimoGarantidoParametroBaseCalculoDepois",
                "DescricaoValorEntregaExcedenteParametroBaseCalculoAntes",
                "ValorEntregaExcedenteParametroBaseCalculoDepois",
                "DescricaoValorEntregaExcedenteParametroBaseCalculoDepois",
                "DescricaoValorPalletExcedenteParametroBaseCalculoAntes",
                "ValorPalletExcedenteParametroBaseCalculoDepois",
                "DescricaoValorPalletExcedenteParametroBaseCalculoDepois",
                "DescricaoValorQuilometragemExcedenteParametroBaseCalculoAntes",
                "ValorQuilometragemParametroBaseCalculoDepois",
                "DescricaoValorQuilometragemParametroBaseCalculoDepois",
                "DescricaoValorPesoExcedenteParametroBaseCalculoAntes",
                "ValorPesoExcedenteParametroBaseCalculoDepois",
                "DescricaoValorPesoExcedenteParametroBaseCalculoDepois",
                "DescricaoValorAjudanteExcedenteParametroBaseCalculoAntes",
                "ValorAjudanteExcedenteParametroBaseCalculoDepois",
                "DescricaoValorAjudanteExcedenteParametroBaseCalculoDepois",
                "DescricaoValorMaximoParametroBaseCalculoAntes",
                "ValorMaximoParametroBaseCalculoDepois",
                "DescricaoValorMaximoParametroBaseCalculoDepois",
                "DescricaoPercentualPagamentoAgregadoParametroBaseCalculoAntes",
                "PercentualPagamentoAgregadoParametroBaseCalculoDepois",
                "DescricaoPercentualPagamentoAgregadoParametroBaseCalculoDepois",
                "DescricaoValorBaseParametroBaseCalculoAntes",
                "ValorBaseParametroBaseCalculoDepois",
                "DescricaoValorBaseParametroBaseCalculoDepois",
                "DescricaoValorHoraExcedenteParametroBaseCalculoAntes",
                "ValorHoraExcedenteParametroBaseCalculoDepois",
                "DescricaoValorHoraExcedenteParametroBaseCalculoDepois",
                "DescricaoValorPacoteExcedenteParametroBaseCalculoAntes",
                "ValorPacoteExcedenteParametroBaseCalculoDepois",
                "DescricaoValorPacoteExcedenteParametroBaseCalculoDepois",
                "TipoParametroObjetoItem",
                "DescricaoTipoParametroObjetoItem",
                "CodigoObjetoItem",
                "DescricaoObjetoItem",
                "TipoValorItem",
                "DescricaoTipoValorItem",
                "ValorItemDepois",
                "DescricaoValorItemDepois",
                "DescricaoValorItemAntes",
                "CodigoItem",
                "TipoAcao"
            };
        }

        #endregion
    }
}

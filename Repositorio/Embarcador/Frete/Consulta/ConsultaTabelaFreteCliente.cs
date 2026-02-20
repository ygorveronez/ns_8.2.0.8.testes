using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaTabelaFreteCliente : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente>
    {
        #region Atributos

        private StringBuilder _where;
        private StringBuilder _wherePorTipoRegistro;

        #endregion

        #region Construtores

        public ConsultaTabelaFreteCliente() : base(tabela: "T_TABELA_FRETE_CLIENTE TabelaFreteCliente") { }

        #endregion

        #region Métodos Privados 

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedade, string direcaoOrdenacao, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa)
        {
            if (string.IsNullOrWhiteSpace(propriedade))
                return propriedade;

            if (propriedade == "DescricaoRemetente")
                return "Remetente";

            if (propriedade == "DescricaoDestinatario")
                return "Destinatario";

            if (propriedade == "DescricaoEmpresa")
                return "Empresa";

            if (propriedade == "Tomador")
                return "Tomador.CLI_NOME";

            if (propriedade == "GrupoPessoas")
                return "GrupoPessoas.GRP_DESCRICAO";

            if (propriedade == "DescricaoTipoPagamento")
                return "TabelaFreteCliente.TFC_TIPO_PAGAMENTO";

            else if (propriedade == "DataInicial")
                return "Vigencia.TFV_DATA_INICIAL";

            else if (propriedade == "DataFinal")
                return "Vigencia.TFV_DATA_FINAL";

            else if (propriedade == "NumeroEntrega")
                return $"ItemItemEntrega.TFN_NUMERO_INICIAL_ENTREGA {direcaoOrdenacao}, ItemItemEntrega.TFN_NUMERO_FINAL_ENTREGA";

            else if (propriedade == "DescricaoValorEntrega")
                return "ItemEntrega.TPI_VALOR";

            else if (propriedade == "DescricaoPeso")
                return $"ItemItemPeso.TFP_PESO {direcaoOrdenacao}, ItemItemPeso.TFP_PESO_INICIAL {direcaoOrdenacao}, ItemItemPeso.TFP_PESO_FINAL ";

            else if (propriedade == "DescricaoValorPeso")
                return "ItemPeso.TPI_VALOR";

            else if (propriedade == "DescricaoDistancia")
                return $"ItemItemDistancia.TFD_QUILOMETROS {direcaoOrdenacao}, ItemItemDistancia.TFD_QUILOMETRAGEM_INICIAL {direcaoOrdenacao}, ItemItemDistancia.TFD_QUILOMETRAGEM_FINAL";

            else if (propriedade == "DescricaoValorDistancia")
                return "ItemDistancia.TPI_TIPO_VALOR";

            else if (propriedade == "TipoCarga")
                return "ItemItemTipoCarga.TCG_DESCRICAO";

            else if (propriedade == "DescricaoValorTipoCarga")
                return "ItemTipoCarga.TPI_VALOR";

            else if (propriedade == "ModeloReboque")
                return "ItemItemModeloReboque.MVC_DESCRICAO";

            else if (propriedade == "DescricaoValorModeloReboque")
                return "ItemModeloReboque.TPI_VALOR";

            else if (propriedade == "ModeloTracao")
                return "ItemItemModeloTracao.MVC_DESCRICAO";

            else if (propriedade == "DescricaoValorModeloTracao")
                return "ItemModeloTracao.TPI_VALOR";

            else if (propriedade == "ValorTotal")
                return "1";

            else if (propriedade == "TabelaFrete")
                return "TabelaFrete.TBF_DESCRICAO";

            else if (propriedade.Contains("DescricaoValorComponente"))
                return $"{propriedade.Replace("Descricao", "")}.TPI_VALOR";

            else if (propriedade.Contains("DescricaoAntigoValorComponente"))
                return $"{propriedade.Replace("Descricao", "")}.TPI_VALOR_ORIGINAL";

            else if (propriedade == "ParametroBase")
            {
                switch (filtrosPesquisa.ParametroBase)
                {
                    case TipoParametroBaseTabelaFrete.TipoCarga: return "TipoCarga.TCG_DESCRICAO";
                    case TipoParametroBaseTabelaFrete.Peso: return $"Peso.TFP_PESO {direcaoOrdenacao}, Peso.TFP_PESO_INICIAL {direcaoOrdenacao}, Peso.TFP_PESO_FINAL {direcaoOrdenacao}, UnidadeMedidaPeso.UNI_SIGLA";
                    case TipoParametroBaseTabelaFrete.Distancia: return $"Distancia.TFD_QUILOMETROS {direcaoOrdenacao}, Distancia.TFD_QUILOMETRAGEM_INICIAL {direcaoOrdenacao}, Distancia.TFD_QUILOMETRAGEM_FINAL";
                    case TipoParametroBaseTabelaFrete.ModeloTracao: return "ModeloTracao.MVC_DESCRICAO";
                    case TipoParametroBaseTabelaFrete.ModeloReboque: return "ModeloReboque.MVC_DESCRICAO";
                    case TipoParametroBaseTabelaFrete.NumeroEntrega: return $"NumeroEntrega.TFN_NUMERO_INICIAL_ENTREGA {direcaoOrdenacao}, NumeroEntrega.TFN_NUMERO_FINAL_ENTREGA";
                }
            }

            return propriedade;
        }

        private List<string> ObterNomeItensJoins(StringBuilder joins)
        {
            List<string> nomeItens = new List<string>();

            if (joins.Contains(" ItemPeso "))
                nomeItens.Add("ItemPeso");

            if (joins.Contains(" ItemDistancia "))
                nomeItens.Add("ItemDistancia");

            if (joins.Contains(" ItemTipoCarga "))
                nomeItens.Add("ItemTipoCarga");

            if (joins.Contains(" ItemTipoEmbalagem "))
                nomeItens.Add("ItemTipoEmbalagem");

            if (joins.Contains(" ItemTempo "))
                nomeItens.Add("ItemTempo");

            if (joins.Contains(" ItemPallet "))
                nomeItens.Add("ItemPallet");

            if (joins.Contains(" ItemAjudante "))
                nomeItens.Add("ItemAjudante");

            if (joins.Contains(" ItemEntrega "))
                nomeItens.Add("ItemEntrega");

            if (joins.Contains(" ItemModeloReboque "))
                nomeItens.Add("ItemModeloReboque");

            if (joins.Contains(" ItemModeloTracao "))
                nomeItens.Add("ItemModeloTracao");

            return nomeItens;
        }

        private void SetarJoinsCanalEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CanalEntrega "))
                joins.Append("left join T_CANAL_ENTREGA CanalEntrega on CanalEntrega.CNE_CODIGO = TabelaFreteCliente.CNE_CODIGO ");
        }

        private void SetarJoinsCanalVenda(StringBuilder joins)
        {
            if (!joins.Contains(" CanalVenda "))
                joins.Append("left join T_CANAL_VENDA CanalVenda on CanalVenda.CNV_CODIGO = TabelaFreteCliente.CNV_CODIGO ");
        }

        private void SetarJoinsConfiguracaoEmpresa(StringBuilder joins)
        {
            SetarJoinsEmpresa(joins);

            if (!joins.Contains(" ConfiguracaoEmpresa "))
                joins.Append("left join T_CONFIG ConfiguracaoEmpresa on ConfiguracaoEmpresa.COF_CODIGO = Empresa.COF_CODIGO ");
        }

        private void SetarJoinsContratoFreteTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" ContratoTransporteFrete "))
                joins.Append("left join T_CONTRATO_TRANSPORTADOR_FRETE ContratoTransporteFrete on ContratoTransporteFrete.CTF_CODIGO = TabelaFreteCliente.CTF_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = TabelaFreteCliente.EMP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            SetarJoinsTabelaFrete(joins);

            if (!joins.Contains(" GrupoPessoas "))
                joins.Append("left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = TabelaFrete.GRP_CODIGO ");
        }

        private void SetarJoinsTabelaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFrete "))
                joins.Append("left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO ");
        }

        private void SetarJoinsTabelaFreteAjuste(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFreteAjuste "))
                joins.Append("left join T_TABELA_FRETE_AJUSTE TabelaFreteAjuste on TabelaFreteAjuste.TFA_CODIGO = TabelaFreteCliente.TFA_CODIGO ");
        }

        private void SetarJoinsTabelaFreteComponente(StringBuilder joins)
        {
            SetarJoinsTabelaFrete(joins);

            if (!joins.Contains(" TabelaFreteComponente "))
                joins.Append("left join T_TABELA_FRETE_COMPONENTE_FRETE TabelaFreteComponente on TabelaFreteComponente.TBF_CODIGO = TabelaFrete.TBF_CODIGO ");
        }

        private void SetarJoinsTabelaFreteTransportador(StringBuilder joins)
        {
            SetarJoinsTabelaFrete(joins);

            if (!joins.Contains(" TabelaFreteTransportador "))
                joins.Append("left join T_TABELA_FRETE_TRANSPORTADOR TabelaFreteTransportador on TabelaFreteTransportador.TBF_CODIGO = TabelaFrete.TBF_CODIGO ");
        }

        private void SetarJoinsTabelaFreteParametroBaseCalculo(StringBuilder joins)
        {
            if (!joins.Contains(" Parametro "))
                joins.Append("left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro on Parametro.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append("left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = TabelaFreteCliente.CLI_CGCCPF_TOMADOR ");
        }

        private void SetarJoinsUsuarioAlteracao(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioAlteracao "))
                joins.Append(" left join T_FUNCIONARIO UsuarioAlteracao on UsuarioAlteracao.FUN_CODIGO = TabelaFreteCliente.FUN_CODIGO_HISTORICO_ALTERACAO ");
        }

        private void SetarJoinsVigencia(StringBuilder joins)
        {
            if (!joins.Contains(" Vigencia "))
                joins.Append("left join T_TABELA_FRETE_VIGENCIA Vigencia on Vigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO ");
        }

        private void SetarJoinsStatusAssinaturaContrato(StringBuilder joins)
        {
            SetarJoinsContratoFreteTransportador(joins);

            if (!joins.Contains(" StatusAssinaturaContrato "))
                joins.Append("left join T_STATUS_ASSINATURA_CONTRATO StatusAssinaturaContrato on ContratoTransporteFrete.STC_CODIGO = StatusAssinaturaContrato.STC_CODIGO ");
        }

        private void SetarSelectPorItens(StringBuilder select, StringBuilder joins, StringBuilder groupBy)
        {
            List<string> nomeItensJoins = ObterNomeItensJoins(joins);

            if (nomeItensJoins.Count == 0)
                return;

            select.Append($"coalesce({string.Join(", ", nomeItensJoins.Select(nomeItem => $"StatusAprovacao{nomeItem}.STC_DESCRICAO"))}, '') StatusAprovacao, ");
            select.Append($"coalesce({string.Join(", ", nomeItensJoins.Select(nomeItem => $"{nomeItem}.TPI_RETORNO_INTEGRACAO"))}, '') MensagemRetornoIntegracao, ");
            select.Append($"coalesce({string.Join(", ", nomeItensJoins.Select(nomeItem => $"IntegracaoFrete{nomeItem}.IFR_CODIGO_ORIGEM, IntegracaoFrete{nomeItem}.IFR_CODIGO"))}, 0) ItemCodigo, ");
            select.Append($"coalesce({string.Join(", ", nomeItensJoins.Select(nomeItem => $"IntegracaoFrete{nomeItem}.IFR_CODIGO_RETORNO_INTEGRACAO"))}, '') ItemCodigoRetornoIntegracao, ");

            foreach (string nomeItem in nomeItensJoins)
            {
                joins.Append($"left join T_STATUS_ASSINATURA_CONTRATO StatusAprovacao{nomeItem} on StatusAprovacao{nomeItem}.STC_CODIGO = {nomeItem}.STC_CODIGO ");
                joins.Append($"left join T_INTEGRACAO_FRETE IntegracaoFrete{nomeItem} on IntegracaoFrete{nomeItem}.IFR_CODIGO_INTEGRACAO = {nomeItem}.TPI_CODIGO and IntegracaoFrete{nomeItem}.IFR_TIPO = {(int)TipoIntegracaoFrete.TabelaFreteCliente} ");
                groupBy.Append($"StatusAprovacao{nomeItem}.STC_DESCRICAO, {nomeItem}.TPI_RETORNO_INTEGRACAO, IntegracaoFrete{nomeItem}.IFR_CODIGO_ORIGEM, IntegracaoFrete{nomeItem}.IFR_CODIGO, IntegracaoFrete{nomeItem}.IFR_CODIGO_RETORNO_INTEGRACAO, ");
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override SQLDinamico ObterSql(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            _where = new StringBuilder();
            _wherePorTipoRegistro = new StringBuilder();

            if (parametrosConsulta != null)
            {
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, filtrosPesquisa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, filtrosPesquisa);
            }

            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarJoinsTabelaFrete(joins);
            SetarJoinsTabelaFreteParametroBaseCalculo(joins);
            select.Append($"TabelaFrete.TBF_DESCRICAO TabelaFrete, isRelatorio = cast({(filtrosPesquisa.IsRelatorio ? "1" : "0")} as bit), isCSV = cast({(filtrosPesquisa.IsCSV ? "1" : "0")} as bit), TabelaFreteCliente.TFC_TIPO Tipo,");

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

            SetarSelectPorItens(select, joins, groupBy);
            SetarOrderBy(parametrosConsulta, select, orderBy, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (filtrosPesquisa.ExibirHistoricoAlteracao)
            {
                StringBuilder condicoesHistorico = new StringBuilder(_where.ToString());

                if (filtrosPesquisa.DataInicialAlteracao.HasValue)
                    condicoesHistorico.Append($"and TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO >= '{filtrosPesquisa.DataInicialAlteracao.Value.ToString("MM/dd/yyyy")} 00:00:00' ");

                if (filtrosPesquisa.DataFinalAlteracao.HasValue)
                    condicoesHistorico.Append($"and TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO <= '{filtrosPesquisa.DataFinalAlteracao.Value.ToString("MM/dd/yyyy")} 23:59:59' ");

                sql.Append("with TabelaFreteClienteHistorico as ");
                sql.Append("( ");
                sql.Append("     select distinct TabelaFreteCliente.TFC_CODIGO ");
                sql.Append($"      from {_tabela} ");
                sql.Append(joins.ToString());

                if (condicoes.Length > 0)
                    sql.Append($" where {condicoes.Substring(4)} ");

                sql.Append(") ");

                if (somenteContarNumeroRegistros)
                    sql.Append("select distinct(count(0) over ()) ");
                else
                    sql.Append($"select TabelaFreteCliente.TFC_CODIGO Codigo, {campos.Substring(0, campos.Length - 1)} "); 

                sql.Append("   from TabelaFreteClienteHistorico ");
                sql.Append("   join t_tabela_frete_cliente TabelaFreteCliente on ( ");
                sql.Append("            (TabelaFreteCliente.TFC_CODIGO = TabelaFreteClienteHistorico.TFC_CODIGO) or ");
                sql.Append($"           (TabelaFreteCliente.TFC_TABELA_ORIGINARIA = TabelaFreteClienteHistorico.TFC_CODIGO and TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.HistoricoAlteracao}) ");
                sql.Append("        ) ");
                sql.Append(joins.ToString());

                if (condicoesHistorico.Length > 0)
                    sql.Append($"where {condicoesHistorico.ToString().Trim().Substring(4)} ");

                sql.Append($" group by TabelaFreteCliente.TFC_CODIGO, TabelaFrete.TBF_DESCRICAO, TabelaFreteCliente.TFC_TIPO, TabelaFreteCliente.TFC_TABELA_ORIGINARIA{(agrupamentos.Length > 0 ? $", {agrupamentos.Substring(0, agrupamentos.Length - 1)}" : "")} ");
            }
            else
            {
                if (somenteContarNumeroRegistros)
                    sql.Append("select distinct(count(0) over ()) ");
                else
                    sql.Append($"select TabelaFreteCliente.TFC_CODIGO Codigo, {campos.Substring(0, campos.Length - 1)} "); 

                sql.Append($" from {_tabela} ");
                sql.Append(joins.ToString());

                if (condicoes.Length > 0)
                    sql.Append($" where {condicoes.Substring(4)} ");

                sql.Append($"group by TabelaFreteCliente.TFC_CODIGO, TabelaFrete.TBF_DESCRICAO, TabelaFreteCliente.TFC_TIPO{(agrupamentos.Length > 0 ? $", {agrupamentos.Substring(0, agrupamentos.Length - 1)}" : "")} ");
            }

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by {(orderBy.Length > 0 ? orderBy.ToString() : "1 asc")}");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), null);
        }

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao,"))
                    {
                        select.Append("TabelaFreteCliente.TFC_CODIGO CodigoIntegracao, ");
                        groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                    }
                    break;

                case "DataAlteracao":
                    if (!select.Contains(" DataAlteracao,"))
                    {
                        select.Append("case ");
                        select.Append("    when TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO is not null then convert(varchar(10), TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, 103) + ' ' + convert(varchar(5), TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, 108) ");
                        select.Append("    else '' ");
                        select.Append("end DataAlteracao, ");
                        groupBy.Append("TabelaFreteCliente.TBF_DATA_HISTORICO_ALTERACAO, ");
                    }
                    break;

                case "CNPJTransportador":
                case "CNPJTransportadorFormatado":
                case "CodigoIntegracaoTransportador":
                case "DescricaoEmpresa":
                    if (!select.Contains(" Empresa,"))
                    {
                        select.Append("coalesce( ");
                        select.Append("    Empresa.EMP_CNPJ, ");
                        select.Append("    TRIM(STR(Tomador.CLI_CGCCPF, 14, 0)), ");
                        select.Append("    ( ");
                        select.Append("        select top (1) TRIM(STR(_cliente.CLI_CGCCPF, 14, 0)) ");
                        select.Append("          from T_TABELA_FRETE_TRANSPORTADOR_TERCEIRO _tabelaFreteTransportadorTerceiro ");
                        select.Append("          join T_CLIENTE _cliente on _cliente.CLI_CGCCPF = _tabelaFreteTransportadorTerceiro.CLI_CGCCPF ");
                        select.Append("         where _tabelaFreteTransportadorTerceiro.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO ");
                        select.Append("    ) ");
                        select.Append(") CNPJTransportador, ");
                        select.Append("coalesce( ");
                        select.Append("    Empresa.EMP_RAZAO, ");
                        select.Append("    Tomador.CLI_NOME, ");
                        select.Append("    ( ");
                        select.Append("        select top (1) _cliente.CLI_NOME ");
                        select.Append("          from T_TABELA_FRETE_TRANSPORTADOR_TERCEIRO _tabelaFreteTransportadorTerceiro ");
                        select.Append("          join T_CLIENTE _cliente on _cliente.CLI_CGCCPF = _tabelaFreteTransportadorTerceiro.CLI_CGCCPF ");
                        select.Append("         where _tabelaFreteTransportadorTerceiro.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO ");
                        select.Append("    ) ");
                        select.Append(") Empresa, ");
                        select.Append("coalesce( ");
                        select.Append("    Empresa.EMP_CODIGO_INTEGRACAO, ");
                        select.Append("    ( ");
                        select.Append("        select top (1) _cliente.CLI_CODIGO_INTEGRACAO ");
                        select.Append("          from T_TABELA_FRETE_TRANSPORTADOR_TERCEIRO _tabelaFreteTransportadorTerceiro ");
                        select.Append("          join T_CLIENTE _cliente on _cliente.CLI_CGCCPF = _tabelaFreteTransportadorTerceiro.CLI_CGCCPF ");
                        select.Append("         where _tabelaFreteTransportadorTerceiro.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO ");
                        select.Append("    ) ");
                        select.Append(") CodigoIntegracaoTransportador, ");

                        groupBy.Append("Empresa.EMP_CNPJ, Empresa.EMP_RAZAO, Empresa.EMP_CODIGO_INTEGRACAO, ");

                        if (!groupBy.Contains(" Tomador.CLI_CGCCPF,"))
                            groupBy.Append(" Tomador.CLI_CGCCPF, Tomador.CLI_NOME, ");

                        if (!groupBy.Contains(" TabelaFreteCliente.TBF_CODIGO,"))
                            groupBy.Append(" TabelaFreteCliente.TBF_CODIGO, ");

                        SetarJoinsEmpresa(joins);
                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Remetente":
                case "DescricaoRemetente":
                    if (!select.Contains(" Remetente,"))
                        select.Append(@"
                            substring(isnull(
                                (select ' / ' + (case when isnull(clienteOrigem1.CLI_CODIGO_INTEGRACAO, '') <> '' then clienteOrigem1.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + clienteOrigem1.CLI_NOME + ' (' + LTRIM(STR(clienteOrigem1.CLI_CGCCPF, 25, 0)) + ') ' as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 join T_CLIENTE clienteOrigem1 on origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF where TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE = 1 for xml path('')),
                                (select ' / ' + (case when isnull(clienteOrigem1.CLI_CODIGO_INTEGRACAO, '') <> '' then clienteOrigem1.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + clienteOrigem1.CLI_NOME + ' (' + LTRIM(STR(clienteOrigem1.CLI_CGCCPF, 25, 0)) + ') ' as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 join T_CLIENTE clienteOrigem1 on origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF where TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and isnull(clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0 for xml path(''))
                            ), 4, 5000) Remetente, "
                        );

                    break;

                case "NomeFantasiaRemetente":
                    if (!select.Contains(" NomeFantasiaRemetente"))
                        select.Append(@"
                            substring(isnull(
                                (SELECT clienteOrigem1.CLI_NOMEFANTASIA AS [text()] FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 INNER JOIN T_CLIENTE clienteOrigem1 ON origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE = 1 FOR XML path('')),
                                (SELECT clienteOrigem1.CLI_NOMEFANTASIA AS [text()] FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 INNER JOIN T_CLIENTE clienteOrigem1 ON origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and isnull(clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0 FOR XML path(''))
                            ), 4, 5000) NomeFantasiaRemetente, "
                        );
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains(" CPFCNPJRemetente"))
                        select.Append(@"
                            isnull(
                                (SELECT top 1 clienteOrigem1.CLI_CGCCPF FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 INNER JOIN T_CLIENTE clienteOrigem1 ON origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE = 1),
                                (SELECT top 1 clienteOrigem1.CLI_CGCCPF FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM origem1 INNER JOIN T_CLIENTE clienteOrigem1 ON origem1.CLI_CGCCPF = clienteOrigem1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO and isnull(clienteOrigem1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0)
                            ) CPFCNPJRemetente,");
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                        select.Append("substring((select ' / ' + localidadeOrigem1.LOC_DESCRICAO + '-' + localidadeOrigem1.UF_SIGLA as [text()] from T_TABELA_FRETE_CLIENTE_ORIGEM origem1 inner join T_LOCALIDADES localidadeOrigem1 on localidadeOrigem1.LOC_CODIGO = origem1.LOC_CODIGO where TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO for xml path('')), 4, 5000) Origem, ");
                    break;

                case "EstadoOrigem":
                    if (!select.Contains(" EstadoOrigem,"))
                        select.Append("substring((select ' / ' + estadoOrigem1.UF_SIGLA as [text()] from T_TABELA_FRETE_CLIENTE_ESTADO_ORIGEM estadoOrigem1 where TabelaFreteCliente.TFC_CODIGO = estadoOrigem1.TFC_CODIGO for xml path('')), 4, 1000) EstadoOrigem, ");
                    break;

                case "CEPOrigem":
                    if (!select.Contains(" CEPOrigem,"))
                        select.Append("substring((select ' / ' + REPLICATE('0', 8-LEN(cepOrigem1.TCO_CEP_INICIAL)) + CONVERT(nvarchar(10), cepOrigem1.TCO_CEP_INICIAL)  + ' à ' + REPLICATE('0', 8-LEN(cepOrigem1.TCO_CEP_FINAL)) + CONVERT(nvarchar(10), cepOrigem1.TCO_CEP_FINAL) as [text()] from T_TABELA_FRETE_CLIENTE_CEP_ORIGEM cepOrigem1 where TabelaFreteCliente.TFC_CODIGO = cepOrigem1.TFC_CODIGO for xml path('')), 4, 1000) CEPOrigem, ");
                    break;

                case "RegiaoOrigem":
                    if (!select.Contains(" RegiaoOrigem,"))
                        select.Append("substring((select ' / ' + regiaoOrigem1.REG_DESCRICAO as [text()] from T_TABELA_FRETE_CLIENTE_REGIAO_ORIGEM origem1 inner join T_REGIAO regiaoOrigem1 on regiaoOrigem1.REG_CODIGO = origem1.REG_CODIGO where TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO for xml path('')), 4, 1000) RegiaoOrigem, ");
                    break;

                case "PaisOrigem":
                    if (!select.Contains(" PaisOrigem,"))
                        select.Append("substring((select ' / ' + pais.PAI_NOME as [text()] from T_TABELA_FRETE_CLIENTE_PAIS_ORIGEM paisOrigem1 inner join T_PAIS pais on pais.PAI_CODIGO = paisOrigem1.PAI_CODIGO where TabelaFreteCliente.TFC_CODIGO = paisOrigem1.TFC_CODIGO for xml path('')), 4, 1000) PaisOrigem, ");
                    break;

                case "Destinatario":
                case "DescricaoDestinatario":
                    if (!select.Contains(" Destinatario,"))
                        select.Append(@"
                            substring(isnull(
                                (select ' / ' + (case when isnull(clienteDestino1.CLI_CODIGO_INTEGRACAO, '') <> '' then clienteDestino1.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + clienteDestino1.CLI_NOME + ' (' + LTRIM(STR(clienteDestino1.CLI_CGCCPF, 25, 0)) + ') ' as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 inner join T_CLIENTE clienteDestino1 on destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF inner join T_LOCALIDADES localidadeDest on localidadeDest.LOC_CODIGO = clienteDestino1.LOC_CODIGO where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE = 1 for xml path('')),
                                (select ' / ' + (case when isnull(clienteDestino1.CLI_CODIGO_INTEGRACAO, '') <> '' then clienteDestino1.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + clienteDestino1.CLI_NOME + ' (' + LTRIM(STR(clienteDestino1.CLI_CGCCPF, 25, 0)) + ') ' as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 inner join T_CLIENTE clienteDestino1 on destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF inner join T_LOCALIDADES localidadeDest on localidadeDest.LOC_CODIGO = clienteDestino1.LOC_CODIGO where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and isnull(clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0 for xml path(''))
                            ), 4, 5000) Destinatario, "
                        );
                    break;

                case "NomeFantasiaDestinatario":
                    if (!select.Contains(" NomeFantasiaDestinatario,"))
                        select.Append(@"
                            substring(isnull(
                                (select clienteDestino1.CLI_NOMEFANTASIA as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 inner join T_CLIENTE clienteDestino1 on destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE = 1 for xml path('')),
                                (select clienteDestino1.CLI_NOMEFANTASIA as [text()] from T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 inner join T_CLIENTE clienteDestino1 on destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and isnull(clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0 for xml path(''))
                            ), 4, 5000) NomeFantasiaDestinatario, "
                        );
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario"))
                        select.Append(@"
                            isnull(
                                (SELECT top 1 clienteDestino1.CLI_CGCCPF FROM T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 INNER JOIN T_CLIENTE clienteDestino1 ON destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE = 1),
                                (SELECT top 1 clienteDestino1.CLI_CGCCPF FROM T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO destino1 INNER JOIN T_CLIENTE clienteDestino1 ON destino1.CLI_CGCCPF = clienteDestino1.CLI_CGCCPF WHERE TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO and isnull(clienteDestino1.CLI_POSSUI_FILIAL_CLIENTE, 0) = 0)
                            ) CPFCNPJDestinatario, ");
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                        select.Append("substring((select ' / ' + localidadeDestino1.LOC_DESCRICAO + '-' + localidadeDestino1.UF_SIGLA as [text()] from T_TABELA_FRETE_CLIENTE_DESTINO destino1 inner join T_LOCALIDADES localidadeDestino1 on localidadeDestino1.LOC_CODIGO = destino1.LOC_CODIGO where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO for xml path('')), 4, 5000) Destino, ");
                    break;

                case "RegiaoDestino":
                    if (!select.Contains(" RegiaoDestino,"))
                        select.Append("substring((select ' / ' + regiaoDestino1.REG_DESCRICAO as [text()] from T_TABELA_FRETE_CLIENTE_REGIAO_DESTINO destino1 inner join T_REGIAO regiaoDestino1 on regiaoDestino1.REG_CODIGO = destino1.REG_CODIGO where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO for xml path('')), 4, 1000) RegiaoDestino, ");
                    break;

                case "PaisDestino":
                    if (!select.Contains(" PaisDestino,"))
                        select.Append("substring((select ' / ' + pais.PAI_NOME as [text()] from T_TABELA_FRETE_CLIENTE_PAIS_DESTINO paisDestino1 inner join T_PAIS pais on pais.PAI_CODIGO = paisDestino1.PAI_CODIGO where TabelaFreteCliente.TFC_CODIGO = paisDestino1.TFC_CODIGO for xml path('')), 4, 1000) PaisDestino, ");
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete,"))
                        select.Append("substring((select ' / ' + rotaDestino1.ROF_DESCRICAO as [text()] from T_TABELA_FRETE_CLIENTE_ROTA_DESTINO destino1 inner join T_ROTA_FRETE rotaDestino1 on rotaDestino1.ROF_CODIGO = destino1.ROF_CODIGO where TabelaFreteCliente.TFC_CODIGO = destino1.TFC_CODIGO for xml path('')), 4, 1000) RotaFrete, ");
                    break;

                case "RotaFreteOrigem":
                    if (!select.Contains(" RotaFreteOrigem,"))
                        select.Append("substring((select ' / ' + rotaOrigem1.ROF_DESCRICAO as [text()] from T_TABELA_FRETE_CLIENTE_ROTA_ORIGEM origem1 inner join T_ROTA_FRETE rotaOrigem1 on rotaOrigem1.ROF_CODIGO = origem1.ROF_CODIGO where TabelaFreteCliente.TFC_CODIGO = origem1.TFC_CODIGO for xml path('')), 4, 1000) RotaFreteOrigem, ");
                    break;

                case "EstadoDestino":
                    if (!select.Contains(" EstadoDestino,"))
                        select.Append("substring((select ' / ' + estadoDestino1.UF_SIGLA as [text()] from T_TABELA_FRETE_CLIENTE_ESTADO_DESTINO estadoDestino1 where TabelaFreteCliente.TFC_CODIGO = estadoDestino1.TFC_CODIGO for xml path('')), 4, 1000) EstadoDestino, ");
                    break;

                case "CEPDestino":
                    if (!select.Contains(" CEPDestino,"))
                        select.Append("substring((select ' / ' + REPLICATE('0', 8-LEN(cepDestino1.TCD_CEP_INICIAL)) + CONVERT(nvarchar(10), cepDestino1.TCD_CEP_INICIAL)  + ' à ' + REPLICATE('0', 8-LEN(cepDestino1.TCD_CEP_FINAL)) + CONVERT(nvarchar(10), cepDestino1.TCD_CEP_FINAL) as [text()] from T_TABELA_FRETE_CLIENTE_CEP_DESTINO cepDestino1 where TabelaFreteCliente.TFC_CODIGO = cepDestino1.TFC_CODIGO for xml path('')), 4, 1000) CEPDestino, ");
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select.Append("Tomador.CLI_CGCCPF CPFCNPJTomador, Tomador.CLI_NOME Tomador, ");

                        if (!groupBy.Contains(" Tomador.CLI_CGCCPF,"))
                            groupBy.Append(" Tomador.CLI_CGCCPF, Tomador.CLI_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");
                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "DataInicial":
                    if (!select.Contains(" DataInicial,"))
                    {
                        select.Append("CONVERT(VARCHAR(10), Vigencia.TFV_DATA_INICIAL, 103) DataInicial, ");
                        groupBy.Append("Vigencia.TFV_DATA_INICIAL, ");
                        SetarJoinsVigencia(joins);
                    }
                    break;

                case "DataFinal":
                    if (!select.Contains(" DataFinal,"))
                    {
                        select.Append("CONVERT(VARCHAR(10), Vigencia.TFV_DATA_FINAL, 103) DataFinal, ");
                        groupBy.Append("Vigencia.TFV_DATA_FINAL, ");
                        SetarJoinsVigencia(joins);
                    }
                    break;

                case "DescricaoTipoPagamento":
                    if (!select.Contains(" TipoPagamento,"))
                    {
                        select.Append("TabelaFreteCliente.TFC_TIPO_PAGAMENTO TipoPagamento, ");
                        groupBy.Append("TabelaFreteCliente.TFC_TIPO_PAGAMENTO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                        select.Append("substring((select ', ' + tipoOperacao1.TOP_DESCRICAO as [text()] from T_TABELA_FRETE_CLIENTE_TIPO_OPERACAO tipoOperacaoTabela inner join T_TIPO_OPERACAO tipoOperacao1 on tipoOperacao1.TOP_CODIGO = tipoOperacaoTabela.TOP_CODIGO where TabelaFreteCliente.TFC_CODIGO = tipoOperacaoTabela.TFC_CODIGO for xml path('')), 3, 1000) TipoOperacao, ");
                    break;

                case "TransportadorTerceiro":
                    if (!select.Contains(" TransportadorTerceiro, "))
                        select.Append("substring((select distinct ', ' + cliente.CLI_NOME from T_CLIENTE cliente join T_TABELA_FRETE_CLIENTE_TRANSPORTADOR_TERCEIRO transportadorTerceiro on transportadorTerceiro.CLI_CGCCPF = cliente.CLI_CGCCPF where transportadorTerceiro.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO for xml path('')), 3, 1000) TransportadorTerceiro, ");
                    break;

                case "UsuarioAlteracao":
                    if (!select.Contains("UsuarioAlteracao, "))
                    {
                        select.Append("UsuarioAlteracao.FUN_NOME UsuarioAlteracao, ");
                        groupBy.Append("UsuarioAlteracao.FUN_NOME, ");
                        SetarJoinsUsuarioAlteracao(joins);
                    }
                    break;

                case "ParametroBase":
                    if (!select.Contains(" TipoParametroBase,"))
                    {
                        switch (filtroPesquisa.ParametroBase.Value)
                        {
                            case TipoParametroBaseTabelaFrete.Distancia:
                                select.Append("Distancia.TFD_QUILOMETROS ValorParametroBase, Distancia.TFD_QUILOMETRAGEM_INICIAL ValorParametroBase1, Distancia.TFD_QUILOMETRAGEM_FINAL ValorParametroBase2, Distancia.TFD_TIPO TipoValorParametroBase, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ");
                                groupBy.Append("Distancia.TFD_QUILOMETROS, Distancia.TFD_QUILOMETRAGEM_INICIAL, Distancia.TFD_QUILOMETRAGEM_FINAL, Distancia.TFD_TIPO, TabelaFrete.TBF_PARAMETRO_BASE, ");
                                joins.Append("left outer join T_TABELA_FRETE_DISTANCIA Distancia on Parametro.TBC_CODIGO_OBJETO = Distancia.TFD_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.ModeloReboque:
                                select.Append("ModeloReboque.MVC_DESCRICAO DescricaoParametroBase, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ParametrosSaidaModeloVeicularCarga.TMV_QUANTIDADE_ENTREGAS QuantidadeEntregasPorModeloVeicularCarga, ParametrosSaidaModeloVeicularCarga.TMV_PERCENTUAL_ROTA PercentualRotaPorModeloVeicularCarga, cast(ParametrosSaidaModeloVeicularCarga.TMV_CAPACIDADE_OTM as varchar(1)) CapacidadeOTMPorModeloVeicularCarga, ");
                                groupBy.Append("ModeloReboque.MVC_DESCRICAO, TabelaFrete.TBF_PARAMETRO_BASE, ParametrosSaidaModeloVeicularCarga.TMV_QUANTIDADE_ENTREGAS, ParametrosSaidaModeloVeicularCarga.TMV_PERCENTUAL_ROTA, ParametrosSaidaModeloVeicularCarga.TMV_CAPACIDADE_OTM, ");
                                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloReboque ON ModeloReboque.MVC_CODIGO = Parametro.TBC_CODIGO_OBJETO ");
                                joins.Append("left join T_TABELA_FRETE_CLIENTE_MODELO_VEICULAR_CARGA ParametrosSaidaModeloVeicularCarga on ParametrosSaidaModeloVeicularCarga.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and ParametrosSaidaModeloVeicularCarga.MVC_CODIGO = ModeloReboque.MVC_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.ModeloTracao:
                                select.Append("ModeloTracao.MVC_DESCRICAO DescricaoParametroBase, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ParametrosSaidaModeloVeicularCarga.TMV_QUANTIDADE_ENTREGAS QuantidadeEntregasPorModeloVeicularCarga, ParametrosSaidaModeloVeicularCarga.TMV_PERCENTUAL_ROTA PercentualRotaPorModeloVeicularCarga, cast(ParametrosSaidaModeloVeicularCarga.TMV_CAPACIDADE_OTM as varchar(1)) CapacidadeOTMPorModeloVeicularCarga, ");
                                groupBy.Append("ModeloTracao.MVC_DESCRICAO, TabelaFrete.TBF_PARAMETRO_BASE, ParametrosSaidaModeloVeicularCarga.TMV_QUANTIDADE_ENTREGAS, ParametrosSaidaModeloVeicularCarga.TMV_PERCENTUAL_ROTA, ParametrosSaidaModeloVeicularCarga.TMV_CAPACIDADE_OTM, ");
                                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloTracao ON ModeloTracao.MVC_CODIGO = Parametro.TBC_CODIGO_OBJETO ");
                                joins.Append("left join T_TABELA_FRETE_CLIENTE_MODELO_VEICULAR_CARGA ParametrosSaidaModeloVeicularCarga on ParametrosSaidaModeloVeicularCarga.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and ParametrosSaidaModeloVeicularCarga.MVC_CODIGO = ModeloTracao.MVC_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.NumeroEntrega:
                                select.Append("NumeroEntrega.TFN_TIPO TipoValorParametroBase, NumeroEntrega.TFN_NUMERO_INICIAL_ENTREGA ValorParametroBase1, NumeroEntrega.TFN_NUMERO_FINAL_ENTREGA ValorParametroBase2, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ");
                                groupBy.Append("NumeroEntrega.TFN_TIPO, NumeroEntrega.TFN_NUMERO_INICIAL_ENTREGA, NumeroEntrega.TFN_NUMERO_FINAL_ENTREGA, TabelaFrete.TBF_PARAMETRO_BASE, ");
                                joins.Append("left outer join T_TABELA_FRETE_NUMERO_ENTREGA NumeroEntrega on Parametro.TBC_CODIGO_OBJETO = NumeroEntrega.TFN_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.Pallets:
                                select.Append("Pallets.TFP_TIPO TipoValorParametroBase, Pallets.TFP_NUMERO_INICIAL_PALLET ValorParametroBase1, Pallets.TFP_NUMERO_FINAL_PALLET ValorParametroBase2, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ");
                                groupBy.Append("Pallets.TFP_TIPO, Pallets.TFP_NUMERO_INICIAL_PALLET, Pallets.TFP_NUMERO_FINAL_PALLET, TabelaFrete.TBF_PARAMETRO_BASE, ");
                                joins.Append("left outer join T_TABELA_FRETE_PALLETS Pallets on Parametro.TBC_CODIGO_OBJETO = Pallets.TFP_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.Peso:
                                select.Append("Peso.TFP_PESO ValorParametroBase, Peso.TFP_PESO_INICIAL ValorParametroBase1, Peso.TFP_PESO_FINAL ValorParametroBase2, Peso.TFP_TIPO TipoValorParametroBase, UnidadeMedidaPeso.UNI_SIGLA DescricaoParametroBase, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ");
                                groupBy.Append("Peso.TFP_PESO, Peso.TFP_PESO_INICIAL, Peso.TFP_PESO_FINAL, Peso.TFP_TIPO, UnidadeMedidaPeso.UNI_SIGLA, TabelaFrete.TBF_PARAMETRO_BASE, ");
                                joins.Append("left outer join T_TABELA_FRETE_PESO Peso on Peso.TFP_CODIGO = Parametro.TBC_CODIGO_OBJETO left outer join T_UNIDADE_MEDIDA UnidadeMedidaPeso on UnidadeMedidaPeso.UNI_CODIGO = Peso.UNI_CODIGO ");
                                break;

                            case TipoParametroBaseTabelaFrete.TipoCarga:
                                select.Append("TipoCarga.TCG_DESCRICAO DescricaoParametroBase, TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase, ");
                                groupBy.Append("TipoCarga.TCG_DESCRICAO, TabelaFrete.TBF_PARAMETRO_BASE, ");
                                joins.Append("left outer join T_TABELA_FRETE_TIPO_CARGA PTipoCarga on Parametro.TBC_CODIGO_OBJETO = PTipoCarga.TCG_CODIGO left outer join T_TIPO_DE_CARGA TipoCarga on PTipoCarga.TCG_CODIGO = TipoCarga.TCG_CODIGO ");
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case "DescricaoValorTempo":
                case "DescricaoAntigoValorTempo":
                case "HoraTempo":
                case "RetornoIntegracaoTempo":
                    if (!select.Contains(" HoraInicialTempo,"))
                    {
                        select.Append(
                            @"ItemTempo.TPI_CODIGO CodigoItemTempo, 
                            convert(varchar(5), ItemItemTempo.TFT_HORA_INICIAL, 108) HoraInicialTempo, 
                            convert(varchar(5), ItemItemTempo.TFT_HORA_FINAL, 108) HoraFinalTempo, 
                            ItemTempo.TPI_VALOR ValorTempo, 
                            ItemTempo.TPI_VALOR_ORIGINAL AntigoValorTempo, 
                            ItemTempo.TPI_TIPO_VALOR TipoValorTempo, "
                        );

                        groupBy.Append("ItemTempo.TPI_CODIGO, ItemItemTempo.TFT_HORA_INICIAL, ItemTempo.TPI_VALOR_ORIGINAL, ItemItemTempo.TFT_HORA_FINAL, ItemTempo.TPI_VALOR, ItemTempo.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTempo on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                            joins.Append("Parametro.TBC_CODIGO = ItemTempo.TBC_CODIGO ");
                        else
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemTempo.TFC_CODIGO ");

                        joins.Append("and ItemTempo.TPI_TIPO_OBJETO = 11 left outer join T_TABELA_FRETE_TEMPO ItemItemTempo on ItemItemTempo.TFT_CODIGO = ItemTempo.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                                _wherePorTipoRegistro.Append("or ItemTempo.TPI_VALOR <> ItemTempo.TPI_VALOR_ORIGINAL ");
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                                _wherePorTipoRegistro.Append("and ItemTempo.TPI_VALOR = ItemTempo.TPI_VALOR_ORIGINAL ");
                        }
                    }
                    break;

                case "DescricaoValorPallets":
                case "DescricaoAntigoValorPallets":
                case "DescricaoValorPalletsExcedente":
                case "DescricaoAntigoValorPalletsExcedente":
                case "NumeroPallets":
                case "RetornoIntegracaoPallets":
                    if (!select.Contains(" NumeroInicialPallets,"))
                    {
                        select.Append(
                            @"ItemPallet.TPI_CODIGO CodigoItemNumeroPallets, 
                            ItemItemPallet.TFP_NUMERO_INICIAL_PALLET NumeroInicialPallets, 
                            ItemItemPallet.TFP_NUMERO_FINAL_PALLET NumeroFinalPallets, 
                            ItemItemPallet.TFP_TIPO TipoPallets, 
                            ItemPallet.TPI_VALOR ValorPallets, 
                            ItemPallet.TPI_VALOR_ORIGINAL AntigoValorPallets, 
                            ItemPallet.TPI_TIPO_VALOR TipoValorPallets, "
                        );

                        groupBy.Append("ItemPallet.TPI_CODIGO, ItemItemPallet.TFP_NUMERO_INICIAL_PALLET, ItemPallet.TPI_VALOR_ORIGINAL, ItemItemPallet.TFP_NUMERO_FINAL_PALLET, ItemItemPallet.TFP_TIPO, ItemPallet.TPI_VALOR, ItemPallet.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemPallet on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append(
                                @"Parametro.TBC_CODIGO CodigoItemNumeroPalletsExcedente,
                                Parametro.TBC_VALOR_PALLET_EXCEDENTE ValorPalletsExcedente,
                                Parametro.TBC_VALOR_PALLET_EXCEDENTE_ORIGINAL AntigoValorPalletsExcedente, "
                            );

                            groupBy.Append("Parametro.TBC_VALOR_PALLET_EXCEDENTE, Parametro.TBC_VALOR_PALLET_EXCEDENTE_ORIGINAL, ");
                            joins.Append("Parametro.TBC_CODIGO = ItemPallet.TBC_CODIGO ");

                            if (!groupBy.Contains("Parametro.TBC_CODIGO,"))
                                groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"TabelaFreteCliente.TFC_CODIGO CodigoItemNumeroPalletsExcedente,
                                TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE ValorPalletsExcedente,
                                TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE_ORIGINAL AntigoValorPalletsExcedente, "
                            );

                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE, TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE_ORIGINAL, ");
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemPallet.TFC_CODIGO ");
                        }

                        joins.Append("and ItemPallet.TPI_TIPO_OBJETO = 10 left outer join T_TABELA_FRETE_PALLETS ItemItemPallet on ItemItemPallet.TFP_CODIGO = ItemPallet.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                _wherePorTipoRegistro.Append(@"or ItemPallet.TPI_VALOR <> ItemPallet.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_PALLET_EXCEDENTE <> Parametro.TBC_VALOR_PALLET_EXCEDENTE_ORIGINAL ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE <> TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                _wherePorTipoRegistro.Append("and (ItemPallet.TPI_VALOR = ItemPallet.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_PALLET_EXCEDENTE = Parametro.TBC_VALOR_PALLET_EXCEDENTE_ORIGINAL) ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE = TabelaFreteCliente.TFC_VALOR_PALLET_EXCEDENTE_ORIGINAL) ");
                            }
                        }
                    }
                    break;

                case "DescricaoValorAjudante":
                case "DescricaoAntigoValorAjudante":
                case "DescricaoValorAjudanteExcedente":
                case "DescricaoAntigoValorAjudanteExcedente":
                case "NumeroAjudante":
                case "RetornoIntegracaoAjudante":
                    if (!select.Contains(" NumeroInicialAjudante,"))
                    {
                        select.Append(
                            @"ItemAjudante.TPI_CODIGO CodigoItemNumeroAjudante,
                            ItemItemAjudante.TFA_NUMERO_INICIAL NumeroInicialAjudante,
                            ItemItemAjudante.TFA_NUMERO_FINAL NumeroFinalAjudante,
                            ItemItemAjudante.TFA_TIPO TipoAjudante,
                            ItemAjudante.TPI_VALOR ValorAjudante,
                            ItemAjudante.TPI_VALOR_ORIGINAL AntigoValorAjudante,
                            ItemAjudante.TPI_TIPO_VALOR TipoValorAjudante, "
                        );

                        groupBy.Append("ItemAjudante.TPI_CODIGO, ItemItemAjudante.TFA_NUMERO_INICIAL, ItemAjudante.TPI_VALOR_ORIGINAL, ItemItemAjudante.TFA_NUMERO_FINAL, ItemItemAjudante.TFA_TIPO, ItemAjudante.TPI_VALOR, ItemAjudante.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemAjudante on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append(
                                @"Parametro.TBC_CODIGO CodigoItemNumeroAjudanteExcedente,
                                Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE ValorAjudanteExcedente,
                                Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL AntigoValorAjudanteExcedente, "
                            );

                            groupBy.Append("Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE, Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL, ");
                            joins.Append("Parametro.TBC_CODIGO = ItemAjudante.TBC_CODIGO ");

                            if (!groupBy.Contains("Parametro.TBC_CODIGO,"))
                                groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"TabelaFreteCliente.TFC_CODIGO CodigoItemNumeroAjudanteExcedente,
                                TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE ValorAjudanteExcedente,
                                TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL AntigoValorAjudanteExcedente, "
                            );

                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE, TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL, ");
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemAjudante.TFC_CODIGO ");
                        }

                        joins.Append("and ItemAjudante.TPI_TIPO_OBJETO = 12 left outer join T_TABELA_FRETE_AJUDANTE ItemItemAjudante on ItemItemAjudante.TFA_CODIGO = ItemAjudante.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                _wherePorTipoRegistro.Append(@"or ItemAjudante.TPI_VALOR <> ItemAjudante.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE <> Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE <> TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                _wherePorTipoRegistro.Append("and (ItemAjudante.TPI_VALOR = ItemAjudante.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE = Parametro.TBC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL) ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE = TabelaFreteCliente.TFC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL) ");
                            }
                        }
                    }
                    break;

                case "DescricaoValorEntrega":
                case "DescricaoAntigoValorEntrega":
                case "DescricaoValorEntregaExcedente":
                case "DescricaoAntigoValorEntregaExcedente":
                case "NumeroEntrega":
                case "RetornoIntegracaoEntrega":
                    if (!select.Contains(" NumeroInicialEntrega,"))
                    {
                        select.Append(
                            @"ItemEntrega.TPI_CODIGO CodigoItemNumeroEntrega,
                            ItemItemEntrega.TFN_NUMERO_INICIAL_ENTREGA NumeroInicialEntrega,
                            ItemItemEntrega.TFN_NUMERO_FINAL_ENTREGA NumeroFinalEntrega,
                            ItemItemEntrega.TFN_TIPO TipoEntrega,
                            ItemEntrega.TPI_VALOR ValorEntrega,
                            ItemEntrega.TPI_VALOR_ORIGINAL AntigoValorEntrega,
                            ItemEntrega.TPI_TIPO_VALOR TipoValorEntrega, "
                        );

                        groupBy.Append("ItemEntrega.TPI_CODIGO, ItemItemEntrega.TFN_NUMERO_INICIAL_ENTREGA, ItemEntrega.TPI_VALOR_ORIGINAL, ItemItemEntrega.TFN_NUMERO_FINAL_ENTREGA, ItemItemEntrega.TFN_TIPO, ItemEntrega.TPI_VALOR, ItemEntrega.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemEntrega on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append(
                                @"Parametro.TBC_CODIGO CodigoItemNumeroEntregaExcedente,
                                Parametro.TBC_VALOR_ENTREGA_EXCEDENTE ValorEntregaExcedente,
                                Parametro.TBC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL AntigoValorEntregaExcedente, "
                            );

                            groupBy.Append("Parametro.TBC_VALOR_ENTREGA_EXCEDENTE, Parametro.TBC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL, ");
                            joins.Append("Parametro.TBC_CODIGO = ItemEntrega.TBC_CODIGO ");

                            if (!groupBy.Contains("Parametro.TBC_CODIGO,"))
                                groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"TabelaFreteCliente.TFC_CODIGO CodigoItemNumeroEntregaExcedente,
                                TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE ValorEntregaExcedente,
                                TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL AntigoValorEntregaExcedente, "
                            );

                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE, TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL, ");
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemEntrega.TFC_CODIGO ");
                        }

                        joins.Append("and ItemEntrega.TPI_TIPO_OBJETO = 5 left outer join T_TABELA_FRETE_NUMERO_ENTREGA ItemItemEntrega on ItemItemEntrega.TFN_CODIGO = ItemEntrega.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                _wherePorTipoRegistro.Append(@"or ItemEntrega.TPI_VALOR <> ItemEntrega.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_ENTREGA_EXCEDENTE <> Parametro.TBC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE <> TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                _wherePorTipoRegistro.Append("and (ItemEntrega.TPI_VALOR = ItemEntrega.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_ENTREGA_EXCEDENTE = Parametro.TBC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL) ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE = TabelaFreteCliente.TFC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL) ");
                            }
                        }
                    }
                    break;

                case "DescricaoValorTipoEmbalagem":
                case "DescricaoAntigoValorTipoEmbalagem":
                case "TipoEmbalagem":
                case "RetornoIntegracaoTipoEmbalagem":
                    if (!select.Contains(" ValorTipoEmbalagem,"))
                    {
                        select.Append(
                            @"ItemTipoEmbalagem.TPI_CODIGO CodigoItemTipoEmbalagem,
                            ItemItemTipoEmbalagem.MRC_DESCRICAO TipoEmbalagem,
                            ItemTipoEmbalagem.TPI_VALOR ValorTipoEmbalagem,
                            ItemTipoEmbalagem.TPI_VALOR_ORIGINAL AntigoValorTipoEmbalagem,
                            ItemTipoEmbalagem.TPI_TIPO_VALOR TipoValorTipoEmbalagem, "
                        );

                        groupBy.Append("ItemTipoEmbalagem.TPI_CODIGO, ItemItemTipoEmbalagem.MRC_DESCRICAO, ItemTipoEmbalagem.TPI_VALOR_ORIGINAL, ItemTipoEmbalagem.TPI_VALOR, ItemTipoEmbalagem.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoEmbalagem on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                            joins.Append("Parametro.TBC_CODIGO = ItemTipoEmbalagem.TBC_CODIGO ");
                        else
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemTipoEmbalagem.TFC_CODIGO ");

                        joins.Append("and ItemTipoEmbalagem.TPI_TIPO_OBJETO = 17 left outer join T_TIPO_EMBALAGEM ItemItemTipoEmbalagem on ItemItemTipoEmbalagem.MRC_CODIGO = ItemTipoEmbalagem.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                                _wherePorTipoRegistro.Append("or ItemTipoEmbalagem.TPI_VALOR <> ItemTipoEmbalagem.TPI_VALOR_ORIGINAL ");
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                                _wherePorTipoRegistro.Append("and ItemTipoEmbalagem.TPI_VALOR = ItemTipoEmbalagem.TPI_VALOR_ORIGINAL ");
                        }
                    }
                    break;

                case "DescricaoValorPeso":
                case "DescricaoAntigoValorPeso":
                case "DescricaoValorPesoExcedente":
                case "DescricaoAntigoValorPesoExcedente":
                case "DescricaoPeso":
                case "RetornoIntegracaoPeso":
                    if (!select.Contains(" PesoInicial,"))
                    {
                        select.Append(
                            @"ItemPeso.TPI_CODIGO CodigoItemPeso, 
                            ItemItemPeso.TFP_PESO Peso, 
                            ItemItemPeso.TFP_PESO_INICIAL PesoInicial, 
                            ItemItemPeso.TFP_PESO_FINAL PesoFinal, 
                            ItemItemPeso.TFP_TIPO TipoPeso, 
                            UnidadeMedidaItemItemPeso.UNI_SIGLA UnidadeMedida, 
                            ItemPeso.TPI_VALOR ValorPeso, 
                            ItemPeso.TPI_VALOR_ORIGINAL AntigoValorPeso, 
                            ItemPeso.TPI_TIPO_VALOR TipoValorPeso, "
                        );

                        groupBy.Append("ItemPeso.TPI_CODIGO, ItemItemPeso.TFP_PESO, ItemPeso.TPI_VALOR_ORIGINAL, ItemItemPeso.TFP_PESO_INICIAL, ItemItemPeso.TFP_PESO_FINAL, ItemItemPeso.TFP_TIPO, UnidadeMedidaItemItemPeso.UNI_SIGLA, ItemPeso.TPI_VALOR, ItemPeso.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemPeso on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append(
                                @"Parametro.TBC_CODIGO CodigoItemPesoExcedente,
                                Parametro.TBC_VALOR_PESO_EXCEDENTE ValorPesoExcedente,
                                Parametro.TBC_VALOR_PESO_EXCEDENTE_ORIGINAL AntigoValorPesoExcedente, "
                            );

                            groupBy.Append("Parametro.TBC_VALOR_PESO_EXCEDENTE, Parametro.TBC_VALOR_PESO_EXCEDENTE_ORIGINAL, ");
                            joins.Append("Parametro.TBC_CODIGO = ItemPeso.TBC_CODIGO ");

                            if (!groupBy.Contains("Parametro.TBC_CODIGO,"))
                                groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"TabelaFreteCliente.TFC_CODIGO CodigoItemPesoExcedente,
                                TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE ValorPesoExcedente,
                                TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE_ORIGINAL AntigoValorPesoExcedente, "
                            );

                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE, TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE_ORIGINAL, ");
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemPeso.TFC_CODIGO ");
                        }

                        joins.Append(
                            @"and ItemPeso.TPI_TIPO_OBJETO = 6 
                            left outer join T_TABELA_FRETE_PESO ItemItemPeso on ItemItemPeso.TFP_CODIGO = ItemPeso.TPI_CODIGO_OBJETO
                            left outer join T_UNIDADE_MEDIDA UnidadeMedidaItemItemPeso on ItemItemPeso.UNI_CODIGO = UnidadeMedidaItemItemPeso.UNI_CODIGO "
                        );

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                _wherePorTipoRegistro.Append(@"or ItemPeso.TPI_VALOR <> ItemPeso.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_PESO_EXCEDENTE <> Parametro.TBC_VALOR_PESO_EXCEDENTE_ORIGINAL ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE <> TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                _wherePorTipoRegistro.Append("and (ItemPeso.TPI_VALOR = ItemPeso.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_PESO_EXCEDENTE = Parametro.TBC_VALOR_PESO_EXCEDENTE_ORIGINAL) ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE = TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE_ORIGINAL) ");
                            }
                        }
                    }
                    break;

                case "DescricaoValorDistancia":
                case "DescricaoAntigoValorDistancia":
                case "DescricaoValorDistanciaExcedente":
                case "DescricaoAntigoValorDistanciaExcedente":
                case "DescricaoDistancia":
                case "RetornoIntegracaoDistancia":
                    if (!select.Contains(" DistanciaInicial,"))
                    {
                        select.Append(
                            @"ItemDistancia.TPI_CODIGO CodigoItemDistancia,
                            ItemItemDistancia.TFD_QUILOMETROS Distancia,
                            ItemItemDistancia.TFD_QUILOMETRAGEM_INICIAL DistanciaInicial,
                            ItemItemDistancia.TFD_QUILOMETRAGEM_FINAL DistanciaFinal,
                            ItemItemDistancia.TFD_TIPO TipoDistancia,
                            ItemDistancia.TPI_VALOR ValorDistancia,
                            ItemDistancia.TPI_VALOR_ORIGINAL AntigoValorDistancia,
                            ItemDistancia.TPI_TIPO_VALOR TipoValorDistancia, "
                        );

                        groupBy.Append("ItemDistancia.TPI_CODIGO, ItemItemDistancia.TFD_QUILOMETROS, ItemDistancia.TPI_VALOR_ORIGINAL, ItemItemDistancia.TFD_QUILOMETRAGEM_INICIAL, ItemItemDistancia.TFD_QUILOMETRAGEM_FINAL, ItemItemDistancia.TFD_TIPO, ItemDistancia.TPI_VALOR, ItemDistancia.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemDistancia on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append(
                                @"Parametro.TBC_CODIGO CodigoItemDistanciaExcedente,
                                Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE ValorDistanciaExcedente,
                                Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL AntigoValorDistanciaExcedente, "
                            );

                            groupBy.Append("Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE, Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL, ");
                            joins.Append("Parametro.TBC_CODIGO = ItemDistancia.TBC_CODIGO ");

                            if (!groupBy.Contains("Parametro.TBC_CODIGO,"))
                                groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"TabelaFreteCliente.TFC_CODIGO CodigoItemDistanciaExcedente,
                                TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE ValorDistanciaExcedente,
                                TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL AntigoValorDistanciaExcedente, "
                            );

                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE, TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL, ");
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemDistancia.TFC_CODIGO ");
                        }

                        joins.Append("and ItemDistancia.TPI_TIPO_OBJETO = 7 left outer join T_TABELA_FRETE_DISTANCIA ItemItemDistancia on ItemItemDistancia.TFD_CODIGO = ItemDistancia.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                _wherePorTipoRegistro.Append(@"or ItemDistancia.TPI_VALOR <> ItemDistancia.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE <> Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE <> TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                _wherePorTipoRegistro.Append("and (ItemDistancia.TPI_VALOR = ItemDistancia.TPI_VALOR_ORIGINAL ");

                                if (filtroPesquisa.ParametroBase.HasValue)
                                    _wherePorTipoRegistro.Append(@"or Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE = Parametro.TBC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL) ");
                                else
                                    _wherePorTipoRegistro.Append(@"or TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE = TabelaFreteCliente.TFC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL) ");
                            }
                        }
                    }
                    break;

                case "DescricaoValorTipoCarga":
                case "DescricaoAntigoValorTipoCarga":
                case "TipoCarga":
                case "RetornoIntegracaoTipoCarga":
                    if (!select.Contains(" ValorTipoCarga,"))
                    {
                        select.Append(
                            @"ItemTipoCarga.TPI_CODIGO CodigoItemTipoCarga,
                            ItemItemTipoCarga.TCG_DESCRICAO TipoCarga,
                            ItemTipoCarga.TPI_VALOR ValorTipoCarga,
                            ItemTipoCarga.TPI_VALOR_ORIGINAL AntigoValorTipoCarga,
                            ItemTipoCarga.TPI_TIPO_VALOR TipoValorTipoCarga, "
                        );

                        groupBy.Append("ItemTipoCarga.TPI_CODIGO, ItemItemTipoCarga.TCG_DESCRICAO, ItemTipoCarga.TPI_VALOR_ORIGINAL, ItemTipoCarga.TPI_VALOR, ItemTipoCarga.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                            joins.Append("Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO ");
                        else
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemTipoCarga.TFC_CODIGO ");

                        joins.Append("and ItemTipoCarga.TPI_TIPO_OBJETO = 1 ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                            {
                                joins.Append("and ItemTipoCarga.TPI_VALOR <> ItemTipoCarga.TPI_VALOR_ORIGINAL ");
                                _wherePorTipoRegistro.Append("or ItemTipoCarga.TPI_VALOR <> ItemTipoCarga.TPI_VALOR_ORIGINAL ");
                            }
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                            {
                                joins.Append("and ItemTipoCarga.TPI_VALOR = ItemTipoCarga.TPI_VALOR_ORIGINAL ");
                                _wherePorTipoRegistro.Append("and ItemTipoCarga.TPI_VALOR = ItemTipoCarga.TPI_VALOR_ORIGINAL ");
                            }
                        }

                        joins.Append("left outer join T_TIPO_DE_CARGA ItemItemTipoCarga on ItemItemTipoCarga.TCG_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO ");
                    }
                    break;

                case "DescricaoValorModeloReboque":
                case "DescricaoAntigoValorModeloReboque":
                case "ModeloReboque":
                case "RetornoIntegracaoModeloReboque":
                    if (!select.Contains(" ValorModeloReboque,"))
                    {
                        select.Append(
                            @"ItemModeloReboque.TPI_CODIGO CodigoItemModeloReboque,
                            ItemItemModeloReboque.MVC_DESCRICAO ModeloReboque,
                            ItemModeloReboque.TPI_VALOR ValorModeloReboque,
                            ItemModeloReboque.TPI_VALOR_ORIGINAL AntigoValorModeloReboque,
                            ItemModeloReboque.TPI_TIPO_VALOR TipoValorModeloReboque, "
                        );

                        groupBy.Append("ItemModeloReboque.TPI_CODIGO, ItemItemModeloReboque.MVC_DESCRICAO, ItemModeloReboque.TPI_VALOR_ORIGINAL, ItemModeloReboque.TPI_VALOR, ItemModeloReboque.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemModeloReboque on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                            joins.Append("Parametro.TBC_CODIGO = ItemModeloReboque.TBC_CODIGO ");
                        else
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemModeloReboque.TFC_CODIGO ");

                        joins.Append("and ItemModeloReboque.TPI_TIPO_OBJETO = 2 left outer join T_MODELO_VEICULAR_CARGA ItemItemModeloReboque on ItemItemModeloReboque.MVC_CODIGO = ItemModeloReboque.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                                _wherePorTipoRegistro.Append("or ItemModeloReboque.TPI_VALOR <> ItemModeloReboque.TPI_VALOR_ORIGINAL ");
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                                _wherePorTipoRegistro.Append("and ItemModeloReboque.TPI_VALOR = ItemModeloReboque.TPI_VALOR_ORIGINAL ");
                        }
                    }
                    break;

                case "DescricaoValorModeloTracao":
                case "DescricaoAntigoValorModeloTracao":
                case "ModeloTracao":
                case "RetornoIntegracaoModeloTracao":
                    if (!select.Contains(" ValorModeloTracao,"))
                    {
                        select.Append(
                            @"ItemModeloTracao.TPI_CODIGO CodigoItemModeloTracao,
                            ItemItemModeloTracao.MVC_DESCRICAO ModeloTracao,
                            ItemModeloTracao.TPI_VALOR ValorModeloTracao,
                            ItemModeloTracao.TPI_VALOR_ORIGINAL AntigoValorModeloTracao,
                            ItemModeloTracao.TPI_TIPO_VALOR TipoValorModeloTracao, "
                        );

                        groupBy.Append("ItemModeloTracao.TPI_CODIGO, ItemItemModeloTracao.MVC_DESCRICAO, ItemModeloTracao.TPI_VALOR_ORIGINAL, ItemModeloTracao.TPI_VALOR, ItemModeloTracao.TPI_TIPO_VALOR, ");
                        joins.Append(@"left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemModeloTracao on ");

                        if (filtroPesquisa.ParametroBase.HasValue)
                            joins.Append("Parametro.TBC_CODIGO = ItemModeloTracao.TBC_CODIGO ");
                        else
                            joins.Append("TabelaFreteCliente.TFC_Codigo = ItemModeloTracao.TFC_CODIGO ");

                        joins.Append("and ItemModeloTracao.TPI_TIPO_OBJETO = 3 left outer join T_MODELO_VEICULAR_CARGA ItemItemModeloTracao on ItemItemModeloTracao.MVC_CODIGO = ItemModeloTracao.TPI_CODIGO_OBJETO ");

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                                _wherePorTipoRegistro.Append("or ItemModeloTracao.TPI_VALOR <> ItemModeloTracao.TPI_VALOR_ORIGINAL ");
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                                _wherePorTipoRegistro.Append("and ItemModeloTracao.TPI_VALOR = ItemModeloTracao.TPI_VALOR_ORIGINAL ");
                        }
                    }
                    break;

                case "ValorMinimo":
                    if (!select.Contains(" ValorMinimo,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_MINIMO_GARANTIDO ValorMinimo, ");
                            groupBy.Append("Parametro.TBC_VALOR_MINIMO_GARANTIDO, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_MINIMO_GARANTIDO ValorMinimo, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_MINIMO_GARANTIDO, ");
                        }
                    }
                    break;

                case "AntigoValorMinimo":
                    if (!select.Contains(" AntigoValorMinimo,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_MINIMO_GARANTIDO_ORIGINAL AntigoValorMinimo, ");
                            groupBy.Append("Parametro.TBC_VALOR_MINIMO_GARANTIDO_ORIGINAL, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_MINIMO_GARANTIDO_ORIGINAL AntigoValorMinimo, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_MINIMO_GARANTIDO_ORIGINAL, ");
                        }
                    }
                    break;

                case "ValorMaximo":
                    if (!select.Contains(" ValorMaximo,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_MAXIMO ValorMaximo, ");
                            groupBy.Append("Parametro.TBC_VALOR_MAXIMO, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_MAXIMO ValorMaximo, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_MAXIMO, ");
                        }
                    }
                    break;

                case "AntigoValorMaximo":
                    if (!select.Contains(" AntigoValorMaximo,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_MAXIMO_ORIGINAL AntigoValorMaximo, ");
                            groupBy.Append("Parametro.TBC_VALOR_MAXIMO_ORIGINAL, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_MAXIMO_ORIGINAL AntigoValorMaximo, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_MAXIMO_ORIGINAL, ");
                        }
                    }
                    break;

                case "ValorBase":
                    if (!select.Contains(" ValorBase,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_BASE ValorBase, ");
                            groupBy.Append("Parametro.TBC_VALOR_BASE, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_BASE ValorBase, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_BASE, ");
                        }
                    }
                    break;

                case "CodigoValorBase":
                    if (!select.Contains(" CodigoValorBase,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_CODIGO CodigoValorBase, ");
                            groupBy.Append("Parametro.TBC_CODIGO, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_CODIGO CodigoValorBase, ");
                            groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                        }
                    }
                    break;

                case "DescricaoValorBase":
                    SetarSelect("CodigoValorBase", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorBase", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "AntigoValorBase":
                    if (!select.Contains(" AntigoValorBase,"))
                    {
                        if (filtroPesquisa.ParametroBase.HasValue)
                        {
                            select.Append("Parametro.TBC_VALOR_BASE_ORIGINAL AntigoValorBase, ");
                            groupBy.Append("Parametro.TBC_VALOR_BASE_ORIGINAL, ");
                        }
                        else
                        {
                            select.Append("TabelaFreteCliente.TFC_VALOR_BASE_ORIGINAL AntigoValorBase, ");
                            groupBy.Append("TabelaFreteCliente.TFC_VALOR_BASE_ORIGINAL, ");
                        }
                    }
                    break;

                case "LeadTime":
                    if (!select.Contains(" LeadTime, "))
                    {
                        select.Append("TabelaFreteCliente.TFC_LEAD_TIME LeadTime, ");
                        groupBy.Append("TabelaFreteCliente.TFC_LEAD_TIME, ");
                    }
                    break;

                case "PrazoDiasUteis":
                    if (!select.Contains(" PrazoDiasUteis, "))
                    {
                        select.Append(@"(select CEPsDestino.TCD_DIAS_UTEIS from T_TABELA_FRETE_CLIENTE_CEP_DESTINO CEPsDestino
                                            where CEPsDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO) PrazoDiasUteis, ");
                        groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                    }
                    break;

                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega, "))
                    {
                        select.Append(@"CanalEntrega.CNE_DESCRICAO CanalEntrega, ");
                        groupBy.Append("CanalEntrega.CNE_DESCRICAO, ");

                        SetarJoinsCanalEntrega(joins);
                    }
                    break;

                case "PercentualRotaFormatado":
                    if (!select.Contains(" PercentualRotaPorTabelaFreteCliente, "))
                    {
                        select.Append("TabelaFreteCliente.TFC_PERCENTUAL_ROTA PercentualRotaPorTabelaFreteCliente, ");
                        groupBy.Append("TabelaFreteCliente.TFC_PERCENTUAL_ROTA, ");

                        SetarSelect("ParametroBase", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;

                case "QuantidadeEntregas":
                    if (!select.Contains(" QuantidadeEntregasPorTabelaFreteCliente, "))
                    {
                        select.Append("TabelaFreteCliente.TFC_QUANTIDADE_ENTREGAS QuantidadeEntregasPorTabelaFreteCliente, ");
                        groupBy.Append("TabelaFreteCliente.TFC_QUANTIDADE_ENTREGAS, ");

                        SetarSelect("ParametroBase", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;

                case "DescricaoCapacidadeOTM":
                    if (!select.Contains(" CapacidadeOTMPorTabelaFreteCliente, "))
                    {
                        select.Append(@"cast(TabelaFreteCliente.TFC_CAPACIDADE_OTM as varchar(1)) CapacidadeOTMPorTabelaFreteCliente, ");
                        groupBy.Append("TabelaFreteCliente.TFC_CAPACIDADE_OTM, ");

                        SetarSelect("ParametroBase", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;

                case "PontoPlanejamentoTransporteDescricao":
                    if (!select.Contains(" PontoPlanejamentoTransporte, "))
                    {
                        select.Append("TabelaFreteCliente.TFC_PONTO_PLANEJAMENTO_TRANSPORTE PontoPlanejamentoTransporte, ");
                        groupBy.Append("TabelaFreteCliente.TFC_PONTO_PLANEJAMENTO_TRANSPORTE, ");
                    }
                    break;

                case "PisTransportadorFormatado":
                    if (!select.Contains(" PisTransportador, "))
                    {
                        select.Append("ConfiguracaoEmpresa.COF_ALIQUOTA_PIS PisTransportador, ");
                        groupBy.Append("ConfiguracaoEmpresa.COF_ALIQUOTA_PIS, ");

                        SetarJoinsConfiguracaoEmpresa(joins);
                    }
                    break;

                case "CofinsTransportadorFormatado":
                    if (!select.Contains(" CofinsTransportador, "))
                    {
                        select.Append("ConfiguracaoEmpresa.COF_ALIQUOTA_COFINS CofinsTransportador, ");
                        groupBy.Append("ConfiguracaoEmpresa.COF_ALIQUOTA_COFINS, ");

                        SetarJoinsConfiguracaoEmpresa(joins);
                    }
                    break;

                case "CanalVenda":
                    if (!select.Contains(" CanalVenda, "))
                    {
                        select.Append(@"CanalVenda.CNV_DESCRICAO CanalVenda, ");
                        groupBy.Append("CanalVenda.CNV_DESCRICAO, ");

                        SetarJoinsCanalVenda(joins);
                    }
                    break;

                case "DescricaoGrupoCarga":
                    if (!select.Contains(" GrupoCarga, "))
                    {
                        select.Append(@"TabelaFreteCliente.TFC_TIPO_GRUPO_CARGA GrupoCarga, ");
                        groupBy.Append("TabelaFreteCliente.TFC_TIPO_GRUPO_CARGA, ");
                    }
                    break;

                case "DescricaoGerenciarCapacidade":
                    if (!select.Contains(" GerenciarCapacidade, "))
                    {
                        select.Append(@"TabelaFreteCliente.TFC_GERENCIAR_CAPACIDADE GerenciarCapacidade, ");
                        groupBy.Append("TabelaFreteCliente.TFC_GERENCIAR_CAPACIDADE, ");
                    }
                    break;

                case "DescricaoEstruturaTabela":
                    if (!select.Contains(" EstruturaTabela, "))
                    {
                        select.Append(@"isnull(TabelaFreteCliente.TFC_ESTRUTURA_TABELA, 99) EstruturaTabela, ");
                        groupBy.Append("TabelaFreteCliente.TFC_ESTRUTURA_TABELA, ");
                    }
                    break;

                case "CodigoContrato":
                    if (!select.Contains(" CodigoContrato, "))
                    {
                        select.Append(@"ContratoTransporteFrete.CTF_NUMERO_CONTRATO CodigoContrato, ");
                        groupBy.Append("ContratoTransporteFrete.CTF_NUMERO_CONTRATO, ");

                        SetarJoinsContratoFreteTransportador(joins);
                    }
                    break;

                case "DescricaoContrato":
                    if (!select.Contains(" DescricaoContrato, "))
                    {
                        select.Append(@"ContratoTransporteFrete.CTF_NOME_CONTRATO DescricaoContrato, ");
                        groupBy.Append("ContratoTransporteFrete.CTF_NOME_CONTRATO, ");

                        SetarJoinsContratoFreteTransportador(joins);
                    }
                    break;

                case "NumeroContrato":
                    if (!select.Contains(" NumeroContrato, "))
                    {
                        select.Append(@"ContratoTransporteFrete.CTF_NUMERO_CONTRATO NumeroContrato, ");
                        groupBy.Append("ContratoTransporteFrete.CTF_NUMERO_CONTRATO, ");

                        SetarJoinsContratoFreteTransportador(joins);
                    }
                    break;


                case "GrupoDaCarga":
                case "GrupoDaCargaEnumerado":
                    if (!select.Contains(" GrupoDaCargaEnumerado, "))
                    {
                        select.Append(@"TabelaFreteCliente.TFC_TIPO_GRUPO_CARGA GrupoDaCargaEnumerado, ");
                        groupBy.Append("TabelaFreteCliente.TFC_TIPO_GRUPO_CARGA, ");
                    }
                    break;


                case "DescricaoSituacaoTabela":
                    if (!select.Contains(" SituacaoTabela, "))
                    {
                        select.Append(@"TabelaFrete.TBF_ATIVO SituacaoTabela, ");
                        groupBy.Append("TabelaFrete.TBF_ATIVO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;


                case "DescricaoTabelaComVinculoCarga":
                    if (!select.Contains(" TabelaComVinculoCarga, "))
                    {
                        select.Append(@"CASE WHEN EXISTS (SELECT 1
                                                              FROM T_CARGA_TABELA_FRETE_CLIENTE
                                                              WHERE T_CARGA_TABELA_FRETE_CLIENTE.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO) THEN 1
                                                     ELSE 0 END TabelaComVinculoCarga, ");

                        if (!groupBy.Contains("TabelaFreteCliente.TFC_CODIGO,"))
                            groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                    }
                    break;

                case "CodigoIntegracaoTabelaFreteCliente":
                    if (!select.Contains(" CodigoIntegracaoTabelaFreteCliente,"))
                    {
                        select.Append("TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracaoTabelaFreteCliente, ");

                        if (!groupBy.Contains("TabelaFreteCliente.TFC_CODIGO_INTEGRACAO,"))
                            groupBy.Append("TabelaFreteCliente.TFC_CODIGO_INTEGRACAO, ");
                    }
                    break;

                //case "ContratoExternoID":
                //    if (!select.Contains(" ContratoExternoID, "))
                //    {
                //        select.Append(@"CAST((SELECT TOP 1 IntegracaoFrete.IFR_CODIGO_INTEGRACAO FROM T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemParametroBaseCalculoTabelaFrete
                //                            LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBaseCalculoTabelaFrete ON ItemParametroBaseCalculoTabelaFrete.TBC_CODIGO = ParametroBaseCalculoTabelaFrete.TBC_CODIGO
                //                            LEFT JOIN T_INTEGRACAO_FRETE IntegracaoFrete ON ItemParametroBaseCalculoTabelaFrete.TPI_CODIGO = IntegracaoFrete.IFR_CODIGO_INTEGRACAO
                //                            WHERE ParametroBaseCalculoTabelaFrete.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO) AS varchar) ContratoExternoID, ");
                //        groupBy.Append("TabelaFreteCliente.TFC_CODIGO, ");
                //    }
                //    break;

                //case "StatusAssinaturaContrato":
                //    if (!select.Contains(" StatusAssinaturaContrato, "))
                //    {
                //        select.Append(@"StatusAssinaturaContrato.STC_DESCRICAO StatusAssinaturaContrato, ");
                //        groupBy.Append("StatusAssinaturaContrato.STC_DESCRICAO, ");

                //        SetarJoinsStatusAssinaturaContrato(joins);
                //    }
                //    break;

                //case "StatusAssinaturaContrato":
                //    if (!select.Contains(" StatusAssinaturaContrato, "))
                //    {
                //        select.Append(@"TabelaFreteClienteIntegracao.INT_PROBLEMA_INTEGRACAO StatusAssinaturaContrato, ");
                //        groupBy.Append("TabelaFreteClienteIntegracao.INT_PROBLEMA_INTEGRACAO, ");

                //        SetarJoinsTabelaFreteClienteIntegracao(joins);
                //    }
                //    break;

                default:
                    if (propriedade.Contains("DescricaoValorComponente") || propriedade.Contains("DescricaoAntigoValorComponente"))
                    {
                        string descricaoPropriedade = propriedade.Replace("Descricao", "").Replace("Antigo", "");

                        if (!somenteContarNumeroRegistros)
                        {
                            if (!select.Contains($" Antigo{descricaoPropriedade}, ") && propriedade.Contains("Antigo"))
                                select.Append($"{descricaoPropriedade}.TPI_VALOR_ORIGINAL Antigo{descricaoPropriedade}, ");
                            else if (!select.Contains($" {descricaoPropriedade}, ") && !propriedade.Contains("Antigo"))
                                select.Append($"{descricaoPropriedade}.TPI_VALOR {descricaoPropriedade}, ");

                            if (!select.Contains($" Tipo{descricaoPropriedade}, "))
                                select.Append($"{descricaoPropriedade}.TPI_TIPO_VALOR Tipo{descricaoPropriedade}, ");

                            if (!select.Contains($" Codigo{descricaoPropriedade}, "))
                                select.Append($"{descricaoPropriedade}.TPI_CODIGO Codigo{descricaoPropriedade}, ");

                            if (!groupBy.Contains($"{descricaoPropriedade}.TPI_VALOR_ORIGINAL,, ") && propriedade.Contains("Antigo"))
                                groupBy.Append($"{descricaoPropriedade}.TPI_VALOR_ORIGINAL, ");
                            else if (!groupBy.Contains($"{descricaoPropriedade}.TPI_VALOR, ") && !propriedade.Contains("Antigo"))
                                groupBy.Append($"{descricaoPropriedade}.TPI_VALOR, ");

                            if (!groupBy.Contains($"{descricaoPropriedade}.TPI_TIPO_VALOR,, "))
                                groupBy.Append($"{descricaoPropriedade}.TPI_TIPO_VALOR, ");

                            if (!groupBy.Contains($"{descricaoPropriedade}.TPI_CODIGO, "))
                                groupBy.Append($"{descricaoPropriedade}.TPI_CODIGO, ");
                        }

                        if (!joins.Contains($" {descricaoPropriedade} "))
                        {
                            joins.Append("left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM " + descricaoPropriedade + " on ");

                            if (filtroPesquisa.ParametroBase.HasValue)
                                joins.Append(descricaoPropriedade + ".TBC_CODIGO = Parametro.TBC_CODIGO ");
                            else
                                joins.Append(descricaoPropriedade + ".TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO ");

                            joins.Append("and " + descricaoPropriedade + ".TPI_TIPO_OBJETO = 4 and " + descricaoPropriedade + ".TPI_CODIGO_OBJETO = " + codigoDinamico + " ");
                        }

                        if (filtroPesquisa.TipoRegistro.HasValue)
                        {
                            if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.Alterados)
                                _wherePorTipoRegistro.Append("or " + descricaoPropriedade + ".TPI_VALOR <> " + descricaoPropriedade + ".TPI_VALOR_ORIGINAL ");
                            else if (filtroPesquisa.TipoRegistro == TipoRegistroAjusteTabelaFrete.SemAlteracao)
                                _wherePorTipoRegistro.Append("and " + descricaoPropriedade + ".TPI_VALOR = " + descricaoPropriedade + ".TPI_VALOR_ORIGINAL ");
                        }
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            List<string> nomeItensJoins = ObterNomeItensJoins(joins);

            if (filtrosPesquisa.CodigosTabelasFreteCliente?.Count > 0)
                where.Append($"and TabelaFreteCliente.TFC_ATIVO = 1 and TabelaFreteCliente.TFC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTabelasFreteCliente)}) ");
            else
                where.Append($"and TabelaFreteCliente.TFC_ATIVO = 1 and TabelaFreteCliente.TBF_CODIGO = {filtrosPesquisa.CodigoTabelaFrete} ");

            if (filtrosPesquisa.CodigoAjusteTabelaFrete > 0 || filtrosPesquisa.CodigosAjustesTabelaFrete?.Count > 0)
            {
                List<int> situacoesSemExibicao = new List<int>()
                {
                    (int)SituacaoAjusteTabelaFrete.EmCriacao,
                    (int)SituacaoAjusteTabelaFrete.ProblemaCriacao
                };

                if (filtrosPesquisa.CodigosAjustesTabelaFrete?.Count > 0 || filtrosPesquisa.CodigosAjustesTabelaFrete != null)
                {
                    where.Append($"and TabelaFreteCliente.TFC_APLICAR_ALTERACOES_DO_AJUSTE = 1 and TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.Ajuste} ");
                    where.Append($"and TabelaFreteAjuste.TFA_SITUACAO not in ({string.Join(",", situacoesSemExibicao)}) and TabelaFreteCliente.TFA_CODIGO in  ({string.Join(", ", filtrosPesquisa.CodigosAjustesTabelaFrete)}) ");

                }
                else
                {
                    where.Append($"and TabelaFreteCliente.TFC_APLICAR_ALTERACOES_DO_AJUSTE = 1 and TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.Ajuste} ");
                    where.Append($"and TabelaFreteAjuste.TFA_SITUACAO not in ({string.Join(",", situacoesSemExibicao)}) and TabelaFreteCliente.TFA_CODIGO = {filtrosPesquisa.CodigoAjusteTabelaFrete} ");
                }

                SetarJoinsTabelaFreteAjuste(joins);
            }
            else if (filtrosPesquisa.CodigoLicitacaoParticipacao > 0)
                where.Append($"and TabelaFreteCliente.LIP_CODIGO = {filtrosPesquisa.CodigoLicitacaoParticipacao} ");
            else if (filtrosPesquisa.CodigoTabelaFreteCliente > 0)
                where.Append($"and TabelaFreteCliente.TFC_CODIGO = {filtrosPesquisa.CodigoTabelaFreteCliente} ");
            else if (filtrosPesquisa.ExibirSomenteAguardandoAprovacao)
                where.Append($"and TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.Alteracao} ");
            else
            {
                //if (filtrosPesquisa.SituacaoAlteracao.HasValue && (filtrosPesquisa.SituacaoAlteracao.Value == SituacaoAlteracaoTabelaFrete.NaoInformada || filtrosPesquisa.SituacaoAlteracao.Value == SituacaoAlteracaoTabelaFrete.AguardandoAprovacao))
                where.Append($"and TabelaFreteCliente.TFC_TIPO in ({(int)TipoTabelaFreteCliente.Calculo},{(int)TipoTabelaFreteCliente.Alteracao}) ");
                //else
                //where.Append($"and TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.Calculo} ");

                if (filtrosPesquisa.CodigoVigencia > 0)
                    where.Append($"and TabelaFreteCliente.TFV_CODIGO = {filtrosPesquisa.CodigoVigencia} ");

                if (filtrosPesquisa.DataInicialVigencia.HasValue || filtrosPesquisa.DataFinalVigencia.HasValue)
                {
                    SetarJoinsVigencia(joins);

                    if (filtrosPesquisa.DataInicialVigencia.HasValue)
                        where.Append($"and Vigencia.TFV_DATA_INICIAL >= '{filtrosPesquisa.DataInicialVigencia.Value.ToString("MM/dd/yyyy")}' ");

                    if (filtrosPesquisa.DataFinalVigencia.HasValue)
                        where.Append($"and Vigencia.TFV_DATA_FINAL <= '{filtrosPesquisa.DataFinalVigencia.Value.ToString("MM/dd/yyyy")}' ");
                }

                if (filtrosPesquisa.CodigosLocalidadeOrigem?.Count > 0)
                    _where.Append($"and TabelaFreteCliente.TFC_CODIGO in (select TFC_CODIGO from T_TABELA_FRETE_CLIENTE_ORIGEM where LOC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosLocalidadeOrigem)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosLocalidadeDestino?.Count > 0)
                    _where.Append($"and TabelaFreteCliente.TFC_CODIGO in (select TFC_CODIGO from T_TABELA_FRETE_CLIENTE_DESTINO where LOC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosLocalidadeDestino)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
                    _where.Append($"and exists (select regiaoDestino1.REG_CODIGO from T_TABELA_FRETE_CLIENTE_REGIAO_DESTINO regiaoDestino1 where regiaoDestino1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and regiaoDestino1.REG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosRegiaoDestino)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosRegiaoOrigem?.Count > 0)
                    _where.Append($"and exists (select regiaoOrigem1.REG_CODIGO from T_TABELA_FRETE_CLIENTE_REGIAO_ORIGEM regiaoOrigem1 where regiaoOrigem1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and regiaoOrigem1.REG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosRegiaoOrigem)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosCanaisEntrega?.Count > 0)
                    _where.Append($"and exists (select top(1) canalVenda1.CNE_CODIGO from T_CANAL_ENTREGA canalEntrega1 where canalEntrega1.CNE_CODIGO = TabelaFreteCliente.CNE_CODIGO and canalEntrega1.CNE_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosCanaisEntrega)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosCanaisVenda?.Count > 0)
                    _where.Append($"and exists (select top(1) canalVenda1.CNV_CODIGO from T_CANAL_VENDA canalVenda1 where canalVenda1.CNV_CODIGO = TabelaFreteCliente.CNV_CODIGO and canalVenda1.CNV_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosCanaisVenda)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosEstadoOrigem?.Count > 0)
                {
                    if (filtrosPesquisa.UtilizarBuscaNasLocalidadesPorEstadoOrigem)
                    {
                        _where.Append("and exists ( ");
                        _where.Append($"   select top(1) estadoOrigem1.UF_SIGLA from T_TABELA_FRETE_CLIENTE_ESTADO_ORIGEM estadoOrigem1 where estadoOrigem1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and estadoOrigem1.UF_SIGLA in ({string.Join(", ", filtrosPesquisa.CodigosEstadoOrigem.Select(o => $"'{o}'"))}) "); // SQL-INJECTION-SAFE
                        _where.Append("     union ");
                        _where.Append($"   select top(1) LocalidadeOrigem.UF_SIGLA from T_TABELA_FRETE_CLIENTE_ORIGEM TabelaFreteClienteOrigem inner join T_LOCALIDADES LocalidadeOrigem on TabelaFreteClienteOrigem.LOC_CODIGO = LocalidadeOrigem.LOC_CODIGO where TabelaFreteClienteOrigem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and LocalidadeOrigem.UF_SIGLA in ({string.Join(", ", filtrosPesquisa.CodigosEstadoOrigem.Select(o => $"'{o}'"))}) "); // SQL-INJECTION-SAFE
                        _where.Append(") ");
                    }
                    else
                        _where.Append($"and exists (select estadoOrigem1.UF_SIGLA from T_TABELA_FRETE_CLIENTE_ESTADO_ORIGEM estadoOrigem1 where estadoOrigem1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and estadoOrigem1.UF_SIGLA in ({string.Join(",", filtrosPesquisa.CodigosEstadoOrigem.Select(o => $"'{o}'"))})) "); // SQL-INJECTION-SAFE
                }

                if (filtrosPesquisa.CodigosEstadoDestino?.Count > 0)
                {
                    if (filtrosPesquisa.UtilizarBuscaNasLocalidadesPorEstadoDestino)
                    {
                        _where.Append("and exists ( ");
                        _where.Append($"   select TOP(1) estadoDestino1.UF_SIGLA from T_TABELA_FRETE_CLIENTE_ESTADO_DESTINO estadoDestino1 where estadoDestino1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and estadoDestino1.UF_SIGLA in ({string.Join(",", filtrosPesquisa.CodigosEstadoDestino.Select(o => $"'{o}'"))}) "); // SQL-INJECTION-SAFE
                        _where.Append("     union ");
                        _where.Append($"   select top(1) LocalidadeDestino.UF_SIGLA from T_TABELA_FRETE_CLIENTE_DESTINO TabelaFreteClienteDestino inner join T_LOCALIDADES LocalidadeDestino on TabelaFreteClienteDestino.LOC_CODIGO = LocalidadeDestino.LOC_CODIGO where TabelaFreteClienteDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and LocalidadeDestino.UF_SIGLA in ({string.Join(",", filtrosPesquisa.CodigosEstadoDestino.Select(o => $"'{o}'"))}) "); // SQL-INJECTION-SAFE
                        _where.Append(") ");
                    }
                    else
                        _where.Append($"and exists (select estadoDestino1.UF_SIGLA from T_TABELA_FRETE_CLIENTE_ESTADO_DESTINO estadoDestino1 where estadoDestino1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and estadoDestino1.UF_SIGLA in ({string.Join(",", filtrosPesquisa.CodigosEstadoDestino.Select(o => $"'{o}'"))})) "); // SQL-INJECTION-SAFE
                }

                if (filtrosPesquisa.CodigosTipoOperacao?.Count() > 0)
                    _where.Append($"and exists (select tipoOperacao1.TOP_CODIGO from T_TABELA_FRETE_TIPOS_OPERACAO tipoOperacao1 where tipoOperacao1.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO and tipoOperacao1.TOP_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoOperacao)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosRotaFreteDestino?.Count > 0)
                    _where.Append($"and exists (select rotaDestino1.ROF_CODIGO from T_TABELA_FRETE_CLIENTE_ROTA_DESTINO rotaDestino1 where rotaDestino1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and rotaDestino1.ROF_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosRotaFreteDestino)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosRotaFreteOrigem?.Count > 0)
                    _where.Append($"and exists (select rotaOrigem1.ROF_CODIGO from T_TABELA_FRETE_CLIENTE_ROTA_ORIGEM rotaOrigem1 where rotaOrigem1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and rotaOrigem1.ROF_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosRotaFreteOrigem)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                    _where.Append($"and TabelaFreteCliente.EMP_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosEmpresa)}) ");

                if (filtrosPesquisa.CodigoContratoTransporteFrete > 0)
                    _where.Append($"and TabelaFreteCliente.CTF_CODIGO = {filtrosPesquisa.CodigoContratoTransporteFrete} ");

                if (filtrosPesquisa.TipoPagamentoEmissao.HasValue)
                    _where.Append($"and TabelaFreteCliente.TFC_TIPO_PAGAMENTO = {(int)filtrosPesquisa.TipoPagamentoEmissao.Value} ");

                if (filtrosPesquisa.CodigosModeloReboque?.Count > 0)
                {
                    if (joins.Contains("ItemItemModeloReboque"))
                        _where.Append($"and ItemItemModeloReboque.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloReboque)}) ");
                    else
                    {
                        if (filtrosPesquisa.ParametroBase.HasValue && (filtrosPesquisa.ParametroBase.Value == TipoParametroBaseTabelaFrete.ModeloReboque))
                        {
                            _where.Append($"and ModeloReboque.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloReboque)}) ");

                            if (!joins.Contains(" ModeloReboque "))
                                _where.Append("left outer join T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque on Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO left outer join T_MODELO_VEICULAR_CARGA ModeloReboque on PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO ");
                        }
                        else
                        {
                            _where.Append($"and FiltroModeloReboque.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloReboque)}) ");

                            joins.Append("left outer join T_TABELA_FRETE_MODELO_REBOQUE FiltroModeloReboque on TabelaFrete.TBF_CODIGO = FiltroModeloReboque.TBF_CODIGO ");
                        }
                    }
                }

                if (filtrosPesquisa.CodigosModeloTracao?.Count > 0)
                {
                    if (joins.Contains("ItemItemModeloTracao"))
                        _where.Append($"and ItemItemModeloTracao.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloTracao)}) ");
                    else
                    {
                        if (filtrosPesquisa.ParametroBase.HasValue && (filtrosPesquisa.ParametroBase.Value == TipoParametroBaseTabelaFrete.ModeloTracao))
                        {
                            _where.Append($"and ModeloTracao.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloTracao)}) ");

                            if (!joins.Contains(" ModeloTracao "))
                                joins.Append("left outer join T_TABELA_FRETE_MODELO_TRACAO PModeloTracao on Parametro.TBC_CODIGO_OBJETO = PModeloTracao.MVC_CODIGO left outer join T_MODELO_VEICULAR_CARGA ModeloTracao on PModeloTracao.MVC_CODIGO = ModeloTracao.MVC_CODIGO ");
                        }
                        else
                        {
                            _where.Append($"and FiltroModeloTracao.MVC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloTracao)}) ");

                            joins.Append("left outer join T_TABELA_FRETE_MODELO_TRACAO FiltroModeloTracao on TabelaFrete.TBF_CODIGO = FiltroModeloTracao.TBF_CODIGO ");
                        }
                    }
                }

                if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                {
                    if (joins.Contains("ItemItemTipoCarga"))
                        _where.Append($"and ItemItemTipoCarga.TCG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoCarga)}) ");
                    else
                    {
                        if (filtrosPesquisa.ParametroBase.HasValue && (filtrosPesquisa.ParametroBase.Value == TipoParametroBaseTabelaFrete.TipoCarga))
                        {
                            _where.Append($"and TipoCarga.TCG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoCarga)}) ");

                            if (!joins.Contains(" TipoCarga "))
                                joins.Append("left outer join T_TABELA_FRETE_TIPO_CARGA PTipoCarga on Parametro.TBC_CODIGO_OBJETO = PTipoCarga.TCG_CODIGO left outer join T_TIPO_DE_CARGA TipoCarga on PTipoCarga.TCG_CODIGO = TipoCarga.TCG_CODIGO ");
                        }
                        else
                        {
                            _where.Append($"and FiltroTipoCarga.TCG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoCarga)}) ");

                            joins.Append("left outer join T_TABELA_FRETE_TIPO_CARGA FiltroTipoCarga on TabelaFrete.TBF_CODIGO = FiltroTipoCarga.TBF_CODIGO ");
                        }
                    }
                }

                if ((filtrosPesquisa.CpfCnpjRemetentes?.Count > 0) && !joins.Contains("Remetente"))
                    _where.Append($"and exists (select clienteOrigem1.CLI_CGCCPF from T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM clienteOrigem1 where clienteOrigem1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and clienteOrigem1.CLI_CGCCPF in ({string.Join(",", filtrosPesquisa.CpfCnpjRemetentes)})) "); // SQL-INJECTION-SAFE

                if ((filtrosPesquisa.CpfCnpjDestinatarios?.Count > 0) && !joins.Contains("Destinatario"))
                    _where.Append($"and exists (select clienteDestino1.CLI_CGCCPF from T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO clienteDestino1 where clienteDestino1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and clienteDestino1.CLI_CGCCPF in ({string.Join(",", filtrosPesquisa.CpfCnpjDestinatarios)})) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CpfCnpjTomadores?.Count > 0)
                {
                    _where.Append($"and Tomador.CLI_CGCCPF in ({string.Join(",", filtrosPesquisa.CpfCnpjTomadores)}) ");

                    SetarJoinsTomador(joins);
                }

                if (filtrosPesquisa.CodigosComplemento?.Count > 0)
                {
                    _where.Append($"and TabelaFreteComponente.CFR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosComplemento)}) ");
                    SetarJoinsTabelaFreteComponente(joins);
                }

                if (filtrosPesquisa.TabelaComCargaRealizada)
                    _where.Append("and (select COUNT(CTC_CODIGO) from T_CARGA_TABELA_FRETE_CLIENTE where TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO) > 0 ");

                if (filtrosPesquisa.CpfCnpjTransportadorTerceiro > 0d)
                    _where.Append($"and exists (select transportadorTerceiro.CLI_CGCCPF from T_TABELA_FRETE_CLIENTE_TRANSPORTADOR_TERCEIRO transportadorTerceiro where transportadorTerceiro.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and transportadorTerceiro.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjTransportadorTerceiro}) "); // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                    _where.Append($"and TabelaFrete.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas} ");

                if (filtrosPesquisa.AjustarPedagiosComSemParar)
                    _where.Append("and exists (select 1 from T_CARGA_TABELA_FRETE_CLIENTE where TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and TabelaFreteCliente.TFC_CODIGO_SEM_PARAR is not null and TabelaFreteCliente.TFC_CODIGO_SEM_PARAR != '')");

                if (filtrosPesquisa.SomenteEmVigencia)
                {
                    SetarJoinsVigencia(joins);

                    _where.Append($"and Vigencia.TFV_DATA_INICIAL <= '{DateTime.Now.Date.ToString("MM/dd/yyyy")}' and (Vigencia.TFV_DATA_FINAL is null or Vigencia.TFV_DATA_FINAL >= '{DateTime.Now.Date.ToString("MM/dd/yyyy")}')");
                }

                if (filtrosPesquisa.SituacaoAlteracao.HasValue && filtrosPesquisa.SituacaoAlteracao.Value != SituacaoAlteracaoTabelaFrete.NaoInformada)
                    _where.Append($"and TabelaFreteCliente.TFC_SITUACAO_ALTERACAO = {(int)filtrosPesquisa.SituacaoAlteracao}");

                if (filtrosPesquisa.CodigosContratoTransportador?.Count > 0)
                    _where.Append($"and TabelaFreteCliente.CTF_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosContratoTransportador)})");

                if (filtrosPesquisa.CodigoTipoOperacao > 0)
                    _where.Append($"and exists (select 1 from T_TABELA_FRETE_CLIENTE_TIPO_OPERACAO tipoOperacaoTabela inner join T_TIPO_OPERACAO tipoOperacao1 on tipoOperacao1.TOP_CODIGO = tipoOperacaoTabela.TOP_CODIGO where TabelaFreteCliente.TFC_CODIGO = tipoOperacaoTabela.TFC_CODIGO and tipoOperacao1.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}) "); // SQL-INJECTION-SAFE

            }

            if (filtrosPesquisa.SomenteRegistrosComValores)
            {
                List<string> itensComvalores = new List<string>();

                foreach (string nomeItem in nomeItensJoins)
                    itensComvalores.Add($"{nomeItem}.TPI_VALOR > 0 or {nomeItem}.TPI_VALOR_ORIGINAL > 0 ");

                if (itensComvalores.Count > 0)
                    _where.Append($"and ({string.Join("or ", itensComvalores)}) ");
            }

            if (filtrosPesquisa.CodigoStatusAceiteValor > 0)
            {
                List<string> itensPorStatusAprovacao = new List<string>();

                foreach (string nomeItem in nomeItensJoins)
                    itensPorStatusAprovacao.Add($"{nomeItem}.STC_CODIGO = {filtrosPesquisa.CodigoStatusAceiteValor}");

                if (itensPorStatusAprovacao.Count > 0)
                    _where.Append($"and ({string.Join("or ", itensPorStatusAprovacao)}) ");
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracaoTabelaFreteCliente))
            {
                where.Append($"and TabelaFreteCliente.TFC_CODIGO_INTEGRACAO = :TABELAFRETECLIENTE_TFC_CODIGO_INTEGRACAO "); 
                parametros.Add(new ParametroSQL("TABELAFRETECLIENTE_TFC_CODIGO_INTEGRACAO", filtrosPesquisa.CodigoIntegracaoTabelaFreteCliente));
            }

            where.Append(_where.ToString());

            if (_wherePorTipoRegistro.Length > 0)
                where.Append($"and ({_wherePorTipoRegistro.ToString().Trim().Substring(3)}) "); 
        }

        #endregion
    }
}

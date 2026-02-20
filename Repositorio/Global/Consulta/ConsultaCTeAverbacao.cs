using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaCTeAverbacao : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados>
    {
        #region Construtores

        public ConsultaCTeAverbacao() : base(tabela: "T_CTE_AVERBACAO as CTeAverbacao ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CTe "))
                joins.Append(" LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = CTeAverbacao.CON_CODIGO ");
        }

        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" EmpresaSerie "))
                joins.Append(" LEFT JOIN T_EMPRESA_SERIE EmpresaSerie ON EmpresaSerie.ESE_CODIGO = CTe.CON_SERIE ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" TerminoPrestacao "))
                joins.Append(" LEFT JOIN T_LOCALIDADES TerminoPrestacao on TerminoPrestacao.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" InicioPrestacao "))
                joins.Append(" LEFT JOIN T_LOCALIDADES InicioPrestacao on InicioPrestacao.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CTeAverbacao.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append(" LEFT JOIN T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = CTe.CON_MODELODOC ");
        }

        private void SetarJoinsApoliceSeguroAverbacao(StringBuilder joins)
        {
            if (!joins.Contains(" ApoliceSeguroAverbacao "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO ApoliceSeguroAverbacao on ApoliceSeguroAverbacao.CPA_CODIGO = CTeAverbacao.CPA_CODIGO ");
        }

        private void SetarJoinsApoliceSeguro(StringBuilder joins)
        {
            SetarJoinsApoliceSeguroAverbacao(joins);

            if (!joins.Contains(" ApoliceSeguro "))
                joins.Append(" LEFT JOIN T_APOLICE_SEGURO_GERAL ApoliceSeguro on ApoliceSeguro.APS_CODIGO = ApoliceSeguroAverbacao.APS_CODIGO ");
        }

        private void SetarJoinSeguradora(StringBuilder joins)
        {
            SetarJoinsApoliceSeguro(joins);

            if (!joins.Contains(" Seguradora "))
                joins.Append(" LEFT JOIN T_SEGURADORA Seguradora on Seguradora.SEA_CODIGO = ApoliceSeguro.SEA_CODIGO ");
        }

        private void SetarJoinsRemetenteCTe(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" LEFT JOIN T_CTE_PARTICIPANTE RemetenteCTe on RemetenteCTe.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsRemetenteProvedorOS(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append(" LEFT JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CTe.CON_CLIENTE_PROVEDOR_OS ");
        }

        private void SetarJoinsTomadorPagador(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Tomador "))
                joins.Append(" LEFT JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsXMLNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" XMLNotaFiscal "))
                joins.Append(" LEFT JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = CTeAverbacao.NFX_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa)
        {
            switch (propriedade)
            {
                #region CTeAverbacao
                case "Codigo":
                    if (!select.Contains(" Codigo "))
                    {
                        select.Append("CTeAverbacao.AVE_CODIGO Codigo, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_CODIGO"))
                            groupBy.Append(" CTeAverbacao.AVE_CODIGO, ");
                    }
                    break;

                case "StatusAverbacaoCTeFormatada":
                    if (!select.Contains(" StatusAverbacaoCTe "))
                    {
                        select.Append("CTeAverbacao.AVE_STATUS StatusAverbacaoCTe, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_STATUS"))
                            groupBy.Append(" CTeAverbacao.AVE_STATUS, ");
                    }
                    break;

                case "MensagemRetorno":
                    if (!select.Contains(" MensagemRetorno "))
                    {
                        select.Append("CTeAverbacao.AVE_MENSAGEM_RETORNO MensagemRetorno, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_MENSAGEM_RETORNO"))
                            groupBy.Append(" CTeAverbacao.AVE_MENSAGEM_RETORNO, ");
                    }
                    break;

                case "NumeroAverbacao":
                    if (!select.Contains(" NumeroAverbacao "))
                    {
                        select.Append("CTeAverbacao.AVE_AVERBACAO NumeroAverbacao, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_AVERBACAO"))
                            groupBy.Append(" CTeAverbacao.AVE_AVERBACAO, ");
                    }
                    break;

                case "DataAverbacaoFormatada":
                    if (!select.Contains(" DataAverbacao "))
                    {
                        select.Append("CTeAverbacao.AVE_DATA_RETORNO DataAverbacao, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_DATA_RETORNO"))
                            groupBy.Append(" CTeAverbacao.AVE_DATA_RETORNO, ");
                    }
                    break;

                case "StatusFechamentoFormatada":
                    if (!select.Contains(" StatusFechamento "))
                    {
                        select.Append("CTeAverbacao.AVE_SITUACAO StatusFechamento, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_SITUACAO"))
                            groupBy.Append(" CTeAverbacao.AVE_SITUACAO, ");
                    }
                    break;

                case "PercentualDesconto":
                    if (!select.Contains(" PercentualDesconto "))
                    {
                        select.Append("CTeAverbacao.AVE_PERCENTUAL PercentualDesconto, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_PERCENTUAL"))
                            groupBy.Append(" CTeAverbacao.AVE_PERCENTUAL, ");
                    }
                    break;

                case "Desconto":
                    if (!select.Contains(" Desconto "))
                    {
                        select.Append("CTeAverbacao.AVE_DESCONTO Desconto, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_DESCONTO"))
                            groupBy.Append(" CTeAverbacao.AVE_DESCONTO, ");
                    }
                    break;

                case "FormaAverbacaoFormatada":
                    if (!select.Contains(" FormaAverbacao "))
                    {
                        select.Append("CTeAverbacao.AVE_FORMA FormaAverbacao, ");

                        if (!groupBy.Contains("CTeAverbacao.AVE_FORMA"))
                            groupBy.Append(" CTeAverbacao.AVE_FORMA, ");
                    }
                    break;
                #endregion

                #region CTe
                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe "))
                    {
                        select.Append("(case when CTe.CON_NUM is not null then CTe.CON_NUM else XMLNotaFiscal.NF_NUMERO end) NumeroCTe, ");

                        groupBy.Append(" CTe.CON_NUM, XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao "))
                    {
                        select.Append("(case when CTe.CON_DATAHORAEMISSAO is not null then CTe.CON_DATAHORAEMISSAO else XMLNotaFiscal.NF_DATA_EMISSAO end) DataEmissao, ");

                        groupBy.Append(" CTe.CON_DATAHORAEMISSAO, XMLNotaFiscal.NF_DATA_EMISSAO, ");

                        SetarJoinsCTe(joins);
                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "DescricaoStatusCTe":
                    if (!select.Contains(" StatusCTe "))
                    {
                        select.Append("CTe.CON_STATUS StatusCTe, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ValorMercadoria":
                    if (!select.Contains(" ValorMercadoria "))
                    {
                        select.Append("CTe.CON_VALOR_TOTAL_MERC ValorMercadoria, ");

                        if (!groupBy.Contains("CTe.CON_VALOR_TOTAL_MERC"))
                            groupBy.Append(" CTe.CON_VALOR_TOTAL_MERC, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ValorCTe":
                    if (!select.Contains(" ValorCTe "))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorCTe, ");

                        if (!groupBy.Contains("CTe.CON_VALOR_RECEBER"))
                            groupBy.Append(" CTe.CON_VALOR_RECEBER, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking "))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING NumeroBooking, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_BOOKING"))
                            groupBy.Append(" CTe.CON_NUMERO_BOOKING, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS "))
                    {
                        select.Append("CTe.CON_NUMERO_OS NumeroOS, ");

                        if (!groupBy.Contains("CTe.CON_NUMERO_OS"))
                            groupBy.Append(" CTe.CON_NUMERO_OS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Container":
                    if (!select.Contains(" Container "))
                    {
                        select.Append("CTe.CON_CONTAINER Container, ");

                        if (!groupBy.Contains("CTe.CON_CONTAINER"))
                            groupBy.Append(" CTe.CON_CONTAINER, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _filial.FIL_CNPJ
                                  from T_CARGA_CTE _cargaCTe 
                                 inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) CNPJFilial, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _filial.FIL_DESCRICAO
                                  from T_CARGA_CTE _cargaCTe 
                                 inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) Filial, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                #endregion

                #region Empresa
                case "Serie":
                    if (!select.Contains(" Serie "))
                    {
                        select.Append("(case when EmpresaSerie.ESE_NUMERO is not null then CAST(EmpresaSerie.ESE_NUMERO AS VARCHAR(20)) else XMLNotaFiscal.NF_SERIE end) Serie, ");

                        groupBy.Append(" EmpresaSerie.ESE_NUMERO, XMLNotaFiscal.NF_SERIE, ");

                        SetarJoinsEmpresaSerie(joins);
                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "CPFCNPJTomadorFormatada":
                    if (!select.Contains(" CPFCNPJTomador "))
                    {
                        select.Append("Tomador.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("Tomador.PCT_CPF_CNPJ"))
                            groupBy.Append(" Tomador.PCT_CPF_CNPJ, ");

                        SetarJoinsTomadorPagador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador "))
                    {
                        select.Append("Tomador.PCT_NOME Tomador, ");

                        if (!groupBy.Contains("Tomador.PCT_NOME"))
                            groupBy.Append(" Tomador.PCT_NOME, ");

                        SetarJoinsTomadorPagador(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador "))
                    {
                        select.Append("Empresa.EMP_RAZAO Transportador, ");

                        if (!groupBy.Contains("Empresa.EMP_RAZAO"))
                            groupBy.Append(" Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CNPJTransportadorFormatada":
                    if (!select.Contains(" CNPJTransportador "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJTransportador, ");

                        if (!groupBy.Contains("Empresa.EMP_CNPJ"))
                            groupBy.Append(" Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;
                #endregion

                #region Carga
                case "Carga":
                    if (!select.Contains(" Carga "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_CARGA_EMBARCADOR"))
                            groupBy.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + TipoCarga.TCG_DESCRICAO
                                        from T_TIPO_DE_CARGA TipoCarga 
                                        inner join T_CARGA Carga on Carga.TCG_CODIGO = TipoCarga.TCG_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataServicoFormatada":
                    if (!select.Contains(" DataServico "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataServico, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO"))
                            groupBy.Append(" Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                #endregion

                #region TipoOperacao
                case "TipoOperacao":
                    if (!select.Contains("TipoOperacao"))
                    {
                        select.Append(" TipoOperacao.TOP_DESCRICAO TipoOperacao, ");

                        if (!groupBy.Contains("TipoOperacao.TOP_DESCRICAO"))
                            groupBy.Append(" TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                #endregion

                #region Veiculo
                case "Veiculo":
                    if (!select.Contains(" Veiculo "))
                    {
                        select.Append(" Veiculo.VEI_PLACA Veiculo, ");

                        if (!groupBy.Contains("Veiculo.VEI_PLACA"))
                            groupBy.Append(" Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;
                #endregion

                #region Localidades
                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacao.LOC_DESCRICAO + '-' + InicioPrestacao.UF_SIGLA InicioPrestacao, ");

                        if (!groupBy.Contains("InicioPrestacao.LOC_DESCRICAO, InicioPrestacao.UF_SIGLA"))
                            groupBy.Append("InicioPrestacao.LOC_DESCRICAO, InicioPrestacao.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "TerminoPrestacao":
                    if (!select.Contains(" TerminoPrestacao,"))
                    {
                        select.Append(" TerminoPrestacao.LOC_DESCRICAO + '-' + TerminoPrestacao.UF_SIGLA TerminoPrestacao, ");

                        if (!groupBy.Contains("TerminoPrestacao.LOC_DESCRICAO, TerminoPrestacao.UF_SIGLA"))
                            groupBy.Append("TerminoPrestacao.LOC_DESCRICAO, TerminoPrestacao.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;
                #endregion

                #region Seguradora
                case "Seguradora":
                    if (!select.Contains(" Seguradora "))
                    {
                        select.Append(" Seguradora.SEA_NOME Seguradora, ");

                        if (!groupBy.Contains("Seguradora.SEA_NOME"))
                            groupBy.Append(" Seguradora.SEA_NOME, ");

                        SetarJoinSeguradora(joins);
                    }
                    break;

                case "Apolice":
                    if (!select.Contains(" Apolice "))
                    {
                        select.Append(" ApoliceSeguro.APS_NUMERO_APOLICE Apolice, ");

                        if (!groupBy.Contains("ApoliceSeguro.APS_NUMERO_APOLICE"))
                            groupBy.Append(" ApoliceSeguro.APS_NUMERO_APOLICE, ");

                        SetarJoinsApoliceSeguro(joins);
                    }
                    break;
                #endregion

                #region ModeloDocumentoFiscal
                case "ModeloDocumentoFiscal":
                    if (!select.Contains(" ModeloDocumentoFiscal "))
                    {
                        select.Append(" ModeloDocumentoFiscal.MOD_ABREVIACAO ModeloDocumentoFiscal, ");

                        if (!groupBy.Contains("ModeloDocumentoFiscal.MOD_ABREVIACAO"))
                            groupBy.Append(" ModeloDocumentoFiscal.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;
                #endregion

                #region Remetente
                case "Cliente":
                    if (!select.Contains(" RemetenteCTe "))
                    {
                        select.Append(" RemetenteCTe.PCT_NOME Cliente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_NOME"))
                            groupBy.Append(" RemetenteCTe.PCT_NOME, ");

                        SetarJoinsRemetenteCTe(joins);
                    }
                    break;

                case "ClienteProvedorOS":
                    if (!select.Contains(" Cliente "))
                    {
                        select.Append(" Cliente.CLI_NOME ClienteProvedorOS, ");

                        if (!groupBy.Contains("Cliente.CLI_NOME"))
                            groupBy.Append(" Cliente.CLI_NOME, ");

                        SetarJoinsRemetenteProvedorOS(joins);
                    }
                    break;
                #endregion

                default:
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Status.HasValue && filtrosPesquisa.Status.Value != StatusAverbacaoCTe.Todos)
                where.Append(" AND CTeAverbacao.AVE_STATUS = " + filtrosPesquisa.Status.Value.ToString("d"));

            if (filtrosPesquisa.SituacaoFechamento.HasValue && filtrosPesquisa.SituacaoFechamento.Value != SituacaoAverbacaoFechamento.Todas)
                where.Append(" AND CTeAverbacao.AVE_SITUACAO = " + filtrosPesquisa.SituacaoFechamento.Value.ToString("d"));

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
            {
                where.Append($" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");
                SetarJoinsEmpresa(joins);
            }

            if (filtrosPesquisa.CodigoSeguradora > 0)
            {
                where.Append(" and Seguradora.SEA_CODIGO = " + filtrosPesquisa.CodigoSeguradora.ToString());
                SetarJoinSeguradora(joins);
            }

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
            {
                where.Append(" and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "'");
                //where.Append(" or XMLNotaFiscal.NF_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "')");
                SetarJoinsCTe(joins);
                SetarJoinsXMLNotaFiscal(joins);
            }

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
            {
                where.Append(" and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(pattern) + "'");
                //where.Append(" or XMLNotaFiscal.NF_DATA_EMISSAO < '" + filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(pattern) + "')");
                SetarJoinsCTe(joins);
                SetarJoinsXMLNotaFiscal(joins);
            }

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
            {
                where.Append(" and ModeloDocumentoFiscal.MOD_CODIGO = " + filtrosPesquisa.CodigoModeloDocumentoFiscal.ToString());
                SetarJoinsModeloDocumentoFiscal(joins);
            }

            if (filtrosPesquisa.CodigoClienteProvedorOS > 0)
            {
                where.Append(" and Cliente.CLI_CGCCPF = " + filtrosPesquisa.CodigoClienteProvedorOS.ToString());
                SetarJoinsRemetenteProvedorOS(joins);
            }

            if (filtrosPesquisa.DataServicoInicial != DateTime.MinValue)
            {
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) >= '" + filtrosPesquisa.DataServicoInicial.ToString(pattern) + "'");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.DataServicoFinal != DateTime.MinValue)
            {
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) <= '" + filtrosPesquisa.DataServicoFinal.ToString(pattern) + "'");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TipoPropriedadeVeiculo != "A")
            {
                where.Append($" and Veiculo.VEI_TIPO = '{filtrosPesquisa.TipoPropriedadeVeiculo}' ");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.GrupoTomador.Count > 0)
            {
                where.Append($" and Tomador.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.GrupoTomador)})");
                SetarJoinsTomadorPagador(joins);
            }

        }

        #endregion
    }
}
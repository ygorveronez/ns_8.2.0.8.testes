using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    sealed class ConsultaProvisaoVolumetria : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria>
    {
        #region Construtores

        public ConsultaProvisaoVolumetria() : base(tabela: "T_XML_NOTA_FISCAL as NFe") { }

        #endregion Construtores

        #region Métodos Privados

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.Append(" left join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NFe.NFX_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA as Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO and Carga.CAR_CARGA_FECHADA = 1 ");
        }

        private void SetarJoinsCategoriaEmitenteNota(StringBuilder joins)
        {
            SetarJoinsEmitenteNota(joins);

            if (!joins.Contains(" CategoriaEmitenteNF "))
                joins.Append(" left join T_CATEGORIA_PESSOA as CategoriaEmitenteNF on CategoriaEmitenteNF.CTP_CODIGO = EmitenteNF.CTP_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsCTeXmlNotaFiscal(joins);

            if (!joins.Contains(" CTe "))
                joins.Append(" left join T_CTE as CTe on CTe.CON_CODIGO = CTeXMLNotaFiscal.CON_CODIGO ");
        }

        private void SetarJoinsCTeXmlNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" CTeXMLNotaFiscal "))
                joins.Append(" left join T_CTE_XML_NOTAS_FISCAIS as CTeXMLNotaFiscal on CTeXMLNotaFiscal.NFX_CODIGO = NFe.NFX_CODIGO ");
        }

        private void SetarJoinsDestinatarioNota(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioNF "))
                joins.Append(" inner join T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = NFe.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsDestinatarioPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsEmitenteNota(StringBuilder joins)
        {
            if (!joins.Contains(" EmitenteNF "))
                joins.Append(" inner join T_CLIENTE as EmitenteNF on EmitenteNF.CLI_CGCCPF = NFe.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsExpedidorNota(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorNF "))
                joins.Append(" left join T_CLIENTE as ExpedidorNF on ExpedidorNF.CLI_CGCCPF = NFe.CLI_CODIGO_EXPEDIDOR ");
        }

        private void SetarJoinsFatura(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Fatura "))
                joins.Append("left join T_FATURA as Fatura on Fatura.FAT_CODIGO = CTe.FAT_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoaDestinatarioNota(StringBuilder joins)
        {
            SetarJoinsDestinatarioNota(joins);

            if (!joins.Contains(" GrupoPessoaDestinatarioNF "))
                joins.Append(" left join T_GRUPO_PESSOAS as GrupoPessoaDestinatarioNF on GrupoPessoaDestinatarioNF.GRP_CODIGO = DestinatarioNF.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoaEmitenteNota(StringBuilder joins)
        {
            SetarJoinsEmitenteNota(joins);

            if (!joins.Contains(" GrupoPessoaEmitenteNF "))
                joins.Append(" left join T_GRUPO_PESSOAS as GrupoPessoaEmitenteNF on GrupoPessoaEmitenteNF.GRP_CODIGO = EmitenteNF.GRP_CODIGO ");
        }

        private void SetarJoinsLocalidadeDestinatarioNota(StringBuilder joins)
        {
            SetarJoinsDestinatarioNota(joins);

            if (!joins.Contains(" LocalidadeDestinatarioNF "))
                joins.Append(" inner join T_LOCALIDADES as LocalidadeDestinatarioNF on LocalidadeDestinatarioNF.LOC_CODIGO = DestinatarioNF.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeEmitenteNota(StringBuilder joins)
        {
            SetarJoinsEmitenteNota(joins);

            if (!joins.Contains(" LocalidadeEmitenteNF "))
                joins.Append(" inner join T_LOCALIDADES as LocalidadeEmitenteNF on LocalidadeEmitenteNF.LOC_CODIGO = EmitenteNF.LOC_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsPaisDestinatarioPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsDestinatarioPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" Pais "))
                joins.Append(" left join T_PAIS Pais on Pais.PAI_CODIGO = Destinatario.PAI_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsProdutoEmbarcador(StringBuilder joins)
        {
            SetarJoinsProdutoXmlNotaFiscal(joins);

            if (!joins.Contains(" ProdutoEmbarcador "))
                joins.Append(" left join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = ProdutoXMLNotaFiscal.PRO_CODIGO ");
        }

        private void SetarJoinsProdutoXmlNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoXMLNotaFiscal "))
                joins.Append(" left join T_XML_NOTA_FISCAL_PRODUTO as ProdutoXMLNotaFiscal on NFe.NFX_CODIGO = ProdutoXMLNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsRotaFrete(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" RotaFrete "))
                joins.Append(" left join T_ROTA_FRETE RotaFrete on RotaFrete.ROF_CODIGO = Pedido.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA as TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO as TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsVeiculoCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" VeiculoCarga "))
                joins.Append(" left join T_VEICULO VeiculoCarga ON VeiculoCarga.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsVendedor(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" Vendedor "))
                joins.Append(" left join T_FUNCIONARIO as Vendedor on Vendedor.FUN_CODIGO = Pedido.FUN_CODIGO_VENDEDOR ");
        }

        private void SetarJoinsCargaPedidoRecebedor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" RecebedorCarga "))
                joins.Append(" left join T_CLIENTE RecebedorCarga on RecebedorCarga.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ");
        }

        private void SetarJoinsCargaPedidoExpedidor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" ExpedidorCarga "))
                joins.Append(" left join T_CLIENTE ExpedidorCarga on ExpedidorCarga.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
        }

        private void SetarJoinsNotaFiscalMigo(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" NotaMigo "))
                joins.Append(" left join T_XML_NOTA_FISCAL_MIGO NotaMigo on NotaMigo.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsIntegracaoRecebimento(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" IntegracaoCTeRecebimento "))
                joins.Append(" left join T_INTEGRACAO_CTE_RECEBIMENTO IntegracaoCTeRecebimento on IntegracaoCTeRecebimento.CON_CODIGO = CTe.CON_CODIGO and IntegracaoCTeRecebimento.ICR_TIPO = 0 ");
        }

        private void SetarJoinsXMLNotaFiscalContabilizacao(StringBuilder joins)
        {
            if (!joins.Contains(" XMLNotaFiscalContabilizacao "))
                joins.Append(" left join T_XML_NOTA_FISCAL_CONTABILIZACAO XMLNotaFiscalContabilizacao ON NFe.NFX_CODIGO = XMLNotaFiscalContabilizacao.NFX_CODIGO ");
        }

        private void SetarJoinsPagamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsDocumentoFaturamento(joins, filtrosPesquisa);

            if (!joins.Contains(" Pagamento "))
                joins.Append(" left join T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = DocumentoFaturamentoCTe.PAG_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            if (!joins.Contains(" DocumentoFaturamentoCTe "))
                joins.Append($" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCTe on CTe.CON_CODIGO = DocumentoFaturamentoCTe.CON_CODIGO and DocumentoFaturamentoCTe.DFA_SITUACAO NOT IN (2, 3) {(filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "" : "and DocumentoFaturamentoCTe.DFA_SITUACAO = 5")}");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" TituloCTe "))
                joins.Append(" left join T_TITULO TituloCTe on TituloCTe.TIT_CODIGO = CTe.TIT_CODIGO ");
        }

        private void SetarJoinsPagamentoNFSAutorizacao(StringBuilder joins)
        {
            SetarJoinsLancamentoNFSManual(joins);

            if (!joins.Contains(" LNA "))
                joins.Append(" left join T_LANCAMENTO_NFS_AUTORIZACAO LNA ON LNA.LNM_CODIGO = LNM.LNM_CODIGO ");
        }

        private void SetarJoinsLancamentoNFSManual(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" LNM "))
                joins.Append(" left join T_LANCAMENTO_NFS_MANUAL LNM ON LNM.CON_CODIGO = CTe.CON_CODIGO");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsPagamentoNFSAutorizacao(joins);

            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on LNA.FUN_CODIGO = Funcionario.FUN_CODIGO");
        }

         private void SetarJoinsPagamentoIntegracao(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            SetarJoinsPagamento(joins, filtrosPesquisa);

            if (!joins.Contains(" PagamentoIntegracao "))
                joins.Append(" left join T_PAGAMENTO_INTEGRACAO PagamentoIntegracao on Pagamento.PAG_CODIGO = PagamentoIntegracao.PAG_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa)
        {
            if (!select.Contains("Codigo, "))
            {
                select.Append("NFe.NFX_CODIGO Codigo, ");

                if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                    groupBy.Append("NFe.NFX_CODIGO, ");
            }

            switch (propriedade)
            {
                case "CNPJEmpresaFormatada":
                    if (!select.Contains(" CNPJEmpresa, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins, filtrosPesquisa);
                    }
                    break;

                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins, filtrosPesquisa);
                    }
                    break;

                case "Placa":
                    if (!select.Contains("Placa"))
                    {
                        select.Append("VeiculoCarga.VEI_PLACA Placa, ");
                        groupBy.Append("VeiculoCarga.VEI_PLACA, ");

                        SetarJoinsVeiculoCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins, filtrosPesquisa);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains("DataEmissao"))
                    {
                        select.Append("NFe.NF_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("NFe.NF_DATA_EMISSAO, ");
                    }
                    break;

                case "NotasFiscal":
                    if (!select.Contains(" NotasFiscal, "))
                    {
                        select.Append("NFe.NF_NUMERO NotasFiscal, ");
                        groupBy.Append("NFe.NF_NUMERO, ");
                    }
                    break;

                case "SerieNotaFiscal":
                    if (!select.Contains("SerieNotaFiscal"))
                    {
                        select.Append("NFe.NF_SERIE SerieNotaFiscal, ");
                        groupBy.Append("NFe.NF_SERIE, ");
                    }
                    break;

                case "ChaveNFe":
                    if (!select.Contains("ChaveNFe"))
                    {
                        select.Append("NFe.NF_CHAVE ChaveNFe, ");
                        groupBy.Append("NFe.NF_CHAVE, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT DISTINCT ', ' + CAST(_cfop.CFO_CFOP AS NVARCHAR(20))
		                        FROM T_CTE _cte 
		                        LEFT JOIN T_CFOP _cfop on _cfop.CFO_CODIGO = _cte.CFO_CODIGO 
		                        LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                        WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A' FOR XML PATH('')), 3, 1000
	                        ) CFOP, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "Volumes":
                    if (!select.Contains(" Volumes, "))
                        select.Append("min(NFe.NF_VOLUMES) Volumes, ");
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("LocalidadeEmitenteNF.LOC_DESCRICAO + '-' + LocalidadeEmitenteNF.UF_SIGLA Origem, ");
                        groupBy.Append("LocalidadeEmitenteNF.LOC_DESCRICAO, LocalidadeEmitenteNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeEmitenteNota(joins);
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("LocalidadeEmitenteNF.UF_SIGLA UFOrigem, ");

                        if (!groupBy.Contains("LocalidadeEmitenteNF.UF_SIGLA"))
                            groupBy.Append("LocalidadeEmitenteNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeEmitenteNota(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("LocalidadeDestinatarioNF.LOC_DESCRICAO + '-' + LocalidadeDestinatarioNF.UF_SIGLA Destino, ");
                        groupBy.Append("LocalidadeDestinatarioNF.LOC_DESCRICAO, LocalidadeDestinatarioNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatarioNota(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("LocalidadeDestinatarioNF.UF_SIGLA UFDestino, ");

                        if (!groupBy.Contains("LocalidadeDestinatarioNF.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatarioNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatarioNota(joins);
                    }
                    break;

                case "CNPJRemetenteFormatado":
                    if (!select.Contains("CNPJRemetente"))
                    {
                        select.Append("EmitenteNF.CLI_CGCCPF CNPJRemetente, EmitenteNF.CLI_FISJUR TipoPessoaRemetente, ");
                        groupBy.Append("EmitenteNF.CLI_CGCCPF, EmitenteNF.CLI_FISJUR, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("EmitenteNF.CLI_NOME Remetente, ");
                        groupBy.Append("EmitenteNF.CLI_NOME, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "CNPJDestinatarioFormatado":
                    if (!select.Contains("CNPJDestinatario"))
                    {
                        select.Append("DestinatarioNF.CLI_CGCCPF CNPJDestinatario, DestinatarioNF.CLI_FISJUR TipoPessoaDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_CGCCPF, DestinatarioNF.CLI_FISJUR, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "DescricaoTipoTomador":
                    if (!select.Contains("TipoTomador"))
                    {
                        select.Append("CTe.CON_TOMADOR TipoTomador, ");
                        groupBy.Append("CTe.CON_TOMADOR, ");
                    }
                    break;

                case "DataEmissaoCTEFormatada":
                    if (!select.Contains("DataEmissaoCTE"))
                    {
                        select.Append(@"
                            (SELECT min(_cte.CON_DATA_AUTORIZACAO)
	                            FROM T_CTE _cte LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A') DataEmissaoCTE, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "CTe":
                    if (!select.Contains(" CTe, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT ', ' + CAST(_cte.CON_NUM AS NVARCHAR(20))
		                        FROM T_CTE _cte 
		                        LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                        WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO and _cte.CON_STATUS = 'A' FOR XML PATH('')), 3, 1000
	                        ) CTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT DISTINCT ', ' + CAST(_empresaserie.ESE_NUMERO AS NVARCHAR(20))
		                        FROM T_CTE _cte 
		                        LEFT JOIN T_EMPRESA_SERIE _empresaserie on _empresaserie.ESE_CODIGO = _cte.CON_SERIE 
		                        LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                        WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A' FOR XML PATH('')), 3, 1000
	                        ) SerieCTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "TipoCTeFormatado":
                    if (!select.Contains(" TipoCTe, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT ', ' + CAST(_cte.CON_TIPO_CTE AS NVARCHAR(20))
		                        FROM T_CTE _cte 
		                        LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                        WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO and _cte.CON_STATUS = 'A' FOR XML PATH('')), 3, 1000
	                        ) TipoCTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains("ChaveCTe"))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "DataImportacao":
                    if (!select.Contains("DataImportacao"))
                    {
                        select.Append("CASE IntegracaoCTeRecebimento.ICR_DATA WHEN NULL THEN '' ELSE convert(nvarchar(10), IntegracaoCTeRecebimento.ICR_DATA, 3) + ' ' + convert(nvarchar(10), IntegracaoCTeRecebimento.ICR_DATA, 108) END DataImportacao, ");
                        groupBy.Append("IntegracaoCTeRecebimento.ICR_DATA, ");

                        SetarJoinsIntegracaoRecebimento(joins);
                    }
                    break;

                case "DescricaoUC":
                    if (!select.Contains("DescricaoUC"))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_DESCRICAO_UC DescricaoUC, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_DESCRICAO_UC, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "DescricaoContaContabil":
                    if (!select.Contains("DescricaoContaContabil"))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_CONTA_CONTABIL DescricaoContaContabil, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_CONTA_CONTABIL, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial"))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins, filtrosPesquisa);
                    }
                    break;

                case "CodFilial":
                    if (!select.Contains(" CodFilial"))
                    {
                        select.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR CodFilial, ");
                        groupBy.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                        SetarJoinsFilial(joins, filtrosPesquisa);
                    }
                    break;

                case "Mercado":
                    if (!select.Contains("Mercado"))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_MERCADO Mercado, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_MERCADO, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "Diretoria":
                    if (!select.Contains("Diretoria"))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_DIRETORIA Diretoria, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_DIRETORIA, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "DataAprovacaoPagamentoFormatada":
                    if (!select.Contains(" DataAprovacaoPagamento, "))
                    {
                        select.Append(@"(select max(AAL_DATA) 
                                            from T_AUTORIZACAO_ALCADA_PAGAMENTO
                                            where PAG_CODIGO = Pagamento.PAG_CODIGO
                                            and AAL_SITUACAO = 1) DataAprovacaoPagamento, ");

                        groupBy.Append("Pagamento.PAG_CODIGO, ");
                    }
                    break;

                case "DataDevolucaoCTRCFormatada":
                    if (!select.Contains(" DataDevolucaoCTRC, "))
                    {
                        select.Append("(select LNA.LAA_DATA where LNA.LAA_SITUACAO = 9) DataDevolucaoCTRC, ");
                        groupBy.Append("LNA.LAA_DATA, ");

                        SetarJoinsPagamentoNFSAutorizacao(joins);
                    }
                    break;

                case "DataAprovacaoFormatada":
                    if (!select.Contains(" DataAprovacao, "))
                    {
                        select.Append("(select LNA.LAA_DATA where LNA.LAA_SITUACAO = 1) DataAprovacao, ");
                        groupBy.Append("LNA.LAA_SITUACAO, ");

                        SetarJoinsPagamentoNFSAutorizacao(joins);
                    }
                    break;

                case "NumeroPagamento":
                    if (!select.Contains(" NumeroPagamento, "))
                    {
                        select.Append("CAST(Pagamento.PAG_NUMERO AS NVARCHAR(20)) NumeroPagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");

                        SetarJoinsPagamento(joins, filtrosPesquisa);
                    }
                    break;

                case "SituacaoPagamentoFormatada":
                    if (!select.Contains(" SituacaoPagamento, "))
                    {
                        select.Append("ISNULL(Pagamento.PAG_SITUACAO, 0) SituacaoPagamento, ");
                        groupBy.Append("Pagamento.PAG_SITUACAO, ");

                        SetarJoinsPagamento(joins, filtrosPesquisa);
                    }
                    break;

                case "DataPagamentoFormatada":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        select.Append("TituloCTe.TIT_DATA_LIQUIDACAO DataPagamento, ");
                        if (!groupBy.Contains("TituloCTe.TIT_DATA_LIQUIDACAO"))
                            groupBy.Append("TituloCTe.TIT_DATA_LIQUIDACAO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga,"))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + cast(_carga.CAR_SITUACAO as varchar(10))
                                  from T_CARGA_CTE _cargaCTe 
                                  join T_CARGA _carga on _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')), 3, 200
                            ) SituacaoCarga, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Pago":
                    if (!select.Contains(" Pago"))
                    {
                        select.Append("CASE WHEN Fatura.FAT_SITUACAO = 2 THEN 'Sim' ELSE 'Não' END Pago, ");

                        if (!groupBy.Contains("Fatura.FAT_SITUACAO"))
                            groupBy.Append("Fatura.FAT_SITUACAO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "NomeAprovador":
                    if (!select.Contains(" NomeAprovador, "))
                    {
                        select.Append("Funcionario.FUN_NOME NomeAprovador, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "CIA":
                    if (!select.Contains(" CIA, "))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_CIA CIA, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_CIA, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "CodUC":
                    if (!select.Contains(" CodUC, "))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_UC CodUC, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_UC, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "DataIntegracaoPagamentoFormatada":
                    if (!select.Contains("DataIntegracaoPagamento"))
                    {
                        select.Append("PagamentoIntegracao.INT_DATA_INTEGRACAO DataIntegracaoPagamento, ");
                        groupBy.Append("PagamentoIntegracao.INT_DATA_INTEGRACAO, ");

                        SetarJoinsPagamentoIntegracao(joins, filtrosPesquisa);
                    }
                    break;

                case "ErroIntegracaoPagamento":
                    if (!select.Contains("ErroIntegracaoPagamento"))
                    {
                        select.Append("PagamentoIntegracao.INT_PROBLEMA_INTEGRACAO ErroIntegracaoPagamento, ");
                        groupBy.Append("PagamentoIntegracao.INT_PROBLEMA_INTEGRACAO, ");

                        SetarJoinsPagamentoIntegracao(joins, filtrosPesquisa);
                    }
                    break;

                case "CodContaContabil":
                    if (!select.Contains(" CodContaContabil, "))
                    {
                        select.Append("XMLNotaFiscalContabilizacao.NFC_COD_CONTA_CONTABIL CodContaContabil, ");
                        groupBy.Append("XMLNotaFiscalContabilizacao.NFC_COD_CONTA_CONTABIL, ");

                        SetarJoinsXMLNotaFiscalContabilizacao(joins);
                    }
                    break;

                case "DataMigracaoNFFormatada":
                    if (!select.Contains(" DataMigracaoNF, "))
                    {
                        select.Append("NFe.NF_DATA_RECEBIMENTO DataMigracaoNF, ");
                        groupBy.Append("NFe.NF_DATA_RECEBIMENTO, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (joins.Contains(" CargaEntrega "))
                where.Append(" and CargaEntrega.CEN_COLETA = 0 ");

            where.Append(" and NFe.NF_ATIVA = 1 ");

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
            {
                where.Append($" and Carga.EMP_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
            {
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.DataVencimentoInicio.HasValue && filtrosPesquisa.DataVencimentoInicio.Value != DateTime.MinValue)
            {
                where.Append($" and Titulo.TIT_DATA_VENCIMENTO >= '{filtrosPesquisa.DataVencimentoInicio.Value.ToString(pattern)}'");

                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataVencimentoFim.HasValue && filtrosPesquisa.DataVencimentoFim.Value != DateTime.MinValue)
            {
                where.Append($" and Titulo.TIT_DATA_VENCIMENTO < '{filtrosPesquisa.DataVencimentoFim.Value.ToString(pattern)}'");

                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataIntegracaoPagamentoInicio.HasValue && filtrosPesquisa.DataIntegracaoPagamentoInicio.Value != DateTime.MinValue)
            {
                where.Append($" and PagamentoIntegracao.INT_DATA_INTEGRACAO >= '{filtrosPesquisa.DataIntegracaoPagamentoInicio.Value.ToString(pattern)}'");

                SetarJoinsPagamentoIntegracao(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataIntegracaoPagamentoFim.HasValue && filtrosPesquisa.DataIntegracaoPagamentoFim.Value != DateTime.MinValue)
            {
                where.Append($" PagamentoIntegracao.INT_DATA_INTEGRACAO < '{filtrosPesquisa.DataIntegracaoPagamentoFim.Value.ToString(pattern)}'");

                SetarJoinsPagamentoIntegracao(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataEmissaoNFInicio.HasValue && filtrosPesquisa.DataEmissaoNFInicio.Value != DateTime.MinValue)
                where.Append($" and NFe.NF_DATA_EMISSAO >= '{filtrosPesquisa.DataEmissaoNFInicio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoNFFim.HasValue && filtrosPesquisa.DataEmissaoNFFim != DateTime.MinValue)
                where.Append($" and NFe.NF_DATA_EMISSAO < '{filtrosPesquisa.DataEmissaoNFFim.Value.AddDays(1).ToString(pattern)}'");
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}

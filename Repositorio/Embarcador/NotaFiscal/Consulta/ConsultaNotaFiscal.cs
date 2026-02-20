using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.NotaFiscal
{
    sealed class ConsultaNotaFiscal : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe>
    {
        #region Construtores

        public ConsultaNotaFiscal(bool somenteRegistrosDistintos) : base(tabela: "T_XML_NOTA_FISCAL as NFe", somenteRegistrosDistintos: somenteRegistrosDistintos) { }

        #endregion Construtores

        #region Métodos Privados

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.Append(" left join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NFe.NFX_CODIGO ");
        }

        private void SetarJoinsCanalEntrega(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" CanalEntrega "))
                joins.Append(" left join T_CANAL_ENTREGA CanalEntrega on CanalEntrega.CNE_CODIGO = Pedido.CNE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
            {
                joins.Append(" left join T_CARGA as Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO and Carga.CAR_CARGA_FECHADA = 1 ");

                if (filtrosPesquisa.NotasFiscaisSemCarga.HasValue)
                    joins.Append($"and Carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}) ");
            }
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" Recebedor "))
                joins.Append(" left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = NFe.CLI_CODIGO_RECEBEDOR ");
        }

        private void SetarJoinsCanhoto(StringBuilder joins)
        {
            if (!joins.Contains(" Canhoto "))
                joins.Append(" left join T_CANHOTO_NOTA_FISCAL Canhoto on Canhoto.NFX_CODIGO = NFe.NFX_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS as CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO and CargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsCargaDaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" CargaDaEntrega "))
                joins.Append(" left join T_CARGA CargaDaEntrega on CargaDaEntrega.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CargaEntregaPedido "))
                joins.Append(" left join T_CARGA_ENTREGA_PEDIDO as CargaEntregaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsCFOP(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP as CFOP on CFOP.CFO_CODIGO = CargaPedido.CFO_CODIGO ");
        }

        private void SetarJoinsCategoriaDestinatarioNota(StringBuilder joins)
        {
            SetarJoinsDestinatarioNota(joins);

            if (!joins.Contains(" CategoriaDestinatarioNF "))
                joins.Append(" left join T_CATEGORIA_PESSOA as CategoriaDestinatarioNF on CategoriaDestinatarioNF.CTP_CODIGO = DestinatarioNF.CTP_CODIGO ");
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

        private void SetarJoinsDestinatarioPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
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

        private void SetarJoinsEmpresa(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

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

        private void SetarJoinsFilial(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

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

        private void SetarJoinsModeloVeicular(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsPaisDestinatarioPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsDestinatarioPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" Pais "))
                joins.Append(" left join T_PAIS Pais on Pais.PAI_CODIGO = Destinatario.PAI_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
            {
                joins.Append(" left join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");

                if (filtrosPesquisa.NotasFiscaisSemCarga.HasValue)
                    joins.Append($" and Pedido.PED_SITUACAO <> {(int)SituacaoPedido.Cancelado} ");
            }
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

        private void SetarJoinsRotaFrete(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsPedido(joins, filtrosPesquisa);

            if (!joins.Contains(" RotaFrete "))
                joins.Append(" left join T_ROTA_FRETE RotaFrete on RotaFrete.ROF_CODIGO = Pedido.ROF_CODIGO ");
        }

        private void SetarJoinsRotaFreteCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" RotaFreteCarga "))
                joins.Append(" left join T_ROTA_FRETE RotaFreteCarga on RotaFreteCarga.ROF_CODIGO = Carga.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA as TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO as TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsVeiculoCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" VeiculoCarga "))
                joins.Append(" left join T_VEICULO VeiculoCarga ON VeiculoCarga.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsVendedor(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
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

        private void SetarJoinsNotaFiscalMiro(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" NotaMiro "))
                joins.Append(" left join T_XML_NOTA_FISCAL_MIRO NotaMiro on NotaMiro.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            if (!select.Contains("Codigo, "))
            {
                select.Append("NFe.NFX_CODIGO Codigo, ");

                if (!groupBy.Contains("NFe.NFX_CODIGO,"))
                    groupBy.Append("NFe.NFX_CODIGO, ");
            }

            switch (propriedade)
            {

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa,"))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO GrupoPessoa, ");
                        groupBy.Append("GrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoa(joins, filtrosPesquisa);
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

                case "MetroCubico":
                    if (!select.Contains("MetroCubico, "))
                    {
                        select.Append("Pedido.PED_CUBAGEM_TOTAL MetroCubico, ");
                        groupBy.Append("Pedido.PED_CUBAGEM_TOTAL, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "MetroCubicoNFe":
                    if (!select.Contains(" MetroCubicoNFe, "))
                    {
                        select.Append("NFe.NF_METROS_CUBICOS MetroCubicoNFe, ");
                        groupBy.Append("NFe.NF_METROS_CUBICOS, ");
                    }
                    break;

                case "CapacidadeVeiculo":
                    if (!select.Contains("CapacidadeVeiculo"))
                    {
                        select.Append("VeiculoCarga.VEI_CAP_KG CapacidadeVeiculo, ");
                        groupBy.Append("VeiculoCarga.VEI_CAP_KG, ");

                        SetarJoinsVeiculoCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "Pedido":
                    if (!select.Contains(" Pedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "PesoPedido":
                    if (!select.Contains(" PesoPedido, "))
                    {
                        select.Append(@"
                            (
		                        SELECT SUM(_produtoPedido.PRP_PESO_UNITARIO * _produtoPedido.PRP_QUANTIDADE)
		                        FROM T_PEDIDO_PRODUTO _produtoPedido
		                        LEFT JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _produtoPedido.PED_CODIGO 
		                        WHERE _cargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO
	                        ) PesoPedido, "
                        );

                        if (!groupBy.Contains("PedidoNotaFiscal.CPE_CODIGO,"))
                            groupBy.Append("PedidoNotaFiscal.CPE_CODIGO, ");
                    }
                    break;

                case "ValorPedido":
                    if (!select.Contains(" ValorPedido, "))
                    {
                        select.Append(@"
                            (
		                        SELECT SUM(_produtoPedido.PRP_VALOR_PRODUTO * _produtoPedido.PRP_QUANTIDADE)
		                        FROM T_PEDIDO_PRODUTO _produtoPedido
		                        LEFT JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _produtoPedido.PED_CODIGO 
		                        WHERE _cargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO
	                        ) ValorPedido, "
                        );

                        if (!groupBy.Contains("PedidoNotaFiscal.CPE_CODIGO,"))
                            groupBy.Append("PedidoNotaFiscal.CPE_CODIGO, ");
                    }
                    break;

                case "NaturezaNF":
                    if (!select.Contains(" NaturezaNF, "))
                    {
                        select.Append("CanalEntrega.CNE_DESCRICAO NaturezaNF, ");
                        groupBy.Append("CanalEntrega.CNE_DESCRICAO, ");

                        SetarJoinsCanalEntrega(joins, filtrosPesquisa);
                    }
                    break;

                case "Itinerario":
                    if (!select.Contains(" Itinerario, "))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO Itinerario, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");

                        SetarJoinsRotaFrete(joins, filtrosPesquisa);
                    }
                    break;

                case "RotaCarga":
                    if (!select.Contains(" RotaCarga, "))
                    {
                        select.Append("RotaFreteCarga.ROF_DESCRICAO RotaCarga, ");
                        groupBy.Append("RotaFreteCarga.ROF_DESCRICAO, ");

                        SetarJoinsRotaFreteCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataPedido":
                case "HoraPedido":
                    if (!select.Contains(" DataHoraPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataHoraPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "DataCarga":
                    if (!select.Contains("DataCarga"))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains("DataEmissao"))
                    {
                        select.Append(@"
                            (
                                SELECT min(_cte.CON_DATA_AUTORIZACAO)
	                            FROM T_CTE _cte LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A'
	                        ) DataEmissao, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "DataNotaFiscal":
                    if (!select.Contains("DataNotaFiscal"))
                    {
                        select.Append("NFe.NF_DATA_EMISSAO DataNotaFiscal, ");
                        groupBy.Append("NFe.NF_DATA_EMISSAO, ");
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

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins, filtrosPesquisa);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga"))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "CodigoEmpresa":
                    if (!select.Contains(" CodigoEmpresa, "))
                    {
                        select.Append("Empresa.EMP_CODIGO_INTEGRACAO CodigoEmpresa, ");
                        groupBy.Append("Empresa.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsEmpresa(joins, filtrosPesquisa);
                    }
                    break;

                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresaSemFormato, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJEmpresaSemFormato, ");
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

                case "CNPJRemetente":
                    if (!select.Contains("CNPJRemetenteSemFormato"))
                    {
                        select.Append("EmitenteNF.CLI_CGCCPF CNPJRemetenteSemFormato, EmitenteNF.CLI_FISJUR TipoPessoaRemetente, ");
                        groupBy.Append("EmitenteNF.CLI_CGCCPF, EmitenteNF.CLI_FISJUR, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "CodigoRemetente":
                    if (!select.Contains(" CodigoRemetente, "))
                    {
                        select.Append("EmitenteNF.CLI_CODIGO_INTEGRACAO CodigoRemetente, ");
                        groupBy.Append("EmitenteNF.CLI_CODIGO_INTEGRACAO, ");

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

                case "EnderecoRemetente":
                    if (!select.Contains(" EnderecoRemetente, "))
                    {
                        select.Append("isnull(EmitenteNF.CLI_ENDERECO, '') + ', ' + isnull(EmitenteNF.CLI_NUMERO, '') + ', ' + isnull(EmitenteNF.CLI_BAIRRO, '') EnderecoRemetente, ");
                        groupBy.Append("EmitenteNF.CLI_ENDERECO, EmitenteNF.CLI_NUMERO, EmitenteNF.CLI_BAIRRO, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "GrupoRemetente":
                    if (!select.Contains(" GrupoRemetente, "))
                    {
                        select.Append("GrupoPessoaEmitenteNF.GRP_DESCRICAO GrupoRemetente, ");
                        groupBy.Append("GrupoPessoaEmitenteNF.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaEmitenteNota(joins);
                    }
                    break;

                case "CategoriaRemetente":
                    if (!select.Contains(" CategoriaRemetente, "))
                    {
                        select.Append("CategoriaEmitenteNF.CTP_DESCRICAO CategoriaRemetente, ");
                        groupBy.Append("CategoriaEmitenteNF.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaEmitenteNota(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("LocalidadeEmitenteNF.LOC_DESCRICAO + '-' + LocalidadeEmitenteNF.UF_SIGLA Origem, ");
                        groupBy.Append("LocalidadeEmitenteNF.LOC_DESCRICAO, LocalidadeEmitenteNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeEmitenteNota(joins);
                    }
                    break;

                case "CodigoDestinatario":
                    if (!select.Contains(" CodigoDestinatario, "))
                    {
                        select.Append("DestinatarioNF.CLI_CODIGO_INTEGRACAO CodigoDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "CNPJDestinatario":
                    if (!select.Contains("CNPJDestinatarioSemFormato"))
                    {
                        select.Append("DestinatarioNF.CLI_CGCCPF CNPJDestinatarioSemFormato, DestinatarioNF.CLI_FISJUR TipoPessoaDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_CGCCPF, DestinatarioNF.CLI_FISJUR, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("DestinatarioNF.CLI_NOME Destinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_NOME, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "EnderecoDestinatario":
                    if (!select.Contains(" EnderecoDestinatario, "))
                    {
                        select.Append("isnull(DestinatarioNF.CLI_ENDERECO, '') + ', ' + isnull(DestinatarioNF.CLI_NUMERO, '') + ', ' + isnull(DestinatarioNF.CLI_BAIRRO, '') EnderecoDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_ENDERECO, DestinatarioNF.CLI_NUMERO, DestinatarioNF.CLI_BAIRRO, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "TelefoneDestinatario":
                    if (!select.Contains(" TelefoneDestinatario, "))
                    {
                        select.Append("DestinatarioNF.CLI_FONE TelefoneDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_FONE, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "EmailDestinatario":
                    if (!select.Contains(" EmailDestinatario, "))
                    {
                        select.Append("DestinatarioNF.CLI_EMAIL EmailDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_EMAIL, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "GrupoDestinatario":
                    if (!select.Contains(" GrupoDestinatario, "))
                    {
                        select.Append("GrupoPessoaDestinatarioNF.GRP_DESCRICAO GrupoDestinatario, ");
                        groupBy.Append("GrupoPessoaDestinatarioNF.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaDestinatarioNota(joins);
                    }
                    break;

                case "CategoriaDestinatario":
                    if (!select.Contains(" CategoriaDestinatario, "))
                    {
                        select.Append("CategoriaDestinatarioNF.CTP_DESCRICAO CategoriaDestinatario, ");
                        groupBy.Append("CategoriaDestinatarioNF.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaDestinatarioNota(joins);
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

                case "Restricoes":
                    if (!select.Contains(" Restricoes,"))
                    {
                        select.Append(@"
                            SUBSTRING((
                                SELECT ', ' + _restricaoEntrega.REE_DESCRICAO
                                FROM T_RESTRICAO_ENTREGA _restricaoEntrega 
                                JOIN T_CLIENTE_RESTRICAO_DESCARGA _clienteRestricaoDescarga ON _clienteRestricaoDescarga.REE_CODIGO = _restricaoEntrega.REE_CODIGO
                                JOIN T_CLIENTE_DESCARGA _clienteDescarga ON _clienteDescarga.CLD_CODIGO = _clienteRestricaoDescarga.CLD_CODIGO
                                WHERE _clienteDescarga.CLI_CGCCPF = NFe.CLI_CODIGO_DESTINATARIO FOR XML PATH('')), 3, 1000
	                        ) Restricoes, "
                        );

                        if (!groupBy.Contains("NFe.CLI_CODIGO_DESTINATARIO, "))
                            groupBy.Append("NFe.CLI_CODIGO_DESTINATARIO, ");
                    }
                    break;

                case "NomeFantasiaExpedidor":
                    if (!select.Contains(" NomeFantasiaExpedidor, "))
                    {
                        select.Append("ExpedidorNF.CLI_NOMEFANTASIA NomeFantasiaExpedidor, ");
                        groupBy.Append("ExpedidorNF.CLI_NOMEFANTASIA, ");

                        SetarJoinsExpedidorNota(joins);
                    }
                    break;

                case "CodigoSapExpedidor":
                    if (!select.Contains(" CodigoSapExpedidor, "))
                    {
                        select.Append("ExpedidorNF.CLI_CODIGO_SAP CodigoSapExpedidor, ");
                        groupBy.Append("ExpedidorNF.CLI_CODIGO_SAP, ");

                        SetarJoinsExpedidorNota(joins);
                    }
                    break;

                case "Produto":
                    if (!select.Contains(" Produto,"))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO Produto, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "CodigoProduto":
                    if (!select.Contains("CodigoProduto"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProduto, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "CodigocEAN":
                    if (!select.Contains("CodigocEAN"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO_CEAN CodigocEAN, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO_CEAN, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "UnidadeComercial":
                    if (!select.Contains("UnidadeComercial"))
                    {
                        select.Append("ProdutoXMLNotaFiscal.XFP_UNIDADE_MEDIDA UnidadeComercial, ");
                        groupBy.Append("ProdutoXMLNotaFiscal.XFP_UNIDADE_MEDIDA, ");

                        SetarJoinsProdutoXmlNotaFiscal(joins);
                    }
                    break;

                case "QuantidadeProduto":
                    if (!select.Contains("QuantidadeProduto"))
                    {
                        select.Append("sum(ProdutoXMLNotaFiscal.XFP_QUANTIDADE) QuantidadeProduto, ");

                        SetarJoinsProdutoXmlNotaFiscal(joins);
                    }
                    break;

                case "QuantidadeTotalProduto":
                    if (!select.Contains("QuantidadeTotalProduto"))
                    {
                        select.Append("(SELECT SUM(XFP_QUANTIDADE) FROM T_XML_NOTA_FISCAL_PRODUTO WHERE NFX_CODIGO = NFe.NFX_CODIGO) QuantidadeTotalProduto, ");

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append("NFe.NFX_CODIGO, ");
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append("NFe.NF_NUMERO NotasFiscais, ");
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

                case "ValorNFe":
                    if (!select.Contains("ValorNFe"))
                        select.Append("min(NFe.NF_VALOR) ValorNFe, ");
                    break;

                case "ChaveNFe":
                    if (!select.Contains("ChaveNFe"))
                    {
                        select.Append("NFe.NF_CHAVE ChaveNFe, ");
                        groupBy.Append("NFe.NF_CHAVE, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                        select.Append("min(NFe.NF_PESO) Peso, ");
                    break;

                case "Volumes":
                    if (!select.Contains(" Volumes, "))
                        select.Append("min(NFe.NF_VOLUMES) Volumes, ");
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP, "))
                    {
                        SetarJoinsCFOP(joins);
                        select.Append(@"
                              coalesce(SUBSTRING(
                                            (
                                              SELECT 
                                                DISTINCT ', ' + CAST(
                                                  _cfop.CFO_CFOP AS NVARCHAR(20)
                                                ) 
                                              FROM 
                                                T_CTE _cte 
                                                LEFT JOIN T_CFOP _cfop on _cfop.CFO_CODIGO = _cte.CFO_CODIGO 
                                                LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
                                              WHERE 
                                                _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO 
                                                and _cte.CON_STATUS = 'A' FOR XML PATH('')
                                            ), 
                                            3, 
                                            1000
                                          ), CAST(CFOP.CFO_CFOP as varchar(4)), NFe.NF_CFOP) CFOP, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                        if (!groupBy.Contains("CFOP.CFO_CFOP"))
                            groupBy.Append(" CFOP.CFO_CFOP, ");
                        if (!groupBy.Contains("NFe.NF_CFOP"))
                            groupBy.Append(" NFe.NF_CFOP, ");
                    }
                    break;

                case "Frete":
                    if (!select.Contains(" Frete, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_FRETE) Frete, ");
                    break;

                case "ValorComponentes":
                    if (!select.Contains(" ValorComponentes, "))
                        select.Append("min(PedidoNotaFiscal.PNF_TOTAL_COMPONENTES) ValorComponentes, ");
                    break;

                case "FreteTotalSemImposto":
                    if (!select.Contains(" Frete, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_FRETE) Frete, ");

                    if (!select.Contains(" ValorComponentes, "))
                        select.Append("min(PedidoNotaFiscal.PNF_TOTAL_COMPONENTES) ValorComponentes, ");
                    break;

                case "CSTIBSCBS":
                    if (!select.Contains(" CSTIBSCBS, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CST_IBSCBS CSTIBSCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CST_IBSCBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "ClassificacaoTributariaIBSCBS":
                    if (!select.Contains(" ClassificacaoTributariaIBSCBS, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CLASSIFICACAO_TRIBUTARIA_IBSCBS ClassificacaoTributariaIBSCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CLASSIFICACAO_TRIBUTARIA_IBSCBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "BaseCalculoIBSCBS":
                    if (!select.Contains(" BaseCalculoIBSCBS, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_BASE_CALCULO_IBSCBS) BaseCalculoIBSCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_BASE_CALCULO_IBSCBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "AliquotaIBSEstadual":
                    if (!select.Contains(" AliquotaIBSEstadual, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_ALIQUOTA_IBS_ESTADUAL) AliquotaIBSEstadual, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_ALIQUOTA_IBS_ESTADUAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "PercentualReducaoIBSEstadual":
                    if (!select.Contains(" PercentualReducaoIBSEstadual, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_IBS_ESTADUAL) PercentualReducaoIBSEstadual, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_IBS_ESTADUAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "ValorIBSEstadual":
                    if (!select.Contains(" ValorIBSEstadual, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_IBS_ESTADUAL) ValorIBSEstadual, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_VALOR_IBS_ESTADUAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "AliquotaIBSMunicipal":
                    if (!select.Contains(" AliquotaIBSMunicipal, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_ALIQUOTA_IBS_MUNICIPAL) AliquotaIBSMunicipal, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_ALIQUOTA_IBS_MUNICIPAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "PercentualReducaoIBSMunicipal":
                    if (!select.Contains(" PercentualReducaoIBSMunicipal, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_IBS_MUNICIPAL) PercentualReducaoIBSMunicipal, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_IBS_MUNICIPAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "ValorIBSMunicipal":
                    if (!select.Contains(" ValorIBSMunicipal, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_IBS_MUNICIPAL) ValorIBSMunicipal, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_VALOR_IBS_MUNICIPAL, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "AliquotaCBS":
                    if (!select.Contains(" AliquotaCBS, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_ALIQUOTA_CBS) AliquotaCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_ALIQUOTA_CBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "PercentualReducaoCBS":
                    if (!select.Contains(" PercentualReducaoCBS, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_CBS) PercentualReducaoCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_PERCENTUAL_REDUCAO_CBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "ValorCBS":
                    if (!select.Contains(" ValorCBS, "))
                    {
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_CBS) ValorCBS, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_VALOR_CBS, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "AliquotaICMS":
                    if (!select.Contains(" AliquotaICMS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_PERCENTUAL_ALICOTA) AliquotaICMS, ");
                    break;

                case "ICMS":
                case "ValorICMSST":
                    if (!select.Contains(" _ICMS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_ICMS) _ICMS, ");

                    if (!select.Contains(" CST, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CST CST, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CST, ");
                    }
                    break;

                case "CST":
                    if (!select.Contains(" CST, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CST CST, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CST, ");
                    }
                    break;

                case "ValorISS":
                case "ValorISSRetido":
                    if (!select.Contains(" ValorISSRetido, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_RETENCAO_ISS) ValorISSRetido, ");

                    if (!select.Contains(" _ValorISS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_ISS) _ValorISS, ");
                    break;

                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_PERCENTUAL_ALICOTA_ISS) AliquotaISS, ");
                    break;

                case "TotalReceber":
                    if (!select.Contains(" Frete, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_FRETE) Frete, ");

                    if (!select.Contains(" ValorComponentes, "))
                        select.Append("min(PedidoNotaFiscal.PNF_TOTAL_COMPONENTES) ValorComponentes, ");

                    if (!select.Contains(" ValorISSRetido, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_RETENCAO_ISS) ValorISSRetido, ");

                    if (!select.Contains(" _ValorISS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_ISS) _ValorISS, ");

                    if (!select.Contains(" _ICMS, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_ICMS) _ICMS, ");

                    if (!select.Contains(" CST, "))
                    {
                        select.Append("PedidoNotaFiscal.PNF_CST CST, ");
                        groupBy.Append("PedidoNotaFiscal.PNF_CST, ");
                    }
                    break;

                case "FreteEmbarcador":
                    if (!select.Contains(" FreteEmbarcador, "))
                        select.Append("min(NFe.NF_VALOR_FRETE) FreteEmbarcador, ");
                    break;

                case "FreteTabelaFrete":
                    if (!select.Contains(" FreteTabelaFrete, "))
                        select.Append("min(PedidoNotaFiscal.PNF_VALOR_FRETE_TABELA_FRETE) FreteTabelaFrete, ");
                    break;

                case "ValorICMS":
                    if (!select.Contains(" ValorICMS, "))
                    {
                        select.Append(@"
                            (SELECT sum(_cte.CON_VAL_ICMS)
	                            FROM T_CTE _cte 
	                            LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO and _cte.CON_STATUS = 'A'
	                        ) ValorICMS, "
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

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_MOTORISTAS Motorista, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_MOTORISTAS, ");

                        SetarJoinsCargaDadosSumarizados(joins, filtrosPesquisa);
                    }
                    break;

                //case "Motorista":
                //    if (!select.Contains(" Motorista, "))
                //    {
                //        select.Append(@"
                //            SUBSTRING((
                //          SELECT DISTINCT ', ' + CTeMotorista.CMO_NOME_MOTORISTA 
                //          FROM T_CTE CTe 
                //          INNER JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal on CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO 
                //                INNER JOIN T_CTE_MOTORISTA CTeMotorista on CTeMotorista.CON_CODIGO = CTe.CON_CODIGO
                //          WHERE CTeXMLNotaFiscal.NFX_CODIGO = NFe.NFX_CODIGO and CTe.CON_STATUS = 'A' FOR XML PATH('')), 3, 1000
                //         ) Motorista, "
                //        );

                //        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                //            groupBy.Append(" NFe.NFX_CODIGO, ");
                //    }
                //    break;

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

                case "ValorMercadoriaTotalCTe":
                    if (!select.Contains("ValorMercadoriaTotalCTe"))
                    {
                        select.Append(@"
                            (SELECT min(_cte.CON_VALOR_TOTAL_MERC)
	                            FROM T_CTE _cte 
	                            LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A'
	                        ) ValorMercadoriaTotalCTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "FreteTotalCTe":
                    if (!select.Contains("FreteTotalCTe"))
                    {
                        select.Append(@"
                            (SELECT sum(_cte.CON_VALOR_RECEBER)
	                            FROM T_CTE _cte 
	                            LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A'
	                        ) FreteTotalCTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "FreteTotalLiquidoCTe":
                    if (!select.Contains("FreteTotalLiquidoCTe"))
                    {
                        select.Append(@"
                            (SELECT sum(_cte.CON_VALOR_FRETE)
	                            FROM T_CTE _cte 
	                            LEFT JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
	                            WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A'
	                        ) FreteTotalLiquidoCTe, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "Fatura":
                    if (!select.Contains(" Fatura, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT DISTINCT ', ' + CAST((CASE WHEN _fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR _fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN _fatura.FAT_NUMERO ELSE _fatura.FAT_NUMERO_FATURA_INTEGRACAO END) AS NVARCHAR(20))
		                        FROM T_DOCUMENTO_FATURAMENTO _documentoFataramento
		                        inner join T_CTE _cte on _cte.CON_CODIGO = _documentoFataramento.CON_CODIGO
		                        inner join t_fatura_documento _faturaDocumento on  _faturaDocumento.DFA_CODIGO = _documentoFataramento.DFA_CODIGO
		                        inner JOIN T_FATURA _fatura on _fatura.FAT_CODIGO = _faturaDocumento.FAT_CODIGO 
		                        inner JOIN T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                        WHERE _ctexmlnotafiscal.NFX_CODIGO = NFe.NFX_CODIGO  and _cte.CON_STATUS = 'A' FOR XML PATH('')
                            ), 3, 1000) Fatura, "
                        );

                        if (!groupBy.Contains("NFe.NFX_CODIGO"))
                            groupBy.Append(" NFe.NFX_CODIGO, ");
                    }
                    break;

                case "NumeroPallet":
                    if (!select.Contains("NumeroPallet"))
                        select.Append("min(NFe.NF_QUANTIDADE_PALLETS) NumeroPallet, ");
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

                case "CEPDestinatario":
                    if (!select.Contains(" CEPDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CEP CEPDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CEP, ");

                        SetarJoinsDestinatarioPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "PaisDestinatario":
                    if (!select.Contains(" PaisDestinatario, "))
                    {
                        select.Append("Pais.PAI_NOME PaisDestinatario, ");
                        groupBy.Append("Pais.PAI_NOME, ");

                        SetarJoinsPaisDestinatarioPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "PrevisaoEntregaPedido":
                    if (!select.Contains(" PrevisaoEntregaPedido, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA PrevisaoEntregaPedido, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "DataRealizadaEntrega":
                    if (!select.Contains(" DataRealizadaEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_FIM_ENTREGA DataRealizadaEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_FIM_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                        SetarJoinsCargaDaEntrega(joins);
                    }
                    break;

                case "Atendimentos":
                    if (!select.Contains(" Atendimentos, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT ', ' + CAST(_chamados.CHA_NUMERO AS NVARCHAR(20))
		                        FROM T_CHAMADOS _chamados 
		                        LEFT JOIN
                                    T_CARGA_ENTREGA _cargaEntrega 
                                        on _cargaEntrega.CEN_CODIGO = _chamados.CEN_CODIGO        
		                        LEFT JOIN
			                        T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido
				                        on _cargaEntregaPedido.CEN_CODIGO = _cargaEntrega.CEN_CODIGO
		                        LEFT JOIN
			                        T_CARGA_PEDIDO _cargaPedido
				                        on _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO
                                WHERE
                                    PedidoNotaFiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO     
		                        FOR XML PATH('')), 3, 1000
	                        ) Atendimentos, "
                        );

                        if (!groupBy.Contains("PedidoNotaFiscal.CPE_CODIGO"))
                            groupBy.Append(" PedidoNotaFiscal.CPE_CODIGO, ");
                    }
                    break;

                case "MotivosAtendimentos":
                    if (!select.Contains(" MotivosAtendimentos, "))
                    {
                        select.Append(@"
                            SUBSTRING((
		                        SELECT ', ' + _motivoChamado.MCH_DESCRICAO
                                FROM T_MOTIVO_CHAMADA _motivoChamado 
                                WHERE _motivoChamado.MCH_CODIGO IN (
                                    SELECT DISTINCT _chamados.MCH_CODIGO
		                            FROM T_CHAMADOS _chamados 
		                            LEFT JOIN
                                        T_CARGA_ENTREGA _cargaEntrega 
                                            on _cargaEntrega.CEN_CODIGO = _chamados.CEN_CODIGO        
		                            LEFT JOIN
			                            T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido
				                            on _cargaEntregaPedido.CEN_CODIGO = _cargaEntrega.CEN_CODIGO
		                            LEFT JOIN
			                            T_CARGA_PEDIDO _cargaPedido
				                            on _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO
                                    WHERE
                                        PedidoNotaFiscal.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                )
		                        FOR XML PATH('')), 3, 1000
	                        ) MotivosAtendimentos, "
                        );

                        if (!groupBy.Contains("PedidoNotaFiscal.CPE_CODIGO"))
                            groupBy.Append(" PedidoNotaFiscal.CPE_CODIGO, ");
                    }
                    break;

                case "SituacaoNotaFiscal":
                    if (!select.Contains(" SituacaoNotaFiscal, "))
                    {
                        select.Append(@"
                            (
                                select top (1) _notaFiscalSituacao.NFS_DESCRICAO
						 	      from T_NOTA_FISCAL_SITUACAO _notaFiscalSituacao
						 	     where _notaFiscalSituacao.NFS_CODIGO = NFe.NFS_CODIGO
                            ) SituacaoNotaFiscal, "
                        );

                        groupBy.Append("NFe.NFS_CODIGO, ");
                    }
                    break;

                case "DescricaoSituacaoEntrega":
                    if (!select.Contains(" SituacaoEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_SITUACAO SituacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_SITUACAO, ");

                        SetarJoinsCargaEntrega(joins);
                        SetarJoinsCargaDaEntrega(joins);
                    }
                    break;

                case "SLADocumento":
                    if (!select.Contains(" SLADocumento, "))
                    {
                        select.Append(@"
                            (SELECT
                                CASE 
                                    WHEN CargaEntrega.CEN_DATA_FIM_ENTREGA <= Pedido.PED_PREVISAO_ENTREGA THEN 1
                                    ELSE 0
                                END
                            ) SLADocumento, "
                        );

                        groupBy.Append("CargaEntrega.CEN_DATA_FIM_ENTREGA, Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                        SetarJoinsCargaEntrega(joins);
                        SetarJoinsCargaDaEntrega(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins, filtrosPesquisa);
                    }
                    break;

                case "NomeVendedor":
                    if (!select.Contains(" NomeVendedor, "))
                    {
                        select.Append("Vendedor.FUN_NOME NomeVendedor, ");
                        groupBy.Append("Vendedor.FUN_NOME, ");

                        SetarJoinsVendedor(joins, filtrosPesquisa);
                    }
                    break;

                case "TelefoneVendedor":
                    if (!select.Contains(" TelefoneVendedor, "))
                    {
                        select.Append("Vendedor.FUN_FONE TelefoneVendedor, ");
                        groupBy.Append("Vendedor.FUN_FONE, ");

                        SetarJoinsVendedor(joins, filtrosPesquisa);
                    }
                    break;

                case "KMFilialCliente":
                    if (!select.Contains(" KMFilialCliente, "))
                    {
                        select.Append("Carga.CAR_DISTANCIA KMFilialCliente, ");
                        groupBy.Append("Carga.CAR_DISTANCIA, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "MesPrevisaoEntregaPedido":
                    if (!select.Contains(" MesPrevisaoEntregaPedido, "))
                    {
                        select.Append("(SELECT DATEPART(month, Pedido.PED_PREVISAO_ENTREGA)) MesPrevisaoEntregaPedido, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "SemanaPrevisaoEntregaPedido":
                    if (!select.Contains(" SemanaPrevisaoEntregaPedido, "))
                    {
                        select.Append(@"
                            (SELECT
                                CASE                                                   
                                    WHEN DATEPART(DAY, Pedido.PED_PREVISAO_ENTREGA) < 21 THEN
						                ((DATEPART(DAY, Pedido.PED_PREVISAO_ENTREGA))/7) + 1
					                ELSE
						                (DATEPART(DAY, Pedido.PED_PREVISAO_ENTREGA))/7
				                END
                            ) SemanaPrevisaoEntregaPedido, "
                        );

                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "NumeroRemessaEspelho":
                    if (!select.Contains(" NumeroRemessaEspelho, "))
                    {
                        select.Append("NFe.NF_NUMERO_DT NumeroRemessaEspelho, ");
                        groupBy.Append(" NFe.NF_NUMERO_DT, ");
                    }
                    break;

                case "ClassificacaoNFe":
                case "ClassificacaoNFeDescricao":
                    if (!select.Contains(" ClassificacaoNFe, "))
                    {
                        select.Append("NFe.NF_CLASSIFICACAO_NFE ClassificacaoNFe, ");
                        groupBy.Append("NFe.NF_CLASSIFICACAO_NFE, ");
                    }
                    break;

                case "QuantidadeVolumes":
                    if (!select.Contains(" QuantidadeVolumes, "))
                    {
                        select.Append("NFe.NF_VOLUMES QuantidadeVolumes, ");
                        groupBy.Append("NFe.NF_VOLUMES, ");
                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains(" NumeroPedidoCliente, "))
                    {
                        select.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE NumeroPedidoCliente, ");
                        groupBy.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("Carga.CAR_SITUACAO Situacao, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DescricaoSituacaoNotaFiscalEntrega":
                    SetarSelect("SituacaoNotaFiscalEntrega", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("UltimaSituacaoEntregaDevolucao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "SituacaoNotaFiscalEntrega":
                    if (!select.Contains(" SituacaoNotaFiscalEntrega, "))
                    {
                        select.Append("NFe.NF_SITUACAO_ENTREGA SituacaoNotaFiscalEntrega, ");
                        groupBy.Append("NFe.NF_SITUACAO_ENTREGA, ");
                    }
                    break;

                case "UltimaSituacaoEntregaDevolucao":
                    if (!select.Contains(" UltimaSituacaoEntregaDevolucao, "))
                    {
                        select.Append("NFe.NF_ULTIMA_SITUACAO_ENTREGA_DEVOLUCAO UltimaSituacaoEntregaDevolucao, ");
                        groupBy.Append("NFe.NF_ULTIMA_SITUACAO_ENTREGA_DEVOLUCAO, ");
                    }
                    break;

                case "DataSituacaoNotaFiscalEntrega":
                case "DataSituacaoNotaFiscalEntregaFormatada":
                    if (!select.Contains(" DataSituacaoNotaFiscalEntrega, "))
                    {
                        select.Append("NFe.NF_DATA_NOTA_FISCAL_SITUACAO DataSituacaoNotaFiscalEntrega, ");
                        groupBy.Append("NFe.NF_DATA_NOTA_FISCAL_SITUACAO, ");
                    }
                    break;

                case "DataDigitalizacaoCanhoto":
                case "DataDigitalizacaoCanhotoFormatada":
                    if (!select.Contains(" DataDigitalizacaoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_DATA_DIGITALIZACAO DataDigitalizacaoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_DATA_DIGITALIZACAO, ");

                        SetarJoinsCanhoto(joins);
                    }
                    break;

                case "SituacaoDigitalizacaoCanhoto":
                case "DescricaoSituacaoDigitalizacaoCanhoto":
                    if (!select.Contains(" SituacaoDigitalizacaoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO SituacaoDigitalizacaoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO, ");

                        SetarJoinsCanhoto(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("Recebedor.CLI_NOME Recebedor, ");
                        groupBy.Append("Recebedor.CLI_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "ObservacaoPedido":
                    if (!select.Contains(" ObservacaoPedido, "))
                    {
                        select.Append("Pedido.PED_OBSERVACAO ObservacaoPedido, ");

                        if (!groupBy.Contains("Pedido.PED_OBSERVACAO"))
                            groupBy.Append("Pedido.PED_OBSERVACAO, ");

                        SetarJoinsPedido(joins, filtrosPesquisa);
                    }
                    break;

                case "QuantidadeCaixa":
                    if (!select.Contains(" QuantidadeCaixa, "))
                    {
                        select.Append("sum(ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA) QuantidadeCaixa, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "QuantidadePecas":
                    SetarSelect("QuantidadeProduto", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("QuantidadeCaixa", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "IdAgrupador":
                    if (!select.Contains(" IdAgrupador, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("     SELECT ', ' +  ");
                        select.Append("         CAST(_cargaPreAgrupamentoAgrupador.PAA_CODIGO_AGRUPAMENTO AS VARCHAR(20))");
                        select.Append("     FROM T_CARGA_PEDIDO _cargaPedido  ");
                        select.Append("         LEFT JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO ");
                        select.Append("         LEFT JOIN T_CARGA_PRE_AGRUPAMENTO _cargaPreAgrupamento ON _cargaPreAgrupamento.CAR_CODIGO = _carga.CAR_CODIGO  ");
                        select.Append("         LEFT JOIN T_CARGA_PRE_AGRUPAMENTO_AGRUPADOR _cargaPreAgrupamentoAgrupador ON _cargaPreAgrupamentoAgrupador.PAA_CODIGO = _cargaPreAgrupamento.PAA_CODIGO ");
                        select.Append("     WHERE _cargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
                        select.Append("     FOR XML PATH('') ");
                        select.Append("), 3, 1000) IdAgrupador, ");

                        if (!groupBy.Contains("PedidoNotaFiscal.CPE_CODIGO"))
                            groupBy.Append("PedidoNotaFiscal.CPE_CODIGO, ");

                        SetarJoinsPedidoNotaFiscal(joins);
                    }
                    break;

                case "CodigoSAPDestinatario":
                    if (!select.Contains(" CodigoSAPDestinatario, "))
                    {
                        select.Append("DestinatarioNF.CLI_CODIGO_SAP CodigoSAPDestinatario, ");
                        groupBy.Append("DestinatarioNF.CLI_CODIGO_SAP, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "NumeroDestino":
                    if (!select.Contains(" NumeroDestino, "))
                    {
                        select.Append("DestinatarioNF.CLI_NUMERO NumeroDestino, ");
                        groupBy.Append("DestinatarioNF.CLI_NUMERO, ");

                        SetarJoinsDestinatarioNota(joins);
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

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("LocalidadeEmitenteNF.UF_SIGLA UFOrigem, ");

                        if (!groupBy.Contains("LocalidadeEmitenteNF.UF_SIGLA"))
                            groupBy.Append("LocalidadeEmitenteNF.UF_SIGLA, ");

                        SetarJoinsLocalidadeEmitenteNota(joins);
                    }
                    break;

                case "DataEntregaFormatada":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append(@"(
                            SELECT TOP 1 
                                cargaEntrega.CEN_DATA_FIM_ENTREGA 
                            FROM T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido 
                                LEFT JOIN T_CARGA_ENTREGA cargaEntrega ON cargaEntrega.CEN_CODIGO = cargaEntregaPedido.CEN_CODIGO 
                            WHERE cargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                AND cargaEntrega.CEN_COLETA = 0 
                                AND cargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO
                        ) DataEntrega, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO"))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CAR_CODIGO"))
                            groupBy.Append("CargaPedido.CAR_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "DataPrevisaoCargaEntregaFormatada":
                    if (!select.Contains(" DataPrevisaoCargaEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA DataPrevisaoCargaEntrega, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA"))
                            groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");

                        SetarJoinsCargaEntrega(joins);
                        SetarJoinsCargaDaEntrega(joins);
                    }
                    break;

                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                    {
                        select.Append($"(select sum(Componente.NFC_VALOR_COMPONENTE) from T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE Componente where Componente.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO and Componente.CFR_CODIGO = {codigoDinamico}) {propriedade}, ");

                        if (!groupBy.Contains("PedidoNotaFiscal.PNF_CODIGO,"))
                            groupBy.Append("PedidoNotaFiscal.PNF_CODIGO, ");
                    }
                    break;

                case "NaturezaOP":
                    if (!select.Contains(" NaturezaOP, "))
                    {
                        select.Append("NFe.NF_NATUREZA_OP NaturezaOP, ");
                        groupBy.Append("NFe.NF_NATUREZA_OP,");
                    }
                    break;

                case "RecebedorCarga":
                    if (!select.Contains(" RecebedorCarga, "))
                    {
                        select.Append("RecebedorCarga.CLI_NOME RecebedorCarga, ");
                        groupBy.Append("RecebedorCarga.CLI_NOME, ");

                        SetarJoinsCargaPedidoRecebedor(joins);
                    }
                    break;

                case "ExpedidorCarga":
                    if (!select.Contains(" ExpedidorCarga, "))
                    {
                        select.Append("ExpedidorCarga.CLI_NOME ExpedidorCarga, ");
                        groupBy.Append("ExpedidorCarga.CLI_NOME, ");

                        SetarJoinsCargaPedidoExpedidor(joins);
                    }
                    break;

                case "CodigoInteragracaoMigo":
                    if (!select.Contains(" CodigoInteragracaoMigo, "))
                    {
                        select.Append("NotaMigo.XNM_CODIGO_IDENTIFICACAO CodigoInteragracaoMigo, ");
                        groupBy.Append("NotaMigo.XNM_CODIGO_IDENTIFICACAO, ");

                        SetarJoinsNotaFiscalMigo(joins);
                    }
                    break;

                case "DataMigo":
                case "DataMigoFormatado":
                    if (!select.Contains(" DataMigo, "))
                    {
                        select.Append("NotaMigo.XNM_DATA DataMigo, ");
                        groupBy.Append("NotaMigo.XNM_DATA, ");

                        SetarJoinsNotaFiscalMigo(joins);
                    }
                    break;
                case "CodigoInteragracaoMiro":
                    if (!select.Contains(" CodigoInteragracaoMiro, "))
                    {
                        select.Append("NotaMiro.XNI_CODIGO_IDENTIFICACAO CodigoInteragracaoMiro, ");
                        groupBy.Append("NotaMiro.XNI_CODIGO_IDENTIFICACAO, ");

                        SetarJoinsNotaFiscalMiro(joins);
                    }
                    break;

                case "DataMiro":
                case "DataMiroFormatado":
                    if (!select.Contains(" DataMiro, "))
                    {
                        select.Append("NotaMiro.XNI_DATA DataMiro, ");
                        groupBy.Append("NotaMiro.XNI_DATA, ");

                        SetarJoinsNotaFiscalMiro(joins);
                    }
                    break;

                case "Transbordo":
                    if (!select.Contains("Transbordo"))
                    {
                        select.Append("(CASE Carga.CAR_CARGA_TRANSBORDO WHEN 1 THEN 'Sim' ELSE 'Não' END) Transbordo, ");
                        groupBy.Append("Carga.CAR_CARGA_TRANSBORDO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;
                case "PesoLiquido":
                    if (!select.Contains(" PesoLiquido, "))
                    {
                        select.Append("NFe.NF_PESO_LIQUIDO PesoLiquido, ");
                        groupBy.Append("NFe.NF_PESO_LIQUIDO, ");
                    }
                    break;
                case "NumeroPedidoNFe":
                    if (!select.Contains(" NumeroPedidoNFe, "))
                    {
                        select.Append("NFe.NF_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoNFe, ");
                        groupBy.Append("NFe.NF_NUMERO_PEDIDO_EMBARCADOR, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsPedidoNotaFiscal(joins);

            if (joins.Contains(" CargaEntrega "))
                where.Append(" and (CargaDaEntrega.CAR_CARGA_AGRUPADA = 0 and (CargaEntrega.CAR_CODIGO IS NULL OR CargaEntrega.CEN_COLETA = 0) OR (CargaDaEntrega.CAR_CARGA_AGRUPADA = 1 AND (CargaEntrega.CAR_CODIGO IS NULL OR CargaEntrega.CEN_COLETA = 0))) ");

            where.Append(" and NFe.NF_ATIVA = 1 ");

            if (filtrosPesquisa.NotasFiscaisSemCarga.HasValue)
            {
                where.Append($@"
                    and {(filtrosPesquisa.NotasFiscaisSemCarga.Value ? "not " : "")}exists(
                            select _carga.CAR_CODIGO
                              from T_CARGA _carga
                              join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CAR_CODIGO = _carga.CAR_CODIGO
                             where _cargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO
                               and _carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada})
                               and (_carga.CAR_CARGA_TRANSBORDO is null or _carga.CAR_CARGA_TRANSBORDO = 0)
                        ) "
                );
            }

            if (filtrosPesquisa.CargaTransbordo.HasValue)
            {
                if (filtrosPesquisa.CargaTransbordo.Value)
                    where.Append($" and Carga.CAR_CARGA_TRANSBORDO = 1 ");
                else
                    where.Append($" and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0) ");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
            {
                where.Append($" and Carga.EMP_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosCarga.Count > 0)
            {
                where.Append($" and (Carga.CAR_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosCarga)}) ");
                where.Append("      OR EXISTS (SELECT 0 ");
                where.Append("                   FROM T_CARGA_PEDIDO CargaPedido ");
                where.Append("             INNER JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO PedSubContratacao ");
                where.Append("                     ON PedSubContratacao.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
                where.Append("             INNER JOIN T_CTE_TERCEIRO CteTerceiro ");
                where.Append("                     ON CteTerceiro.CPS_CODIGO = PedSubContratacao.CPS_CODIGO ");
                where.Append("             INNER JOIN T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL_NFE CteTerceiroDocAdicNFe ");
                where.Append("                     ON CteTerceiroDocAdicNFe.CPS_CODIGO = CteTerceiro.CPS_CODIGO ");
                where.Append($"                 WHERE CargaPedido.CAR_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosCarga)}) AND CteTerceiroDocAdicNFe.NFX_CODIGO = NFe.NFX_CODIGO) )");


                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosOrigem.Count > 0)
            {
                where.Append($" and LocalidadeEmitenteNF.LOC_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosOrigem)})");

                SetarJoinsLocalidadeEmitenteNota(joins);
            }

            if (filtrosPesquisa.CodigosDestino.Count > 0)
            {
                where.Append($" and LocalidadeDestinatarioNF.LOC_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosDestino)})");

                SetarJoinsLocalidadeDestinatarioNota(joins);
            }

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append($" and NFe.NF_NUMERO >= {filtrosPesquisa.NumeroInicial} ");

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append($" and NFe.NF_NUMERO <= {filtrosPesquisa.NumeroFinal} ");

            if (filtrosPesquisa.CodigosGrupoPessoas.Count > 0)
            {
                where.Append($" and Carga.GRP_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoas)})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor > 0)
            {
                where.Append($" and Carga.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor} ");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
            {
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
            {
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CpfCnpjClienteFornecedor > 0d)
                where.Append($" and (NFe.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjClienteFornecedor} OR NFe.CLI_CODIGO_DESTINATARIO = {filtrosPesquisa.CpfCnpjClienteFornecedor}) ");

            if (filtrosPesquisa.CpfCnpjsRemetente.Count > 0)
                where.Append($" and NFe.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.CpfCnpjsRemetente)})");

            if (filtrosPesquisa.CpfCnpjsDestinatario.Count > 0)
                where.Append($" and NFe.CLI_CODIGO_DESTINATARIO in ({string.Join(", ", filtrosPesquisa.CpfCnpjsDestinatario)})");

            if (filtrosPesquisa.CpfCnpjsExpedidor.Count > 0)
                where.Append($" and NFe.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CpfCnpjsExpedidor)})");

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where.Append($" and NFe.NF_DATA_EMISSAO >= '{filtrosPesquisa.DataInicialEmissao.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where.Append($" and NFe.NF_DATA_EMISSAO < '{filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.DataInicialEmissaoCarga != DateTime.MinValue || filtrosPesquisa.DataFinalEmissaoCarga != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialEmissaoCarga != DateTime.MinValue)
                    where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicialEmissaoCarga.ToString(pattern)}'");

                if (filtrosPesquisa.DataFinalEmissaoCarga != DateTime.MinValue)
                    where.Append($" and Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataFinalEmissaoCarga.AddDays(1).ToString(pattern)}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataInicialEmissaoCTe != DateTime.MinValue || filtrosPesquisa.DataFinalEmissaoCTe != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialEmissaoCTe != DateTime.MinValue)
                    where.Append($" and Carga.CAR_DATA_FINALIZACAO_EMISSAO >= '{filtrosPesquisa.DataInicialEmissaoCTe.ToString(pattern)}'");

                if (filtrosPesquisa.DataFinalEmissaoCTe != DateTime.MinValue)
                    where.Append($" and Carga.CAR_DATA_FINALIZACAO_EMISSAO <= '{filtrosPesquisa.DataFinalEmissaoCTe.AddDays(1).ToString(pattern)}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataInicialPrevisaoEntregaPedido != DateTime.MinValue || filtrosPesquisa.DataFinalPrevisaoEntregaPedido != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialPrevisaoEntregaPedido != DateTime.MinValue)
                    where.Append($" and Pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataInicialPrevisaoEntregaPedido.ToString("yyyyMMdd HH:mm")}'");

                if (filtrosPesquisa.DataFinalPrevisaoEntregaPedido != DateTime.MinValue)
                    where.Append($" and Pedido.PED_PREVISAO_ENTREGA <= '{filtrosPesquisa.DataFinalPrevisaoEntregaPedido.ToString("yyyyMMdd HH:mm")}'");

                SetarJoinsPedido(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataInicialInicioViagemPlanejada != DateTime.MinValue || filtrosPesquisa.DataFinalInicioViagemPlanejada != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialInicioViagemPlanejada != DateTime.MinValue)
                    where.Append($" and CargaDadosSumarizados.CDS_DATA_PREVISAO_INICIO_VIAGEM >= '{filtrosPesquisa.DataInicialInicioViagemPlanejada.ToString(pattern)}'");

                if (filtrosPesquisa.DataFinalInicioViagemPlanejada != DateTime.MinValue)
                    where.Append($" and CargaDadosSumarizados.CDS_DATA_PREVISAO_INICIO_VIAGEM <= '{filtrosPesquisa.DataFinalInicioViagemPlanejada.AddDays(1).ToString(pattern)}'");

                SetarJoinsCargaDadosSumarizados(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.TipoLocalPrestacao != TipoLocalPrestacao.todos)
            {
                if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal)
                    where.Append(" and CargaPedido.LOC_CODIGO_ORIGEM = CargaPedido.LOC_CODIGO_DESTINO");
                else if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal)
                    where.Append(" and CargaPedido.LOC_CODIGO_ORIGEM <> CargaPedido.LOC_CODIGO_DESTINO");

                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.SituacaoFatura != null)
            {
                where.Append($" and Fatura.FAT_SITUACAO = {(int)filtrosPesquisa.SituacaoFatura}");

                SetarJoinsFatura(joins);
            }

            if (filtrosPesquisa.EstadosOrigem.Count > 0)
            {
                where.Append($" and LocalidadeEmitenteNF.UF_SIGLA IN ({string.Join(", ", from o in filtrosPesquisa.EstadosOrigem select "'" + o + "'")})");

                SetarJoinsLocalidadeEmitenteNota(joins);
            }

            if (filtrosPesquisa.EstadosDestino.Count > 0)
            {
                where.Append($" and LocalidadeDestinatarioNF.UF_SIGLA IN ({string.Join(", ", from o in filtrosPesquisa.EstadosDestino select "'" + o + "'")})");

                SetarJoinsLocalidadeDestinatarioNota(joins);
            }

            if (filtrosPesquisa.CodigosRestricoes.Count > 0)
            {
                where.Append($@"
                    and exists (
                        select top(1) _clienteDescarga.CLI_CGCCPF
                          from T_CLIENTE_DESCARGA _clienteDescarga
                          join T_CLIENTE_RESTRICAO_DESCARGA _clienteRestricaoDescarga on _clienteRestricaoDescarga.CLD_CODIGO = _clienteDescarga.CLD_CODIGO
                         where _clienteDescarga.CLI_CGCCPF = NFe.CLI_CODIGO_DESTINATARIO
                           and _clienteRestricaoDescarga.REE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosRestricoes)})
                    ) "
                );
            }

            if (filtrosPesquisa.CodigosRestricoes.Count > 0)
            {
                where.Append($" and LocalidadeDestinatarioNF.UF_SIGLA IN ({string.Join(", ", filtrosPesquisa.CodigosRestricoes)})");

                SetarJoinsLocalidadeDestinatarioNota(joins);
            }

            if (filtrosPesquisa.CodigosMotorista.Count > 0)
            {
                where.Append($@"
                    and exists (
                        select top(1) _cargaMotorista.CAR_MOTORISTA
                          from T_CARGA_MOTORISTA _cargaMotorista
                         where _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                           and _cargaMotorista.CAR_MOTORISTA in ({string.Join(", ", filtrosPesquisa.CodigosMotorista)})
                    )"
                );

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosNotasFiscais.Count > 0)
                where.Append($" and PedidoNotaFiscal.CPE_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosNotasFiscais)})");

            if (filtrosPesquisa.TiposCTe.Count > 0)
            {
                where.Append($@"
                    and NFe.NFX_CODIGO in (
                            select _ctexmlnotafiscal.NFX_CODIGO
                              from T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal 
                             inner join T_CTE _cte on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
                             where _cte.CON_STATUS = 'A' and _cte.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ToString("D")))})
                        )"
                );
            }

            if (filtrosPesquisa.QuantidadeVolumesInicial > 0)
                where.Append($" and NFe.NF_VOLUMES >= {filtrosPesquisa.QuantidadeVolumesInicial}");

            if (filtrosPesquisa.QuantidadeVolumesFinal > 0)
                where.Append($" and NFe.NF_VOLUMES <= {filtrosPesquisa.QuantidadeVolumesFinal}");

            if (filtrosPesquisa.ClassificacaoNFe != ClassificacaoNFe.Todos)
            {
                if (filtrosPesquisa.ClassificacaoNFe == ClassificacaoNFe.SemClassificacao)
                    where.Append($" and NFe.NF_CLASSIFICACAO_NFE IS NULL");
                else
                    where.Append($" and NFe.NF_CLASSIFICACAO_NFE = {(int)filtrosPesquisa.ClassificacaoNFe}");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
            {
                where.Append($" and Pedido.PED_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}'");

                SetarJoinsPedido(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.Situacoes?.Count > 0)
            {
                where.Append($" and Carga.CAR_SITUACAO in ({string.Join(", ", filtrosPesquisa.Situacoes.Select(o => (int)o))})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and (1 = 0 ");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    where.Append($" or Carga.CAR_SITUACAO = 13");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    where.Append($" or Carga.CAR_SITUACAO = 18");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    where.Append($" or (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    where.Append($" or (Carga.CAR_TODOS_CTES_COM_MERCANTE != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    where.Append($" or Carga.CAR_SITUACAO = 15");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    where.Append($" or (Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and ");
                    where.Append($@" exists (
                    select
                        cargacte5_.CCT_CODIGO 
                    from
                        T_CARGA_CTE cargactes4_,
                        T_CARGA_CTE cargacte5_ 
                    left outer join
                        T_CTE conhecimen6_ 
                            on cargacte5_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                    where
                        Carga.CAR_CODIGO=cargactes4_.CAR_CODIGO 
                        and cargactes4_.CCT_CODIGO=cargacte5_.CCT_CODIGO 
                        and  not (exists (select
                            ctesvmmult7_.CSM_CODIGO 
                        from
                            T_CTE_SVM_MULTIMODAL ctesvmmult7_ 
                        inner join
                            T_CTE conhecimen8_ 
                                on ctesvmmult7_.CON_CODIGO_SVM=conhecimen8_.CON_CODIGO 
                        inner join
                            T_CTE conhecimen9_ 
                                on ctesvmmult7_.CON_CODIGO_MULTIMODAL=conhecimen9_.CON_CODIGO 
                        where
                            conhecimen8_.CON_STATUS='A'
                            and conhecimen9_.CON_TIPO_CTE=0
                            and (conhecimen9_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                            or (conhecimen9_.CON_CODIGO is null) 
                            and (conhecimen6_.CON_CODIGO is null)))))
                    )");

                }
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    where.Append($" or Carga.CAR_SITUACAO = 15 or Carga.CAR_SITUACAO = 6 or Carga.CAR_PROBLEMA_CTE = 1");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    where.Append($" or (Carga.CAR_SITUACAO = 11 and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_FATURADOS = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3)) and ");
                    where.Append($" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))) ");
                }
                where.Append($" )");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.SituacoesEntrega?.Count > 0 || filtrosPesquisa.StatusEntrega.HasValue)
            {
                if (filtrosPesquisa.SituacoesEntrega?.Count <= 0)
                    filtrosPesquisa.SituacoesEntrega = new System.Collections.Generic.List<SituacaoEntrega> { filtrosPesquisa.StatusEntrega.Value };

                where.Append($" and CargaEntrega.CEN_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacoesEntrega.Select(o => (int)o))})");

                SetarJoinsCargaEntrega(joins);
                SetarJoinsCargaDaEntrega(joins);
            }

            if (filtrosPesquisa.PossuiExpedidor == 1)
                where.Append($" and NFe.CLI_CODIGO_EXPEDIDOR is not null ");
            else if (filtrosPesquisa.PossuiExpedidor == 0)
                where.Append($" and NFe.CLI_CODIGO_EXPEDIDOR is null ");

            if (filtrosPesquisa.PossuiRecebedor == 1)
                where.Append($" and NFe.CLI_CODIGO_RECEBEDOR is not null ");
            else if (filtrosPesquisa.PossuiRecebedor == 0)
                where.Append($" and NFe.CLI_CODIGO_RECEBEDOR is null ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.FiltroDinamico))
            {
                bool filtroPossuiLetras = filtrosPesquisa.FiltroDinamico.ObterSomenteNumerosELetrasComEspaco().ObterSomenteNumeros().Length == 0;

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.FiltroDinamico))
                {
                    SetarJoinsDestinatarioNota(joins);
                    SetarJoinsEmitenteNota(joins);
                    SetarJoinsLocalidadeDestinatarioNota(joins);

                    where.Append(" AND (");

                    if (!filtroPossuiLetras)
                    {
                        int filtroNumeroNF = filtrosPesquisa.FiltroDinamico.ObterSomenteNumerosELetrasComEspaco().ToInt();
                        double filtroCNPJ = 0;

                        if (filtroNumeroNF == 0)
                        {
                            filtroCNPJ = filtrosPesquisa.FiltroDinamico.ObterSomenteNumerosELetrasComEspaco().ToDouble();

                            where.Append($" DestinatarioNF.CLI_CGCCPF = {filtroCNPJ} OR ");
                            where.Append($" EmitenteNF.CLI_CGCCPF = {filtroCNPJ} ");
                        }
                        else
                            where.Append($" NFe.NF_NUMERO = {filtroNumeroNF}");
                    }
                    else
                    {
                        where.Append($" DestinatarioNF.CLI_NOME LIKE '{filtrosPesquisa.FiltroDinamico}' OR ");
                        where.Append($" EmitenteNF.CLI_NOME LIKE '{filtrosPesquisa.FiltroDinamico}' OR ");
                        where.Append($" LocalidadeDestinatarioNF.LOC_DESCRICAO LIKE '{filtrosPesquisa.FiltroDinamico}' ");
                    }

                    where.Append(" )");
                }
            }

            if (filtrosPesquisa.NumeroCanhoto > 0)
            {
                SetarJoinsCanhoto(joins);
                where.Append($" AND Canhoto.CNF_NUMERO = {filtrosPesquisa.NumeroCanhoto} ");
            }

            if (filtrosPesquisa.DataPrevisaoCargaEntregaInicial != DateTime.MinValue)
            {
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.DataPrevisaoCargaEntregaInicial.ToString("yyyyMMdd HH:mm")}'");
                SetarJoinsCargaEntrega(joins);
                SetarJoinsCargaDaEntrega(joins);
            }

            if (filtrosPesquisa.DataPrevisaoCargaEntregaFinal != DateTime.MinValue)
            {
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.DataPrevisaoCargaEntregaFinal.ToString("yyyyMMdd HH:mm")}'");
                SetarJoinsCargaEntrega(joins);
                SetarJoinsCargaDaEntrega(joins);
            }

            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
            {
                where.Append($@" and (NFe.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}

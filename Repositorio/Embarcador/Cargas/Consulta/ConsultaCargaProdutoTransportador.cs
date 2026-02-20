using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaProdutoTransportador : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador>
    {
        #region Construtores

        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
        private readonly int codigoEmpresa;

        public ConsultaCargaProdutoTransportador(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa) : base(tabela: "T_CARGA_PEDIDO_PRODUTO as CargaPedidoProduto", true) 
        {
            this.tipoServicoMultisoftware = tipoServicoMultisoftware;
            this.codigoEmpresa = codigoEmpresa;
        }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append("join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append("join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoProduto "))
                joins.Append("inner join T_GRUPO_PRODUTO AS GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsJanelaCarregamento(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" JanelaCarregamento "))
                joins.Append("join T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento on JanelaCarregamento.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append("join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsProdutoEmbarcador(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoEmbarcador "))
                joins.Append("inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Destinatario "))
                joins.Append("join T_CLIENTE as Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append("join T_LOCALIDADES as LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        public void SetarJoinsRecebedorCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" RecebedorCargaPedido "))
                joins.Append(" LEFT JOIN T_CLIENTE RecebedorCargaPedido on RecebedorCargaPedido.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ");
        }

        public void SetarJoinsLocalidadeRecebedorCargaPedido(StringBuilder joins)
        {
            SetarJoinsRecebedorCargaPedido(joins);

            if (!joins.Contains(" LocalidadeRecebedorCargaPedido "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES LocalidadeRecebedorCargaPedido on LocalidadeRecebedorCargaPedido.LOC_CODIGO = RecebedorCargaPedido.LOC_CODIGO ");
        }

        public void SetarJoinsXMLNotaFiscalProduto(StringBuilder joins)
        {
            SetarJoinsProdutoEmbarcador(joins);

            if (!joins.Contains(" XMLNotaFiscalProduto "))
                joins.Append(" left join T_XML_NOTA_FISCAL_PRODUTO XMLNotaFiscalProduto on XMLNotaFiscalProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO ");
        }

        public void SetarJoinsLocalidadeOrigem(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" LocalidadeOrigem "))
                joins.Append(" left join T_LOCALIDADES as LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = CargaPedido.LOC_CODIGO_ORIGEM ");
        }
        public void SetarJoinsLocalidadeDestino(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" LocalidadeDestino "))
                joins.Append(" left join T_LOCALIDADES as LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = CargaPedido.LOC_CODIGO_DESTINO ");
        }

        public void SetarJoinsXMLNotaFiscal(StringBuilder joins)
        {
            SetarJoinsXMLNotaFiscalProduto(joins);

            if (!joins.Contains(" XMLNotaFiscal "))
                joins.Append(" left join T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = XMLNotaFiscalProduto.NFX_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                        groupBy.Append("Carga.CAR_CODIGO, Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "UnidadeMedida":
                    if (!select.Contains(" UnidadeMedida"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_SIGLA_UNIDADE as UnidadeMedida, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO, ProdutoEmbarcador.PRO_SIGLA_UNIDADE, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "CodigoProdutoEmbarcador":
                    if (!select.Contains(" CodigoProdutoEmbarcador"))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR as CodigoProdutoEmbarcador, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO, ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo"))
                    {
                        select.Append("CargaPedidoProduto.CPP_CODIGO as Codigo, ");
                        groupBy.Append("CargaPedidoProduto.CPP_CODIGO, ");
                    }
                    break;

                case "CnpjTransportador":
                    if (!select.Contains(" CnpjTransportador"))
                    {
                        select.Append("Empresa.EMP_CNPJ as CnpjTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "DescricaoProduto":
                    if (!select.Contains(" DescricaoProduto"))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO as DescricaoProduto, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador"))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "PesoUnitario":
                    if (!select.Contains(" PesoUnitario"))
                    {
                        select.Append("CargaPedidoProduto.CPP_PESO_UNITARIO as PesoUnitario, ");
                        groupBy.Append("CargaPedidoProduto.CPP_PESO_UNITARIO, ");
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade"))
                    {
                        select.Append("SUM(CargaPedidoProduto.CPP_QUANTIDADE) as Quantidade, ");
                    }
                    break;

                case "RazaoSocialTransportador":
                    if (!select.Contains(" RazaoSocialTransportador"))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocialTransportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF as CPFCNPJDestinatario, Destinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME as Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "LocalidadeDestinatario":
                    if (!select.Contains(" LocalidadeDestinatario, "))
                    {
                        select.Append("(LocalidadeDestinatario.LOC_DESCRICAO + ' - ' + LocalidadeDestinatario.UF_SIGLA) as LocalidadeDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "EnderecoDestinatario":
                    if (!select.Contains(" EnderecoDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_ENDERECO EnderecoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_ENDERECO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "BairroDestinatario":
                    if (!select.Contains(" BairroDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_BAIRRO BairroDestinatario, ");
                        groupBy.Append("Destinatario.CLI_BAIRRO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NumeroEnderecoDestinatario":
                    if (!select.Contains(" NumeroEnderecoEntrega, "))
                    {
                        select.Append("Destinatario.CLI_NUMERO NumeroEnderecoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_NUMERO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ComplementoEnderecoDestinatario":
                    if (!select.Contains(" ComplementoEnderecoDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_COMPLEMENTO ComplementoEnderecoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_COMPLEMENTO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "SituacaoJanelaCarregamento":
                case "DescricaoSituacaoJanelaCarregamento":
                    if (!select.Contains(" SituacaoJanelaCarregamento, "))
                    {
                        select.Append("JanelaCarregamento.CJC_SITUACAO as SituacaoJanelaCarregamento, ");
                        groupBy.Append("JanelaCarregamento.CJC_SITUACAO, ");

                        SetarJoinsJanelaCarregamento(joins);
                    }
                    break;

                case "DataCarregamentoCarga":
                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamentoCarga"))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO as DataCarregamentoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TotalPallets":
                    if (!select.Contains(" TotalPallets"))
                    {
                        select.Append("(Pedido.PED_NUMERO_PALETES + Pedido.PED_NUMERO_PALETES_FRACIONADO) as TotalPallets, ");
                        groupBy.Append("Pedido.PED_NUMERO_PALETES, Pedido.PED_NUMERO_PALETES_FRACIONADO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CargaProdutoDescricao":
                    if (!select.Contains(" CargaProdutoDescricao"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR + ' / ' + ProdutoEmbarcador.GRP_DESCRICAO as CargaProdutoDescricao, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsCarga(joins);
                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "TipoFrete":
                    if (!select.Contains(" TipoFrete"))
                    {
                        select.Append(@"(SELECT CASE WHEN Pedido.PED_TIPO_TOMADOR = 0 THEN 'CIF' WHEN Pedido.PED_TIPO_TOMADOR = 3 THEN 'FOB' ELSE '' END) as TipoFrete, ");
                        groupBy.Append("Pedido.PED_TIPO_TOMADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "SequenciaRoteirizacao":
                    if (!select.Contains(" SequenciaRoteirizacao, "))
                    {
                        select.Append(" CargaPedido.PED_ORDEM_ENTREGA SequenciaRoteirizacao, ");
                        groupBy.Append("CargaPedido.PED_ORDEM_ENTREGA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "Bloco":
                    if (!select.Contains("Bloco"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_BLOCO FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as Bloco, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "OrdemCarregamento":
                    if (!select.Contains("OrdemCarregamento"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_ORDEM_CARREGAMENTO FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as OrdemCarregamento, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "OrdemEntrega":
                    if (!select.Contains("OrdemEntrega"))
                    {
                        select.Append("(SELECT top 1 BlocoCarregamento.BLC_ORDEM_ENTREGA FROM T_BLOCO_CARREGAMENTO BlocoCarregamento WHERE BlocoCarregamento.PED_CODIGO = Pedido.PED_CODIGO AND BlocoCarregamento.CRG_CODIGO = Carga.CRG_CODIGO) as OrdemEntrega, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains("GrupoProduto"))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO as GrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;

                case "CubagemPedido":
                    if (!select.Contains("CubagemPedido"))
                    {
                        select.Append("Pedido.PED_CUBAGEM_TOTAL as CubagemPedido, ");
                        groupBy.Append("Pedido.PED_CUBAGEM_TOTAL, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                case "CPFCNPJRecebedorFormatado":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_CGCCPF as CPFCNPJRecebedor, RecebedorCargaPedido.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_CGCCPF, RecebedorCargaPedido.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "RecebedorDescricao":
                    if (!select.Contains(" RecebedorDescricao, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_NOME RecebedorDescricao, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_NOME, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "LocalidadeRecebedor":
                    if (!select.Contains(" LocalidadeRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedorCargaPedido.LOC_DESCRICAO LocalidadeRecebedor, ");
                        groupBy.Append("LocalidadeRecebedorCargaPedido.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeRecebedorCargaPedido(joins);
                    }
                    break;

                case "EnderecoRecebedor":
                    if (!select.Contains(" EnderecoRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_ENDERECO EnderecoRecebedor, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_ENDERECO, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "BairroRecebedor":
                    if (!select.Contains(" BairroRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_BAIRRO BairroRecebedor, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_BAIRRO, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "NumeroEnderecoRecebedor":
                    if (!select.Contains(" NumeroEnderecoRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_NUMERO NumeroEnderecoRecebedor, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_NUMERO, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "ComplementoEnderecoRecebedor":
                    if (!select.Contains(" ComplementoEnderecoRecebedor, "))
                    {
                        select.Append("RecebedorCargaPedido.CLI_COMPLEMENTO ComplementoEnderecoRecebedor, ");
                        groupBy.Append("RecebedorCargaPedido.CLI_COMPLEMENTO, ");

                        SetarJoinsRecebedorCargaPedido(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CONCAT(LocalidadeOrigem.LOC_DESCRICAO,' - ', LocalidadeOrigem.UF_SIGLA) Origem, ");
                        groupBy.Append("LocalidadeOrigem.LOC_DESCRICAO, ");
                        groupBy.Append("LocalidadeOrigem.UF_SIGLA, ");

                        SetarJoinsLocalidadeOrigem(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("CONCAT(LocalidadeDestino.LOC_DESCRICAO,' - ', LocalidadeDestino.UF_SIGLA) Destino, ");
                        groupBy.Append("LocalidadeDestino.LOC_DESCRICAO, ");
                        groupBy.Append("LocalidadeDestino.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestino(joins);
                    }
                    break;

                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select.Append("XMLNotaFiscal.NF_NUMERO NotaFiscal, ");
                        groupBy.Append("XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            where.Append($" and (Carga.CAR_SITUACAO <> 13 AND Carga.CAR_SITUACAO <> 18) ");
            SetarJoinsCarga(joins);

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
            {
                where.Append($" and JanelaCarregamento.CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento}");

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.CodigoPedido > 0)
            {
                where.Append($" and Pedido.PED_CODIGO = {filtrosPesquisa.CodigoPedido}");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                where.Append($" and ProdutoEmbarcador.PRO_CODIGO = {filtrosPesquisa.CodigoProduto}");

                SetarJoinsProdutoEmbarcador(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                SetarJoinsEmpresa(joins);
            }

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                where.Append($" and Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjDestinatario}");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataInicial.HasValue)
            {
                where.Append($" and JanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd HH:mm:ss")}'");

                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.DataLimite.HasValue)
            {
                where.Append($" and JanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

                SetarJoinsJanelaCarregamento(joins);
            }

            string situacoesCargaNaoFaturadas = " in (5, 4, 2, 1, 6) ";

            if (filtrosPesquisa.SituacaoFaturada && !filtrosPesquisa.SituacaoNaoFaturada && !(filtrosPesquisa.Situacao.Count() > 0))
                where.Append($" and Carga.CAR_SITUACAO not {situacoesCargaNaoFaturadas}");
            else if (filtrosPesquisa.SituacaoNaoFaturada && !filtrosPesquisa.SituacaoFaturada && !(filtrosPesquisa.Situacao.Count() > 0))
                where.Append($" and Carga.CAR_SITUACAO {situacoesCargaNaoFaturadas}");
            else if (filtrosPesquisa.Situacao.Count() > 0)
            {
                where.Append($" and JanelaCarregamento.CJC_SITUACAO in ( " + string.Join(", ", filtrosPesquisa.Situacao.Select(o => (int)o)) + ") ");
                SetarJoinsJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)})");

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                where.Append($" and Empresa.EMP_CODIGO = {codigoEmpresa}");

                SetarJoinsEmpresa(joins);
            }
        }

        #endregion
    }
}

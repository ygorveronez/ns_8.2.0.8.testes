using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Canhotos
{
    sealed class ConsultaCanhoto : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto>
    {
        #region Construtores

        public ConsultaCanhoto() : base(tabela: "T_CANHOTO_NOTA_FISCAL as Canhoto") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsXmlNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" XmlNotaFiscal "))
                joins.Append("LEFT JOIN T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = Canhoto.NFX_CODIGO ");
        }

        private void SetarJoinsEmitente(StringBuilder joins)
        {
            if (!joins.Contains(" Emitente "))
                joins.Append("LEFT JOIN T_CLIENTE Emitente on Emitente.CLI_CGCCPF = Canhoto.CLI_CODIGO_EMITENTE ");
        }

        private void SetarJoinsTerceiroResponsavel(StringBuilder joins)
        {
            if (!joins.Contains(" TerceiroResponsavel "))
                joins.Append("LEFT JOIN T_CLIENTE TerceiroResponsavel on TerceiroResponsavel.CLI_CGCCPF = Canhoto.CLI_CGCCPF_TERCEIRO_RESPONSAVEL ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("LEFT JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = Canhoto.EMP_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append("LEFT JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Canhoto.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append("LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Canhoto.FIL_CODIGO ");
        }

        private void SetarJoinsLocalArmazenamentoCanhoto(StringBuilder joins)
        {
            if (!joins.Contains(" LocalArmazenamentoCanhoto "))
                joins.Append("LEFT JOIN T_LOCAL_ARMAZENAMENTO_CANHOTO LocalArmazenamentoCanhoto on LocalArmazenamentoCanhoto.LAC_CODIGO = Canhoto.LAC_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("LEFT JOIN T_CARGA Carga on Carga.CAR_CODIGO = Canhoto.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append("LEFT JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = Canhoto.PED_CODIGO ");
        }

        private void SetarJoinsTipoDeCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" TipoDeCarga "))
                joins.Append("LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("LEFT JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Canhoto.FUN_CODIGO ");
        }

        private void SetarJoinsLocalidadeEmitente(StringBuilder joins)
        {
            SetarJoinsEmitente(joins);
            if (!joins.Contains(" LocalidadeEmitente "))
                joins.Append("LEFT JOIN T_LOCALIDADES LocalidadeEmitente on LocalidadeEmitente.LOC_CODIGO = Emitente.LOC_CODIGO ");
        }

        private void SetarJoinsEstadoEmitente(StringBuilder joins)
        {
            SetarJoinsLocalidadeEmitente(joins);
            if (!joins.Contains(" EstadoEmitente "))
                joins.Append("LEFT JOIN T_UF EstadoEmitente on EstadoEmitente.UF_SIGLA = LocalidadeEmitente.UF_SIGLA ");
        }

        private void SetarJoinsEstadoDestinatario(StringBuilder joins)
        {
            SetarJoinsLocalidadeDestinatario(joins);
            if (!joins.Contains(" EstadoDestinatario "))
                joins.Append("LEFT JOIN T_UF EstadoDestinatario on EstadoDestinatario.UF_SIGLA = LocalidadeDestinatario.UF_SIGLA ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);
            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append("LEFT JOIN T_LOCALIDADES LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        private void SetarJoinsTomadorCTe(StringBuilder joins)
        {
            SetarJoinsCte(joins);
            if (!joins.Contains(" TomadorCTe "))
                joins.Append("LEFT JOIN T_CTE_PARTICIPANTE TomadorCTe on TomadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Remetente "))
                joins.Append("LEFT JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsGrupoPessoaEmitente(StringBuilder joins)
        {
            SetarJoinsEmitente(joins);
            if (!joins.Contains(" GrupoPessoaEmitente "))
                joins.Append("LEFT JOIN T_GRUPO_PESSOAS GrupoPessoaEmitente on GrupoPessoaEmitente.GRP_CODIGO = Emitente.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoaDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);
            if (!joins.Contains(" GrupoPessoaDestinatario "))
                joins.Append("LEFT JOIN T_GRUPO_PESSOAS GrupoPessoaDestinatario on GrupoPessoaDestinatario.GRP_CODIGO = Destinatario.GRP_CODIGO ");
        }

        private void SetarJoinsCteSubContratacao(StringBuilder joins)
        {
            if (!joins.Contains(" CTeSubContratacao "))
                joins.Append("LEFT JOIN T_CTE_TERCEIRO CTeSubContratacao on CTeSubContratacao.CPS_CODIGO = Canhoto.CPS_CODIGO ");
        }

        private void SetarJoinsCanhotoAvulso(StringBuilder joins)
        {
            if (!joins.Contains(" CanhotoAvulso "))
                joins.Append("LEFT JOIN T_CANHOTO_AVULSO CanhotoAvulso on CanhotoAvulso.CAV_CODIGO = Canhoto.CAV_CODIGO ");
        }

        private void SetarJoinsViewCTe(StringBuilder joins)
        {
            if (!joins.Contains(" ViewCTe "))
                joins.Append("LEFT JOIN V_CANHOTO_CTE ViewCTe on Canhoto.CNF_CODIGO = ViewCTe.CNF_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            SetarJoinsViewCTe(joins);
            if (!joins.Contains(" DocumentoFaturamento "))
                joins.Append("LEFT JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on DocumentoFaturamento.CON_CODIGO = ViewCTe.CON_CODIGO ");
        }

        private void SetarJoinsFatura(StringBuilder joins)
        {
            SetarJoinsViewCTe(joins);
            if (!joins.Contains(" Fatura "))
                joins.Append("LEFT JOIN T_FATURA Fatura on Fatura.COM_CODIGO = ViewCTe.CON_CODIGO ");
        }

        private void SetarJoinsCte(StringBuilder joins)
        {
            SetarJoinsViewCTe(joins);
            if (!joins.Contains(" CTe "))
                joins.Append("LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = ViewCTe.CON_CODIGO ");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Operador "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Operador on Operador.FUN_CODIGO = Carga.CAR_OPERADOR ");
        }

        private void SetarJoinsCargaCte(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append("LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = Canhoto.CCT_CODIGO ");
        }

        private void SetarJoinsMalote(StringBuilder joins)
        {
            if (!joins.Contains(" Malote "))
                joins.Append("LEFT JOIN T_MALOTE_CANHOTO Malote on Malote.MCA_CODIGO = Canhoto.MCA_CODIGO ");
        }

        private void SetarJoinsPedidoXmlNotaFiscal(StringBuilder joins)
        {
            SetarJoinsXmlNotaFiscal(joins);
            if (!joins.Contains(" PedidoXmlNotaFiscal "))
                joins.Append("LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on PedidoXmlNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCargaEntregaNotaFiscal(StringBuilder joins)
        {
            SetarJoinsPedidoXmlNotaFiscal(joins);
            if (!joins.Contains(" CargaEntregaNotaFiscal "))
                joins.Append("LEFT JOIN T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal on CargaEntregaNotaFiscal.PNF_CODIGO = PedidoXmlNotaFiscal.PNF_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Veiculo "))
                joins.Append("LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsUsuarioResponsavelDigitalizacao(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioResponsavelDigitalizacao "))
                joins.Append("LEFT JOIN T_FUNCIONARIO UsuarioResponsavelDigitalizacao on UsuarioResponsavelDigitalizacao.FUN_CODIGO = Canhoto.UDC_CODIGO ");
        }

        private void SetarJoinsUsuarioResponsavelLiberacaoPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioResponsavelLiberacaoPagamento "))
                joins.Append("LEFT JOIN T_FUNCIONARIO UsuarioResponsavelLiberacaoPagamento on UsuarioResponsavelLiberacaoPagamento.FUN_CODIGO = Canhoto.ULP_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Origem "))
                joins.Append(" LEFT JOIN T_LOCALIDADES Origem ON Origem.LOC_CODIGO = Pedido.LOC_CODIGO_ORIGEM ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Destino "))
                joins.Append(" LEFT JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = Canhoto.CPE_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Expedidor "))
                joins.Append(" LEFT JOIN T_CLIENTE Expedidor ON Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);
            SetarJoinsXmlNotaFiscal(joins);
            if (!joins.Contains(" Recebedor "))
                joins.Append("LEFT JOIN T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR OR Recebedor.CLI_CGCCPF = XmlNotaFiscal.CLI_CODIGO_RECEBEDOR ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Canhoto.CNF_CODIGO as Codigo, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "ChaveNF":
                    if (!select.Contains(" ChaveNF, "))
                    {
                        select.Append("XmlNotaFiscal.NF_CHAVE as ChaveNF, ");
                        groupBy.Append("XmlNotaFiscal.NF_CHAVE, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }
                    break;

                case "CNPJEmitente":
                case "CPFCNPJEmitenteFormatado":
                    if (!select.Contains(" CNPJEmitente, "))
                    {
                        select.Append("Emitente.CLI_CGCCPF as CNPJEmitente, Emitente.CLI_FISJUR as TipoEmitente, ");
                        groupBy.Append("Emitente.CLI_CGCCPF, Emitente.CLI_FISJUR, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "CNPJTerceiroResponsavel":
                case "CNPJTerceiroResponsavelFormatado":
                    if (!select.Contains(" CNPJTerceiroResponsavel, "))
                    {
                        select.Append("TerceiroResponsavel.CLI_CGCCPF as CNPJTerceiroResponsavel, TerceiroResponsavel.CLI_FISJUR as TipoTerceiroResponsavel,");
                        groupBy.Append("TerceiroResponsavel.CLI_CGCCPF, TerceiroResponsavel.CLI_FISJUR, ");

                        SetarJoinsTerceiroResponsavel(joins);
                    }
                    break;

                case "CNPJTransportador":
                case "CNPJTransportadorFormatado":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Empresa.EMP_CNPJ as CNPJTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF as CPFCNPJDestinatario, Destinatario.CLI_FISJUR as TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "DataEmissao":
                case "DescricaoDataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("Canhoto.CNF_DATA_EMISSAO as DataEmissao, ");
                        groupBy.Append("Canhoto.CNF_DATA_EMISSAO, ");
                    }
                    break;

                case "DataEnvioCanhoto":
                case "DescricaoDataEnvioCanhoto":
                    if (!select.Contains(" DataEnvioCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_DATA_ENVIO_CANHOTO as DataEnvioCanhoto, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENVIO_CANHOTO, ");
                    }
                    break;

                case "DataDigitalizacao":
                case "DataDigitalizacaoFormatada":
                    if (!select.Contains(" DataDigitalizacao, "))
                    {
                        select.Append("Canhoto.CNF_DATA_DIGITALIZACAO as DataDigitalizacao, ");
                        groupBy.Append("Canhoto.CNF_DATA_DIGITALIZACAO, ");
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

                case "Emitente":
                    if (!select.Contains(" Emitente, "))
                    {
                        select.Append("Emitente.CLI_NOME as Emitente, ");
                        groupBy.Append("Emitente.CLI_NOME, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "CpfCnpjRecebedorFormatado":
                case "CpfCnpjRecebedor":
                    if (!select.Contains(" CpfCnpjRecebedor, "))
                    {
                        select.Append("Recebedor.CLI_CGCCPF as CpfCnpjRecebedor, Recebedor.CLI_FISJUR as TipoRecebedor,  ");
                        groupBy.Append("Recebedor.CLI_CGCCPF, Recebedor.CLI_FISJUR, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("Recebedor.CLI_NOME as Recebedor, ");
                        groupBy.Append("Recebedor.CLI_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "CpfCnpjRemetenteFormatado":
                case "CpfCnpjRemetente":
                    if (!select.Contains(" CpfCnpjRemetente, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF as CpfCnpjRemetente, Remetente.CLI_FISJUR as TipoRemetente,  ");
                        groupBy.Append("Remetente.CLI_CGCCPF, Remetente.CLI_FISJUR, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("Remetente.CLI_NOME as Remetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        if (!select.Contains(" ModalidadeFrete, "))
                        {
                            select.Append("XmlNotaFiscal.NF_MODALIDADE_FRETE ModalidadeFrete, ");
                            groupBy.Append("XmlNotaFiscal.NF_MODALIDADE_FRETE, ");
                            SetarJoinsXmlNotaFiscal(joins);
                        }

                        select.Append("GrupoPessoaEmitente.GRP_DESCRICAO as GrupoPessoasEmitente, GrupoPessoaDestinatario.GRP_DESCRICAO as GrupoPessoasDestinatario, ");
                        groupBy.Append("GrupoPessoaEmitente.GRP_DESCRICAO, GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaDestinatario(joins);
                        SetarJoinsGrupoPessoaEmitente(joins);
                    }
                    break;

                case "ModalidadeFrete":
                case "DescricaoModalidadeFrete":
                    if (!select.Contains(" ModalidadeFrete, "))
                    {
                        select.Append("XmlNotaFiscal.NF_MODALIDADE_FRETE ModalidadeFrete, ");
                        groupBy.Append("XmlNotaFiscal.NF_MODALIDADE_FRETE, ");
                        SetarJoinsXmlNotaFiscal(joins);
                    }
                    break;

                case "LocalArmazenamentoCanhoto":
                    if (!select.Contains(" LocalArmazenamentoCanhoto, "))
                    {
                        select.Append("LocalArmazenamentoCanhoto.LAC_DESCRICAO as LocalArmazenamentoCanhoto, ");
                        groupBy.Append("LocalArmazenamentoCanhoto.LAC_DESCRICAO, ");

                        SetarJoinsLocalArmazenamentoCanhoto(joins);
                    }
                    break;

                case "NaturezaOP":
                    if (!select.Contains(" NaturezaOP, "))
                    {
                        select.Append("XmlNotaFiscal.NF_NATUREZA_OP as NaturezaOP, ");
                        groupBy.Append("XmlNotaFiscal.NF_NATUREZA_OP, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("Canhoto.CNF_NUMERO as Numero, ");
                        groupBy.Append("Canhoto.CNF_NUMERO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("CONVERT(DECIMAL(18,3), Canhoto.CNF_PESO) as PesoBruto, ");
                        groupBy.Append("Canhoto.CNF_PESO, ");
                    }
                    break;

                case "TipoCanhoto":
                case "DescricaoTipoCanhoto":
                    if (!select.Contains(" TipoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_TIPO_CANHOTO as TipoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_TIPO_CANHOTO, ");
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("XmlNotaFiscal.NF_SERIE as Serie, ");
                        groupBy.Append("XmlNotaFiscal.NF_SERIE, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }
                    break;

                case "SituacaoCanhoto":
                case "DescricaoSituacaoCanhoto":
                    if (!select.Contains(" SituacaoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_SITUACAO_CANHOTO as SituacaoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_SITUACAO_CANHOTO, ");
                    }
                    break;

                case "SituacaoDigitalizacaoCanhoto":
                case "DescricaoSituacaoDigitalizacaoCanhoto":
                    if (!select.Contains(" SituacaoDigitalizacaoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO as SituacaoDigitalizacaoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO, ");
                    }
                    break;

                case "TerceiroResponsavel":
                    if (!select.Contains(" TerceiroResponsavel, "))
                    {
                        select.Append("TerceiroResponsavel.CLI_NOME as TerceiroResponsavel, ");
                        groupBy.Append("TerceiroResponsavel.CLI_NOME, ");

                        SetarJoinsTerceiroResponsavel(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoDeCarga.TCG_DESCRICAO as TipoCarga, ");
                        groupBy.Append("TipoDeCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoDeCarga(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("ISNULL(Empresa.EMP_RAZAO, 'NÃO INFORMADO') as Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("CONVERT(DECIMAL(18, 2), Canhoto.CNF_VALOR) as Valor, ");
                        groupBy.Append("Canhoto.CNF_VALOR, ");
                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("Canhoto.CNF_OBSERVACAO as Justificativa, ");
                        groupBy.Append("Canhoto.CNF_OBSERVACAO, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CTe.CON_CHAVECTE as ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append(@"Veiculo.VEI_PLACA + ISNULL((SELECT DISTINCT ', ' + _veiculo.VEI_PLACA FROM T_VEICULO _veiculo JOIN T_CARGA_VEICULOS_VINCULADOS _veiculosVinculados ON _veiculosVinculados.VEI_CODIGO = _veiculo.VEI_CODIGO WHERE _veiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH ('')), '') as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, Carga.CAR_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_NOME FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                                                                                    join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO FOR XML PATH('')), 3, 1000) as Motorista, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "Frota":
                    if (!select.Contains(" Frota, "))
                    {
                        select.Append(@"Veiculo.VEI_NUMERO_FROTA + ', ' + SUBSTRING((SELECT DISTINCT ', ' + _veiculo.VEI_NUMERO_FROTA FROM T_VEICULO _veiculo
                                                                                     JOIN T_CARGA_VEICULOS_VINCULADOS _veiculosVinculados 
                                                                                     ON _veiculosVinculados.VEI_CODIGO = _veiculo.VEI_CODIGO WHERE _veiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH ('')), 3, 1000) as Frota, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_FROTA, Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTe.CON_NUM AS VARCHAR(100)) FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    left join V_CANHOTO_CTE ViewCTe on ViewCTe.CNF_CODIGO = CanhotoNotaFiscal.CNF_CODIGO
                                                                                    LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = ViewCTe.CON_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO FOR XML PATH('')), 3, 1000) as NumeroCTe, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "TelefoneMotorista":
                    if (!select.Contains(" TelefoneMotorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_FONE FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                            join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                                                                                            join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                                                                            WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO FOR XML PATH('')), 3, 1000) as TelefoneMotorista, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "ValorCTe":
                    if (!select.Contains(" ValorCTe, "))
                    {
                        select.Append(@"(SELECT TOP(1) CTe2.CON_VALOR_RECEBER FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscais on CanhotoNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscais.NFX_CODIGO
                                                                                    join T_CTE CTe2 on CTe2.con_codigo = CTeXMLNotasFiscais.CON_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO and CTe2.CON_CODIGO = CTe.CON_CODIGO) as ValorCTe, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, CTe.CON_CODIGO,  ");
                    }
                    break;

                case "Pacote":
                    if (!select.Contains(" Pacote, "))
                    {
                        select.Append("Canhoto.CNF_PACOTE_ARMAZENADO as Pacote, ");
                        groupBy.Append("Canhoto.CNF_PACOTE_ARMAZENADO, ");
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("Canhoto.CNF_POSICAO_NO_PACOTE as Posicao, ");
                        groupBy.Append("Canhoto.CNF_POSICAO_NO_PACOTE, ");
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("EstadoEmitente.UF_SIGLA as UFOrigem, ");
                        groupBy.Append("EstadoEmitente.UF_SIGLA, ");

                        SetarJoinsEstadoEmitente(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("EstadoDestinatario.UF_SIGLA as UFDestino, ");
                        groupBy.Append("EstadoDestinatario.UF_SIGLA, ");

                        SetarJoinsEstadoDestinatario(joins);
                    }
                    break;

                case "MotivoRejeicaoDigitalizacao":
                    if (!select.Contains(" MotivoRejeicaoDigitalizacao, "))
                    {
                        select.Append("ISNULL((SELECT TOP 1 _MotivoInconsistencia.CMI_DESCRICAO FROM T_INCONSISTENCIA_DIGITACAO_CANHOTO _InconsistenciaDigitacao JOIN T_MOTIVO_INCONSISTENCIA_DIGITACAO_CANHOTO _MotivoInconsistencia ON _InconsistenciaDigitacao.CMI_CODIGO =  _MotivoInconsistencia.CMI_CODIGO WHERE _InconsistenciaDigitacao.CNF_CODIGO = Canhoto.CNF_CODIGO ORDER BY _InconsistenciaDigitacao.CID_CODIGO DESC), '') as MotivoRejeicaoDigitalizacao, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "DescricaoSituacaoPagamento":
                case "SituacaoPagamentoCanhoto":
                    if (!select.Contains(" SituacaoPagamentoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_SITUACAO_PGTO_CANHOTO as SituacaoPagamentoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_SITUACAO_PGTO_CANHOTO, ");
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

                case "ObservacaoRecebimentoFisico":
                    if (!select.Contains(" ObservacaoRecebimentoFisico, "))
                    {
                        select.Append("Canhoto.CNF_OBSERVACAO_RECEBIMENTO_FISICO as ObservacaoRecebimentoFisico, ");
                        groupBy.Append("Canhoto.CNF_OBSERVACAO_RECEBIMENTO_FISICO, ");
                    }
                    break;

                case "OrigemDigitalizacao":
                case "DescricaoOrigemDigitalizacao":
                    if (!select.Contains(" OrigemDigitalizacao, "))
                    {
                        select.Append("Canhoto.CNF_ORIGEM_DIGITALIZACAO as OrigemDigitalizacao, ");
                        groupBy.Append("Canhoto.CNF_ORIGEM_DIGITALIZACAO, ");
                    }
                    break;

                case "DataConfirmacaoEntregaTransportador":
                case "DescricaoDataConfirmacaoEntregaTransportador":
                    if (!select.Contains(" DataConfirmacaoEntregaTransportador, "))
                    {
                        select.Append(@"Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE as DataConfirmacaoEntregaTransportador, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE, ");
                    }
                    break;

                case "DataConfirmacaoEntrega":
                case "DescricaoDataConfirmacaoEntrega":
                    if (!select.Contains(" DataConfirmacaoEntrega, "))
                    {
                        select.Append(@"(select TOP 1 cargaEntrega.CEN_DATA_ENTREGA from T_CARGA_ENTREGA cargaEntrega
                                        left join t_carga carga on carga.car_codigo = cargaEntrega.CAR_CODIGO
                                        left join T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido on cargaEntrega.cen_codigo = cargaEntregaPedido.CEN_CODIGO
                                        left join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = cargaEntregaPedido.CPE_CODIGO
                                        left join t_pedido _pedido on _pedido.ped_codigo = cargapedido.PED_CODIGO
                                        where _pedido.PED_CODIGO = Pedido.PED_CODIGO order by cargaEntrega.CEN_CODIGO desc) DataConfirmacaoEntrega, ");

                        groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataIntegracaoEntregaFormatada":
                    if (!select.Contains(" DataIntegracaoEntrega, "))
                    {
                        select.Append("Canhoto.CNF_DATA_INTEGRACAO_ENTREGA as DataIntegracaoEntrega, ");
                        groupBy.Append("Canhoto.CNF_DATA_INTEGRACAO_ENTREGA, ");
                    }
                    break;

                case "ResponsavelDigitalizacao":
                    if (!select.Contains(" ResponsavelDigitalizacao, "))
                    {
                        select.Append("UsuarioResponsavelDigitalizacao.FUN_NOME as ResponsavelDigitalizacao, ");
                        groupBy.Append("UsuarioResponsavelDigitalizacao.FUN_NOME, ");

                        SetarJoinsUsuarioResponsavelDigitalizacao(joins);
                    }
                    break;

                case "ResponsavelEnvioFisico":
                    if (!select.Contains(" ResponsavelEnvioFisico, "))
                    {
                        select.Append("Usuario.FUN_NOME as ResponsavelEnvioFisico, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "ResponsavelLiberacaoPagamento":
                    if (!select.Contains(" ResponsavelLiberacaoPagamento, "))
                    {
                        select.Append("UsuarioResponsavelLiberacaoPagamento.FUN_NOME as ResponsavelLiberacaoPagamento, ");
                        groupBy.Append("UsuarioResponsavelLiberacaoPagamento.FUN_NOME, ");

                        SetarJoinsUsuarioResponsavelLiberacaoPagamento(joins);
                    }
                    break;

                case "DataLiberacaoPagamento":
                case "DataLiberacaoPagamentoFormatada":
                    if (!select.Contains(" DataLiberacaoPagamento, "))
                    {
                        select.Append("Canhoto.CNF_DATA_LIBERACAO_PAGAMENTO as DataLiberacaoPagamento, ");
                        groupBy.Append("Canhoto.CNF_DATA_LIBERACAO_PAGAMENTO, ");
                    }
                    break;

                case "Protocolo":
                    if (!select.Contains(" Protocolo, "))
                    {
                        select.Append("CTe.CON_PROTOCOLO Protocolo, ");
                        groupBy.Append("CTe.CON_PROTOCOLO, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "DataMalote":
                case "DataMaloteFormatada":
                    if (!select.Contains(" DataMalote, "))
                    {
                        select.Append("Malote.MCA_DATA_MALOTE DataMalote, ");
                        groupBy.Append("Malote.MCA_DATA_MALOTE, ");

                        SetarJoinsMalote(joins);
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        select.Append("Operador.FUN_NOME Operador, ");
                        groupBy.Append("Operador.FUN_NOME, ");

                        SetarJoinsOperador(joins);
                    }
                    break;

                case "NumeroLoteLiberado":
                    if (!select.Contains(" NumeroLoteLiberado, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(_pagamentoLiberado.PAG_NUMERO AS VARCHAR(20)) FROM T_PAGAMENTO _pagamentoLiberado
                                                                                            join T_DOCUMENTO_FATURAMENTO _documentoFaturamento on _documentoFaturamento.PAG_CODIGO_LIBERACAO = _pagamentoLiberado.PAG_CODIGO
                                                                                            WHERE _documentoFaturamento.CON_CODIGO = ViewCTe.CON_CODIGO AND _documentoFaturamento.DFA_SITUACAO != 2 FOR XML PATH('')), 3, 1000) as NumeroLoteLiberado, ");
                        groupBy.Append("ViewCTe.CON_CODIGO, ");

                        SetarJoinsViewCTe(joins);
                    }
                    break;

                case "NumeroLoteBloqueado":
                    if (!select.Contains(" NumeroLoteBloqueado, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(_pagamentoBloqueado.PAG_NUMERO AS VARCHAR(20)) FROM T_PAGAMENTO _pagamentoBloqueado
                                                                                            join T_DOCUMENTO_FATURAMENTO _documentoFaturamento on _documentoFaturamento.PAG_CODIGO = _pagamentoBloqueado.PAG_CODIGO
                                                                                            WHERE _documentoFaturamento.CON_CODIGO = ViewCTe.CON_CODIGO AND _documentoFaturamento.DFA_SITUACAO != 2 FOR XML PATH('')), 3, 1000) as NumeroLoteBloqueado, ");
                        groupBy.Append("ViewCTe.CON_CODIGO, ");

                        SetarJoinsViewCTe(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorCTe.PCT_NOME Tomador, ");
                        groupBy.Append("TomadorCTe.PCT_NOME, ");

                        SetarJoinsTomadorCTe(joins);
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, TomadorCTe.PCT_TIPO as TipoTomador, ");
                        groupBy.Append("TomadorCTe.PCT_CPF_CNPJ, TomadorCTe.PCT_TIPO, ");

                        SetarJoinsTomadorCTe(joins);
                    }
                    break;

                case "TipoCTe":
                    if (!select.Contains(" TipoCTe, "))
                    {
                        select.Append(@"(CASE 
                                        WHEN CTe.CON_TIPO_CTE = 0 THEN 'Normal' 
                                        WHEN CTe.CON_TIPO_CTE = 1 THEN 'Complemento'                                         
                                        WHEN CTe.CON_TIPO_CTE = 2 THEN 'Anulação' 
                                        WHEN CTe.CON_TIPO_CTE = 3 THEN 'Substituto' 
                                        ELSE ''
                                        END) TipoCTe, ");
                        groupBy.Append("CTe.CON_TIPO_CTE, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "PlacaVeiculoResponsavelEntrega":
                    if (!select.Contains(" PlacaVeiculoResponsavelEntrega, "))
                    {
                        select.Append("Veiculo.VEI_PLACA PlacaVeiculoResponsavelEntrega, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DataHistorico":
                    if (!select.Contains(" DataHistorico, "))
                    {
                        if (filtroPesquisa.SituacaoHistorico.HasValue && filtroPesquisa.DataInicialHistorico.HasValue && filtroPesquisa.DataFinalHistorico.HasValue)
                        {
                            select.Append($"(SELECT TOP 1 CONVERT(nvarchar, Historico.CNH_DATA_ENVIO_CANHOTO, 103) FROM T_CANHOTO_NOTA_FISCAL_HISTORICO Historico WHERE Historico.CNF_CODIGO = Canhoto.CNF_CODIGO AND Historico.CNH_SITUACAO_CANHOTO = {filtroPesquisa.SituacaoHistorico.Value.ToString("D")} AND Historico.CNH_DATA_ENVIO_CANHOTO >= '{filtroPesquisa.DataInicialHistorico:yyyy-MM-dd}' AND Historico.CNH_DATA_ENVIO_CANHOTO < '{filtroPesquisa.DataFinalHistorico.Value.AddDays(1):yyyy-MM-dd}') DataHistorico, "); // SQL-INJECTION-SAFE
                            groupBy.Append("Canhoto.CNF_CODIGO, ");
                        }
                        else
                            select.Append("null DataHistorico, ");
                    }
                    break;

                case "DataEntregaNotaCliente":
                case "DataEntregaNotaClienteFormatada":
                    if (!select.Contains(" DataEntregaNotaCliente, "))
                    {
                        select.Append("Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE DataEntregaNotaCliente, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE, ");
                    }
                    break;

                case "Origem":
                    SetarSelect("CidadeOrigem", 0, select, joins, groupBy, false, filtroPesquisa);
                    SetarSelect("EstadoOrigem", 0, select, joins, groupBy, false, filtroPesquisa);
                    break;

                case "CidadeOrigem":
                    if (!select.Contains(" CidadeOrigem, "))
                    {
                        select.Append("Origem.LOC_DESCRICAO CidadeOrigem, ");

                        if (!groupBy.Contains("Origem.LOC_DESCRICAO"))
                            groupBy.Append("Origem.LOC_DESCRICAO, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "EstadoOrigem":
                    if (!select.Contains(" EstadoOrigem, "))
                    {
                        select.Append("Origem.UF_SIGLA EstadoOrigem, ");

                        if (!groupBy.Contains("Origem.UF_SIGLA"))
                            groupBy.Append("Origem.UF_SIGLA, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "Destino":
                    SetarSelect("CidadeDestino", 0, select, joins, groupBy, false, filtroPesquisa);
                    SetarSelect("EstadoDestino", 0, select, joins, groupBy, false, filtroPesquisa);
                    break;

                case "CidadeDestino":
                    if (!select.Contains(" CidadeDestino, "))
                    {
                        select.Append("Destino.LOC_DESCRICAO CidadeDestino, ");

                        if (!groupBy.Contains("Destino.LOC_DESCRICAO"))
                            groupBy.Append("Destino.LOC_DESCRICAO, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "EstadoDestino":
                    if (!select.Contains(" EstadoDestino, "))
                    {
                        select.Append("Destino.UF_SIGLA EstadoDestino, ");

                        if (!groupBy.Contains("Destino.UF_SIGLA"))
                            groupBy.Append("Destino.UF_SIGLA, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "RazaoExpedidor":
                    if (!select.Contains(" RazaoExpedidor, "))
                    {
                        select.Append("Expedidor.CLI_NOME RazaoExpedidor, ");

                        if (!groupBy.Contains("Expedidor.CLI_NOME"))
                            groupBy.Append("Expedidor.CLI_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "NomeFantasiaExpedidor":
                    if (!select.Contains(" NomeFantasiaExpedidor, "))
                    {
                        select.Append("Expedidor.CLI_NOMEFANTASIA NomeFantasiaExpedidor, ");

                        if (!groupBy.Contains("Expedidor.CLI_NOMEFANTASIA"))
                            groupBy.Append("Expedidor.CLI_NOMEFANTASIA, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "ValorFreteNF":
                    if (!select.Contains(" ValorFreteNF, "))
                    {
                        select.Append("SUM(PedidoXmlNotaFiscal.PNF_VALOR_FRETE) ValorFreteNF, ");

                        SetarJoinsPedidoXmlNotaFiscal(joins);
                    }
                    break;

                case "EnderecoDeOrigem":
                    if (!select.Contains(" EnderecoDeOrigem, "))
                    {
                        select.Append("CONCAT(Emitente.CLI_ENDERECO,', #',Emitente.CLI_NUMERO,', ',Emitente.CLI_BAIRRO) EnderecoDeOrigem, ");

                        groupBy.Append("Emitente.CLI_NUMERO,Emitente.CLI_BAIRRO,Emitente.CLI_ENDERECO, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "EnderecoDeDestino":
                    if (!select.Contains(" EnderecoDeDestino, "))
                    {
                        select.Append("CONCAT(Destinatario.CLI_ENDERECO,', #',Destinatario.CLI_NUMERO,', ',Destinatario.CLI_BAIRRO) EnderecoDeDestino, ");
                        groupBy.Append("Destinatario.CLI_NUMERO,Destinatario.CLI_BAIRRO,Destinatario.CLI_ENDERECO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CPFMotorista":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_CPF FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                            join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                                                                                            join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                                                                            WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO FOR XML PATH('')), 3, 1000) as CPFMotorista, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;

                case "CodigoDaTransportadora":
                    if (!select.Contains(" CodigoDaTransportadora, "))
                    {
                        select.Append("Empresa.EMP_CODIGO_INTEGRACAO  CodigoDaTransportadora, ");

                        if (!groupBy.Contains("Empresa.EMP_CODIGO_INTEGRACAO"))
                            groupBy.Append("Empresa.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CodigoDestino":
                    if (!select.Contains(" CodigoDestino, "))
                    {
                        select.Append("Destinatario.CLI_CODIGO_INTEGRACAO CodigoDestino, ");

                        if (!groupBy.Contains("Destinatario.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("Destinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "DataDigitacaoAprovacao":
                    if (!select.Contains(" DataDigitacaoAprovacao, "))
                    {
                        select.Append("Canhoto.CNF_DATA_APROVACAO_DIGITALIZACAO DataDigitacaoAprovacao, ");
                        groupBy.Append("Canhoto.CNF_DATA_APROVACAO_DIGITALIZACAO, ");
                    }
                    break;

                case "DataRecebimentoFisico":
                    if (!select.Contains(" DataRecebimentoFisico, "))
                    {
                        select.Append(@" CASE
                                                WHEN Canhoto.CNF_SITUACAO_CANHOTO = 3 
                                                    THEN Canhoto.CNF_DATA_ENVIO_CANHOTO 
                                                ELSE null
                                        END AS DataRecebimentoFisico, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENVIO_CANHOTO, ");

                        if (!groupBy.Contains("Canhoto.CNF_SITUACAO_CANHOTO"))
                            groupBy.Append("Canhoto.CNF_SITUACAO_CANHOTO, ");
                    }
                    break;

                case "DataRecebimentoFormatada":
                    if (!select.Contains(" DataRecebimento, "))
                    {
                        select.Append("Canhoto.CNF_DATA_RECEBIMENTO as DataRecebimento, ");
                        groupBy.Append("Canhoto.CNF_DATA_RECEBIMENTO, ");
                    }
                    break;

                case "NumeroProtocolo":
                    if (!select.Contains(" NumeroProtocolo, "))
                    {
                        select.Append("Canhoto.CNF_NUMERO_PROTOCOLO as NumeroProtocolo, ");
                        groupBy.Append("Canhoto.CNF_NUMERO_PROTOCOLO, ");
                    }
                    break;

                case "MaloteProtocolo":
                    if (!select.Contains(" MaloteProtocolo, "))
                    {
                        select.Append("Malote.MCA_PROTOCOLO MaloteProtocolo, ");

                        if (!groupBy.Contains("Malote.MCA_PROTOCOLO"))
                            groupBy.Append("Malote.MCA_PROTOCOLO, ");

                        SetarJoinsMalote(joins);
                    }
                    break;
                case "DataEmissaoCte":
                case "DataEmissaoCteFormatada":
                    if (!select.Contains(" DataEmissaoCte, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCte, ");

                        if (!groupBy.Contains("CTe.CON_DATAHORAEMISSAO"))
                            groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "SituacaoNotaFiscalDescricao":
                    if (!select.Contains(" SituacaoNotaFiscal, "))
                    {
                        select.Append("XmlNotaFiscal.NF_SITUACAO_ENTREGA as SituacaoNotaFiscal, ");
                        groupBy.Append("XmlNotaFiscal.NF_SITUACAO_ENTREGA, ");

                        SetarJoinsXmlNotaFiscal(joins);
                    }
                    break;
                case "DataAlteracao":
                case "DataAlteracaoFormatada":
                    if (!select.Contains(" DataAlteracao, "))
                    {
                        select.Append("Canhoto.CNF_DATA_ALTERACAO as DataAlteracao, ");
                        groupBy.Append("Canhoto.CNF_DATA_ALTERACAO, ");
                    }
                    break;
                case "CodigoRastreio":
                    if (!select.Contains(" CodigoRastreio, "))
                    {
                        select.Append("Canhoto.CNF_CODIGO_RASTREIO as CodigoRastreio, ");
                        groupBy.Append("Canhoto.CNF_CODIGO_RASTREIO, ");
                    }
                    break;

                case "ValidacaoViaOCRFormatado":
                    if (!select.Contains(" ValidacaoViaOCR, "))
                    {
                        select.Append("Canhoto.CNF_VALIDACAO_VIA_OCR as ValidacaoViaOCR, ");
                        groupBy.Append("Canhoto.CNF_VALIDACAO_VIA_OCR, ");
                    }
                    break;

                case "SituacaoViagem":
                case "SituacaoViagemFormatada":
                    if (!select.Contains(" SituacaoViagem, "))
                    {
                        select.Append($@"CASE
                                            WHEN Carga.CAR_DATA_INICIO_VIAGEM IS NOT NULL AND Carga.CAR_DATA_FIM_VIAGEM IS NULL THEN 5 
                                            WHEN Carga.CAR_DATA_FIM_VIAGEM IS NOT NULL THEN 2
                                            WHEN Carga.CAR_DATA_INICIO_VIAGEM IS NULL THEN 3
                                            ELSE NULL
		                                    END AS SituacaoViagem, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_INICIO_VIAGEM"))
                            groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_FIM_VIAGEM"))
                            groupBy.Append("Carga.CAR_DATA_FIM_VIAGEM, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "CentroResultadoCarga":
                    if (!select.Contains(" CentroResultadoCarga, "))
                    {
                        select.Append("	substring((	");
                        select.Append("	       select distinct ', ' + convert(nvarchar(20), CentroResultado.Cre_descricao) 	");
                        select.Append("	         from T_CARGA_pedido CargaPedido	");
                        select.Append("	 join T_pedido Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO	");
                        select.Append("	         join T_CENTRO_RESULTADO CentroResultado ON CentroResultado.cre_codigo = Pedido.CRE_CODIGO 	");
                        select.Append("	        where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO	");
                        select.Append("	          for xml path('') 	");
                        select.Append("	            ), 3, 1000) AS CentroResultadoCarga,	");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "MDFesCarga":
                    if (!select.Contains(" MDFesCarga, "))
                    {
                        select.Append("	substring((	");
                        select.Append("	 select distinct ', ' + convert(nvarchar(20), Mdfe.MDF_NUMERO) 	");
                        select.Append("	       from T_CARGA_MDFE CargaMdfe 	");
                        select.Append("	 join T_mdfe Mdfe ON CargaMdfe.MDF_CODIGO = Mdfe.MDF_CODIGO 	");
                        select.Append("	 where CargaMdfe.CAR_CODIGO = Carga.CAR_CODIGO	");
                        select.Append("	 for xml path('') 	");
                        select.Append("	 ), 3, 1000) AS MDFesCarga,	");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "DataEmissaoMdfe":
                case "DescricaoDataEmissaoMdfe":
                    if (!select.Contains(" DataEmissaoMdfe, "))
                    {
                        select.Append("	 (select min(Mdfe.MDF_DATA_EMISSAO ) 	");
                        select.Append("	       from T_CARGA_MDFE CargaMdfe 	");
                        select.Append("	 join T_mdfe Mdfe ON CargaMdfe.MDF_CODIGO = Mdfe.MDF_CODIGO 	");
                        select.Append("	 where CargaMdfe.CAR_CODIGO = Carga.CAR_CODIGO	");
                        select.Append("	 ) AS DataEmissaoMdfe,	");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "SerieCTe":
                    if (!select.Contains(" SerieCTe, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT '/ ' + CONVERT(VARCHAR(20), Serie.ESE_NUMERO) FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    left join V_CANHOTO_CTE ViewCTe on ViewCTe.CNF_CODIGO = CanhotoNotaFiscal.CNF_CODIGO
                                                                                    LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = ViewCTe.CON_CODIGO
                                                                                    LEFT JOIN T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = Canhoto.CNF_CODIGO FOR XML PATH('')), 3, 1000) as SerieCTe, ");
                        groupBy.Append("Canhoto.CNF_CODIGO, ");
                    }
                    break;
                case "OrigemDaCarga":
                    if (!select.Contains(" OrigemDaCarga, "))
                    {
                        select.Append("DadosSumarizados.CDS_ORIGENS OrigemDaCarga, ");
                        groupBy.Append("DadosSumarizados.CDS_ORIGENS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCteSubContratacao(joins);
            SetarJoinsXmlNotaFiscal(joins);
            SetarJoinsCanhotoAvulso(joins);
            SetarJoinsCte(joins);
            SetarJoinsCarga(joins);

            where.Append($@" and ((Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.CTeSubcontratacao}) OR 
                                  (Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.NFe} and XmlNotaFiscal.NF_ATIVA  = 1) OR 
                                  (Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.Avulso}) OR 
                                  (Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.CTe}))");

            where.Append($" and (Canhoto.CAR_CODIGO is null or (Carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 and Carga.CAR_CARGA_FECHADA = 1)) ");

            if (filtrosPesquisa.OrigemDigitalizacao > 0)
                where.Append($" and Canhoto.CNF_ORIGEM_DIGITALIZACAO = {(int)filtrosPesquisa.OrigemDigitalizacao}");

            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0)
                where.Append($" and Canhoto.CNF_SITUACAO_CANHOTO in ({string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.SituacoesDigitalizacaoCanhoto != null && filtrosPesquisa.SituacoesDigitalizacaoCanhoto?.Count > 0)
                where.Append($" and Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO in ({string.Join(", ", filtrosPesquisa.SituacoesDigitalizacaoCanhoto.Select(o => o.ToString("D")))})");
            //Tela de Canhotos e Relatorio de Canhotos utilizam a mesma consulta e mesmo objeto de valor, porém o relatório recebe somente uma situação por vez.
            if (filtrosPesquisa.SituacaoDigitalizacaoCanhoto != null && filtrosPesquisa.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Todas)
                where.Append($" and Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = {(int)filtrosPesquisa.SituacaoDigitalizacaoCanhoto}");

            if (filtrosPesquisa.SituacaoPgtoCanhoto.HasValue && filtrosPesquisa.SituacaoPgtoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Todas)
                where.Append($" and Canhoto.CNF_SITUACAO_PGTO_CANHOTO = {(int)filtrosPesquisa.SituacaoPgtoCanhoto}");

            if (filtrosPesquisa.Motorista > 0)
                where.Append($" and EXISTS(SELECT TOP 1 1 FROM T_CANHOTO_FRETE_MOTORISTAS_RESPONSAVEIS _MotoristasResponsaveis WHERE Canhoto.CNF_CODIGO = _MotoristasResponsaveis.CAR_CODIGO AND _MotoristasResponsaveis.FUN_CODIGO = {filtrosPesquisa.Motorista})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Pessoa > 0)
            {
                where.Append($" and Emitente.CLI_CGCCPF = {filtrosPesquisa.Pessoa}");
                SetarJoinsEmitente(joins);
            }

            if (filtrosPesquisa.GrupoPessoa > 0)
            {
                where.Append($" and GrupoPessoaEmitente.GRP_CODIGO = {filtrosPesquisa.GrupoPessoa}");
                SetarJoinsGrupoPessoaEmitente(joins);
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.Chave))
            {
                where.Append($" and XmlNotaFiscal.NF_CHAVE LIKE '{filtrosPesquisa.Chave}'");
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos)
                where.Append($" and Canhoto.CNF_TIPO_CANHOTO = {(int)filtrosPesquisa.TipoCanhoto}");

            if (filtrosPesquisa.Numeros?.Count > 0)
                where.Append($" and Canhoto.CNF_CODIGO IN ({string.Join(", ", filtrosPesquisa.Numeros)})");

            if (filtrosPesquisa.Filiais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ))");
            }
            else if (filtrosPesquisa.Filiais?.Count > 0)
                where.Append($" and Canhoto.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.Filiais)})");

            if (filtrosPesquisa.ObrigatorioFilial)
                where.Append($" and Canhoto.FIL_CODIGO IS NOT NULL");

            if (filtrosPesquisa.Empresa > 0)
                where.Append($" and Canhoto.EMP_CODIGO = {filtrosPesquisa.Empresa}");

            if (filtrosPesquisa.Empresas?.Count > 0)
                where.Append($" and Canhoto.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.Empresas)})");

            if (filtrosPesquisa.Terceiro > 0)
                where.Append($" and TerceiroResponsavel.CLI_CGCCPF = {filtrosPesquisa.Terceiro}");

            if (filtrosPesquisa.CodigosCargaEmbarcador?.Count > 0)
            {
                where.Append($" and Carga.CAR_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosCargaEmbarcador)})");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Carga > 0)
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.Carga}");

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                where.Append($" and CAST(Canhoto.CNF_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicio.ToString(pattern)}'");

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                where.Append($" and CAST(Canhoto.CNF_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFim.ToString(pattern)}'");

            if (filtrosPesquisa.DataInicioDigitalizacao.HasValue || filtrosPesquisa.DataFimDigitalizacao.HasValue)
            {
                where.Append($" and Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO != {(int)SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao}");

                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (filtrosPesquisa.DataInicioDigitalizacao.HasValue)
                        where.Append($" and CAST(Canhoto.CNF_DATA_DIGITALIZACAO AS DATE) >= '{filtrosPesquisa.DataInicioDigitalizacao.Value.ToString(pattern)}'");

                    if (filtrosPesquisa.DataFimDigitalizacao.HasValue)
                        where.Append($" and CAST(Canhoto.CNF_DATA_DIGITALIZACAO AS DATE) <= '{filtrosPesquisa.DataFimDigitalizacao.Value.ToString(pattern)}'");
                }
                else
                {
                    if (filtrosPesquisa.DataInicioDigitalizacao.HasValue)
                        where.Append($" and CAST(Canhoto.CNF_DATA_ENVIO_CANHOTO AS DATE) >= '{filtrosPesquisa.DataInicioDigitalizacao.Value.ToString(pattern)}'");

                    if (filtrosPesquisa.DataFimDigitalizacao.HasValue)
                        where.Append($" and CAST(Canhoto.CNF_DATA_ENVIO_CANHOTO AS DATE) <= '{filtrosPesquisa.DataFimDigitalizacao.Value.ToString(pattern)}'");
                }
            }

            if (filtrosPesquisa.CodigosConhecimentos != null && filtrosPesquisa.CodigosConhecimentos.Count > 0)
            {
                if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto != TipoCanhoto.CTe && filtrosPesquisa.TipoCanhoto != TipoCanhoto.Todos)
                {
                    where.Append($" and EXISTS(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteXmlNotaFiscal WHERE _cteXmlNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO AND _cteXmlNotaFiscal.CON_CODIGO in {string.Join(", ", filtrosPesquisa.CodigosConhecimentos)})"); // SQL-INJECTION-SAFE
                    SetarJoinsXmlNotaFiscal(joins);
                }
                else if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto == TipoCanhoto.CTe)
                {
                    where.Append($" and CTe.CON_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConhecimentos)})");
                    SetarJoinsCte(joins);
                }
                else
                {
                    where.Append($" and (EXISTS(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteXmlNotaFiscal WHERE _cteXmlNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO AND _cteXmlNotaFiscal.CON_CODIGO in {string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}) or CTe.CON_CODIGO in {string.Join(", ", filtrosPesquisa.CodigosConhecimentos)})"); // SQL-INJECTION-SAFE
                    SetarJoinsXmlNotaFiscal(joins);
                    SetarJoinsCte(joins);
                }
            }
            else if (filtrosPesquisa.CodigoCTe > 0)
            {
                where.Append($" and (EXISTS(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteXmlNotaFiscal WHERE _cteXmlNotaFiscal.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO AND _cteXmlNotaFiscal.CON_CODIGO in {string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}) or CTe.CON_CODIGO = {filtrosPesquisa.CodigoCTe})"); // SQL-INJECTION-SAFE
                SetarJoinsXmlNotaFiscal(joins);
                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.CodigosCanhotos != null && filtrosPesquisa.CodigosCanhotos.Count > 0)
                where.Append($" and Canhoto.CNF_CODIGO in {string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}");

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
            {
                where.Append($" and LocalArmazenamentoCanhoto.LAC_CODIGO = {filtrosPesquisa.CodigoLocalArmazenamento}");
                SetarJoinsLocalArmazenamentoCanhoto(joins);
            }

            if (filtrosPesquisa.TiposCarga?.Count > 0)
            {
                where.Append($" and TipoDeCarga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposCarga)})");
                SetarJoinsTipoDeCarga(joins);
            }

            if (filtrosPesquisa.TiposOperacao?.Count > 0)
            {
                where.Append($" and TipoOperacao.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposOperacao)})");
                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.Recebedor > 0d)
            {
                where.Append($" and Recebedor.CLI_CGCCPF = {filtrosPesquisa.Recebedor}");
                SetarJoinsRecebedor(joins);
            }

            if (filtrosPesquisa.Destinatario?.Count > 0)
                where.Append($" and Destinatario.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.Destinatario)})");

            if (filtrosPesquisa.Pacote > 0)
                where.Append($" and Canhoto.CNF_PACOTE_ARMAZENADO = {filtrosPesquisa.Pacote}");

            if (filtrosPesquisa.Posicao > 0)
                where.Append($" and Canhoto.CNF_POSICAO_NO_PACOTE = {filtrosPesquisa.Posicao}");

            if (filtrosPesquisa.Serie > 0)
                where.Append($" and Canhoto.CNF_SERIE LIKE '{filtrosPesquisa.Serie}'");

            if (filtrosPesquisa.SemMalote)
            {
                where.Append($" and (Canhoto.MCA_CODIGO IS NULL OR Malote.MCA_SITUACAO = {(int)SituacaoMaloteCanhoto.Inconsistente})");
                SetarJoinsMalote(joins);
            }

            if (filtrosPesquisa.Malote.HasValue)
                where.Append($" and EXISTS(SELECT TOP 1 1 FROM T_MALOTE_CANHOTO_CANHOTO _malote WHERE _malote.CNF_CODIGO = Canhoto.CNF_CODIGO)");

            if (filtrosPesquisa.DataEmissaoCTeInicial.HasValue)
            {
                where.Append($" AND EXISTS(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteNotasFiscais JOIN T_CTE _cte ON _cte.CON_CODIGO = _cteNotasFiscais.CON_CODIGO WHERE _cteNotasFiscais.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO AND CAST(_cte.CON_DATAHORAEMISSAO AS DATE) >= '{filtrosPesquisa.DataEmissaoCTeInicial.Value.ToString(pattern)}')"); // SQL-INJECTION-SAFE
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.DataEmissaoCTeFinal.HasValue)
            {
                where.Append($" AND EXISTS(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteNotasFiscais JOIN T_CTE _cte ON _cte.CON_CODIGO = _cteNotasFiscais.CON_CODIGO WHERE _cteNotasFiscais.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO AND CAST(_cte.CON_DATAHORAEMISSAO AS DATE) <= '{filtrosPesquisa.DataEmissaoCTeFinal.Value.ToString(pattern)}')"); // SQL-INJECTION-SAFE
                SetarJoinsXmlNotaFiscal(joins);
            }

            if (filtrosPesquisa.Usuario > 0)
            {
                where.Append($" AND Usuario.FUN_CODIGO = {filtrosPesquisa.Usuario}");
                SetarJoinsUsuario(joins);
            }

            if (filtrosPesquisa.DataInicioEnvio.HasValue)
                where.Append($" and CAST(Canhoto.CNF_DATA_ENVIO_CANHOTO AS DATE) >= '{filtrosPesquisa.DataInicioEnvio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFimEnvio.HasValue)
                where.Append($" and CAST(Canhoto.CNF_DATA_ENVIO_CANHOTO AS DATE) <= '{filtrosPesquisa.DataInicioEnvio.Value.ToString(pattern)}'");

            if (filtrosPesquisa.TipoLocalPrestacao != TipoLocalPrestacao.todos)
            {
                SetarJoinsXmlNotaFiscal(joins);
                if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal)
                    where.Append(@" and Exists(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteXmlNotaFiscal JOIN T_CTE _cte ON _cte.CON_CODIGO = _cteXmlNotaFiscal.CON_CODIGO JOIN T_CTE_PARTICIPANTE _cteRemetente on _cteRemetente.PCT_CODIGO = _cte.CON_REMETENTE_CTE JOIN
                                    T_LOCALIDADES _localidadeRemetente on _localidadeRemetente.LOC_CODIGO = _cteRemetente.LOC_CODIGO JOIN T_CTE_PARTICIPANTE _cteDestinatario on _cteDestinatario.PCT_CODIGO = _cte.CON_DESTINATARIO_CTE JOIN
                                    T_LOCALIDADES _localidadeDestinatario on _localidadeDestinatario.LOC_CODIGO = _cteDestinatario.LOC_CODIGO WHERE _localidadeDestinatario.LOC_CODIGO = _localidadeDestinatario.LOC_CODIGO AND XmlNotaFiscal.NFX_CODIGO = _cteXmlNotaFiscal.NFX_CODIGO)");
                else if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal)
                    where.Append(@" and Exists(SELECT TOP 1 1 FROM T_CTE_XML_NOTAS_FISCAIS _cteXmlNotaFiscal JOIN T_CTE _cte ON _cte.CON_CODIGO = _cteXmlNotaFiscal.CON_CODIGO JOIN T_CTE_PARTICIPANTE _cteRemetente on _cteRemetente.PCT_CODIGO = _cte.CON_REMETENTE_CTE JOIN
                                    T_LOCALIDADES _localidadeRemetente on _localidadeRemetente.LOC_CODIGO = _cteRemetente.LOC_CODIGO JOIN T_CTE_PARTICIPANTE _cteDestinatario on _cteDestinatario.PCT_CODIGO = _cte.CON_DESTINATARIO_CTE JOIN
                                    T_LOCALIDADES _localidadeDestinatario on _localidadeDestinatario.LOC_CODIGO = _cteDestinatario.LOC_CODIGO WHERE _localidadeDestinatario.LOC_CODIGO != _localidadeDestinatario.LOC_CODIGO AND XmlNotaFiscal.NFX_CODIGO = _cteXmlNotaFiscal.NFX_CODIGO)");
            }

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" and Canhoto.EMP_CODIGO = {filtrosPesquisa.Transportador}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculoResponsavelEntrega))
            {
                where.Append($" and Veiculo.VEI_PLACA = '{filtrosPesquisa.PlacaVeiculoResponsavelEntrega.Replace("-", "").Trim()}'");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.SituacaoHistorico.HasValue && filtrosPesquisa.DataInicialHistorico.HasValue && filtrosPesquisa.DataFinalHistorico.HasValue)
                where.Append($" AND EXISTS (SELECT TOP 1 Historico.CNH_CODIGO FROM T_CANHOTO_NOTA_FISCAL_HISTORICO Historico WHERE Historico.CNF_CODIGO = Canhoto.CNF_CODIGO AND Historico.CNH_SITUACAO_CANHOTO = {filtrosPesquisa.SituacaoHistorico.Value:D} AND Historico.CNH_DATA_ENVIO_CANHOTO >= '{filtrosPesquisa.DataInicialHistorico:yyyy-MM-dd}' AND Historico.CNH_DATA_ENVIO_CANHOTO < '{filtrosPesquisa.DataFinalHistorico.Value.AddDays(1):yyyy-MM-dd}')"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoLocalidadeOrigem > 0)
            {
                where.Append($" AND Pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoLocalidadeOrigem} ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CodigoLocalidadeDestino > 0)
            {
                where.Append($" AND Pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoLocalidadeDestino} ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.CnpjExpedidor > 0d)
            {
                where.Append($" AND CargaPedido.CLI_CODIGO_EXPEDIDOR = {filtrosPesquisa.CnpjExpedidor} ");
                SetarJoinsCargaPedido(joins);
            }

            if (filtrosPesquisa.CodigoGrupoPessoaTomador > 0)
            {
                where.Append($" AND TomadorCTe.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoaTomador} ");
                SetarJoinsTomadorCTe(joins);
            }

            if (filtrosPesquisa.CodigoMalote > 0)
                where.Append($" AND Canhoto.MCA_CODIGO = {filtrosPesquisa.CodigoMalote} ");

            if (filtrosPesquisa.DataCriacaoCargaInicial.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataCriacaoCargaInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoCargaFinal.HasValue)
                where.Append($" and CAST(Carga.CAR_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataCriacaoCargaFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.SituacaoViagem.HasValue)
            {

                switch (filtrosPesquisa.SituacaoViagem.Value)
                {
                    case StatusViagemControleEntrega.EmAndamento:
                        where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is not null and Carga.CAR_DATA_FIM_VIAGEM is null ");
                        break;
                    case StatusViagemControleEntrega.Finalizada:
                        where.Append(" and Carga.CAR_DATA_FIM_VIAGEM is not null ");
                        break;
                    case StatusViagemControleEntrega.Iniciada:
                        where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is not null ");
                        break;
                    case StatusViagemControleEntrega.NaoFinalizada:
                        where.Append(" and Carga.CAR_DATA_FIM_VIAGEM is null ");
                        break;
                    case StatusViagemControleEntrega.NaoIniciada:
                        where.Append(" and Carga.CAR_DATA_INICIO_VIAGEM is null ");
                        break;
                    default: break;
                }

                SetarJoinsCarga(joins);
            }
        }

        #endregion
    }
}


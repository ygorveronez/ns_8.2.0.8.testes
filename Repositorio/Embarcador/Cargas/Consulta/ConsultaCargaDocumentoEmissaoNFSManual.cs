using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaDocumentoEmissaoNFSManual : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual>
    {
        #region Construtores

        public ConsultaCargaDocumentoEmissaoNFSManual() : base(tabela: "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL as CargaNFeParaEmissaoNFSManual") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = CargaNFeParaEmissaoNFSManual.CAR_CODIGO ");
        }

        private void SetarJoinsCargaOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" CargaOcorrencia "))
                joins.Append("left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.COC_CODIGO = CargaNFeParaEmissaoNFSManual.COC_CODIGO ");
        }

        private void SetarJoinsDadosNFSManual(StringBuilder joins)
        {
            SetarJoinsLancamentoNFSManual(joins);

            if (!joins.Contains(" DadosNFSManual "))
                joins.Append("left join T_DADOS_NFS_MANUAL DadosNFSManual on DadosNFSManual.NSM_CODIGO = LancamentoNFSManual.NSM_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append("left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = CargaNFeParaEmissaoNFSManual.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCargaOrigem(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = CargaOrigem.EMP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCargaOrigem(joins);

            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = CargaOrigem.FIL_CODIGO ");
        }

        private void SetarJoinsLancamentoNFSManual(StringBuilder joins)
        {
            if (!joins.Contains(" LancamentoNFSManual "))
                joins.Append("left join T_LANCAMENTO_NFS_MANUAL LancamentoNFSManual on LancamentoNFSManual.LNM_CODIGO = CargaNFeParaEmissaoNFSManual.LNM_CODIGO ");
        }

        private void SetarJoinsLocalidadePrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" LocalidadePrestacao "))
                joins.Append("left join T_LOCALIDADES LocalidadePrestacao on LocalidadePrestacao.LOC_CODIGO = CargaNFeParaEmissaoNFSManual.LOC_CODIGO_PRESTACAO ");
        }

        private void SetarJoinsModeloDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumento "))
                joins.Append("left join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = CargaNFeParaEmissaoNFSManual.MOD_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append("left join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = CargaNFeParaEmissaoNFSManual.MOD_CODIGO ");
        }

        private void SetarJoinsNFSManual(StringBuilder joins)
        {
            SetarJoinsLancamentoNFSManual(joins);

            if (!joins.Contains(" NFSManual "))
                joins.Append("left join T_CTE NFSManual on NFSManual.CON_CODIGO = LancamentoNFSManual.CON_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append("left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = CargaNFeParaEmissaoNFSManual.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsSerieNFSManual(StringBuilder joins)
        {
            SetarJoinsNFSManual(joins);

            if (!joins.Contains(" SerieNFSManual "))
                joins.Append("left join T_EMPRESA_SERIE SerieNFSManual on SerieNFSManual.ESE_CODIGO = NFSManual.CON_SERIE ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append("left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaNFeParaEmissaoNFSManual.CLI_CODIGO_TOMADOR ");
        }

        private void SetarJoinsCargaOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" CargaOrigem "))
                joins.Append("left join T_CARGA CargaOrigem on CargaOrigem.CAR_CODIGO = CargaNFeParaEmissaoNFSManual.CAR_CODIGO_ORIGEM ");
        }

        private void SetarJoinsGrupoPessoasTomador(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" GrupoPessoasTomador "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoasTomador ON GrupoPessoasTomador.GRP_CODIGO = Tomador.GRP_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.Append(" LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal ON PedidoNotaFiscal.PNF_CODIGO = CargaNFeParaEmissaoNFSManual.PNF_CODIGO ");
        }

        private void SetarJoinsNotaFiscal(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" NotaFiscal "))
                joins.Append(" LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsEnderecoOrigem(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" EnderecoOrigem "))
                joins.Append(" LEFT JOIN T_PEDIDO_ENDERECO EnderecoOrigem ON EnderecoOrigem.PEN_CODIGO = Pedido.PEN_CODIGO_ORIGEM ");
        }

        private void SetarJoinsEnderecoDestino(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" EnderecoDestino "))
                joins.Append(" LEFT JOIN T_PEDIDO_ENDERECO EnderecoDestino ON EnderecoDestino.PEN_CODIGO = Pedido.PEN_CODIGO_DESTINO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsVeiculoCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains("Codigo,"))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_CODIGO Codigo, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_CODIGO, ");
                    }
                    break;

                case "Chave":
                    if (!select.Contains("Chave,"))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_CHAVE Chave, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_CHAVE, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains("Numero,"))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_NUMERO Numero, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_NUMERO, ");
                    }
                    break;

                case "Serie":
                    if (!select.Contains("Serie,"))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_SERIE Serie, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_SERIE, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains("Descricao,"))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_DESCRICAO Descricao, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_DESCRICAO, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains("Peso,"))
                        select.Append("sum(CargaNFeParaEmissaoNFSManual.NEM_PESO) Peso, ");
                    break;

                case "ValorFrete":
                    if (!select.Contains("ValorFrete,"))
                        select.Append("sum(CargaNFeParaEmissaoNFSManual.NEM_VALOR_FRETE) ValorFrete, ");
                    break;

                case "ValorPrestacaoServico":
                    if (!select.Contains("ValorPrestacaoServico,"))
                        select.Append("sum(CargaNFeParaEmissaoNFSManual.NEM_VALOR_PRESTACAO_SERVICO) ValorPrestacaoServico, ");
                    break;

                case "BaseCalculoISS":
                    if (!select.Contains("BaseCalculoISS,"))
                        select.Append("sum(CargaNFeParaEmissaoNFSManual.NEM_BASE_CALCULO_ISS) BaseCalculoISS, ");
                    break;

                case "DataEmissao":
                    if (!select.Contains("DataEmissao,"))
                    {
                        select.Append("convert(nvarchar(50), CargaNFeParaEmissaoNFSManual.NEM_DATA_EMISSAO, 103) DataEmissao, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_DATA_EMISSAO, ");
                    }
                    break;

                case "AliquotaISS":
                    if (!select.Contains("AliquotaISS,"))
                    {
                        select.Append("DadosNFSManual.NSM_ALIQUOTA_ISS AliquotaISS, ");
                        groupBy.Append("DadosNFSManual.NSM_ALIQUOTA_ISS, ");

                        SetarJoinsDadosNFSManual(joins);
                    }
                    break;

                case "ValorISS":
                    if (!select.Contains("ValorISS,"))
                        select.Append("sum(CargaNFeParaEmissaoNFSManual.NEM_VALOR_ISS) ValorISS, ");
                    break;

                case "PercentualRetencaoISS":
                    if (!select.Contains("PercentualRetencaoISS,"))
                    {
                        select.Append("DadosNFSManual.NSM_PERCENTUAL_RETENCAO PercentualRetencaoISS, ");
                        groupBy.Append("DadosNFSManual.NSM_PERCENTUAL_RETENCAO, ");

                        SetarJoinsDadosNFSManual(joins);
                    }
                    break;

                case "ValorRetencaoISS":
                    if (!select.Contains("ValorRetencaoISS,"))
                    {
                        select.Append("SUM(CargaNFeParaEmissaoNFSManual.NEM_VALOR_RETENCAO_ISS) ValorRetencaoISS, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_VALOR_RETENCAO_ISS, ");
                    }
                    break;

                case "IncluirISSBaseCalculo":
                    if (!select.Contains("IncluirISSBaseCalculo,"))
                    {
                        select.Append("(case DadosNFSManual.NSM_INCLUIR_ISS_BC when 1 then 'Sim' else 'Não' end) IncluirISSBaseCalculo, ");
                        groupBy.Append("DadosNFSManual.NSM_INCLUIR_ISS_BC, ");

                        SetarJoinsDadosNFSManual(joins);
                    }
                    break;

                case "NumeroNFS":
                    if (!select.Contains("NumeroNFS,"))
                    {
                        select.Append("DadosNFSManual.NSM_NUMERO NumeroNFS, ");
                        groupBy.Append("DadosNFSManual.NSM_NUMERO, ");

                        SetarJoinsDadosNFSManual(joins);
                    }
                    break;

                case "SerieNFS":
                    if (!select.Contains("SerieNFS,"))
                    {
                        select.Append("SerieNFSManual.ESE_NUMERO SerieNFS, ");
                        groupBy.Append("SerieNFSManual.ESE_NUMERO, ");

                        SetarJoinsSerieNFSManual(joins);
                    }
                    break;

                case "DataEmissaoNFSManual":
                    if (!select.Contains("DataEmissaoNFSManual, "))
                    {
                        select.Append("convert(nvarchar(50), NFSManual.CON_DATAHORAEMISSAO, 103) DataEmissaoNFSManual, ");
                        groupBy.Append("NFSManual.CON_DATAHORAEMISSAO, ");

                        SetarJoinsNFSManual(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroOcorrencia":
                    if (!select.Contains("NumeroOcorrencia,"))
                    {
                        select.Append("CargaOcorrencia.COC_NUMERO_CONTRATO NumeroOcorrencia, ");
                        groupBy.Append("CargaOcorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsCargaOcorrencia(joins);
                    }
                    break;

                case "NomeRemetente":
                    if (!select.Contains("NomeRemetente, "))
                    {
                        select.Append("Remetente.CLI_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CPFCNPJRemetenteFormatado":
                    if (!select.Contains("CPFCNPJRemetente, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF CPFCNPJRemetente, Remetente.CLI_FISJUR TipoRemetente, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, Remetente.CLI_FISJUR, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "NomeDestinatario":
                    if (!select.Contains("NomeDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME NomeDestinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains("CPFCNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF CPFCNPJDestinatario, Destinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NomeTomador":
                    if (!select.Contains("NomeTomador, "))
                    {
                        select.Append("Tomador.CLI_NOME NomeTomador, ");
                        groupBy.Append("Tomador.CLI_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains("CPFCNPJTomador, "))
                    {
                        select.Append("Tomador.CLI_CGCCPF CPFCNPJTomador, Tomador.CLI_FISJUR TipoTomador, ");
                        groupBy.Append("Tomador.CLI_CGCCPF, Tomador.CLI_FISJUR, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "LocalidadePrestacao":
                    if (!select.Contains("LocalidadePrestacao, "))
                    {
                        select.Append("LocalidadePrestacao.LOC_DESCRICAO + ' - ' + LocalidadePrestacao.UF_SIGLA LocalidadePrestacao, ");
                        groupBy.Append("LocalidadePrestacao.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadePrestacao.UF_SIGLA"))
                            groupBy.Append("LocalidadePrestacao.UF_SIGLA, ");

                        SetarJoinsLocalidadePrestacao(joins);
                    }
                    break;

                case "EstadoPrestacao":
                    if (!select.Contains("EstadoPrestacao, "))
                    {
                        select.Append("LocalidadePrestacao.UF_SIGLA EstadoPrestacao, ");

                        if (!groupBy.Contains("LocalidadePrestacao.UF_SIGLA"))
                            groupBy.Append("LocalidadePrestacao.UF_SIGLA, ");

                        SetarJoinsLocalidadePrestacao(joins);
                    }
                    break;

                case "ModeloDocumentoFiscal":
                    if (!select.Contains("ModeloDocumentoFiscal, "))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_ABREVIACAO ModeloDocumentoFiscal, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;

                case "Empresa":
                    if (!select.Contains("Empresa.EMP_RAZAO Empresa, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CpfCnpjEmpresaFormatado":
                    if (!select.Contains("CpfCnpjEmpresa, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CpfCnpjEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains("Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ModeloDocumento":
                    if (!select.Contains("ModeloDocumento, "))
                    {
                        select.Append("ModeloDocumento.MOD_DESCRICAO ModeloDocumento, ");
                        groupBy.Append("ModeloDocumento.MOD_DESCRICAO, ");

                        SetarJoinsModeloDocumento(joins);
                    }
                    break;

                case "SituacaoNFS":
                case "SituacaoNFSFormatada":
                    if (!select.Contains("SituacaoNFS, "))
                    {
                        select.Append("LancamentoNFSManual.LNM_SITUACAO SituacaoNFS, ");
                        groupBy.Append("LancamentoNFSManual.LNM_SITUACAO, ");

                        SetarJoinsLancamentoNFSManual(joins);
                    }
                    break;

                case "NFSGerada":
                    if (!select.Contains("NFSGerada, "))
                    {
                        select.Append("(case when LancamentoNFSManual.CON_CODIGO is null then 'Não' else 'Sim' end) NFSGerada, ");
                        groupBy.Append("LancamentoNFSManual.CON_CODIGO, ");

                        SetarJoinsLancamentoNFSManual(joins);
                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains("NumeroPedidoCliente, "))
                    {
                        select.Append("CargaNFeParaEmissaoNFSManual.NEM_NUMERO_PEDIDO_CLIENTE NumeroPedidoCliente, ");
                        groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_NUMERO_PEDIDO_CLIENTE, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append(@"ISNULL(SUBSTRING((
                                            SELECT ', ' + 
                                                CTe.CON_OBSGERAIS 
                                            FROM T_CTE CTe 
                                                LEFT JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO 
                                            WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                                AND CTe.CON_NUM = CargaNFeParaEmissaoNFSManual.NEM_NUMERO 
                                            FOR XML PATH('')), 3, 1000), 
                                        '') Observacao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("CargaNFeParaEmissaoNFSManual.NEM_NUMERO,"))
                            groupBy.Append("CargaNFeParaEmissaoNFSManual.NEM_NUMERO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "GrupoPessoasTomador":
                    if (!select.Contains(" GrupoPessoasTomador, "))
                    {
                        select.Append("GrupoPessoasTomador.GRP_DESCRICAO GrupoPessoasTomador, ");
                        groupBy.Append("GrupoPessoasTomador.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoasTomador(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "ValorNotaFiscal":
                    if (!select.Contains(" ValorNotaFiscal, "))
                    {
                        select.Append("SUM(NotaFiscal.NF_VALOR) ValorNotaFiscal, ");

                        SetarJoinsNotaFiscal(joins);
                    }
                    break;

                case "CEPOrigem":
                    if (!select.Contains(" CEPOrigem, "))
                    {
                        select.Append("EnderecoOrigem.PEN_CEP CEPOrigem, ");
                        groupBy.Append("EnderecoOrigem.PEN_CEP, ");

                        SetarJoinsEnderecoOrigem(joins);
                    }
                    break;

                case "CEPDestino":
                    if (!select.Contains(" CEPDestino, "))
                    {
                        select.Append("EnderecoDestino.PEN_CEP CEPDestino, ");
                        groupBy.Append("EnderecoDestino.PEN_CEP, ");

                        SetarJoinsEnderecoDestino(joins);
                    }
                    break;

                case "ModeloVeicularCarga":
                    if (!select.Contains("ModeloVeicularCarga,"))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "VeiculoCarga":
                case "VeiculoCargaFormatada":
                    if (!select.Contains("VeiculoCarga,"))
                    {
                        select.Append("Veiculo.VEI_PLACA VeiculoCarga, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculoCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCarga(joins);
            SetarJoinsCargaOcorrencia(joins);

            where.Append($" and (CargaOcorrencia.COC_SITUACAO_OCORRENCIA is null or CargaOcorrencia.COC_SITUACAO_OCORRENCIA <> {(int)SituacaoOcorrencia.Cancelada}) AND (Carga.CAR_SITUACAO NOT IN ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}))");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and CargaNFeParaEmissaoNFSManual.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.PossuiNFSGerada.HasValue)
            {
                where.Append($" and LancamentoNFSManual.CON_CODIGO is {(filtrosPesquisa.PossuiNFSGerada.Value ? "not " : "")} null");

                SetarJoinsLancamentoNFSManual(joins);
            }

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                where.Append($" and CargaNFeParaEmissaoNFSManual.NEM_DATA_EMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.Value.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataEmissaoFinal.HasValue)
                where.Append($" and CargaNFeParaEmissaoNFSManual.NEM_DATA_EMISSAO < '{filtrosPesquisa.DataEmissaoFinal.Value.AddDays(1).ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataEmissaoNFSInicial.HasValue || filtrosPesquisa.DataEmissaoNFSFinal.HasValue)
            {
                if (filtrosPesquisa.DataEmissaoNFSInicial.HasValue)
                    where.Append($" and NFSManual.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoNFSInicial.Value.ToString("yyyy-MM-dd")}'");

                if (filtrosPesquisa.DataEmissaoNFSFinal.HasValue)
                    where.Append($" and NFSManual.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoNFSFinal.Value.AddDays(1).ToString("yyyy-MM-dd")}'");

                SetarJoinsNFSManual(joins);
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                where.Append($" and LancamentoNFSManual.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");
                SetarJoinsLancamentoNFSManual(joins);
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (CargaOrigem.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinsCargaOrigem(joins);
            }
            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
            {
                where.Append($" and CargaOrigem.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)})");

                SetarJoinsCargaOrigem(joins);
            }

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                where.Append($" and CargaNFeParaEmissaoNFSManual.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjRemetente}");

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                where.Append($" and CargaNFeParaEmissaoNFSManual.CLI_CODIGO_DESTINATARIO = {filtrosPesquisa.CpfCnpjDestinatario}");

            if (filtrosPesquisa.CpfCnpjTomador > 0d)
                where.Append($" and CargaNFeParaEmissaoNFSManual.CLI_CODIGO_TOMADOR = {filtrosPesquisa.CpfCnpjTomador}");

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append($" and Tomador.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas}");

                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.CodigoLocalidadePrestacao > 0)
                where.Append($" and CargaNFeParaEmissaoNFSManual.LOC_CODIGO_PRESTACAO = {filtrosPesquisa.CodigoLocalidadePrestacao}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoLocalidadePrestacao) && (filtrosPesquisa.EstadoLocalidadePrestacao != "0"))
            {
                where.Append($" and LocalidadePrestacao.UF_SIGLA = '{filtrosPesquisa.EstadoLocalidadePrestacao}'");

                SetarJoinsLocalidadePrestacao(joins);
            }

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append($" and CargaNFeParaEmissaoNFSManual.NEM_NUMERO >= {filtrosPesquisa.NumeroInicial}");

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append($" and CargaNFeParaEmissaoNFSManual.NEM_NUMERO <= {filtrosPesquisa.NumeroFinal}");

            if ((filtrosPesquisa.NumeroInicialNFS > 0) || (filtrosPesquisa.NumeroFinalNFS > 0))
            {
                if (filtrosPesquisa.NumeroInicialNFS > 0)
                    where.Append($" and DadosNFSManual.NSM_NUMERO >= {filtrosPesquisa.NumeroInicialNFS}");

                if (filtrosPesquisa.NumeroFinalNFS > 0)
                    where.Append($" and DadosNFSManual.NSM_NUMERO <= {filtrosPesquisa.NumeroFinalNFS}");

                SetarJoinsNFSManual(joins);
            }

            if (filtrosPesquisa.SituacaoNFS.Count > 0 && !filtrosPesquisa.SituacaoNFS.Contains(SituacaoLancamentoNFSManual.Todas))
            {
                string situacoes = string.Join(", ", filtrosPesquisa.SituacaoNFS.Select(s => (int)s));
                where.Append($" and LancamentoNFSManual.LNM_SITUACAO in ({situacoes})");

                SetarJoinsLancamentoNFSManual(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                where.Append($" and CargaNFeParaEmissaoNFSManual.NEM_NUMERO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}'");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($" AND Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");
                SetarJoinsCarga(joins);
            }
            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
                SetarJoinsCarga(joins);
            }

        }


        #endregion
    }
}

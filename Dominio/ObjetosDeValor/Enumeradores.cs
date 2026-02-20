using System.Collections.Generic;

namespace Dominio.Enumeradores
{
    public enum InclusaoISSNFSe
    {
        ConformeIntegracao = 0,
        SempreIncliur = 1,
        NuncaIncluir = 2
    }

    public enum EncerramentoMDFeAutomatico
    {
        Padrao = 0,
        Nenhum = 1,
        ApenasNoAmbiente = 2,
        Todos = 3
    }

    public enum TipoSefaz
    {
        NFe = 0,
        CTe = 1,
        MDFe = 2
    }

    public enum TipoItemProposta
    {
        Valor = 0,
        Percentual = 1
    }

    public enum TipoOrdemDeCompra
    {
        Materiais = 0,
        Servicos = 1
    }

    public enum TipoAgrupamentoEmissaoEmail
    {
        Nenhum = 0,
        Destinatario = 1,
        Placa = 2,
        Observacao = 3
    }

    public enum TipoDocumento
    {
        Nenhum = -1,
        CTe = 0,
        NFSe = 1,
        Outros = 2,
        NFS = 3,
        Subcontratacao = 4,
        Todos = 99
    }

    public static class TipoDocumentoHelper
    {
        public static string ObterDescricao(this TipoDocumento tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumento.Nenhum: return "Nenhum";
                case TipoDocumento.CTe: return "CT-e";
                case TipoDocumento.NFSe: return "NFS-e";
                case TipoDocumento.Outros: return "Outros";
                case TipoDocumento.NFS: return "NFS";
                case TipoDocumento.Subcontratacao: return "Subcontratação";
                default: return string.Empty;
            }
        }

        public static List<Dominio.Enumeradores.TipoDocumento> ObterTiposDocumentosPadraoCte()
        {
            // Listando todos os tipos que não são NFS manual
            return new List<Dominio.Enumeradores.TipoDocumento>
            {
                Dominio.Enumeradores.TipoDocumento.CTe,
                Dominio.Enumeradores.TipoDocumento.NFSe,
                Dominio.Enumeradores.TipoDocumento.Outros,
                Dominio.Enumeradores.TipoDocumento.Subcontratacao
            };
        }
    }

    public enum TipoEmpresaCIOT
    {
        Transportador = 0,
        Embarcador = 1
    }

    public enum StatusSolicitacaoEmissao
    {
        Pendente = 0,
        Alocado = 1,
        Finalizado = 2
    }

    public enum StatusFreteSubcontratado
    {
        Aberto = 0,
        Fechado = 1
    }

    public enum TipoFreteSubcontratado
    {
        Entrega = 0,
        Reentrega = 1,
        Coleta = 2,
        Devolucao = 3
    }

    public enum StatusComissaoCliente
    {
        Inativo = 0,
        Ativo = 1
    }

    public enum TipoMovimento
    {
        Entrada = 0,
        Saida = 1
    }

    public static class TipoMovimentoHelper
    {
        public static string ObterDescricao(this TipoMovimento tipoMovimento)
        {
            switch (tipoMovimento)
            {
                case TipoMovimento.Entrada: return "Entrada";
                case TipoMovimento.Saida: return "Saída";
                default: return string.Empty;
            }
        }
    }

    public enum TipoMovimentoAcerto
    {
        PorAcerto = 0,
        Detalhado = 1
    }


    public enum TipoDuplicata
    {
        AReceber = 0,
        APagar = 1
    }

    public enum TipoLogAcesso
    {
        Entrada = 0,
        Saída = 1
    }

    public static class TipoLogAcessoHelper
    {
        public static string ObterDescricao(this TipoLogAcesso tipoLogAcesso)
        {
            switch (tipoLogAcesso)
            {
                case TipoLogAcesso.Entrada: return "Entrada";
                case TipoLogAcesso.Saída: return "Saída";
                default: return string.Empty;
            }
        }
    }

    public enum TipoHistorico
    {
        Informacao = 1,
        Atualizacao = 2,
        Inativacao = 3
    }

    public enum StatusVendaCertificado
    {
        AguardandoDadosParaAdesao = 1,
        CertificadoVendido = 2,
        SemContato = 3,
        Retornar = 4,
        Providenciando = 5,
        Bloqueado = 6,

        Atualizado = 8,
        Inativado = 9
    }

    public enum StatusDuplicata
    {
        Pendente = 0,
        Paga = 1
    }

    public enum TipoPesoNFe : int
    {
        Bruto = 0,
        Liquido = 1
    }


    public enum TipoComissao : int
    {
        ValorLiquido = 0,
        ValorBruto = 1
    }

    public enum TipoValeAcertoViagem
    {
        Vale = 1,
        Devolucao = 2
    }

    public enum TipoPagamento
    {
        Pago = 0,
        A_Pagar = 1,
        Outros = 2
    }

    public static class TipoPagamentoHelper
    {
        public static string ObterDescricao(this TipoPagamento tipo)
        {
            switch (tipo)
            {
                case TipoPagamento.A_Pagar: return "A Pagar";
                case TipoPagamento.Outros: return "Outros";
                case TipoPagamento.Pago: return "Pago";
                default: return string.Empty;
            }
        }

        public static TipoCondicaoPagamento? ObterTipoCondicaoPagamento(this TipoPagamento tipo)
        {
            switch (tipo)
            {
                case TipoPagamento.A_Pagar: return TipoCondicaoPagamento.FOB;
                case TipoPagamento.Pago: return TipoCondicaoPagamento.CIF;
                default: return null;
            }
        }
    }

    public enum TipoCondicaoPagamento
    {
        CIF = 1,
        FOB = 2
    }

    public static class TipoCondicaoPagamentoHelper
    {
        public static string ObterDescricao(this TipoCondicaoPagamento tipo)
        {
            switch (tipo)
            {
                case TipoCondicaoPagamento.CIF: return "CIF";
                case TipoCondicaoPagamento.FOB: return "FOB";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoResumida(this TipoCondicaoPagamento tipo)
        {
            switch (tipo)
            {
                case TipoCondicaoPagamento.CIF: return "C";
                case TipoCondicaoPagamento.FOB: return "F";
                default: return string.Empty;
            }
        }
    }

    public enum SituacaoSubcontratacao
    {
        Pendente = 0,
        AgProcessamento = 1,
        FalhaProcessamento = 2,
        EmitindoCTes = 3,
        Finalizado = 4,
        RejeicaoCTe = 5,
        DocumentosCancelados = 6
    }


    public enum TipoPagamentoRegraICMS
    {
        Todos = -1,
        Pago = 0,
        A_Pagar = 1,
        Outros = 2
    }

    public enum TipoPagamentoFrete
    {
        Todos = 0,
        Pago = 1,
        A_Pagar = 2
    }

    public enum TipoCalculoFreteKMTipoVeiculo
    {
        Acerto = 1,
        CTe = 2
    }

    public enum TipoImpressao
    {
        Retrato = 1,
        Paisagem = 2
    }

    public enum ModalProposta
    {
        Rodoviario = 1,
        Rodoaereo = 2,
        Outros = 9
    }

    public enum TipoAmbiente
    {
        Nenhum = 0,
        Producao = 1,
        Homologacao = 2
    }

    public static class TipoAmbienteHelper
    {
        public static string ObterDescricao(this TipoAmbiente tipoAmbiente)
        {
            switch (tipoAmbiente)
            {
                case TipoAmbiente.Nenhum: return "Nenhum";
                case TipoAmbiente.Producao: return "Produção";
                case TipoAmbiente.Homologacao: return "Homologação";
                default: return string.Empty;
            }
        }
    }

    public enum RegimeEspecialEmpresa
    {
        Nenhum = 0,
        MicroempresaMunicipal = 1,
        Estimativa = 2,
        SociedadeProfissionais = 3,
        Cooperativa = 4,
        MicroempresarioIndividual = 5,
        MicroempresarioEmpresaPP = 6,
        LucroReal = 7,
        LucroPresumido = 8,
        SimplesNacional = 9,
        Imune = 10,
        EmpresaIndividualEireli = 11,
        EmpresaPP = 12,
        MicroEmpresario = 13,
        Outros = 14,
        MovimentoMensal = 15,
        ISSQNAutonomos = 16,
        ISSQNSociedade = 17,
        NotarioRegistrador = 18,
        TribFaturamentoVariavel = 19,
        Fixo = 20,
        Isencao = 21,
        ExigibilidadeSuspensaoJudicial = 22,
        ExigibilidadeSuspensaAdm = 23
    }

    public enum TipoAcesso
    {
        Emissao = 0,
        Admin = 1,
        AdminInterno = 2,
        Embarcador = 3,
        Terceiro = 4,
        Fornecedor = 5
    }

    public enum TipoSistema
    {
        MultiCTe = 0,
        MultiEmbarcador = 1
    }

    public enum EmissaoPor
    {
        CallCenter = 1,
        Web = 2
    }

    public enum TipoTransportador
    {
        ETC = 1,
        TAC = 2,
        CTC = 3
    }

    public enum TipoAjuda
    {
        Arquivo = 1,
        Video = 2
    }

    public enum TipoCTE
    {
        Todos = -1,
        Normal = 0,
        Complemento = 1,
        Anulacao = 2,
        Substituto = 3,
        Simplificado = 4,
        SimplificadoSubstituto = 5
    }

    public static class TipoCTeHelper
    {
        public static string ObterDescricao(this TipoCTE tipoCTE)
        {
            switch (tipoCTE)
            {
                case TipoCTE.Todos: return "Todos";
                case TipoCTE.Normal: return "Normal";
                case TipoCTE.Complemento: return "Complemento";
                case TipoCTE.Anulacao: return "Anulação";
                case TipoCTE.Substituto: return "Substituto";
                case TipoCTE.Simplificado: return "Simplificado";
                case TipoCTE.SimplificadoSubstituto: return "Simp. Substituto";
                default: return string.Empty;
            }
        }
    }

    public enum TipoCTEAverbacao
    {
        ApenasNormal = 0,
        ApenasSubcontratacao = 1,
        ApenasRedespacho = 2,
        ApenasRedIntermediario = 3,
        ApenasServVinculadoMultimodal = 4,
        ApenasTransporteDePessoas = 6,
        ApenasTransporteDeValores = 7,
        ApenasExcessoDeBagagem = 8,
        Todos = 99
    }


    public enum TipoServico
    {
        Normal = 0,
        SubContratacao = 1,
        Redespacho = 2,
        RedIntermediario = 3,
        ServVinculadoMultimodal = 4,
        TransporteDePessoas = 6,
        TransporteDeValores = 7,
        ExcessoDeBagagem = 8
    }

    public static class TipoServicoHelper
    {
        public static string ObterDescricao(this TipoServico tipoServico)
        {
            switch (tipoServico)
            {
                case TipoServico.Normal: return "Normal";
                case TipoServico.Redespacho: return "Redespacho";
                case TipoServico.RedIntermediario: return "Red. Intermediário";
                case TipoServico.SubContratacao: return "Subcontratação";
                case TipoServico.ServVinculadoMultimodal: return "Serv. Vinculado Multimodal";
                case TipoServico.TransporteDePessoas: return "Transporte de Pessoas";
                case TipoServico.TransporteDeValores: return "Transporte de Valores";
                case TipoServico.ExcessoDeBagagem: return "Excesso de Bagagem";
                default: return string.Empty;
            }
        }
    }

    public enum TipoSso
    {
        OAuth2 = 1,
        Saml2 = 2,
        CyberArk = 3
    }

    public enum TipoTomador
    {
        NaoInformado = -1,
        Remetente = 0,
        Expedidor = 1,
        Recebedor = 2,
        Destinatario = 3,
        Intermediario = 5,
        Tomador = 6,
        Outros = 4
    }

    public static class TipoTomadorHelper
    {
        public static string ObterDescricao(this TipoTomador tipoTomador)
        {
            switch (tipoTomador)
            {
                case TipoTomador.Destinatario: return "Destinatário";
                case TipoTomador.Expedidor: return "Expedidor";
                case TipoTomador.Intermediario: return "Intermediário";
                case TipoTomador.Tomador: return "Tomador";
                case TipoTomador.NaoInformado: return "Não Informado";
                case TipoTomador.Outros: return "Outros";
                case TipoTomador.Recebedor: return "Recebedor";
                case TipoTomador.Remetente: return "Remetente";
                default: return string.Empty;
            }
        }
    }
    public enum OpcaoSimNao
    {
        Nao = 0,
        Sim = 1,
        Todos = 9
    }

    public enum ReutilizaNumeracao
    {
        Nao = 0,
        Sim = 1,
        Reutilizado = 2
    }


    public static class OpcaoSimNaoHelper
    {
        public static string ObterDescricao(this OpcaoSimNao opcao)
        {
            switch (opcao)
            {
                case OpcaoSimNao.Nao: return "Não";
                case OpcaoSimNao.Sim: return "Sim";
                default: return string.Empty;
            }
        }
    }

    public enum OpcaoSimNaoPesquisa
    {
        Nao = 0,
        Sim = 1,
        Todos = 9
    }

    public static class OpcaoSimNaoPesquisaHelper
    {
        public static string ObterDescricao(this OpcaoSimNaoPesquisa opcao)
        {
            switch (opcao)
            {
                case OpcaoSimNaoPesquisa.Nao: return "Não";
                case OpcaoSimNaoPesquisa.Sim: return "Sim";
                default: return string.Empty;
            }
        }
    }

    public enum GeradoPendente
    {
        Todos = 0,
        Pendente = 1,
        Gerado = 2
    }

    public static class GeradoPendenteHelper
    {
        public static string ObterDescricao(this GeradoPendente opcao)
        {
            switch (opcao)
            {
                case GeradoPendente.Pendente: return "Pendente";
                case GeradoPendente.Gerado: return "Gerado";
                default: return string.Empty;
            }
        }
    }

    public enum TipoSeguro
    {
        Remetente = 0,
        Expedidor = 1,
        Recebedor = 2,
        Destinatario = 3,
        Emitente_CTE = 4,
        Tomador_Servico = 5
    }

    public static class TipoSeguroHelper
    {
        public static string ObterDescricao(this TipoSeguro tipoSeguro)
        {
            switch (tipoSeguro)
            {
                case TipoSeguro.Destinatario: return "Destinatário";
                case TipoSeguro.Emitente_CTE: return "Emitente";
                case TipoSeguro.Expedidor: return "Expedidor";
                case TipoSeguro.Recebedor: return "Recebedor";
                case TipoSeguro.Remetente: return "Remetente";
                case TipoSeguro.Tomador_Servico: return "Tomador";
                default: return string.Empty;
            }
        }
    }

    public enum TipoResponsavelSeguroMDFe
    {
        Emitente = 1,
        Contratante = 2,
    }

    public enum TipoPessoaCTe
    {
        Remetente = 1,
        Expedidor = 2,
        Recebedor = 3,
        Destinatario = 4
    }

    public enum TipoICMS
    {
        ICMS_Normal_00 = 1,
        ICMS_Reducao_Base_Calculo_20 = 2,
        ICMS_Isencao_40 = 3,
        ICMS_Nao_Tributado_41 = 4,
        ICMS_Diferido_51 = 5,
        ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60 = 6,
        ICMS_Outras_Situacoes_90 = 9,
        ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90 = 10,
        Simples_Nacional = 11
    }

    public static class TipoICMSHelper
    {
        public static string ObterCST(this TipoICMS tipoICMS)
        {
            switch (tipoICMS)
            {
                case TipoICMS.ICMS_Normal_00: return "00";
                case TipoICMS.ICMS_Reducao_Base_Calculo_20: return "20";
                case TipoICMS.ICMS_Isencao_40: return "40";
                case TipoICMS.ICMS_Nao_Tributado_41: return "41";
                case TipoICMS.ICMS_Diferido_51: return "51";
                case TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60: return "60";
                case TipoICMS.ICMS_Outras_Situacoes_90: return "90";
                case TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90: return "91";
                case TipoICMS.Simples_Nacional: return "";
                default: return null;
            }
        }

        public static string ObterCSTParaRelatorio(this TipoICMS tipoICMS)
        {
            switch (tipoICMS)
            {
                case TipoICMS.ICMS_Normal_00: return "'00'";
                case TipoICMS.ICMS_Reducao_Base_Calculo_20: return "'20'";
                case TipoICMS.ICMS_Isencao_40: return "'40'";
                case TipoICMS.ICMS_Nao_Tributado_41: return "'41'";
                case TipoICMS.ICMS_Diferido_51: return "'51'";
                case TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60: return "'60'";
                case TipoICMS.ICMS_Outras_Situacoes_90: return "'90'";
                case TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90: return "'91'";
                case TipoICMS.Simples_Nacional: return "''";
                default: return null;
            }
        }

        public static string ObterDescricao(this TipoICMS tipoICMS)
        {
            switch (tipoICMS)
            {
                case TipoICMS.ICMS_Normal_00: return "00 - Normal";
                case TipoICMS.ICMS_Reducao_Base_Calculo_20: return "20 - Com redução na base de cálculo";
                case TipoICMS.ICMS_Isencao_40: return "40 - Isenção";
                case TipoICMS.ICMS_Nao_Tributado_41: return "41 - Não tributado";
                case TipoICMS.ICMS_Diferido_51: return "51 - Diferido";
                case TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60: return "60 - Cobrado por substituição tributária";
                case TipoICMS.ICMS_Outras_Situacoes_90: return "90 - Outras situações";
                case TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90: return "90 - Devido à UF de origem da prestação, quando diferente da UF do emitente";
                case TipoICMS.Simples_Nacional: return "Simples Nacional";
                default: return null;
            }
        }

        public static string ObterTipoDescricao(string tipo)
        {
            switch (tipo)
            {
                case "00": return "00 - Normal";
                case "20": return "20 - Com redução na base de cálculo";
                case "40": return "40 - Isenção";
                case "41": return "41 - Não tributado";
                case "51": return "51 - Diferido";
                case "60": return "60 - Cobrado por substituição tributária";
                case "90": return "90 - Outras situações";
                case "91": return "90 - Devido à UF de origem da prestação, quando diferente da UF do emitente";
                case "": return "Simples Nacional";
                default: return "";
            }
        }
    }

    public enum TipoPIS
    {
        Operacao_Tributavel_Com_Aliquota_Basica_01 = 1,
        Operacao_Tributavel_Com_Aliquota_Diferenciada_02 = 2,
        Operacao_Tributavel_A_Aliquota_Zero_06 = 6,
        Operacao_Isenta_Da_Contribuicao_07 = 7,
        Operacao_Sem_Incidencia_Da_Contribuicao_08 = 8,
        Operacao_Com_Suspensao_Da_Contribuicao_09 = 9,
        Outras_Operacoes_De_Saida_49 = 49,
        Outras_Operacoes_99 = 99
    }

    public enum TipoCOFINS
    {
        Operacao_Tributavel_Com_Aliquota_Basica_01 = 1,
        Operacao_Tributavel_Com_Aliquota_Diferenciada_02 = 2,
        Operacao_Tributavel_A_Aliquota_Zero_06 = 6,
        Operacao_Isenta_Da_Contribuicao_07 = 7,
        Operacao_Sem_Incidencia_Da_Contribuicao_08 = 8,
        Operacao_Com_Suspensao_Da_Contribuicao_09 = 9,
        Outras_Operacoes_De_Saida_49 = 49,
        Outras_Operacoes_99 = 99
    }

    public enum UnidadeMedida
    {
        M3 = 0,
        KG = 1,
        TON = 2,
        UN = 3,
        LT = 4,
        MMBTU = 5,
        M3_ST = 99
    }

    public enum UnidadeMedidaMDFe
    {
        KG = 1,
        TON = 2
    }

    public static class UnidadeMedidaMDFeHelper
    {
        public static string ObterDescricao(this UnidadeMedidaMDFe unidade)
        {
            switch (unidade)
            {
                case UnidadeMedidaMDFe.KG: return "Kg";
                case UnidadeMedidaMDFe.TON: return "Ton";
                default: return string.Empty;
            }
        }
    }

    public enum TipoObservacao
    {
        Todos = -1,
        Contribuinte = 0,
        Fisco = 1,
        Geral = 2
    }

    public enum TipoDocumentoAnulacao
    {
        NFe = 2,
        CTe = 1,
        CTouNF = 3
    }

    public enum TipoArquivoRelatorio
    {
        /// <summary>
        /// Portable Document Format
        /// </summary>
        PDF = 0,
        /// <summary>
        /// Rich Text Format
        /// </summary>
        RTF = 1,
        /// <summary>
        /// MS Word
        /// </summary>
        DOC = 2,
        /// <summary>
        /// MS Excel
        /// </summary>
        XLS = 3,

        CSV = 4
    }

    public static class TipoArquivoRelatorioHelper
    {
        public static string ObterDescricao(this TipoArquivoRelatorio tipo)
        {
            switch (tipo)
            {
                case TipoArquivoRelatorio.CSV: return "csv";
                case TipoArquivoRelatorio.DOC: return "doc";
                case TipoArquivoRelatorio.PDF: return "pdf";
                case TipoArquivoRelatorio.RTF: return "rtf";
                case TipoArquivoRelatorio.XLS: return "xls";
                default: return string.Empty;
            }
        }
    }

    public enum TipoXMLCTe
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Desacordo = 2,
        EnvioIntegracao = 3,
        RetornoIntegracao = 4,
        EnvioConsultaIntegracao = 5,
        RetornoConsultaIntegracao = 6
    }

    public enum TipoXMLMDFe
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Encerramento = 2,
        Contingencia = 3,
        InclusaoMorotista = 4
    }

    public enum TipoFreteValor : int
    {
        ValorMinimoMaisPercentual = 0,
        ValorMinimoGarantido = 1,
        SomentePercentualSobreNF = 2
    }

    public enum TipoFrete : int
    {
        ValorMinimoGarantido = 0,
        ValorMinimoMaisComponentes = 1
    }

    public enum TipoRateioTabelaFreteValor : int
    {
        Nenhum = 0,
        ValorNotaFiscal = 1,
        Peso = 2,
        PorDocumento = 3,
        Volume = 4,
        PorCTe = 5,
        PesoDestinatarioRemetente = 6,
        ValorMercadoriaDestinatarioRemetente = 7
    }

    public enum TipoRateioProdutos : int
    {
        Nenhum = 0,
        Peso = 1,
        Valor = 2
    }

    public enum IncluiICMSFrete : int
    {
        Nao = 0,
        Sim = 1,
    }

    public enum TipoProprietarioVeiculo : int
    {
        TACAgregado = 0,
        TACIndependente = 1,
        Outros = 2,
        NaoAplicado = 3,
        Todos = 4
    }

    public static class TipoProprietarioVeiculoHelper
    {
        public static string ObterDescricao(this TipoProprietarioVeiculo tipoPorprietarioVeiculo)
        {
            switch (tipoPorprietarioVeiculo)
            {
                case TipoProprietarioVeiculo.TACAgregado: return "TAC Agregado";
                case TipoProprietarioVeiculo.TACIndependente: return "TAC Independente";
                case TipoProprietarioVeiculo.Outros: return "Outros";
                case TipoProprietarioVeiculo.NaoAplicado: return "Não Aplicado";
                case TipoProprietarioVeiculo.Todos: return "Todos";
                default: return string.Empty;
            }
        }
    }

    public enum IdentificacaoEmail : int
    {
        Outros = 0,
        CTe = 1,
        MDFe = 2,
        NFSe = 3
    }


    public enum FinalidadeDoArquivoSPED : int
    {
        RemessaOriginal = 0,
        RemessaSubstituto = 1
    }

    public enum FinalidadeDoArquivoPH : int
    {
        RemessaOriginal = 0,
        RemessaSubstituto = 1
    }

    public enum TipoDeAtividade : int
    {
        IndustrialOuEquiparado = 0,
        Outros = 1
    }

    public enum StatusIntegracao : int
    {
        Pendente = 0,
        Integrado = 1,
        Impresso = 2,
        AguardandoGeracaoCTe = 3,
        AguardandoGeracaoNFSeTemporaria = 4,
        AguardandoConfirmacao = 5
    }

    public enum StatusIntegracaoCarga : int
    {
        Pendente = 0,
        Gerado = 1,
        PendenteCancelamento = 2,
        SolicitadoCancelamento = 3,
        CancelamentoNaoEfetuado = 4,
        Erro = 9
    }

    public enum TipoIntegracao : int
    {
        Emissao = 0,
        Cancelamento = 1,
        Inutilizacao = 2,
        Todos = 9
    }

    public enum TipoArquivoIntegracao : int
    {
        NFe = 0,
        EDI = 1,
        TXT = 2,
        Objeto = 3,
        CTe = 4
    }

    public enum TipoRetornoIntegracao : int
    {
        Todos = 0,
        XML = 2,
        PDF = 3,
        CONEMB = 4,
        TXT = 5,
        XML_PDF = 6,
        XML_PDFBIN = 7

    }

    public enum TipoArquivo : int
    {
        TXT = 0,
        ZIP = 1
    }

    public enum StatusImportacaoPreCTe : int
    {
        Pendente = 0,
        Finalizada = 1
    }

    public enum TipoCampoCCe
    {
        Texto = 0,
        Inteiro = 1,
        Decimal = 2,
        Selecao = 3,
        Data = 4
    }

    public enum TipoCampoCCeAutomatico
    {
        Nenhum = 0,
        Booking = 1,
        Navio = 2,
        PortoPassagem = 3,
        PortoPassagem2 = 4,
        PortoPassagem3 = 5,
        PortoPassagem4 = 6,
        PortoPassagem5 = 7
    }

    public enum StatusCCe
    {
        EmDigitacao = 0,
        Pendente = 1,
        Enviado = 2,
        Autorizado = 3,
        Rejeicao = 9
    }

    public enum TipoObjetoConsulta
    {
        CTe,
        CCe,
        MDFe,
        NFSe,
        Averbacao,
        AverbacaoMDFe
    }

    public enum TipoSerie
    {
        CTe = 0,
        MDFe = 1,
        NFSe = 2,
        NFe = 3,
        OutrosDocumentos = 4,
        NFCe = 5,
        CTeRec = 9,
        DTe = 10
    }
    public enum StatusIntegracaoCTeManual : int
    {
        EmitirCTeManual = 0,
        EmitirSubstituicao = 1,
        EmitirAnulacao = 2,
        AnularCTe = 3,
        CancelarCTeManual = 4,
    }

    public static class StatusIntegracaoCTeManualHelper
    {
        public static string ObterDescricao(this StatusIntegracaoCTeManual statusIntegracaoCTeManual)
        {
            switch (statusIntegracaoCTeManual)
            {
                case StatusIntegracaoCTeManual.EmitirCTeManual: return "Emitir CT-e Manual";
                case StatusIntegracaoCTeManual.EmitirSubstituicao: return "Emitir Substituição";
                case StatusIntegracaoCTeManual.EmitirAnulacao: return "Emitir Anulação";
                case StatusIntegracaoCTeManual.AnularCTe: return "Anular CT-e";
                case StatusIntegracaoCTeManual.CancelarCTeManual: return "Cancelar CT-e Manual";
                default: return string.Empty;
            }
        }
    }

    public static class TipoSerieHelper
    {
        public static string ObterDescricao(this TipoSerie tipoSerie)
        {
            switch (tipoSerie)
            {
                case TipoSerie.CTe: return "CT-e";
                case TipoSerie.MDFe: return "MDF-e";
                case TipoSerie.NFSe: return "NFS-e";
                case TipoSerie.NFe: return "NF-e";
                case TipoSerie.NFCe: return "NFC-e";
                case TipoSerie.DTe: return "DT-e";
                case TipoSerie.OutrosDocumentos: return "Outros Documentos";
                case TipoSerie.CTeRec: return "CT-e Recebido";
                default: return string.Empty;
            }
        }
    }

    public enum StatusMDFe
    {
        Todos = -1,
        EmDigitacao = 0,
        Pendente = 1,
        Enviado = 2,
        Autorizado = 3,
        EmEncerramento = 4,
        Encerrado = 5,
        EmCancelamento = 6,
        Cancelado = 7,
        Rejeicao = 9,
        EmitidoContingencia = 10,
        AguardandoCompraValePedagio = 11,
        EventoInclusaoMotoristaEnviado = 12
    }

    public enum TipoMotoristaMDFe
    {
        Normal = 0,
        SolicitadoEventoInclusao = 1,
        Incluido = 2,
        EventoInclusaoRejeitado = 9
    }

    public static class StatusMDFeHelper
    {
        public static string ObterDescricao(this StatusMDFe status)
        {
            switch (status)
            {
                case StatusMDFe.Autorizado:
                    return "Autorizado";
                case StatusMDFe.Cancelado:
                    return "Cancelado";
                case StatusMDFe.EmCancelamento:
                    return "Em Cancelamento";
                case StatusMDFe.EmDigitacao:
                    return "Em Digitação";
                case StatusMDFe.EmEncerramento:
                    return "Em Encerramento";
                case StatusMDFe.Encerrado:
                    return "Encerrado";
                case StatusMDFe.Enviado:
                    return "Enviado";
                case StatusMDFe.Pendente:
                    return "Pendente";
                case StatusMDFe.Rejeicao:
                    return "Rejeição";
                case StatusMDFe.EmitidoContingencia:
                    return "Contingência";
                case StatusMDFe.AguardandoCompraValePedagio:
                    return "Aguardando compra Vale Pedágio";
                case StatusMDFe.EventoInclusaoMotoristaEnviado:
                    return "Enviado Evento de Inclusão de Motorista";
                default:
                    return string.Empty;
            }
        }
    }

    public enum TipoErroSefaz
    {
        CTe = 0,
        MDFe = 1
    }

    public enum TipoEmissaoMDFe
    {
        Normal = 0,
        Contingencia = 1
    }

    public enum TipoEmitenteMDFe
    {
        PrestadorDeServicoDeTransporte = 1,
        NaoPrestadorDeServicoDeTransporte = 2,
        TransporteCTeGlobalizado = 3,
        PrestadorDeServicoDeTransporteApenasChaveCTe = 9
    }

    public enum TipoCargaMDFe
    {
        NaoDefinido = 0,
        GranelSolido = 1,
        GranelLiquido = 2,
        Frigorificada = 3,
        Conteinerizada = 4,
        CargaGeral = 5,
        Neogranel = 6,
        PerigosaGranelSolido = 7,
        PerigosaGranelLiquido = 8,
        PerigosaFrigorificada = 9,
        PerigosaConteinerizada = 10,
        PerigosaCargaGeral = 11,
        GranelPressurizada = 12
    }

    public static class TipoCargaMDFeHelper
    {
        public static string ObterDescricao(this Dominio.Enumeradores.TipoCargaMDFe tipo)
        {
            return tipo switch
            {
                TipoCargaMDFe.NaoDefinido => "Não Definido",
                TipoCargaMDFe.GranelSolido => "Granel Sólido",
                TipoCargaMDFe.GranelLiquido => "Granel Líquido",
                TipoCargaMDFe.Frigorificada => "Frigorificada",
                TipoCargaMDFe.Conteinerizada => "Conteinerizada",
                TipoCargaMDFe.CargaGeral => "Carga Geral",
                TipoCargaMDFe.Neogranel => "Neogranel",
                TipoCargaMDFe.PerigosaGranelSolido => "Perigosa - Granel Sólido",
                TipoCargaMDFe.PerigosaGranelLiquido => "Perigosa - Granel Líquido",
                TipoCargaMDFe.PerigosaFrigorificada => "Perigosa - Frigorificada",
                TipoCargaMDFe.PerigosaConteinerizada => "Perigosa - Conteinerizada",
                TipoCargaMDFe.PerigosaCargaGeral => "Perigosa - Carga Geral",
                TipoCargaMDFe.GranelPressurizada => "Granel Pressurizada",
                _ => string.Empty
            };
        }
    }

    public enum TipoDocumentoCTe
    {
        NFe = 0,
        NF = 1,
        Outros = 2
    }

    public static class TipoDocumentoCTeHelper
    {
        public static string ObterDescricao(this TipoDocumentoCTe tipo)
        {
            switch (tipo)
            {
                case TipoDocumentoCTe.NFe: return "NF-e (Nota Fiscal Eletrônica)";
                case TipoDocumentoCTe.NF: return "Nota Fiscal";
                case TipoDocumentoCTe.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }

    public enum TipoDocumentoAnteriorCTe
    {
        Eletronico = 0,
        Papel = 1
    }

    public enum TipoCampoEDI
    {
        Alfanumerico = 0,
        Numerico = 1,
        Decimal = 2,
        DataEHora = 4
    }

    public enum CondicaoCampoEDI
    {
        Nenhum = 0,
        Sum = 1,
        Max = 2,
        Min = 3,
        First = 4,
        Count = 5,
        Last = 6
    }

    public enum ObjetoCampoEDI
    {
        Nenhum = 0,
        CTe = 1,
        NotaFiscal = 2,
        InformacaoCarga = 3,
        ComponentePrestacao = 4,
        Veiculo = 5,
        Ocorrencia = 6,
        Global = 7,
        Dinamico = 8,
        Fatura = 9,
        FaturaCTe = 10,
        NFe = 11,
        NFeVolumes = 12,
        Outro = 13,
        MDFe = 14,
        Lacre = 15,
        ObsCont = 16,
        Duplicata = 17,
        DuplicataParcela = 18,
        CTeAnterior = 19

    }

    public enum AlinhamentoCampoEDI
    {
        Direita = 0,
        Esquerda = 1
    }

    public enum TipoLayoutEDI
    {
        CONEMB = 0,
        DOCCOB = 1,
        NOTFIS = 2,
        OCOREN = 3,
        SEGURO = 4,
        EBS = 5,
        OUTRO = 6,
        FISCAL = 7,
        EBSCancelados = 8,
        INTDNE = 9,
        CONEMB_MB = 10, //caso específico da Martin Brower
        DESPESACOMPLEMENTAR = 11,
        NOTFIS_NOVA_IMPORTACAO = 12,
        INTNC = 13,
        EBSProduto = 14,
        EBSNotaEntrada = 15,
        EAI = 16, // Danone
        INTPFAR = 17,
        OCOREN_NFS = 18,
        PREFAT = 19,
        CONEMB_CANCELAMENTO = 20,
        EBSBaixas = 21,
        EBSComissaoMotorista = 22,
        PROV = 23,
        INTNC_CANCELAMENTO = 24,
        GEN = 25,
        PROV_INTPFAR = 26,
        QuestorComissaoMotorista = 27,
        MarterSAF = 28,
        MasterSAFCancelamento = 29,
        RPSNotaServico = 30,
        RetornoRPSNotaServico = 31,
        IntegracaoCarregamento = 32,
        OcorenOTIF = 33,
        TransportationPlann = 34,
        VGM = 35,
        CONEMB_NF = 36,
        AGRO = 37,
        CONEMB_CT_IMP = 38,//Caterpillar Importacao
        CONEMB_CT_EXP = 39,//Caterpillar Exportacao
        DOCCOB_CT = 40,//Caterpillar DOCCOB
        Cliente = 41,
        Pedido = 42,
        ImportsysCTe = 43,
        ImportsysVP = 44,
        UVT_RN = 45,
        DOCCOB_VAXXINOVA = 46,
        CONEMB_VOLKS = 47,

        Todos = 99
    }

    public static class TipoLayoutEDIHelper
    {
        public static string ObterDescricao(this TipoLayoutEDI tipoLayoutEDI)
        {
            switch (tipoLayoutEDI)
            {
                case TipoLayoutEDI.CONEMB: return "CONEMB";
                case TipoLayoutEDI.CONEMB_MB: return "CONEMB MB";
                case TipoLayoutEDI.CONEMB_NF: return "CONEMB NF";
                case TipoLayoutEDI.CONEMB_CANCELAMENTO: return "CONEMB Cancelamento";
                case TipoLayoutEDI.DOCCOB: return "DOCCOB";
                case TipoLayoutEDI.NOTFIS: return "NOTFIS";
                case TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO: return "NOTFIS - Nova Importação";
                case TipoLayoutEDI.OCOREN: return "OCOREN";
                case TipoLayoutEDI.OCOREN_NFS: return "OCOREN NFS";
                case TipoLayoutEDI.PREFAT: return "PREFAT";
                case TipoLayoutEDI.SEGURO: return "SEGURO";
                case TipoLayoutEDI.DESPESACOMPLEMENTAR: return "DESPESA COMPLEMENTAR";
                case TipoLayoutEDI.EBS: return "EBS";
                case TipoLayoutEDI.EBSProduto: return "EBS Produto";
                case TipoLayoutEDI.EBSNotaEntrada: return "EBS Nota Entrada";
                case TipoLayoutEDI.EBSBaixas: return "EBS Baixas";
                case TipoLayoutEDI.EBSComissaoMotorista: return "EBS Comissão Motorista";
                case TipoLayoutEDI.INTDNE: return "INTDNE";
                case TipoLayoutEDI.INTPFAR: return "INTPFAR";
                case TipoLayoutEDI.PROV_INTPFAR: return "PROV INTPFAR";
                case TipoLayoutEDI.INTNC: return "INTNC";
                case TipoLayoutEDI.INTNC_CANCELAMENTO: return "INTNC Cancelamento";
                case TipoLayoutEDI.EAI: return "EAI";
                case TipoLayoutEDI.PROV: return "PROV";
                case TipoLayoutEDI.GEN: return "GEN";
                case TipoLayoutEDI.OUTRO: return "OUTRO";
                case TipoLayoutEDI.MarterSAF: return "MasterSAF";
                case TipoLayoutEDI.MasterSAFCancelamento: return "MasterSAF Cancelamento";
                case TipoLayoutEDI.RPSNotaServico: return "RPS de Nota de Serviço";
                case TipoLayoutEDI.RetornoRPSNotaServico: return "Retorno RPS de Nota de Serviço";
                case TipoLayoutEDI.QuestorComissaoMotorista: return "Questor Comissão Motorista";
                case TipoLayoutEDI.IntegracaoCarregamento: return "Integração Carregamento";
                case TipoLayoutEDI.OcorenOTIF: return "OCOREN OTIF";
                case TipoLayoutEDI.TransportationPlann: return "Transportation Plann";
                case TipoLayoutEDI.VGM: return "VGM";
                case TipoLayoutEDI.AGRO: return "AGRO";
                case TipoLayoutEDI.CONEMB_CT_IMP: return "CONEMB CT IMP";
                case TipoLayoutEDI.CONEMB_CT_EXP: return "CONEMB CT EXP";
                case TipoLayoutEDI.DOCCOB_CT: return "DOCCOB CT";
                case TipoLayoutEDI.Cliente: return "Cliente";
                case TipoLayoutEDI.Pedido: return "Pedido";
                case TipoLayoutEDI.ImportsysCTe: return "ImportsysCTe";
                case TipoLayoutEDI.ImportsysVP: return "ImportsysVP";
                case TipoLayoutEDI.UVT_RN: return "UVT - RN";
                case TipoLayoutEDI.DOCCOB_VAXXINOVA: return "DOCCOB VAXXINOVA";
                case TipoLayoutEDI.CONEMB_VOLKS: return "CONEMB VOLKS";
                default: return string.Empty;
            }
        }
    }

    public enum TipoIntegracaoMDFe
    {
        Emissao = 0,
        Encerramento = 1,
        Cancelamento = 2,
        Todos = 9
    }

    public enum TipoGeracaoCTeWS
    {
        GerarCTe = 0,
        GerarMDFe = 1
    }

    public enum StatusImpressaoCTe
    {
        Pendente = 0,
        Impresso = 1
    }

    public enum TipoPessoa
    {
        Fisica = 0,
        Juridica = 1
    }

    public enum TipoCFOP
    {
        Entrada = 0,
        Saida = 1
    }

    public static class TipoCFOPHelper
    {
        public static string ObterDescricao(this TipoCFOP tipoCFOP)
        {
            switch (tipoCFOP)
            {
                case TipoCFOP.Entrada: return "Entrada";
                case TipoCFOP.Saida: return "Saída";
                default: return string.Empty;
            }
        }
    }

    public enum CargaTrechos
    {
        Todos = 9,
        Nao = 0,
        ApenasTrechos = 1,
    }

    public static class EnumCargaTrechosHelper
    {
        public static string ObterDescricao(this CargaTrechos opcao)
        {
            switch (opcao)
            {
                case CargaTrechos.Todos: return "Todos";
                case CargaTrechos.Nao: return "Não";
                case CargaTrechos.ApenasTrechos: return "Apenas Trechos";
                default: return string.Empty;
            }
        }
    }

    public enum StatusDocumentoEntrada
    {
        Aberto = 0,
        Finalizado = 1,
        Cancelado = 2
    }

    public enum IndicadorPagamentoDocumentoEntrada
    {
        AVista = 0,
        APrazo = 1,
        Outros = 9
    }

    public enum PerfilEmpresa
    {
        A = 0,
        B = 1,
        C = 2
    }

    public enum TipoTabelaFrete
    {
        PorPeso = 1,
        PorValor = 2,
        PorTipoDeVeiculo = 3,
        FracionadaPorUnidade = 4
    }

    public enum StatusRelacaoCTesEntregues
    {
        Aberto = 1,
        Fechado = 2,
        Cancelado = 9
    }

    public enum TipoDiariaRelacaoCTesEntregues
    {
        SemDiaria = 0,
        Diaria = 1,
        MeiaDiaria = 2
    }

    public enum LoteCalculoFrete
    {
        Padrao = 0,
        Integracao = 1,
        Reprocessamento = 2
    }

    public enum TipoIntegracaoEmillenium
    {
        Pedidos = 0,
        NotasFiscais = 1,
        PedidosDevolucao = 2

    }


    #region Integracao Avon

    public enum StatusManifestoAvon
    {
        Enviado = 0,
        Emitido = 1,
        Finalizado = 2,
        FalhaNoRetorno = 3
    }

    public enum StatusDocumentoManifestoAvon
    {
        Enviado = 0,
        Emitido = 1,
        Finalizado = 2,
        FalhaNoRetorno = 3,
        Todas = 99,
    }

    public enum StatusImportacaoFTP
    {
        Salvo = 0,
        Processado = 1,
        ErroProcessamento = 9
    }

    public enum StatusEnvioFTP
    {
        Pendente = 0,
        Sucesso = 1,
        Erro = 9
    }

    public enum TipoArquivoFTP
    {
        ImportacaoNOTFIS = 0,
        ImportacaoXMLNFe = 1,
        EnvioCONEMB = 2,
        EnvioOCORENCTe = 3,
        EnvioOCORENNFSe = 4,
        EnvioXMLCTe = 5,
        EnvioOCORENNFe = 6
    }

    public enum TipoProcessamentoArquivoFTP
    {
        PorExtensao = 1,
        Texto = 2,
        XML = 3,
        CSV = 4,
        XLSX = 5,
        XLSXRiachuelo = 6,
    }

    public enum TipoRateioFTP
    {
        Remetente = 0,
        Destinatario = 1,
        RemetenteDestinatario = 2,
        PorNFe = 3,
    }

    public enum ConexaoFTP
    {
        FTP = 1,
        SFPT = 2,
        FTPS = 3
    }
    #endregion

    #region SPED Contribuições

    public enum TipoDeAtividadeSPEDContribuicoes
    {
        IndustrialOuEquiparado = 0,
        PrestadorDeServicos = 1,
        Comercio = 2,
        PessoasJuridicas = 3,
        Imobiliaria = 4,
        Outros = 9
    }

    public enum TipoEscrituracao
    {
        Original = 0,
        Retificadora = 1
    }

    public enum IndicadorDeSituacaoEspecial
    {
        Abertura = 0,
        Cisao = 1,
        Fusao = 2,
        Incorporacao = 3,
        Encerramento = 4
    }

    public enum IndicadorDaIncidenciaTributariaNoPeriodo
    {
        IncidenciaExclusivamenteNoRegimeNaoCumulativo = 1,
        IncidenciaExclusivamenteNoRegimeCumulativo = 2,
        IncidenciaNosRegimesNaoCumulativoECumulativo = 3
    }

    public enum IndicadorDeMetodoDeApropriacaoDeCreditosComuns
    {
        ApropriacaoDireta = 1,
        RateioProporcional = 2
    }

    public enum IndicadorDoTipoDeContribuicaoApuradaNoPeriodo
    {
        ExclusivamenteAliquotaBasica = 1,
        AliquotasEspecificas = 2
    }

    public enum IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado
    {
        RegimeDeCaixa_EscrituracaoConsolidada = 1,
        RegimeDeCompetencia_EscrituracaoConsolidada = 2,
        RegimeDeCompetencia_EscrituracaoDetalhada = 9
    }

    #endregion

    #region Siga Fácil

    public enum RegraQuitacaoAdiantamento
    {
        NaoDefinido = 0,
        Posto = 1,
        Filial = 2
    }

    public enum RegraQuitacaoQuitacao
    {
        NaoDefinido = 0,
        Posto = 1,
        Filial = 2
    }

    public enum TipoViagem
    {
        PorPeso = 0,
        PorUnidade = 1
    }

    public enum TipoFavorecido
    {
        NaoDefinido = 0,
        Contratado = 1,
        SubContratante = 2,
        Motorista = 3
    }

    public enum CategoriaTransportadorANTT
    {
        NaoTAC = 0,
        TAC = 1
    }

    public enum DocumentosObrigatorios
    {
        NaoDefinido = 0,
        ExigeCTRC = 5,
        ExigeCTRCETicketBalanca = 6,
        ExigeCTRCECanhotoNF = 7,
        ExigeCTRCECanhotoNFETicketBalanca = 8
    }

    public enum TipoViagemANTT
    {
        NaoDefinido = 0,
        OperacaoTransportePadrao = 1,
        OperacaoTransporteTACAgregado = 3
    }

    public enum TipoPeso
    {
        PesoCarregado = 0,
        PesoLotacao = 1
    }

    public enum RecalculoFrete
    {
        NaoDefinido = 0,
        CobraDiferenca = 1,
        NaoCobraDiferenca = 2
    }

    public enum ExigePesoChegada
    {
        Nao = 0,
        Sim = 1
    }

    public enum TipoQuebra
    {
        NaoDefinido = 0,
        Integral = 1,
        Parcial = 2,
        SemQuebra = 3
    }

    public enum TipoTolerancia
    {
        NaoDefinido = 0,
        Percentual = 1,
        Peso = 2
    }

    #endregion

    #region NFSe

    public enum StatusNFSe
    {
        EmDigitacao = 0,
        Pendente = 1,
        Enviado = 2,
        Autorizado = 3,
        EmCancelamento = 4,
        Cancelado = 5,
        AgGeracaoNFSeManual = 6,
        AgDadosNFSeManual = 7,
        AgAprovacaoNFSeManual = 8,
        NFSeManualGerada = 10,
        AguardandoAutorizacaoRPS = 11,
        Inutilizada = 12,
        Rejeicao = 9
    }

    public enum ExigibilidadeISS
    {
        Exigivel = 1,
        NaoInicidencia = 2,
        Isencao = 3,
        Exportacao = 4,
        Imunidade = 5,
        SuspensaDecisaoJudicial = 6,
        SuspensaProcessoAdministrativo = 7,
        NaoInformado = 8
    }

    public enum TipoXMLNFSe
    {
        Autorizacao = 0,
        Cancelamento = 1
    }

    public enum TipoNotaFiscalServico
    {
        Todos = 0,
        Manual = 1,
        Eletronica = 2,
        Conjugada = 3
    }

    public enum TipoClienteNotaFiscalServico
    {
        Tomador = 1,
        Intermediario = 2
    }

    public enum TipoIntegracaoNFSe
    {
        Emissao = 0,
        Cancelamento = 1
    }

    public enum TipoFretamento
    {
        Eventual = 1,
        Continuo = 2
    }

    public enum StatusConsultaCNPJ
    {
        Pendente = 0,
        EmProcessamento = 1,
        Sucesso = 2,
        Erro = 9
    }

    #endregion

    #region Averbação CT-e

    public enum IntegradoraAverbacao
    {
        NaoDefinido = 0,
        ATM = 1,
        Quorum = 2,
        PortoSeguro = 3,
        ELT = 4,
        Senig = 5
    }

    public enum TipoAverbacaoCTe
    {
        Autorizacao = 0,
        Cancelamento = 1
    }

    public enum FormaAverbacaoCTE
    {
        Definitiva = 0,
        Provisoria = 1
    }

    public static class FormaAverbacaoCTEHelper
    {
        public static string ObterDescricao(this FormaAverbacaoCTE forma)
        {
            switch (forma)
            {
                case FormaAverbacaoCTE.Definitiva: return "Definitiva";
                case FormaAverbacaoCTE.Provisoria: return "Provisória";
                default: return string.Empty;
            }
        }
    }

    public enum TipoDocumentoAverbacao
    {
        Conhecimento = 0,
        OrdemServico = 1
    }

    public enum TipoPropostaFeeder
    {
        Mercosul = 0,
        Alianca = 1,
        HSBR = 2,
        Outros = 3
    }

    public enum StatusAverbacaoCTe
    {
        Pendente = 0,
        Sucesso = 1,
        Cancelado = 2,
        Enviado = 3,
        AgEmissao = 4,
        AgCancelamento = 5,
        Rejeicao = 9,
        Todos = 99
    }

    public static class StatusAverbacaoCTeHelper
    {
        public static string ObterDescricao(this StatusAverbacaoCTe status)
        {
            switch (status)
            {
                case StatusAverbacaoCTe.Pendente: return "Pendente";
                case StatusAverbacaoCTe.Sucesso: return "Averbado";
                case StatusAverbacaoCTe.Cancelado: return "Cancelado";
                case StatusAverbacaoCTe.Enviado: return "Enviado";
                case StatusAverbacaoCTe.AgEmissao: return "Ag. Emissão";
                case StatusAverbacaoCTe.AgCancelamento: return "Ag. Cancelamento";
                case StatusAverbacaoCTe.Rejeicao: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }

    public enum FiltroAverbacaoCTe
    {
        Averbados = 1,
        NaoAverbados = 2
    }

    #endregion

    #region Averbação MDF-e

    public enum TipoAverbacaoMDFe
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Encerramento = 2
    }

    public enum StatusAverbacaoMDFe
    {
        Pendente = 0,
        Sucesso = 1,
        Cancelado = 2,
        Encerrado = 3,
        AgEmissao = 4,
        AgCancelamento = 5,
        AgEncerramento = 6,
        Rejeicao = 9
    }

    public static class StatusAverbacaoMDFeHelper
    {
        public static string Descricao(this StatusAverbacaoMDFe statusAverbacaoMDFe)
        {
            switch (statusAverbacaoMDFe)
            {
                case StatusAverbacaoMDFe.Pendente:
                    return "Pendente";
                case StatusAverbacaoMDFe.Sucesso:
                    return "Sucesso";
                case StatusAverbacaoMDFe.Cancelado:
                    return "Cancelado";
                case StatusAverbacaoMDFe.Encerrado:
                    return "Encerrado";
                case StatusAverbacaoMDFe.AgEmissao:
                    return "Ag. Emissão";
                case StatusAverbacaoMDFe.AgCancelamento:
                    return "Ag. Cancelamento";
                case StatusAverbacaoMDFe.AgEncerramento:
                    return "Ag. Encerramento";
                case StatusAverbacaoMDFe.Rejeicao:
                    return "Rejeição";
                default:
                    return string.Empty;
            }
        }

        public static string CorLinha(this StatusAverbacaoMDFe statusAverbacaoMDFe)
        {
            switch (statusAverbacaoMDFe)
            {
                case StatusAverbacaoMDFe.Sucesso:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
                case StatusAverbacaoMDFe.Cancelado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;
                case StatusAverbacaoMDFe.Encerrado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;
                case StatusAverbacaoMDFe.Rejeicao:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;
            }
        }

        public static string CorFonte(this StatusAverbacaoMDFe statusAverbacaoMDFe)
        {
            if (statusAverbacaoMDFe == StatusAverbacaoMDFe.Rejeicao || statusAverbacaoMDFe == StatusAverbacaoMDFe.Cancelado || statusAverbacaoMDFe == StatusAverbacaoMDFe.Encerrado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;
            else
                return string.Empty;
        }
    }

    public enum StatusIntegracaoEnvioCTe : int
    {
        Pendente = 0,
        Integrado = 1,
        Erro = 9
    }

    public enum StatusIntegracaoRecebimentoCTe : int
    {
        Pendente = 0,
        Alocado = 1,
        Aprovado = 2,
        Reprovado = 3
    }

    public enum TipoIntegracaoEnvioCTe : int
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Inutilizacao = 2
    }

    #endregion

    #region NFe

    public enum ListaServico
    {
        LS101 = 1,
        LS102 = 2,
        LS103 = 3,
        LS104 = 4,
        LS105 = 5,
        LS106 = 6,
        LS107 = 7,
        LS108 = 8,
        LS201 = 9,
        LS301 = 10,
        LS302 = 11,
        LS303 = 12,
        LS304 = 13,
        LS305 = 14,
        LS401 = 15,
        LS402 = 16,
        LS403 = 17,
        LS404 = 18,
        LS405 = 19,
        LS406 = 20,
        LS407 = 21,
        LS408 = 22,
        LS409 = 23,
        LS410 = 24,
        LS411 = 25,
        LS412 = 26,
        LS413 = 27,
        LS414 = 28,
        LS415 = 29,
        LS416 = 30,
        LS417 = 31,
        LS418 = 32,
        LS419 = 33,
        LS420 = 34,
        LS421 = 35,
        LS422 = 36,
        LS423 = 37,
        LS501 = 38,
        LS502 = 39,
        LS503 = 40,
        LS504 = 41,
        LS505 = 42,
        LS506 = 43,
        LS507 = 44,
        LS508 = 45,
        LS509 = 46,
        LS601 = 47,
        LS602 = 48,
        LS603 = 49,
        LS604 = 50,
        LS605 = 51,
        LS701 = 52,
        LS702 = 53,
        LS703 = 54,
        LS704 = 55,
        LS705 = 56,
        LS706 = 57,
        LS707 = 58,
        LS708 = 59,
        LS709 = 60,
        LS710 = 61,
        LS711 = 62,
        LS712 = 63,
        LS713 = 64,
        LS714 = 65,
        LS715 = 66,
        LS716 = 67,
        LS717 = 68,
        LS718 = 69,
        LS719 = 70,
        LS720 = 71,
        LS721 = 72,
        LS722 = 73,
        LS801 = 74,
        LS802 = 75,
        LS901 = 76,
        LS902 = 77,
        LS903 = 78,
        LS1001 = 79,
        LS1002 = 80,
        LS1003 = 81,
        LS1004 = 82,
        LS1005 = 83,
        LS1006 = 84,
        LS1007 = 85,
        LS1008 = 86,
        LS1009 = 87,
        LS1010 = 88,
        LS1101 = 89,
        LS1102 = 90,
        LS1103 = 91,
        LS1104 = 92,
        LS1201 = 93,
        LS1202 = 94,
        LS1203 = 95,
        LS1204 = 96,
        LS1205 = 97,
        LS1206 = 98,
        LS1207 = 99,
        LS1208 = 100,
        LS1209 = 101,
        LS1210 = 102,
        LS1211 = 103,
        LS1212 = 104,
        LS1213 = 105,
        LS1214 = 106,
        LS1215 = 107,
        LS1216 = 108,
        LS1217 = 109,
        LS1301 = 110,
        LS1302 = 111,
        LS1303 = 112,
        LS1304 = 113,
        LS1305 = 114,
        LS1401 = 115,
        LS1402 = 116,
        LS1403 = 117,
        LS1404 = 118,
        LS1405 = 119,
        LS1406 = 120,
        LS1407 = 121,
        LS1408 = 122,
        LS1409 = 123,
        LS1410 = 124,
        LS1411 = 125,
        LS1412 = 126,
        LS1413 = 127,
        LS1501 = 128,
        LS1502 = 129,
        LS1503 = 130,
        LS1504 = 131,
        LS1505 = 132,
        LS1506 = 133,
        LS1507 = 134,
        LS1508 = 135,
        LS1509 = 136,
        LS1510 = 137,
        LS1511 = 138,
        LS1512 = 139,
        LS1513 = 140,
        LS1514 = 141,
        LS1515 = 142,
        LS1516 = 143,
        LS1517 = 144,
        LS1518 = 145,
        LS1601 = 146,
        LS1701 = 147,
        LS1702 = 148,
        LS1703 = 149,
        LS1704 = 150,
        LS1705 = 151,
        LS1706 = 152,
        LS1707 = 153,
        LS1708 = 154,
        LS1709 = 155,
        LS1710 = 156,
        LS1711 = 157,
        LS1712 = 158,
        LS1713 = 159,
        LS1714 = 160,
        LS1715 = 161,
        LS1716 = 162,
        LS1717 = 163,
        LS1718 = 164,
        LS1719 = 165,
        LS1720 = 166,
        LS1721 = 167,
        LS1722 = 168,
        LS1723 = 169,
        LS1724 = 170,
        LS1801 = 171,
        LS1901 = 172,
        LS2001 = 173,
        LS2002 = 174,
        LS2003 = 175,
        LS2101 = 176,
        LS2201 = 177,
        LS2301 = 178,
        LS2401 = 179,
        LS2501 = 180,
        LS2502 = 181,
        LS2503 = 182,
        LS2504 = 183,
        LS2601 = 184,
        LS2701 = 185,
        LS2801 = 186,
        LS2901 = 187,
        LS3001 = 188,
        LS3101 = 189,
        LS3201 = 190,
        LS3301 = 191,
        LS3401 = 192,
        LS3501 = 193,
        LS3601 = 194,
        LS3701 = 195,
        LS3801 = 196,
        LS3901 = 197,
        LS4001 = 198
    }

    public enum TipoEmissaoNFe
    {
        Entrada = 0,
        Saida = 1
    }

    public static class TipoEmissaoNFeHelper
    {
        public static string ObterDescricao(this TipoEmissaoNFe tipoEmissao)
        {
            switch (tipoEmissao)
            {
                case TipoEmissaoNFe.Entrada: return "Entrada";
                case TipoEmissaoNFe.Saida: return "Saída";
                default: return string.Empty;
            }
        }
    }

    public enum FinalidadeNFe
    {
        Normal = 1,
        Complementar = 2,
        Ajuste = 3,
        Devolucao = 4
    }

    public static class FinalidadeNFeHelper
    {
        public static string ObterDescricao(this FinalidadeNFe finalidade)
        {
            switch (finalidade)
            {
                case FinalidadeNFe.Normal: return "Normal";
                case FinalidadeNFe.Complementar: return "Complementar";
                case FinalidadeNFe.Ajuste: return "Ajuste";
                case FinalidadeNFe.Devolucao: return "Devolução";
                default: return string.Empty;
            }
        }
    }

    public enum IndicadorPresencaNFe
    {
        NaoSeAplica = 0,
        Presencial = 1,
        Internet = 2,
        Teleatendimento = 3,
        NFCe = 4,
        PresencialForaEmpresa = 5,
        Outros = 9
    }

    public static class IndicadorPresencaNFeHelper
    {
        public static string ObterDescricao(this IndicadorPresencaNFe indicadorPresenca)
        {
            switch (indicadorPresenca)
            {
                case IndicadorPresencaNFe.NaoSeAplica: return "Não se aplica";
                case IndicadorPresencaNFe.Presencial: return "Presencial";
                case IndicadorPresencaNFe.Internet: return "Não presencial, Internet";
                case IndicadorPresencaNFe.Teleatendimento: return "Não presencial, Teleatendimento";
                case IndicadorPresencaNFe.NFCe: return "NFC-e entrega a domicílio";
                case IndicadorPresencaNFe.PresencialForaEmpresa: return "Presencial, fora da empresa";
                case IndicadorPresencaNFe.Outros: return "Não presencial, Outros";
                default: return string.Empty;
            }
        }
    }

    public enum IndicadorIntermediadorNFe
    {
        SemIntermediador = 0,
        SitePlataformaTerceiros = 1
    }

    public enum FormaPagamento
    {
        fpDinheiro = 1,
        fpCheque = 2,
        fpCartaoCredito = 3,
        fpCartaoDebito = 4,
        fpCreditoLoja = 5,
        fpValeAlimentacao = 10,
        fpValeRefeicao = 11,
        fpValePresente = 12,
        fpValeCombustivel = 13,
        fpDuplicataMercantil = 14,
        fpBoletoBancario = 15,
        fpDepositoBancario = 16,
        fpPagamentoInstantaneoPIX = 17,
        fpTransferenciabancaria = 18,
        fpProgramadefidelidade = 19,
        fpSemPagamento = 90,
        fpOutro = 99
    }

    public enum StatusNFe
    {
        Emitido = 1,
        Inutilizado = 2,
        Cancelado = 3,
        Autorizado = 4,
        Denegado = 5,
        Rejeitado = 6,
        EmProcessamento = 7,
        AguardandoAssinar = 8,
        AguardandoCancelarAssinar = 9,
        AguardandoInutilizarAssinar = 10,
        AguardandoCartaCorrecaoAssinar = 11
    }

    public static class StatusNFeHelper
    {
        public static string ObterDescricao(this StatusNFe statusNF)
        {
            switch (statusNF)
            {
                case StatusNFe.Emitido: return "Em Digitação";
                case StatusNFe.Inutilizado: return "Inutilizado";
                case StatusNFe.Cancelado: return "Cancelado";
                case StatusNFe.Autorizado: return "Autorizado";
                case StatusNFe.Denegado: return "Denegado";
                case StatusNFe.Rejeitado: return "Rejeitado";
                case StatusNFe.EmProcessamento: return "Em Processamento";
                case StatusNFe.AguardandoAssinar: return "Aguardando Assinatura do XML";
                case StatusNFe.AguardandoCancelarAssinar: return "Aguardando Cancelamento do XML";
                case StatusNFe.AguardandoInutilizarAssinar: return "Aguardando Inutilizacao do XML";
                case StatusNFe.AguardandoCartaCorrecaoAssinar: return "Aguardando Carta Correção do XML";
                default: return string.Empty;
            }
        }
    }

    public enum ModalidadeFrete
    {
        Emitente = 0,
        Destinatario = 1,
        Terceiros = 2,
        ProprioRemetente = 3,
        ProprioDestinatario = 4,
        SemFrete = 9
    }

    public static class ModalidadeFreteHelper
    {
        public static string ObterDescricao(this ModalidadeFrete modalidadeFrete)
        {
            switch (modalidadeFrete)
            {
                case ModalidadeFrete.Emitente: return "Contratação do Frete por conta do Remetente (CIF)";
                case ModalidadeFrete.Destinatario: return "Contratação do Frete por conta do Destinatário (FOB)";
                case ModalidadeFrete.Terceiros: return "Contratação do Frete por conta de Terceiros";
                case ModalidadeFrete.ProprioRemetente: return "Transporte Próprio por conta do Remetente";
                case ModalidadeFrete.ProprioDestinatario: return "Transporte Próprio por conta do Destinatário";
                case ModalidadeFrete.SemFrete: return "Sem Ocorrência de Transporte";
                default: return string.Empty;
            }
        }
    }

    public enum ViaTransporteInternacional
    {
        Maritima = 1,
        Fluvial = 2,
        Lacustre = 3,
        Aerea = 4,
        Postal = 5,
        Ferroviaria = 6,
        Rodoviaria = 7,
        CondutoRedeTransmissao = 8,
        MeiosProprios = 9,
        EntradaSaidaFicta = 10,
        Courier = 11,
        Handcarry = 12
    }

    public enum IntermediacaoImportacao
    {
        Propria = 1,
        ContaOrdem = 2,
        Encomenda = 3
    }

    public enum MotivoDesoneracaoICMS
    {
        Taxi = 1,
        DeficienteFisico = 2,
        ProdutorAgropecuario = 3,
        FrotistaLocadora = 4,
        DiplomaticoConsular = 5,
        UtilitariosMotocicleta = 6,
        SUFRAMA = 7,
        VendaOrgaoPublico = 8,
        Outros = 9,
        DeficienteCondutor = 10,
        DeficienteNaoCondutor = 11,
        FomentoEDesenvolvimentoAgropecuario = 12,
        OlimpiadasRio2016 = 16,
        SolicitadoPeloFisco = 90
    }

    public enum TipoDocumentoReferenciaNFe
    {
        NF = 1,
        NFModelo1 = 2,
        NFProdutorRural = 3,
        CTe = 4,
        CupomFiscal = 5
    }

    public enum TipoArquivoXML
    {
        Distribuicao = 1,
        Cancelamento = 2,
        CartaCorrecao = 3,
        Inutilizacao = 4,
        XMLSemAssinatura = 5,
        XMLCancelamentoNaoAssinado = 6,
        XMLInutilizacaoNaoAssinado = 7,
        XMLCartaCorrecaoNaoAssinado = 8
    }

    public enum TipoAcaoEmail
    {
        Atualizar = 1,
        Adicionar = 2,
        Excluir = 3
    }

    #endregion

    #region Integração Vale Pedagio CT-e

    public enum IntegradoraValePedagio
    {
        Nenhuma = 0,
        Target = 1,
        SemParar = 2
    }

    public enum TipoIntegracaoValePedagio
    {
        Autorizacao = 0,
        Cancelamento = 1
    }

    public enum TipoCompraValePedagio
    {
        Nenhum = 0,
        Cartao = 1,
        Tag = 2,
        Cupom = 3,
        ValePedagioAutoExpresso = 4
    }

    public enum TipoPagamentoValePedagio
    {
        Cartao = 1,
        Tag = 2,
    }

    public static class TipoPagamentoValePedagioHelper
    {
        public static string ObterDescricao(this TipoPagamentoValePedagio tipoPagamentoValePedagio)
        {
            switch (tipoPagamentoValePedagio)
            {
                case TipoPagamentoValePedagio.Cartao: return "Cartão";
                case TipoPagamentoValePedagio.Tag: return "Tag";
                default: return string.Empty;
            }
        }
    }

    public static class TipoCompraValePedagioHelper
    {
        public static string ObterDescricao(this TipoCompraValePedagio tipoCompraValePedagio)
        {
            switch (tipoCompraValePedagio)
            {
                case TipoCompraValePedagio.Nenhum: return "Nenhum";
                case TipoCompraValePedagio.Cartao: return "Cartão";
                case TipoCompraValePedagio.Tag: return "Tag";
                case TipoCompraValePedagio.Cupom: return "Cupom";
                default: return string.Empty;
            }
        }
    }

    public enum StatusIntegracaoValePedagio
    {
        Pendente = 0,
        Enviado = 1,
        Sucesso = 2,
        PendenteCancelamento = 3,
        Cancelado = 4,
        SemRota = 5,
        RotaSemCusto = 6,
        RejeicaoCancelamento = 8,
        RejeicaoCompra = 9,
        FilialNaoLiberadaParaCompra = 10,
        ProblemaNaIntegracao = 11,
        Todos = 99
    }


    public enum IntegradoraSM
    {
        Nenhuma = 0,
        Trafegus = 1,
        Buonny = 2
    }

    public enum TipoIntegracaoSM
    {
        Autorizacao = 0,
        Cancelamento = 1
    }

    public enum StatusIntegracaoSM
    {
        Pendente = 0,
        Enviado = 1,
        Sucesso = 2,
        PendenteCancelamento = 3,
        Cancelado = 4,
        CancelamentoRejeitado = 8,
        Rejeitado = 9
    }

    public enum TipoXMLValePedagio
    {
        BuscarRotaIBGE = 0,
        BuscarCustoRota = 1,
        ComprarValePedagio = 2,
        CancelarCompraValePedagio = 3,
        BuscarCartaoMotorista = 4,
        BuscarDocumento = 5,
        ConfirmacaoPedagioTAG = 6
    }

    public enum SituacaoCTeIntegracaoRetorno
    {
        Aguardando = 0,
        Sucesso = 1,
        Falha = 9
    }

    public enum TiposCargaTerceiros
    {
        Todas = 0,
        Terceiros = 1,
        Proprias = 2
    }

    #endregion

}

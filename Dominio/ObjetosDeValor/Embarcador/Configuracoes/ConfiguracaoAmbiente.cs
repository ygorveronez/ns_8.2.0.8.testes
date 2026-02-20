namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{

    public class ConfiguracaoAmbiente
    {
        public virtual int Codigo { get; set; }

        public virtual bool AmbienteProducao { get; set; }

        public virtual bool AmbienteSeguro { get; set; }

        public virtual string IdentificacaoAmbiente { get; set; }

        public virtual string CodigoLocalidadeNaoCadastrada { get; set; }

        public virtual bool? RecalcularICMSNaEmissaoCTe { get; set; }

        public virtual bool? AplicarValorICMSNoComplemento { get; set; }

        public virtual bool? AdicionarCTesFilaConsulta { get; set; }

        public virtual bool? NaoCalcularDIFALParaCSTNaoTributavel { get; set; }

        public virtual bool? NaoUtilizarColetaNaBuscaRotaFrete { get; set; }

        public virtual string CodificacaoEDI { get; set; }

        public virtual string LinkCotacaoCompra { get; set; }

        public virtual string LogoPersonalizadaFornecedor { get; set; }

        public virtual string LayoutPersonalizadoFornecedor { get; set; }

        public virtual bool? OcultarConteudoColog { get; set; }

        public virtual bool? ConsultarPeloCustoDaRota { get; set; }

        public virtual string ConcessionariasComDescontos { get; set; }

        public virtual string PercentualDescontoConcessionarias { get; set; }

        public virtual string PlacaPadraoConsultaValorPedagio { get; set; }

        public virtual bool? CalcularHorarioDoCarregamento { get; set; }

        public virtual bool? EnviarTodasNotificacoesPorEmail { get; set; }

        public virtual string APIOCRLink { get; set; }

        public virtual string APIOCRKey { get; set; }

        public virtual string QuantidadeSelecaoAgrupamentoCargaAutomatico { get; set; }

        public virtual string QuantidadeCargasAgrupamentoCargaAutomatico { get; set; }

        public virtual string HorarioExecucaoThreadDiaria { get; set; }

        public virtual bool? CalcularFreteFechamento { get; set; }

        public virtual bool? GerarDocumentoFechamento { get; set; }

        public virtual bool? NovoLayoutPortalFornecedor { get; set; }

        public virtual bool? NovoLayoutCabotagem { get; set; }

        public virtual string FornecedorTMS { get; set; }

        public virtual bool? UtilizarIntegracaoSaintGobainNova { get; set; }

        public virtual bool? FiltrarCargasPorProprietario { get; set; }

        public virtual bool? CargaControleEntrega_Habilitar_ImportacaoCargaFluvial { get; set; }

        public virtual string TipoArmazenamento { get; set; }

        public virtual string TipoArmazenamentoLeitorOCR { get; set; }

        public virtual string EnderecoFTP { get; set; }

        public virtual string UsuarioFTP { get; set; }

        public virtual string SenhaFTP { get; set; }

        public virtual string PortaFTP { get; set; }

        public virtual string PrefixosFTP { get; set; }

        public virtual string EmailsFTP { get; set; }

        public virtual bool? FTPPassivo { get; set; }

        public virtual bool? UtilizaSFTP { get; set; }

        public virtual bool? GerarNotFisPorNota { get; set; }

        public virtual string CodigoEmpresaMultisoftware { get; set; }

        public virtual string MinutosParaConsultaNatura { get; set; }

        public virtual string FiliaisNatura { get; set; }

        public virtual bool? UtilizarMetodoImportacaoTabelaFretePorServico { get; set; }

        public virtual bool? UtilizarLayoutImportacaoTabelaFreteGPA { get; set; }

        public virtual bool? ExibirSituacaoIntegracaoXMLGPA { get; set; }

        public virtual string WebServiceConsultaCTe { get; set; }

        public virtual bool? ProcessarCTeMultiCTe { get; set; }

        public virtual bool? NaoUtilizarCNPJTransportador { get; set; }

        public virtual bool? BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe { get; set; }

        public virtual bool? SempreUsarAtividadeCliente { get; set; }

        public virtual bool? AtualizarFantasiaClienteIntegracaoCTe { get; set; }

        public virtual bool? CadastrarMotoristaIntegracaoCTe { get; set; }

        public virtual bool? CTeUtilizaProprietarioCadastro { get; set; }

        public virtual bool? CTeCarregarVinculosVeiculosCadastro { get; set; }

        public virtual bool? CTeAtualizaTipoVeiculo { get; set; }

        public virtual bool? NaoAtualizarCadastroVeiculo { get; set; }
        public virtual bool? AgruparQuantidadesImportacaoCTe { get; set; }

        public virtual bool? EncerraMDFeAutomatico { get; set; }

        public virtual bool? EnviaContingenciaMDFeAutomatico { get; set; }

        public virtual bool? EnviarCertificadoOracle { get; set; }

        public virtual bool? EnviarCertificadoKeyVault { get; set; }

        public virtual string EmpresasUsuariosMultiCTe { get; set; }

        public virtual bool? LimparMotoristaIntegracaoVeiculo { get; set; }

        public virtual bool? LoginAD { get; set; }

        public virtual bool? RegerarDACTEOracle { get; set; }

        public virtual bool? ReenviarErroIntegracaoCTe { get; set; }

        public virtual bool? AtualizarTipoEmpresa { get; set; }

        public virtual bool? ValidarNFeJaImportada { get; set; }

        public virtual bool? UtilizaOptanteSimplesNacionalDaIntegracao { get; set; }

        public virtual bool? ReenviarErroIntegracaoMDFe { get; set; }

        public virtual bool? EncerraMDFeAutomaticoComMesmaData { get; set; }

        public virtual bool? EncerraMDFeAntesDaEmissao { get; set; }

        public virtual bool? EncerraMDFeAutomaticoOutrosSistemas { get; set; }

        public virtual bool? EnviarEmailMDFeClientes { get; set; }

        public virtual decimal? PesoMaximoIntegracaoCarga { get; set; }

        public virtual bool? UtilizarDocaDoComplementoFilial { get; set; }

        public virtual bool? RetornarModeloVeiculo { get; set; }

        public virtual bool? MDFeUtilizaDadosVeiculoCadastro { get; set; }

        public virtual bool? MDFeUtilizaVeiculoReboqueComoTracao { get; set; }

        public virtual bool? GerarCTeDasNFSeAutorizadas { get; set; }

        public virtual bool? IncluirISSNFSeLocalidadeTomadorDiferentePrestador { get; set; }

        public virtual bool? IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples { get; set; }

        public virtual bool? AtualizarValorFrete_AtualizarICMS { get; set; }

        public virtual bool? ConsultarDuplicidadeOracle { get; set; }

        public virtual bool? EnviarIntegracaoMagalogNoRetorno { get; set; }

        public virtual bool? EnviarIntegracaoErroMDFeMagalog { get; set; }

        public virtual string PrefixoMSMQ { get; set; }

        public virtual int IntervaloDocumentosFiscaisEmbarcador { get; set; }

        public virtual string EnderecoComputadorRemotoFila { get; set; }
        public virtual string EndpointServiceFila { get; set; }
        public virtual string UrlReportAPI { get; set; }

        public virtual bool? Autoscaling { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProtocolo? TipoProtocolo { get; set; }

        public virtual bool UtilizaLayoutPersonalizado
        {
            get
            {
                return LayoutPersonalizadoFornecedor switch
                {
                    "botoes-amarelo" or "botoes-amarelo-ppce" => true,
                    _ => false,
                };
            }
        }

        public virtual bool? DesabilitarPopUpsNotificacao { get; set; }
    }



}

using System.Linq;
using System.Reflection;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAmbiente : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente>
    {
        #region Constructores
        public ConfiguracaoAmbiente(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente>();

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente
            {
                Codigo = o.Codigo,
                AmbienteProducao = o.AmbienteProducao,
                AmbienteSeguro = o.AmbienteSeguro,
                IdentificacaoAmbiente = o.IdentificacaoAmbiente,
                CodigoLocalidadeNaoCadastrada = o.CodigoLocalidadeNaoCadastrada,
                RecalcularICMSNaEmissaoCTe = o.RecalcularICMSNaEmissaoCTe,
                AplicarValorICMSNoComplemento = o.AplicarValorICMSNoComplemento,
                AdicionarCTesFilaConsulta = o.AdicionarCTesFilaConsulta,
                NaoCalcularDIFALParaCSTNaoTributavel = o.NaoCalcularDIFALParaCSTNaoTributavel,
                NaoUtilizarColetaNaBuscaRotaFrete = o.NaoUtilizarColetaNaBuscaRotaFrete,
                CodificacaoEDI = o.CodificacaoEDI,
                LinkCotacaoCompra = o.LinkCotacaoCompra,
                LogoPersonalizadaFornecedor = o.LogoPersonalizadaFornecedor,
                LayoutPersonalizadoFornecedor = o.LayoutPersonalizadoFornecedor,
                OcultarConteudoColog = o.OcultarConteudoColog,
                ConsultarPeloCustoDaRota = o.ConsultarPeloCustoDaRota,
                ConcessionariasComDescontos = o.ConcessionariasComDescontos,
                PercentualDescontoConcessionarias = o.PercentualDescontoConcessionarias,
                PlacaPadraoConsultaValorPedagio = o.PlacaPadraoConsultaValorPedagio,
                CalcularHorarioDoCarregamento = o.CalcularHorarioDoCarregamento,
                EnviarTodasNotificacoesPorEmail = o.EnviarTodasNotificacoesPorEmail,
                APIOCRLink = o.APIOCRLink,
                APIOCRKey = o.APIOCRKey,
                QuantidadeSelecaoAgrupamentoCargaAutomatico = o.QuantidadeSelecaoAgrupamentoCargaAutomatico,
                QuantidadeCargasAgrupamentoCargaAutomatico = o.QuantidadeCargasAgrupamentoCargaAutomatico,
                HorarioExecucaoThreadDiaria = o.HorarioExecucaoThreadDiaria,
                CalcularFreteFechamento = o.CalcularFreteFechamento,
                GerarDocumentoFechamento = o.GerarDocumentoFechamento,
                NovoLayoutPortalFornecedor = o.NovoLayoutPortalFornecedor,
                NovoLayoutCabotagem = o.NovoLayoutCabotagem,
                FornecedorTMS = o.FornecedorTMS,
                UtilizarIntegracaoSaintGobainNova = o.UtilizarIntegracaoSaintGobainNova,
                FiltrarCargasPorProprietario = o.FiltrarCargasPorProprietario,
                CargaControleEntrega_Habilitar_ImportacaoCargaFluvial = o.CargaControleEntrega_Habilitar_ImportacaoCargaFluvial,
                TipoArmazenamento = o.TipoArmazenamento,
                TipoArmazenamentoLeitorOCR = o.TipoArmazenamentoLeitorOCR,
                EnderecoFTP = o.EnderecoFTP,
                UsuarioFTP = o.UsuarioFTP,
                SenhaFTP = o.SenhaFTP,
                PortaFTP = o.PortaFTP,
                PrefixosFTP = o.PrefixosFTP,
                EmailsFTP = o.EmailsFTP,
                FTPPassivo = o.FTPPassivo,
                UtilizaSFTP = o.UtilizaSFTP,
                GerarNotFisPorNota = o.GerarNotFisPorNota,
                CodigoEmpresaMultisoftware = o.CodigoEmpresaMultisoftware,
                MinutosParaConsultaNatura = o.MinutosParaConsultaNatura,
                FiliaisNatura = o.FiliaisNatura,
                UtilizarMetodoImportacaoTabelaFretePorServico = o.UtilizarMetodoImportacaoTabelaFretePorServico,
                UtilizarLayoutImportacaoTabelaFreteGPA = o.UtilizarLayoutImportacaoTabelaFreteGPA,
                ExibirSituacaoIntegracaoXMLGPA = o.ExibirSituacaoIntegracaoXMLGPA,
                WebServiceConsultaCTe = o.WebServiceConsultaCTe,
                ProcessarCTeMultiCTe = o.ProcessarCTeMultiCTe,
                NaoUtilizarCNPJTransportador = o.NaoUtilizarCNPJTransportador,
                BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe = o.BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe,
                SempreUsarAtividadeCliente = o.SempreUsarAtividadeCliente,
                AtualizarFantasiaClienteIntegracaoCTe = o.AtualizarFantasiaClienteIntegracaoCTe,
                CadastrarMotoristaIntegracaoCTe = o.CadastrarMotoristaIntegracaoCTe,
                CTeUtilizaProprietarioCadastro = o.CTeUtilizaProprietarioCadastro,
                CTeCarregarVinculosVeiculosCadastro = o.CTeCarregarVinculosVeiculosCadastro,
                CTeAtualizaTipoVeiculo = o.CTeAtualizaTipoVeiculo,
                NaoAtualizarCadastroVeiculo = o.NaoAtualizarCadastroVeiculo,
                AgruparQuantidadesImportacaoCTe = o.AgruparQuantidadesImportacaoCTe,
                EncerraMDFeAutomatico = o.EncerraMDFeAutomatico,
                EnviaContingenciaMDFeAutomatico = o.EnviaContingenciaMDFeAutomatico,
                EnviarCertificadoOracle = o.EnviarCertificadoOracle,
                EnviarCertificadoKeyVault = o.EnviarCertificadoKeyVault,
                EmpresasUsuariosMultiCTe = o.EmpresasUsuariosMultiCTe,
                LimparMotoristaIntegracaoVeiculo = o.LimparMotoristaIntegracaoVeiculo,
                LoginAD = o.LoginAD,
                RegerarDACTEOracle = o.RegerarDACTEOracle,
                ReenviarErroIntegracaoCTe = o.ReenviarErroIntegracaoCTe,
                AtualizarTipoEmpresa = o.AtualizarTipoEmpresa,
                ValidarNFeJaImportada = o.ValidarNFeJaImportada,
                UtilizaOptanteSimplesNacionalDaIntegracao = o.UtilizaOptanteSimplesNacionalDaIntegracao,
                ReenviarErroIntegracaoMDFe = o.ReenviarErroIntegracaoMDFe,
                EncerraMDFeAutomaticoComMesmaData = o.EncerraMDFeAutomaticoComMesmaData,
                EncerraMDFeAntesDaEmissao = o.EncerraMDFeAntesDaEmissao,
                EncerraMDFeAutomaticoOutrosSistemas = o.EncerraMDFeAutomaticoOutrosSistemas,
                EnviarEmailMDFeClientes = o.EnviarEmailMDFeClientes,
                PesoMaximoIntegracaoCarga = o.PesoMaximoIntegracaoCarga,
                UtilizarDocaDoComplementoFilial = o.UtilizarDocaDoComplementoFilial,
                RetornarModeloVeiculo = o.RetornarModeloVeiculo,
                MDFeUtilizaDadosVeiculoCadastro = o.MDFeUtilizaDadosVeiculoCadastro,
                MDFeUtilizaVeiculoReboqueComoTracao = o.MDFeUtilizaVeiculoReboqueComoTracao,
                GerarCTeDasNFSeAutorizadas = o.GerarCTeDasNFSeAutorizadas,
                IncluirISSNFSeLocalidadeTomadorDiferentePrestador = o.IncluirISSNFSeLocalidadeTomadorDiferentePrestador,
                IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples = o.IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples,
                AtualizarValorFrete_AtualizarICMS = o.AtualizarValorFrete_AtualizarICMS,
                ConsultarDuplicidadeOracle = o.ConsultarDuplicidadeOracle,
                EnviarIntegracaoMagalogNoRetorno = o.EnviarIntegracaoMagalogNoRetorno,
                EnviarIntegracaoErroMDFeMagalog = o.EnviarIntegracaoErroMDFeMagalog,
                IntervaloDocumentosFiscaisEmbarcador = o.IntervaloDocumentosFiscaisEmbarcador.HasValue ? o.IntervaloDocumentosFiscaisEmbarcador.Value : 0,
                PrefixoMSMQ = o.PrefixoMSMQ,
                EnderecoComputadorRemotoFila = o.EnderecoComputadorRemotoFila,
                EndpointServiceFila = o.EndpointServiceFila,
                UrlReportAPI = o.UrlReportAPI,
                Autoscaling = o.Autoscaling,
                TipoProtocolo = o.TipoProtocolo,
                DesabilitarPopUpsNotificacao = o.DesabilitarPopUpsDeNotificacao
            }).FirstOrDefault();


        }

        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente BuscarConfiguracaoDebugLocal()
        {
            var config = BuscarConfiguracaoPadrao();

            if (config == null)
                config = new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente();

            var fields = config.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var valorAtual = (string)field.GetValue(config);
                if (valorAtual == null)
                    valorAtual = "";
                else
                {
                    if (valorAtual.Contains("D:"))
                        valorAtual = valorAtual.Replace("D:", "C:");
                    if (valorAtual.Contains("F:"))
                        valorAtual = valorAtual.Replace("F:", "C:");


                    if (field.Name.Contains("PrefixoMSMQ"))
                        valorAtual = $"LOCAL_{valorAtual}";


                    field.SetValue(config, valorAtual);
                }
            }

            var fieldsBool = config.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == typeof(bool) || f.FieldType == typeof(bool?));

            foreach (var field in fieldsBool)
            {
                var valorAtual = (bool?)field.GetValue(config);
                if (valorAtual == null)
                {
                    valorAtual = false;
                    field.SetValue(config, valorAtual);
                }
            }



            return config;
        }

        #endregion
    }
}

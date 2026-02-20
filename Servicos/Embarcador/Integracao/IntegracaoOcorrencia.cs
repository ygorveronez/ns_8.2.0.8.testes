using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoOcorrencia : ServicoBase
    {        
        public IntegracaoOcorrencia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoOcorrencia(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #region Métodos Públicos

        public void InformarIntegracaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao ocorrenciaIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao();

            ocorrenciaIntegracao.CargaOcorrencia = ocorrencia;
            ocorrenciaIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            repOcorrenciaIntegracao.Inserir(ocorrenciaIntegracao);

        }

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoCTe serIntegracaoCte = new IntegracaoCTe(unitOfWork);
            IntegracaoEnvioProgramado serIntegracaoEnvioProgramado = new IntegracaoEnvioProgramado(unitOfWork);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao> ocorrenciaIntegracoes = repOcorrenciaIntegracao.BuscarPorOcorrencia(ocorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao ocorrenciaIntegracao in ocorrenciaIntegracoes)
            {
                //cada tipo de integração deve adicionar os documentos necessários nas filas
                switch (ocorrenciaIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverInfrutifera:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRecusa:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverOcorrencias:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, null, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, null, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                        if (ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros && ocorrencia.TipoOcorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo || ocorrencia.TipoOcorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                            serIntegracaoCte.AdcionarCTesDistintosParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        else if (ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo);
                        else
                            serIntegracaoCte.AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                        serIntegracaoCte.AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog:
                        serIntegracaoCte.AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars:
                        serIntegracaoEnvioProgramado.AdicionarIntegracaoProgramadaOcorrencia(ocorrencia, ocorrenciaIntegracao.TipoIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        serIntegracaoEnvioProgramado.AdicionarIntegracaoProgramadaOcorrencia(ocorrencia, ocorrenciaIntegracao.TipoIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork, false, new List<string> { "CT-e", "NFS-e", "NFS", "ND" });
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Olfar:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrencia, ocorrenciaIntegracao.TipoIntegracao.Tipo, unitOfWork, true, new List<string> { "ND" });
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task VerificarIntegracoesPendentesAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadraoAsync();

            Servicos.Embarcador.Integracao.IntegracaoCTe servicoIntegracaoCTe = new IntegracaoCTe(unitOfWork, _cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoEDI serIntegracaoEDI = new IntegracaoEDI(unitOfWork, tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = await servicoIntegracaoCTe.VerificarIntegracoesPendentesIndividuaisOcorrenciasAsync(unitOfWork, clienteMultisoftware, clienteURLAcesso, configuracaoOcorrencia);
            ocorrencias.AddRange(await serIntegracaoEDI.VerificarIntegracoesPendentesOcorrenciasAsync(unitOfWork));
            ocorrencias.AddRange(servicoIntegracaoCTe.VerificarIntegracoesPendentesLoteOcorrencia(unitOfWork));


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
            {
                AtualizarSituacaoOcorrenciaIntegracao(ocorrencia, configuracao, unitOfWork, StringConexao, tipoServicoMultisoftware);
            }
        }
        public static void AtualizarSituacaoOcorrenciaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Hubs.Ocorrencia svcHubOcorrencia = new Hubs.Ocorrencia();
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao ||
                ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao)
            {
                if (repOcorrenciaCTeIntegracao.ContarPorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                    repOcorrenciaEDIIntegracao.ContarPorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao;
                    ocorrencia.ReenviouIntegracao = false;
                    ocorrencia.ReenviouIntegracaoFilialEmissora = false;
                    repOcorrencia.Atualizar(ocorrencia);

                    //vai passar muitas vezes por aqui se for avon, por isso não deixa cair no hub da ocorrencia
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        return;
                }
                else if (repOcorrenciaCTeIntegracao.ContarPorOcorrencia(ocorrencia.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno }) > 0 ||
                         repOcorrenciaEDIIntegracao.ContarPorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    Servicos.Embarcador.Integracao.IntegracaoOcorrencia.IntegracaoFinalizada(ocorrencia, unitOfWork, tipoServicoMultisoftware);
                    ocorrencia.ReenviouIntegracao = false;
                    ocorrencia.ReenviouIntegracaoFilialEmissora = false;

                    repOcorrencia.Atualizar(ocorrencia);
                }

                svcHubOcorrencia.InformarOcorrenciaAtualizada(ocorrencia.Codigo);
            }
        }

        public static void IntegracaoFinalizada(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ocorrencia.ReenviouIntegracaoFilialEmissora)
            {
                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;
            }
            else if (ocorrencia.IntegrandoFilialEmissora && !ocorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            {
                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;
                ocorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = true;
                ocorrencia.GerouTodosDocumentos = false;
            }
            else
                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;


            if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada)
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(ocorrencia.Codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                {
                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, null, tipoServicoMultisoftware);

                    if (chamado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.AgIntegracao && chamado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.LiberadaOcorrencia)
                    {
                        chamado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Finalizado;

                        repChamado.Atualizar(chamado);
                    }
                }
            }
        }

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repIntegracaoEDI = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repIntegracaoCTe = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
            //Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao repIntegracaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao(unidadeDeTrabalho);

            if (repIntegracaoCTe.ContarPorOcorrenciaESituacaoDiff(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            if (repIntegracaoEDI.ContarPorOcorrenciaESituacaoDiff(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            //if (repIntegracaoOcorrencia.ContarPorOcorrenciaESituacaoDiff(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
            //    return true;

            return false;
        }

        public static void AdicionarIntegracoesOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracaoFrimesa = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux repIntegracaoElectrolux = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoComprovei repIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unidadeDeTrabalho);
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unidadeDeTrabalho);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repositorioGrupoTipoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa integracaoFrimesa = repIntegracaoFrimesa.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux integracaoElectrolux = repIntegracaoElectrolux.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei integracaoComprovei = repIntegracaoComprovei.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM integracaoKMM = repIntegracaoKMM.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus integracaoGlobus = repIntegracaoGlobus.Buscar();

            bool gerouIntegracaoPorTipoOcorrenciaEGrupoPessoa = false;
            Dominio.Entidades.Embarcador.Chamados.Chamado existeChamadoCriado = repositorioChamado.BuscarPorCarga(ocorrencia?.Carga?.Codigo ?? 0);

            #region Integração Frimesa
            if (integracaoFrimesa != null && !string.IsNullOrWhiteSpace(integracaoFrimesa.Usuario))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(ocorrencia.TipoOcorrencia.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(ocorrencia.Carga?.Codigo ?? 0);
                List<Dominio.Entidades.Cliente> tomadores = cargaPedidos.Select(o => o.ObterTomador()).ToList();

                if (tomadores.Count > 0)
                {
                    List<int> gruposPessoas = tomadores.Where(obj => obj.GrupoPessoas != null).Select(o => o.GrupoPessoas.Codigo).ToList();
                    List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracoesGrupoPessoas = new List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();
                    if (gruposPessoas.Count > 0)
                        integracoesGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGruposPessoas(gruposPessoas);

                    bool possuiIntegracaoFrimesaTipoOcorrencia = tiposIntegracao.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa);
                    bool possuiIntegracaoFrimesaTomadores = integracoesGrupoPessoas.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa);

                    if (possuiIntegracaoFrimesaTipoOcorrencia && possuiIntegracaoFrimesaTomadores)
                    {
                        GeraRegistroIntegracaoOcorrencia(ocorrencia, repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa), unidadeDeTrabalho);
                        gerouIntegracaoPorTipoOcorrenciaEGrupoPessoa = true;
                    }
                }
            }
            #endregion

            #region Integração Electrolux
            if (integracaoElectrolux != null && !string.IsNullOrWhiteSpace(integracaoElectrolux.Usuario))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(ocorrencia.TipoOcorrencia.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(ocorrencia.Carga?.Codigo ?? 0);
                List<Dominio.Entidades.Cliente> tomadores = cargaPedidos.Select(o => o.ObterTomador()).ToList();

                if (tomadores.Count > 0)
                {
                    List<int> gruposPessoas = tomadores.Where(obj => obj.GrupoPessoas != null).Select(o => o.GrupoPessoas.Codigo).ToList();
                    List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracoesGrupoPessoas = new List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa>();
                    if (gruposPessoas.Count > 0)
                        integracoesGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGruposPessoas(gruposPessoas);

                    bool possuiIntegracaoElectroluxTipoOcorrencia = tiposIntegracao.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux);
                    bool possuiIntegracaoElectroluxTomadores = integracoesGrupoPessoas.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux);

                    if (possuiIntegracaoElectroluxTipoOcorrencia && possuiIntegracaoElectroluxTomadores)
                    {
                        GeraRegistroIntegracaoOcorrencia(ocorrencia, repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux), unidadeDeTrabalho);
                        gerouIntegracaoPorTipoOcorrenciaEGrupoPessoa = true;
                    }
                }
            }
            #endregion

            if (ocorrencia.TipoOcorrencia != null && !gerouIntegracaoPorTipoOcorrenciaEGrupoPessoa)
            {
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(ocorrencia.TipoOcorrencia.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
            }

            if (existeChamadoCriado != null && existeChamadoCriado.MotivoChamado != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaoConfiguradasoNoGrupo = repositorioGrupoTipoChamado.BuscarTiposIntegracoesPorGrupo(existeChamadoCriado?.MotivoChamado?.GrupoMotivoChamado?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoes = repTipoIntegracao.BuscarPorTipos(tipoIntegracaoConfiguradasoNoGrupo);
                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoConfiguracao in tiposIntegracaoes)
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracaoConfiguracao, unidadeDeTrabalho);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.Cliente tomador = null;

                if (ocorrencia.Responsavel.HasValue)
                {
                    if (ocorrencia.Responsavel.Value == Dominio.Enumeradores.TipoTomador.Remetente)
                        tomador = repCliente.BuscarPorCPFCNPJ(cargaCTe.CTe?.Remetente.Cliente.Codigo ?? cargaCTe.PreCTe.Remetente.Cliente.Codigo);
                    else
                        tomador = repCliente.BuscarPorCPFCNPJ(cargaCTe.CTe?.Destinatario.Cliente.Codigo ?? cargaCTe.PreCTe.Destinatario.Cliente.Codigo);
                }
                else if ((cargaCTe.CTe?.Tomador?.Cliente != null) || cargaCTe.PreCTe?.Tomador != null)
                    tomador = repCliente.BuscarPorCPFCNPJ(cargaCTe.CTe?.Tomador.Cliente.Codigo ?? cargaCTe.PreCTe.Tomador.Cliente.Codigo);

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;
                if ((ocorrencia.Carga?.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && ocorrencia.Carga?.TipoOperacao?.TipoIntegracao != null)
                {
                    if (ocorrencia.Carga.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && ocorrencia.Carga.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada && repOcorrenciaIntegracao.ContarPorOcorrenciaETipoIntegracao(ocorrencia.Codigo, ocorrencia.Carga.TipoOperacao.TipoIntegracao.Tipo) <= 0)
                        tipoIntegracao = ocorrencia.Carga.TipoOperacao.TipoIntegracao;
                }

                if (tipoIntegracao == null && tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        if (tomador.TipoIntegracao != null && tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repOcorrenciaIntegracao.ContarPorOcorrenciaETipoIntegracao(ocorrencia.Codigo, tomador.TipoIntegracao.Tipo) <= 0)
                            tipoIntegracao = tomador.TipoIntegracao;
                    }
                    else if (tomador.GrupoPessoas != null)
                    {
                        if (tomador.GrupoPessoas.TipoIntegracao != null && tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repOcorrenciaIntegracao.ContarPorOcorrenciaETipoIntegracao(ocorrencia.Codigo, tomador.GrupoPessoas.TipoIntegracao.Tipo) <= 0)
                            tipoIntegracao = tomador.GrupoPessoas.TipoIntegracao;

                        if (ocorrencia.TipoOcorrencia.TipoOcorrenciaControleEntrega && tomador.GrupoPessoas.UtilizaMultiEmbarcador.HasValue && tomador.GrupoPessoas.UtilizaMultiEmbarcador.Value && tomador.GrupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador.HasValue && tomador.GrupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador.Value)
                            tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                    }
                }

                if (tipoIntegracao != null && tipoIntegracao.GerarIntegracaoNasOcorrencias)
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);

                if (ocorrencia.OcorrenciaRecebidaDeIntegracao != true && (tipoIntegracao == null || tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab) && (integracaoIntercab?.AtivarIntegracaoOcorrencias ?? false))
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }

                if (integracaoEMP != null && ocorrencia.OcorrenciaRecebidaDeIntegracao != true && (tipoIntegracao == null || tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP) && (integracaoEMP?.PossuiIntegracaoEMP ?? false) && integracaoEMP.AtivarIntegracaoOcorrenciaEMP)
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }


                if (integracaoEMP != null && ocorrencia.OcorrenciaRecebidaDeIntegracao != true && (tipoIntegracao == null || tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP) && integracaoEMP.AtivarIntegracaoNFTPEMP)
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }

                if (integracaoComprovei != null && ocorrencia.OcorrenciaRecebidaDeIntegracao != true && (tipoIntegracao == null || tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei) && (integracaoComprovei?.PossuiIntegracao ?? false) && ocorrencia.TipoOcorrencia.AtivarIntegracaoComprovei)
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }

                if (integracaoKMM != null && (tipoIntegracao == null || tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM) && (integracaoKMM?.PossuiIntegracao ?? false))
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }

                if (integracaoGlobus != null && integracaoGlobus?.PossuiIntegracao == true)
                {
                    tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus);
                    GeraRegistroIntegracaoOcorrencia(ocorrencia, tipoIntegracao, unidadeDeTrabalho);
                }
            }
        }

        #endregion

        #region Métodos Privados
        private static void GeraRegistroIntegracaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao ocorrenciaIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao
            {
                CargaOcorrencia = ocorrencia,
                TipoIntegracao = tipoIntegracao
            };
            repOcorrenciaIntegracao.Inserir(ocorrenciaIntegracao);
        }

        //private static bool ValidarGerarIntegracaoCarrefour(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        //{
        //    if (ocorrencia.ModeloDocumentoFiscal == null)
        //        return false;

        //    // Integra quando é débtio
        //    if (ocorrencia.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito)
        //        return true;

        //    // Integra quando é CTE
        //    if (ocorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
        //        return true;

        //    return false;
        //}

        //private void AdicionarOcorrenciaParaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        //{
        //    Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao(unidadeTrabalho);

        //    if (repOcorrenciaIntegracao.ContarPorOcorrencia(ocorrencia.Codigo) > 0)
        //        return;

        //    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao ocorrenciaIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao();

        //    ocorrenciaIntegracao.Ocorrencia = ocorrencia;
        //    ocorrenciaIntegracao.DataIntegracao = DateTime.Now;
        //    ocorrenciaIntegracao.NumeroTentativas = 0;
        //    ocorrenciaIntegracao.ProblemaIntegracao = "";
        //    ocorrenciaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
        //    ocorrenciaIntegracao.TipoIntegracao = tipoIntegracao;

        //    repOcorrenciaIntegracao.Inserir(ocorrenciaIntegracao);
        //}

        //private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> VerificarOcorrenciaIntegracaoPendentes(Repositorio.UnitOfWork unitOfWork)
        //{
        //    //todo: ver a possibilidade de tornar dinamico;
        //    int numeroTentativas = 2;
        //    double minutosACadaTentativa = 5;
        //    int numeroRegistrosPorVez = 15;

        //    Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao repOcorrenciaOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao(unitOfWork);

        //    List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao> integracoesPendentes = repOcorrenciaOcorrenciaIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

        //    List<Dominio.Entidades.Embarcador.Ocorrencias.Ocorrencia> ocorrencias = (from obj in integracoesPendentes select obj.Ocorrencia).Distinct().ToList();

        //    foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaOcorrenciaIntegracao integracaoPendente in integracoesPendentes)
        //    {
        //        switch (integracaoPendente.TipoIntegracao.Tipo)
        //        {
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
        //                Servicos.Embarcador.Integracao.OpenTech.IntegracaoOcorrenciaOpenTech.IntegrarOcorrencia(integracaoPendente, unitOfWork, StringConexao);
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return ocorrencias;
        //}

        #endregion
    }
}

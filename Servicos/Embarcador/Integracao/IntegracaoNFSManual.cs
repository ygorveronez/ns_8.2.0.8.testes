using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoNFSManual: ServicoBase
    {
        public IntegracaoNFSManual(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #region Métodos Públicos

        public void InformarIntegracaoCarga(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSIntegracao repLancamentoNFSIntegracao = new Repositorio.Embarcador.NFS.LancamentoNFSIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao lancamentoNFSManualIntegracao = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao();

            lancamentoNFSManualIntegracao.LancamentoNFSManual = lancamentoNFSManual;
            lancamentoNFSManualIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            repLancamentoNFSIntegracao.Inserir(lancamentoNFSManualIntegracao);
        }

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            IntegracaoCTe servicoIntegracaoCte = new IntegracaoCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                        if (ShouldAdicionarIntegracaoSaintgobain(lancamentoNFSManual))
                            servicoIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(lancamentoNFSManual, tipoIntegracao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa:
                        servicoIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(lancamentoNFSManual, tipoIntegracao, _unitOfWork);
                        break;
                }
            }
        }

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(lancamentoNFSManual, _unitOfWork);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe servicoIntegracaoCTe = new IntegracaoCTe(_unitOfWork,_tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentoNFSManuals = await servicoIntegracaoCTe.VerificarIntegracoesPendentesIndividuaisLancamentoNFSManualAsync();
            
            IntegracaoEDI servicoIntegracaoEDI = new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            lancamentoNFSManuals.AddRange(servicoIntegracaoCTe.VerificarIntegracoesNFSManualPendentesLote(_unitOfWork));
            lancamentoNFSManuals.AddRange(await servicoIntegracaoEDI.VerificarIntegracoesPendentesNFSManualAsync());

            lancamentoNFSManuals = lancamentoNFSManuals.Distinct().ToList();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();

            foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual in lancamentoNFSManuals)
            {
                AtualizarSituacaoNFSManualIntegracao(lancamentoNFSManual, configuracao, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);
            }
        }

        #endregion

        #region Métodos Públicos Estáticos

        public static void AtualizarSituacaoNFSManualIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            
            Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();
            
            lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(lancamentoNFSManual.Codigo);

            if (lancamentoNFSManual.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgIntegracao)
            {
                if (repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                    repNFSManualEDIIntegracao.ContarPorLancamentoNFSManual(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.FalhaIntegracao;
                    repLancamentoNFSManual.Atualizar(lancamentoNFSManual);

                    //vai passar muitas vezes por aqui se for avon, por isso não deixa cair no hub da lancamentoNFSManual
                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        return;
                }
                else if (repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(lancamentoNFSManual.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno }) > 0 ||
                         repNFSManualEDIIntegracao.ContarPorLancamentoNFSManual(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Finalizada;
                    lancamentoNFSManual = AtualizarNumeracaoNFSManualIntegracao(lancamentoNFSManual, unitOfWork);

                    repLancamentoNFSManual.Atualizar(lancamentoNFSManual);

                }
                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);
            }
        }

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

            if (repNFSManualCTeIntegracao.ContarPorLancamentoNFSManualESituacaoDiff(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            if (repNFSManualEDIIntegracao.ContarPorLancamentoNFSManualESituacaoDiff(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        public static void AdicionarIntegracoesCarga(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSIntegracao repLancamentoNFSIntegracao = new Repositorio.Embarcador.NFS.LancamentoNFSIntegracao(unidadeDeTrabalho);

            Dominio.Entidades.Cliente tomador = lancamentoNFSManual.Tomador;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao = null;

            if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                {
                    if (tomador.TipoIntegracao != null && tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repLancamentoNFSIntegracao.ContarPorLancamentoNFSManualETipoIntegracao(lancamentoNFSManual.Codigo, tomador.TipoIntegracao.Tipo) <= 0)
                        TipoIntegracao = tomador.TipoIntegracao;
                }
                else
                {
                    if (tomador.GrupoPessoas.TipoIntegracao != null && tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repLancamentoNFSIntegracao.ContarPorLancamentoNFSManualETipoIntegracao(lancamentoNFSManual.Codigo, tomador.GrupoPessoas.TipoIntegracao.Tipo) <= 0)
                        TipoIntegracao = tomador.GrupoPessoas.TipoIntegracao;
                }
            }

            if (TipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao lancamentoNFSManualIntegracao = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao();
                lancamentoNFSManualIntegracao.LancamentoNFSManual = lancamentoNFSManual;
                lancamentoNFSManualIntegracao.TipoIntegracao = TipoIntegracao;
                repLancamentoNFSIntegracao.Inserir(lancamentoNFSManualIntegracao);
            }

        }

        public static Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual AtualizarNumeracaoNFSManualIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {

            if (lancamentoNFSManual?.CTe?.Numero != 0)
            {
                lancamentoNFSManual.DadosNFS.Numero = lancamentoNFSManual.CTe.Numero;
            }

            return lancamentoNFSManual;
        }
        #endregion

        #region Métodos Privados

        private bool ShouldAdicionarIntegracaoSaintgobain(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            return lancamentoNFSManual.Tomador?.GrupoPessoas?.ControlaPagamentos ?? false;
        }

        #endregion
    }
}

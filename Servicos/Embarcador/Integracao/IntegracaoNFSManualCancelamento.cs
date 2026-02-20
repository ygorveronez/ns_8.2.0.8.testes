using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoNFSManualCancelamento : ServicoBase
    {
        #region Construtores
       
        public IntegracaoNFSManualCancelamento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }      
        
        public IntegracaoNFSManualCancelamento() : base()
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoCTe serIntegracaoCte = new IntegracaoCTe();

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracao repNFSManualCancelamentoIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> nfsManualCancelamentoIntegracoes = repNFSManualCancelamentoIntegracao.BuscarPorNFSManualCancelamento(nfsManualCancelamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao nfsManualCancelamentoIntegracao in nfsManualCancelamentoIntegracoes)
            {
                //cada tipo de integração deve adicionar os documentos necessários nas filas
                switch (nfsManualCancelamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(nfsManualCancelamentoIntegracao.NFSManualCancelamento, nfsManualCancelamentoIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(nfsManualCancelamentoIntegracao.NFSManualCancelamento, nfsManualCancelamentoIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(nfsManualCancelamentoIntegracao.NFSManualCancelamento, nfsManualCancelamentoIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(nfsManualCancelamentoIntegracao.NFSManualCancelamento, nfsManualCancelamentoIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(nfsManualCancelamento, unitOfWork, tipoServicoMultisoftware);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe servicoIntegracaoCTe = new IntegracaoCTe(_unitOfWork,_tipoServicoMultisoftware, _cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoEDI servicoIntegracaoEDI = new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentos = await servicoIntegracaoCTe.VerificarIntegracoesPendentesIndividuaisNFSManualCancelamentoAsync();

            nfsManualCancelamentos.AddRange(await servicoIntegracaoEDI.VerificarIntegracoesPendentesNFSManualCancelamentoAsync());

            nfsManualCancelamentos = nfsManualCancelamentos.Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento in nfsManualCancelamentos)
            {
                AtualizarSituacaoNFSManualCancelamentoIntegracao(nfsManualCancelamento, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);
            }
        }

        public static void AtualizarSituacaoNFSManualCancelamentoIntegracao(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unitOfWork);

            nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(nfsManualCancelamento.Codigo);

            if (nfsManualCancelamento.SituacaoNFSManualCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.AgIntegracao)
            {
                if (repNFSManualCancelamentoIntegracaoCTe.ContarPorNFSManualCancelamento(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                    repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.IntegracaoRejeitada;

                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                    //vai passar muitas vezes por aqui se for avon, por isso não deixa cair no hub da lancamentoNFSManual
                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        return;
                }
                else if (repNFSManualCancelamentoIntegracaoCTe.ContarPorNFSManualCancelamento(nfsManualCancelamento.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno }) > 0 ||
                         repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada;

                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);
                }
            }
        }

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

            if (repNFSManualCancelamentoIntegracaoCTe.ContarPorNFSManualCancelamentoESituacaoDiff(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            if (repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamentoESituacaoDiff(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        public void AdicionarNFSManualCancelamentoParaIntegracao(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoCTe serIntegracaoCte = new IntegracaoCTe();

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repositorioNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();
            Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe = repositorioNFSManualCancelamentoIntegracaoCTe.BuscarPorCodigo(nfsManualCancelamento.Codigo);

            if (nfsManualCancelamentoIntegracaoCTe?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || nfsManualCancelamentoIntegracaoCTe != null)
                return;


            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if (nfsManualCancelamento.SituacaoNFSManualCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada)
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(nfsManualCancelamento, tipoIntegracao, unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}

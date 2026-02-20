using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaMDFeManualCancelamento
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoCargaMDFeManualCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        AdcionarCargaMDFeManualCancelamentoIntegracaoTrizy(cargaMDFeManualCancelamento, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe serIntegracaoCTe = new IntegracaoCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> cargasMDFeManualCancelamento = this.VerificarIntegracoesPendentesCargaMDFeManualCancelamento(_unitOfWork);

            cargasMDFeManualCancelamento = cargasMDFeManualCancelamento.Distinct().ToList();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento in cargasMDFeManualCancelamento)
            {
                AtualizarSituacaoCargaMDFeManualCancelamentoIntegracao(cargaMDFeManualCancelamento, configuracao, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);
            }
        }

        public static void AtualizarSituacaoCargaMDFeManualCancelamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            cargaMDFeManualCancelamento = repCargaMDFeManualCancelamento.BuscarPorCodigo(cargaMDFeManualCancelamento.Codigo, false);

            if (cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.AgIntegracao)
            {
                if (repCargaMDFeManualCancelamentoIntegracao.ContarPorCargaMDFeManualCancelamento(cargaMDFeManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.FalhaIntegracao;
                    repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);

                }
                else if (repCargaMDFeManualCancelamentoIntegracao.ContarPorCargaMDFeManualCancelamento(cargaMDFeManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    cargaMDFeManualCancelamento.CargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Cancelado;
                    cargaMDFeManualCancelamento.CargaMDFeManual.SituacaoCancelamento = cargaMDFeManualCancelamento.CargaMDFeManual.Situacao;
                    cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.Cancelada;

                    repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);
                    repCargaMDFeManual.Atualizar(cargaMDFeManualCancelamento.CargaMDFeManual);

                }
            }
        }
        #endregion

        #region Métodos Públicos Estáticos

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unidadeDeTrabalho);

            if (repCargaMDFeManualCancelamento.ContarPorCargaMDFeManualCancelamentoESituacaoDiff(cargaMDFeManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;


            return false;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> VerificarIntegracoesPendentesCargaMDFeManualCancelamento(Repositorio.UnitOfWork unitOfWork)
        {
            //todo: ver a possibilidade de tornar dinamico;
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> cargasMDFeManualCancelamentoIntegracao = repCargaMDFeManualCancelamentoIntegracao.BuscarCargaMDFeManualCancelamentoIntegracaoPendente(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> cargasMDFeManualCancelamento = (from obj in cargasMDFeManualCancelamentoIntegracao select obj.CargaMDFeManualCancelamento).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeManualCancelamentoIntegracao in cargasMDFeManualCancelamentoIntegracao)
            {
                switch (cargaMDFeManualCancelamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMDFeManualCancelamento(cargaMDFeManualCancelamentoIntegracao, unitOfWork);
                        break;
                    default:
                        break;
                }

                repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeManualCancelamentoIntegracao);
            }

            return cargasMDFeManualCancelamento;
        }
        public void AdcionarCargaMDFeManualCancelamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repositorioCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);

            if (repositorioCargaMDFeManualCancelamentoIntegracao.ExistePorCargaMDFeManualCancelamentoETipo(cargaMDFeManualCancelamento.Codigo, mdfe.Codigo, tipoIntegracao))
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCargaMDFeManualCancelamento = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeManualCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao()
            {
                CargaMDFeManualCancelamento = cargaMDFeManualCancelamento,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoCargaMDFeManualCancelamento,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                MDFe = mdfe
            };

            repositorioCargaMDFeManualCancelamentoIntegracao.Inserir(cargaMDFeManualCancelamentoIntegracao);
        }

        private void AdcionarCargaMDFeManualCancelamentoIntegracaoTrizy(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes == null || cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes.Count() == 0 || !(configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false) || !integracaoTrizy.DocumentosFiscaisEnvioPDF.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy.MDFe))
                return;

            foreach (var mdfe in cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCargaMDFe.BuscarCargaPorMDFe(mdfe.Codigo);
                if (carga == null)
                    return;

                if (((carga.Motoristas == null) || !(carga.Motoristas.Any()) || (carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false)) && !(carga.Filial?.HabilitarPreViagemTrizy ?? false) && (carga.Empresa?.NaoGerarIntegracaoSuperAppTrizy ?? false))
                    return;

                if (integracaoTrizy?.ValidarIntegracaoPorOperacao ?? false)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unitOfWork);
                    if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy))
                        return;
                }

                AdcionarCargaMDFeManualCancelamentoIntegracao(cargaMDFeManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unitOfWork, mdfe.MDFe);
            }

        }
        #endregion

    }
}

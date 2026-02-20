using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaMDFeManual
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoCargaMDFeManual(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork; 
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual)
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
                        AdcionarCargaMDFeManualIntegracaoTrizy(cargaMDFeManual, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe serIntegracaoCTe = new IntegracaoCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> cargasMDFeManual = this.VerificarIntegracoesPendentesCargaMDFeManual(_unitOfWork);

            cargasMDFeManual = cargasMDFeManual.Distinct().ToList();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual in cargasMDFeManual)
            {
                AtualizarSituacaoCargaMDFeManualIntegracao(cargaMDFeManual, configuracao, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);
            }
        }

        public static void AtualizarSituacaoCargaMDFeManualIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(cargaMDFeManual.Codigo, false);

            if (cargaMDFeManual.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao)
            {
                if (repCargaMDFeManualIntegracao.ContarPorCargaMDFeManual(cargaMDFeManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.FalhaIntegracao;
                    repCargaMDFeManual.Atualizar(cargaMDFeManual);

                }
                else if (repCargaMDFeManualIntegracao.ContarPorCargaMDFeManual(cargaMDFeManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Finalizado;

                    repCargaMDFeManual.Atualizar(cargaMDFeManual);

                }
            }
        }
        #endregion

        #region Métodos Públicos Estáticos

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeDeTrabalho);

            if (repCargaMDFeManual.ContarPorCargaMDFeManualESituacaoDiff(cargaMDFeManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;


            return false;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> VerificarIntegracoesPendentesCargaMDFeManual(Repositorio.UnitOfWork unitOfWork)
        {
            //todo: ver a possibilidade de tornar dinamico;
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> cargasMDFeManualIntegracao = repCargaMDFeManualIntegracao.BuscarCargaMDFeManualIntegracaoPendente(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> cargasMDFeManual = (from obj in cargasMDFeManualIntegracao select obj.CargaMDFeManual).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao in cargasMDFeManualIntegracao)
            {
                switch (cargaMDFeManualIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMDFeManual(cargaMDFeManualIntegracao, unitOfWork);
                        break;
                    default:
                        break;
                }

                repCargaMDFeManualIntegracao.Atualizar(cargaMDFeManualIntegracao);
            }

            return cargasMDFeManual;
        }
        public void AdcionarCargaMDFeManualIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repositorioCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);

            if (repositorioCargaMDFeManualIntegracao.ExistePorCargaMDFeManualETipo(cargaMDFeManual.Codigo, mdfe.Codigo, tipoIntegracao))
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCargaMDFeManual = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao()
            {
                CargaMDFeManual = cargaMDFeManual,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoCargaMDFeManual,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                MDFe = mdfe
            };

            repositorioCargaMDFeManualIntegracao.Inserir(cargaMDFeManualIntegracao);
        }

        private void AdcionarCargaMDFeManualIntegracaoTrizy(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (cargaMDFeManual.MDFeManualMDFes == null || cargaMDFeManual.MDFeManualMDFes.Count() == 0 || !(configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false) || !integracaoTrizy.DocumentosFiscaisEnvioPDF.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy.MDFe))
                return;

            foreach (var mdfe in cargaMDFeManual.MDFeManualMDFes)
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

                AdcionarCargaMDFeManualIntegracao(cargaMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unitOfWork, mdfe.MDFe);
            }           

        }
        #endregion

    }
}

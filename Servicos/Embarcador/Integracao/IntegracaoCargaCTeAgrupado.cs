using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaCTeAgrupado
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoCargaCTeAgrupado(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdcionarCargaCTeAgrupadoIntegracao(cargaCTeAgrupado, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Autorizacao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        AdcionarCargaCTeAgrupadoIntegracao(cargaCTeAgrupado, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Autorizacao, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }
        public void IniciarIntegracoesComDocumentosCancelamento(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdcionarCargaCTeAgrupadoIntegracao(cargaCTeAgrupado, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        AdcionarCargaCTeAgrupadoIntegracao(cargaCTeAgrupado, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe serIntegracaoCTe = new IntegracaoCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> cargasCTeAgrupado = this.VerificarIntegracoesPendentesCargaCTeAgrupado(_unitOfWork);

            cargasCTeAgrupado = cargasCTeAgrupado.Distinct().ToList();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado in cargasCTeAgrupado)
            {
                AtualizarSituacaoCargaCTeAgrupadoIntegracao(cargaCTeAgrupado, configuracao, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);
            }
        }

        public static void AtualizarSituacaoCargaCTeAgrupadoIntegracao(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);

            cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupado.Codigo, false);

            if (cargaCTeAgrupado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.AgIntegracao)
            {
                if (repCargaCTeAgrupadoIntegracao.ContarPorCargaCTeAgrupado(cargaCTeAgrupado.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.FalhaIntegracao;
                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                    if (cargaCTeAgrupado.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        return;
                }
                else if (repCargaCTeAgrupadoIntegracao.ContarPorCargaCTeAgrupado(cargaCTeAgrupado.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0 )
                {
                    return;
                }
                else
                {
                    cargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Finalizado;

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                }
            }
        }
        #endregion

        #region Métodos Públicos Estáticos

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(unidadeDeTrabalho);

            if (repCargaCTeAgrupado.ContarPorCargaCTeAgrupadoESituacaoDiff(cargaCTeAgrupado.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;


            return false;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> VerificarIntegracoesPendentesCargaCTeAgrupado(Repositorio.UnitOfWork unitOfWork)
        {
            //todo: ver a possibilidade de tornar dinamico;
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> cargasCTeAgrupadoIntegracao = repCargaCTeAgrupadoIntegracao.BuscarCargaCTeAgrupadoIntegracaoPendente(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> cargasCTeAgrupado = (from obj in cargasCTeAgrupadoIntegracao select obj.CargaCTeAgrupado).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao cargaCTeAgrupadoIntegracao in cargasCTeAgrupadoIntegracao)
            {
                switch (cargaCTeAgrupadoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if(cargaCTeAgrupadoIntegracao.TipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Autorizacao)
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCargaCTeAgrupado(cargaCTeAgrupadoIntegracao);
                        else
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCancelamentoCargaCTeAgrupado(cargaCTeAgrupadoIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (cargaCTeAgrupadoIntegracao.TipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Autorizacao)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCargaCTeAgrupado(cargaCTeAgrupadoIntegracao);
                        else
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCancelamentoCargaCTeAgrupado(cargaCTeAgrupadoIntegracao);
                        break;
                    default:
                        break;
                }

                repCargaCTeAgrupadoIntegracao.Atualizar(cargaCTeAgrupadoIntegracao);
            }

            return cargasCTeAgrupado;
        }
        public void AdcionarCargaCTeAgrupadoIntegracao(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repositorioCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(unitOfWork);

            if (repositorioCargaCTeAgrupadoIntegracao.ExistePorCargaCTeAgrupadoETipo(cargaCTeAgrupado.Codigo, tipoIntegracao, tipoAcaoIntegracao))
                return;

            if(tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
            {
                if ((repositorioCargaCTeAgrupadoIntegracao.BuscarPorCargaCTeAgrupadoETipo(cargaCTeAgrupado.Codigo, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Autorizacao)?.FirstOrDefault()?.SituacaoIntegracao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) 
                    != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCargaCTeAgrupado = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao cargaCTeAgrupadoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao()
            {
                CargaCTeAgrupado = cargaCTeAgrupado,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoCargaCTeAgrupado,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                TipoAcaoIntegracao = tipoAcaoIntegracao,
            };

            repositorioCargaCTeAgrupadoIntegracao.Inserir(cargaCTeAgrupadoIntegracao);
        }
        #endregion

    }
}

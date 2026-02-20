using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatioIntegracao
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados

        #region Construtores

        public FluxoGestaoPatioIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapa)
        {
            List<TipoIntegracao> tiposPermitidos = new List<TipoIntegracao>() { TipoIntegracao.Deca, TipoIntegracao.P44, TipoIntegracao.Trizy, TipoIntegracao.Senior, TipoIntegracao.Pager, TipoIntegracao.Bind };
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposPermitidos);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Deca:
                        if (new Integracao.Deca.IntegracaoDeca(_unitOfWork).ValidarEtapaIntegracao(etapa))
                            AdicionarIntegracaoCarga(fluxoGestaoPatio.Carga, etapa, tipoIntegracao);
                        break;

                    case TipoIntegracao.P44:
                        break;

                    case TipoIntegracao.Trizy:
                        if (new Integracao.Trizy.IntegracaoTrizy(_unitOfWork).ValidarEtapaIntegracao(etapa))
                            AdicionarIntegracaoCarga(fluxoGestaoPatio.Carga, etapa, tipoIntegracao);
                        break;

                    case TipoIntegracao.Senior:
                        if (new Integracao.Senior.IntegracaoSenior(_unitOfWork).ValidarEtapaIntegracao(etapa))
                            AdicionarIntegracaoCarga(fluxoGestaoPatio.Carga, etapa, tipoIntegracao);
                        break;

                    case TipoIntegracao.Pager:
                        if (new Integracao.Pager.IntegracaoPager(_unitOfWork).ValidarEtapaIntegracao(etapa, fluxoGestaoPatio))
                            AdicionarIntegracaoCarga(fluxoGestaoPatio.Carga, etapa, tipoIntegracao);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> integracoesPendentes = repositorioFluxoPatioIntegracao.BuscarIntegracaoesPententes();

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracao in integracoesPendentes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Deca:
                        if (integracao.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem)
                            new Integracao.Deca.IntegracaoDeca(_unitOfWork).IntegrarInicioViagem(integracao);
                        else
                            new Integracao.Deca.IntegracaoDeca(_unitOfWork).Integrar(integracao);
                        break;

                    case TipoIntegracao.P44:
                        new Integracao.P44.IntegracaoP44(_unitOfWork).Integrar(integracao);
                        break;

                    case TipoIntegracao.Trizy:
                        new Integracao.Trizy.IntegracaoTrizy(_unitOfWork).Integrar(integracao);
                        break;

                    case TipoIntegracao.Senior:
                        new Integracao.Senior.IntegracaoSenior(_unitOfWork).Integrar(integracao);
                        break;

                    case TipoIntegracao.Pager:
                        new Integracao.Pager.IntegracaoPager(_unitOfWork).Integrar(integracao);
                        break;

                    case TipoIntegracao.Bind:
                        new Integracao.Bind.IntegracaoBind(_unitOfWork).Integrar(integracao);
                        break;

                    default:
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.ProblemaIntegracao = "Tipo de integração não implementada";
                        repositorioFluxoPatioIntegracao.Atualizar(integracao);
                        break;
                }
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao AdicionarIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);

            if (repositorioFluxoPatioIntegracao.ExistePorEtapaETipoIntegracao(carga.Codigo, etapa, tipoIntegracao.Codigo))
                return null;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao novoRegistroFluxoPatioIntegracao = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao()
            {
                NumeroTentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                EtapaFluxoGestaoPatio = etapa,
                TipoIntegracao = tipoIntegracao,
                Carga = carga,
                Pedido = pedido,
            };

            repositorioFluxoPatioIntegracao.Inserir(novoRegistroFluxoPatioIntegracao);
            return novoRegistroFluxoPatioIntegracao;
        }

        private void AdicionarIntegracaoCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (carga.CargaAgrupada)
                cargas.AddRange(carga.CargasAgrupamento);
            else
                cargas.Add(carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAdicionarIntegracao in cargas)
                AdicionarIntegracaoCarga(cargaAdicionarIntegracao, etapa, tipoIntegracao);
        }

        public async Task<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> AdicionarIntegracaoCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);

            if (repositorioFluxoPatioIntegracao.ExistePorEtapaETipoIntegracao(carga.Codigo, etapa, tipoIntegracao.Codigo))
                return null;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao novoRegistroFluxoPatioIntegracao = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao()
            {
                NumeroTentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                EtapaFluxoGestaoPatio = etapa,
                TipoIntegracao = tipoIntegracao,
                Carga = carga,
                Pedido = pedido,
            };

            await repositorioFluxoPatioIntegracao.InserirAsync(novoRegistroFluxoPatioIntegracao);
            return novoRegistroFluxoPatioIntegracao;
        }

        #endregion Métodos Privados
    }
}

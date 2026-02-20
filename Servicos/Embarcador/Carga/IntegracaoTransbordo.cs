using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoTransbordo
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoTransbordo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public bool AdicionarIntegracoesTransbordo(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork).BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> listaTipoIntegracaoAmbiente = repTipoIntegracao.BuscarAtivos();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> listaCargaCargaIntegracao = repositorioCargaCargaIntegracao.BuscarPorCarga(transbordo.Carga.Codigo);
            bool gerouIntegracao = false;

            if (listaTipoIntegracaoAmbiente == null || listaTipoIntegracaoAmbiente.Count == 0)
                return gerouIntegracao;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in listaTipoIntegracaoAmbiente)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Trizy:
                        if (configuracaoIntegracao.PossuiIntegracaoTrizy &&
                            listaCargaCargaIntegracao.Exists(integracao => integracao.TipoIntegracao.Tipo == tipoIntegracao.Tipo))
                        {
                            GerarTransbordoIntegracao(transbordo, tipoIntegracao, unitOfWork);
                            gerouIntegracao = true;
                        }
                        break;
                    default:
                        continue;
                }
            }
            return gerouIntegracao;
        }

        public void VerificarIntegracoesPendentes(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.TransbordoIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.TransbordoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> listaIntegracaoPendente = repIntegracao.BuscarListaIntegracaoPendente();

            List<Dominio.Entidades.Embarcador.Cargas.Transbordo> listaTransbordo = new List<Dominio.Entidades.Embarcador.Cargas.Transbordo>();
            foreach (Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao integracaoPendente in listaIntegracaoPendente)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Trizy:
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarTransbordo(integracaoPendente.Transbordo.Carga, "CANCELED", _unitOfWork, integracaoPendente);
                        break;
                    default:
                        break;
                }
                if (!listaTransbordo.Contains(integracaoPendente.Transbordo))
                    listaTransbordo.Add(integracaoPendente.Transbordo);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo in listaTransbordo)
            {
                if (transbordo.SituacaoTransbordo == SituacaoTransbordo.Finalizado)
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> integracoes = repIntegracao.BuscarPorTransbordo(transbordo.Codigo);

                if (integracoes.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao))
                    transbordo.SituacaoTransbordo = SituacaoTransbordo.FalhaIntegracao;
                else if (integracoes.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao))
                    transbordo.SituacaoTransbordo = SituacaoTransbordo.AgIntegracao;
                else if (integracoes.All(obj => obj.SituacaoIntegracao == SituacaoIntegracao.Integrado))
                {
                    Servicos.Embarcador.Carga.Transbordo svcTransbordo = new Servicos.Embarcador.Carga.Transbordo(_unitOfWork, tipoServicoMultisoftware, auditado);
                    svcTransbordo.GerarCargaTransbordo(transbordo, clienteMultisoftware);
                    transbordo.SituacaoTransbordo = SituacaoTransbordo.Finalizado;
                }

                repTransbordo.Atualizar(transbordo);

                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                servicoHubCarga.InformarTransbordoAtualizado(transbordo.Codigo);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void GerarTransbordoIntegracao(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TransbordoIntegracao repTransbordoIntegracao = new Repositorio.Embarcador.Cargas.TransbordoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao transbordoIntegracao = new Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao()
            {
                Transbordo = transbordo,
                NumeroTentativas = 0,
                ProblemaIntegracao = string.Empty,
                TipoIntegracao = tipoIntegracao,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                DataIntegracao = DateTime.Now
            };
            repTransbordoIntegracao.Inserir(transbordoIntegracao);
        }

        #endregion Métodos Privados
    }
}

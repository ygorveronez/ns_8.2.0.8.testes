using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaEvento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos

        #region Construtores

        public IntegracaoCargaEvento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AdicionarIntegracaoIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaCarga etapa, string mensagem, List<TipoIntegracao> tiposIntegracao, bool sucesso = false)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> listaTipoIntegracaoAmbiente = repTipoIntegracao.BuscarPorTipos(tiposIntegracao);

            if (listaTipoIntegracaoAmbiente == null || listaTipoIntegracaoAmbiente.Count == 0 || carga == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in listaTipoIntegracaoAmbiente)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case TipoIntegracao.ArcelorMittal:
                        if (_tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                            GerarIntegracaoEvento(carga, tipoIntegracao, etapa, mensagem, sucesso);
                        break;
                    default:
                        continue;
                }
            }
            return;
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> listaIntegracaoPendente = repIntegracao.BuscarListaIntegracaoPendente();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracaoPendente in listaIntegracaoPendente)
            {
                Servicos.Log.TratarErro($"Integrado : {integracaoPendente.Codigo}", "IntegracaoArcelorMittal");
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.ArcelorMittal:
                        new Servicos.Embarcador.Integracao.ArcelorMittal.IntegracaoArcelorMittal(_unitOfWork).IntegrarCargaEvento(integracaoPendente);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void GerarIntegracaoEvento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, EtapaCarga etapa, string mensagem, bool sucesso)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(_unitOfWork);
            var integracao = repIntegracao.BuscarPorCargaEtapa(carga.Codigo, etapa);
            if (integracao != null)
            {
                if (integracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    integracao.ProblemaIntegracao = $"Reenviando integracao {DateTime.Now}";
                else
                    integracao.ProblemaIntegracao = "";
                integracao.Mensagem = mensagem;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.EnvioSucesso = sucesso;
                repIntegracao.Atualizar(integracao);
                return;
            }
            integracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento()
            {
                Carga = carga,
                NumeroTentativas = 0,
                ProblemaIntegracao = string.Empty,
                TipoIntegracao = tipoIntegracao,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                DataIntegracao = DateTime.Now,
                Etapa = etapa,
                Mensagem = mensagem,
                ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(),
                EnvioSucesso = sucesso
            };
            repIntegracao.Inserir(integracao);
        }

        #endregion Métodos Privados
    }
}

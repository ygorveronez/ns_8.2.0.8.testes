using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoLoteCarregamento
    {
        #region  Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoLoteCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento AdicionarIntegracaoCarregamento(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> listaCarregamentos, TipoIntegracao tipo, SituacaoIntegracao situacaoPadrao = SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repLoteIntegracaoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipo);

            if (tipoIntegracao == null)
                throw new ServicoException("Tipo integração isis não encontrada.");

            if (listaCarregamentos.Count == 0)
                throw new ServicoException("Nenhum carregamento selecionado.");

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento loteIntegracaoCarregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento()
            {
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = situacaoPadrao,
                TipoIntegracao = tipoIntegracao,
                Carregamentos = listaCarregamentos,
            };

            // Persiste dados
            repLoteIntegracaoCarregamento.Inserir(loteIntegracaoCarregamento);
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in listaCarregamentos)
            {
                carregamento.LoteIntegracaoCarregamento = loteIntegracaoCarregamento;
                repCarregamento.Atualizar(carregamento);
                if (Auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carregamento, null, "Adicionou carregamento a integração via lote", _unitOfWork);
            }

            _unitOfWork.CommitChanges();

            return loteIntegracaoCarregamento;

        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repositorioLoteCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> carregamentosIntegracao = repositorioLoteCarregamentoIntegracao.BuscarLoteCarregamentoIntegracaoPendente(limiteRegistros: 20);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento lotecarregamentoIntegracao in carregamentosIntegracao)
            {
                switch (lotecarregamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Isis:
                        new Isis.IntegracaoIsis(_unitOfWork).IntegrarCarregamentoLote(lotecarregamentoIntegracao);
                        break;

                    default:
                        lotecarregamentoIntegracao.NumeroTentativas++;
                        lotecarregamentoIntegracao.DataIntegracao = DateTime.Now;
                        lotecarregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        lotecarregamentoIntegracao.ProblemaIntegracao = "Tipo de integração não implementado para integração de lote de carregamentos.";
                        repositorioLoteCarregamentoIntegracao.Atualizar(lotecarregamentoIntegracao);
                        break;
                }
            }
        }

        #endregion
    }
}

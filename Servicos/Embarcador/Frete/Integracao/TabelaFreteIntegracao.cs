using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class TabelaFreteIntegracao
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete _configuracaoTabelaFrete;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public TabelaFreteIntegracao(Repositorio.UnitOfWork unitOfWork) : this (unitOfWork, configuracaoTabelaFrete: null) { }

        public TabelaFreteIntegracao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete)
        {
            _unitOfWork = unitOfWork;
            _configuracaoTabelaFrete = configuracaoTabelaFrete;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao AdicionarTabelaFreteIntegrarAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao repositorioTabelaFreteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao()
            {
                Numero = repositorioTabelaFreteIntegrarAlteracao.BuscarProximoNumeroPorTabelaFrete(tabelaFrete.Codigo),
                Situacao = SituacaoTabelaFreteIntegrarAlteracao.PendenteIntegracao,
                TabelaFrete = tabelaFrete
            };

            repositorioTabelaFreteIntegrarAlteracao.Inserir(tabelaFreteIntegrarAlteracao);

            return tabelaFreteIntegrarAlteracao;
        }

        private void AdicionarOuAtualizarTabelaFreteClienteIntegrarAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao repositorioTabelaFreteClienteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao tabelaFreteClienteIntegrarAlteracao = repositorioTabelaFreteClienteIntegrarAlteracao.BuscarPorAlteracao(tabelaFreteIntegrarAlteracao.Codigo, tabelaFreteCliente.Codigo);

            if (tabelaFreteClienteIntegrarAlteracao == null)
            {
                tabelaFreteClienteIntegrarAlteracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaFreteIntegrarAlteracao = tabelaFreteIntegrarAlteracao,
                    TabelaFreteCliente = tabelaFreteCliente
                };

                repositorioTabelaFreteClienteIntegrarAlteracao.Inserir(tabelaFreteClienteIntegrarAlteracao);
            }
            else
            {
                tabelaFreteClienteIntegrarAlteracao.DataAlteracao = DateTime.Now;

                repositorioTabelaFreteClienteIntegrarAlteracao.Atualizar(tabelaFreteClienteIntegrarAlteracao);
            }
        }

        private void AdicionarIntegracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(_unitOfWork);

            if (repositorioIntegracao.ExistePorTipoIntegracao(tabelaFreteIntegrarAlteracao.Codigo, tipoIntegracao.Codigo))
                return;

            Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao tabelaFreteIntegracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao()
            {
                DataIntegracao = DateTime.Now,
                TabelaFreteIntegrarAlteracao = tabelaFreteIntegrarAlteracao,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracao
            };

            repositorioIntegracao.Inserir(tabelaFreteIntegracao);
        }

        private void AtualizarSituacao(List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao> tabelasFreteIntegrarAlteracao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao repositorioTabelaFreteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao in tabelasFreteIntegrarAlteracao)
            {
                if (repositorioIntegracao.ContarPorSituacao(tabelaFreteIntegrarAlteracao.Codigo, SituacaoIntegracao.AgIntegracao) > 0)
                    return;

                if (repositorioIntegracao.ContarPorSituacao(tabelaFreteIntegrarAlteracao.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    tabelaFreteIntegrarAlteracao.Situacao = SituacaoTabelaFreteIntegrarAlteracao.FalhaIntegracao;
                    repositorioTabelaFreteIntegrarAlteracao.Atualizar(tabelaFreteIntegrarAlteracao);
                    return;
                }

                tabelaFreteIntegrarAlteracao.Situacao = SituacaoTabelaFreteIntegrarAlteracao.IntegracaoFinalizada;
                repositorioTabelaFreteIntegrarAlteracao.Atualizar(tabelaFreteIntegrarAlteracao);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete ObterConfiguracaoTabelaFrete()
        {
            if (_configuracaoTabelaFrete == null)
                _configuracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoTabelaFrete;
        }

        private List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao> VerificarIntegracoes()
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao> integracoes = repositorioIntegracao.BuscarIntegracoesPendentes(numeroTentativas: 3, minutosACadaTentativa: 5);

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Marisa:
                        new Integracao.IntegracaoMarisa(_unitOfWork).IntegrarTabelaFrete(integracao);
                        break;
                }
            }

            return integracoes.Select(integracao => integracao.TabelaFreteIntegrarAlteracao).ToList();
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AdicionarAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = ObterConfiguracaoTabelaFrete();

            if (!configuracaoTabelaFrete.UtilizarIntegracaoAlteracaoTabelaFrete)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao repositorioTabelaFreteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao = repositorioTabelaFreteIntegrarAlteracao.BuscarPendentePorTabelaFrete(tabelaFreteCliente.TabelaFrete.Codigo);

            if (tabelaFreteIntegrarAlteracao == null)
                tabelaFreteIntegrarAlteracao = AdicionarTabelaFreteIntegrarAlteracao(tabelaFreteCliente.TabelaFrete);

            AdicionarOuAtualizarTabelaFreteClienteIntegrarAlteracao(tabelaFreteIntegrarAlteracao, tabelaFreteCliente);
        }

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao repositorioTabelaFreteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao tabelaFreteIntegrarAlteracao = repositorioTabelaFreteIntegrarAlteracao.BuscarPendentePorTabelaFrete(tabelaFrete.Codigo);

            if (tabelaFreteIntegrarAlteracao == null)
                throw new ServicoException("Não existe integração de alterações pendentes para a tabela de frete");

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(new List<TipoIntegracao>() { TipoIntegracao.Marisa });

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                AdicionarIntegracao(tabelaFreteIntegrarAlteracao, tipoIntegracao);

            tabelaFreteIntegrarAlteracao.Situacao = (tiposIntegracao.Count > 0) ? SituacaoTabelaFreteIntegrarAlteracao.IntegracaoIniciada : SituacaoTabelaFreteIntegrarAlteracao.IntegracaoFinalizada;

            repositorioTabelaFreteIntegrarAlteracao.Atualizar(tabelaFreteIntegrarAlteracao);
        }

        public void VerificarIntegracoesPendentes()
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao> tabelaFretesIntegrarAlteracao = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao>();

            tabelaFretesIntegrarAlteracao.AddRange(VerificarIntegracoes());

            AtualizarSituacao(tabelaFretesIntegrarAlteracao.Distinct().ToList());
        }

        #endregion Métodos Públicos
    }
}

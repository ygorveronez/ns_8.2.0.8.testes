using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class TabelaFreteClienteIntegracao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> _tiposIntegracao;

        #endregion Atributos

        #region Construtores

        public TabelaFreteClienteIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> ObterTiposIntegracao(List<TipoIntegracao> tiposIntegracaoUtilizar)
        {
            if (_tiposIntegracao == null)
                _tiposIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork).BuscarPorTipos(new List<TipoIntegracao>() { TipoIntegracao.LBC, TipoIntegracao.SaintGobainFrete });

            return _tiposIntegracao.Where(tipo => tiposIntegracaoUtilizar.Contains(tipo.Tipo)).ToList();
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AdicionarIntegracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFreteClienteIntegracao = repositorioTabelaFreteClienteIntegracao.BuscarPendentePorTipoIntegracao(tabelaFreteCliente.Codigo, tipoIntegracao.Codigo);

            if (tabelaFreteClienteIntegracao == null)
            {
                tabelaFreteClienteIntegracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao()
                {
                    DataIntegracao = DateTime.Now,
                    TabelaFreteCliente = tabelaFreteCliente,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao
                };

                repositorioTabelaFreteClienteIntegracao.Inserir(tabelaFreteClienteIntegracao);
            }
            else
            {
                tabelaFreteClienteIntegracao.DataIntegracao = DateTime.Now;
                tabelaFreteClienteIntegracao.NumeroTentativas = 0;
                tabelaFreteClienteIntegracao.ProblemaIntegracao = "";
                tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                repositorioTabelaFreteClienteIntegracao.Atualizar(tabelaFreteClienteIntegracao);
            }
        }

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = ObterTiposIntegracao(new List<TipoIntegracao>() { TipoIntegracao.SaintGobainFrete });

            if (tiposIntegracao.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                AdicionarIntegracao(tabelaFreteCliente, tipoIntegracao);

            tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao;

            repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
        }

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao)
        {
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = ObterTiposIntegracao(new List<TipoIntegracao>() { TipoIntegracao.LBC });

            if (tiposIntegracao.Count == 0)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                repositorioTabelaFreteClienteIntegracao.InserirOuAtualizarIntegracoesPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, tipoIntegracao.Codigo);

            repositorioTabelaFreteCliente.AtualizarSituacaoIntegracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao);
        }

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete)
        {
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = ObterTiposIntegracao(new List<TipoIntegracao>() { TipoIntegracao.LBC });

            if (tiposIntegracao.Count == 0)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                repositorioTabelaFreteClienteIntegracao.InserirOuAtualizarIntegracoesPorAjusteTabelaFrete(ajusteTabelaFrete.Codigo, tipoIntegracao.Codigo);

            repositorioTabelaFreteCliente.AtualizarSituacaoIntegracaoPorAjusteTabelaFrete(ajusteTabelaFrete.Codigo, SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao);
        }

        public void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);

            if (repositorioTabelaFreteClienteIntegracao.ContarPorTabelaFreteClienteESituacao(tabelaFreteCliente.Codigo, SituacaoIntegracao.AgIntegracao) > 0)
                return;

            if (repositorioTabelaFreteClienteIntegracao.ContarPorTabelaFreteClienteESituacao(tabelaFreteCliente.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
            {
                tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = SituacaoIntegracaoTabelaFreteCliente.FalhaIntegracao;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
                return;
            }

            if (repositorioTabelaFreteClienteIntegracao.ContarPorTabelaFreteClienteESituacao(tabelaFreteCliente.Codigo, SituacaoIntegracao.AgRetorno) > 0)
            {
                tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = SituacaoIntegracaoTabelaFreteCliente.AguardandoRetorno;
                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
                return;
            }

            tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = SituacaoIntegracaoTabelaFreteCliente.Integrado;
            repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
        }

        public bool PossuiIntegracaoControlaSituacaoItens()
        {
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = ObterTiposIntegracao(new List<TipoIntegracao>() { TipoIntegracao.LBC });

            return (tiposIntegracao.Count > 0);
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> integracoes = repositorioTabelaFreteClienteIntegracao.BuscarIntegracoesPendentes(numeroTentativas: 3, minutosACadaTentativa: 5);

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.LBC:
                        new Integracao.LBC.IntegracaoLBC(_unitOfWork).IntegrarTabelaFreteCliente(integracao);
                        break;
                    case TipoIntegracao.SaintGobainFrete:
                        new Integracao.SaintGobain.IntegracaoSaintGobain(_unitOfWork).IntegrarTabelaFreteCliente(integracao);
                        break;
                    default:
                        integracao.ProblemaIntegracao = "Tipo de integração não implementado.";
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.NumeroTentativas++;
                        integracao.DataIntegracao = DateTime.Now;
                        repositorioTabelaFreteClienteIntegracao.Atualizar(integracao);
                        break;
                }

                AtualizarSituacaoIntegracao(integracao.TabelaFreteCliente);
            }
        }

        #endregion Métodos públicos
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoHistorico
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion Atributos

        #region Construtores

        public FilaCarregamentoVeiculoHistorico(Repositorio.UnitOfWork unitOfWork) : this (unitOfWork, usuario: null) { }

        public FilaCarregamentoVeiculoHistorico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _usuario = usuario;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico Adicionar(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar historicoAdicionar)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historico = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico()
            {
                Data = DateTime.Now,
                Descricao = historicoAdicionar.Descricao,
                FilaCarregamentoVeiculo = historicoAdicionar.FilaCarregamentoVeiculo,
                MotivoAlteracaoPosicaoFilaCarregamento = historicoAdicionar.MotivoAlteracaoPosicaoFilaCarregamento,
                MotivoRetiradaFilaCarregamento = historicoAdicionar.MotivoRetiradaFilaCarregamento,
                MotivoSelecaoMotoristaForaOrdem = historicoAdicionar.MotivoSelecaoMotoristaForaOrdem,
                OrigemAlteracao = historicoAdicionar.OrigemAlteracao,
                Posicao = historicoAdicionar.FilaCarregamentoVeiculo.Posicao,
                Tipo = historicoAdicionar.Tipo,
                Usuario = _usuario,
                Observacao = historicoAdicionar.Observacao
            };

            if ((historicoAdicionar.Tipo == TipoFilaCarregamentoVeiculoHistorico.PreCargaAlocada) || (historicoAdicionar.Tipo == TipoFilaCarregamentoVeiculoHistorico.PreCargaRecusada))
                historico.PreCarga = historicoAdicionar.FilaCarregamentoVeiculo.PreCarga;

            repositorioHistorico.Inserir(historico);

            return historico;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ObterUltimo(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(_unitOfWork);

            return repositorioHistorico.BuscarUltimoPorFilaCarregamentoVeiculoETipo(codigoFilaCarregamentoVeiculo, tipo: null);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ObterUltimoPorTipo(int codigoFilaCarregamentoVeiculo, TipoFilaCarregamentoVeiculoHistorico tipo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(_unitOfWork);

            return repositorioHistorico.BuscarUltimoPorFilaCarregamentoVeiculoETipo(codigoFilaCarregamentoVeiculo, tipo);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> ObterTodos(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(_unitOfWork);

            return repositorioHistorico.BuscarPorFilaCarregamentoVeiculo(codigoFilaCarregamentoVeiculo);
        }

        #endregion Métodos Públicos
    }
}

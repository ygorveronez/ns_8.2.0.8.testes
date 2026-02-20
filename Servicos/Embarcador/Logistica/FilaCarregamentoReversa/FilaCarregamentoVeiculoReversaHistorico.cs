using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoReversaHistorico
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtores

        public FilaCarregamentoVeiculoReversaHistorico(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, usuario: null) { }

        public FilaCarregamentoVeiculoReversaHistorico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico Adicionar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa, string descricao)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico historico = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico()
            {
                Data = DateTime.Now,
                Descricao = descricao,
                FilaCarregamentoVeiculoReversa = filaCarregamentoVeiculoReversa,
                Usuario = _usuario
            };

            repositorioHistorico.Inserir(historico);

            return historico;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico> ObterTodos(int codigoFilaCarregamentoVeiculoReversa)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico(_unitOfWork);

            return repositorioHistorico.BuscarPorFilaCarregamentoVeiculoReversa(codigoFilaCarregamentoVeiculoReversa);
        }

        #endregion
    }
}

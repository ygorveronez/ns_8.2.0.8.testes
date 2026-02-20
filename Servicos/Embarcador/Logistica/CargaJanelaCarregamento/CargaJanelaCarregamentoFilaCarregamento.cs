using Dominio.Excecoes.Embarcador;
using System;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoFilaCarregamento
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoFilaCarregamento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public CargaJanelaCarregamentoFilaCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void BloquearOuLiberarCargaFilaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool isLiberarCargaFilaCarregamento)
        {
            string tipoAcao = isLiberarCargaFilaCarregamento ? "liberada" : "bloqueada";

            if (!IsPermitirBloquearOuLiberarCargaFilaCarregamento(cargaJanelaCarregamento, isLiberarCargaFilaCarregamento))
                throw new ServicoException($"A carga já está {tipoAcao} para a fila de carregamento");

            cargaJanelaCarregamento.CargaBloqueadaFilaCarregamentoManualmente = !isLiberarCargaFilaCarregamento;
            cargaJanelaCarregamento.CargaLiberadaFilaCarregamentoManualmente = isLiberarCargaFilaCarregamento;

            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                if (_auditado != null)
                    Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento, $"Carga {tipoAcao} para a fila de carregamento.", _unitOfWork);

                _unitOfWork.CommitChanges();

                if (cargaJanelaCarregamento.Carga != null)
                    new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private void BloquearOuLiberarCargaFilaCarregamentoPorCarga(int codigoCarga, bool isLiberarCargaFilaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorProtocoloCarga(codigoCarga) /*repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga)*/ ?? throw new ServicoException("Janela de carregamento não encontrada");

            BloquearOuLiberarCargaFilaCarregamento(cargaJanelaCarregamento, isLiberarCargaFilaCarregamento);
        }

        private void BloquearOuLiberarCargaFilaCarregamentoPorJanelaCarregamento(int codigoJanelaCarregamento, bool isLiberarCargaFilaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento) ?? throw new ServicoException("Janela de carregamento não encontrada");

            BloquearOuLiberarCargaFilaCarregamento(cargaJanelaCarregamento, isLiberarCargaFilaCarregamento);
        }

        private bool IsPermitirBloquearOuLiberarCargaFilaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool isLiberarCargaFilaCarregamento)
        {
            return (
                (isLiberarCargaFilaCarregamento && !cargaJanelaCarregamento.CargaLiberadaFilaCarregamentoManualmente) ||
                (!isLiberarCargaFilaCarregamento && !cargaJanelaCarregamento.CargaBloqueadaFilaCarregamentoManualmente)
            );
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void BloquearCargaFilaCarregamentoPorCarga(int codigoCarga)
        {
            BloquearOuLiberarCargaFilaCarregamentoPorCarga(codigoCarga, isLiberarCargaFilaCarregamento: false);
        }

        public void BloquearCargaFilaCarregamentoPorJanelaCarregamento(int codigoJanelaCarregamento)
        {
            BloquearOuLiberarCargaFilaCarregamentoPorJanelaCarregamento(codigoJanelaCarregamento, isLiberarCargaFilaCarregamento: false);
        }

        public void LiberarCargaFilaCarregamentoPorCarga(int codigoCarga)
        {
            BloquearOuLiberarCargaFilaCarregamentoPorCarga(codigoCarga, isLiberarCargaFilaCarregamento: true);
        }

        public void LiberarCargaFilaCarregamentoPorJanelaCarregamento(int codigoJanelaCarregamento)
        {
            BloquearOuLiberarCargaFilaCarregamentoPorJanelaCarregamento(codigoJanelaCarregamento, isLiberarCargaFilaCarregamento: true);
        }

        #endregion Métodos Públicos
    }
}

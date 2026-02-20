using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoPrioridade
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoPrioridade(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public CargaJanelaCarregamentoPrioridade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AdicionarPrioridade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Excedente || (cargaJanelaCarregamento.CentroCarregamento == null))
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            int prioridade = 0;

            if (cargaJanelaCarregamento.CargaBase.IsCarga())
            {
                prioridade = repositorioCargaJanelaCarregamento.BuscarMenorPrioridadePorCargaOriginal(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.InicioCarregamento);

                if (prioridade > 0)
                {
                    int maiorPrioridade = repositorioCargaJanelaCarregamento.BuscarMaiorPrioridade(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento);

                    if (prioridade > maiorPrioridade)
                        prioridade = maiorPrioridade + 1;
                }
            }

            if (prioridade == 0)
                prioridade = repositorioCargaJanelaCarregamento.BuscarProximaPrioridade(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento);

            if (prioridade == 0)
                prioridade = repositorioCargaJanelaCarregamento.BuscarPrioridadeAnterior(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento, cargaJanelaCarregamento.Codigo) + 1;

            repositorioCargaJanelaCarregamento.AdicionarPrioridade(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento, prioridade);
            cargaJanelaCarregamento.Prioridade = prioridade;
        }

        public void AlterarPrioridade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, int novaPrioridade)
        {
            if (cargaJanelaCarregamento.CentroCarregamento.TipoOrdenacaoJanelaCarregamento != TipoOrdenacaoJanelaCarregamento.Prioridade)
                throw new ServicoException("Não é possível alterar a prioridade.");

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            cargaJanelaCarregamento.Prioridade = repositorioCargaJanelaCarregamento.BuscarPrioridadeAtual(cargaJanelaCarregamento.Codigo);

            if (novaPrioridade == cargaJanelaCarregamento.Prioridade)
                throw new ServicoException("O carregamento já está na prioridade informada.");

            if (novaPrioridade > cargaJanelaCarregamento.Prioridade)
                repositorioCargaJanelaCarregamento.AtualizarPrioridadesInferiores(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento, novaPrioridade, cargaJanelaCarregamento.Prioridade);
            else
                repositorioCargaJanelaCarregamento.AtualizarPrioridadesSuperiores(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento, novaPrioridade, cargaJanelaCarregamento.Prioridade);

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento, $"Alterada a prioridade do carregamento de {cargaJanelaCarregamento.Prioridade} para {novaPrioridade}", _unitOfWork);

            cargaJanelaCarregamento.Prioridade = novaPrioridade;
        }

        public void RemoverPrioridade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            RemoverPrioridade(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento);
        }

        public void RemoverPrioridade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            cargaJanelaCarregamento.Prioridade = repositorioCargaJanelaCarregamento.BuscarPrioridadeAtual(cargaJanelaCarregamento.Codigo);

            if (cargaJanelaCarregamento.Prioridade == 0 || centroCarregamento == null)
                return;

            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork).RemoverPrioridade(centroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento, cargaJanelaCarregamento.Prioridade);

            cargaJanelaCarregamento.Prioridade = 0;
        }

        public void RemoverPrioridadesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarParaRemover(carga.Codigo);

            if (listaCargaJanelaCarregamento.Count <= 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamento)
            {
                RemoverPrioridade(cargaJanelaCarregamento);
                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
        }

        #endregion Métodos Públicos
    }
}

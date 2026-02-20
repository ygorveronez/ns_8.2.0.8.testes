using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Transportadores
{
    public sealed class MotoristaJornada
    {
        #region Atributos Protegidos Somente Leitura

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWorkContainer _unitOfWorkContainer;

        #endregion

        #region Construtores

        public MotoristaJornada(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public MotoristaJornada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(unitOfWork);
        }

        public MotoristaJornada(Repositorio.UnitOfWorkContainer unitOfWorkContainer) : this(unitOfWorkContainer, configuracaoEmbarcador: null) { }

        public MotoristaJornada(Repositorio.UnitOfWorkContainer unitOfWorkContainer, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWorkContainer = unitOfWorkContainer;
        }

        #endregion

        #region Métodos Privados de Consulta

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoTMS()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWorkContainer.UnitOfWork);

                _configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada ObterJornadaAtual(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Transportadores.MotoristaJornada repositorioMotoristaJornada = new Repositorio.Embarcador.Transportadores.MotoristaJornada(_unitOfWorkContainer.UnitOfWork);

            return repositorioMotoristaJornada.BuscarAtualPorMotorista(motorista.Codigo);
        }

        private TimeSpan? ObterTempoJornadaAtual(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada jornadaAtual = ObterJornadaAtual(motorista);

            if (jornadaAtual == null)
                return null;

            return TimeSpan.FromSeconds(DateTime.Now.Subtract(jornadaAtual.DataInicial).TotalSeconds);
        }

        #endregion

        #region Métodos Públicos

        public void FinalizarJornada(Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Transportadores.MotoristaJornada repositorioJornada = new Repositorio.Embarcador.Transportadores.MotoristaJornada(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada jornada = repositorioJornada.BuscarAtualPorMotorista(motorista.Codigo);

            if (jornada == null)
                throw new ServicoException("Nenhuma jornada iniciada para o motorista");

            try
            {
                _unitOfWorkContainer.StartContainer();

                jornada.DataFinal = DateTime.Now;

                repositorioJornada.Atualizar(jornada);

                Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Sistema);

                if (!servicoFilaCarregamentoVeiculo.RemoverPorMotoristaJornadaEncerrada(motorista, tipoServicoMultisoftware))
                {
                    Logistica.FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Sistema);

                    servicoFilaCarregamentoMotorista.RemoverPorMotoristaJornadaFinalizada(motorista);
                }                

                _unitOfWorkContainer.CommitChangesContainer();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.RollbackContainer();
                throw;
            }
        }

        public void IniciarJornada(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Transportadores.MotoristaJornada repositorioJornada = new Repositorio.Embarcador.Transportadores.MotoristaJornada(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada jornada = repositorioJornada.BuscarAtualPorMotorista(motorista.Codigo);

            if (jornada != null)
                throw new ServicoException("Já existe uma jornada iniciada para o motorista");

            jornada = new Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada()
            {
                DataInicial = DateTime.Now,
                Motorista = motorista
            };

            repositorioJornada.Inserir(jornada);
        }

        public void ValidarJornadaTrabalhoExcedida(Dominio.Entidades.Usuario motorista, bool utilizarMotoristaJornadaExcedida, int tempoDeViagemEmMinutos)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoTMS();

            if (!(configuracaoEmbarcador?.UtilizarControleJornadaMotorista ?? false))
                return;

            if (!configuracaoEmbarcador?.JornadaDiariaMotorista.HasValue ?? false)
                throw new ServicoException("A jornada de trabalho diária não foi configurada.");

            TimeSpan? horasTrabalhadas = ObterTempoJornadaAtual(motorista);

            if (horasTrabalhadas == null)
                throw new ServicoException("O motorista não possui uma jornada de trabalho iniciada.");

            if (utilizarMotoristaJornadaExcedida)
                return;

            double totalHorasEmMinutos = (double)tempoDeViagemEmMinutos + horasTrabalhadas.Value.TotalMinutes;

            if (totalHorasEmMinutos > configuracaoEmbarcador.JornadaDiariaMotorista.Value.TotalMinutes)
                throw new ServicoException("A jornada de trabalho diária foi excedida.", errorCode: CodigoExcecao.JornadaTrabalhoExcedida);
        }

        public void ValidarJornadaTrabalhoIniciada(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoTMS();

            if (!(configuracaoEmbarcador?.UtilizarControleJornadaMotorista ?? false))
                return;

            if (!configuracaoEmbarcador?.JornadaDiariaMotorista.HasValue ?? false)
                throw new ServicoException("A jornada de trabalho diária não foi configurada.");

            Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada jornadaAtual = ObterJornadaAtual(motorista);

            if (jornadaAtual == null)
                throw new ServicoException("O motorista não possui uma jornada de trabalho iniciada.");
        }

        #endregion

        #region Métodos Públicos de Consulta

        public bool IsJornadaMotoristaIniciada(Dominio.Entidades.Usuario motorista)
        {
            if ((motorista == null) || !(ObterConfiguracaoTMS()?.UtilizarControleJornadaMotorista ?? false))
                return false;

            Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada jornadaAtual = ObterJornadaAtual(motorista);

            return (jornadaAtual != null);
        }

        public string ObterJornadaMotorista(Dominio.Entidades.Usuario motorista)
        {
            if ((motorista == null) || !(ObterConfiguracaoTMS()?.UtilizarControleJornadaMotorista ?? false))
                return "";

            TimeSpan? tempoJornadaAtual = ObterTempoJornadaAtual(motorista);

            if (tempoJornadaAtual == null)
                return "";

            return string.Format("{0:00}:{1:00}", Math.Floor(tempoJornadaAtual.Value.TotalHours), tempoJornadaAtual.Value.Minutes);
        }

        #endregion
    }
}

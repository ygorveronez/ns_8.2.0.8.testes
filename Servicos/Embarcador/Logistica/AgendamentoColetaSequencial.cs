using System;

namespace Servicos.Embarcador.Logistica
{
    public class AgendamentoColetaSequencial
    {
        #region Atributos

        private static readonly Lazy<AgendamentoColetaSequencial> _agendamentoColetaSequencial = new Lazy<AgendamentoColetaSequencial>(() => new AgendamentoColetaSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private AgendamentoColetaSequencial() { }

        public static AgendamentoColetaSequencial GetInstance()
        {
            return _agendamentoColetaSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockSequencial) {
                int proximoNumeroSequencial = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork).ObterProximoNumeroSequencial();

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos
    }
}

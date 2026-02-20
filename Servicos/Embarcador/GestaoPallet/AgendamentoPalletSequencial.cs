using System;

namespace Servicos.Embarcador.GestaoPallet
{
    public class AgendamentoPalletSequencial
    {
        #region Atributos

        private static readonly Lazy<AgendamentoPalletSequencial> _agendamentoPalletSequencial = new Lazy<AgendamentoPalletSequencial>(() => new AgendamentoPalletSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private AgendamentoPalletSequencial() { }

        public static AgendamentoPalletSequencial GetInstance()
        {
            return _agendamentoPalletSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockSequencial)
            {
                int proximoNumeroSequencial = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork).ObterProximoNumeroSequencial();

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos
    }
}

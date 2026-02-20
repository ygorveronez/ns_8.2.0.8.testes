using System;

namespace Servicos.Embarcador.GestaoPallet
{
    public class AgendamentoColetaPalletSequencial
    {
        #region Atributos

        private static readonly Lazy<AgendamentoColetaPalletSequencial> _agendamentoColetaPalletSequencial = new Lazy<AgendamentoColetaPalletSequencial>(() => new AgendamentoColetaPalletSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private AgendamentoColetaPalletSequencial() { }

        public static AgendamentoColetaPalletSequencial GetInstance()
        {
            return _agendamentoColetaPalletSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockSequencial)
            {
                int proximoNumeroSequencial = new Repositorio.Embarcador.GestaoPallet.AgendamentoColetaPallet(unitOfWork).ObterProximoNumeroSequencial();

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos
    }
}

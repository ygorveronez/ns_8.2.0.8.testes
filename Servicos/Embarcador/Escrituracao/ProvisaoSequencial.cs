using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Escrituracao
{
    public class ProvisaoSequencial
    {
        #region Atributos
        private static readonly Lazy<ProvisaoSequencial> _provisaoSequencial = new Lazy<ProvisaoSequencial>(() => new ProvisaoSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private ProvisaoSequencial() { }

        public static ProvisaoSequencial GetInstance()
        {
            return _provisaoSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockSequencial)
            {
                int proximoNumeroSequencial = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork).ObterProximoNumero();

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos
    }
}

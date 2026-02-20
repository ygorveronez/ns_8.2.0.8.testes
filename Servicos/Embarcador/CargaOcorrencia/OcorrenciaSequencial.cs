using System;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public class OcorrenciaSequencial
    {
        #region Atributos

        private static readonly Lazy<OcorrenciaSequencial> _ocorrenciaSequencial = new Lazy<OcorrenciaSequencial>(() => new OcorrenciaSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private OcorrenciaSequencial() { }

        public static OcorrenciaSequencial GetInstance()
        {
            return _ocorrenciaSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockSequencial)
            {
                int proximoNumeroSequencial = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork).BuscarProximoCodigo();

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos
    }
}

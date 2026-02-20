using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio
{
    public class ControleFalhaThread : RepositorioBase<Dominio.Entidades.ControleFalhaThread>
    {
        #region Construtores

        public ControleFalhaThread(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<(int CodigoEntidade, int NumeroTentativas)> BuscarRegistrosComFalhaPorCodigosEntidades(List<int> codigosEntidades, IdentificadorControlePosicaoThread identificador)
        {
            var consultaControleFalhaThread = this.SessionNHiBernate.Query<Dominio.Entidades.ControleFalhaThread>()
                .Where(controleFalhaThread =>
                    codigosEntidades.Contains(controleFalhaThread.CodigoEntidade) &&
                    controleFalhaThread.Identificador == identificador &&
                    controleFalhaThread.RegistroComFalha == true
                );

            return consultaControleFalhaThread
                .OrderBy(controleFalhaThread => controleFalhaThread.DataUltimaTentativa)
                .Select(controleFalhaThread => new ValueTuple<int, int>(controleFalhaThread.CodigoEntidade, controleFalhaThread.NumeroTentativas))
                .ToList();
        }

        public Dominio.Entidades.ControleFalhaThread BuscarComFalhaPorCodigoEntidadeComIdentificador(int codigoEntidade, IdentificadorControlePosicaoThread identificador)
        {
            var consultaControleFalhaThread = this.SessionNHiBernate.Query<Dominio.Entidades.ControleFalhaThread>()
                .Where(controleFalhaThread =>
                    controleFalhaThread.CodigoEntidade == codigoEntidade &&
                    controleFalhaThread.Identificador == identificador &&
                    controleFalhaThread.RegistroComFalha == true
                );

            return consultaControleFalhaThread.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
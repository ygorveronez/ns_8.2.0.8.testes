using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public sealed class SugestaoProgramacaoCargaEstadoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino>
    {
        #region Construtores

        public SugestaoProgramacaoCargaEstadoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Estado> BuscarEstadosPorSugestaoProgramacaoCarga(int codigoSugestaoProgramacaoCarga)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino>()
                .Where(estadoDestino => estadoDestino.SugestaoProgramacaoCarga.Codigo == codigoSugestaoProgramacaoCarga);

            return consultaEstadoDestino
                .Select(estadoDestino => estadoDestino.Estado)
                .ToList();
        }

        public List<(int CodigoSugestaoProgramacaoCarga, string Estado)> BuscarPorSugestoesProgramacaoCarga(List<int> codigosSugestaoProgramacaoCarga)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino>()
                .Where(estadoDestino => codigosSugestaoProgramacaoCarga.Contains(estadoDestino.SugestaoProgramacaoCarga.Codigo));

            return consultaEstadoDestino
                .Select(estadoDestino => ValueTuple.Create(estadoDestino.SugestaoProgramacaoCarga.Codigo, estadoDestino.Estado.Nome))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}

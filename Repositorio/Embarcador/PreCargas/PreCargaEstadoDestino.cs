using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaEstadoDestino : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>
    {
        #region Construtores

        public PreCargaEstadoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<(int CodigoPreCarga, string Estado)> BuscarPorPreCarga(int codigoPreCarga)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>()
                .Where(destino => destino.PreCarga.Codigo == codigoPreCarga);

            return consultaEstadoDestino
                .Select(estadoDestino => ValueTuple.Create(estadoDestino.PreCarga.Codigo, estadoDestino.Estado.Nome))
                .ToList();
        }

        public List<(int CodigoPreCarga, string Estado)> BuscarPorPreCargas(List<int> codigosPreCarga)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>()
                .Where(destino => codigosPreCarga.Contains(destino.PreCarga.Codigo));

            return consultaEstadoDestino
                .Select(estadoDestino => ValueTuple.Create(estadoDestino.PreCarga.Codigo, estadoDestino.Estado.Nome))
                .ToList();
        }

        public List<string> BuscarSiglasEstadosDestinoPorPreCarga(int codigoPreCarga)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>()
                .Where(destino => destino.PreCarga.Codigo == codigoPreCarga);

            return consultaEstadoDestino
                .Select(estadoDestino => estadoDestino.Estado.Sigla)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}

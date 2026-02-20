using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaDestino : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>
    {
        #region Construtores

        public PreCargaDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosDestinosPorPreCarga(int codigoPreCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                .Where(destino => destino.PreCarga.Codigo == codigoPreCarga);

            return consultaDestino
                .Select(destino => destino.Localidade.Codigo)
                .ToList();
        }

        public List<(int CodigoLocalidade, string SiglaEstado)> BuscarLocalidadeEEstadoPorPreCarga(int codigoPreCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                .Where(destino => destino.PreCarga.Codigo == codigoPreCarga);

            return consultaDestino
                .Select(destino => ValueTuple.Create(destino.Localidade.Codigo, destino.Localidade.Estado.Sigla))
                .ToList();
        }

        public List<(int CodigoPreCarga, string Destino)> BuscarPorPreCarga(int codigoPreCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                .Where(destino => destino.PreCarga.Codigo == codigoPreCarga);

            return consultaDestino
                .Select(destino => ValueTuple.Create(destino.PreCarga.Codigo, destino.Localidade.Descricao))
                .ToList();
        }

        public List<(int CodigoPreCarga, string Destino)> BuscarPorPreCargas(List<int> codigosPreCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                .Where(destino => codigosPreCarga.Contains(destino.PreCarga.Codigo));

            return consultaDestino
                .Select(destino => ValueTuple.Create(destino.PreCarga.Codigo, destino.Localidade.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}

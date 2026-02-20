using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PreCargas
{
    public sealed class PreCargaRegiaoDestino : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>
    {
        #region Construtores

        public PreCargaRegiaoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosRegioesDestinoPorPreCarga(int codigoPreCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>()
                .Where(regiaoDestino => regiaoDestino.PreCarga.Codigo == codigoPreCarga);

            return consultaRegiaoDestino
                .Select(regiaoDestino => regiaoDestino.Regiao.Codigo)
                .ToList();
        }

        public List<(int CodigoLocalidade, string SiglaEstado)> BuscarLocalidadeEEstadoPorPreCarga(int codigoPreCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>()
                 .Where(regiaoDestino => regiaoDestino.PreCarga.Codigo == codigoPreCarga);

            var consultaLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                 .Where(localidade => consultaRegiaoDestino.Any(regiaoDestino => regiaoDestino.Regiao.Codigo == localidade.Regiao.Codigo));

            return consultaLocalidade
                .Select(localidade => ValueTuple.Create(localidade.Codigo, localidade.Estado.Sigla))
                .ToList();
        }

        public List<(int CodigoPreCarga, string Regiao)> BuscarPorPreCarga(int codigoPreCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>()
                .Where(regiaoDestino => regiaoDestino.PreCarga.Codigo == codigoPreCarga);

            return consultaRegiaoDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.PreCarga.Codigo, regiaoDestino.Regiao.Descricao))
                .ToList();
        }

        public List<(int CodigoPreCarga, string Regiao)> BuscarPorPreCargas(List<int> codigosPreCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>()
                .Where(regiaoDestino => codigosPreCarga.Contains(regiaoDestino.PreCarga.Codigo));

            return consultaRegiaoDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.PreCarga.Codigo, regiaoDestino.Regiao.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}

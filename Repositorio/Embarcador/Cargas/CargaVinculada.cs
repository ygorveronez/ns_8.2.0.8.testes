using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaVinculada : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaVinculada>
    {
        #region Construtores

        public CargaVinculada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasPorCarga(int codigoCarga)
        {
            var consultaCargaVinculada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVinculada>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaVinculada
                .Select(o => o.Vinculo)
                .ToList();
        }

        public List<int> BuscarCodigosCargasPorCarga(int codigoCarga)
        { 
            var consultaCargaVinculada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVinculada>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaVinculada
                .Select(o => o.Vinculo.Codigo)
                .Distinct()
                .ToList();
        }

		public List<Dominio.Entidades.Embarcador.Cargas.CargaVinculada> BuscarPorCargas(List<int> codigosCargas)
		{
			var consultaCargaVinculada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVinculada>()
				.Where(o =>
					codigosCargas.Contains(o.Carga.Codigo) &&
					o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
					o.Carga.SituacaoCarga != SituacaoCarga.Anulada
				);

			return consultaCargaVinculada
				.Distinct()
				.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaVinculada BuscarPorCargaEVinculo(int codigoCarga, int codigoVinculo)
        {
            var consultaCargaVinculada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVinculada>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Vinculo.Codigo == codigoVinculo &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaVinculada.FirstOrDefault();
        }

        #endregion
    }
}
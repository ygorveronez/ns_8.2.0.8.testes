using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class RastreamentoCarga : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga>
    {
        #region Construtores

        public RastreamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        [Obsolete("Método utilizado somente no fluxo de entrega (DESCONTINUADO)")]
        public Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga BuscarPorCarga(int codigoCarga)
        {
            var consultaRastreamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaRastreamentoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga BuscarPorVeiculoDaCarga(int veiculo)
        {
            var consultaRastreamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga>()
                .Where(o =>
                    o.Carga.Veiculo.Codigo == veiculo &&
                    o.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Cancelado
                );
            
            return consultaRastreamentoCarga
                .OrderByDescending(o => o.Carga.DataCriacaoCarga)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaRastreamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaRastreamentoCarga.FirstOrDefault();
        }

        #endregion
    }
}

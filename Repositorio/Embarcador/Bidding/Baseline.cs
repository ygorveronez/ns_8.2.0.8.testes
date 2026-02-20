using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{

    public sealed class Baseline : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.Baseline>
    {
        #region Construtores

        public Baseline(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Baseline(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Públicos

        public async Task<List<Dominio.Entidades.Embarcador.Bidding.Baseline>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.Baseline>();

            var codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Bidding.Baseline> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public List<Dominio.Entidades.Embarcador.Bidding.Baseline> BuscarPorBiddingConvite(int codigoConvite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.Baseline> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                .Where(o => o.BiddingOfertaRota.BiddingOferta.BiddingConvite.Codigo == codigoConvite);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.Baseline> BuscarPorBiddingConviteRota(int codigoConvite, int codigoRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.Baseline> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                .Where(o => o.BiddingOfertaRota.BiddingOferta.BiddingConvite.Codigo == codigoConvite && o.BiddingOfertaRota.Codigo == codigoRota);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Bidding.Baseline>> BuscarPorBiddingConviteRotaAsync(int codigoConvite, int codigoRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.Baseline> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                .Where(o => o.BiddingOfertaRota.BiddingOferta.BiddingConvite.Codigo == codigoConvite && o.BiddingOfertaRota.Codigo == codigoRota);

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Bidding.Baseline>> BuscarPorBiddingConviteERotaAsync(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.Baseline> consultaBaseline = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                .Where(o => o.BiddingOfertaRota.BiddingOferta.BiddingConvite.Codigo == filtroPesquisaBiddingOfertas.Codigo);

            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                            .Where(o => o.BiddingOferta.BiddingConvite.Codigo == filtroPesquisaBiddingOfertas.Codigo);

            if (filtroPesquisaBiddingOfertas.CodigoRota > 0)
                consultaBaseline.Where(o => o.BiddingOfertaRota.Codigo == filtroPesquisaBiddingOfertas.CodigoRota);

            if (filtroPesquisaBiddingOfertas.CodigoModeloVeicular > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.ModelosVeiculares.Any(o => o.Codigo == filtroPesquisaBiddingOfertas.CodigoModeloVeicular)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoFilialParticipante > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.FiliaisParticipante.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoFilialParticipante)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoOrigem > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.Origens.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoOrigem)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoDestino > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.Destinos.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoDestino)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }


            if (filtroPesquisaBiddingOfertas.CodigoRegiaoDestino > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.Destinos.Any(x => x.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.Origens.Any(x => x.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }


            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino > 0)
            {
                IQueryable<int> codigosTranportadoresRota = subQuery.Where(o => o.RegioesDestino.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosTranportadoresRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem > 0)
            {
                IQueryable<int> codigosTranportadoresRota = subQuery.Where(o => o.RegioesOrigem.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem)).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosTranportadoresRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.QuantidadeEntregas > 0)

            {
                IQueryable<int> codigosRota = subQuery.Where(o => o.NumeroEntrega == filtroPesquisaBiddingOfertas.QuantidadeEntregas).Select(x => x.Codigo);

                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.QuantidadeAjudantes > 0)
            {
                IQueryable<int> codigosRota = subQuery.Where(o => filtroPesquisaBiddingOfertas.QuantidadeAjudantes == o.QuantidadeAjudantePorVeiculo).Select(x => x.Codigo);
                consultaBaseline = consultaBaseline.Where(o => codigosRota.Contains(o.BiddingOfertaRota.Codigo));
            }

            return consultaBaseline.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Bidding.Baseline> BuscarPorBiddingConviteERotas(int codigoConvite, int codigoRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.Baseline> consultaBaseline = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.Baseline>()
                .Where(o => o.BiddingOfertaRota.BiddingOferta.BiddingConvite.Codigo == codigoConvite && o.BiddingOfertaRota.Codigo == codigoRota);

            return consultaBaseline.ToList();
        }

        #endregion
    }

}

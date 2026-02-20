using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaTrajeto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto>
    {
        public CargaTrajeto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaTrajeto BuscarPorCarga(int codigoCarga)
        {
            var queryCargaTrajetoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>();

            queryCargaTrajetoCarga = queryCargaTrajetoCarga.Where(obj => obj.Carga.Codigo == codigoCarga);

            return queryCargaTrajetoCarga.Select(o => o.CargaTrajeto).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto> BuscarPorCargaAsync(int codigoCarga)
        {
            var queryCargaTrajetoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>();

            queryCargaTrajetoCarga = queryCargaTrajetoCarga.Where(obj => obj.Carga.Codigo == codigoCarga);

            return queryCargaTrajetoCarga.Select(o => o.CargaTrajeto).FirstOrDefaultAsync();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaTrajeto = Consultar(filtrosPesquisa);

            return ObterLista(consultaCargaTrajeto, parametrosConsulta);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto filtrosPesquisa)
        {
            var consultaCargaTrajeto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto>();

            if (filtrosPesquisa.SituacaoTrajeto.HasValue)
                consultaCargaTrajeto = consultaCargaTrajeto.Where(obj => obj.SituacaoTrajeto == filtrosPesquisa.SituacaoTrajeto);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                var consultaCargaTrajetoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga>()
                    .Where(obj => obj.Carga.CodigoCargaEmbarcador == filtrosPesquisa.Carga);

                List<int> listaCodigosCargaTrajeto = consultaCargaTrajetoCarga.Select(obj => obj.CargaTrajeto.Codigo).ToList();

                consultaCargaTrajeto = consultaCargaTrajeto.Where(obj => listaCodigosCargaTrajeto.Contains(obj.Codigo));
            }

            return consultaCargaTrajeto;
        }

        #endregion
    }
}

using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public abstract class RepositorioRelacionamentoParametrosOfertas<T> : RepositorioBase<T> where T : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRelacionamentoParametrosOfertas
    {
        protected RepositorioRelacionamentoParametrosOfertas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public virtual async Task DeletarPorCodigoAsync(int codigo)
        {
            var op = SessionNHiBernate.Query<T>();

            await op.Where(o => o.Codigo == codigo).DeleteAsync();
        }

        public virtual async Task<List<T>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = MontarConsulta(filtro);

            consulta.Fetch(o => o.ParametrosOfertas);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public virtual async Task<List<T>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro)
        {
            return await ObterListaAsync(MontarConsulta(filtro), null);
        }

        public virtual async Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro)
        {
            return await MontarConsulta(filtro).CountAsync(CancellationToken);
        }

        protected static IQueryable<T> AplicarFiltros(IQueryable<T> consulta, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro)
        {
            if (filtro.CodigoParametrosOfertas > 0)
            {
                consulta = consulta.Where(o => o.ParametrosOfertas.Codigo.Equals(filtro.CodigoParametrosOfertas));
            }

            return consulta;
        }

        private IQueryable<T> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro)
        {
            return AplicarFiltros(SessionNHiBernate.Query<T>(), filtro);
        }
    }
}

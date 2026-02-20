using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>
    {
        public TipoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoCarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.TipoCarregamento BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>();

            query = query.Where(o => o.CodigoIntegracao == codigoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoCarregamento BuscarTipoPadraoCargaAgrupada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>();
            query = query.Where(o => o.TipoPadraoAgrupamentoCarga && o.Situacao);

            return query.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento> BuscarTipoPadraoCargaAgrupadaAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>();
            query = query.Where(o => o.TipoPadraoAgrupamentoCarga && o.Situacao);

            return await query.FirstOrDefaultAsync(CancellationToken);
        }
        public List<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = ConsultarTipoCarregamento(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento filtrosPesquisa)
        {
            var consulta = ConsultarTipoCarregamento(filtrosPesquisa);

            return consulta.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento> ConsultarTipoCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>();
            var result = from obj in query select obj;

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (!string.IsNullOrEmpty(filtrosPesquisa.Observacao))
                query = query.Where(o => o.Observacao.Contains(filtrosPesquisa.Observacao));

            if (filtrosPesquisa.Situacao == true)
                query = query.Where(o => o.Situacao == true);

            else if (filtrosPesquisa.Situacao == false)
                query = query.Where(o => o.Situacao == false);


            return query;
        }

        #endregion
    }
}


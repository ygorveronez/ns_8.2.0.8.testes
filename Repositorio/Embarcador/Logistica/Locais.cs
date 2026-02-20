using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class Locais : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Locais>
    {
        public Locais(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Locais(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Logistica.Locais BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigos.Contains(ent.Codigo));

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarPorTipoDeLocal(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipolocal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Tipo == tipolocal);

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarPorTipoDeLocalEFiliais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipolocal, List<int> filiais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Tipo == tipolocal);
            if (filiais.Count > 0)
                result = result.Where(ent => filiais.Contains(ent.Filial.Codigo));
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.Locais>> BuscarPorTipoDeLocalEFiliaisAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipolocal, List<int> filiais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Tipo == tipolocal);
            if (filiais.Count > 0)
                result = result.Where(ent => filiais.Contains(ent.Filial.Codigo));
            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarPorTiposDeLocalEFiliais(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal> tiposlocal, List<int> filiais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();
            var result = from obj in query select obj;
            result = result.Where(ent => tiposlocal.Contains(ent.Tipo));
            if (filiais.Count > 0)
                result = result.Where(ent => filiais.Contains(ent.Filial.Codigo));
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.Locais[] BuscarPorTiposDeLocais(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal> tipolocal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();
            var result = from obj in query select obj;
            result = result.Where(ent => tipolocal.Contains(ent.Tipo));
            return result.ToArray();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Locais> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais filtroPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Locais>();

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.TipoLocal.HasValue)
                consulta = consulta.Where(o => o.Tipo == filtroPesquisa.TipoLocal);

            if (filtroPesquisa.CodigosFiliais.Count > 0)
                consulta = consulta.Where(o => filtroPesquisa.CodigosFiliais.Contains(o.Filial.Codigo));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtroPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocais filtroPesquisa)
        {
            var consulta = Consultar(filtroPesquisa);

            return consulta.Count();
        }

    }


}

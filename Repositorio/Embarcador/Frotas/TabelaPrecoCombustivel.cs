using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Frotas
{
    public class TabelaPrecoCombustivel : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel>
    {
        public TabelaPrecoCombustivel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel BuscarPorTipoOleoVigencia(int codTipoOleo, DateTime? data)
        {
            data = data == DateTime.MinValue ? null : data;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel>();
            var result = from obj in query
                         where obj.TipoOleo.Codigo == codTipoOleo
                            && (data == null || obj.DataInicioVigencia == null || obj.DataInicioVigencia <= data)
                         orderby obj.DataInicioVigencia descending
                         select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel> Consultar(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel> result = Consultar(filtrosPesquisa);

            result
                .Fetch(o => o.Empresa);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel> Consultar(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoTipoOleo > 0)
                result = result.Where(obj => obj.TipoOleo.Codigo == filtrosPesquisa.CodigoTipoOleo);

            if (filtrosPesquisa.DataInicioVigencia != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicioVigencia.Date == filtrosPesquisa.DataInicioVigencia.Date);

            return result;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaTabelaPrecoCombustivel filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TabelaPrecoCombustivel> result = Consultar(filtrosPesquisa);

            return result.Count();
        }
    }
}
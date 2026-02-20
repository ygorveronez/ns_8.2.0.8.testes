using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class BloqueioEmissaoPorNaoConformidade : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>
    {
        public BloqueioEmissaoPorNaoConformidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool ExisteRegraDuplicada(int codigoTipoNaoConformidade, int codigoBloqueio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>();
            if(codigoBloqueio > 0)
                query = query.Where(r => r.Codigo != codigoBloqueio);

            if (codigoTipoNaoConformidade > 0)
                query = query.Where(r => r.TipoNaoConformidade.Codigo == codigoTipoNaoConformidade);

            var result = query.FirstOrDefault();
            return result == null ? false : true;

        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>();
            query = from obj in query select obj;

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                query = query.Where(obj => obj.TiposOperacao.Any(ped => filtrosPesquisa.CodigosTipoOperacao.Contains(ped.Codigo)));

            if (filtrosPesquisa.CodigoTipoNaoConformidade > 0)
                query = query.Where(obj => obj.TipoNaoConformidade.Codigo == filtrosPesquisa.CodigoTipoNaoConformidade);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            return query;
        }
        #endregion
    }
}

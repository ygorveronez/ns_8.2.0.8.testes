using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Moedas
{
    public class Moeda : RepositorioBase<Dominio.Entidades.Embarcador.Moedas.Moeda>
    {
        public Moeda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Moedas.Moeda BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Moeda>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Moedas.Moeda BuscarPorCodigoIntegracao(int codigoMoeda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Moeda>();
            var result = from obj in query where obj.CodigoMoeda == codigoMoeda select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Moedas.Moeda BuscarPorSigla(string siglaMoeda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Moeda>();
            var result = from obj in query where obj.Simbolo == siglaMoeda select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Moedas.Moeda> Consultar(Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Moedas.Moeda> Consultar(Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Moeda>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoMoeda))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.DescricaoMoeda));

            if (filtrosPesquisa.Situacao)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoMoeda > 0)
                query = query.Where(obj => obj.CodigoMoeda == filtrosPesquisa.CodigoMoeda);

            return query;
        }
        #endregion
    }
}

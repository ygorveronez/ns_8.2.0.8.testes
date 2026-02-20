using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class RegrasInclusaoICMS : RepositorioBase<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS>
    {
        public RegrasInclusaoICMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = ConsultarRegrasInclusaoICMS(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS filtrosPesquisa)
        {
            var consulta = ConsultarRegrasInclusaoICMS(filtrosPesquisa);

            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS BuscarRegraParaCarga(double cnpjCpfTomador, int codigoGrupoPessoas, int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS>()
                .Where(o => o.Situacao);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfTomador || o.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfTomador);

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao || o.TipoOperacao == null);

            return query.OrderByDescending(o => o.Pessoa).OrderByDescending(o => o.GrupoPessoas).FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS> ConsultarRegrasInclusaoICMS(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS>();
            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoPessoa > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ.Equals(filtrosPesquisa.CodigoPessoa));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return query;
        }

        #endregion
    }
}

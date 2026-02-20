using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class MarcaProduto : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.MarcaProduto>
    {
        public MarcaProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> Consultar(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.MarcaProduto>();

            var result = from obj in query where codigosIntegracao.Contains(obj.CodigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.MarcaProduto BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.MarcaProduto>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> Consultar(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.MarcaProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            return result;
        }

        #endregion
    }
}

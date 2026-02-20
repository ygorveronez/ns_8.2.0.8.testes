using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ConversaoUnidadeMedida : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida>
    {
        public ConversaoUnidadeMedida(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida)
        {
            var consultaConversor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida>();

            consultaConversor = consultaConversor.Where(obj => obj.Codigo != conversaoUnidadeMedida.Codigo);
            consultaConversor = consultaConversor.Where(obj => obj.Descricao.Equals(conversaoUnidadeMedida.Descricao));
            consultaConversor = consultaConversor.Where(obj => obj.UnidadeMedidaOrigem == conversaoUnidadeMedida.UnidadeMedidaOrigem);
            consultaConversor = consultaConversor.Where(obj => obj.UnidadeMedidaDestino == conversaoUnidadeMedida.UnidadeMedidaDestino);

            return consultaConversor.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            return query;
        }
        
        #endregion
    }
}

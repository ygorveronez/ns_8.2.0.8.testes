using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalImportacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao>
    {
        public XMLNotaFiscalImportacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Get<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao>(codigo);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }
        
        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }
        
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao>();

            if (filtrosPesquisa.DataInicio.HasValue)
                query = query.Where(obj => obj.Data >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(obj => obj.Data <= filtrosPesquisa.DataLimite);

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                query = from importacao in query 
                        join xmlNotaFiscal in SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>() 
                        on importacao.Codigo equals xmlNotaFiscal.XMLNotaFiscalImportacao.Codigo
                        where xmlNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal select importacao;
            
            return query
                .Fetch(obj => obj.ImportacaoNotaFiscal);
        }
        
        #endregion
    }
}

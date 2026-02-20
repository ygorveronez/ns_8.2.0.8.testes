using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Produtos;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class OrdemCompraPrincipal : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal>
    {
        #region Construtores
        public OrdemCompraPrincipal(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal>()
                .Where(c => c.ControleIntegracaoEmbarcador.Contains(codigoIntegracao));

            return query.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal> Consultar(FiltroPesquisaOrdemDeCompra filtroPesquisa, ParametroConsulta parametroConsulta)
        {
            var consulta = Consulta(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }
        public int ContarConsulta(FiltroPesquisaOrdemDeCompra filtroPesquisa)
        {
            var consulta = Consulta(filtroPesquisa);
            return consulta.Count();
        }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal> Consulta(FiltroPesquisaOrdemDeCompra filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal>();
            IQueryable<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento>();

            if (!string.IsNullOrEmpty(filtroPesquisa.CodigoControleIntegracao))
                query = from obj in query where obj.ControleIntegracaoEmbarcador.Contains(filtroPesquisa.CodigoControleIntegracao) select obj;

            if(filtroPesquisa.CodigoFilial > 0)
                query = from obj in query where queryDocumentos.Any(d => d.OrdemDeCompraPrincipal.Codigo == obj.Codigo && d.Filial.Codigo == filtroPesquisa.CodigoFilial) select obj;

            if (filtroPesquisa.CodigoFornecedor > 0)
                query = from obj in query where queryDocumentos.Any(d => d.OrdemDeCompraPrincipal.Codigo == obj.Codigo && d.Fornecedor.CPF_CNPJ == filtroPesquisa.CodigoFornecedor) select obj;

            return query;
        }
        #endregion
    }
}

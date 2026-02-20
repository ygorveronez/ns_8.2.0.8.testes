using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class DataPreferencialDescargaFornecedorCategoria : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>
    {
        public DataPreferencialDescargaFornecedorCategoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> BuscarPorDataPreferencialDescarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            query = query.Where(obj => obj.DataPreferencialDescarga.Codigo == codigo);

            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> BuscarPorDataPreferencialDescargaComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            query = query.Where(obj => obj.DataPreferencialDescarga.Codigo == codigo);

            return query
                .Fetch(obj => obj.Categoria)
                .Fetch(obj => obj.Fornecedor)
                .Fetch(obj => obj.GrupoFornecedor)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> BuscarPorDataPreferencialDescargaComFetch(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            query = query.Where(obj => codigos.Contains(obj.DataPreferencialDescarga.Codigo));

            return query
                .Fetch(obj => obj.Categoria)
                .Fetch(obj => obj.Fornecedor)
                .ToList();
        }

        public int BuscarContagemPorCPFCategoriaCentroDescarregamento(double CPFCNPJFornecedor, int codigoGrupoFornecedor, int codigoProduto, int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            if (CPFCNPJFornecedor > 0)
                query = query.Where(obj => obj.Fornecedor.CPF_CNPJ == CPFCNPJFornecedor && obj.Categoria.Codigo == codigoProduto && obj.DataPreferencialDescarga.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            else
                query = query.Where(obj => obj.GrupoFornecedor.Codigo == codigoGrupoFornecedor && obj.Categoria.Codigo == codigoProduto && obj.DataPreferencialDescarga.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query
                .Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> BuscarPorCPFCategorias(double CPFCNPJFornecedor, List<int> categorias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            string raizCnpj = CPFCNPJFornecedor.ToString().Substring(0, 8);
            
            query = query.Where(obj => (obj.Fornecedor.CPF_CNPJ == CPFCNPJFornecedor || obj.GrupoFornecedor.RaizesCNPJ.Any(r => r.RaizCNPJ == raizCnpj)) && categorias.Contains(obj.Categoria.Codigo));

            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> consulta = Consultar(filtrosPesquisa);

            consulta = consulta
                .Fetch(obj => obj.DataPreferencialDescarga)
                .ThenFetch(obj => obj.CentroDescarregamento);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDataPreferencialDescarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();

            if (filtrosPesquisa.CodigoCentroDescarregamento > 0)
                query = query.Where(obj => obj.DataPreferencialDescarga.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            if (filtrosPesquisa.DiaPreferencial > 0)
                query = query.Where(obj => obj.DataPreferencialDescarga.DiaPreferencial == filtrosPesquisa.DiaPreferencial);

            return query;
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public sealed class ProdutoEmbarcadorEstoqueArmazemHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico>
    {
        #region Construtores

        public ProdutoEmbarcadorEstoqueArmazemHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consultar(codigo);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(int codigo)
        {
            var consulta = Consultar(codigo);
            return consulta.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico> Consultar(int codigo)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico>();

            if (codigo > 0)
                consulta = consulta.Where(o => o.ProdutoEmbarcadorEstoqueArmazem.Codigo == codigo);

            return consulta;
        }
        #endregion
    }
}

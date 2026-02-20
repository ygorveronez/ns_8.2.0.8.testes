using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Produtos
{
    public class LocalArmazenamentoProdutoTransferencia : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia>
    {
        public LocalArmazenamentoProdutoTransferencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia> BuscarPorLocalArmazenamentoProduto(int codigoLocalArmazenamentoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia>();
            var result = from obj in query where obj.LocalArmazenamentoProduto.Codigo == codigoLocalArmazenamentoProduto select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia> BuscarTransferenciasPendentesData()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLocalArmazanamentoProdutoTransferencia.AgTransferencia && obj.DataTransferencia.Date == System.DateTime.Now  select obj;
            return result.ToList();
        }
        #endregion
    }
}

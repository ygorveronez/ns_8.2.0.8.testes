using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ProdutoEstoqueHistorico : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico>
    {
        public ProdutoEstoqueHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico> Consultar(int codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int produto, int codigoEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigo, tipoServicoMultisoftware, produto, codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int produto, int codigoEmpresa)
        {
            var result = _Consultar(codigo, tipoServicoMultisoftware, produto, codigoEmpresa);

            return result.Count();
        }

        public void DeletarTodosPorCodigo(int codigoProdutoEstoque)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM ProdutoEstoqueHistorico c WHERE c.ProdutoEstoque.Codigo = :codigo").SetInt32("codigo", codigoProdutoEstoque).ExecuteUpdate();
        }

        public void TransferirHistoricoEstoque(int codigoProdutoEstoqueAnterior, int codigoProdutoEstoqueNovo, int codigoEmpresa, int codigoLocalArmazenameno)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_PRODUTO_ESTOQUE_HISTORICO SET EMP_CODIGO = :codigoEmpresa, LAP_CODIGO = :codigoLocalArmazenameno, PRE_CODIGO = :codigoProdutoEstoqueNovo WHERE PRE_CODIGO = :codigoProdutoEstoqueAnterior")
                .SetInt32("codigoProdutoEstoqueAnterior", codigoProdutoEstoqueAnterior)
                .SetInt32("codigoProdutoEstoqueNovo", codigoProdutoEstoqueNovo)
                .SetInt32("codigoLocalArmazenameno", codigoLocalArmazenameno)                
                .SetInt32("codigoEmpresa", codigoEmpresa).ExecuteUpdate();
        }

        public void AtualizarHistoricoEstoque(int codigoProduto, int codigoEmpresa, int codigoEmpresaAnterior)
        {
            if (codigoEmpresa != codigoEmpresaAnterior)
            {
                if (codigoEmpresaAnterior > 0 && codigoEmpresa > 0)
                    UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_PRODUTO_ESTOQUE_HISTORICO SET EMP_CODIGO = :codigoEmpresa WHERE PRO_CODIGO = :codigoProduto AND EMP_CODIGO = :codigoEmpresaAnterior").SetInt32("codigoEmpresaAnterior", codigoEmpresaAnterior).SetInt32("codigoProduto", codigoProduto).SetInt32("codigoEmpresa", codigoEmpresa).ExecuteUpdate();
                else if (codigoEmpresaAnterior == 0 && codigoEmpresa > 0)
                    UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_PRODUTO_ESTOQUE_HISTORICO SET EMP_CODIGO = :codigoEmpresa WHERE PRO_CODIGO = :codigoProduto AND EMP_CODIGO is null").SetInt32("codigoProduto", codigoProduto).SetInt32("codigoEmpresa", codigoEmpresa).ExecuteUpdate();
                else if (codigoEmpresaAnterior > 0 && codigoEmpresa == 0)
                    UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_PRODUTO_ESTOQUE_HISTORICO SET EMP_CODIGO = NULL WHERE PRO_CODIGO = :codigoProduto AND EMP_CODIGO = :codigoEmpresaAnterior").SetInt32("codigoEmpresaAnterior", codigoEmpresaAnterior).SetInt32("codigoProduto", codigoProduto).ExecuteUpdate();
            }
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico> _Consultar(int codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int produto, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico>();

            var result = from obj in query where obj.Produto.Codigo == produto select obj;

            if (codigo > 0)
            {
                result = result.Where(o => o.ProdutoEstoque.Codigo == codigo);
            }
            else
            {
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && codigoEmpresa > 0)
                    result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
                else if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    result = result.Where(o => o.Empresa == null);
                else if (codigoEmpresa > 0)
                    result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
            }

            return result;
        }

        #endregion
    }
}

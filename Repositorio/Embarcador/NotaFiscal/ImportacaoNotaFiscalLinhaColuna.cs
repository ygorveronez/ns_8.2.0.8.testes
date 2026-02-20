using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ImportacaoNotaFiscalLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna>
    {
        public ImportacaoNotaFiscalLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> BuscarPorImportacaoLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha && (o.Linha.Pedido == null && o.Linha.XMLNotaFiscal == null));

            return query.Fetch(o => o.Linha).ToList();
        }


        public List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> BuscarPorImportacaoLinhas(List<int> codigosLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna>();

            query = query.Where(o => codigosLinha.Contains(o.Linha.Codigo) && (o.Linha.Pedido == null && o.Linha.XMLNotaFiscal == null));

            return query.Fetch(o => o.Linha).ToList();
        }

        //public void DeletarPorCodigos(List<int> codigos)
        //{
        //    if (codigos.Count == 0)
        //        return;

        //    string sql = "DELETE FROM T_IMPORTACAO_NOTA_FISCAL_LINHA_COLUNA WHERE IMC_CODIGO IN (:codigos)";
        //    var query = this.SessionNHiBernate.CreateSQLQuery(sql);
        //    query.SetParameterList("codigos", codigos);

        //    int quantidadeDeletada = query.ExecuteUpdate();
        //}

        public void DeletarPorLinha(int linha)
        {
            string sql = "DELETE FROM T_IMPORTACAO_NOTA_FISCAL_LINHA_COLUNA WHERE IML_CODIGO = :linha";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetInt32("linha", linha);

            int quantidadeDeletada = query.ExecuteUpdate();
        }

        //public void DeletarPorCodigos(List<int> codigos)
        //{
        //    if (codigos.Count == 0)
        //        return;

        //    string sql = "DELETE FROM T_IMPORTACAO_NOTA_FISCAL_LINHA_COLUNA WHERE IML_CODIGO IN (:codigos)";
        //    var query = this.SessionNHiBernate.CreateSQLQuery(sql);
        //    query.SetParameterList("codigos", codigos);

        //    int quantidadeDeletada = query.ExecuteUpdate();
        //}
    }
}

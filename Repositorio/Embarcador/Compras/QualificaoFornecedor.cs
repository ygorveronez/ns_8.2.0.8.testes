using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class QualificaoFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor>
    {

        public QualificaoFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor BuscarPorOrdemCompra(int compra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor>();
            var result = from obj in query where obj.OrdemCompra.Codigo == compra select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoFornecedor> RelatorioPontuacaoFornecedor(int codigoEmpresa, int codigoComprador, int codigoProduto, int codigoNotaEntrada, int codigoFornecedor, int codigoOrdemCompra, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "";

            query = @"  select  O.ORC_NUMERO OrdemCompra,
                O.ORC_DATA DataOrdemCompra,
                F.FUN_NOME Comprador,
                C.CLI_NOME Fornecedor,
                ISNULL((SELECT AVG(Q.QFO_CRITERIO_PRAZO_ENTREGA_PONTUALIDADE) FROM T_QUALIFICACAO_FORNECEDOR Q WHERE Q.ORC_CODIGO = O.ORC_CODIGO), 0) CriterioPrazoEntregaPontualidade,
                ISNULL((SELECT AVG(Q.QFO_CRITERIO_CARACTERISTICA_ESPECIFICACOES) FROM T_QUALIFICACAO_FORNECEDOR Q WHERE Q.ORC_CODIGO = O.ORC_CODIGO), 0) CriterioCaracteristicaEspecificacoes,
                ISNULL((SELECT AVG(Q.QFO_CRITERIO_QUANTIDADE_RECEBIDA) FROM T_QUALIFICACAO_FORNECEDOR Q WHERE Q.ORC_CODIGO = O.ORC_CODIGO), 0) CriterioQuantidadeRecebida,
                ISNULL((SELECT AVG(Q.QFO_CRITERIO_INTEGRIDADE_FISICA) FROM T_QUALIFICACAO_FORNECEDOR Q WHERE Q.ORC_CODIGO = O.ORC_CODIGO), 0) CriterioIntegridadeFisica,
                ISNULL((SELECT AVG(Q.QFO_CRITERIO_ATENDIMENTO) FROM T_QUALIFICACAO_FORNECEDOR Q WHERE Q.ORC_CODIGO = O.ORC_CODIGO), 0) CriterioAtendimento,
                CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(E.TDE_NUMERO_LONG AS NVARCHAR(2000))
                FROM T_TMS_DOCUMENTO_ENTRADA E
                WHERE E.ORC_CODIGO = O.ORC_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) NotasEntradas
                from T_ORDEM_COMPRA O
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = O.FUN_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = O.CLI_FORNECEDOR WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += @" AND O.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoComprador > 0)
                query += @" AND F.FUN_CODIGO = " + codigoComprador.ToString();

            if (codigoProduto > 0)
                query += @" AND O.ORC_CODIGO IN (SELECT M.ORC_CODIGO FROM T_ORDEM_COMPRA_MERCADORIA M WHERE M.PRO_CODIGO = " + codigoProduto.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoNotaEntrada > 0)
                query += @" AND O.ORC_CODIGO IN (SELECT M.ORC_CODIGO FROM T_TMS_DOCUMENTO_ENTRADA M WHERE M.TDE_CODIGO = " + codigoNotaEntrada.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFornecedor > 0)
                query += @" AND C.CLI_CGCCPF = " + codigoFornecedor.ToString(); // SQL-INJECTION-SAFE

            if (codigoOrdemCompra > 0)
                query += @" AND O.ORC_CODIGO = " + codigoOrdemCompra.ToString(); // SQL-INJECTION-SAFE

            if (dataInicial != DateTime.MinValue)
                query += " AND O.ORC_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND O.ORC_DATA <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;  
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao; 
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao; 
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoFornecedor)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoFornecedor>();
        }

        public int ContarRelatorioPontuacaoFornecedor(int codigoEmpresa, int codigoComprador, int codigoProduto, int codigoNotaEntrada, int codigoFornecedor, int codigoOrdemCompra, DateTime dataInicial, DateTime dataFinal)
        {
            string query = "";

            query = @"   SELECT  COUNT(0) as CONTADOR 
                from T_ORDEM_COMPRA O
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = O.FUN_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = O.CLI_FORNECEDOR WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += @" AND O.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoComprador > 0)
                query += @" AND F.FUN_CODIGO = " + codigoComprador.ToString();

            if (codigoProduto > 0)
                query += @" AND O.ORC_CODIGO IN (SELECT M.ORC_CODIGO FROM T_ORDEM_COMPRA_MERCADORIA M WHERE M.PRO_CODIGO = " + codigoProduto.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoNotaEntrada > 0)
                query += @" AND O.ORC_CODIGO IN (SELECT M.ORC_CODIGO FROM T_TMS_DOCUMENTO_ENTRADA M WHERE M.TDE_CODIGO = " + codigoNotaEntrada.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFornecedor > 0)
                query += @" AND C.CLI_CGCCPF = " + codigoFornecedor.ToString();

            if (codigoOrdemCompra > 0)
                query += @" AND O.ORC_CODIGO = " + codigoOrdemCompra.ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND O.ORC_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND O.ORC_DATA <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }
    }
}

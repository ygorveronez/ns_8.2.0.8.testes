using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoEletronicoRetorno : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno>
    {
        public PagamentoEletronicoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool RetornoJaInserido(int codigoTitulo, string comando, string agendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno>();
            var result = from obj in query where obj.Titulo.Codigo == codigoTitulo && obj.Comando == comando select obj;
            if (!string.IsNullOrWhiteSpace(agendamento))
                result = result.Where(o => o.Agendamento == agendamento);
            return result.Count() > 0;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento> RelatorioRetornoPagamento(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)/*int codigoTitulo, double cnpjFornecedor, int codigoEmpresa, DateTime dataInicialImportacao, DateTime dataFinalImportacao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, int banco, string comando, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false, string codigosRetornos = "", List<int> codigosBanco = null, int codigoBancoPessoa = 0)*/
        {
            string query = @"SELECT B.PER_COMANDO Comando,
                B.PER_NOSSO_NUMERO NossoNumero,
                B.PER_CODIGO_BANCO Banco,
                Banco.BCO_DESCRICAO NomeBanco,
                B.PER_DATA_VENCIMENTO DataVencimento,
                B.PER_DATA_LIQUIDACAO DataPagamento,
                B.PER_VALOR_RETORNO ValorRetorno,
                B.PER_VALOR_RECEBIDO ValorRecebido,
                B.PER_DATA_IMPORTACAO DataImportacao,
                B.TIT_CODIGO CodigoTitulo,
                T.TIT_DATA_VENCIMENTO VencimentoTitulo,
                T.TIT_DATA_EMISSAO EmissaoTitulo,
                T.TIT_VALOR_ORIGINAL ValorTitulo,
                T.TIT_NOSSO_NUMERO NossoNumeroTitulo,
                CL.CLI_NOME Fornecedor,
                BancoCL.BCO_DESCRICAO NomeBancoPessoa,
                ISNULL(B.PER_AGENDAMENTO, '') Agendamento,
                B.PER_NOME_ARQUIVO NomeArquivo
                FROM T_PAGAMENTO_ELETRONICO_RETORNO B
                LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = B.TIT_CODIGO
                LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = T.CLI_CGCCPF
                LEFT OUTER JOIN T_BANCO Banco ON Banco.BCO_NUMERO = B.PER_CODIGO_BANCO 
                LEFT OUTER JOIN T_BANCO BancoCL ON BancoCL.BCO_CODIGO = CL.BCO_CODIGO ";

            query += " WHERE 1 = 1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigosRetornos))
                query += " AND B.PER_CODIGO in (" + filtrosPesquisa.CodigosRetornos + ")";
            else
            {

                if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                    query += " AND B.PER_DATA_IMPORTACAO >= '" + filtrosPesquisa.DataInicialImportacao.ToString("MM/dd/yyyy") + "' AND B.PER_DATA_IMPORTACAO <= '" + filtrosPesquisa.DataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "'";
                else if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao == DateTime.MinValue)
                    query += " AND B.PER_DATA_IMPORTACAO >= '" + filtrosPesquisa.DataInicialImportacao.ToString("MM/dd/yyyy") + "' ";
                else if (filtrosPesquisa.DataInicialImportacao == DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                    query += " AND B.PER_DATA_IMPORTACAO <= '" + filtrosPesquisa.DataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

                if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                    query += " AND B.PER_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataInicialPagamento.ToString("MM/dd/yyyy") + "' AND B.PER_DATA_LIQUIDACAO <= '" + filtrosPesquisa.DataFinalPagamento.ToString("MM/dd/yyyy 23:59:59") + "'";
                else if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento == DateTime.MinValue)
                    query += " AND B.PER_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataInicialPagamento.ToString("MM/dd/yyyy") + "' ";
                else if (filtrosPesquisa.DataInicialPagamento == DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                    query += " AND B.PER_DATA_LIQUIDACAO <= '" + filtrosPesquisa.DataFinalPagamento.ToString("MM/dd/yyyy 23:59:59") + "' ";

                if (filtrosPesquisa.CodigoEmpresa > 0)
                    query += " AND B.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

                if (filtrosPesquisa.CodigoTitulo > 0)
                    query += " AND B.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo.ToString();

                if (filtrosPesquisa.CnpjFornecedor > 0)
                    query += " AND B.TIT_CODIGO IS NOT NULL AND CL.CLI_CGCCPF = " + filtrosPesquisa.CnpjFornecedor.ToString();

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Comando))
                    query += $" AND B.PER_COMANDO = '{filtrosPesquisa.Comando}'";

                if (filtrosPesquisa.CodigosBanco?.Count > 0)
                    query += $" AND Banco.BCO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosBanco)}) ";

                if (filtrosPesquisa.CodigosBancoPessoa?.Count > 0)
                    query += $" AND CL.BCO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosBancoPessoa)})";

                if (filtrosPesquisa.CodigoConfiguracaoBoleto > 0)
                    query += $" AND T.BCF_CODIGO = {filtrosPesquisa.CodigoConfiguracaoBoleto} ";
            }

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (filtrosPesquisa.Paginar)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento>();
        }

        public int ContarRetornoPagamento(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento filtrosPesquisa/*int codigoTitulo, double cnpjFornecedor, int codigoEmpresa, DateTime dataInicialImportacao, DateTime dataFinalImportacao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, List<int> codigosBanco = null, int codigoBancoPessoa = 0*/)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                FROM T_PAGAMENTO_ELETRONICO_RETORNO B
                LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = B.TIT_CODIGO
                LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = T.CLI_CGCCPF
                LEFT OUTER JOIN T_BANCO Banco ON Banco.BCO_NUMERO = B.PER_CODIGO_BANCO 
                LEFT OUTER JOIN T_BANCO BancoCL ON BancoCL.BCO_CODIGO = CL.BCO_CODIGO ";

            query += " WHERE 1 = 1 ";

            if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                query += " AND B.PER_DATA_IMPORTACAO >= '" + filtrosPesquisa.DataInicialImportacao.ToString("MM/dd/yyyy") + "' AND B.PER_DATA_IMPORTACAO <= '" + filtrosPesquisa.DataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao == DateTime.MinValue)
                query += " AND B.PER_DATA_IMPORTACAO >= '" + filtrosPesquisa.DataInicialImportacao.ToString("MM/dd/yyyy") + "' ";
            else if (filtrosPesquisa.DataInicialImportacao == DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                query += " AND B.PER_DATA_IMPORTACAO <= '" + filtrosPesquisa.DataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                query += " AND B.PER_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataInicialPagamento.ToString("MM/dd/yyyy") + "' AND B.PER_DATA_LIQUIDACAO <= '" + filtrosPesquisa.DataFinalPagamento.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento == DateTime.MinValue)
                query += " AND B.PER_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataInicialPagamento.ToString("MM/dd/yyyy") + "' ";
            else if (filtrosPesquisa.DataInicialPagamento == DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                query += " AND B.PER_DATA_LIQUIDACAO <= '" + filtrosPesquisa.DataFinalPagamento.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND B.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CodigoTitulo > 0)
                query += " AND B.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo.ToString();

            if (filtrosPesquisa.CnpjFornecedor > 0)
                query += " AND B.TIT_CODIGO IS NOT NULL AND CL.CLI_CGCCPF = " + filtrosPesquisa.CnpjFornecedor.ToString();

            if (filtrosPesquisa.CodigosBanco?.Count > 0)
                query += $" AND Banco.BCO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosBanco)}) ";

            if (filtrosPesquisa.CodigosBancoPessoa?.Count > 0)
                query += $" AND CL.BCO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosBancoPessoa)}) ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }
    }
}

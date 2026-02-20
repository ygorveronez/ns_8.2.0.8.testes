using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoRetorno : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno>
    {
        public BoletoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno> BuscarPorArquivoRetorno(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno>();
            var result = from obj in query where obj.BoletoRetornoArquivo.Codigo == codigo select obj;
            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> RelatorioRetornoBoleto(int codigoEmpresa, DateTime dataInicialImportacao, DateTime dataFinalImportacao, DateTime dataInicialOcorrencia, DateTime dataFinalOcorrencia, int codigoBoletoConfiguracao, int codigoBoletoComando, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false, int codigoArquivoRetorno = 0)
        {
            string query = @"SELECT DISTINCT B.BRT_COMANDO Comando,
                  B.BRT_NOSSO_NUMERO NossoNumero,
                  B.BRT_CODIGO_BANCO Banco,
                  B.BRT_DATA_VENCIMENTO DataVencimento,
                  B.BRT_DATA_LIQUIDACAO DataOcorrencia,
                  B.BRT_VALOR_RETORNO ValorRetorno,
                  B.BRT_VALOR_TITULO ValorDocumento,
                  B.BRT_JUROS ValorJuros,
                  B.BRT_OUTROS ValorOutrasDespesas,
                  B.BRT_TARIFA ValorTarifa,
                  B.BRT_VALOR_RECEBIDO ValorRecebido,
                  B.BRT_DATA_CREDITO DataCredito,
                  B.BRT_CODIGO_REJEICAO CodigoRejeicao,
                  B.BRT_DATA_BAIXA DataBaixa,
                  B.BRT_DATA_ARQUIVO DataImportacao,   
	              T.TIT_CODIGO CodigoTitulo,         
	              T.TIT_DATA_VENCIMENTO VencimentoTitulo,
	              T.TIT_DATA_EMISSAO EmissaoTitulo,
	              T.TIT_VALOR_ORIGINAL ValorTitulo,
	              T.TIT_NOSSO_NUMERO NossoNumeroTitulo,
	              T.TIT_SEQUENCIA Sequencia,
	              CL.CLI_NOME Cliente,
	              E.EMP_FANTASIA Empresa,
	              C.BRC_DESCRICAO DescricaoComando
              FROM T_BOLETO_RETORNO B
              LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = B.TIT_CODIGO
              LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = T.CLI_CGCCPF
              LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
              LEFT OUTER JOIN T_BOLETO_RETORNO_COMANDO C ON C.BRC_CODIGO = B.BRC_CODIGO";

            query += " WHERE 1 = 1 ";

            if (codigoArquivoRetorno > 0)
                query += " AND B.BRA_CODIGO = " + codigoArquivoRetorno.ToString();

            if (dataInicialImportacao > DateTime.MinValue && dataFinalImportacao > DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO >= '" + dataInicialImportacao.ToString("MM/dd/yyyy") + "' AND B.BRT_DATA_ARQUIVO <= '" + dataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialImportacao > DateTime.MinValue && dataFinalImportacao == DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO >= '" + dataInicialImportacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialImportacao == DateTime.MinValue && dataFinalImportacao > DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO <= '" + dataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialOcorrencia > DateTime.MinValue && dataFinalOcorrencia > DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO >= '" + dataInicialOcorrencia.ToString("MM/dd/yyyy") + "' AND B.BRT_DATA_LIQUIDACAO <= '" + dataFinalOcorrencia.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialOcorrencia > DateTime.MinValue && dataFinalOcorrencia == DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO >= '" + dataInicialOcorrencia.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialOcorrencia == DateTime.MinValue && dataFinalOcorrencia > DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO <= '" + dataFinalOcorrencia.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (codigoBoletoConfiguracao > 0)
                query += " AND T.BCF_CODIGO = " + codigoBoletoConfiguracao.ToString();

            if (codigoBoletoComando > 0)
                query += " AND B.BRC_CODIGO = " + codigoBoletoComando.ToString();

            if (codigoEmpresa > 0)
                query += " AND B.EMP_CODIGO = " + codigoEmpresa.ToString();

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

            if (maximoRegistros > 0 && paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto>();
        }

        public int ContarRetornoBoleto(int codigoEmpresa, DateTime dataInicialImportacao, DateTime dataFinalImportacao, DateTime dataInicialOcorrencia, DateTime dataFinalOcorrencia, int codigoBoletoConfiguracao, int codigoBoletoComando)
        {
            string query = @"SELECT COUNT(DISTINCT B.TIT_CODIGO) as CONTADOR FROM T_BOLETO_RETORNO B
              LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = B.TIT_CODIGO
              LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = T.CLI_CGCCPF
              LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
              LEFT OUTER JOIN T_BOLETO_RETORNO_COMANDO C ON C.BRC_CODIGO = B.BRC_CODIGO";

            query += " WHERE 1 = 1 ";

            if (dataInicialImportacao > DateTime.MinValue && dataFinalImportacao > DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO >= '" + dataInicialImportacao.ToString("MM/dd/yyyy") + "' AND B.BRT_DATA_ARQUIVO <= '" + dataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialImportacao > DateTime.MinValue && dataFinalImportacao == DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO >= '" + dataInicialImportacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialImportacao == DateTime.MinValue && dataFinalImportacao > DateTime.MinValue)
                query += " AND B.BRT_DATA_ARQUIVO <= '" + dataFinalImportacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialOcorrencia > DateTime.MinValue && dataFinalOcorrencia > DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO >= '" + dataInicialOcorrencia.ToString("MM/dd/yyyy") + "' AND B.BRT_DATA_LIQUIDACAO <= '" + dataFinalOcorrencia.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialOcorrencia > DateTime.MinValue && dataFinalOcorrencia == DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO >= '" + dataInicialOcorrencia.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialOcorrencia == DateTime.MinValue && dataFinalOcorrencia > DateTime.MinValue)
                query += " AND B.BRT_DATA_LIQUIDACAO <= '" + dataFinalOcorrencia.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (codigoBoletoConfiguracao > 0)
                query += " AND T.BCF_CODIGO = " + codigoBoletoConfiguracao.ToString();

            if (codigoBoletoComando > 0)
                query += " AND B.BRC_CODIGO = " + codigoBoletoComando.ToString();

            if (codigoEmpresa > 0)
                query += " AND B.EMP_CODIGO = " + codigoEmpresa.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }
    }
}

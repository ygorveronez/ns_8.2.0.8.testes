using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoEletronico : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>
    {
        public PagamentoEletronico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero(int codigoBoletoConfiguracao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();
            var result = from obj in query where obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao && obj.Empresa.Codigo == codigoEmpresa select obj;

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico situacaoAutorizacaoPagamentoEletronico, int numero, int codigoBoletoConfiguracao, int codigoEmpresa, int codigoPagamentoEletronico, int codigoTitulo, double cnpjFornecedor, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);
            if (codigoBoletoConfiguracao > 0)
                result = result.Where(o => o.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
            if (codigoPagamentoEletronico > 0)
                result = result.Where(o => o.Codigo == codigoPagamentoEletronico);
            if (dataPagamentoInicial > DateTime.MinValue)
                result = result.Where(o => o.DataPagamento.Value.Date >= dataPagamentoInicial);
            if (dataPagamentoFinal > DateTime.MinValue)
                result = result.Where(o => o.DataPagamento.Value.Date <= dataPagamentoFinal);
            if (situacaoAutorizacaoPagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Iniciada)
                result = result.Where(o => o.SituacaoAutorizacaoPagamentoEletronico.Value == situacaoAutorizacaoPagamentoEletronico || o.SituacaoAutorizacaoPagamentoEletronico == null);
            else if (situacaoAutorizacaoPagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Iniciada && situacaoAutorizacaoPagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Todos)
                result = result.Where(o => o.SituacaoAutorizacaoPagamentoEletronico.Value == situacaoAutorizacaoPagamentoEletronico && o.SituacaoAutorizacaoPagamentoEletronico != null);

            if (codigoTitulo > 0)
                result = result.Where(o => o.Titulos.Any(t => t.Titulo.Codigo == codigoTitulo));
            if (cnpjFornecedor > 0)
                result = result.Where(o => o.Titulos.Any(t => t.Titulo.Pessoa.CPF_CNPJ == cnpjFornecedor));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico situacaoAutorizacaoPagamentoEletronico, int numero, int codigoBoletoConfiguracao, int codigoEmpresa, int codigoPagamentoEletronico, int codigoTitulo, double cnpjFornecedor, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);
            if (codigoBoletoConfiguracao > 0)
                result = result.Where(o => o.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
            if (codigoPagamentoEletronico > 0)
                result = result.Where(o => o.Codigo == codigoPagamentoEletronico);
            if (dataPagamentoInicial > DateTime.MinValue)
                result = result.Where(o => o.DataPagamento.Value.Date >= dataPagamentoInicial);
            if (dataPagamentoFinal > DateTime.MinValue)
                result = result.Where(o => o.DataPagamento.Value.Date <= dataPagamentoFinal);
            if (situacaoAutorizacaoPagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Iniciada)
                result = result.Where(o => o.SituacaoAutorizacaoPagamentoEletronico.Value == situacaoAutorizacaoPagamentoEletronico || o.SituacaoAutorizacaoPagamentoEletronico == null);
            else if (situacaoAutorizacaoPagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Iniciada && situacaoAutorizacaoPagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Todos)
                result = result.Where(o => o.SituacaoAutorizacaoPagamentoEletronico.Value == situacaoAutorizacaoPagamentoEletronico && o.SituacaoAutorizacaoPagamentoEletronico != null);

            if (codigoTitulo > 0)
                result = result.Where(o => o.Titulos.Any(t => t.Titulo.Codigo == codigoTitulo));
            if (cnpjFornecedor > 0)
                result = result.Where(o => o.Titulos.Any(t => t.Titulo.Pessoa.CPF_CNPJ == cnpjFornecedor));

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RemessaPagamento> RelatorioRemessaPagamento(int codigo)
        {
            string query = @"SELECT E.EMP_RAZAO Empresa, 
                E.EMP_CNPJ CNPJEmpresa,
                B.BCF_NUMERO_BANCO NumeroBanco, 
                B.BCF_DIGITO_BANCO DigitoBanco, 
                B.BCF_DESCRICAO_BANCO Banco, 
                B.BCF_NUMERO_AGENCIA NumeroAgencia,
                B.BCF_DIGITO_AGENCIA DigitoAgencia, 
                B.BCF_NUMERO_CONTA NumeroConta,
                B.BCF_DIGITO_CONTA DigitoConta,
                B.BCF_NUMERO_CONVENIO NumeroConvenio,
                T.TIT_CODIGO CodigoTitulo,
                T.TIT_SEQUENCIA Parcela,
                T.TIT_NOSSO_NUMERO NumeroBoleto,
                T.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento,
                (T.TIT_VALOR_ORIGINAL -  ISNULL(T.TIT_DESCONTO, 0) + ISNULL(T.TIT_ACRESCIMO, 0)) Valor,
                T.TIT_DATA_VENCIMENTO DataVencimento,
                T.TIT_OBSERVACAO Observacao,
                F.FUN_NOME Operador,
                P.PAE_DATA_GERACAO DataGeracao,
                P.PAE_DATA_PAGAMENTO DataPagamento,
                P.PAE_FINALIDADE Finalidade,
                P.PAE_MODALIDADE Modalidade,
                P.PAE_NUMERO Numero,
                P.PAE_QUANTIDADE_TITULOS QtdTitulos,
                P.PAE_TIPO_CONTA TipoConta,
                P.PAE_VALOR_TOTAL ValorTotal,
                C.CLI_NOME +
                CASE 
					WHEN C.CLI_CGCCPF_PORTADOR_CONTA IS NULL THEN ''
					ELSE ' (' + Portador.CLI_NOME + ')'
				END Fornecedor,
                LTRIM(STR(C.CLI_CGCCPF, 25, 0)) +
				CASE 
					WHEN C.CLI_CGCCPF_PORTADOR_CONTA IS NULL THEN ''
					ELSE ' (' + LTRIM(STR(Portador.CLI_CGCCPF, 25, 0)) + ')'
				END CNPJFornecedor
                FROM T_PAGAMENTO_ELETRONICO P
                JOIN T_PAGAMENTO_ELETRONICO_TITULO PT ON PT.PAE_CODIGO = P.PAE_CODIGO
                JOIN T_TITULO T ON T.TIT_CODIGO = PT.TIT_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = P.EMP_CODIGO
                JOIN T_BOLETO_CONFIGURACAO B ON B.BCF_CODIGO = P.BCF_CODIGO
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = P.FUN_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                LEFT OUTER JOIN T_CLIENTE Portador ON Portador.CLI_CGCCPF = C.CLI_CGCCPF_PORTADOR_CONTA
                WHERE P.PAE_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RemessaPagamento)));

            return nhQuery.SetTimeout(3000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RemessaPagamento>();
        }

        #endregion
    }
}

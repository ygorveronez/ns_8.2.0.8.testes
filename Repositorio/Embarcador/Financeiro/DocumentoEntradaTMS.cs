using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaTMS : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>
    {
        public DocumentoEntradaTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public DocumentoEntradaTMS(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaDocumentoEntrada = Consultar(filtrosPesquisa);

            consultaDocumentoEntrada = consultaDocumentoEntrada
                .Fetch(o => o.Fornecedor)
                .Fetch(o => o.NaturezaOperacao)
                .Fetch(o => o.TipoMovimento)
                .Fetch(o => o.CFOP);

            return ObterLista(consultaDocumentoEntrada, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> ConsultarDocumentoReferencia(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoSPEDFiscal situacaoDocumentoSPEDFiscal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataEntradaInicial, DateTime dataEntradaFinal, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, int codigoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, string serie, string chave, decimal valorTotal, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaDocumentoEntrada = ConsultarDocumentoReferencia(situacaoDocumentoSPEDFiscal, dataEmissaoInicial, dataEmissaoFinal, dataEntradaInicial, dataEntradaFinal, acerto, codigoVeiculo, tipoServico, numeroInicial, numeroFinal, serie, chave, valorTotal, codigoEmpresa);

            consultaDocumentoEntrada = consultaDocumentoEntrada.Fetch(o => o.Fornecedor);

            return ObterLista(consultaDocumentoEntrada, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsultaDocumentoReferencia(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoSPEDFiscal situacaoDocumentoSPEDFiscal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataEntradaInicial, DateTime dataEntradaFinal, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, int codigoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, string serie, string chave, decimal valorTotal, int codigoEmpresa)
        {
            var consultaDocumentoEntrada = ConsultarDocumentoReferencia(situacaoDocumentoSPEDFiscal, dataEmissaoInicial, dataEmissaoFinal, dataEntradaInicial, dataEntradaFinal, acerto, codigoVeiculo, tipoServico, numeroInicial, numeroFinal, serie, chave, valorTotal, codigoEmpresa);

            return consultaDocumentoEntrada.Count();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(o => o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<long> ConsultarDocumentoPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoDocumentoEntrada[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(o => o.DataEntrada.Date <= dataFechamento.Date && situacao.Contains(o.Situacao));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Destinatario.Codigo == codigoEmpresa);

            return query.Select(o => o.Numero).ToList();
        }

        public int BuscarUltimoNumeroLancamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            return query.Max(o => (int?)o.NumeroLancamento) ?? 0;
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }
       
        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorChave(string chave, int codigoDiff = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Chave == chave &&
                          obj.Situacao != SituacaoDocumentoEntrada.Cancelado &&
                          obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            if (codigoDiff > 0)
                query = query.Where(o => o.Codigo != codigoDiff);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarPorFornecedorNumeroEPeriodo(int numero, double cpfCNPJFornecedor, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.Fornecedor.CPF_CNPJ == cpfCNPJFornecedor &&
                                        obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date <= dataEmissaoFinal.Date);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorFornecedorNumeroESerie(Int64 numero, string serie, double cpfCNPJFornecedor, int codigoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Numero == numero && obj.Serie == serie && obj.Fornecedor.CPF_CNPJ == cpfCNPJFornecedor &&
                            obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            if (codigoDiff > 0)
                query = query.Where(o => o.Codigo != codigoDiff);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorFornecedorENumero(Int64 numero, double cpfCNPJFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Numero == numero && obj.Fornecedor.CPF_CNPJ == cpfCNPJFornecedor &&
                            obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorFornecedorENumeroESerie(Int64 numero, double cpfCNPJFornecedor, string serie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Numero == numero && obj.Fornecedor.CPF_CNPJ == cpfCNPJFornecedor && obj.Serie == serie &&
                            obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorChaveECodigoDiff(int codigoDiff, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Chave == chave && obj.Codigo != codigoDiff &&
                            obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS BuscarPorFornecedorNumeroSerieECodigoDiff(int codigoDiff, Int64 numero, string serie, double cpfCNPJFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = from obj in query
                    where obj.Numero == numero && obj.Serie == serie && obj.Fornecedor.CPF_CNPJ == cpfCNPJFornecedor && obj.Codigo != codigoDiff &&
                            obj.Situacao != SituacaoDocumentoEntrada.Cancelado && obj.Situacao != SituacaoDocumentoEntrada.Anulado
                    select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarDocumentoEntrada(int tipoDocumentoEmEBS, List<int> codigosNotas, bool selecionarTodos, List<int> modelosDocumento, int codigoFilial, DateTime dataEntradaInicial, DateTime dataEntradaFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(o => o.Situacao == SituacaoDocumentoEntrada.Finalizado);

            if (modelosDocumento != null && modelosDocumento.Count > 0)
                query = query.Where(obj => modelosDocumento.Contains(obj.Modelo.Codigo));

            if (selecionarTodos)
                query = query.Where(o => !codigosNotas.Contains(o.Codigo));
            else
                query = query.Where(o => codigosNotas.Contains(o.Codigo));

            if (codigoFilial > 0)
                query = query.Where(obj => obj.Destinatario.Codigo == codigoFilial);

            if (tipoDocumentoEmEBS == 1)//nunca gerado
                query = query.Where(obj => obj.DocumentoEmEBS == false || obj.DocumentoEmEBS == null);
            else if (tipoDocumentoEmEBS == 2)//gerado em algum arquivo
                query = query.Where(obj => obj.DocumentoEmEBS == true);

            if (dataEntradaInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataEntrada >= dataEntradaInicial.Date);

            if (dataEntradaFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataEntrada < dataEntradaFinal.AddDays(1).Date);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarDocumentoPorEmpresa(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.Situacao == SituacaoDocumentoEntrada.Finalizado && obj.CFOP != null && obj.CFOP.Tipo == Dominio.Enumeradores.TipoCFOP.Entrada);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Destinatario.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date <= dataEmissaoFinal.Date);

            return query
                .Fetch(c => c.Destinatario).ThenFetch(c => c.Localidade)
                .Fetch(c => c.Modelo)
                .Fetch(c => c.CFOP)
                .Fetch(c => c.Fornecedor).ThenFetch(c => c.Localidade).ThenFetch(c => c.Pais)
                .Fetch(c => c.TipoMovimento).ThenFetch(c => c.PlanoDeContaDebito)
                .Fetch(c => c.TipoMovimento).ThenFetch(c => c.PlanoDeContaCredito)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarNFSePorEmpresa(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, List<int> codigosModeloDocumentoFiscal, DateTime dataEntradaInicial, DateTime dataEntradaFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.Situacao == SituacaoDocumentoEntrada.Finalizado);

            if (codigosModeloDocumentoFiscal != null && codigosModeloDocumentoFiscal.Count > 0)
                query = query.Where(obj => codigosModeloDocumentoFiscal.Contains(obj.Modelo.Codigo));
            else
                query = query.Where(obj => obj.Modelo.Numero == "39");

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Destinatario.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao <= dataEmissaoFinal);

            if (dataEntradaInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEntrada >= dataEntradaInicial);

            if (dataEntradaFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEntrada <= dataEntradaFinal);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();
            var result = from obj in query where obj.OrdemServico.Codigo == codigoOrdemServico select obj;
            return result.ToList();
        }

        public decimal BuscarValorTotalPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(o => o.Situacao == SituacaoDocumentoEntrada.Finalizado && o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Sum(o => (decimal?)o.ValorTotal) ?? 0m;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.AcertosFinalizadosDocumentoEntrada> AbastecimentosEmAcertoFechado(int codigoDocumentoEntrada)
        {
            string query = @" SELECT distinct Acerto.ACV_NUMERO NumeroAcerto FROM T_ACERTO_DE_VIAGEM Acerto
                JOIN T_ACERTO_ABASTECIMENTO AcertoAbastecimento on AcertoAbastecimento.ACV_CODIGO = Acerto.ACV_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbastecimento on ItemAbastecimento.ABA_CODIGO = AcertoAbastecimento.ABA_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM Item on Item.TDI_CODIGO = ItemAbastecimento.TDI_CODIGO
                where Acerto.ACV_SITUACAO = 2 ";

            query += " AND Item.TDE_CODIGO = " + codigoDocumentoEntrada;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.AcertosFinalizadosDocumentoEntrada)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Financeiro.AcertosFinalizadosDocumentoEntrada>();
        }

        public void DeletarAbastecimentosEmAcerto(int codigoDocumentoEntrada)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery(
                        $@"DELETE AcertoAbastecimento 
                            FROM T_ACERTO_DE_VIAGEM Acerto
                            JOIN T_ACERTO_ABASTECIMENTO AcertoAbastecimento on AcertoAbastecimento.ACV_CODIGO = Acerto.ACV_CODIGO
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbastecimento on ItemAbastecimento.ABA_CODIGO = AcertoAbastecimento.ABA_CODIGO
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM Item on Item.TDI_CODIGO = ItemAbastecimento.TDI_CODIGO
                            where Acerto.ACV_SITUACAO = 3 AND Item.TDE_CODIGO = {codigoDocumentoEntrada} ;").ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery(
                                $@"DELETE AcertoAbastecimento 
                                FROM T_ACERTO_DE_VIAGEM Acerto
                                JOIN T_ACERTO_ABASTECIMENTO AcertoAbastecimento on AcertoAbastecimento.ACV_CODIGO = Acerto.ACV_CODIGO
                                JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbastecimento on ItemAbastecimento.ABA_CODIGO = AcertoAbastecimento.ABA_CODIGO
                                JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM Item on Item.TDI_CODIGO = ItemAbastecimento.TDI_CODIGO
                                where Acerto.ACV_SITUACAO = 3 AND Item.TDE_CODIGO = {codigoDocumentoEntrada} ;").ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public void DeletarAbastecimentosEmComissao(int codigoDocumentoEntrada)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery(
                        $@"DELETE CA FROM T_COMISSAO_FUNCIONARIO_MOTORISTA_ABASTECIMENTO CA 
                            JOIN T_COMISSAO_FUNCIONARIO C ON C.CMF_CODIGO = CA.CFM_CODIGO
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbastecimento on ItemAbastecimento.ABA_CODIGO = CA.ABA_CODIGO
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM Item on Item.TDI_CODIGO = ItemAbastecimento.TDI_CODIGO
                            WHERE C.CMF_SITUACAO_COMISSAO_FUNCIONARIO = 5 AND Item.TDE_CODIGO = {codigoDocumentoEntrada} ;").ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery(
                                $@"DELETE DELETE CA FROM T_COMISSAO_FUNCIONARIO_MOTORISTA_ABASTECIMENTO CA 
                                    JOIN T_COMISSAO_FUNCIONARIO C ON C.CMF_CODIGO = CA.CFM_CODIGO
                                    JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbastecimento on ItemAbastecimento.ABA_CODIGO = CA.ABA_CODIGO
                                    JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM Item on Item.TDI_CODIGO = ItemAbastecimento.TDI_CODIGO
                                    WHERE C.CMF_SITUACAO_COMISSAO_FUNCIONARIO = 5 AND Item.TDE_CODIGO = {codigoDocumentoEntrada} ;").ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> ConsultarDocumentosVinculadosComOrdemCompra(int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.OrdemCompra.Codigo == codigoOrdemCompra && obj.Situacao == SituacaoDocumentoEntrada.Finalizado);

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> BuscarDocumentosEntradaPendentesIntegracao(int inicio, int quantidadeRegistros, string codigoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();
            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            if (!string.IsNullOrEmpty(codigoTipoMovimento))
                query = query.Where(obj => obj.TipoMovimento.CodigoIntegracao == codigoTipoMovimento);

            return query.OrderBy(c => c.Codigo).Skip(inicio).Take(quantidadeRegistros > 0 ? quantidadeRegistros : 100).ToList();
        }

        public int ContarDocumentoPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();
            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);
            return query.Count();
        }

        public int ContarConsultarDocumentosVinculadosComOrdemCompra(int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.OrdemCompra.Codigo == codigoOrdemCompra && obj.Situacao == SituacaoDocumentoEntrada.Finalizado);

            return query.Count();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryDocumentoEntradaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>();
            var resultDocumentoEntradaIntegracao = from obj in queryDocumentoEntradaIntegracao where obj.Codigo == codigo select obj;

            var queryDocumentoEntradaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultDocumentoEntradaIntegracaoArquivo = from obj in queryDocumentoEntradaIntegracaoArquivo where resultDocumentoEntradaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultDocumentoEntradaIntegracaoArquivo.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryDocumentoEntradaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>();
            var resultDocumentoEntradaIntegracao = from obj in queryDocumentoEntradaIntegracao where obj.Codigo == codigo select obj;

            var queryDocumentoEntradaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultDocumentoEntradaIntegracaoArquivo = from obj in queryDocumentoEntradaIntegracaoArquivo where resultDocumentoEntradaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultDocumentoEntradaIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public decimal SomarConsultarDocumentosVinculadosComOrdemCompra(int codigoDocumentoEntrada, int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            query = query.Where(obj => obj.OrdemCompra.Codigo == codigoOrdemCompra && obj.Codigo != codigoDocumentoEntrada && obj.Situacao == SituacaoDocumentoEntrada.Finalizado);

            return query.Sum(obj => (decimal?)obj.ValorTotal) ?? 0m;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao> RelatorioManutencao(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string pattern = "yyyy-MM-dd";

            string query = @"SELECT D.TDE_DATA_EMISSAO DataEmissao,
                C.CLI_NOME Fornecedor,
                ISNULL(S.VSE_DESCRICAO, 'SEM SEGMENTO') Segmento,
                V.VEI_PLACA Placa,
                T.TIM_DESCRICAO TipoMovimento,
                N.NAT_DESCRICAO NaturezaOperacao,
                P.PRO_DESCRICAO Produto,
                I.TDI_QUANTIDADE Quantidade,
                I.TDI_VALOR_UNITARIO ValorUnitario,
                I.TDI_VALOR_TOTAL ValorTotal,
                E.EQP_DESCRICAO Equipamento,
                I.TDI_VALOR_CUSTO_UNITARIO ValorCustoUnitario,
                I.TDI_VALOR_CUSTO_TOTAL ValorCustoTotal,
                CAST(D.TDE_NUMERO_LONG AS NVARCHAR(30))  NumeroDocumento,
                CAST(D.TDE_SERIE AS NVARCHAR(30)) SerieDocumento,
                D.CLI_CGCCPF CNPJFornecedor,
                OS.OSE_HORIMETRO Horimetro,
                OS.OSE_QUILOMETRAGEM_VEICULO QuilometragemVeiculo,
                SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_DESCRICAO
		                FROM T_CENTRO_RESULTADO centroResultado
                        JOIN T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO documentoCentroResultado ON documentoCentroResultado.CRE_CODIGO = centroResultado.CRE_CODIGO
		                WHERE documentoCentroResultado.TDE_CODIGO = D.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS CentroResultado,
                D.TDE_SITUACAO SituacaoLancDocEntrada

            FROM T_TMS_DOCUMENTO_ENTRADA D
            JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
            LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = D.VEI_CODIGO
            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM I ON I.TDE_CODIGO = D.TDE_CODIGO
            JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
            JOIN T_TIPO_MOVIMENTO T ON T.TIM_CODIGO = I.TIM_CODIGO
            JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
            LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = D.OSE_CODIGO
            LEFT OUTER JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
            LEFT OUTER JOIN T_EQUIPAMENTO E ON E.EQP_CODIGO = D.EQP_CODIGO
            WHERE 1 = 1";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'";

            if (filtrosPesquisa.CodigoPlacas != null && filtrosPesquisa.CodigoPlacas.Count > 0)
            {
                IEnumerable<int> codigosVeiculossValidos = filtrosPesquisa.CodigoPlacas.Where(o => o > 0);

                if (codigosVeiculossValidos.Count() > 0)
                    query += " and (D.VEI_CODIGO in (" + string.Join(",", codigosVeiculossValidos) + "))";
                else
                    query += " and (D.VEI_CODIGO IS NULL)";
            }

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                query += " and V.VSE_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosSegmento) + ")";

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                query += " AND T.TIM_CODIGO  = " + filtrosPesquisa.CodigoTipoMovimento.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao.Count > 0)
                query += " AND N.NAT_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosNaturezaOperacao) + ")";

            if (filtrosPesquisa.CodigosEquipamento != null && filtrosPesquisa.CodigosEquipamento.Count > 0)
                query += " and D.EQP_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosEquipamento) + ")";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND D.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND D.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjCpfFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfFornecedor.ToString();

            if (filtrosPesquisa.CentrosResultado.Count > 0)
                query += $" AND D.TDE_CODIGO in (select TDE_CODIGO from T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO where CRE_CODIGO in ({string.Join(",", filtrosPesquisa.CentrosResultado)}))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Produtos.Count > 0)
                query += " AND I.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.Produtos) + ")";

            if (filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento != null && filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento == true)
                query += " AND (V.VEI_CODIGO is not null or E.EQP_CODIGO is not null)";

            if (filtrosPesquisa?.SituacaoLancDocEntrada > 0)
                query += $" AND (D.TDE_SITUACAO = {filtrosPesquisa?.SituacaoLancDocEntrada.GetHashCode()})";

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao>> RelatorioManutencaoAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string pattern = "yyyy-MM-dd";

            string query = @"SELECT D.TDE_DATA_EMISSAO DataEmissao,
                C.CLI_NOME Fornecedor,
                ISNULL(S.VSE_DESCRICAO, 'SEM SEGMENTO') Segmento,
                V.VEI_PLACA Placa,
                T.TIM_DESCRICAO TipoMovimento,
                N.NAT_DESCRICAO NaturezaOperacao,
                P.PRO_DESCRICAO Produto,
                I.TDI_QUANTIDADE Quantidade,
                I.TDI_VALOR_UNITARIO ValorUnitario,
                I.TDI_VALOR_TOTAL ValorTotal,
                E.EQP_DESCRICAO Equipamento,
                I.TDI_VALOR_CUSTO_UNITARIO ValorCustoUnitario,
                I.TDI_VALOR_CUSTO_TOTAL ValorCustoTotal,
                CAST(D.TDE_NUMERO_LONG AS NVARCHAR(30))  NumeroDocumento,
                CAST(D.TDE_SERIE AS NVARCHAR(30)) SerieDocumento,
                D.CLI_CGCCPF CNPJFornecedor,
                OS.OSE_HORIMETRO Horimetro,
                OS.OSE_QUILOMETRAGEM_VEICULO QuilometragemVeiculo,
                SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_DESCRICAO
		                FROM T_CENTRO_RESULTADO centroResultado
                        JOIN T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO documentoCentroResultado ON documentoCentroResultado.CRE_CODIGO = centroResultado.CRE_CODIGO
		                WHERE documentoCentroResultado.TDE_CODIGO = D.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS CentroResultado,
                D.TDE_SITUACAO SituacaoLancDocEntrada

            FROM T_TMS_DOCUMENTO_ENTRADA D
            JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
            LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = D.VEI_CODIGO
            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM I ON I.TDE_CODIGO = D.TDE_CODIGO
            JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
            JOIN T_TIPO_MOVIMENTO T ON T.TIM_CODIGO = I.TIM_CODIGO
            JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
            LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = D.OSE_CODIGO
            LEFT OUTER JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
            LEFT OUTER JOIN T_EQUIPAMENTO E ON E.EQP_CODIGO = D.EQP_CODIGO
            WHERE 1 = 1";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'";

            if (filtrosPesquisa.CodigoPlacas != null && filtrosPesquisa.CodigoPlacas.Count > 0)
            {
                IEnumerable<int> codigosVeiculossValidos = filtrosPesquisa.CodigoPlacas.Where(o => o > 0);

                if (codigosVeiculossValidos.Count() > 0)
                    query += " and (D.VEI_CODIGO in (" + string.Join(",", codigosVeiculossValidos) + "))";
                else
                    query += " and (D.VEI_CODIGO IS NULL)";
            }

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                query += " and V.VSE_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosSegmento) + ")";

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                query += " AND T.TIM_CODIGO  = " + filtrosPesquisa.CodigoTipoMovimento.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao.Count > 0)
                query += " AND N.NAT_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosNaturezaOperacao) + ")";

            if (filtrosPesquisa.CodigosEquipamento != null && filtrosPesquisa.CodigosEquipamento.Count > 0)
                query += " and D.EQP_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosEquipamento) + ")";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND D.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND D.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjCpfFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfFornecedor.ToString();

            if (filtrosPesquisa.CentrosResultado.Count > 0)
                query += $" AND D.TDE_CODIGO in (select TDE_CODIGO from T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO where CRE_CODIGO in ({string.Join(",", filtrosPesquisa.CentrosResultado)}))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Produtos.Count > 0)
                query += " AND I.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.Produtos) + ")";

            if (filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento != null && filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento == true)
                query += " AND (V.VEI_CODIGO is not null or E.EQP_CODIGO is not null)";

            if (filtrosPesquisa?.SituacaoLancDocEntrada > 0)
                query += $" AND (D.TDE_SITUACAO = {filtrosPesquisa?.SituacaoLancDocEntrada.GetHashCode()})";
            
            if (filtrosPesquisa.CodigosVeiculo != null && filtrosPesquisa.CodigosVeiculo.Count > 0)
                query += " AND V.VEI_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosVeiculo) + ")";

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao)));

            return await nhQuery.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao>();
        }


        public int ContarManutencao(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            string query = @"SELECT COUNT(0) as CONTADOR
                FROM T_TMS_DOCUMENTO_ENTRADA D
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = D.VEI_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM I ON I.TDE_CODIGO = D.TDE_CODIGO
                JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
                JOIN T_TIPO_MOVIMENTO T ON T.TIM_CODIGO = I.TIM_CODIGO
                JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
                LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = D.OSE_CODIGO
                LEFT OUTER JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                LEFT OUTER JOIN T_EQUIPAMENTO E ON E.EQP_CODIGO = D.EQP_CODIGO
                WHERE 1 = 1";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += $" and CAST(D.TDE_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'";

            if (filtrosPesquisa.CodigoPlacas != null && filtrosPesquisa.CodigoPlacas.Count > 0)
            {
                IEnumerable<int> codigosVeiculossValidos = filtrosPesquisa.CodigoPlacas.Where(o => o > 0);

                if (codigosVeiculossValidos.Count() > 0)
                    query += " and (D.VEI_CODIGO in (" + string.Join(",", codigosVeiculossValidos) + "))";
                else
                    query += " and (D.VEI_CODIGO IS NULL)";
            }

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                query += " and V.VSE_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosSegmento) + ")";

            if (filtrosPesquisa.CodigosEquipamento != null && filtrosPesquisa.CodigosEquipamento.Count > 0)
                query += " and D.EQP_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosEquipamento) + ")";

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                query += " AND T.TIM_CODIGO  = " + filtrosPesquisa.CodigoTipoMovimento.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao.Count > 0)
                query += " AND N.NAT_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosNaturezaOperacao) + ")";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND D.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND D.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjCpfFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfFornecedor.ToString();

            if (filtrosPesquisa.CentrosResultado.Count > 0)
                query += $" AND D.TDE_CODIGO in (select TDE_CODIGO from T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO where CRE_CODIGO in ({string.Join(",", filtrosPesquisa.CentrosResultado)}))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Produtos.Count > 0)
                query += " AND I.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.Produtos) + ")";

            if (filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento != null && filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento == true)
                query += " AND (V.VEI_CODIGO is not null or E.EQP_CODIGO is not null)";

            if (filtrosPesquisa?.SituacaoLancDocEntrada > 0)
                query += $" AND (D.TDE_SITUACAO = {filtrosPesquisa?.SituacaoLancDocEntrada.GetHashCode()})";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo> RelatorioDespesaVeiculo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, int codigoProduto, int codigoGrupoProduto, int codigoGrupoProdutoPai, double codigoFornecedor, List<int> codigosNaturezaOperacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = @"   SELECT I.TDE_CODIGO CodigoNota,
                TDE_NUMERO_LONG NumeroNota,
                TDE_DATA_EMISSAO DataEmissao,
                CLI_NOME Fornecedor,
                NAT_DESCRICAO NaturezaOperacao,
                PRO_DESCRICAO Produto,
                GRP_DESCRICAO GrupoProduto,
                TDI_QUANTIDADE Quantidade,
                TDI_VALOR_UNITARIO ValorUnitario,
                TDI_VALOR_TOTAL ValorTotal,
                TDI_VALOR_CUSTO_UNITARIO CustoUnitario,
                TDI_VALOR_CUSTO_TOTAL CustoTotal,
                VEI_PLACA Veiculo,
                TDI_KM_ABASTECIMENTO KM 
                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA D ON D.TDE_CODIGO = I.TDE_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                JOIN T_VEICULO V ON V.VEI_CODIGO = I.VEI_CODIGO
                JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
                WHERE 1 = 1";

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (codigosNaturezaOperacao != null && codigosNaturezaOperacao.Count > 0)
            {
                IEnumerable<int> codigosNaturezaOperacaoValidos = codigosNaturezaOperacao.Where(o => o > 0);

                if (codigosNaturezaOperacaoValidos.Count() > 0)
                    query += " AND (N.NAT_CODIGO IN (" + string.Join(",", codigosNaturezaOperacaoValidos) + "))";
            }

            if (codigoFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + codigoFornecedor.ToString();

            if (codigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoVeiculo > 0)
                query += " AND V.VEI_CODIGO = " + codigoVeiculo.ToString();

            if (codigoGrupoProduto > 0)
                query += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            if (codigoGrupoProdutoPai > 0)
                query += " AND G.GPR_CODIGO_PAI = " + codigoGrupoProdutoPai.ToString();

            if (codigoEmpresa > 0)
                query += " AND D.EMP_CODIGO  = " + codigoEmpresa.ToString();

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo>> RelatorioDespesaVeiculoAsync(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, int codigoProduto, int codigoGrupoProduto, int codigoGrupoProdutoPai, double codigoFornecedor, List<int> codigosNaturezaOperacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = @"   SELECT I.TDE_CODIGO CodigoNota,
                TDE_NUMERO_LONG NumeroNota,
                TDE_DATA_EMISSAO DataEmissao,
                CLI_NOME Fornecedor,
                NAT_DESCRICAO NaturezaOperacao,
                PRO_DESCRICAO Produto,
                GRP_DESCRICAO GrupoProduto,
                TDI_QUANTIDADE Quantidade,
                TDI_VALOR_UNITARIO ValorUnitario,
                TDI_VALOR_TOTAL ValorTotal,
                TDI_VALOR_CUSTO_UNITARIO CustoUnitario,
                TDI_VALOR_CUSTO_TOTAL CustoTotal,
                VEI_PLACA Veiculo,
                TDI_KM_ABASTECIMENTO KM 
                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA D ON D.TDE_CODIGO = I.TDE_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                JOIN T_VEICULO V ON V.VEI_CODIGO = I.VEI_CODIGO
                JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
                WHERE 1 = 1";

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (codigosNaturezaOperacao != null && codigosNaturezaOperacao.Count > 0)
            {
                IEnumerable<int> codigosNaturezaOperacaoValidos = codigosNaturezaOperacao.Where(o => o > 0);

                if (codigosNaturezaOperacaoValidos.Count() > 0)
                    query += " AND (N.NAT_CODIGO IN (" + string.Join(",", codigosNaturezaOperacaoValidos) + "))";
            }

            if (codigoFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + codigoFornecedor.ToString();

            if (codigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoVeiculo > 0)
                query += " AND V.VEI_CODIGO = " + codigoVeiculo.ToString();

            if (codigoGrupoProduto > 0)
                query += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            if (codigoGrupoProdutoPai > 0)
                query += " AND G.GPR_CODIGO_PAI = " + codigoGrupoProdutoPai.ToString();

            if (codigoEmpresa > 0)
                query += " AND D.EMP_CODIGO  = " + codigoEmpresa.ToString();

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo)));

            return await nhQuery.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo>();
        }


        public int ContarRelatorioDespesaVeiculo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, int codigoProduto, int codigoGrupoProduto, int codigoGrupoProdutoPai, double codigoFornecedor, List<int> codigosNaturezaOperacao)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR
                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM I
                JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA D ON D.TDE_CODIGO = I.TDE_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                JOIN T_VEICULO V ON V.VEI_CODIGO = I.VEI_CODIGO
                JOIN T_NATUREZAOPERACAO N ON N.NAT_CODIGO = I.NAT_CODIGO
                WHERE 1 = 1";

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                query += " AND D.TDE_DATA_EMISSAO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (codigosNaturezaOperacao != null && codigosNaturezaOperacao.Count > 0)
            {
                IEnumerable<int> codigosNaturezaOperacaoValidos = codigosNaturezaOperacao.Where(o => o > 0);

                if (codigosNaturezaOperacaoValidos.Count() > 0)
                    query += " AND (N.NAT_CODIGO IN (" + string.Join(",", codigosNaturezaOperacaoValidos) + "))";
            }

            if (codigoFornecedor > 0)
                query += " AND C.CLI_CGCCPF = " + codigoFornecedor.ToString();

            if (codigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoVeiculo > 0)
                query += " AND V.VEI_CODIGO = " + codigoVeiculo.ToString();

            if (codigoGrupoProduto > 0)
                query += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            if (codigoGrupoProdutoPai > 0)
                query += " AND G.GPR_CODIGO_PAI = " + codigoGrupoProdutoPai.ToString();

            if (codigoEmpresa > 0)
                query += " AND D.EMP_CODIGO  = " + codigoEmpresa.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            if (filtrosPesquisa.ModelosDocumento != null && filtrosPesquisa.ModelosDocumento.Count > 0)
            {
                IEnumerable<int> modelosDocumentoValidos = filtrosPesquisa.ModelosDocumento.Where(o => o > 0);

                if (modelosDocumentoValidos.Count() > 0)
                    query = query.Where(obj => modelosDocumentoValidos.Contains(obj.Modelo.Codigo));
            }

            if (filtrosPesquisa.TipoDocumentoEmEBS > 0)
            {
                if (filtrosPesquisa.TipoDocumentoEmEBS == 1)//nunca gerado
                    query = query.Where(obj => obj.DocumentoEmEBS == false || obj.DocumentoEmEBS == null);
                else if (filtrosPesquisa.TipoDocumentoEmEBS == 2)//gerado em algum arquivo
                    query = query.Where(obj => obj.DocumentoEmEBS == true);
            }

            if (filtrosPesquisa.DataEntradaInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEntrada.Date >= filtrosPesquisa.DataEntradaInicial.Date);

            if (filtrosPesquisa.DataEntradaFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEntrada.Date <= filtrosPesquisa.DataEntradaFinal.Date);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date <= filtrosPesquisa.DataEmissaoFinal.Date);

            if (filtrosPesquisa.CodigoDestinatario > 0)
                query = query.Where(o => o.Destinatario.Codigo == filtrosPesquisa.CodigoDestinatario);

            if (filtrosPesquisa.ValorInicial > 0m)
                query = query.Where(o => o.ValorTotal >= filtrosPesquisa.ValorInicial);

            if (filtrosPesquisa.ValorFinal > 0m)
                query = query.Where(o => o.ValorTotal <= filtrosPesquisa.ValorFinal);

            if (filtrosPesquisa.CpfCnpjFornecedor > 0d)
                query = query.Where(o => o.Fornecedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjFornecedor);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.NumeroLancamentoInicial > 0)
                query = query.Where(o => o.NumeroLancamento >= filtrosPesquisa.NumeroLancamentoInicial);

            if (filtrosPesquisa.NumeroLancamentoFinal > 0)
                query = query.Where(o => o.NumeroLancamento <= filtrosPesquisa.NumeroLancamentoFinal);

            if (filtrosPesquisa.NumeroDocumentoInicial > 0)
                query = query.Where(o => o.Numero >= filtrosPesquisa.NumeroDocumentoInicial);

            if (filtrosPesquisa.NumeroDocumentoFinal > 0)
                query = query.Where(o => o.Numero <= filtrosPesquisa.NumeroDocumentoFinal);

            if (filtrosPesquisa.CodigoCFOP > 0)
                query = query.Where(o => o.CFOP.Codigo == filtrosPesquisa.CodigoCFOP);

            if (filtrosPesquisa.CodigoNaturezaOperacao > 0)
                query = query.Where(o => o.NaturezaOperacao.Codigo == filtrosPesquisa.CodigoNaturezaOperacao);

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                query = query.Where(o => o.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.NumeroFogo > 0)
                query = query.Where(o => o.Itens.Any(i => i.NumeroFogoInicial == filtrosPesquisa.NumeroFogo));

            if (filtrosPesquisa.NumeroTitulo > 0)
            {
                var queryTitulo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
                var resultTitulo = from obj in queryTitulo where obj.Codigo == filtrosPesquisa.NumeroTitulo select obj;
                var resultDocumentos = resultTitulo.Select(obj => obj.DuplicataDocumentoEntrada);

                query = query.Where(obj => resultDocumentos.Select(a => a.DocumentoEntrada).Contains(obj));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                query = query.Where(o => o.Chave == filtrosPesquisa.Chave);

            if (filtrosPesquisa.StatusFinanceiro.HasValue)
            {
                if (filtrosPesquisa.StatusFinanceiro == StatusFinanceiroDocumentoEntrada.Pago)
                    query = query.Where(o => (o.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.AVista && o.Situacao == SituacaoDocumentoEntrada.Finalizado) || o.StatusFinanceiro.Contains("Pago"));
                else if (filtrosPesquisa.StatusFinanceiro == StatusFinanceiroDocumentoEntrada.ContratoFinanciamento)
                    query = query.Where(o => o.Situacao == SituacaoDocumentoEntrada.Finalizado && o.ContratoFinanciamento != null);
                else if (filtrosPesquisa.StatusFinanceiro == StatusFinanceiroDocumentoEntrada.Aberto)
                    query = query.Where(o => o.StatusFinanceiro.Contains("Aberto") && !(o.Situacao == SituacaoDocumentoEntrada.Finalizado && o.ContratoFinanciamento != null) && !(o.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.AVista && o.Situacao == SituacaoDocumentoEntrada.Finalizado));
                else if (filtrosPesquisa.StatusFinanceiro == StatusFinanceiroDocumentoEntrada.Renegociado)
                    query = query.Where(o => o.StatusFinanceiro.Contains("Renegociado") && !(o.Situacao == SituacaoDocumentoEntrada.Finalizado && o.ContratoFinanciamento != null) && !(o.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.AVista && o.Situacao == SituacaoDocumentoEntrada.Finalizado));
            }

            if (filtrosPesquisa.CodigoProduto > 0)
                query = query.Where(o => o.Itens.Any(i => i.Produto.Codigo == filtrosPesquisa.CodigoProduto));

            if (filtrosPesquisa.CodigoCategoria > 0)
                query = query.Where(o => o.Fornecedor.Categoria.Codigo == filtrosPesquisa.CodigoCategoria);

            if (filtrosPesquisa.CodigoStatusLancamento > 0)
                query = query.Where(o => o.SituacaoLancamentoDocumentoEntrada.Codigo == filtrosPesquisa.CodigoStatusLancamento);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> ConsultarDocumentoReferencia(SituacaoDocumentoSPEDFiscal situacaoDocumentoSPEDFiscal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataEntradaInicial, DateTime dataEntradaFinal, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, int codigoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, int numeroInicial, int numeroFinal, string serie, string chave, decimal valorTotal, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();

            var result = from obj in query select obj;

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(obj => obj.Serie == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(obj => obj.Chave.Contains(chave));

            if (valorTotal > 0)
                result = result.Where(obj => obj.ValorTotal == valorTotal);

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (acerto != null)
                result = result.Where(obj => obj.DataEmissao <= acerto.DataFinal && obj.Situacao == SituacaoDocumentoEntrada.Finalizado);

            if (codigoEmpresa > 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                result = result.Where(obj => obj.Destinatario.Codigo.Equals(codigoEmpresa));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Date >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Date <= dataEmissaoFinal.Date);

            if (dataEntradaInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada.Date >= dataEntradaInicial.Date);

            if (dataEntradaFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada.Date <= dataEntradaFinal.Date);

            if (situacaoDocumentoSPEDFiscal == SituacaoDocumentoSPEDFiscal.SomenteDocumentosGerados)
                result = result.Where(o => o.DocumentoEmEBS.Value == true);
            else if (situacaoDocumentoSPEDFiscal == SituacaoDocumentoSPEDFiscal.SomenteDocumentosNaoGerados)
                result = result.Where(o => o.DocumentoEmEBS.Value == false);

            return result;
        }

        #endregion
    }
}

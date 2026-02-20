using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Dominio.Relatorios.Embarcador.DataSource.Financeiros;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoFaturamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>
    {
        #region Construtores

        public DocumentoFaturamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public DocumentoFaturamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPendentesIntegracaoEmbarcador(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => obj.PendenteIntegracaoEmbarcador && obj.Pagamento.Situacao == SituacaoPagamento.Finalizado && obj.CTe.Status == "A");

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query
                .Fetch(obj => obj.CargaPagamento)
                .Fetch(obj => obj.CargaOcorrenciaPagamento)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.EmpresaSerie)
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.FechamentoFrete)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .ToList();
        }

        public bool CargaContemDocumentoFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => obj.CargaPagamento.Codigo == codigoCarga);

            return query.Any();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.RetornoDocumentoPagamentoStage> BuscarDadosStagePorCte(List<int> cte)
        {
            if (cte.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.RetornoDocumentoPagamentoStage>();

            var sql = $@"select   _Stage.sta_codigo as CodigoStage,_provisao.PRV_SITUACAO as SituacaoProvisao,  CargaCTe.CON_CODIGO CodigoDocumento, _Stage.sta_cancelado as StageCancelada,_Stage.STA_NUMERO_FOLHA as NumeroFolha,_documentoProvisao.PRV_CODIGO as CodigoProvisao
                    from T_CARGA_CTE CargaCTe 
                    join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                    join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                    join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                    join T_DOCUMENTO_PROVISAO _documentoProvisao on _documentoProvisao.STA_cODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO 
                    join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO 
                    join T_PROVISAO _provisao on _provisao.PRV_CODIGO = _documentoProvisao.PRV_CODIGO
                    where CargaCTe.CON_CODIGO IN  ({string.Join(", ", cte)})
                    group by _Stage.sta_codigo,_Stage.sta_cancelado,_Stage.STA_NUMERO_FOLHA,_documentoProvisao.PRV_CODIGO,CargaCTe.CON_CODIGO,_provisao.PRV_SITUACAO";


            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.RetornoDocumentoPagamentoStage)));

            return consulta.SetTimeout(7000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.RetornoDocumentoPagamentoStage>();
        }

        public List<int> BuscarDocumentoFaturamentoCargaReentregue(int codigoCarga)
        {
            string sql = @"SELECT DISTINCT DFA.DFA_CODIGO
            FROM T_DOCUMENTO_FATURAMENTO DFA
            INNER JOIN T_CTE CTE 
	            ON CTE.CON_CODIGO = DFA.CON_CODIGO
            INNER JOIN T_CARGA_CTE CARGACTE 
                ON CTE.CON_CODIGO = CARGACTE.CON_CODIGO 
                AND CARGACTE.CAR_CODIGO < :codigoCarga
                AND CARGACTE.CCC_CODIGO IS NULL
	            AND CARGACTE.CCT_CODIGO_SUB_CONTRATACAO_FILIAL_EMISSORA IS NULL
            INNER JOIN T_CTE_XML_NOTAS_FISCAIS CTEXMLNOTAFISCAL 
                ON CTEXMLNOTAFISCAL.CON_CODIGO = CTE.CON_CODIGO
            WHERE DFA.DFA_SITUACAO IN (1, 4, 5)
            AND CTE.CON_RECEBEDOR_CTE IS NULL
            AND EXISTS (
                SELECT 1
                FROM T_PEDIDO_XML_NOTA_FISCAL NOTA
                WHERE NOTA.NFX_CODIGO = CTEXMLNOTAFISCAL.NFX_CODIGO
                AND EXISTS (
                    SELECT 1
                    FROM T_CARGA_PEDIDO CP
                    WHERE CP.CAR_CODIGO = :codigoCarga AND NOTA.CPE_CODIGO = CP.CPE_CODIGO
                )
            )";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetInt32("codigoCarga", codigoCarga);

            return query.List<int>().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> CargaDocumentoFaturamento(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(obj => codigosCarga.Contains(obj.CargaPagamento.Codigo));
            return query.Select(obj => new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento
            {
                CargaPagamento = new Dominio.Entidades.Embarcador.Cargas.Carga
                {
                    Codigo = obj.CargaPagamento.Codigo
                },
                Codigo = obj.Codigo
            }).ToList();

        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ContemCTeSemFaturamento(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(obj => codigosCarga.Contains(obj.CargaPagamento.Codigo) && (obj.Titulo == null || obj.Titulo.StatusTitulo == StatusTitulo.Cancelado) && (obj.Fatura == null || obj.Fatura.Situacao == SituacaoFatura.Cancelado));
            return query.Select(obj => new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento
            {
                CargaPagamento = new Dominio.Entidades.Embarcador.Cargas.Carga
                {
                    Codigo = obj.CargaPagamento.Codigo
                },
                Codigo = obj.Codigo
            }).ToList();

        }

        public bool ContemCTeSemFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(obj => obj.CargaPagamento.Codigo == codigoCarga && (obj.Titulo == null || obj.Titulo.StatusTitulo == StatusTitulo.Cancelado) && (obj.Fatura == null || obj.Fatura.Situacao == SituacaoFatura.Cancelado));
            return query.Any();
        }

        public bool ContemDocumentoPendenteFaturamento(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => obj.CTe.Chave == chaveCTe && obj.Situacao == SituacaoDocumentoFaturamento.Autorizado);

            return query.Any();
        }

        public int ContarPendentesIntegracaoEmbarcador()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => obj.PendenteIntegracaoEmbarcador);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> Consultar(List<int> pagamentos, int cancelamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa, bool bloquearEnvioAutomatico, bool somentePagamentoLiberado, bool retornarSomenteDocumentosDesbloqueados, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, IList<int> codigosDocumentosComCanhotoDigitalizado)
        {
            var result = Consultar(pagamentos, cancelamento, filtroPesquisa, bloquearEnvioAutomatico, somentePagamentoLiberado, retornarSomenteDocumentosDesbloqueados, codigosDocumentosComCanhotoDigitalizado);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                result = result.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                result = result.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                result = result.Take(parametrosConsulta.LimiteRegistros);

            return result
                .Fetch(obj => obj.CargaPagamento)
                .Fetch(obj => obj.CargaOcorrenciaPagamento)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.EmpresaSerie)
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.FechamentoFrete)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .ToList();
        }

        public int ContarConsulta(List<int> pagamentos, int cancelamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPequisa, bool bloquearEnvioAutomatico, bool somentePagamentoLiberado, bool retornarSomenteDocumentosDesbloqueados, IList<int> codigosDocumentosComCanhotoDigitalizado)
        {
            var result = Consultar(pagamentos, cancelamento, filtroPequisa, bloquearEnvioAutomatico, somentePagamentoLiberado, retornarSomenteDocumentosDesbloqueados, codigosDocumentosComCanhotoDigitalizado);
            return result.Count();
        }

        public decimal SomarValorPagamentoTotalConsulta(List<int> pagamentos, int cancelamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPequisa, bool bloquearEnvioAutomatico, bool somentePagamentoLiberado, bool retornarSomenteDocumentosDesbloqueados)
        {
            var result = Consultar(pagamentos, cancelamento, filtroPequisa, bloquearEnvioAutomatico, somentePagamentoLiberado, retornarSomenteDocumentosDesbloqueados, null);

            return result.Sum(o => (decimal?)(o.ValorDocumento)) ?? 0m;
        }

        public bool ExisteDocumentoPagoPorCarga(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CargaPagamento.Codigo == codigo &&
                                     o.Situacao == SituacaoDocumentoFaturamento.Liquidado);

            return query.Any();
        }

        public bool ExisteDocumentoPagoPorCargaAgrupada(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CargaPagamento.CargaAgrupamento.Codigo == codigo &&
                                     o.Situacao == SituacaoDocumentoFaturamento.Liquidado);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.Pagamento ExistePagamentoEmFechamento(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual && obj.Situacao == SituacaoDocumentoFaturamento.EmFechamento
                        select obj.Pagamento;

            return resut.FirstOrDefault();
        }

        public bool ExisteDocumentoPagoPorLancamentoNFSManual(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual && obj.Situacao == SituacaoDocumentoFaturamento.Liquidado
                        select obj.Codigo;

            if (resut.Timeout(120).FirstOrDefault() > 0)
                return true;
            else
                return false;
        }

        public bool ExisteDocumentoProvisionadoPorOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CargaOcorrenciaPagamento.Codigo == codigo && obj.Situacao == SituacaoDocumentoFaturamento.Liquidado
                        select obj;

            return resut.Any();
        }

        public List<int> BuscarCodigosPorPagamentoLiberado(int pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.PagamentoLiberacao.Codigo == pagamento
                        select obj;
            return resut.Select(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            var resut = from obj in query
                        where obj.Fatura.Codigo == codigoFatura
                        select obj;
            return resut.Select(obj => obj.Documento).ToList();
        }
        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            var resut = from obj in query
                        where obj.Fatura.Codigo == codigoFatura
                        select obj;
            return resut.Select(obj => obj.Documento.CTe).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarDocumentoCargaEmFechamento(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CargaPagamento.Codigo == carga && obj.Situacao == SituacaoDocumentoFaturamento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarDocumentoCargaAgrupamentoEmFechamento(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CargaPagamento.CargaAgrupamento.Codigo == carga && obj.Situacao == SituacaoDocumentoFaturamento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarDocumentoOcorrenciaEmFechamento(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CargaOcorrenciaPagamento.Codigo == ocorrencia && obj.Situacao == SituacaoDocumentoFaturamento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public decimal ValorTotalPorPagamentoLiberado(int pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.PagamentoLiberacao.Codigo == pagamento
                        select obj;
            return resut.Sum(obj => obj.ValorDocumento);
        }

        public decimal ValorTotalPorPagamento(int pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.Pagamento.Codigo == pagamento
                        select obj;
            return resut.Sum(obj => obj.ValorAFaturar);
        }

        public decimal ValorTotalPorCancelamentoPagamento(int cancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CancelamentoPagamento.Codigo == cancelamentoPagamento
                        select obj;
            return resut.Sum(obj => obj.ValorAFaturar);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada> BuscarCodigosPorPagamentoEmFechamento(int pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(o => o.Pagamento.Codigo == pagamento && o.Situacao == SituacaoDocumentoFaturamento.EmFechamento && !o.MovimentoFinanceiroGerado);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada()
            {
                Codigo = o.Codigo,
                Empresa = o.Empresa.Codigo,
                Filial = o.Filial != null ? o.Filial.Codigo : 0,
                TipoCarga = o.TipoCarga.Codigo,
                TipoOperacao = o.TipoOperacao.Codigo,
                Tomador = o.Tomador.CPF_CNPJ,
                CategoriaTomador = o.Tomador.Categoria.Codigo,
                RotaFrete = o.CargaPagamento.Rota.Codigo,
                GrupoTomador = o.Tomador.GrupoPessoas.Codigo,
                ProvisaoPorNotaFiscal = o.ProvisaoPorNotaFiscal,
                ChaveCTeComplementado = o.CTe.ChaveCTESubComp,
                Remetente = o.CTe.Remetente.Cliente.CPF_CNPJ,
                Origem = o.CTe.LocalidadeInicioPrestacao.Codigo,
                CategoriaRemetente = o.CTe.Remetente.Cliente.Categoria.Codigo,
                Destinatario = o.CTe.Destinatario.Cliente.CPF_CNPJ,
                GrupoDestinatario = o.CTe.Destinatario.Cliente.GrupoPessoas.Codigo,
                CategoriaDestinatario = o.CTe.Destinatario.Cliente.Categoria.Codigo,
                GrupoRemetente = o.CTe.Remetente.Cliente.GrupoPessoas.Codigo,
                CodigoCTe = o.CTe.Codigo,
                TipoDocumentoEmissao = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                ModeloDocumentoFiscal = o.CTe.ModeloDocumentoFiscal.Codigo,
                TipoCTe = o.CTe.TipoCTE,
                DataEmissao = o.DataEmissao,
                CargaPagamento = o.CargaPagamento.Codigo,
                TipoOcorrencia = o.CargaOcorrenciaPagamento.TipoOcorrencia.Codigo,
                LancamentoNFSManual = o.LancamentoNFSManual.Codigo,
                OcorrenciaPagamento = o.CargaOcorrenciaPagamento.Codigo,
                Fechamento = o.FechamentoFrete.Codigo,
                Expedidor = o.CTe.Expedidor.Cliente.CPF_CNPJ,
                Recebedor = o.CTe.Recebedor.Cliente.CPF_CNPJ
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada> BuscarCodigosPorPagamentoEmCancelamento(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(o => o.CancelamentoPagamento.Codigo == codigoCancelamentoProvisao && o.Situacao == SituacaoDocumentoFaturamento.EmCancelamento && !o.MovimentoFinanceiroGerado);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoPagamentoSumarizada()
            {
                Codigo = o.Codigo,
                Empresa = o.Empresa.Codigo,
                TipoOperacao = o.TipoOperacao.Codigo,
                Tomador = o.Tomador.CPF_CNPJ,
                GrupoTomador = o.Tomador.GrupoPessoas.Codigo,
                Remetente = o.Remetente.CPF_CNPJ,
                Destinatario = o.Destinatario.CPF_CNPJ,
                GrupoDestinatario = o.Destinatario.GrupoPessoas.Codigo,
                GrupoRemetente = o.Remetente.GrupoPessoas.Codigo,
                CodigoCTe = o.CTe.Codigo,
                TipoDocumentoEmissao = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                DataEmissao = o.DataEmissao,
                CargaPagamento = o.CargaPagamento.Codigo,
                OcorrenciaPagamento = o.CargaOcorrenciaPagamento.Codigo
            }).ToList();
        }

        public void SetarDocumentoMovimentoGeradoCancelamento(int documento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set  documentoFaturamento.Situacao= :Situacao, documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.Codigo = :Codigo ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Codigo", documento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado);
            query.SetBoolean("MovimentoFinanceiroGerado", true);
            query.ExecuteUpdate();
        }

        public void LiberarPagamentosPorCTes(List<int> ctes, DateTime dataLiberacacao)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set  documentoFaturamento.PagamentoDocumentoBloqueado= :PagamentoDocumentoBloqueado, documentoFaturamento.DataLiberacaoPagamento= :DataLiberacaoPagamento where documentoFaturamento.CTe in (:ctes) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("PagamentoDocumentoBloqueado", false);
            query.SetDateTime("DataLiberacaoPagamento", dataLiberacacao);

            query.SetParameterList("ctes", ctes);
            query.ExecuteUpdate();
        }

        public void LiberarPagamentosPorDFAs(List<int> dfas, DateTime dataLiberacacao)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set  documentoFaturamento.PagamentoDocumentoBloqueado= :PagamentoDocumentoBloqueado, documentoFaturamento.DataLiberacaoPagamento= :DataLiberacaoPagamento where documentoFaturamento.Codigo in (:dfas) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("PagamentoDocumentoBloqueado", false);
            query.SetDateTime("DataLiberacaoPagamento", dataLiberacacao);

            query.SetParameterList("dfas", dfas);
            query.ExecuteUpdate();
        }

        public void SetarDocumentoMovimentoGeradoPagamento(int documento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.Codigo = :Codigo ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Codigo", documento);
            query.SetBoolean("MovimentoFinanceiroGerado", true);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosLiberadosPagamento(int pagamento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.BloquearEnvioAutomatico = :BloquearEnvioAutomatico, documentoFaturamento.Pagamento = null, documentoFaturamento.Situacao= :Situacao, documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.Pagamento= :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.SetBoolean("BloquearEnvioAutomatico", true);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosLiberadosPagamentoLiberado(int pagamento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.PagamentoLiberacao = null where documentoFaturamento.PagamentoLiberacao= :PagamentoLiberacao ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("PagamentoLiberacao", pagamento);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosLiberadosCancelamentoPagamento(int codigo)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.CancelamentoPagamento = null, documentoFaturamento.Situacao= :Situacao, documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.CancelamentoPagamento= :CancelamentoPagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CancelamentoPagamento", codigo);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Liquidado);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosGerarMovimentoPagamento(int pagamento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.Pagamento= :Pagamento ";

            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosGerarMovimentoCancelamentoPagamento(int codigo)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where documentoFaturamento.CancelamentoPagamento= :CancelamentoPagamento ";

            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CancelamentoPagamento", codigo);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.ExecuteUpdate();
        }

        public void ConfirmarPagamentoDocumentos(int pagamento)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.PendenteIntegracaoEmbarcador = :PendenteIntegracaoEmbarcador, documentoFaturamento.Situacao= :Situacao where documentoFaturamento.Pagamento= :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetBoolean("PendenteIntegracaoEmbarcador", true);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Liquidado);
            query.ExecuteUpdate();
        }

        public void ConfirmarCancelamentoPagamentoDocumentos(int codigo)
        {
            string hql = "update DocumentoFaturamento documentoFaturamento set documentoFaturamento.Situacao= :Situacao where documentoFaturamento.CancelamentoPagamento= :CancelamentoPagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CancelamentoPagamento", codigo);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado);
            query.ExecuteUpdate();
        }

        public int SetarDocumentosParaCancelamentoPagamento(int cancelamento, int carga, int cargaOcorrencia, DateTime dataInicio, DateTime dataFim, double tomador, int filial, int empresa, List<int> codigosPagamentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento situacao, List<int> codigosNaoSelecionadas)
        {
            string hql =
@"UPDATE 
    DocumentoFaturamento documentoFaturamento 
SET 
    documentoFaturamento.Situacao = :Situacao, 
    documentoFaturamento.CancelamentoPagamento = :CancelamentoPagamento,
    documentoFaturamento.MovimentoFinanceiroGerado = :MovimentoFinanceiroGerado
WHERE 
    documentoFaturamento.Situacao = :SituacaoAtual 
    and TipoLiquidacao = :TipoLiquidacao
    and documentoFaturamento.Pagamento.Codigo IN (
        SELECT pagamento.Codigo FROM Pagamento pagamento WHERE pagamento.Situacao = :SituacaoPagamento
    )
";

            if (dataInicio != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao >= :DataInicio ";
            if (dataFim != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao < :DataFim ";
            if (tomador > 0)
                hql += "AND documentoFaturamento.Tomador = :Tomador ";
            if (filial > 0)
                hql += "AND documentoFaturamento.Filial = :Filial ";
            if (empresa > 0)
                hql += "AND documentoFaturamento.Empresa = :Empresa ";
            if (carga > 0)
                hql += "AND documentoFaturamento.CargaPagamento = :CargaPagamento ";
            if (cargaOcorrencia > 0)
                hql += "AND documentoFaturamento.CargaOcorrenciaPagamento = :CargaOcorrenciaPagamento ";
            if (codigosPagamentos.Count() > 0)
                hql += "AND documentoFaturamento.Pagamento in (:CodigosPagamentos)";
            if (codigosNaoSelecionadas.Count > 0)
                hql += "AND documentoFaturamento.Codigo not in (:CodigosFora) ";

            var query = this.SessionNHiBernate.CreateQuery(hql);


            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Liquidado);
            query.SetEnum("TipoLiquidacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador);
            query.SetEnum("SituacaoPagamento", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado);
            query.SetEnum("Situacao", situacao);
            query.SetInt32("CancelamentoPagamento", cancelamento);
            if (dataInicio != DateTime.MinValue)
                query.SetDateTime("DataInicio", dataInicio.Date);
            if (dataFim != DateTime.MinValue)
                query.SetDateTime("DataFim", dataFim.AddDays(1).Date);
            if (empresa > 0)
                query.SetInt32("Empresa", empresa);
            if (tomador > 0)
                query.SetDouble("Tomador", tomador);
            if (filial > 0)
                query.SetInt32("Filial", filial);
            if (codigosPagamentos.Count() > 0)
                query.SetParameterList("CodigosPagamentos", codigosPagamentos);
            if (carga > 0)
                query.SetInt32("CargaPagamento", carga);
            if (cargaOcorrencia > 0)
                query.SetInt32("CargaOcorrenciaPagamento", cargaOcorrencia);
            if (codigosNaoSelecionadas.Count > 0)
                query.SetParameterList("CodigosFora", codigosNaoSelecionadas);


            return query.ExecuteUpdate();
        }

        public int SetarDocumentosParaPagamentoLiberado(int pagamento, FiltroPesquisaDocumento filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento situacao, List<int> codigosNaoSelecionadas)
        {
            string hql = @" UPDATE 
                                DocumentoFaturamento documentoFaturamento 
                            SET 
                                documentoFaturamento.PagamentoLiberacao = :Pagamento 
                            WHERE 
                                documentoFaturamento.Situacao = :SituacaoAtual 
                                and TipoLiquidacao = :TipoLiquidacao ";

            if (filtroPesquisaDocumento.DataInicio != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataLiberacaoPagamento >= :DataInicio ";
            if (filtroPesquisaDocumento.DataFim != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataLiberacaoPagamento < :DataFim ";
            if (filtroPesquisaDocumento.DataInicialEmissao != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao >= :DataInicioEmissao ";
            if (filtroPesquisaDocumento.DataFinalEmissao != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao < :DataFimEmissao ";
            if (filtroPesquisaDocumento.CodigosTomador?.Count == 1)
                hql += "AND documentoFaturamento.Tomador = :Tomador ";
            if (filtroPesquisaDocumento.CodigosFilial?.Count == 1)
                hql += "AND documentoFaturamento.Filial = :Filial ";
            if (filtroPesquisaDocumento.CodigosTransportador?.Count == 1)
                hql += "AND documentoFaturamento.Empresa = :Empresa ";
            if (filtroPesquisaDocumento.CodigoCarga > 0)
                hql += "AND documentoFaturamento.CargaPagamento = :CargaPagamento ";
            if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                hql += "AND documentoFaturamento.CargaOcorrenciaPagamento = :CargaOcorrenciaPagamento ";
            if (codigosNaoSelecionadas.Count > 0)
                hql += "AND documentoFaturamento.Codigo not in (:CodigosFora) ";

            hql += "AND documentoFaturamento.PagamentoDocumentoBloqueado = :PagamentoDocumentoBloqueado AND documentoFaturamento.Pagamento is not null and documentoFaturamento.PagamentoLiberacao is null ";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Liquidado);
            query.SetEnum("TipoLiquidacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador);

            query.SetInt32("Pagamento", pagamento);

            if (filtroPesquisaDocumento.DataInicio != DateTime.MinValue)
                query.SetDateTime("DataInicio", filtroPesquisaDocumento.DataInicio.Date);
            if (filtroPesquisaDocumento.DataFim != DateTime.MinValue)
                query.SetDateTime("DataFim", filtroPesquisaDocumento.DataFim.AddDays(1).Date);
            if (filtroPesquisaDocumento.DataInicialEmissao != DateTime.MinValue)
                query.SetDateTime("DataInicioEmissao", filtroPesquisaDocumento.DataInicialEmissao.Date);
            if (filtroPesquisaDocumento.DataFinalEmissao != DateTime.MinValue)
                query.SetDateTime("DataFimEmissao", filtroPesquisaDocumento.DataFinalEmissao.AddDays(1).Date);
            if (filtroPesquisaDocumento.CodigosTransportador?.Count == 1)
                query.SetInt32("Empresa", filtroPesquisaDocumento.CodigosTransportador.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigosTomador?.Count == 1)
                query.SetDouble("Tomador", filtroPesquisaDocumento.CodigosTomador.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigosFilial?.Count == 1)
                query.SetInt32("Filial", filtroPesquisaDocumento.CodigosFilial.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigoCarga > 0)
                query.SetInt32("CargaPagamento", filtroPesquisaDocumento.CodigoCarga);
            if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                query.SetInt32("CargaOcorrenciaPagamento", filtroPesquisaDocumento.CodigoOcorrencia);

            query.SetBoolean("PagamentoDocumentoBloqueado", false);

            if (codigosNaoSelecionadas.Count > 0)
                query.SetParameterList("CodigosFora", codigosNaoSelecionadas);

            return query.ExecuteUpdate();
        }

        public IList<int> ObterCodigosDocumentosParaPagamento(FiltroPesquisaDocumento filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento situacao, List<int> codigosNaoSelecionadas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoGerado? tipoDocumentoGerado)
        {
            string hql = @"SELECT documentoFaturamento.Codigo
                             FROM DocumentoFaturamento documentoFaturamento
                            WHERE 0 = 0 ";

            hql += ObterFiltrosDocumentosParaPagamento(filtroPesquisaDocumento, situacao, codigosNaoSelecionadas, tipoDocumentoGerado);

            NHibernate.IQuery query = ObterParametrosDocumentosParaPagamento(hql, filtroPesquisaDocumento, codigosNaoSelecionadas);

            return query.List<int>();
        }


        public int SetarDocumentosParaPagamento(int pagamento, FiltroPesquisaDocumento filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento situacao, List<int> codigosNaoSelecionadas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoGerado? tipoDocumentoGerado)
        {
            string hql = @" UPDATE 
                                DocumentoFaturamento documentoFaturamento 
                            SET 
                                documentoFaturamento.Situacao = :Situacao, 
                                documentoFaturamento.Pagamento = :Pagamento 
                            WHERE 0 = 0 ";

            hql += ObterFiltrosDocumentosParaPagamento(filtroPesquisaDocumento, situacao, codigosNaoSelecionadas, tipoDocumentoGerado);

            NHibernate.IQuery query = ObterParametrosDocumentosParaPagamento(hql, filtroPesquisaDocumento, codigosNaoSelecionadas);
            query.SetEnum("Situacao", situacao);
            query.SetInt32("Pagamento", pagamento);

            return query.ExecuteUpdate();
        }

        private NHibernate.IQuery ObterParametrosDocumentosParaPagamento(string hql, FiltroPesquisaDocumento filtroPesquisaDocumento, List<int> codigosNaoSelecionadas)
        {
            NHibernate.IQuery query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado);
            query.SetEnum("TipoLiquidacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador);

            if (filtroPesquisaDocumento.DataInicio != DateTime.MinValue)
                query.SetDateTime("DataInicio", filtroPesquisaDocumento.DataInicio.Date);
            if (filtroPesquisaDocumento.DataFim != DateTime.MinValue)
                query.SetDateTime("DataFim", filtroPesquisaDocumento.DataFim.AddDays(1).Date);
            if (filtroPesquisaDocumento.DataInicialEmissao != DateTime.MinValue)
                query.SetDateTime("DataInicioEmissao", filtroPesquisaDocumento.DataInicialEmissao.Date);
            if (filtroPesquisaDocumento.DataFinalEmissao != DateTime.MinValue)
                query.SetDateTime("DataFimEmissao", filtroPesquisaDocumento.DataFinalEmissao.AddDays(1).Date);
            if (filtroPesquisaDocumento.CodigosTransportador?.Count == 1)
                query.SetInt32("Empresa", filtroPesquisaDocumento.CodigosTransportador.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigosTomador?.Count == 1)
                query.SetDouble("Tomador", filtroPesquisaDocumento.CodigosTomador.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigosFilial?.Count == 1)
                query.SetInt32("Filial", filtroPesquisaDocumento.CodigosFilial.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigosTiposOperacao?.Count == 1)
                query.SetInt32("TipoOperacao", filtroPesquisaDocumento.CodigosTiposOperacao.FirstOrDefault());
            if (filtroPesquisaDocumento.CodigoCarga > 0)
                query.SetInt32("CargaPagamento", filtroPesquisaDocumento.CodigoCarga);
            if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                query.SetInt32("CargaOcorrenciaPagamento", filtroPesquisaDocumento.CodigoOcorrencia);

            if (codigosNaoSelecionadas.Count > 0)
                query.SetParameterList("CodigosFora", codigosNaoSelecionadas);

            return query;
        }

        private string ObterFiltrosDocumentosParaPagamento(FiltroPesquisaDocumento filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento situacao, List<int> codigosNaoSelecionadas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoGerado? tipoDocumentoGerado)
        {
            bool exigeCargaPagamento = false;
            string hql = @" AND documentoFaturamento.Situacao = :SituacaoAtual
                            AND TipoLiquidacao = :TipoLiquidacao ";

            if (filtroPesquisaDocumento.DataInicio != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataLiberacaoPagamento >= :DataInicio ";
            if (filtroPesquisaDocumento.DataFim != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataLiberacaoPagamento < :DataFim ";
            if (filtroPesquisaDocumento.DataInicialEmissao != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao >= :DataInicioEmissao ";
            if (filtroPesquisaDocumento.DataFinalEmissao != DateTime.MinValue)
                hql += "AND documentoFaturamento.DataEmissao < :DataFimEmissao ";
            if (filtroPesquisaDocumento.CodigosTomador?.Count == 1)
                hql += "AND documentoFaturamento.Tomador = :Tomador ";
            if (filtroPesquisaDocumento.CodigosFilial?.Count == 1)
                hql += "AND documentoFaturamento.Filial = :Filial ";
            if (filtroPesquisaDocumento.CodigosTransportador?.Count == 1)
                hql += "AND documentoFaturamento.Empresa = :Empresa ";
            if (filtroPesquisaDocumento.CodigoCarga > 0)
                hql += "AND documentoFaturamento.CargaPagamento = :CargaPagamento ";
            if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                hql += "AND documentoFaturamento.CargaOcorrenciaPagamento = :CargaOcorrenciaPagamento ";
            if (codigosNaoSelecionadas.Count > 0)
                hql += "AND documentoFaturamento.Codigo not in (:CodigosFora) ";
            if (tipoDocumentoGerado.HasValue)
            {
                if (tipoDocumentoGerado.Value == TipoDocumentoGerado.SomenteCargas)
                {
                    hql += "AND documentoFaturamento.CargaOcorrenciaPagamento = NULL ";
                    exigeCargaPagamento = true;
                }
                else if (tipoDocumentoGerado.Value == TipoDocumentoGerado.SomenteOcorrencias)
                {
                    hql += "AND documentoFaturamento.CargaOcorrenciaPagamento <> NULL ";
                    exigeCargaPagamento = true;
                }
            }
            if (filtroPesquisaDocumento.CodigosTiposOperacao?.Count == 1)
            {
                hql += "AND documentoFaturamento.CargaPagamento in (SELECT carga.Codigo FROM Carga carga WHERE carga.TipoOperacao = :TipoOperacao) ";
                exigeCargaPagamento = true;
            }

            if (exigeCargaPagamento)
                hql += "AND documentoFaturamento.CargaPagamento IS NOT NULL ";

            return hql;
        }

        public List<int> BuscarListaCodigosPagamentoNaoSelecionados(int pagamento, FiltroPesquisaDocumento filtroPesquisaDocumento, List<int> codigosNaoSelecionadas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoGerado? tipoDocumentoGerado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => obj.Pagamento.Codigo == pagamento);

            if (filtroPesquisaDocumento.CodigoCarga > 0)
                query = query.Where(obj => obj.CargaPagamento.Codigo == filtroPesquisaDocumento.CodigoCarga);

            if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                query = query.Where(obj => obj.CargaOcorrenciaPagamento.Codigo == filtroPesquisaDocumento.CodigoOcorrencia);

            if (filtroPesquisaDocumento.DataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataLiberacaoPagamento >= filtroPesquisaDocumento.DataInicio);

            if (filtroPesquisaDocumento.DataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataLiberacaoPagamento < filtroPesquisaDocumento.DataFim);

            if (filtroPesquisaDocumento.DataInicialEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao >= filtroPesquisaDocumento.DataInicialEmissao);

            if (filtroPesquisaDocumento.DataFinalEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao < filtroPesquisaDocumento.DataFinalEmissao);

            if (filtroPesquisaDocumento.CodigosTomador?.Count > 0)
                query = query.Where(obj => obj.Tomador.CPF_CNPJ == filtroPesquisaDocumento.CodigosTomador.FirstOrDefault());

            if (filtroPesquisaDocumento.CodigosFilial?.Count > 0)
                query = query.Where(obj => obj.Filial.Codigo == filtroPesquisaDocumento.CodigosFilial.FirstOrDefault());

            if (filtroPesquisaDocumento.CodigosTransportador?.Count > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtroPesquisaDocumento.CodigosTransportador.FirstOrDefault());

            if (tipoDocumentoGerado.HasValue)
            {
                if (tipoDocumentoGerado.Value == TipoDocumentoGerado.SomenteCargas)
                    query = query.Where(obj => obj.CargaPagamento != null && obj.CargaOcorrenciaPagamento == null);
                else if (tipoDocumentoGerado.Value == TipoDocumentoGerado.SomenteOcorrencias)
                    query = query.Where(obj => obj.CargaPagamento != null && obj.CargaOcorrenciaPagamento != null);
            }

            if (filtroPesquisaDocumento.SituacaoDocumentoPagamento.HasValue)
                if (filtroPesquisaDocumento.SituacaoDocumentoPagamento.Value == SituacaoDocumentoPagamento.Bloqueado)
                    query = query.Where(o => o.PagamentoDocumentoBloqueado);
                else
                    query = query.Where(o => !o.PagamentoDocumentoBloqueado);

            if (codigosNaoSelecionadas.Count > 0)
                query = query.Where(obj => !codigosNaoSelecionadas.Contains(obj.Codigo));

            return query.Select(obj => obj.Codigo).ToList();
        }

        public bool PossuiDocumentoBloqueado(List<int> codigosDocumentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(obj => codigosDocumentos.Contains(obj.Codigo) && obj.PagamentoDocumentoBloqueado);

            return query.Any();
        }

        public Task<List<Dominio.Entidades.Cliente>> BuscarTomadoresPagamentoAsync(int pagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberacao.Codigo == pagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == pagamento);

            return query.Select(obj => obj.Tomador).Distinct().Fetch(obj => obj.GrupoPessoas).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresPagamento(int pagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberacao.Codigo == pagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == pagamento);

            return query.Select(obj => obj.Empresa).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarTomadoresCancelamentoPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            var resut = from obj in query
                        where obj.CancelamentoPagamento.Codigo == codigo
                        select obj;

            return resut.Select(obj => obj.Tomador).Distinct().Fetch(obj => obj.GrupoPessoas).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ConsultarDocumentosParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = _ConsultarParaFaturamento(filtros);

            return query.Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeInicioPrestacao).ThenFetch(o => o.Estado)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao).ThenFetch(o => o.Estado)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaDocumentosParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = _ConsultarParaFaturamento(filtros);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ConsultarDocumentosParaFaturar(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, int serie, DateTime dataInicial, DateTime dataFinal, string numeroCarga, int numeroDocumentoInicial, int numeroDocumentoFinal, int codigoGrupoPessoas, double cpfCnpjTomador, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoOrigem, int codigoDestino, string numeroPedido, string numeroOcorrencia, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = ObterConsultaDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataInicial, dataFinal, numeroCarga, numeroDocumentoInicial, numeroDocumentoFinal, codigoGrupoPessoas, cpfCnpjTomador, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia);

            return query.OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaDocumentosParaFaturar(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, int serie, DateTime dataInicial, DateTime dataFinal, string numeroCarga, int numeroDocumentoInicial, int numeroDocumentoFinal, int codigoGrupoPessoas, double cpfCnpjTomador, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoOrigem, int codigoDestino, string numeroPedido, string numeroOcorrencia)
        {
            var query = ObterConsultaDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataInicial, dataFinal, numeroCarga, numeroDocumentoInicial, numeroDocumentoFinal, codigoGrupoPessoas, cpfCnpjTomador, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia);

            return query.Count();
        }

        public List<int> ObterCodigosDocumentosParaFaturar(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, int serie, DateTime dataInicial, DateTime dataFinal, string numeroCarga, int numeroDocumentoInicial, int numeroDocumentoFinal, int codigoGrupoPessoas, double cpfCnpjTomador, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoOrigem, int codigoDestino, string numeroPedido, string numeroOcorrencia, bool selecionarTodos, List<int> codigosDocumentos)
        {
            var query = ObterConsultaDocumentosParaFaturar(codigoEmpresa, codigoVeiculo, codigoMotorista, serie, dataInicial, dataFinal, numeroCarga, numeroDocumentoInicial, numeroDocumentoFinal, codigoGrupoPessoas, cpfCnpjTomador, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, numeroPedido, numeroOcorrencia);

            if (selecionarTodos)
                query = query.Where(o => !codigosDocumentos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosDocumentos.Contains(o.Codigo));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorChaveNFe(string chaveNotaFiscal)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => (o.Situacao == SituacaoDocumentoFaturamento.Autorizado) && (o.TipoLiquidacao == TipoLiquidacao.Fatura) && o.CTe.Documentos.Any(cte => cte.ChaveNFE == chaveNotaFiscal));

            return consultaDocumentoFaturamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorNotaFiscalCarga(int codigoXMLNotaFiscal, int carga)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal) && o.CargaPagamento.Codigo == carga
                  && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento && o.Situacao != SituacaoDocumentoFaturamento.Anulado
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorNotaFiscalCargaAsync(int codigoXMLNotaFiscal, int carga)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal)
                    && o.CargaPagamento.Codigo == carga
                    && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento && o.Situacao != SituacaoDocumentoFaturamento.Anulado
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorNotaFiscal(int codigoXMLNotaFiscal, int empresa)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => (o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador) && o.CTe.Empresa.Codigo == empresa && o.CancelamentoPagamento == null && o.PagamentoDocumentoBloqueado && o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal));

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentoFaturamentoPorNotasFiscaisAsync(List<int> codigosXMLNotaFiscais, int empresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o =>
                    o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador &&
                    o.CTe.Empresa.Codigo == empresa &&
                    o.CancelamentoPagamento == null &&
                    o.PagamentoDocumentoBloqueado &&
                    o.CTe.XMLNotaFiscais.Any(obj => codigosXMLNotaFiscais.Contains(obj.Codigo))
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarDocumentoFaturamentoPagamentoPorCargaNotaFiscal(List<int> codigosXMLNotasFiscais, int codigoCarga)
        {
            IQueryable<int> subqueryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(obj => obj.Carga.Codigo == codigoCarga
                && obj.CTe != null
                && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto
                && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento //CT-es complementares não precisam de desbloqueio
                && obj.CTe.XMLNotaFiscais.Any(xml => codigosXMLNotasFiscais.Contains(xml.Codigo))
                ).Select(obj => obj.CTe.Codigo);

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.Pagamento != null
                    && o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador
                    && o.Situacao != SituacaoDocumentoFaturamento.Cancelado
                    );

            consultaDocumentoFaturamento = consultaDocumentoFaturamento.Where(obj => subqueryCargaCTe.Contains(obj.CTe.Codigo))
            .OrderByDescending(o => o.Pagamento.Codigo);

            return consultaDocumentoFaturamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorDocumentosNotaFiscalETransportador(int codigoXMLNotaFiscal, int empresa)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.Empresa.Codigo == empresa
                && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento && o.Situacao != SituacaoDocumentoFaturamento.Anulado
                && o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal));

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>> BuscarPorDocumentosNotaFiscalETransportadorAsync(int codigoXMLNotaFiscal, int empresa)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.Empresa.Codigo == empresa &&
                            o.Situacao != SituacaoDocumentoFaturamento.Cancelado &&
                            o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento &&
                            o.Situacao != SituacaoDocumentoFaturamento.Anulado &&
                            o.CTe != null &&
                            o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal)
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentoFaturamentoPorNotaFiscalAsync(int codigoXMLNotaFiscal, int empresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador &&
                            o.CTe.Empresa.Codigo == empresa &&
                            o.Situacao != SituacaoDocumentoFaturamento.Cancelado &&
                            o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento &&
                            o.Situacao != SituacaoDocumentoFaturamento.Anulado &&
                            o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal) &&
                            o.PagamentoDocumentoBloqueado &&
                            o.CTe != null
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>> BuscarPorDocumentosNotaFiscalAsync(int codigoXMLNotaFiscal, int empresa)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => (o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador) && o.CTe.Empresa.Codigo == empresa &&
                            o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.EmCancelamento && o.Situacao != SituacaoDocumentoFaturamento.Anulado &&
                            o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoXMLNotaFiscal)
                );

            return consultaDocumentoFaturamento.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorChaveNFe(string[] chavesNotasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Situacao == SituacaoDocumentoFaturamento.Autorizado && o.TipoLiquidacao == TipoLiquidacao.Fatura && o.CTe.Documentos.Any(d => chavesNotasFiscais.Contains(d.ChaveNFE)));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorChaveCTe(string chaveCTe)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => (o.Situacao == SituacaoDocumentoFaturamento.Autorizado) && (o.TipoLiquidacao == TipoLiquidacao.Fatura) && o.CTe.Chave == chaveCTe);

            return consultaDocumentoFaturamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPrimeiroPorChaveCTe(string chaveCTe)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => (o.Situacao == SituacaoDocumentoFaturamento.Autorizado) && o.CTe.Chave == chaveCTe);

            return consultaDocumentoFaturamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPrimeiroQualquerPorChaveCTe(string chaveCTe)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.Chave == chaveCTe);

            return consultaDocumentoFaturamento.FirstOrDefault();
        }

        public IList<int> BuscarCodigosCargaNaoFaturadasAvon()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"select distinct cargaCTe.CAR_CODIGO from t_carga_cte cargaCTe
                                                                inner join t_cte cte on cargaCTe.CON_CODIGO = cte.CON_CODIGO
                                                                inner join t_cte_participante participanteTomador on participanteTomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                                                inner join t_cliente tomador on tomador.CLI_CGCCPF = participanteTomador.CLI_CODIGO
                                                                inner join t_carga carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO
                                                                where cte.con_status = 'A' 
                                                                and car_situacao in (15,11,10,9,8,7)
                                                                and car_carga_transbordo = 0
                                                                and tomador.GRP_CODIGO = 1025
                                                                and cargaCTe.CCC_CODIGO is null
                                                                and cargaCTe.con_codigo not in(select faturaCargaDocumento.CON_CODIGO 
                                                                from t_fatura fatura
                                                                inner join t_fatura_carga_documento faturaCargaDocumento on fatura.FAT_CODIGO = faturaCargaDocumento.FAT_CODIGO
                                                                where 
                                                                fatura.fat_situacao in (2,4) 
                                                                and faturaCargaDocumento.fcd_status_documento = 1)
                                                                and cargaCTe.con_codigo not in (select con_codigo from t_documento_faturamento where con_codigo is not null)
                                                                and cargaCTe.car_codigo not in (select car_codigo from t_documento_faturamento where car_codigo is not null)");

            return query.SetTimeout(300).List<int>();
        }

        public IList<int> BuscarCodigosCargaCTeDeOcorrenciaNaoFaturadosAvon()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"select distinct cargaCTe.CCT_CODIGO from t_carga_cte cargaCTe
                                                                inner join t_cte cte on cargaCTe.CON_CODIGO = cte.CON_CODIGO
                                                                inner join t_cte_participante participanteTomador on participanteTomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                                                inner join t_cliente tomador on tomador.CLI_CGCCPF = participanteTomador.CLI_CODIGO
                                                                inner join t_carga carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO
                                                                where cte.con_status = 'A' 
                                                                and car_situacao in (15,11,10,9,8,7)
                                                                and car_carga_transbordo = 0
                                                                and tomador.GRP_CODIGO = 1025
                                                                and cargaCTe.CCC_CODIGO is not null
                                                                and cargaCTe.con_codigo not in(select faturaCargaDocumento.CON_CODIGO 
                                                                from t_fatura fatura
                                                                inner join t_fatura_carga_documento faturaCargaDocumento on fatura.FAT_CODIGO = faturaCargaDocumento.FAT_CODIGO
                                                                where 
                                                                fatura.fat_situacao in (2,4) 
                                                                and faturaCargaDocumento.fcd_status_documento = 1)
                                                                and cargaCTe.con_codigo not in (select con_codigo from t_documento_faturamento where con_codigo is not null)");

            return query.List<int>();
        }

        public IList<int> BuscarCodigosCargaCTeNaoFaturadosSemAvon()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"select distinct cargaCTe.CCT_CODIGO from t_carga_cte cargaCTe
                                                                inner join t_cte cte on cargaCTe.CON_CODIGO = cte.CON_CODIGO
                                                                inner join t_cte_participante participanteTomador on participanteTomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                                                inner join t_cliente tomador on tomador.CLI_CGCCPF = participanteTomador.CLI_CODIGO
                                                                inner join t_carga carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO
                                                                where cte.con_status = 'A' 
                                                                and car_situacao in (15,11,10,9,8,7,18)
                                                                and car_carga_transbordo = 0
                                                                and (tomador.GRP_CODIGO is null or tomador.GRP_CODIGO <> 1025)
                                                                and cargaCTe.con_codigo not in(select faturaCargaDocumento.CON_CODIGO 
                                                                from t_fatura fatura
                                                                inner join t_fatura_carga_documento faturaCargaDocumento on fatura.FAT_CODIGO = faturaCargaDocumento.FAT_CODIGO
                                                                where 
                                                                fatura.fat_situacao in (2,4) 
                                                                and faturaCargaDocumento.fcd_status_documento = 1)
                                                                and cargaCTe.con_codigo not in (select con_codigo from t_documento_faturamento where con_codigo is not null)");

            return query.List<int>();
        }

        public IList<int> BuscarCodigosCargaCTeNaoFaturadosDanone()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"select distinct cargaCTe.CCT_CODIGO from t_carga_cte cargaCTe
                                                                inner join t_cte cte on cargaCTe.CON_CODIGO = cte.CON_CODIGO
                                                                inner join t_cte_participante participanteTomador on participanteTomador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                                                inner join t_cliente tomador on tomador.CLI_CGCCPF = participanteTomador.CLI_CODIGO
                                                                inner join t_carga carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO
                                                                where cte.con_status = 'A' 
                                                                and car_situacao in (15,11,10,9,8,7,18)
                                                                and car_carga_transbordo = 0
                                                                and cargaCTe.con_codigo not in(select faturaCargaDocumento.CON_CODIGO 
                                                                from t_fatura fatura
                                                                inner join t_fatura_carga_documento faturaCargaDocumento on fatura.FAT_CODIGO = faturaCargaDocumento.FAT_CODIGO
                                                                where 
                                                                fatura.fat_situacao in (2,4) 
                                                                and faturaCargaDocumento.fcd_status_documento = 1)
                                                                and cargaCTe.con_codigo not in (select con_codigo from t_documento_faturamento where con_codigo is not null)
                                                                and carga.TOP_CODIGO in (1001, 1005, 1006, 1008, 1010)");

            return query.List<int>();
        }

        public List<int> ConsultarCodigosConhecimentosPendenteFaturamento(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarConhecimentosPendenteFaturamento(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Cliente)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Serie)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Empresa)
                .Select(o => o.CTe).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarConhecimentosPendenteFaturamento(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Count();
        }

        public List<int> ConsultarCodigosGrupoPessoaParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            query = query.Where(o => o.Tomador.GrupoPessoas != null);

            return query.Select(o => o.Tomador.GrupoPessoas.Codigo).Distinct().ToList();
        }

        public List<double> ConsultarCodigosTomadorParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Select(o => o.Tomador.CPF_CNPJ).Distinct()
                        .ToList();
        }

        public List<string> ConsultarInscricaoTomadorParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Select(o => o.CTe.TomadorPagador.IE_RG).Distinct()
                        .ToList();
        }

        public List<int> ConsultarCodigosMDFeParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            var queryMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            queryMDFe = queryMDFe.Where(obj => obj.MunicipiosDescarregamento.Any(m => m.Documentos.Any(c => query.Any(f => f.CTe == c.CTe))));

            return queryMDFe.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> ConsultarCodigosPedidoNavioViagemParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            query = query.Where(o => o.CTe.Viagem != null);

            return query.Select(o => o.CTe.Viagem.Codigo).Distinct().ToList();
        }

        public List<int> ConsultarCodigosTerminalDestinoParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            query = query.Where(o => o.CTe.TerminalDestino != null);

            return query.Select(o => o.CTe.TerminalDestino.Codigo).Distinct().ToList();
        }

        public List<string> ConsultarBookingsParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            query = query.Where(o => o.CTe.NumeroBooking != "");

            return query.Select(o => o.CTe.NumeroBooking).Distinct().ToList();
        }

        public List<int> ConsultarCTesParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            return query.Select(o => o.CTe.Codigo).Distinct().ToList();
        }

        public List<int> ConsultarCodigosContainerParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            var queryContainerCTE = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            queryContainerCTE = queryContainerCTE.Where(obj => query.Any(f => f.CTe == obj.CTE));

            queryContainerCTE = queryContainerCTE.Where(o => o.Container != null);
            return queryContainerCTE.Select(o => o.Container.Codigo).Distinct().ToList();
        }

        public List<string> ConsultarNumeroControleClienteParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => query.Any(o => o.CTe == obj.CTe));

            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido != null && obj.Pedido.CodigoPedidoCliente != "" && queryCargaCTe.Any(o => o.Carga == obj.Carga));

            return queryCargaPedido.Select(o => o.Pedido.CodigoPedidoCliente).Distinct().ToList();
        }

        public List<string> ConsultarNumeroReferenciaEDIParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            var query = _ConsultarParaFaturamento(filtros);

            var queryDocumentosCTE = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            queryDocumentosCTE = queryDocumentosCTE.Where(obj => obj.NumeroReferenciaEDI != "" && query.Any(f => f.CTe == obj.CTE));

            return queryDocumentosCTE.Select(o => o.NumeroReferenciaEDI).Distinct().ToList();
        }

        public List<int> ConsultarCodigosDocumentosParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros, int inicio, int limite)
        {
            var result = _ConsultarParaFaturamento(filtros);

            return result.Select(o => o.Codigo)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCodigo(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Codigo == codigoDocumento);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorCodigos(IList<int> codigosDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => codigosDocumento.Contains(o.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCodigoComFetch(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Codigo == codigoDocumento);

            return query.Fetch(o => o.Carga).Fetch(o => o.CTe).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarTodosPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>> BuscarTodosPorCTeAsync(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.CTe.Codigo == codigoCTe);

            return query.ToListAsync(CancellationToken);
        }

        public bool ExisteCTeComCancelamentoPagamento(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.CancelamentoPagamento != null);

            return query.Count() > 0;
        }

        public List<(int codigoRegistro, int CodigoCte)> BuscarPorCTeComSemBloqueio(List<int> codigoCtes, string tipobloqueio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => codigoCtes.Contains(o.CTe.Codigo));

            if (!string.IsNullOrEmpty(tipobloqueio))
                query = query.Where(o => o.Bloqueio == tipobloqueio);

            return query.Select(x => ValueTuple.Create(x.Codigo, x.CTe.Codigo)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorPreFaturaNatura(int codigoPreFaturaNatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> queryItemPreFaturaNatura = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            queryItemPreFaturaNatura = queryItemPreFaturaNatura.Where(o => o.PreFatura.Codigo == codigoPreFaturaNatura);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => queryItemPreFaturaNatura.Select(i => i.CargaCTe.CTe.Codigo).Contains(o.CTe.Codigo));

            return queryDocumentoFaturamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorCTeOriginal(int numeroCTe, int numeroSerie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CTe.DocumentosOriginarios.Any(doc => doc.Numero == numeroCTe && int.Parse(doc.Serie) == numeroSerie) && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.Anulado);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarDocumentoAtivoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.Anulado);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao? tipoLiquidacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (tipoLiquidacao.HasValue)
                query = query.Where(o => o.TipoLiquidacao == tipoLiquidacao.Value);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCTe(int codigoCTe)
        {
            return this.BuscarPorCTe(codigoCTe, null, null);
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCTe(int codigoCTe, TipoLiquidacao? tipoLiquidacao, SituacaoDocumentoFaturamento? situacaoDocumentoFaturamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            if (tipoLiquidacao.HasValue)
                query = query.Where(o => o.TipoLiquidacao == tipoLiquidacao.Value);

            if (situacaoDocumentoFaturamento.HasValue)
                query = query.Where(o => o.Situacao == situacaoDocumentoFaturamento.Value);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorTituloBaixaCTe(int codigoTituloBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao? tipoLiquidacao = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> queryTituloBaixaAgrupadoDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            queryTituloBaixaAgrupadoDocumento = queryTituloBaixaAgrupadoDocumento.Where(o => o.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoTituloBaixa);

            if (tipoLiquidacao.HasValue)
                query = query.Where(o => o.TipoLiquidacao == tipoLiquidacao);

            query = query.Where(o => (o.TipoDocumento == TipoDocumentoFaturamento.CTe && queryTituloBaixaAgrupadoDocumento.Any(c => c.TituloDocumento.CTe != null && c.TituloDocumento.CTe == o.CTe)));

            return query.Fetch(o => o.Empresa)
                        .Fetch(o => o.CTe)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorTituloBaixaCarga(int codigoTituloBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao? tipoLiquidacao = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> queryTituloBaixaAgrupadoDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento>();

            queryTituloBaixaAgrupadoDocumento = queryTituloBaixaAgrupadoDocumento.Where(o => o.TituloBaixaAgrupado.TituloBaixa.Codigo == codigoTituloBaixa);

            if (tipoLiquidacao.HasValue)
                query = query.Where(o => o.TipoLiquidacao == tipoLiquidacao);

            query = query.Where(o => (o.TipoDocumento == TipoDocumentoFaturamento.Carga && queryTituloBaixaAgrupadoDocumento.Any(c => c.TituloDocumento.Carga != null && c.TituloDocumento.Carga == o.Carga)));

            return query.Fetch(o => o.Empresa)
                        .Fetch(o => o.Carga)
                        .ToList();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga);

            return query.Any();
        }

        //public bool ExistePorCTe(int codigoCTe)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

        //    query = query.Where(o => o.CTe.Codigo == codigoCTe && o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe);

        //    return query.Any();
        //}

        public bool ExistePorCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga && o.CTe != null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => o.TipoDocumento == TipoDocumentoFaturamento.CTe && queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.CTe));//Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo));

            return queryDocumentoFaturamento.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPorCTeComCodigoCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga && o.CTe != null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => o.TipoDocumento == TipoDocumentoFaturamento.CTe && queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.CTe));//Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo));

            return queryDocumentoFaturamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorEmpresaECTe(string cnpjEmpresa, int numeroCTe, int serieCTe, decimal valor, string modelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Situacao == SituacaoDocumentoFaturamento.Autorizado && o.CTe.Empresa.CNPJ == cnpjEmpresa && o.TipoLiquidacao == TipoLiquidacao.Fatura && o.CTe.Numero == numeroCTe && o.CTe.Serie.Numero == serieCTe);

            if (valor > 0m)
                query = query.Where(o => o.ValorDocumento == valor);

            if (!string.IsNullOrWhiteSpace(modelo))
                query = query.Where(o => o.ModeloDocumentoFiscal.Abreviacao == modelo);

            return query.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento> ConsultarRelatorioDocumentoFaturamento(List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioDocumentoFaturamento(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento)));

            return query.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento>();
        }

        public int ContarConsultaRelatorioDocumentoFaturamento(List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioDocumentoFaturamento(true, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(6000).UniqueResult<int>();
        }

        public IList<PosicaoDocumentoReceber> ConsultarRelatorioPosicaoDocumentoReceber(List<PropriedadeAgrupamento> agrupamentos, DateTime dataPosicao, TipoFaturamentoPosicao? tipoFaturamento, int codigoEmpresa, int codigoGrupoPessoas, int codigoOrigem, int codigoDestino, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjTomador, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioPosicaoDocumentoReceber(false, agrupamentos, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(PosicaoDocumentoReceber)));

            return query.SetTimeout(99999).List<PosicaoDocumentoReceber>();
        }

        public int ContarConsultaRelatorioPosicaoDocumentoReceber(List<PropriedadeAgrupamento> agrupamentos, DateTime dataPosicao, TipoFaturamentoPosicao? tipoFaturamento, int codigoEmpresa, int codigoGrupoPessoas, int codigoOrigem, int codigoDestino, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjTomador, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioPosicaoDocumentoReceber(true, agrupamentos, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0));

            return query.SetTimeout(99999).UniqueResult<int>();
        }

        public decimal ObterValorTotalNaoPagoPorPessoaOuGrupoPessoas(int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => (o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Tomador.CPF_CNPJ == cpfCnpjPessoa) && o.Situacao != SituacaoDocumentoFaturamento.Cancelado && o.Situacao != SituacaoDocumentoFaturamento.Anulado);

            return query.Sum(o => (decimal?)(o.ValorDocumento - o.ValorPago - o.ValorDesconto + o.ValorAcrescimo)) ?? 0m;
        }

        public List<int> BuscarNumeroDocumentoInvalidoParaCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CargaPagamento.Codigo == codigoCarga &&
                                     o.Situacao == SituacaoDocumentoFaturamento.EmFechamento);

            return query.Select(o => o.Pagamento.Numero).ToList();
        }

        public List<int> BuscarNumeroDocumentoInvalidoParaCancelamentoCargaAgrupamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.CargaPagamento.CargaAgrupamento.Codigo == codigoCarga &&
                                     o.Situacao == SituacaoDocumentoFaturamento.EmFechamento);

            return query.Select(o => o.Pagamento.Numero).ToList();
        }

        public List<int> BuscarCodigosAtivosPorCargaECTeSemComplementares(int codigoCarga)
        {
            List<SituacaoDocumentoFaturamento> situacoesNaoPermitidas = new List<SituacaoDocumentoFaturamento>()
            {
                  SituacaoDocumentoFaturamento.Anulado,
                  SituacaoDocumentoFaturamento.Cancelado
            };

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => !situacoesNaoPermitidas.Contains(o.Situacao) && (queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo) || o.Carga.Codigo == codigoCarga));

            return queryDocumentoFaturamento.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosAtivosPorCTeSemComplementares(int codigoCarga)
        {
            List<SituacaoDocumentoFaturamento> situacoesNaoPermitidas = new List<SituacaoDocumentoFaturamento>()
            {
                  SituacaoDocumentoFaturamento.Anulado,
                  SituacaoDocumentoFaturamento.Cancelado
            };

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null && obj.LancamentoNFSManual == null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => !situacoesNaoPermitidas.Contains(o.Situacao) && queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo));

            return queryDocumentoFaturamento.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosAtivosPorCargaSemComplementares(int codigoCarga)
        {
            List<SituacaoDocumentoFaturamento> situacoesNaoPermitidas = new List<SituacaoDocumentoFaturamento>()
            {
                  SituacaoDocumentoFaturamento.Anulado,
                  SituacaoDocumentoFaturamento.Cancelado
            };

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null && obj.LancamentoNFSManual == null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => !situacoesNaoPermitidas.Contains(o.Situacao) && o.Carga.Codigo == codigoCarga);

            return queryDocumentoFaturamento.Select(o => o.Codigo).Distinct().ToList();
        }

        public void LiberarPagamentoPorCarga(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("update DocumentoFaturamento set AguardandoAutorizacao = 0 where CargaPagamento.Codigo = :codigoCarga")
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        public void LiberarPagamentosAutomaticamentePorPagamento(int pagamento)
        {
            UnitOfWork.Sessao.CreateQuery("update DocumentoFaturamento set DataLiberacaoPagamento = :DataLiberacaoPagamento, PagamentoDocumentoBloqueado = :SetPagamentoDocumentoBloqueado where Pagamento.Codigo = :Pagamento and PagamentoDocumentoBloqueado = :PagamentoDocumentoBloqueado and LiberarPagamentoAutomaticamente = :LiberarPagamentoAutomaticamente")
                .SetBoolean("SetPagamentoDocumentoBloqueado", false)
                .SetInt32("Pagamento", pagamento)
                .SetBoolean("PagamentoDocumentoBloqueado", true)
                .SetBoolean("LiberarPagamentoAutomaticamente", true)
                .SetDateTime("DataLiberacaoPagamento", DateTime.Now)
                .ExecuteUpdate();
        }
        public List<Dominio.Enumeradores.TipoDocumento> BuscarPorTipoDocumento(int codigoPagamento, bool retornarSomenteDocumentosDesbloqueados)
        {
            return BuscarPorTipoDocumento(new List<int> { codigoPagamento }, retornarSomenteDocumentosDesbloqueados);
        }

        public List<Dominio.Enumeradores.TipoDocumento> BuscarPorTipoDocumento(List<int> codigosPagamento, bool retornarSomenteDocumentosDesbloqueados)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador);

            consultaDocumentoFaturamento = consultaDocumentoFaturamento.Where(obj =>
                (obj.PagamentoLiberacao.LotePagamentoLiberado == true && codigosPagamento.Contains(obj.PagamentoLiberacao.Codigo)) ||
                (obj.Pagamento.LotePagamentoLiberado == false && codigosPagamento.Contains(obj.Pagamento.Codigo))
            );

            if (retornarSomenteDocumentosDesbloqueados)
                consultaDocumentoFaturamento = consultaDocumentoFaturamento.Where(o => o.PagamentoDocumentoBloqueado == false);

            return consultaDocumentoFaturamento
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Select(obj => obj.ModeloDocumentoFiscal.TipoDocumentoEmissao)
                .Distinct()
                .ToList();
        }



        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorPagamento(int codigoPagamento, bool retornarSomenteDocumentosDesbloqueados)
        {
            return BuscarPorPagamentos(new List<int>() { codigoPagamento }, retornarSomenteDocumentosDesbloqueados);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorPagamentos(List<int> codigosPagamento, bool retornarSomenteDocumentosDesbloqueados)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(o => o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador);

            consultaDocumentoFaturamento = consultaDocumentoFaturamento.Where(obj =>
                (obj.PagamentoLiberacao.LotePagamentoLiberado == true && codigosPagamento.Contains(obj.PagamentoLiberacao.Codigo)) ||
                (obj.Pagamento.LotePagamentoLiberado == false && codigosPagamento.Contains(obj.Pagamento.Codigo))
            );

            if (retornarSomenteDocumentosDesbloqueados)
                consultaDocumentoFaturamento = consultaDocumentoFaturamento.Where(o => o.PagamentoDocumentoBloqueado == false);

            return consultaDocumentoFaturamento
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.CentroResultadoCOFINS)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.CentroResultadoPIS)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.CentroResultadoICMS)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.Serie)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.Empresa)
                .ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFaturaCarga> ConsultaFaturaCarga(int codigoCarga)
        {
            var sql = $@"select 
                        FatDoc.FAT_CODIGO CodigoFatura,
                        Titulo.TIT_CODIGO CodigoTitulo,

                        CASE WHEN FatDoc.FAT_CODIGO IS NOT NULL THEN
                                (SELECT SUM(DISTINCT C.FAT_TOTAL) from T_FATURA_DOCUMENTO F
                                JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                                JOIN T_FATURA C ON C.FAT_CODIGO = D.FAT_CODIGO
                                WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO)
                                ELSE SUM(DFA_VALOR_DOCUMENTO)
                                END Valor,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        CASE WHEN FatDoc.FAT_CODIGO IS NOT NULL THEN
                        SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CON_NUM AS VARCHAR(20)) from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) 
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CON_NUM AS VARCHAR(20)) from T_DOCUMENTO_FATURAMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE D.CAR_CODIGO_PAGAMENTO = docu.CAR_CODIGO_PAGAMENTO and D.TIT_CODIGO is null and D.FAT_CODIGO is null FOR XML PATH('')), 3, 1000) 
                        END
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CON_NUM AS VARCHAR(20)) from T_TITULO_DOCUMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END NumerosFiscais,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST((CASE WHEN C.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR C.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN C.FAT_NUMERO ELSE C.FAT_NUMERO_FATURA_INTEGRACAO END) AS VARCHAR(20)) from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_FATURA C ON C.FAT_CODIGO = D.FAT_CODIGO
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000), '') 
                        ELSE ''
                        END NumeroFatura,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + ISNULL(T.TIT_NOSSO_NUMERO, '')
                        FROM T_FATURA_PARCELA FP
                        JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                        WHERE FP.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000), '')
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + C.TIT_NOSSO_NUMERO from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END NumeroBoletos,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST((CASE WHEN T.TIT_CODIGO_RECEBIDO_INTEGRACAO IS NULL OR T.TIT_CODIGO_RECEBIDO_INTEGRACAO = 0 THEN T.TIT_CODIGO ELSE T.TIT_CODIGO_RECEBIDO_INTEGRACAO END) AS VARCHAR(20))
                        FROM T_FATURA_PARCELA FP
                        JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                        WHERE FP.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000)  , '')
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CAST((CASE WHEN C.TIT_CODIGO_RECEBIDO_INTEGRACAO IS NULL OR C.TIT_CODIGO_RECEBIDO_INTEGRACAO = 0 THEN C.TIT_CODIGO ELSE C.TIT_CODIGO_RECEBIDO_INTEGRACAO END) AS VARCHAR(20)) from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END NumeroTitulos,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), FP.FAP_DATA_VENCIMENTO , 103) 
                        FROM T_FATURA_PARCELA FP
                        WHERE FP.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) , '')
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), C.TIT_DATA_VENCIMENTO , 103)  from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END Vencimento,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), FP.FAP_DATA_EMISSAO , 103) 
                        FROM T_FATURA_PARCELA FP
                        WHERE FP.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) , '')
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), C.TIT_DATA_EMISSAO , 103)  from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END Emissao,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        CASE WHEN FatDoc.FAT_CODIGO IS NOT NULL THEN
                        SUBSTRING((SELECT DISTINCT ', ' + C.CON_NUMERO_CONTROLE from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) 
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + C.CON_NUMERO_CONTROLE from T_DOCUMENTO_FATURAMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE D.CAR_CODIGO_PAGAMENTO = docu.CAR_CODIGO_PAGAMENTO and D.TIT_CODIGO is null and D.FAT_CODIGO is null FOR XML PATH('')), 3, 1000) 
                        END
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + C.CON_NUMERO_CONTROLE from T_TITULO_DOCUMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END NumeroControle,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        CASE WHEN FatDoc.FAT_CODIGO IS NOT NULL THEN
                        SUBSTRING((SELECT DISTINCT ', ' + Cli.CLI_NOME from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_FATURA C ON C.FAT_CODIGO = D.FAT_CODIGO
                        JOIN T_CLIENTE Cli on Cli.CLI_CGCCPF = C.CLI_CGCCPF_TOMADOR
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) 
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + P.PCT_NOME from T_DOCUMENTO_FATURAMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        JOIN T_CTE_PARTICIPANTE P ON P.PCT_CODIGO = C.CON_TOMADOR_PAGADOR_CTE
                        WHERE D.CAR_CODIGO_PAGAMENTO = docu.CAR_CODIGO_PAGAMENTO and D.TIT_CODIGO is null and D.FAT_CODIGO is null FOR XML PATH('')), 3, 1000) 
                        END
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + Cli.CLI_NOME  from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        JOIN T_CLIENTE Cli on Cli.CLI_CGCCPF = C.CLI_CGCCPF
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END Tomador,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN C.FAT_SITUACAO = 2 THEN 'Fechado'
                        WHEN C.FAT_SITUACAO = 2 THEN 'Cancelado'
                        ELSE 'Em Andamento'
                        END
                        from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_FATURA C ON C.FAT_CODIGO = D.FAT_CODIGO
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) , '')
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN C.TIT_STATUS = 4 THEN 'Cancelado'
                        WHEN C.TIT_STATUS = 3 THEN 'Quitado'
                        ELSE 'Fechado'
                        END  from T_TITULO_DOCUMENTO D 
                        JOIN T_TITULO C ON C.TIT_CODIGO = D.TIT_CODIGO
                        WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END Situacao,

                        CASE WHEN Titulo.TIT_CODIGO IS NULL THEN
                        CASE WHEN FatDoc.FAT_CODIGO IS NOT NULL THEN
                        SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN C.CON_TIPO_CTE = 0 THEN 'Normal'
                        WHEN C.CON_TIPO_CTE = 1 THEN 'Complemento'
                        WHEN C.CON_TIPO_CTE = 2 THEN 'Anulação'
                        WHEN C.CON_TIPO_CTE = 3 THEN 'Substituto'
                        ELSE ''
                        END
                        from T_FATURA_DOCUMENTO F
                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE F.FAT_CODIGO = FatDoc.FAT_CODIGO FOR XML PATH('')), 3, 1000) 
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN C.CON_TIPO_CTE = 0 THEN 'Normal'
                        WHEN C.CON_TIPO_CTE = 1 THEN 'Complemento'
                        WHEN C.CON_TIPO_CTE = 2 THEN 'Anulação'
                        WHEN C.CON_TIPO_CTE = 3 THEN 'Substituto'
                        ELSE ''
                        END from T_DOCUMENTO_FATURAMENTO D 
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE D.CAR_CODIGO_PAGAMENTO = docu.CAR_CODIGO_PAGAMENTO and D.TIT_CODIGO is null and D.FAT_CODIGO is null FOR XML PATH('')), 3, 1000) 
                        END
                        ELSE SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN C.CON_TIPO_CTE = 0 THEN 'Normal'
                        WHEN C.CON_TIPO_CTE = 1 THEN 'Complemento'
                        WHEN C.CON_TIPO_CTE = 2 THEN 'Anulação'
                        WHEN C.CON_TIPO_CTE = 3 THEN 'Substituto'
                        ELSE ''
                        END
                        from T_TITULO F
                        JOIN T_TITULO_DOCUMENTO D ON D.TIT_CODIGO = F.TIT_CODIGO
                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                        WHERE F.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) 
                        END TipoCTe

                        from
                                T_DOCUMENTO_FATURAMENTO docu             
                        LEFT OUTER JOIN T_FATURA_DOCUMENTO FatDoc on FatDoc.DFA_CODIGO = docu.DFA_CODIGO and FatDoc.FDO_CANCELADO = 0
                        LEFT OUTER JOIN T_TITULO Titulo on Titulo.TIT_CODIGO = docu.TIT_CODIGO
                            where
                                docu.CAR_CODIGO_PAGAMENTO = {codigoCarga}
		                        group by FatDoc.FAT_CODIGO, Titulo.TIT_CODIGO, docu.CAR_CODIGO_PAGAMENTO";

            var consultaCargas = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaCargas.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFaturaCarga)));

            return consultaCargas.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFaturaCarga>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamentoParaFatura> ConsultarDocumentosFaturamentoParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro)
        {
            var sqlDinamico = QueryDocumentosFaturamentoParaFatura(filtrosPesquisa, false, parametrosConsulta, configFinanceiro);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamentoParaFatura)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamentoParaFatura>();
        }

        public int ContarConsultaDocumentosFaturamentoParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            var sqlDinamico = QueryDocumentosFaturamentoParaFatura(filtrosPesquisa, true, parametrosConsulta, configFinanceiro);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ConsultarCargaFaturamento(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            var result = from obj in query where obj.CargaPagamento.Codigo == codigoCarga select obj;

            result = result
                .Fetch(o => o.Titulo).ThenFetch(o => o.Pessoa)
                .Fetch(o => o.Fatura).ThenFetch(o => o.Cliente)
                .Fetch(o => o.Fatura).ThenFetch(o => o.GrupoPessoas);

            return ObterLista(result, parametroConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarPorCancelamentoPagamento(int codigoCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamento select obj;
            return result.ToList();
        }

        public int ContarConsultaCargaFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            var result = from obj in query where obj.CargaPagamento.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public List<int> BuscarCodigosCargasPorCodigoDocumentos(int codigoPagamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(q => codigoPagamento == q.Pagamento.Codigo
                                && q.CargaPagamento.TipoOperacao != null
                                && q.CargaPagamento.TipoOperacao.ConfiguracaoCarga != null
                                && q.CargaPagamento.TipoOperacao.ConfiguracaoCarga.PrecisaEsperarNotasFilhaParaGerarPagamento == true);
            return query.Select(d => d.CargaPagamento.Codigo).ToList();
        }

        public int BuscarCodigoDocumentoPorPagamentoECargaAgNotaFilha(int codigoPagamento, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            query = query.Where(q => q.Pagamento.Codigo == codigoPagamento
                                && q.CargaPagamento.Codigo == codigoCarga
                                && q.CargaPagamento.TipoOperacao != null
                                && q.CargaPagamento.TipoOperacao.ConfiguracaoCarga != null
                                && q.CargaPagamento.TipoOperacao.ConfiguracaoCarga.PrecisaEsperarNotasFilhaParaGerarPagamento == true);

            return query.Select(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCteDosDocumentoFaturamentoPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(p => p.Pagamento.Codigo == codigoPagamento &&
                                p.CargaPagamento.TipoOperacao != null
                                && p.CargaPagamento.TipoOperacao.ConfiguracaoCarga != null
                                && p.CargaPagamento.TipoOperacao.ConfiguracaoCarga.PrecisaEsperarNotaTransferenciaParaGeraPagamento == true);

            return query.Select(d => d.CTe).ToList();
        }

        public int BuscarCodigoDocumentoFaturamentoPorCTeEPagamento(int codigoPagamento, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(p => p.Pagamento.Codigo == codigoPagamento
                                && p.CargaPagamento.TipoOperacao != null
                                && p.CargaPagamento.TipoOperacao.ConfiguracaoCarga != null
                                && p.CargaPagamento.TipoOperacao.ConfiguracaoCarga.PrecisaEsperarNotaTransferenciaParaGeraPagamento == true
                                && p.CTe.Codigo == codigoCTe);

            return query.Select(obj => obj.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCtePorNumeroMiro(string numeroMiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(p => p.NumeroMiro == numeroMiro);

            return query.Select(d => d.CTe).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarDocumentosFaturamentoLiberadosPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>()
                .Where(p => p.Pagamento.Codigo == codigoPagamento);

            return query.ToList();
        }

        public int LiberarDocumentosFaturamentoComProvisaoGerada()
        {
            string sql = @"UPDATE T_DOCUMENTO_FATURAMENTO
                           SET T_DOCUMENTO_FATURAMENTO.DFA_PROVISAO_GERADA = 1
                           FROM T_DOCUMENTO_FATURAMENTO
                           JOIN T_CARGA_CTE ON T_CARGA_CTE.CON_CODIGO = T_DOCUMENTO_FATURAMENTO.CON_CODIGO
                           WHERE ISNULL(T_DOCUMENTO_FATURAMENTO.DFA_PROVISAO_GERADA, 0) = 0
                           AND PAG_CODIGO IS NULL
                           AND (
                                    NOT EXISTS (SELECT 1 
	                                           FROM T_DOCUMENTO_PROVISAO 
	                                           JOIN T_CTE_XML_NOTAS_FISCAIS ON T_CTE_XML_NOTAS_FISCAIS.NFX_CODIGO = T_DOCUMENTO_PROVISAO.NFX_CODIGO 
                                               AND T_CTE_XML_NOTAS_FISCAIS.CON_CODIGO = T_DOCUMENTO_FATURAMENTO.CON_CODIGO
	                                           WHERE T_DOCUMENTO_PROVISAO.DPV_SITUACAO = :codigoSituacaoProvisaoPendente AND T_DOCUMENTO_PROVISAO.CAR_CODIGO = T_CARGA_CTE.CAR_CODIGO)
                                    OR EXISTS (SELECT 1 
	                                            FROM T_DOCUMENTO_PROVISAO 
	                                            JOIN T_CTE_XML_NOTAS_FISCAIS ON T_CTE_XML_NOTAS_FISCAIS.NFX_CODIGO = T_DOCUMENTO_PROVISAO.NFX_CODIGO 
                                                AND T_CTE_XML_NOTAS_FISCAIS.CON_CODIGO = T_DOCUMENTO_FATURAMENTO.CON_CODIGO
												WHERE T_DOCUMENTO_PROVISAO.COC_CODIGO IS NOT NULL)
                           )";

            var query = this.SessionNHiBernate
                .CreateSQLQuery(sql)
                .SetParameter("codigoSituacaoProvisaoPendente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao);

            return query.ExecuteUpdate();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.DocumentoPagamento> BuscarParaAPIDePagamentos(List<int> codigosPagamentos, DateTime inicial, DateTime final)
        {
            string pattern = "yyyy-MM-dd";
            StringBuilder sql = new();

            sql.Append($@"SELECT DFA.PAG_CODIGO AS CodigoPagamento, 
                        DFA_CODIGO AS CodigoDocumento,
                        DFA_NUMERO AS NumeroDocumento, 
                        COALESCE(ESE_NUMERO_TEXTO, CAST(ESE_NUMERO AS VARCHAR)) AS serieDocumento,
                        CON_CHAVECTE AS Chave, 
                        CON_DATAHORAEMISSAO AS DataEmissaoDocumento, 
                        DFA_TIPO_DOCUMENTO AS TipoDocumentoInt, 
                        DFA.CON_CODIGO AS ProtocoloCTe, 
                        DFA_VALOR_LIQUIDO AS ValorFrete, 
                        DFA_NUMERO_FISCAL AS NotaFiscal,
                        CPG_NUMERO AS NumeroCancelamento, 
                        CPG_SITUACAO AS SituacaoCancelamentoInt,
                        MCP_DESCRICAO AS MotivoCancelamento
                        FROM T_DOCUMENTO_FATURAMENTO DFA LEFT JOIN T_CTE CON ON DFA.CON_CODIGO = CON.CON_CODIGO
                        LEFT JOIN T_CANCELAMENTO_PAGAMENTO CPG ON DFA.CPG_CODIGO = CPG.CPG_CODIGO
                        LEFT JOIN T_EMPRESA_SERIE ESE ON ESE.ESE_CODIGO = CON.CON_SERIE
                        LEFT JOIN T_MOTIVO_CANCELAMENTO_PAGAMENTO MCP ON CPG.MCP_CODIGO = MCP.MCP_CODIGO"
                        );

            if (!codigosPagamentos.IsNullOrEmpty())
                sql.Append($@" WHERE DFA.PAG_CODIGO IN ({string.Join(", ", codigosPagamentos)})");
            else
                sql.Append($@" WHERE DFA.PAG_CODIGO = 0");

            sql.Append($@" AND CON.CON_DATAHORAEMISSAO BETWEEN '{inicial.ToString(pattern)}' AND '{final.ToString(pattern)}'");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.DocumentoPagamento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.DocumentoPagamento>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> _ConsultarParaFaturamento(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            IQueryable<int> subQueryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>().Where(o => o.Fatura.Codigo == filtros.CodigoFatura).Select(o => o.Documento.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            query = query.Where(o => (o.CTe.Status == "A" || situacoesCarga.Contains(o.Carga.SituacaoCarga)) &&
                                     o.ValorAFaturar > 0m &&
                                     !subQueryFaturaDocumento.Any(c => c == o.Codigo) &&
                                     o.TipoLiquidacao == TipoLiquidacao.Fatura &&
                                     !o.CTe.CargaCTes.Any(c => c.Carga.CargaSVM));

            if (filtros.DataInicial.HasValue && filtros.DataInicial.Value > DateTime.MinValue && filtros.DataFinal.HasValue && filtros.DataFinal.Value > DateTime.MinValue)
                query = query.Where(o => (o.Carga.DataFinalizacaoEmissao >= filtros.DataInicial.Value.Date && o.Carga.DataFinalizacaoEmissao < filtros.DataFinal.Value.AddDays(1).Date) ||
                                                     (o.CTe.DataEmissao >= filtros.DataInicial.Value.Date && o.CTe.DataEmissao < filtros.DataFinal.Value.AddDays(1).Date));
            else if (filtros.DataInicial.HasValue && filtros.DataInicial.Value > DateTime.MinValue)
                query = query.Where(o => (o.Carga.DataFinalizacaoEmissao >= filtros.DataInicial.Value.Date) ||
                                                     (o.CTe.DataEmissao >= filtros.DataInicial.Value.Date));
            else if (filtros.DataFinal.HasValue && filtros.DataFinal.Value > DateTime.MinValue)
                query = query.Where(o => (o.Carga.DataFinalizacaoEmissao < filtros.DataFinal.Value.AddDays(1).Date) ||
                                                     (o.CTe.DataEmissao < filtros.DataFinal.Value.AddDays(1).Date));

            if (filtros.CodigosCTes != null && filtros.CodigosCTes.Count > 0)
                query = query.Where(o => filtros.CodigosCTes.Contains(o.CTe.Codigo));

            query = query.Where(o => o.FaturamentoPermissaoExclusiva == filtros.ApenasFaturaExclusiva);

            if (filtros.CPFCNPJTomador > 0d)
                query = query.Where(o => o.Tomador.CPF_CNPJ == filtros.CPFCNPJTomador);

            if (!string.IsNullOrWhiteSpace(filtros.IETomador))
                query = query.Where(o => o.CTe.TomadorPagador.IE_RG == filtros.IETomador);

            if (filtros.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.Tomador.GrupoPessoas.Codigo == filtros.CodigoGrupoPessoas || o.GrupoPessoas.Codigo == filtros.CodigoGrupoPessoas);

            if (filtros.CodigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == filtros.CodigoCarga || o.CTe.CargaCTes.Any(c => c.Carga.Codigo == filtros.CodigoCarga));

            if (filtros.TipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == filtros.TipoOperacao);

            if (filtros.Empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtros.Empresa);

            if (filtros.TipoCarga > 0)
                query = query.Where(o => o.TipoCarga.Codigo == filtros.TipoCarga);

            if (filtros.AliquotaICMS.HasValue)
                query = query.Where(o => o.AliquotaICMS == filtros.AliquotaICMS.Value);

            if (filtros.PedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == filtros.PedidoViagemNavio);

            if (filtros.TerminalDestino > 0)
                query = query.Where(o => o.CTe.TerminalDestino.Codigo == filtros.TerminalDestino);

            if (filtros.TerminalOrigem > 0)
                query = query.Where(o => o.CTe.TerminalOrigem.Codigo == filtros.TerminalOrigem);

            if (filtros.Origem > 0)
                query = query.Where(o => o.CTe.LocalidadeInicioPrestacao.Codigo == filtros.Origem);

            if (filtros.PaisOrigem > 0)
                query = query.Where(o => o.CTe.LocalidadeInicioPrestacao.Pais != null && o.CTe.LocalidadeInicioPrestacao.Pais.Codigo == filtros.PaisOrigem);

            if (filtros.Destino > 0)
                query = query.Where(o => o.CTe.LocalidadeTerminoPrestacao.Codigo == filtros.Destino);

            if (filtros.Filial > 0)
                query = query.Where(o => o.Filial.Codigo == filtros.Filial);

            if (!string.IsNullOrWhiteSpace(filtros.NumeroBooking))
                query = query.Where(o => o.CTe.NumeroBooking == filtros.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtros.NumeroDocumento))
            {
                if (int.TryParse(filtros.NumeroDocumento, out int numero))
                    query = query.Where(o => o.CTe.Numero == numero || o.Carga.CodigoCargaEmbarcador == filtros.NumeroDocumento);
                else
                    query = query.Where(o => o.Carga.CodigoCargaEmbarcador == filtros.NumeroDocumento);
            }

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (filtros.TipoPropostaMultimodal != null && filtros.TipoPropostaMultimodal.Count > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                queryCargaPedido = queryCargaPedido.Where(obj => filtros.TipoPropostaMultimodal.Contains(obj.TipoPropostaMultimodal));
                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));
                query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj.CTe));
            }

            if (filtros.TiposPropostasMultimodal != null && filtros.TiposPropostasMultimodal.Count > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                queryCargaPedido = queryCargaPedido.Where(obj => filtros.TiposPropostasMultimodal.Contains(obj.TipoPropostaMultimodal));
                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));
                query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj.CTe));
            }

            if (filtros.CodigoContainer > 0)
                query = query.Where(o => o.CTe.Containers.Any(c => c.Container.Codigo == filtros.CodigoContainer));

            if (!string.IsNullOrWhiteSpace(filtros.NumeroControleCliente))
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.CodigoPedidoCliente == filtros.NumeroControleCliente);
                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));
                query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj.CTe));
            }

            if (!string.IsNullOrWhiteSpace(filtros.NumeroReferenciaEDI))
                query = query.Where(o => o.CTe.Documentos.Any(c => c.NumeroReferenciaEDI == filtros.NumeroReferenciaEDI));

            if (filtros.CodigoCTe > 0)
                query = query.Where(o => o.CTe.Codigo == filtros.CodigoCTe);

            if (filtros.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculos.Any(vei => vei.Codigo == filtros.CodigoVeiculo));

            if (filtros.TipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                query = query.Where(o => o.CTe.TipoCTE == filtros.TipoCTe);

            if (filtros.NumeroDocumentoInicial > 0)
            {
                query = query.Where(o => o.CTe.Numero >= filtros.NumeroDocumentoInicial);
            }

            if (filtros.NumeroDocumentoFinal > 0)
            {
                query = query.Where(o => o.CTe.Numero <= filtros.NumeroDocumentoFinal);
            }

            if (filtros.Serie > 0)
            {
                query = query.Where(o => o.CTe.Serie.Numero == filtros.Serie);
            }

            if (filtros.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados)
                query = query.Where(o => o.CanhotosDigitalizados == filtros.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados);

            if (filtros.TiposOSConvertidos != null && filtros.TiposOSConvertidos.Count > 0)
                query = query.Where(o => o.CargaPagamento.TipoOSConvertido.HasValue && filtros.TiposOSConvertidos.Contains((TipoOSConvertido)(int)o.CargaPagamento.TipoOSConvertido.Value));

            if (filtros.GerarDocumentosApenasCanhotosAprovados)
            {
                IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> queryCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                IQueryable<int> codigosCTe = query.Select(obj => obj.CTe.Codigo);

                IQueryable<int> codigosCarga = queryCargaCTe
                    .Where(obj => codigosCTe.Contains(obj.CTe.Codigo))
                    .Select(cargaCTe => cargaCTe.Carga.Codigo);

                IQueryable<int> cargasComCanhotosPendentes = queryCanhoto
                    .Where(canhoto => codigosCarga.Contains(canhoto.Carga.Codigo) &&
                                      canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                    .Select(canhoto => canhoto.Carga.Codigo);

                IQueryable<int> ctesComCanhotosPendentes = queryCargaCTe
                    .Where(obj => cargasComCanhotosPendentes.Contains(obj.Carga.Codigo))
                    .Select(obj => obj.CTe.Codigo);

                query = query.Where(obj => !ctesComCanhotosPendentes.Contains(obj.CTe.Codigo));
            }

            return query;
        }

        public IList<int> ObterCodigosDocumentosFaturamentoSemPagamentoComCanhotoDigitalizado()
        {
            string sql = $@"SELECT DISTINCT DocumentoFaturamento.DFA_CODIGO
                              FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                              JOIN T_CTE CTe ON CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                              JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                             WHERE 0 = 0
                               AND DocumentoFaturamento.PAG_CODIGO IS NULL
                               AND DocumentoFaturamento.DFA_BLOQUEAR_ENVIO_AUTOMATICO = 0
                               AND DocumentoFaturamento.DFA_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado}
                               AND DocumentoFaturamento.DFA_TIPO_LIQUIDACAO = {(int)TipoLiquidacao.PagamentoTransportador}
                               AND CTe.CON_TIPO_CTE <> {(int)Dominio.Enumeradores.TipoCTE.Substituto}
                               AND EXISTS (SELECT 1 
                                             FROM T_CANHOTO_NOTA_FISCAL Canhoto
                                            WHERE Canhoto.CNF_DATA_DIGITALIZACAO IS NOT NULL
                                              AND Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = {(int)SituacaoDigitalizacaoCanhoto.Digitalizado}
                                              AND (
                                                   (Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.Avulso} 
                                                    AND EXISTS(SELECT 1 
                                                                 FROM T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL CanhotoAvulsoPedido 
                                                                 JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal ON PedidoXMLNotaFiscal.PNF_CODIGO = CanhotoAvulsoPedido.PNF_CODIGO
                                                                 JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO     
                                                                WHERE CanhotoAvulsoPedido.CAV_CODIGO = Canhoto.CAV_CODIGO 
                                                                  AND XMLNotaFiscal.NFX_CODIGO = CTeXMLNotaFiscal.NFX_CODIGO)
                                                   ) OR 
                                                   (Canhoto.CNF_TIPO_CANHOTO <> {(int)TipoCanhoto.Avulso}
                                                    AND Canhoto.NFX_CODIGO = CTeXMLNotaFiscal.NFX_CODIGO
                                                   )
                                                  )
                                   )";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.List<int>();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> Consultar(List<int> pagamentos, int cancelamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPequisa, bool bloquearEnvioAutomatico, bool somentePagamentoLiberado, bool retornarSomenteDocumentosDesbloqueados, IList<int> codigosDocumentosComCanhotoDigitalizado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            var result = query.Where(obj => obj.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador
                            && (cancelamento > 0
                               || (obj.Situacao != SituacaoDocumentoFaturamento.Anulado)
                               && (obj.CargaPagamento == null || (obj.CargaPagamento.SituacaoCarga != SituacaoCarga.Cancelada && obj.CargaPagamento.SituacaoCarga != SituacaoCarga.Anulada))))
                 .Select(obj => obj);

            if (cancelamento == 0)
                result = result.Where(obj => (obj.Situacao != SituacaoDocumentoFaturamento.Cancelado
                    || (obj.CancelamentoPagamento != null && obj.CancelamentoPagamento.Situacao == SituacaoCancelamentoPagamento.Cancelado))
                    && obj.CTe.Status != "C");

            if (pagamentos?.Count > 0 && !filtroPequisa.PagamentoLiberado)
                result = result.Where(obj => pagamentos.Contains(obj.Pagamento.Codigo));

            if (pagamentos?.Count > 0 && filtroPequisa.PagamentoLiberado)
                result = result.Where(obj => pagamentos.Contains(obj.PagamentoLiberacao.Codigo));

            if (cancelamento > 0)
                result = result.Where(obj => obj.CancelamentoPagamento.Codigo == cancelamento);

            if (filtroPequisa.PagamentoFinalizados && cancelamento == 0)
                result = result.Where(obj => obj.Pagamento.Situacao == SituacaoPagamento.Finalizado || obj.PagamentoLiberacao.Situacao == SituacaoPagamento.Finalizado);

            if (filtroPequisa.CodigoCarga > 0)
                result = result.Where(o => o.CargaPagamento.Codigo == filtroPequisa.CodigoCarga);

            if (filtroPequisa.CodigoOcorrencia > 0)
                result = result.Where(o => o.CargaOcorrenciaPagamento.Codigo == filtroPequisa.CodigoOcorrencia);

            if (filtroPequisa.DataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataLiberacaoPagamento.Value >= filtroPequisa.DataInicio.Date);

            if (filtroPequisa.DataFim != DateTime.MinValue)
            {
                if (filtroPequisa.ConsiderarHorasRetroativas)
                {
                    if (filtroPequisa.ConsultarPorDataEmissao && codigosDocumentosComCanhotoDigitalizado != null)
                        result = result.Where(obj => obj.DataEmissao < filtroPequisa.DataFim || codigosDocumentosComCanhotoDigitalizado.Contains(obj.Codigo));
                    else if (filtroPequisa.ConsultarPorDataEmissao)
                        result = result.Where(obj => obj.DataEmissao < filtroPequisa.DataFim);
                    else
                        result = result.Where(obj => obj.DataLiberacaoPagamento.Value < filtroPequisa.DataFim);
                }
                else
                {
                    if (filtroPequisa.ConsultarPorDataEmissao)
                        result = result.Where(obj => obj.DataEmissao < filtroPequisa.DataFim.AddDays(1).Date);
                    else
                        result = result.Where(obj => obj.DataLiberacaoPagamento.Value < filtroPequisa.DataFim.AddDays(1).Date);
                }

            }

            if (filtroPequisa.DataInicialEmissao != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao >= filtroPequisa.DataInicialEmissao.Date);

            if (filtroPequisa.DataFinalEmissao != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao < filtroPequisa.DataFinalEmissao.AddDays(1).Date);

            if (filtroPequisa.CodigosTransportador?.Count > 0)
                result = result.Where(o => filtroPequisa.CodigosTransportador.Contains(o.Empresa.Codigo));

            if (filtroPequisa.CodigosFilial?.Count > 0)
                result = result.Where(o => filtroPequisa.CodigosFilial.Contains(o.Filial.Codigo));

            if (filtroPequisa.CodigosTomador?.Count > 0)
                result = result.Where(o => filtroPequisa.CodigosTomador.Contains(o.Tomador.CPF_CNPJ));

            if (filtroPequisa.CodigosTiposOperacao?.Count > 0)
                result = result.Where(o => filtroPequisa.CodigosTiposOperacao.Contains(o.CargaPagamento.TipoOperacao.Codigo));

            if (filtroPequisa.SituacaoPagamentoDocumento != SituacaoDocumentoFaturamento.Todos && !filtroPequisa.PagamentoLiberado)
                result = result.Where(o => o.Situacao == filtroPequisa.SituacaoPagamentoDocumento);

            if (bloquearEnvioAutomatico)
                result = result.Where(o => o.BloquearEnvioAutomatico == false);

            if (somentePagamentoLiberado)
                result = result.Where(o => !o.AguardandoAutorizacao);

            if (filtroPequisa.PagamentoLiberado && ((pagamentos?.Count ?? 0) == 0))
                result = result.Where(o => o.PagamentoDocumentoBloqueado == false && o.Pagamento != null && o.Situacao == SituacaoDocumentoFaturamento.Liquidado && o.PagamentoLiberacao == null);

            if (retornarSomenteDocumentosDesbloqueados)
                result = result.Where(o => o.PagamentoDocumentoBloqueado == false);

            if (filtroPequisa.NaoGerarAutomaticamenteLotesCancelados)
                result = result.Where(o => !o.BloqueioGeracaoAutomaticaPagamento && o.CancelamentoPagamento == null);
            else if (filtroPequisa.SomenteComDocumentosDesbloqueados)
                result = result.Where(o => o.BloqueioGeracaoAutomaticaPagamento != true);

            if (filtroPequisa.ModeloDocumentoFiscal > 0)
                result = result.Where(o => o.ModeloDocumentoFiscal.Codigo == filtroPequisa.ModeloDocumentoFiscal);

            if (filtroPequisa.TipoDocumentoGerado.HasValue)
            {
                switch (filtroPequisa.TipoDocumentoGerado.Value)
                {
                    case TipoDocumentoGerado.SomenteCargas:
                        result = result.Where(o => o.CargaPagamento != null && o.CargaOcorrenciaPagamento == null);
                        break;
                    case TipoDocumentoGerado.SomenteOcorrencias:
                        result = result.Where(o => o.CargaPagamento != null && o.CargaOcorrenciaPagamento != null);
                        break;
                    default:
                        break;
                }

            }

            if (filtroPequisa.SituacaoDocumentoPagamento.HasValue)
                if (filtroPequisa.SituacaoDocumentoPagamento.Value == SituacaoDocumentoPagamento.Bloqueado)
                    result = result.Where(o => o.PagamentoDocumentoBloqueado);
                else
                    result = result.Where(o => !o.PagamentoDocumentoBloqueado);

            if (filtroPequisa.SomenteComProvisaoGerada)
                result = result.Where(o => o.ProvisaoGerada);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ObterConsultaDocumentosParaFaturar(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, int serie, DateTime dataInicial, DateTime dataFinal, string numeroCarga, int numeroDocumentoInicial, int numeroDocumentoFinal, int codigoGrupoPessoas, double cpfCnpjTomador, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoOrigem, int codigoDestino, string numeroPedido, string numeroOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            query = query.Where(o => o.Situacao == SituacaoDocumentoFaturamento.Autorizado && o.TipoLiquidacao == TipoLiquidacao.Fatura && o.ValorAFaturar > 0);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (cpfCnpjTomador > 0d)
                query = query.Where(o => o.Tomador.CPF_CNPJ == cpfCnpjTomador);

            if (cpfCnpjRemetente > 0d)
                query = query.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);

            if (cpfCnpjDestinatario > 0d)
                query = query.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Tomador.GrupoPessoas.Codigo == codigoGrupoPessoas || o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino);

            if (serie > 0)
                query = query.Where(o => o.EmpresaSerie.Numero == serie);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculos.Any(v => v.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.NumeroCarga == numeroCarga);

            if (numeroDocumentoInicial > 0)
                query = query.Where(o => o.CTe.Numero >= numeroDocumentoInicial);

            if (numeroDocumentoFinal > 0)
                query = query.Where(o => o.CTe.Numero <= numeroDocumentoFinal);

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                query = query.Where(o => o.NumeroPedidoCliente.Any(n => n.Contains(numeroPedido)));

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                query = query.Where(o => o.NumeroPedidoOcorrenciaCliente.Any(n => n.Contains(numeroOcorrencia)));

            return query;
        }

        private string ObterSelectConsultaRelatorioPosicaoDocumentoReceber(bool count, List<PropriedadeAgrupamento> propriedades, DateTime dataPosicao, TipoFaturamentoPosicao? tipoFaturamento, int codigoEmpresa, int codigoGrupoPessoas, int codigoOrigem, int codigoDestino, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjTomador, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                    groupBy = string.Empty,
                    joins = string.Empty,
                    where = string.Empty,
                    having = string.Empty,
                    orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioPosicaoDocumentoReceber(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioPosicaoDocumentoReceber(ref where, ref having, ref groupBy, ref joins, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaDocumentos(propAgrupa, 0, ref select, ref groupBy, ref joins, count, null);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return "DECLARE @dataPosicao datetime = dateadd(day, 1, '" + dataPosicao.ToString("yyyy-MM-dd") + "');" +
                   (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento LEFT OUTER JOIN T_CTE CTe ON CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO LEFT OUTER JOIN T_TITULO_DOCUMENTO TituloDocumento ON(TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = CTe.CON_CODIGO) OR(TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = Carga.CAR_CODIGO) LEFT OUTER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO " +
                   " LEFT OUTER JOIN T_FATURA_DOCUMENTO FaturaDocumento on FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO  LEFT OUTER JOIN T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO "
                   + joins +
                   " where (CASE WHEN DocumentoFaturamento.DFA_SITUACAO = 2 AND DocumentoFaturamento.DFA_DATA_CANCELAMENTO >= @dataPosicao THEN 1 WHEN DocumentoFaturamento.DFA_SITUACAO = 3 AND DocumentoFaturamento.DFA_DATA_ANULACAO >= @dataPosicao THEN 1 WHEN DocumentoFaturamento.DFA_SITUACAO = 1 AND DocumentoFaturamento.DFA_DATA_AUTORIZACAO >= @dataPosicao THEN 0 ELSE DocumentoFaturamento.DFA_SITUACAO END) = 1 " +
                   " AND (CASE  WHEN Fatura.FAT_CODIGO IS NULL THEN 1 WHEN Fatura.FAT_SITUACAO in (1) THEN 1 WHEN Fatura.FAT_SITUACAO in (3) THEN 0 WHEN Fatura.FAT_DATA_FECHAMENTO >= dateadd(DAY, -1, @dataPosicao) THEN 1 ELSE 0 END) = 1"
                   + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (having.Length > 0 ? " having " + having : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereRelatorioPosicaoDocumentoReceber(ref string where, ref string having, ref string groupBy, ref string joins, DateTime dataPosicao, TipoFaturamentoPosicao? tipoFaturamento, int codigoEmpresa, int codigoGrupoPessoas, int codigoOrigem, int codigoDestino, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjTomador)
        {
            if (tipoFaturamento.HasValue)
            {
                if (tipoFaturamento == TipoFaturamentoPosicao.Faturado)
                    having += " SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR END) > 0 ";
                else if (tipoFaturamento == TipoFaturamentoPosicao.NaoFaturado)
                    having += " SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR END) <> DocumentoFaturamento.DFA_VALOR_DOCUMENTO ";
            }

            where += " and DocumentoFaturamento.DFA_TIPO_LIQUIDACAO = 0 AND DocumentoFaturamento.DFA_DATAHORAEMISSAO <= @dataPosicao ";

            if (codigoEmpresa > 0)
                where += " and DocumentoFaturamento.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoGrupoPessoas > 0)
            {
                where += " and GrupoPessoas.GRP_CODIGO = " + codigoGrupoPessoas.ToString();

                if (!joins.Contains(" GrupoPessoas "))
                    joins += "left outer join T_GRUPO_PESSOAS GrupoPessoas on (DocumentoFaturamento.GRP_CODIGO IS NOT NULL AND DocumentoFaturamento.GRP_CODIGO = GrupoPessoas.GRP_CODIGO) or (Tomador.GRP_CODIGO IS NOT NULL AND Tomador.GRP_CODIGO = GrupoPessoas.GRP_CODIGO) ";
            }

            if (cpfCnpjTomador > 0d)
                where += " and DocumentoFaturamento.CLI_CODIGO_TOMADOR = " + cpfCnpjTomador.ToString("F0");

            if (cpfCnpjRemetente > 0d)
                where += " and DocumentoFaturamento.CLI_CODIGO_REMETENTE = " + cpfCnpjRemetente.ToString("F0");

            if (cpfCnpjDestinatario > 0d)
                where += " and DocumentoFaturamento.CLI_CODIGO_DESTINATARIO = " + cpfCnpjDestinatario.ToString("F0");

            if (codigoOrigem > 0)
                where += " and DocumentoFaturamento.LOC_ORIGEM = " + codigoOrigem.ToString();

            if (codigoDestino > 0)
                where += " and DocumentoFaturamento.LOC_DESTINO = " + codigoDestino.ToString();
        }

        private void SetarSelectRelatorioPosicaoDocumentoReceber(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Numero":
                    if (!select.Contains(" Numero"))
                    {
                        select += "DocumentoFaturamento.DFA_NUMERO Numero, ";
                        groupBy += "DocumentoFaturamento.DFA_NUMERO, ";
                    }
                    break;
                case "Serie":
                    if (!select.Contains(" Serie"))
                    {
                        select += "Serie.ESE_NUMERO Serie, ";
                        groupBy += "Serie.ESE_NUMERO, ";

                        if (!joins.Contains(" Serie "))
                            joins += "left outer join T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa,"))
                    {
                        select += "Empresa.EMP_RAZAO Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoFaturamento.EMP_CODIGO ";
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador"))
                    {
                        select += "Tomador.CLI_NOME Tomador, ";
                        groupBy += "Tomador.CLI_NOME, ";

                        if (!joins.Contains(" Tomador "))
                            joins += "left outer join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR ";
                    }
                    break;
                case "GrupoPessoasTomador":
                    if (!select.Contains(" GrupoPessoasTomador"))
                    {
                        select += "GrupoTomador.GRP_DESCRICAO GrupoPessoasTomador, ";
                        groupBy += "GrupoTomador.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoPessoasTomador "))
                            joins += "left outer join T_GRUPO_PESSOAS GrupoPessoas on (DocumentoFaturamento.GRP_CODIGO IS NOT NULL AND DocumentoFaturamento.GRP_CODIGO = GrupoPessoas.GRP_CODIGO) or (Tomador.GRP_CODIGO IS NOT NULL AND Tomador.GRP_CODIGO = GrupoPessoas.GRP_CODIGO) ";
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente"))
                    {
                        select += "Remetente.CLI_NOME Remetente, ";
                        groupBy += "Remetente.CLI_NOME, ";

                        if (!joins.Contains(" Remetente "))
                            joins += "left outer join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario"))
                    {
                        select += "Destinatario.CLI_NOME Destinatario, ";
                        groupBy += "Destinatario.CLI_NOME, ";

                        if (!joins.Contains(" Destinatario "))
                            joins += "left outer join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo"))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumentoFaturamento1 inner join T_VEICULO veiculo1 on veiculoDocumentoFaturamento1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumentoFaturamento1.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Veiculo, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista"))
                    {
                        select += "substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumentoFaturamento1 inner join T_FUNCIONARIO motorista1 on motoristaDocumentoFaturamento1.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumentoFaturamento1.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motorista, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATAHORAEMISSAO, 103) DataEmissao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATAHORAEMISSAO, ";
                    }
                    break;
                case "DataAutorizacao":
                    if (!select.Contains(" DataAutorizacao"))
                    {
                        select += "CASE WHEN DocumentoFaturamento.DFA_DATA_AUTORIZACAO >= @dataPosicao THEN NULL ELSE CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_AUTORIZACAO, 103) END DataAutorizacao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_AUTORIZACAO, ";
                    }
                    break;
                case "DataCancelamento":
                    if (!select.Contains(" DataCancelamento"))
                    {
                        select += "CASE WHEN DocumentoFaturamento.DFA_DATA_CANCELAMENTO >= @dataPosicao THEN NULL ELSE CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_CANCELAMENTO, 103) END DataCancelamento, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_CANCELAMENTO, ";
                    }
                    break;
                case "DataAnulacao":
                    if (!select.Contains(" DataAnulacao"))
                    {
                        select += "CASE WHEN DocumentoFaturamento.DFA_DATA_ANULACAO >= @dataPosicao THEN NULL ELSE CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_ANULACAO, 103) END DataAnulacao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_ANULACAO, ";
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select += " Origem.LOC_DESCRICAO + '-' + Origem.UF_SIGLA Origem, ";
                        groupBy += "Origem.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("Origem.UF_SIGLA"))
                            groupBy += "Origem.UF_SIGLA, ";

                        if (!joins.Contains(" Origem "))
                            joins += "left outer join T_LOCALIDADES Origem on DocumentoFaturamento.LOC_ORIGEM = Origem.LOC_CODIGO ";
                    }
                    break;
                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select += " Destino.LOC_DESCRICAO + '-' + Destino.UF_SIGLA Destino, ";
                        groupBy += "Destino.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("Destino.UF_SIGLA"))
                            groupBy += "Destino.UF_SIGLA, ";

                        if (!joins.Contains(" Destino "))
                            joins += "left outer join T_LOCALIDADES Destino on DocumentoFaturamento.LOC_DESTINO = Destino.LOC_CODIGO ";
                    }
                    break;
                case "ValorICMS":
                    if (!count && !select.Contains(" ValorICMS"))
                        select += "DocumentoFaturamento.DFA_VALOR_ICMS ValorICMS, ";
                    if (!groupBy.Contains("ValorICMS"))
                        groupBy += "DocumentoFaturamento.DFA_VALOR_ICMS, ";
                    break;
                case "ValorLiquidoDocumento":
                    if (!count && !select.Contains(" ValorLiquidoDocumento"))
                        select += "DocumentoFaturamento.DFA_VALOR_LIQUIDO ValorLiquidoDocumento, ";
                    if (!groupBy.Contains("ValorLiquidoDocumento"))
                        groupBy += "DocumentoFaturamento.DFA_VALOR_LIQUIDO, ";
                    break;
                case "ValorBrutoDocumento":
                    if (!count && !select.Contains(" ValorBrutoDocumento"))
                        select += "DocumentoFaturamento.DFA_VALOR_DOCUMENTO ValorBrutoDocumento, ";
                    if (!groupBy.Contains("ValorBrutoDocumento"))
                        groupBy += "DocumentoFaturamento.DFA_VALOR_DOCUMENTO, ";
                    break;
                case "ValorDocumentoEmTitulo":
                    if (!count && !select.Contains(" ValorDocumentoEmTitulo"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0 WHEN Titulo.TBN_CODIGO is not null THEN 0 ELSE TituloDocumento.TDO_VALOR END) ValorDocumentoEmTitulo, ";
                    break;
                case "ValorTotalTitulo":
                    if (!count && !select.Contains(" ValorTotalTitulo"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	ELSE TituloDocumento.TDO_VALOR_PENDENTE + TDO_VALOR_PAGO + TDO_VALOR_PAGO_ACRESCIMO END) ValorTotalTitulo, ";
                    break;
                case "ValorPago":
                    if (!count && !select.Contains(" ValorPago"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0 WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_PAGO END) ValorPago, ";
                    break;
                case "ValorBaixadoDocumento":
                    if (!count && !select.Contains(" ValorBaixadoDocumento"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_PAGO_DOCUMENTO END) ValorBaixadoDocumento, ";
                    break;
                case "ValorBaixadoAcrescimo":
                    if (!count && !select.Contains(" ValorBaixadoAcrescimo"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_PAGO_ACRESCIMO END) ValorBaixadoAcrescimo, ";
                    break;
                case "ValorPendentePagamento":
                    if (!count && !select.Contains(" ValorPendentePagamento"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0 WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_PENDENTE END) ValorPendentePagamento, ";
                    break;
                case "ValorAFaturar":
                    if (!count && !select.Contains(" ValorAFaturar"))
                        select += "(DocumentoFaturamento.DFA_VALOR_DOCUMENTO - SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0 WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0 WHEN Titulo.TBN_CODIGO is not null THEN 0 ELSE TituloDocumento.TDO_VALOR END)) ValorAFaturar, ";
                    break;
                case "ValorAcrescimoGeracao":
                    if (!count && !select.Contains(" ValorAcrescimoGeracao"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	ELSE TituloDocumento.TDO_VALOR_ACRESCIMO END) ValorAcrescimoGeracao, ";
                    break;
                case "ValorDescontoGeracao":
                    if (!count && !select.Contains(" ValorDescontoGeracao"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	ELSE TituloDocumento.TDO_VALOR_DESCONTO END) ValorDescontoGeracao, ";
                    break;
                case "ValorAcrescimoBaixa":
                    if (!count && !select.Contains(" ValorAcrescimoBaixa"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0 WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_ACRESCIMO_BAIXA END) ValorAcrescimoBaixa, ";
                    break;
                case "ValorDescontoBaixa":
                    if (!count && !select.Contains(" ValorDescontoBaixa"))
                        select += "SUM(CASE WHEN Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao THEN 0 WHEN Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao THEN 0	WHEN Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao THEN 0 ELSE TituloDocumento.TDO_VALOR_DESCONTO_BAIXA END) ValorDescontoBaixa, ";
                    break;
            }
        }

        private SQLDinamico ObterSelectConsultaRelatorioDocumentoFaturamento(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            var parametros = new List<ParametroSQL>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaDocumentos(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

            SetarWhereRelatorioConsultaDocumentos(ref where, ref groupBy, ref joins, ref parametros, count, propriedades, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaDocumentos(propAgrupa, 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return new SQLDinamico((count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;"), parametros);
        }

        private void SetarWhereRelatorioConsultaDocumentos(ref string where, ref string groupBy, ref string joins, ref List<ParametroSQL> parametros, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            string pattern = "yyyy-MM-dd";

            where += " and DocumentoFaturamento.DFA_TIPO_LIQUIDACAO = 0 ";

            if (filtrosPesquisa.ValorInicial > 0m)
                where += " and DocumentoFaturamento.DFA_VALOR_DOCUMENTO >= " + filtrosPesquisa.ValorInicial.ToString("F2", cultura);

            if (filtrosPesquisa.ValorFinal > 0m)
                where += " and DocumentoFaturamento.DFA_VALOR_DOCUMENTO <= " + filtrosPesquisa.ValorFinal.ToString("F2", cultura);

            if (filtrosPesquisa.Situacao.HasValue)
                where += " and DocumentoFaturamento.DFA_SITUACAO = " + filtrosPesquisa.Situacao.Value.ToString("D");

            if (filtrosPesquisa.NumeroInicial > 0)
                where += " and DocumentoFaturamento.DFA_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString();

            if (filtrosPesquisa.TipoOcorrencia.Count > 0)
            {
                where += ($"AND TipoDeOcorrenciaDeCTe.OCO_CODIGO in ({string.Join(",", filtrosPesquisa.TipoOcorrencia)})");

                if (!joins.Contains(" CargaOcorrenciaPagamento "))
                    joins += " LEFT OUTER JOIN T_CARGA_OCORRENCIA CargaOcorrenciaPagamento ON CargaOcorrenciaPagamento.COC_CODIGO = DocumentoFaturamento.COC_CODIGO_PAGAMENTO ";

                if (!joins.Contains(" TipoDeOcorrenciaDeCTe "))
                    joins += " LEFT OUTER JOIN T_OCORRENCIA TipoDeOcorrenciaDeCTe ON TipoDeOcorrenciaDeCTe.OCO_CODIGO = CargaOcorrenciaPagamento.OCO_CODIGO ";
            }

            if (filtrosPesquisa.NumeroFinal > 0)
                where += " and DocumentoFaturamento.DFA_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.DataAutorizacaoInicial != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_AUTORIZACAO >= '" + filtrosPesquisa.DataAutorizacaoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataAutorizacaoFinal != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_AUTORIZACAO < '" + filtrosPesquisa.DataAutorizacaoFinal.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.DataAnulacaoInicial != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_ANULACAO >= '" + filtrosPesquisa.DataAnulacaoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataAnulacaoFinal != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_ANULACAO < '" + filtrosPesquisa.DataAnulacaoFinal.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.DataCancelamentoInicial != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_CANCELAMENTO >= '" + filtrosPesquisa.DataCancelamentoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataCancelamentoFinal != DateTime.MinValue)
                where += " and DocumentoFaturamento.DFA_DATA_CANCELAMENTO < '" + filtrosPesquisa.DataCancelamentoFinal.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += " and DocumentoFaturamento.EMP_CODIGO = " + filtrosPesquisa.CodigoTransportador.ToString();

            if (filtrosPesquisa.ModeloDocumento > 0)
                where += " and DocumentoFaturamento.MOD_CODIGO = " + filtrosPesquisa.ModeloDocumento.ToString();

            if (filtrosPesquisa.CodigoFilial > 0)
                where += " and DocumentoFaturamento.FIL_CODIGO = " + filtrosPesquisa.CodigoFilial.ToString();

            if (filtrosPesquisa.CodigoOrigem > 0)
                where += " and DocumentoFaturamento.LOC_ORIGEM = " + filtrosPesquisa.CodigoOrigem.ToString();

            if (filtrosPesquisa.CodigoDestino > 0)
                where += " and DocumentoFaturamento.LOC_DESTINO = " + filtrosPesquisa.CodigoDestino.ToString();

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                where += " and DocumentoFaturamento.CLI_CODIGO_REMETENTE = " + filtrosPesquisa.CpfCnpjRemetente.ToString("F0");

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                where += " and DocumentoFaturamento.CLI_CODIGO_DESTINATARIO = " + filtrosPesquisa.CpfCnpjDestinatario.ToString("F0");

            if (filtrosPesquisa.CpfCnpjTomador != null && filtrosPesquisa.CpfCnpjTomador.Count > 0d)
                where += (" and DocumentoFaturamento.CLI_CODIGO_TOMADOR in (" + string.Join(",", filtrosPesquisa.CpfCnpjTomador) + ")");

            if (filtrosPesquisa.GruposPessoas != null && filtrosPesquisa.GruposPessoas.Count > 0)
                where += (" and DocumentoFaturamento.GRP_CODIGO in (" + string.Join(",", filtrosPesquisa.GruposPessoas) + ")");

            if (filtrosPesquisa.GruposPessoasDiferente != null && filtrosPesquisa.GruposPessoasDiferente.Count > 0)
                where += ("  and (DocumentoFaturamento.GRP_CODIGO not in (" + string.Join(",", filtrosPesquisa.GruposPessoasDiferente) + ") or DocumentoFaturamento.GRP_CODIGO IS NULL)");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem) && filtrosPesquisa.EstadoOrigem != "0")
            {
                where += " and Origem.UF_SIGLA = '" + filtrosPesquisa.EstadoOrigem + "'";

                if (!joins.Contains(" Origem "))
                    joins += "left outer join T_LOCALIDADES Origem on DocumentoFaturamento.LOC_ORIGEM = Origem.LOC_CODIGO ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino) && filtrosPesquisa.EstadoDestino != "0")
            {
                where += " and Destino.UF_SIGLA = '" + filtrosPesquisa.EstadoDestino + "'";

                if (!joins.Contains(" Destino "))
                    joins += "left outer join T_LOCALIDADES Destino on DocumentoFaturamento.LOC_DESTINO = Destino.LOC_CODIGO ";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where += $@" and DocumentoFaturamento.DFA_CODIGO in (select VeiculoDocumento.DFA_CODIGO from T_DOCUMENTO_FATURAMENTO_VEICULO VeiculoDocumento 
                                                    join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoDocumento.VEI_CODIGO
                                                    where Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
            {
                where += " and exists (select dfa_codigo from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO where DFA_NUMERO_PEDIDO like :DFA_NUMERO_PEDIDO and DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO) ";
                parametros.Add(new ParametroSQL("DFA_NUMERO_PEDIDO", $"%{filtrosPesquisa.NumeroPedidoCliente}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrenciaCliente))
            {
                where += " and exists (select dfa_codigo from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA where DFA_NUMERO_PEDIDO_OCORRENCIA like :DFA_NUMERO_PEDIDO_OCORRENCIA and DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO) ";
                parametros.Add(new ParametroSQL("DFA_NUMERO_PEDIDO_OCORRENCIA", $"%{filtrosPesquisa.NumeroOcorrenciaCliente}%"));
            }

            if (filtrosPesquisa.NumeroOcorrencia > 0)
                where += " and DocumentoFaturamento.DFA_NUMERO_OCORRENCIA = " + filtrosPesquisa.NumeroOcorrencia.ToString() + " ";

            if (filtrosPesquisa.NumeroDocumentoOriginario > 0)
                where += " and DocumentoFaturamento.DFA_NUMERO_DOCUMENTO_ORIGINARIO = " + filtrosPesquisa.NumeroDocumentoOriginario.ToString() + " ";

            if (filtrosPesquisa.TipoFaturamento != null && filtrosPesquisa.TipoFaturamento.Count() > 0)
            {
                where += " and ";

                if (filtrosPesquisa.TipoFaturamento.Count() > 1)
                    where += "(";

                if (filtrosPesquisa.TipoFaturamento.Contains(TipoFaturamentoRelatorioDocumentoFaturamento.EmFatura))
                    where += "exists (SELECT DFA_CODIGO FROM T_FATURA_DOCUMENTO FaturaDocumento INNER JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO WHERE Fatura.FAT_SITUACAO = 1 AND FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO) OR ";

                if (filtrosPesquisa.TipoFaturamento.Contains(TipoFaturamentoRelatorioDocumentoFaturamento.Faturado))
                    where += " (  EXISTS (  SELECT 1 FROM T_TITULO_DOCUMENTO titDocumento  WHERE DocumentoFaturamento.CON_CODIGO IS NOT NULL and titDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR EXISTS (  SELECT 1  FROM T_TITULO_DOCUMENTO titDocumento  WHERE DocumentoFaturamento.CAR_CODIGO IS NOT NULL and titDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO) ) OR ";


                if (filtrosPesquisa.TipoFaturamento.Contains(TipoFaturamentoRelatorioDocumentoFaturamento.NaoFaturado))
                    where += "DocumentoFaturamento.DFA_VALOR_A_FATURAR > 0 OR ";

                where = where.Remove(where.Length - 4, 4);

                if (filtrosPesquisa.TipoFaturamento.Count() > 1)
                    where += ")";
            }

            if (filtrosPesquisa.TipoLiquidacao.HasValue)
            {
                if (filtrosPesquisa.TipoLiquidacao == TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente)
                    where += " and DocumentoFaturamento.DFA_VALOR_DOCUMENTO <> (DocumentoFaturamento.DFA_VALOR_PAGO - DocumentoFaturamento.DFA_VALOR_ACRESCIMO + DocumentoFaturamento.DFA_VALOR_DESCONTO) ";
                else if (filtrosPesquisa.TipoLiquidacao == TipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado)
                    where += " and DocumentoFaturamento.DFA_VALOR_DOCUMENTO = (DocumentoFaturamento.DFA_VALOR_PAGO - DocumentoFaturamento.DFA_VALOR_ACRESCIMO + DocumentoFaturamento.DFA_VALOR_DESCONTO) ";

            }

            if (filtrosPesquisa.NumeroFatura > 0)
                where += $@" and DocumentoFaturamento.DFA_CODIGO in (select FaturaDocumento.DFA_CODIGO from T_FATURA_DOCUMENTO FaturaDocumento 
                                                    join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                                                    where Fatura.FAT_NUMERO = {filtrosPesquisa.NumeroFatura}) ";

            if (filtrosPesquisa.DocumentoComCanhotosRecebidos.HasValue)
            {
                if (filtrosPesquisa.DocumentoComCanhotosRecebidos.Value)
                    where += $" and DocumentoFaturamento.DFA_CANHOTOS_RECEBIDOS = 1 ";
                else
                    where += $" and (DocumentoFaturamento.DFA_CANHOTOS_RECEBIDOS is null or DocumentoFaturamento.DFA_CANHOTOS_RECEBIDOS = 0) ";
            }

            if (filtrosPesquisa.DocumentoComCanhotosDigitalizados.HasValue)
            {
                if (filtrosPesquisa.DocumentoComCanhotosDigitalizados.Value)
                    where += $" and DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS = 1 ";
                else
                    where += $" and (DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS is null or DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS = 0) ";
            }

            if (filtrosPesquisa.TipoCTe.Count > 0)
            {
                where += $" and CTe.CON_TIPO_CTE in({string.Join(", ", filtrosPesquisa.TipoCTe)}) ";

                if (!joins.Contains(" CTe "))
                    joins += "left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";
            }

            if (filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue || filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue || filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue || filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue)
            {
                where += @" AND EXISTS( 
                                    SELECT TIT_DATA_LIQUIDACAO 
                                    FROM T_TITULO_DOCUMENTO TituloDocumento 
                                    INNER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO 
                                    WHERE 
                                        TituloDocumento.TDO_TIPO_DOCUMENTO = 1
                                    AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";

                where += filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString(pattern)}' " : string.Empty;

                where += "          UNION ALL ";

                where += @"         SELECT TIT_DATA_LIQUIDACAO 
                                    FROM T_TITULO_DOCUMENTO TituloDocumento 
                                    INNER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO 
                                    WHERE 
                                        TituloDocumento.TDO_TIPO_DOCUMENTO = 2
                                    AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO";

                where += filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString(pattern)}' " : string.Empty;
                where += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString(pattern)}' " : string.Empty;
                where += " ) ";
            }

            if (filtrosPesquisa.TipoServico.Count > 0)
            {
                where += $" and CTe.CON_TIPO_SERVICO in({string.Join(", ", filtrosPesquisa.TipoServico)}) ";

                if (!joins.Contains(" CTe "))
                    joins += "left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";
            }

            //if (filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue)
            //{
            //    where += $@" and EXISTIS( 
            //                        SELECT 
            //                            TIT_DATA_LIQUIDACAO 
            //                        FROM T_TITULO_DOCUMENTO TituloDocumento 
            //                            LEFT OUTER JOIN T_TITULO Titulo 
            //                                ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO 
            //                        WHERE (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR 
            //                            (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
            //                            AND CAST(TIT_DATA_LIQUIDACAO AS DATE) <= '{filtrosPesquisa.DataLiquidacaoFinal.ToString(pattern)}') ";
            //}
        }

        private void SetarSelectRelatorioConsultaDocumentos(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Filial":
                    if (!select.Contains(" Filial"))
                    {
                        select += "Filial.FIL_DESCRICAO Filial, ";
                        groupBy += "Filial.FIL_DESCRICAO, ";
                        if (!joins.Contains(" Filial "))
                            joins += " left outer join T_FILIAL Filial on DocumentoFaturamento.FIL_CODIGO = Filial.FIL_CODIGO ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero,"))
                    {
                        select += "DocumentoFaturamento.DFA_NUMERO Numero, ";
                        groupBy += "DocumentoFaturamento.DFA_NUMERO, ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga"))
                    {
                        select += "DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga, ";
                        groupBy += "DocumentoFaturamento.DFA_NUMERO_CARGA, ";
                    }
                    break;
                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia,"))
                    {
                        select += "DocumentoFaturamento.DFA_NUMERO_OCORRENCIA NumeroOcorrencia, ";
                        groupBy += "DocumentoFaturamento.DFA_NUMERO_OCORRENCIA, ";
                    }
                    break;
                case "NumeroDocumentoOriginario":
                    if (!select.Contains(" NumeroDocumentoOriginario,"))
                    {
                        select += "DocumentoFaturamento.DFA_NUMERO_DOCUMENTO_ORIGINARIO NumeroDocumentoOriginario, ";
                        groupBy += "DocumentoFaturamento.DFA_NUMERO_DOCUMENTO_ORIGINARIO, ";
                    }
                    break;
                case "CanhotosRecebidos":
                    if (!select.Contains(" CanhotosRecebidos,"))
                    {
                        select += "CASE WHEN DocumentoFaturamento.DFA_CANHOTOS_RECEBIDOS = 1 THEN 'Sim' ELSE 'Não' END CanhotosRecebidos, ";
                        groupBy += "DocumentoFaturamento.DFA_CANHOTOS_RECEBIDOS, ";
                    }
                    break;
                case "CanhotosDigitalizados":
                    if (!select.Contains(" CanhotosDigitalizados,"))
                    {
                        select += "CASE WHEN DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS = 1 THEN 'Sim' ELSE 'Não' END CanhotosDigitalizados, ";
                        groupBy += "DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS, ";
                    }
                    break;
                case "DataEmissaoDocumentoOriginario":
                    if (!select.Contains(" DataEmissaoDocumentoOriginario,"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_EMISSAO_DOCUMENTO_ORIGINARIO, 103) DataEmissaoDocumentoOriginario, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_EMISSAO_DOCUMENTO_ORIGINARIO, ";
                    }
                    break;
                case "Serie":
                    if (!select.Contains(" Serie"))
                    {
                        select += "EmpresaSerie.ESE_NUMERO Serie, ";
                        groupBy += "EmpresaSerie.ESE_NUMERO, ";

                        if (!joins.Contains(" EmpresaSerie "))
                            joins += " left outer join T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO ";
                    }
                    break;
                case "CNPJEmpresaFormatado":
                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresa,"))
                    {
                        select += "Empresa.EMP_CNPJ CNPJEmpresa, ";
                        groupBy += "Empresa.EMP_CNPJ, ";

                        if (!joins.Contains(" Empresa "))
                            joins += " left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoFaturamento.EMP_CODIGO ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa,"))
                    {
                        select += "Empresa.EMP_RAZAO Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, ";

                        if (!joins.Contains(" Empresa "))
                            joins += " left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoFaturamento.EMP_CODIGO ";
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select += "Tomador.CLI_NOME Tomador, ";
                        groupBy += "Tomador.CLI_NOME, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left outer join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR ";
                    }
                    break;
                case "CPFCNPJTomadorFormatado":
                case "CNPJTomador":
                    if (!select.Contains(" CNPJTomador"))
                    {
                        select += "Tomador.CLI_CGCCPF CNPJTomador, Tomador.CLI_FISJUR TipoTomador, ";
                        groupBy += "Tomador.CLI_CGCCPF, Tomador.CLI_FISJUR, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left outer join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR ";
                    }
                    break;
                case "GrupoTomador":
                    if (!select.Contains(" GrupoTomador"))
                    {
                        select += "GrupoTomador.GRP_DESCRICAO GrupoTomador, ";
                        groupBy += "GrupoTomador.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoTomador "))
                            joins += " left outer join T_GRUPO_PESSOAS GrupoTomador on DocumentoFaturamento.GRP_CODIGO = GrupoTomador.GRP_CODIGO ";
                    }
                    break;
                case "CidadeTomador":
                    if (!select.Contains(" GrupoTomador"))
                    {
                        select += "LocalidadeTomador.LOC_DESCRICAO CidadeTomador, ";
                        groupBy += "LocalidadeTomador.LOC_DESCRICAO, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left outer join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR ";

                        if (!joins.Contains(" LocalidadeTomador "))
                            joins += " left outer join T_LOCALIDADES LocalidadeTomador on Tomador.LOC_CODIGO = LocalidadeTomador.LOC_CODIGO ";
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente"))
                    {
                        select += "Remetente.CLI_NOME Remetente, ";
                        groupBy += "Remetente.CLI_NOME, ";

                        if (!joins.Contains(" Remetente "))
                            joins += " left outer join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE ";
                    }
                    break;
                case "CPFCNPJRemetenteFormatado":
                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente"))
                    {
                        select += "Remetente.CLI_CGCCPF CNPJRemetente, Remetente.CLI_FISJUR TipoRemetente, ";
                        groupBy += "Remetente.CLI_CGCCPF, Remetente.CLI_FISJUR, ";

                        if (!joins.Contains(" Remetente "))
                            joins += " left outer join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario"))
                    {
                        select += "Destinatario.CLI_NOME Destinatario, ";
                        groupBy += "Destinatario.CLI_NOME, ";

                        if (!joins.Contains(" Destinatario "))
                            joins += " left outer join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO ";
                    }
                    break;
                case "CPFCNPJDestinatarioFormatado":
                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario"))
                    {
                        select += "Destinatario.CLI_CGCCPF CNPJDestinatario, Destinatario.CLI_FISJUR TipoDestinatario, ";
                        groupBy += "Destinatario.CLI_CGCCPF, Destinatario.CLI_FISJUR, ";

                        if (!joins.Contains(" Destinatario "))
                            joins += " left outer join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO ";
                    }
                    break;
                case "Expedidor":
                    if (!select.Contains(" Expedidor"))
                    {
                        select += "Expedidor.CLI_NOME Expedidor, ";
                        groupBy += "Expedidor.CLI_NOME, ";

                        if (!joins.Contains(" Expedidor "))
                            joins += " left outer join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_EXPEDIDOR ";
                    }
                    break;
                case "CPFCNPJExpedidorFormatado":
                case "CNPJExpedidor":
                    if (!select.Contains(" CNPJExpedidor"))
                    {
                        select += "Expedidor.CLI_CGCCPF CNPJExpedidor, Expedidor.CLI_FISJUR TipoExpedidor, ";
                        groupBy += "Expedidor.CLI_CGCCPF, Expedidor.CLI_FISJUR, ";

                        if (!joins.Contains(" Expedidor "))
                            joins += " left outer join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_EXPEDIDOR ";
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor"))
                    {
                        select += "Recebedor.CLI_NOME Recebedor, ";
                        groupBy += "Recebedor.CLI_NOME, ";

                        if (!joins.Contains(" Recebedor "))
                            joins += " left outer join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_RECEBEDOR ";
                    }
                    break;
                case "CPFCNPJRecebedorFormatado":
                case "CNPJRecebedor":
                    if (!select.Contains(" CNPJRecebedor"))
                    {
                        select += "Recebedor.CLI_CGCCPF CNPJRecebedor, Recebedor.CLI_FISJUR TipoRecebedor, ";
                        groupBy += "Recebedor.CLI_CGCCPF, Recebedor.CLI_FISJUR, ";

                        if (!joins.Contains(" Recebedor "))
                            joins += " left outer join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_RECEBEDOR ";
                    }
                    break;
                case "Frotas":
                    if (!select.Contains(" Frotas"))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_NUMERO_FROTA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Frotas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "Faturas":
                    if (!select.Contains(" Faturas"))
                    {
                        select += "substring((select ', ' + convert(nvarchar(15), (CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END)) from T_FATURA_DOCUMENTO FaturaDocumento inner join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO where Fatura.FAT_SITUACAO <> 3 AND FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Faturas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "VencimentosFaturas":
                    if (!select.Contains(" VencimentosFaturas"))
                    {
                        select += @"ISNULL(substring(
                                                   (SELECT ', ' + convert(nvarchar(15), FaturaParcela.FAP_DATA_VENCIMENTO, 3)
                                                    FROM T_FATURA_DOCUMENTO FaturaDocumento
                                                    INNER JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                                                    INNER JOIN T_FATURA_PARCELA FaturaParcela ON FaturaParcela.FAT_CODIGO = Fatura.FAT_CODIGO
                                                    WHERE Fatura.FAT_SITUACAO <> 3
                                                      AND FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO
                                                    GROUP BY FaturaParcela.FAP_DATA_VENCIMENTO
                                                    ORDER BY FaturaParcela.FAP_DATA_VENCIMENTO
                                                    FOR XML path('')), 3, 1000), 
			                                substring(
                                                   (SELECT ', ' + convert(nvarchar(15), Titulo.TIT_DATA_VENCIMENTO, 3)
                                                    FROM T_TITULO_DOCUMENTO TituloDocumento
                                                    INNER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO                    
                                                    WHERE Titulo.TIT_STATUS <> 4
                                                      AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                                      AND Titulo.TIT_TIPO = 1
                                                    GROUP BY Titulo.TIT_DATA_VENCIMENTO
                                                    ORDER BY Titulo.TIT_DATA_VENCIMENTO
                                                    FOR XML path('')), 3, 1000)) VencimentosFaturas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";
                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia"))
                    {
                        select += @"TipoDeOcorrenciaDeCTe.OCO_DESCRICAO TipoOcorrencia,";

                        if (!joins.Contains(" CargaOcorrenciaPagamento "))
                            joins += " LEFT OUTER JOIN T_CARGA_OCORRENCIA CargaOcorrenciaPagamento ON CargaOcorrenciaPagamento.COC_CODIGO = DocumentoFaturamento.COC_CODIGO_PAGAMENTO ";

                        if (!joins.Contains(" TipoDeOcorrenciaDeCTe "))
                            joins += " LEFT OUTER JOIN T_OCORRENCIA TipoDeOcorrenciaDeCTe ON TipoDeOcorrenciaDeCTe.OCO_CODIGO = CargaOcorrenciaPagamento.OCO_CODIGO ";

                        if (!groupBy.Contains("TipoDeOcorrenciaDeCTe.OCO_DESCRICAO"))
                            groupBy += "TipoDeOcorrenciaDeCTe.OCO_DESCRICAO, ";
                    }
                    break;

                case "EmissoesFaturas":
                    if (!select.Contains(" EmissoesFaturas"))
                    {
                        select += @"ISNULL(substring(
                                                   (SELECT ', ' + convert(nvarchar(15), FaturaParcela.FAP_DATA_EMISSAO, 3)
                                                    FROM T_FATURA_DOCUMENTO FaturaDocumento
                                                    INNER JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                                                    INNER JOIN T_FATURA_PARCELA FaturaParcela ON FaturaParcela.FAT_CODIGO = Fatura.FAT_CODIGO
                                                    WHERE Fatura.FAT_SITUACAO <> 3
                                                      AND FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO
                                                    GROUP BY FaturaParcela.FAP_DATA_EMISSAO
                                                    ORDER BY FaturaParcela.FAP_DATA_EMISSAO
                                                    FOR XML path('')), 3, 1000), 
			                                substring(
                                                   (SELECT ', ' + convert(nvarchar(15), Titulo.TIT_DATA_EMISSAO, 3)
                                                    FROM T_TITULO_DOCUMENTO TituloDocumento
                                                    INNER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO                    
                                                    WHERE Titulo.TIT_STATUS <> 4
                                                      AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                                      AND Titulo.TIT_TIPO = 1
                                                    GROUP BY Titulo.TIT_DATA_EMISSAO
                                                    ORDER BY Titulo.TIT_DATA_EMISSAO
                                                    FOR XML path('')), 3, 1000)) EmissoesFaturas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";
                    }
                    break;
                case "Placas":
                    if (!select.Contains(" Placas"))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Placas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains(" Motoristas"))
                    {
                        select += "substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumento inner join T_FUNCIONARIO motorista1 on motoristaDocumento.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motoristas, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "NumeroPedidoCliente":
                    if (!select.Contains(" NumeroPedidoCliente"))
                    {
                        select += "substring((select ', ' + NumeroPedido.DFA_NUMERO_PEDIDO from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO NumeroPedido where NumeroPedido.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroPedidoCliente, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "NumeroOcorrenciaCliente":
                    if (!select.Contains(" NumeroOcorrenciaCliente"))
                    {
                        select += "substring((select ', ' + NumeroOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA NumeroOcorrencia where NumeroOcorrencia.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroOcorrenciaCliente, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao"))
                    {
                        select += "DocumentoFaturamento.DFA_SITUACAO Situacao, ";
                        groupBy += "DocumentoFaturamento.DFA_SITUACAO, ";
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao,"))
                    {
                        select += "DocumentoFaturamento.DFA_DATAHORAEMISSAO DataEmissao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATAHORAEMISSAO, ";
                    }
                    break;
                case "DataAutorizacao":
                    if (!select.Contains(" DataAutorizacao"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_AUTORIZACAO, 103) DataAutorizacao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_AUTORIZACAO, ";
                    }
                    break;
                case "DataCancelamento":
                    if (!select.Contains(" DataCancelamento"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_CANCELAMENTO, 103) DataCancelamento, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_CANCELAMENTO, ";
                    }
                    break;
                case "DataAnulacao":
                    if (!select.Contains(" DataAnulacao"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_ANULACAO, 103) DataAnulacao, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_ANULACAO, ";
                    }
                    break;
                case "DataEnvioUltimoCanhoto":
                    if (!select.Contains(" DataEnvioUltimoCanhoto"))
                    {
                        select += "CONVERT(nvarchar(20), DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, 103) DataEnvioUltimoCanhoto, ";
                        groupBy += "DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, ";
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select += " Origem.LOC_DESCRICAO + '-' + Origem.UF_SIGLA Origem, ";
                        groupBy += "Origem.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("Origem.UF_SIGLA"))
                            groupBy += "Origem.UF_SIGLA, ";

                        if (!joins.Contains(" Origem "))
                            joins += "left outer join T_LOCALIDADES Origem on DocumentoFaturamento.LOC_ORIGEM = Origem.LOC_CODIGO ";
                    }
                    break;
                case "UFOrigem":
                    if (!select.Contains("UFOrigem"))
                    {
                        select += " Origem.UF_SIGLA UFOrigem, ";

                        if (!groupBy.Contains("Origem.UF_SIGLA"))
                            groupBy += "Origem.UF_SIGLA, ";

                        if (!joins.Contains(" Origem "))
                            joins += "left outer join T_LOCALIDADES Origem on DocumentoFaturamento.LOC_ORIGEM = Origem.LOC_CODIGO ";
                    }
                    break;
                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select += " Destino.LOC_DESCRICAO + '-' + Destino.UF_SIGLA Destino, ";
                        groupBy += "Destino.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("Destino.UF_SIGLA"))
                            groupBy += "Destino.UF_SIGLA, ";

                        if (!joins.Contains(" Destino "))
                            joins += "left outer join T_LOCALIDADES Destino on DocumentoFaturamento.LOC_DESTINO = Destino.LOC_CODIGO ";
                    }
                    break;
                case "UFDestino":
                    if (!select.Contains("UFDestino"))
                    {
                        select += " Destino.UF_SIGLA UFDestino, ";

                        if (!groupBy.Contains("Destino.UF_SIGLA"))
                            groupBy += " Destino.UF_SIGLA, ";

                        if (!joins.Contains(" Destino "))
                            joins += "left outer join T_LOCALIDADES Destino on DocumentoFaturamento.LOC_DESTINO = Destino.LOC_CODIGO ";
                    }
                    break;
                case "DescricaoAbreviacao":
                    if (!select.Contains(" TipoDocumento"))
                    {
                        select += "DocumentoFaturamento.DFA_TIPO_DOCUMENTO TipoDocumento, ";
                        groupBy += "DocumentoFaturamento.DFA_TIPO_DOCUMENTO, ";
                    }
                    if (!select.Contains(" ModeloDocumento"))
                    {
                        select += "ModeloDocumento.MOD_ABREVIACAO AbreviacaoModeloDocumentoFiscal, ";
                        groupBy += "ModeloDocumento.MOD_ABREVIACAO, ";
                        joins += "left outer join T_MODDOCFISCAL ModeloDocumento on DocumentoFaturamento.MOD_CODIGO = ModeloDocumento.MOD_CODIGO ";
                    }
                    break;
                case "AliquotaICMS":
                    if (!select.Contains(" AliquotaICMS"))
                    {
                        select += "DocumentoFaturamento.DFA_ALIQUOTA_ICMS AliquotaICMS, ";
                        groupBy += "DocumentoFaturamento.DFA_ALIQUOTA_ICMS, ";
                    }
                    break;
                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS"))
                    {
                        select += "DocumentoFaturamento.DFA_ALIQUOTA_ISS AliquotaISS, ";
                        groupBy += "DocumentoFaturamento.DFA_ALIQUOTA_ISS, ";
                    }
                    break;
                case "PercentualRetencaoISS":
                    if (!select.Contains(" PercentualRetencaoISS"))
                    {
                        select += "DocumentoFaturamento.DFA_PERCENTUAL_RETENCAO_ISS PercentualRetencaoISS, ";
                        groupBy += "DocumentoFaturamento.DFA_PERCENTUAL_RETENCAO_ISS, ";
                    }
                    break;
                case "ValorDocumento":
                    if (!count && !select.Contains(" ValorDocumento"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_DOCUMENTO) ValorDocumento, ";
                    break;
                case "ValorAcrescimo":
                    if (!count && !select.Contains(" ValorAcrescimo"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_ACRESCIMO) ValorAcrescimo, ";
                    break;
                case "ValorDesconto":
                    if (!count && !select.Contains(" ValorDesconto"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_DESCONTO) ValorDesconto, ";
                    break;
                case "ValorPago":
                    if (!count && !select.Contains(" ValorPago"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_PAGO) ValorPago, ";
                    break;
                case "ValorEmFatura":
                    if (!count && !select.Contains(" ValorEmFatura"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_EM_FATURA) ValorEmFatura, ";
                    break;
                case "ValorAFaturar":
                    if (!count && !select.Contains(" ValorAFaturar"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_A_FATURAR) ValorAFaturar, ";
                    break;

                case "ValorICMS":
                    if (!count && !select.Contains(" ValorICMS"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_ICMS) ValorICMS, ";
                    break;
                case "ValorISS":
                    if (!count && !select.Contains(" ValorISS"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_ISS) ValorISS, ";
                    break;
                case "ValorImpostos":
                    if (!count && !select.Contains(" ValorImpostos"))
                        select += "SUM(DocumentoFaturamento.DFA_VALOR_ICMS + DocumentoFaturamento.DFA_VALOR_ISS) ValorImpostos, ";
                    break;
                case "CodigoDocumento":
                    if (!select.Contains(" CodigoDocumento"))
                    {
                        select += "DocumentoFaturamento.DFA_CODIGO CodigoDocumento, ";
                        groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao"))
                    {
                        select += "CTe.CON_OBSGERAIS Observacao, ";
                        groupBy += "CTe.CON_OBSGERAIS, ";
                        if (!joins.Contains(" CTe "))
                            joins += " left join T_CTE CTe on DocumentoFaturamento.CON_CODIGO = CTe.CON_CODIGO ";
                    }
                    break;
                case "ChaveAcesso":
                    if (!select.Contains("ChaveAcesso"))
                    {
                        select += "CTe.CON_CHAVECTE ChaveAcesso, ";
                        groupBy += "CTe.CON_CHAVECTE, ";
                        if (!joins.Contains(" CTe "))
                            joins += "left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";
                    }
                    break;

                case "NumeroTitulos":
                    if (!select.Contains(" NumeroTitulos, "))
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += "         SELECT ', ' + ";
                        select += "             CAST((CASE WHEN Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO IS NULL OR Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO = 0 THEN Titulo.TIT_CODIGO ELSE Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO END) AS VARCHAR(10)) ";
                        select += "         FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += "             LEFT OUTER JOIN T_TITULO Titulo ";
                        select += "                 ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += "         WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += "         FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') NumeroTitulos, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;
                case "StatusTitulos":
                    if (!select.Contains(" StatusTitulos, "))
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += " 		SELECT ', ' + ";
                        select += " 			CASE ";
                        select += " 				WHEN Titulo.TIT_STATUS = 1 THEN 'Em Aberto' ";
                        select += " 				WHEN Titulo.TIT_STATUS = 2 THEN 'Atrasado' ";
                        select += " 				WHEN Titulo.TIT_STATUS = 3 THEN 'Quitado' ";
                        select += " 				WHEN Titulo.TIT_STATUS = 4 THEN 'Cancelado' ";
                        select += " 				WHEN Titulo.TIT_STATUS = 5 THEN 'Em Negociação' ";
                        select += " 				WHEN Titulo.TIT_STATUS = 6 THEN 'Bloqueado' ";
                        select += " 				ELSE '' ";
                        select += " 			END ";
                        select += " 		FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += " 			LEFT OUTER JOIN T_TITULO Titulo ";
                        select += " 				ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += " 		WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += " 		FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') StatusTitulos, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;

                case "ValorPendente":
                    if (!select.Contains(" ValorPendente, "))
                    {
                        select += "((SUM(DocumentoFaturamento.DFA_VALOR_DOCUMENTO)) - ((SUM(DocumentoFaturamento.DFA_VALOR_PAGO)) + SUM(ISNULL(DocumentoFaturamento.DFA_VALOR_DESCONTO, 0)) - SUM(ISNULL(DocumentoFaturamento.DFA_VALOR_ACRESCIMO, 0)))) ValorPendente, ";
                    }
                    break;

                case "DataLiquidacao":
                    if (!select.Contains(" DataLiquidacao, ") && filtrosPesquisa.TipoLiquidacao != TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente)
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += " 		SELECT ', ' + ";
                        select += " 			CONVERT(NVARCHAR(20),TIT_DATA_LIQUIDACAO, 103) ";
                        select += " 		FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += " 			LEFT OUTER JOIN T_TITULO Titulo ";
                        select += " 				ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += " 		WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += " 		FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') DataLiquidacao, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains(" TipoCTe, "))
                    {
                        select += "CTe.CON_TIPO_CTE TipoCTe, ";
                        groupBy += "CTe.CON_TIPO_CTE, ";

                        if (!joins.Contains(" CTe "))
                            joins += " left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains(" TipoServico, "))
                    {
                        select += "CTe.CON_TIPO_SERVICO TipoServico, ";
                        groupBy += "CTe.CON_TIPO_SERVICO, ";

                        if (!joins.Contains(" CTe "))
                            joins += " left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ";
                    }
                    break;

                case "DataBaseLiquidacao":
                    if (!select.Contains(" DataBaseLiquidacao, ") && filtrosPesquisa.TipoLiquidacao != TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente)
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += " 		SELECT ', ' + ";
                        select += " 			CONVERT(NVARCHAR(20),TIT_DATA_BASE_LIQUIDACAO, 103) ";
                        select += " 		FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += " 			LEFT OUTER JOIN T_TITULO Titulo ";
                        select += " 				ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += " 		WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += " 		FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') DataBaseLiquidacao, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;
                case "ObservacaoFatura":
                    if (!select.Contains(" ObservacaoFatura"))
                    {
                        select += @"substring((select ', ' + Fatura.FAT_OBSERVACAO_FATURA from T_FATURA_DOCUMENTO FaturaDocumento 
                                    inner join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                                    where Fatura.FAT_SITUACAO <> 3
                                    AND FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) ObservacaoFatura, ";

                        if (!groupBy.Contains("DocumentoFaturamento.DFA_CODIGO"))
                            groupBy += "DocumentoFaturamento.DFA_CODIGO, ";
                    }
                    break;

                case "NumeroNotaFiscal":
                    if (!select.Contains(" NumeroNotaFiscal"))
                    {
                        select += @"substring((select DISTINCT ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 
			                        join T_CARGA_CTE _cargaCte on _cargaCte.CON_CODIGO = notaFiscal1.CON_CODIGO 
			                        left outer join T_CANHOTO_NOTA_FISCAL _canhoto ON _canhoto.CAR_CODIGO = _cargaCte.CAR_CODIGO 
			                        where _cargaCte.CON_CODIGO = DocumentoFaturamento.CON_CODIGO for xml path('')), 3, 100000) NumeroNotaFiscal, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";
                    }
                    break;

                case "NumeroDocumentoAnterior":
                    if (!select.Contains(" NumeroDocumentoAnterior"))
                    {
                        select += @"(select TOP 1 NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ORDER BY notaFiscal1.NFC_DATAEMISSAO) NumeroDocumentoAnterior, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";
                    }
                    break;

                case "ValoresPagos":
                    if (!select.Contains(" ValoresPagos, ") && filtrosPesquisa.TipoLiquidacao != TipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado)
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += " 		SELECT ', ' + ";
                        select += " 			CONVERT(NVARCHAR(20),TIT_VALOR_PAGO, 103) ";
                        select += " 		FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += " 			LEFT OUTER JOIN T_TITULO Titulo ";
                        select += " 				ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += " 		WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_LIQUIDACAO < '{filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO >= '{filtrosPesquisa.DataBaseLiquidacaoInicial.ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += filtrosPesquisa != null && filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue ? $" AND TIT_DATA_BASE_LIQUIDACAO < '{filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;
                        select += " 		FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') ValoresPagos, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;

                case "DataVencimentoTitulo":
                    if (!select.Contains(" DataVencimentoTitulo, "))
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += " 		SELECT ', ' + ";
                        select += " 			CONVERT(NVARCHAR(20),TIT_DATA_VENCIMENTO, 103) ";
                        select += " 		FROM T_TITULO_DOCUMENTO TituloDocumento ";
                        select += " 			LEFT OUTER JOIN T_TITULO Titulo ";
                        select += " 				ON Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO ";
                        select += " 		WHERE ((TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR ";
                        select += "             (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) ";
                        select += " 		FOR XML PATH('')), ";
                        select += "     3, 1000), ";
                        select += " '') DataVencimentoTitulo, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CON_CODIGO"))
                            groupBy += "DocumentoFaturamento.CON_CODIGO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.CAR_CODIGO"))
                            groupBy += "DocumentoFaturamento.CAR_CODIGO, ";
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao"))
                    {
                        select += "TipoOperacao.TOP_DESCRICAO TipoOperacao, ";

                        if (!groupBy.Contains("TipoOperacao.TOP_DESCRICAO"))
                            groupBy += "TipoOperacao.TOP_DESCRICAO, ";

                        if (!groupBy.Contains("DocumentoFaturamento.TOP_CODIGO"))
                            groupBy += "DocumentoFaturamento.TOP_CODIGO, ";

                        if (!joins.Contains(" TipoOperacao "))
                            joins += " left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = DocumentoFaturamento.TOP_CODIGO ";
                    }
                    break;
            }
        }

        private SQLDinamico QueryDocumentosFaturamentoParaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro)
        {
            var parametros = new List<ParametroSQL>();
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
            {
                sql = @"select
                        DocumentoFaturamento.DFA_CODIGO Codigo,
                        DocumentoFaturamento.DFA_NUMERO Numero,
                        DocumentoFaturamento.DFA_DATAHORAEMISSAO DataEmissao,
                        DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga,
                        Carga.CAR_CODIGO CodigoCarga,
                        DocumentoFaturamento.DFA_TIPO_DOCUMENTO TipoDocumento,
                        ISNULL(DocumentoFaturamento.DFA_MOEDA, -1) Moeda,
                        DocumentoFaturamento.DFA_VALOR_DOCUMENTO ValorDocumento,
                        DocumentoFaturamento.DFA_VALOR_A_FATURAR ValorAFaturar,
                        EmpresaSerie.ESE_NUMERO Serie,
                        (Origem.LOC_DESCRICAO + ' - ' + Origem.UF_SIGLA) Origem,
                        (Destino.LOC_DESCRICAO + ' - ' + Destino.UF_SIGLA) Destino,
                        ModeloDocumentoFiscal.MOD_ABREVIACAO AbreviacaoModeloDocumentoFiscal,
                        ISNULL(CTe.CON_TIPO_CTE, -1) TipoCTE,
                        SUBSTRING((SELECT DISTINCT ', ' + funcionario.FUN_NOME
                                        from T_DOCUMENTO_FATURAMENTO_MOTORISTA documentoFaturamentoMotorista
                                        inner join T_FUNCIONARIO funcionario on funcionario.FUN_CODIGO = documentoFaturamentoMotorista.FUN_CODIGO
                                 WHERE documentoFaturamentoMotorista.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motorista,
                        SUBSTRING((SELECT DISTINCT ', ' + CAST(notaFiscal.NF_NUMERO as varchar)
                                        from T_XML_NOTA_FISCAL notaFiscal
                                        inner join T_CTE_XML_NOTAS_FISCAIS cteNotaFiscal on cteNotaFiscal.NFX_CODIGO = notaFiscal.NFX_CODIGO
                                 WHERE cteNotaFiscal.CON_CODIGO = cte.CON_CODIGO for xml path('')), 3, 1000) NotaFiscal";

                if (configFinanceiro.AtivarColunaCSTConsultaDocumentosFatura)
                {
                    sql += @", CASE WHEN ModeloDocumentoFiscal.MOD_NUM = '57' THEN CTe.CON_CST ELSE '' END CST";
                }

                if (configFinanceiro.AtivarColunaNumeroContainerConsultaDocumentosFatura)
                {
                    sql += @", SUBSTRING((SELECT DISTINCT ', ' + container.CTR_NUMERO
                            from T_CTE_CONTAINER cteContainer
                            JOIN T_CONTAINER container on container.CTR_CODIGO = cteContainer.CTR_CODIGO                                        
                            WHERE DocumentoFaturamento.CON_CODIGO = cteContainer.CON_CODIGO for xml path('')), 3, 1000) Container";
                }
            }

            sql += @"   from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                        join T_LOCALIDADES Origem on Origem.LOC_CODIGO = DocumentoFaturamento.LOC_ORIGEM
                        join T_LOCALIDADES Destino on Destino.LOC_CODIGO = DocumentoFaturamento.LOC_DESTINO
                        left join T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                        left join T_CTE_CONTAINER CteContainer on CteContainer.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                        left join T_CONTAINER Container on Container.CTR_CODIGO = CteContainer.CTR_CODIGO   
                        left join T_CTE_PARTICIPANTE TomadorPagador on TomadorPagador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                        left join T_LOCALIDADES LocalidadeInicioPrestacao on LocalidadeInicioPrestacao.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO
                        left join T_CARGA Carga on Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO
                        left join T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO
                        left join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = DocumentoFaturamento.MOD_CODIGO
                        left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR";

            string pattern = "yyyy-MM-dd";

            SituacaoCarga[] situacoesCarga = new SituacaoCarga[]
            {
                  SituacaoCarga.EmTransporte,
                  SituacaoCarga.Encerrada
            };

            sql += $" where DocumentoFaturamento.DFA_SITUACAO = 1 and (CTe.CON_STATUS = 'A' or Carga.CAR_SITUACAO in ({string.Join(", ", situacoesCarga.Select(o => o.ToString("d")))})) and DocumentoFaturamento.DFA_VALOR_A_FATURAR > 0 and DFA_TIPO_LIQUIDACAO = 0";
            sql += $" and DocumentoFaturamento.DFA_CODIGO not in (select faturaDocumento.DFA_CODIGO from T_FATURA_DOCUMENTO faturaDocumento where faturaDocumento.FAT_CODIGO = {filtrosPesquisa.CodigoFatura})"; // SQL-INJECTION-SAFE
            sql += @" and not exists (select cargaCTe.CCT_CODIGO from T_CARGA_CTE cargaCTe 
                                        inner join T_CARGA carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO 
                                        where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and carga.CAR_CARGA_SVM = 1)";

            if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value > DateTime.MinValue && filtrosPesquisa.DataFinal.HasValue && filtrosPesquisa.DataFinal.Value > DateTime.MinValue)
                sql += $@" and ((Carga.CAR_DATA_FINALIZACAO_EMISSAO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' and Carga.CAR_DATA_FINALIZACAO_EMISSAO < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern)}') 
                                or (CTe.CON_DATAHORAEMISSAO) >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern)}')";
            else if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value > DateTime.MinValue)
                sql += $" and (Carga.CAR_DATA_FINALIZACAO_EMISSAO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' or CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}')";
            else if (filtrosPesquisa.DataFinal.HasValue && filtrosPesquisa.DataFinal.Value > DateTime.MinValue)
                sql += $" and (Carga.CAR_DATA_FINALIZACAO_EMISSAO < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern)}' or CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString(pattern)}')";

            if (filtrosPesquisa.Notas > 0)
            {
                sql += $@" and exists (select XmlNotaFiscal.NF_NUMERO from T_CTE_XML_NOTAS_FISCAIS CteNotaFiscal
                                        left join T_XML_NOTA_FISCAL XmlNotaFiscal  on XmlNotaFiscal.NFX_CODIGO = CteNotaFiscal.NFX_CODIGO
                                        where CteNotaFiscal.CON_CODIGO = CTe.CON_CODIGO and XmlNotaFiscal.NF_NUMERO = {filtrosPesquisa.Notas})";
            }

            if (filtrosPesquisa.CPFCNPJTomador > 0d)
                sql += $" and Tomador.CLI_CGCCPF = {filtrosPesquisa.CPFCNPJTomador}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IETomador))
                sql += $" and TomadorPagador.PCT_IERG = '{filtrosPesquisa.IETomador}'";

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                sql += $" and (Tomador.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas} or DocumentoFaturamento.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas})";

            if (filtrosPesquisa.CodigoCarga > 0)
                sql += $" and (Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga} or CTe.CON_CODIGO in (select cargaCTe.CON_CODIGO from T_CARGA_CTE cargaCTe where cargaCTe.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}))"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoOperacao > 0)
                sql += $" and DocumentoFaturamento.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}";

            if (filtrosPesquisa.Empresa > 0)
                sql += $" and DocumentoFaturamento.EMP_CODIGO = {filtrosPesquisa.Empresa}";

            if (filtrosPesquisa.TipoCarga > 0)
                sql += $" and DocumentoFaturamento.TCG_CODIGO = {filtrosPesquisa.TipoCarga}";

            if (filtrosPesquisa.AliquotaICMS.HasValue)
                sql += $" and DocumentoFaturamento.DFA_ALIQUOTA_ICMS = {filtrosPesquisa.AliquotaICMS.Value}";

            if (filtrosPesquisa.Filial > 0)
                sql += $" and DocumentoFaturamento.FIL_CODIGO = {filtrosPesquisa.Filial}";

            if (filtrosPesquisa.PedidoViagemNavio > 0)
                sql += $" and CTe.CON_VIAGEM = {filtrosPesquisa.PedidoViagemNavio}";

            if (filtrosPesquisa.TerminalDestino > 0)
                sql += $" and CTe.CON_TERMINAL_DESTINO = {filtrosPesquisa.TerminalDestino}";

            if (filtrosPesquisa.TerminalOrigem > 0)
                sql += $" and CTe.CON_TERMINAL_ORIGEM = {filtrosPesquisa.TerminalOrigem}";

            if (filtrosPesquisa.Origem > 0)
                sql += $" and CTe.CON_LOCINICIOPRESTACAO = {filtrosPesquisa.Origem}";

            if (filtrosPesquisa.PaisOrigem > 0)
                sql += $" and LocalidadeInicioPrestacao.PAI_CODIGO = {filtrosPesquisa.PaisOrigem}";

            if (filtrosPesquisa.Destino > 0)
                sql += $" and CTe.CON_LOCTERMINOPRESTACAO = {filtrosPesquisa.Destino}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                sql += $" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
            {
                if (int.TryParse(filtrosPesquisa.NumeroDocumento, out int numero))
                    sql += $" and (CTe.CON_NUM = {numero} or Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroDocumento}')";
                else
                    sql += $" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroDocumento}'";
            }

            if (filtrosPesquisa.TipoPropostaMultimodal?.Count > 0 || filtrosPesquisa.TiposPropostasMultimodal?.Count > 0)
            {
                List<TipoPropostaMultimodal> tipoProposta = new List<TipoPropostaMultimodal>();
                if (filtrosPesquisa.TipoPropostaMultimodal?.Count > 0)
                    tipoProposta.AddRange(filtrosPesquisa.TipoPropostaMultimodal);
                if (filtrosPesquisa.TiposPropostasMultimodal?.Count > 0)
                    tipoProposta.AddRange(filtrosPesquisa.TiposPropostasMultimodal);

                sql += $@" and exists (select cargaCTe.CCT_CODIGO from T_CARGA_CTE cargaCTe 
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO = cargaCTe.CAR_CODIGO 
                                        where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", tipoProposta.Select(o => o.ToString("d")))}))";
            }

            if (filtrosPesquisa.TiposOSConvertidos?.Count > 0)
            {
                sql += $@" and exists (select cargaCTe.CCT_CODIGO from T_CARGA_CTE cargaCTe 
                                        inner join T_CARGA carga on carga.CAR_CODIGO = cargaCTe.CAR_CODIGO 
                                        where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and carga.CAR_TIPO_OS_CONVERTIDO in ({string.Join(", ", filtrosPesquisa.TiposOSConvertidos.Select(o => o.ToString("d")))}))";
            }

            if (filtrosPesquisa.CodigoContainer > 0)
                sql += $" and exists (select cteContainer.CON_CODIGO from T_CTE_CONTAINER cteContainer where cteContainer.CON_CODIGO = CTe.CON_CODIGO and cteContainer.CTR_CODIGO = {filtrosPesquisa.CodigoContainer})"; // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControleCliente))
            {
                sql += $@" and exists (select cargaCTe.CCT_CODIGO from T_CARGA_CTE cargaCTe 
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO = cargaCTe.CAR_CODIGO 
                                        inner join T_PEDIDO pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO 
                                        where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and pedido.PED_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroControleCliente}')";
            }

            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                sql += $@" and exists (select tipoOperacaoPagamentos.CTP_CODIGO from T_CONFIGURACAO_TIPO_OPERACAO_PAGAMENTOS tipoOperacaoPagamentos 
                                        inner join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.CTP_CODIGO = tipoOperacaoPagamentos.CTP_CODIGO
                                        inner join T_CARGA carga on carga.TOP_CODIGO = tipoOperacao.TOP_CODIGO
                                        inner join T_CARGA_CTE cargaCTe on cargaCTe.CAR_CODIGO = carga.CAR_CODIGO
                                        where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and tipoOperacaoPagamentos.CRE_CODIGO = {filtrosPesquisa.CodigoCentroResultado})";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroReferenciaEDI))
            {
                sql += $" and exists (select cteDocs.CON_CODIGO from T_CTE_DOCS cteDocs where cteDocs.CON_CODIGO = CTe.CON_CODIGO and cteDocs.NFC_NUMERO_REFERENCIA_EDI = :CTEDOCS_NFC_NUMERO_REFERENCIA_EDI)";
                parametros.Add(new ParametroSQL("CTEDOCS_NFC_NUMERO_REFERENCIA_EDI", filtrosPesquisa.NumeroReferenciaEDI));
            }

            if (filtrosPesquisa.CodigoCTe > 0)
                sql += $" and CTe.CON_CODIGO = {filtrosPesquisa.CodigoCTe}";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                sql += $" and exists (select veiculo.DFA_CODIGO from T_DOCUMENTO_FATURAMENTO_VEICULO veiculo where veiculo.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO and veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                sql += $" and CTe.CON_TIPO_CTE = {filtrosPesquisa.TipoCTe.ToString("d")}";

            if (filtrosPesquisa.NumeroDocumentoInicial > 0)
                sql += $" and CTe.CON_NUM >= {filtrosPesquisa.NumeroDocumentoInicial}";

            if (filtrosPesquisa.NumeroDocumentoFinal > 0)
                sql += $" and CTe.CON_NUM <= {filtrosPesquisa.NumeroDocumentoFinal}";

            if (filtrosPesquisa.Serie > 0)
                sql += $" and EmpresaSerie.ESE_NUMERO = {filtrosPesquisa.Serie}";

            if (filtrosPesquisa.TomadorFatura > 0)
                sql += $" and Tomador.CLI_CGCCPF = {filtrosPesquisa.TomadorFatura}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContainer))
                sql += $" and Container.CTR_NUMERO = '{filtrosPesquisa.NumeroContainer}'";

            if (filtrosPesquisa.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados)
                sql += $" and DocumentoFaturamento.DFA_CANHOTOS_DIGITALIZADOS = 1";

            if (filtrosPesquisa.GerarDocumentosApenasCanhotosAprovados)
            {
                sql += @" and 
                        not exists(
                            select 
                                CNF_CODIGO 
                            FROM 
                                T_CANHOTO_NOTA_FISCAL CANHOTO 
                            WHERE 
                                CANHOTO.CAR_CODIGO IN (
                                SELECT 
                                    cargaCTe.CAR_CODIGO 
                                from 
                                    T_CARGA_CTE cargaCTe 
                                where 
                                    cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                ) 
                                AND CANHOTO.CNF_SITUACAO_DIGITALIZACAO_CANHOTO != 3
                        )
                        ";
            }

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return new SQLDinamico(sql, parametros);
        }

        #endregion
    }
}

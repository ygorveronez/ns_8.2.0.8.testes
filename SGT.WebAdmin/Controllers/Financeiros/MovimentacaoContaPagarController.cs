using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/MovimentacaoContaPagar")]
    public class MovimentacaoContaPagarController : BaseController
    {
		#region Construtores

		public MovimentacaoContaPagarController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMovimentacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransportador = Request.GetIntParam("Transportador");
                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");

                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacao = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacoes = repositorioMovimentacao.BuscarPorTransportador(codigoTransportador, dataInicio, dataFinal);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Documento", "DataDocumento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tax Codigo", "TaxCode", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Numero Documento", "NumeroDocumento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Termo Pagamento", "TermoPagamento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Numero Cte", "NumeroCte", 15, Models.Grid.Align.left, false);

                var retorno = (from obj in movimentacoes
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   DataDocumento = obj?.DataDocumento?.ToString("dd/MM/yyyy") ?? "",
                                   TipoDocumento = obj?.TipoDocumento ?? "",
                                   ValorTotal = obj?.ValorTotal.ToString("n2") ?? "",
                                   TaxCode = obj?.CodigoTaxa ?? "",
                                   NumeroDocumento = obj?.NumeroDocumento ?? "",
                                   TermoPagamento = obj?.TermoPagamento ?? "",
                                   NumeroCte = obj?.NumeroCte ?? "",
                               }).ToList();

                grid.setarQuantidadeTotal(movimentacoes.Count);
                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ReprocessamentoContaPagar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);

                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                DateTime? dataFim = Request.GetNullableDateTimeParam("DataFim");
                repositorioMovimentacaoPagar.ReprocessarContasPagarSemMiro(dataInicio, dataFim);
                return new JsonpResult(true, "Registro enviado para processamento com sucesso");

            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar Reprocessar.");
            }
        }
        public async Task<IActionResult> VincularCTeManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int codigoCte = Request.GetIntParam("Cte");
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar existeMovimentacao = repositorioMovimentacaoPagar.BuscarPorCodigo(codigo, false);

                if (existeMovimentacao == null)
                    return new JsonpResult(false, "Registro não encontrado");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(codigoCte);

                if (cte == null)
                    return new JsonpResult(false, "CT-e não encontrado");

                existeMovimentacao.CTe = cte;
                existeMovimentacao.SituacaoProcessamento = SituacaoProcessamento.Processado;
                existeMovimentacao.MiroRecebida = true;
                existeMovimentacao.ObservacaoMiro = "Documento Vinculado Manualmente";
                repositorioMovimentacaoPagar.Atualizar(existeMovimentacao);
                return new JsonpResult(true, "Registro enviado para processamento com sucesso");

            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar Reprocessar.");
            }
        }

        public async Task<IActionResult> VerificarManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<int> codigoSelecionados = Request.GetListParam<int>("Codigos");
                bool SelecionouTodos = Request.GetBoolParam("SelecionouTodos");

                if (codigoSelecionados.Count == 0 && !SelecionouTodos)
                    return new JsonpResult(false, "Precisa Selecionar registros para ser processado");

                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamenteo = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacoesAprovar = repositorioContaPagar.BuscarMovimentacaoesAprovarManualmente(codigoSelecionados);

                foreach (var movimentacao in movimentacoesAprovar)
                {
                    movimentacao.SituacaoProcessamento = SituacaoProcessamento.Processado;
                    repositorioContaPagar.Atualizar(movimentacao);
                }

                return new JsonpResult(true);
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Problema ao tentar processar manualmente");
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion

        #region Metodos Privados 
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCTe", false);
            grid.AdicionarCabecalho("Código Empresa", "CodigoEmpresa", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Pmnt Bloc", "PmntBlock", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Posting Key", "PostingKey", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Due Date", "DueDate", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Referência", "Referencia", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Taxa Code", "TaxaCode", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data Documento", "DataDocumento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data Compensação (Clearing)", "DataCompensacao", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("ClrngDoc", "ClrngDoc", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Text", "Text", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Valor Monetário (AmountInLC)", "ValorMonetario", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("EspecialGL", "EspecialGL", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Moeda", "Moeda", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("User Name", "NomeUsuario", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Invoice ref", "InvoiceRef", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Posting Date", "DataPostagem", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Assignment", "Assignment", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Disc. base", "DiscountBase", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Termo de Pagamento", "TermoPagamento", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCte", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Série CT-e", "SerieCte", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Doc.Header Text", "DocHeaderText", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Reference Key", "ChaveReferencia", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Material Doc.", "MaterialDocument", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Name (transportador)", "Name", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Total Value", "TotaValorTotalValue", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Purchasing Doc.", "PurchasingDocument", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Trading Partner", "TradingPartner", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("G/L", "GL", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Profit Center", "ProfitCenter", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Segment", "Segment", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Order", "Order", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Cost Center", "CostCenter", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Debit/Credit", "DebitCredit", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CNPJ Sender", "CNPJSender", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Sender name", "SenderName", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CNPJ Receiver", "CNPJReceiver", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Receiver name", "ReceiverName", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CNPJ Dispatcher", "CNPJDispatcher", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Dispatcher name", "DispatcherName", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CNPJ Receipt", "CNPJReceipt", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Receipt name", "ReceiptName", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("City", "City", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Region", "Region", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Log Message", "LogMessage", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Protocol No.", "ProtocolNo", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Comments", "Comments", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Access Key", "AccessKey", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Tax Approval Date", "TaxApprovalDate", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor líquido (Net Amount)", "Netamount", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Date da Upload", "DateUpload", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Termo Quitação", "TermoQuitacao", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Obs. Miro", "ObsMiro", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Tipo Arquivo", "TipoArquivo", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Situação Processamento", "SituacaoProcessamento", 10, Models.Grid.Align.right, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "MovimentacaoContaPagar/Pesquisa", "grid-movimentacao-contas-pagar");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtroPesquisa = ObterFiltroPesquisa();

            if (filtroPesquisa.TodasFiliaisTransportador)
            {
                var transportador = repositorioTransportador.BuscarPorCodigo(filtroPesquisa.Transportador);
                var filiais = transportador != null ? transportador.Filiais.ToList() : new List<Dominio.Entidades.Empresa>();
                var codigoFiliais = filiais.Select(x => x.Codigo).ToList();
                filtroPesquisa.CodigosFiliaisTransportador.AddRange(codigoFiliais);
            }

            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacaoConta = repositorioMovimentacaoContaPagar.Pesquisar(filtroPesquisa, grid.ObterParametrosConsulta());
            int quantidade = repositorioMovimentacaoContaPagar.ContarPesquisa(filtroPesquisa);

            var retorno = (from obj in movimentacaoConta
                           select new
                           {
                               obj.Codigo,
                               CodigoCTe = obj?.CTe?.Codigo ?? 0,
                               CodigoEmpresa = obj?.CodigoEmpresa ?? "",
                               PmntBlock = obj?.PmntBlock ?? "",
                               DueDate = obj.DueData.HasValue ? obj.DueData.Value.ToString("dd/MM/yyyy") : "",
                               PostingKey = (int)obj.PostedKey,
                               Transportador = obj.Transportador?.CodigoIntegracao ?? string.Empty,
                               Referencia = obj?.Referencia ?? "",
                               DataCompensacao = obj.DataCompensamento.HasValue ? obj.DataCompensamento.Value.ToString("dd/MM/yyyy") : "",
                               TaxaCode = obj?.CodigoTaxa ?? "",
                               ClrngDoc = obj?.ClrngDoc ?? "",
                               Text = obj?.Texto ?? "",
                               DataDocumento = obj.DataDocumento?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                               TipoDocumento = obj?.TipoDocumento ?? "",
                               ValorMonetario = obj.ValorMonetario.ToString("n2"),
                               Netamount = obj.NetValor.ToString("n2"),
                               NumeroDocumento = obj?.NumeroDocumento ?? "",
                               Moeda = obj?.Moeda ?? "",
                               TermoPagamento = obj?.TermoPagamento ?? "",
                               NomeUsuario = obj?.NomeUsuario ?? "",
                               EspecialGL = obj?.EspecialGL ?? "",
                               InvoiceRef = obj?.InvoiceRef ?? "",
                               DataPostagem = obj.DataPostagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                               Assignment = obj?.Atribuicao ?? "",
                               DiscountBase = obj.DisBase,
                               NumeroCte = obj?.CTe?.Numero ?? 0,
                               SerieCte = obj?.CTe?.Serie?.Numero ?? 0,
                               DocHeaderText = obj?.DocHeaderText ?? "",
                               ChaveReferencia = obj?.ChaveReferencia ?? "",
                               MaterialDocument = obj?.ChaveReferencia ?? "",
                               Name = obj?.NomeUsuario ?? "",
                               TotaValorTotalValue = obj?.ValorTotal.ToString("n2") ?? "",
                               PurchasingDocument = obj?.DocumentoCompra ?? "",
                               TradingPartner = obj?.ParceiroComercial ?? "",
                               GL = obj?.GL ?? "",
                               ProfitCenter = obj.CentroLucro,
                               Order = obj?.Ordem ?? "",
                               Segment = obj?.Segmento ?? "",
                               CostCenter = obj?.CentroCusto ?? "",
                               DebitCredit = obj?.DebitoCredito ?? "",
                               CNPJReceipt = obj?.Recebedor?.CPF_CNPJ_Formatado ?? "",
                               ReceiptName = obj?.Recebedor?.Descricao ?? "",
                               CNPJSender = obj?.Remetente?.CPF_CNPJ_Formatado ?? "",
                               SenderName = obj?.Remetente?.Descricao ?? "",
                               CNPJReceiver = obj?.Destinatario?.CPF_CNPJ_Formatado ?? "",
                               ReceiverName = obj?.Destinatario?.Descricao ?? "",
                               CNPJDispatcher = obj?.Expedidor?.CPF_CNPJ_Formatado ?? "",
                               DispatcherName = obj?.Expedidor?.Descricao ?? "",
                               Region = "",
                               City = "",
                               LogMessage = obj?.MensageLog ?? "",
                               ProtocolNo = obj?.Protocolo ?? "",
                               ObsMiro = obj?.ObservacaoMiro ?? "",
                               Comments = obj?.Comments ?? "",
                               AccessKey = obj?.ChaveAcesso ?? "",
                               TaxApprovalDate = obj.DataAprovacaoTaxa.HasValue ? obj.DataAprovacaoTaxa.Value.ToString("dd/MM/yyyy") : "",
                               DateUpload = obj.DataUpload.HasValue ? obj.DataUpload.Value.ToString("dd/MM/yyyy") : "",
                               TipoArquivo = obj?.ContaPagar?.TipoRegistro.ObterDescricao() ?? "Sem Tipo",
                               SituacaoProcessamento = obj?.SituacaoProcessamento.ObterDescricao(),
                               TermoQuitacao = obj?.TermoQuitacaoFinanceiro?.NumeroTermo ?? 0,
                               DT_RowColor = obj.SituacaoProcessamento == SituacaoProcessamento.Processado && obj.MiroRecebida ? CorGrid.Verde : obj.SituacaoProcessamento == SituacaoProcessamento.Processado && !obj.MiroRecebida ? CorGrid.Yellow : CorGrid.Azul
                           }).ToList();

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(quantidade);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar ObterFiltroPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar()
            {
                DataCompensacaoFinal = Request.GetNullableDateTimeParam("DataCompensacaoFinal"),
                DataCompensacaoInicial = Request.GetNullableDateTimeParam("DataCompensacaoInicial"),
                DataDocFinal = Request.GetNullableDateTimeParam("DataDocFinal"),
                DataDocInicial = Request.GetNullableDateTimeParam("DataDocInicial"),
                TipoArquivo = Request.GetEnumParam<TipoRegistro>("TipoArquivo"),
                Transportador = Request.GetIntParam("Transportador"),
                NumeroTermoQuitacao = Request.GetIntParam("NumeroTermoQuitacao"),
                TodasFiliaisTransportador = Request.GetBoolParam("TodasFiliaisTransportador"),
                SituacaoProcessamento = Request.GetNullableEnumParam<SituacaoProcessamento>("SituacaoProcessamento"),
                SituacaoDocumentoMovimentacao = Request.GetNullableEnumParam<SituacaoDocumentoMovimentacao>("SituacaoDocumento"),
                CodigosFiliaisTransportador = new List<int>(),
            };
        }

        #endregion
    }
}

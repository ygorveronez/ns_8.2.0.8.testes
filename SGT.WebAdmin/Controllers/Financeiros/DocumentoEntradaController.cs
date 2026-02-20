using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.Entidades.Embarcador.Financeiro;
using System.Xml.Linq;
using System.Text;
using System.Web;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/DocumentoEntrada", "Cargas/CargaMDFeManual")]
    public class DocumentoEntradaController : BaseController
    {
		#region Construtores

		public DocumentoEntradaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentoReferencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcerto = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                int numeroInicial, numeroFinal, codigoVeiculo = 0, codigoAcertoViagem = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("AcertoViagem"), out codigoAcertoViagem);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = null;
                if (codigoAcertoViagem > 0)
                    acerto = repAcerto.BuscarPorCodigo(codigoAcertoViagem);

                string serie = Request.Params("Serie");
                string chave = Request.Params("Chave");
                decimal valorTotal = Request.GetDecimalParam("ValorTotal");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataEntradaInicial, dataEntradaFinal;
                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params("DataEntradaInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaInicial);
                DateTime.TryParseExact(Request.Params("DataEntradaFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaFinal);

                SituacaoDocumentoSPEDFiscal situacaoDocumentoSPEDFiscal = SituacaoDocumentoSPEDFiscal.TodosDocumentos;
                Enum.TryParse(Request.Params("SituacaoDocumentoSPEDFiscal"), out situacaoDocumentoSPEDFiscal);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Nota", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Entrada", "DataEntrada", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Chave", "Chave", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("CodigoFornecedor", false);

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntrada = repDocumentoEntrada.ConsultarDocumentoReferencia(situacaoDocumentoSPEDFiscal, dataEmissaoInicial, dataEmissaoFinal, dataEntradaInicial, dataEntradaFinal, acerto, codigoVeiculo, TipoServicoMultisoftware, numeroInicial, numeroFinal, serie, chave, valorTotal, codigoEmpresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repDocumentoEntrada.ContarConsultaDocumentoReferencia(situacaoDocumentoSPEDFiscal, dataEmissaoInicial, dataEmissaoFinal, dataEntradaInicial, dataEntradaFinal, acerto, codigoVeiculo, TipoServicoMultisoftware, numeroInicial, numeroFinal, serie, chave, valorTotal, codigoEmpresa));
                var lista = (from p in listaDocumentoEntrada
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.Serie,
                                 DataEmissao = p.DataEmissao.ToDateTimeString(),
                                 DataEntrada = p.DataEntrada.ToDateString(),
                                 Fornecedor = p.Fornecedor?.NomeCNPJ ?? string.Empty,
                                 CodigoFornecedor = p.Fornecedor?.Codigo ?? 0,
                                 p.Chave,
                                 ValorTotal = p.ValorTotal.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                        return new JsonpResult(false, true, "Você não possui permissões para adicionar um documento de entrada.");

                DateTime dataEntrada, dataEmissao, dataAbsAux;
                DateTime? dataAbastecimento = null;
                DateTime.TryParseExact(Request.Params("DataEntrada"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);
                DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                DateTime.TryParseExact(Request.Params("DataAbastecimento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataAbsAux);
                if (dataAbsAux > DateTime.MinValue)
                    dataAbastecimento = dataAbsAux;

                if (dataEntrada.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "A data de entrada não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (dataEmissao.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "A data de emissão não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (dataEmissao.Date > dataEntrada.Date)
                    return new JsonpResult(false, true, "A data de emissão não pode ser maior que a data de entrada.");

                if (dataEntrada.Date < DateTime.Now.Date && !this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_BloquearLancamentoComDataRetroativa))
                    return new JsonpResult(false, true, "Você não possui permissão para adicionar um documento de entrada com data retroativa.");

                Int64.TryParse(Request.Params("Numero"), out Int64 numero);

                double cpfCnpjFornecedor = 0d;
                int situacaoLancamentoDocumentoEntrada = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Fornecedor")), out cpfCnpjFornecedor);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("StatusLancamento")), out situacaoLancamentoDocumentoEntrada);

                bool? encerrarOrdemServico = Request.GetNullableBoolParam("EncerrarOrdemServico");

                Enum.TryParse(Request.Params("Situacao"), out SituacaoDocumentoEntrada situacao);

                string serie = Request.Params("Serie");
                string chave = Request.Params("Chave");
                string documentoImportadoXML = Request.Params("DocumentoImportadoXML");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (situacao == SituacaoDocumentoEntrada.Finalizado && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Finalizar))
                        return new JsonpResult(false, true, "Você não possui permissões para finalizar um documento de entrada.");

                    if ((situacao == SituacaoDocumentoEntrada.Cancelado || situacao == SituacaoDocumentoEntrada.Anulado) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar))
                        return new JsonpResult(false, true, "Você não possui permissões para cancelar um documento de entrada.");
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento = !string.IsNullOrWhiteSpace(chave) ? repDocumentoEntrada.BuscarPorChave(chave) : null;

                if (documento == null)
                    documento = repDocumentoEntrada.BuscarPorFornecedorNumeroESerie(numero, serie, cpfCnpjFornecedor, 0);

                if (documento != null)
                    return new JsonpResult(false, true, "O documento informado já se encontra cadastrado no sistema. Número do lançamento: " + documento.NumeroLancamento + ".");

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS();

                PreencherDocumentoEntrada(documentoEntrada, unidadeDeTrabalho);

                documentoEntrada.EncerrarOrdemServico = encerrarOrdemServico;
                documentoEntrada.Chave = chave;
                documentoEntrada.DataEmissao = dataEmissao;
                documentoEntrada.DataEntrada = dataEntrada;
                documentoEntrada.Fornecedor = cpfCnpjFornecedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor) : null;
                documentoEntrada.SituacaoLancamentoDocumentoEntrada = situacaoLancamentoDocumentoEntrada > 0 ? repSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(situacaoLancamentoDocumentoEntrada) : null;
                documentoEntrada.Numero = numero;
                documentoEntrada.NumeroLancamento = repDocumentoEntrada.BuscarUltimoNumeroLancamento() + 1;
                documentoEntrada.Serie = serie;
                documentoEntrada.Situacao = situacao;
                documentoEntrada.DataAbastecimento = dataAbastecimento != null && dataAbastecimento > DateTime.MinValue ? dataAbastecimento : null;
                documentoEntrada.OperadorLancamentoDocumento = this.Usuario;
                documentoEntrada.DocumentoImportadoXML = documentoImportadoXML != null ? documentoImportadoXML : null;

                ValidarRegrasDocumentoEntrada(documentoEntrada, unidadeDeTrabalho);

                repDocumentoEntrada.Inserir(documentoEntrada, Auditado);

                string erro = string.Empty;
                bool contemQuantidadePendenteOrdemCompra = false;
                List<int> codigoTitulos = new List<int>();

                if (!SalvarItens(out erro, documentoEntrada, unidadeDeTrabalho))
                    throw new ControllerException(erro);

                int countDuplicatas = SalvarDuplicatas(documentoEntrada, unidadeDeTrabalho);
                SalvarCentrosResultado(documentoEntrada, unidadeDeTrabalho);
                SalvarCentrosResultadoTiposDespesa(documentoEntrada, unidadeDeTrabalho);
                SalvarVeiculos(documentoEntrada, unidadeDeTrabalho);

                if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                    Servicos.Embarcador.Financeiro.TituloAPagar svcTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unidadeDeTrabalho);
                    Servicos.Embarcador.Patrimonio.Bem svcBem = new Servicos.Embarcador.Patrimonio.Bem(unidadeDeTrabalho);

                    if (!svcDocumentoEntrada.ValidarRegraEntrada(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro))
                        throw new ControllerException(erro);

                    if (!Servicos.Embarcador.Frota.OrdemServico.VincularDocumentoEntradaAOrdemServico(documentoEntrada, out erro, unidadeDeTrabalho, this.Usuario, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!Servicos.Embarcador.Compras.OrdemCompra.FinalizarOrdemCompra(documentoEntrada.Codigo, out erro, out contemQuantidadePendenteOrdemCompra, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!contemQuantidadePendenteOrdemCompra)
                        Servicos.Embarcador.Compras.OrdemCompra.CriarQualificacaoFornecedor(documentoEntrada, unidadeDeTrabalho);

                    svcDocumentoEntrada.GerarCentrosResultadoTiposDespesaAutomaticamente(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware);

                    if (!svcTituloAPagar.AtualizarTitulos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (!svcTituloAPagar.AtualizarGuias(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.AtualizarCusto(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.MovimentarEstoque(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    bool possuiPermissaoGravarValorDiferente = this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;
                    if (!svcDocumentoEntrada.GerarAbastecimentos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada, possuiPermissaoGravarValorDiferente, false))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.CadastrarPneu(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado))
                        throw new ControllerException(erro);

                    if (!svcBem.CadastrarBem(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.GerarMovimentoFinanceiroDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (!svcDocumentoEntrada.GerarMovimentoFinanceiroEmissaoItens(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                            throw new ControllerException(erro);
                    }

                    svcDocumentoEntrada.GerarBaixarTituloDuplicata(documentoEntrada, unidadeDeTrabalho, tipoAmbiente, countDuplicatas, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

                    documentoEntrada.DataFinalizacao = DateTime.Now;
                    documentoEntrada.OperadorFinalizaDocumento = this.Usuario;

                    Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                    codigoTitulos = repTitulo.BuscarCodigosPorDocumentoEntrada(documentoEntrada.Codigo);
                }

                SalvarLog(documentoEntrada, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                bool realizarRateioDespesaVeiculo = repDocumentoEntradaItem.RealizarRateioDespesaVeiculo(documentoEntrada.Codigo);
                decimal valorRateioDespesaVeiculo = repDocumentoEntradaItem.ValorRateioDespesaVeiculo(documentoEntrada.Codigo);
                new Servicos.Embarcador.Integracao.IntegracaoDocumentoEntrada(unidadeDeTrabalho).IniciarIntegracoesDeDocumentoEntrada(documentoEntrada);

                return new JsonpResult(new
                {
                    ExibirQualificaoFornecedor = !contemQuantidadePendenteOrdemCompra && documentoEntrada.OrdemCompra != null && documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado,
                    MostrarMensagemOSNaoFinalizada = encerrarOrdemServico != documentoEntrada.EncerrarOrdemServico,
                    CodigosTitulos = string.Join(", ", codigoTitulos),
                    RealizarRateioDespesaVeiculo = realizarRateioDespesaVeiculo,
                    Codigo = documentoEntrada.Codigo,
                    NumeroDocumento = Utilidades.String.OnlyNumbers(documentoEntrada.Numero.ToString("n0")),
                    TipoDocumento = "DOC",
                    Valor = valorRateioDespesaVeiculo.ToString("n2"),
                    RealizarRateioSomenteQuandoNaoTiverOS = (documentoEntrada?.CFOP?.RealizarRateioSomenteQuandoTiverOS ?? false) ? (documentoEntrada.OrdemServico == null ? true : false) : true,
                    Pessoa = new
                    {
                        Codigo = documentoEntrada.Fornecedor?.Codigo ?? 0,
                        Descricao = documentoEntrada.Fornecedor?.Descricao ?? string.Empty,
                    }
                });
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                        return new JsonpResult(false, true, "Você não possui permissões para alterar um documento de entrada.");

                DateTime dataEntrada, dataEmissao, dataAbsAux;
                DateTime? dataAbastecimento = null;
                DateTime.TryParseExact(Request.Params("DataEntrada"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);
                DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                DateTime.TryParseExact(Request.Params("DataAbastecimento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataAbsAux);
                if (dataAbsAux > DateTime.MinValue)
                    dataAbastecimento = dataAbsAux;

                if (dataEntrada.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "A data de entrada não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (dataEmissao.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "A data de emissão não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (dataEmissao.Date > dataEntrada.Date)
                    return new JsonpResult(false, true, "A data de emissão não pode ser maior que a data de entrada.");

                if (dataEntrada.Date < DateTime.Now.Date && !this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_BloquearLancamentoComDataRetroativa))
                    return new JsonpResult(false, true, "Você não possui permissão para alterar um documento de entrada de data retroativa.");

                Int64.TryParse(Request.Params("Numero"), out Int64 numero);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                double cpfCnpjFornecedor = 0d;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Fornecedor")), out cpfCnpjFornecedor);

                bool? encerrarOrdemServico = Request.GetNullableBoolParam("EncerrarOrdemServico");

                Enum.TryParse(Request.Params("Situacao"), out SituacaoDocumentoEntrada situacao);

                string serie = Request.Params("Serie");
                string chave = Request.Params("Chave");

                int situacaoLancamentoDocumentoEntrada = 0;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("StatusLancamento")), out situacaoLancamentoDocumentoEntrada);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (situacao == SituacaoDocumentoEntrada.Finalizado && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Finalizar))
                        return new JsonpResult(false, true, "Você não possui permissões para finalizar um documento de entrada.");

                    if ((situacao == SituacaoDocumentoEntrada.Cancelado || situacao == SituacaoDocumentoEntrada.Anulado) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar))
                        return new JsonpResult(false, true, "Você não possui permissões para cancelar um documento de entrada.");
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento = !string.IsNullOrWhiteSpace(chave) ? repDocumentoEntrada.BuscarPorChaveECodigoDiff(codigo, chave) : null;

                if (documento == null)
                    documento = repDocumentoEntrada.BuscarPorFornecedorNumeroSerieECodigoDiff(codigo, numero, serie, cpfCnpjFornecedor);

                if (documento != null)
                    return new JsonpResult(false, true, "O documento informado já se encontra cadastrado no sistema. Número do lançamento: " + documento.NumeroLancamento + ".");

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo, true);

                if ((documentoEntrada.Situacao == SituacaoDocumentoEntrada.Cancelado || documentoEntrada.Situacao == SituacaoDocumentoEntrada.Anulado) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_PermitirAtualizarNotaCancelada))
                    return new JsonpResult(false, true, "Você não possui permissões para alterar um documento de entrada anulado/cancelado.");

                if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                    throw new ControllerException("O documento de entrada já foi finalizado anteriormente! Não sendo mais possível atualizar.");

                PreencherDocumentoEntrada(documentoEntrada, unidadeDeTrabalho);

                documentoEntrada.EncerrarOrdemServico = encerrarOrdemServico;
                documentoEntrada.Chave = chave;
                documentoEntrada.DataEmissao = dataEmissao;
                documentoEntrada.DataEntrada = dataEntrada;
                documentoEntrada.Fornecedor = cpfCnpjFornecedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor) : null;
                documentoEntrada.Numero = numero;
                documentoEntrada.Serie = serie;
                documentoEntrada.Situacao = situacao;
                documentoEntrada.DataAbastecimento = dataAbastecimento != null && dataAbastecimento.Value > DateTime.MinValue ? dataAbastecimento : null;
                documentoEntrada.SituacaoLancamentoDocumentoEntrada = situacaoLancamentoDocumentoEntrada > 0 ? repSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(situacaoLancamentoDocumentoEntrada) : null;

                ValidarRegrasDocumentoEntrada(documentoEntrada, unidadeDeTrabalho);

                repDocumentoEntrada.Atualizar(documentoEntrada, Auditado);

                string erro = string.Empty;
                bool contemQuantidadePendenteOrdemCompra = false;
                List<int> codigoTitulos = new List<int>();

                if (!SalvarItens(out erro, documentoEntrada, unidadeDeTrabalho))
                    throw new ControllerException(erro);

                int countDuplicatas = SalvarDuplicatas(documentoEntrada, unidadeDeTrabalho);
                SalvarCentrosResultado(documentoEntrada, unidadeDeTrabalho);
                SalvarCentrosResultadoTiposDespesa(documentoEntrada, unidadeDeTrabalho);
                SalvarVeiculos(documentoEntrada, unidadeDeTrabalho);

                if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                    Servicos.Embarcador.Financeiro.TituloAPagar svcTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unidadeDeTrabalho);
                    Servicos.Embarcador.Patrimonio.Bem svcBem = new Servicos.Embarcador.Patrimonio.Bem(unidadeDeTrabalho);
                    Servicos.Embarcador.Financeiro.ContratoFinanciamento svcContratoFinanciamento = new Servicos.Embarcador.Financeiro.ContratoFinanciamento(unidadeDeTrabalho);

                    if (!svcDocumentoEntrada.ValidarRegraEntrada(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro))
                        throw new ControllerException(erro);

                    if (!Servicos.Embarcador.Frota.OrdemServico.VincularDocumentoEntradaAOrdemServico(documentoEntrada, out erro, unidadeDeTrabalho, this.Usuario, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!Servicos.Embarcador.Compras.OrdemCompra.FinalizarOrdemCompra(documentoEntrada.Codigo, out erro, out contemQuantidadePendenteOrdemCompra, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!contemQuantidadePendenteOrdemCompra)
                        Servicos.Embarcador.Compras.OrdemCompra.CriarQualificacaoFornecedor(documentoEntrada, unidadeDeTrabalho);

                    svcDocumentoEntrada.GerarCentrosResultadoTiposDespesaAutomaticamente(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware);

                    if (!svcTituloAPagar.AtualizarTitulos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (!svcTituloAPagar.AtualizarGuias(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.AtualizarCusto(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.MovimentarEstoque(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    bool possuiPermissaoGravarValorDiferente = this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;
                    if (!svcDocumentoEntrada.GerarAbastecimentos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada, possuiPermissaoGravarValorDiferente, false))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.CadastrarPneu(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado))
                        throw new ControllerException(erro);

                    if (!svcBem.CadastrarBem(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado))
                        throw new ControllerException(erro);

                    if (!svcDocumentoEntrada.GerarMovimentoFinanceiroDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (!svcDocumentoEntrada.GerarMovimentoFinanceiroEmissaoItens(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                            throw new ControllerException(erro);
                    }

                    svcContratoFinanciamento.VincularDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, Auditado);

                    svcDocumentoEntrada.GerarBaixarTituloDuplicata(documentoEntrada, unidadeDeTrabalho, tipoAmbiente, countDuplicatas, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

                    documentoEntrada.DataFinalizacao = DateTime.Now;
                    documentoEntrada.OperadorFinalizaDocumento = this.Usuario;

                    Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                    codigoTitulos = repTitulo.BuscarCodigosPorDocumentoEntrada(documentoEntrada.Codigo);
                }

                SalvarLog(documentoEntrada, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                bool realizarRateioDespesaVeiculo = repDocumentoEntradaItem.RealizarRateioDespesaVeiculo(documentoEntrada.Codigo);
                decimal valorRateioDespesaVeiculo = repDocumentoEntradaItem.ValorRateioDespesaVeiculo(documentoEntrada.Codigo);
                new Servicos.Embarcador.Integracao.IntegracaoDocumentoEntrada(unidadeDeTrabalho).IniciarIntegracoesDeDocumentoEntrada(documentoEntrada);

                return new JsonpResult(new
                {
                    ExibirQualificaoFornecedor = !contemQuantidadePendenteOrdemCompra && documentoEntrada.OrdemCompra != null && documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado,
                    MostrarMensagemOSNaoFinalizada = encerrarOrdemServico != documentoEntrada.EncerrarOrdemServico,
                    CodigosTitulos = string.Join(", ", codigoTitulos),
                    RealizarRateioDespesaVeiculo = realizarRateioDespesaVeiculo,
                    Codigo = documentoEntrada.Codigo,
                    NumeroDocumento = Utilidades.String.OnlyNumbers(documentoEntrada.Numero.ToString("n0")),
                    TipoDocumento = "DOC",
                    Valor = valorRateioDespesaVeiculo.ToString("n2"),
                    ValorTotalCusto = documentoEntrada.ValorTotalCusto.ToString("n2"),
                    RealizarRateioSomenteQuandoNaoTiverOS = (documentoEntrada?.CFOP?.RealizarRateioSomenteQuandoTiverOS ?? false) ? (documentoEntrada.OrdemServico == null ? true : false) : true,
                    Pessoa = new
                    {
                        Codigo = documentoEntrada.Fornecedor?.Codigo ?? 0,
                        Descricao = documentoEntrada.Fornecedor?.Descricao ?? string.Empty,
                    }
                });
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReverterFaturamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ReAbrir))
                        return new JsonpResult(false, true, "Você não possui permissões para reverter o faturamento do documento de entrada.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo);

                if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Finalizado)
                    return new JsonpResult(false, true, "Não é possível reverter o faturamento na situação atual do documento de entrada.");

                if (documentoEntrada.Itens.Any(o => o.OrdemServico != null && o.OrdemServico.Situacao == SituacaoOrdemServicoFrota.Finalizada))
                    return new JsonpResult(false, true, "Não é possível reverter o faturamento pois há uma ordem de serviço finalizada vinculada a este documento de entrada.");

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.AcertosFinalizadosDocumentoEntrada> acertosFechados = repDocumentoEntrada.AbastecimentosEmAcertoFechado(codigo);
                if (acertosFechados != null && acertosFechados.Count > 0)
                    return new JsonpResult(false, true, "Não é possível reverter o faturamento, existe(m) acerto(s) finalizado(s) contendo o abastecimento lançado neste documento de entrada Nº: " + string.Join(", ", acertosFechados.Select(c => c.NumeroAcerto).ToList()) + ".");

                unidadeDeTrabalho.Start();

                documentoEntrada.Situacao = SituacaoDocumentoEntrada.Aberto;
                documentoEntrada.DataAlteracao = DateTime.Now;

                repDocumentoEntrada.Atualizar(documentoEntrada);

                string erro = string.Empty;

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                Servicos.Embarcador.Financeiro.TituloAPagar svcTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unidadeDeTrabalho);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if ((ConfiguracaoEmbarcador.ControlarEstoqueNegativo || documentoEntrada.Destinatario.ControlarEstoqueNegativo) && (documentoEntrada.CFOP.GeraEstoque && documentoEntrada.NaturezaOperacao.ControlaEstoque))
                {
                    Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
                    Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeDeTrabalho);

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
                    {
                        if (item.Produto.CategoriaProduto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Servicos)
                        {
                            if (item.Produto != null && !servicoEstoque.ValidarProdutoComEstoque(out string erroEstoqueNegativo, item.Produto, item.Quantidade, null, documentoEntrada.Destinatario, ConfiguracaoEmbarcador, item.LocalArmazenamento))
                                throw new ControllerException(erroEstoqueNegativo);
                        }
                        
                    }
                }

                try
                {
                    repDocumentoEntrada.DeletarAbastecimentosEmAcerto(codigo);
                }
                catch (Exception e)
                {
                    throw new ControllerException(e.Message);
                }

                if (!svcTituloAPagar.AtualizarTitulos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                    throw new ControllerException(erro);

                if (!svcTituloAPagar.AtualizarGuias(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                    throw new ControllerException(erro);

                if (!svcDocumentoEntrada.ReverterCusto(documentoEntrada, unidadeDeTrabalho, Auditado, out erro, ConfiguracaoEmbarcador))
                    throw new ControllerException(erro);

                if (!svcDocumentoEntrada.ReverterEstoque(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro))
                    throw new ControllerException(erro);

                if (!svcDocumentoEntrada.ReverterAbastecimentos(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, Auditado))
                    throw new ControllerException(erro);

                if (!svcDocumentoEntrada.ReverterMovimentoFinanceiroDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                    throw new ControllerException(erro);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (!svcDocumentoEntrada.GerarMovimentoFinanceiroReversaoItens(documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada))
                        throw new ControllerException(erro);
                }

                if (!Servicos.Embarcador.Frota.OrdemServico.DesvincularDocumentoEntradaDoFechamento(documentoEntrada.Codigo, out erro, unidadeDeTrabalho))
                    throw new ControllerException(erro);

                if (!Servicos.Embarcador.Compras.OrdemCompra.EstornarOrdemCompra(documentoEntrada.Codigo, out erro, unidadeDeTrabalho, TipoServicoMultisoftware, _conexao.StringConexao, Auditado))
                    throw new ControllerException(erro);

                documentoEntrada.DataFinalizacao = null;
                SalvarLog(documentoEntrada, unidadeDeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Reverteu Faturamento.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reverter o faturamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa repDocumentoEntradaCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repDocumentoEntradaCentroResultadoTipoDespesa.BuscarPorDocumentoEntrada(codigo);

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);

                decimal saldoContaAdiantamento = 0m;
                if (ConfiguracaoEmbarcador.PlanoContaAdiantamentoFornecedor != null && documentoEntrada != null && documentoEntrada.Fornecedor != null && documentoEntrada.Situacao == SituacaoDocumentoEntrada.Aberto)
                    saldoContaAdiantamento = repMovimento.BuscarSaldoContaCliente(ConfiguracaoEmbarcador.PlanoContaAdiantamentoFornecedor.Codigo, documentoEntrada.Fornecedor.CPF_CNPJ);

                if (documentoEntrada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    documentoEntrada.BaseCalculoICMS,
                    documentoEntrada.BaseCalculoICMSST,
                    documentoEntrada.Codigo,

                    documentoEntrada.Chave,
                    DataEmissao = documentoEntrada.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                    DataEntrada = documentoEntrada.DataEntrada.ToString("dd/MM/yyyy"),
                    Especie = new
                    {
                        Codigo = documentoEntrada.Especie?.Codigo ?? 0,
                        Descricao = documentoEntrada.Especie?.Descricao ?? ""
                    },
                    Fornecedor = new
                    {
                        Codigo = documentoEntrada.Fornecedor.CPF_CNPJ,
                        Descricao = documentoEntrada.Fornecedor.Nome
                    },
                    StatusLancamento = new
                    {
                        Codigo = documentoEntrada.SituacaoLancamentoDocumentoEntrada?.Codigo ?? 0,
                        Descricao = documentoEntrada.SituacaoLancamentoDocumentoEntrada?.Descricao ?? ""
                    },
                    Destinatario = new
                    {
                        Codigo = documentoEntrada.Destinatario?.Codigo ?? 0,
                        Descricao = documentoEntrada.Destinatario?.RazaoSocial ?? string.Empty
                    },
                    CFOP = new
                    {
                        Codigo = documentoEntrada.CFOP?.Codigo,
                        Descricao = documentoEntrada.CFOP != null ? !string.IsNullOrWhiteSpace(documentoEntrada.CFOP.Extensao) ? documentoEntrada.CFOP.CodigoCFOP.ToString() + " (" + documentoEntrada.CFOP.Extensao + ") " + documentoEntrada.CFOP.Descricao : documentoEntrada.CFOP.CodigoCFOP.ToString() + " " + documentoEntrada.CFOP.Descricao : string.Empty
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = documentoEntrada.NaturezaOperacao?.Codigo ?? 0,
                        Descricao = documentoEntrada.NaturezaOperacao?.Descricao ?? string.Empty,
                        QuantidadeCFOPs = documentoEntrada.NaturezaOperacao?.CFOPs.Count ?? 0
                    },
                    documentoEntrada.IndicadorPagamento,
                    Modelo = new
                    {
                        Codigo = documentoEntrada.Modelo?.Codigo ?? 0,
                        Descricao = documentoEntrada.Modelo?.Descricao ?? "",
                        Numero = documentoEntrada.Modelo?.Numero ?? ""
                    },
                    DocumentoComMoedaEstrangeira = documentoEntrada.Modelo?.DocumentoComMoedaEstrangeira ?? false,
                    DocumentoMoedaCotacaoBancoCentral = documentoEntrada.Modelo?.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real,
                    OrdemServico = new
                    {
                        Codigo = documentoEntrada.OrdemServico?.Codigo ?? 0,
                        Descricao = documentoEntrada.OrdemServico != null ? documentoEntrada.OrdemServico.Numero + " (" + (documentoEntrada.OrdemServico.Veiculo?.Placa ?? string.Empty) + " " + (documentoEntrada.OrdemServico.Equipamento?.Descricao ?? string.Empty) + ") " + (documentoEntrada.OrdemServico.TipoOrdemServico?.Descricao ?? string.Empty) : string.Empty
                    },
                    OrdemCompra = new
                    {
                        Codigo = documentoEntrada.OrdemCompra?.Codigo ?? 0,
                        Descricao = documentoEntrada.OrdemCompra != null ? documentoEntrada.OrdemCompra.Numero.ToString() : string.Empty
                    },
                    KMAbastecimento = documentoEntrada.KMAbastecimento.ToString("n0"),
                    DataAbastecimento = documentoEntrada.DataAbastecimento != null && documentoEntrada.DataAbastecimento.Value > DateTime.MinValue ? documentoEntrada.DataAbastecimento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    documentoEntrada.Numero,
                    documentoEntrada.NumeroLancamento,
                    documentoEntrada.Serie,
                    documentoEntrada.Situacao,
                    documentoEntrada.EncerrarOrdemServico,
                    documentoEntrada.MoedaCotacaoBancoCentral,
                    DataBaseCRT = documentoEntrada.DataBaseCRT.HasValue ? documentoEntrada.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    ValorMoedaCotacao = documentoEntrada.ValorMoedaCotacao.ToString("n10"),
                    documentoEntrada.ValorTotal,
                    documentoEntrada.ValorTotalCOFINS,
                    documentoEntrada.ValorTotalDesconto,
                    documentoEntrada.ValorTotalFrete,
                    documentoEntrada.ValorTotalICMS,
                    documentoEntrada.ValorTotalICMSST,
                    documentoEntrada.ValorTotalIPI,
                    documentoEntrada.ValorTotalOutrasDespesas,
                    documentoEntrada.ValorTotalPIS,
                    documentoEntrada.ValorTotalCreditoPresumido,
                    documentoEntrada.ValorTotalDiferencial,
                    documentoEntrada.ValorTotalSeguro,
                    documentoEntrada.ValorTotalFreteFora,
                    documentoEntrada.ValorTotalOutrasDespesasFora,
                    documentoEntrada.ValorTotalDescontoFora,
                    documentoEntrada.ValorTotalImpostosFora,
                    documentoEntrada.ValorTotalDiferencialFreteFora,
                    documentoEntrada.ValorTotalICMSFreteFora,
                    documentoEntrada.ValorTotalCusto,
                    documentoEntrada.ValorTotalRetencaoCOFINS,
                    documentoEntrada.ValorTotalRetencaoCSLL,
                    documentoEntrada.ValorTotalRetencaoISS,
                    documentoEntrada.ValorTotalRetencaoIR,
                    documentoEntrada.ValorTotalRetencaoINSS,
                    documentoEntrada.ValorTotalRetencaoIPI,
                    documentoEntrada.ValorTotalRetencaoOutras,
                    documentoEntrada.ValorTotalRetencaoPIS,
                    documentoEntrada.ValorBruto,
                    documentoEntrada.BaseSTRetido,
                    documentoEntrada.ValorSTRetido,
                    documentoEntrada.ValorProdutos,
                    Horimetro = documentoEntrada.Horimetro.ToString("n0"),
                    documentoEntrada.ChaveNotaAnulacao,
                    documentoEntrada.Observacao,
                    documentoEntrada.TipoFrete,
                    documentoEntrada.Motivo,
                    TipoMovimento = new
                    {
                        Codigo = documentoEntrada.TipoMovimento?.Codigo ?? 0,
                        Descricao = documentoEntrada.TipoMovimento?.Descricao ?? string.Empty
                    },
                    Veiculo = new
                    {
                        Codigo = documentoEntrada.Veiculo?.Codigo ?? 0,
                        Descricao = documentoEntrada.Veiculo?.DescricaoComMarcaModelo ?? string.Empty
                    },
                    Equipamento = new
                    {
                        Codigo = documentoEntrada.Equipamento?.Codigo ?? 0,
                        Descricao = documentoEntrada.Equipamento?.DescricaoComMarcaModelo ?? string.Empty
                    },
                    Expedidor = new
                    {
                        Codigo = documentoEntrada.Expedidor?.CPF_CNPJ ?? 0,
                        Descricao = documentoEntrada.Expedidor?.Nome ?? string.Empty
                    },
                    Recebedor = new
                    {
                        Codigo = documentoEntrada.Recebedor?.CPF_CNPJ ?? 0,
                        Descricao = documentoEntrada.Recebedor?.Nome ?? string.Empty
                    },
                    ContratoFinanciamento = new
                    {
                        Codigo = documentoEntrada.ContratoFinanciamento?.Codigo ?? 0,
                        Descricao = documentoEntrada.ContratoFinanciamento?.Numero.ToString() ?? string.Empty
                    },
                    LocalidadeInicioPrestacao = new
                    {
                        Codigo = documentoEntrada.LocalidadeInicioPrestacao?.Codigo ?? 0,
                        Descricao = documentoEntrada.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty
                    },
                    LocalidadeTerminoPrestacao = new
                    {
                        Codigo = documentoEntrada.LocalidadeTerminoPrestacao?.Codigo ?? 0,
                        Descricao = documentoEntrada.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty
                    },
                    Servico = new { Codigo = documentoEntrada.Servico?.Codigo ?? 0, Descricao = documentoEntrada.Servico?.Descricao ?? string.Empty },
                    LocalidadePrestacaoServico = new { Codigo = documentoEntrada.LocalidadePrestacaoServico?.Codigo ?? 0, Descricao = documentoEntrada.LocalidadePrestacaoServico?.DescricaoCidadeEstado ?? string.Empty },
                    documentoEntrada.TipoDocumento,
                    CSTServico = documentoEntrada.CSTServico?.ToString("d") ?? string.Empty,
                    documentoEntrada.AliquotaSimplesNacional,
                    documentoEntrada.DocumentoFiscalProvenienteSimplesNacional,
                    documentoEntrada.TributaISSNoMunicipio,

                    Itens = (from obj in documentoEntrada.Itens
                             select new
                             {
                                 AliquotaICMS = new { val = obj.AliquotaICMS, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 AliquotaIPI = new { val = obj.AliquotaIPI, tipo = "decimal" },
                                 AliquotaCOFINS = new { val = obj.AliquotaCOFINS, tipo = "decimal" },
                                 AliquotaCreditoPresumido = new { val = obj.AliquotaCreditoPresumido, tipo = "decimal" },
                                 AliquotaDiferencial = new { val = obj.AliquotaDiferencial, tipo = "decimal" },
                                 AliquotaICMSST = new { val = obj.AliquotaICMSST, tipo = "decimal" },
                                 AliquotaPIS = new { val = obj.AliquotaPIS, tipo = "decimal" },
                                 BaseCalculoICMS = new { val = obj.BaseCalculoICMS, tipo = "decimal" },
                                 BaseCalculoICMSST = new { val = obj.BaseCalculoICMSST, tipo = "decimal" },
                                 BaseCalculoICMSFornecedor = new { val = obj.BaseCalculoICMSFornecedor, tipo = "decimal" },
                                 BaseCalculoIPI = new { val = obj.BaseCalculoIPI, tipo = "decimal" },
                                 BaseCalculoCOFINS = new { val = obj.BaseCalculoCOFINS, tipo = "decimal" },
                                 BaseCalculoCreditoPresumido = new { val = obj.BaseCalculoCreditoPresumido, tipo = "decimal" },
                                 BaseCalculoDiferencial = new { val = obj.BaseCalculoDiferencial, tipo = "decimal" },
                                 BaseCalculoPIS = new { val = obj.BaseCalculoPIS, tipo = "decimal" },
                                 NaturezaOperacao = new
                                 {
                                     Codigo = obj.NaturezaOperacao?.Codigo ?? 0,
                                     Descricao = obj.NaturezaOperacao?.Descricao ?? string.Empty
                                 },
                                 CFOP = new
                                 {
                                     Codigo = obj.CFOP?.Codigo ?? 0,
                                     Descricao = obj.CFOP != null ? obj.CFOP.CodigoCFOP + (!string.IsNullOrWhiteSpace(obj.CFOP.Extensao)? " ("+ obj.CFOP.Extensao + ") " : " ") + obj.CFOP.Descricao : string.Empty
                                 },
                                 obj.Codigo,
                                 CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                                 obj.CSTCOFINS,
                                 obj.CSTICMS,
                                 obj.CSTIPI,
                                 obj.CSTPIS,
                                 Desconto = new { val = obj.Desconto, tipo = "decimal" },
                                 ValorOutrasDespesas = new { val = obj.OutrasDespesas, tipo = "decimal" },
                                 OrdemServico = new
                                 {
                                     Codigo = obj.OrdemServico?.Codigo ?? 0,
                                     Descricao = obj.OrdemServico != null ? obj.OrdemServico.Numero + " (" + (obj.OrdemServico.Veiculo?.Placa ?? string.Empty) + " " + (obj.OrdemServico.Equipamento?.Descricao ?? string.Empty) + ") " + (obj.OrdemServico.TipoOrdemServico?.Descricao ?? string.Empty) : string.Empty
                                 },
                                 OrdemCompraMercadoria = new
                                 {
                                     Codigo = obj.OrdemCompraMercadoria?.Codigo ?? 0,
                                     Descricao = obj.OrdemCompraMercadoria != null ? obj.OrdemCompraMercadoria.OrdemCompra.Numero.ToString() + " (" + obj.OrdemCompraMercadoria.Produto.Descricao + ")" : string.Empty
                                 },
                                 RegraEntradaDocumento = new
                                 {
                                     Codigo = obj.RegraEntradaDocumento?.Codigo ?? 0,
                                     Descricao = obj.RegraEntradaDocumento != null ? obj.RegraEntradaDocumento.Descricao : string.Empty
                                 },
                                 Produto = new
                                 {
                                     Codigo = obj.Produto?.Codigo ?? 0,
                                     Descricao = obj.Produto?.Descricao ?? string.Empty
                                 },
                                 TipoMovimento = new
                                 {
                                     Codigo = obj.TipoMovimento?.Codigo ?? 0,
                                     Descricao = obj.TipoMovimento?.Descricao ?? string.Empty
                                 },
                                 Equipamento = new
                                 {
                                     Codigo = obj.Equipamento?.Codigo ?? 0,
                                     Descricao = obj.Equipamento?.DescricaoComMarcaModelo ?? string.Empty
                                 },
                                 Veiculo = new
                                 {
                                     Codigo = obj.Veiculo?.Codigo ?? 0,
                                     Descricao = obj.Veiculo?.DescricaoComMarcaModelo ?? string.Empty
                                 },
                                 Horimetro = obj.Horimetro,
                                 KMAbastecimento = obj.KMAbastecimento,
                                 DataAbastecimento = obj.DataAbastecimento != null && obj.DataAbastecimento.Value > DateTime.MinValue ? obj.DataAbastecimento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 ObservacaoItem = obj.Observacao ?? string.Empty,
                                 CodigoProdutoFornecedor = obj.CodigoProdutoFornecedor ?? string.Empty,
                                 DescricaoProdutoFornecedor = obj.DescricaoProdutoFornecedor ?? string.Empty,
                                 CodigoBarrasEAN = obj.CodigoBarrasEAN ?? string.Empty,
                                 NCMProdutoFornecedor = obj.NCMProdutoFornecedor ?? string.Empty,
                                 CESTProdutoFornecedor = obj.CESTProdutoFornecedor ?? string.Empty,
                                 Quantidade = new { val = obj.Quantidade, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 obj.Sequencial,
                                 obj.UnidadeMedida,
                                 NCM = obj.Produto?.CodigoNCM ?? string.Empty,
                                 SiglaUnidadeMedida = UnidadeDeMedidaHelper.ObterSigla(obj.UnidadeMedida),
                                 PercentualReducaoBaseCalculoCOFINS = new { val = obj.PercentualReducaoBaseCalculoCOFINS, tipo = "decimal" },
                                 PercentualReducaoBaseCalculoIPI = new { val = obj.PercentualReducaoBaseCalculoIPI, tipo = "decimal" },
                                 PercentualReducaoBaseCalculoPIS = new { val = obj.PercentualReducaoBaseCalculoPIS, tipo = "decimal" },
                                 ValorCOFINS = new { val = obj.ValorCOFINS, tipo = "decimal" },
                                 ValorFrete = new { val = obj.ValorFrete, tipo = "decimal" },
                                 ValorICMS = new { val = obj.ValorICMS, tipo = "decimal" },
                                 ValorICMSST = new { val = obj.ValorICMSST, tipo = "decimal" },
                                 BaseSTRetido = new { val = obj.BaseSTRetido, tipo = "decimal" },
                                 ValorSTRetido = new { val = obj.ValorSTRetido, tipo = "decimal" },
                                 ValorIPI = new { val = obj.ValorIPI, tipo = "decimal" },
                                 ValorPIS = new { val = obj.ValorPIS, tipo = "decimal" },
                                 ValorCreditoPresumido = new { val = obj.ValorCreditoPresumido, tipo = "decimal" },
                                 ValorDiferencial = new { val = obj.ValorDiferencial, tipo = "decimal" },
                                 ValorTotal = new { val = obj.ValorTotal, tipo = "decimal" },
                                 ValorUnitario = new { val = obj.ValorUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorSeguro = new { val = obj.ValorSeguro, tipo = "decimal" },
                                 ValorFreteFora = new { val = obj.ValorFreteFora, tipo = "decimal" },
                                 ValorOutrasDespesasFora = new { val = obj.ValorOutrasDespesasFora, tipo = "decimal" },
                                 ValorDescontoFora = new { val = obj.ValorDescontoFora, tipo = "decimal" },
                                 ValorImpostosFora = new { val = obj.ValorImpostosFora, tipo = "decimal" },
                                 ValorDiferencialFreteFora = new { val = obj.ValorDiferencialFreteFora, tipo = "decimal" },
                                 ValorICMSFreteFora = new { val = obj.ValorICMSFreteFora, tipo = "decimal" },
                                 ValorCustoUnitario = new { val = obj.ValorCustoUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorCustoTotal = new { val = obj.ValorCustoTotal, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorRetencaoCOFINS = new { val = obj.ValorRetencaoCOFINS, tipo = "decimal" },
                                 ValorRetencaoCSLL = new { val = obj.ValorRetencaoCSLL, tipo = "decimal" },
                                 ValorRetencaoISS = new { val = obj.ValorRetencaoISS, tipo = "decimal" },
                                 ValorRetencaoIR = new { val = obj.ValorRetencaoIR, tipo = "decimal" },
                                 ValorRetencaoINSS = new { val = obj.ValorRetencaoINSS, tipo = "decimal" },
                                 ValorRetencaoIPI = new { val = obj.ValorRetencaoIPI, tipo = "decimal" },
                                 ValorRetencaoOutras = new { val = obj.ValorRetencaoOutras, tipo = "decimal" },
                                 ValorRetencaoPIS = new { val = obj.ValorRetencaoPIS, tipo = "decimal" },
                                 CalculoCustoProduto = obj.CalculoCustoProduto ?? string.Empty,
                                 NumeroFogoInicial = obj.NumeroFogoInicial > 0 ? obj.NumeroFogoInicial.ToString() : string.Empty,
                                 TipoAquisicao = obj.TipoAquisicao.HasValue ? obj.TipoAquisicao.Value.ToString("d") : string.Empty,
                                 VidaAtual = obj.VidaAtual.HasValue ? obj.VidaAtual.Value.ToString("d") : string.Empty,
                                 Almoxarifado = new { Codigo = obj.Almoxarifado?.Codigo ?? 0, Descricao = obj.Almoxarifado?.Descricao ?? string.Empty },
                                 ProdutoVinculado = new { Codigo = obj.ProdutoVinculado?.Codigo ?? 0, Descricao = obj.ProdutoVinculado?.Descricao ?? string.Empty },
                                 QuantidadeProdutoVinculado = new { val = obj.QuantidadeProdutoVinculado, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 LocalArmazenamento = new { Codigo = obj.LocalArmazenamento?.Codigo ?? 0, Descricao = obj.LocalArmazenamento?.Descricao ?? string.Empty },
                                 UnidadeMedidaFornecedor = obj.UnidadeMedidaFornecedor ?? string.Empty,
                                 QuantidadeFornecedor = new { val = obj.QuantidadeFornecedor, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorUnitarioFornecedor = new { val = obj.ValorUnitarioFornecedor, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 CentroResultado = new { Codigo = obj.CentroResultado?.Codigo ?? 0, Descricao = obj.CentroResultado?.Descricao ?? string.Empty },
                                 OrigemMercadoria = obj.OrigemMercadoria.HasValue ? obj.OrigemMercadoria.Value.ToString("d") : string.Empty,
                                 CstIcmsFornecedor = obj.CstIcmsFornecedor ?? string.Empty,
                                 ValorICMSFornecedor = new { val = obj.ValorICMSFornecedor, tipo = "decimal" },
                                 CfopFornecedor = obj.CfopFornecedor ?? string.Empty,
                                 AliquotaICMSFornecedor = new { val = obj.AliquotaICMSFornecedor, tipo = "decimal" },

                                 EncerrarOrdemServico = obj.EncerrarOrdemServico,

                                 ItensAbastecimentos = (from objAbas in obj.Abastecimentos
                                                        select new
                                                        {
                                                            CodigoInterno = objAbas.Codigo,
                                                            objAbas.Abastecimento.Codigo,
                                                            CodigoItem = objAbas.DocumentoEntradaItem.Codigo,

                                                            CodigoEquipamento = objAbas.Abastecimento.Equipamento?.Codigo ?? 0,
                                                            objAbas.Abastecimento.Horimetro,
                                                            Placa = objAbas.Abastecimento.Veiculo != null ? objAbas.Abastecimento.Veiculo.Placa : string.Empty,
                                                            Posto = objAbas.Abastecimento.Posto != null ? objAbas.Abastecimento.Posto.Nome : string.Empty,
                                                            Data = objAbas.Abastecimento.Data != null ? objAbas.Abastecimento.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                            KM = objAbas.Abastecimento.Kilometragem.ToString("n0"),
                                                            Litros = objAbas.Abastecimento.Litros.ToString("n4")
                                                        }).ToList(),
                                 ItensOrdensServico = (from objAbas in obj.OrdensServico
                                                       select new
                                                       {
                                                           CodigoInterno = objAbas.Codigo,
                                                           objAbas.OrdemServico.Codigo,
                                                           CodigoItem = objAbas.DocumentoEntradaItem.Codigo,

                                                           objAbas.OrdemServico.Numero,
                                                           DataProgramada = objAbas.OrdemServico.DataProgramada.ToString("dd/MM/yyyy"),
                                                           Veiculo = objAbas.OrdemServico.Veiculo?.Placa ?? string.Empty,
                                                           Equipamento = objAbas.OrdemServico.Equipamento?.Descricao ?? string.Empty,
                                                           NumeroFrota = objAbas.OrdemServico.Veiculo?.NumeroFrota ?? string.Empty,
                                                           Motorista = objAbas.OrdemServico.Motorista?.Nome ?? string.Empty,
                                                           LocalManutencao = objAbas.OrdemServico.LocalManutencao?.Nome ?? string.Empty,
                                                           Operador = objAbas.OrdemServico.Operador.Nome,
                                                           TipoManutencao = objAbas.OrdemServico.DescricaoTipoManutencao,
                                                           Situacao = objAbas.OrdemServico.DescricaoSituacao
                                                       }).ToList()
                             }).ToList(),
                    Duplicatas = (from obj in documentoEntrada.Duplicatas
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Sequencia,
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      obj.Numero,
                                      Valor = new { val = obj.Valor, tipo = "decimal" },
                                      obj.CodigoTitulo,
                                      ValorTitulo = new { val = obj.ValorTitulo, tipo = "decimal" },
                                      obj.StatusTitulo,
                                      DataPagamento = obj.DataPagamentoTitulo.Date > DateTime.MinValue ? obj.DataPagamentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                      FormaTitulo = obj.Forma,
                                      NumeroBoleto = !string.IsNullOrWhiteSpace(obj.NumeroBoleto) ? obj.NumeroBoleto : string.Empty,
                                      Portador = new { Codigo = obj.Portador?.Codigo ?? 0, Descricao = obj.Portador?.Descricao ?? string.Empty },
                                      Observacao = !string.IsNullOrWhiteSpace(obj.Observacao) ? obj.Observacao : string.Empty,
                                  }).ToList(),
                    Guias = (from obj in documentoEntrada.Guias
                             select new
                             {
                                 obj.Codigo,
                                 DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                 obj.Numero,
                                 Valor = new { val = obj.Valor, tipo = "decimal" },
                                 CodigoTitulo = obj.CodigoTitulo,
                                 ValorTitulo = new { val = obj.ValorTitulo, tipo = "decimal" },
                                 StatusTitulo = obj.StatusTitulo,
                                 DataPagamento = obj.DataPagamentoTitulo.Date > DateTime.MinValue ? obj.DataPagamentoTitulo.ToString("dd/MM/yyyy") : string.Empty
                             }).ToList(),
                    CentroCustos = (from obj in documentoEntrada.LancamentosCentroResultado
                                    select new
                                    {
                                        obj.Codigo,
                                        CentroResultado = obj.CentroResultado != null ? obj.CentroResultado.Descricao : string.Empty,
                                        CodigoCentroResultado = obj.CentroResultado != null ? obj.CentroResultado.Codigo : 0,
                                        Percentual = new { val = obj.Percentual, tipo = "decimal" }
                                    }).ToList(),
                    CentrosResultadoTiposDespesa = (from obj in centrosResultadoTiposDespesa
                                                    select new
                                                    {
                                                        obj.Codigo,
                                                        CentroResultado = new { obj.CentroResultado.Codigo, obj.CentroResultado.Descricao },
                                                        TipoDespesa = new { obj.TipoDespesaFinanceira.Codigo, obj.TipoDespesaFinanceira.Descricao },
                                                        Percentual = new { val = obj.Percentual, tipo = "decimal" }
                                                    }).ToList(),
                    Veiculos = (from obj in documentoEntrada.Veiculos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Placa,
                                    ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao,
                                    obj.NumeroFrota
                                }).ToList(),
                    SaldoContaAdiantamento = saldoContaAdiantamento.ToString("n2"),

                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImportarOrdemCompraPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int sequencia = 1;

                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    BaseCalculoICMS = 0.0,
                    BaseCalculoICMSST = 0.0,
                    Codigo = Guid.NewGuid().ToString(),
                    Chave = "",
                    DataEmissao = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm"),
                    DataEntrada = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Especie = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    Fornecedor = new
                    {
                        Codigo = ordemCompra.Fornecedor.CPF_CNPJ,
                        Descricao = ordemCompra.Fornecedor.Nome
                    },
                    Destinatario = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    CFOP = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    IndicadorPagamento = IndicadorPagamentoDocumentoEntrada.APrazo,
                    Modelo = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    OrdemServico = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    OrdemCompra = new
                    {
                        Codigo = ordemCompra?.Codigo ?? 0,
                        Descricao = ordemCompra?.Numero.ToString() ?? string.Empty
                    },
                    KMAbastecimento = "",
                    DataAbastecimento = string.Empty,
                    Numero = "",
                    NumeroLancamento = "",
                    Serie = "",
                    Situacao = "",
                    ValorTotal = ordemCompra.Mercadorias.Select(o => o.ValorTotal).Sum(),
                    ValorTotalCOFINS = 0.0,
                    ValorTotalDesconto = 0.0,
                    ValorTotalFrete = 0.0,
                    ValorTotalICMS = 0.0,
                    ValorTotalICMSST = 0.0,
                    ValorTotalIPI = 0.0,
                    ValorTotalOutrasDespesas = 0.0,
                    ValorTotalPIS = 0.0,
                    ValorTotalCreditoPresumido = 0.0,
                    ValorTotalDiferencial = 0.0,
                    ValorTotalSeguro = 0.0,
                    ValorTotalFreteFora = 0.0,
                    ValorTotalOutrasDespesasFora = 0.0,
                    ValorTotalDescontoFora = 0.0,
                    ValorTotalImpostosFora = 0.0,
                    ValorTotalDiferencialFreteFora = 0.0,
                    ValorTotalICMSFreteFora = 0.0,
                    ValorTotalCusto = 0.0,
                    ValorTotalRetencaoCOFINS = 0.0,
                    ValorTotalRetencaoCSLL = 0.0,
                    ValorTotalRetencaoISS = 0.0,
                    ValorTotalRetencaoIR = 0.0,
                    ValorTotalRetencaoINSS = 0.0,
                    ValorTotalRetencaoIPI = 0.0,
                    ValorTotalRetencaoOutras = 0.0,
                    ValorTotalRetencaoPIS = 0.0,
                    ValorBruto = ordemCompra.Mercadorias.Select(o => o.ValorTotal).Sum(),
                    BaseSTRetido = 0.0,
                    ValorSTRetido = 0.0,
                    ValorProdutos = ordemCompra.Mercadorias.Select(o => o.ValorTotal).Sum(),
                    Horimetro = 0,
                    TipoMovimento = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    Veiculo = new
                    {
                        Codigo = ordemCompra.Veiculo?.Codigo ?? 0,
                        Descricao = ordemCompra.Veiculo?.Placa ?? string.Empty
                    },
                    Equipamento = new
                    {
                        Codigo = 0,
                        Descricao = ""
                    },
                    Observacao = string.Empty,
                    Motivo = string.Empty,
                    Itens = (from obj in ordemCompra.Mercadorias
                             select new
                             {
                                 AliquotaICMS = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 AliquotaIPI = new { val = 0.0, tipo = "decimal" },
                                 AliquotaCOFINS = new { val = 0.0, tipo = "decimal" },
                                 AliquotaCreditoPresumido = new { val = 0.0, tipo = "decimal" },
                                 AliquotaDiferencial = new { val = 0.0, tipo = "decimal" },
                                 AliquotaICMSST = new { val = 0.0, tipo = "decimal" },
                                 AliquotaPIS = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoICMS = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoICMSST = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoIPI = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoCOFINS = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoCreditoPresumido = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoDiferencial = new { val = 0.0, tipo = "decimal" },
                                 BaseCalculoPIS = new { val = 0.0, tipo = "decimal" },
                                 NaturezaOperacao = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 CFOP = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 Codigo = Guid.NewGuid().ToString(),
                                 CodigoProduto = obj.Produto?.Codigo ?? 0,
                                 CSTCOFINS = "",
                                 CSTICMS = "",
                                 CSTIPI = "",
                                 CSTPIS = "",
                                 Desconto = new { val = 0.0, tipo = "decimal" },
                                 ValorOutrasDespesas = new { val = 0.0, tipo = "decimal" },
                                 OrdemServico = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 OrdemCompraMercadoria = new
                                 {
                                     Codigo = obj.Codigo,
                                     Descricao = obj != null ? obj.OrdemCompra.Numero.ToString() + " (" + obj.Produto.Descricao + ")" : string.Empty
                                 },
                                 RegraEntradaDocumento = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 Produto = new
                                 {
                                     Codigo = obj.Produto?.Codigo ?? 0,
                                     Descricao = obj.Produto?.Descricao ?? string.Empty
                                 },
                                 TipoMovimento = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 Veiculo = new
                                 {
                                     Codigo = ordemCompra.Veiculo?.Codigo ?? 0,
                                     Descricao = ordemCompra.Veiculo?.Placa ?? string.Empty
                                 },
                                 Equipamento = new
                                 {
                                     Codigo = 0,
                                     Descricao = ""
                                 },
                                 Horimetro = 0,
                                 KMAbastecimento = "",
                                 DataAbastecimento = string.Empty,
                                 ObservacaoItem = "",
                                 CodigoProdutoFornecedor = "",
                                 DescricaoProdutoFornecedor = "",
                                 CodigoBarrasEAN = "",
                                 NCMProdutoFornecedor = "",
                                 CESTProdutoFornecedor = "",
                                 Quantidade = new { val = obj.Quantidade, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 Sequencial = (sequencia++),
                                 UnidadeMedida = obj.Produto.UnidadeDeMedida,
                                 NCM = obj.Produto?.CodigoNCM ?? string.Empty,
                                 SiglaUnidadeMedida = UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida),
                                 PercentualReducaoBaseCalculoCOFINS = new { val = 0.0, tipo = "decimal" },
                                 PercentualReducaoBaseCalculoIPI = new { val = 0.0, tipo = "decimal" },
                                 PercentualReducaoBaseCalculoPIS = new { val = 0.0, tipo = "decimal" },
                                 ValorCOFINS = new { val = 0.0, tipo = "decimal" },
                                 ValorFrete = new { val = 0.0, tipo = "decimal" },
                                 ValorICMS = new { val = 0.0, tipo = "decimal" },
                                 ValorICMSST = new { val = 0.0, tipo = "decimal" },
                                 BaseSTRetido = new { val = 0.0, tipo = "decimal" },
                                 ValorSTRetido = new { val = 0.0, tipo = "decimal" },
                                 ValorIPI = new { val = 0.0, tipo = "decimal" },
                                 ValorPIS = new { val = 0.0, tipo = "decimal" },
                                 ValorCreditoPresumido = new { val = 0.0, tipo = "decimal" },
                                 ValorDiferencial = new { val = 0.0, tipo = "decimal" },
                                 ValorTotal = new { val = obj.ValorTotal, tipo = "decimal" },
                                 ValorUnitario = new { val = obj.ValorUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorSeguro = new { val = 0.0, tipo = "decimal" },
                                 ValorFreteFora = new { val = 0.0, tipo = "decimal" },
                                 ValorOutrasDespesasFora = new { val = 0.0, tipo = "decimal" },
                                 ValorDescontoFora = new { val = 0.0, tipo = "decimal" },
                                 ValorImpostosFora = new { val = 0.0, tipo = "decimal" },
                                 ValorDiferencialFreteFora = new { val = 0.0, tipo = "decimal" },
                                 ValorICMSFreteFora = new { val = 0.0, tipo = "decimal" },
                                 ValorCustoUnitario = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorCustoTotal = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorRetencaoCOFINS = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoCSLL = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoISS = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoIR = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoINSS = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoIPI = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoOutras = new { val = 0.0, tipo = "decimal" },
                                 ValorRetencaoPIS = new { val = 0.0, tipo = "decimal" },
                                 CalculoCustoProduto = obj.Produto.CalculoCustoProduto ?? string.Empty,
                                 NumeroFogoInicial = string.Empty,
                                 TipoAquisicao = string.Empty,
                                 VidaAtual = string.Empty,
                                 Almoxarifado = new { Codigo = 0, Descricao = string.Empty },
                                 ProdutoVinculado = new { Codigo = 0, Descricao = string.Empty },
                                 QuantidadeProdutoVinculado = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 LocalArmazenamento = new { Codigo = 0, Descricao = string.Empty },
                                 UnidadeMedidaFornecedor = string.Empty,
                                 QuantidadeFornecedor = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 ValorUnitarioFornecedor = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                                 CentroResultado = new
                                 {
                                     Codigo = ordemCompra.Veiculo?.CentroResultado?.Codigo ?? 0,
                                     Descricao = ordemCompra.Veiculo?.CentroResultado?.Descricao ?? string.Empty
                                 },
                                 OrigemMercadoria = obj.Produto?.OrigemMercadoria ?? OrigemMercadoria.Origem0,
                                 EncerrarOrdemServico = false
                             }).ToList(),
                    Duplicatas = new List<dynamic>(),
                    Guias = new List<dynamic>(),
                    CentroCustos = new List<dynamic>(),
                    Veiculos = new List<dynamic>()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao importar de Ordem de Compra.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Excluir))
                        return new JsonpResult(false, true, "Você não possui permissões para excluir o documento de entrada.");

                unidadeDeTrabalho.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo, true);

                if (documentoEntrada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Aberto)
                    return new JsonpResult(false, true, "Não é possível excluir o documento de entrada nessa situação");

                repDocumentoEntrada.Deletar(documentoEntrada, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesParticipantes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDestinatario;
                int.TryParse(Request.Params("Destinatario"), out codigoDestinatario);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoDestinatario = this.Usuario.Empresa.Codigo;

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Fornecedor")), out double cpfCnpjFornecedor);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCodigo(codigoDestinatario);
                Dominio.Entidades.Cliente fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor);

                if (fornecedor == null || destinatario == null)
                    return new JsonpResult(false, true, "Selecione o remetente e destinatário para o cálculo dos impostos.");

                return new JsonpResult(new
                {
                    Interestadual = fornecedor.Localidade.Estado.Sigla != destinatario.Localidade.Estado.Sigla
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados dos participantes da nota fiscal.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ObterDadosNFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_ImportarXML))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.DTO.CustomFile file = HttpContext.GetFile();

                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xml")
                    return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xml.");
                
                object notaFiscal = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);
                if (notaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível realizar a leitura da NF-e.");

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                string documentoImportadoXML = ObterConteudoXML(file);
                object documento = svcDocumentoEntrada.ObterDetalhesPorNFe(notaFiscal, Empresa, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador.DataEntradaDocumentoEntrada, documentoImportadoXML);
                if (documento == null)
                    return new JsonpResult(false, true, "Versão da NF-e não foi localizada.");


                return new JsonpResult(documento);
            }
            catch (ServicoException se)
            {
                return new JsonpResult(false, true, se.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados da NF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ObterDadosNFSeCuritiba()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_ImportarXML))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.DTO.CustomFile file = HttpContext.GetFile();

                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xml")
                    return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xml.");

                var nfse = Servicos.Embarcador.Integracao.NFSe.Curitiba.Ler(file.InputStream);
                if (nfse == null)
                    return new JsonpResult(false, true, "Não foi possível realizar a leitura da NFS-e de Curitiba.");

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);

                object documento = svcDocumentoEntrada.ObterDetalhesPorNFseCuritiba(nfse, Empresa, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador.DataEntradaDocumentoEntrada);
                if (documento == null)
                    return new JsonpResult(false, true, "Versão do NFS-e de Curitiba não foi localizada.");

                return new JsonpResult(documento);
            }
            catch (ServicoException se)
            {
                return new JsonpResult(false, true, se.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados da NFS-e de Curitiba.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ObterDadosCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_ImportarXML))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.DTO.CustomFile file = HttpContext.GetFile();
                
                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xml")
                    return new JsonpResult(false, "Extensão do arquivo inválida. Selecione um arquivo com a extensão .xml.");

                object conhecimento = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                if (conhecimento == null)
                    return new JsonpResult(false, true, "Não foi possível realizar a leitura do CT-e.");

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                string documentoImportadoXML = ObterConteudoXML(file);
                object documento = svcDocumentoEntrada.ObterDetalhesPorCTe(conhecimento, Empresa, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador.DataEntradaDocumentoEntrada, documentoImportadoXML);
                if (documento == null)
                    return new JsonpResult(false, true, "Versão do CT-e não foi localizada.");

                return new JsonpResult(documento);
            }
            catch (ServicoException se)
            {
                return new JsonpResult(false, true, se.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados da CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarEmpresaUsuario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                var retorno = new
                {
                    Codigo = this.Usuario.Empresa.Codigo,
                    Nome = this.Usuario.Empresa.RazaoSocial
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar a empresa logada");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarLancamentoDocumento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Int64 numero;
                Int64.TryParse(Request.Params("Numero"), out numero);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                string serieDocumentoEntrada = Request.GetStringParam("Serie");

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);

                double cpfCnpjFornecedor = 0d;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Fornecedor")), out cpfCnpjFornecedor);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento = codigo > 0 ? repDocumentoEntrada.BuscarPorCodigo(codigo) : null;

                if (documento == null)
                    documento = repDocumentoEntrada.BuscarPorFornecedorENumeroESerie(numero, cpfCnpjFornecedor, serieDocumentoEntrada);

                if (documento != null)
                    return new JsonpResult(false, true, "O documento informado já se encontra cadastrado no sistema. Número do lançamento: " + documento.NumeroLancamento + ".");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar documento lançado anteriormente");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXMLNFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo);

                if (documentoEntrada.DocumentoImportadoXML != null)
                {
                    byte[] arquivoXML = Encoding.UTF8.GetBytes(documentoEntrada.DocumentoImportadoXML);

                    return Arquivo(arquivoXML, "text/xml", $"{documentoEntrada.Chave}.xml");
                }
                else
                {

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !documentoEntrada.Destinatario.HabilitaSincronismoDocumentosDestinados)
                        return new JsonpResult(false, true, "Você não possui o módulo de Documentos Destinados para visualização de NF-e.");

                    string chave = Utilidades.String.OnlyNumbers(documentoEntrada.Chave);
                    if (string.IsNullOrWhiteSpace(chave) || !Utilidades.Validate.ValidarChave(chave))
                        return new JsonpResult(false, true, "A chave informada está inválida.");

                    string tipoDocumento = chave.Substring(20, 2);
                    if (tipoDocumento != "55")
                        return new JsonpResult(false, true, "A chave informada não é de nota fiscal eletrônica.");

                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documentoEntrada.Destinatario.Codigo,
                        new TipoDocumentoDestinadoEmpresa[] { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }, chave);

                    if (documentoDestinado == null)
                        return new JsonpResult(false, true, "Documento Destinado não disponível.");

                    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                    else
                    {
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                        else
                            return new JsonpResult(false, true, "Não foi possível realizar o download do XML da NF-e da SEFAZ. Dica: Verifique se o documento destinado possui manifestação e, se possui, aguarde, pois a SEFAZ pode demorar até 24h da data de manifestação para liberar o documento para download.");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDANFENFeDestinados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigo);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !documentoEntrada.Destinatario.HabilitaSincronismoDocumentosDestinados)
                    return new JsonpResult(false, true, "Você não possui o módulo de Documentos Destinados para visualização de NF-e.");

                string chave = Utilidades.String.OnlyNumbers(documentoEntrada.Chave);
                if (string.IsNullOrWhiteSpace(chave) || !Utilidades.Validate.ValidarChave(chave))
                    return new JsonpResult(false, true, "A chave informada está inválida.");

                string tipoDocumento = chave.Substring(20, 2);
                if (tipoDocumento != "55")
                    return new JsonpResult(false, true, "A chave informada não é de nota fiscal eletrônica.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", chave + ".pdf");
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documentoEntrada.Destinatario.Codigo,
                        new TipoDocumentoDestinadoEmpresa[] { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }, chave);

                    if (documentoDestinado == null)
                        return new JsonpResult(false, true, "Documento Destinado não disponível.");

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    {
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            return new JsonpResult(false, "Não foi possível realizar o download do XML da NF-e da SEFAZ. Dica: Verifique se o documento destinado possui manifestação.");
                    }

                    Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    string retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unidadeDeTrabalho);

                    if (string.IsNullOrWhiteSpace(retorno))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                    else
                        return new JsonpResult(false, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unidadeDeTrabalho);

            int.TryParse(Request.Params("Especie"), out int codigoEspecie);
            int.TryParse(Request.Params("Modelo"), out int codigoModelo);
            int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
            int.TryParse(Request.Params("Equipamento"), out int codigoEquipamento);
            int.TryParse(Request.Params("Destinatario"), out int codigoDestinatario);
            int.TryParse(Request.Params("CFOP"), out int codigoCFOP);
            int.TryParse(Request.Params("NaturezaOperacao"), out int codigoNaturezaOperacao);
            int.TryParse(Request.Params("OrdemServico"), out int codigoOrdemServico);
            int.TryParse(Request.Params("OrdemCompra"), out int codigoOrdemCompra);
            int.TryParse(Request.Params("TipoMovimento"), out int codigoTipoMovimento);
            int.TryParse(Request.Params("KMAbastecimento"), out int kmAbastecimento);
            int.TryParse(Request.Params("ContratoFinanciamento"), out int codigoContratoFinanciamento);
            int codigoLocalidadeInicioPrestacao = Request.GetIntParam("LocalidadeInicioPrestacao");
            int codigoLocalidadeTerminoPrestacao = Request.GetIntParam("LocalidadeTerminoPrestacao");
            int codigoServico = Request.GetIntParam("Servico");
            int codigoLocalidadePrestacaoServico = Request.GetIntParam("LocalidadePrestacaoServico");
            int.TryParse(Request.Params("PesquisaStatusLancamento"), out int situacaoLancamentoDocumentoEntrada);


            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Expedidor")), out double cpfCnpjExpedidor);
            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Recebedor")), out double cpfCnpjRecebedor);

            decimal.TryParse(Request.Params("ValorTotal"), out decimal valorTotal);
            decimal.TryParse(Request.Params("ValorTotalDesconto"), out decimal valorTotalDesconto);
            decimal.TryParse(Request.Params("ValorTotalOutrasDespesas"), out decimal valorTotalOutrasDespesas);
            decimal.TryParse(Request.Params("ValorTotalFrete"), out decimal valorTotalFrete);
            decimal.TryParse(Request.Params("BaseCalculoICMS"), out decimal baseCalculoICMS);
            decimal.TryParse(Request.Params("ValorTotalICMS"), out decimal valorTotalICMS);
            decimal.TryParse(Request.Params("BaseCalculoICMSST"), out decimal baseCalculoICMSST);
            decimal.TryParse(Request.Params("ValorTotalICMSST"), out decimal valorTotalICMSST);
            decimal.TryParse(Request.Params("ValorTotalIPI"), out decimal valorTotalIPI);
            decimal.TryParse(Request.Params("ValorTotalPIS"), out decimal valorTotalPIS);
            decimal.TryParse(Request.Params("ValorTotalCOFINS"), out decimal valorTotalCOFINS);
            decimal.TryParse(Request.Params("ValorTotalCreditoPresumido"), out decimal valorTotalCreditoPresumido);
            decimal.TryParse(Request.Params("ValorTotalDiferencial"), out decimal valorTotalDiferencial);
            decimal.TryParse(Request.Params("ValorTotalSeguro"), out decimal valorTotalSeguro);
            decimal.TryParse(Request.Params("ValorTotalFreteFora"), out decimal valorTotalFreteFora);
            decimal.TryParse(Request.Params("ValorTotalOutrasDespesasFora"), out decimal valorTotalOutrasDespesasFora);
            decimal.TryParse(Request.Params("ValorTotalDescontoFora"), out decimal valorTotalDescontoFora);
            decimal.TryParse(Request.Params("ValorTotalImpostosFora"), out decimal valorTotalImpostosFora);
            decimal.TryParse(Request.Params("ValorTotalDiferencialFreteFora"), out decimal valorTotalDiferencialFreteFora);
            decimal.TryParse(Request.Params("ValorTotalICMSFreteFora"), out decimal valorTotalICMSFreteFora);
            decimal.TryParse(Request.Params("ValorTotalCusto"), out decimal valorTotalCusto);
            decimal.TryParse(Request.Params("ValorTotalRetencaoPIS"), out decimal valorTotalRetencaoPIS);
            decimal.TryParse(Request.Params("ValorTotalRetencaoCOFINS"), out decimal valorTotalRetencaoCOFINS);
            decimal.TryParse(Request.Params("ValorTotalRetencaoIPI"), out decimal valorTotalRetencaoIPI);
            decimal.TryParse(Request.Params("ValorTotalRetencaoINSS"), out decimal valorTotalRetencaoINSS);
            decimal.TryParse(Request.Params("ValorTotalRetencaoCSLL"), out decimal valorTotalRetencaoCSLL);
            decimal.TryParse(Request.Params("ValorTotalRetencaoIR"), out decimal valorTotalRetencaoIR);
            decimal.TryParse(Request.Params("ValorTotalRetencaoISS"), out decimal valorTotalRetencaoISS);
            decimal.TryParse(Request.Params("ValorTotalRetencaoOutras"), out decimal valorTotalRetencaoOutras);
            decimal.TryParse(Request.Params("ValorBruto"), out decimal valorBruto);
            decimal.TryParse(Request.Params("BaseSTRetido"), out decimal baseSTRetido);
            decimal.TryParse(Request.Params("ValorSTRetido"), out decimal valorSTRetido);
            decimal.TryParse(Request.Params("ValorProdutos"), out decimal valorProdutos);

            Enum.TryParse(Request.Params("IndicadorPagamento"), out IndicadorPagamentoDocumentoEntrada indicadorPagamento);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoDestinatario = this.Usuario.Empresa.Codigo;

            documentoEntrada.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            documentoEntrada.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
            documentoEntrada.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");

            documentoEntrada.BaseCalculoICMS = baseCalculoICMS;
            documentoEntrada.BaseCalculoICMSST = baseCalculoICMSST;
            documentoEntrada.Especie = repEspecie.BuscarPorCodigo(codigoEspecie);
            documentoEntrada.IndicadorPagamento = indicadorPagamento;
            documentoEntrada.Modelo = repModelo.BuscarPorId(codigoModelo);
            documentoEntrada.ValorTotal = valorTotal;
            documentoEntrada.ValorTotalCOFINS = valorTotalCOFINS;
            documentoEntrada.ValorTotalDesconto = valorTotalDesconto;
            documentoEntrada.ValorTotalFrete = valorTotalFrete;
            documentoEntrada.ValorTotalICMS = valorTotalICMS;
            documentoEntrada.ValorTotalICMSST = valorTotalICMSST;
            documentoEntrada.ValorTotalIPI = valorTotalIPI;
            documentoEntrada.ValorTotalOutrasDespesas = valorTotalOutrasDespesas;
            documentoEntrada.ValorTotalPIS = valorTotalPIS;
            documentoEntrada.ValorTotalCreditoPresumido = valorTotalCreditoPresumido;
            documentoEntrada.ValorTotalDiferencial = valorTotalDiferencial;
            documentoEntrada.ValorTotalSeguro = valorTotalSeguro;
            documentoEntrada.ValorTotalFreteFora = valorTotalFreteFora;
            documentoEntrada.ValorTotalOutrasDespesasFora = valorTotalOutrasDespesasFora;
            documentoEntrada.ValorTotalDescontoFora = valorTotalDescontoFora;
            documentoEntrada.ValorTotalImpostosFora = valorTotalImpostosFora;
            documentoEntrada.ValorTotalDiferencialFreteFora = valorTotalDiferencialFreteFora;
            documentoEntrada.ValorTotalICMSFreteFora = valorTotalICMSFreteFora;
            documentoEntrada.ValorTotalCusto = valorTotalCusto;
            documentoEntrada.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            documentoEntrada.Equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
            documentoEntrada.Horimetro = Request.GetIntParam("Horimetro");
            documentoEntrada.KMAbastecimento = kmAbastecimento;
            documentoEntrada.CFOP = codigoCFOP > 0 ? repCFOP.BuscarPorCodigo(codigoCFOP) : null;
            documentoEntrada.Destinatario = codigoDestinatario > 0 ? repEmpresa.BuscarPorCodigo(codigoDestinatario) : null;
            documentoEntrada.NaturezaOperacao = codigoNaturezaOperacao > 0 ? repNaturezaOperacao.BuscarPorId(codigoNaturezaOperacao) : null;
            documentoEntrada.OrdemServico = codigoOrdemServico > 0 ? repOrdemServico.BuscarPorCodigo(codigoOrdemServico) : null;
            documentoEntrada.OrdemCompra = codigoOrdemCompra > 0 ? repOrdemCompra.BuscarPorCodigo(codigoOrdemCompra) : null;
            documentoEntrada.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
            documentoEntrada.ValorTotalRetencaoCOFINS = valorTotalRetencaoCOFINS;
            documentoEntrada.ValorTotalRetencaoCSLL = valorTotalRetencaoCSLL;
            documentoEntrada.ValorTotalRetencaoISS = valorTotalRetencaoISS;
            documentoEntrada.ValorTotalRetencaoIR = valorTotalRetencaoIR;
            documentoEntrada.ValorTotalRetencaoINSS = valorTotalRetencaoINSS;
            documentoEntrada.ValorTotalRetencaoIPI = valorTotalRetencaoIPI;
            documentoEntrada.ValorTotalRetencaoOutras = valorTotalRetencaoOutras;
            documentoEntrada.ValorTotalRetencaoPIS = valorTotalRetencaoPIS;
            documentoEntrada.ValorProdutos = valorProdutos;
            documentoEntrada.ValorBruto = valorBruto;
            documentoEntrada.BaseSTRetido = baseSTRetido;
            documentoEntrada.ValorSTRetido = valorSTRetido;
            documentoEntrada.DataAlteracao = DateTime.Now;
            documentoEntrada.ChaveNotaAnulacao = Request.GetStringParam("ChaveNotaAnulacao");
            documentoEntrada.Observacao = Request.GetStringParam("Observacao");
            documentoEntrada.Expedidor = cpfCnpjExpedidor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor) : null;
            documentoEntrada.Recebedor = cpfCnpjRecebedor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;
            documentoEntrada.ContratoFinanciamento = codigoContratoFinanciamento > 0 ? repContratoFinanciamento.BuscarPorCodigo(codigoContratoFinanciamento) : null;
            documentoEntrada.TipoFrete = Request.GetNullableEnumParam<Dominio.Enumeradores.ModalidadeFrete>("TipoFrete");
            documentoEntrada.LocalidadeInicioPrestacao = codigoLocalidadeInicioPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeInicioPrestacao) : null;
            documentoEntrada.LocalidadeTerminoPrestacao = codigoLocalidadeTerminoPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao) : null;
            documentoEntrada.Motivo = Request.GetStringParam("Motivo");
            documentoEntrada.Servico = codigoServico > 0 ? repServico.BuscarPorCodigo(codigoServico) : null;
            documentoEntrada.LocalidadePrestacaoServico = codigoLocalidadePrestacaoServico > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadePrestacaoServico) : null;
            documentoEntrada.TipoDocumento = Request.GetNullableEnumParam<TipoDocumentoServico>("TipoDocumento");
            documentoEntrada.CSTServico = Request.GetNullableEnumParam<CSTServico>("CSTServico");
            documentoEntrada.AliquotaSimplesNacional = Request.GetDecimalParam("AliquotaSimplesNacional");
            documentoEntrada.DocumentoFiscalProvenienteSimplesNacional = Request.GetBoolParam("DocumentoFiscalProvenienteSimplesNacional");
            documentoEntrada.TributaISSNoMunicipio = Request.GetBoolParam("TributaISSNoMunicipio");

            documentoEntrada.SituacaoLancamentoDocumentoEntrada = situacaoLancamentoDocumentoEntrada > 0 ? repSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(situacaoLancamentoDocumentoEntrada) : null;
        }

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Lançamento", "NumeroLancamento", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Núm. Doc.", "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Entrada", "DataEntrada", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Categoria", "Categoria", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Empresa/Filial", "Destinatario", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor Total", "ValorTotal", 12, Models.Grid.Align.right, true);

            if (!filtrosPesquisa.Situacao.HasValue)
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho("Natureza da Operação", "DescricaNaturezaOperacao", 12, Models.Grid.Align.left, false);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                grid.AdicionarCabecalho("Tipo Movimento", "DescricaoTipoMovimento", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Status Financeiro", "StatusFinanceiro", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status Lançamento", "SituacaoLancamentoDocumentoEntrada", 12, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);

            int countDocumentosEntrada = repDocumentoEntrada.ContarConsulta(filtrosPesquisa);

            if (exportacao && countDocumentosEntrada > 5000)
                return new JsonpResult(false, true, "A quantidade de registros para exportação não pode ser superior a 5000.");

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntrada = repDocumentoEntrada.Consultar(filtrosPesquisa, parametrosConsulta);
            grid.setarQuantidadeTotal(countDocumentosEntrada);

            var retorno = (from obj in listaDocumentoEntrada
                           select new
                           {
                               obj.Codigo,
                               obj.NumeroLancamento,
                               obj.Numero,
                               DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                               DataEntrada = obj.DataEntrada.ToString("dd/MM/yyyy"),
                               Fornecedor = obj.Fornecedor != null ? obj.Fornecedor.CPF_CNPJ_Formatado + " - " + obj.Fornecedor.Nome : string.Empty,
                               SituacaoLancamentoDocumentoEntrada = obj.SituacaoLancamentoDocumentoEntrada != null ? obj.SituacaoLancamentoDocumentoEntrada.Descricao : string.Empty,
                               Categoria = obj.Fornecedor?.Categoria?.Descricao ?? string.Empty,
                               Destinatario = obj.Destinatario?.Descricao ?? string.Empty,
                               ValorTotal = obj.ValorTotal.ToString("n2"),
                               DescricaoSituacao = obj.Situacao.ObterDescricao(),
                               DescricaNaturezaOperacao = obj.NaturezaOperacao != null ? obj.NaturezaOperacao.Descricao : string.Empty,
                               DescricaoTipoMovimento = obj.TipoMovimento != null ? obj.TipoMovimento.Descricao : string.Empty,
                               CFOP = obj.CFOP != null ? obj.CFOP.CodigoCFOP.ToString("n0") : string.Empty,
                               StatusFinanceiro = obj.Situacao == SituacaoDocumentoEntrada.Finalizado && obj.ContratoFinanciamento != null ? "Contrato de Financiamento" :
                                                    obj.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.AVista && obj.Situacao == SituacaoDocumentoEntrada.Finalizado ? "Pago" : obj.StatusFinanceiro,
                               DT_RowClass = ObterCorSituacaoDocumentoEntrada(obj, exportacao)
                           }).ToList();

            grid.AdicionaRows(retorno);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }
        }

        private string ObterCorSituacaoDocumentoEntrada(DocumentoEntradaTMS documentoEntrada, bool exportacao)
        {
            if (exportacao)
                return null;

            if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Aberto)
                return ClasseCorFundo.Warning(IntensidadeCor._100);

            if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Cancelado)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Anulado)
                return ClasseCorFundo.Fusion(IntensidadeCor._100);

            return ClasseCorFundo.Sucess(IntensidadeCor._100);
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataEntradaInicial = Request.GetDateTimeParam("DataEntradaInicial"),
                DataEntradaFinal = Request.GetDateTimeParam("DataEntradaFinal"),
                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                CpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor"),
                NumeroLancamentoInicial = Request.GetIntParam("NumeroLancamentoInicial"),
                NumeroLancamentoFinal = Request.GetIntParam("NumeroLancamentoFinal"),
                NumeroDocumentoInicial = Request.GetLongParam("NumeroDocumentoInicial"),
                NumeroDocumentoFinal = Request.GetLongParam("NumeroDocumentoFinal"),
                CodigoNaturezaOperacao = Request.GetIntParam("NaturezaOperacao"),
                CodigoCFOP = Request.GetIntParam("CFOP"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoDestinatario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : Request.GetIntParam("Destinatario"),
                Situacao = Request.GetNullableEnumParam<SituacaoDocumentoEntrada>("Situacao"),
                Chave = Request.GetStringParam("Chave"),
                StatusFinanceiro = Request.GetNullableEnumParam<StatusFinanceiroDocumentoEntrada>("StatusFinanceiro"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoCategoria = Request.GetIntParam("Categoria"),
                CodigoStatusLancamento = Request.GetIntParam("PesquisaStatusLancamento"),
                NumeroFogo = Request.GetIntParam("NumeroFogo")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Fornecedor")
                return propriedadeOrdenar += ".Nome";

            return propriedadeOrdenar;
        }

        private bool SalvarItens(out string erro, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unidadeDeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento repDocumentoEntradaItemAbastecimento = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico repDocumentoEntradaItemOrdemServico = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unidadeDeTrabalho);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unidadeDeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = documentoEntrada.OrdemCompra != null ? repOrdemCompra.BuscarPorCodigo(documentoEntrada.OrdemCompra.Codigo) : null;
            List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> ordemCompraMercadoria = ordemCompra != null ? repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo) : null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            dynamic itens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Itens"));
            dynamic itensAbastecimentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensAbastecimentos"));
            dynamic itensOrdensServico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensOrdensServico"));

            if (documentoEntrada.Itens != null && documentoEntrada.Itens.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var item in itens)
                    if (item.Codigo != null)
                    {
                        int.TryParse((string)item.Codigo, out int codigo);
                        if (codigo > 0)
                            codigos.Add((int)item.Codigo);
                    }

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDeletar = (from obj in documentoEntrada.Itens where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < itensDeletar.Count; i++)
                    repItem.Deletar(itensDeletar[i], Auditado);
            }
            else
                documentoEntrada.Itens = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            foreach (dynamic item in itens)
            {
                int codigo = 0, codigoOrdemServico = 0, codigoOrdemCompraMercadoria, codigoTipoMovimento = 0, codigoVeiculo = 0, codigoEquipamento = 0, codigoRegraEntradaDocumento;
                int.TryParse((string)item.Codigo, out codigo);

                codigoOrdemServico = item.OrdemServico != null ? (int)item.OrdemServico.Codigo : 0;
                codigoOrdemCompraMercadoria = item.OrdemCompraMercadoria != null ? (int)item.OrdemCompraMercadoria.Codigo : 0;

                codigoRegraEntradaDocumento = item.RegraEntradaDocumento != null ? (int)item.RegraEntradaDocumento.Codigo : 0;
                codigoTipoMovimento = item.TipoMovimento != null ? (int)item.TipoMovimento.Codigo : 0;
                codigoVeiculo = item.Veiculo != null ? (int)item.Veiculo.Codigo : 0;
                codigoEquipamento = item.Equipamento != null ? (int)item.Equipamento.Codigo : 0;

                DateTime dataAbsAux;
                DateTime? dataAbastecimento = null;
                DateTime.TryParseExact((string)item.DataAbastecimento, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataAbsAux);
                if (dataAbsAux > DateTime.MinValue)
                    dataAbastecimento = dataAbsAux;

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem itemDoc = codigo > 0 ? repItem.BuscarPorCodigo(codigo) : null;

                if (itemDoc == null)
                    itemDoc = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem();
                else
                    itemDoc.Initialize();

                itemDoc.DocumentoEntrada = documentoEntrada;
                itemDoc.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                itemDoc.Equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
                itemDoc.Horimetro = item.Horimetro != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers((string)item.Horimetro)) ? int.Parse(Utilidades.String.OnlyNumbers((string)item.Horimetro)) : 0;
                itemDoc.KMAbastecimento = item.KMAbastecimento != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers((string)item.KMAbastecimento)) ? int.Parse(Utilidades.String.OnlyNumbers((string)item.KMAbastecimento)) : 0;
                itemDoc.DataAbastecimento = dataAbastecimento != null && dataAbastecimento.Value > DateTime.MinValue ? dataAbastecimento : null;
                itemDoc.AliquotaICMS = item.AliquotaICMS != null ? (decimal)item.AliquotaICMS : 0m;
                itemDoc.AliquotaIPI = item.AliquotaIPI != null ? (decimal)item.AliquotaIPI : 0m;
                itemDoc.AliquotaCOFINS = item.AliquotaCOFINS != null ? (decimal)item.AliquotaCOFINS : 0m;
                itemDoc.AliquotaCreditoPresumido = item.AliquotaCreditoPresumido != null ? (decimal)item.AliquotaCreditoPresumido : 0m;
                itemDoc.AliquotaDiferencial = item.AliquotaDiferencial != null ? (decimal)item.AliquotaDiferencial : 0m;
                itemDoc.AliquotaICMSST = item.AliquotaICMSST != null ? (decimal)item.AliquotaICMSST : 0m;
                itemDoc.AliquotaPIS = item.AliquotaPIS != null ? (decimal)item.AliquotaPIS : 0m;
                itemDoc.BaseCalculoICMS = item.BaseCalculoICMS != null ? (decimal)item.BaseCalculoICMS : 0m;
                itemDoc.BaseCalculoICMSST = item.BaseCalculoICMSST != null ? (decimal)item.BaseCalculoICMSST : 0m;
                itemDoc.BaseCalculoIPI = item.BaseCalculoIPI != null ? (decimal)item.BaseCalculoIPI : 0m;
                itemDoc.BaseCalculoCOFINS = item.BaseCalculoCOFINS != null ? (decimal)item.BaseCalculoCOFINS : 0m;
                itemDoc.BaseCalculoCreditoPresumido = item.BaseCalculoCreditoPresumido != null ? (decimal)item.BaseCalculoCreditoPresumido : 0m;
                itemDoc.BaseCalculoDiferencial = item.BaseCalculoDiferencial != null ? (decimal)item.BaseCalculoDiferencial : 0m;
                itemDoc.BaseCalculoPIS = item.BaseCalculoPIS != null ? (decimal)item.BaseCalculoPIS : 0m;
                itemDoc.CFOP = repCFOP.BuscarPorCodigo((int)item.CFOP.Codigo);
                itemDoc.NaturezaOperacao = repNaturezaOperacao.BuscarPorId((int)item.NaturezaOperacao.Codigo);
                itemDoc.CSTICMS = (string)item.CSTICMS;
                itemDoc.CSTCOFINS = (string)item.CSTCOFINS;
                itemDoc.CSTIPI = (string)item.CSTIPI;
                itemDoc.CSTPIS = (string)item.CSTPIS;
                itemDoc.Desconto = item.Desconto != null ? (decimal)item.Desconto : 0m;
                itemDoc.OrdemServico = codigoOrdemServico > 0 ? repOrdemServico.BuscarPorCodigo(codigoOrdemServico) : null;
                itemDoc.OrdemCompraMercadoria = codigoOrdemCompraMercadoria > 0 ? repOrdemCompraMercadoria.BuscarPorCodigo(codigoOrdemCompraMercadoria) : null;
                itemDoc.RegraEntradaDocumento = codigoRegraEntradaDocumento > 0 ? repRegraEntradaDocumento.BuscarPorCodigo(codigoRegraEntradaDocumento) : null;
                itemDoc.OutrasDespesas = item.ValorOutrasDespesas != null ? (decimal)item.ValorOutrasDespesas : 0m;
                itemDoc.Produto = repProduto.BuscarPorCodigo((int)item.Produto.Codigo);
                itemDoc.CodigoProdutoFornecedor = (string)item.CodigoProdutoFornecedor;
                itemDoc.DescricaoProdutoFornecedor = (string)item.DescricaoProdutoFornecedor;
                itemDoc.CodigoBarrasEAN = (string)item.CodigoBarrasEAN;
                itemDoc.NCMProdutoFornecedor = (string)item.NCMProdutoFornecedor;
                itemDoc.CESTProdutoFornecedor = (string)item.CESTProdutoFornecedor;
                itemDoc.Quantidade = (decimal)item.Quantidade;
                itemDoc.Sequencial = (int)item.Sequencial;
                itemDoc.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                itemDoc.Observacao = (string)item.ObservacaoItem;
                itemDoc.UnidadeMedida = (UnidadeDeMedida)item.UnidadeMedida;
                itemDoc.PercentualReducaoBaseCalculoCOFINS = item.PercentualReducaoBaseCalculoCOFINS != null ? (decimal)item.PercentualReducaoBaseCalculoCOFINS : 0m;
                itemDoc.PercentualReducaoBaseCalculoIPI = item.PercentualReducaoBaseCalculoIPI != null ? (decimal)item.PercentualReducaoBaseCalculoIPI : 0m;
                itemDoc.PercentualReducaoBaseCalculoPIS = item.PercentualReducaoBaseCalculoPIS != null ? (decimal)item.PercentualReducaoBaseCalculoPIS : 0m;
                itemDoc.ValorCOFINS = item.ValorCOFINS != null ? (decimal)item.ValorCOFINS : 0m;
                itemDoc.ValorFrete = item.ValorFrete != null ? (decimal)item.ValorFrete : 0m;
                itemDoc.ValorICMS = item.ValorICMS != null ? (decimal)item.ValorICMS : 0m;
                itemDoc.ValorICMSST = item.ValorICMSST != null ? (decimal)item.ValorICMSST : 0m;
                itemDoc.BaseSTRetido = item.BaseSTRetido != null ? (decimal)item.BaseSTRetido : 0m;
                itemDoc.ValorSTRetido = item.ValorSTRetido != null ? (decimal)item.ValorSTRetido : 0m;
                itemDoc.ValorIPI = item.ValorIPI != null ? (decimal)item.ValorIPI : 0m;
                itemDoc.ValorPIS = item.ValorPIS != null ? (decimal)item.ValorPIS : 0m;
                itemDoc.ValorCreditoPresumido = item.ValorCreditoPresumido != null ? (decimal)item.ValorCreditoPresumido : 0m;
                itemDoc.ValorDiferencial = item.ValorDiferencial != null ? (decimal)item.ValorDiferencial : 0m;
                itemDoc.ValorTotal = item.ValorTotal != null ? (decimal)item.ValorTotal : 0m;
                itemDoc.ValorUnitario = (decimal)item.ValorUnitario;
                itemDoc.ValorSeguro = item.ValorSeguro != null ? (decimal)item.ValorSeguro : 0m;
                itemDoc.ValorFreteFora = item.ValorFreteFora != null ? (decimal)item.ValorFreteFora : 0m;
                itemDoc.ValorOutrasDespesasFora = item.ValorOutrasDespesasFora != null ? (decimal)item.ValorOutrasDespesasFora : 0m;
                itemDoc.ValorDescontoFora = item.ValorDescontoFora != null ? (decimal)item.ValorDescontoFora : 0m;
                itemDoc.ValorImpostosFora = item.ValorImpostosFora != null ? (decimal)item.ValorImpostosFora : 0m;
                itemDoc.ValorDiferencialFreteFora = item.ValorDiferencialFreteFora != null ? (decimal)item.ValorDiferencialFreteFora : 0m;
                itemDoc.ValorICMSFreteFora = item.ValorICMSFreteFora != null ? (decimal)item.ValorICMSFreteFora : 0m;
                itemDoc.ValorCustoUnitario = item.ValorCustoUnitario != null ? (decimal)item.ValorCustoUnitario : 0m;
                itemDoc.ValorCustoTotal = item.ValorCustoTotal != null ? (decimal)item.ValorCustoTotal : 0m;
                itemDoc.CalculoCustoProduto = item.CalculoCustoProduto != null && !string.IsNullOrWhiteSpace((string)item.CalculoCustoProduto) ? (string)item.CalculoCustoProduto : string.Empty;
                itemDoc.ValorRetencaoCOFINS = item.ValorRetencaoCOFINS != null ? (decimal)item.ValorRetencaoCOFINS : 0m;
                itemDoc.ValorRetencaoCSLL = item.ValorRetencaoCSLL != null ? (decimal)item.ValorRetencaoCSLL : 0m;
                itemDoc.ValorRetencaoIR = item.ValorRetencaoIR != null ? (decimal)item.ValorRetencaoIR : 0m;
                itemDoc.ValorRetencaoISS = item.ValorRetencaoISS != null ? (decimal)item.ValorRetencaoISS : 0m;
                itemDoc.ValorRetencaoINSS = item.ValorRetencaoINSS != null ? (decimal)item.ValorRetencaoINSS : 0m;
                itemDoc.ValorRetencaoIPI = item.ValorRetencaoIPI != null ? (decimal)item.ValorRetencaoIPI : 0m;
                itemDoc.ValorRetencaoOutras = item.ValorRetencaoOutras != null ? (decimal)item.ValorRetencaoOutras : 0m;
                itemDoc.ValorRetencaoPIS = item.ValorRetencaoPIS != null ? (decimal)item.ValorRetencaoPIS : 0m;
                itemDoc.NumeroFogoInicial = item.NumeroFogoInicial != null ? ((string)item.NumeroFogoInicial).ToInt() : 0;
                itemDoc.TipoAquisicao = item.TipoAquisicao != null ? ((string)item.TipoAquisicao).ToNullableEnum<TipoAquisicaoPneu>() : null;
                itemDoc.VidaAtual = item.VidaAtual != null ? ((string)item.VidaAtual).ToNullableEnum<VidaPneu>() : null;
                int codigoAlmoxarifado = item.Almoxarifado != null ? (int)item.Almoxarifado.Codigo : 0;
                itemDoc.Almoxarifado = codigoAlmoxarifado > 0 ? repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado) : null;
                int codigoProdutoVinculado = item.ProdutoVinculado != null ? (int)item.ProdutoVinculado.Codigo : 0;
                itemDoc.ProdutoVinculado = codigoProdutoVinculado > 0 ? repProduto.BuscarPorCodigo(codigoProdutoVinculado) : null;
                itemDoc.QuantidadeProdutoVinculado = item.QuantidadeProdutoVinculado != null ? (decimal)item.QuantidadeProdutoVinculado : 0m;
                int codigoLocalArmazenamento = item.LocalArmazenamento != null ? (int)item.LocalArmazenamento.Codigo : 0;
                itemDoc.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;
                itemDoc.UnidadeMedidaFornecedor = (string)item.UnidadeMedidaFornecedor;
                itemDoc.QuantidadeFornecedor = item.QuantidadeFornecedor != null ? (decimal)item.QuantidadeFornecedor : 0m;
                itemDoc.ValorUnitarioFornecedor = item.ValorUnitarioFornecedor != null ? (decimal)item.ValorUnitarioFornecedor : 0m;
                int codigoCentroResultado = item.CentroResultado != null ? (int)item.CentroResultado.Codigo : 0;
                itemDoc.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                itemDoc.GeraRateioDespesaVeiculo = itemDoc.CFOP?.RealizarRateioDespesaVeiculo ?? false;
                itemDoc.OrigemMercadoria = item.OrigemMercadoria != null ? ((string)item.OrigemMercadoria).ToNullableEnum<OrigemMercadoria>() : null;
                itemDoc.CstIcmsFornecedor = (string)item.CstIcmsFornecedor;
                string cfopFornecedorDescricao = item.CfopFornecedor != null && !string.IsNullOrWhiteSpace(item.CfopFornecedor.ToString()) ? item.CfopFornecedor.ToString().Contains("Descricao") ? item.CfopFornecedor.Descricao.ToString() : string.Empty : string.Empty;
                itemDoc.CfopFornecedor = cfopFornecedorDescricao.Length > 50 ? cfopFornecedorDescricao.Substring(0, 50) : cfopFornecedorDescricao;
                itemDoc.AliquotaICMSFornecedor = item.AliquotaICMSFornecedor != null ? (decimal)item.AliquotaICMSFornecedor : 0m;
                itemDoc.ValorICMSFornecedor = item.ValorICMSFornecedor != null ? (decimal)item.ValorICMSFornecedor : 0m;
                itemDoc.EncerrarOrdemServico = ((string)item.EncerrarOrdemServico).ToBool();
                itemDoc.BaseCalculoICMSFornecedor = item.BaseCalculoICMSFornecedor != null ? (decimal)item.BaseCalculoICMSFornecedor : 0m;


                string formulaCusto = itemDoc.Produto?.CalculoCustoProduto ?? Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho);
                if ((itemDoc.ValorCustoUnitario <= 0 || itemDoc.ValorCustoTotal <= 0) && !string.IsNullOrWhiteSpace(formulaCusto))
                {
                    string[] campos = formulaCusto.Split('#');
                    for (int i = 0; i < campos.Length; i++)
                    {
                        if (campos[i].Trim() == "")
                        {
                            campos.ToList().Remove(campos[i]);
                        }
                        else
                        {
                            campos[i] = campos[i].Trim();
                        }

                    }

                    decimal quantidade = itemDoc.Quantidade;
                    decimal valorUnitario = itemDoc.ValorUnitario;
                    decimal valorTotal = itemDoc.ValorTotal;

                    if ((valorTotal) > 0)
                    {
                        decimal valorICMS = itemDoc.ValorICMS;
                        decimal valorCreditoPresumido = itemDoc.ValorCreditoPresumido;
                        decimal valorDiferencial = itemDoc.ValorDiferencial;
                        decimal valorICMSST = itemDoc.ValorICMSST;
                        decimal valorIPI = itemDoc.ValorIPI;
                        decimal valorFrete = itemDoc.ValorFrete;
                        decimal valorOutras = itemDoc.ValorOutrasDespesasFora;
                        decimal valorSeguro = itemDoc.ValorSeguro;
                        decimal valorDesconto = itemDoc.Desconto;
                        decimal valorDescontoFora = itemDoc.ValorDescontoFora;
                        decimal valorImpostoFora = itemDoc.ValorImpostosFora;
                        decimal valorOutrasFora = itemDoc.ValorOutrasDespesasFora;
                        decimal valorFreteFora = itemDoc.ValorFreteFora;
                        decimal valorICMSFreteFora = itemDoc.ValorICMSFreteFora;
                        decimal valorDiferencialFreteFora = itemDoc.ValorDiferencialFreteFora;
                        decimal valorPIS = itemDoc.ValorPIS;
                        decimal valorCOFINS = itemDoc.ValorCOFINS;
                        decimal custoUnitario = 0m;
                        decimal custoTotal = 0m;

                        custoUnitario = (valorTotal);

                        if (valorDesconto > 0)
                        {
                            if (campos.Contains("ValorDesconto") && campos[campos.ToList().IndexOf("ValorDesconto") - 1] == "+")
                                custoUnitario = custoUnitario + valorDesconto;
                            else if (campos.Contains("ValorDesconto") && campos[campos.ToList().IndexOf("ValorDesconto") - 1] == "-")
                                custoUnitario = custoUnitario - valorDesconto;
                        }

                        if (valorOutras > 0)
                        {
                            if (campos.Contains("ValorOutras") && campos[campos.ToList().IndexOf("ValorOutras") - 1] == "+")
                                custoUnitario = custoUnitario + valorOutras;
                            else if (campos.Contains("ValorOutras") && campos[campos.ToList().IndexOf("ValorOutras") - 1] == "-")
                                custoUnitario = custoUnitario - valorOutras;
                        }

                        if (valorFrete > 0)
                        {
                            if (campos.Contains("ValorFrete") && campos[campos.ToList().IndexOf("ValorFrete") - 1] == "+")
                                custoUnitario = custoUnitario + valorFrete;
                            else if (campos.Contains("ValorFrete") && campos[campos.ToList().IndexOf("ValorFrete") - 1] == "-")
                                custoUnitario = custoUnitario - valorFrete;
                        }

                        if (valorSeguro > 0)
                        {
                            if (campos.Contains("ValorSeguro") && campos[campos.ToList().IndexOf("ValorSeguro") - 1] == "+")
                                custoUnitario = custoUnitario + valorSeguro;
                            else if (campos.Contains("ValorSeguro") && campos[campos.ToList().IndexOf("ValorSeguro") - 1] == "-")
                                custoUnitario = custoUnitario - valorSeguro;
                        }

                        if (valorICMS > 0)
                        {
                            if (campos.Contains("ValorICMS") && campos[campos.ToList().IndexOf("ValorICMS") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMS;
                            else if (campos.Contains("ValorICMS") && campos[campos.ToList().IndexOf("ValorICMS") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMS;
                        }

                        if (valorIPI > 0)
                        {
                            if (campos.Contains("ValorIPI") && campos[campos.ToList().IndexOf("ValorIPI") - 1] == "+")
                                custoUnitario = custoUnitario + valorIPI;
                            else if (campos.Contains("ValorIPI") && campos[campos.ToList().IndexOf("ValorIPI") - 1] == "-")
                                custoUnitario = custoUnitario - valorIPI;
                        }

                        if (valorICMSST > 0)
                        {
                            if (campos.Contains("ValorICMSST") && campos[campos.ToList().IndexOf("ValorICMSST") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMSST;
                            else if (campos.Contains("ValorICMSST") && campos[campos.ToList().IndexOf("ValorICMSST") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMSST;
                        }

                        if (valorCreditoPresumido > 0)
                        {
                            if (campos.Contains("ValorCreditoPresumido") && campos[campos.ToList().IndexOf("ValorCreditoPresumido") - 1] == "+")
                                custoUnitario = custoUnitario + valorCreditoPresumido;
                            else if (campos.Contains("ValorCreditoPresumido") && campos[campos.ToList().IndexOf("ValorCreditoPresumido") - 1] == "-")
                                custoUnitario = custoUnitario - valorCreditoPresumido;
                        }

                        if (valorDiferencial > 0)
                        {
                            if (campos.Contains("ValorDiferencial") && campos[campos.ToList().IndexOf("ValorDiferencial") - 1] == "+")
                                custoUnitario = custoUnitario + valorDiferencial;
                            else if (campos.Contains("ValorDiferencial") && campos[campos.ToList().IndexOf("ValorDiferencial") - 1] == "-")
                                custoUnitario = custoUnitario - valorDiferencial;
                        }

                        if (valorFreteFora > 0)
                        {
                            if (campos.Contains("ValorFreteFora") && campos[campos.ToList().IndexOf("ValorFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorFreteFora;
                            else if (campos.Contains("ValorFreteFora") && campos[campos.ToList().IndexOf("ValorFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorFreteFora;
                        }

                        if (valorOutrasFora > 0)
                        {
                            if (campos.Contains("ValorOutrasFora") && campos[campos.ToList().IndexOf("ValorOutrasFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorOutrasFora;
                            else if (campos.Contains("ValorOutrasFora") && campos[campos.ToList().IndexOf("ValorOutrasFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorOutrasFora;
                        }

                        if (valorImpostoFora > 0)
                        {
                            if (campos.Contains("ValorImpostoFora") && campos[campos.ToList().IndexOf("ValorImpostoFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorImpostoFora;
                            else if (campos.Contains("ValorImpostoFora") && campos[campos.ToList().IndexOf("ValorImpostoFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorImpostoFora;
                        }

                        if (valorDiferencialFreteFora > 0)
                        {
                            if (campos.Contains("ValorDiferencialFreteFora") && campos[campos.ToList().IndexOf("ValorDiferencialFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorDiferencialFreteFora;
                            else if (campos.Contains("ValorDiferencialFreteFora") && campos[campos.ToList().IndexOf("ValorDiferencialFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorDiferencialFreteFora;
                        }

                        if (valorPIS > 0)
                        {
                            if (campos.Contains("ValorPIS") && campos[campos.ToList().IndexOf("ValorPIS") - 1] == "+")
                                custoUnitario = custoUnitario + valorPIS;
                            else if (campos.Contains("ValorPIS") && campos[campos.ToList().IndexOf("ValorPIS") - 1] == "-")
                                custoUnitario = custoUnitario - valorPIS;
                        }

                        if (valorCOFINS > 0)
                        {
                            if (campos.Contains("ValorCOFINS") && campos[campos.ToList().IndexOf("ValorCOFINS") - 1] == "+")
                                custoUnitario = custoUnitario + valorCOFINS;
                            else if (campos.Contains("ValorCOFINS") && campos[campos.ToList().IndexOf("ValorCOFINS") - 1] == "-")
                                custoUnitario = custoUnitario - valorCOFINS;
                        }

                        if (valorICMSFreteFora > 0)
                        {
                            if (campos.Contains("ValorICMSFreteFora") && campos[campos.ToList().IndexOf("ValorICMSFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMSFreteFora;
                            else if (campos.Contains("ValorICMSFreteFora") && campos[campos.ToList().IndexOf("ValorICMSFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMSFreteFora;
                        }

                        if (valorDescontoFora > 0)
                        {
                            if (campos.Contains("ValorDescontoFora") && campos[campos.ToList().IndexOf("ValorDescontoFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorDescontoFora;
                            else if (campos.Contains("ValorDescontoFora") && campos[campos.ToList().IndexOf("ValorDescontoFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorDescontoFora;
                        }

                        custoTotal = custoUnitario;
                        custoUnitario = custoUnitario / quantidade;

                        if (custoUnitario > 0 && custoTotal > 0)
                        {
                            itemDoc.ValorCustoUnitario = custoUnitario;
                            itemDoc.ValorCustoTotal = custoTotal;
                        }
                    }
                }

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                    {
                        if (itemDoc.NaturezaOperacao == null)
                        {
                            erro = "Deve-se informar a natureza de operação do item " + itemDoc.Sequencial + ".";
                            return false;
                        }

                        if (itemDoc.TipoMovimento == null)
                        {
                            erro = "Deve-se informar o tipo de movimento do item " + itemDoc.Sequencial + ".";
                            return false;
                        }

                        if (itemDoc.NaturezaOperacao.ControlaEstoque && itemDoc.Produto == null)
                        {
                            erro = "É necessário associar um produto ao item " + itemDoc.Sequencial + ", pois a natureza da operação controla estoque.";
                            return false;
                        }
                    }
                }

                if ((itemDoc.CFOP?.ObrigarInformarLocalArmazenamento ?? false) && itemDoc.LocalArmazenamento == null)
                {
                    erro = "É necessário informar local de armazenamento do item " + itemDoc.Sequencial + ", pois existe configuração de CFOP habilitada com obrigatoriedade (Aba outros do item).";
                    return false;
                }

                if (itemDoc.Abastecimentos != null && itemDoc.Abastecimentos.Count > 0 && itemDoc.Veiculo != null && itemDoc.Quantidade > itemDoc.Veiculo.CapacidadeTanque && itemDoc.Veiculo.CapacidadeTanque > 0)
                {
                    erro = "Litros abastecidos no veículo do item " + itemDoc.Sequencial + " é maior que sua Capacidade de Tanque (" + itemDoc.Veiculo.CapacidadeTanque.ToString() + ").";
                    return false;
                }

                if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if ((itemDoc.DocumentoEntrada?.Destinatario?.CadastrarProdutoAutomaticamenteDocumentoEntrada ?? false) && itemDoc.Produto == null)
                        {
                            if (!Servicos.Embarcador.Financeiro.DocumentoEntrada.RegistrarProduto(out erro, ref itemDoc, documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, this.Usuario.Empresa, ncmsAbastecimento, ConfiguracaoEmbarcador, configuracaoDocumentoEntrada, Auditado))
                                return false;
                        }
                        else if (itemDoc.Produto == null)
                        {
                            erro = "É necessário associar um produto ao item " + itemDoc.Sequencial + ".";
                            return false;
                        }
                    }
                    else
                    {
                        if (itemDoc.Produto == null && ConfiguracaoEmbarcador.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada)
                        {
                            erro = "É necessário associar um produto ao item " + itemDoc.Sequencial + ".";
                            return false;
                        }
                        else if (itemDoc.Produto == null)
                        {
                            if (!Servicos.Embarcador.Financeiro.DocumentoEntrada.RegistrarProduto(out erro, ref itemDoc, documentoEntrada, unidadeDeTrabalho, TipoServicoMultisoftware, this.Usuario.Empresa, ncmsAbastecimento, ConfiguracaoEmbarcador, configuracaoDocumentoEntrada, Auditado))
                                return false;
                        }
                    }
                }

                if (itemDoc.Codigo > 0)
                    repItem.Atualizar(itemDoc, Auditado);
                else
                    repItem.Inserir(itemDoc, Auditado);

                if (itemDoc.Produto != null && !string.IsNullOrWhiteSpace(itemDoc.CodigoProdutoFornecedor))
                {
                    Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);

                    Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(itemDoc.CodigoProdutoFornecedor, documentoEntrada.Fornecedor.CPF_CNPJ,
                        TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? documentoEntrada.Destinatario != null ? documentoEntrada.Destinatario.Codigo : 0 : 0);

                    if (produtoFornecedor == null)
                        produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();

                    produtoFornecedor.CodigoProduto = itemDoc.CodigoProdutoFornecedor;
                    produtoFornecedor.Fornecedor = documentoEntrada.Fornecedor;
                    produtoFornecedor.Produto = itemDoc.Produto;

                    if (produtoFornecedor.Codigo > 0)
                        repProdutoFornecedor.Atualizar(produtoFornecedor);
                    else
                    {
                        produtoFornecedor.FatorConversao = 0;
                        repProdutoFornecedor.Inserir(produtoFornecedor);
                    }
                }

                //Aba Abastecimentos
                if (itemDoc.Abastecimentos != null && itemDoc.Abastecimentos.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (var abastecimento in itensAbastecimentos)
                        if ((string)abastecimento.CodigoItem == (string)item.Codigo)
                            if (abastecimento.CodigoInterno != null)
                                codigos.Add((int)abastecimento.CodigoInterno);

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> abastecimentosDeletar = (from obj in itemDoc.Abastecimentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (var i = 0; i < abastecimentosDeletar.Count; i++)
                        repDocumentoEntradaItemAbastecimento.Deletar(abastecimentosDeletar[i]);
                }
                else
                    itemDoc.Abastecimentos = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento>();

                if (itemDoc.CFOP != null)
                {
                    if (itemDoc.CFOP.ObrigarVincularAbastecimentoAoItemDocumentoEntrada && (itensAbastecimentos == null || itensAbastecimentos.Count == 0))
                    {
                        erro = "CFOP do item exige informar um abastecimento para o item: " + itemDoc.Descricao + ".";
                        return false;
                    }
                }

                foreach (var abastecimento in itensAbastecimentos)
                {
                    if ((string)abastecimento.CodigoItem == (string)item.Codigo)
                    {
                        int.TryParse((string)abastecimento.CodigoInterno, out int codigoAbastecimento);

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento itemAbas = codigoAbastecimento > 0 ? repDocumentoEntradaItemAbastecimento.BuscarPorCodigo(codigoAbastecimento) : null;

                        if (itemAbas == null)
                            itemAbas = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento();

                        itemAbas.Abastecimento = repAbastecimento.BuscarPorCodigo((int)abastecimento.Codigo);
                        itemAbas.DocumentoEntradaItem = itemDoc;

                        if (itemAbas.Codigo > 0)
                            repDocumentoEntradaItemAbastecimento.Atualizar(itemAbas);
                        else
                            repDocumentoEntradaItemAbastecimento.Inserir(itemAbas);
                    }
                }

                //Aba Ordens de Serviço
                if (itemDoc.OrdensServico != null && itemDoc.OrdensServico.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (var ordemServico in itensOrdensServico)
                        if ((string)ordemServico.CodigoItem == (string)item.Codigo)
                            if (ordemServico.CodigoInterno != null)
                                codigos.Add((int)ordemServico.CodigoInterno);

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico> ordensDeletar = (from obj in itemDoc.OrdensServico where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (var i = 0; i < ordensDeletar.Count; i++)
                        repDocumentoEntradaItemOrdemServico.Deletar(ordensDeletar[i]);
                }
                else
                    itemDoc.OrdensServico = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico>();

                foreach (var ordemServico in itensOrdensServico)
                {
                    if ((string)ordemServico.CodigoItem == (string)item.Codigo)
                    {
                        int.TryParse((string)ordemServico.CodigoInterno, out int codigoOrdem);

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico itemAbas = codigoOrdem > 0 ? repDocumentoEntradaItemOrdemServico.BuscarPorCodigo(codigoOrdem) : null;
                        if (itemAbas == null)
                            itemAbas = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico();

                        itemAbas.OrdemServico = repOrdemServico.BuscarPorCodigo((int)ordemServico.Codigo);
                        itemAbas.DocumentoEntradaItem = itemDoc;

                        if (itemAbas.Codigo > 0)
                            repDocumentoEntradaItemOrdemServico.Atualizar(itemAbas);
                        else
                            repDocumentoEntradaItemOrdemServico.Inserir(itemAbas);
                    }
                }

                if (ordemCompra != null && configuracaoFinanceiro.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra)
                {
                    bool produtoExisteNaOrdemDeCompra = ordemCompraMercadoria.Any(mercadoria => itemDoc.Produto.CodigoNCM == mercadoria.Produto.CodigoNCM);
                    bool fornecedorExisteNaOrdemDeCompra = documentoEntrada.Fornecedor.CPF_CNPJ != ordemCompra.Fornecedor.CPF_CNPJ ? false : true;
                    if (!produtoExisteNaOrdemDeCompra || !fornecedorExisteNaOrdemDeCompra)
                        throw new ControllerException("Ao adicionar uma ordem de compra, o fornecedor e produto adicionados no documento de entrada devem ser os mesmos da ordem de compra!");
                }
                if (itemDoc.OrdemCompraMercadoria != null)
                {
                    Servicos.Embarcador.Compras.OrdemCompra.FinalizarOrdemCompraPorItem(documentoEntrada.Codigo, itemDoc.OrdemCompraMercadoria.OrdemCompra.Codigo, out erro, out bool contemQuantidadePendente, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador);
                }
            }

            erro = string.Empty;
            return true;
        }

        private int SalvarDuplicatas(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            dynamic duplicatas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Duplicatas"));
            int countDuplicatas = 0;

            if (documentoEntrada.Duplicatas != null && documentoEntrada.Duplicatas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var duplicata in duplicatas)
                    if (duplicata.Codigo != null)
                        codigos.Add((int)duplicata.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata> duplicatasDeletar = (from obj in documentoEntrada.Duplicatas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < duplicatasDeletar.Count; i++)
                    repDuplicata.Deletar(duplicatasDeletar[i], Auditado);
            }
            else
                documentoEntrada.Duplicatas = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata>();

            foreach (var duplicata in duplicatas)
            {
                int codigoDuplicata = 0;
                int.TryParse((string)duplicata.Codigo, out codigoDuplicata);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicataDoc = codigoDuplicata > 0 ? repDuplicata.BuscarPorCodigo(codigoDuplicata, true) : null;

                if (duplicataDoc == null)
                    duplicataDoc = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();

                duplicataDoc.DocumentoEntrada = documentoEntrada;
                duplicataDoc.DataVencimento = DateTime.ParseExact((string)duplicata.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);
                duplicataDoc.Numero = (string)duplicata.Numero;
                duplicataDoc.Sequencia = !string.IsNullOrWhiteSpace((string)duplicata.Sequencia) ? int.Parse(Utilidades.String.OnlyNumbers((string)duplicata.Sequencia)) : 0;
                duplicataDoc.NumeroBoleto = (string)duplicata.NumeroBoleto;
                duplicataDoc.Valor = (decimal)duplicata.Valor;
                Enum.TryParse((string)duplicata.FormaTitulo, out FormaTitulo formaTitulo);
                duplicataDoc.Forma = formaTitulo;
                duplicataDoc.Observacao = (string)duplicata.Observacao;

                double portador = ((string)duplicata.Portador.Codigo).ToDouble();
                duplicataDoc.Portador = portador > 0 ? repCliente.BuscarPorCPFCNPJ(portador) : null;

                if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(duplicataDoc.DataVencimento, dataLimiteVencimento);

                    if (result > 0)
                        throw new ControllerException($"A data da duplicata {duplicataDoc.Numero} é maior que a data limite estipulada nas configurações.");
                }

                if (duplicataDoc.Codigo > 0)
                    repDuplicata.Atualizar(duplicataDoc, Auditado);
                else
                    repDuplicata.Inserir(duplicataDoc, Auditado);
                countDuplicatas = countDuplicatas + 1;
            }

            return countDuplicatas;
        }

        private void SalvarCentrosResultado(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic centrosResultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentroCustos"));

            if (documentoEntrada.LancamentosCentroResultado != null && documentoEntrada.LancamentosCentroResultado.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroResultado in centrosResultado)
                {
                    int codigo = 0;
                    if (centroResultado.Codigo != null && int.TryParse((string)centroResultado.Codigo, out codigo) && codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> centrosResultadoDeletar = (from obj in documentoEntrada.LancamentosCentroResultado where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < centrosResultadoDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado centroResultado = centrosResultadoDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, "Removeu o centro de resultados " + centroResultado.Descricao, unitOfWork);

                    repLancamentoCentroResultado.Deletar(centroResultado, Auditado);
                }
            }

            decimal valorTotalRateado = 0m, percentual = 0m;
            int countCentroResultado = centrosResultado.Count;

            for (int i = 0; i < countCentroResultado; i++)
            {
                dynamic centroResultado = centrosResultado[i];

                int codigoCentro = 0, codigoCentroResultado = 0;
                int.TryParse((string)centroResultado.Codigo, out codigoCentro);
                int.TryParse((string)centroResultado.CodigoCentroResultado, out codigoCentroResultado);

                Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamento = codigoCentro > 0 ? repLancamentoCentroResultado.BuscarPorCodigo(codigoCentro, true) : null;

                if (lancamento == null)
                    lancamento = new Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado();

                lancamento.TipoDocumento = TipoDocumentoLancamentoCentroResultado.Titulo;
                lancamento.Ativo = true;
                lancamento.DocumentoEntrada = documentoEntrada;
                lancamento.Data = DateTime.Now;
                lancamento.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                lancamento.Percentual = Utilidades.Decimal.Converter((string)centroResultado.Percentual);

                if ((i + 1) == countCentroResultado)
                    lancamento.Valor = documentoEntrada.ValorTotal - valorTotalRateado;
                else
                    lancamento.Valor = Math.Round(Math.Floor(documentoEntrada.ValorTotal * lancamento.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                valorTotalRateado += lancamento.Valor;
                percentual += lancamento.Percentual;

                if (lancamento.Codigo > 0)
                    repLancamentoCentroResultado.Atualizar(lancamento, Auditado);
                else
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, "Adicionou o centro de resultados " + lancamento.Descricao, unitOfWork);
                    repLancamentoCentroResultado.Inserir(lancamento, Auditado);
                }

                if (lancamento.CentroResultado != null && lancamento.CentroResultado.Veiculos != null && lancamento.CentroResultado.Veiculos.Count > 0)
                {
                    if (documentoEntrada.Veiculos == null)
                        documentoEntrada.Veiculos = new List<Dominio.Entidades.Veiculo>();
                    foreach (var veiculo in lancamento.CentroResultado.Veiculos)
                    {
                        if (!documentoEntrada.Veiculos.Any(v => v.Codigo == veiculo.Codigo))
                        {
                            Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo(veiculo.Codigo);

                            documentoEntrada.Veiculos.Add(veic);
                        }
                    }
                }
            }

            if (countCentroResultado > 0)
            {
                if (percentual != 100m)
                    throw new ControllerException("O percentual rateado entre os centros de resultado difere de 100%.");

                if (valorTotalRateado != documentoEntrada.ValorTotal)
                    throw new ControllerException("Ocorreram problemas ao realizar o rateio dos valores entre os centros de resultado.");
            }
        }

        private void SalvarCentrosResultadoTiposDespesa(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa repDocumentoEntradaCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            dynamic dynCentrosResultadoTiposDespesa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultadoTiposDespesa"));

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repDocumentoEntradaCentroResultadoTipoDespesa.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            if (centrosResultadoTiposDespesa.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroResultadoTipoDespesa in dynCentrosResultadoTiposDespesa)
                {
                    int codigo = ((string)centroResultadoTipoDespesa.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> deletar = (from obj in centrosResultadoTiposDespesa where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repDocumentoEntradaCentroResultadoTipoDespesa.Deletar(deletar[i]);
            }

            decimal percentual = 0m;
            foreach (dynamic centroResultadoTipoDespesa in dynCentrosResultadoTiposDespesa)
            {
                int codigo = ((string)centroResultadoTipoDespesa.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa documentoEntradaCentroResultadoTipoDespesa = codigo > 0 ? repDocumentoEntradaCentroResultadoTipoDespesa.BuscarPorCodigo(codigo, false) : null;

                if (documentoEntradaCentroResultadoTipoDespesa == null)
                {
                    documentoEntradaCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa();

                    int codigoTipoDespesa = ((string)centroResultadoTipoDespesa.TipoDespesa.Codigo).ToInt();
                    int codigoCentroResultado = ((string)centroResultadoTipoDespesa.CentroResultado.Codigo).ToInt();

                    documentoEntradaCentroResultadoTipoDespesa.DocumentoEntrada = documentoEntrada;
                    documentoEntradaCentroResultadoTipoDespesa.TipoDespesaFinanceira = repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesa);
                    documentoEntradaCentroResultadoTipoDespesa.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
                    documentoEntradaCentroResultadoTipoDespesa.Percentual = ((string)centroResultadoTipoDespesa.Percentual).ToDecimal();

                    percentual += documentoEntradaCentroResultadoTipoDespesa.Percentual;

                    repDocumentoEntradaCentroResultadoTipoDespesa.Inserir(documentoEntradaCentroResultadoTipoDespesa);
                }
                else
                    percentual += documentoEntradaCentroResultadoTipoDespesa.Percentual;
            }

            if (dynCentrosResultadoTiposDespesa.Count > 0 && percentual != 100m)
                throw new ControllerException("O percentual rateado entre os Centros de Resultado/Tipos de Despesa difere de 100%.");
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            //if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Aberto)
            //    return;

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            if (documentoEntrada.Veiculos == null)
                documentoEntrada.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic veiculo in veiculos)
                    codigos.Add((int)veiculo);

                List<Dominio.Entidades.Veiculo> veiculosDeletar = documentoEntrada.Veiculos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Veiculo veiculoDeletar in veiculosDeletar)
                    documentoEntrada.Veiculos.Remove(veiculoDeletar);
            }
            if (veiculos != null)
            {
                foreach (var veiculo in veiculos)
                {
                    if (!documentoEntrada.Veiculos.Any(o => o.Codigo == (int)veiculo))
                    {
                        Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo((int)veiculo);

                        documentoEntrada.Veiculos.Add(veic);
                    }
                }
            }
        }

        private void SalvarLog(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMSLog repLog = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMSLog(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMSLog log = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMSLog();

            log.Data = DateTime.Now;
            log.DocumentoEntrada = documentoEntrada;

            if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                log.Tipo = TipoLogDocumentoEntrada.Finalizacao;
            else if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Cancelado)
                log.Tipo = TipoLogDocumentoEntrada.Cancelamento;
            else if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Anulado)
                log.Tipo = TipoLogDocumentoEntrada.Anulacao;
            else
                log.Tipo = TipoLogDocumentoEntrada.Abertura;

            log.Usuario = this.Usuario;

            repLog.Inserir(log);
        }

        private void ValidarRegrasDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.CFOP cfop = documentoEntrada.CFOP;
            if (cfop != null && cfop.BloqueioDocumentoEntrada != BloqueioDocumentoEntrada.SemBloqueio && documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
            {
                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemCompra && documentoEntrada.OrdemCompra == null)
                    throw new ControllerException("É necessário selecionar uma ordem de compra.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServico && documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0))
                    throw new ControllerException("É necessário selecionar uma ordem de serviço.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServicoESemOrdemCompra && ((documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0)) || documentoEntrada.OrdemCompra == null))
                    throw new ControllerException("É necessário selecionar uma ordem de serviço e uma ordem de compra.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServicoOuSemOrdemCompra && documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0) && documentoEntrada.OrdemCompra == null)
                    throw new ControllerException("É necessário selecionar uma ordem de serviço e uma ordem de compra.");
            }

            if (documentoEntrada.Modelo.Numero.Equals("55") || documentoEntrada.Modelo.Numero.Equals("57"))
            {
                if (string.IsNullOrWhiteSpace(documentoEntrada.Chave))
                    throw new ControllerException("É necessário informar a chave do documento.");
                else if (!Utilidades.Validate.ValidarChave(documentoEntrada.Chave))
                    throw new ControllerException("A chave do documento informada é inválida.");
            }
        }

        private string ObterConteudoXML(Servicos.DTO.CustomFile file)
        {
            using(StreamReader reader = new StreamReader(file.InputStream))
            {
                return reader.ReadToEnd();
            };
        }
        #endregion
    }
}

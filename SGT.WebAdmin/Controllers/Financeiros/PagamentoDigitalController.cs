using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "DownloadRelatorioRemessa", "DownloadArquivoRemessa" }, "Financeiros/PagamentoDigital", "Financeiros/RemessaPagamento")]
    public class PagamentoDigitalController : BaseController
    {
		#region Construtores

		public PagamentoDigitalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                int.TryParse(Request.Params("Descricao"), out int numero);

                int codigoEmpresa = 0;
                int.TryParse(Request.Params("BoletoConfiguracao"), out int codigoBoletoConfiguracao);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("PagamentoEletronico"), out int codigoPagamentoEletronico);
                int.TryParse(Request.Params("Titulo"), out int codigoTitulo);

                double.TryParse(Request.Params("Pessoa"), out double cnpjFornecedor);

                DateTime.TryParse(Request.Params("DataPagamentoInicial"), out DateTime dataPagamentoInicial);
                DateTime.TryParse(Request.Params("DataPagamentoFinal"), out DateTime dataPagamentoFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico situacaoAutorizacaoPagamentoEletronico = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico>("SituacaoAutorizacaoPagamentoEletronico");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Banco", "BoletoConfiguracao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modalidade", "Modalidade", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor total", "ValorTotal", 10, Models.Grid.Align.right, true);

                string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (ordenacao == "Descricao")
                    ordenacao = "Numero";

                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> listaPagamentoEletronico = repPagamentoEletronico.Consultar(situacaoAutorizacaoPagamentoEletronico, numero, codigoBoletoConfiguracao, codigoEmpresa, codigoPagamentoEletronico, codigoTitulo, cnpjFornecedor, dataPagamentoInicial, dataPagamentoFinal, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPagamentoEletronico.ContarConsulta(situacaoAutorizacaoPagamentoEletronico, numero, codigoBoletoConfiguracao, codigoEmpresa, codigoPagamentoEletronico, codigoTitulo, cnpjFornecedor, dataPagamentoInicial, dataPagamentoFinal));

                var lista = (from p in listaPagamentoEletronico
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.Numero.ToString("n0"),
                                 p.Numero,
                                 BoletoConfiguracao = p.BoletoConfiguracao.Descricao,
                                 ValorTotal = p.ValorTotal.ToString("n2"),
                                 Modalidade = p.ModalidadePagamentoEletronico.ObterDescricao()
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumentoEntrada", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Parcela", "Sequencia", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fornecedor", "Pessoa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Programação Pgto", "DataProgramacaoPagamento", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome do Banco", "BancoNome", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Banco", "Banco", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Emissão", "DataEmissao", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Vencimento", "DataVencimento", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Boleto", "NossoNumero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Pag. Digital", "NumeroPagamentoDigital", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Chave", "TipoChavePixPessoa", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave PIX", "ChavePixPessoa", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Numero")
                    propOrdenar = "Codigo";
                else if (propOrdenar == "NumeroDocumentoEntrada")
                    propOrdenar = "NumeroDocumentoTituloOriginal, DuplicataDocumentoEntrada.DocumentoEntrada.Numero";
                else if (propOrdenar == "Banco")
                    propOrdenar = "Pessoa.Banco.Numero";

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.ConsultarTitulosPendentes(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarTitulosPendentes(filtrosPesquisa));

                var lista = (from p in listaTitulo
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Codigo.ToString("n0"),
                                 NumeroDocumentoEntrada = !string.IsNullOrWhiteSpace(p.NumeroDocumentoTituloOriginal) ? p.NumeroDocumentoTituloOriginal : p.DuplicataDocumentoEntrada != null && p.DuplicataDocumentoEntrada.DocumentoEntrada != null ? p.DuplicataDocumentoEntrada.DocumentoEntrada.Numero.ToString("n0") : string.Empty,
                                 Sequencia = p.Sequencia.ToString("n0"),
                                 Pessoa = p.Pessoa != null ? p.Pessoa.Nome : string.Empty,
                                 BancoNome = p.Pessoa != null && p.Pessoa.Banco != null ? p.Pessoa.Banco.Descricao : string.Empty,
                                 Banco = p.Pessoa != null && p.Pessoa.Banco != null ? p.Pessoa.Banco.Numero.ToString() : string.Empty,
                                 DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 ValorOriginal = p.Saldo.ToString("n2"),
                                 p.NossoNumero,
                                 DataProgramacaoPagamento = p.DataProgramacaoPagamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 p.NumeroPagamentoDigital,
                                 TipoChavePixPessoa = p.Pessoa != null && p.Pessoa.TipoChavePix != null ? p.Pessoa.TipoChavePix.Value.ObterDescricao() : string.Empty,
                                 ChavePixPessoa = p.Pessoa != null && p.Pessoa.ChavePix != null ? p.Pessoa.ChavePix : string.Empty
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValoresTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente();

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = null;
                listaTitulo = repTitulo.ConsultarTitulosPendentes(filtrosPesquisa, "", "", 0, 0);

                if (selecionarTodos)
                    listaTitulo = listaTitulo.Where(o => !codigosTitulos.Contains(o.Codigo)).ToList();
                else
                    listaTitulo = listaTitulo.Where(o => codigosTitulos.Contains(o.Codigo)).ToList();

                if (listaTitulo == null || listaTitulo.Count == 0)
                    return new JsonpResult(false, "Nenhum título selecionado.");

                codigosTitulos = listaTitulo.Select(o => o.Codigo).ToList();

                dynamic dynRetorno;
                dynRetorno = new
                {
                    QuantidadeTitulos = listaTitulo.Count().ToString("n0"),
                    ValorTotal = listaTitulo.Sum(o => o.Saldo).ToString("n2"),
                    CodigosTitulos = codigosTitulos
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados dos títulos selecionados");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Servicos.Embarcador.Financeiro.PagamentoEletronico servicoPagamentoEletronico = new Servicos.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);

                decimal.TryParse(Request.Params("ValorTotal"), out decimal valorTotal);

                int codigoEmpresa;
                int.TryParse(Request.Params("BoletoConfiguracao"), out int codigoBoletoConfiguracao);
                int.TryParse(Request.Params("LayoutEDI"), out int codigoLayoutEDI);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("QuantidadeTitulos"), out int quantidadeTitulos);

                DateTime.TryParse(Request.Params("DataPagamento"), out DateTime dataPagamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico modalidadePagamentoEletronico;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaPagamentoEletronico tipoContaPagamentoEletronico;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadePagamentoEletronico finalidadePagamentoEletronico;
                Enum.TryParse(Request.Params("ModalidadePagamentoEletronico"), out modalidadePagamentoEletronico);
                Enum.TryParse(Request.Params("TipoContaPagamentoEletronico"), out tipoContaPagamentoEletronico);
                Enum.TryParse(Request.Params("FinalidadePagamentoEletronico"), out finalidadePagamentoEletronico);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));

                if (dataPagamento == null || dataPagamento == DateTime.MinValue)
                    return new JsonpResult(false, "Favor informe a data do pagamento.");

                if (codigosTitulos == null || codigosTitulos.Count == 0)
                    return new JsonpResult(false, "Nenhum título selecionado.");

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorCodigosComFetch(codigosTitulos);
                if (listaTitulos == null || listaTitulos.Count == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = Empresa.Codigo;

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = new Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico();
                pagamentoEletronico.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                pagamentoEletronico.DataGeracao = DateTime.Now;
                pagamentoEletronico.DataPagamento = dataPagamento;
                pagamentoEletronico.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                pagamentoEletronico.FinalidadePagamentoEletronico = finalidadePagamentoEletronico;
                pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DescricaoUsoEmpresaPagamentoEletronico>("DescricaoUsoEmpresaPagamentoEletronico");
                pagamentoEletronico.LayoutEDI = repLayoutEDI.BuscarPorCodigo(codigoLayoutEDI);
                pagamentoEletronico.ModalidadePagamentoEletronico = modalidadePagamentoEletronico;
                pagamentoEletronico.Numero = repPagamentoEletronico.BuscarProximoNumero(codigoBoletoConfiguracao, codigoEmpresa);
                pagamentoEletronico.QuantidadeTitulos = quantidadeTitulos;
                pagamentoEletronico.TipoContaPagamentoEletronico = tipoContaPagamentoEletronico;
                pagamentoEletronico.Usuario = this.Usuario;
                pagamentoEletronico.ValorTotal = valorTotal;

                pagamentoEletronico.FormaLancamentoPagamentoEletronico = Request.GetNullableEnumParam<FormaLancamentoPagamentoEletronico>("FormaLancamentoPagamentoEletronico");
                pagamentoEletronico.TipoServicoPagamentoEletronico = Request.GetNullableEnumParam<TipoServicoPagamentoEletronico>("TipoServicoPagamentoEletronico");

                repPagamentoEletronico.Inserir(pagamentoEletronico, Auditado);

                foreach (var titulo in listaTitulos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo pagamentoEletronicoTitulo = new Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo();
                    pagamentoEletronicoTitulo.PagamentoEletronico = pagamentoEletronico;
                    pagamentoEletronicoTitulo.Titulo = titulo;

                    if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CC_CreditoContaCorrente ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.OP_ChequeOP ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.DOC_DOCCompre ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCR_CreditoConta ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDS_TEDSTR)
                    {
                        if (titulo.Fornecedor != null)
                        {
                            if (titulo.Fornecedor.Banco == null || string.IsNullOrWhiteSpace(titulo.Fornecedor.NumeroConta) || string.IsNullOrWhiteSpace(titulo.Fornecedor.Agencia))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, "O fornecedor " + titulo.Pessoa.Nome + " e seu portador da conta " + titulo.Fornecedor.Nome + " não possui configuração de banco para gerar esta remessa.");
                            }
                        }
                    }

                    repPagamentoEletronicoTitulo.Inserir(pagamentoEletronicoTitulo, Auditado);
                }
                servicoPagamentoEletronico.EtapaAprovacao(pagamentoEletronico, TipoServicoMultisoftware, false);

                repPagamentoEletronico.Atualizar(pagamentoEletronico);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = pagamentoEletronico.Codigo, SituacaoAutorizacaoPagamentoEletronico = pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar nova remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadRelatorioRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);

                // Valida
                if (pagamentoEletronico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.HasValue && pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value != SituacaoAutorizacaoPagamentoEletronico.Finalizada && pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value != SituacaoAutorizacaoPagamentoEletronico.Iniciada)
                    return new JsonpResult(false, true, "A situação atual do pagamento (" + pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value.ObterDescricao() + ") não permite realizar o download da remessa.");

                byte[] pdf = ReportRequest.WithType(ReportType.RemessaPagamento)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoPagamento", codigo.ToString())
                    .CallReport()
                    .GetContentFile();
                
                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "Remessa de Pagamento " + codigo.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadArquivoRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Valida
                if (codigo == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);

                if (pagamentoEletronico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.HasValue && pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value != SituacaoAutorizacaoPagamentoEletronico.Finalizada && pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value != SituacaoAutorizacaoPagamentoEletronico.Iniciada)
                    return new JsonpResult(false, true, "A situação atual do pagamento (" + pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value.ObterDescricao() + ") não permite realizar o download da remessa.");

                MemoryStream arquivoINPUT = new MemoryStream();
                if (Servicos.Embarcador.Financeiro.RemessaPagamento.GerarRemessaPagamento(codigo, _conexao.StringConexao, ref arquivoINPUT, out string msgErro, out int numero, ConfiguracaoEmbarcador.NaoUtilizarDeafultParaPagamentoDeTributos, out string nomeArquivo))
                    return Arquivo(arquivoINPUT, "application/txt", !string.IsNullOrEmpty(nomeArquivo) ? nomeArquivo : "PG" + numero.ToString("n0") + ".txt");
                else
                    return new JsonpResult(false, msgErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);

                if (pagamentoEletronico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                Servicos.Embarcador.Financeiro.PagamentoEletronico servicoPagamentoEletronico = new Servicos.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                servicoPagamentoEletronico.EtapaAprovacao(pagamentoEletronico, TipoServicoMultisoftware, ConfiguracaoEmbarcador.BloquearSemRegraAprovacaoOrdemServico);

                repPagamentoEletronico.Atualizar(pagamentoEletronico);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.PagamentoEletronico.ObterDetalhesAprovacao(pagamentoEletronico, unitOfWork));
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente ObterFiltrosPesquisaTituloPendente()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente()
            {
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataProgramacaoPagamentoInicial = Request.GetDateTimeParam("DataProgramacaoPagamentoInicial"),
                DataProgramacaoPagamentoFinal = Request.GetDateTimeParam("DataProgramacaoPagamentoFinal"),
                ValorTitulo = Request.GetDecimalParam("ValorTitulo"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                CodigoBanco = Request.GetIntParam("Banco"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigoPagamentoEletronico = Request.GetIntParam("PagamentoEletronico"),
                CnpjPessoa = Request.GetDoubleParam("Fornecedor"),
                SituacaoBoletoTitulo = Request.GetEnumParam<SituacaoBoletoTitulo>("SituacaoBoletoTitulo"),
                SituacaoPagamentoEletronico = Request.GetEnumParam<SituacaoPagamentoEletronico>("SituacaoPagamentoEletronico"),
                TiposDocumento = Request.GetListEnumParam<TipoDocumentoPesquisaTitulo>("TipoDocumento"),
                SomenteTitulosDeNegociacao = Dominio.Enumeradores.OpcaoSimNao.Todos,
                FormaTitulo = FormaTitulo.Todos,
                TipoTitulo = TipoTitulo.Pagar,
                TipoServico = TipoServicoMultisoftware,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                DataAutorizacaoInicial = Request.GetDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("DataAutorizacaoFinal")
            };
        }

        #endregion
    }
}

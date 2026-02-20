using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.Entidades.Embarcador.Pedidos;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize(new string[] { "BuscarDadosEmpresa", "BuscarPorCodigo" }, "NotasFiscais/NotaFiscalEletronica")]
    public class NotaFiscalEletronicaController : BaseController
    {
        #region Construtores

        public NotaFiscalEletronicaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                int numeroInicial, numeroFinal, serie, naturezaOperacao, atividade = 0, empresa = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Serie"), out serie);
                int.TryParse(Request.Params("NaturezaOperacao"), out naturezaOperacao);
                int.TryParse(Request.Params("Atividade"), out atividade);
                int.TryParse(Request.Params("Empresa"), out empresa);

                double cnpjcpfPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjcpfPessoa);

                DateTime dataInicial, dataFinal, dataProcessamento, dataSaida;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                DateTime.TryParse(Request.Params("DataProcessamento"), out dataProcessamento);
                DateTime.TryParse(Request.Params("DataSaida"), out dataSaida);

                Dominio.Enumeradores.StatusNFe status;
                Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao;
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("TipoEmissao"), out tipoEmissao);

                string chave = Request.Params("Chave");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.NumeroNota, "Numero", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.Serie, "Serie", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao, "DataEmissao", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.DataSaida, "DataSaida", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.UltimoRetornoSefaz, "UltimoStatusSEFAZ", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.Pessoa, "NomePessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.NaturezaDaOperacao, "NaturezaOperacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.Chave, "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.NotaFiscalEletronica.Valor, "ValorTotalNota", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("CodigoNaturezaOperacao", false);
                grid.AdicionarCabecalho("AtivarEnvioDanfeSMS", false);
                grid.AdicionarCabecalho("HabilitarEtiquetaProdutosNFe", false);

                string ordenacao = "";
                ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (string.IsNullOrWhiteSpace(ordenacao) || ordenacao == "Codigo")
                    ordenacao = "Numero";

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                    empresa = this.Usuario.Empresa.Codigo;
                }

                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> listaNotaFiscal = repNotaFiscal.Consultar(numeroInicial, numeroFinal, serie, naturezaOperacao, atividade, cnpjcpfPessoa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, chave, empresa, tipoAmbiente, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarConsulta(numeroInicial, numeroFinal, serie, naturezaOperacao, atividade, cnpjcpfPessoa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, chave, empresa, tipoAmbiente));
                var lista = (from p in listaNotaFiscal
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.Numero,
                                 p.Numero,
                                 Serie = p.EmpresaSerie?.Numero ?? 0,
                                 p.DataEmissao,
                                 DataSaida = p.DataSaida != null && p.DataSaida.HasValue ? p.DataSaida.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.DescricaoStatus,
                                 p.UltimoStatusSEFAZ,
                                 NomePessoa = p.Cliente?.Nome ?? string.Empty,
                                 NaturezaOperacao = p.NaturezaDaOperacao?.Descricao ?? string.Empty,
                                 p.Chave,
                                 ValorTotalNota = p.ValorTotalNota.ToString("n2"),
                                 p.Status,
                                 CodigoNaturezaOperacao = p.NaturezaDaOperacao != null ? p.NaturezaDaOperacao.Codigo : 0,
                                 AtivarEnvioDanfeSMS = p.Empresa?.AtivarEnvioDanfeSMS ?? false,
                                 HabilitarEtiquetaProdutosNFe = p.Empresa?.HabilitarEtiquetaProdutosNFe ?? false
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNotaFiscal.BuscarPorCodigo(codigo);

                if (nfe == null)
                    return new JsonpResult(false, "NF-e não encontrado.");

                var retorno = new
                {
                    NFe = ObterDetalhesNFe(nfe),
                    ProdutosServicos = ObterItensNFe(nfe, unitOfWork),
                    LotesProdutos = ObterLotesItensNFe(nfe, unitOfWork),
                    Totalizador = ObterTotalizadorNFe(nfe),
                    Observacao = ObterObservacaoNFe(nfe),
                    Transporte = ObterTransporteNFe(nfe),
                    RetiradaEntrega = ObterRetiradaEntregaNFe(nfe),
                    Referencia = ObterReferenciaNFe(nfe),
                    Parcelas = ObterParcelamentoNFe(nfe),
                    ExportacaoCompra = ObterExportacaoCompraNFe(nfe)
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarPedidoVendaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<int> codigos = new List<int>();
                codigos = RetornaCodigosPedidos(unitOfWork);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda = repPedidoVenda.BuscarPorCodigos(codigos);

                if (pedidosVenda == null || pedidosVenda.Count <= 0)
                    return new JsonpResult(false, "Pedido Venda não encontrado.");

                var retorno = new
                {
                    NFe = ObterDetalhesPedidoVenda(pedidosVenda[0]),
                    ProdutosServicos = ObterItensPedidoVenda(pedidosVenda, codigos, unitOfWork),
                    Totalizador = ObterTotalizadorPedidoVenda(pedidosVenda),
                    Observacao = ObterObservacaoPedidoVenda(pedidosVenda),
                    Parcelas = ObterParcelasPedidoVenda(pedidosVenda, codigos, unitOfWork)
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarDocumentoEntradaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<int> codigos = new List<int>();
                codigos = RetornaCodigosPedidos(unitOfWork);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentosEntrada = repDocumentoEntrada.BuscarPorCodigos(codigos);

                if (documentosEntrada == null || documentosEntrada.Count <= 0)
                    return new JsonpResult(false, "Documento Entrada não encontrado.");
                if (documentosEntrada[0].Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                    return new JsonpResult(false, "Documento Entrada não se encontra finalizado.");

                var retorno = new
                {
                    NFe = ObterDetalhesDocumentoEntrada(documentosEntrada[0]),
                    ProdutosServicos = ObterItensDocumentoEntrada(documentosEntrada, codigos, unitOfWork)
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar os dados do documento de entrada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                int codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (codigoEmpresa == 0)
                    return new JsonpResult(false, "Favor selecione uma Empresa cadastrada.");

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe>();
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".txt")
                    {
                        Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
                        List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica> nfeIntegracao = serNotaFiscalEletronica.ConverterLayoutTXTParaNFe(file.InputStream, unitOfWork, this.Usuario.Empresa);

                        for (int i = 0; i < nfeIntegracao.Count(); i++)
                        {
                            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe;
                            nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();

                            if (!repNFe.NotaEmitida(nfeIntegracao[i].Numero, nfeIntegracao[i].Serie, codigoEmpresa))
                                serNotaFiscalEletronica.SalvarDadosNFe(ref nfe, nfeIntegracao[i], permissoes, Usuario, unitOfWork, Usuario.Empresa, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                        }

                        unitOfWork.CommitChanges();

                        return new JsonpResult(true, "Sucesso");
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(true, "Layout do arquivo está fora de padrão.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar da NFe. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EmitirNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

                dynamic dynNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("NFe"));
                List<int> codigoPedidoVenda = new List<int>();
                List<int> codigoDocumentoEntrada = new List<int>();
                if (dynNFe.NFe.CodigoPedidoVenda != null)
                    codigoPedidoVenda = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFe.NFe.CodigoPedidoVenda);
                if (dynNFe.NFe.CodigoDocumentoEntrada != null)
                    codigoDocumentoEntrada = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFe.NFe.CodigoDocumentoEntrada);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe>();
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao = serNotaFiscalEletronica.ConverterDynamicParaNFe(dynNFe, unitOfWork, this.Usuario.Empresa);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFe(nfeIntegracao.Codigo);

                if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                    return new JsonpResult(false, false, "NF-e vinculada a um Faturamento Mensal, impossível Atualizar.");

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe;
                if (nfeIntegracao.Codigo == 0)
                    nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                else
                {
                    nfe = repNFe.BuscarPorCodigo(nfeIntegracao.Codigo);

                    if (nfe.Status == StatusNFe.Autorizado || nfe.Status == StatusNFe.Inutilizado || nfe.Status == StatusNFe.Cancelado || nfe.Status == StatusNFe.Denegado)
                        return new JsonpResult(false, false, $"A Nota Fiscal está com o Status {nfe.Status.ObterDescricao()}! Não sendo mais possível emitir.");
                }

                Repositorio.NaturezaDaOperacao repTipoMovimentoNatureza = new Repositorio.NaturezaDaOperacao(unitOfWork);
                if (repTipoMovimentoNatureza.PossuiTipoMovimentoPorCodigo(nfeIntegracao.NaturezaDaOperacao.Codigo))
                    return new JsonpResult(false, false, "Processo cancelado: Natureza de operação está marcada para gerar títulos e não possui Tipo de Movimento vinculado, favor selecionar no cadastro da natureza de operação!");

                if (repTipoMovimentoNatureza.ObrigarParcelasNFe(nfeIntegracao.NaturezaDaOperacao.Codigo))
                    if (nfeIntegracao.ParcelasNFe.Count < 1)
                        return new JsonpResult(false, false, "Processo cancelado: Natureza de operação está marcada para gerar títulos e as parcelas não foram geradas!");

                serNotaFiscalEletronica.SalvarDadosNFe(ref nfe, nfeIntegracao, permissoes, Usuario, unitOfWork, Usuario.Empresa, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                string retornoValidacao = "";
                retornoValidacao = ValidarCampos(nfe, unitOfWork, out bool cnpj);
                if (!string.IsNullOrWhiteSpace(retornoValidacao))
                    return new JsonpResult(false, retornoValidacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Emitiu NF-e.", unitOfWork);
                unitOfWork.CommitChanges();

                if (codigoPedidoVenda.Count > 0)
                {
                    if (!SalvarVinculoNotaPedidoVenda(nfe, codigoPedidoVenda, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Pedido com a Nota! Favor contatar o suporte imediatamente.");
                }
                else if (codigoDocumentoEntrada.Count > 0)
                {
                    if (!SalvarVinculoNotaDocumentoEntrada(nfe, codigoDocumentoEntrada, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Documento de Entrada com a Nota! Favor contatar o suporte imediatamente.");
                }

                string retorno = EmitirNFe(nfe.Codigo, unitOfWork, Request.Params("Relatorio"), Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), this.Usuario);
                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

                dynamic dynNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("NFe"));
                List<int> codigoPedidoVenda = new List<int>();
                List<int> codigoDocumentoEntrada = new List<int>();
                if (dynNFe.NFe.CodigoPedidoVenda != null)
                    codigoPedidoVenda = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFe.NFe.CodigoPedidoVenda);
                if (dynNFe.NFe.CodigoDocumentoEntrada != null)
                    codigoDocumentoEntrada = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFe.NFe.CodigoDocumentoEntrada);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe>();
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao = serNotaFiscalEletronica.ConverterDynamicParaNFe(dynNFe, unitOfWork, this.Usuario.Empresa);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFe(nfeIntegracao.Codigo);
                if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                    return new JsonpResult(false, false, "NF-e vinculada a um Faturamento Mensal, impossível Atualizar.");

                Repositorio.NaturezaDaOperacao repTipoMovimentoNatureza = new Repositorio.NaturezaDaOperacao(unitOfWork);
                if (repTipoMovimentoNatureza.PossuiTipoMovimentoPorCodigo(nfeIntegracao.NaturezaDaOperacao.Codigo))
                    return new JsonpResult(false, false, "Processo cancelado: Natureza de operação está marcada para gerar títulos e não possui Tipo de Movimento vinculado, favor selecionar no cadastro da natureza de operação!");

                if (repTipoMovimentoNatureza.ObrigarParcelasNFe(nfeIntegracao.NaturezaDaOperacao.Codigo))
                    if (nfeIntegracao.ParcelasNFe.Count < 1)
                        return new JsonpResult(false, false, "Processo cancelado: Natureza de operação está marcada para gerar títulos e as parcelas não foram geradas!");

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe;
                if (nfeIntegracao.Codigo == 0)
                    nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                else
                {
                    nfe = repNFe.BuscarPorCodigo(nfeIntegracao.Codigo);

                    if (nfe.Status == StatusNFe.Autorizado || nfe.Status == StatusNFe.Inutilizado || nfe.Status == StatusNFe.Cancelado || nfe.Status == StatusNFe.Denegado)
                        return new JsonpResult(false, false, $"A Nota Fiscal está com o Status {nfe.Status.ObterDescricao()}! Não sendo mais possível atualizar.");
                }

                serNotaFiscalEletronica.SalvarDadosNFe(ref nfe, nfeIntegracao, permissoes, Usuario, unitOfWork, Usuario.Empresa, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                bool cnpj = false;
                string retornoValidacao = "";
                retornoValidacao = ValidarCampos(nfe, unitOfWork, out cnpj);
                if (!string.IsNullOrWhiteSpace(retornoValidacao))
                    return new JsonpResult(false, retornoValidacao);

                if (!string.IsNullOrWhiteSpace(nfe.TranspCNPJCPF) && (nfe.TranspCNPJCPF.Length == 11 || nfe.TranspCNPJCPF.Length == 14))
                {
                    if (!CadastrarTransportadora(nfe, cnpj, unitOfWork))
                        return new JsonpResult(false, "Favor informar todos os dados da Transportadora para a mesma ser cadastrada!");
                }
                if (!string.IsNullOrWhiteSpace(nfe.TranspPlacaVeiculo) && nfe.TranspPlacaVeiculo.Length == 7)
                {
                    if (!CadastrarVeículo(nfe, unitOfWork))
                        return new JsonpResult(false, "Favor informar o Estado do Veículo para o mesmo ser cadastrado!");
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Salvou NF-e.", unitOfWork);
                unitOfWork.CommitChanges();

                if (codigoPedidoVenda.Count > 0)
                {
                    if (!SalvarVinculoNotaPedidoVenda(nfe, codigoPedidoVenda, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Pedido com a Nota! Favor contatar o suporte imediatamente.");
                }
                else if (codigoDocumentoEntrada.Count > 0)
                {
                    if (!SalvarVinculoNotaDocumentoEntrada(nfe, codigoDocumentoEntrada, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Documento de Entrada com a Nota! Favor contatar o suporte imediatamente.");
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar salvar o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

                if (nfe.Status == StatusNFe.Autorizado || nfe.Status == StatusNFe.Inutilizado || nfe.Status == StatusNFe.Cancelado || nfe.Status == StatusNFe.Denegado)
                    return new JsonpResult(false, false, $"A Nota Fiscal está com o Status {nfe.Status.ObterDescricao()}! Não sendo mais possível atualizar.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Enviou NF-e.", unitOfWork);
                unitOfWork.CommitChanges();

                string retorno = EmitirNFe(nfe.Codigo, unitOfWork, Request.Params("Relatorio"), Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), this.Usuario);
                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VisualizarDANFE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                var retorno = z.VisualizarDANFE(codigoNFe, unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar visualizar a DANFE da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarEmailNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                string urlBase = _conexao.ObterHost;

                var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string relatorio = Request.Params("Relatorio");
                string caminhoRelatoriosEmbarcador = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
                var retorno = z.EnviarEmailNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, this.Usuario, urlBase);

                unitOfWork.CommitChanges();
                unitOfWork.Dispose();

                unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Enviou E-mail NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar enviar e-mail da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> InutilizarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoNFe = Request.GetIntParam("Codigo");

                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string retorno = z.InutilizarNFe(codigoNFe, unitOfWork);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Inutilizou NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar inutilizar a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> InutilizarFaixaNotas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa repInutilizarFaixa = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa inutilizacaoFaixa = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa();

                string justificativaInutilizacao = Request.GetStringParam("Justificativa");
                if (justificativaInutilizacao.Length < 15)
                    return new JsonpResult(false, true, "A justificava deve possuir no mínimo 15 caracteres.");

                inutilizacaoFaixa.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : repEmpresa.BuscarPorCodigo(Request.GetIntParam("Empresa"));
                inutilizacaoFaixa.NumeroInicial = Request.GetIntParam("NumeroInicial");
                inutilizacaoFaixa.NumeroFinal = Request.GetIntParam("NumeroFinal");
                inutilizacaoFaixa.Serie = Request.GetIntParam("Serie");
                inutilizacaoFaixa.Justificativa = Utilidades.String.RemoveAccents(justificativaInutilizacao);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                if (repNotaFiscal.ExisteFaixaNotaPorNumeroSerie(Empresa.Codigo, inutilizacaoFaixa.NumeroInicial, inutilizacaoFaixa.NumeroFinal, inutilizacaoFaixa.Serie))
                    return new JsonpResult(false, true, "Existem números na faixa e série indicada, os quais estão gravados no sistema, favor inutilizar estes através do formulário de cadastro!");

                repInutilizarFaixa.Inserir(inutilizacaoFaixa);
                unitOfWork.CommitChanges();

                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retorno = z.InutilizarFaixaNFe(inutilizacaoFaixa.Serie.ToString(), inutilizacaoFaixa.NumeroInicial.ToString(), inutilizacaoFaixa.NumeroFinal.ToString(), inutilizacaoFaixa.Justificativa, inutilizacaoFaixa.Empresa.Localidade.Estado.CodigoIBGE, "55", inutilizacaoFaixa.Empresa, unitOfWork);
                inutilizacaoFaixa.CodigoRetorno = retorno.cStat.ToInt();
                inutilizacaoFaixa.MensagemRetorno = retorno.xMotivo;
                repInutilizarFaixa.Atualizar(inutilizacaoFaixa);
                if (retorno.cStat == "102" || retorno.cStat == "135" || retorno.cStat == "206" || retorno.cStat == "256" || retorno.cStat == "563" || retorno.cStat == "662")
                    return new JsonpResult(true, true, retorno.cStat + " - " + retorno.xMotivo);
                else
                    return new JsonpResult(false, true, retorno.cStat + " - " + retorno.xMotivo);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DuplicarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoNFe);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal repNotaFiscalObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia repNotaFiscalReferencia = new Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes repNotaFiscalProdutosLotes = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNFe);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal> observacoes = repNotaFiscalObservacaoFiscal.BuscarPorNota(codigoNFe);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> parcelas = repNotaFiscalParcela.BuscarPorNota(codigoNFe);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia referencia = repNotaFiscalReferencia.BuscarPorNota(codigoNFe);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes> lotes = repNotaFiscalProdutosLotes.BuscarPorNota(codigoNFe);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal novaNotaFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                novaNotaFiscal = notaFiscal.Clonar();
                novaNotaFiscal.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                novaNotaFiscal.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                novaNotaFiscal.VersaoNFe = this.Usuario.Empresa.VersaoNFe;
                novaNotaFiscal.DataEmissao = DateTime.Now;
                novaNotaFiscal.DataSaida = DateTime.Now;
                novaNotaFiscal.DataPrestacaoServico = DateTime.Now;
                novaNotaFiscal.UltimoStatus = "";
                novaNotaFiscal.UltimoStatusSEFAZ = "";
                novaNotaFiscal.NumeroRecibo = "";
                novaNotaFiscal.Usuario = this.Usuario;

                novaNotaFiscal.Numero = repNotaFiscal.BuscarUltimoNumero(notaFiscal.Empresa.Codigo, notaFiscal.EmpresaSerie.Numero, this.Usuario.Empresa.TipoAmbiente, "55") + 1;
                int proximoNumeroSerie = repEmpresaSerie.BuscarProximoNumeroDocumentoPorSerie(notaFiscal.Empresa.Codigo, notaFiscal.EmpresaSerie.Numero, Dominio.Enumeradores.TipoSerie.NFe);
                if (novaNotaFiscal.Numero < proximoNumeroSerie)
                    novaNotaFiscal.Numero = proximoNumeroSerie;

                novaNotaFiscal.Chave = null;
                novaNotaFiscal.DataProcessamento = null;
                novaNotaFiscal.Protocolo = null;
                novaNotaFiscal.ItensNFe = null;
                novaNotaFiscal.ParcelasNFe = null;
                novaNotaFiscal.ReferenciaNFe = null;

                repNotaFiscal.Inserir(novaNotaFiscal);

                for (int i = 0; i < observacoes.Count; i++)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal novaObservacao = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal();
                    novaObservacao = observacoes[i].Clonar();
                    novaObservacao.NotaFiscal = novaNotaFiscal;

                    repNotaFiscalObservacaoFiscal.Inserir(novaObservacao);
                }

                for (int i = 0; i < itens.Count; i++)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos novaItem = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                    novaItem = itens[i].Clonar();
                    novaItem.NotaFiscal = novaNotaFiscal;
                    novaItem.LotesItem = null;

                    repNotaFiscalProdutos.Inserir(novaItem);

                    for (int j = 0; j < lotes.Count; j++)
                    {
                        if (lotes[j].NotaFiscalProdutos.Codigo == itens[i].Codigo)
                        {
                            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes novaLote = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes();
                            novaLote = lotes[j].Clonar();
                            novaLote.NotaFiscalProdutos = novaItem;

                            repNotaFiscalProdutosLotes.Inserir(novaLote);
                        }
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, notaFiscal, null, "Duplicou NF-e para " + novaNotaFiscal.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar duplicar a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CancelarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                int codigoNFe = Request.GetIntParam("Codigo");
                string justificativa = Request.GetStringParam("Justificativa");

                DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string retorno = z.CancelarNFe(dataEmissao, codigoNFe, unitOfWork, justificativa, Request.Params("Relatorio"),
                    Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), Usuario, TipoServicoMultisoftware, clienteAcesso);

                unitOfWork.CommitChanges();
                unitOfWork.Dispose();

                unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Cancelou NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar cancelar a nota selecionada.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CorrigirNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);
                string correcao = Request.Params("Correcao");
                DateTime dataEmissao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                var retorno = z.CartaCorrecaoNFe(dataEmissao, codigoNFe, correcao, unitOfWork, Request.Params("Relatorio"), Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), this.Usuario);

                unitOfWork.CommitChanges();
                unitOfWork.Dispose();

                unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Corrigiu NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar corrigir a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLCCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                if (codigoNFe > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarUltimoPorNota(codigoNFe);

                    if (arquivo != null && !string.IsNullOrWhiteSpace(arquivo.XMLCartaCorrecao))
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(arquivo.XMLCartaCorrecao);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(arquivo.NotaFiscal.Chave, "CCe.xml"));
                        }
                    }
                }
                return new JsonpResult(false, true, "Não foi encontrado o arquivo XML de CCe da nota selecionada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                if (codigoNFe > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);

                    if (arquivo != null && !string.IsNullOrWhiteSpace(arquivo.XMLDistribuicao))
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(arquivo.XMLDistribuicao);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(arquivo.NotaFiscal.Chave, ".xml"));
                        }
                    }
                }
                return new JsonpResult(false, true, "Não foi encontrado o arquivo XML da nota selecionada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPDF(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                if (codigoNFe > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nota = repNotaFiscal.BuscarPorCodigo(codigoNFe);

                    if (nota != null && !string.IsNullOrWhiteSpace(nota.Chave))
                    {
                        string caminhoRelatorio = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), nota.Chave + ".pdf");

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
                        {
                            byte[] data = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(caminhoRelatorio, cancellationToken);

                            if (data != null)
                            {
                                return Arquivo(data, "application/pdf", string.Concat(nota.Chave, ".pdf"));
                            }

                            //Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                            //Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(nota.Codigo);

                            //Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, arquivo.XMLDistribuicao, caminhoRelatorio, false, false);
                        }
                    }
                }
                return new JsonpResult(false, false, "Não foi encontrado o arquivo PDF da nota selecionada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);

                if (codigoNFe > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);

                    if (arquivo != null && !string.IsNullOrWhiteSpace(arquivo.XMLCancelamento))
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(arquivo.XMLCancelamento);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(arquivo.NotaFiscal.Chave, "-Cancelamento.xml"));
                        }
                    }
                }
                return new JsonpResult(false, true, "Não foi encontrado o arquivo XML de cancelamento da nota selecionada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML de cancelamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAssinador()
        {
            try
            {
                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(Servicos.FS.GetPath("C:\\SetupMultiNFe.exe")), "application/x-pkcs12", "SetupMultiNFe.exe");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do assinador de xml.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosNFe()
        {
            try
            {
                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(Servicos.FS.GetPath("C:\\InstaladoresEstruturaNFe.rar")), "application/x-pkcs12", "InstaladoresEstruturaNFe.rar");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos estrutura NF-e.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;
                if (codigoEmpresa == 0)
                    codigoEmpresa = repEmpresa.BuscarPrincipalEmissoraTMS()?.Codigo ?? 0;

                List<Dominio.Entidades.EmpresaSerie> listaSerie = repSerie.BuscarSeriesPorEmpresaTipo(codigoEmpresa, Dominio.Enumeradores.TipoSerie.NFe);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                if (listaSerie == null)
                    return new JsonpResult(false, "Série não encontrada. Por favor cadastre uma série antes de lançar uma NF-e.");

                int proximoNumero = 1, proximoNumeroSerie = 1;
                if (listaSerie != null && listaSerie.Count() == 1)
                {
                    proximoNumeroSerie = repNFe.BuscarUltimoNumero(codigoEmpresa, listaSerie[0].Numero, empresa.TipoAmbiente, "55") + 1;
                    proximoNumero = repSerie.BuscarProximoNumeroDocumentoPorSerie(codigoEmpresa, listaSerie[0].Numero, Dominio.Enumeradores.TipoSerie.NFe);
                    if (proximoNumeroSerie < proximoNumero)
                        proximoNumeroSerie = proximoNumero;
                }

                var retorno = new
                {
                    Serie = listaSerie != null && listaSerie.Count() == 1 ? new
                    {
                        Codigo = listaSerie[0].Codigo,
                        Descricao = listaSerie[0].Numero
                    } : null,
                    CidadeEmpresa = empresa.Localidade != null ? new
                    {
                        Codigo = empresa.Localidade.Codigo,
                        Descricao = empresa.Localidade.DescricaoCidadeEstado
                    } : null,
                    Empresa = empresa != null ? new
                    {
                        Codigo = empresa.Codigo,
                        Descricao = empresa.RazaoSocial
                    } : null,
                    EmpresaSimples = empresa.OptanteSimplesNacional && !empresa.OptanteSimplesNacionalComExcessoReceitaBruta,
                    CasasQuantidadeProdutoNFe = empresa.CasasQuantidadeProdutoNFe > 0 ? empresa.CasasQuantidadeProdutoNFe : 4,
                    CasasValorProdutoNFe = empresa.CasasValorProdutoNFe > 0 ? empresa.CasasValorProdutoNFe : 5,
                    ProximoNumero = proximoNumeroSerie,
                    empresa.EmitirVendaPrazoNFCe,
                    empresa.SubtraiDescontoBaseICMS,
                    empresa.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual,
                    empresa.HabilitarEtiquetaProdutosNFe
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
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> DanfeSMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chaveNotaFiscal = Request.GetStringParam("ChaveNFe");
                int codigoEmpresa = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorChave(chaveNotaFiscal, codigoEmpresa);

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

                //ContentResult = Mostra uma página em branco com apenas essa mensagem, caso venha a ocorrer o erro
                if (nfe == null)
                    return new ContentResult() { Content = "Não foi encontrada a Nota Fiscal!" };

                if (!nfe.Empresa.ArmazenarDanfeParaSMS)
                    return new ContentResult() { Content = "Nota Fiscal não disponível!." };

                string folderDanfeSMS = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDanfeSMS;
                if (string.IsNullOrWhiteSpace(folderDanfeSMS))
                    return new ContentResult() { Content = "Nota Fiscal não configurada!." };

                string nomeArquivo = nfe.Chave;
                if (nfe.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                    nomeArquivo = nomeArquivo + "-canc";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Baixou a DANFE por URL enviada via SMS.", unitOfWork);
                string caminhoArquivo = serNotaFiscalEletronica.GerarDanfeParaSMS(unitOfWork, nfe);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                    return new ContentResult() { Content = "Anexo da Nota Fiscal não encontrado!" };

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo);
                if (arquivo == null)
                    return new ContentResult() { Content = "Arquivo da Nota Fiscal inválido!" };

                return Arquivo(arquivo, "application/pdf", nomeArquivo + ".pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new ContentResult() { Content = "Ocorreu uma falha ao baixar a nota fiscal solicitada via SMS." };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarSMSNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();
                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);
                string retorno = srvSMS.EnviarMensagem(nfe, clienteAcesso.URLAcesso, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Enviou SMS NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar enviar SMS da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImprimirEtiquetaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R103_EtiquetaVolume, TipoServicoMultisoftware);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repItens = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R234_EtiquetaVolumeNFe, TipoServicoMultisoftware, "Etiqueta de Volume NF-e", "NFe", "EtiquetaVolumeNFe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe> dadosEtiquetaNFe = new List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe>();
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repItens.BuscarPorNota(codigo);

                if (itens != null && itens.Count > 0)
                {
                    foreach (var item in itens)
                    {
                        for (int i = 0; i < item.Quantidade; i++)
                        {
                            dadosEtiquetaNFe.Add(new Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe()
                            {
                                Cliente = item.NotaFiscal.Cliente.Nome,
                                Numero = item.NotaFiscal.Numero,
                                Volume = i + 1,
                                TotalVolume = item.Quantidade
                            });
                        }
                    }

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    string stringConexao = _conexao.StringConexao;
                    string nomeCliente = Cliente.NomeFantasia;
                    string caminhoLogo = this.Usuario?.Empresa?.CaminhoLogoDacte ?? "";

                    Task.Factory.StartNew(() => GerarEtiquetasVolume(codigo, nomeCliente, stringConexao, relatorioControleGeracao, dadosEtiquetaNFe, caminhoLogo));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de item selecionado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Nenhum registro de item selecionado.");
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarEtiquetasVolume(int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe> dadosEtiquetaVolume, string caminhoLogo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                var result = ReportRequest.WithType(ReportType.EtiquetaVolumeNFe)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosEtiquetaVolume", dadosEtiquetaVolume.ToJson())
                    .AddExtraData("caminhoLogo", caminhoLogo)
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string EmitirNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            string mensagem = "";
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            if (nfe.Empresa.DataFinalCertificado != null && nfe.Empresa.DataFinalCertificado > DateTime.MinValue)
            {
                if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                    return "O certificado digital cadastrado na empresa se encontra vencido.";
            }

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);

            if (nfe == null)
                mensagem = "O NF-e informado não foi localizado";
            else if (nfe != null && (itens == null || itens.Count == 0))
                mensagem = "A NF-e não possui nenhum item lançado.";
            else
            {
                bool controlarEstoqueNegativo = ConfiguracaoEmbarcador.ControlarEstoqueNegativo || nfe.Empresa.ControlarEstoqueNegativo;
                if (controlarEstoqueNegativo && nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida)
                {
                    foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item in itens)
                    {
                        if (item.Produto != null && (item.CFOP?.GeraEstoque ?? false) &&
                            !servicoEstoque.ValidarProdutoComEstoque(out string erro, item.Produto, item.Quantidade, null, item.NotaFiscal.Empresa, ConfiguracaoEmbarcador, item.LocalArmazenamento))
                            return erro;
                    }
                }

                AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                unitOfWorkAdmin.Dispose();

                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string urlBase = _conexao.ObterHost;
                mensagem = z.CriarEnviarNFe(codigoNFe, unitOfWork, TipoServicoMultisoftware, relatorio, caminhoRelatoriosEmbarcador, usuario, "55", 1, true, true, clienteAcesso, urlBase);
            }

            return mensagem;
        }

        private object ObterDetalhesNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                Codigo = nfe.Codigo,
                Numero = nfe.Numero,
                Serie = new { Codigo = nfe.EmpresaSerie.Codigo, Descricao = nfe.EmpresaSerie.Numero },
                Empresa = new { Codigo = nfe.Empresa.Codigo, Descricao = nfe.Empresa.RazaoSocial },
                TipoEmissao = nfe.TipoEmissao,
                DataEmissao = nfe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                DataSaida = nfe.DataSaida != null && nfe.DataSaida.HasValue ? nfe.DataSaida.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Chave = nfe.Chave,
                Status = nfe.Status,
                Protocolo = nfe.Protocolo,
                DataProcessamento = nfe.DataProcessamento != null ? nfe.DataProcessamento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                DataPrestacaoServico = nfe.DataPrestacaoServico != null ? nfe.DataPrestacaoServico.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Finalidade = nfe.Finalidade,
                IndicadorPresenca = nfe.IndicadorPresenca,
                IndicadorIntermediador = nfe.IndicadorIntermediador?.ToString("d") ?? string.Empty,
                Pessoa = new
                {
                    Codigo = nfe.Cliente.Codigo,
                    Descricao = nfe.Cliente.Nome + " (" + nfe.Cliente.Localidade.DescricaoCidadeEstado + ")"
                },
                Atividade = new { Codigo = nfe.Atividade.Codigo, Descricao = nfe.Atividade.Descricao },
                CidadePrestacao = new { Codigo = nfe.LocalidadePrestacaoServico.Codigo, Descricao = nfe.LocalidadePrestacaoServico.DescricaoCidadeEstado },
                NaturezaOperacao = new { Codigo = nfe.NaturezaDaOperacao.Codigo, Descricao = nfe.NaturezaDaOperacao.CFOP != null ? nfe.NaturezaDaOperacao.Descricao + " (" + nfe.NaturezaDaOperacao.CFOP.CodigoCFOP + ")" : nfe.NaturezaDaOperacao.Descricao },
                Intermediador = new { Codigo = nfe.Intermediador?.Codigo ?? 0, Descricao = nfe.Intermediador?.Nome ?? string.Empty }
            };

            return retorno;
        }

        private object ObterDetalhesPedidoVenda(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda)
        {
            var retorno = new
            {
                Pessoa = new
                {
                    Codigo = pedidoVenda.Cliente.Codigo,
                    Descricao = pedidoVenda.Cliente.Nome + " (" + pedidoVenda.Cliente.Localidade.DescricaoCidadeEstado + ")"
                },
                Atividade = new { Codigo = pedidoVenda.Cliente.Atividade.Codigo, Descricao = pedidoVenda.Cliente.Atividade.Descricao }
            };

            return retorno;
        }

        private object ObterDetalhesDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada)
        {
            var retorno = new
            {
                Pessoa = new
                {
                    Codigo = documentoEntrada.Fornecedor.Codigo,
                    Descricao = documentoEntrada.Fornecedor.Nome + " (" + documentoEntrada.Fornecedor.Localidade.DescricaoCidadeEstado + ")"
                },
                Atividade = new { Codigo = documentoEntrada.Fornecedor.Atividade.Codigo, Descricao = documentoEntrada.Fornecedor.Atividade.Descricao }
            };

            return retorno;
        }

        private object ObterItensNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(nfe.Codigo);
            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

            var retorno = (from obj in itens
                           select new
                           {
                               obj.Codigo,
                               CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                               CodigoServico = obj.Servico != null ? obj.Servico.Codigo : 0,
                               CodigoCFOP = obj.CFOP != null ? obj.CFOP.Codigo : 0,
                               CodigoCSTICMS = obj.CSTICMS != null ? (int)obj.CSTICMS : 0,
                               OrigemMercadoria = obj.OrigemMercadoria != null ? (int)obj.OrigemMercadoria : obj.Produto != null && obj.Produto.OrigemMercadoria != null ? (int)obj.Produto.OrigemMercadoria : 0,
                               CodigoNFCI = obj.CodigoNFCI != null && !string.IsNullOrWhiteSpace((string)obj.CodigoNFCI) ? (string)obj.CodigoNFCI : "",
                               ValorFrete = obj.ValorFrete.ToString("n2"),
                               ValorSeguro = obj.ValorSeguro.ToString("n2"),
                               ValorOutras = obj.ValorOutrasDespesas.ToString("n2"),
                               ValorDesconto = obj.ValorDesconto.ToString("n2"),
                               NumeroOrdemCompra = obj.NumeroOrdemCompra,
                               NumeroItemOrdemCompra = obj.NumeroItemOrdemCompra,
                               AliquotaRemICMSRet = obj.AliquotaRemICMSRet.ToString("n2"),
                               ValorICMSMonoRet = obj.ValorICMSMonoRet.ToString("n2"),
                               BCICMS = obj.BCICMS.ToString("n2"),
                               ReducaoBCICMS = obj.ReducaoBCICMS.ToString("n6"),
                               AliquotaICMS = obj.AliquotaICMS.ToString("n2"),
                               ValorICMS = obj.ValorICMS.ToString("n2"),
                               BCICMSDestino = obj.BCICMSDestino.ToString("n2"),
                               AliquotaICMSDestino = obj.AliquotaICMSDestino.ToString("n2"),
                               AliquotaICMSInterno = obj.AliquotaICMSInterno.ToString("n2"),
                               PercentualPartilha = obj.PercentualPartilha.ToString("n2"),
                               ValorICMSDestino = obj.ValorICMSDestino.ToString("n2"),
                               ValorICMSRemetente = obj.ValorICMSRemetente.ToString("n2"),
                               AliquotaFCP = obj.AliquotaFCP.Value.ToString("n2"),
                               ValorFCP = obj.ValorFCP.Value.ToString("n2"),
                               BCICMSST = obj.BCICMSST.ToString("n2"),
                               ReducaoBCICMSST = obj.ReducaoBCICMSST.ToString("n2"),
                               PercentualMVA = obj.MVAICMSST.ToString("n2"),
                               AliquotaICMSST = obj.AliquotaICMSST.ToString("n2"),
                               AliquotaInterestadual = obj.AliquotaICMSSTInterestadual.ToString("n2"),
                               CodigoCSTPIS = obj.CSTPIS != null ? (int)obj.CSTPIS : 0,
                               BCPIS = obj.BCPIS.ToString("n2"),
                               ReducaoBCPIS = obj.ReducaoBCPIS.ToString("n2"),
                               AliquotaPIS = obj.AliquotaPIS.ToString("n2"),
                               ValorPIS = obj.ValorPIS.ToString("n2"),
                               CodigoCSTCOFINS = obj.CSTCOFINS != null ? (int)obj.CSTCOFINS : 0,
                               BCCOFINS = obj.BCCOFINS.ToString("n2"),
                               ReducaoBCCOFINS = obj.ReducaoBCCOFINS.ToString("n2"),
                               AliquotaCOFINS = obj.AliquotaCOFINS.ToString("n2"),
                               ValorCOFINS = obj.ValorCOFINS.ToString("n2"),
                               CodigoCSTIPI = obj.CSTIPI != null ? (int)obj.CSTIPI : 0,
                               BCIPI = obj.BCIPI.ToString("n2"),
                               ReducaoBCIPI = obj.ReducaoBCIPI.ToString("n2"),
                               AliquotaIPI = obj.AliquotaIPI.ToString("n2"),
                               BCISS = obj.BaseISS.ToString("n2"),
                               AliquotaISS = obj.AliquotaISS.ToString("n2"),
                               ValorISS = obj.ValorISS.ToString("n2"),
                               BaseDeducaoISS = obj.BCDeducao.Value.ToString("n2"),
                               OutrasRetencoesISS = obj.OutrasRetencoes.Value.ToString("n2"),
                               DescontoIncondicional = obj.DescontoIncondicional.Value.ToString("n2"),
                               DescontoCondicional = obj.DescontoCondicional.Value.ToString("n2"),
                               RetencaoISS = obj.RetencaoISS.Value.ToString("n2"),
                               CodigoExigibilidadeISS = (int)obj.ExigibilidadeISS,
                               GeraIncentivoFiscal = obj.IncentivoFiscal,
                               obj.ProcessoJudicial,
                               BCII = obj.BaseII.ToString("n2"),
                               DespesaII = obj.ValorDespesaII.ToString("n2"),
                               ValorII = obj.ValorII.ToString("n2"),
                               ValorIOFII = obj.ValorIOFII.ToString("n2"),
                               NumeroDocumentoII = obj.NumeroDocImportacao,
                               DataRegistroII = obj.DataRegistroImportacao != null && obj.DataRegistroImportacao.HasValue ? obj.DataRegistroImportacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                               LocalDesembaracoII = obj.LocalDesembaraco,
                               EstadoDesembaracoII = obj.UFDesembaraco,
                               DataDesembaracoII = obj.DataDesembaraco != null && obj.DataDesembaraco.HasValue ? obj.DataDesembaraco.Value.ToString("dd/MM/yyyy") : string.Empty,
                               obj.CNPJAdquirente,
                               CodigoViaTransporteII = (int)obj.ViaTransporteII,
                               ValorFreteMaritimoII = obj.ValorFreteMarinho.ToString("n2"),
                               CodigoIntermediacaoII = (int)obj.IntermediacaoII,
                               ValorICMSDesonerado = obj.ValorICMSDesonerado.Value.ToString("n2"),
                               CodigoMotivoDesoneracao = (int)obj.MotivoDesoneracao,
                               ValorICMSOperacao = obj.ValorICMSOperacao.Value.ToString("n2"),
                               AliquotaICMSOperacao = obj.AliquotaICMSOperacao.Value.ToString("n2"),
                               ValorICMSDeferido = obj.ValorICMSDiferido.Value.ToString("n2"),
                               Descricao = obj.Produto != null ? obj.Produto.Descricao : obj.Servico != null ? obj.Servico.Descricao : string.Empty,
                               CSTICMS = obj.NumeroCSTICMS,
                               CFOP = obj.CFOP != null ? obj.CFOP.CodigoCFOP : 0,
                               Qtd = obj.Quantidade.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                               ValorUnitario = obj.ValorUnitario.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                               ValorTotal = obj.ValorTotal.ToString("n2"),
                               ValorST = obj.ValorICMSST.ToString("n2"),
                               ValorIPI = obj.ValorIPI.ToString("n2"),
                               obj.Sequencial,
                               obj.CodigoItem,
                               obj.DescricaoItem,
                               UnidadeMedida = obj.Produto != null && obj.Produto.UnidadeDeMedida != null ? obj.Produto.UnidadeDeMedida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico,
                               DescricaoUnidadeMedida = obj.Produto != null && obj.Produto.UnidadeDeMedida != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida) : "SERV",
                               BaseFCPICMS = obj.BCFCPICMS != null && obj.BCFCPICMS.HasValue ? obj.BCFCPICMS.Value.ToString("n2") : 0.ToString("n2"),
                               PercentualFCPICMS = obj.PercentualFCPICMS != null && obj.PercentualFCPICMS.HasValue ? obj.PercentualFCPICMS.Value.ToString("n2") : 0.ToString("n2"),
                               ValorFCPICMS = obj.ValorFCPICMS != null && obj.ValorFCPICMS.HasValue ? obj.ValorFCPICMS.Value.ToString("n2") : 0.ToString("n2"),
                               BaseFCPICMSST = obj.BCFCPICMSST != null && obj.BCFCPICMSST.HasValue ? obj.BCFCPICMSST.Value.ToString("n2") : 0.ToString("n2"),
                               PercentualFCPICMSST = obj.PercentualFCPICMSST != null && obj.PercentualFCPICMSST.HasValue ? obj.PercentualFCPICMSST.Value.ToString("n2") : 0.ToString("n2"),
                               ValorFCPICMSST = obj.ValorFCPICMSST != null && obj.ValorFCPICMSST.HasValue ? obj.ValorFCPICMSST.Value.ToString("n2") : 0.ToString("n2"),
                               AliquotaFCPICMSST = obj.AliquotaFCPICMSST != null && obj.AliquotaFCPICMSST.HasValue ? obj.AliquotaFCPICMSST.Value.ToString("n2") : 0.ToString("n2"),
                               BaseFCPDestino = obj.BCFCPDestino != null && obj.BCFCPDestino.HasValue ? obj.BCFCPDestino.Value.ToString("n2") : 0.ToString("n2"),
                               PercentualIPIDevolvido = obj.PercentualIPIDevolvido > 0 ? obj.PercentualIPIDevolvido.ToString("n2") : 0.ToString("n2"),
                               ValorIPIDevolvido = obj.ValorIPIDevolvido > 0 ? obj.ValorIPIDevolvido.ToString("n2") : 0.ToString("n2"),
                               InformacoesAdicionaisItem = obj.InformacoesAdicionais,
                               obj.IndicadorEscalaRelevante,
                               obj.CNPJFabricante,
                               obj.CodigoBeneficioFiscal,
                               obj.CodigoANP,
                               PercentualGLP = obj.PercentualGLP > 0 ? obj.PercentualGLP.Value.ToString("n4") : 0.ToString("n4"),
                               PercentualGNN = obj.PercentualGNN > 0 ? obj.PercentualGNN.Value.ToString("n4") : 0.ToString("n4"),
                               PercentualGNI = obj.PercentualGNI > 0 ? obj.PercentualGNI.Value.ToString("n4") : 0.ToString("n4"),
                               PercentualOrigemComb = obj.PercentualOrigemComb > 0 ? obj.PercentualOrigemComb.Value.ToString("n4") : 0.ToString("n4"),
                               PercentualMisturaBiodiesel = obj.PercentualMisturaBiodiesel > 0 ? obj.PercentualMisturaBiodiesel.Value.ToString("n4") : 0.ToString("n4"),
                               ValorPartidaANP = obj.ValorPartidaANP > 0 ? obj.ValorPartidaANP.Value.ToString("n2") : 0.ToString("n2"),
                               QuantidadeTributavel = obj.QuantidadeTributavel > 0 ? obj.QuantidadeTributavel.Value.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()) : 0.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                               ValorUnitarioTributavel = obj.ValorUnitarioTributavel > 0 ? obj.ValorUnitarioTributavel.Value.ToString("n" + empresa.CasasValorProdutoNFe.ToString()) : 0.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                               obj.UnidadeDeMedidaTributavel,
                               obj.CodigoEANTributavel,
                               BCICMSSTRetido = obj.BCICMSSTRetido > 0 ? obj.BCICMSSTRetido.Value.ToString("n2") : 0.ToString("n2"),
                               AliquotaICMSSTRetido = obj.AliquotaICMSSTRetido > 0 ? obj.AliquotaICMSSTRetido.Value.ToString("n2") : 0.ToString("n2"),
                               ValorICMSSTSubstituto = obj.ValorICMSSTSubstituto > 0 ? obj.ValorICMSSTSubstituto.Value.ToString("n2") : 0.ToString("n2"),
                               ValorICMSSTRetido = obj.ValorICMSSTRetido > 0 ? obj.ValorICMSSTRetido.Value.ToString("n2") : 0.ToString("n2"),
                               BCICMSEfetivo = obj.BCICMSEfetivo > 0 ? obj.BCICMSEfetivo.Value.ToString("n2") : 0.ToString("n2"),
                               AliquotaICMSEfetivo = obj.AliquotaICMSEfetivo > 0 ? obj.AliquotaICMSEfetivo.Value.ToString("n2") : 0.ToString("n2"),
                               ReducaoBCICMSEfetivo = obj.ReducaoBCICMSEfetivo > 0 ? obj.ReducaoBCICMSEfetivo.Value.ToString("n2") : 0.ToString("n2"),
                               ValorICMSEfetivo = obj.ValorICMSEfetivo > 0 ? obj.ValorICMSEfetivo.Value.ToString("n2") : 0.ToString("n2"),
                               obj.NumeroDrawback,
                               obj.NumeroRegistroExportacao,
                               obj.ChaveAcessoExportacao,
                               CodigoLocalArmazenamento = obj.LocalArmazenamento?.Codigo ?? 0,
                               LocalArmazenamento = obj.LocalArmazenamento?.Descricao ?? string.Empty,
                               BCICMSSTDestino = obj.BCICMSSTDestino > 0 ? obj.BCICMSSTDestino.Value.ToString("n2") : 0.ToString("n2"),
                               ValorICMSSTDestino = obj.ValorICMSSTDestino > 0 ? obj.ValorICMSSTDestino.Value.ToString("n2") : 0.ToString("n2")
                           }).ToList();

            return retorno;
        }

        private object ObterItensPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda, List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoVendaItens = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> itens = repPedidoVendaItens.BuscarPorPedidos(codigosPedido);

            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

            List<object> retorno = new List<object>();
            for (int i = 0; i < itens.Count; i++)
            {
                var obj = itens[i];
                var CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0;
                var CodigoServico = obj.Servico != null ? obj.Servico.Codigo : 0;

                dynamic dynTributacao = serNotaFiscalEletronica.BuscarTributacaoItem(unitOfWork, CodigoProduto, CodigoServico, 0, pedidosVenda[0].Cliente.Atividade.Codigo, pedidosVenda[0].Cliente.CPF_CNPJ, obj.ValorTotal, 0, 0, 0, 0, 0, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, obj.Quantidade, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario?.Empresa?.Codigo ?? 0 : 0);
                dynTributacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dynTributacao.ToString().Replace("=", ":").Replace(".", "").Replace(",", ".").Replace(". ", ", "));

                retorno.Add(new
                {
                    obj.Codigo,
                    CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                    CodigoServico = obj.Servico != null ? obj.Servico.Codigo : 0,
                    CodigoCFOP = dynTributacao.CodigoCFOP != 0 ? (int)dynTributacao.CodigoCFOP : 0,
                    CodigoCSTICMS = dynTributacao.CSTICMS != 0 ? (int)dynTributacao.CSTICMS : 0,
                    OrigemMercadoria = obj.Produto != null && obj.Produto.OrigemMercadoria != null ? (int)obj.Produto.OrigemMercadoria : 0,
                    ValorFrete = "0,00",
                    ValorSeguro = "0,00",
                    ValorOutras = "0,00",
                    ValorDesconto = "0,00",
                    NumeroOrdemCompra = obj.NumeroOrdemCompra,
                    NumeroItemOrdemCompra = obj.NumeroItemOrdemCompra,
                    BCICMS = dynTributacao.BaseICMS.ToString("n2"),
                    ReducaoBCICMS = dynTributacao.ReducaoBaseICMS.ToString("n6"),
                    AliquotaICMS = dynTributacao.AliquotaICMS.ToString("n2"),
                    ValorICMS = dynTributacao.ValorICMS.ToString("n2"),
                    BCICMSDestino = dynTributacao.BaseICMSDestino.ToString("n2"),
                    AliquotaICMSDestino = dynTributacao.AliquotaICMSDestino.ToString("n2"),
                    AliquotaICMSInterno = dynTributacao.AliquotaICMSInterno.ToString("n2"),
                    PercentualPartilha = dynTributacao.PercentualPartilha.ToString("n2"),
                    ValorICMSDestino = dynTributacao.ValorICMSDestino.ToString("n2"),
                    ValorICMSRemetente = dynTributacao.ValorICMSRemetente.ToString("n2"),
                    AliquotaFCP = dynTributacao.PercentualFCP.ToString("n2"),
                    ValorFCP = dynTributacao.ValorFCP.ToString("n2"),
                    BCICMSST = dynTributacao.BaseICMSST.ToString("n2"),
                    ReducaoBCICMSST = dynTributacao.ReducaoBaseICMSST.ToString("n2"),
                    PercentualMVA = dynTributacao.PercentualMVA.ToString("n2"),
                    AliquotaICMSST = dynTributacao.AliquotaICMSST.ToString("n2"),
                    AliquotaInterestadual = dynTributacao.AliquotaInterestadual.ToString("n2"),
                    CodigoCSTPIS = dynTributacao.CSTPIS != 0 ? (int)dynTributacao.CSTPIS : 0,
                    BCPIS = dynTributacao.BasePIS.ToString("n2"),
                    ReducaoBCPIS = dynTributacao.ReducaoBasePIS.ToString("n2"),
                    AliquotaPIS = dynTributacao.AliquotaPIS.ToString("n2"),
                    ValorPIS = dynTributacao.ValorPIS.ToString("n2"),
                    CodigoCSTCOFINS = dynTributacao.CSTCOFINS != 0 ? (int)dynTributacao.CSTCOFINS : 0,
                    BCCOFINS = dynTributacao.BaseCOFINS.ToString("n2"),
                    ReducaoBCCOFINS = dynTributacao.ReducaoBaseCOFINS.ToString("n2"),
                    AliquotaCOFINS = dynTributacao.AliquotaCOFINS.ToString("n2"),
                    ValorCOFINS = dynTributacao.ValorCOFINS.ToString("n2"),
                    CodigoCSTIPI = dynTributacao.CSTIPI != 0 ? (int)dynTributacao.CSTIPI : 0,
                    BCIPI = dynTributacao.BaseIPI.ToString("n2"),
                    ReducaoBCIPI = dynTributacao.ReducaoBaseIPI.ToString("n2"),
                    AliquotaIPI = dynTributacao.AliquotaIPI.ToString("n2"),
                    BCISS = dynTributacao.BaseISS.ToString("n2"),
                    AliquotaISS = dynTributacao.AliquotaISS.ToString("n2"),
                    ValorISS = dynTributacao.ValorISS.ToString("n2"),
                    AliquotaRemICMSRet = "0,00",
                    ValorICMSMonoRet = "0,00",
                    BaseDeducaoISS = "0,00",
                    OutrasRetencoesISS = "0,00",
                    DescontoIncondicional = "0,00",
                    DescontoCondicional = "0,00",
                    RetencaoISS = "0,00",
                    CodigoExigibilidadeISS = 0,
                    GeraIncentivoFiscal = string.Empty,
                    ProcessoJudicial = string.Empty,
                    BCII = "0,00",
                    DespesaII = "0,00",
                    ValorII = "0,00",
                    ValorIOFII = "0,00",
                    NumeroDocumentoII = string.Empty,
                    DataRegistroII = string.Empty,
                    LocalDesembaracoII = string.Empty,
                    EstadoDesembaracoII = string.Empty,
                    DataDesembaracoII = string.Empty,
                    CNPJAdquirente = string.Empty,
                    CodigoViaTransporteII = 0,
                    ValorFreteMaritimoII = "0,00",
                    CodigoIntermediacaoII = 0,
                    ValorICMSDesonerado = "0,00",
                    CodigoMotivoDesoneracao = 0,
                    ValorICMSOperacao = "0,00",
                    AliquotaICMSOperacao = "0,00",
                    ValorICMSDeferido = "0,00",
                    Descricao = obj.Produto != null ? obj.Produto.Descricao : obj.Servico != null ? obj.Servico.Descricao : string.Empty,
                    CSTICMS = dynTributacao.DescricaoCSTICMS != null ? dynTributacao.DescricaoCSTICMS : string.Empty,
                    CFOP = dynTributacao.NumeroCFOP != 0 ? (int)dynTributacao.NumeroCFOP : 0,
                    Qtd = obj.Quantidade.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                    ValorUnitario = obj.ValorUnitario.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                    ValorTotal = obj.ValorTotal.ToString("n2"),
                    ValorST = dynTributacao.ValorICMSST.ToString("n2"),
                    ValorIPI = dynTributacao.ValorIPI.ToString("n2"),
                    obj.CodigoItem,
                    obj.DescricaoItem,
                    UnidadeMedida = obj.Produto != null && obj.Produto.UnidadeDeMedida != null ? obj.Produto.UnidadeDeMedida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico,
                    DescricaoUnidadeMedida = obj.Produto != null && obj.Produto.UnidadeDeMedida != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida) : "SERV",
                    BaseFCPICMS = "0,00",
                    PercentualFCPICMS = "0,00",
                    ValorFCPICMS = "0,00",
                    BaseFCPICMSST = "0,00",
                    PercentualFCPICMSST = "0,00",
                    ValorFCPICMSST = "0,00",
                    AliquotaFCPICMSST = "0,00",
                    BaseFCPDestino = dynTributacao.PercentualFCP.ToString("n2") != "0,00" ? dynTributacao.BaseICMS.ToString("n2") : "0,00",
                    PercentualIPIDevolvido = "0,00",
                    ValorIPIDevolvido = "0,00",
                    InformacoesAdicionaisItem = string.Empty,
                    IndicadorEscalaRelevante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum,
                    CNPJFabricante = string.Empty,
                    CodigoBeneficioFiscal = string.Empty,
                    CodigoANP = obj.Produto != null && !string.IsNullOrWhiteSpace(obj.Produto.CodigoANP) ? obj.Produto.CodigoANP : string.Empty,
                    PercentualGLP = "0,0000",
                    PercentualGNN = "0,0000",
                    PercentualGNI = "0,0000",
                    PercentualOrigemComb = obj.Produto != null && obj.Produto.PercentualOrigemCombustivel > 0 ? obj.Produto.PercentualOrigemCombustivel.ToString("n4") : "0,0000",
                    PercentualMisturaBiodiesel = obj.Produto != null && obj.Produto.PercentualMisturaBiodiesel > 0 ? obj.Produto.PercentualMisturaBiodiesel.ToString("n4") : "0,0000",
                    ValorPartidaANP = "0,00",
                    QuantidadeTributavel = 0.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                    ValorUnitarioTributavel = 0.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                    UnidadeDeMedidaTributavel = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Nenhum,
                    CodigoEANTributavel = string.Empty,
                    BCICMSSTRetido = dynTributacao.BCICMSSTRetido.ToString("n2"),
                    AliquotaICMSSTRetido = dynTributacao.AliquotaICMSSTRetido.ToString("n2"),
                    ValorICMSSTSubstituto = "0,00",
                    ValorICMSSTRetido = dynTributacao.ValorICMSSTRetido.ToString("n2"),
                    BCICMSEfetivo = dynTributacao.BCICMSEfetivo.ToString("n2"),
                    AliquotaICMSEfetivo = dynTributacao.AliquotaICMSEfetivo.ToString("n2"),
                    ReducaoBCICMSEfetivo = "0,00",
                    ValorICMSEfetivo = dynTributacao.ValorICMSEfetivo.ToString("n2"),
                    NumeroDrawback = string.Empty,
                    NumeroRegistroExportacao = string.Empty,
                    ChaveAcessoExportacao = string.Empty,
                    CodigoNFCI = string.Empty,
                    CodigoLocalArmazenamento = 0,
                    LocalArmazenamento = string.Empty,
                    BCICMSSTDestino = 0.ToString("n2"),
                    ValorICMSSTDestino = 0.ToString("n2"),
                    Sequencial = 0
                });
            }

            return retorno;
        }

        private object ObterItensDocumentoEntrada(List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentosEntrada, List<int> codigosDocumentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repItemDocumentoEntrada.BuscarPorDocumentosEntrada(codigosDocumentoEntrada);

            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

            List<object> retorno = new List<object>();
            for (int i = 0; i < itens.Count; i++)
            {
                var obj = itens[i];
                dynamic dynTributacao = serNotaFiscalEletronica.BuscarTributacaoItem(unitOfWork, obj.Produto.Codigo, 0, 0, documentosEntrada[0].Fornecedor.Atividade.Codigo, documentosEntrada[0].Fornecedor.CPF_CNPJ, obj.ValorTotal, 0, 0, 0, 0, 0, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, obj.Quantidade, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario?.Empresa?.Codigo ?? 0 : 0);
                dynTributacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dynTributacao.ToString().Replace("=", ":").Replace(".", "").Replace(",", ".").Replace(". ", ", "));

                retorno.Add(new
                {
                    obj.Codigo,
                    CodigoProduto = obj.Produto.Codigo,
                    CodigoServico = 0,
                    CodigoCFOP = dynTributacao.CodigoCFOP != 0 ? (int)dynTributacao.CodigoCFOP : 0,
                    CodigoCSTICMS = dynTributacao.CSTICMS != 0 ? (int)dynTributacao.CSTICMS : 0,
                    OrigemMercadoria = obj.Produto != null && obj.Produto.OrigemMercadoria != null ? (int)obj.Produto.OrigemMercadoria : 0,
                    ValorFrete = "0,00",
                    ValorSeguro = "0,00",
                    ValorOutras = "0,00",
                    ValorDesconto = "0,00",
                    NumeroOrdemCompra = string.Empty,
                    NumeroItemOrdemCompra = string.Empty,
                    BCICMS = dynTributacao.BaseICMS.ToString("n2"),
                    ReducaoBCICMS = dynTributacao.ReducaoBaseICMS.ToString("n6"),
                    AliquotaICMS = dynTributacao.AliquotaICMS.ToString("n2"),
                    ValorICMS = dynTributacao.ValorICMS.ToString("n2"),
                    BCICMSDestino = dynTributacao.BaseICMSDestino.ToString("n2"),
                    AliquotaICMSDestino = dynTributacao.AliquotaICMSDestino.ToString("n2"),
                    AliquotaICMSInterno = dynTributacao.AliquotaICMSInterno.ToString("n2"),
                    PercentualPartilha = dynTributacao.PercentualPartilha.ToString("n2"),
                    ValorICMSDestino = dynTributacao.ValorICMSDestino.ToString("n2"),
                    ValorICMSRemetente = dynTributacao.ValorICMSRemetente.ToString("n2"),
                    AliquotaFCP = dynTributacao.PercentualFCP.ToString("n2"),
                    ValorFCP = dynTributacao.ValorFCP.ToString("n2"),
                    BCICMSST = dynTributacao.BaseICMSST.ToString("n2"),
                    ReducaoBCICMSST = dynTributacao.ReducaoBaseICMSST.ToString("n2"),
                    PercentualMVA = dynTributacao.PercentualMVA.ToString("n2"),
                    AliquotaICMSST = dynTributacao.AliquotaICMSST.ToString("n2"),
                    AliquotaInterestadual = dynTributacao.AliquotaInterestadual.ToString("n2"),
                    CodigoCSTPIS = dynTributacao.CSTPIS != 0 ? (int)dynTributacao.CSTPIS : 0,
                    BCPIS = dynTributacao.BasePIS.ToString("n2"),
                    ReducaoBCPIS = dynTributacao.ReducaoBasePIS.ToString("n2"),
                    AliquotaPIS = dynTributacao.AliquotaPIS.ToString("n2"),
                    ValorPIS = dynTributacao.ValorPIS.ToString("n2"),
                    CodigoCSTCOFINS = dynTributacao.CSTCOFINS != 0 ? (int)dynTributacao.CSTCOFINS : 0,
                    BCCOFINS = dynTributacao.BaseCOFINS.ToString("n2"),
                    ReducaoBCCOFINS = dynTributacao.ReducaoBaseCOFINS.ToString("n2"),
                    AliquotaCOFINS = dynTributacao.AliquotaCOFINS.ToString("n2"),
                    ValorCOFINS = dynTributacao.ValorCOFINS.ToString("n2"),
                    CodigoCSTIPI = dynTributacao.CSTIPI != 0 ? (int)dynTributacao.CSTIPI : 0,
                    BCIPI = dynTributacao.BaseIPI.ToString("n2"),
                    ReducaoBCIPI = dynTributacao.ReducaoBaseIPI.ToString("n2"),
                    AliquotaIPI = dynTributacao.AliquotaIPI.ToString("n2"),
                    BCISS = dynTributacao.BaseISS.ToString("n2"),
                    AliquotaISS = dynTributacao.AliquotaISS.ToString("n2"),
                    ValorISS = dynTributacao.ValorISS.ToString("n2"),
                    AliquotaRemICMSRet = "0,00",
                    ValorICMSMonoRet = "0,00",
                    BaseDeducaoISS = "0,00",
                    OutrasRetencoesISS = "0,00",
                    DescontoIncondicional = "0,00",
                    DescontoCondicional = "0,00",
                    RetencaoISS = "0,00",
                    CodigoExigibilidadeISS = 0,
                    GeraIncentivoFiscal = string.Empty,
                    ProcessoJudicial = string.Empty,
                    BCII = "0,00",
                    DespesaII = "0,00",
                    ValorII = "0,00",
                    ValorIOFII = "0,00",
                    NumeroDocumentoII = string.Empty,
                    DataRegistroII = string.Empty,
                    LocalDesembaracoII = string.Empty,
                    EstadoDesembaracoII = string.Empty,
                    DataDesembaracoII = string.Empty,
                    CNPJAdquirente = string.Empty,
                    CodigoViaTransporteII = 0,
                    ValorFreteMaritimoII = "0,00",
                    CodigoIntermediacaoII = 0,
                    ValorICMSDesonerado = "0,00",
                    CodigoMotivoDesoneracao = 0,
                    ValorICMSOperacao = "0,00",
                    AliquotaICMSOperacao = "0,00",
                    ValorICMSDeferido = "0,00",
                    Descricao = obj.Produto.Descricao,
                    CSTICMS = dynTributacao.DescricaoCSTICMS != null ? dynTributacao.DescricaoCSTICMS : string.Empty,
                    CFOP = dynTributacao.NumeroCFOP != 0 ? (int)dynTributacao.NumeroCFOP : 0,
                    Qtd = obj.Quantidade.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                    ValorUnitario = obj.ValorUnitario.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                    ValorTotal = obj.ValorTotal.ToString("n2"),
                    ValorST = dynTributacao.ValorICMSST.ToString("n2"),
                    ValorIPI = dynTributacao.ValorIPI.ToString("n2"),
                    CodigoItem = !string.IsNullOrWhiteSpace(obj.Produto.CodigoProduto) ? obj.Produto.CodigoProduto : obj.Produto.Codigo.ToString(),
                    DescricaoItem = obj.Produto.DescricaoNotaFiscal,
                    UnidadeMedida = obj.Produto.UnidadeDeMedida != null ? obj.Produto.UnidadeDeMedida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico,
                    DescricaoUnidadeMedida = obj.Produto.UnidadeDeMedida != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida) : "SERV",
                    BaseFCPICMS = "0,00",
                    PercentualFCPICMS = "0,00",
                    ValorFCPICMS = "0,00",
                    BaseFCPICMSST = "0,00",
                    PercentualFCPICMSST = "0,00",
                    ValorFCPICMSST = "0,00",
                    AliquotaFCPICMSST = "0,00",
                    BaseFCPDestino = dynTributacao.PercentualFCP.ToString("n2") != "0,00" ? dynTributacao.BaseICMS.ToString("n2") : "0,00",
                    PercentualIPIDevolvido = "0,00",
                    ValorIPIDevolvido = "0,00",
                    InformacoesAdicionaisItem = string.Empty,
                    IndicadorEscalaRelevante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum,
                    CNPJFabricante = string.Empty,
                    CodigoBeneficioFiscal = string.Empty,
                    CodigoANP = !string.IsNullOrWhiteSpace(obj.Produto.CodigoANP) ? obj.Produto.CodigoANP : string.Empty,
                    PercentualGLP = "0,0000",
                    PercentualGNN = "0,0000",
                    PercentualGNI = "0,0000",
                    PercentualOrigemComb = obj.Produto != null && obj.Produto.PercentualOrigemCombustivel > 0 ? obj.Produto.PercentualOrigemCombustivel.ToString("n4") : "0,0000",
                    PercentualMisturaBiodiesel = obj.Produto != null && obj.Produto.PercentualMisturaBiodiesel > 0 ? obj.Produto.PercentualMisturaBiodiesel.ToString("n4") : "0,0000",
                    ValorPartidaANP = "0,00",
                    QuantidadeTributavel = 0.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                    ValorUnitarioTributavel = 0.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                    UnidadeDeMedidaTributavel = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Nenhum,
                    CodigoEANTributavel = string.Empty,
                    BCICMSSTRetido = dynTributacao.BCICMSSTRetido.ToString("n2"),
                    AliquotaICMSSTRetido = dynTributacao.AliquotaICMSSTRetido.ToString("n2"),
                    ValorICMSSTSubstituto = "0,00",
                    ValorICMSSTRetido = dynTributacao.ValorICMSSTRetido.ToString("n2"),
                    BCICMSEfetivo = dynTributacao.BCICMSEfetivo.ToString("n2"),
                    AliquotaICMSEfetivo = dynTributacao.AliquotaICMSEfetivo.ToString("n2"),
                    ReducaoBCICMSEfetivo = "0,00",
                    ValorICMSEfetivo = dynTributacao.ValorICMSEfetivo.ToString("n2"),
                    NumeroDrawback = string.Empty,
                    NumeroRegistroExportacao = string.Empty,
                    ChaveAcessoExportacao = string.Empty,
                    CodigoNFCI = string.Empty,
                    CodigoLocalArmazenamento = 0,
                    LocalArmazenamento = string.Empty,
                    BCICMSSTDestino = 0.ToString("n2"),
                    ValorICMSSTDestino = 0.ToString("n2"),
                    Sequencial = 0
                });
            }

            return retorno;
        }

        private object ObterTotalizadorNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                BaseICMS = nfe.BCICMS,
                ValorICMS = nfe.ValorICMS,
                ValorICMSDesonerado = nfe.ICMSDesonerado,
                ValorII = nfe.ValorII,
                BaseICMSST = nfe.BCICMSST,
                ValorICMSST = nfe.ValorICMSST,
                ValorTotalProdutos = nfe.ValorProdutos,
                ValorFrete = nfe.ValorFrete,
                ValorSeguro = nfe.ValorSeguro,
                nfe.ValorDesconto,
                nfe.ValorOutrasDespesas,
                nfe.ValorIPI,
                ValorTotalNFe = nfe.ValorTotalNota,
                ValorTotalServicos = nfe.ValorServicos,
                BaseISS = nfe.BCISSQN,
                ValorISS = nfe.ValorISSQN,
                BaseDeducao = nfe.BCDeducao,
                ValorOutrasRetencoes = nfe.ValorOutrasRetencoes,
                ValorDescontoIncondicional = nfe.ValorDescontoIncondicional,
                nfe.ValorDescontoCondicional,
                nfe.ValorRetencaoISS,
                BasePIS = nfe.BCPIS,
                ValorPIS = nfe.ValorPIS,
                BaseCOFINS = nfe.BCCOFINS,
                nfe.ValorCOFINS,
                nfe.ValorFCP,
                nfe.ValorICMSDestino,
                nfe.ValorICMSRemetente,
                nfe.ValorFCPICMS,
                nfe.ValorFCPICMSST,
                nfe.ValorIPIDevolvido,
                nfe.BCICMSSTRetido,
                nfe.ValorICMSSTRetido
            };

            return retorno;
        }

        private object ObterTotalizadorPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda)
        {
            var retorno = new
            {
                ValorTotalProdutos = pedidosVenda.Sum(obj => obj.ValorProdutos),
                ValorTotalNFe = pedidosVenda.Sum(obj => obj.ValorTotal),
                ValorTotalServicos = pedidosVenda.Sum(obj => obj.ValorServicos)
            };

            return retorno;
        }

        private object ObterObservacaoNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                ObservacaoTributaria = nfe.ObservacaoTributaria,
                ObservacaoNFe = nfe.ObservacaoNota
            };

            return retorno;
        }

        private object ObterObservacaoPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda)
        {
            string ObservacaoNFe = "";
            foreach (var pedidoVenda in pedidosVenda)
            {
                var placa = pedidoVenda.Veiculo != null ? pedidoVenda.Veiculo.Placa : string.Empty;
                if (placa != string.Empty)
                    placa = " - Placa do Veículo: " + placa;

                string km = pedidoVenda.KM > 0 ? " - KM: " + pedidoVenda.KM.ToString() : "";

                var numeroReferencia = !string.IsNullOrWhiteSpace(pedidoVenda.Referencia) ? " N Ref.: " + pedidoVenda.Referencia : "";

                ObservacaoNFe += " " + pedidoVenda.Observacao + " - Pedido/OS N: " + pedidoVenda.Numero + placa + km + numeroReferencia;
            }
            var retorno = new
            {
                ObservacaoNFe = ObservacaoNFe
            };
            return retorno;
        }

        private object ObterExportacaoCompraNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                nfe.UFEmbarque,
                nfe.LocalEmbarque,
                nfe.LocalDespacho,
                nfe.InformacaoCompraNotaEmpenho,
                nfe.InformacaoCompraPedido,
                nfe.InformacaoCompraContrato
            };

            return retorno;
        }

        private object ObterRetiradaEntregaNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {

                UtilizarEnderecoRetirada = nfe.UtilizarEnderecoRetirada,
                ClienteRetirada = nfe.ClienteRetirada != null ? new
                {
                    Codigo = nfe.ClienteRetirada.Codigo,
                    Descricao = nfe.ClienteRetirada.Nome + " (" + nfe.ClienteRetirada.Localidade.DescricaoCidadeEstado + ")"
                } : null,
                LocalidadeRetirada = nfe.LocalidadeRetirada != null ? new
                {
                    Codigo = nfe.LocalidadeRetirada.Codigo,
                    Descricao = nfe.LocalidadeRetirada.DescricaoCidadeEstado
                } : null,
                RetiradaLogradouro = nfe.RetiradaLogradouro,
                RetiradaNumeroLogradouro = nfe.RetiradaNumeroLogradouro,
                RetiradaComplementoLogradouro = nfe.RetiradaComplementoLogradouro,
                RetiradaBairro = nfe.RetiradaBairro,
                RetiradaCEP = nfe.RetiradaCEP,
                RetiradaTelefone = nfe.RetiradaTelefone,
                RetiradaEmail = nfe.RetiradaEmail,
                RetiradaIE = nfe.RetiradaIE,

                UtilizarEnderecoEntrega = nfe.UtilizarEnderecoEntrega,
                ClienteEntrega = nfe.ClienteEntrega != null ? new
                {
                    Codigo = nfe.ClienteEntrega.Codigo,
                    Descricao = nfe.ClienteEntrega.Nome + " (" + nfe.ClienteEntrega.Localidade.DescricaoCidadeEstado + ")"
                } : null,
                LocalidadeEntrega = nfe.LocalidadeEntrega != null ? new
                {
                    Codigo = nfe.LocalidadeEntrega.Codigo,
                    Descricao = nfe.LocalidadeEntrega.DescricaoCidadeEstado
                } : null,
                EntregaLogradouro = nfe.EntregaLogradouro,
                EntregaNumeroLogradouro = nfe.EntregaNumeroLogradouro,
                EntregaComplementoLogradouro = nfe.EntregaComplementoLogradouro,
                EntregaBairro = nfe.EntregaBairro,
                EntregaCEP = nfe.EntregaCEP,
                EntregaTelefone = nfe.EntregaTelefone,
                EntregaEmail = nfe.EntregaEmail,
                EntregaIE = nfe.EntregaIE
            };

            return retorno;
        }


        private object ObterTransporteNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                Transportadora = nfe.Transportadora != null ? new
                {
                    Codigo = nfe.Transportadora.Codigo,
                    Descricao = nfe.Transportadora.Nome + " (" + nfe.Transportadora.Localidade.DescricaoCidadeEstado + ")"
                } : null,
                CNPJTransportadora = nfe.TranspCNPJCPF,
                NomeTransportadora = nfe.TranspNome,
                IETransportadora = nfe.TranspIE,
                EstadoTransportadora = nfe.TranspUF,
                CidadeTransportadora = nfe.LocalidadeTranspMunicipio != null ? new
                {
                    Codigo = nfe.LocalidadeTranspMunicipio.Codigo,
                    Descricao = nfe.LocalidadeTranspMunicipio.DescricaoCidadeEstado
                } : null,
                EmailTransportadora = nfe.TranspEmail,
                Veiculo = nfe.Veiculo != null ? new
                {
                    Codigo = nfe.Veiculo.Codigo,
                    Descricao = nfe.Veiculo.Placa
                } : null,
                PlacaVeiculo = nfe.TranspPlacaVeiculo,
                EstadoVeiculo = nfe.TranspUFVeiculo,
                ANTTVeiculo = nfe.TranspANTTVeiculo,
                Quantidade = nfe.TranspQuantidade,
                Especie = nfe.TranspEspecie,
                Marca = nfe.TranspMarca,
                Volume = nfe.TranspVolume,
                PesoBruto = nfe.TranspPesoBruto,
                PesoLiquido = nfe.TranspPesoLiquido,
                TipoFrete = nfe.TipoFrete,
                EnderecoTransportadora = nfe.TranspEndereco
            };

            return retorno;
        }

        private object ObterReferenciaNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = new
            {
                TipoDocumento = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 ? nfe.ReferenciaNFe[0].TipoDocumento : 0,
                Modelo = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 ? nfe.ReferenciaNFe[0].Modelo : string.Empty,
                Chave = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].Chave != null ? nfe.ReferenciaNFe[0].Chave : string.Empty,
                Estado = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].UF != null ? nfe.ReferenciaNFe[0].UF : string.Empty,
                DataEmissao = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].DataEmissao != null ? nfe.ReferenciaNFe[0].DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                CNPJCPFEmitente = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 ? !string.IsNullOrWhiteSpace(nfe.ReferenciaNFe[0].CNPJEmitente) ? nfe.ReferenciaNFe[0].CNPJEmitente : !string.IsNullOrWhiteSpace(nfe.ReferenciaNFe[0].CPFEmitente) ? nfe.ReferenciaNFe[0].CPFEmitente : string.Empty : string.Empty,
                IEEmitente = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].IEEmitente != null ? nfe.ReferenciaNFe[0].IEEmitente : string.Empty,
                Serie = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].Serie != null ? nfe.ReferenciaNFe[0].Serie : string.Empty,
                Numero = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].Numero != null ? nfe.ReferenciaNFe[0].Numero : string.Empty,
                NumeroECF = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].NumeroECF != null ? nfe.ReferenciaNFe[0].NumeroECF : string.Empty,
                NumeroCOO = nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count() > 0 && nfe.ReferenciaNFe[0].COO != null ? nfe.ReferenciaNFe[0].COO : string.Empty
            };

            return retorno;
        }

        private object ObterParcelamentoNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var retorno = (from obj in nfe.ParcelasNFe
                           select new
                           {
                               obj.Codigo,
                               obj.Sequencia,
                               DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                               CodigoStatus = (int)obj.Situacao,
                               obj.Acrescimo,
                               Parcela = obj.Sequencia,
                               Valor = obj.Valor.ToString("n2"),
                               Desconto = obj.Desconto.ToString("n2"),
                               DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                               obj.DescricaoSituacao,
                               FormaTitulo = obj.Forma
                           }).ToList();

            return retorno;
        }

        private object ObterParcelasPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda, List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela repPedidoVendaParcela = new Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela> parcelas = repPedidoVendaParcela.BuscarPorPedidos(codigosPedido);

            List<object> retorno = new List<object>();
            for (int i = 0; i < parcelas.Count; i++)
            {
                var obj = parcelas[i];
                retorno.Add(new
                {
                    obj.Codigo,
                    obj.Sequencia,
                    DataEmissao = obj.PedidoVenda.DataEmissao.Value.ToString("dd/MM/yyyy"),
                    CodigoStatus = 1,
                    Acrescimo = "0,00",
                    Parcela = obj.Sequencia,
                    Valor = obj.Valor.ToString("n2"),
                    Desconto = obj.Desconto.ToString("n2"),
                    DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                    DescricaoSituacao = "Em Aberto",
                    FormaTitulo = obj.Forma
                });
            }

            return retorno;
        }

        private bool CadastrarTransportadora(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, bool cnpj, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                double cpfcnpj = double.Parse(nfe.TranspCNPJCPF);

                Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente clienteExiste = repCliente.BuscarPorCPFCNPJ(cpfcnpj);

                if (clienteExiste == null)
                {
                    cliente.CPF_CNPJ = cpfcnpj;
                    if (cnpj)
                        cliente.Tipo = "J";
                    else
                        cliente.Tipo = "F";
                    cliente.Localidade = new Dominio.Entidades.Localidade() { Codigo = nfe.LocalidadeTranspMunicipio.Codigo };
                    cliente.Atividade = new Dominio.Entidades.Atividade() { Codigo = 3 };
                    cliente.Nome = nfe.TranspNome;
                    cliente.DataCadastro = DateTime.Now;
                    cliente.NomeFantasia = nfe.TranspNome;
                    cliente.IE_RG = nfe.TranspIE.ToUpper();
                    cliente.Endereco = nfe.TranspEndereco.ToUpper();
                    cliente.Email = nfe.TranspEmail;
                    cliente.Ativo = true;
                    repCliente.Inserir(cliente);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool CadastrarVeículo(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                string placa = nfe.TranspPlacaVeiculo;

                Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo();
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculoExiste = repVeiculo.BuscarPorPlaca(placa);

                if (veiculoExiste == null)
                {
                    veiculo.Placa = placa.ToUpper();
                    veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = nfe.TranspUFVeiculo };
                    int antt = 0;
                    int.TryParse(nfe.TranspANTTVeiculo, out antt);
                    veiculo.RNTRC = antt;
                    veiculo.Ativo = true;
                    repVeiculo.Inserir(veiculo);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool SalvarVinculoNotaPedidoVenda(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<int> codigosPedidoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                for (int i = 0; i < codigosPedidoVenda.Count; i++)
                {
                    int codigoPedidoVenda = codigosPedidoVenda[i];

                    Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido repNFePedido = new Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido(unitOfWork);
                    Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigoPedidoVenda);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido notaPedido = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido();

                    notaPedido.NotaFiscal = nfe;
                    notaPedido.PedidoVenda = pedidoVenda;
                    repNFePedido.Inserir(notaPedido);
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool SalvarVinculoNotaDocumentoEntrada(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<int> codigosDocumentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();

                for (int i = 0; i < codigosDocumentoEntrada.Count; i++)
                {
                    int codigoDocumentoEntrada = codigosDocumentoEntrada[i];

                    Repositorio.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada repNFeDocumentoEntrada = new Repositorio.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada(unitOfWork);
                    Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada notaDocumento = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada();

                    notaDocumento.NotaFiscal = nfe;
                    notaDocumento.DocumentoEntrada = documentoEntrada;
                    repNFeDocumentoEntrada.Inserir(notaDocumento);
                }

                unitOfWork.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return false;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<int> RetornaCodigosPedidos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Codigos")))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Codigos"));
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        private List<int> RetornaCodigoPedidoVenda(Repositorio.UnitOfWork unidadeDeTrabalho, string codigoPedidoVenda)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(codigoPedidoVenda))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(codigoPedidoVenda);
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        private object ObterLotesItensNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes repNotaFiscalProdutosLotes = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes> lotes = repNotaFiscalProdutosLotes.BuscarPorNota(nfe.Codigo);

            var retorno = (from obj in lotes
                           select new
                           {
                               obj.Codigo,
                               CodigoItem = obj.NotaFiscalProdutos.Codigo,
                               obj.NumeroLote,
                               QuantidadeLote = obj.QuantidadeLote.ToString("n3"),
                               DataFabricacao = obj.DataFabricacao.ToString("dd/MM/yyyy"),
                               DataValidade = obj.DataValidade.ToString("dd/MM/yyyy"),
                               obj.CodigoAgregacao
                           }).ToList();
            return retorno;
        }

        private string ValidarCampos(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, out bool cnpj)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia repNotaFiscalReferencia = new Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia(unitOfWork);
            cnpj = false;

            if (!string.IsNullOrWhiteSpace(nfe.Empresa.Tipo) && nfe.Empresa.Tipo.Equals("F") && (nfe.EmpresaSerie.Numero < 920 || nfe.EmpresaSerie.Numero > 969))
                return "Só é permitido utilizar a série entre 920 e 969 para emitentes Pessoa Física!";

            if (nfe.TranspCNPJCPF != null && !string.IsNullOrWhiteSpace(nfe.TranspCNPJCPF) && nfe.TranspCNPJCPF.Length == 14)
            {
                if (!Utilidades.Validate.ValidarCNPJ(nfe.TranspCNPJCPF))
                {
                    return "CNPJ da Transportadora é inválido!";
                }
                cnpj = true;
            }
            else if (nfe.TranspCNPJCPF != null && !string.IsNullOrWhiteSpace(nfe.TranspCNPJCPF) && nfe.TranspCNPJCPF.Length == 11)
            {
                if (!Utilidades.Validate.ValidarCPF(nfe.TranspCNPJCPF))
                {
                    return "CPF da Transportadora é inválido!";
                }
            }
            else if (nfe.TranspCNPJCPF != null && !string.IsNullOrWhiteSpace(nfe.TranspCNPJCPF) && nfe.TranspCNPJCPF.Length > 0)
            {
                return "CNPJ/CPF da Transportadora é inválido!";
            }

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia notaFiscalReferencia = repNotaFiscalReferencia.BuscarPorNota(nfe.Codigo);
            if (notaFiscalReferencia != null)
            {
                if (notaFiscalReferencia.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFModelo1 || notaFiscalReferencia.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFProdutorRural)
                {
                    if (!string.IsNullOrWhiteSpace(notaFiscalReferencia.CNPJEmitente) && notaFiscalReferencia.CNPJEmitente.Length == 14)
                    {
                        if (!Utilidades.Validate.ValidarCNPJ(notaFiscalReferencia.CNPJEmitente))
                        {
                            return "CNPJ da Nota de Referência é inválido!";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(notaFiscalReferencia.CPFEmitente) && notaFiscalReferencia.CPFEmitente.Length == 11)
                    {
                        if (!Utilidades.Validate.ValidarCPF(notaFiscalReferencia.CPFEmitente))
                        {
                            return "CPF da Nota de Referência é inválido!";
                        }
                    }
                }
                else if (notaFiscalReferencia.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NF)
                {
                    if (!string.IsNullOrWhiteSpace(notaFiscalReferencia.Chave))
                    {
                        if (!Utilidades.Validate.ValidarChave(notaFiscalReferencia.Chave))
                        {
                            return "Chave da Nota de Referência é inválida!";
                        }
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }
}

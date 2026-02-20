using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "RegrasAprovacao", "ConsultarCTesOcorrencia", "ExportarPesquisa", "RegrasAprovacao", "ConsultarCTesDaCarga", "ConsultarClientesDoPedidosDaCarga", "HistoricoAutorizacao" }, "Ocorrencias/AutorizacaoOcorrencia")]
    public class AutorizacaoOcorrenciaController : BaseController
    {
        #region Construtores

        public AutorizacaoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciasObjetoValor = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();



                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaOcorrencias, ref listaOcorrenciasObjetoValor, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                dynamic lista = RetornaDyn(listaOcorrencias, listaOcorrenciasObjetoValor, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarOcorrenciaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigoComFetch(codigo);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovacoesDesbloqueadas = repCargaOcorrenciaAutorizacao.BuscarDesbloqueadasNaoDelegadasPorCargaOcorrencia(ocorrencia.Codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> cargaOcorrenciasAtualizacao = repCargaOcorrenciaAutorizacao.ResponsavelOcorrencia(ocorrencia.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(ocorrencia.Codigo);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                return new JsonpResult(ObterOcorrencia(ocorrencia, aprovacoesDesbloqueadas, cargaOcorrenciasAtualizacao, chamados, configuracao, configuracaoOcorrencia, ocorrencia, unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciasObjetoValor = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaOcorrencias, ref listaOcorrenciasObjetoValor, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                dynamic lista = RetornaDyn(listaOcorrencias, listaOcorrenciasObjetoValor, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.Prop("Codigo");
                grid.Prop("CPF_CNPJ").Nome("CNPJ").Tamanho(30).Align(Models.Grid.Align.center);
                grid.Prop("Nome").Nome("Nome").Tamanho(40).Align(Models.Grid.Align.left);
                grid.Prop("Localidade").Nome("Cidade").Tamanho(20).Align(Models.Grid.Align.left);

                string objClientes = Request.Params("Clientes");

                List<double> cnpjsClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(objClientes);
                List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarPorVariosCPFCNPJ(cnpjsClientes);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCliente> lista = (from o in clientes
                                                                                      select new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCliente
                                                                                      {
                                                                                          CPF_CNPJ = o.CPF_CNPJ_Formatado,
                                                                                          Nome = o.Nome,
                                                                                          Localidade = o.Localidade.DescricaoCidadeEstado
                                                                                      }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "Clientes pedido.csv");
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarClientesDoPedidosDaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codOcorrencia);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedidos.BuscarPorOcorrencia(codOcorrencia);

                List<Dominio.Entidades.Cliente> clientes = pedidos.SelectMany(x => x.Clientes.Select(y => y)).ToList();


                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CNPJ", "CPF_CNPJ", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome", "Nome", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Localidade", "Localidade", 7, Models.Grid.Align.center, true);


                var lista = (from obj in clientes
                             where obj != null
                             select new
                             {
                                 obj.Codigo,
                                 obj.Nome,
                                 CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                                 Localidade = obj.Localidade.DescricaoCidadeEstado

                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCTesDaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codOcorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Numero, "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Serie, "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DocumentoFiscal, "AbreviacaoModeloDocumentoFiscal", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Notas, "NumeroNotas", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Tomador, "Tomador", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorReceber, "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Status, "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RetornoSefaz, "RetornoSefaz", 15, Models.Grid.Align.left, false, false);


                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Tomador")
                    propOrdenacao = "TomadorPagador.Nome";
                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();


                // CT-es selecionados
                string[] statusCTe = new string[] { "A", "Z" };
                List<int> ctesSelecionados = repCargaOcorrenciaDocumento.ObterCodigoCtesPorOcorrencia(ocorrencia.Codigo);
                int quantidadeTotal = repCargaOcorrenciaDocumento.ContarCTesPorOcorrencia(ocorrencia.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaOcorrenciaDocumento.BuscarCTesPorOcorrencia(ocorrencia.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                if ((configuracaoOcorrencia?.ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia ?? false) && ocorrencia.Carga != null)
                {
                    cargaCTes = repCargaCTe.BuscarPorCarga(ocorrencia.Carga.Codigo);
                    quantidadeTotal = cargaCTes.Count;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTe((from obj in cargaCTes select obj.Codigo).ToList());

                var lista = (
                    from obj in cargaCTes
                    select new
                    {
                        obj.Codigo,
                        CodigoCTE = obj.CTe?.Codigo ?? 0,
                        DescricaoTipoServico = obj.CTe?.DescricaoTipoServico ?? "",
                        NumeroModeloDocumentoFiscal = obj.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                        AbreviacaoModeloDocumentoFiscal = obj.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                        CodigoEmpresa = obj.CTe?.Empresa.Codigo ?? obj.PreCTe.Empresa.Codigo,
                        Numero = obj.CTe?.Numero.ToString() ?? "",
                        SituacaoCTe = obj.CTe?.Status ?? "A",
                        Serie = obj.CTe?.Serie.Numero.ToString() ?? "",
                        ValorFrete = obj.CTe?.ValorAReceber.ToString("n2") ?? obj.PreCTe?.ValorAReceber.ToString("n2"),
                        NumeroNotas = string.Join(", ", (from nf in cargaPedidosXMLsNotaFiscalCTe where nf.CargaCTe.Codigo == obj.Codigo select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()),
                        Status = obj.CTe?.DescricaoStatus ?? "",
                        RetornoSefaz = obj.CTe != null ? (!string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "") : "",
                        Destinatario = obj.CTe != null ? (obj.CTe.Destinatario?.Descricao ?? "") : (obj.PreCTe.Destinatario?.Descricao ?? ""),
                        Tomador = obj.CTe != null ? (obj.CTe.TomadorPagador?.Cliente?.Descricao ?? "") : (obj.PreCTe.Tomador?.Cliente.Descricao ?? ""),
                        DT_RowColor = obj.CTe != null ? (ctesSelecionados.Contains(obj.CTe.Codigo) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info : "") : ""
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(quantidadeTotal);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCTesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codOcorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Numero, "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Serie, "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DocumentoFiscal, "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Tomador, "Tomador", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorReceber, "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Status, "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RetornoSefaz, "RetornoSefaz", 15, Models.Grid.Align.left, false, false);


                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Tomador")
                    propOrdenacao = "TomadorPagador.Nome";
                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                {
                    Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                    serOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, WebServiceConsultaCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, Auditado, WebServiceOracle, clienteMultisoftware: Cliente);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrencia.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from obj in cargaCTeComplementoInfo
                             where obj.CTe != null
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.CTe.Codigo,
                                 obj.CTe.DescricaoTipoServico,
                                 NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                 AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 obj.CTe.Numero,
                                 SituacaoCTe = obj.CTe.Status,
                                 Serie = obj.CTe.Serie.Numero,
                                 ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                 Status = obj.CTe.DescricaoStatus,
                                 RetornoSefaz = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "",
                                 Tomador = obj.CTe.TomadorPagador != null ? string.Concat(!string.IsNullOrWhiteSpace(obj.CTe.Tomador.Cliente?.CodigoIntegracao) ? obj.CTe.Tomador.Cliente?.CodigoIntegracao + " - " : string.Empty, obj.CTe.TomadorPagador.Nome + "(" + obj.CTe.TomadorPagador.CPF_CNPJ_Formatado + ")") : string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(repCargaCTeComplementoInfo.ContarPorCTEsOcorrencia(ocorrencia.Codigo));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista repOcorrenciaContratoMotorista = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigoComFetch(codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamento = repOcorrenciaCancelamento.BuscarPorOcorrencia(codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista manobristas = repOcorrenciaContratoMotorista.BuscarPorOcorrencia(codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                // Busca os parametros da ocorrencia
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroPeriodo = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Periodo);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroBooleano = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Booleano);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroTexto = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Texto);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroData = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Data);

                // Busca hora parametro quando a ocorrencia possui parametros por periodo
                decimal horasPeriodo = 0;
                string periodoHoras = "00:00";
                if (ocorrenciaParametroPeriodo != null)
                {
                    if (ocorrenciaParametroPeriodo.TotalHoras == 0)
                    {
                        TimeSpan diferenca = ocorrenciaParametroPeriodo.DataFim.Value - ocorrenciaParametroPeriodo.DataInicio.Value;
                        horasPeriodo = Convert.ToDecimal(diferenca.TotalHours);
                    }
                    else
                        horasPeriodo = ocorrenciaParametroPeriodo.TotalHoras;

                    periodoHoras = horasPeriodo.FromHoursToFormattedTime();
                }

                // Cliente pedido
                string cliente = RetornarClientePedido(ocorrencia, unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                DateTime? dataPrevisaoEntrega = ocorrencia.Carga != null ? repCargaPedido.BuscarPrimeiraPrevisaoEntrega(ocorrencia.Carga.Codigo) : null;
                List<dynamic> chamadosDyn = new List<dynamic>();

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrenciaComFetch(codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                    chamadosDyn.Add(serChamado.ObterResumoChamado(chamado, unitOfWork, configuracao, TipoServicoMultisoftware));

                DateTime? dataChegadaVeiculo = null;
                string descricaoDataChegadaVeiculo = "";
                Dominio.Entidades.Empresa transportador = null;

                if (ocorrencia.Carga != null)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = repFluxoGestaoPatio.BuscarPorCargaETipo(ocorrencia.Carga.Codigo, TipoFluxoGestaoPatio.Origem);

                    if (fluxoPatio != null)
                    {
                        descricaoDataChegadaVeiculo = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork).ObterDescricaoEtapa(fluxoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo)?.Descricao ?? string.Empty;
                        dataChegadaVeiculo = fluxoPatio.DataChegadaVeiculo;
                    }
                }

                if (ocorrencia.Emitente != null)
                    transportador = ocorrencia.Emitente;
                else if (ocorrencia.Carga != null)
                    transportador = ocorrencia.Carga.Empresa;
                else if (ocorrencia.ContratoFrete != null)
                    transportador = ocorrencia.ContratoFrete.Transportador;

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaOcorrenciaDocumento.BuscarCTePorOcorrencia(ocorrencia.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (cargaCTe != null) ? repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPrimeiraCargaPedidoPorCargaCTe(cargaCTe.Codigo) : null;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (cargaPedido != null) ? repositorioCargaEntrega.BuscarEntregaPorCargaPedido(cargaPedido.Codigo) : null;

                string dataEntradaRaio = cargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
                string dataSaidaRaio = cargaEntrega?.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;

                var dynOcorrencia = new
                {
                    ocorrencia.Codigo,
                    ocorrencia.NumeroOcorrencia,
                    ocorrencia.Pagamento,
                    Solicitante = ocorrencia.Usuario?.Nome,
                    DataHoraPrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataOcorrencia = ocorrencia.DescricaoDataOcorrencia,
                    CodigoCarga = ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    TipoOcorrencia = ocorrencia.TipoOcorrencia?.Descricao ?? string.Empty,
                    TipoDocumentoCreditoDebito = ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito.ObterDescricao() ?? string.Empty,
                    OcorrenciaPorPeriodo = ocorrencia.OrigemOcorrenciaPorPeriodo,
                    ComponenteFrete = ocorrencia.ComponenteFrete != null ? ocorrencia.ComponenteFrete.Descricao : string.Empty,
                    Situacao = ocorrencia.DescricaoSituacao,
                    EnumSituacao = ocorrencia.SituacaoOcorrencia,
                    DataEntradaRaio = dataEntradaRaio,
                    DataSaidaRaio = dataSaidaRaio,

                    Remetentes = ocorrencia.Carga?.DadosSumarizados?.Remetentes,
                    Distancia = ocorrencia.Carga?.Distancia > 0 ? ocorrencia.Carga.Distancia : ocorrencia.Carga?.DadosSumarizados?.Distancia ?? 0,
                    ModeloVeicularCarga = ocorrencia.Carga?.ModeloVeicularCarga?.Descricao,

                    Cliente = cliente,
                    Transportador = transportador?.RazaoSocial ?? string.Empty,
                    Terceiro = ocorrencia.Usuario?.ClienteTerceiro?.Nome ?? ocorrencia.Carga?.Terceiro?.Nome,
                    Filial = ocorrencia.Carga?.Filial?.Descricao ?? string.Empty,
                    Motoristas = !string.IsNullOrWhiteSpace(ocorrencia.Carga?.DadosSumarizados?.Motoristas) ? ocorrencia.Carga?.DadosSumarizados?.Motoristas : ocorrencia.Carga?.Motoristas.FirstOrDefault()?.Descricao,
                    Veiculos = ocorrencia.Carga?.DadosSumarizados?.Veiculos ?? ocorrencia.Carga?.Veiculo?.Placa + (ocorrencia.Carga?.VeiculosVinculados?.Count > 0 ? ", " + ocorrencia.Carga?.VeiculosVinculados.FirstOrDefault().Placa : ""),
                    Origens = ocorrencia.Carga?.DadosSumarizados?.Origens ?? string.Empty,
                    Destinos = ocorrencia.Carga?.DadosSumarizados?.Destinos ?? string.Empty,

                    ParametroDataInicio = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataInicio.ToString() : string.Empty,
                    DescricaoParametroDataInicio = ocorrenciaParametroPeriodo?.ParametroOcorrencia?.DescricaoParametro ?? string.Empty,
                    ParametroDataFim = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataFim.ToString() : string.Empty,
                    DescricaoParametroDataFim = ocorrenciaParametroPeriodo?.ParametroOcorrencia?.DescricaoParametroFinal ?? string.Empty,

                    ParametroData = ocorrenciaParametroData != null ? ocorrenciaParametroData.Data.ToString() : string.Empty,
                    DescricaoParametroData = ocorrenciaParametroData?.ParametroOcorrencia?.DescricaoParametro ?? string.Empty,

                    ParametroApenasReboque = ocorrenciaParametroBooleano != null ? ocorrenciaParametroBooleano.Booleano == true ? "Sim" : "Não" : string.Empty,
                    CalculaValorPorTabelaFrete = ocorrencia.TipoOcorrencia?.CalculaValorPorTabelaFrete ?? false,
                    ParametroPeriodoHoras = periodoHoras,
                    ParametroTexto = ocorrenciaParametroTexto?.Texto ?? string.Empty,
                    ocorrencia.Observacao,
                    Chamados = chamadosDyn,
                    MotivoCancelamento = cancelamento?.MotivoCancelamento ?? string.Empty,
                    PermiteSelecionarTomador = ocorrencia.TipoOcorrencia?.PermiteSelecionarTomador ?? false,
                    PermitirAnalisarOcorrencia = ocorrencia.ResponsavelAutorizacao == null && (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao || ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao),
                    PermitirRemoverAnaliseOcorrencia = ocorrencia.ResponsavelAutorizacao != null && ocorrencia.ResponsavelAutorizacao.Codigo == this.Usuario.Codigo,
                    CodigoClienteTomador = ocorrencia.Tomador != null ? ocorrencia.Tomador.CPF_CNPJ.ToString() : string.Empty,
                    NomeClienteTomador = ocorrencia.Tomador != null ? ocorrencia.Tomador.Nome + "(" + ocorrencia.Tomador.CPF_CNPJ.ToString() + ")" : string.Empty,
                    ExibirParcelasNaAprovacao = ocorrencia.TipoOcorrencia?.ExibirParcelasNaAprovacao ?? false,
                    ocorrencia.QuantidadeParcelas,
                    ocorrencia.PeriodoPagamento,
                    Pedido = ObterNumeroPedidosCarga(ocorrencia.Carga, unitOfWork),
                    NotaFiscal = ObterNumeroNotasFiscaisCarga(ocorrencia.Carga, unitOfWork),
                    PercentualJurosParcela = ocorrencia.PercentualJurosParcela.ToString("n2"),
                    ObservacaoImpressa = ocorrencia.ObservacaoCTe,
                    Manobristas = manobristas != null ? new
                    {
                        manobristas.QuantidadeDias,
                        manobristas.QuantidadeMotoristas,
                        manobristas.ValorDiaria,
                        manobristas.ValorQuinzena,
                        manobristas.Total,
                    } : null,
                    DataChegadaVeiculo = dataChegadaVeiculo.HasValue ? new
                    {
                        DataChegadaVeiculo = dataChegadaVeiculo.Value.ToDateTimeString(),
                        Descricao = descricaoDataChegadaVeiculo
                    } : null,

                    ValorOcorrencia = ocorrencia.ValorOcorrencia.ToString("n2"),
                    Tomador = ocorrencia.Responsavel,
                    TomadorPadrao = ocorrencia.TipoOcorrencia == null ? Dominio.Enumeradores.TipoTomador.NaoInformado : ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Remetente ? Dominio.Enumeradores.TipoTomador.Remetente :
                                    ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Destinatario ? Dominio.Enumeradores.TipoTomador.Destinatario :
                                    ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros ? Dominio.Enumeradores.TipoTomador.Outros : Dominio.Enumeradores.TipoTomador.NaoInformado,

                    ocorrencia.ErroIntegracaoComGPA,
                    ocorrencia.ObservacaoAprovador,
                    ocorrencia.CodigoAprovacao
                };

                return new JsonpResult(dynOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int usuario = 0;// int.TryParse(Request.Params("Usuario"), out int usuario); Alterado para ao detalhar uma ocorrência trazer todos os aprovadores

                EtapaAutorizacaoOcorrencia? etapaAutorizacaoOcorrencia = Request.GetNullableEnumParam<EtapaAutorizacaoOcorrencia>("EtapaAutorizacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Regra, "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Usuario, "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Setor, "Setor", 15, Models.Grid.Align.left, false);

                if (etapaAutorizacaoOcorrencia.HasValue)
                    grid.AdicionarCabecalho("Etapa", false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Etapa, "Etapa", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Data Hora", "DataHora", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("MotivoRejeicao", false);
                grid.AdicionarCabecalho("InformarMotivoNaAprovacao", false);
                grid.AdicionarCabecalho("OcorrenciaProvisionada", false);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> regras = repCargaOcorrenciaAutorizacao.BuscarPorOcorrenciaUsuarioEtapa(codigo, usuario, etapaAutorizacaoOcorrencia);

                var lista = (from ocorrenciaAutorizacao in regras
                             select new
                             {
                                 ocorrenciaAutorizacao.Codigo,
                                 Regra = TituloRegra(ocorrenciaAutorizacao),
                                 MotivoRejeicao = ocorrenciaAutorizacao.MotivoRejeicaoOcorrencia?.Descricao ?? string.Empty,
                                 Observacao = ocorrenciaAutorizacao.Motivo,
                                 ocorrenciaAutorizacao.DescricaoSituacao,
                                 ocorrenciaAutorizacao.Situacao,
                                 InformarMotivoNaAprovacao = ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia?.InformarMotivoNaAprovacao ?? false,
                                 OcorrenciaProvisionada = ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia?.OcorrenciaProvisionada ?? false,
                                 Usuario = ocorrenciaAutorizacao.Usuario?.Nome ?? string.Empty,
                                 Setor = ocorrenciaAutorizacao.Usuario?.Setor?.Descricao ?? string.Empty,
                                 Etapa = ocorrenciaAutorizacao.DescricaoEtapaAutorizacaoOcorrencia,
                                 DataHora = ocorrenciaAutorizacao.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 PodeAprovar = repCargaOcorrenciaAutorizacao.VerificarSePodeAprovar(codigo, ocorrenciaAutorizacao.Codigo, this.Usuario.Codigo),
                                 DT_RowColor = this.CoresRegras(ocorrenciaAutorizacao)
                             }).ToList();

                grid.setarQuantidadeTotal(regras.Count());
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

        public async Task<IActionResult> AprovarMultiplasOcorrencias(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias;

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);

                Dominio.Enumeradores.TipoTomador? tomador = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("Tomador")) && Request.Params("Tomador") != "9")
                    if (Enum.TryParse(Request.Params("Tomador"), out Dominio.Enumeradores.TipoTomador tomadorAux))
                        tomador = tomadorAux;

                Dominio.Entidades.Cliente clienteTomador = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("ClienteTomador")))
                    clienteTomador = await repCliente.BuscarPorCPFCNPJAsync(double.Parse(Utilidades.String.OnlyNumbers(Request.Params("ClienteTomador"))));

                if (tomador != null && tomador == Dominio.Enumeradores.TipoTomador.Outros && clienteTomador == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ClienteTomadorNaoInformado);

                int quantidadeParcelas = Request.GetIntParam("QuantidadeParcelas");
                decimal percentualJurosParcela = Request.GetDecimalParam("PercentualJurosParcela");
                string codigoAprovacao = Request.GetStringParam("CodigoAprovacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento>("PeriodoPagamento");

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    ocorrencias = await ObterOcorrenciasSelecionadas(unitOfWork, cancellationToken);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                if (ocorrencias != null && ocorrencias.Count > 0 && ocorrencias.FirstOrDefault().TipoOcorrencia.ExibirParcelasNaAprovacao && quantidadeParcelas <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObrigatorioInformarQuantidadeParcelasParaAprovar);

                // Se a ocorrência exige, seta o código de aprovação
                if (ocorrencias.Any(o => o.TipoOcorrencia.ExigirCodigoParaAprovacao && string.IsNullOrWhiteSpace(codigoAprovacao)))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoOcorrenciaExigeInformarCodigoAprovacao);

                if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                    DefinirCodigoAprovacaoMultiplasOcorrencias(ocorrencias, codigoAprovacao, unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork, cancellationToken).BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork, cancellationToken).BuscarConfiguracaoPadraoAsync();
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = servicoAutorizacaoOcorrencia.BuscarRegrasPorOcorrencias(ocorrencias, Usuario.Codigo);

                int quantidadeAprovadas = servicoAutorizacaoOcorrencia.AprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, tomador, quantidadeParcelas, percentualJurosParcela, periodoPagamento);

                return new JsonpResult(new
                {
                    RegrasModificadas = quantidadeAprovadas,
                    RegrasExigemMotivo = ocorrenciasAutorizacoes.Count - quantidadeAprovadas
                });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoAprovarOcorrencias);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReprovarMultiplasOcorrencias(CancellationToken cancellationToken)
        {
            /* Busca todas as ocorrencias selecionadas
             * Busca todas as regras das ocorrencias selecionadas
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias;

                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                Dominio.Enumeradores.TipoTomador? tomador = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("Tomador")) && Request.Params("Tomador") != "9")
                    if (Enum.TryParse(Request.Params("Tomador"), out Dominio.Enumeradores.TipoTomador tomadorAux))
                        tomador = tomadorAux;

                Dominio.Entidades.Cliente clienteTomador = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("ClienteTomador")))
                    clienteTomador = await repCliente.BuscarPorCPFCNPJAsync(double.Parse(Utilidades.String.OnlyNumbers(Request.Params("ClienteTomador"))));

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigoJustificativa);

                if (justificativa == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroAoBuscarJustificativa);

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoObrigatorio);

                try
                {
                    ocorrencias = await ObterOcorrenciasSelecionadas(unitOfWork, cancellationToken);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                int quantidadeParcelas = Request.GetIntParam("QuantidadeParcelas");
                decimal percentualJurosParcela = Request.GetDecimalParam("PercentualJurosParcela");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento>("PeriodoPagamento");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadraoAsync();

                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repCargaOcorrenciaAutorizacao);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = servicoAutorizacaoOcorrencia.BuscarRegrasPorOcorrencias(ocorrencias, this.Usuario.Codigo);

                servicoAutorizacaoOcorrencia.ReprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, motivo, justificativa, tomador, clienteTomador, percentualJurosParcela, quantidadeParcelas, periodoPagamento);

                return new JsonpResult(new
                {
                    RegrasModificadas = ocorrenciasAutorizacoes.Count
                });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoReprovarOcorrencias);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DelegarMultiplasOcorrencias(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork, cancellationToken);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);

                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Usuario responsavel = await repUsuario.BuscarPorCodigoAsync(codigoUsuario);

                if (responsavel == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroBuscarUsuario);

                if (Usuario.Codigo == codigoUsuario)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoEPossivelDelegarParaVoceMesmo);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OperacaoNaoPermitida);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias;

                try
                {
                    ocorrencias = await ObterOcorrenciasSelecionadas(unitOfWork, cancellationToken);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = await repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadraoAsync();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia in ocorrencias)
                {
                    bool usuarioPodeDelegar = configuracaoEmbarcador.SomenteAutorizadoresPodemDelegarOcorrencia ? await repCargaOcorrenciaAutorizacao.VerificarSeUsuarioEstaNaRegraOcorrenciaAsync(cargaOcorrencia.Codigo, Usuario.Codigo) : true;

                    if (cargaOcorrencia.SituacaoOcorrencia.IsPermiteDelegar() && usuarioPodeDelegar)
                        EfetuarResponsabilidade(cargaOcorrencia, responsavel, unitOfWork, observacao, configuracaoAprovacao.PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoDelegarSolicitacoes);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            /* Busca todas as regras da ocorrencia
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoOcorrencia);

                Dominio.Enumeradores.TipoTomador? tomador = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoTomador>("Tomador");

                Dominio.Entidades.Cliente clienteTomador = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("ClienteTomador")))
                    clienteTomador = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(Request.Params("ClienteTomador"))));

                if (tomador != null && tomador == Dominio.Enumeradores.TipoTomador.Outros && clienteTomador == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ClienteTomadorNaoInformado);

                int quantidadeParcelas = Request.GetIntParam("QuantidadeParcelas");
                decimal percentualJurosParcela = Request.GetDecimalParam("PercentualJurosParcela");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento>("PeriodoPagamento");

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = repCargaOcorrenciaAutorizacao.BuscarPendentesPorOcorrenciaEUsuario(codigoOcorrencia, this.Usuario.Codigo);

                if (ocorrenciasAutorizacoes != null && ocorrenciasAutorizacoes.Count > 0 && ocorrenciasAutorizacoes.FirstOrDefault().CargaOcorrencia.TipoOcorrencia.ExibirParcelasNaAprovacao && quantidadeParcelas <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObrigatorioInformarQuantidadeParcelasParaAprovar);

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repCargaOcorrenciaAutorizacao);

                int quantidadeAprovadas = 0;
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao ocorrenciaAutorizacao in ocorrenciasAutorizacoes)
                {
                    if (ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia?.InformarMotivoNaAprovacao ?? false)
                        continue;
                    servicoAutorizacaoOcorrencia.EfetuarAprovacao(ocorrenciaAutorizacao, null, null, null, Request.GetEnumParam<AutorizacaoOcorrenciaPagamento>("Pagamento"));
                    quantidadeAprovadas++;
                }

                if (quantidadeAprovadas > 0)
                    servicoAutorizacaoOcorrencia.VerificarSituacaoOcorrencia(repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia), tomador, clienteTomador, quantidadeParcelas, percentualJurosParcela, periodoPagamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = quantidadeAprovadas,
                    RegrasExigemMotivo = ocorrenciasAutorizacoes.Count() - quantidadeAprovadas
                });
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
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoAprovarOcorrencias);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");
                int codigoOcorrencia = Request.GetIntParam("Ocorrencia");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                Dominio.Entidades.Usuario responsavel = repUsuario.BuscarPorCodigo(codigoUsuario);

                if (cargaOcorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroAoBuscarOcorrencia);

                if (Usuario.Codigo == codigoUsuario)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoEPossivelDelegarParaVoceMesmo);

                if (!cargaOcorrencia.SituacaoOcorrencia.IsPermiteDelegar())
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteEssaOperacao);

                if (responsavel == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroBuscarUsuario);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OperacaoNaoPermitida);

                bool usuarioPodeDelegar = configuracaoEmbarcador.SomenteAutorizadoresPodemDelegarOcorrencia ? repCargaOcorrenciaAutorizacao.VerificarSeUsuarioEstaNaRegraOcorrencia(codigoOcorrencia, Usuario.Codigo) : true;
                if (!usuarioPodeDelegar)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceNaoEstaAutorizadoParaDelegarEssaOcorrencia);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                string erro = EfetuarResponsabilidade(cargaOcorrencia, responsavel, unitOfWork, observacao, configuracaoAprovacao.PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas);

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoDelegarOcorrencias);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao ocorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorOcorrencia(ocorrenciaAutorizacao.CargaOcorrencia.Codigo);

                if (ocorrenciaAutorizacao == null || ocorrenciaAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreUmaFlhaAoBuscarDados);

                if (ocorrenciaAutorizacao.Bloqueada)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoEPermitidoAutorizarAlcadaAntesQueAlcadas);

                if ((ocorrenciaAutorizacao.CargaOcorrencia.ResponsavelAutorizacao != null) && (ocorrenciaAutorizacao.CargaOcorrencia.ResponsavelAutorizacao.Codigo == this.Usuario.Codigo))
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovacaoDaOcorrenciaJaEsta, ocorrenciaAutorizacao.CargaOcorrencia.ResponsavelAutorizacao.Descricao));

                if (ocorrenciaAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoDaAprovacaoNaoPermiteAlteracoesDaMesma);

                if (ocorrenciaAutorizacao.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao && ocorrenciaAutorizacao.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAutorizacaoEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteAprovacoes);

                if (ocorrenciaCancelamento != null && (ocorrenciaCancelamento.Situacao == SituacaoCancelamentoOcorrencia.EmCancelamento || ocorrenciaCancelamento.Situacao == SituacaoCancelamentoOcorrencia.Cancelada))
                    return new JsonpResult(false, true, "Não é possível aprovar ocorrência cancelada ou em cancelamento");

                int quantidadeParcelas = Request.GetIntParam("QuantidadeParcelas");
                decimal percentualJurosParcela = Request.GetDecimalParam("PercentualJurosParcela");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento>("PeriodoPagamento");

                if (ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia.ExibirParcelasNaAprovacao && quantidadeParcelas <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObrigatorioInformarQuantidadeParcelasParaAprovar);

                Dominio.Enumeradores.TipoTomador? tipoTomador = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoTomador>("Tomador");
                AutorizacaoOcorrenciaPagamento? pagamento = Request.GetNullableEnumParam<AutorizacaoOcorrenciaPagamento>("Pagamento");
                double cpfCnpjClienteTomador = Request.GetStringParam("").ObterSomenteNumeros().ToDouble();
                Dominio.Entidades.Cliente clienteTomador = null;

                if (cpfCnpjClienteTomador > 0d)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                    clienteTomador = repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteTomador);
                }

                if (tipoTomador.HasValue && tipoTomador == Dominio.Enumeradores.TipoTomador.Outros && clienteTomador == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ClienteTomadorNaoInformado);

                string motivo = Request.GetNullableStringParam("Motivo");
                int codigoJustificativa = Request.GetIntParam("JustificativaAprovacao");
                int codigoCentroResultado = Request.GetIntParam("CentroResultado");

                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = codigoJustificativa > 0 ? repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigoJustificativa) : null;
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repCargaOcorrenciaAutorizacao);
                servicoAutorizacaoOcorrencia.EfetuarAprovacao(ocorrenciaAutorizacao, justificativa, motivo, centroResultado, pagamento);
                servicoAutorizacaoOcorrencia.VerificarSituacaoOcorrencia(ocorrenciaAutorizacao.CargaOcorrencia, tipoTomador, clienteTomador, quantidadeParcelas, percentualJurosParcela, periodoPagamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoAprovar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao ocorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarPorCodigo(codigo);
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

                if (ocorrenciaAutorizacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoBuscarPorDados);

                if (ocorrenciaAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelRealizarAcaoDeOutroUsuario);

                if (ocorrenciaAutorizacao.Bloqueada)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoEPermitidoAutorizarAlcadaAntesQueAlcadas);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = ocorrenciaAutorizacao.CargaOcorrencia;
                if ((ocorrencia.ResponsavelAutorizacao != null) && (ocorrencia.ResponsavelAutorizacao.Codigo == this.Usuario.Codigo))
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovacaoDaOcorrenciaJaEsta, ocorrencia.ResponsavelAutorizacao.Descricao));

                if (ocorrenciaAutorizacao.Situacao != SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoDaAprovacaoNaoPermiteAlteracoesDaMesma);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAprovacao && ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAutorizacaoEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteAprovacoes);

                string motivo = Request.GetNullableStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoObrigatorio);

                int codigoJustificativa = Request.GetIntParam("Justificativa");
                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigoJustificativa);

                if (justificativa == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroAoBuscarJustificativa);

                Dominio.Enumeradores.TipoTomador? tipoTomador = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoTomador>("Tomador");
                double cpfCnpjClienteTomador = Request.GetStringParam("").ObterSomenteNumeros().ToDouble();
                Dominio.Entidades.Cliente clienteTomador = null;

                int quantidadeParcelas = Request.GetIntParam("QuantidadeParcelas");
                decimal percentualJurosParcela = Request.GetDecimalParam("PercentualJurosParcela");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento>("PeriodoPagamento");

                if (cpfCnpjClienteTomador > 0d)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                    clienteTomador = repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteTomador);
                }

                if (tipoTomador.HasValue && tipoTomador == Dominio.Enumeradores.TipoTomador.Outros && clienteTomador == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ClienteTomadorNaoInformado);

                unitOfWork.Start();

                ocorrenciaAutorizacao.Data = DateTime.Now;
                ocorrenciaAutorizacao.Situacao = SituacaoOcorrenciaAutorizacao.Rejeitada;
                ocorrenciaAutorizacao.Motivo = motivo;
                ocorrenciaAutorizacao.MotivoRejeicaoOcorrencia = justificativa;

                repCargaOcorrenciaAutorizacao.Atualizar(ocorrenciaAutorizacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RejeitadoPor, ocorrenciaAutorizacao.Usuario?.Nome), unitOfWork);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repCargaOcorrenciaAutorizacao);

                servicoAutorizacaoOcorrencia.NotificarUsuarioAlteracao(false, ocorrencia);
                servicoAutorizacaoOcorrencia.VerificarSituacaoOcorrencia(ocorrencia, tipoTomador, clienteTomador, quantidadeParcelas, percentualJurosParcela, periodoPagamento);

                if (ocorrenciaAutorizacao.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada || ocorrenciaAutorizacao.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Assumida)
                    ocorrencia.UltimoUsuarioDelegado = ocorrencia.UltimoUsuarioDelegou;

                ocorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;
                repCargaOcorrencia.Atualizar(ocorrencia);

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(ocorrencia.Codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                {
                    if (chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                    {
                        chamado.Situacao = SituacaoChamado.LiberadaOcorrencia;
                        repChamado.Atualizar(chamado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RetornadoSituacaoParaLiberadoParaOcorrenciaAoRejeitarOcorrencia, ocorrencia.NumeroOcorrencia), unitOfWork);
                    }
                }

                unitOfWork.CommitChanges();

                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
                    if ((chamado.Situacao == SituacaoChamado.LiberadaOcorrencia || chamado.Situacao == SituacaoChamado.Finalizado) && cargaEntrega != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
                        if (cargaEvento != null)
                        {
                            servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.FinalizadoAposFinalizacaoAtendimento);
                            servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                        }
                    }
                }

                if (ocorrenciaAutorizacao.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada)
                    servicoAutorizacaoOcorrencia.NotificarOcorrenciaDelegadaRejeitadaPeloAprovador(ocorrencia, this.Usuario, ocorrencia.UltimoUsuarioDelegado);

                return new JsonpResult(true);
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
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoRejeitar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnexarArquivosAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.QuantidadeArquivosInvalidaParaImportacao);


                if (cargaOcorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaNaoLocalizadaParaAnexarArquivo);

                if (//cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.DebitoRejeitadoTransportador ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada ||
                    cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteAnexarArquivos);

                if (repCargaOcorrenciaAutorizacao.BuscarTodosPorOcorrenciaEUsuario(codigo, this.Usuario.Codigo).Count() <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SomenteUsuariosAlcadaPodemInserirAnexosAutorizacao);

                List<object> retorno = new List<object>();

                for (int i = 0; i < files.Count; i++)
                {
                    try
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                        // Extrai dados
                        string nomeArquivo = file.FileName;
                        string extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                        string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                        string caminho = this.CaminhoArquivos(unitOfWork);

                        // Salva na pasta
                        file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos cargaOcorrenciaAnexos = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos();
                        cargaOcorrenciaAnexos.CargaOcorrencia = cargaOcorrencia;
                        cargaOcorrenciaAnexos.Descricao = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AnexoInseridoPorEm, this.Usuario.Nome, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        cargaOcorrenciaAnexos.GuidArquivo = guidArquivo;
                        cargaOcorrenciaAnexos.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));

                        repCargaOcorrenciaAnexos.Inserir(cargaOcorrenciaAnexos);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrenciaAnexos.CargaOcorrencia, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AdicionouAnexo, cargaOcorrenciaAnexos.NomeArquivo), unitOfWork);

                        return new JsonpResult(true);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        return new JsonpResult(false, false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoFoiPossivelAdicionarAnexo);
                    }
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoAnexo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnalisarAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAprovacao && ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAutorizacaoEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteAnalisarAprovacao);

                if (ocorrencia.ResponsavelAutorizacao != null)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovacaoDaOcorrenciaJaEsta, ocorrencia.ResponsavelAutorizacao.Descricao));

                //int codigoUsuario = Request.GetIntParam("Usuario");
                //Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                //Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                //if (usuario == null)
                //    return new JsonpResult(false, true, "Não foi possível encontrar o usuário.");

                Dominio.Entidades.Usuario usuario = this.Usuario;
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                bool existemAprovacoesPendentesUsuario = repositorioCargaOcorrenciaAutorizacao.VerificarExistemAprovacoesPendentes(codigoOcorrencia, usuario.Codigo, ocorrencia.EtapaAutorizacaoOcorrencia);

                if (!existemAprovacoesPendentesUsuario)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoExistemAprovacoesPendentesParaUsuario);

                ocorrencia.ResponsavelAutorizacao = usuario;

                repositorioCargaOcorrencia.Atualizar(ocorrencia);

                return new JsonpResult(new
                {
                    PermitirRemoverAnaliseOcorrencia = usuario.Codigo == this.Usuario.Codigo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoSolicitarAnaliseAprovacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverAnaliseAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.ResponsavelAutorizacao == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovacaoOcorrenciaNaoEstaSendoAnalisada);

                if (ocorrencia.ResponsavelAutorizacao.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AnaliseAprovacaoOcorrenciaPodeSerRemovidaSomentoPor, ocorrencia.ResponsavelAutorizacao.Descricao));

                ocorrencia.ResponsavelAutorizacao = null;

                repositorioCargaOcorrencia.Atualizar(ocorrencia);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoRemoverAnaliseAprovacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacaoAprovador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia, true);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AutorizacaoPendente && ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAprovacao && ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAutorizacaoEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteSalvarObservacaoAprovador);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                bool podeSalvar = repositorioCargaOcorrenciaAutorizacao.VerificarSeUsuarioEstaNaRegraOcorrencia(codigoOcorrencia, Usuario.Codigo);

                if (!podeSalvar)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceNaoEstaAutorizadoParaSalvarObservacaoAprovador);

                ocorrencia.ObservacaoAprovador = Request.GetStringParam("ObservacaoAprovador");

                repositorioCargaOcorrencia.Atualizar(ocorrencia, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoSalvarObservacaoAprovador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [Obsolete("Este método será removido. Criado apenas para ajustar as situações das ocorrências que não estão com as próximas hierarquias liberadas")]
        public async Task<IActionResult> VerificarAprovacoesMultiplasOcorrencias(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias;

                try
                {
                    ocorrencias = await ObterOcorrenciasSelecionadas(unitOfWork, cancellationToken);
                }
                catch (Exception excecaoObterOcorrencias)
                {
                    throw new ControllerException(excecaoObterOcorrencias.Message);
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadraoAsync();

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia in ocorrencias)
                    servicoAutorizacaoOcorrencia.VerificarSituacaoOcorrencia(cargaOcorrencia, tomador: null, tomadorOutros: null, cargaOcorrencia.QuantidadeParcelas, cargaOcorrencia.PercentualJurosParcela, cargaOcorrencia.PeriodoPagamento);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoVerificarAprovacoesOcorrencias);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AssumirOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NaoFoiPossivelEncontrarRegistro);

                if (!ocorrencia.SituacaoOcorrencia.IsPermiteDelegar())
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SituacaoOcorrenciaNaoPermiteEssaOperacao);

                if (!Usuario.PermiteAssumirOcorrencia)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceNaoTemPermissaoParaAssumirOcorrencia);

                if (repositorioCargaOcorrenciaAutorizacao.VerificarSeUsuarioEstaNaRegraOcorrencia(codigo, Usuario.Codigo))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceJaEstaAdicionadoNaListaAprovadores);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao()
                {
                    CargaOcorrencia = ocorrencia,
                    Usuario = Usuario,
                    OrigemRegraOcorrencia = OrigemRegraOcorrencia.Assumida,
                    EtapaAutorizacaoOcorrencia = ocorrencia.EtapaAutorizacaoOcorrencia,
                    Observacao = "",
                    Data = DateTime.Now,
                    NumeroAprovadores = 1,
                    NumeroReprovadores = 1,
                    PrioridadeAprovacao = (ocorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia) ? ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao : ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao
                };

                repositorioCargaOcorrenciaAutorizacao.Inserir(cargaOcorrenciaAutorizacao);

                ocorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;
                ocorrencia.UltimoUsuarioDelegou = ocorrencia.UltimoUsuarioDelegado;
                ocorrencia.UltimoUsuarioDelegado = Usuario;
                repositorioCargaOcorrencia.Atualizar(ocorrencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AssumiuResposabilidadeOcorrencia, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorreuUmaFalhaAoAssumirOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterMiniaturasOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");


                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> cargaOcorrenciaAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(codigo);
                if (cargaOcorrenciaAnexos == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleEntrega.NaoFoiPossivelEncontrarEntrega);

                List<dynamic> imagens = new List<dynamic>();

                imagens.AddRange((from anexo in cargaOcorrenciaAnexos
                                  select new
                                  {
                                      anexo.Codigo,
                                      anexo.Descricao,
                                      Miniatura = servicoAutorizacaoOcorrencia.ObterMiniatura(anexo, unitOfWork),
                                  }).ToList());

                return new JsonpResult(new
                {
                    Imagens = imagens
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultasOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Data, "DataOcorrencia", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Ocorrencia, "NumeroOcorrencia", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Viagem, "NumeroViagem", 7, Models.Grid.Align.center, true);

            if (ConfiguracaoEmbarcador.ExibirColunaCodigosAgrupadosOcorrencia)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OutrosNumeros, "CodigosAgrupados", 7, Models.Grid.Align.left, false);

            if (ConfiguracaoEmbarcador.ExibirPrioridadesAutorizacaoOcorrencia)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Atendimento, "Atendimento", 6, Models.Grid.Align.center, false);

            if (ConfiguracaoEmbarcador.ExibirColunaValorFreteCargaOcorrencia)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorFreteCarga, "ValorFreteCarga", 6, Models.Grid.Align.right, false);

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Responsavel, "Responsavel", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoOcorrencia, "Ocorrencia", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Transportador, "Transportador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoOperacao, "TipoOperacao", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoVeiculo, "TipoVeiculo", 10, Models.Grid.Align.left, false, false);

            if (!ConfiguracaoEmbarcador.ExibirPrioridadesAutorizacaoOcorrencia)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Terceiro, "Proprietario", 7, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NumeroPedidoCliente, "NumeroPedidoCliente", 15, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EscritorioVendas, "EscritorioVendas", 6, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Matriz, "Matriz", 6, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Vendedor, "Vendedor", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Supervisor, "Supervisor", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Gerente, "Gerente", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.UFDestino, "UFDestino", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NumeroNF, "NumeroNF", 15, Models.Grid.Align.center, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Filial, "Filial", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Destino, "Destino", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Valor, "Valor", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Situacao, "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoCreditoDebito, "TipoDocumentoCreditoDebito", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Observacao, "Observacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Placa, "Placa", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Motorista, "Motorista", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ModeloVeicular, "ModeloVeicular", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Destinatario, "Destinatario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataAprovacaoPrimeiraAlcada, "DataAprovacaoPrimeiraAlcada", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CNPJTomador, "CNPJTomador", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CNPJEmpresaFilial, "CNPJEmpresaFilial", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CentroResultado, "CentroDeResultado", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PossuiAnexo, "PossuiAnexo", 10, Models.Grid.Align.left, false);

            if (ConfiguracaoEmbarcador.ExibirPrioridadesAutorizacaoOcorrencia)
            {
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Prioridade, "Prioridade", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Usuario, "Usuario", 6, Models.Grid.Align.left, false);
            }

            grid.AdicionarCabecalho("ExibirParcelasNaAprovacao", false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AutorizacaoOcorrencia/Pesquisa", "grid-autorizacao-ocorrencias");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Valor")
                propOrdena = "ValorOcorrencia";
            else if (propOrdena == "Situacao")
                propOrdena = "SituacaoOcorrencia";
            else if (propOrdena == "NumeroViagem")
                propOrdena = "Carga.CodigoCargaEmbarcador";
            else if (propOrdena == "Ocorrencia")
                propOrdena = "TipoOcorrencia.Descricao";
            else if (propOrdena == "Transportador")
                propOrdena = "Carga.Empresa.RazaoSocial";
            else if (propOrdena == "Filial")
                propOrdena = "Carga.Filial.Descricao";
            else if (propOrdena == "Proprietario")
                propOrdena = "Usuario.ClienteTerceiro.Nome";
        }

        private async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> ObterOcorrenciasSelecionadas(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciasObjetoValor = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    /*dynamic dynPesquisa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Pesquisa"));

                    // Converte parametros
                    int numeroOcorrencia = dynPesquisa.NumeroOcorrencia != null ? (int)dynPesquisa.NumeroOcorrencia : 0;
                    int tipoOcorrencia = dynPesquisa.Ocorrencia != null ? (int)dynPesquisa.Ocorrencia : 0;
                    int transportador = dynPesquisa.Transportador != null ? (int)dynPesquisa.Transportador : 0;
                    int filial = dynPesquisa.Filial != null ? (int)dynPesquisa.Filial : 0;

                    DateTime dataInicial, dataFinal;
                    DateTime.TryParseExact((string)dynPesquisa.DataInicial, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                    DateTime.TryParseExact((string)dynPesquisa.DataFinal, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia situacao;
                    Enum.TryParse((string)dynPesquisa.Situacao, out situacao);

                    string codigoCarga = (string)dynPesquisa.CodigoCarga;

                    int totalRegistros = repCargaOcorrenciaAutorizacao.ContarConsulta(this.Usuario.Codigo, dataInicial, dataFinal, situacao, numeroOcorrencia, codigoCarga, tipoOcorrencia, transportador, filial);
                    listaOcorrencias = repCargaOcorrenciaAutorizacao.Consultar(this.Usuario.Codigo, dataInicial, dataFinal, situacao, numeroOcorrencia, codigoCarga, tipoOcorrencia, transportador, filial, "Codigo", "", 0, totalRegistros);*/
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaOcorrencias, ref listaOcorrenciasObjetoValor, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ErroAoConverterDados);
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaOcorrenciasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("OcorrenciasNaoSelecionadas"));
                foreach (dynamic dybOcorrenciaNaoSelecionada in listaOcorrenciasNaoSelecionadas)
                    listaOcorrencias.Remove(new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia() { Codigo = (int)dybOcorrenciaNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaOcorrenciasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("OcorrenciasSelecionadas"));
                List<int> codigosOcorrenciasSelecionadas = new List<int>();

                foreach (dynamic dynOcorrenciasSelecionada in listaOcorrenciasSelecionadas)
                    codigosOcorrenciasSelecionadas.Add((int)dynOcorrenciasSelecionada.Codigo);

                listaOcorrencias.AddRange(await repCargaOcorrencia.BuscarPorCodigoAsync(codigosOcorrenciasSelecionadas));
            }

            return listaOcorrencias;
        }

        private string RetornarClientePedido(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (ocorrencia.Carga == null) return null;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Cliente cliente = configuracaoOcorrencia.ExibirDestinatarioOcorrencia ? repCargaOcorrenciaDocumento.ObterPrimeiroDestinatario(ocorrencia.Codigo) : repCargaOcorrenciaDocumento.ObterPrimeiroTomador(ocorrencia.Codigo);
            if (cliente != null)
                return cliente.Descricao;

            return string.Empty;
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao regra)
        {
            if (regra.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada)
                return CorGrid.Success;

            if (regra.Situacao == SituacaoOcorrenciaAutorizacao.Rejeitada)
                return CorGrid.Danger;

            if (regra.CargaOcorrencia.ResponsavelAutorizacao != null && regra.CargaOcorrencia.ResponsavelAutorizacao.Codigo == regra.Usuario?.Codigo)
                return CorGrid.Amarelo;

            if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada)
                return CorGrid.Info;

            if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Assumida)
                return CorGrid.Turquoise;

            return "";
        }

        private dynamic ObterOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovacoesDesbloqueadas, List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> cargaOcorrenciaAutorizacaos, List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaObjValor, Repositorio.UnitOfWork unitOfWork)
        {
            int prioridadeAprovacaoAtual = 0;
            string aprovadoresPrioridadeAtual = "";
            Dominio.Entidades.Empresa transportador = null;
            //ocorrenciaObjValor é uma instancia com somente partes do objeto carregado, não acessar propriedades que não estejam sendo carregadas nele

            if (configuracao.ExibirPrioridadesAutorizacaoOcorrencia)
            {
                if (aprovacoesDesbloqueadas.Where(o => o.CargaOcorrencia.Codigo == ocorrencia.Codigo).Count() > 0)
                {
                    prioridadeAprovacaoAtual = (from o in aprovacoesDesbloqueadas where o.CargaOcorrencia.Codigo == ocorrencia.Codigo select o.Prioridade).Max();
                    aprovadoresPrioridadeAtual = string.Join(", ", (from o in aprovacoesDesbloqueadas where o.Prioridade == prioridadeAprovacaoAtual && o.CargaOcorrencia.Codigo == ocorrencia.Codigo select o.Usuario.Nome).ToList().Distinct());
                }
            }

            if (ocorrencia.Emitente != null)
                transportador = ocorrencia.Emitente;
            else if (ocorrencia.Carga != null)
                transportador = ocorrencia.Carga.Empresa;
            else if (ocorrencia.ContratoFrete != null)
                transportador = ocorrencia.ContratoFrete.Transportador;

            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            int codigoCargaOcorrencia = ocorrencia.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Pedidos.Pedido centroResultados = repPedidos.BuscarPorCarga(codigoCargaOcorrencia)?.FirstOrDefault() ?? null;
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar> clientesComplementares = ocorrencia?.Carga != null ? repositorioCargaPedido.BuscarClientesComplementaresPorCargas(new List<int>() { codigoCargaOcorrencia }).Result : new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar>();

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);
            int anexos = repCargaOcorrenciaAnexos.ContarConsulta(ocorrencia.Codigo);

            return new
            {
                ocorrencia.Codigo,
                DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy"),
                ocorrencia.NumeroOcorrencia,
                ocorrencia.Observacao,
                Proprietario = ocorrencia.Usuario?.ClienteTerceiro?.Nome ?? string.Empty,
                NumeroViagem = ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                Atendimento = string.Join(", ", chamados.Select(o => o.Descricao)),
                Responsavel = (configuracaoOcorrencia?.VisualizarUltimoUsuarioDelegadoOcorrencia ?? false) && ocorrencia.UltimoUsuarioDelegado != null ? ocorrencia.UltimoUsuarioDelegado.Nome :
                                string.Join(", ", (from obj in cargaOcorrenciaAutorizacaos where obj.CargaOcorrencia.Codigo == ocorrencia.Codigo && (obj.EtapaAutorizacaoOcorrencia == ocorrencia.EtapaAutorizacaoOcorrencia || obj.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada) select obj.Usuario.Nome).Distinct().ToList()),
                Ocorrencia = ocorrencia.TipoOcorrencia != null ? ocorrencia.TipoOcorrencia.Descricao : string.Empty,
                Transportador = transportador?.Descricao ?? string.Empty,
                PossuiAnexo = anexos > 0 ? "Com Anexo" : "Não",

                TipoOperacao = ocorrenciaObjValor.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                TipoVeiculo = ocorrenciaObjValor.Carga?.Veiculo?.DescricaoTipoVeiculo ?? string.Empty,
                Destinatario = ocorrenciaObjValor.DestinatariosCTes ?? string.Empty,
                CodigosAgrupados = ocorrencia.Carga != null ? string.Join(", ", ocorrenciaObjValor.Carga.CodigosAgrupados) : string.Empty,
                ModeloVeicular = ocorrenciaObjValor.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,


                Filial = ocorrenciaObjValor.Carga?.Filial?.Descricao ?? string.Empty,
                Valor = ocorrencia.ValorOcorrencia.ToString("n2"),
                Situacao = ocorrencia.DescricaoSituacao,
                Destino = ocorrenciaObjValor.Carga?.DadosSumarizados?.Destinos ?? string.Empty,
                Prioridade = prioridadeAprovacaoAtual,
                Usuario = aprovadoresPrioridadeAtual,
                ValorFreteCarga = ocorrencia.Carga?.ValorFrete.ToString("n2") ?? 0.ToString("n2"),
                TipoDocumentoCreditoDebito = ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito.ObterDescricao() ?? string.Empty,
                Motorista = ocorrenciaObjValor.Carga?.DadosSumarizados?.Motoristas ?? string.Empty,
                Placa = ocorrenciaObjValor.Carga?.DadosSumarizados?.Veiculos ?? string.Empty,
                ExibirParcelasNaAprovacao = ocorrencia.TipoOcorrencia?.ExibirParcelasNaAprovacao ?? false,
                DataAprovacaoPrimeiraAlcada = cargaOcorrenciaAutorizacaos.Where(x => x.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada && x.CargaOcorrencia.Codigo == ocorrencia.Codigo).OrderBy(c => c.Data).Select(c => c.Data).FirstOrDefault(),

                NumeroPedidoCliente = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarNumeroPedidosClientePorCarga(codigoCargaOcorrencia)),
                EscritorioVendas = string.Join(", ", clientesComplementares.Select(clienteComplementar => clienteComplementar.EscritorioVendas)) ?? string.Empty,
                Matriz = string.Join(", ", clientesComplementares.Select(clienteComplementar => clienteComplementar.Matriz)) ?? string.Empty,
                Vendedor = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarNomeFuncionariosVendedoresPorCarga(codigoCargaOcorrencia)),
                Supervisor = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarSupervisoresPorCarga(codigoCargaOcorrencia)),
                Gerente = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarGerentesPorCarga(codigoCargaOcorrencia)),
                UFDestino = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarUFsDestino(codigoCargaOcorrencia)),
                NumeroNF = ocorrencia.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarNFsPorCargas(new List<int>() { codigoCargaOcorrencia })),

                DT_RowColor = ocorrencia.DataPrazoAprovacao.HasValue && ocorrencia.DataPrazoAprovacao.Value < DateTime.Today ? CorGrid.Danger : string.Empty,
                DT_FontColor = CorGrid.Black,
                DT_Enable = true,
                CNPJTomador = ocorrencia.Carga?.Pedidos?.Count > 0 ? ocorrencia.Carga?.Pedidos?.FirstOrDefault().ObterTomador()?.CPF_CNPJ_Formatado : string.Empty,
                CNPJEmpresaFilial = ocorrencia.Carga?.Empresa != null ? ocorrencia.Carga?.Empresa?.CNPJ_Formatado : (ocorrencia.Filial?.CNPJ_Formatado ?? string.Empty),
                CentroDeResultado = centroResultados?.CentroResultado != null ? centroResultados?.CentroResultado?.Descricao : string.Empty
            };
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias, ref List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciasObjetoValor, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaAutorizacaoOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaAutorizacaoOcorrencia()
            {
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia"),
                CodigosTipoOcorrencia = Request.GetListParam<int>("Ocorrencia"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                CodigosUsuario = Request.GetListParam<int>("Usuario"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                SituacaoOcorrencia = Request.GetEnumParam<SituacaoOcorrencia>("Situacao"),
                EtapaAutorizacao = Request.GetNullableEnumParam<EtapaAutorizacaoOcorrencia>("EtapaAutorizacao"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                PrioridadesAprovacao = Request.GetListParam<int>("PrioridadeAprovacao"),
                TipoDocumentoCreditoDebito = Request.GetEnumParam("TipoDocumentoCreditoDebito", TipoDocumentoCreditoDebito.Todos),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CodigosClienteComplementar = Request.GetListParam<int>("ClienteComplementar"),
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                CodigosSupervisor = Request.GetListParam<int>("Supervisor"),
                CodigosGerente = Request.GetListParam<int>("Gerente"),
                CodigosUFDestino = Request.GetListParam<string>("UFDestino"),
                NumeroNF = Request.GetIntParam("NumeroNF")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigosTransportador = new List<int> { this.Usuario.Empresa.Codigo };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosOcorrencia = new List<int>();

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);


            codigosOcorrencia = repCargaOcorrenciaAutorizacao.BuscarCodigosOcorrencia(filtrosPesquisa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            listaOcorrencias = repCargaOcorrenciaAutorizacao.BuscarPorCodigosFetchPaginado(codigosOcorrencia, propOrdenacao, dirOrdenacao);

            listaOcorrenciasObjetoValor = repCargaOcorrenciaAutorizacao.BuscarObjetoParcialPorCodigosPaginado(codigosOcorrencia);

            totalRegistros = repCargaOcorrenciaAutorizacao.ContarConsulta(filtrosPesquisa);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias, List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrenciasComplementar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            List<int> codigosOcorrencia = (from obj in listaOcorrencias select obj.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovacoesDesbloqueadas = repCargaOcorrenciaAutorizacao.BuscarDesbloqueadasNaoDelegadasPorCargasOcorrenciasPaginado(codigosOcorrencia);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> cargaOcorrenciasAtualizacao = repCargaOcorrenciaAutorizacao.ResponsavelOcorrenciasPaginado(codigosOcorrencia);
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrenciaPaginado(codigosOcorrencia);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            IEnumerable<dynamic> lista = from ocorrencia in listaOcorrencias
                                         select ObterOcorrencia(ocorrencia, aprovacoesDesbloqueadas, cargaOcorrenciasAtualizacao, chamados, configuracao, configuracaoOcorrencia, listaOcorrenciasComplementar.Where(x => x.Codigo == ocorrencia.Codigo).FirstOrDefault(), unitOfWork);


            return lista.ToList();
        }

        /// <summary>
        /// Vincula o responsável à aprovação de ocorrência de acordo com a situação da ocorrência
        /// </summary>
        private string EfetuarResponsabilidade(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Usuario responsavel, Repositorio.UnitOfWork unitOfWork, string observacao, bool permitirDelegarParaUsuarioComTodasAlcadasRejeitadas)
        {
            string mensagem = string.Empty;

            if (cargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao || cargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> verificacao = repositorioCargaOcorrenciaAutorizacao.BuscarPorOcorrenciaUsuarioEtapa(cargaOcorrencia.Codigo, responsavel.Codigo, cargaOcorrencia.EtapaAutorizacaoOcorrencia);
                bool permitirDelegar = (verificacao.Count() == 0) || (verificacao.All(obj => obj.Situacao == SituacaoOcorrenciaAutorizacao.Rejeitada) && permitirDelegarParaUsuarioComTodasAlcadasRejeitadas);

                if (permitirDelegar)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                    Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao()
                    {
                        CargaOcorrencia = cargaOcorrencia,
                        Usuario = responsavel,
                        OrigemRegraOcorrencia = OrigemRegraOcorrencia.Delegada,
                        EtapaAutorizacaoOcorrencia = cargaOcorrencia.EtapaAutorizacaoOcorrencia,
                        Observacao = observacao,
                        Data = DateTime.Now,
                        NumeroAprovadores = 1,
                        NumeroReprovadores = 1,
                        PrioridadeAprovacao = (cargaOcorrencia.EtapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia) ? cargaOcorrencia.PrioridadeAprovacaoAtualEtapaAprovacao : cargaOcorrencia.PrioridadeAprovacaoAtualEtapaEmissao
                    };

                    cargaOcorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;
                    cargaOcorrencia.UltimoUsuarioDelegado = responsavel;
                    cargaOcorrencia.UltimoUsuarioDelegou = Usuario;

                    repositorioCargaOcorrenciaAutorizacao.Inserir(cargaOcorrenciaAutorizacao);
                    servicoAutorizacaoOcorrencia.NotificarOcorrenciaDelegada(cargaOcorrencia, responsavel, Usuario);
                    servicoAutorizacaoOcorrencia.NotificarOcorrenciaDelegadaAoAprovador(cargaOcorrencia, responsavel, Usuario);
                    repositorioCargaOcorrencia.Atualizar(cargaOcorrencia);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DelegadoPara, responsavel.Nome), unitOfWork);
                }
                else
                    mensagem = Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioSelecionadoJaEResponsavelPelaOcorrencia;
            }

            return mensagem;
        }

        /// <summary>
        /// Cria os CT-es da ocorrência e envia para autorização
        /// </summary>
        private void ExecutaProximoPassoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            string codigoCFOPIntegracao = string.Empty;
            if (ocorrencia.Carga != null && ocorrencia.Carga.CargaCTes != null && ocorrencia.Carga.CargaCTes.Count > 0)
            {
                if (ocorrencia != null && ocorrencia.TipoOcorrencia != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in ocorrencia.Carga.CargaCTes where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

                    if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                    else if (cte != null)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(codigoCFOPIntegracao))
                {
                    ocorrencia.CFOP = codigoCFOPIntegracao;
                    repCargaOcorrencia.Atualizar(ocorrencia);
                }
            }

            // Instancia Servicos
            Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            // Executa serviço
            serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork);
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao regra)
        {
            if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada)
                return "(Delegado)" + (!string.IsNullOrWhiteSpace(regra.Observacao) ? " - " + regra.Observacao : "");
            else if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Assumida)
                return "(Assumido)";
            else
                return regra.RegrasAutorizacaoOcorrencia?.Descricao;
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia");

            return caminho;
        }

        private string ObterNumeroNotasFiscaisCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork UnitOfWork)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXml = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(UnitOfWork);
                List<int> ListaNotas = repPedidoXml.BuscarNumeroNotasFiscaisPorCarga(carga.Codigo);
                return string.Join(", ", ListaNotas.Select(x => x.ToString()).ToArray());
            }
            else
                return "";
        }

        private string ObterNumeroPedidosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork UnitOfWork)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXml = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(UnitOfWork);
                List<string> ListaPedidos = repPedidoXml.BuscarNumeroPedidoEmbarcadorPorCarga(carga.Codigo);
                return string.Join(", ", ListaPedidos.Select(x => x).ToArray());
            }
            else
                return "";
        }

        private void DefinirCodigoAprovacaoMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias, string codigoAprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
            {
                ocorrencia.CodigoAprovacao = codigoAprovacao;
                repCargaOcorrencia.Atualizar(ocorrencia);
            }
        }

        #endregion
    }
}

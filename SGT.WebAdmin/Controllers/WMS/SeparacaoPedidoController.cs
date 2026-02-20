using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Diagnostics;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/SeparacaoPedido")]
    public class SeparacaoPedidoController : BaseController
    {
		#region Construtores

		public SeparacaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                //Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
                //Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao separacaoPedidoIntegracao = repSeparacaoPedidoIntegracao.BuscarPorCodigo(1);
                //Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec.EnviarPedidoSeparacaoOrtec(separacaoPedidoIntegracao, unitOfWork, TipoServicoMultisoftware);

                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 25, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, false);

                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido> separacoesPedidos = repSeparacaoPedido.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSeparacaoPedido.ContarConsulta(filtrosPesquisa));
                var lista = (from p in separacoesPedidos
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Situacao = p.Situacao.ObterDescricao(),
                                 Data = p.Data.ToString("dd/MM/yyyy HH:mm")
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
        public async Task<IActionResult> ObterPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos filtrosPesquisa = ObterFiltrosPesquisaPedidos();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repSeparacaoPedido.ConsultarPedidos(filtrosPesquisa, "Codigo", "desc");
                int quantidade = repSeparacaoPedido.ContarConsultaPedidos(filtrosPesquisa);

                var retorno = new
                {
                    Quantidade = quantidade,
                    Registros = (from obj in pedidos
                                 select new
                                 {
                                     obj.Codigo,
                                     DataCarregamentoPedido = obj.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? "",
                                     Filial = (obj.Filial?.Descricao ?? "") + (obj.ResponsavelRedespacho != null ? (" - " + obj.ResponsavelRedespacho.Descricao + "") : ""),
                                     obj.NumeroPedidoEmbarcador,
                                     Destino = obj.Destino?.DescricaoCidadeEstado ?? "",
                                     Remetente = obj.GrupoPessoas != null ? obj.GrupoPessoas.Descricao : obj.Remetente?.Descricao ?? "",
                                     Destinatario = obj.Destinatario?.Descricao ?? "",
                                     Peso = obj.PesoTotal.ToString("n4"),
                                     Agrupamento = obj.PalletAgrupamento ?? "",
                                     obj.DisponivelParaSeparacao,
                                     Reentrega = obj.ReentregaSolicitada,
                                     PrevisaoEntregaTeorica = obj.PrevisaoEntrega?.ToString("dd/MM/yyyy") ?? "",
                                     NumeroPedido = obj.Numero.ToString("D")
                                 }).ToList()
                };
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> CriarSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

                dynamic dynSelecao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Selecao"));

                int codigo = (int)dynSelecao.Codigo;

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido = new Dominio.Entidades.Embarcador.WMS.SeparacaoPedido();
                separacaoPedido.Numero = repSeparacaoPedido.BuscarProximoNumero();
                separacaoPedido.Data = DateTime.Now;
                separacaoPedido.Situacao = SituacaoSeparacaoPedido.Aberto;


                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;
                repSeparacaoPedido.Inserir(separacaoPedido, Auditado);

                if (!SalvarPedidos(separacaoPedido, dynSelecao.Pedidos, historicoObjeto, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor verifique os pedidos selecionados.");
                }

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    separacaoPedido.Codigo,
                    separacaoPedido.Situacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                double localEntrega = Request.GetDoubleParam("LocalEntrega");
                DateTime? dataExpedicao = Request.GetNullableDateTimeParam("DataExpedicao");
                bool salvarNotas = Request.GetBoolParam("SelecionarNotas");
                dynamic dyncodigosNotasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasSelecionadas"));

                List<Dominio.ObjetosDeValor.Embarcador.WMS.NotasSelecionadasParaIntegracao> codigosNotasSelecionadasParaIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.WMS.NotasSelecionadasParaIntegracao>();

                foreach (dynamic codigoNota in dyncodigosNotasSelecionadas)
                    codigosNotasSelecionadasParaIntegracao.Add( new Dominio.ObjetosDeValor.Embarcador.WMS.NotasSelecionadasParaIntegracao { CodigoPedido = codigoNota.CodigoPedido, CodigoNF = codigoNota.Codigo });

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido = repSeparacaoPedido.BuscarPorCodigo(codigo, true);

                if (localEntrega > 0)
                    separacaoPedido.LocalEntrega = repCliente.BuscarPorCPFCNPJ(localEntrega);

                if (separacaoPedido == null)
                    return new JsonpResult(false, true, "A separação pedido informada não foi localizada.");

                if (separacaoPedido.Situacao != SituacaoSeparacaoPedido.Aberto)
                    return new JsonpResult(false, true, "A atual situação da separação não permite sua finalização.");

                separacaoPedido.DataExpedicao = dataExpedicao;
                separacaoPedido.SelecionarNotasParaIntegracao = salvarNotas;

                unitOfWork.Start();

                separacaoPedido.Situacao = SituacaoSeparacaoPedido.AguardandoIntegracao;
                Servicos.Embarcador.WMS.SeparacaoPedido.AtualizarSituacaoNotasFiscais(separacaoPedido, SituacaoNotaFiscal.AgReentrega, unitOfWork);
                Servicos.Embarcador.WMS.SeparacaoPedido.GerarIntegracoes(separacaoPedido, codigosNotasSelecionadasParaIntegracao, unitOfWork);
                Servicos.Embarcador.WMS.SeparacaoPedido.GerarOcorrenciaReentregaSeparacao(separacaoPedido, this.Cliente, unitOfWork);
                repSeparacaoPedidoPedido.RemoverDisponibilidadePedidosSeparacao(separacaoPedido.Codigo);

                repSeparacaoPedido.Atualizar(separacaoPedido, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    separacaoPedido.Codigo,
                    separacaoPedido.Situacao
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar.");
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
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido = repSeparacaoPedido.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> separacaoPedidoIntegracao = repSeparacaoPedidoIntegracao.BuscarPorSeparacaoPedido(codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = await repositorioPedido.BuscarDadosPedidosPorSeparacaoPedido(separacaoPedido.Codigo);
                List<string> filiais = repFilial.BuscarListaCNPJAtivas();

                var retorno = new
                {
                    SeparacaoPedido = new
                    {
                        separacaoPedido.Codigo,
                        separacaoPedido.Numero,
                        separacaoPedido.Situacao,
                        EntregueNaoEntregaClienteFinal = separacaoPedido.LocalEntrega != null,
                        LocalEntrega = new { Codigo = separacaoPedido.LocalEntrega?.Codigo ?? 0, Descricao = separacaoPedido.LocalEntrega?.Descricao ?? "" },
                        Pedidos = (from pedido in pedidos select serPedido.ObterDetalhesPedido(pedido, filiais, null, ConfiguracaoEmbarcador, unitOfWork)).ToList(),
                        DataExpedicao = separacaoPedido.DataExpedicao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        SelecionarNotasParaIntegracao = separacaoPedido?.SelecionarNotasParaIntegracao ?? false,
                        NotasIntegradas = separacaoPedidoIntegracao != null ? string.Join(", ", (from obj in separacaoPedidoIntegracao select obj.XMLNotaFiscal.Numero).ToList()) : "",
                    }
                };
                stopwatch.Stop();
                Servicos.Log.TratarErro($"Tempo Novo: {stopwatch.ElapsedMilliseconds} mm");

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

        public async Task<IActionResult> CancelarSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido = repSeparacaoPedido.BuscarPorCodigo(codigo, true);

                if (separacaoPedido == null)
                    return new JsonpResult(false, true, "A separação informada não foi localizada");

                separacaoPedido.Situacao = SituacaoSeparacaoPedido.Cancelada;
                repSeparacaoPedido.Atualizar(separacaoPedido, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    separacaoPedido.Codigo,
                    separacaoPedido.Situacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterNotasPedidosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

                List<int> codigosPedidosSelecionados = ObterCodigosPedidosSelecionados();

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidos = repPedidoXMLNotaFiscal.BuscarPorPedidos(codigosPedidosSelecionados);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPedido", false);
                grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Nota Fiscal", "NumeroNotaFiscal", 25, Models.Grid.Align.left, true);

                var retorno =
                    (from obj in pedidos
                     select new
                     {
                         obj.Codigo,
                         CodigoPedido = obj.CargaPedido.Pedido.Codigo,
                         NumeroPedido = obj.CargaPedido.Pedido?.NumeroPedidoEmbarcador ?? "",
                         NumeroNotaFiscal = obj.XMLNotaFiscal?.Numero ?? 0
                     }).ToList().Distinct();

                grid.setarQuantidadeTotal(retorno.Count());
                grid.AdicionaRows(retorno);
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

        #endregion

        #region Métodos Integrações

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repSeparacaoPedido.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSeparacaoPedido.ContarBuscarArquivosPorIntegracao(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repSeparacaoPedido.BuscarIntegracaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao integracao = repSeparacaoPedidoIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.SeparacaoPedido.Situacao = SituacaoSeparacaoPedido.AguardandoIntegracao;

                repSeparacaoPedidoIntegracao.Atualizar(integracao);
                repSeparacaoPedido.Atualizar(integracao.SeparacaoPedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.SeparacaoPedido, "solicitou o reenvio da integração", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao integrar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaSeparacaoPedidoIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaSeparacaoPedidoIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> listaIntegracoes = repSeparacaoPedidoIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repSeparacaoPedidoIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Descricao = integracao.XMLNotaFiscal != null ? (integracao.XMLNotaFiscal.Numero.ToString() + " - " + integracao.XMLNotaFiscal.Serie) : "Pedidos Agrupados",
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var motivo = Request.Params("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "O motivo deve ser informado.");

                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao integracao = repSeparacaoPedidoIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = motivo.Trim();
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repSeparacaoPedidoIntegracao.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repSeparacaoPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repSeparacaoPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repSeparacaoPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repSeparacaoPedidoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Situacao = Request.GetNullableEnumParam<SituacaoSeparacaoPedido>("Situacao"),
                CodigoPedido = Request.GetIntParam("Pedido")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos ObterFiltrosPesquisaPedidos()
        {
            return new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos()
            {
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoPedido = Request.GetIntParam("Pedido"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoSeparacaoPedido = Request.GetIntParam("Codigo"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CpfCnpjRemetentes = Request.GetListParam<double>("Remetente"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjLocalExpedicao = Request.GetDoubleParam("LocalExpedicao"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                Inicio = Request.GetIntParam("Inicio"),
                Limite = Request.GetIntParam("Limite"),
                NumerosNotaFiscal = Request.GetListParam<int>("NumeroNotaFiscal"),
                SomentePedidosDeReentrega = Request.GetBoolParam("ExibirSomentePedidosDeReentrega"),
                SomentePedidosEmAberto = Request.GetBoolParam("ExibirSomentePedidosEmAberto"),
                CodigosPedidos = Request.GetListParam<int>("CodigosPedidos")
            };
        }

        private string ObterPropriedadeOrdenarPesquisaSeparacaoPedidoIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        private bool SalvarPedidos(Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido, dynamic dynPedidos, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> selecaoPedidoExiste = separacaoPedido.Pedidos != null ? separacaoPedido.Pedidos.ToList() : new List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>();
            bool contemRegistro = false;
            List<int> codigos = new List<int>();
            List<string> agrupamentos = new List<string>();

            foreach (dynamic dynPedido in dynPedidos)
            {
                codigos.Add((int)dynPedido.Codigo);
                string agrupamento = ((string)dynPedido.Agrupamento);

                if (!string.IsNullOrWhiteSpace(agrupamento))
                    if (!agrupamentos.Contains(agrupamento))
                        agrupamentos.Add(agrupamento);
            }

            List<int> codigosPedidosAgrupados = repPedido.BuscarPorAgrupamento(agrupamentos);

            foreach (int codigoPedidoAgrupado in codigosPedidosAgrupados)
                if (!codigos.Contains(codigoPedidoAgrupado))
                    codigos.Add(codigoPedidoAgrupado);

            //List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> selecaoPedidoOutros = repSeparacaoPedidoPedido.BuscarPorPedidos(codigos);
            //for (int i = 0; i < selecaoPedidoOutros.Count; i++)
            //{
            //    Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido selecaoPedido = selecaoPedidoOutros[i];
            //    if (selecaoPedido.SeparacaoPedido.Codigo != separacaoPedido.Codigo)
            //    {
            //        Servicos.Auditoria.Auditoria.Auditar(Auditado, selecaoPedido.SeparacaoPedido, null, "O pedido " + selecaoPedido.Pedido.Numero + " foi removido desta seleção e foi vinculado a separação " + separacaoPedido.Numero.ToString() + ".", unitOfWork);
            //        repSeparacaoPedidoPedido.Deletar(selecaoPedido);
            //    }
            //}

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAuditar = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            for (int i = 0; i < codigos.Count; i++)
            {
                int codigoPedido = codigos[i];

                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido selecaoPedido = (from obj in selecaoPedidoExiste where obj.Pedido.Codigo == codigoPedido select obj).FirstOrDefault();

                if (selecaoPedido == null)
                {
                    selecaoPedido = new Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido();
                    selecaoPedido.SeparacaoPedido = separacaoPedido;
                    selecaoPedido.Pedido = repPedido.BuscarPorCodigo(codigoPedido);

                    pedidosAuditar.Add(selecaoPedido.Pedido);
                    repSeparacaoPedidoPedido.Inserir(selecaoPedido, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
                contemRegistro = true;
            }

            for (int i = 0; i < selecaoPedidoExiste.Count; i++)
            {
                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido pedidoExiste = selecaoPedidoExiste[i];
                if (!codigos.Contains(pedidoExiste.Pedido.Codigo))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoExiste.SeparacaoPedido, null, "Removido pedido " + pedidoExiste.Pedido.Numero.ToString() + ".", unitOfWork);

                    repSeparacaoPedidoPedido.Deletar(pedidoExiste, historicoObjeto != null ? Auditado : null, historicoObjeto);
                }
            }

            return contemRegistro;
        }

        private List<int> ObterCodigosPedidosSelecionados()
        {
            dynamic jsonCodigosPedidosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCodigosPedidosSelecionados"));

            List<int> codigosPedidosSelecionados = new List<int>();

            foreach (dynamic codigoPedido in jsonCodigosPedidosSelecionados)
                codigosPedidosSelecionados.Add((int)codigoPedido);

            return codigosPedidosSelecionados;
        }
        #endregion
    }
}

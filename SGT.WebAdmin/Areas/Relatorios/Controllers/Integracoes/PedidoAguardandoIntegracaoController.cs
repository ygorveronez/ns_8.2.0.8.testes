using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Integracoes
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Integracoes/PedidoAguardandoIntegracao")]
    public class PedidoAguardandoIntegracaoController : BaseController
    {
        #region Construtores

        public PedidoAguardandoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            try
            {
                Models.Grid.Grid grid = GridPadrao();
                Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                int numeroItens = repPedidoAguardandoIntegracao.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(numeroItens);
                if (numeroItens > 0)
                {
                    var lista = repPedidoAguardandoIntegracao.Consultar(filtrosPesquisa, parametrosConsulta.PropriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.AdicionaRows((from o in lista
                                       select new
                                       {
                                           Codigo = o.Codigo,
                                           NumeroPedido = o.IdIntegracao,
                                           //NumeroPedidoCliente = repPedido.BuscarPorNumeroPedidoEmbarcadorOuNumeroPedidoCliente(o.IdIntegracao)?.CodigoPedidoCliente ?? "",
                                           Filial = o.Filial?.Descricao,
                                           DataCriacaoPedido = o.DataCriacaoPedido,
                                           CreatedAt = o.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                                           UltimaDataEmbarqueLista = o.UltimaDataEmbarqueLista.HasValue ? o.UltimaDataEmbarqueLista.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                           DescricaoSituacaoIntegracao = o.SituacaoIntegracao.ObterDescricao(),
                                           Informacao = o.Informacao,
                                           TipoIntegracao = o.TipoIntegracao?.ObterDescricao() ?? "",
                                           Situacao = o.SituacaoIntegracao,
                                           UltimotransId = 0
                                       }));
                }
                else
                {
                    grid.AdicionaRows(new List<dynamic> { });
                }

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            try
            {
                Models.Grid.Grid grid = GridPadrao();
                Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                int numeroItens = repPedidoAguardandoIntegracao.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(numeroItens);
                if (numeroItens > 0)
                {
                    var lista = repPedidoAguardandoIntegracao.Consultar(filtrosPesquisa, parametrosConsulta.PropriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.AdicionaRows((from o in lista
                                       select new
                                       {
                                           Codigo = o.Codigo,
                                           NumeroPedido = o.IdIntegracao,
                                           //NumeroPedidoCliente = repPedido.BuscarPorNumeroPedidoEmbarcadorOuNumeroPedidoCliente(o.IdIntegracao)?.CodigoPedidoCliente ?? "",
                                           Filial = o.Filial?.Descricao,
                                           CreatedAt = o.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                                           DataCriacaoPedido = o.DataCriacaoPedido,
                                           UltimaDataEmbarqueLista = o.UltimaDataEmbarqueLista.HasValue ? o.UltimaDataEmbarqueLista.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                           DescricaoSituacaoIntegracao = o.SituacaoIntegracao.ObterDescricao(),
                                           Informacao = o.Informacao,
                                           TipoIntegracao = o.TipoIntegracao?.ObterDescricao() ?? "",
                                           UltimotransId = 0
                                       }));
                }
                else
                {
                    grid.AdicionaRows(new List<dynamic> { });
                }

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                //Dominio.Entidades.Embarcador.Integracao
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao integracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarNumeroNFChaveDeAcesso()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string codigoPedidoIntegracao = Request.GetStringParam("CodigoPedidoIntegracao");
                string NumeroPedidoInformacao = Request.GetStringParam("NumeroPedidoInformacao");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NF", "NF", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Chave de Acesso", "ChaveAcesso", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorNumeroPedidoEmbarcadorOuNumeroPedidoCliente(NumeroPedidoInformacao);
                int numeroItens = pedido?.NotasFiscais?.Count ?? 0;

                grid.setarQuantidadeTotal(numeroItens);
                if (numeroItens > 0)
                {
                    grid.AdicionaRows((from obj in pedido?.NotasFiscais?.OrderByDescending(o => o.Numero).Skip(grid.inicio).Take(grid.limite)
                                       select new
                                       {
                                           obj.Codigo,
                                           NF = obj.Numero,
                                           ChaveAcesso = obj.Chave
                                       }).ToList());
                }
                else
                    grid.AdicionaRows(new List<dynamic> { });

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao integracao = repPedidoAguardandoIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração Pedidos " + integracao.Codigo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao integracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                if (integracao != null)
                {
                    if (integracao.TipoIntegracao == TipoIntegracao.VTEX && (integracao.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao || integracao.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ErroGenerico))
                    {
                        var serIntegracaoVtex = new Servicos.Embarcador.Integracao.VTEX.IntegracaoVtexBuscaPedidos(unidadeDeTrabalho, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao);
                        Repositorio.Embarcador.Integracao.IntegracaoDocas repIntegracaoDocas = new Repositorio.Embarcador.Integracao.IntegracaoDocas(unidadeDeTrabalho);
                        List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas = repIntegracaoDocas.BuscarTodos();
                        serIntegracaoVtex.IntegrarPedidoAguardandoIntegracao(integracao.Codigo, configuracaoEmbarcador, docas);
                    }
                    else if (integracao.TipoIntegracao == TipoIntegracao.Emillenium && (integracao.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga || integracao.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao))
                    {
                        integracao.SituacaoIntegracao = integracao.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga ? SituacaoPedidoAguardandoIntegracao.AgGerarCarga : SituacaoPedidoAguardandoIntegracao.AgIntegracao;
                        integracao.Informacao = "";
                        repPedidoAguardandoIntegracao.Atualizar(integracao);
                    }
                    else
                        return new JsonpResult(null, true, "Só é possível reenviar integrações com falhas.");
                }
                else
                    return new JsonpResult(false, "Registro não encontrado");



                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarNFesEmillenium()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao integracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(codigo);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);


                Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium svcIntegracao = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unidadeDeTrabalho, TipoServicoMultisoftware, unidadeDeTrabalho.StringConexao);


                if (integracao != null && !string.IsNullOrWhiteSpace(integracao.IdIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPedidoPorNumeroPedidoCliente(integracao.IdIntegracao);

                    if (pedido == null)
                        return new JsonpResult(false, true, "Pedido não localizado");

                    unidadeDeTrabalho.Start();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento retorno = svcIntegracao.BuscarNotasPorPedidosPendentes(new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido });

                    if (retorno.status)
                    {
                        integracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                        integracao.Informacao = retorno.mensagem;

                        repPedidoAguardandoIntegracao.Atualizar(integracao);
                    }

                    unidadeDeTrabalho.CommitChanges();

                }
                else
                    return new JsonpResult(false, "Registro não encontrado");



                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultaManualNota()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var serIntegracaoEmillenium = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmillenium(unidadeDeTrabalho, TipoServicoMultisoftware, _conexao.StringConexao);

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaEmillenium) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioEmillenium))
                    return new JsonpResult(false, "Usuario e senha Emillenium não encontrados na configuração de integração.");

                int transId = Request.GetIntParam("TransIDBusca");
                // int top = Request.GetIntParam("top");

                Models.Grid.Grid grid = GridPadrao();

                if (transId > 0)
                {
                    var lista = serIntegracaoEmillenium.BuscarNotasPedidos(transId);
                    if (lista != null && lista.ListapedidoAguardandoIntegracaoNotaretorno.Count > 0)
                    {
                        grid.AdicionaRows((from o in lista.ListapedidoAguardandoIntegracaoNotaretorno
                                           select new
                                           {
                                               Codigo = o.Codigo,
                                               NumeroPedido = o.IdIntegracao,
                                               Filial = o.Filial?.Descricao,
                                               DataCriacaoPedido = o.DataCriacaoPedido,
                                               CreatedAt = o.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                                               UltimaDataEmbarqueLista = o.UltimaDataEmbarqueLista.HasValue ? o.UltimaDataEmbarqueLista.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                               DescricaoSituacaoIntegracao = o.SituacaoIntegracao.ObterDescricao(),
                                               Informacao = o.Informacao,
                                               TipoIntegracao = o.TipoIntegracao?.ObterDescricao() ?? "",
                                               Situacao = o.SituacaoIntegracao,
                                               UltimotransId = lista.ultimaTransId
                                           }));


                        grid.setarQuantidadeTotal(lista.ListapedidoAguardandoIntegracaoNotaretorno.Count);
                    }
                    else
                        grid.AdicionaRows(new List<dynamic> { });
                }
                else
                    return new JsonpResult(false, "Trans ID não informado.");


                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("UltimotransId", false);
            grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 3, Models.Grid.Align.center);
            //grid.AdicionarCabecalho("Número Pedido Cliente", "NumeroPedidoCliente", 3, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Tipo de integracao", "TipoIntegracao", 4, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Filial", "Filial", 4, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Integração", "CreatedAt", 4, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Criação Pedido", "DataCriacaoPedido", 4, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Última Data Embarque Lista", "UltimaDataEmbarqueLista", 4, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacaoIntegracao", 5, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Informação", "Informacao", 14, Models.Grid.Align.center, false, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao()
            {
                Pesquisa = Request.GetStringParam("Pesquisa"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoPedidoAguardandoIntegracao>("SituacaoIntegracao"),
                TipoIntegracao = Request.GetNullableEnumParam<TipoIntegracao>("TipoIntegracao"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataEmbarqueInicio = Request.GetDateTimeParam("DataEmbarqueInicio"),
                DataEmbarqueFim = Request.GetDateTimeParam("DataEmbarqueFim"),
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}

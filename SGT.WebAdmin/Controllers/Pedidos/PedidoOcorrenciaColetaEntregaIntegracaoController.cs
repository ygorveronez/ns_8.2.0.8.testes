using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Pedidos/PedidoOcorrenciaColetaEntregaIntegracao")]
    public class PedidoOcorrenciaColetaEntregaIntegracaoController : BaseController
    {
		#region Construtores

		public PedidoOcorrenciaColetaEntregaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = ObterGridPesquisa(unitOfWork);
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                if (pedidoOcorrenciaColetaEntregaIntegracao == null) return new JsonpResult(false, "Integração não encontrada.");

                Models.Grid.Grid grid = new Models.Grid.Grid() { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Chave", "Chave", 30, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data de emissão", "DataEmissao", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Peso", "Peso", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);
                var listaNotas = (
                    from row in notasFiscais
                    select new
                    {
                        row.Codigo,
                        row.Numero,
                        row.Serie,
                        row.Chave,
                        DataEmissao = row.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                        Peso = row.Peso.ToString("n"),
                        Valor = row.Valor.ToString("f2")
                    }
                ).ToList();
                grid.AdicionaRows(listaNotas);
                grid.setarQuantidadeTotal(listaNotas.Count());

                return new JsonpResult(new
                {
                    pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Codigo,
                    pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.CodigoCargaEmbarcador,
                    pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador,
                    Origem = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Origem?.DescricaoCidadeEstado,
                    Destino = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Destino?.DescricaoCidadeEstado,
                    Remetente = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Remetente?.Descricao,
                    Tomador = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Tomador?.Descricao,
                    Destinatario = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Destinatario?.Descricao,
                    Transportador = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa?.Descricao,
                    Veiculo = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.VeiculoTracao?.Placa_Formatada,
                    Motorista = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.Pedido.NomeMotoristas,
                    TipoOcorrencia = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao,
                    DataOcorrencia = pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                    GridNotasFiscais = grid
                });
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                if (pedidoOcorrenciaColetaEntregaIntegracao == null) return new JsonpResult(false, "Integração não encontrada.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left);

                var arquivosRetornar = (
                    from arquivoTransacao in pedidoOcorrenciaColetaEntregaIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosRetornar);
                grid.setarQuantidadeTotal(arquivosRetornar.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico de integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();


                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                if (pedidoOcorrenciaColetaEntregaIntegracao == null) return new JsonpResult(false, "Integração não encontrada.");
                if (pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) return new JsonpResult(false, "Apenas integrações com problema podem ser reenviadas.");

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                pedidoOcorrenciaColetaEntregaIntegracao.Initialize();

                Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao servicoPedidoOcorrenciaColetaEntregaIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork, clienteURLAcesso.URLAcesso);
                servicoPedidoOcorrenciaColetaEntregaIntegracao.ProcessarIntegracaoPendente(pedidoOcorrenciaColetaEntregaIntegracao.Codigo, TipoServicoMultisoftware, unitOfWorkAdmin, clienteURLAcesso.Cliente, clienteURLAcesso.Cliente.Codigo);

                // Auditoria
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    Usuario = base.Usuario
                };
                Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoOcorrenciaColetaEntregaIntegracao, pedidoOcorrenciaColetaEntregaIntegracao.GetChanges(), "Solicitou o reenvio da integração.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodasIntegracoesComFalha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repositorioPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");

                if (!dataInicial.HasValue || !dataFinal.HasValue)
                    return new JsonpResult(false, "É preciso informar os filtros de data e a diferença entre as datas precisa ser de, no máximo, 1 dia.");

                if ((dataFinal.Value.Date - dataInicial.Value.Date).TotalDays > 1)
                    return new JsonpResult(false, "As datas precisam ter, no máximo, 1 dia de diferença.");

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> listaOcorrenciasComFalha = repositorioPedidoOcorrenciaColetaEntregaIntegracao.BuscarIntegracoesPorSituacaoEData(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, dataInicial.Value, dataFinal.Value);

                if (listaOcorrenciasComFalha.Count == 0)
                    return new JsonpResult(false, "Nenhuma integração com falha foi encontrada.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao in listaOcorrenciasComFalha)
                {
                    pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repositorioPedidoOcorrenciaColetaEntregaIntegracao.Atualizar(pedidoOcorrenciaColetaEntregaIntegracao, Auditado, null, "Clicou em ReenviarTodasIntegracoesComFalha");
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar as integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigoArquivo(codigo);
                if (pedidoOcorrenciaColetaEntregaIntegracao == null) return new JsonpResult(false, "Histórico de integração não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = pedidoOcorrenciaColetaEntregaIntegracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();
                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });
                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {pedidoOcorrenciaColetaEntregaIntegracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadEdiPedidoOcorrenciaColetaEntregaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(codigo, false);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

                if (integracao == null || integracao.LayoutEDI == null) return new JsonpResult(false, "Registro não encontrada.");

                string numero = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador;
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao.LayoutEDI, numero);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                string NumerosNotasFiscais = notasFiscais != null && notasFiscais.Count > 0 ? String.Join("", (from notas in notasFiscais select notas.Numero)) : "";

                Dominio.Entidades.Empresa empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa;

                if (configuracaoGeralCarga.UtilizarEmpresaFilialEmissoraNoArquivoEDI && integracao.PedidoOcorrenciaColetaEntrega.Carga?.EmpresaFilialEmissora != null)
                    empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga.EmpresaFilialEmissora;

                if (empresa == null)
                    empresa = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa;

                MemoryStream arquivoEDI = Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao.ObterArquivoEDIOcoren(integracao, empresa, notasFiscais, unitOfWork);

                StreamReader readerProcessados = new StreamReader(arquivoEDI);
                byte[] retorno = arquivoEDI.ToArray();
                arquivoEDI.Dispose();
                return Arquivo(retorno, "text/txt", nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("LayoutEDI", false);
            grid.AdicionarCabecalho("CodigoSituacaoIntegracao", false);
            grid.AdicionarCabecalho("CodigoTipoDeOcorrencia", false);
            grid.AdicionarCabecalho("CodigoTipoIntegracao", false);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de ocorrência", "DescricaoTipoDeOcorrencia", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "TransportadorFormatado", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "TomadorFormatado", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integradora", "DescricaoTipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataIntegracaoFormatada", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "SituacaoIntegracaoFormatada", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 20, Models.Grid.Align.left, false);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
            int totalRegistros = repPedidoOcorrenciaColetaEntregaIntegracao.ContarConsulta(filtroPesquisa);
            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> lista = repPedidoOcorrenciaColetaEntregaIntegracao.Consultar(filtroPesquisa, false, parametrosConsulta);

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                CodigoTipoDeOcorrencia = Request.GetIntParam("TipoDeOcorrencia"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoTomador = Request.GetDoubleParam("Tomador"),
                CodigoTipoIntegracao = Request.GetIntParam("TipoIntegracao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TomadorFormatado")
                return "ClienteRemetenteNome";

            if (propriedadeOrdenar == "TransportadorFormatado")
                return "CargaRazaoSocial";

            if (propriedadeOrdenar == "DataIntegracaoFormatada")
                return "DataIntegracao";


            return propriedadeOrdenar;
        }


        #endregion
    }
}

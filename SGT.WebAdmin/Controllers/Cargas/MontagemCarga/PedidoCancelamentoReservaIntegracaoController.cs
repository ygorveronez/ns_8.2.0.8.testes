using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/PedidoCancelamentoReservaIntegracao")]
    public class PedidoCancelamentoReservaIntegracaoController : BaseController
    {
		#region Construtores

		public PedidoCancelamentoReservaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao PedidoCancelamentoReservaIntegracao = repCancelamentoReservaIntegracao.BuscarPorCodigo(codigo);

                PedidoCancelamentoReservaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, PedidoCancelamentoReservaIntegracao, null, Localization.Resources.Cargas.MontagemCargaMapa.ReenvioCancelamentoReservaIntegracao, unidadeTrabalho);

                repCancelamentoReservaIntegracao.Atualizar(PedidoCancelamentoReservaIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao integracao = repCancelamentoReservaIntegracao.BuscarPorCodigo(codigo);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroPedido").Nome(Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Emitente").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Emitente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Integradora").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Integradora).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Data).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Situacao).Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MensagemRetorno").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Retorno).Tamanho(30).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);

            // Dados do filtro
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
            string numeroPedido = Request.GetStringParam("NumeroPedido");
            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            double emitente = Request.GetDoubleParam("Emitente");
            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao > listaGrid = repCancelamentoReservaIntegracao.Consultar(dataInicio, dataFim, situacao, numeroPedido, codigoEmpresa, emitente, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCancelamentoReservaIntegracao.ContarConsulta(dataInicio, dataFim, situacao, numeroPedido, codigoEmpresa, emitente);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            NumeroPedido = obj.Pedido?.NumeroPedidoEmbarcador,
                            Emitente = obj.Pedido?.Filial.Descricao,
                            Integradora = obj.TipoIntegracao.Descricao,
                            Data = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            MensagemRetorno = obj.ProblemaIntegracao
                        };

            return lista.ToList();
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao integracao = repCancelamentoReservaIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.MontagemCargaMapa.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.MontagemCargaMapa.NaoHaRegistrosDeArquivosSalvosParaEsteHistoricoDeConsulta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", Localization.Resources.Cargas.MontagemCargaMapa.ArquivoIntegracaoCarregamento + " " + integracao.Pedido?.NumeroPedidoEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoRealizarDownloadDoArquivoDeIntegracao);
            }
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Situacao") propOrdenar = "SituacaoIntegracao";
            else if (propOrdenar == "NumeroPedido") propOrdenar = "Pedido.NumeroPedidoEmbarcador";
            else if (propOrdenar == "Integradora") propOrdenar = "TipoIntegracao.Descricao";
            else if (propOrdenar == "Emitente") propOrdenar = "Pedido.Filial.Descricao";
            else if (propOrdenar == "Data") propOrdenar = "DataIntegracao";
            else if (propOrdenar == "MensagemRetorno") propOrdenar = "ProblemaIntegracao";
        }

        #endregion
    }
}

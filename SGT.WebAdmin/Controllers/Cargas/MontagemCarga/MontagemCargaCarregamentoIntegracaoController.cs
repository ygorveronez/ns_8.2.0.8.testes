using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/MontagemCargaCarregamentoIntegracao")]
    public class MontagemCargaCarregamentoIntegracaoController : BaseController
    {
		#region Construtores

		public MontagemCargaCarregamentoIntegracaoController(Conexao conexao) : base(conexao) { }

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
        
        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, 0, 0, unitOfWork);
                
                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao CarregamentoIntegracao = repCarregamentoIntegracao.BuscarPorCodigo(codigo);

                CarregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, CarregamentoIntegracao, null, Localization.Resources.Cargas.MontagemCargaMapa.ReenviouIntegracao, unidadeTrabalho);

                repCarregamentoIntegracao.Atualizar(CarregamentoIntegracao);

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

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao integracao = repCarregamentoIntegracao.BuscarPorCodigo(codigo);
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
            grid.Prop("NumeroCarregamento").Nome(Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Protocolo").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Protocolo).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Carga").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Carga).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Emitente").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Emitente).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Destinatario).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("TipoCarga").Nome(Localization.Resources.Cargas.MontagemCargaMapa.TipoCarga).Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Integradora").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Integradora).Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("NumeroExp").Nome(Localization.Resources.Cargas.MontagemCargaMapa.NumeroEXP).Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("PrimeiroEnvio").Nome(Localization.Resources.Cargas.MontagemCargaMapa.PrimeiroEnvio).Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("UltimoEnvio").Nome(Localization.Resources.Cargas.MontagemCargaMapa.UltimoEnvio).Tamanho(15).Align(Models.Grid.Align.left);    
            grid.Prop("NumeroEnvios").Nome(Localization.Resources.Cargas.MontagemCargaMapa.NumeroEnvios).Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Data").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Data).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Situacao).Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MensagemRetorno").Nome(Localization.Resources.Cargas.MontagemCargaMapa.Retorno).Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            // Dados do filtro
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
            string numeroCarregamento = Request.GetStringParam("NumeroCarregamento");
            string carga = Request.GetStringParam("Carga");
            int protocolo = Request.GetIntParam("Protocolo");
            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            double emitente = Request.GetDoubleParam("Emitente");
            double destinatario = Request.GetDoubleParam("Destinatario");
            string numeroExp = Request.GetStringParam("NumeroExp");

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao> listaGrid = repCarregamentoIntegracao.Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente, carga, protocolo, numeroExp, destinatario, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCarregamentoIntegracao.ContarConsulta(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente, carga, protocolo, numeroExp, destinatario);

            List<int> codigosCarregamentos = listaGrid.Select(obj => obj.Carregamento.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = codigosCarregamentos.Count > 0 ? repositorioCargaPedido.BuscarPorCarregamentos(codigosCarregamentos) : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var lista = from obj in listaGrid select FormatarObjetoPesquisa(obj, (from cp in cargaPedidos where cp.Carga.Carregamento.Codigo == obj.Carregamento.Codigo select cp).ToList(), unitOfWork);

            return lista.ToList();
        }

        private dynamic FormatarObjetoPesquisa(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargasPedido.FirstOrDefault()?.Carga;

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo primeiraIntegracao = carregamento.ArquivosTransacao.OrderBy(obj => obj.Data).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo ultimaIntegracao = carregamento.ArquivosTransacao.OrderByDescending(obj => obj.Data).FirstOrDefault();
            //int numeroEnvios = carregamento.ArquivosTransacao.Count;
            
            return new
            {
                carregamento.Codigo,
                carregamento.Carregamento?.NumeroCarregamento,
                NumeroExp = string.Join(", ", (from cp in cargasPedido select cp.Pedido.NumeroEXP).Distinct().ToList()),
                Protocolo = carga?.Protocolo ?? 0,
                Carga = (carga?.CargaFechada ?? false) ? carga?.CodigoCargaEmbarcador : string.Empty,
                Emitente = string.Join(", ", carregamento.Carregamento.Pedidos.Select(a => a.Pedido.Remetente.Descricao).ToList()),
                Destinatario = string.Join(", ", carregamento.Carregamento.Pedidos.Select(a => a.Pedido.Destinatario.Descricao).ToList()),
                TipoCarga = carregamento.Carregamento?.TipoDeCarga?.Descricao,
                Integradora = carregamento.TipoIntegracao.Descricao,
                Data = carregamento.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                Situacao = carregamento.DescricaoSituacaoIntegracao,
                MensagemRetorno = carregamento.ProblemaIntegracao,
                NumeroEnvios = carregamento.NumeroTentativas,
                PrimeiroEnvio = primeiraIntegracao?.Data.ToString("dd/MM/yyyy HH:mm"),
                UltimoEnvio = ultimaIntegracao?.Data.ToString("dd/MM/yyyy HH:mm")
            };
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.MontagemCargaMapa.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.MontagemCargaMapa.NaoHaRegistrosDeArquivosSalvosParaEsteHistoricoDeConsulta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", Localization.Resources.Cargas.MontagemCargaMapa.ArquivoIntegracaoCarregamento + " " + integracao.Carregamento?.NumeroCarregamento + ".zip");
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
        }
        #endregion
    }
}

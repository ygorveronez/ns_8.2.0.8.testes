using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/CargaEntregaIntegracao")]
    public class CargaEntregaIntegracaoController : BaseController
    {
		#region Construtores

		public CargaEntregaIntegracaoController(Conexao conexao) : base(conexao) { }

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCargaEntregaIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao cargaEntregaIntegracao = repCargaEntregaIntegracao.BuscarPorCodigo(codigo, false);

                cargaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntregaIntegracao, null, "Reenviou integração", unidadeTrabalho);

                repCargaEntregaIntegracao.Atualizar(cargaEntregaIntegracao);

                //para testar integracao
                Servicos.Embarcador.Carga.ControleEntrega.EntregaIntegracao.VerificarIntegracoesPendentes(unidadeTrabalho, TipoServicoMultisoftware);

                //return new JsonpResult(false, true, "Nenhum registro encontrado");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar reenviar o arquivo para integração.");
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

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCargaEntregaIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao integracao = repCargaEntregaIntegracao.BuscarPorCodigo(codigo, false);
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
            grid.Prop("Carga").Nome("Carga").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Integradora").Nome("Integradora").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MensagemRetorno").Nome("Retorno").Tamanho(30).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unitOfWork);

            // Dados do filtro
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
            string codigoCarga = Request.GetStringParam("NumeroCarga");
            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            double emitente = Request.GetDoubleParam("Emitente");
            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao> listaIntegracoes = repCarregamentoIntegracao.Consultar(dataInicio, dataFim, situacao, codigoCarga, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCarregamentoIntegracao.ContarConsulta(dataInicio, dataFim, situacao, codigoCarga);

            var lista = from obj in listaIntegracoes
                        select new
                        {
                            obj.Codigo,
                            Carga = obj.Carga.CodigoCargaEmbarcador,
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

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivo Integração Carregamento " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
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

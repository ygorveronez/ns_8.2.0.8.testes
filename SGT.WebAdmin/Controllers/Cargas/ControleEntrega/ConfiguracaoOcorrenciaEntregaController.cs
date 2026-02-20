using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/ConfiguracaoOcorrenciaEntrega")]
    public class ConfiguracaoOcorrenciaEntregaController : BaseController
    {
		#region Construtores

		public ConfiguracaoOcorrenciaEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega();

                PreencherEntidade(configuracaoOcorrenciaEntrega, unitOfWork);

                if (configuracaoOcorrenciaEntrega.EventoColetaEntrega == EventoColetaEntrega.RecalculoPrevisao && configuracaoOcorrenciaEntrega.TempoRecalculo < 30)
                    return new JsonpResult(false, true, "Tempo de recalculo precisa ser maior que 30 minutos.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaEntrega.ValidarSeExiste(configuracaoOcorrenciaEntrega.AlvoDoPedido, configuracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega, configuracaoOcorrenciaEntrega.EventoColetaEntrega, configuracaoOcorrenciaEntrega.Reentrega, configuracaoOcorrenciaEntrega.TipoOperacao, configuracaoOcorrenciaEntrega.TipoDeOcorrencia);

                if (configuracaoOcorrenciaExiste == null)
                {
                    unitOfWork.Start();

                    repConfiguracaoOcorrenciaEntrega.Inserir(configuracaoOcorrenciaEntrega, Auditado);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuração criada para os parametros informados.");
                }

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = repConfiguracaoOcorrenciaEntrega.BuscarPorCodigo(codigo, true);

                if (configuracaoOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(configuracaoOcorrenciaEntrega, unitOfWork);

                if (configuracaoOcorrenciaEntrega.EventoColetaEntrega == EventoColetaEntrega.RecalculoPrevisao && configuracaoOcorrenciaEntrega.TempoRecalculo < 30)
                    return new JsonpResult(false, true, "Tempo de recalculo precisa ser maior que 30 minutos.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaEntrega.ValidarSeExiste(configuracaoOcorrenciaEntrega.AlvoDoPedido, configuracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega, configuracaoOcorrenciaEntrega.EventoColetaEntrega, configuracaoOcorrenciaEntrega.Reentrega, configuracaoOcorrenciaEntrega.TipoOperacao, configuracaoOcorrenciaEntrega.TipoDeOcorrencia);

                if (configuracaoOcorrenciaExiste == null || configuracaoOcorrenciaExiste.Codigo == configuracaoOcorrenciaEntrega.Codigo)
                {
                    unitOfWork.Start();
                    repConfiguracaoOcorrenciaEntrega.Atualizar(configuracaoOcorrenciaEntrega, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuração criada para os parametros informados.");
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = repConfiguracaoOcorrenciaEntrega.BuscarPorCodigo(codigo, false);

                if (configuracaoOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    configuracaoOcorrenciaEntrega.Codigo,
                    configuracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega,
                    configuracaoOcorrenciaEntrega.AlvoDoPedido,
                    configuracaoOcorrenciaEntrega.EventoColetaEntrega,
                    configuracaoOcorrenciaEntrega.TempoRecalculo,
                    TipoOcorrencia = new
                    {
                        configuracaoOcorrenciaEntrega.TipoDeOcorrencia.Codigo,
                        configuracaoOcorrenciaEntrega.TipoDeOcorrencia.Descricao
                    },
                    TipoOperacao = configuracaoOcorrenciaEntrega.TipoOperacao != null ? new
                    {
                        configuracaoOcorrenciaEntrega.TipoOperacao.Codigo,
                        configuracaoOcorrenciaEntrega.TipoOperacao.Descricao
                    } : null
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = repConfiguracaoOcorrenciaEntrega.BuscarPorCodigo(codigo, true);

                if (configuracaoOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();


                repConfiguracaoOcorrenciaEntrega.Deletar(configuracaoOcorrenciaEntrega, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            configuracaoOcorrenciaEntrega.AlvoDoPedido = Request.GetBoolParam("AlvoDoPedido");
            configuracaoOcorrenciaEntrega.TipoDeOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoOcorrencia"));
            configuracaoOcorrenciaEntrega.TipoAplicacaoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");
            configuracaoOcorrenciaEntrega.EventoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega>("EventoColetaEntrega");
            configuracaoOcorrenciaEntrega.TempoRecalculo = Request.GetIntParam("TempoRecalculo");
            configuracaoOcorrenciaEntrega.Reentrega = Request.GetBoolParam("Reentrega");

            if (Request.GetIntParam("TipoOperacao") > 0)
                configuracaoOcorrenciaEntrega.TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };


                int tipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega>("EventoColetaEntrega");


                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Evento", "EventoColetaEntrega", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Aplicação", "TipoAplicacaoColetaEntrega", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Ocorrência", "TipoOcorrencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Alvo do Pedido", "AlvoDoPedido", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Reentrega", "Reentrega", 5, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> listaConfiguracaoOcorrenciaEntrega = repConfiguracaoOcorrenciaEntrega.Consultar(tipoOcorrencia, tipoAplicacaoColetaEntrega, eventoColetaEntrega, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoOcorrenciaEntrega.ContarConsulta(tipoOcorrencia, tipoAplicacaoColetaEntrega, eventoColetaEntrega);

                var retorno = listaConfiguracaoOcorrenciaEntrega.Select(motivo => new
                {
                    motivo.Codigo,
                    EventoColetaEntrega = motivo.EventoColetaEntrega.ObterDescricao(),
                    TipoAplicacaoColetaEntrega = motivo.TipoAplicacaoColetaEntrega.ObterDescricao(),
                    AlvoDoPedido = motivo.AlvoDoPedido ? "Sim" : "Não",
                    TipoOcorrencia = motivo.TipoDeOcorrencia.Descricao,
                    Reentrega = motivo.Reentrega ? "Sim" : "Não",
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            if (propriedadeOrdenar == "TipoOcorrencia")
                return "TipoDeOcorrencia.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

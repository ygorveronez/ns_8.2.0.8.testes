using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/ConfiguracaoOcorrenciaPedido")]
    public class ConfiguracaoOcorrenciaPedidoController : BaseController
    {
		#region Construtores

		public ConfiguracaoOcorrenciaPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repConfiguracaoOcorrenciaPedidos = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedidos = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido();

                PreencherEntidade(configuracaoOcorrenciaPedidos, unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaExiste = null;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                if (configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento)
                    configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaPedidos.BuscarPorTipoOcorrenciaEEvento(configuracaoOcorrenciaPedidos.TipoDeOcorrencia.Codigo, configuracaoOcorrenciaPedidos.EventoColetaEntrega);
                else
                    configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaPedidos.ValidarSeExiste(configuracaoOcorrenciaPedidos.EventoColetaEntrega);

                if (configuracaoOcorrenciaExiste != null)
                    throw new ControllerException("Já existe uma configuração criada para os parametros informados.");

                unitOfWork.Start();
                repConfiguracaoOcorrenciaPedidos.Inserir(configuracaoOcorrenciaPedidos, Auditado);
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

                Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido = repConfiguracaoOcorrenciaPedido.BuscarPorCodigo(codigo, true);

                if (configuracaoOcorrenciaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(configuracaoOcorrenciaPedido, unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaExiste = null;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                if (configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento)
                    configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaPedido.BuscarPorTipoOcorrenciaEEvento(configuracaoOcorrenciaPedido.TipoDeOcorrencia.Codigo, configuracaoOcorrenciaPedido.EventoColetaEntrega);
                else
                    configuracaoOcorrenciaExiste = repConfiguracaoOcorrenciaPedido.ValidarSeExiste(configuracaoOcorrenciaPedido.EventoColetaEntrega);

                if (!(configuracaoOcorrenciaExiste == null || configuracaoOcorrenciaExiste.Codigo == configuracaoOcorrenciaPedido.Codigo))
                    throw new ControllerException("Já existe uma configuração criada para os parametros informados.");

                unitOfWork.Start();
                repConfiguracaoOcorrenciaPedido.Atualizar(configuracaoOcorrenciaPedido, Auditado);
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

                Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido = repConfiguracaoOcorrenciaPedido.BuscarPorCodigo(codigo, false);

                if (configuracaoOcorrenciaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    configuracaoOcorrenciaPedido.Codigo,
                    EventoColetaPedido = configuracaoOcorrenciaPedido.EventoColetaEntrega,
                    TipoOcorrencia = new
                    {
                        configuracaoOcorrenciaPedido.TipoDeOcorrencia.Codigo,
                        configuracaoOcorrenciaPedido.TipoDeOcorrencia.Descricao
                    }
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

                Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido = repConfiguracaoOcorrenciaPedido.BuscarPorCodigo(codigo, true);

                if (configuracaoOcorrenciaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();


                repConfiguracaoOcorrenciaPedido.Deletar(configuracaoOcorrenciaPedido, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            configuracaoOcorrenciaPedido.TipoDeOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoOcorrencia"));
            configuracaoOcorrenciaPedido.EventoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega>("EventoColetaPedido");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };


                int tipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega>("EventoColetaPedido");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Evento", "EventoColetaEntrega", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Ocorrência", "TipoOcorrencia", 20, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> listaConfiguracaoOcorrenciaPedido = repConfiguracaoOcorrenciaPedido.Consultar(tipoOcorrencia, eventoColetaEntrega, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoOcorrenciaPedido.ContarConsulta(tipoOcorrencia, eventoColetaEntrega);

                var retorno = listaConfiguracaoOcorrenciaPedido.Select(motivo => new
                {
                    motivo.Codigo,
                    EventoColetaEntrega = motivo.EventoColetaEntrega.ObterDescricao(),
                    TipoOcorrencia = motivo.TipoDeOcorrencia.Descricao,
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

            return propriedadeOrdenar;
        }

        #endregion
    }
}

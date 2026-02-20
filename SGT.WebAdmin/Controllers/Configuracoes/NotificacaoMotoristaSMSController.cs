using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Embarcador.Configuracoes;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/NotificacaoMotoristaSMS")]
    public class NotificacaoMotoristaSMSController : BaseController
    {
        #region Construtores

        public NotificacaoMotoristaSMSController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = new Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS();

                PreencherDados(notificacaoMotoristaSMS);

                await repNotificacaoMotoristaSMS.InserirAsync(notificacaoMotoristaSMS, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = await repNotificacaoMotoristaSMS.BuscarPorCodigoAsync(codigo);

                PreencherDados(notificacaoMotoristaSMS);

                if (notificacaoMotoristaSMS == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await unitOfWork.StartAsync(cancellationToken);

                await repNotificacaoMotoristaSMS.AtualizarAsync(notificacaoMotoristaSMS, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = await repNotificacaoMotoristaSMS.BuscarPorCodigoAsync(codigo);

                if (notificacaoMotoristaSMS == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    notificacaoMotoristaSMS.Codigo,
                    notificacaoMotoristaSMS.Descricao,
                    notificacaoMotoristaSMS.Mensagem,
                    notificacaoMotoristaSMS.TipoEnvio,
                    notificacaoMotoristaSMS.Ativo,
                    notificacaoMotoristaSMS.NotificacaoSuperApp
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = await repNotificacaoMotoristaSMS.BuscarPorCodigoAsync(codigo, auditavel: true);

                if (notificacaoMotoristaSMS == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await unitOfWork.StartAsync(cancellationToken);

                await repNotificacaoMotoristaSMS.DeletarAsync(notificacaoMotoristaSMS, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisa(cancellationToken);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Mensagem", false);
                grid.AdicionarCabecalho("Descricao", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Ativo", 30, Models.Grid.Align.left, false);

                NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = await repNotificacaoMotoristaSMS.ContarConsultaAsync(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS> listaNotificacaoMotoristaSMS = totalRegistros > 0 ? await repNotificacaoMotoristaSMS.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS>();

                var lista = (from p in listaNotificacaoMotoristaSMS
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Mensagem,
                                 Ativo = p.Ativo.ObterDescricaoAtivo(),
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaNotificacaoMotoristaSMS()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableBoolParam("Ativo"),
                TipoNotificacaoSMS = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoMotoristaSMS>("TipoNotificacao"),
                NotificacaoSuperApp = Request.GetNullableBoolParam("NotificacaoSuperApp")
            };
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS)
        {
            notificacaoMotoristaSMS.Codigo = Request.GetIntParam("Codigo");
            notificacaoMotoristaSMS.Descricao = Request.GetStringParam("Descricao");
            notificacaoMotoristaSMS.Mensagem = Request.GetStringParam("Mensagem");
            notificacaoMotoristaSMS.TipoEnvio = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoMotoristaSMS>("TipoEnvio");
            notificacaoMotoristaSMS.Ativo = Request.GetBoolParam("Ativo");
            notificacaoMotoristaSMS.NotificacaoSuperApp = Request.GetBoolParam("NotificacaoSuperApp");
        }
        #endregion
    }
}

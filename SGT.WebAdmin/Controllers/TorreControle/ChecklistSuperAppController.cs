using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.TorreControle

{
    [CustomAuthorize("TorreControle/ChecklistSuperApp")]

    public class ChecklistSuperAppController : BaseController
    {
        #region Construtores

        public ChecklistSuperAppController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisa(unitOfWork, cancellationToken));
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

        public async Task<IActionResult> ExcluirChecklistSuperApp(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repositorioChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork, cancellationToken);
                Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
                Repositorio.Embarcador.SuperApp.EventoSuperApp repositorioEventoSuperApp = new Repositorio.Embarcador.SuperApp.EventoSuperApp(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp = await repositorioChecklistSuperApp.BuscarPorCodigoAsync(codigo);
                List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa = await repositorioChecklistSuperAppEtapa.BuscarPorChecklistAsync(ChecklistSuperApp.Codigo);
                List<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp> listaEventos = await repositorioEventoSuperApp.BuscarPorCodigoChecklistSuperAppAsync(codigo);
                List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaEtapasRemover = (from etapa in listaChecklistSuperAppEtapa select etapa).ToList();


                List<int> codigosEventos = listaEventos.Select(e => e.Codigo).ToList();

                Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp repositorioTipoOperacaoEvento = new Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> LsttipoOperacaoSuperApp = await repositorioTipoOperacaoEvento.BuscarPorCodigosEventoSuperAppAsync(codigosEventos);
                List<int> codigosTipoOperacao = LsttipoOperacaoSuperApp.Select(t => t.TipoOperacao.Codigo).Distinct().ToList();                           

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTipoOperacao = await repositorioTipoOperacao.BuscarPorCodigosAsync(codigosTipoOperacao);

                await unitOfWork.StartAsync(cancellationToken);


                if (listaTipoOperacao?.Any() == true)
                {
                    // Verifica se há eventos com o mesmo Tipo no checklist
                    throw new ControllerException($"Existem Evento(s) vinculado(s) a este checklist no Tipo Operação: {string.Join(", ", listaTipoOperacao.Select(t => t.Descricao).ToList())}. Exclusão não permitida.");
                }

                if (ChecklistSuperApp == null)
                {
                    throw new ControllerException("Erro ao buscar o checklist selecionado.");
                }

                if (listaEtapasRemover.Any())
                {
                    foreach (Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapa in listaEtapasRemover)
                        await repositorioChecklistSuperAppEtapa.DeletarAsync(etapa);
                }

                await repositorioChecklistSuperApp.DeletarAsync(ChecklistSuperApp);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp = new Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp();

                PreencherEntidade(ChecklistSuperApp, unitOfWork);

                await repositorioChecklistSuperApp.InserirAsync(ChecklistSuperApp);

                await AtualizarEtapasChecklistAsync(ChecklistSuperApp, unitOfWork, cancellationToken);

                await IntegrarSuperApp(ChecklistSuperApp.Codigo, unitOfWork, cancellationToken);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp = await repositorioChecklistSuperApp.BuscarPorCodigoAsync(codigo);

                PreencherEntidade(ChecklistSuperApp, unitOfWork);

                await repositorioChecklistSuperApp.AtualizarAsync(ChecklistSuperApp);

                await AtualizarEtapasChecklistAsync(ChecklistSuperApp, unitOfWork, cancellationToken);

                await IntegrarSuperApp(ChecklistSuperApp.Codigo, unitOfWork, cancellationToken);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
                Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repositorioChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork, cancellationToken);


                Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp = await repositorioChecklistSuperApp.BuscarPorCodigoAsync(codigo);
                List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaEtapasChecklist = await repositorioChecklistSuperAppEtapa.BuscarPorChecklistAsync(codigo);

                if (ChecklistSuperApp == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    Codigo = duplicar ? 0 : ChecklistSuperApp.Codigo,
                    Titulo = ChecklistSuperApp.Titulo,
                    Descricao = ChecklistSuperApp.Descricao,
                    IdSuperApp = ChecklistSuperApp.IdSuperApp,
                    TipoFluxo = ChecklistSuperApp.TipoFluxo,
                    TipoFluxoDescricao = TipoFluxoChecklistSuperAppHelper.ObterDescricao(ChecklistSuperApp.TipoFluxo),
                    EtapasChecklist = ObterEtapasChecklist(listaEtapasChecklist, duplicar),
                };

                return new JsonpResult(retorno);
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

        #endregion

        #region Métodos Privados
        private async Task<Models.Grid.Grid> ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

            grid.AdicionarCabecalho("Codigo", "Codigo", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descricao", "Descricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Id Super App", "IdSuperApp", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de Fluxo", "TipoFluxo", 7, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
            int totalRegistros = await repositorioChecklistSuperApp.ContarConsultaAsync(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> lista = totalRegistros > 0 ? await repositorioChecklistSuperApp.ConsultarAsync(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>();

            var listaRetornar = (
                from obj in lista
                select new
                {
                    obj.Codigo,
                    obj.Titulo,
                    Descricao = obj.Descricao,
                    IdSuperApp = obj.IdSuperApp ?? string.Empty,
                    TipoFluxo = TipoFluxoChecklistSuperAppHelper.ObterDescricao(obj.TipoFluxo),
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp()
            {
                Codigo = Request.GetIntParam("Codigo"),
                IdSuperApp = Request.GetStringParam("IdSuperApp"),
                TipoFluxo = Request.GetIntParam("TipoFluxo"),
                Titulo = Request.GetStringParam("Titulo"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp, Repositorio.UnitOfWork unitOfWork)
        {
            ChecklistSuperApp.TipoFluxo = Request.GetEnumParam<TipoFluxoChecklistSuperApp>("TipoFluxo");
            ChecklistSuperApp.Titulo = Request.GetStringParam("Titulo");
            ChecklistSuperApp.IdSuperApp = Request.GetNullableStringParam("IdSuperApp");
        }
        private async Task AtualizarEtapasChecklistAsync(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklistSuperApp, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            dynamic listaChecklist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("ListaEtapasChecklist"));

            await ExcluirEtapasRemovidasAsync(checklistSuperApp, listaChecklist, unitOfWork, cancellationToken);
            await AtualizarEtapasAdicionadasAsync(checklistSuperApp, listaChecklist, unitOfWork, cancellationToken);
        }

        private async Task ExcluirEtapasRemovidasAsync(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklistSuperApp, dynamic etapas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repositorioChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa = await repositorioChecklistSuperAppEtapa.BuscarPorChecklistAsync(checklistSuperApp.Codigo);

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic etapa in etapas)
            {
                int? codigoEtapa = ((string)etapa.Codigo).ToNullableInt();

                if (codigoEtapa.HasValue)
                    listaCodigosAtualizados.Add(codigoEtapa.Value);
            }

            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaEtapasRemover = (from etapa in listaChecklistSuperAppEtapa where !listaCodigosAtualizados.Contains(etapa.Codigo) select etapa).ToList();

            foreach (Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapa in listaEtapasRemover)
                await repositorioChecklistSuperAppEtapa.DeletarAsync(etapa);
        }

        private async Task AtualizarEtapasAdicionadasAsync(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklistSuperApp, dynamic etapas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repositorioChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork, cancellationToken);

            foreach (dynamic etapa in etapas)
            {
                int? codigoEtapa = ((string)etapa.Codigo).ToNullableInt();

                if (!codigoEtapa.HasValue)
                {
                    Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa checklistSuperAppEtapa = new Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa()
                    {
                        Tipo = ((string)etapa.Tipo).ToEnum<TipoEtapaChecklistSuperApp>(),
                        Titulo = etapa.Titulo,
                        Descricao = etapa.Descricao,
                        Ordem = etapa.Ordem,
                        Obrigatorio = etapa.Obrigatorio,
                        Configuracoes = etapa.Configuracoes,
                        ChecklistSuperApp = checklistSuperApp,
                        TipoEvidencia = ((string)etapa.TipoEvidencia).ToEnum<TipoEvidenciaSuperApp>(),
                    };
                    await repositorioChecklistSuperAppEtapa.InserirAsync(checklistSuperAppEtapa);

                }
                else
                {
                    Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapaChecklist = await repositorioChecklistSuperAppEtapa.BuscarPorCodigoAsync(codigoEtapa.Value);

                    etapaChecklist.Tipo = ((string)etapa.Tipo).ToEnum<TipoEtapaChecklistSuperApp>();
                    etapaChecklist.Titulo = etapa.Titulo;
                    etapaChecklist.Descricao = etapa.Descricao;
                    etapaChecklist.Ordem = etapa.Ordem;
                    etapaChecklist.Obrigatorio = etapa.Obrigatorio;
                    etapaChecklist.Configuracoes = etapa.Configuracoes;
                    etapaChecklist.TipoEvidencia = etapa.TipoEvidencia;

                    await repositorioChecklistSuperAppEtapa.AtualizarAsync(etapaChecklist);
                }
            }
        }

        private dynamic ObterEtapasChecklist(List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaEtapasChecklist, bool duplicar)
        {
            List<dynamic> listaEtapas = new List<dynamic>();
            foreach (Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapaChecklist in listaEtapasChecklist)
            {
                listaEtapas.Add(new
                {
                    Codigo = duplicar ? 0 : etapaChecklist.Codigo,
                    Tipo = (int)etapaChecklist.Tipo,
                    TipoDescricao = etapaChecklist.Tipo.ObterDescricao(),
                    TipoEvidencia = etapaChecklist.TipoEvidencia,
                    Titulo = etapaChecklist.Titulo,
                    Descricao = etapaChecklist.Descricao,
                    Obrigatorio = etapaChecklist.Obrigatorio,
                    ObrigatorioDescricao = etapaChecklist.Obrigatorio ? "Sim" : "Não",
                    Ordem = etapaChecklist.Ordem,
                    IdSuperApp = etapaChecklist.IdSuperApp,
                    Configuracoes = !string.IsNullOrWhiteSpace(etapaChecklist?.Configuracoes) ? JsonConvert.DeserializeObject<dynamic>(etapaChecklist.Configuracoes) : null,
                });
            }
            return listaEtapas;
        }

        private async Task IntegrarSuperApp(int codigoChecklist, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.SuperApp.ChecklistSuperApp repositorioChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork, cancellationToken);
            Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repositorioChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklist = await repositorioChecklistSuperApp.BuscarPorCodigoAsync(codigoChecklist);
            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> etapas = await repositorioChecklistSuperAppEtapa.BuscarPorChecklistAsync(codigoChecklist);

            etapas.Sort((a, b) => a.Ordem.CompareTo(b.Ordem));

            var resultado = Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarChecklist(checklist, etapas, unitOfWork);

            if (!string.IsNullOrEmpty(resultado.mensagemErro))
            {
                throw new ControllerException(resultado.mensagemErro);
            }
            else if (resultado.checklist != null)
            {
                if (!string.IsNullOrEmpty(resultado.checklist?.checklistflow?._id))
                {
                    checklist.IdSuperApp = resultado.checklist.checklistflow._id;
                    await repositorioChecklistSuperApp.AtualizarAsync(checklist);
                }
                foreach (Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapa in etapas)
                {
                    var TipoEtapaChecklistSuperApp = resultado.checklist.checklistflow.steps.FirstOrDefault(step => step.externalInfo.id == etapa.Codigo.ToString());

                    if (TipoEtapaChecklistSuperApp != null)
                    {
                        etapa.IdSuperApp = TipoEtapaChecklistSuperApp._id;
                        await repositorioChecklistSuperAppEtapa.AtualizarAsync(etapa);
                    }
                }

            }
        }
        #endregion
    }
}

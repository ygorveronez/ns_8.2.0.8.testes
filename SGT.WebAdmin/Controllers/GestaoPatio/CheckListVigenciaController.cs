using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/CheckListVigencia")]
    public class CheckListVigenciaController : BaseController
    {
        #region Construtores

        public CheckListVigenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", "Filial", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Fim Vigência", "DataFimVigencia", 25, Models.Grid.Align.left, true);

                if (!filtrosPesquisa.Ativo.HasValue)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia> listaCheckListCargaVigencia = await repositorioCheckListCargaVigencia.ConsultarAsync(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(await repositorioCheckListCargaVigencia.ContarConsultaAsync(filtrosPesquisa));

                var lista = (from vigencia in listaCheckListCargaVigencia
                             select new
                             {
                                 vigencia.Codigo,
                                 Filial = vigencia.Filial.Descricao,
                                 TipoOperacao = vigencia.TipoOperacao?.Descricao,
                                 DataFimVigencia = vigencia.DataFimVigencia.ToString("d"),
                                 DescricaoSituacao = vigencia.Ativo.ObterDescricaoAtiva()
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia();

                PreencherCheckListVigencia(checkListCargaVigencia, unitOfWork);

                await repositorioCheckListCargaVigencia.InserirAsync(checkListCargaVigencia, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
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
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia = await repositorioCheckListCargaVigencia.BuscarPorCodigoAsync(codigo, false);

                if (checkListCargaVigencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherCheckListVigencia(checkListCargaVigencia, unitOfWork);

                await repositorioCheckListCargaVigencia.AtualizarAsync(checkListCargaVigencia, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia = await repositorioCheckListCargaVigencia.BuscarPorCodigoAsync(codigo, false);

                if (checkListCargaVigencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynCheckListCargaVigencia = new
                {
                    checkListCargaVigencia.Codigo,
                    Filial = new { checkListCargaVigencia.Filial.Codigo, checkListCargaVigencia.Filial.Descricao },
                    TipoOperacao = new { checkListCargaVigencia.TipoOperacao?.Codigo, checkListCargaVigencia.TipoOperacao?.Descricao },
                    Situacao = checkListCargaVigencia.Ativo,
                    DataFimVigencia = checkListCargaVigencia.DataFimVigencia.ToString("dd/MM/yyyy HH:ss"),
                };

                return new JsonpResult(dynCheckListCargaVigencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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

                Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia = await repositorioCheckListCargaVigencia.BuscarPorCodigoAsync(codigo, false);

                if (checkListCargaVigencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await repositorioCheckListCargaVigencia.DeletarAsync(checkListCargaVigencia, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

        #endregion

        #region Métodos Privados

        private void PreencherCheckListVigencia(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            if (codigoTipoOperacao > 0)
                tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) ?? throw new ControllerException("Tipo de Operação não encontrado.");

            checkListCargaVigencia.DataFimVigencia = Request.GetDateTimeParam("DataFimVigencia");
            checkListCargaVigencia.Ativo = Request.GetBoolParam("Situacao");
            checkListCargaVigencia.PreenchimentoManualObrigatorio = true;
            checkListCargaVigencia.Filial = filial ?? throw new ControllerException("Filial não encontrada.");
            checkListCargaVigencia.TipoOperacao = tipoOperacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Ativo = Request.GetNullableBoolParam("Situacao"),
            };
        }

    }
    #endregion
}

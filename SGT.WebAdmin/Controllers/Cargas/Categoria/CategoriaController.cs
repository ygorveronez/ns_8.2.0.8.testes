using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Carga
{
    [CustomAuthorize("Cargas/Categoria")]
    public class CategoriaController : BaseController
    {
        #region Construtores

        public CategoriaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 30, Models.Grid.Align.left, true);
                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.Categoria repCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.Categoria> pesquisaCategoria = await repCategoria.ConsultarAsync(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(await repCategoria.ContarConsultaAsync(filtrosPesquisa));

                var lista = (from p in pesquisaCategoria
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 Situacao = p.Situacao.ObterDescricaoAtivo()
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

                Repositorio.Embarcador.Cargas.Categoria repCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Categoria categoria = new Dominio.Entidades.Embarcador.Cargas.Categoria();

                PreencherCategoria(categoria);

                bool existeDuplicado = await repCategoria.ExisteDuplicadoAsync(categoria);

                if (existeDuplicado)
                    throw new ControllerException("Já existe uma categoria com esses dados.");

                await repCategoria.InserirAsync(categoria, Auditado);

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

                Repositorio.Embarcador.Cargas.Categoria repCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Categoria categoria = await repCategoria.BuscarPorCodigoAsync(codigo, true);

                if (categoria == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherCategoria(categoria);

                bool existeDuplicado = await repCategoria.ExisteDuplicadoAsync(categoria);

                if (existeDuplicado)
                    throw new ControllerException("Já existe uma categoria com esses dados.");

                await repCategoria.AtualizarAsync(categoria, Auditado);

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

                Repositorio.Embarcador.Cargas.Categoria repCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Categoria categoria = await repCategoria.BuscarPorCodigoAsync(codigo, false);

                if (categoria == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynCategoria = new
                {
                    categoria.Codigo,
                    categoria.Descricao,
                    categoria.Situacao,
                    categoria.Observacao,
                    categoria.CodigoIntegracao
                };

                return new JsonpResult(dynCategoria);
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
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Categoria repCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Categoria categoria = await repCategoria.BuscarPorCodigoAsync(codigo, true);

                if (categoria == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await repCategoria.DeletarAsync(categoria, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherCategoria(Dominio.Entidades.Embarcador.Cargas.Categoria categoria)
        {
            categoria.Descricao = Request.GetStringParam("Descricao");
            categoria.Situacao = Request.GetBoolParam("Situacao");
            categoria.Observacao = Request.GetStringParam("Observacao");
            categoria.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCategoria()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao")
            };
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/TipoPlotagem")]
    public class TipoPlotagemController : BaseController
    {
        #region Construtores

        public TipoPlotagemController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unidadeTrabalho, cancellationToken);

                List<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem> listaTipoPlotagem = await repTipoPlotagem.ConsultarAsync(descricao, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(await repTipoPlotagem.ContarConsultaAsync(descricao, situacao));

                grid.AdicionaRows((from p in listaTipoPlotagem select new { p.Codigo, p.Descricao, p.DescricaoAtivo }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }
        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unidadeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem tipoPlotagem = new Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem();

                tipoPlotagem.Ativo = ativo;
                tipoPlotagem.Descricao = descricao;

                await repTipoPlotagem.InserirAsync(tipoPlotagem, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }
        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unidadeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem tipoPlotagem = await repTipoPlotagem.BuscarPorCodigoAsync(codigo, true);

                tipoPlotagem.Ativo = ativo;
                tipoPlotagem.Descricao = descricao;

                await repTipoPlotagem.AtualizarAsync(tipoPlotagem, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
               await unidadeTrabalho.DisposeAsync();
            }
        }
        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unidadeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem tipoPlotagem = await repTipoPlotagem.BuscarPorCodigoAsync(codigo);

                await repTipoPlotagem.DeletarAsync(tipoPlotagem, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }
        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unidadeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem tipoPlotagem = await repTipoPlotagem.BuscarPorCodigoAsync(codigo);

                var retorno = new
                {
                    tipoPlotagem.Ativo,
                    tipoPlotagem.Codigo,
                    tipoPlotagem.Descricao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

    }
}

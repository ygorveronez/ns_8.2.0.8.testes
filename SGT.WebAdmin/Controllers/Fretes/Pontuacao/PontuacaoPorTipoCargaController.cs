using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorTipoCargaController : BaseController
    {
		#region Construtores

		public PontuacaoPorTipoCargaController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("TipoCarga").Nome("Tipo de Operação").Tamanho(60);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga> pontuacaoPorTipoCargas = repPontuacaoPorTipoCarga.BuscarTodas(grid.inicio, grid.limite);
                int totalRegistros = repPontuacaoPorTipoCarga.ContarTodos();

                var lista = (from obj in pontuacaoPorTipoCargas
                             select new
                             {
                                 obj.Codigo,
                                 TipoCarga = obj.TipoCarga.Descricao,
                                 obj.Pontuacao
                             }).ToList();

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



        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCarga = repPontuacaoPorTipoCarga.BuscarPorCodigo(codigo, true);

                var dynPontuacao = new
                {
                    pontuacaoPorTipoCarga.Codigo,
                    TipoCarga = new { pontuacaoPorTipoCarga.TipoCarga.Codigo, pontuacaoPorTipoCarga.TipoCarga.Descricao },
                    pontuacaoPorTipoCarga.Pontuacao
                };

                return new JsonpResult(dynPontuacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                int.TryParse(Request.Params("TipoCarga"), out int tipoCarga);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCarga = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga();

                pontuacaoPorTipoCarga.TipoCarga = repTipoCarga.BuscarPorCodigo(tipoCarga);
                pontuacaoPorTipoCarga.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCargaExiste = repPontuacaoPorTipoCarga.VerificarExistePorTipoCarga(tipoCarga);

                if (pontuacaoPorTipoCargaExiste == null)
                {
                    repPontuacaoPorTipoCarga.Inserir(pontuacaoPorTipoCarga, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa faixa.");
                }
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                int.TryParse(Request.Params("TipoCarga"), out int tipoCarga);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCarga = repPontuacaoPorTipoCarga.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                pontuacaoPorTipoCarga.TipoCarga = repTipoCarga.BuscarPorCodigo(tipoCarga);
                pontuacaoPorTipoCarga.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCargaExiste = repPontuacaoPorTipoCarga.VerificarExistePorTipoCarga(tipoCarga);

                if (pontuacaoPorTipoCargaExiste == null || pontuacaoPorTipoCarga.Codigo == pontuacaoPorTipoCargaExiste.Codigo)
                {
                    repPontuacaoPorTipoCarga.Atualizar(pontuacaoPorTipoCarga, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa faixa.");
                }
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCarga = repPontuacaoPorTipoCarga.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                repPontuacaoPorTipoCarga.Deletar(pontuacaoPorTipoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}

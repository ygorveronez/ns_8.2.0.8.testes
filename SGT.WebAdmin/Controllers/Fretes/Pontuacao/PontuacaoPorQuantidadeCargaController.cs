using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorQuantidadeCargaController : BaseController
    {
		#region Construtores

		public PontuacaoPorQuantidadeCargaController(Conexao conexao) : base(conexao) { }

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
                grid.Prop("QuantidadeInicio").Nome("Quantidade de Cargas Mês Inicial").Tamanho(30);
                grid.Prop("QuantidadeFim").Nome("Quantidade de Cargas Mês Final").Tamanho(30);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga> pontuacaoPorQuantidadeCargas = repPontuacaoPorQuantidadeCarga.BuscarTodas(grid.inicio, grid.limite);
                int totalRegistros = repPontuacaoPorQuantidadeCarga.ContarTodos();

                var lista = (from obj in pontuacaoPorQuantidadeCargas
                             select new
                             {
                                 obj.Codigo,
                                 QuantidadeInicio = obj.QuantidadeInicio.ToString(),
                                 QuantidadeFim = obj.QuantidadeFim.ToString(),
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCarga = repPontuacaoPorQuantidadeCarga.BuscarPorCodigo(codigo, true);

                var dynPontuacao = new
                {
                    pontuacaoPorQuantidadeCarga.Codigo,
                    pontuacaoPorQuantidadeCarga.QuantidadeInicio,
                    pontuacaoPorQuantidadeCarga.QuantidadeFim,
                    pontuacaoPorQuantidadeCarga.Pontuacao
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(unitOfWork);

                int.TryParse(Request.Params("QuantidadeInicio"), out int quantidadeInicio);
                int.TryParse(Request.Params("QuantidadeFim"), out int quantidadeFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCarga = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga();
                pontuacaoPorQuantidadeCarga.QuantidadeInicio = quantidadeInicio;
                pontuacaoPorQuantidadeCarga.QuantidadeFim = quantidadeFim;
                pontuacaoPorQuantidadeCarga.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCargaExiste = repPontuacaoPorQuantidadeCarga.VerificarExisteEntrePeriodo(quantidadeInicio, quantidadeFim);

                if (quantidadeInicio > quantidadeFim)
                    return new JsonpResult(false, true, "A quantidade inicial não deve ser maior que o final.");

                if (pontuacaoPorQuantidadeCargaExiste == null)
                {
                    repPontuacaoPorQuantidadeCarga.Inserir(pontuacaoPorQuantidadeCarga, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(unitOfWork);

                int.TryParse(Request.Params("QuantidadeInicio"), out int quantidadeInicio);
                int.TryParse(Request.Params("QuantidadeFim"), out int quantidadeFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCarga = repPontuacaoPorQuantidadeCarga.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                pontuacaoPorQuantidadeCarga.QuantidadeInicio = quantidadeInicio;
                pontuacaoPorQuantidadeCarga.QuantidadeFim = quantidadeFim;
                pontuacaoPorQuantidadeCarga.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCargaExiste = repPontuacaoPorQuantidadeCarga.VerificarExisteEntrePeriodo(quantidadeInicio, quantidadeFim);

                if (quantidadeInicio > quantidadeFim)
                    return new JsonpResult(false, true, "A quantidade inicial não deve ser maior que o final.");

                if (pontuacaoPorQuantidadeCargaExiste == null || pontuacaoPorQuantidadeCarga.Codigo == pontuacaoPorQuantidadeCargaExiste.Codigo)
                {
                    repPontuacaoPorQuantidadeCarga.Atualizar(pontuacaoPorQuantidadeCarga, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCarga = repPontuacaoPorQuantidadeCarga.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                repPontuacaoPorQuantidadeCarga.Deletar(pontuacaoPorQuantidadeCarga, Auditado);

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

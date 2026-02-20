using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorTempoAtividadeController : BaseController
    {
		#region Construtores

		public PontuacaoPorTempoAtividadeController(Conexao conexao) : base(conexao) { }

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
                grid.Prop("AnoInicio").Nome("Quantidade de Anos Inicial").Tamanho(30);
                grid.Prop("AnoFim").Nome("Quantidade de Anos Final").Tamanho(30);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade> pontuacaoPorTempoAtividades = repPontuacaoPorTempoAtividade.BuscarTodas(grid.inicio, grid.limite);
                int totalRegistros = repPontuacaoPorTempoAtividade.ContarTodos();


                var lista = (from obj in pontuacaoPorTempoAtividades
                             select new
                             {
                                 obj.Codigo,
                                 AnoInicio = obj.AnoInicio.ToString(),
                                 AnoFim = obj.AnoFim.ToString(),
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividade = repPontuacaoPorTempoAtividade.BuscarPorCodigo(codigo, true);

                var dynPontuacao = new
                {
                    pontuacaoPorTempoAtividade.Codigo,
                    pontuacaoPorTempoAtividade.AnoInicio,
                    pontuacaoPorTempoAtividade.AnoFim,
                    pontuacaoPorTempoAtividade.Pontuacao
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(unitOfWork);

                int.TryParse(Request.Params("AnoInicio"), out int anoInicio);
                int.TryParse(Request.Params("AnoFim"), out int anoFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividade = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade();
                pontuacaoPorTempoAtividade.AnoInicio = anoInicio;
                pontuacaoPorTempoAtividade.AnoFim = anoFim;
                pontuacaoPorTempoAtividade.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividadeExiste = repPontuacaoPorTempoAtividade.VerificarExisteEntrePeriodo(anoInicio, anoFim);

                if (anoInicio > anoFim)
                    return new JsonpResult(false, true, "O ano inicial não deve ser maior que o final.");

                if (pontuacaoPorTempoAtividadeExiste == null)
                {
                    repPontuacaoPorTempoAtividade.Inserir(pontuacaoPorTempoAtividade, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(unitOfWork);

                int.TryParse(Request.Params("AnoInicio"), out int anoInicio);
                int.TryParse(Request.Params("AnoFim"), out int anoFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividade = repPontuacaoPorTempoAtividade.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                pontuacaoPorTempoAtividade.AnoInicio = anoInicio;
                pontuacaoPorTempoAtividade.AnoFim = anoFim;
                pontuacaoPorTempoAtividade.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividadeExiste = repPontuacaoPorTempoAtividade.VerificarExisteEntrePeriodo(anoInicio, anoFim);

                if (anoInicio > anoFim)
                    return new JsonpResult(false, true, "O ano inicial não deve ser maior que o final.");

                if (pontuacaoPorTempoAtividadeExiste == null || pontuacaoPorTempoAtividade.Codigo == pontuacaoPorTempoAtividadeExiste.Codigo)
                {
                    repPontuacaoPorTempoAtividade.Atualizar(pontuacaoPorTempoAtividade, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividade = repPontuacaoPorTempoAtividade.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                repPontuacaoPorTempoAtividade.Deletar(pontuacaoPorTempoAtividade, Auditado);

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

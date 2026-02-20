using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoCarga
{
    [CustomAuthorize("Cargas/LeilaoTipoOperacaoConfiguracao")]
    public class LeilaoTipoOperacaoConfiguracaoController : BaseController
    {
		#region Construtores

		public LeilaoTipoOperacaoConfiguracaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacaoEmissao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Permite Leilão", "PermiteLeilao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Limite para Leilão (Horas)", "LimiteTempoLeilaoEmHoras", 15, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfiguracao = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao> listaLeilaoTipoOperacaoConfiguracao = repLeilaoTipoOperacaoConfiguracao.Consultar(tipoOperacaoEmissao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repLeilaoTipoOperacaoConfiguracao.ContarConsulta(tipoOperacaoEmissao));
                var lista = (from p in listaLeilaoTipoOperacaoConfiguracao
                            select new
                            {
                                p.Codigo,
                                TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissaoDescricao.RetornarDescricao(p.TipoOperacaoEmissao),
                                PermiteLeilao = p.PermiteLeilao == true ? "Sim" : "Não",
                                LimiteTempoLeilaoEmHoras = p.LimiteTempoLeilaoEmHoras > 0 ? p.LimiteTempoLeilaoEmHoras.ToString() : "Sem Prazo"
                            }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfiguracao = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfiguracao = new Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao();
                leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));
                if (!string.IsNullOrWhiteSpace(Request.Params("LimiteTempoLeilaoEmHoras")))
                    leilaoTipoOperacaoConfiguracao.LimiteTempoLeilaoEmHoras = int.Parse(Request.Params("LimiteTempoLeilaoEmHoras"));
                leilaoTipoOperacaoConfiguracao.PermiteLeilao = bool.Parse(Request.Params("PermiteLeilao"));

                
                if (repLeilaoTipoOperacaoConfiguracao.BuscarPorTipoOperacao(leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao) == null)
                {
                    repLeilaoTipoOperacaoConfiguracao.Inserir(leilaoTipoOperacaoConfiguracao, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuração cadastrada para esse tipo de operação");
                }
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
                Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfiguracao = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfiguracao = repLeilaoTipoOperacaoConfiguracao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));
                if (!string.IsNullOrWhiteSpace(Request.Params("LimiteTempoLeilaoEmHoras")))
                    leilaoTipoOperacaoConfiguracao.LimiteTempoLeilaoEmHoras = int.Parse(Request.Params("LimiteTempoLeilaoEmHoras"));
                leilaoTipoOperacaoConfiguracao.PermiteLeilao = bool.Parse(Request.Params("PermiteLeilao"));

                Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfiguracaoExiste = repLeilaoTipoOperacaoConfiguracao.BuscarPorTipoOperacao(leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao);
                if (leilaoTipoOperacaoConfiguracaoExiste == null || (leilaoTipoOperacaoConfiguracaoExiste.Codigo == leilaoTipoOperacaoConfiguracao.Codigo))
                {
                    repLeilaoTipoOperacaoConfiguracao.Atualizar(leilaoTipoOperacaoConfiguracao, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuraçãoo cadastrada para esse tipo de operação");
                }
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfiguracao = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfiguracao = repLeilaoTipoOperacaoConfiguracao.BuscarPorCodigo(codigo);
                var dynLeilaoTipoOperacaoConfiguracao = new
                {
                    leilaoTipoOperacaoConfiguracao.Codigo,
                    leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao,
                    leilaoTipoOperacaoConfiguracao.PermiteLeilao,
                    leilaoTipoOperacaoConfiguracao.LimiteTempoLeilaoEmHoras
                };
                return new JsonpResult(dynLeilaoTipoOperacaoConfiguracao);
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


        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfiguracao = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfiguracao = repLeilaoTipoOperacaoConfiguracao.BuscarPorCodigo(codigo);
                repLeilaoTipoOperacaoConfiguracao.Deletar(leilaoTipoOperacaoConfiguracao, Auditado);
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
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ProcessoMovimento")]
    public class ProcessoMovimentoController : BaseController
    {
		#region Construtores

		public ProcessoMovimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoProcesso;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);
                Enum.TryParse(Request.Params("TipoProcesso"), out tipoProcesso);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Processo", "DescricaoTipoProcessoMovimento", 25, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);


                List<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento> listaProcessoMovimento = repProcessoMovimento.Consultar(descricao, ativo, tipoProcesso, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProcessoMovimento.ContarConsulta(descricao, ativo, tipoProcesso));
                var lista = (from p in listaProcessoMovimento
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.DescricaoTipoProcessoMovimento,
                                p.DescricaoAtivo
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

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento processoMovimento = new Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento();
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoProcesso;
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                Enum.TryParse(Request.Params("TipoProcesso"), out tipoProcesso);

                processoMovimento.Descricao = Request.Params("Descricao");
                processoMovimento.Observacao = Request.Params("Observacao");
                processoMovimento.Ativo = ativo;
                processoMovimento.TipoProcessoMovimento = tipoProcesso;

                repProcessoMovimento.Inserir(processoMovimento, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                unitOfWork.Start();
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento processoMovimento = repProcessoMovimento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento tipoProcesso;
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                Enum.TryParse(Request.Params("TipoProcesso"), out tipoProcesso);

                processoMovimento.Descricao = Request.Params("Descricao");
                processoMovimento.Observacao = Request.Params("Observacao");
                processoMovimento.Ativo = ativo;
                processoMovimento.TipoProcessoMovimento = tipoProcesso;
                repProcessoMovimento.Atualizar(processoMovimento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento processoMovimento = repProcessoMovimento.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    processoMovimento.Codigo,
                    processoMovimento.Descricao,
                    processoMovimento.Observacao,
                    processoMovimento.Ativo,
                    TipoProcesso = processoMovimento.TipoProcessoMovimento
                };
                return new JsonpResult(dynProcessoMovimento);
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
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento processoMovimento = repProcessoMovimento.BuscarPorCodigo(codigo);
                repProcessoMovimento.Deletar(processoMovimento, Auditado);
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

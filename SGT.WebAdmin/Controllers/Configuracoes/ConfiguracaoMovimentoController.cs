using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Configuracoes/ConfiguracaoMovimento")]
    public class ConfiguracaoMovimentoController : BaseController
    {
		#region Construtores

		public ConfiguracaoMovimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento repConfiguracaoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento(unitOfWork);

                int processoMovimento = 0;
                int.TryParse(Request.Params("ProcessoMovimento"), out processoMovimento);
                string descricao = Request.Params("Descricao");                
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);
                

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Processo Movimento", "DescricaoProcessoMovimento", 25, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);


                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento> listaConfiguracaoMovimento = repConfiguracaoMovimento.Consultar(descricao, ativo, processoMovimento, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConfiguracaoMovimento.ContarConsulta(descricao, ativo, processoMovimento));
                var lista = (from p in listaConfiguracaoMovimento
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                DescricaoProcessoMovimento = p.ProcessoMovimento != null ? p.ProcessoMovimento.Descricao: string.Empty,
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento repConfiguracaoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento configuracaoMovimento = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento();

                int processoMovimento, centroResultado, tipoMovimento, empresa = 0;
                int.TryParse(Request.Params("ProcessoMovimento"), out processoMovimento);
                int.TryParse(Request.Params("CentroResultado"), out centroResultado);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                int.TryParse(Request.Params("Empresa"), out empresa);
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);                

                configuracaoMovimento.Descricao = Request.Params("Descricao");
                configuracaoMovimento.Ativo = ativo;
                if (processoMovimento > 0)
                    configuracaoMovimento.ProcessoMovimento = repProcessoMovimento.BuscarPorCodigo(processoMovimento);
                if (centroResultado > 0)
                    configuracaoMovimento.CentroResultado = repCentroResultado.BuscarPorCodigo(centroResultado);
                if (tipoMovimento > 0)
                    configuracaoMovimento.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);
                if (empresa > 0)
                    configuracaoMovimento.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                repConfiguracaoMovimento.Inserir(configuracaoMovimento, Auditado);
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento repConfiguracaoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ProcessoMovimento repProcessoMovimento = new Repositorio.Embarcador.Configuracoes.ProcessoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento configuracaoMovimento = repConfiguracaoMovimento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int processoMovimento, centroResultado, tipoMovimento, empresa = 0;
                int.TryParse(Request.Params("ProcessoMovimento"), out processoMovimento);
                int.TryParse(Request.Params("CentroResultado"), out centroResultado);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                int.TryParse(Request.Params("Empresa"), out empresa);
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                configuracaoMovimento.Descricao = Request.Params("Descricao");
                configuracaoMovimento.Ativo = ativo;
                if (processoMovimento > 0)
                    configuracaoMovimento.ProcessoMovimento = repProcessoMovimento.BuscarPorCodigo(processoMovimento);
                else
                    configuracaoMovimento.ProcessoMovimento = null;
                if (centroResultado > 0)
                    configuracaoMovimento.CentroResultado = repCentroResultado.BuscarPorCodigo(centroResultado);
                else
                    configuracaoMovimento.CentroResultado = null;
                if (tipoMovimento > 0)
                    configuracaoMovimento.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);
                else
                    configuracaoMovimento.TipoMovimento = null;
                if (empresa > 0)
                    configuracaoMovimento.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                else
                    configuracaoMovimento.Empresa = null;

                repConfiguracaoMovimento.Atualizar(configuracaoMovimento, Auditado);
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento repConfiguracaoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento configuracaoMovimento = repConfiguracaoMovimento.BuscarPorCodigo(codigo);
                var dynConfiguracaoMovimento = new
                {
                    configuracaoMovimento.Codigo,
                    configuracaoMovimento.Descricao,                    
                    configuracaoMovimento.Ativo,
                    Empresa = new { Codigo = configuracaoMovimento.Empresa != null ? configuracaoMovimento.Empresa.Codigo : 0, Descricao = configuracaoMovimento.Empresa != null ? configuracaoMovimento.Empresa.RazaoSocial : "" },
                    TipoMovimento = new { Codigo = configuracaoMovimento.TipoMovimento != null ? configuracaoMovimento.TipoMovimento.Codigo : 0, Descricao = configuracaoMovimento.TipoMovimento != null ? configuracaoMovimento.TipoMovimento.Descricao : "" },
                    CentroResultado = new { Codigo = configuracaoMovimento.CentroResultado != null ? configuracaoMovimento.CentroResultado.Codigo : 0, Descricao = configuracaoMovimento.CentroResultado != null ? configuracaoMovimento.CentroResultado.Descricao : "" },
                    ProcessoMovimento = new { Codigo = configuracaoMovimento.ProcessoMovimento != null ? configuracaoMovimento.ProcessoMovimento.Codigo : 0, Descricao = configuracaoMovimento.ProcessoMovimento != null ? configuracaoMovimento.ProcessoMovimento.Descricao : "" }
                };
                return new JsonpResult(dynConfiguracaoMovimento);
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento repConfiguracaoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMovimento configuracaoMovimento = repConfiguracaoMovimento.BuscarPorCodigo(codigo);
                repConfiguracaoMovimento.Deletar(configuracaoMovimento, Auditado);
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

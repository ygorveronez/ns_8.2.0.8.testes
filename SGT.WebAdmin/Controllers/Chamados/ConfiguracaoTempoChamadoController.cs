using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ConfiguracaoTempoChamado")]
    public class ConfiguracaoTempoChamadoController : BaseController
    {
		#region Construtores

		public ConfiguracaoTempoChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tempo Atendimento", "TempoAtendimento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Models.Grid.Align.left, false);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> configuracaoTempoChamados = repConfiguracaoTempoChamado.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repConfiguracaoTempoChamado.ContarConsulta(filtrosPesquisa));

                var lista = (from p in configuracaoTempoChamados
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.TempoAtendimento,
                                 p.DescricaoAtivo,
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 Filial = p.Filial?.Descricao ?? string.Empty,
                                 TipoOperacao = p.TipoOperacao?.Descricao ?? string.Empty,
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

                Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = new Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado();

                PreencherConfiguracaoTempoChamado(configuracaoTempoChamado, unitOfWork);

                repConfiguracaoTempoChamado.Inserir(configuracaoTempoChamado, Auditado);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = repConfiguracaoTempoChamado.BuscarPorCodigo(codigo, true);

                if (configuracaoTempoChamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherConfiguracaoTempoChamado(configuracaoTempoChamado, unitOfWork);

                repConfiguracaoTempoChamado.Atualizar(configuracaoTempoChamado, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = repConfiguracaoTempoChamado.BuscarPorCodigo(codigo, false);

                if (configuracaoTempoChamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynConfiguracaoTempoChamado = new
                {
                    configuracaoTempoChamado.Codigo,
                    configuracaoTempoChamado.Descricao,
                    configuracaoTempoChamado.Ativo,
                    configuracaoTempoChamado.TempoAtendimento,
                    Cliente = new { Codigo = configuracaoTempoChamado.Cliente?.Codigo ?? 0, Descricao = configuracaoTempoChamado.Cliente?.Descricao ?? string.Empty },
                    Filial = new { Codigo = configuracaoTempoChamado.Filial?.Codigo ?? 0, Descricao = configuracaoTempoChamado.Filial?.Descricao ?? string.Empty },
                    TipoOperacao = new { Codigo = configuracaoTempoChamado.TipoOperacao?.Codigo ?? 0, Descricao = configuracaoTempoChamado.TipoOperacao?.Descricao ?? string.Empty }
                };

                return new JsonpResult(dynConfiguracaoTempoChamado);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = repConfiguracaoTempoChamado.BuscarPorCodigo(codigo, true);

                if (configuracaoTempoChamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repConfiguracaoTempoChamado.Deletar(configuracaoTempoChamado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #region Métodos Privados

        private void PreencherConfiguracaoTempoChamado(Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            double codigoCliente = Request.GetDoubleParam("Cliente");

            configuracaoTempoChamado.Descricao = Request.GetStringParam("Descricao");
            configuracaoTempoChamado.Ativo = Request.GetBoolParam("Ativo");
            configuracaoTempoChamado.TempoAtendimento = Request.GetIntParam("TempoAtendimento");

            configuracaoTempoChamado.Cliente = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;
            configuracaoTempoChamado.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
            configuracaoTempoChamado.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CnpjCpfCliente = Request.GetDoubleParam("Cliente"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao")
            };
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MotivoReagendamento")]
    public class MotivoReagendamentoController : BaseController
    {
		#region Construtores

		public MotivoReagendamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoResponsavelAtrasoEntrega", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento> motivoReagendamentos = repositorioMotivoReagendamento.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioMotivoReagendamento.ContarConsulta(filtrosPesquisa));

                var lista = (from obj in motivoReagendamentos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.DescricaoAtivo,
                                 TipoResponsavelAtrasoEntrega = obj.TipoResponsavelAtrasoEntrega?.Codigo ?? 0
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch(Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento = repositorioMotivoReagendamento.BuscarPorCodigo(codigo, false);

                if (motivoReagendamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynMotivoReagendamento = new
                {
                    motivoReagendamento.Codigo,
                    motivoReagendamento.Descricao,
                    motivoReagendamento.Ativo,
                    motivoReagendamento.Observacao,
                    motivoReagendamento.ConsiderarOnTime,
                    TipoResponsavelAtrasoEntrega = new { Codigo = motivoReagendamento.TipoResponsavelAtrasoEntrega?.Codigo ?? 0, Descricao = motivoReagendamento.TipoResponsavelAtrasoEntrega?.Descricao ?? string.Empty },
                };

                return new JsonpResult(dynMotivoReagendamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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

                Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento = new Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento();
                
                PreencherMotivoReagendamento(motivoReagendamento, unitOfWork);

                repositorioMotivoReagendamento.Inserir(motivoReagendamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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

                Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento = repositorioMotivoReagendamento.BuscarPorCodigo(codigo, true);

                if (motivoReagendamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherMotivoReagendamento(motivoReagendamento, unitOfWork);

                repositorioMotivoReagendamento.Atualizar(motivoReagendamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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

                Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento = repositorioMotivoReagendamento.BuscarPorCodigo(codigo, true);

                if (motivoReagendamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioMotivoReagendamento.Deletar(motivoReagendamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherMotivoReagendamento(Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repositorioTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);

            int codigoTipoResponsavelAtrasoEntrega = Request.GetIntParam("TipoResponsavelAtrasoEntrega");

            motivoReagendamento.Descricao = Request.GetStringParam("Descricao");
            motivoReagendamento.Ativo = Request.GetBoolParam("Ativo");
            motivoReagendamento.Observacao = Request.GetStringParam("Observacao");
            motivoReagendamento.ConsiderarOnTime = Request.GetBoolParam("ConsiderarOnTime");
            motivoReagendamento.TipoResponsavelAtrasoEntrega = codigoTipoResponsavelAtrasoEntrega > 0 ? repositorioTipoResponsavelAtrasoEntrega.BuscarPorCodigo(codigoTipoResponsavelAtrasoEntrega, false) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo")
            };
        }

        #endregion

    }
}

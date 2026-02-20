using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/ServicoVeiculo")]
    public class ServicoVeiculoController : BaseController
    {
		#region Construtores

		public ServicoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoServico = Request.GetIntParam("GrupoServico");
                string descricao = Request.Params("Descricao");

                SituacaoAtivoPesquisa? ativo = null;
                SituacaoAtivoPesquisa ativoAux;
                if (Enum.TryParse(Request.Params("Ativo"), out ativoAux))
                    ativo = ativoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 75, Models.Grid.Align.left, true);

                if (ativo.HasValue && ativo.Value == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("TempoEstimado", false);
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> listaServico = repServico.Consultar(codigoEmpresa, descricao, ativo, codigoGrupoServico, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repServico.ContarConsulta(codigoEmpresa, descricao, ativo, codigoGrupoServico));
                grid.AdicionaRows((from obj in listaServico
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao,
                                       obj.DescricaoAtivo,
                                       obj.TempoEstimado
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repositorioServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = new Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota();

                PreencherServicoVeiculoFrota(servico, unidadeTrabalho);

                repositorioServico.Inserir(servico, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repositorioServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repositorioServico.BuscarPorCodigo(codigo, true);

                PreencherServicoVeiculoFrota(servico, unidadeTrabalho);

                repositorioServico.Atualizar(servico, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repositorioServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repositorioServico.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Frota.GrupoServico> gruposServico = repositorioGrupoServico.BuscarPorServicoVeiculo(codigo);

                var retorno = new
                {
                    servico.Ativo,
                    servico.Codigo,
                    servico.CodigoIntegracao,
                    servico.Descricao,
                    servico.ExecucaoUnica,
                    servico.PermiteLancamentoSemValor,
                    servico.ServicoParaEquipamento,
                    servico.Observacao,
                    PlanoConta = new { Codigo = servico.PlanoConta?.Codigo ?? 0, Descricao = servico.PlanoConta?.Descricao ?? string.Empty },
                    servico.Tipo,
                    servico.ToleranciaDias,
                    servico.ToleranciaKM,
                    servico.ValidadeDias,
                    servico.ValidadeKM,
                    servico.ValidadeHorimetro,
                    servico.ToleranciaHorimetro,
                    servico.Motivo,
                    servico.ObrigatorioParaRealizarCarga,
                    servico.TempoEstimado,
                    servico.TipoManutencao,
                    servico.Cores,
                    servico.Prioridade,
                    GruposServico = (from obj in gruposServico
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repositorioServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repositorioServico.BuscarPorCodigo(codigo);

                repositorioServico.Deletar(servico, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Dispose();

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
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherServicoVeiculoFrota(Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repositorioPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeTrabalho);

            int codigoPlanoConta = Request.GetIntParam("PlanoConta");

            servico.ObrigatorioParaRealizarCarga = Request.GetBoolParam("ObrigatorioParaRealizarCarga");
            servico.ServicoParaEquipamento = Request.GetBoolParam("ServicoParaEquipamento");
            servico.Ativo = Request.GetBoolParam("Ativo");
            servico.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            servico.Descricao = Request.GetStringParam("Descricao");
            servico.ExecucaoUnica = Request.GetBoolParam("ExecucaoUnica");
            servico.PermiteLancamentoSemValor = Request.GetBoolParam("PermiteLancamentoSemValor");
            servico.Motivo = Request.GetEnumParam<MotivoServicoVeiculo>("Motivo");
            servico.Observacao = Request.GetStringParam("Observacao");
            servico.Tipo = Request.GetEnumParam<TipoServicoVeiculo>("Tipo");
            servico.ToleranciaDias = 0;
            servico.ToleranciaKM = 0;
            servico.ValidadeDias = 0;
            servico.ValidadeKM = 0;
            servico.ValidadeHorimetro = 0;
            servico.ToleranciaHorimetro = 0;
            servico.TempoEstimado = Request.GetIntParam("TempoEstimado");

            if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorDia || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
            {
                servico.ToleranciaDias = Request.GetIntParam("ToleranciaDias");
                servico.ValidadeDias = Request.GetIntParam("ValidadeDias");
            }

            if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorKM || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.Ambos))
            {
                servico.ToleranciaKM = Request.GetIntParam("ToleranciaKM");
                servico.ValidadeKM = Request.GetIntParam("ValidadeKM");
            }

            if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorHorimetro || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
            {
                servico.ValidadeHorimetro = Request.GetIntParam("ValidadeHorimetro");
                servico.ToleranciaHorimetro = Request.GetIntParam("ToleranciaHorimetro");
            }

            if (servico.Codigo == 0)
                servico.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;

            servico.PlanoConta = codigoPlanoConta > 0 ? repositorioPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
            servico.TipoManutencao = Request.GetEnumParam<TipoManutencaoServicoVeiculo>("TipoManutencao");
            servico.Cores = Request.GetEnumParam<Cores>("Cores");
            servico.Prioridade = Request.GetIntParam("Prioridade");
        }

        #endregion
    }
}

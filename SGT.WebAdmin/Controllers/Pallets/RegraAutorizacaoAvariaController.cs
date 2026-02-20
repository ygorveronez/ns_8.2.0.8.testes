using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { }, "Pallets/RegraAutorizacaoTransferencia")]
    public class RegraAutorizacaoAvariaController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria>
    {
		#region Construtores

		public RegraAutorizacaoAvariaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorMotivoAvaria = regra.RegraPorMotivoAvaria,
                UsarRegraPorSetor = regra.RegraPorSetor,
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasMotivoAvaria = (from alcada in regra.AlcadasMotivoAvaria select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaMotivoAvaria, Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>(alcada)).ToList(),
                AlcadasSetor = (from alcada in regra.AlcadasSetor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaSetor, Dominio.Entidades.Setor>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList()
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioMotivoAvaria = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                var repositorioSetor = new Repositorio.Setor(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoAvaria = new Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria();

                PreencherRegra(regraAutorizacaoAvaria, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorMotivoAvaria = Request.GetBoolParam("UsarRegraPorMotivoAvaria");
                    regra.RegraPorSetor = Request.GetBoolParam("UsarRegraPorSetor");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                repositorioRegra.Inserir(regraAutorizacaoAvaria, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoAvaria, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaMotivoAvaria, Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>(unitOfWork, regraAutorizacaoAvaria, "AlcadasMotivoAvaria", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet motivoAvaria = repositorioMotivoAvaria.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = motivoAvaria ?? throw new ControllerException("Motivo da avaria não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaSetor, Dominio.Entidades.Setor>(unitOfWork, regraAutorizacaoAvaria, "AlcadasSetor", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Setor setor = repositorioSetor.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = setor ?? throw new ControllerException("Setor não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoAvaria, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Empresa/Filial não encontrada.");
                }));

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public override IActionResult Atualizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioMotivoAvaria = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                var repositorioSetor = new Repositorio.Setor(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoAvaria = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoAvaria.Initialize();

                PreencherRegra(regraAutorizacaoAvaria, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorMotivoAvaria = Request.GetBoolParam("UsarRegraPorMotivoAvaria");
                    regra.RegraPorSetor = Request.GetBoolParam("UsarRegraPorSetor");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoAvaria, regraAutorizacaoAvaria.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    var filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaMotivoAvaria, Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>(unitOfWork, regraAutorizacaoAvaria, regraAutorizacaoAvaria.AlcadasMotivoAvaria, "AlcadasMotivoAvaria", ((valorPropriedade, alcada) =>
                {
                    var motivoAvaria = repositorioMotivoAvaria.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = motivoAvaria ?? throw new ControllerException("Motivo da avaria não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaSetor, Dominio.Entidades.Setor>(unitOfWork, regraAutorizacaoAvaria, regraAutorizacaoAvaria.AlcadasSetor, "AlcadasSetor", ((valorPropriedade, alcada) =>
                {
                    var setor = repositorioSetor.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = setor ?? throw new ControllerException("Setor não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoAvaria, regraAutorizacaoAvaria.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    var transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Empresa/Filial não encontrada.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoAvaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
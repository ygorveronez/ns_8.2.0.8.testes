using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize(new string[] { }, "Veiculos/RegraAutorizacaoCadastroVeiculo")]
    public class RegraAutorizacaoCadastroVeiculoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>
    {
		#region Construtores

		public RegraAutorizacaoCadastroVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo regra)
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
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                UsarRegraPorModeloVeicular = regra.RegraPorModeloVeicular,
                UsarRegraPorFilial = regra.RegraPorFilial,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasModeloVeicular = (from alcada in regra.AlcadasModeloVeicular select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaModeloVeicular, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoCadastroVeiculo = new Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo();

                PreencherRegra(regraAutorizacaoCadastroVeiculo, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorModeloVeicular = Request.GetBoolParam("UsarRegraPorModeloVeicular");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                }));

                repositorioRegra.Inserir(regraAutorizacaoCadastroVeiculo, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoCadastroVeiculo, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaModeloVeicular, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacaoCadastroVeiculo, "AlcadasModeloVeicular", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo Veicular não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoCadastroVeiculo, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrado.");
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
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo>(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoCadastroVeiculo = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoCadastroVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoCadastroVeiculo.Initialize();

                PreencherRegra(regraAutorizacaoCadastroVeiculo, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorModeloVeicular = Request.GetBoolParam("UsarRegraPorModeloVeicular");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoCadastroVeiculo, regraAutorizacaoCadastroVeiculo.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa Transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = Transportador ?? throw new ControllerException("Transportador não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaModeloVeicular, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacaoCadastroVeiculo, regraAutorizacaoCadastroVeiculo.AlcadasModeloVeicular, "AlcadasModeloVeicular", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo Veicular não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoCadastroVeiculo, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoCadastroVeiculo, Auditado);

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
using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { }, "GestaoPatio/RegrasAutorizacaoPesagem")]
    public class RegrasAutorizacaoPesagemController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem>
    {
		#region Construtores

		public RegrasAutorizacaoPesagemController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem regra)
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
                UsarRegraPorModeloVeicularCarga = regra.RegraPorModeloVeicularCarga,
                UsarRegraPorTipoCarga = regra.RegraPorTipoCarga,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasModeloVeicularCarga = (from alcada in regra.AlcadasModeloVeicularCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(alcada)).ToList(),
                AlcadasTipoCarga = (from alcada in regra.AlcadasTipoCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                TipoRegraAutorizacaoToleranciaPesagem = regra.TipoRegra
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem>(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem regraAutorizacao = new Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                regraAutorizacao.TipoRegra = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem>("TipoRegraAutorizacaoToleranciaPesagem");

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);


                AdicionarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo veicular de carga não encontrado.");
                }));


                AdicionarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem>(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                regraAutorizacao.TipoRegra = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem>("TipoRegraAutorizacaoToleranciaPesagem");

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasModeloVeicularCarga, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo veicular de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoCarga, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
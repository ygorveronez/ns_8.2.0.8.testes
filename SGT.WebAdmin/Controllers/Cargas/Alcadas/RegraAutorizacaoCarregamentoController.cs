using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { }, "Cargas/RegraAutorizacaoCarregamento")]
    public class RegraAutorizacaoCarregamentoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento>
    {
		#region Construtores

		public RegraAutorizacaoCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento regra)
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
                UsarRegraPorDiferencaValorApoliceTransportador = regra.RegraPorDiferencaValorApoliceTransportador,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorModeloVeicularCarga = regra.RegraPorModeloVeicularCarga,
                UsarRegraPorPercentualOcupacaoCubagem = regra.RegraPorPercentualOcupacaoCubagem,
                UsarRegraPorPercentualOcupacaoPallet = regra.RegraPorPercentualOcupacaoPallet,
                UsarRegraPorPercentualOcupacaoPeso = regra.RegraPorPercentualOcupacaoPeso,
                UsarRegraPorTipoCarga = regra.RegraPorTipoCarga,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasDiferencaValorApoliceTransportador = (from alcada in regra.AlcadasDiferencaValorApoliceTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador, decimal>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasModeloVeicularCarga = (from alcada in regra.AlcadasModeloVeicularCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(alcada)).ToList(),
                AlcadasPercentualOcupacaoCubagem = (from alcada in regra.AlcadasPercentualOcupacaoCubagem select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoCubagem, decimal>(alcada)).ToList(),
                AlcadasPercentualOcupacaoPallet = (from alcada in regra.AlcadasPercentualOcupacaoPallet select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet, decimal>(alcada)).ToList(),
                AlcadasPercentualOcupacaoPeso = (from alcada in regra.AlcadasPercentualOcupacaoPeso select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPeso, decimal>(alcada)).ToList(),
                AlcadasTipoCarga = (from alcada in regra.AlcadasTipoCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                EnviarLinkParaAprovacaoPorEmail = regra.EnviarLinkParaAprovacaoPorEmail
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento>(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento regraAutorizacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorDiferencaValorApoliceTransportador = Request.GetBoolParam("UsarRegraPorDiferencaValorApoliceTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorPercentualOcupacaoCubagem = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoCubagem");
                    regra.RegraPorPercentualOcupacaoPallet = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoPallet");
                    regra.RegraPorPercentualOcupacaoPeso = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoPeso");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                regraAutorizacao.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador, decimal>(unitOfWork, regraAutorizacao, "AlcadasDiferencaValorApoliceTransportador", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("A diferença no valor da apólice do transportador deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo veicular de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoCubagem, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualOcupacaoCubagem", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação da cubagem deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualOcupacaoPallet", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação de pallets deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPeso, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualOcupacaoPeso", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação do peso deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento>(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorDiferencaValorApoliceTransportador = Request.GetBoolParam("UsarRegraPorDiferencaValorApoliceTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorPercentualOcupacaoCubagem = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoCubagem");
                    regra.RegraPorPercentualOcupacaoPallet = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoPallet");
                    regra.RegraPorPercentualOcupacaoPeso = Request.GetBoolParam("UsarRegraPorPercentualOcupacaoPeso");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                regraAutorizacao.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValorApoliceTransportador, "AlcadasDiferencaValorApoliceTransportador", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("A diferença no valor da apólice do transportador deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasModeloVeicularCarga, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("Modelo veicular de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoCubagem, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualOcupacaoCubagem, "AlcadasPercentualOcupacaoCubagem", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação da cubagem deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualOcupacaoPallet, "AlcadasPercentualOcupacaoPallet", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação de pallets deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPeso, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualOcupacaoPeso, "AlcadasPercentualOcupacaoPeso", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual de ocupação do peso deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoCarga, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
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
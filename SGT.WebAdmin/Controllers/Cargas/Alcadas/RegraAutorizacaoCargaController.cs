using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { }, "Cargas/RegraAutorizacaoCarga")]
    public class RegraAutorizacaoCargaController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>
    {
		#region Construtores

		public RegraAutorizacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga regra)
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
                regra.TipoRegra,
                UsarRegraPorComponenteFrete = regra.RegraPorComponenteFrete,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorModeloVeicularCarga = regra.RegraPorModeloVeicularCarga,
                UsarRegraPorPercentualAcrescimoValorTabelaFrete = regra.RegraPorPercentualAcrescimoValorTabelaFrete,
                UsarRegraPorPercentualDescontoValorTabelaFrete = regra.RegraPorPercentualDescontoValorTabelaFrete,
                UsarRegraPorPercentualDiferencaFreteLiquidoFreteTerceiro = regra.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro,
                UsarRegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro = regra.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro,
                UsarRegraPorTipoCarga = regra.RegraPorTipoCarga,
                UsarRegraPorTomador = regra.RegraPorTomador,
                UsarRegraPorPortoDestino = regra.RegraPorPortoDestino,
                UsarRegraPorPortoOrigem = regra.RegraPorPortoOrigem,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorValorFrete = regra.RegraPorValorFrete,
                UsarRegraPorPesoContainer = regra.RegraPorPesoContainer,
                UsarRegraPorMotivoSolicitacaoFrete = regra.RegraPorMotivoSolicitacaoFrete,
                UsarRegraPorValorAcrescimoValorTabelaFrete = regra.RegraPorValorAcrescimoValorTabelaFrete,
                UsarRegraPorPercentualFreteSobreNota = regra.RegraPorPercentualFreteSobreNota,
                UsarRegraPorDiferencaValorFrete = regra.RegraPorDiferencaValorFrete,
                UsarRegraPorAutorizacaoTipoTerceiro = regra.RegraPorAutorizacaoTipoTerceiro,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasComponenteFrete = (from alcada in regra.AlcadasComponenteFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasModeloVeicularCarga = (from alcada in regra.AlcadasModeloVeicularCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(alcada)).ToList(),
                AlcadasPercentualAcrescimoValorTabelaFrete = (from alcada in regra.AlcadasPercentualAcrescimoValorTabelaFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete, decimal>(alcada)).ToList(),
                AlcadasPercentualDescontoValorTabelaFrete = (from alcada in regra.AlcadasPercentualDescontoValorTabelaFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete, decimal>(alcada)).ToList(),
                AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro = (from alcada in regra.AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro, decimal>(alcada)).ToList(),
                AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro = (from alcada in regra.AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro, decimal>(alcada)).ToList(),
                AlcadasTipoCarga = (from alcada in regra.AlcadasTipoCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasValorFrete = (from alcada in regra.AlcadasValorFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorFrete, decimal>(alcada)).ToList(),
                AlcadasPesoContainer = (from alcada in regra.AlcadasPesoContainer select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPesoContainer, decimal>(alcada)).ToList(),
                AlcadasMotivoSolicitacaoFrete = (from alcada in regra.AlcadasMotivoSolicitacaoFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaMotivoSolicitacaoFrete, Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>(alcada)).ToList(),
                AlcadasTomador = (from alcada in regra.AlcadasTomador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTomador, Dominio.Entidades.Cliente>(alcada)).ToList(),
                AlcadasPortoDestino = (from alcada in regra.AlcadasPortoDestino select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoDestino, Dominio.Entidades.Embarcador.Pedidos.Porto>(alcada)).ToList(),
                AlcadasPortoOrigem = (from alcada in regra.AlcadasPortoOrigem select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto>(alcada)).ToList(),
                AlcadasValorAcrescimoValorTabelaFrete = (from alcada in regra.AlcadasValorAcrescimoValorTabelaFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete, decimal>(alcada)).ToList(),
                AlcadasPercentualFreteSobreNota = (from alcada in regra.AlcadasPercentualFreteSobreNota select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualFreteSobreNota, decimal>(alcada)).ToList(),
                AlcadasDiferencaValorFrete = (from alcada in regra.AlcadasDiferencaValorFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaDiferencaValorFrete, decimal>(alcada)).ToList(),
                AlcadasAutorizacaoTipoTerceiro = (from alcada in regra.AlcadasAutorizacaoTipoTerceiro select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaAutorizacaoTipoTerceiro, Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>(alcada)).ToList(),
                EnviarLinkParaAprovacaoPorEmail = regra.EnviarLinkParaAprovacaoPorEmail,
                ExigirInformarJustificativaCustoExtraCadastrado = regra.ExigirInformarJustificativaCustoExtraCadastrado,
                regra.ValidarAcrescimoValorTabelaFretePorComponenteFrete                
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga regraAutorizacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorComponenteFrete = Request.GetBoolParam("UsarRegraPorComponenteFrete");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorPercentualAcrescimoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorPercentualAcrescimoValorTabelaFrete");
                    regra.RegraPorPercentualDescontoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorPercentualDescontoValorTabelaFrete");
                    regra.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro = Request.GetBoolParam("UsarRegraPorPercentualDiferencaFreteLiquidoFreteTerceiro");
                    regra.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro = Request.GetBoolParam("UsarRegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorMotivoSolicitacaoFrete = Request.GetBoolParam("UsarRegraPorMotivoSolicitacaoFrete");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                    regra.RegraPorPesoContainer = Request.GetBoolParam("UsarRegraPorPesoContainer");
                    regra.TipoRegra = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga>("TipoRegra");
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorPortoOrigem = Request.GetBoolParam("UsarRegraPorPortoOrigem");
                    regra.RegraPorPortoDestino = Request.GetBoolParam("UsarRegraPorPortoDestino");
                    regra.RegraPorValorAcrescimoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorValorAcrescimoValorTabelaFrete");
                    regra.RegraPorPercentualFreteSobreNota = Request.GetBoolParam("UsarRegraPorPercentualFreteSobreNota");
                    regra.RegraPorDiferencaValorFrete = Request.GetBoolParam("UsarRegraPorDiferencaValorFrete");
                    regra.ValidarAcrescimoValorTabelaFretePorComponenteFrete = Request.GetBoolParam("ValidarAcrescimoValorTabelaFretePorComponenteFrete");
                    regra.RegraPorAutorizacaoTipoTerceiro = Request.GetBoolParam("UsarRegraPorAutorizacaoTipoTerceiro");
                }));

                regraAutorizacao.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                regraAutorizacao.ExigirInformarJustificativaCustoExtraCadastrado = Request.GetBoolParam("ExigirInformarJustificativaCustoExtraCadastrado");

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                var repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                var repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                var repositorioMotivoSolicitacaoFrete = new Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete(unitOfWork);
                var repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                var repositorioCliente = new Repositorio.Cliente(unitOfWork);
                var repositorioPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                var repositorioTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete>(unitOfWork, regraAutorizacao, "AlcadasComponenteFrete", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repositorioComponenteFrete.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = componenteFrete ?? throw new ControllerException("Componente de frete não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("modelo veicular de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualAcrescimoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualDescontoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacao, "AlcadasTomador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = tomador ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoDestino, Dominio.Entidades.Embarcador.Pedidos.Porto>(unitOfWork, regraAutorizacao, "AlcadasPortoDestino", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = repositorioPorto.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = portoDestino ?? throw new ControllerException("Porto de Destino não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto>(unitOfWork, regraAutorizacao, "AlcadasPortoOrigem", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repositorioPorto.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = portoOrigem ?? throw new ControllerException("Porto de Origem não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPesoContainer, decimal>(unitOfWork, regraAutorizacao, "AlcadasPesoContainer", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaMotivoSolicitacaoFrete, Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>(unitOfWork, regraAutorizacao, "AlcadasMotivoSolicitacaoFrete", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete motivo = repositorioMotivoSolicitacaoFrete.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = motivo ?? throw new ControllerException("Motivo da Solicitação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorAcrescimoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));


                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualFreteSobreNota, decimal>(unitOfWork, regraAutorizacao, "AlcadasPercentualFreteSobreNota", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaDiferencaValorFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValorFrete, "AlcadasDiferencaValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaAutorizacaoTipoTerceiro, Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>(unitOfWork, regraAutorizacao, "AlcadasAutorizacaoTipoTerceiro", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = repositorioTipoTerceiro.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoTerceiro ?? throw new ControllerException("Tido de Terceiro não encontrado.");
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

                int codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>(unitOfWork);
                var regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorComponenteFrete = Request.GetBoolParam("UsarRegraPorComponenteFrete");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorModeloVeicularCarga = Request.GetBoolParam("UsarRegraPorModeloVeicularCarga");
                    regra.RegraPorPercentualAcrescimoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorPercentualAcrescimoValorTabelaFrete");
                    regra.RegraPorPercentualDescontoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorPercentualDescontoValorTabelaFrete");
                    regra.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro = Request.GetBoolParam("UsarRegraPorPercentualDiferencaFreteLiquidoFreteTerceiro");
                    regra.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro = Request.GetBoolParam("UsarRegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                    regra.RegraPorPesoContainer = Request.GetBoolParam("UsarRegraPorPesoContainer");
                    regra.RegraPorMotivoSolicitacaoFrete = Request.GetBoolParam("UsarRegraPorMotivoSolicitacaoFrete");
                    regra.TipoRegra = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga>("TipoRegra");
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorPortoOrigem = Request.GetBoolParam("UsarRegraPorPortoOrigem");
                    regra.RegraPorPortoDestino = Request.GetBoolParam("UsarRegraPorPortoDestino");
                    regra.RegraPorValorAcrescimoValorTabelaFrete = Request.GetBoolParam("UsarRegraPorValorAcrescimoValorTabelaFrete");
                    regra.RegraPorPercentualFreteSobreNota = Request.GetBoolParam("UsarRegraPorPercentualFreteSobreNota");
                    regra.RegraPorDiferencaValorFrete = Request.GetBoolParam("UsarRegraPorDiferencaValorFrete");
                    regra.ValidarAcrescimoValorTabelaFretePorComponenteFrete = Request.GetBoolParam("ValidarAcrescimoValorTabelaFretePorComponenteFrete");
                    regra.RegraPorAutorizacaoTipoTerceiro = Request.GetBoolParam("UsarRegraPorAutorizacaoTipoTerceiro");
                }));

                regraAutorizacao.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                regraAutorizacao.ExigirInformarJustificativaCustoExtraCadastrado = Request.GetBoolParam("ExigirInformarJustificativaCustoExtraCadastrado");

                var repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                var repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                var repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                var repositorioMotivoSolicitacaoFrete = new Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete(unitOfWork);
                var repositorioCliente = new Repositorio.Cliente(unitOfWork);
                var repositorioPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                var repositorioTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasComponenteFrete, "AlcadasComponenteFrete", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repositorioComponenteFrete.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = componenteFrete ?? throw new ControllerException("Componente de frete não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasModeloVeicularCarga, "AlcadasModeloVeicularCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = modeloVeicularCarga ?? throw new ControllerException("modelo veicular de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualAcrescimoValorTabelaFrete, "AlcadasPercentualAcrescimoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualDescontoValorTabelaFrete, "AlcadasPercentualDescontoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro, "AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro, "AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoCarga, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTomador, "AlcadasTomador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = tomador ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoDestino, Dominio.Entidades.Embarcador.Pedidos.Porto>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPortoDestino, "AlcadasPortoDestino", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = repositorioPorto.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = portoDestino ?? throw new ControllerException("Porto de Destino não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPortoOrigem, "AlcadasPortoOrigem", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repositorioPorto.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = portoOrigem ?? throw new ControllerException("Porto de Origem não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaAutorizacaoTipoTerceiro, Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasAutorizacaoTipoTerceiro, "AlcadasAutorizacaoTipoTerceiro", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = repositorioTipoTerceiro.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoTerceiro ?? throw new ControllerException("Tipo de Terceiro não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasValorFrete, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPesoContainer, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPesoContainer, "AlcadasPesoContainer", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaMotivoSolicitacaoFrete, Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasMotivoSolicitacaoFrete, "AlcadasMotivoSolicitacaoFrete", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete motivo = repositorioMotivoSolicitacaoFrete.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = motivo ?? throw new ControllerException("Motivo da Solicitação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasValorAcrescimoValorTabelaFrete, "AlcadasValorAcrescimoValorTabelaFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualFreteSobreNota, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasPercentualFreteSobreNota, "AlcadasPercentualFreteSobreNota", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O percentual deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaDiferencaValorFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValorFrete, "AlcadasDiferencaValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
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
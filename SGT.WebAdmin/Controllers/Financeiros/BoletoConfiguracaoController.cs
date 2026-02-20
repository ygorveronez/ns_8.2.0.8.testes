using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoConfiguracao")]
    public class BoletoConfiguracaoController : BaseController
    {
		#region Construtores

		public BoletoConfiguracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string numeroAgencia = Request.Params("NumeroAgencia");
                if (string.IsNullOrWhiteSpace(numeroAgencia))
                    numeroAgencia = Request.Params("Descricao");
                string numeroConta = Request.Params("NumeroConta");
                string boletoConfiguracao = Request.Params("Plano");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco banco;
                Enum.TryParse(Request.Params("Banco"), out banco);

                string numeroBanco = "";
                int intNumeroBanco = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("Descricao")))
                {
                    int.TryParse(Request.Params("Descricao"), out intNumeroBanco);
                    if (intNumeroBanco > 0)
                    {
                        numeroAgencia = "";
                        numeroBanco = Request.Params("Descricao");
                    }
                }

                bool? utilizaConfiguracaoPagamentoEletronico = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("UtilizaConfiguracaoPagamentoEletronico")))
                {
                    bool pagamentoEletronico = false;
                    bool.TryParse(Request.Params("UtilizaConfiguracaoPagamentoEletronico"), out pagamentoEletronico);
                    utilizaConfiguracaoPagamentoEletronico = pagamentoEletronico;
                }

                int situacao = Request.GetIntParam("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.BoletoConfiguracao.NumeroBanco, "NumeroBanco", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.BoletoConfiguracao.Banco, "DescricaoBanco", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.BoletoConfiguracao.Agencia, "NumeroAgencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.BoletoConfiguracao.Conta, "NumeroConta", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.BoletoConfiguracao.Boleto, "DescricaoBoletoBanco", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 7, Models.Grid.Align.center, false, situacao == 0 ? true : false);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao> listaPlanoDeConta = repBoletoConfiguracao.Consultar(utilizaConfiguracaoPagamentoEletronico, numeroBanco, codigoEmpresa, numeroAgencia, numeroConta, banco, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBoletoConfiguracao.ContarConsulta(utilizaConfiguracaoPagamentoEletronico, numeroBanco, codigoEmpresa, numeroAgencia, numeroConta, banco, situacao));
                var lista = (from p in listaPlanoDeConta
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NumeroBanco,
                                 p.DescricaoBanco,
                                 p.NumeroAgencia,
                                 p.NumeroConta,
                                 DescricaoBoletoBanco = p.BoletoBanco.ObterDescricao(),
                                 Situacao = SituacaoAtivaPesquisaHelper.ObterDescricao(p.Situacao)
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco banco;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo caracteristica;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao responsavel;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira tipoCarteira;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB tipoCNAB;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout tipoLayout;

                Enum.TryParse(Request.Params("Banco"), out banco);
                Enum.TryParse(Request.Params("CaracteristicaTitulo"), out caracteristica);
                Enum.TryParse(Request.Params("ResponsavelEmissao"), out responsavel);
                Enum.TryParse(Request.Params("TipoCarteira"), out tipoCarteira);
                Enum.TryParse(Request.Params("TipoCNAB"), out tipoCNAB);
                Enum.TryParse(Request.Params("TipoLayout"), out tipoLayout);

                string numeroBanco = Request.Params("NumeroBanco");
                string digitoBanco = Request.Params("DigitoBanco");
                string nomeBanco = Request.Params("NomeBanco");
                string numeroAgencia = Request.Params("NumeroAgencia");
                string digitoAgencia = Request.Params("DigitoAgencia");
                string codigoCedente = Request.Params("CodigoCedente");
                string codigoTransmissao = Request.Params("CodigoTransmissao");
                string numeroConta = Request.Params("NumeroConta");
                string digitoConta = Request.Params("DigitoConta");
                string numeroConvenio = Request.Params("NumeroConvenio");
                string modalidade = Request.Params("Modalidade");
                string especieTitulo = Request.Params("EspecieTitulo");
                string carteiraTitulo = Request.Params("CarteiraTitulo");
                string diasProtesto = Request.Params("DiasProtesto");
                string codigoInstrucaoMovimento = Request.Params("CodigoInstrucaoMovimento");
                string codigoBanco = Request.Params("CodigoBanco");
                string codigoProtesto = Request.Params("CodigoProtesto");
                string codigoMulta = Request.Params("CodigoMulta");
                string caminhoRemessa = Request.Params("CaminhoRemessa");
                string localPagamento = Request.Params("LocalPagamento");
                string instrucao = Request.Params("Instrucao");
                string assuntoEmail = Request.Params("AssuntoEmail");
                string mensagemEmail = Request.Params("MensagemEmail");
                string digitoAgenciaConta = Request.Params("DigitoAgenciaConta");

                int tagBanco, tamanhoMaximoNossoNumero, proximoNossoNumero, tagTitulo = 0, numeroInicialSequenciaRemessa = 0, acrescimoDiaDataMoraJuros = 0, acrescimoDiaDataMulta = 0;
                int.TryParse(Request.Params("TagBanco"), out tagBanco);
                int.TryParse(Request.Params("TagTitulo"), out tagTitulo);
                int.TryParse(Request.Params("TamanhoMaximoNossoNumero"), out tamanhoMaximoNossoNumero);
                int.TryParse(Request.Params("ProximoNossoNumero"), out proximoNossoNumero);
                int.TryParse(Request.Params("NumeroInicialSequenciaRemessa"), out numeroInicialSequenciaRemessa);
                int.TryParse(Request.Params("AcrescimoDiaDataMoraJuros"), out acrescimoDiaDataMoraJuros);
                int.TryParse(Request.Params("AcrescimoDiaDataMulta"), out acrescimoDiaDataMulta);

                decimal valorJurosAoMes, percentualMulta = 0;
                decimal.TryParse(Request.Params("ValorJurosAoMes"), out valorJurosAoMes);
                decimal.TryParse(Request.Params("PercentualMulta"), out percentualMulta);

                double codigoBeneficiario, codigoSacador = 0, codigoPortador = 0;
                double.TryParse(Request.Params("Beneficiario"), out codigoBeneficiario);
                double.TryParse(Request.Params("Sacador"), out codigoSacador);
                double.TryParse(Request.Params("Portador"), out codigoPortador);

                bool aceite, liquidarComValorIntegral = false, utilizaConfiguracaoPagamentoEletronico = false;
                bool.TryParse(Request.Params("Aceite"), out aceite);
                bool.TryParse(Request.Params("LiquidarComValorIntegral"), out liquidarComValorIntegral);
                bool.TryParse(Request.Params("UtilizaConfiguracaoPagamentoEletronico"), out utilizaConfiguracaoPagamentoEletronico);

                int codigoPlanoConta, codigoTipoMovimentoLiquidacao, codigoTipoMovimentoBaixa, codigoTipoMovimentoTarifa, codigoTipoMovimentoJuros, codigoTipoMovimentoDesconto, codigoEmpresa = 0; ;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);
                int.TryParse(Request.Params("TipoMovimentoLiquidacao"), out codigoTipoMovimentoLiquidacao);
                int.TryParse(Request.Params("TipoMovimentoBaixa"), out codigoTipoMovimentoBaixa);
                int.TryParse(Request.Params("TipoMovimentoTarifa"), out codigoTipoMovimentoTarifa);
                int.TryParse(Request.Params("TipoMovimentoJuros"), out codigoTipoMovimentoJuros);
                int.TryParse(Request.Params("TipoMovimentoDesconto"), out codigoTipoMovimentoDesconto);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = new Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao();
                if (codigoBeneficiario > 0)
                    boletoConfiguracao.Beneficiario = repCliente.BuscarPorCPFCNPJ(codigoBeneficiario);
                else
                    boletoConfiguracao.Beneficiario = null;
                if (codigoPortador > 0)
                    boletoConfiguracao.Portador = repCliente.BuscarPorCPFCNPJ(codigoPortador);
                else
                    boletoConfiguracao.Portador = null;
                if (codigoSacador > 0)
                    boletoConfiguracao.Sacador = repCliente.BuscarPorCPFCNPJ(codigoSacador);
                else
                    boletoConfiguracao.Sacador = null;
                boletoConfiguracao.Aceite = aceite;
                boletoConfiguracao.LiquidarComValorIntegral = liquidarComValorIntegral;
                boletoConfiguracao.UtilizaConfiguracaoPagamentoEletronico = utilizaConfiguracaoPagamentoEletronico;
                boletoConfiguracao.BoletoBanco = banco;
                boletoConfiguracao.CaminhoRemessa = caminhoRemessa;
                boletoConfiguracao.CaracteristicaTitulo = caracteristica;
                boletoConfiguracao.DescricaoBanco = nomeBanco;
                boletoConfiguracao.CarteiraTitulo = carteiraTitulo;
                boletoConfiguracao.CodigoBancoTitulo = codigoBanco;
                boletoConfiguracao.CodigoTransmissao = codigoTransmissao;
                boletoConfiguracao.CodigoCedente = codigoCedente;
                boletoConfiguracao.CodigoMulta = codigoMulta;
                boletoConfiguracao.CodigoProtesto = codigoProtesto;
                boletoConfiguracao.DigitoAgencia = digitoAgencia;
                boletoConfiguracao.DiasProtesto = diasProtesto;
                boletoConfiguracao.CodigoInstrucaoMovimento = codigoInstrucaoMovimento;
                boletoConfiguracao.DigitoBanco = digitoBanco;
                boletoConfiguracao.DigitoConta = digitoConta;
                boletoConfiguracao.EspecieTitulo = especieTitulo;
                boletoConfiguracao.Instrucao = instrucao;
                boletoConfiguracao.AssuntoEmail = assuntoEmail;
                boletoConfiguracao.MensagemEmail = mensagemEmail;
                boletoConfiguracao.JurosAoMes = valorJurosAoMes;
                boletoConfiguracao.LocalPagamento = localPagamento;
                boletoConfiguracao.Modalidade = modalidade;
                boletoConfiguracao.NumeroAgencia = numeroAgencia;
                boletoConfiguracao.NumeroBanco = numeroBanco;
                boletoConfiguracao.NumeroConta = numeroConta;
                boletoConfiguracao.NumeroConvenio = numeroConvenio;
                boletoConfiguracao.PercentualMulta = percentualMulta;
                boletoConfiguracao.ProximoNumeroNossoNumero = proximoNossoNumero;
                boletoConfiguracao.ResponsavelEmissao = responsavel;
                boletoConfiguracao.TagBanco = tagBanco;
                boletoConfiguracao.TagTitulo = tagTitulo;
                boletoConfiguracao.TamanhoMaximoNossoNumero = tamanhoMaximoNossoNumero;
                boletoConfiguracao.TipoCarteira = tipoCarteira;
                boletoConfiguracao.AcrescimoDiaDataMoraJuros = acrescimoDiaDataMoraJuros;
                boletoConfiguracao.AcrescimoDiaDataMulta = acrescimoDiaDataMulta;
                boletoConfiguracao.QuantidadeDiasRecebimentoAposVencimento = Request.GetIntParam("QuantidadeDiasRecebimentoAposVencimento");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    boletoConfiguracao.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                else
                    boletoConfiguracao.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                boletoConfiguracao.BoletoTipoCNAB = tipoCNAB;
                boletoConfiguracao.BoletoTipoLayout = tipoLayout;
                boletoConfiguracao.NumeroInicialSequenciaRemessa = numeroInicialSequenciaRemessa;

                if (codigoPlanoConta > 0)
                    boletoConfiguracao.PlanoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                else
                    boletoConfiguracao.PlanoConta = null;
                if (codigoTipoMovimentoLiquidacao > 0)
                    boletoConfiguracao.TipoMovimentoLiquidacao = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoLiquidacao);
                else
                    boletoConfiguracao.TipoMovimentoLiquidacao = null;
                if (codigoTipoMovimentoBaixa > 0)
                    boletoConfiguracao.TipoMovimentoBaixa = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaixa);
                else
                    boletoConfiguracao.TipoMovimentoBaixa = null;
                if (codigoTipoMovimentoTarifa > 0)
                    boletoConfiguracao.TipoMovimentoTarifa = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTarifa);
                else
                    boletoConfiguracao.TipoMovimentoTarifa = null;
                if (codigoTipoMovimentoJuros > 0)
                    boletoConfiguracao.TipoMovimentoJuros = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoJuros);
                else
                    boletoConfiguracao.TipoMovimentoJuros = null;
                if (codigoTipoMovimentoDesconto > 0)
                    boletoConfiguracao.TipoMovimentoDesconto = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoDesconto);
                else
                    boletoConfiguracao.TipoMovimentoDesconto = null;
                boletoConfiguracao.DigitoAgenciaConta = digitoAgenciaConta;

                boletoConfiguracao.CodigoFinalidadeTED = Request.GetStringParam("CodigoFinalidadeTED");
                boletoConfiguracao.Situacao = Request.GetBoolParam("Situacao");

                repBoletoConfiguracao.Inserir(boletoConfiguracao, Auditado);
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
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco banco;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoCaracteristicaTitulo caracteristica;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoResponsavelEmissao responsavel;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCarteira tipoCarteira;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB tipoCNAB;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoLayout tipoLayout;

                Enum.TryParse(Request.Params("Banco"), out banco);
                Enum.TryParse(Request.Params("CaracteristicaTitulo"), out caracteristica);
                Enum.TryParse(Request.Params("ResponsavelEmissao"), out responsavel);
                Enum.TryParse(Request.Params("TipoCarteira"), out tipoCarteira);
                Enum.TryParse(Request.Params("TipoCNAB"), out tipoCNAB);
                Enum.TryParse(Request.Params("TipoLayout"), out tipoLayout);

                string numeroBanco = Request.Params("NumeroBanco");
                string digitoBanco = Request.Params("DigitoBanco");
                string nomeBanco = Request.Params("NomeBanco");
                string numeroAgencia = Request.Params("NumeroAgencia");
                string digitoAgencia = Request.Params("DigitoAgencia");
                string codigoCedente = Request.Params("CodigoCedente");
                string codigoTransmissao = Request.Params("CodigoTransmissao");
                string numeroConta = Request.Params("NumeroConta");
                string digitoConta = Request.Params("DigitoConta");
                string numeroConvenio = Request.Params("NumeroConvenio");
                string modalidade = Request.Params("Modalidade");
                string especieTitulo = Request.Params("EspecieTitulo");
                string carteiraTitulo = Request.Params("CarteiraTitulo");
                string diasProtesto = Request.Params("DiasProtesto");
                string codigoInstrucaoMovimento = Request.Params("CodigoInstrucaoMovimento");
                string codigoBanco = Request.Params("CodigoBanco");
                string codigoProtesto = Request.Params("CodigoProtesto");
                string codigoMulta = Request.Params("CodigoMulta");
                string caminhoRemessa = Request.Params("CaminhoRemessa");
                string localPagamento = Request.Params("LocalPagamento");
                string instrucao = Request.Params("Instrucao");
                string assuntoEmail = Request.Params("AssuntoEmail");
                string mensagemEmail = Request.Params("MensagemEmail");
                string digitoAgenciaConta = Request.Params("DigitoAgenciaConta");

                int tagBanco, tamanhoMaximoNossoNumero, proximoNossoNumero, tagTitulo = 0, numeroInicialSequenciaRemessa = 0, acrescimoDiaDataMoraJuros = 0, acrescimoDiaDataMulta = 0;
                int.TryParse(Request.Params("TagBanco"), out tagBanco);
                int.TryParse(Request.Params("TagTitulo"), out tagTitulo);
                int.TryParse(Request.Params("TamanhoMaximoNossoNumero"), out tamanhoMaximoNossoNumero);
                int.TryParse(Request.Params("ProximoNossoNumero"), out proximoNossoNumero);
                int.TryParse(Request.Params("NumeroInicialSequenciaRemessa"), out numeroInicialSequenciaRemessa);
                int.TryParse(Request.Params("AcrescimoDiaDataMoraJuros"), out acrescimoDiaDataMoraJuros);
                int.TryParse(Request.Params("AcrescimoDiaDataMulta"), out acrescimoDiaDataMulta);

                decimal valorJurosAoMes, percentualMulta = 0;
                decimal.TryParse(Request.Params("ValorJurosAoMes"), out valorJurosAoMes);
                decimal.TryParse(Request.Params("PercentualMulta"), out percentualMulta);

                double codigoBeneficiario, codigoSacador = 0, codigoPortador = 0;
                double.TryParse(Request.Params("Beneficiario"), out codigoBeneficiario);
                double.TryParse(Request.Params("Sacador"), out codigoSacador);
                double.TryParse(Request.Params("Portador"), out codigoPortador);

                bool aceite, liquidarComValorIntegral = false, utilizaConfiguracaoPagamentoEletronico = false;
                bool.TryParse(Request.Params("Aceite"), out aceite);
                bool.TryParse(Request.Params("LiquidarComValorIntegral"), out liquidarComValorIntegral);
                bool.TryParse(Request.Params("UtilizaConfiguracaoPagamentoEletronico"), out utilizaConfiguracaoPagamentoEletronico);

                int codigoPlanoConta, codigoTipoMovimentoLiquidacao, codigoTipoMovimentoBaixa, codigoTipoMovimentoTarifa, codigoTipoMovimentoJuros, codigoTipoMovimentoDesconto, codigoEmpresa = 0; ;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);
                int.TryParse(Request.Params("TipoMovimentoLiquidacao"), out codigoTipoMovimentoLiquidacao);
                int.TryParse(Request.Params("TipoMovimentoBaixa"), out codigoTipoMovimentoBaixa);
                int.TryParse(Request.Params("TipoMovimentoTarifa"), out codigoTipoMovimentoTarifa);
                int.TryParse(Request.Params("TipoMovimentoJuros"), out codigoTipoMovimentoJuros);
                int.TryParse(Request.Params("TipoMovimentoDesconto"), out codigoTipoMovimentoDesconto);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                if (codigoBeneficiario > 0)
                    boletoConfiguracao.Beneficiario = repCliente.BuscarPorCPFCNPJ(codigoBeneficiario);
                else
                    boletoConfiguracao.Beneficiario = null;
                if (codigoPortador > 0)
                    boletoConfiguracao.Portador = repCliente.BuscarPorCPFCNPJ(codigoPortador);
                else
                    boletoConfiguracao.Portador = null;
                if (codigoSacador > 0)
                    boletoConfiguracao.Sacador = repCliente.BuscarPorCPFCNPJ(codigoSacador);
                else
                    boletoConfiguracao.Sacador = null;
                boletoConfiguracao.Aceite = aceite;
                boletoConfiguracao.LiquidarComValorIntegral = liquidarComValorIntegral;
                boletoConfiguracao.UtilizaConfiguracaoPagamentoEletronico = utilizaConfiguracaoPagamentoEletronico;
                boletoConfiguracao.BoletoBanco = banco;
                boletoConfiguracao.CaminhoRemessa = caminhoRemessa;
                boletoConfiguracao.CaracteristicaTitulo = caracteristica;
                boletoConfiguracao.DescricaoBanco = nomeBanco;
                boletoConfiguracao.CarteiraTitulo = carteiraTitulo;
                boletoConfiguracao.CodigoBancoTitulo = codigoBanco;
                boletoConfiguracao.CodigoCedente = codigoCedente;
                boletoConfiguracao.CodigoMulta = codigoMulta;
                boletoConfiguracao.CodigoProtesto = codigoProtesto;
                boletoConfiguracao.CodigoTransmissao = codigoTransmissao;
                boletoConfiguracao.DiasProtesto = diasProtesto;
                boletoConfiguracao.CodigoInstrucaoMovimento = codigoInstrucaoMovimento;
                boletoConfiguracao.DigitoAgencia = digitoAgencia;
                boletoConfiguracao.DigitoBanco = digitoBanco;
                boletoConfiguracao.DigitoConta = digitoConta;
                boletoConfiguracao.EspecieTitulo = especieTitulo;
                boletoConfiguracao.Instrucao = instrucao;
                boletoConfiguracao.AssuntoEmail = assuntoEmail;
                boletoConfiguracao.MensagemEmail = mensagemEmail;
                boletoConfiguracao.JurosAoMes = valorJurosAoMes;
                boletoConfiguracao.LocalPagamento = localPagamento;
                boletoConfiguracao.Modalidade = modalidade;
                boletoConfiguracao.NumeroAgencia = numeroAgencia;
                boletoConfiguracao.NumeroBanco = numeroBanco;
                boletoConfiguracao.NumeroConta = numeroConta;
                boletoConfiguracao.NumeroConvenio = numeroConvenio;
                boletoConfiguracao.PercentualMulta = percentualMulta;
                boletoConfiguracao.ProximoNumeroNossoNumero = proximoNossoNumero;
                boletoConfiguracao.ResponsavelEmissao = responsavel;
                boletoConfiguracao.TagBanco = tagBanco;
                boletoConfiguracao.TagTitulo = tagTitulo;
                boletoConfiguracao.TamanhoMaximoNossoNumero = tamanhoMaximoNossoNumero;
                boletoConfiguracao.TipoCarteira = tipoCarteira;
                boletoConfiguracao.AcrescimoDiaDataMoraJuros = acrescimoDiaDataMoraJuros;
                boletoConfiguracao.AcrescimoDiaDataMulta = acrescimoDiaDataMulta;
                boletoConfiguracao.QuantidadeDiasRecebimentoAposVencimento = Request.GetIntParam("QuantidadeDiasRecebimentoAposVencimento");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    boletoConfiguracao.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                else
                    boletoConfiguracao.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                boletoConfiguracao.BoletoTipoCNAB = tipoCNAB;
                boletoConfiguracao.BoletoTipoLayout = tipoLayout;
                boletoConfiguracao.NumeroInicialSequenciaRemessa = numeroInicialSequenciaRemessa;

                if (codigoPlanoConta > 0)
                    boletoConfiguracao.PlanoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                else
                    boletoConfiguracao.PlanoConta = null;
                if (codigoTipoMovimentoLiquidacao > 0)
                    boletoConfiguracao.TipoMovimentoLiquidacao = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoLiquidacao);
                else
                    boletoConfiguracao.TipoMovimentoLiquidacao = null;
                if (codigoTipoMovimentoBaixa > 0)
                    boletoConfiguracao.TipoMovimentoBaixa = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaixa);
                else
                    boletoConfiguracao.TipoMovimentoBaixa = null;
                if (codigoTipoMovimentoTarifa > 0)
                    boletoConfiguracao.TipoMovimentoTarifa = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTarifa);
                else
                    boletoConfiguracao.TipoMovimentoTarifa = null;
                if (codigoTipoMovimentoJuros > 0)
                    boletoConfiguracao.TipoMovimentoJuros = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoJuros);
                else
                    boletoConfiguracao.TipoMovimentoJuros = null;
                if (codigoTipoMovimentoDesconto > 0)
                    boletoConfiguracao.TipoMovimentoDesconto = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoDesconto);
                else
                    boletoConfiguracao.TipoMovimentoDesconto = null;
                boletoConfiguracao.DigitoAgenciaConta = digitoAgenciaConta;

                boletoConfiguracao.CodigoFinalidadeTED = Request.GetStringParam("CodigoFinalidadeTED");
                boletoConfiguracao.Situacao = Request.GetBoolParam("Situacao");

                repBoletoConfiguracao.Atualizar(boletoConfiguracao, Auditado);
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
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repPlanoDeConta = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repPlanoDeConta.BuscarPorCodigo(codigo);
                var dynBoletoConfiguracao = new
                {
                    Banco = boletoConfiguracao.BoletoBanco,
                    boletoConfiguracao.CaminhoRemessa,
                    boletoConfiguracao.CaracteristicaTitulo,
                    boletoConfiguracao.CarteiraTitulo,
                    CodigoBanco = boletoConfiguracao.CodigoBancoTitulo,
                    boletoConfiguracao.CodigoCedente,
                    boletoConfiguracao.CodigoMulta,
                    boletoConfiguracao.CodigoProtesto,
                    boletoConfiguracao.CodigoTransmissao,
                    NomeBanco = boletoConfiguracao.DescricaoBanco,
                    boletoConfiguracao.DiasProtesto,
                    boletoConfiguracao.CodigoInstrucaoMovimento,
                    boletoConfiguracao.DigitoAgencia,
                    boletoConfiguracao.DigitoBanco,
                    boletoConfiguracao.DigitoConta,
                    boletoConfiguracao.EspecieTitulo,
                    boletoConfiguracao.Instrucao,
                    boletoConfiguracao.AssuntoEmail,
                    boletoConfiguracao.MensagemEmail,
                    ValorJurosAoMes = boletoConfiguracao.JurosAoMes,
                    boletoConfiguracao.LocalPagamento,
                    boletoConfiguracao.Modalidade,
                    boletoConfiguracao.NumeroAgencia,
                    boletoConfiguracao.NumeroBanco,
                    boletoConfiguracao.NumeroConta,
                    boletoConfiguracao.NumeroConvenio,
                    PercentualMulta = boletoConfiguracao.PercentualMulta,
                    ProximoNossoNumero = boletoConfiguracao.ProximoNumeroNossoNumero,
                    boletoConfiguracao.ResponsavelEmissao,
                    boletoConfiguracao.TagBanco,
                    boletoConfiguracao.TagTitulo,
                    boletoConfiguracao.TamanhoMaximoNossoNumero,
                    boletoConfiguracao.TipoCarteira,
                    boletoConfiguracao.Codigo,
                    TipoCNAB = boletoConfiguracao.BoletoTipoCNAB,
                    TipoLayout = boletoConfiguracao.BoletoTipoLayout,
                    boletoConfiguracao.Aceite,
                    boletoConfiguracao.NumeroInicialSequenciaRemessa,
                    Beneficiario = new { Codigo = boletoConfiguracao.Beneficiario != null ? boletoConfiguracao.Beneficiario.Codigo : 0, Descricao = boletoConfiguracao.Beneficiario != null ? boletoConfiguracao.Beneficiario.Nome : "" },
                    Sacador = new { Codigo = boletoConfiguracao.Sacador != null ? boletoConfiguracao.Sacador.Codigo : 0, Descricao = boletoConfiguracao.Sacador != null ? boletoConfiguracao.Sacador.Nome : "" },
                    Portador = new { Codigo = boletoConfiguracao.Portador != null ? boletoConfiguracao.Portador.Codigo : 0, Descricao = boletoConfiguracao.Portador != null ? boletoConfiguracao.Portador.Nome : "" },
                    PlanoConta = new { Codigo = boletoConfiguracao.PlanoConta != null ? boletoConfiguracao.PlanoConta.Codigo : 0, Descricao = boletoConfiguracao.PlanoConta != null ? boletoConfiguracao.PlanoConta.Descricao : "" },
                    TipoMovimentoLiquidacao = new { Codigo = boletoConfiguracao.TipoMovimentoLiquidacao != null ? boletoConfiguracao.TipoMovimentoLiquidacao.Codigo : 0, Descricao = boletoConfiguracao.TipoMovimentoLiquidacao != null ? boletoConfiguracao.TipoMovimentoLiquidacao.Descricao : "" },
                    TipoMovimentoBaixa = new { Codigo = boletoConfiguracao.TipoMovimentoBaixa != null ? boletoConfiguracao.TipoMovimentoBaixa.Codigo : 0, Descricao = boletoConfiguracao.TipoMovimentoBaixa != null ? boletoConfiguracao.TipoMovimentoBaixa.Descricao : "" },
                    TipoMovimentoTarifa = new { Codigo = boletoConfiguracao.TipoMovimentoTarifa != null ? boletoConfiguracao.TipoMovimentoTarifa.Codigo : 0, Descricao = boletoConfiguracao.TipoMovimentoTarifa != null ? boletoConfiguracao.TipoMovimentoTarifa.Descricao : "" },
                    TipoMovimentoJuros = new { Codigo = boletoConfiguracao.TipoMovimentoJuros != null ? boletoConfiguracao.TipoMovimentoJuros.Codigo : 0, Descricao = boletoConfiguracao.TipoMovimentoJuros != null ? boletoConfiguracao.TipoMovimentoJuros.Descricao : "" },
                    TipoMovimentoDesconto = new { Codigo = boletoConfiguracao.TipoMovimentoDesconto != null ? boletoConfiguracao.TipoMovimentoDesconto.Codigo : 0, Descricao = boletoConfiguracao.TipoMovimentoDesconto != null ? boletoConfiguracao.TipoMovimentoDesconto.Descricao : "" },
                    Empresa = new { Codigo = boletoConfiguracao.Empresa != null ? boletoConfiguracao.Empresa.Codigo : 0, Descricao = boletoConfiguracao.Empresa != null ? boletoConfiguracao.Empresa.NomeFantasia : "" },
                    boletoConfiguracao.LiquidarComValorIntegral,
                    boletoConfiguracao.UtilizaConfiguracaoPagamentoEletronico,
                    boletoConfiguracao.AcrescimoDiaDataMoraJuros,
                    boletoConfiguracao.AcrescimoDiaDataMulta,
                    boletoConfiguracao.DigitoAgenciaConta,
                    boletoConfiguracao.CodigoFinalidadeTED,
                    boletoConfiguracao.Situacao,
                    boletoConfiguracao.QuantidadeDiasRecebimentoAposVencimento
                };
                return new JsonpResult(dynBoletoConfiguracao);
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
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repPlanoDeConta = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracaoDeConta = repPlanoDeConta.BuscarPorCodigo(codigo);
                repPlanoDeConta.Deletar(boletoConfiguracaoDeConta, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { }, "Financeiros/RegraAutorizacaoPagamentoEletronico")]
    public class RegraAutorizacaoPagamentoEletronicoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico>
    {
		#region Construtores

		public RegraAutorizacaoPagamentoEletronicoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico regra)
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
                UsarRegraPorFornecedor = regra.RegraPorFornecedor,
                UsarRegraPorBoletoConfiguracao = regra.RegraPorBoletoConfiguracao,
                UsarRegraPorValor = regra.RegraPorValor,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFornecedor = (from alcada in regra.AlcadasFornecedor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaFornecedor, Dominio.Entidades.Cliente>(alcada)).ToList(),
                AlcadasBoletoConfiguracao = (from alcada in regra.AlcadasBoletoConfiguracao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaValor, decimal>(alcada)).ToList()
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repositorioBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico regraAutorizacaoOrdemServico = new Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico();

                PreencherRegra(regraAutorizacaoOrdemServico, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFornecedor = Request.GetBoolParam("UsarRegraPorFornecedor");
                    regra.RegraPorBoletoConfiguracao = Request.GetBoolParam("UsarRegraPorBoletoConfiguracao");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
                }));

                repositorioRegra.Inserir(regraAutorizacaoOrdemServico, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaFornecedor, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasFornecedor", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Fornecedor não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasBoletoConfiguracao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repositorioBoletoConfiguracao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = boletoConfiguracao ?? throw new ControllerException("Configuração do Boleto não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoOrdemServico, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
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

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repositorioBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Produto repositorioProduto = new Repositorio.Produto(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico regraAutorizacaoOrdemServico = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoOrdemServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoOrdemServico, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFornecedor = Request.GetBoolParam("UsarRegraPorFornecedor");
                    regra.RegraPorBoletoConfiguracao = Request.GetBoolParam("UsarRegraPorBoletoConfiguracao");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaFornecedor, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasFornecedor, "AlcadasFornecedor", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Fornecedor não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasBoletoConfiguracao, "AlcadasBoletoConfiguracao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repositorioBoletoConfiguracao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = boletoConfiguracao ?? throw new ControllerException("Configuração do Boleto não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoOrdemServico, regraAutorizacaoOrdemServico.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoOrdemServico, Auditado);

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
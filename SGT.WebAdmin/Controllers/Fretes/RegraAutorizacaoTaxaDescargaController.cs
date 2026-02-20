using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { }, "Fretes/RegraAutorizacaoTaxaDescarga")]
    public class RegraAutorizacaoTaxaDescargaController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>
    {
		#region Construtores

		public RegraAutorizacaoTaxaDescargaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga regra)
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
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorValor = regra.RegraPorValor,
                UsarRegraPorTipoDeCarga = regra.RegraPorTipoDeCarga,
                UsarRegraPorCliente = regra.RegraPorCliente,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaValor, decimal>(alcada)).ToList(),
                AlcadasTipoDeCarga = (from alcada in regra.AlcadasTipoDeCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoDeCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(alcada)).ToList(),
                AlcadasCliente = (from alcada in regra.AlcadasCliente select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaCliente, Dominio.Entidades.Cliente>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga regraAutorizacaoTaxaDescarga = new Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga();

                PreencherRegra(regraAutorizacaoTaxaDescarga, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorCliente = Request.GetBoolParam("UsarRegraPorCliente");
                    regra.RegraPorTipoDeCarga = Request.GetBoolParam("UsarRegraPorTipoDeCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                repositorioRegra.Inserir(regraAutorizacaoTaxaDescarga, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarCodigoPorCNPJ(valorPropriedade);

                    alcada.PropriedadeAlcada = empresa ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaCliente, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasCliente", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = cliente ?? throw new ControllerException("Cliente não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoDeCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasTipoDeCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoDeCarga ?? throw new ControllerException("Tipo de Carga não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoTaxaDescarga, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo operação não encontrado.");
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga regraAutorizacaoTaxaDescarga = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoTaxaDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoTaxaDescarga, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorCliente = Request.GetBoolParam("UsarRegraPorCliente");
                    regra.RegraPorTipoDeCarga = Request.GetBoolParam("UsarRegraPorTipoDeCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = empresa ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoDeCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasTipoDeCarga, "AlcadasTipoDeCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoDeCarga ?? throw new ControllerException("Tipo de carga não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaCliente, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoTaxaDescarga, regraAutorizacaoTaxaDescarga.AlcadasCliente, "AlcadasCliente", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = cliente ?? throw new ControllerException("Cliente não encontrado.");
                }));


                repositorioRegra.Atualizar(regraAutorizacaoTaxaDescarga, Auditado);

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

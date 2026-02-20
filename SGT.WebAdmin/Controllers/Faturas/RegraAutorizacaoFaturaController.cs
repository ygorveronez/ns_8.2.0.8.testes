using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize(new string[] { }, "Faturas/RegraAutorizacaoFatura", "SAC/AtendimentoCliente")]
    public class RegraAutorizacaoFaturaController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>
    {
		#region Construtores

		public RegraAutorizacaoFaturaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura regra)
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
                UsarRegraPorTomador = regra.RegraPorTomador,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorValor = regra.RegraPorValor,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTomador = (from alcada in regra.AlcadasTomador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTomador, Dominio.Entidades.Cliente>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaValor, decimal>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura regraAutorizacaoFatura = new Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura();

                PreencherRegra(regraAutorizacaoFatura, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                repositorioRegra.Inserir(regraAutorizacaoFatura, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoFatura, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoFatura, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoFatura, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoFatura, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de Operação não encontrada.");
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Produto repositorioProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura regraAutorizacaoFatura = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoFatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoFatura, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTomador = Request.GetBoolParam("UsarRegraPorTomador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTomador, Dominio.Entidades.Cliente>(unitOfWork, regraAutorizacaoFatura, regraAutorizacaoFatura.AlcadasTomador, "AlcadasTomador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ((double)valorPropriedade);

                    alcada.PropriedadeAlcada = fornecedor ?? throw new ControllerException("Tomador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoFatura, regraAutorizacaoFatura.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoFatura, regraAutorizacaoFatura.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoFatura, regraAutorizacaoFatura.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrada.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoFatura, Auditado);

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
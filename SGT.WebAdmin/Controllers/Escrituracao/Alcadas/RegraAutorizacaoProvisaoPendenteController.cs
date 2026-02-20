using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Alcadas
{
    [CustomAuthorize(new string[] { }, "Escrituracao/RegraAutorizacaoProvisaoPendente")]
    public class RegraAutorizacaoProvisaoPendenteController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>
    {
		#region Construtores

		public RegraAutorizacaoProvisaoPendenteController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.TipoGeracaoRegraProvisao,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorValorProvisao = regra.RegraPorValorProvisao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasValorProvisao = (from alcada in regra. AlcadaValorProvisao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaValorProvisao, decimal>(alcada)).ToList()
            });
        }

        protected override void PreencherAprovadores(Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regra, Repositorio.UnitOfWork unitOfWork)
        {
            regra.Aprovadores = new List<Dominio.Entidades.Usuario>();

                regra.Aprovadores = ObterAprovadores(regra, unitOfWork);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regraAutorizacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValorProvisao = Request.GetBoolParam("UsarRegraPorValorProvisao");
                    regra.TipoGeracaoRegraProvisao = Request.GetEnumParam<TipoGeracaoRegraProvisao>("TipoGeracaoRegraProvisao");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaValorProvisao, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorProvisao", ((valorPropriedade, alcada) =>
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValorProvisao = Request.GetBoolParam("UsarRegraPorValorProvisao");
                    regra.TipoGeracaoRegraProvisao = Request.GetEnumParam<TipoGeracaoRegraProvisao>("TipoGeracaoRegraProvisao");
                }));

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaValorProvisao, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadaValorProvisao, "AlcadasValorProvisao", ((valorPropriedade, alcada) =>
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
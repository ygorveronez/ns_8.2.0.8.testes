using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { }, "Escrituracao/RegraAutorizacaoPagamento")]
    public class RegraAutorizacaoPagamentoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento>
    {
		#region Construtores

		public RegraAutorizacaoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento regra)
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
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                UsarRegraPorValorPagamento = regra.RegraPorValorPagamento,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasValorPagamento = (from alcada in regra.AlcadasValorPagamento select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaValorPagamento, decimal>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento regraAutorizacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorValorPagamento = Request.GetBoolParam("UsarRegraPorValorPagamento");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaValorPagamento, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorPagamento", ((valorPropriedade, alcada) =>
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorValorPagamento = Request.GetBoolParam("UsarRegraPorValorPagamento");
                }));
                
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaValorPagamento, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasValorPagamento, "AlcadasValorPagamento", ((valorPropriedade, alcada) =>
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
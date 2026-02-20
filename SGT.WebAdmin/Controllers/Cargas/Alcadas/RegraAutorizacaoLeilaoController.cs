using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Leilao
{
    [CustomAuthorize(new string[] { }, "Cargas/RegraAutorizacaoLeilao")]
    public class RegraAutorizacaoLeilaoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao>
    {
		#region Construtores

		public RegraAutorizacaoLeilaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao regra)
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
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorValor = regra.RegraPorValor,
                UsarRegraPorCentroCarregamento = regra.RegraPorCentroCarregamento,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaValor, decimal>(alcada)).ToList(),
                AlcadasCentroCarregamento = (from alcada in regra.AlcadasCentroCarregamento select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaCentroCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao>(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao regraAutorizacaoLeilao = new Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao();

                PreencherRegra(regraAutorizacaoLeilao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorCentroCarregamento = Request.GetBoolParam("UsarRegraPorCentroCarregamento");
                }));

                repositorioRegra.Inserir(regraAutorizacaoLeilao, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoLeilao, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarCodigoPorCNPJ(valorPropriedade);

                    alcada.PropriedadeAlcada = empresa ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoLeilao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoLeilao, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaCentroCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>(unitOfWork, regraAutorizacaoLeilao, "AlcadasCentroCarregamento", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = centroCarregamento ?? throw new ControllerException("Tipo de Operação não encontrada.");
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao>(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao regraAutorizacaoLeilao = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoLeilao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoLeilao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                    regra.RegraPorCentroCarregamento = Request.GetBoolParam("UsarRegraPorCentroCarregamento");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoLeilao, regraAutorizacaoLeilao.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = empresa ?? throw new ControllerException("Transportador não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoLeilao, regraAutorizacaoLeilao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoLeilao, regraAutorizacaoLeilao.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaCentroCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>(unitOfWork, regraAutorizacaoLeilao, regraAutorizacaoLeilao.AlcadasCentroCarregamento, "AlcadasCentroCarregamento", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = centroCarregamento ?? throw new ControllerException("Tipo de operação não encontrada.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoLeilao, Auditado);

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
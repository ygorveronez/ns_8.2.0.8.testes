using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { }, "Logistica/RegraAutorizacaoTermoQuitacao")]
    public class RegraAutorizacaoTermoQuitacaoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao>
    {
		#region Construtores

		public RegraAutorizacaoTermoQuitacaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao regra)
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
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao>(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao regraAutorizacaoTermoQuitacao = new Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao();

                PreencherRegra(regraAutorizacaoTermoQuitacao, unitOfWork, ((regra) => {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                repositorioRegra.Inserir(regraAutorizacaoTermoQuitacao, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTermoQuitacao, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Empresa/Filial não encontrada.");
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
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao>(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao regraAutorizacaoTermoQuitacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoTermoQuitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoTermoQuitacao.Initialize();

                PreencherRegra(regraAutorizacaoTermoQuitacao, unitOfWork, ((regra) => {
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoTermoQuitacao, regraAutorizacaoTermoQuitacao.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Empresa/Filial não encontrada.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoTermoQuitacao, Auditado);

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
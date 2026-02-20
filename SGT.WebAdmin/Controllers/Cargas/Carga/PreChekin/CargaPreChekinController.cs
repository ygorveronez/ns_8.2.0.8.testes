using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.PreChekin
{
    [CustomAuthorize(new string[] { "ObterCteNfeTransferencia" }, "Cargas/Carga")]
    public class CargaPreChekinController : BaseController
    {
		#region Construtores

		public CargaPreChekinController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Globais

        public async Task<IActionResult> ConfirmarPreCheckin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigo);

                if (cargaCTe == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                bool cargaConsolidado = cargaCTe.Carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn;

                if (cargaConsolidado)
                {
                    //vamos validar se outras coletas ja foram emitidas, se nao emitidas nao pode confirmar.
                    List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentos = repStageAgrupamento.BuscarPorCargaDt(cargaCTe.Carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentos)
                    {
                        if (agrupamento.CargaGerada == null)//nao tem carga, nao tem CargaCte da filho.
                            return new JsonpResult(false, "Carga de sub trecho ainda não criada, não é permitido confirmar pre-checkin até todos os sub trechos gerados e emitidos");
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilho = repositorioCargaCTe.BuscarPrimeirPorCarga(agrupamento.CargaGerada.Codigo);
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(agrupamento.CargaGerada.Codigo);

                            if (cargaCTeFilho == null && cargaPedido != null && cargaPedido.StageRelevanteCusto != null)
                                return new JsonpResult(false, "Existem cargas de sub trecho que ainda não emitiram documento, não é permitido confirmar pre-checkin até todos os sub trechos serem emitidos");
                        }
                    }
                }

                cargaCTe.SituacaoCheckin = SituacaoCheckin.Confirmado;

                new Servicos.Embarcador.Carga.Carga(unitOfWork).AvancarEtapaSubCargasConsolidado(cargaCTe.Carga, cargaCTe, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, "Checkin confirmado", unitOfWork);
                repositorioCargaCTe.Atualizar(cargaCTe);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarRegistroIntegracaoConsolidado(cargaCTe.Carga, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);

                return new JsonpResult(ObterCargaCTeRetornar(cargaCTe));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao Confirmar o checkin.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCteNfeTransferencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                List<dynamic> listaCargaCTeRetornar = new List<dynamic>();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(listaCargaCTeRetornar);

                if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.Agrupadora)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioCargaCte.BuscarCargaCtePorCargaAtivos(carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                        listaCargaCTeRetornar.Add(ObterCargaCTeRetornar(cargaCTe));

                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentos = repositorioStageAgrupamento.BuscarPorCargaDt(codigoCarga);

                    if (agrupamentos.Count == 0)
                        return new JsonpResult(listaCargaCTeRetornar);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentos)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioCargaCte.BuscarCargaCtePorCargaAtivos(agrupamento.CargaGerada?.Codigo ?? 0);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                            listaCargaCTeRetornar.Add(ObterCargaCTeRetornar(cargaCTe));
                    }
                }

                return new JsonpResult(listaCargaCTeRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados de checkin");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecusarPreCheckin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigo);

                if (cargaCTe == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                bool cargaConsolidado = cargaCTe.Carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn;
                if (cargaConsolidado)
                {
                    //vamos validar se outras coletas ja foram emitidas, se nao emitidas nao pode confirmar.
                    List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentos = repStageAgrupamento.BuscarPorCargaDt(cargaCTe.Carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentos)
                    {
                        if (agrupamento.CargaGerada == null)//nao tem carga, nao tem CargaCte da filho.
                            return new JsonpResult(false, "Carga de sub trecho ainda não criada, não é permitido recusar pre-checkin até todos os sub trechos gerados e emitidos");
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilho = repositorioCargaCTe.BuscarPrimeirPorCarga(agrupamento.CargaGerada.Codigo);
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(agrupamento.CargaGerada.Codigo);

                            if (cargaCTeFilho == null && cargaPedido != null && cargaPedido.StageRelevanteCusto != null)
                                return new JsonpResult(false, "Existem cargas de sub trecho que ainda não emitiram documento, não é permitido recusar pre-checkin até todos os sub trechos serem emitidos");
                        }
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, "Recusa do checkin solicitada", unitOfWork);
                new Servicos.Embarcador.Frete.RecusaCheckinAprovacao(unitOfWork, Auditado).CriarAprovacao(cargaCTe, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterCargaCTeRetornar(cargaCTe));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao recusar o checkin.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Metodos Globais

        #region Métodos Privados

        private dynamic ObterCargaCTeRetornar(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            return new
            {
                cargaCTe.Codigo,
                cargaCTe.SituacaoCheckin,
                NumeroCTe = cargaCTe.CTe?.Numero ?? 0,
                Valor = cargaCTe.CTe?.ValorAReceber ?? 0,
                Peso = cargaCTe.CTe?.Peso ?? 0,
                SituacaoCheckinDescricao = cargaCTe.SituacaoCheckin.ObterDescricao()
            };
        }

        #endregion Métodos Privados
    }
}

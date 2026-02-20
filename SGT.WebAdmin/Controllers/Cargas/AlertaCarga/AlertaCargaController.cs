using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.AlertaCarga
{
    [CustomAuthorize("TorreControle/AcompanhamentoCarga")]
    public class AlertaCargaController : BaseController
    {
		#region Construtores

		public AlertaCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarPorCodigo(codigo, auditavel: true);
                if (cargaEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        cargaEvento.Codigo,
                        cargaEvento.TipoAlerta,
                        DataCadastro = cargaEvento.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss"),
                        DataEvento = cargaEvento.DataEvento.ToString("dd/MM/yyyy HH:mm:ss"),
                        cargaEvento.Descricao,
                        CodigoCarga = cargaEvento.Carga?.Codigo,
                        CodigoEntrega = cargaEvento.CargaEntrega?.Codigo,
                        CodigoChamado = cargaEvento.Chamado?.Codigo
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarETratarAlertaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool tratativa = Request.GetBoolParam("TratativaAutomatica");

                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarPorCodigo(codigo, auditavel: true);
                if (cargaEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (tratativa)
                {
                    servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Tratamento de alerta ao clicar no alerta");
                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                }

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        DescricaoAlerta = "Alerta: " + cargaEvento.TipoAlerta.ObterDescricao() + " Carga: " + cargaEvento.Carga.CodigoCargaEmbarcador,
                        cargaEvento.Codigo,
                        cargaEvento.TipoAlerta,
                        DataCadastro = cargaEvento.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss"),
                        DataEvento = cargaEvento.DataEvento.ToString("dd/MM/yyyy HH:mm:ss"),
                        cargaEvento.Descricao,
                        CodigoCarga = cargaEvento.Carga?.Codigo,
                        CodigoEntrega = cargaEvento.CargaEntrega?.Codigo,
                        CodigoChamado = cargaEvento.Chamado?.Codigo
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> EfetuarTratativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);
            try
            {
                int codigo = Request.GetIntParam("CodigoAlerta");
                string observacao = Request.GetStringParam("Observacao");
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, observacao);
                servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao efetuar a tratativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        #endregion


    }
}

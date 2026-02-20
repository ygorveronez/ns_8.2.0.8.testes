using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/CancelamentoCarga")]
    public class CancelamentoCargaCIOTController : BaseController
    {
		#region Construtores

		public CancelamentoCargaCIOTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);

                int codigoCarga = Request.GetIntParam("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Abertura", "DataAbertura", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 12, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Numero")
                    propOrdenar = "CIOT.Numero";

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> ciots = repCargaCIOT.Consultar(codigoCarga, cargaCIOT?.CIOT?.Codigo ?? 0, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int count = repCargaCIOT.ContarConsulta(codigoCarga, cargaCIOT?.CIOT?.Codigo ?? 0);

                grid.setarQuantidadeTotal(count);

                var retorno = (from obj in ciots
                               select new
                               {
                                   obj.CIOT.Codigo,
                                   obj.CIOT.Situacao,
                                   obj.CIOT.Numero,
                                   DataAbertura = obj.CIOT.DataAbertura.Value.ToString("dd/MM/yyyy HH:mm"),
                                   DescricaoStatus = obj.CIOT.DescricaoSituacao,
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuFalhaAoConsultarMDFes);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarCIOT()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);

                int codigo = Request.GetIntParam("Cancelamento");

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigo);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                if (cargaCIOT.CIOT.Situacao == SituacaoCIOT.Cancelado)
                    return new JsonpResult(false, false, "Não é possível reenviar um CIOT cancelado");

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;
                cargaCancelamento.EnviouCIOTCancelamento = false;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}

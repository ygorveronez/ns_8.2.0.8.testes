using Dominio.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/CampoCCe")]
    public class CampoCCeController : BaseController
    {
		#region Construtores

		public CampoCCeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nomeCampo = Request.Params("NomeCampo");
                string descricaoCampo = Request.Params("Descricao");
                string grupoCampo = Request.Params("GrupoCampo");
                string status = Request.Params("Status");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo", "GrupoCampo", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeCampo", 30, Models.Grid.Align.left, true);
                
                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                List<Dominio.Entidades.CampoCCe> listaCamposCCe = repCampoCCe.Consultar(descricaoCampo, nomeCampo, grupoCampo, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int countCamposCCe = repCampoCCe.ContarConsulta(descricaoCampo, nomeCampo, grupoCampo, status);

                grid.setarQuantidadeTotal(countCamposCCe);

                grid.AdicionaRows((from obj in listaCamposCCe select new { obj.Codigo, obj.GrupoCampo, obj.NomeCampo, obj.Descricao }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os campos da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);
                Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorCodigo(codigo);

                if (campoCCe == null)
                    return new JsonpResult(false, "Campo da CC-e não encontrado.");

                var retorno = new
                {
                    campoCCe.Codigo,
                    campoCCe.Descricao,
                    campoCCe.GrupoCampo,
                    campoCCe.IndicadorRepeticao,
                    campoCCe.NomeCampo,
                    campoCCe.QuantidadeCaracteres,
                    campoCCe.QuantidadeDecimais,
                    campoCCe.QuantidadeInteiros,
                    campoCCe.TipoCampo,
                    campoCCe.Status,
                    TipoCampoCCeAutomatica = campoCCe.TipoCampoCCeAutomatico
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes do campo da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, quantidadeCaracteres, quantidadeDecimais, quantidadeInteiros;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("QuantidadeCaracteres"), out quantidadeCaracteres);
                int.TryParse(Request.Params("QuantidadeDecimais"), out quantidadeDecimais);
                int.TryParse(Request.Params("QuantidadeInteiros"), out quantidadeInteiros);

                Dominio.Enumeradores.TipoCampoCCe tipoCampo;
                Enum.TryParse<Dominio.Enumeradores.TipoCampoCCe>(Request.Params("TipoCampo"), out tipoCampo);

                bool indicadorRepeticao = false;
                bool.TryParse(Request.Params("IndicadorRepeticao"), out indicadorRepeticao);

                string descricao = Request.Params("Descricao");
                string status = Request.Params("Status");
                string nomeCampo = Request.Params("NomeCampo");
                string grupoCampo = Request.Params("GrupoCampo");

                if (string.IsNullOrWhiteSpace(descricao))
                    return new JsonpResult(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(nomeCampo))
                    return new JsonpResult(false, false, "Nome do campo inválido.");

                if (string.IsNullOrWhiteSpace(status))
                    return new JsonpResult(false, false, "Status inválido.");

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = null;

                if (codigo > 0)
                    campoCCe = repCampoCCe.BuscarPorCodigo(codigo, true);
                else
                    campoCCe = new Dominio.Entidades.CampoCCe();

                campoCCe.Descricao = descricao;
                campoCCe.GrupoCampo = grupoCampo;
                campoCCe.IndicadorRepeticao = indicadorRepeticao;
                campoCCe.NomeCampo = nomeCampo;
                campoCCe.QuantidadeCaracteres = quantidadeCaracteres;
                campoCCe.QuantidadeDecimais = quantidadeDecimais;
                campoCCe.QuantidadeInteiros = quantidadeInteiros;
                campoCCe.TipoCampo = tipoCampo;
                campoCCe.Status = status;
                campoCCe.TipoCampoCCeAutomatico = Request.GetEnumParam<TipoCampoCCeAutomatico>("TipoCampoCCeAutomatica");

                if (campoCCe.Codigo > 0)
                    repCampoCCe.Atualizar(campoCCe, Auditado);
                else
                    repCampoCCe.Inserir(campoCCe, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o campo da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}

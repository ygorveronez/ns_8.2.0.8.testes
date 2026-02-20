using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Localidades
{
    [CustomAuthorize("Localidades/Pais")]
    public class PaisController : BaseController
    {
		#region Construtores

		public PaisController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Descricao", 60, Models.Grid.Align.left, true);

                string propOrdeno = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdeno == "Descricao")
                    propOrdeno = "Nome";

                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

                List<Dominio.Entidades.Pais> paises = repPais.Consulta(descricao, propOrdeno, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPais.ContarConsulta(descricao));

                var lista = (from obj in paises
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = obj.Nome
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo(codigo);

                var dynPais = new
                {
                    pais.Codigo,
                    pais.Nome,
                    pais.Sigla,
                    pais.Abreviacao,
                    CodigoTelefonico = pais.CodigoTelefonico,
                    pais.LicencaTNTI,
                    VencimentoLicencaTNTI = pais.VencimentoLicencaTNTI.HasValue ? pais.VencimentoLicencaTNTI.Value.ToString("dd/MM/yyyy") : "",
                    pais.CodigoPais
                };

                return new JsonpResult(dynPais);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                PreencherPais(unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherPais(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Codigo");

            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo(codigo);

            pais.Abreviacao = Request.GetStringParam("Abreviacao");
            pais.CodigoTelefonico = Request.GetIntParam("CodigoTelefonico");
            pais.LicencaTNTI = Request.GetStringParam("LicencaTNTI");
            pais.VencimentoLicencaTNTI = Request.GetNullableDateTimeParam("VencimentoLicencaTNTI");
            pais.CodigoPais = Request.GetStringParam("CodigoPais");

            repPais.Atualizar(pais, Auditado);
        }
    }
}

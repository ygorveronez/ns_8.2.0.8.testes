using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe.CartaCorrecao
{
    [CustomAuthorize("CTe/CampoCartaCorrecao")]
    public class CampoCartaCorrecaoController : BaseController
    {
		#region Construtores

		public CampoCartaCorrecaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = new Dominio.Entidades.CampoCCe();

                PreencherEntidade(campoCCe, unitOfWork);

                unitOfWork.Start();

                repCampoCCe.Inserir(campoCCe, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorCodigo(codigo, true);

                if (campoCCe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(campoCCe, unitOfWork);

                unitOfWork.Start();

                repCampoCCe.Atualizar(campoCCe, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorCodigo(codigo, false);

                if (campoCCe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    campoCCe.Codigo,
                    campoCCe.Descricao,
                    Situacao = campoCCe.Status == "A" ? true : false,
                    campoCCe.GrupoCampo,
                    campoCCe.IndicadorRepeticao,
                    campoCCe.NomeCampo,
                    campoCCe.QuantidadeCaracteres,
                    campoCCe.QuantidadeDecimais,
                    campoCCe.QuantidadeInteiros,
                    campoCCe.TipoCampo
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorCodigo(codigo, false);

                if (campoCCe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repCampoCCe.Deletar(campoCCe, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.CampoCCe campoCCe, Repositorio.UnitOfWork unitOfWork)
        {
            bool ativo = Request.GetBoolParam("Situacao");
            bool indicadorRepeticao = Request.GetBoolParam("IndicadorRepeticao");

            string descricao = Request.Params("Descricao");
            string grupoCampo = Request.Params("GrupoCampo");
            string nomeCampo = Request.Params("NomeCampo");

            int quantidadeCaracteres = Request.GetIntParam("QuantidadeCaracteres");
            int quantidadeDecimais = Request.GetIntParam("QuantidadeDecimais");
            int quantidadeInteiros = Request.GetIntParam("QuantidadeInteiros");

            Dominio.Enumeradores.TipoCampoCCe tipoCampoCCe = Request.GetEnumParam<Dominio.Enumeradores.TipoCampoCCe>("TipoCampo");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            campoCCe.Status = ativo ? "A" : "I";
            campoCCe.Descricao = descricao;
            campoCCe.GrupoCampo = grupoCampo;
            campoCCe.IndicadorRepeticao = indicadorRepeticao;
            campoCCe.NomeCampo = nomeCampo;
            campoCCe.QuantidadeCaracteres = quantidadeCaracteres;
            campoCCe.QuantidadeDecimais = quantidadeDecimais;
            campoCCe.QuantidadeInteiros = quantidadeInteiros;
            campoCCe.TipoCampo = tipoCampoCCe;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                List<Dominio.Entidades.CampoCCe> listaCamposCCe = repCampoCCe.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCampoCCe.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaCamposCCe
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoStatus
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoStatus")
                return "Status";

            return propriedadeOrdenar;
        }

        #endregion

    }
}

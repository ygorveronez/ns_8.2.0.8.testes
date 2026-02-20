using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Empresa
{
    public class EmpresaController : BaseController
    {
		#region Construtores

		public EmpresaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                bool somenteEmpresaFilho = Request.GetBoolParam("SomenteEmpresaFilho");

                int codigoEmpresa = 0;
                bool listarSomenteEmpresasFiliais = false;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = Usuario.Empresa.Codigo;
                    listarSomenteEmpresasFiliais = Request.GetBoolParam("ListarSomenteEmpresasFiliais") && Usuario.Empresa.PermiteAlterarEmpresaOrdemServicoVenda;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("DescricaoCidadeEstado", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Empresa.RazaoSocial, "RazaoSocial", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Empresa.CNPJ, "CNPJ_Formatado", 15, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Descricao", false);

                List<Dominio.Entidades.Empresa> listaEmpresa = repEmpresa.Consultar(descricao, somenteEmpresaFilho, codigoEmpresa, ativo, listarSomenteEmpresasFiliais, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repEmpresa.ContarConsulta(descricao, somenteEmpresaFilho, codigoEmpresa, ativo, listarSomenteEmpresasFiliais));
                var lista = (from p in listaEmpresa
                             select new
                             {
                                 p.Codigo,
                                 p.Localidade.DescricaoCidadeEstado,
                                 p.RazaoSocial,
                                 Localidade = p.Localidade != null ? p.Localidade.DescricaoCidadeEstado : "",
                                 p.CNPJ_Formatado,
                                 p.DescricaoStatus,
                                 p.Descricao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

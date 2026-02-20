using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Filiais
{
    [CustomAuthorize("Filiais/SequenciaGestaoPatio")]
    public class SequenciaGestaoPatioController : BaseController
    {
		#region Construtores

		public SequenciaGestaoPatioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("CodigoTipoOperacao", false);
                grid.AdicionarCabecalho("Filial", "Filial", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 30, Models.Grid.Align.left, true);

                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                int totalRegistros = repositorioSequenciaGestaoPatio.ContarConsulta(codigoFilial, codigoTipoOperacao);
                List<Dominio.ObjetosDeValor.Embarcador.Filial.SequenciaGestaoPatio> listaSequenciaGestaoPatio = (totalRegistros > 0) ? repositorioSequenciaGestaoPatio.Consultar(codigoFilial, codigoTipoOperacao, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Filial.SequenciaGestaoPatio>();

                grid.AdicionaRows(listaSequenciaGestaoPatio);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion Métodos Privados
    }
}

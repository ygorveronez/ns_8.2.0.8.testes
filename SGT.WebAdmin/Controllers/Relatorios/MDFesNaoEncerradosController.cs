using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Relatorios
{
    [CustomAuthorize("RelatoriosRV/MDFesNaoEncerrados")]
    public class MDFesNaoEncerradosController : BaseController
    {
		#region Construtores

		public MDFesNaoEncerradosController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Empresa", "Empresa", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Numero", "Numero", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data Autorização", "DataAutorizacao", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Retorno SEFAZ", "MensagemRetornoSefaz", 40, Models.Grid.Align.left);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);



                List<int> codigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
                List<double> codigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);


                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.ConsultarNaoEncerrados(this.Empresa.Codigo, DateTime.Now.AddDays(-25), codigosFiliais, codigosRecebedores, grid.inicio, grid.limite);
                int countMDFes = repMDFe.ContarConsultaNaoEncerrados(this.Empresa.Codigo, DateTime.Now.AddDays(-25));

                grid.AdicionaRows((from obj in mdfes
                                   select new
                                   {
                                       obj.Codigo,
                                       Empresa = obj.Empresa.CNPJ + " - " + obj.Empresa.NomeFantasia,
                                       Numero = obj.Numero + " - " + obj.Serie.Numero,
                                       DataAutorizacao = obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                       obj.DescricaoStatus,
                                       obj.MensagemRetornoSefaz
                                   }).ToList());

                grid.setarQuantidadeTotal(countMDFes);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}

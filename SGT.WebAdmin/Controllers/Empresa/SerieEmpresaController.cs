using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Empresa
{
    public class SerieEmpresaController : BaseController
    {
		#region Construtores

		public SerieEmpresaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                int numero = 0;
                int.TryParse(Request.Params("Descricao"), out numero);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                Dominio.Enumeradores.TipoSerie tipoSerie;

                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                Enum.TryParse(Request.Params("TipoSerie"), out tipoSerie);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                }
                Dominio.Enumeradores.TipoAmbiente ambiente = this.Usuario.Empresa.TipoAmbiente;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ProximoNumero", false);
                grid.AdicionarCabecalho("Número", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Razão Social", "RazaoSocial", 50, Models.Grid.Align.left, false);

                List<Dominio.Entidades.EmpresaSerie> listaEmpresaSerie = repEmpresaSerie.ConsultarPorTipo(codigoEmpresa, numero, "A", tipoSerie, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repEmpresaSerie.ContarConsultaPorTipo(codigoEmpresa, numero, "A", tipoSerie));
                var lista = (from p in listaEmpresaSerie
                             select new
                             {
                                 p.Codigo,
                                 ProximoNumero = tipoSerie == Dominio.Enumeradores.TipoSerie.NFe ? this.ProximoNumeroNota(unitOfWork, p.Numero, codigoEmpresa, tipoSerie, ambiente) : 1,
                                 Descricao = p.Numero,
                                 p.Empresa.RazaoSocial
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

        private int ProximoNumeroNota(Repositorio.UnitOfWork unitOfWork, int numeroSerie, int codigoEmpresa, Dominio.Enumeradores.TipoSerie tipoSerie, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

            string numeroTipoSerie = "";
            if (tipoSerie == Dominio.Enumeradores.TipoSerie.CTe)
                numeroTipoSerie = "57";
            else if (tipoSerie == Dominio.Enumeradores.TipoSerie.NFe)
                numeroTipoSerie = "55";

            int proximoNumeroSerie = 1;
            if (!string.IsNullOrWhiteSpace(numeroTipoSerie))
                proximoNumeroSerie = repNFe.BuscarUltimoNumero(codigoEmpresa, numeroSerie, ambiente, numeroTipoSerie) + 1;
            int proximoNumero = repSerie.BuscarProximoNumeroDocumentoPorSerie(codigoEmpresa, numeroSerie, Dominio.Enumeradores.TipoSerie.NFe);
            if (proximoNumeroSerie < proximoNumero)
                proximoNumeroSerie = proximoNumero;

            return proximoNumeroSerie;
        }
    }
}

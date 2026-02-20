using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoEncerramentoCarga
{
    [CustomAuthorize("Cargas/FluxoEncerramentoCarga")]
    public class FluxoEncerramentoCargaCIOTController : BaseController
    {
		#region Construtores

		public FluxoEncerramentoCargaCIOTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT() { NumeroCarga = carga.CodigoCargaEmbarcador };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Operadora", "Operadora", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Transportador")
                    propOrdenar = "Transportador.Nome";

                int countCIOTs = repCIOT.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> ciots = new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                if (countCIOTs > 0)
                    ciots = repCIOT.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var retorno = (from obj in ciots
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   obj.CodigoVerificador,
                                   Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOTHelper.ObterDescricao(obj.Operadora),
                                   Transportador = obj.Transportador?.Nome,
                                   Situacao = obj.DescricaoSituacao,
                               }).ToList();

                grid.setarQuantidadeTotal(countCIOTs);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar CIOT");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadContrato()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCIOT = 0;
                int.TryParse(Request.Params("Codigo"), out codigoCIOT);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return new JsonpResult(true, false, "CIOT não encontrado, atualize a página e tente novamente.");

                if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado &&
                    ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto &&
                    ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado)
                    return new JsonpResult(true, false, "O status do CIOT não permite a geração do contrato.");

                string mensagemErro = string.Empty;

                byte[] pdf = new Servicos.Embarcador.CIOT.CIOT().GerarContratoFrete(ciot.Codigo, unidadeTrabalho, out mensagemErro);

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                return Arquivo(pdf, "application/pdf", "CIOT-" + ciot.Numero + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do contrato de frete.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}

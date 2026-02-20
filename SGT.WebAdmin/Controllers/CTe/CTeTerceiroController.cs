using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/CTeTerceiro")]
    public class CTeTerceiroController : BaseController
    {
		#region Construtores

		public CTeTerceiroController(Conexao conexao) : base(conexao) { }

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

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Emitente", "Emitente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro filtros = ObterFiltrosPesquisa();

                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repCTeTerceiro.Consultar(filtros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCTeTerceiro.ContarConsulta(filtros);

                var retorno = ctesTerceiros.Select(o => new
                {
                    o.Codigo,
                    o.Descricao,
                    o.Numero,
                    o.Serie,
                    Emitente = o.Emitente.Descricao,
                    Remetente = o.Remetente.Descricao,
                    Destinatario = o.Destinatario.Descricao,
                    Origem = o.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                    Destino = o.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                    o.ValorAReceber
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

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                TipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe"),
                CPFCNPJEmitente = Request.GetDoubleParam("Emitente"),
                CPFCNPJDestinatario = Request.GetDoubleParam("Destinatario"),
                CPFCNPJRemetente = Request.GetDoubleParam("Remetente"),
                PossuiOcorrenciaGerada = Request.GetBoolParam("PossuiOcorrenciaGerada")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

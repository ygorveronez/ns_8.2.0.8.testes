using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/AutorizacaoCTeLote")]
    public class AutorizacaoCTeLoteController : BaseController
    {
		#region Construtores

		public AutorizacaoCTeLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repCTe.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = totalRegistros > 0 ? repCTe.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.NumeroOS,
                                 p.NumeroBooking,
                                 Serie = p.Serie.Numero,
                                 p.DataEmissao,
                                 p.DescricaoStatus,
                                 DescricaoModal = p.TipoModal.ObterDescricao(),
                                 Remetente = p.Remetente?.Descricao,
                                 Origem = p.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 PortoOrigem = p.PortoOrigem?.Descricao,
                                 TerminalOrigem = p.TerminalOrigem?.Descricao,
                                 Viagem = p.Viagem?.Descricao,

                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
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

        [AllowAuthenticate]
        public async Task<IActionResult> SolicitarAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa = ObterFiltrosPesquisa();

                bool teveCargasAtualizadas = false;
                int totalRegistros = repCTe.ContarConsulta(filtrosPesquisa);
                if (totalRegistros == 0)
                    return new JsonpResult(false, true, "Nenhum CT-e filtrado");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCTe.Consultar(filtrosPesquisa, null);
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTe)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTe?.Carga ?? null;

                    if (carga != null && !carga.EmitindoCTes)
                    {
                        if (carga.PossuiPendencia)
                        {
                            carga.PossuiPendencia = false;
                            carga.problemaCTE = false;
                            carga.MotivoPendencia = "";
                        }

                        carga.EmitindoCTes = true;
                        repCarga.Atualizar(carga);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Autorizou Emissão dos documentos.", unitOfWork);
                        teveCargasAtualizadas = true;
                    }
                }

                if (!teveCargasAtualizadas)
                    return new JsonpResult(false, true, "Dos CT-es filtrados, nenhuma Carga possui pendência.");

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao Solicitar a Autorização.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                //TipoModal = Request.GetEnumParam<TipoModal>("TipoModal"),
                TipoModal = Request.GetListParam<TipoModal>("TipoModal"),
                Status = Request.GetListParam<string>("Status"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                CodigoContainer = Request.GetIntParam("Container"),
                NumeroNF = Request.GetStringParam("NumeroNF")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Série", "Serie", 4, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modal", "DescricaoModal", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Viagem", "Viagem", 10, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}

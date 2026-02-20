using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/AutorizacaoAverbacaoLote")]
    public class AutorizacaoAverbacaoLoteController : BaseController
    {
		#region Construtores

		public AutorizacaoAverbacaoLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                if (parametrosConsulta.PropriedadeOrdenar == "NumeroControle")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.NumeroControle";
                else if (parametrosConsulta.PropriedadeOrdenar == "Numero")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.Numero";
                else if (parametrosConsulta.PropriedadeOrdenar == "CodigoCargaEmbarcador")
                    parametrosConsulta.PropriedadeOrdenar = "Carga.CodigoCargaEmbarcador";
                else if (parametrosConsulta.PropriedadeOrdenar == "NumeroOS")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.NumeroOS";
                else if (parametrosConsulta.PropriedadeOrdenar == "NumeroBooking")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.NumeroBooking";
                else if (parametrosConsulta.PropriedadeOrdenar == "Viagem")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.Viagem";
                else if (parametrosConsulta.PropriedadeOrdenar == "DataEmissao")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.DataEmissao";
                else if (parametrosConsulta.PropriedadeOrdenar == "Remetente")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.Remetente";
                else if (parametrosConsulta.PropriedadeOrdenar == "LocalidadeInicioPrestacao")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.LocalidadeInicioPrestacao";
                else if (parametrosConsulta.PropriedadeOrdenar == "PortoOrigem")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.PortoOrigem";
                else if (parametrosConsulta.PropriedadeOrdenar == "TerminalOrigem")
                    parametrosConsulta.PropriedadeOrdenar = "CTe.TerminalOrigem";

                int totalRegistros = repAverbacaoCTe.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.AverbacaoCTe> listaAverbacaoCTe = totalRegistros > 0 ? repAverbacaoCTe.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.AverbacaoCTe>();

                var lista = (from p in listaAverbacaoCTe
                             select new
                             {
                                 p.Codigo,
                                 NumeroControle = p.CTe?.NumeroControle ?? "",
                                 Numero = p.CTe?.Numero ?? p.XMLNotaFiscal.Numero,
                                 CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador ?? "",
                                 NumeroOS = p.CTe?.NumeroOS ?? "",
                                 NumeroBooking = p.CTe?.NumeroBooking ?? "",
                                 Viagem = p.CTe.Viagem?.Descricao,
                                 DataEmissao = p.CTe?.DataEmissao ?? p.XMLNotaFiscal.DataEmissao,
                                 p.DescricaoStatus,
                                 DescricaoModal = p.CTe?.TipoModal.ObterDescricao() ?? "",
                                 Remetente = p.CTe?.Remetente?.Descricao ?? p.XMLNotaFiscal.Emitente.Descricao,
                                 Origem = p.CTe?.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? (p.XMLNotaFiscal.Emitente.Localidade.Descricao),
                                 PortoOrigem = p.CTe?.PortoOrigem?.Descricao ?? "",
                                 TerminalOrigem = p.CTe?.TerminalOrigem?.Descricao ?? "",
                                 SeguradoraAverbacao = p.DescricaoSeguradoraAverbacao,
                                 p.DataRetorno,
                                 p.MensagemRetorno
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

        public async Task<IActionResult> SolicitarAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                string stringConexao = _conexao.StringConexao;
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa = ObterFiltrosPesquisa();

                bool teveCargasAtualizadas = false;
                int totalRegistros = repAverbacaoCTe.ContarConsulta(filtrosPesquisa);
                List<int> codigosCargas = new List<int>();
                if (totalRegistros == 0)
                    return new JsonpResult(false, true, "Nenhum CT-e filtrado");

                List<Dominio.Entidades.AverbacaoCTe> listaAverbacaoCTe = repAverbacaoCTe.Consultar(filtrosPesquisa, null);
                foreach (Dominio.Entidades.AverbacaoCTe averbacao in listaAverbacaoCTe)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = averbacao?.Carga ?? null;

                    if (carga != null)
                    {
                        if (carga.PossuiPendencia)
                        {
                            carga.PossuiPendencia = false;
                            carga.problemaAverbacaoCTe = false;
                            carga.MotivoPendencia = "";

                        }
                        if (!codigosCargas.Contains(carga.Codigo))
                            codigosCargas.Add(carga.Codigo);

                        repCarga.Atualizar(carga);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou averbações rejeitadas.", unitOfWork);
                        teveCargasAtualizadas = true;
                    }
                }

                if (!teveCargasAtualizadas)
                    return new JsonpResult(false, true, "Dos CT-es filtrados, nenhuma Carga possui pendência de averbação.");

                unitOfWork.CommitChanges();

                if (codigosCargas.Count > 0)
                    Task.Factory.StartNew(() => AsyncAverbacaoRejeitados(codigosCargas, stringConexao));

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
                TipoModal = Request.GetListParam<TipoModal>("TipoModal"),
                StatusAverbacao = Request.GetListEnumParam<Dominio.Enumeradores.StatusAverbacaoCTe>("StatusAverbacao"),
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
            grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Nº Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Viagem", "Viagem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 9, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modal", "DescricaoModal", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Averbadora", "SeguradoraAverbacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Retorno", "DataRetorno", 9, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 10, Models.Grid.Align.left, true);


            return grid;
        }

        private void AsyncAverbacaoRejeitados(List<int> codigosCargas, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            try
            {
                // Repositorios e servicos
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                foreach (var codigoCarga in codigosCargas)
                {
                    // Contador da unity
                    int count = 0;

                    // Itera todos e averba
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(codigoCarga);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cte in cargaCTes)
                    {
                        // Manda pro Oracle
                        svcCTe.EmitirAverbacoesOracle(cte.CTe.Empresa.Codigo, cte.CTe.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao, unitOfWork);

                        // Incrementa operação
                        count++;

                        // Reinicia a unit
                        if (count == 25)
                        {
                            count = 0;
                            repCargaCTe = null;

                            unitOfWork.Dispose();
                            unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                            repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                        }
                    }

                    if (cargaCTes.Count > 0)
                        VerificaAverbacao(cargaCTes.FirstOrDefault().Carga, unitOfWork);

                    svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void VerificaAverbacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            // Atualiza situação da carga
            if (repAverbacaoCTe.BuscarPorCargaESituacao(carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao).Count == 0)
            {
                carga.PossuiPendencia = false;
                carga.problemaAverbacaoCTe = false;
                carga.MotivoPendencia = "";

                repCarga.Atualizar(carga);
            }
        }

        #endregion
    }
}

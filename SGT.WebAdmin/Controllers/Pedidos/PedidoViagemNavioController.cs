using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/ViagemNavio")]
    public class PedidoViagemNavioController : BaseController
    {
		#region Construtores

		public PedidoViagemNavioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("CodigoNavio", false);
                grid.AdicionarCabecalho("DescricaoNavio", false);
                grid.AdicionarCabecalho("Direcao", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (ordenacao == "CodigoNavio" || ordenacao == "DescricaoNavio" || ordenacao == "Direcao")
                    ordenacao = "Descricao";

                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio> viagens = repPedidoViagemNavio.Consultar(descricao, codigoIntegracao, status, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoViagemNavio.ContarConsulta(descricao, codigoIntegracao, status));

                var lista = (from p in viagens
                             select new
                             {
                                 CodigoNavio = p.Navio?.Codigo ?? 0,
                                 DescricaoNavio = p.Navio?.Descricao ?? "",
                                 Direcao = p.DirecaoViagemMultimodal,
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 p.DescricaoStatus
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaSchedules()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repMercadoria = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaSchudele();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int.TryParse(Request.Params("Codigo"), out int navioViagem);
                int totalRegistros = repMercadoria.ContarConsultaPorNavioViagem(navioViagem);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> listaGrid = repMercadoria.ConsultarPorNavioViagem(navioViagem, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 obj.Codigo,
                                 TerminalAtracacao = obj.TerminalAtracacao?.Descricao ?? "",
                                 PortoAtracacao = obj.PortoAtracacao?.Descricao ?? "",
                                 DataPrevisaoChegadaNavio = obj.DataPrevisaoChegadaNavio.HasValue ? obj.DataPrevisaoChegadaNavio.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 DataPrevisaoSaidaNavio = obj.DataPrevisaoSaidaNavio.HasValue ? obj.DataPrevisaoSaidaNavio.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 DataDeadLine = obj.DataDeadLine.HasValue ? obj.DataDeadLine.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 ETAConfirmado = obj.ETAConfirmado,
                                 ETSConfirmado = obj.ETSConfirmado,
                                 obj.Status
                             }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemNavio = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio();

                PreencherViagemNavio(viagemNavio, unitOfWork);
                repPedidoViagemNavio.Inserir(viagemNavio, Auditado);
                SalvarSchedules(viagemNavio, unitOfWork, Auditado, null);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigo, true);

                PreencherViagemNavio(viagemNavio, unitOfWork);
                SalvarSchedules(viagemNavio, unitOfWork, Auditado, null);
                repPedidoViagemNavio.Atualizar(viagemNavio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigo, false);

                var dynNavio = new
                {
                    viagemNavio.Codigo,
                    viagemNavio.Descricao,
                    viagemNavio.CodigoIntegracao,
                    viagemNavio.NumeroViagem,
                    viagemNavio.Status,
                    Direcao = viagemNavio.DirecaoViagemMultimodal,
                    Navio = viagemNavio.Navio != null ? new { viagemNavio.Navio.Codigo, viagemNavio.Navio.Descricao } : null,
                    PortoAtracacao = viagemNavio.PortoAtracacao != null ? new { viagemNavio.PortoAtracacao.Codigo, viagemNavio.PortoAtracacao.Descricao } : null,
                    TerminalAtracacao = viagemNavio.TerminalAtracacao != null ? new { viagemNavio.TerminalAtracacao.Codigo, viagemNavio.TerminalAtracacao.Descricao } : null,
                    DataPrevisaoChegadaNavio = viagemNavio.DataPrevisaoChegadaNavio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataPrevisaoSaidaNavio = viagemNavio.DataPrevisaoSaidaNavio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataDeadLine = viagemNavio.DataDeadLine?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    viagemNavio.ETSConfirmado,
                    viagemNavio.ETAConfirmado,
                    ConsumoPlugs = viagemNavio.ConsumoPlugs.ToString("n4"),
                    ConsumoTeus = viagemNavio.ConsumoTeus.ToString("n4"),
                    ConsumoTons = viagemNavio.ConsumoTons.ToString("n4"),
                    CapacidadePlug = viagemNavio.Navio?.CapacidadePlug.ToString("n4") ?? "",
                    CapacidadeTeus = viagemNavio.Navio?.CapacidadeTeus.ToString("n4") ?? "",
                    CapacidadeTons = viagemNavio.Navio?.CapacidadeTons.ToString("n4") ?? "",
                    Schedules = viagemNavio.Schedules != null && viagemNavio.Schedules.Count > 0 ?
                        (
                        from o in viagemNavio.Schedules
                        select new
                        {
                            o.Codigo,
                            PortoAtracacao = o.PortoAtracacao != null ? new { o.PortoAtracacao.Codigo, o.PortoAtracacao.Descricao } : null,
                            TerminalAtracacao = o.TerminalAtracacao != null ? new { o.TerminalAtracacao.Codigo, o.TerminalAtracacao.Descricao } : null,
                            DataPrevisaoChegadaNavio = o.DataPrevisaoChegadaNavio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            DataPrevisaoSaidaNavio = o.DataPrevisaoSaidaNavio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            DataDeadLine = o.DataDeadLine?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            o.ETSConfirmado,
                            o.ETAConfirmado,
                            o.Status
                        }
                    ).ToList() : null
                };

                return new JsonpResult(dynNavio);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigo, true);

                if (viagemNavio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repPedidoViagemNavio.Deletar(viagemNavio, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherViagemNavio(Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagemNavio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            int numeroViagem = Request.GetIntParam("NumeroViagem");
            int codigoNavio = Request.GetIntParam("Navio");
            int codigoPortoAtracacao = Request.GetIntParam("PortoAtracacao");
            int codigoTerminalAtracacao = Request.GetIntParam("TerminalAtracacao");
            bool.TryParse(Request.Params("Status"), out bool status);


            Dominio.Entidades.Embarcador.Pedidos.Navio navio = codigoNavio > 0 ? repNavio.BuscarPorCodigo(codigoNavio) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto porto = codigoPortoAtracacao > 0 ? repPorto.BuscarPorCodigo(codigoPortoAtracacao) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = codigoTerminalAtracacao > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalAtracacao) : null;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal direcao;
            Enum.TryParse(Request.Params("Direcao"), out direcao);

            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string descricao = navio.Descricao + "/" + numeroViagem.ToString("D") + Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(direcao);

            viagemNavio.Integrado = false;
            viagemNavio.Status = status;
            viagemNavio.Descricao = descricao;
            viagemNavio.CodigoIntegracao = codigoIntegracao;
            viagemNavio.NumeroViagem = numeroViagem;
            viagemNavio.DirecaoViagemMultimodal = direcao;
            viagemNavio.Navio = navio;
            viagemNavio.PortoAtracacao = porto;
            viagemNavio.TerminalAtracacao = terminal;
            viagemNavio.DataPrevisaoChegadaNavio = Request.GetNullableDateTimeParam("DataPrevisaoChegadaNavio");
            viagemNavio.DataPrevisaoSaidaNavio = Request.GetNullableDateTimeParam("DataPrevisaoSaidaNavio");
            viagemNavio.DataDeadLine = Request.GetNullableDateTimeParam("DataDeadLine");
            viagemNavio.ETSConfirmado = Request.GetBoolParam("ETSConfirmado");
            viagemNavio.ConsumoPlugs = Request.GetDecimalParam("ConsumoPlugs");
            viagemNavio.ConsumoTeus = Request.GetDecimalParam("ConsumoTeus");
            viagemNavio.ConsumoTons = Request.GetDecimalParam("ConsumoTons");




        }

        private Models.Grid.Grid GridPesquisaSchudele()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("TerminalAtracacao").Nome("Terminal").Tamanho(30).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("PortoAtracacao").Nome("Porto").Tamanho(30).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("DataPrevisaoChegadaNavio").Nome("ETA").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("DataPrevisaoSaidaNavio").Nome("ETS").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("DataDeadLine").Nome("Dead Line").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("ETAConfirmado");
            grid.Prop("ETSConfirmado");

            return grid;
        }

        private void SalvarSchedules(Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoViagemNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            List<dynamic> schedules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Schedules"));
            if (schedules == null) return;

            List<int> codigosSchedules = new List<int>();
            foreach (dynamic codigo in schedules)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                if (intcodigo > 0)
                    codigosSchedules.Add(intcodigo);
            }
            codigosSchedules = codigosSchedules.Where(o => o > 0).Distinct().ToList();

            List<int> codigosExcluir = repPedidoViagemNavioSchedule.BuscarNaoPesentesNaLista(pedidoViagemNavio.Codigo, codigosSchedules);

            foreach (dynamic dynSchedule in schedules)
            {
                int.TryParse((string)dynSchedule.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repPedidoViagemNavioSchedule.BuscarPorRequisicaoESchedule(pedidoViagemNavio.Codigo, codigo);

                if (schedule == null)
                    schedule = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule();
                else schedule.Initialize();

                int.TryParse((string)dynSchedule.TerminalAtracacao.Codigo, out int codigoTerminalAtracacao);
                int.TryParse((string)dynSchedule.PortoAtracacao.Codigo, out int codigoPortoAtracacao);

                DateTime.TryParseExact((string)dynSchedule.DataDeadLine, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataDeadLine);
                DateTime.TryParseExact((string)dynSchedule.DataPrevisaoChegadaNavio, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoChegadaNavio);
                DateTime.TryParseExact((string)dynSchedule.DataPrevisaoSaidaNavio, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoSaidaNavio);

                DateTime? nullDataDeadLine = null;
                DateTime? nullDataPrevisaoChegadaNavio = null;
                DateTime? nullDataPrevisaoSaidaNavio = null;

                if (dataDeadLine > DateTime.MinValue)
                    nullDataDeadLine = dataDeadLine;
                if (dataPrevisaoChegadaNavio > DateTime.MinValue)
                    nullDataPrevisaoChegadaNavio = dataPrevisaoChegadaNavio;
                if (dataPrevisaoSaidaNavio > DateTime.MinValue)
                    nullDataPrevisaoSaidaNavio = dataPrevisaoSaidaNavio;

                bool.TryParse((string)dynSchedule.ETAConfirmado, out bool etaConfirmado);
                bool.TryParse((string)dynSchedule.ETSConfirmado, out bool etsConfirmado);
                bool.TryParse((string)dynSchedule.Status, out bool status);

                schedule.PedidoViagemNavio = pedidoViagemNavio;

                schedule.TerminalAtracacao = repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalAtracacao);
                schedule.PortoAtracacao = repPorto.BuscarPorCodigo(codigoPortoAtracacao);
                schedule.Status = status;
                schedule.DataPrevisaoChegadaNavio = nullDataPrevisaoChegadaNavio;
                schedule.DataPrevisaoSaidaNavio = nullDataPrevisaoSaidaNavio;
                schedule.DataDeadLine = nullDataDeadLine;
                schedule.ETAConfirmado = etaConfirmado;

                bool etsAnterior = schedule.ETSConfirmado;
                schedule.ETSConfirmado = etsConfirmado;

                if (schedule.Codigo == 0)
                    repPedidoViagemNavioSchedule.Inserir(schedule, auditado, historicoPai);
                else
                    repPedidoViagemNavioSchedule.Atualizar(schedule, auditado, historicoPai);

                if (!etsAnterior && schedule.ETSConfirmado)
                {
                    if (ConfiguracaoEmbarcador.EncerrarMDFeAutomaticamente && schedule.PortoAtracacao != null && schedule.PortoAtracacao.Localidade != null && schedule.PedidoViagemNavio != null)
                        Servicos.Embarcador.Carga.MDFe.EncerrarMDFePeloETSConfirmado(out string erro, this.Usuario, WebServiceConsultaCTe, schedule.PedidoViagemNavio.Codigo, schedule.PortoAtracacao.Codigo, unitOfWork, _conexao.StringConexao, Auditado);
                }
            }

            foreach (int excluir in codigosExcluir)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule objParExcluir = repPedidoViagemNavioSchedule.BuscarPorRequisicaoESchedule(pedidoViagemNavio.Codigo, excluir);
                if (objParExcluir != null) repPedidoViagemNavioSchedule.Deletar(objParExcluir, auditado, historicoPai);
            }
        }


        #endregion
    }
}

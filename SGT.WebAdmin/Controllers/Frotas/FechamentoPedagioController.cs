using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize(new string[] { "BuscarPedagios" }, "Frotas/FechamentoPedagio")]
    public class FechamentoPedagioController : BaseController
    {
		#region Construtores

		public FechamentoPedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarFechamentoPedagios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
                DateTime dataFim = Request.GetDateTimeParam("DataFim");

                //Valida se algum campo está preenchido
                if ((dataInicio != DateTime.MinValue && dataFim == DateTime.MinValue) || dataInicio == DateTime.MinValue && dataFim != DateTime.MinValue)
                    return new JsonpResult(false, true, "Favor informar as duas datas para a geração.");
                if (dataInicio == DateTime.MinValue && codigoVeiculo == 0)
                    return new JsonpResult(false, true, "Favor informar pelo menos um dos campos para a geração.");

                // Situacao fixo
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio? situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;

                // Busca todos pedagios para vincular
                int countPedagio = repPedagio.ContarConsultarParaFechamento(codigoVeiculo, dataInicio, dataFim, situacao);

                // Se não houver pedagio, não gera fechamento
                if (countPedagio == 0)
                    return new JsonpResult(false, true, "Nenhum pedágio aberto com os filtros selecionados.");

                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagios = repPedagio.ConsultarParaFechamento(codigoVeiculo, dataInicio, dataFim, situacao, "Codigo", "", 0, countPedagio);

                // Gera fechamento
                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = new Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio();
                fechamentoPedagio.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                if (dataInicio != DateTime.MinValue)
                    fechamentoPedagio.DataInicio = dataInicio;
                if (dataFim != DateTime.MinValue)
                    fechamentoPedagio.DataFim = dataFim;
                fechamentoPedagio.Operador = this.Usuario;
                fechamentoPedagio.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Pendente;

                repFechamentoPedagio.Inserir(fechamentoPedagio, Auditado);

                // Vincula os pedagios ao fechamento
                int resetUnitOfWork = 0;
                for (var i = 0; i < listaPedagios.Count(); i++)
                {
                    // Inicia instancia
                    unitOfWork.Start();

                    // Vincula fechamento
                    Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(listaPedagios[i].Codigo, true);
                    pedagio.FechamentoPedagio = fechamentoPedagio;
                    repPedagio.Atualizar(pedagio);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedagio, pedagio.GetChanges(), "Gerar Fechamento Pedágio", unitOfWork);

                    // Comitta 
                    unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                        unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                        repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                return new JsonpResult(fechamentoPedagio.Codigo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            /* Busca as informacoes do fechamento a partir do codigo do fechamento
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = repFechamentoPedagio.BuscarPorCodigo(codigo);

                if (fechamentoPedagio == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynFechamentoPedagio = new
                {
                    fechamentoPedagio.Codigo,
                    Veiculo = fechamentoPedagio.Veiculo != null ? new { fechamentoPedagio.Veiculo.Codigo, Descricao = fechamentoPedagio.Veiculo.Placa } : null,
                    DataInicio = fechamentoPedagio.DataInicio.HasValue ? fechamentoPedagio.DataInicio.Value.ToString("dd/MM/yyyy") : null,
                    DataFim = fechamentoPedagio.DataFim.HasValue ? fechamentoPedagio.DataFim.Value.ToString("dd/MM/yyyy") : null,
                    Situacao = fechamentoPedagio.Situacao
                };
                return new JsonpResult(dynFechamentoPedagio);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPedagios()
        {
            /* Busca os fechamentos vinculados a esse fechamento
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 7);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Passagem", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Praça", "Praca", 25, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Rodovia", "Rodovia", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true, false, false, false, true, editableValor);

                // Codigo do fechamento
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);

                // Define a coluna de ordenacao
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo." + propOrdenar;
                else if (propOrdenar == "Situacao")
                    propOrdenar = "SituacaoPedagio";

                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = repFechamentoPedagio.BuscarPorCodigo(codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                if (fechamentoPedagio.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Finalizado)
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos;

                // Busca os pedagios
                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagios = repFechamentoPedagio.ConsultarPorFechamento(codigo, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFechamentoPedagio.ContarConsultarPorFechamento(codigo, situacao));

                var lista = (from obj in listaPedagios select RetornarPedagioDadosGrid(obj, true)).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, true);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                // Situacao
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo." + propOrdenar;

                // Instancia repositorios
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio> listaFechamentos = repFechamentoPedagio.ConsultarFechamentos(codigoVeiculo, dataInicio, dataFim, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFechamentoPedagio.ContarConsultarFechamentos(codigoVeiculo, dataInicio, dataFim, situacao));

                var lista = (from obj in listaFechamentos
                             select new
                             {
                                 obj.Codigo,
                                 Placa = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                 DataInicio = obj.DataInicio.HasValue ? obj.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFim = obj.DataFim.HasValue ? obj.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Situacao = obj.DescricaoSituacao,
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

        public async Task<IActionResult> FecharPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Codigo do fechamento
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio repConfiguracaoFinanceiraPedagio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio configuracaoFinanceiraPedagio = repConfiguracaoFinanceiraPedagio.BuscarPrimeiroRegistro();

                // Busca fechamento
                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = repFechamentoPedagio.BuscarPorCodigo(codigo, true);

                if (fechamentoPedagio == null)
                    return new JsonpResult(false, "Fechamento não encontrado.");

                if (fechamentoPedagio.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Pendente)
                    return new JsonpResult(false, true, "A situação do Fechamento não permite essa ação.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;

                // Busca os pedagios
                int totalPedagios = repFechamentoPedagio.ContarConsultarPorFechamento(codigo, situacao);

                // Se não houver pedagio, não gera fechamento
                if (totalPedagios == 0)
                    return new JsonpResult(false, true, "Nenhum pedágio para gerar fechamento.");

                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagios = repFechamentoPedagio.ConsultarPorFechamento(codigo, situacao, "Codigo", "", 0, totalPedagios);

                string erro = "";
                int resetUnitOfWork = 0;
                for (int i = 0; i < listaPedagios.Count; i++)
                {
                    // Inicia instancia
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(listaPedagios[i].Codigo, true);

                    // Fecha pedagio
                    pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado;
                    pedagio.DataAlteracao = DateTime.Now;

                    if (pedagio.TipoMovimento == null)
                        pedagio.TipoMovimento = configuracaoFinanceiraPedagio?.TipoMovimentoLancamentoPedagio;

                    repPedagio.Atualizar(pedagio, Auditado);

                    string obsMovimentacao = "Fechamento de pedágio do veículo " + pedagio.Veiculo.Placa;
                    if (!serProcessoMovimento.GerarMovimentacao(out erro, pedagio.TipoMovimento, pedagio.Data, pedagio.Valor, pedagio.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, TipoServicoMultisoftware))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    // Comitta 
                    unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                        unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                        repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                        repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                unitOfWork.Dispose();
                unitOfWork = null;
                unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                unitOfWork.Start();
                repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                fechamentoPedagio.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Finalizado;
                repFechamentoPedagio.Atualizar(fechamentoPedagio, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Codigo do fechamento
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                // Busca fechamento
                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = repFechamentoPedagio.BuscarPorCodigo(codigo);

                if (fechamentoPedagio == null)
                    return new JsonpResult(false, "Fechamento não encontrado.");

                if (fechamentoPedagio.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.FalhaNaGeracao &&
                    fechamentoPedagio.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Pendente)
                    return new JsonpResult(false, true, "A atual situação do fechamento (" + fechamentoPedagio.DescricaoSituacao + ") não permite o cancelamento da mesma.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos;

                // Busca os pedagios
                int totalPedagios = repFechamentoPedagio.ContarConsultarPorFechamento(codigo, situacao);
                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagios = repFechamentoPedagio.ConsultarPorFechamento(codigo, situacao, "Codigo", "", 0, totalPedagios);

                int resetUnitOfWork = 0;
                for (int i = 0; i < listaPedagios.Count; i++)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(listaPedagios[i].Codigo, true);

                    // Desvinula fechamento dos pedagios e reabre
                    pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                    pedagio.DataAlteracao = DateTime.Now;
                    pedagio.FechamentoPedagio = null;
                    repPedagio.Atualizar(pedagio);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedagio, pedagio.GetChanges(), "Cancelamento do Fechamento Pedágio", unitOfWork);

                    unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                        unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                        repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                        repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                // Deleta o fechamento
                unitOfWork.Dispose();
                unitOfWork = null;
                unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                unitOfWork.Start();
                repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                repFechamentoPedagio.Deletar(fechamentoPedagio, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarValorPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                // Converte valores
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                decimal valor;
                decimal.TryParse(Request.Params("Valor"), out valor);

                bool adicionaAoFechamento;
                bool.TryParse(Request.Params("AdicionaAoFechamento"), out adicionaAoFechamento);

                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo, true);

                // Só é possível alterar pedagios ativos
                if (pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado)
                    return new JsonpResult(false, true, "A atual situação do pedágio (" + pedagio.DescricaoSituacaoPedagio + ") não permite sua alteração. ");

                // Inicia instancia
                unitOfWork.Start();

                // Atualiza informacoes
                pedagio.Valor = valor;
                repPedagio.Atualizar(pedagio, Auditado);

                // Commita alteracoes
                unitOfWork.CommitChanges();
                return new JsonpResult(RetornarPedagioDadosGrid(pedagio, adicionaAoFechamento));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarPedagio()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                // Converte codigo
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca o pedagio
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo, true);

                // Só é possível alterar pedagios ativos
                if (pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado)
                    return new JsonpResult(false, true, "A atual situação do pedágio (" + pedagio.DescricaoSituacaoPedagio + ") não permite sua alteração. ");

                if (pedagio.TipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito)
                    return new JsonpResult(false, true, "Só é permitido alterar pedágios do tipo de Débito. ");

                // Converte valores
                int codigoVeiculo;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                decimal valor;
                decimal.TryParse(Request.Params("Valor"), out valor);

                bool adicionaAoFechamento;
                bool.TryParse(Request.Params("AdicionaAoFechamento"), out adicionaAoFechamento);

                DateTime dataPedagio;
                DateTime.TryParseExact(Request.Params("Data"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataPedagio);

                string rodovia = !string.IsNullOrWhiteSpace(Request.Params("Rodovia")) ? Request.Params("Rodovia") : "";
                string praca = !string.IsNullOrWhiteSpace(Request.Params("Praca")) ? Request.Params("Praca") : "";

                // Inicia instancia
                unitOfWork.Start();

                pedagio.Veiculo = veiculo;
                pedagio.Valor = valor;
                pedagio.Data = dataPedagio;
                pedagio.Praca = praca;
                pedagio.Rodovia = rodovia;

                if (pedagio.Veiculo.Tipo == "T")
                {
                    int codigoMovimento;
                    int.TryParse(Request.Params("TipoMovimento"), out codigoMovimento);
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoMovimento);
                    pedagio.TipoMovimento = tipoMovimento;
                }

                repPedagio.Atualizar(pedagio, Auditado);

                // Commita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(RetornarPedagioDadosGrid(pedagio, adicionaAoFechamento));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarAoFechamento()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoPedagio repFechamentoPedagio = new Repositorio.Embarcador.Frotas.FechamentoPedagio(unitOfWork);
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                // Converte valores
                int codigoPedagio = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPedagio);

                int codigoFechamento = 0;
                int.TryParse(Request.Params("Fechamento"), out codigoFechamento);

                // Busca fechamento e pedagio
                Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio fechamentoPedagio = repFechamentoPedagio.BuscarPorCodigo(codigoFechamento);
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigoPedagio, true);

                if (fechamentoPedagio == null || pedagio == null)
                    return new JsonpResult(false, true, "Erro ao buscar informações.");

                if (pedagio.TipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito)
                    return new JsonpResult(false, true, "Só é permitido adicionar pedágio do tipo de Débito.");

                // Adiciona ao pedagio
                if (pedagio.FechamentoPedagio != null)
                    return new JsonpResult(false, true, "Pedágio selecionado já possui um fechamento.");

                // Inicia instancia
                unitOfWork.Start();

                pedagio.FechamentoPedagio = fechamentoPedagio;
                repPedagio.Atualizar(pedagio, Auditado);

                // Commita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(RetornarPedagioDadosGrid(pedagio, true));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoveDoFechamento()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                int codigoPedagio = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPedagio);

                int codigoFechamento = 0;
                int.TryParse(Request.Params("Fechamento"), out codigoFechamento);

                // Instacia respositorios
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                // Busca fechamento e pedagio
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigoPedagio, true);

                if (pedagio == null || pedagio.FechamentoPedagio.Codigo != codigoFechamento)
                    return new JsonpResult(false, true, "Erro ao buscar informações.");

                if (pedagio.FechamentoPedagio.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Pendente)
                    return new JsonpResult(false, true, "Não é possível remover pedágio de um fechamento finalizado.");

                // Remove fechamento
                pedagio.FechamentoPedagio = null;
                repPedagio.Atualizar(pedagio, Auditado);

                // Commita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(RetornarPedagioDadosGrid(pedagio, false));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic RetornarPedagioDadosGrid(Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio, bool adicionaAoFechamento)
        {
            var retorno = new
            {
                pedagio.Codigo,
                Placa = pedagio.Veiculo != null ? pedagio.Veiculo.Placa : string.Empty,
                Valor = pedagio.Valor.ToString("n2"),
                Data = pedagio.Data != null ? pedagio.Data.ToString("dd/MM/yyyy") : string.Empty,
                Praca = !string.IsNullOrWhiteSpace(pedagio.Praca) ? pedagio.Praca : string.Empty,
                Rodovia = !string.IsNullOrWhiteSpace(pedagio.Rodovia) ? pedagio.Rodovia : string.Empty,
                Situacao = pedagio.DescricaoSituacaoPedagio,
                DT_Enable = adicionaAoFechamento,
                DT_RowColor = pedagio != null && pedagio.ContagemDuplicado > 1 ? "#FF8C69" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco
            };
            return retorno;
        }

        #endregion
    }
}

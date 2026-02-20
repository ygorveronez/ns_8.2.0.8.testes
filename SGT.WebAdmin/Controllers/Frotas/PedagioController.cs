using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/Pedagio", "Frotas/ImportacaoDePedagio")]
    public class PedagioController : BaseController
    {
		#region Construtores

		public PedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPedagiosSemAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio;
                Enum.TryParse(Request.Params("TipoPedagio"), out tipoPedagio);

                string praca = Request.Params("Praca");
                int codigoAcertoViagem, codigoVeiculo;
                int.TryParse(Request.Params("AcertoViagem"), out codigoAcertoViagem);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                DateTime data;
                DateTime.TryParse(Request.Params("Data"), out data);
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("CodigoAcertoPedagio", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("LancadoManualmente", false);
                grid.AdicionarCabecalho("Data e Hora", "DataHora", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("SemParar", false);
                grid.AdicionarCabecalho("Praça", "Praca", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Rodovia", "Rodovia", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PedagioDuplicado", false);
                grid.AdicionarCabecalho("SituacaoPedagio", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho("TipoPedagio", false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("ValorMoedaCotacao", false);
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);

                string propOrdenacao = "Data";//grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagio = repPedagio.ConsultarPedagiosDoAcertoViagem(dataFinal, tipoPedagio, codigoEmpresa, praca, data, codigoAcertoViagem, codigoVeiculo, repAcertoViagem.BuscarPorCodigo(codigoAcertoViagem), propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repPedagio.ContarConsultarPedagiosDoAcertoViagem(dataFinal, tipoPedagio, codigoEmpresa, praca, data, codigoAcertoViagem, codigoVeiculo, repAcertoViagem.BuscarPorCodigo(codigoAcertoViagem));

                grid.setarQuantidadeTotal(quantidade);

                var dynListaPedagio = (from obj in listaPedagio
                                       select new
                                       {
                                           CodigoAcertoPedagio = 0,
                                           obj.Codigo,
                                           LancadoManualmente = codigoAcertoViagem == 0 ? true : false,
                                           DataHora = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                           obj.Veiculo.Placa,
                                           CodigoVeiculo = obj.Veiculo.Codigo,
                                           SemParar = obj.ImportadoDeSemParar,
                                           obj.Praca,
                                           obj.Rodovia,
                                           Tipo = obj.ImportadoDeSemParar == true ? "Sem Parar" : "Pago pelo Motorista",
                                           Valor = obj.Valor.ToString("n2"),
                                           PedagioDuplicado = repAcertoPedagio.PedagioDuplicado(obj.TipoPedagio, obj.Praca, obj.Rodovia, obj.Data, obj.Veiculo.Codigo),
                                           obj.SituacaoPedagio,
                                           DT_RowColor = "#FFFFFF",
                                           obj.TipoPedagio,
                                           obj.MoedaCotacaoBancoCentral,
                                           DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                           ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                           ValorOriginalMoedaEstrangeira = obj.ValorOriginalMoedaEstrangeira.ToString("n2")
                                       }).ToList();

                grid.AdicionaRows(dynListaPedagio);
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

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();

                PreencherPedagio(pedagio, unitOfWork);

                repPedagio.Inserir(pedagio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo." + propOrdenar;
                else if (propOrdenar == "Situacao")
                    propOrdenar = "SituacaoPedagio";

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo." + propOrdenar;
                else if (propOrdenar == "Situacao")
                    propOrdenar = "SituacaoPedagio";

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo);

                var dynPedagio = new
                {
                    pedagio.Codigo,
                    Veiculo = pedagio.Veiculo != null ? new { Codigo = pedagio.Veiculo.Codigo, Descricao = pedagio.Veiculo.Placa, Tipo = pedagio.Veiculo.Tipo } : new { Codigo = 0, Descricao = string.Empty, Tipo = string.Empty },
                    Placa = pedagio.Veiculo == null ? pedagio.PlacaVeiculoNaoCadastrado : null,
                    Valor = pedagio.Valor.ToString("n2"),
                    Data = pedagio.Data.ToString("dd/MM/yyyy HH:mm"),
                    Praca = !string.IsNullOrEmpty(pedagio.Praca) ? pedagio.Praca : string.Empty,
                    Situacao = pedagio.SituacaoPedagio,
                    Rodovia = !string.IsNullOrEmpty(pedagio.Rodovia) ? pedagio.Rodovia : string.Empty,
                    TipoMovimento = pedagio.TipoMovimento != null ? new { Codigo = pedagio.TipoMovimento.Codigo, Descricao = pedagio.TipoMovimento.Descricao } : new { Codigo = 0, Descricao = string.Empty },
                    pedagio.TipoPedagio,
                    pedagio.MoedaCotacaoBancoCentral,
                    DataBaseCRT = pedagio.DataBaseCRT.HasValue ? pedagio.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    ValorMoedaCotacao = pedagio.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = pedagio.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    pedagio.Observacao,
                    Motorista = new {Codigo = pedagio.Motorista?.Codigo ?? 0, Descricao = pedagio.Motorista?.Descricao ?? string.Empty}
                };

                return new JsonpResult(dynPedagio);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo, true);

                if (pedagio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado && pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente)
                    return new JsonpResult(false, true, "Situação não permite alteração.");

                PreencherPedagio(pedagio, unitOfWork);

                repPedagio.Atualizar(pedagio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo);

                if (pedagio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado)
                    return new JsonpResult(false, true, "Só é possível excluir pedágios na situação Lançado.");

                repPedagio.Deletar(pedagio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reabrir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio repConfiguracaoFinanceiraPedagio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio configuracaoPedagio = repConfiguracaoFinanceiraPedagio.BuscarPrimeiroRegistro();

                if (pedagio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pedagio.SituacaoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado)
                    return new JsonpResult(false, true, "Só é possível reabrir pedágios fechados.");

                if (configuracaoPedagio == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    return new JsonpResult(false, true, "Nenhum movimento financeiro padrão configurado para essa operação.");

                string obsMovimentacao = "Reversão de pedágio do veículo " + pedagio.Veiculo.Placa;
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (pedagio.TipoMovimento != null)
                        serProcessoMovimento.GerarMovimentacao(null, pedagio.Data, pedagio.Valor, pedagio.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, pedagio.TipoMovimento.PlanoDeContaDebito, pedagio.TipoMovimento.PlanoDeContaCredito);
                    else
                        serProcessoMovimento.GerarMovimentacao(configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio, pedagio.Data, pedagio.Valor, pedagio.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0);
                }

                pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                pedagio.DataAlteracao = DateTime.Now;
                pedagio.FechamentoPedagio = null;

                repPedagio.Atualizar(pedagio, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedagio, null, "Pedágio Reaberto", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPedagio(Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio repConfiguracaoFinanceiraPedagio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio configuracaoPedagio = repConfiguracaoFinanceiraPedagio.BuscarPrimeiroRegistro();

            int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

            decimal.TryParse(Request.Params("Valor"), out decimal valor);

            DateTime.TryParseExact(Request.Params("Data"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPedagio);

            string rodovia = !string.IsNullOrWhiteSpace(Request.Params("Rodovia")) ? Request.Params("Rodovia") : "";
            string praca = !string.IsNullOrWhiteSpace(Request.Params("Praca")) ? Request.Params("Praca") : "";

            int.TryParse(Request.Params("TipoMovimento"), out int codigoMovimento);
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoMovimento);

            Enum.TryParse(Request.Params("TipoPedagio"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio);

            int codigoMotorista = Request.GetIntParam("Motorista");

            pedagio.TipoPedagio = tipoPedagio;
            pedagio.Veiculo = veiculo;
            pedagio.Valor = valor;
            pedagio.Data = dataPedagio;
            pedagio.Praca = praca;
            pedagio.Rodovia = rodovia;
            pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
            pedagio.DataAlteracao = DateTime.Now;
            pedagio.TipoMovimento = tipoMovimento;
            pedagio.Observacao = Request.GetStringParam("Observacao");
            pedagio.Motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

            if (pedagio.Codigo == 0)
            {
                pedagio.ImportadoDeSemParar = true;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    pedagio.Empresa = this.Usuario.Empresa;
            }

            pedagio.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            pedagio.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
            pedagio.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
            pedagio.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

            // Valida dados
            if (pedagio.Veiculo == null)
                throw new ControllerException("Veículo é obrigatório.");

            if (!(pedagio.Valor > 0))
                throw new ControllerException("Valor é obrigatório.");

            if (pedagio.Data == DateTime.MinValue)
                throw new ControllerException("Data Passagem é obrigatório.");

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (pedagio.Veiculo.Tipo == "T" && pedagio.TipoMovimento == null)
                    throw new ControllerException("Movimento Financeiro é obrigatório quando Veículo é de Terceiro.");

                // Coloca tipo movimento quando o veiculo e 
                if (pedagio.TipoMovimento == null)
                {
                    // É obrigatorio ter movimento financeiro configurado
                    if (configuracaoPedagio == null)
                        throw new ControllerException("Nenhum movimento financeiro padrão configurado para essa operação.");

                    pedagio.TipoMovimento = configuracaoPedagio.TipoMovimentoLancamentoPedagio;
                }
            }
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Passagem", "Data", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Praça", "Praca", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Rodovia", "Rodovia", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, true);

            if (Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio>("Situacao") == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
            grid.AdicionarCabecalho("DataBaseCRT", false);
            grid.AdicionarCabecalho("ValorMoedaCotacao", false);
            grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoVeiculo = 0;
            int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao;
            Enum.TryParse(Request.Params("Situacao"), out situacao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio;
            Enum.TryParse(Request.Params("TipoPedagio"), out tipoPedagio);

            DateTime dataImportacao, dataInicial, dataFinal;
            DateTime.TryParse(Request.Params("DataImportacao"), out dataImportacao);
            DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

            string rodovia = !string.IsNullOrWhiteSpace(Request.Params("Rodovia")) ? Request.Params("Rodovia") : "";
            string praca = !string.IsNullOrWhiteSpace(Request.Params("Praca")) ? Request.Params("Praca") : "";

            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
            string placa = veiculo != null ? veiculo.Placa : "";

            List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagios = repPedagio.Consultar(tipoPedagio, dataImportacao, codigoEmpresa, placa, rodovia, praca, dataInicial, dataFinal, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPedagio.ContarConsultar(tipoPedagio, dataImportacao, codigoEmpresa, placa, rodovia, praca, dataInicial, dataFinal, situacao);

            var lista = (from p in listaPedagios
                         select new
                         {
                             Codigo = p.Codigo.ToString("n0"),
                             Placa = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                             Valor = p.Valor.ToString("n2"),
                             Data = p.Data != null ? p.Data.ToString("dd/MM/yyyy") : string.Empty,
                             Praca = !string.IsNullOrWhiteSpace(p.Praca) ? p.Praca : string.Empty,
                             Rodovia = !string.IsNullOrWhiteSpace(p.Rodovia) ? p.Rodovia : string.Empty,
                             Situacao = p.DescricaoSituacaoPedagio,
                             p.MoedaCotacaoBancoCentral,
                             DataBaseCRT = p.DataBaseCRT.HasValue ? p.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             ValorMoedaCotacao = p.ValorMoedaCotacao.ToString("n10"),
                             ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2"),
                             p.Observacao
                         }).ToList();

            return lista.ToList();
        }

        #endregion
    }
}

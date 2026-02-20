using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Zen.Barcode;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/ControleTacografo")]
    public class ControleTacografoController : BaseController
    {
		#region Construtores

		public ControleTacografoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
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
                return ObterGridPesquisa(unitOfWork, true);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = new Dominio.Entidades.Embarcador.Frota.ControleTacografo();

                PreencherControleTacografo(controleTacografo, unitOfWork);

                repControleTacografo.Inserir(controleTacografo, Auditado);

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

        public async Task<IActionResult> AdicionarEmLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = new Dominio.Entidades.Embarcador.Frota.ControleTacografo();
                List<Dominio.Entidades.Embarcador.Frota.ControleTacografo> controlesTacografos = new List<Dominio.Entidades.Embarcador.Frota.ControleTacografo>();
                List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> dadosEtiquetas = new List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo>();

                var quantidade = Request.GetIntParam("Quantidade");

                for (int i = 0; i < quantidade; i++)
                {
                    controleTacografo = new Dominio.Entidades.Embarcador.Frota.ControleTacografo();

                    PreencherControleTacografo(controleTacografo, unitOfWork);

                    repControleTacografo.Inserir(controleTacografo, Auditado);

                    controlesTacografos.Add(controleTacografo);

                }

                dadosEtiquetas = RetornarDadosEtiqueta(controlesTacografos);

                Etiquetas(dadosEtiquetas, unitOfWork);

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

                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(codigo, true);

                PreencherControleTacografo(controleTacografo, unitOfWork);

                repControleTacografo.Atualizar(controleTacografo, Auditado);

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
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(codigo, false);

                var dynControleTacografo = new
                {
                    controleTacografo.Codigo,
                    DataRecebimento = controleTacografo.DataRecebimento.ToString("dd/MM/yyyy"),
                    DataRetorno = controleTacografo.DataRetorno?.ToString("dd/MM/yyyy") ?? "",
                    controleTacografo.Excesso,
                    controleTacografo.Observacao,
                    controleTacografo.Status,
                    controleTacografo.Situacao,
                    Veiculo = controleTacografo.Veiculo != null ? new { controleTacografo.Veiculo.Codigo, controleTacografo.Veiculo.Descricao } : null,
                    Motorista = controleTacografo.Motorista != null ? new { controleTacografo.Motorista.Codigo, Descricao = controleTacografo.Motorista.Nome } : null
                };

                return new JsonpResult(dynControleTacografo);
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
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(codigo, true);

                if (controleTacografo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repControleTacografo.Deletar(controleTacografo, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoMotorista = Request.GetIntParam("Motorista");
            DateTime dataRecebimentoInicial = Request.GetDateTimeParam("DataRecebimentoInicial");
            DateTime dataRecebimentoFinal = Request.GetDateTimeParam("DataRecebimentoFinal");
            int codigo = Request.GetIntParam("Codigo");
            bool consultaAcerto = Request.GetBoolParam("ConsultaAcerto");

            Dominio.Enumeradores.OpcaoSimNaoPesquisa excesso = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("Excesso");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Disco", "Codigo", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descricao", false);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

            Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.ControleTacografo> controleTacografos = repControleTacografo.Consultar(consultaAcerto, codigo, codigoVeiculo, codigoMotorista, dataRecebimentoInicial, dataRecebimentoFinal, excesso, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repControleTacografo.ContarConsulta(consultaAcerto, codigo, codigoVeiculo, codigoMotorista, dataRecebimentoInicial, dataRecebimentoFinal, excesso, status));

            var lista = (from p in controleTacografos
                         select new
                         {
                             p.Codigo,
                             DataRecebimento = p.DataRecebimento.ToString("dd/MM/yyyy"),
                             Veiculo = p.Veiculo?.Placa_Formatada,
                             Motorista = p.Motorista?.Descricao,
                             p.DescricaoStatus,
                             p.Descricao
                         }).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private void PreencherControleTacografo(Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoMotorista = Request.GetIntParam("Motorista");
            DateTime? dataRetorno = Request.GetNullableDateTimeParam("DataRetorno");

            if (controleTacografo.Codigo == 0)
            {
                controleTacografo.Data = DateTime.Now;
                controleTacografo.Operador = this.Usuario;
            }
            controleTacografo.DataRecebimento = Request.GetDateTimeParam("DataRecebimento");
            controleTacografo.DataRetorno = dataRetorno;
            controleTacografo.Situacao = Request.GetIntParam("Situacao");
            controleTacografo.Observacao = Request.GetStringParam("Observacao");
            controleTacografo.Excesso = Request.GetBoolParam("Excesso");
            controleTacografo.Status = Request.GetBoolParam("Status");
            controleTacografo.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            controleTacografo.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
        }

        public async Task<IActionResult> ImprimirEtiqueta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.ControleTacografo> controlesTacografos = new List<Dominio.Entidades.Embarcador.Frota.ControleTacografo>();
                List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> dadosEtiquetas = new List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo>();

                string msg = string.Empty;
                bool valid = true;

                int codigo = Request.GetIntParam("Codigo");

                if (codigo == 0)
                {
                    valid = false;
                    msg = "Controle de Tacógrafo inválido.";
                }

                if (!valid)
                    return new JsonpResult(false, false, "Existe dado(s) inválido(s) para a geração da etiqueta: " + msg);


                Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(codigo, false);

                controlesTacografos.Add(controleTacografo);

                dadosEtiquetas = RetornarDadosEtiqueta(controlesTacografos);

                return Etiquetas(dadosEtiquetas, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        private JsonpResult Etiquetas(List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> dadosEtiqueta, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R272_EtiquetaControleTacografo, TipoServicoMultisoftware, "Etiqueta Controle Tacógrafo", "ControleTacografo", "EtiquetaControleTacografo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

            string stringConexao = _conexao.StringConexao;
            string nomeCliente = Cliente.NomeFantasia;

            if (!dadosEtiqueta.Any())
                return new JsonpResult(false, false, "Nenhum registro de etiqueta selecionado.");

            Task.Factory.StartNew(() => GerarEtiqueta(nomeCliente, stringConexao, relatorioControleGeracao, dadosEtiqueta));
            return new JsonpResult(true);
        }

        private void GerarEtiqueta(string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> dadosEtiqueta)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.EtiquetaControleTacografo)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DadosEtiqueta", dadosEtiqueta.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> RetornarDadosEtiqueta(List<Dominio.Entidades.Embarcador.Frota.ControleTacografo> controleTacografos)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo> listaRetorno = new List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo>();
            BarcodeMetrics1d metricas = new BarcodeMetrics1d();

            foreach (var controleTacografo in controleTacografos)
            {
                Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo etiqueta = new Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo();

                etiqueta.Motorista = controleTacografo.Motorista?.Descricao ?? "";
                etiqueta.Veiculo = controleTacografo.Veiculo?.Placa_Formatada ?? "";

                string codigoBarrasFormatado = controleTacografo.Codigo.ToString("D").PadLeft(8, '0');

                byte[] codigoBarras = Utilidades.Barcode.Gerar(codigoBarrasFormatado, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);

                etiqueta.CodigoDeBarras = codigoBarras;

                listaRetorno.Add(etiqueta);
            }

            return listaRetorno;
        }

        #endregion
    }
}

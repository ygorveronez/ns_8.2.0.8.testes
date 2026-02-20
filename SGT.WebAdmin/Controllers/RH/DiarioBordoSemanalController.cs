using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/DiarioBordoSemanal")]
    public class DiarioBordoSemanalController : BaseController
    {
		#region Construtores

		public DiarioBordoSemanalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
                Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordoSemanal = new Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal();

                PreencherDiarioBordoSemanal(diarioBordoSemanal, unitOfWork);

                diarioBordoSemanal.Usuario = this.Usuario;
                diarioBordoSemanal.DataGeracao = DateTime.Now;
                diarioBordoSemanal.Numero = repDiarioBordoSemanal.BuscarProximoNumero();

                repDiarioBordoSemanal.Inserir(diarioBordoSemanal, Auditado);

                unitOfWork.CommitChanges();

                var dynDiarioBordoSemanal = new
                {
                    diarioBordoSemanal.Codigo
                };

                return new JsonpResult(dynDiarioBordoSemanal, true, "Sucesso");
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

                Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
                Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordoSemanal = repDiarioBordoSemanal.BuscarPorCodigo(codigo, true);

                PreencherDiarioBordoSemanal(diarioBordoSemanal, unitOfWork);
                repDiarioBordoSemanal.Atualizar(diarioBordoSemanal, Auditado);

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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
                Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordoSemanal = repDiarioBordoSemanal.BuscarPorCodigo(codigo, false);

                var dynDiarioBordoSemanal = new
                {
                    diarioBordoSemanal.Codigo,
                    diarioBordoSemanal.Numero,
                    Carga = diarioBordoSemanal.Carga != null ? new { diarioBordoSemanal.Carga.Codigo, diarioBordoSemanal.Carga.Descricao } : null,
                    Veiculo = diarioBordoSemanal.Veiculo != null ? new { diarioBordoSemanal.Veiculo.Codigo, diarioBordoSemanal.Veiculo.Descricao } : null,
                    Motorista = diarioBordoSemanal.Motorista != null ? new { diarioBordoSemanal.Motorista.Codigo, diarioBordoSemanal.Motorista.Descricao } : null,
                    DataInicio = diarioBordoSemanal.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = diarioBordoSemanal.DataFim.ToString("dd/MM/yyyy"),
                    diarioBordoSemanal.SituacaoDiarioBordoSemanal,
                    diarioBordoSemanal.Observacao
                };

                return new JsonpResult(dynDiarioBordoSemanal);
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
                Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
                Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordoSemanal = repDiarioBordoSemanal.BuscarPorCodigo(codigo, true);

                if (diarioBordoSemanal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repDiarioBordoSemanal.Deletar(diarioBordoSemanal, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Verifique se o cadastro não possui vínculos, caso exita, favor alterar a sua situação para Cancelado..");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                // Busca informacoes
                Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordo = repDiarioBordoSemanal.BuscarPorCodigo(codigo);
                var arquivo  = ReportRequest.WithType(ReportType.DiarioBordoSemanal)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo.ToString())
                    .CallReport()
                    .GetContentFile();
                
                return Arquivo(arquivo, "application/pdf", "Diário de Bordo - " + diarioBordo.Numero + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDiarioBordoSemanal(Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal diarioBordoSemanal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoCarga = Request.GetIntParam("Carga");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal situacao = Request.GetEnumParam("SituacaoDiarioBordoSemanal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal.Aberto);

            diarioBordoSemanal.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
            diarioBordoSemanal.Carga = codigoCarga > 0 ? repCarga.BuscarPorCodigo(codigoCarga) : null;
            diarioBordoSemanal.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            diarioBordoSemanal.DataInicio = dataInicio;
            diarioBordoSemanal.DataFim = dataInicio.AddDays(6);
            diarioBordoSemanal.Observacao = Request.GetStringParam("Observacao");
            diarioBordoSemanal.SituacaoDiarioBordoSemanal = situacao;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

            // Dados do filtro
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoCarga = Request.GetIntParam("Carga");
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int numero = Request.GetIntParam("Numero");

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal situacao = Request.GetEnumParam("SituacaoDiarioBordoSemanal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal.Todas);

            // Consulta
            Repositorio.Embarcador.RH.DiarioBordoSemanal repDiarioBordoSemanal = new Repositorio.Embarcador.RH.DiarioBordoSemanal(unitOfWork);
            List<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal> diarioBordoSemanal = repDiarioBordoSemanal.Consultar(codigoMotorista, codigoCarga, codigoVeiculo, numero, dataInicio, dataFim, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDiarioBordoSemanal.ContarConsulta(codigoMotorista, codigoCarga, codigoVeiculo, numero, dataInicio, dataFim, situacao);

            var lista = (from p in diarioBordoSemanal
                         select new
                         {
                             p.Codigo,
                             p.Numero,
                             Motorista = p.Motorista?.Descricao ?? "",
                             Veiculo = p.Veiculo?.Descricao ?? "",
                             DataInicio = p.DataInicio.ToString("dd/MM/yyyy"),
                             DataFim = p.DataFim.ToString("dd/MM/yyyy"),
                             SituacaoDiarioBordoSemanal = p.SituacaoDiarioBordoSemanal.ObterDescricao()
                         }).ToList();

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicio", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Final", "DataFim", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "SituacaoDiarioBordoSemanal", 20, Models.Grid.Align.left, false);

            return grid;
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Financeiros.Conciliacao
{
    [CustomAuthorize("Financeiros/ExtratoBancario")]
    public class ExtratoBancarioController : BaseController
    {
		#region Construtores

		public ExtratoBancarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
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
                int planoConta = 0, empresa = 0, tipoLancamento = 0;
                int.TryParse(Request.Params("ExtratoBancarioTipoLancamento"), out tipoLancamento);
                int.TryParse(Request.Params("PlanoConta"), out planoConta);
                int.TryParse(Request.Params("Empresa"), out empresa);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);

                decimal valorMovimento = 0;
                decimal.TryParse(Request.Params("Valor"), out valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumentoMovimento"), out tipo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito;
                Enum.TryParse(Request.Params("DebitoCredito"), out debitoCredito);

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario();

                extratoBancario.DataGeracaoMovimento = dataGeracaoMovimento;
                extratoBancario.DataMovimento = dataMovimento;
                extratoBancario.Valor = valorMovimento;
                extratoBancario.TipoDocumentoMovimento = tipo;
                extratoBancario.Documento = Request.Params("Documento");
                extratoBancario.Observacao = Request.Params("Observacao");
                extratoBancario.CodigoLancamento = Request.Params("CodigoLancamento");
                extratoBancario.DebitoCredito = debitoCredito;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    extratoBancario.Empresa = Usuario.Empresa;
                else
                    extratoBancario.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                extratoBancario.TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual;
                extratoBancario.ExtratoConcolidado = false;
                extratoBancario.Colaborador = this.Usuario;
                if (planoConta > 0)
                    extratoBancario.PlanoConta = repPlanoConta.BuscarPorCodigo(planoConta);
                if (tipoLancamento > 0)
                    extratoBancario.ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(tipoLancamento);

                if (extratoBancario.PlanoConta.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                repExtratoBancario.Inserir(extratoBancario, Auditado);

                unitOfWork.CommitChanges();

                var dynExtratoBancario = new
                {
                    extratoBancario.Codigo,
                    TipoDocumento = extratoBancario.Observacao,
                    NumeroDocumento = extratoBancario.Documento,
                    Valor = extratoBancario.Valor.ToString("n2")
                };
                return new JsonpResult(dynExtratoBancario);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int planoConta = 0, empresa = 0, tipoLancamento = 0;
                int.TryParse(Request.Params("ExtratoBancarioTipoLancamento"), out tipoLancamento);
                int.TryParse(Request.Params("PlanoConta"), out planoConta);
                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);

                decimal valorMovimento = 0;
                decimal.TryParse(Request.Params("Valor"), out valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumentoMovimento"), out tipo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito;
                Enum.TryParse(Request.Params("DebitoCredito"), out debitoCredito);

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = repExtratoBancario.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                extratoBancario.DataMovimento = dataMovimento;
                extratoBancario.Valor = valorMovimento;
                extratoBancario.TipoDocumentoMovimento = tipo;
                extratoBancario.Documento = Request.Params("Documento");
                extratoBancario.Observacao = Request.Params("Observacao");
                extratoBancario.CodigoLancamento = Request.Params("CodigoLancamento");
                extratoBancario.DebitoCredito = debitoCredito;
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    extratoBancario.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                if (planoConta > 0)
                    extratoBancario.PlanoConta = repPlanoConta.BuscarPorCodigo(planoConta);
                if (tipoLancamento > 0)
                    extratoBancario.ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(tipoLancamento);

                if (extratoBancario.PlanoConta.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                repExtratoBancario.Atualizar(extratoBancario, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = repExtratoBancario.BuscarPorCodigo(codigo);

                if (extratoBancario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynExtratoBancario = new
                {
                    extratoBancario.Codigo,
                    DataMovimento = extratoBancario.DataMovimento.ToString("dd/MM/yyyy"),
                    Valor = extratoBancario.Valor,
                    TipoDocumentoMovimento = extratoBancario.TipoDocumentoMovimento,
                    Documento = extratoBancario.Documento,
                    extratoBancario.Observacao,
                    extratoBancario.DebitoCredito,
                    extratoBancario.CodigoLancamento,
                    PlanoConta = new { Codigo = extratoBancario.PlanoConta != null ? extratoBancario.PlanoConta.Codigo : 0, Descricao = extratoBancario.PlanoConta != null ? "(" + extratoBancario.PlanoConta.Plano + ") " + extratoBancario.PlanoConta.Descricao : "" },
                    Empresa = new { Codigo = extratoBancario.Empresa != null ? extratoBancario.Empresa.Codigo : 0, Descricao = extratoBancario.Empresa != null ? extratoBancario.Empresa.Descricao : "" },
                    ExtratoBancarioTipoLancamento = new { Codigo = extratoBancario.ExtratoBancarioTipoLancamento != null ? extratoBancario.ExtratoBancarioTipoLancamento.Codigo : 0, Descricao = extratoBancario.ExtratoBancarioTipoLancamento != null ? extratoBancario.ExtratoBancarioTipoLancamento.DescricaoCompleta : "" }
                };

                return new JsonpResult(dynExtratoBancario);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = repExtratoBancario.BuscarPorCodigo(codigo);

                if (extratoBancario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria> conciliacoes = repConciliacaoBancaria.BuscarPorExtrato(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Cancelado);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacao in conciliacoes)
                    repConciliacaoBancaria.DeletarExtratoBancario(codigo, conciliacao.Codigo);

                repExtratoBancario.Deletar(extratoBancario, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo se econtra em uma conciliação bancária que não está cancelada.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarExtratoBancario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoPlanoConta = Request.GetIntParam("CodigoPlanoConta");
                int codigoEmpresa = Request.GetIntParam("CodigoEmpresa");
                bool apenasLeitura = Request.GetBoolParam("ApenasLeitura");

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                Dominio.Entidades.Empresa empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = Usuario.Empresa;

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");

                if (!apenasLeitura)
                    unitOfWork.Start();

                Servicos.DTO.CustomFile file = files[0];                
                StreamReader streamReader = new StreamReader(file.InputStream);

                decimal saldoInicial = 0m;
                decimal saldoFinal = 0m;

                List<dynamic> dadosExtratoBancarioImportacao = new List<dynamic>();

                if (System.IO.Path.GetExtension(file.FileName).ToLower() == ".ofx")
                    dadosExtratoBancarioImportacao = Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ImportarExtratoBancarioOFX(this.Usuario, empresa, planoConta.Codigo, streamReader, unitOfWork, Auditado, apenasLeitura, out saldoInicial, out saldoFinal);
                else
                    dadosExtratoBancarioImportacao = Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ImportarExtratoBancario(this.Usuario, empresa, planoConta.Codigo, streamReader, unitOfWork, Auditado, apenasLeitura, out saldoInicial, out saldoFinal);

                if (!apenasLeitura)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, true, "Importação do extrato foi realizada com sucesso.");
                }

                if (dadosExtratoBancarioImportacao.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível ler nenhum registro!");

                dynamic retorno = new
                {
                    SaldoInicial = saldoInicial.ToString("n2"),
                    SaldoFinal = saldoFinal.ToString("n2"),
                    Itens = (from obj in dadosExtratoBancarioImportacao
                             select new
                             {
                                 Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                                 Data = obj.DataMovimento.ToString("dd/MM/yyyy"),
                                 Valor = obj.Valor.ToString("n2"),
                                 DebitoCredito = obj.DescricaoDebitoCredito,
                                 TipoDocumento = obj.DescricaoTipoDocumentoMovimento,
                                 NumeroDocumento = obj.Documento,
                                 PlanoConta = obj.PlanoConta.Descricao,
                                 TipoLancamento = obj.ExtratoBancarioTipoLancamento?.Descricao ?? "",
                                 CodigoTipoLancamento = obj.CodigoLancamento,
                                 Observacao = obj.Observacao
                             }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (ServicoException se)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, se.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o extrato. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", "Codigo", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data", "DataMovimento", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Nº Documento", "Documento", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Doc", "DescricaoTipoDocumentoMovimento", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Conta de Conta", "PlanoConta", 15, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataMovimento;
            DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);
            decimal valor = 0;
            decimal.TryParse(Request.Params("Valor"), out valor);

            string documento = Request.Params("Documento");
            string observacao = Request.Params("Observacao");

            int codigo = 0, planoConta = 0, empresa = 0;
            int.TryParse(Request.Params("Codigo"), out codigo);
            int.TryParse(Request.Params("PlanoConta"), out planoConta);
            int.TryParse(Request.Params("Empresa"), out empresa);

            bool? extratoConsolidado = Request.GetNullableBoolParam("ExtratoConsolidado");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito;
            Enum.TryParse(Request.Params("DebitoCredito"), out debitoCredito);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                empresa = this.Usuario.Empresa.Codigo;

            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario> listaExtratoBancario = repExtratoBancario.Consultar(debitoCredito, codigo, empresa, dataMovimento, valor, documento, observacao, planoConta, extratoConsolidado, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repExtratoBancario.ContarConsulta(debitoCredito, codigo, empresa, dataMovimento, valor, documento, observacao, planoConta, extratoConsolidado);
            var lista = (from p in listaExtratoBancario
                         select new
                         {
                             Codigo = p.Codigo,
                             DataMovimento = p.DataMovimento.ToString("dd/MM/yyyy"),
                             Valor = p.Valor.ToString("n2"),
                             p.Documento,
                             p.DescricaoTipoDocumentoMovimento,
                             PlanoConta = p.PlanoConta != null ? "(" + p.PlanoConta.Plano + ") " + p.PlanoConta.Descricao : string.Empty,
                             DT_RowColor = p.ExtratoConcolidado ? CorGrid.Verde : CorGrid.Branco
                         }).ToList();

            return lista.ToList();
        }

        #endregion
    }
}

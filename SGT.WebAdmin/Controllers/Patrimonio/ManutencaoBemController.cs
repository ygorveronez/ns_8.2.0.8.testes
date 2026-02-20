using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/ManutencaoBem")]
    public class ManutencaoBemController : BaseController
    {
		#region Construtores

		public ManutencaoBemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Bem"), out int codigoBem);
                int.TryParse(Request.Params("MotivoDefeito"), out int codigoDefeito);
                DateTime? dataEntrega = Request.GetNullableDateTimeParam("DataEntrega");
                DateTime? dataRetorno = Request.GetNullableDateTimeParam("DataRetorno");


                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem;
                Enum.TryParse(Request.Params("Status"), out statusBem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Patrimônio", "Bem", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Defeito", "MotivoDefeito", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número de Série", "NumeroSerie", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Retorno", "DataRetorno", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Entrega", "DataEntrega", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Orçado", "ValorOrcado", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Patrimonio.BemManutencao repBemManutencao = new Repositorio.Embarcador.Patrimonio.BemManutencao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao> manutencoesBem = repBemManutencao.Consultar(codigoEmpresa, codigoBem, codigoDefeito, dataEntrega, dataRetorno, statusBem, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBemManutencao.ContarConsulta(codigoEmpresa, codigoBem, statusBem));

                var lista = (from p in manutencoesBem
                             select new
                             {
                                 p.Codigo,
                                 Bem = p.Bem?.Descricao ?? string.Empty,
                                 NumeroSerie = p.Bem?.NumeroSerie ?? string.Empty,
                                 DescricaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBemHelper.ObterDescricao(p.Status),
                                 DataEntrega = p.DataEntrega != null ? p.DataEntrega?.ToString("dd/MM/yyyy") : string.Empty,
                                 ValorOrcado = p.ValorOrcado.ToString("n2"),
                                 MotivoDefeito = p.MotivoDefeito != null ? p.MotivoDefeito.Descricao : string.Empty,
                                 DataRetorno = p.DataRetorno != null ? p.DataRetorno?.ToString("dd/MM/yyyy") : string.Empty


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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Patrimonio.BemManutencao repBemManutencao = new Repositorio.Embarcador.Patrimonio.BemManutencao(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemManutencao bemManutencao = new Dominio.Entidades.Embarcador.Patrimonio.BemManutencao();

                PreencherBemManutencao(bemManutencao, unitOfWork);
                repBemManutencao.Inserir(bemManutencao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Patrimonio.BemManutencao repBemManutencao = new Repositorio.Embarcador.Patrimonio.BemManutencao(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemManutencao bemManutencao = repBemManutencao.BuscarPorCodigo(codigo, true);

                PreencherBemManutencao(bemManutencao, unitOfWork);
                repBemManutencao.Atualizar(bemManutencao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Patrimonio.BemManutencao repBemManutencao = new Repositorio.Embarcador.Patrimonio.BemManutencao(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemManutencao bemManutencao = repBemManutencao.BuscarPorCodigo(codigo);

                var dynBemManutencao = new
                {
                    bemManutencao.Codigo,
                    DataEntrega = bemManutencao.DataEntrega.HasValue ? bemManutencao.DataEntrega.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataGarantia = bemManutencao.DataGarantia.HasValue ? bemManutencao.DataGarantia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    ValorOrcado = bemManutencao.ValorOrcado.ToString("n2"),
                    ValorPago = bemManutencao.ValorPago.ToString("n2"),
                    bemManutencao.Status,
                    bemManutencao.ObservacaoSaida,
                    bemManutencao.ObservacaoRetorno,
                    Bem = bemManutencao.Bem != null ? new { bemManutencao.Bem.Codigo, bemManutencao.Bem.Descricao } : null,
                    NotaFiscal = bemManutencao.NotaFiscal != null ? new { bemManutencao.NotaFiscal.Codigo, bemManutencao.NotaFiscal.Descricao } : null,
                    DocumentoEntrada = bemManutencao.DocumentoEntrada != null ? new { bemManutencao.DocumentoEntrada.Codigo, Descricao = bemManutencao.DocumentoEntrada.Numero } : null,
                    Pessoa = bemManutencao.Pessoa != null ? new { bemManutencao.Pessoa.Codigo, bemManutencao.Pessoa.Descricao } : null,
                };

                return new JsonpResult(dynBemManutencao);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Patrimonio.BemManutencao repBemManutencao = new Repositorio.Embarcador.Patrimonio.BemManutencao(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemManutencao bemManutencao = repBemManutencao.BuscarPorCodigo(codigo, true);

                if (bemManutencao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repBemManutencao.Deletar(bemManutencao, Auditado);
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

        private void PreencherBemManutencao(Dominio.Entidades.Embarcador.Patrimonio.BemManutencao bemManutencao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
            Repositorio.Embarcador.Patrimonio.MotivoDefeito repMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("NotaFiscal"), out int codigoNotaFiscal);
            int.TryParse(Request.Params("DocumentoEntrada"), out int codigoDocumentoEntrada);
            int.TryParse(Request.Params("Bem"), out int codigoBem);
            int.TryParse(Request.Params("Defeito"), out int codigoDefeito);


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            double.TryParse(Request.Params("Pessoa"), out double pessoa);
            decimal.TryParse(Request.Params("ValorOrcado"), out decimal valorOrcado);
            decimal.TryParse(Request.Params("ValorPago"), out decimal valorPago);

            string observacaoSaida = Request.Params("ObservacaoSaida");
            string observacaoRetorno = Request.Params("ObservacaoRetorno");

            DateTime.TryParse(Request.Params("DataEntrega"), out DateTime dataEntrega);
            DateTime.TryParse(Request.Params("DataGarantia"), out DateTime dataGarantia);
            DateTime.TryParse(Request.Params("DataRetorno"), out DateTime dataRetorno);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem;
            Enum.TryParse(Request.Params("Status"), out statusBem);

            bemManutencao.Status = statusBem;
            bemManutencao.MotivoDefeito = repMotivoDefeito.BuscarPorCodigo(codigoDefeito, false);
            bemManutencao.ValorOrcado = valorOrcado;
            bemManutencao.ValorPago = valorPago;
            bemManutencao.ObservacaoSaida = observacaoSaida;
            bemManutencao.ObservacaoRetorno = observacaoRetorno;
            bemManutencao.Bem = repBem.BuscarPorCodigo(codigoBem);

            if (dataRetorno > DateTime.MinValue)
            {
                bemManutencao.DataRetorno = dataRetorno;
            }
            if (dataEntrega > DateTime.MinValue)
                bemManutencao.DataEntrega = dataEntrega;
            else
                bemManutencao.DataEntrega = null;
            if (dataGarantia > DateTime.MinValue)
                bemManutencao.DataGarantia = dataGarantia;
            else
                bemManutencao.DataGarantia = null;

            if (codigoNotaFiscal > 0)
                bemManutencao.NotaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);
            else
                bemManutencao.NotaFiscal = null;
            if (codigoDocumentoEntrada > 0)
                bemManutencao.DocumentoEntrada = repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntrada);
            else
                bemManutencao.DocumentoEntrada = null;
            if (pessoa > 0)
                bemManutencao.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
            else
                bemManutencao.Pessoa = null;
            if (codigoEmpresa > 0)
                bemManutencao.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            else
                bemManutencao.Empresa = bemManutencao.Bem.Empresa;
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{
    [CustomAuthorize("FaturamentosMensais/FaturamentoMensalCliente")]
    public class FaturamentoMensalClienteController : BaseController
    {
		#region Construtores

		public FaturamentoMensalClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoGrupoFaturamento = 0, codigoServico = 0, diaFatura = 0;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("DiaFatura"), out diaFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Faturamento", "GrupoFaturamento", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Dia Fatura", "DiaFatura", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente> listaFaturamentoMensalCliente = repFaturamentoMensalCliente.Consulta(false, null, this.Usuario.Empresa.Codigo, 0, 0, ativo, codigoGrupoFaturamento, cnpjPessoa, diaFatura, codigoServico, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensalCliente.ContaConsulta(this.Usuario.Empresa.Codigo, 0, 0, ativo, codigoGrupoFaturamento, cnpjPessoa, diaFatura, codigoServico));
                var lista = (from p in listaFaturamentoMensalCliente
                             select new
                             {
                                 p.Codigo,
                                 Pessoa = p.Pessoa.Nome,
                                 GrupoFaturamento = p.FaturamentoMensalGrupo.Descricao,
                                 DiaFatura = p.DiaFatura.ToString("n0"),
                                 p.DescricaoAtivo
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

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico repFaturamentoMensalServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                int diaFatura = 0, codigoGrupoFaturamento = 0, codigoServicoPrincipal = 0, codigoNaturezaOperacao = 0, codigoTipoMovimento = 0, codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("DiaFatura"), out diaFatura);
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("ServicoPrincipal"), out codigoServicoPrincipal);
                int.TryParse(Request.Params("NaturezaOperacao"), out codigoNaturezaOperacao);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                decimal valorTotal = 0, valorServicoPrincipal = 0, valorAdesao = 0;
                decimal.TryParse(Request.Params("ValorAdesao"), out valorAdesao);
                decimal.TryParse(Request.Params("ValorTotal"), out valorTotal);
                decimal.TryParse(Request.Params("ValorServicoPrincipal"), out valorServicoPrincipal);

                DateTime dataUltimaFatura, dataProximaFatura, dataContrato, dataLancamento, dataLancamentoAte;
                DateTime.TryParse(Request.Params("DataUltimaFatura"), out dataUltimaFatura);
                DateTime.TryParse(Request.Params("DataProximaFatura"), out dataProximaFatura);
                DateTime.TryParse(Request.Params("DataContrato"), out dataContrato);
                DateTime.TryParse(Request.Params("DataLancamento"), out dataLancamento);
                DateTime.TryParse(Request.Params("DataLancamentoAte"), out dataLancamentoAte);

                string observacao = Request.Params("Observacao");
                string numeroPedidoCompra = Request.Params("NumeroPedidoCompra");
                string numeroPedidoItemCompra = Request.Params("NumeroPedidoItemCompra");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota tipoNota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota.Todas;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);
                Enum.TryParse(Request.Params("TipoNota"), out tipoNota);

                if (diaFatura > 31)
                    return new JsonpResult(false, "Favor informe uma dia da fatura menor que 31.");

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensalCliente = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente();
                faturamentoMensalCliente.Ativo = ativo;
                faturamentoMensalCliente.TipoNotaFiscal = tipoNota;
                if (codigoBoletoConfiguracao > 0)
                    faturamentoMensalCliente.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    faturamentoMensalCliente.BoletoConfiguracao = null;
                faturamentoMensalCliente.DataProximaFatura = null;
                faturamentoMensalCliente.DataUltimaFatura = null;
                faturamentoMensalCliente.DiaFatura = diaFatura;
                faturamentoMensalCliente.Empresa = this.Usuario.Empresa;
                faturamentoMensalCliente.FaturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigoGrupoFaturamento);
                faturamentoMensalCliente.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacao);
                faturamentoMensalCliente.Observacao = observacao;
                faturamentoMensalCliente.Pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                faturamentoMensalCliente.Servico = repServico.BuscarPorCodigo(codigoServicoPrincipal);
                faturamentoMensalCliente.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                faturamentoMensalCliente.TipoObservacaoFaturamentoMensal = tipoObservacao;
                faturamentoMensalCliente.ValorServicoPrincipal = valorServicoPrincipal;
                faturamentoMensalCliente.ValorAdesao = valorAdesao;
                faturamentoMensalCliente.NumeroPedidoCompra = numeroPedidoCompra;
                faturamentoMensalCliente.NumeroPedidoItemCompra = numeroPedidoItemCompra;
                if (dataContrato > DateTime.MinValue)
                    faturamentoMensalCliente.DataContrato = dataContrato;
                if (dataLancamento > DateTime.MinValue)
                    faturamentoMensalCliente.DataLancamento = dataLancamento;
                if (dataLancamentoAte > DateTime.MinValue)
                    faturamentoMensalCliente.DataLancamentoAte = dataLancamentoAte;
                decimal valorTotalGeral = valorServicoPrincipal;

                repFaturamentoMensalCliente.Inserir(faturamentoMensalCliente, Auditado);

                dynamic dynServicosExtras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ServicosExtras"));
                foreach (var dynServico in dynServicosExtras)
                {
                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico servicoExtra = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico();

                    servicoExtra.FaturamentoMensalCliente = faturamentoMensalCliente;
                    DateTime DataLancamento, DataLancamentoAte;
                    DateTime.TryParse((string)dynServico.DataLancamentoServico, out DataLancamento);
                    if (DataLancamento > DateTime.MinValue)
                        servicoExtra.DataLancamento = DataLancamento;
                    DateTime.TryParse((string)dynServico.DataLancamentoServicoAte, out DataLancamentoAte);
                    if (DataLancamentoAte > DateTime.MinValue)
                        servicoExtra.DataLancamentoAte = DataLancamentoAte;
                    servicoExtra.Observacao = (string)dynServico.ObservacaoServicoExtra;
                    servicoExtra.Quantidade = decimal.Parse((string)dynServico.Quantidade);
                    servicoExtra.ValorTotal = decimal.Parse((string)dynServico.ValorTotalServicoExtra);
                    servicoExtra.ValorUnitario = decimal.Parse((string)dynServico.ValorUnitario);
                    servicoExtra.Servico = repServico.BuscarPorCodigo(int.Parse((string)dynServico.CodigoServico));
                    valorTotalGeral += servicoExtra.ValorTotal;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacaoServicoExtra = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                    Enum.TryParse((string)dynServico.TipoObservacaoServicoExtra, out tipoObservacaoServicoExtra);
                    servicoExtra.TipoObservacaoFaturamentoMensal = tipoObservacaoServicoExtra;

                    servicoExtra.NumeroPedidoCompra = (string)dynServico.NumeroPedidoCompraExtra;
                    servicoExtra.NumeroPedidoItemCompra = (string)dynServico.NumeroPedidoItemCompraExtra;
                    servicoExtra.Historico = (string)dynServico.Historico;

                    repFaturamentoMensalServico.Inserir(servicoExtra, Auditado);
                }

                faturamentoMensalCliente.ValorTotal = valorTotalGeral;
                repFaturamentoMensalCliente.Atualizar(faturamentoMensalCliente);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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
                unitOfWork.Start();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico repFaturamentoMensalServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                int diaFatura = 0, codigoGrupoFaturamento = 0, codigoServicoPrincipal = 0, codigoNaturezaOperacao = 0, codigoTipoMovimento = 0, codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("DiaFatura"), out diaFatura);
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("ServicoPrincipal"), out codigoServicoPrincipal);
                int.TryParse(Request.Params("NaturezaOperacao"), out codigoNaturezaOperacao);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                decimal valorTotal = 0, valorServicoPrincipal = 0, valorAdesao = 0;
                decimal.TryParse(Request.Params("ValorAdesao"), out valorAdesao);
                decimal.TryParse(Request.Params("ValorTotal"), out valorTotal);
                decimal.TryParse(Request.Params("ValorServicoPrincipal"), out valorServicoPrincipal);

                DateTime dataUltimaFatura, dataProximaFatura, dataContrato, dataLancamento, dataLancamentoAte;
                DateTime.TryParse(Request.Params("DataUltimaFatura"), out dataUltimaFatura);
                DateTime.TryParse(Request.Params("DataProximaFatura"), out dataProximaFatura);
                DateTime.TryParse(Request.Params("DataContrato"), out dataContrato);
                DateTime.TryParse(Request.Params("DataLancamento"), out dataLancamento);
                DateTime.TryParse(Request.Params("DataLancamentoAte"), out dataLancamentoAte);

                string observacao = Request.Params("Observacao");
                string numeroPedidoCompra = Request.Params("NumeroPedidoCompra");
                string numeroPedidoItemCompra = Request.Params("NumeroPedidoItemCompra");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota tipoNota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota.Todas;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);
                Enum.TryParse(Request.Params("TipoNota"), out tipoNota);

                if (diaFatura > 31)
                    return new JsonpResult(false, "Favor informe uma dia da fatura menor que 31.");

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensalCliente = repFaturamentoMensalCliente.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                faturamentoMensalCliente.Ativo = ativo;
                faturamentoMensalCliente.TipoNotaFiscal = tipoNota;
                if (codigoBoletoConfiguracao > 0)
                    faturamentoMensalCliente.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    faturamentoMensalCliente.BoletoConfiguracao = null;
                faturamentoMensalCliente.DataProximaFatura = null;
                faturamentoMensalCliente.DataUltimaFatura = null;
                faturamentoMensalCliente.DiaFatura = diaFatura;
                faturamentoMensalCliente.Empresa = this.Usuario.Empresa;
                faturamentoMensalCliente.FaturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigoGrupoFaturamento);
                faturamentoMensalCliente.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacao);
                faturamentoMensalCliente.Observacao = observacao;
                faturamentoMensalCliente.Pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                faturamentoMensalCliente.Servico = repServico.BuscarPorCodigo(codigoServicoPrincipal);
                faturamentoMensalCliente.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                faturamentoMensalCliente.TipoObservacaoFaturamentoMensal = tipoObservacao;
                faturamentoMensalCliente.ValorServicoPrincipal = valorServicoPrincipal;
                faturamentoMensalCliente.ValorAdesao = valorAdesao;
                faturamentoMensalCliente.NumeroPedidoCompra = numeroPedidoCompra;
                faturamentoMensalCliente.NumeroPedidoItemCompra = numeroPedidoItemCompra;
                if (dataContrato > DateTime.MinValue)
                    faturamentoMensalCliente.DataContrato = dataContrato;
                if (dataLancamento > DateTime.MinValue)
                    faturamentoMensalCliente.DataLancamento = dataLancamento;
                if (dataLancamentoAte > DateTime.MinValue)
                    faturamentoMensalCliente.DataLancamentoAte = dataLancamentoAte;
                decimal valorTotalGeral = valorServicoPrincipal;

                repFaturamentoMensalServico.DeletarPorFaturamento(faturamentoMensalCliente.Codigo);
                dynamic dynServicosExtras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ServicosExtras"));
                foreach (var dynServico in dynServicosExtras)
                {
                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico servicoExtra;
                    servicoExtra = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico();
                    servicoExtra.FaturamentoMensalCliente = faturamentoMensalCliente;
                    DateTime DataLancamento, DataLancamentoAte;
                    DateTime.TryParse((string)dynServico.DataLancamentoServico, out DataLancamento);
                    if (DataLancamento > DateTime.MinValue)
                        servicoExtra.DataLancamento = DataLancamento;
                    DateTime.TryParse((string)dynServico.DataLancamentoServicoAte, out DataLancamentoAte);
                    if (DataLancamentoAte > DateTime.MinValue)
                        servicoExtra.DataLancamentoAte = DataLancamentoAte;
                    servicoExtra.Observacao = (string)dynServico.ObservacaoServicoExtra;
                    servicoExtra.Quantidade = decimal.Parse((string)dynServico.Quantidade);
                    servicoExtra.ValorTotal = decimal.Parse((string)dynServico.ValorTotalServicoExtra);
                    servicoExtra.ValorUnitario = decimal.Parse((string)dynServico.ValorUnitario);
                    servicoExtra.Servico = repServico.BuscarPorCodigo(int.Parse((string)dynServico.CodigoServico));
                    valorTotalGeral += servicoExtra.ValorTotal;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacaoServicoExtra = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                    Enum.TryParse((string)dynServico.TipoObservacaoServicoExtra, out tipoObservacaoServicoExtra);
                    servicoExtra.TipoObservacaoFaturamentoMensal = tipoObservacaoServicoExtra;

                    servicoExtra.NumeroPedidoCompra = (string)dynServico.NumeroPedidoCompraExtra;
                    servicoExtra.NumeroPedidoItemCompra = (string)dynServico.NumeroPedidoItemCompraExtra;
                    servicoExtra.Historico = (string)dynServico.Historico;

                    repFaturamentoMensalServico.Inserir(servicoExtra, Auditado);
                }

                faturamentoMensalCliente.ValorTotal = valorTotalGeral;

                repFaturamentoMensalCliente.Atualizar(faturamentoMensalCliente, Auditado);
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
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensalCliente = repFaturamentoMensalCliente.BuscarPorCodigo(codigo);
                Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal servFaturamentoMensal = new Servicos.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                DateTime? dataUltimaFatura, dataProximaFatura;
                dataUltimaFatura = servFaturamentoMensal.UltimaDataVencimento(codigo, unitOfWork);
                dataProximaFatura = servFaturamentoMensal.ProximaDataVencimento(codigo, unitOfWork);

                var dynFaturamentoCliente = new
                {
                    faturamentoMensalCliente.Codigo,
                    faturamentoMensalCliente.Ativo,
                    TipoNota = faturamentoMensalCliente.TipoNotaFiscal,
                    faturamentoMensalCliente.DiaFatura,
                    ValorTotal = servFaturamentoMensal.ValorTotalFaturamentoCliente(faturamentoMensalCliente.Codigo, dataProximaFatura, unitOfWork),
                    DataUltimaFatura = dataUltimaFatura != null && dataUltimaFatura.HasValue ? dataUltimaFatura.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataProximaFatura = dataProximaFatura != null && dataProximaFatura.HasValue ? dataProximaFatura.Value.ToString("dd/MM/yyyy") : string.Empty,
                    faturamentoMensalCliente.ValorServicoPrincipal,
                    faturamentoMensalCliente.ValorAdesao,
                    TipoObservacao = faturamentoMensalCliente.TipoObservacaoFaturamentoMensal,
                    faturamentoMensalCliente.Observacao,
                    DataContrato = faturamentoMensalCliente.DataContrato != null && faturamentoMensalCliente.DataContrato.HasValue ? faturamentoMensalCliente.DataContrato.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLancamento = faturamentoMensalCliente.DataLancamento != null && faturamentoMensalCliente.DataLancamento.HasValue ? faturamentoMensalCliente.DataLancamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLancamentoAte = faturamentoMensalCliente.DataLancamentoAte != null && faturamentoMensalCliente.DataLancamentoAte.HasValue ? faturamentoMensalCliente.DataLancamentoAte.Value.ToString("dd/MM/yyyy") : string.Empty,
                    faturamentoMensalCliente.NumeroPedidoCompra,
                    faturamentoMensalCliente.NumeroPedidoItemCompra,
                    GrupoFaturamento = new
                    {
                        Codigo = faturamentoMensalCliente.FaturamentoMensalGrupo != null ? faturamentoMensalCliente.FaturamentoMensalGrupo.Codigo : 0,
                        Descricao = faturamentoMensalCliente.FaturamentoMensalGrupo != null ? faturamentoMensalCliente.FaturamentoMensalGrupo.Descricao : ""
                    },
                    Pessoa = new
                    {
                        Codigo = faturamentoMensalCliente.Pessoa != null ? faturamentoMensalCliente.Pessoa.Codigo : 0,
                        Descricao = faturamentoMensalCliente.Pessoa != null ? faturamentoMensalCliente.Pessoa.Nome : ""
                    },
                    ServicoPrincipal = new
                    {
                        Codigo = faturamentoMensalCliente.Servico != null ? faturamentoMensalCliente.Servico.Codigo : 0,
                        Descricao = faturamentoMensalCliente.Servico != null ? faturamentoMensalCliente.Servico.Descricao : ""
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = faturamentoMensalCliente.NaturezaDaOperacao != null ? faturamentoMensalCliente.NaturezaDaOperacao.Codigo : 0,
                        Descricao = faturamentoMensalCliente.NaturezaDaOperacao != null ? faturamentoMensalCliente.NaturezaDaOperacao.Descricao : ""
                    },
                    TipoMovimento = new
                    {
                        Codigo = faturamentoMensalCliente.TipoMovimento != null ? faturamentoMensalCliente.TipoMovimento.Codigo : 0,
                        Descricao = faturamentoMensalCliente.TipoMovimento != null ? faturamentoMensalCliente.TipoMovimento.Descricao : ""
                    },
                    BoletoConfiguracao = new
                    {
                        Codigo = faturamentoMensalCliente.BoletoConfiguracao != null ? faturamentoMensalCliente.BoletoConfiguracao.Codigo : 0,
                        Descricao = faturamentoMensalCliente.BoletoConfiguracao != null ? faturamentoMensalCliente.BoletoConfiguracao.DescricaoBanco : ""
                    },
                    ServicosExtras = faturamentoMensalCliente.ServicosExtras != null && faturamentoMensalCliente.ServicosExtras.Count > 0 ? (from obj in faturamentoMensalCliente.ServicosExtras
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 obj.Codigo,
                                                                                                                                                 CodigoServico = obj.Servico.Codigo,
                                                                                                                                                 Descricao = obj.Servico.Descricao,
                                                                                                                                                 Quantidade = obj.Quantidade.ToString("n2"),
                                                                                                                                                 ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                                                                                                                                 ValorTotalServicoExtra = obj.ValorTotal.ToString("n2"),
                                                                                                                                                 DataLancamentoServico = obj.DataLancamento != null && obj.DataLancamento.HasValue ? obj.DataLancamento.Value.ToString("dd/MM/yyyy") : "",
                                                                                                                                                 DataLancamentoServicoAte = obj.DataLancamentoAte != null && obj.DataLancamentoAte.HasValue ? obj.DataLancamentoAte.Value.ToString("dd/MM/yyyy") : "",
                                                                                                                                                 TipoObservacaoServicoExtra = obj.TipoObservacaoFaturamentoMensal,
                                                                                                                                                 ObservacaoServicoExtra = obj.Observacao,
                                                                                                                                                 NumeroPedidoCompraExtra = obj.NumeroPedidoCompra,
                                                                                                                                                 NumeroPedidoItemCompraExtra = obj.NumeroPedidoItemCompra,
                                                                                                                                                 Historico = obj.Historico
                                                                                                                                             }).ToList() : null
                };
                return new JsonpResult(dynFaturamentoCliente);
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

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamentoMensal = repFaturamentoMensal.BuscarPorCodigo(codigo);

                repFaturamentoMensal.Deletar(faturamentoMensal, Auditado);

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

        #endregion
    }
}

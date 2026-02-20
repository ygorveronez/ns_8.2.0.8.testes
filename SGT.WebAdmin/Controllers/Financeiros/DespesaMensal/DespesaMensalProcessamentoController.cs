using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.DespesaMensal
{
    [CustomAuthorize("Financeiros/DespesaMensalProcessamento")]
    public class DespesaMensalProcessamentoController : BaseController
    {
		#region Construtores

		public DespesaMensalProcessamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("TipoDespesa"), out int codigoTipoDespesa);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes? mes = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes>("Mes");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Despesa", "TipoDespesaFinanceira", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mês", "Mes", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Geração", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Pagamento", "DataPagamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Quantidade Despesas", "QuantidadeDespesas", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Total Pago", "ValorTotalPagar", 10, Models.Grid.Align.right, false);

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento repDespesaMensalProcessamento = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento> despesaMensaisProcessamento = repDespesaMensalProcessamento.Consultar(codigoEmpresa, codigoTipoDespesa, mes, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repDespesaMensalProcessamento.ContarConsulta(codigoEmpresa, codigoTipoDespesa, mes));

                var lista = (from p in despesaMensaisProcessamento
                             select new
                             {
                                 p.Codigo,
                                 TipoDespesaFinanceira = p.TipoDespesaFinanceira?.Descricao ?? string.Empty,
                                 Mes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MesHelper.ObterDescricao(p.Mes),
                                 Data = p.Data.ToString("dd/MM/yyyy"),
                                 DataPagamento = p.DataPagamento.HasValue ? p.DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.QuantidadeDespesas,
                                 ValorTotalPagar = p.ValorTotalPagar.ToString("n2")
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

        public async Task<IActionResult> PesquisaDespesasMensais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TipoDespesa"), out int codigoTipoDespesa);
                bool.TryParse(Request.Params("FazendoABusca"), out bool fazendoABusca);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes mes = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes>("Mes");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.EditableCell editableValorPago = null;
                editableValorPago = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 15);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia Provisão", "DiaProvisao", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Mês Provisão", "MesProvisao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Ano Provisão", "AnoProvisao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Provisão", "ValorProvisao", 10, Models.Grid.Align.right, true);
                if (codigo > 0)
                {
                    grid.AdicionarCabecalho("DataPagamentoAnterior", false);
                    grid.AdicionarCabecalho("ValorPagoAnterior", false);
                    grid.AdicionarCabecalho("Valor Pago", "ValorPago", 10, Models.Grid.Align.right, false);
                }
                else
                {
                    grid.AdicionarCabecalho("Data Pag. Ant.", "DataPagamentoAnterior", 8, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho("Valor Pago Ant.", "ValorPagoAnterior", 7, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Valor a Pagar", "ValorPago", 10, Models.Grid.Align.right, false, false, false, false, true, editableValorPago);
                }

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento repDespesaMensalProcessamento = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento(unitOfWork);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (codigo > 0)
                {
                    if (propOrdenar == "Descricao")
                        propOrdenar = "DespesaMensal.Descricao";
                    else if (propOrdenar == "DiaProvisao")
                        propOrdenar = "DespesaMensal.DiaProvisao";
                    else if (propOrdenar == "ValorProvisao")
                        propOrdenar = "DespesaMensal.ValorProvisao";

                    List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas> despesasMensais = repDespesaMensalProcessamento.ConsultarPorCodigoDespesaMensalProcessamento(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repDespesaMensalProcessamento.ContarConsultarPorCodigoDespesaMensalProcessamento(codigo));

                    var lista = (from p in despesasMensais
                                 select new
                                 {
                                     p.DespesaMensal.Codigo,
                                     p.DespesaMensal.Descricao,
                                     p.DespesaMensal.DiaProvisao,
                                     MesProvisao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MesHelper.ObterDescricao(p.DespesaMensalProcessamento.Mes),
                                     AnoProvisao = p.DespesaMensalProcessamento.Data.Year,
                                     ValorProvisao = p.DespesaMensal.ValorProvisao.ToString("n2"),
                                     DataPagamentoAnterior = string.Empty,
                                     ValorPagoAnterior = 0.ToString("n2"),
                                     ValorPago = p.ValorPago.ToString("n2")
                                 }).ToList();
                    grid.AdicionaRows(lista);
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal> despesasMensais = repDespesaMensalProcessamento.ConsultarDespesasMensais(codigoEmpresa, codigoTipoDespesa, fazendoABusca, mes, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repDespesaMensalProcessamento.ContarConsultaDespesasMensais(codigoEmpresa, codigoTipoDespesa, fazendoABusca, mes));

                    var lista = (from p in despesasMensais
                                 select new
                                 {
                                     p.Codigo,
                                     p.Descricao,
                                     p.DiaProvisao,
                                     MesProvisao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MesHelper.ObterDescricao(mes),
                                     AnoProvisao = DateTime.Now.Year,
                                     ValorProvisao = p.ValorProvisao.ToString("n2"),
                                     DataPagamentoAnterior = repDespesaMensalProcessamento.VerificarPagamentoDespesaPorMesAno(codigoEmpresa, p.Codigo, mes)?.Titulo?.DataLiquidacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                     ValorPagoAnterior = repDespesaMensalProcessamento.VerificarPagamentoDespesaPorMesAno(codigoEmpresa, p.Codigo, mes)?.Titulo?.ValorPago ?? 0,
                                     ValorPago = p.ValorProvisao.ToString("n2")
                                 }).ToList();

                    grid.AdicionaRows(lista);
                }
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as Despesas Mensais");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Processar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento repDespesaMensalProcessamento = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento despesaMensalProcessamento = new Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento();

                PreencherDespesaMensalProcessamento(despesaMensalProcessamento, unitOfWork);
                repDespesaMensalProcessamento.Inserir(despesaMensalProcessamento, Auditado);
                AdicionarDespesasMensais(despesaMensalProcessamento, unitOfWork);
                CriarBaixarTitulosAPagar(despesaMensalProcessamento, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao Processar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento repDespesaMensalProcessamento = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento despesaMensalProcessamento = repDespesaMensalProcessamento.BuscarPorCodigo(codigo, false);

                var dynDespesaMensalProcessamento = new
                {
                    despesaMensalProcessamento.Codigo,
                    despesaMensalProcessamento.Mes,
                    TipoDespesa = despesaMensalProcessamento.TipoDespesaFinanceira != null ? new { despesaMensalProcessamento.TipoDespesaFinanceira.Codigo, despesaMensalProcessamento.TipoDespesaFinanceira.Descricao } : null,
                    despesaMensalProcessamento.QuantidadeDespesas,
                    ValorTotalPagar = despesaMensalProcessamento.ValorTotalPagar.ToString("n2"),
                    DataPagamento = despesaMensalProcessamento.DataPagamento.HasValue ? despesaMensalProcessamento.DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty
                };

                return new JsonpResult(dynDespesaMensalProcessamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDespesaMensalProcessamento(Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento despesaMensalProcessamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("TipoDespesa"), out int codigoTipoDespesaFinanceira);

            DateTime.TryParse(Request.Params("DataPagamento"), out DateTime dataPagamento);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes mes = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes>("Mes");

            despesaMensalProcessamento.Data = DateTime.Now;
            despesaMensalProcessamento.Mes = mes;
            despesaMensalProcessamento.DataPagamento = dataPagamento;
            despesaMensalProcessamento.TipoDespesaFinanceira = codigoTipoDespesaFinanceira > 0 ? repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesaFinanceira) : null;
            despesaMensalProcessamento.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        private void AdicionarDespesasMensais(Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento despesaMensalProcessamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas repDespesaMensalProcessamentoDespesas = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas(unitOfWork);
            Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);

            dynamic despesas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DespesasSelecionadas"));
            despesaMensalProcessamento.Despesas = new List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>();

            foreach (var despesa in despesas)
            {
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas despesaMensalProcessamentoDespesas = new Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas();

                int.TryParse((string)despesa.Codigo, out int codigoDespesa);

                despesaMensalProcessamentoDespesas.ValorPago = Utilidades.Decimal.Converter((string)despesa.ValorPago);
                despesaMensalProcessamentoDespesas.DespesaMensal = repDespesaMensal.BuscarPorCodigo(codigoDespesa);
                despesaMensalProcessamentoDespesas.DespesaMensalProcessamento = despesaMensalProcessamento;

                repDespesaMensalProcessamentoDespesas.Inserir(despesaMensalProcessamentoDespesas, Auditado);
            }
        }

        private void CriarBaixarTitulosAPagar(Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento despesaMensalProcessamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas repDespesaMensalProcessamentoDespesas = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas> despesasProcessamentos = repDespesaMensalProcessamentoDespesas.BuscarPorDespesaMensalProcessamento(despesaMensalProcessamento.Codigo);

            foreach (var despesa in despesasProcessamentos)
            {
                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
                Dominio.Entidades.Cliente pessoa = despesa.DespesaMensal.Pessoa;
                if (pessoa == null)
                {
                    pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                    if (pessoa == null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa clienteEmpresa = serEmpresa.ConverterObjetoEmpresa(empresa);
                        serCliente.ConverterParaTransportadorTerceiro(clienteEmpresa, "Empresa", unitOfWork);
                        pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                    }
                }

                DateTime? dataDespesa = null;
                if (despesaMensalProcessamento.DataPagamento.HasValue)
                    dataDespesa = despesaMensalProcessamento.DataPagamento.Value;
                else
                {
                    try
                    {
                        dataDespesa = new DateTime(DateTime.Now.Year, (int)despesaMensalProcessamento.Mes, despesa.DespesaMensal.DiaProvisao);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            dataDespesa = new DateTime(DateTime.Now.Year, (int)despesaMensalProcessamento.Mes, despesa.DespesaMensal.DiaProvisao - 1);//Dia 30
                        }
                        catch (Exception)
                        {
                            try
                            {
                                dataDespesa = new DateTime(DateTime.Now.Year, (int)despesaMensalProcessamento.Mes, despesa.DespesaMensal.DiaProvisao - 2);//Dia 29
                            }
                            catch (Exception)
                            {
                                dataDespesa = new DateTime(DateTime.Now.Year, (int)despesaMensalProcessamento.Mes, despesa.DespesaMensal.DiaProvisao - 3);//Dia 28
                            }
                        }
                    }
                }

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                titulo.DataEmissao = dataDespesa;
                titulo.DataVencimento = dataDespesa;
                titulo.DataProgramacaoPagamento = dataDespesa;
                titulo.Pessoa = pessoa;
                titulo.GrupoPessoas = pessoa.GrupoPessoas;
                titulo.Sequencia = 1;
                titulo.ValorOriginal = despesa.ValorPago;
                titulo.ValorPendente = 0;
                titulo.ValorPago = titulo.ValorOriginal;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                titulo.DataAlteracao = DateTime.Now;
                titulo.Historico = "GERADO A PARTIR DO PROCESSAMENTO DE DESPESAS MENSAIS - Despesa: " + despesa.DespesaMensal.Descricao;
                titulo.Empresa = empresa;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "Despesa Mensal";
                titulo.NumeroDocumentoTituloOriginal = despesaMensalProcessamento.Codigo.ToString();
                titulo.TipoMovimento = despesa.DespesaMensal.TipoMovimento;
                titulo.DataLiquidacao = dataDespesa;
                titulo.DataBaseLiquidacao = dataDespesa;
                titulo.Usuario = this.Usuario;
                titulo.DataLancamento = DateTime.Now;
                titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                repTitulo.Inserir(titulo, Auditado);

                servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(),
                    "GERAÇÃO DO TÍTULO AUTOMÁTICO VIA PROCESSAMENTO DE DESPESAS MENSAIS", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual,
                    TipoServicoMultisoftware, 0, null, null, titulo.Codigo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                tituloBaixa.DataBaixa = dataDespesa;
                tituloBaixa.DataBase = dataDespesa;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = "Gerado automaticamente pelo Processamento de Despesas Mensais - Despesa: " + despesa.DespesaMensal.Descricao;
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = titulo.ValorOriginal;
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                tituloBaixa.Pessoa = pessoa;
                tituloBaixa.Titulo = titulo;
                tituloBaixa.ModeloAntigo = true;
                tituloBaixa.Usuario = this.Usuario;
                repTituloBaixa.Inserir(tituloBaixa, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento();
                tituloBaixaTipoPagamentoRecebimento.TituloBaixa = tituloBaixa;
                tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento = despesa.DespesaMensal.TipoPagamentoRecebimento;
                tituloBaixaTipoPagamentoRecebimento.Valor = titulo.ValorOriginal;
                repTituloBaixaTipoPagamentoRecebimento.Inserir(tituloBaixaTipoPagamentoRecebimento, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                tituloBaixaAgrupado.Titulo = titulo;
                tituloBaixaAgrupado.TituloBaixa = tituloBaixa;
                tituloBaixaAgrupado.DataBaixa = tituloBaixa.DataOperacao.Value;
                tituloBaixaAgrupado.DataBase = tituloBaixa.DataBase.Value;
                repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado, Auditado);

                servProcessoMovimento.GerarMovimentacao(null, titulo.DataLiquidacao.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(),
                    "BAIXA DO TÍTULO A PAGAR AUTOMÁTICA VIA PROCESSAMENTO DE DESPESAS MENSAIS", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento,
                    TipoServicoMultisoftware, 0, tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento.PlanoConta, titulo.TipoMovimento.PlanoDeContaCredito, 0, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas);

                despesa.Titulo = titulo;
                repDespesaMensalProcessamentoDespesas.Atualizar(despesa);
            }
        }

        #endregion
    }
}

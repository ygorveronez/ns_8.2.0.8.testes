using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/GeracaoMovimentoLote")]
    public class GeracaoMovimentoLoteController : BaseController
    {
		#region Construtores

		public GeracaoMovimentoLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais        

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarMovimentacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool comRelatorio = false;
                bool.TryParse(Request.Params("ComRelatorio"), out comRelatorio);

                unitOfWork.Start();
                if (!SalvarMovimentosSelecionados(unitOfWork, comRelatorio))
                {
                    return new JsonpResult(false, "Não é possível gerar movimento com data superior a data atual.");
                }
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os movimentos dos motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMotoristasAtivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 8);
                Models.Grid.EditableCell editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 500);

                int codigoTipoMovimento = 0, codigoCentroResultado = 0, codigoContaDebito = 0, codigoContaCredito = 0, codigoMotorista = 0;
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("CentroResultado"), out codigoCentroResultado);
                int.TryParse(Request.Params("ContaDebito"), out codigoContaDebito);
                int.TryParse(Request.Params("ContaCredito"), out codigoContaCredito);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);
                DateTime dataMovimento;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);
                string observacao = Request.Params("Observacao");
                string documento = Request.Params("Documento");

                int quantidadeSelecionados = 0, quantidadeNaoSelecionados, quantidadeTotalMotoristas = 0;
                int.TryParse(Request.Params("QuantidadeSelecionados"), out quantidadeSelecionados);
                int.TryParse(Request.Params("QuantidadeNaoSelecionados"), out quantidadeNaoSelecionados);
                int.TryParse(Request.Params("QuantidadeTotal"), out quantidadeTotalMotoristas);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTipoMovimento", false);
                grid.AdicionarCabecalho("CodigoCentroResultado", false);
                grid.AdicionarCabecalho("CodigoContaDebito", false);
                grid.AdicionarCabecalho("CodigoContaCredito", false);
                grid.AdicionarCabecalho("Documento", false);
                grid.AdicionarCabecalho("CPF", "CPF_Formatado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome", "Nome", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false, false, false, false, true, editableValor);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, false, false, false, false, true, editableValorString);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CPF_Formatado")
                    propOrdenar = "CPF";

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa.Codigo;

                if (codigoContaCredito == 0)
                    codigoMotorista = -5;

                Repositorio.Usuario repUsuarios = new Repositorio.Usuario(unitOfWork);

                grid.limite = 1000;
                List<Dominio.Entidades.Usuario> listaMotoristas = repUsuarios.ConsultarMotoristas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Todos, "", "", empresa, codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio, true, grid.inicio, 1000, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                grid.setarQuantidadeTotal(repUsuarios.ContarConsultaMotoristas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Todos, "", "", empresa, codigoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio, true));
                var lista = (from p in listaMotoristas
                             orderby p.Nome
                             select new
                             {
                                 p.Codigo,
                                 CodigoTipoMovimento = codigoTipoMovimento,
                                 CodigoCentroResultado = codigoCentroResultado,
                                 CodigoContaDebito = codigoContaDebito,
                                 CodigoContaCredito = codigoContaCredito,
                                 Documento = documento,
                                 p.CPF_Formatado,
                                 p.Nome,
                                 Valor = valor.ToString("n2"),
                                 Data = dataMovimento.ToString("dd/MM/yyyy"),
                                 Observacao = observacao,
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristasa ativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarValorMotorista()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            try
            {
                unitOfWork.Start();
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigo);
                //motorista.Salario = valor;
                //repUsuario.Atualizar(motorista);
                var retorno = RetornarObjetoDadosGrid(unitOfWork, codigo, valor, motorista);
                unitOfWork.CommitChanges();
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o valor a ser movimentado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        public dynamic RetornarObjetoDadosGrid(Repositorio.UnitOfWork unitOfWork, int codigo, decimal valor, Dominio.Entidades.Usuario motorista)
        {
            int codigoTipoMovimento = 0, codigoCentroResultado = 0, codigoContaDebito = 0, codigoContaCredito = 0, codigoMotorista = 0;
            int.TryParse(Request.Params("CodigoTipoMovimento"), out codigoTipoMovimento);
            int.TryParse(Request.Params("CodigoCentroResultado"), out codigoCentroResultado);
            int.TryParse(Request.Params("CodigoContaDebito"), out codigoContaDebito);
            int.TryParse(Request.Params("CodigoContaCredito"), out codigoContaCredito);
            int.TryParse(Request.Params("Codigo"), out codigoMotorista);
            DateTime dataMovimento;
            DateTime.TryParse(Request.Params("Data"), out dataMovimento);
            string observacao = Request.Params("Observacao");
            string documento = Request.Params("Documento");

            var retorno = new
            {
                Codigo = codigo,
                CodigoTipoMovimento = codigoTipoMovimento,
                CodigoCentroResultado = codigoCentroResultado,
                CodigoContaDebito = codigoContaDebito,
                CodigoContaCredito = codigoContaCredito,
                Documento = documento,
                motorista.CPF_Formatado,
                motorista.Nome,
                Valor = valor.ToString("n2"),
                Data = dataMovimento.ToString("dd/MM/yyyy"),
                Observacao = observacao
            };
            return retorno;
        }

        private bool SalvarMovimentosSelecionados(Repositorio.UnitOfWork unidadeDeTrabalho, bool comRelatorio)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade repMovimentoFinanceiroEntidade = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista>();

            dynamic listaMotoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("MotoristasSelecionados"));
            if (listaMotoristas != null)
            {
                foreach (var mot in listaMotoristas)
                {
                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimento = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro();
                    Dominio.Entidades.Usuario motorista = null;
                    if ((int)mot.CodigoTipoMovimento.val > 0)
                        movimento.TipoMovimento = repTipoMovimento.BuscarPorCodigo((int)mot.CodigoTipoMovimento.val);

                    if ((int)mot.CodigoCentroResultado.val > 0)
                        movimento.CentroResultado = repCentroResultado.BuscarPorCodigo((int)mot.CodigoCentroResultado.val);

                    if ((int)mot.CodigoContaDebito.val > 0)
                        movimento.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaDebito.val);

                    if ((int)mot.CodigoContaCredito.val > 0)
                        movimento.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaCredito.val);

                    if ((int)mot.Codigo.val > 0)
                        motorista = repUsuario.BuscarPorCodigo((int)mot.Codigo.val);

                    DateTime dataGeracaoMovimento = DateTime.Now;
                    DateTime dataMovimento;
                    DateTime.TryParse((string)mot.Data.val, out dataMovimento);
                    DateTime.TryParse(Request.Params("DataLancamento"), out dataGeracaoMovimento);

                    //if (dataMovimento > DateTime.Now.Date)
                    //{
                    //    return false;
                    //}
                    movimento.DataMovimento = dataMovimento;
                    movimento.DataBase = dataMovimento;
                    movimento.DataGeracaoMovimento = dataGeracaoMovimento;
                    movimento.Documento = (string)mot.Documento.val;
                    movimento.Empresa = this.Usuario.Empresa;
                    if (motorista != null)
                    {
                        double.TryParse(motorista.CPF, out double cpfMotorosita);
                        movimento.Pessoa = repCliente.BuscarPorCPFCNPJ(cpfMotorosita);
                    }

                    if (movimento.TipoMovimento != null && !string.IsNullOrWhiteSpace(movimento.TipoMovimento.Observacao))
                        movimento.Observacao = movimento.TipoMovimento.Observacao + " - " + (string)mot.Observacao.val;
                    else
                        movimento.Observacao = (string)mot.Observacao.val;

                    movimento.TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros;
                    movimento.TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual;
                    decimal valor = 0;
                    if (!string.IsNullOrWhiteSpace((string)mot.Valor.val))
                        valor = Utilidades.Decimal.Converter((string)mot.Valor.val);
                    movimento.Valor = valor;
                    movimento.Observacao += " GERADO POR: " + this.Usuario.Nome;

                    repMovimentoFinanceiro.Inserir(movimento, Auditado);

                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroDebito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                    movimentoFinanceiroDebito.DataMovimento = dataMovimento;
                    movimentoFinanceiroDebito.DataGeracaoMovimento = dataGeracaoMovimento;
                    movimentoFinanceiroDebito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito;
                    movimentoFinanceiroDebito.MovimentoFinanceiro = movimento;
                    movimentoFinanceiroDebito.PlanoDeConta = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaDebito.val);
                    movimentoFinanceiroDebito.Valor = valor;

                    repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroDebito, Auditado);

                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroCredito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                    movimentoFinanceiroCredito.DataMovimento = dataMovimento;
                    movimentoFinanceiroCredito.DataGeracaoMovimento = dataGeracaoMovimento;
                    movimentoFinanceiroCredito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito;
                    movimentoFinanceiroCredito.MovimentoFinanceiro = movimento;
                    movimentoFinanceiroCredito.PlanoDeConta = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaCredito.val);
                    movimentoFinanceiroCredito.Valor = valor;

                    repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroCredito, Auditado);

                    if ((int)mot.Codigo.val > 0)
                    {
                        motorista = repUsuario.BuscarPorCodigo((int)mot.Codigo.val);
                        if (Servicos.Embarcador.Transportadores.Motorista.GetHabilitarFichaMotorista(motorista, unidadeDeTrabalho))
                        {
                            Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade movimentoEntidade = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade();
                            movimentoEntidade.Motorista = repUsuario.BuscarPorCodigo((int)mot.Codigo.val);
                            movimentoEntidade.MovimentoFinanceiro = movimento;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoMovimento;
                            Enum.TryParse((string)mot.TipoMovimentoEntidade.val, out tipoMovimento);
                            if ((int)tipoMovimento == 0)
                                tipoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida;

                            movimentoEntidade.TipoMovimentoEntidade = tipoMovimento;

                            repMovimentoFinanceiroEntidade.Inserir(movimentoEntidade);

                            if (Servicos.Embarcador.Transportadores.Motorista.GetHabilitarFichaMotorista(movimentoEntidade.Motorista, unidadeDeTrabalho))
                            {
                                Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista dado = new Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista();
                                dado.Codigo = movimento.Codigo;
                                if (movimentoEntidade.Motorista.Banco != null)
                                    dado.Banco = movimentoEntidade.Motorista.Banco.Descricao + " (" + movimentoEntidade.Motorista.Banco.Numero + ")";
                                else
                                    dado.Banco = "";
                                dado.DigitoAgencia = movimentoEntidade.Motorista.DigitoAgencia;
                                dado.Nome = movimentoEntidade.Motorista.Nome;
                                dado.CodigoIntegracao = movimentoEntidade.Motorista.CodigoIntegracao;
                                dado.CPF = movimentoEntidade.Motorista.CPF_Formatado;
                                dado.NumeroAgencia = movimentoEntidade.Motorista.Agencia;
                                dado.NumeroConta = movimentoEntidade.Motorista.NumeroConta;
                                dado.Valor = movimento.Valor;
                                dado.Observacao = movimento.Observacao;
                                if (movimentoEntidade.Motorista.TipoContaBanco != null)
                                    dado.TipoConta = movimentoEntidade.Motorista.DescricaoTipoContaBanco;
                                else
                                    dado.TipoConta = "";
                                dado.DataMovimentacao = movimentoEntidade.MovimentoFinanceiro.DataMovimento;

                                relatorioMovimentosMotorista.Add(dado);
                            }
                        }
                    }
                }
            }
            if (comRelatorio && relatorioMovimentosMotorista != null && relatorioMovimentosMotorista.Count > 0)
                GerarRelatorio(relatorioMovimentosMotorista);
            return true;
        }

        private void GerarRelatorio(List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R050_Movimento_Motorista, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R050_Movimento_Motorista, TipoServicoMultisoftware, "Relatorio Movimentos ao Motorista", "Financeiros", "MovimentoMotorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                Task.Factory.StartNew(() => GerarRelatorioMovimentoMotorista(relatorioMovimentosMotorista, stringConexao, relatorioControleGeracao));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void GerarRelatorioMovimentoMotorista(List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                ReportRequest.WithType(ReportType.MovimentacaoMotorista)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("RelatorioMovimentosMotorista", relatorioMovimentosMotorista.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport();
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
    }
}

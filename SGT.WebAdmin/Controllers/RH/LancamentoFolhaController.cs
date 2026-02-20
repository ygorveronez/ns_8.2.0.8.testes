using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("Financeiros/LancamentoConta", "Financeiros/TituloFinanceiro", "Pessoas/Pessoa", "Financeiros/PlanoOrcamentario", "RH/FolhaLancamento")]
    public class LancamentoFolhaController : BaseController
    {
		#region Construtores

		public LancamentoFolhaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);

                dynamic dynContas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Conta"));

                DateTime dataEmissao, dataCompetencia;
                DateTime.TryParse((string)dynContas.DataEmissao, out dataEmissao);
                DateTime.TryParse((string)dynContas.DataCompetencia, out dataCompetencia);

                decimal valorBase = 0, valorReferencia = 0, valor = 0;
                decimal.TryParse((string)dynContas.ValorBase, out valorBase);
                decimal.TryParse((string)dynContas.ValorReferencia, out valorReferencia);
                decimal.TryParse((string)dynContas.Valor, out valor);

                int numeroOcorrencia = dynContas.NumeroOcorrencia != null ? (int)dynContas.NumeroOcorrencia : 0;
                bool repetir = (bool)dynContas.Repetir;
                bool dividir = (bool)dynContas.Dividir;
                bool simularParcelas = (bool)dynContas.SimularParcelas;

                int diaVencimento = 0;
                int.TryParse((string)dynContas.DiaVencimento, out diaVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade;
                Enum.TryParse((string)dynContas.TipoRepetir, out periodicidade);

                int funcionario = ((string)dynContas.Funcionario).ToInt();
                int folhaInformacao = ((string)dynContas.FolhaInformacao).ToInt();
                int numeroEvento = ((string)dynContas.NumeroEvento).ToInt();
                int numeroContrato = ((string)dynContas.NumeroContrato).ToInt();

                string descricao = (string)dynContas.Descricao;

                if (dataCompetencia > DateTime.MinValue && dataCompetencia.Date < DateTime.Now.Date)
                    return new JsonpResult(false, "A data de competencia não pode ser retroativa.");

                List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> folhasLancamentos = null;

                if (simularParcelas)
                    folhasLancamentos = RetornaTitulosSimulacao(unitOfWork, numeroOcorrencia, repetir, valor, periodicidade, diaVencimento, dataEmissao, descricao,
                        dataCompetencia, valorBase, valorReferencia, funcionario, folhaInformacao, numeroEvento, numeroContrato);
                else
                    folhasLancamentos = RetornaListaTitulos(unitOfWork, numeroOcorrencia, repetir, valor, periodicidade, diaVencimento, dataEmissao, descricao,
                        dataCompetencia, valorBase, valorReferencia, funcionario, folhaInformacao, numeroEvento, numeroContrato);

                if (folhasLancamentos == null || folhasLancamentos.Count <= 0)
                    return new JsonpResult(false, "Verfique os valores informados para a geração das folhas.");

                foreach (var folhaLancamento in folhasLancamentos)
                {
                    repFolhaLancamento.Inserir(folhaLancamento, Auditado);
                    if (ConfiguracaoEmbarcador.GerarTituloFolhaPagamento)
                    {
                        if (!GerarTituloFolhaLancamento(folhaLancamento, unitOfWork, TipoServicoMultisoftware, out string erro, this.Empresa.TipoAmbiente))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Sucesso");
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
        public async Task<IActionResult> PesquisaTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

                dynamic dynContas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Conta"));

                DateTime dataEmissao, dataCompetencia;
                DateTime.TryParse((string)dynContas.DataEmissao, out dataEmissao);
                DateTime.TryParse((string)dynContas.DataCompetencia, out dataCompetencia);

                decimal valorBase = 0, valorReferencia = 0, valor = 0;
                decimal.TryParse((string)dynContas.ValorBase, out valorBase);
                decimal.TryParse((string)dynContas.ValorReferencia, out valorReferencia);
                decimal.TryParse((string)dynContas.Valor, out valor);

                int numeroOcorrencia = dynContas.NumeroOcorrencia != null ? (int)dynContas.NumeroOcorrencia : 0;
                bool repetir = (bool)dynContas.Repetir;
                bool dividir = (bool)dynContas.Dividir;
                bool simularParcelas = (bool)dynContas.SimularParcelas;

                int diaVencimento = 0;
                int.TryParse((string)dynContas.DiaVencimento, out diaVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade;
                Enum.TryParse((string)dynContas.TipoRepetir, out periodicidade);

                int funcionario = ((string)dynContas.Funcionario).ToInt();
                int folhaInformacao = ((string)dynContas.FolhaInformacao).ToInt();
                int numeroEvento = ((string)dynContas.NumeroEvento).ToInt();
                int numeroContrato = ((string)dynContas.NumeroContrato).ToInt();

                string descricao = (string)dynContas.Descricao;

                List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> folhasLancamentos = RetornaListaTitulos(unitOfWork, numeroOcorrencia, repetir, valor, periodicidade, diaVencimento, dataEmissao, descricao,
                    dataCompetencia, valorBase, valorReferencia, funcionario, folhaInformacao, numeroEvento, numeroContrato);
                int i = 1;
                var dynRetorno = new
                {
                    ListaTitulos = (from p in folhasLancamentos
                                    select new
                                    {
                                        Codigo = Guid.NewGuid().ToString(),
                                        Sequencia = i++,
                                        Descricao = p.Descricao,
                                        NumeroEvento = p.NumeroEvento,
                                        DataEmissao = p.DataInicial.ToString("dd/MM/yyyy"),
                                        DataCompetencia = p.DataCompetencia.HasValue ? p.DataCompetencia.Value.ToString("dd/MM/yyyy") : "",
                                        ValorBase = p.Base.ToString("n2"),
                                        ValorReferencia = p.Referencia.ToString("n2"),
                                        Valor = p.Valor.ToString("n2")
                                    }).ToList()
                };


                return new JsonpResult(dynRetorno);

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

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> RetornaListaTitulos(Repositorio.UnitOfWork unitOfWork, int numeroOcorrencia, bool repetir, decimal valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade, int diaVencimento, DateTime dataEmissao, string descricao,
             DateTime dataCompetencia, decimal valorBase, decimal valorReferencia, int funcionario, int folhaInformacao, int numeroEvento, int numeroContrato)
        {
            Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> listaRetorno = new List<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();

            DateTime dataVencimentoParcela;
            dataVencimentoParcela = dataCompetencia;

            if (numeroOcorrencia == 0)
                numeroOcorrencia = 1;

            if (!repetir)
            {
                valor = valorBase / numeroOcorrencia;
                //valorBase = valorBase / numeroOcorrencia;
                //valorReferencia = valorReferencia / numeroOcorrencia;
            }
            else
                valor = valorBase;

            for (int i = 0; i < numeroOcorrencia; i++)
            {
                if (i > 0)
                {
                    if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                        dataVencimentoParcela = dataVencimentoParcela.AddDays(7);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(1);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Bimestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(2);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Trimestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(3);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(6);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Anual)
                        dataVencimentoParcela = dataVencimentoParcela.AddYears(1);
                }

                if (diaVencimento > 0 && periodicidade != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                {
                    DateTime? novaData = null;
                    try
                    {
                        novaData = new DateTime(dataVencimentoParcela.Year, dataVencimentoParcela.Month, diaVencimento);
                    }
                    catch (Exception)
                    {
                        novaData = dataVencimentoParcela;
                    }
                    if (novaData == null)
                        novaData = dataVencimentoParcela;

                    dataVencimentoParcela = novaData.Value;
                }

                Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento();

                folhaLancamento.Descricao = descricao;
                folhaLancamento.DataInicial = dataVencimentoParcela;
                folhaLancamento.DataFinal = dataVencimentoParcela;
                folhaLancamento.NumeroEvento = numeroEvento;
                folhaLancamento.NumeroContrato = numeroContrato;
                folhaLancamento.Base = valorBase;
                folhaLancamento.Referencia = valorReferencia;
                folhaLancamento.Valor = valor;
                folhaLancamento.FolhaInformacao = folhaInformacao > 0 ? repFolhaInformacao.BuscarPorCodigo(folhaInformacao) : null;
                folhaLancamento.Funcionario = funcionario > 0 ? repFuncionario.BuscarPorCodigo(funcionario) : null;
                folhaLancamento.DataCompetencia = dataVencimentoParcela;

                listaRetorno.Add(folhaLancamento);
            }

            return listaRetorno;
        }

        private List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> RetornaTitulosSimulacao(Repositorio.UnitOfWork unitOfWork, int numeroOcorrencia, bool repetir, decimal valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade, int diaVencimento, DateTime dataEmissao, string descricao,
             DateTime dataCompetencia, decimal valorBase, decimal valorReferencia, int funcionario, int folhaInformacao, int numeroEvento, int numeroContrato)
        {
            Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> listaRetorno = new List<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    int i = 0;
                    foreach (var vTitulo in listaTitulo)
                    {
                        Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento();

                        folhaLancamento.Descricao = (string)vTitulo.Titulo.Descricao;
                        folhaLancamento.DataInicial = DateTime.Parse((string)vTitulo.Titulo.DataEmissao);
                        folhaLancamento.DataFinal = DateTime.Parse((string)vTitulo.Titulo.DataEmissao);
                        folhaLancamento.NumeroEvento = (int)vTitulo.Titulo.NumeroEvento;
                        folhaLancamento.NumeroContrato = numeroContrato;
                        folhaLancamento.Base = decimal.Parse((string)vTitulo.Titulo.ValorBase);
                        folhaLancamento.Referencia = decimal.Parse((string)vTitulo.Titulo.ValorReferencia);
                        folhaLancamento.Valor = decimal.Parse((string)vTitulo.Titulo.Valor);
                        folhaLancamento.FolhaInformacao = folhaInformacao > 0 ? repFolhaInformacao.BuscarPorCodigo(folhaInformacao) : null;
                        folhaLancamento.Funcionario = funcionario > 0 ? repFuncionario.BuscarPorCodigo(funcionario) : null;
                        folhaLancamento.DataCompetencia = DateTime.Parse((string)vTitulo.Titulo.DataCompetencia);

                        listaRetorno.Add(folhaLancamento);
                        i = i + 1;
                    }
                }
            }
            return listaRetorno;
        }

        private bool GerarTituloFolhaLancamento(Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            erro = string.Empty;

            Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            if (folhaLancamento.DataCompetencia.HasValue && folhaLancamento.Titulo == null && folhaLancamento.Valor > 0 && folhaLancamento.FolhaInformacao != null && folhaLancamento.FolhaInformacao.Justificativa != null && folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa != null)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                DateTime? dataVencimento = folhaLancamento.DataCompetencia;

                Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(folhaLancamento.Funcionario.CPF));
                if (pessoa == null)
                {
                    if (folhaLancamento.Funcionario.Localidade == null)
                    {
                        erro = "Funcionário está com o endereço incompleto, favor ajustar antes de prosseguir.";
                        return false;
                    }

                    pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(folhaLancamento.Funcionario, unitOfWork);
                    repCliente.Inserir(pessoa);
                }

                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                titulo.DataEmissao = DateTime.Now;
                titulo.DataVencimento = dataVencimento;
                titulo.DataProgramacaoPagamento = dataVencimento;
                titulo.Pessoa = pessoa;
                titulo.GrupoPessoas = pessoa.GrupoPessoas;
                titulo.Sequencia = 1;
                titulo.ValorOriginal = folhaLancamento.Valor;
                titulo.ValorPendente = folhaLancamento.Valor;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                titulo.DataAlteracao = DateTime.Now;
                titulo.Observacao = string.Concat("Referente à Folha do Funcionário ", folhaLancamento.Funcionario.Descricao).Trim();
                titulo.Empresa = folhaLancamento.Funcionario.Empresa;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "Folha Funcionário";
                titulo.NumeroDocumentoTituloOriginal = folhaLancamento.Codigo.ToString();
                titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                titulo.TipoMovimento = folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa;
                titulo.Usuario = this.Usuario;
                titulo.DataLancamento = DateTime.Now;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = tipoAmbiente;

                repTitulo.Inserir(titulo);

                if (!svcProcessoMovimento.GerarMovimentacao(out erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, folhaLancamento.Codigo.ToString(), titulo.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value))
                    return false;

                folhaLancamento.Titulo = titulo;
                repFolhaLancamento.Atualizar(folhaLancamento);
            }

            return true;
        }

        #endregion
    }
}

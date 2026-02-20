using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoAlteracao")]
    public class BoletoAlteracaoController : BaseController
    {
		#region Construtores

		public BoletoAlteracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal;
                double cnpjPessoa = 0;
                DateTime.TryParse(Request.Params("DataVencimentoInicial"), out dataVencimentoInicial);
                DateTime.TryParse(Request.Params("DataVencimentoFinal"), out dataVencimentoFinal);
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out dataEmissaoFinal);
                double.TryParse(Request.Params("Cliente"), out cnpjPessoa);
                int codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Banco", "BoletoConfiguracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vcto. Inicial", "DataVencimentoInicial", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Vcto. Final", "DataVencimentoFinal", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Emissão Inicial", "DataEmissaoInicial", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Emissão Final", "DataEmissaoFinal", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Etapa", "Etapa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao> listaBoletoAlteracao = repBoletoAlteracao.Consulta(cnpjPessoa, codigoEmpresa, dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal, codigoBoletoConfiguracao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBoletoAlteracao.ContarConsulta(cnpjPessoa, codigoEmpresa, dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal, codigoBoletoConfiguracao));

                var lista = (from p in listaBoletoAlteracao
                             select new
                             {
                                 p.Codigo,
                                 Pessoa = p.Pessoa != null ? p.Pessoa.Nome : string.Empty,
                                 BoletoConfiguracao = p.BoletoConfiguracao != null ? p.BoletoConfiguracao.DescricaoBanco : string.Empty,
                                 DataVencimentoInicial = p.DataVencimentoInicial != null && p.DataVencimentoInicial.HasValue ? p.DataVencimentoInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimentoFinal = p.DataVencimentoFinal != null && p.DataVencimentoFinal.HasValue ? p.DataVencimentoFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataEmissaoInicial = p.DataEmissaoInicial != null && p.DataEmissaoInicial.HasValue ? p.DataEmissaoInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataEmissaoFinal = p.DataEmissaoFinal != null && p.DataEmissaoFinal.HasValue ? p.DataEmissaoFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Etapa = p.DescricaoBoletoAlteracaoEtapa,
                                 Status = p.DescricaoBoletoAlteracaoStatus
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaBoletosSelecao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataVencimentoInicial"), out DateTime dataVencimentoInicial);
                DateTime.TryParse(Request.Params("DataVencimentoFinal"), out DateTime dataVencimentoFinal);
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out DateTime dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out DateTime dataEmissaoFinal);

                double.TryParse(Request.Params("Cliente"), out double cnpjPessoa);

                int.TryParse(Request.Params("BoletoConfiguracao"), out int codigoBoletoConfiguracao);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = Empresa.TipoAmbiente;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Título", "CodigoTitulo", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Banco", "BoletoConfiguracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nosso Número", "NossoNumero", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumento", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remessa", "NumeroRemessa", 6, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 25, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CodigoTitulo")
                    propOrdenar = "Codigo";
                else if (propOrdenar == "NumeroRemessa")
                    propOrdenar = "BoletoRemessa.NumeroSequencial";

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.ConsultaAlteracaoBoleto(codigo, cnpjPessoa, codigoEmpresa, dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal, codigoBoletoConfiguracao, tipoAmbiente, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultaAlteracaoBoleto(codigo, cnpjPessoa, codigoEmpresa, dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal, codigoBoletoConfiguracao, tipoAmbiente));

                var lista = (from p in listaTitulo
                             select new
                             {
                                 p.Codigo,
                                 Pessoa = p.Pessoa?.Nome ?? string.Empty,
                                 CodigoTitulo = p.Codigo.ToString("n0"),
                                 BoletoConfiguracao = p.BoletoConfiguracao?.DescricaoBanco ?? string.Empty,
                                 DataEmissao = p.DataEmissao != null && p.DataEmissao.HasValue ? p.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimento = p.DataVencimento != null && p.DataVencimento.HasValue ? p.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 ValorOriginal = p.ValorOriginal.ToString("n2"),
                                 p.NossoNumero,
                                 NumeroDocumento = p.NumeroDocumentoTituloOriginal,
                                 NumeroRemessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0"),
                                 p.Observacao
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

        public async Task<IActionResult> IniciarAlteracaoBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                DateTime dataVencimentoInicial, dataVencimentoFinal, dataEmissaoInicial, dataEmissaoFinal;
                double cnpjPessoa = 0;
                DateTime.TryParse(Request.Params("DataVencimentoInicial"), out dataVencimentoInicial);
                DateTime.TryParse(Request.Params("DataVencimentoFinal"), out dataVencimentoFinal);
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out dataEmissaoFinal);
                double.TryParse(Request.Params("Cliente"), out cnpjPessoa);
                int codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao boletoAlteracao;
                if (codigo > 0)
                {
                    boletoAlteracao = repBoletoAlteracao.BuscarPorCodigo(codigo, true);
                }
                else
                {
                    boletoAlteracao = new Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao
                    {
                        Data = DateTime.Now,
                        Empresa = this.Usuario.Empresa
                    };
                }

                if (codigoBoletoConfiguracao > 0)
                    boletoAlteracao.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    boletoAlteracao.BoletoConfiguracao = null;

                if (dataEmissaoFinal > DateTime.MinValue)
                    boletoAlteracao.DataEmissaoFinal = dataEmissaoFinal;
                if (dataEmissaoInicial > DateTime.MinValue)
                    boletoAlteracao.DataEmissaoInicial = dataEmissaoInicial;
                if (dataVencimentoFinal > DateTime.MinValue)
                    boletoAlteracao.DataVencimentoFinal = dataVencimentoFinal;
                if (dataVencimentoInicial > DateTime.MinValue)
                    boletoAlteracao.DataVencimentoInicial = dataVencimentoInicial;

                boletoAlteracao.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Alteracao;
                if (cnpjPessoa > 0)
                    boletoAlteracao.Pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                else
                    boletoAlteracao.Pessoa = null;
                boletoAlteracao.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Aberto;

                if (codigo > 0)
                    repBoletoAlteracao.Atualizar(boletoAlteracao, Auditado);
                else
                {
                    repBoletoAlteracao.Inserir(boletoAlteracao, Auditado);
                    SalvarTitulosAlteracao(unitOfWork, boletoAlteracao);
                }

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = boletoAlteracao.Codigo,
                    Etapa = boletoAlteracao.Etapa
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar a alteração dos boletos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaBoletosAlteracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Models.Grid.EditableCell editableValorString = null;
                Models.Grid.EditableCell editableValorDate = null;
                editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 300);
                editableValorDate = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aData);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFaturamentoMensal etapa;
                Enum.TryParse(Request.Params("Etapa"), out etapa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoBoletoAlteracao", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Título", "CodigoTitulo", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Banco", "BoletoConfiguracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Vencimento Original", "DataVencimentoOriginal", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Vencimento Alterado", "DataVencimentoAlterado", 8, Models.Grid.Align.center, false, false, false, false, true, editableValorDate);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nosso Número", "NossoNumero", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumento", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remessa", "NumeroRemessa", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 25, Models.Grid.Align.left, false, false, false, false, true, editableValorString);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;


                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaBoletoAlteracaoTitulo = repBoletoAlteracao.ConsultarPorCodigoBoletoAlteracao(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBoletoAlteracao.ContarConsultarPorCodigoBoletoAlteracao(codigo));

                var lista = (from p in listaBoletoAlteracaoTitulo
                             select new
                             {
                                 Codigo = p.Codigo,
                                 CodigoBoletoAlteracao = p.BoletoAlteracao.Codigo,
                                 Pessoa = p.Titulo.Pessoa.Nome,
                                 CodigoTitulo = p.Titulo.Codigo.ToString("n0"),
                                 BoletoConfiguracao = p.Titulo.BoletoConfiguracao.DescricaoBanco,
                                 DataEmissao = p.Titulo.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimentoOriginal = p.DataVencimentoOriginal.ToString("dd/MM/yyyy"),
                                 DataVencimentoAlterado = p.DataVencimentoAlterado.ToString("dd/MM/yyyy"),
                                 ValorOriginal = p.Titulo.ValorOriginal.ToString("n2"),
                                 NossoNumero = p.Titulo.NossoNumero,
                                 NumeroDocumento = p.Titulo.NumeroDocumentoTituloOriginal,
                                 NumeroRemessa = p.Titulo.BoletoRemessa.NumeroSequencial.ToString("n0"),
                                 Observacao = p.ObservacaoAlterada
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

        public async Task<IActionResult> AlterarDataVencimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                DateTime novaDataVencimento;
                DateTime.TryParse(Request.Params("NovaDataVencimento"), out novaDataVencimento);
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");
                if (novaDataVencimento == DateTime.MinValue)
                    return new JsonpResult(false, "Favor selecione uma data para realizar a alteração.");

                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    listaTitulo[i].DataVencimentoAlterado = novaDataVencimento;
                    listaTitulo[i].Titulo.DataVencimento = novaDataVencimento;
                    listaTitulo[i].Titulo.DataProgramacaoPagamento = novaDataVencimento;

                    repBoletoAlteracaoTitulo.Atualizar(listaTitulo[i]);
                    repTitulo.Atualizar(listaTitulo[i].Titulo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, listaTitulo[i].Titulo, null, "Alterou a Data do Vencimento para " + novaDataVencimento.ToString("dd/MM/yyyy") + ".", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, null);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a data do faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> AtualizarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa etapa;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao boletoAlteracao = repBoletoAlteracao.BuscarPorCodigo(codigo);

                if (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Impresao && boletoAlteracao.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoEtapa.Alteracao)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaTitulos = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                    for (int i = 0; i < listaTitulos.Count; i++)
                    {
                        listaTitulos[i].BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;

                        listaTitulos[i].Titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                        listaTitulos[i].Titulo.CaminhoBoleto = string.Empty;

                        repBoletoAlteracaoTitulo.Atualizar(listaTitulos[i]);
                        repTitulo.Atualizar(listaTitulos[i].Titulo);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, listaTitulos[i].Titulo, null, "Alterou a Etapa.", unitOfWork);

                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                        servTitulo.IntegrarEmitido(listaTitulos[i].Titulo, unitOfWork);
                    }
                }

                boletoAlteracao.Etapa = etapa;
                repBoletoAlteracao.Atualizar(boletoAlteracao);

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = boletoAlteracao.Codigo,
                    Etapa = boletoAlteracao.Etapa
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a etapa do faturamento mensal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> SalvarAlteracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                DateTime dataVencimentoAlterado;
                DateTime.TryParse(Request.Params("DataVencimentoAlterado"), out dataVencimentoAlterado);
                string observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo boletoAlteracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorCodigo(codigo);

                if (boletoAlteracaoTitulo.Titulo != null && dataVencimentoAlterado > DateTime.MinValue)
                {
                    boletoAlteracaoTitulo.DataVencimentoAlterado = dataVencimentoAlterado;
                    boletoAlteracaoTitulo.ObservacaoAlterada = observacao;

                    boletoAlteracaoTitulo.Titulo.Initialize();
                    boletoAlteracaoTitulo.Titulo.DataVencimento = dataVencimentoAlterado;
                    boletoAlteracaoTitulo.Titulo.Observacao = observacao;

                    repBoletoAlteracaoTitulo.Atualizar(boletoAlteracaoTitulo);
                    repTitulo.Atualizar(boletoAlteracaoTitulo.Titulo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoAlteracaoTitulo.Titulo, boletoAlteracaoTitulo.Titulo.GetChanges(), "Atualizou", unitOfWork);
                }

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os boletos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaBoletoAlteracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);

                var lista = (from p in listaBoletoAlteracaoTitulo
                             select new
                             {
                                 Codigo = p.Codigo,
                                 CodigoRemessa = p.BoletoRemessa != null ? p.BoletoRemessa.Codigo : 0,
                                 BoletoStatusTitulo = p.Titulo.BoletoStatusTitulo,
                                 Pessoa = p.Titulo.Pessoa.Nome,
                                 CodigoTitulo = p.Titulo.Codigo.ToString("n0"),
                                 DataEmissao = p.Titulo.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimentoOriginal = p.DataVencimentoOriginal.ToString("dd/MM/yyyy"),
                                 DataVencimentoAlterado = p.DataVencimentoAlterado.ToString("dd/MM/yyyy"),
                                 Valor = p.Titulo.ValorOriginal.ToString("n2"),
                                 NossoNumero = p.Titulo.NossoNumero,
                                 NumeroDocumento = p.Titulo.NumeroDocumentoTituloOriginal,
                                 Observacao = p.ObservacaoAlterada,
                                 NumeroRemessa = p.BoletoRemessa != null ? p.BoletoRemessa.NumeroSequencial.ToString("n0") : string.Empty
                             }).ToList();

                return new JsonpResult(lista);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao boletoAlteracao = repBoletoAlteracao.BuscarPorCodigo(codigo);

                var dynRetorno = new
                {
                    Codigo = boletoAlteracao.Codigo,
                    Etapa = boletoAlteracao.Etapa,
                    BoletoConfiguracao = new { Codigo = boletoAlteracao.BoletoConfiguracao != null ? boletoAlteracao.BoletoConfiguracao.Codigo : 0, Descricao = boletoAlteracao.BoletoConfiguracao != null ? boletoAlteracao.BoletoConfiguracao.DescricaoBanco : "" },
                    Cliente = new { Codigo = boletoAlteracao.Pessoa != null ? boletoAlteracao.Pessoa.CPF_CNPJ : 0, Descricao = boletoAlteracao.Pessoa != null ? boletoAlteracao.Pessoa.Nome : "" },
                    DataEmissaoFinal = boletoAlteracao.DataEmissaoFinal != null && boletoAlteracao.DataEmissaoFinal > DateTime.MinValue ? boletoAlteracao.DataEmissaoFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataEmissaoInicial = boletoAlteracao.DataEmissaoInicial != null && boletoAlteracao.DataEmissaoInicial > DateTime.MinValue ? boletoAlteracao.DataEmissaoInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimentoFinal = boletoAlteracao.DataVencimentoFinal != null && boletoAlteracao.DataVencimentoFinal > DateTime.MinValue ? boletoAlteracao.DataVencimentoFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimentoInicial = boletoAlteracao.DataVencimentoInicial != null && boletoAlteracao.DataVencimentoInicial > DateTime.MinValue ? boletoAlteracao.DataVencimentoInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                    boletoAlteracao.Status
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a alteração de boleto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> GerarRemessaAlteracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);

                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                List<int> listaBancos = new List<int>();
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    if (listaTitulo[i].Titulo.BoletoConfiguracao != null && listaTitulo[i].BoletoRemessa == null)
                    {
                        if (!listaBancos.Contains(listaTitulo[i].Titulo.BoletoConfiguracao.Codigo))
                            listaBancos.Add(listaTitulo[i].Titulo.BoletoConfiguracao.Codigo);
                    }
                }

                if (listaBancos.Count() == 0)
                    return new JsonpResult(false, true, "Não foi encontrada nenhuma configuração de banco, ou a remessa já foi gerada.");

                for (int i = 0; i < listaBancos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa = new Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa();
                    remessa.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(listaBancos[i]);
                    remessa.DataGeracao = DateTime.Now;
                    remessa.Empresa = this.Empresa;
                    remessa.NumeroSequencial = repBoletoRemessa.BuscarProximaNumereracao(listaBancos[i]);
                    remessa.Observacao = "";

                    repBoletoRemessa.Inserir(remessa, Auditado);
                    for (int k = 0; k < listaTitulo.Count; k++)
                    {
                        if (listaTitulo[k].Titulo.BoletoConfiguracao == remessa.BoletoConfiguracao)
                        {
                            listaTitulo[k].BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.AguardandoRemessa;
                            listaTitulo[k].BoletoRemessa = remessa;

                            repBoletoAlteracaoTitulo.Atualizar(listaTitulo[k]);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar a geração da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> EnviarEmailBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                string mensagemDigitada = Request.Params("Mensagem");
                string assunto = "Boleto " + empresa.NomeFantasia;
                string mensagemErro = "Erro ao enviar e-mail";

                List<int> listaCodigos = new List<int>();
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaBoletoAlteracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                for (int i = 0; i < listaBoletoAlteracaoTitulo.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(listaBoletoAlteracaoTitulo[i].Titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(listaBoletoAlteracaoTitulo[i].Titulo.CaminhoBoleto))
                    {
                        if (!listaCodigos.Contains(listaBoletoAlteracaoTitulo[i].Titulo.Codigo))
                            listaCodigos.Add(listaBoletoAlteracaoTitulo[i].Titulo.Codigo);
                    }
                }

                if (listaCodigos.Count() == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                for (int i = 0; i < listaCodigos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);
                    Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo ateracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorTituloEAlteracao(listaCodigos[i], codigo);

                    string mensagemEmail = "Olá,<br/><br/>Seguem em anexo o boleto da empresa " + empresa.NomeFantasia + ".<br/><br/>";
                    if (!string.IsNullOrWhiteSpace(mensagemDigitada))
                    {
                        string msgFormatada = "";
                        msgFormatada = mensagemDigitada.Replace("#NumeroDocumento", titulo.NumeroDocumentoTituloOriginal);
                        msgFormatada = msgFormatada.Replace("#ValorOriginal", titulo.ValorOriginal.ToString("n2"));
                        msgFormatada = msgFormatada.Replace("#ObservacaoBoleto", titulo.Observacao);
                        msgFormatada = msgFormatada.Replace("#DataVencimentoOriginal", ateracaoTitulo != null ? ateracaoTitulo.DataVencimentoOriginal.ToString("dd/MM/yyyy") : "");
                        msgFormatada = msgFormatada.Replace("#DataVencimentoAlterada", titulo.DataVencimento.Value.ToString("dd/MM/yyyy"));
                        mensagemEmail += msgFormatada + "<br/>";
                    }
                    mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                    if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                        mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                    if (!string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                    {
                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(titulo.Pessoa.Email))
                            emails.AddRange(titulo.Pessoa.Email.Split(';').ToList());

                        for (int a = 0; a < titulo.Pessoa.Emails.Count; a++)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = titulo.Pessoa.Emails[a];
                            if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A"
                                && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo)
                                emails.Add(outroEmail.Email);
                        }

                        if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                            emails.AddRange(empresa.Email.Split(';').ToList());

                        if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                            emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                        emails = emails.Distinct().ToList();
                        if (emails.Count > 0)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Enviou Boleto para E-mail", unitOfWork);

                            byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);
                            bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), System.IO.Path.GetFileName(titulo.CaminhoBoleto), "application/pdf") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                            if (!sucesso)
                                return new JsonpResult(false, "Problemas ao enviar o boleto por e-mail: " + mensagemErro);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mail com os boletos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoAlteracao repBoletoAlteracao = new Repositorio.Embarcador.Financeiro.BoletoAlteracao(unitOfWork);

                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                DateTime dataVencimentoAlterado;
                DateTime.TryParse(Request.Params("DataVencimentoAlterado"), out dataVencimentoAlterado);
                string observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao boletoAlteracao = repBoletoAlteracao.BuscarPorCodigo(codigo);
                boletoAlteracao.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Finalizado;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoAlteracao, null, "Finalizou Alteração", unitOfWork);

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = 0
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os boletos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<int> listaBoletos = new List<int>();
                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaBoletoAlteracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                for (int i = 0; i < listaBoletoAlteracaoTitulo.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(listaBoletoAlteracaoTitulo[i].Titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(listaBoletoAlteracaoTitulo[i].Titulo.CaminhoBoleto))
                    {
                        if (!listaBoletos.Contains(listaBoletoAlteracaoTitulo[i].Titulo.Codigo))
                            listaBoletos.Add(listaBoletoAlteracaoTitulo[i].Titulo.Codigo);
                    }
                }

                if (listaBoletos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum arquivo de boleto encontrada nos títulos.");

                Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                return Arquivo(svcNFe.ObterLoteDeBoleto(listaBoletos, unitOfWork), "application/zip", "LoteBoleto.zip");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRemessas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo <= 0)
                    return new JsonpResult(false, "Favor selecione uma alteração.");

                List<int> listaCodigos = new List<int>();
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> listaBoletoAlteracaoTitulo = repBoletoAlteracaoTitulo.BuscarPorAlteracao(codigo);
                for (int i = 0; i < listaBoletoAlteracaoTitulo.Count(); i++)
                {
                    if (listaBoletoAlteracaoTitulo[i].BoletoRemessa != null)
                    {
                        if (!listaCodigos.Contains(listaBoletoAlteracaoTitulo[i].BoletoRemessa.Codigo))
                            listaCodigos.Add(listaBoletoAlteracaoTitulo[i].BoletoRemessa.Codigo);
                    }
                }

                if (listaCodigos.Count() == 0)
                    return new JsonpResult(false, "Nenhuma remessa encontrada.");

                else if (listaCodigos.Count() == 1)
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(listaCodigos[0]);
                    if (boletoRemessa != null && !string.IsNullOrWhiteSpace(boletoRemessa.Observacao))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(boletoRemessa.Observacao))
                            {
                                boletoRemessa.DownloadRealizado = true;
                                repBoletoRemessa.Atualizar(boletoRemessa);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoRemessa, null, "Realizou o download da Remessa pela Alteração de Boletos.", unitOfWork);
                                unitOfWork.CommitChanges();

                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(boletoRemessa.Observacao), "application/x-pkcs12", System.IO.Path.GetFileName(boletoRemessa.Observacao));
                            }
                            else
                                return new JsonpResult(false, "O arquivo da remessa " + boletoRemessa.Observacao + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do txt do remessa.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Este boleto não possui o txt disponível para download.");
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> remessas = repBoletoRemessa.BuscarPorNotas(listaCodigos);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa in remessas)
                    {
                        boletoRemessa.DownloadRealizado = true;
                        repBoletoRemessa.Atualizar(boletoRemessa);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoRemessa, null, "Realizou o download da Remessa por Lote pela Alteração de Boletos.", unitOfWork);
                        unitOfWork.CommitChanges();
                    }

                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                    return Arquivo(svcNFe.ObterLoteDeRemessa(listaCodigos, unitOfWork), "application/zip", "LoteRemessas.zip");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRemessaAlteracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoRemessa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRemessa);

                if (codigoRemessa > 0)
                {
                    Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                    Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa = repBoletoRemessa.BuscarPorCodigo(codigoRemessa);

                    if (remessa != null && !string.IsNullOrWhiteSpace(remessa.Observacao))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(remessa.Observacao))
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(remessa.Observacao), "application/x-pkcs12", System.IO.Path.GetFileName(remessa.Observacao));
                            else
                                return new JsonpResult(false, "O arquivo da remessa " + remessa.Observacao + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do txt do remessa.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Este boleto não possui o txt disponível para download.");
                }
                return new JsonpResult(false, true, "Título não encontrado");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private void SalvarTitulosAlteracao(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao boletoAlteracao)
        {
            Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo repBoletoAlteracaoTitulo = new Repositorio.Embarcador.Financeiro.BoletoAlteracaoTitulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);

            if (!string.IsNullOrWhiteSpace(Request.Params("ListaBoletos")))
            {
                dynamic listaBoleto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaBoletos"));
                if (listaBoleto != null)
                {
                    foreach (var boleto in listaBoleto)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo boletoAlteracaoTitulo = new Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo();
                        int codigoTitulo = 0;
                        int.TryParse((string)boleto.Codigo, out codigoTitulo);

                        if (codigoTitulo > 0)
                            boletoAlteracaoTitulo.Titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                        else
                            boletoAlteracaoTitulo.Titulo = null;

                        if (boletoAlteracaoTitulo.Titulo != null)
                        {
                            boletoAlteracaoTitulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum;
                            boletoAlteracaoTitulo.DataVencimentoOriginal = boletoAlteracaoTitulo.Titulo.DataVencimento.Value;
                            boletoAlteracaoTitulo.ObservacaoOriginal = boletoAlteracaoTitulo.Titulo.Observacao;
                            boletoAlteracaoTitulo.DataVencimentoAlterado = boletoAlteracaoTitulo.Titulo.DataVencimento.Value;
                            boletoAlteracaoTitulo.ObservacaoAlterada = boletoAlteracaoTitulo.Titulo.Observacao;
                        }
                        boletoAlteracaoTitulo.BoletoAlteracao = boletoAlteracao;

                        repBoletoAlteracaoTitulo.Inserir(boletoAlteracaoTitulo);
                    }
                }
            }
        }

    }

}

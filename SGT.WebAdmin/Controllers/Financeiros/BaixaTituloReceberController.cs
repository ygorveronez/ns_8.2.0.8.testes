using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BaixaTituloReceber")]
    public class BaixaTituloReceberController : BaseController
    {
		#region Construtores

		public BaixaTituloReceberController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoOperador, numeroTitulo, numeroFatura, grupoPessoa, codigoConhecimento;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("NumeroTitulo"), out numeroTitulo);
                int.TryParse(Request.Params("NumeroFatura"), out numeroFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out grupoPessoa);
                int.TryParse(Request.Params("Conhecimento"), out codigoConhecimento);

                double pessoa;
                double.TryParse(Request.Params("Pessoa"), out pessoa);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                DateTime dataBaseInicial, dataBaseFinal;
                DateTime.TryParse(Request.Params("DataBaseInicial"), out dataBaseInicial);
                DateTime.TryParse(Request.Params("DataBaseFinal"), out dataBaseFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa;
                Enum.TryParse(Request.Params("Situacao"), out etapaBaixa);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("Títulos", "CodigosTitulos", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Faturas", "NumeroFaturas", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho("Pessoa", "Pessoa", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Cargas", "NumeroCargas", 30, Models.Grid.Align.left, false, false);
                }
                else
                {
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("Títulos", "CodigosTitulos", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Faturas", "NumeroFaturas", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Cargas", "NumeroCargas", 30, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoBaixaTitulo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Base", "DataBase", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data da Baixa", "DataBaixa", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Original", "ValorOriginal", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoSituacao")
                    propOrdenar = "Situacao";

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> listaTitulo = repTituloBaixa.ConsultaBaixaReceber(dataBaseInicial, dataBaseFinal, codigoConhecimento, codigoEmpresa, numeroTitulo, numeroFatura, dataInicial, dataFinal, etapaBaixa, grupoPessoa, pessoa, codigoOperador, TipoServicoMultisoftware, tipoAmbiente, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixa.ContaConsultaBaixaReceber(dataBaseInicial, dataBaseFinal, codigoConhecimento, codigoEmpresa, numeroTitulo, numeroFatura, dataInicial, dataFinal, etapaBaixa, grupoPessoa, pessoa, codigoOperador, TipoServicoMultisoftware, tipoAmbiente));

                var lista = (from p in listaTitulo
                             select new
                             {
                                 p.Codigo,
                                 CodigosTitulos = p.CodigosTitulos,
                                 NumeroFaturas = p.NumeroFaturas,
                                 Pessoa = p.Pessoa != null ? p.Pessoa.Nome : string.Empty,
                                 GrupoPessoa = p.GrupoPessoas != null ? p.GrupoPessoas.Descricao : p.Pessoa != null && p.Pessoa.GrupoPessoas != null ? p.Pessoa.GrupoPessoas.Descricao : string.Empty,
                                 NumeroCargas = p.NumeroCargas,
                                 p.DescricaoSituacaoBaixaTitulo,
                                 DataBase = p.DataBase.HasValue && p.DataBase > DateTime.MinValue ? p.DataBase.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataBaixa = p.DataBaixa.Value.ToString("dd/MM/yyyy"),
                                 ValorOriginal = p.ValorOriginal.ToString("n2"),
                                 Valor = p.Valor.ToString("n2")
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
        public async Task<IActionResult> PesquisaTitulosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("CNPJPessoa", false);
                    grid.AdicionarCabecalho("Código", "CodigoTitulo", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Carga (s)", "Cargas", 12, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("Fatura", "Fatura", 8, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("CT-e (s)", "Conhecimentos", 12, Models.Grid.Align.center, false, true);
                    grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 8, Models.Grid.Align.right, true);
                }
                else
                {
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("CNPJPessoa", false);
                    grid.AdicionarCabecalho("Código", "CodigoTitulo", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Carga (s)", "Cargas", 12, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho("Fatura", "Fatura", 8, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho("CT-e (s)", "Conhecimentos", 12, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 8, Models.Grid.Align.right, true, false);
                }

                grid.AdicionarCabecalho("Parcela", "NumeroParcela", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumentoTituloOriginal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Boleto", "NossoNumero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cliente", "Pessoa", 25, Models.Grid.Align.left, false);
                if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    grid.AdicionarCabecalho("Valor Moeda Estrangeira", "ValorOriginalMoedaEstrangeira", 12, Models.Grid.Align.right, false);
                else
                    grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CodigoTitulo")
                    propOrdenar = "Codigo";
                else if (propOrdenar == "NumeroParcela")
                    propOrdenar = "Sequencia";

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTitulosPendentes(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarTitulosPendentes(filtrosPesquisa));

                var lista = (from p in listaTitulos select ObterDetalhesTituloGrid(p)).ToList();
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
        public async Task<IActionResult> PesquisarAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoBaixa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBaixa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoBaixa", false);
                grid.AdicionarCabecalho("Justificativa", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.ConsultarAcrescimoDesconto(codigoBaixa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaAcrescimo.ContarAcrescimoDesconto(codigoBaixa));

                var lista = (from p in listaTituloBaixaAcrescimo
                             select new
                             {
                                 p.Codigo,
                                 CodigoBaixa = p.TituloBaixa.Codigo,
                                 Descricao = p.Justificativa.Descricao,
                                 DescricaoTipo = p.Justificativa.DescricaoTipoJustificativa,
                                 Valor = p.Valor.ToString("n2")
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

        public async Task<IActionResult> InserirAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDescontoAcrescimo))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para inserir acréscimo ou desconto na baixa de título a receber.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoTipoDePagamento = Request.GetIntParam("TipoDePagamento");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                decimal valor = Request.GetDecimalParam("Valor");

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para inserir desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para inserir acréscimo na baixa de título a receber.");
                }

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                tituloBaixa.TipoPagamentoRecebimento = codigoTipoDePagamento > 0 ? repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento) : null;

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo tituloAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo();
                tituloAcrescimoDesconto.TituloBaixa = tituloBaixa;
                tituloAcrescimoDesconto.Justificativa = justificativa;
                tituloAcrescimoDesconto.Valor = valor;

                tituloAcrescimoDesconto.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                tituloAcrescimoDesconto.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                tituloAcrescimoDesconto.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                if (!ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                    tituloAcrescimoDesconto.ValorOriginalMoedaEstrangeira = tituloAcrescimoDesconto.ValorMoedaCotacao > 0 ? Math.Round(valor / tituloAcrescimoDesconto.ValorMoedaCotacao, 2) : 0;

                repTituloBaixaAcrescimo.Inserir(tituloAcrescimoDesconto);

                decimal saldoDevedor = tituloBaixa.ValorPendente;
                decimal descontos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto);
                decimal acrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo);
                decimal valorPendente = tituloBaixa.ValorPendente - tituloBaixa.Valor - descontos + acrescimos;
                if (valorPendente < 0)
                    valorPendente = valorPendente * -1;
                tituloBaixa.ValorPendente = valorPendente;
                //if (valorPendente == valor)
                //{
                //    if (tituloAcrescimoDesconto.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                //        tituloBaixa.Valor += valor;
                //    else
                //        tituloBaixa.Valor -= valor;
                //}
                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloAcrescimoDesconto, null, "Adicionou um " + tituloAcrescimoDesconto.Justificativa.DescricaoTipoJustificativa + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um " + tituloAcrescimoDesconto.Justificativa.DescricaoTipoJustificativa + " ao título " + tituloAcrescimoDesconto.Descricao + ".", unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar acréscimo / desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDescontoAcrescimo))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo ou desconto na baixa de título a receber.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoBaixa = Request.GetIntParam("CodigoBaixa");
                int codigoTipoDePagamento = Request.GetIntParam("TipoDePagamento");

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo tituloAcrescimoDesconto = repTituloBaixaAcrescimo.BuscarPorCodigo(codigo);
                TipoJustificativa tipo = tituloAcrescimoDesconto.Justificativa.TipoJustificativa;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarDesconto) && tipo == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_NaoPermitirLancarAcrescimo) && tipo == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo na baixa de título a receber.");
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                decimal valor = tituloAcrescimoDesconto.Valor;

                repTituloBaixaAcrescimo.Deletar(tituloAcrescimoDesconto);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixa);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                tituloBaixa.TipoPagamentoRecebimento = codigoTipoDePagamento > 0 ? repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento) : null;

                decimal saldoDevedor = tituloBaixa.ValorPendente;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo) : 0;
                decimal valorPendente = tituloBaixa.ValorPendente - tituloBaixa.Valor - descontos + acrescimos;
                if (valorPendente < 0)
                    valorPendente = valorPendente * -1;
                if (valorPendente == valor && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (tipo == TipoJustificativa.Acrescimo)
                        tituloBaixa.Valor -= valor;
                    else
                        tituloBaixa.Valor += valor;
                }
                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloAcrescimoDesconto, null, "Removeu um " + tituloAcrescimoDesconto.Justificativa.DescricaoTipoJustificativa + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Removeu um " + tituloAcrescimoDesconto.Justificativa.DescricaoTipoJustificativa + " ao título " + tituloAcrescimoDesconto.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover acréscimo/desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                var dynRetorno = servBaixaTituloReceber.RetornaObjetoCompletoTitulo(codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
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

        public async Task<IActionResult> BaixarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_PermiteBaixarTitulo)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para iniciar a baixa do título.");

                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string observacao = Request.Params("Observacao");

                decimal valorBaixado;
                decimal.TryParse(Request.Params("ValorBaixado"), out valorBaixado);

                DateTime dataBaixa;
                DateTime.TryParse(Request.Params("DataBaixa"), out dataBaixa);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixaTitulo;
                Enum.TryParse(Request.Params("Etapa"), out etapaBaixaTitulo);

                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulos(unitOfWork);
                if (listaCodigos.Count() == 0 && codigo == 0)
                    return new JsonpResult(false, "Nenhum título selecionado.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
                if (codigo > 0)
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);
                else
                    tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();

                if (dataBaixa > DateTime.Now)
                    return new JsonpResult(false, "Não é possível realizar uma baixa com data maior que a atual.");

                if (dataBase > DateTime.Now)
                    return new JsonpResult(false, "Não é possível realizar uma baixa com data da base maior que a atual.");
                
                unitOfWork.Start();

                tituloBaixa.DataBaixa = dataBaixa;
                tituloBaixa.DataBase = dataBase;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = observacao;
                tituloBaixa.SituacaoBaixaTitulo = etapaBaixaTitulo;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = valorBaixado;
                tituloBaixa.ModeloAntigo = true;
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                tituloBaixa.Usuario = this.Usuario;

                tituloBaixa.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                tituloBaixa.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                tituloBaixa.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                tituloBaixa.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                double codigoPessoa = 0;
                for (int i = 0; i < listaCodigos.Count(); i++)
                {
                    if (repTitulo.BuscarPorCodigo(listaCodigos[i])?.Pessoa != null)
                    {
                        if (codigoPessoa == 0)
                            codigoPessoa = repTitulo.BuscarPorCodigo(listaCodigos[i]).Pessoa.CPF_CNPJ;
                        else
                        {
                            if (codigoPessoa != repTitulo.BuscarPorCodigo(listaCodigos[i]).Pessoa.CPF_CNPJ)
                            {
                                codigoPessoa = 0;
                                break;
                            }
                        }
                    }
                }
                if (codigoPessoa > 0)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);
                else
                    tituloBaixa.Pessoa = null;

                int codigoGrupoPessoa = 0;
                for (int i = 0; i < listaCodigos.Count(); i++)
                {
                    if (repTitulo.BuscarPorCodigo(listaCodigos[i])?.GrupoPessoas != null)
                    {
                        if (codigoGrupoPessoa == 0)
                            codigoGrupoPessoa = repTitulo.BuscarPorCodigo(listaCodigos[i]).GrupoPessoas.Codigo;
                        else
                        {
                            if (codigoGrupoPessoa != repTitulo.BuscarPorCodigo(listaCodigos[i]).GrupoPessoas.Codigo)
                            {
                                codigoGrupoPessoa = 0;
                                break;
                            }
                        }
                    }
                }
                if (codigoGrupoPessoa > 0)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);
                else
                    tituloBaixa.GrupoPessoas = null;

                if (codigo > 0)
                {
                    repTituloBaixa.Atualizar(tituloBaixa);
                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                    for (int i = 0; i < listaParcelas.Count; i++)
                        repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < listaTituloBaixaAcrescimo.Count; i++)
                        repTituloBaixaAcrescimo.Deletar(listaTituloBaixaAcrescimo[i]);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto> listaTituloBaixaDesconto = repTituloBaixaDesconto.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < listaTituloBaixaDesconto.Count; i++)
                        repTituloBaixaDesconto.Deletar(listaTituloBaixaDesconto[i]);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> listaTituloBaixaConhecimentoRemovido = repTituloBaixaConhecimentoRemovido.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < listaTituloBaixaConhecimentoRemovido.Count; i++)
                        repTituloBaixaConhecimentoRemovido.Deletar(listaTituloBaixaConhecimentoRemovido[i]);
                }
                else
                {
                    repTituloBaixa.Inserir(tituloBaixa, Auditado);
                }

                if (codigo == 0)
                {
                    for (int i = 0; i < listaCodigos.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                        tituloAgrupado.TituloBaixa = tituloBaixa;
                        tituloAgrupado.Titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);
                        tituloAgrupado.DataBaixa = dataBaixa;
                        tituloAgrupado.DataBase = dataBase;

                        repTituloBaixaAgrupado.Inserir(tituloAgrupado);
                    }
                }

                unitOfWork.CommitChanges();

                var dynRetorno = servBaixaTituloReceber.RetornaObjetoCompletoTitulo(tituloBaixa.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
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

        public async Task<IActionResult> CarregarNegociacaoBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int codigoTipoDePagamento = 0;
                int.TryParse(Request.Params("TipoDePagamento"), out codigoTipoDePagamento);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamento = null;
                if (codigoTipoDePagamento > 0)
                {
                    tipoPagamento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento);
                }

                DateTime dataEmissao = DateTime.MinValue;
                DateTime dataEmissaoTitulo = DateTime.MinValue;
                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulos(unitOfWork);
                if (listaCodigos.Count() == 0 && codigo == 0)
                    return new JsonpResult(false, "Nenhum título selecionado.");

                if (listaCodigos.Count() > 0)
                {
                    for (int i = 0; i < listaCodigos.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                        dataEmissaoTitulo = repTitulo.BuscarPorCodigo(listaCodigos[i]).DataEmissao.Value;
                        if (dataEmissaoTitulo > dataEmissao)
                            dataEmissao = dataEmissaoTitulo;
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < titulosAgrupados.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                        dataEmissaoTitulo = titulosAgrupados[i].Titulo.DataEmissao.Value;
                        if (dataEmissaoTitulo > dataEmissao)
                            dataEmissao = dataEmissaoTitulo;
                    }
                }
                if (dataEmissao == DateTime.MinValue)
                    dataEmissao = DateTime.Now;

                decimal valorABaixar = 0;
                if (tituloBaixa != null)
                    valorABaixar = tituloBaixa.Valor;

                decimal valorCTeRemovidos = tituloBaixa.ConhecimentosRemovidos != null ? (from p in tituloBaixa.ConhecimentosRemovidos select p.CTe.ValorAReceber).Sum() : 0;
                decimal valorTotalCTe = tituloBaixa.ValorConhecimentosFaturamento;
                decimal saldoDevedor = tituloBaixa.ValorOriginal - tituloBaixa.Valor;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo) : 0;

                descontos = descontos + (tituloBaixa != null ? repTituloBaixaDetalheConhecimento.TotalDescontoPorTituloBaixa(codigo) : 0);
                acrescimos = acrescimos + (tituloBaixa != null ? repTituloBaixaDetalheConhecimento.TotalAcrescimoPorTituloBaixa(codigo) : 0);

                decimal valorPendente = tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos;// + valorCTeRemovidos;
                if (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada)
                {
                    valorPendente = 0;
                    valorABaixar = 0;
                    saldoDevedor = 0;
                }

                if (tituloBaixa != null && tituloBaixa.TipoPagamentoRecebimento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.TipoPagamentoRecebimento;
                else if (tituloBaixa != null && tituloBaixa.GrupoPessoas != null && tituloBaixa.GrupoPessoas.FormaPagamento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.GrupoPessoas.FormaPagamento;
                else if (tituloBaixa != null && tituloBaixa.Pessoa != null && tituloBaixa.Pessoa.FormaPagamento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.Pessoa.FormaPagamento;

                string codigos = tituloBaixa.CodigosTitulos;
                if (!string.IsNullOrWhiteSpace(codigos) && codigos.Length > 30)
                    codigos = codigos.Substring(0, 29) + "...";

                decimal saldoContaAdiantamento = 0m;
                if (ConfiguracaoEmbarcador.PlanoContaAdiantamentoCliente != null && tituloBaixa != null && tituloBaixa.Pessoa != null && (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao || tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada))
                    saldoContaAdiantamento = repMovimento.BuscarSaldoContaCliente(ConfiguracaoEmbarcador.PlanoContaAdiantamentoCliente.Codigo, tituloBaixa.Pessoa.CPF_CNPJ);

                var dynRetorno = new
                {
                    CodigoFatura = repTituloBaixa.CodigoFaturaBaixaAReceber(codigo),
                    NumeroFatura = repTituloBaixa.NumeroFaturaBaixaAReceber(codigo).ToString("n0"),
                    Codigo = tituloBaixa != null ? tituloBaixa.Codigo : 0,
                    NumeroTitulo = codigos,
                    ValorOriginal = tituloBaixa.ValorOriginal.ToString("n2"),
                    ValorABaixar = valorABaixar.ToString("n2"),
                    DataBaixar = tituloBaixa != null ? tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataBase = tituloBaixa != null ? tituloBaixa.DataBase.Value.ToString("dd/MM/yyyy") : string.Empty,
                    tituloBaixa.MoedaCotacaoBancoCentral,
                    DataBaseCRT = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    SaldoDevedor = saldoDevedor.ToString("n2"),
                    Operador = tituloBaixa.Usuario?.Nome ?? this.Usuario.Nome,
                    Descontos = descontos.ToString("n2"),
                    Acrescimos = acrescimos.ToString("n2"),
                    ValorPendente = valorPendente.ToString("n2"),
                    ValorTotalCTesRemovidos = valorCTeRemovidos.ToString("n2"),
                    ValorTotalCTesNaoRemovidos = (valorTotalCTe - valorCTeRemovidos).ToString("n2"),
                    CodigoPessoa = tituloBaixa.Pessoa != null ? tituloBaixa.Pessoa.CPF_CNPJ : 0,
                    Parcelas = tituloBaixa != null && tituloBaixa.TitulosNegociacao != null ? (from obj in tituloBaixa.TitulosNegociacao
                                                                                               orderby obj.Sequencia
                                                                                               select new
                                                                                               {
                                                                                                   obj.Codigo,
                                                                                                   Acrescimo = obj.Acrescimo.ToString("n2"),
                                                                                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                                                                                   DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                                                                                   Desconto = obj.Desconto.ToString("n2"),
                                                                                                   obj.DescricaoSituacao,
                                                                                                   obj.Sequencia,
                                                                                                   obj.SituacaoFaturaParcela,
                                                                                                   Valor = obj.Valor.ToString("n2")
                                                                                               }).ToList() : null,
                    TipoDePagamento = tituloBaixa.TipoPagamentoRecebimento == null ? new { Codigo = tipoPagamento != null ? tipoPagamento.Codigo : 0, Descricao = tipoPagamento != null ? tipoPagamento.Descricao : "" } : new { Codigo = tituloBaixa.TipoPagamentoRecebimento != null ? tituloBaixa.TipoPagamentoRecebimento.Codigo : 0, Descricao = tituloBaixa.TipoPagamentoRecebimento != null ? tituloBaixa.TipoPagamentoRecebimento.Descricao : "" },
                    DataEmissao = dataEmissao.ToString("dd/MM/yyyy"),
                    SaldoContaAdiantamento = saldoContaAdiantamento.ToString("n2"),
                    Situacao = tituloBaixa != null ? tituloBaixa.SituacaoBaixaTitulo : SituacaoBaixaTitulo.Iniciada
                };

                return new JsonpResult(dynRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados para a negociação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaParcelasNegociacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTituloBaixa", false);
                grid.AdicionarCabecalho("Cód. Título", "CodigoTitulo", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Parcela", "Parcela", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Desconto", "Desconto", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Parcela")
                    propOrdenar = "Sequencia";
                if (propOrdenar == "CodigoTitulo")
                    propOrdenar = "Sequencia";

                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaTituloBaixaNegociacao = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaNegociacao.ContarBuscarPorTituloBaixa(codigo));
                var dynRetorno = (from obj in listaTituloBaixaNegociacao
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoTituloBaixa = obj.TituloBaixa.Codigo,
                                      CodigoTitulo = obj.CodigoTitulo.ToString("n0"),
                                      Parcela = obj.Sequencia.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      Desconto = obj.Desconto.ToString("n2"),
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      obj.DescricaoSituacao
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as parcelas da negociação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConhecimentosRemovidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_PermiteSalvarValores)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para salvar os valores de negociação.");

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoTipoDePagamento = 0;
                int.TryParse(Request.Params("TipoDePagamento"), out codigoTipoDePagamento);

                if (codigo == 0)
                    return new JsonpResult(false, "Por favor inicie uma baixa antes de remover os conhecimentos.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                SalvarListaCTesRemovidos(tituloBaixa, unitOfWork);
                if (codigoTipoDePagamento > 0)
                {
                    tituloBaixa.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento);
                }
                else
                {
                    tituloBaixa.TipoPagamentoRecebimento = null;
                }

                decimal valorCTeRemovidos = tituloBaixa.ConhecimentosRemovidos != null ? (from p in tituloBaixa.ConhecimentosRemovidos select p.CTe.ValorAReceber).Sum() : 0;
                tituloBaixa.ValorPendente = tituloBaixa.ValorOriginal - tituloBaixa.Valor;

                repTituloBaixa.Atualizar(tituloBaixa);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Salvou Conhecimentos Removidos.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os conhecimentos removidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_PermiteSalvarValores)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para salvar os valores de negociação.");

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);

                int codigo, justificativaAcrescimo, justificativaDesconto = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("JustificativaDesconto"), out justificativaDesconto);
                int.TryParse(Request.Params("JustificativaAcrescimo"), out justificativaAcrescimo);

                decimal valorDesconto, valorAcrescimo = 0;
                decimal.TryParse(Request.Params("ValorDesconto"), out valorDesconto);
                decimal.TryParse(Request.Params("ValorAcrescimo"), out valorAcrescimo);

                if (codigo == 0)
                    return new JsonpResult(false, "Por favor inicie uma baixa antes de lançar os valores.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                if (justificativaAcrescimo > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimo = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo();
                    acrescimo.TituloBaixa = tituloBaixa;
                    acrescimo.Valor = valorAcrescimo;
                    acrescimo.Justificativa = repJustificativa.BuscarPorCodigo(justificativaAcrescimo);

                    repTituloBaixaAcrescimo.Inserir(acrescimo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um acréscimo no valor de " + valorAcrescimo.ToString("n2") + ".", unitOfWork);
                }
                if (justificativaDesconto > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto desconto = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto();
                    desconto.TituloBaixa = tituloBaixa;
                    desconto.Valor = valorDesconto;
                    desconto.Justificativa = repJustificativa.BuscarPorCodigo(justificativaDesconto);

                    repTituloBaixaDesconto.Inserir(desconto);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um desconto no valor de " + valorDesconto.ToString("n2") + ".", unitOfWork);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                SalvarListaCTesRemovidos(tituloBaixa, unitOfWork);


                return new JsonpResult(true, "Sucesso");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados para a negociação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarParcelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_PermiteGerarParcelas)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para gerar parcelas de negociação.");

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int codigoBaixaTitulo, quantidadeParcelas, intervaloDeDias = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBaixaTitulo);
                int.TryParse(Request.Params("QuantidadeParcelas"), out quantidadeParcelas);
                int.TryParse(Request.Params("IntervaloDeDias"), out intervaloDeDias);

                int codigoTipoDePagamento = 0;
                int.TryParse(Request.Params("TipoDePagamento"), out codigoTipoDePagamento);

                DateTime dataPrimeiroVencimento;
                DateTime.TryParse(Request.Params("DataPrimeiroVencimento"), out dataPrimeiroVencimento);

                DateTime dataEmissao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento tipoArredondamento;
                Enum.TryParse(Request.Params("TipoArredondamento"), out tipoArredondamento);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixaTitulo);
                //List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixaTitulo);
                //DateTime dataEmissaoTitulo = DateTime.MinValue;
                //DateTime dataEmissaoAux = DateTime.MinValue;
                //for (int i = 0; i < titulosAgrupados.Count(); i++)
                //{
                //    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                //    dataEmissaoTitulo = titulosAgrupados[i].Titulo.DataEmissao.Value;
                //    if (dataEmissaoTitulo > dataEmissaoAux)
                //        dataEmissaoAux = dataEmissaoTitulo;
                //}
                //if (dataEmissaoAux == DateTime.MinValue)
                //    dataEmissaoAux = DateTime.Now;

                //if (dataEmissao.Date < dataEmissaoAux.Date || dataEmissao.Date > tituloBaixa.DataBaixa.Value.Date)
                //    return new JsonpResult(false, "A data de emissão não pode ser inferior que a maior data dos títulos selecionados (" + dataEmissaoAux.ToString("dd/MM/yyyy") + "), e também não pode ser superior que a data atual.");

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixaTitulo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixaTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixaTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo) : 0;

                descontos = descontos + (tituloBaixa != null ? repTituloBaixaDetalheConhecimento.TotalDescontoPorTituloBaixa(codigoBaixaTitulo) : 0);
                acrescimos = acrescimos + (tituloBaixa != null ? repTituloBaixaDetalheConhecimento.TotalAcrescimoPorTituloBaixa(codigoBaixaTitulo) : 0);

                decimal valorCTeRemovidos = tituloBaixa.ConhecimentosRemovidos != null ? (from p in tituloBaixa.ConhecimentosRemovidos select p.CTe.ValorAReceber).Sum() : 0;
                decimal valorTotal = Math.Round((tituloBaixa.ValorOriginal - tituloBaixa.Valor) - descontos + acrescimos, 2);
                decimal valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2);
                decimal valorDiferenca = valorTotal - Math.Round(valorParcela * quantidadeParcelas, 2);
                DateTime dataUltimaParcela = dataPrimeiroVencimento;

                if (valorParcela <= 0)
                {
                    return new JsonpResult(false, "O valor pendente está zerado. Favor verifique os lançamentos antes de gerar a negociação.");
                }

                for (int i = 0; i < quantidadeParcelas; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao();
                    parcela.DataEmissao = tituloBaixa.DataBaixa.Value.Date;
                    if (i == 0)
                        parcela.DataVencimento = dataPrimeiroVencimento;
                    else
                        parcela.DataVencimento = dataUltimaParcela.AddDays(intervaloDeDias);
                    dataUltimaParcela = parcela.DataVencimento;
                    parcela.Desconto = 0;
                    parcela.TituloBaixa = tituloBaixa;
                    parcela.Sequencia = i + 1;
                    parcela.SituacaoFaturaParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;

                    if (i == 0 && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira)
                        parcela.Valor = valorParcela + valorDiferenca;
                    else if ((i + 1) == quantidadeParcelas && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Ultima)
                        parcela.Valor = valorParcela + valorDiferenca;
                    else
                        parcela.Valor = valorParcela;
                    parcela.FormaParcela = formaTitulo;

                    repTituloBaixaNegociacao.Inserir(parcela);
                }

                if (codigoTipoDePagamento > 0)
                {
                    tituloBaixa.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento);
                }
                else
                {
                    tituloBaixa.TipoPagamentoRecebimento = null;
                }

                repTituloBaixa.Atualizar(tituloBaixa);
                SalvarListaCTesRemovidos(tituloBaixa, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Gerou " + quantidadeParcelas.ToString() + " Parcela(s).", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as parcelas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                int codigoParcela = 0;
                int.TryParse(Request.Params("Codigo"), out codigoParcela);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = repTituloBaixaNegociacao.BuscarPorCodigo(codigoParcela);

                var dynRetorno = new
                {
                    Codigo = parcela.Codigo,
                    Sequencia = parcela.Sequencia.ToString("n0"),
                    Valor = parcela.Valor.ToString("n2"),
                    ValorDesconto = parcela.Desconto.ToString("n2"),
                    DataEmissao = parcela.DataEmissao.ToString("dd/MM/yyyy"),
                    DataVencimento = parcela.DataVencimento.ToString("dd/MM/yyyy"),
                    FormaTitulo = parcela.FormaParcela
                };

                return new JsonpResult(dynRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da parcela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int codigoParcela, codigoBaixa, sequencia = 0;
                int.TryParse(Request.Params("CodigoParcela"), out codigoParcela);
                int.TryParse(Request.Params("Codigo"), out codigoBaixa);
                int.TryParse(Request.Params("Sequencia"), out sequencia);

                decimal valor, valorDesconto = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);
                decimal.TryParse(Request.Params("ValorDesconto"), out valorDesconto);

                DateTime dataEmissao, dataVencimento;
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                //List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
                //DateTime dataEmissaoTitulo = DateTime.MinValue;
                //DateTime dataEmissaoAux = DateTime.MinValue;
                //for (int i = 0; i < titulosAgrupados.Count(); i++)
                //{
                //    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                //    dataEmissaoTitulo = titulosAgrupados[i].Titulo.DataEmissao.Value;
                //    if (dataEmissaoTitulo > dataEmissaoAux)
                //        dataEmissaoAux = dataEmissaoTitulo;
                //}
                //if (dataEmissaoAux == DateTime.MinValue)
                //    dataEmissaoAux = DateTime.Now;

                //if (dataEmissao.Date < dataEmissaoAux.Date || dataEmissao.Date > DateTime.Now.Date)
                //    return new JsonpResult(false, "A data de emissão não pode ser inferior que a maior data dos títulos selecionados (" + dataEmissaoAux.ToString("dd/MM/yyyy") + "), e também não pode ser superior que a data atual.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = repTituloBaixaNegociacao.BuscarPorCodigo(codigoParcela);
                parcela.Valor = valor;
                parcela.Desconto = valorDesconto;
                parcela.DataVencimento = dataVencimento;
                parcela.FormaParcela = formaTitulo;

                repTituloBaixaNegociacao.Atualizar(parcela);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Atualizou a parcela " + parcela.Sequencia.ToString() + ".", unitOfWork);

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo) : 0;
                decimal valorTotal = Math.Round(tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos, 2);

                decimal valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() : 0;
                valorParcelas = Math.Round(valorParcelas, 2);
                int qtdParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p).Count() : 0;

                if ((valorParcelas > 0) && (valorParcelas != valorTotal))
                {
                    decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2);
                    int parcelasRatear = qtdParcelas - sequencia;
                    if (parcelasRatear > 0)
                    {
                        decimal valorRatearParcelas = Math.Round(valorDiferenca / parcelasRatear, 2);

                        for (int i = sequencia; i < tituloBaixa.TitulosNegociacao.Count(); i++)
                        {
                            tituloBaixa.TitulosNegociacao[i].Valor = tituloBaixa.TitulosNegociacao[i].Valor + valorRatearParcelas;
                            repTituloBaixaNegociacao.Atualizar(tituloBaixa.TitulosNegociacao[i]);
                        }
                    }

                    valorTotal = Math.Round(tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos, 2);
                    valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() : 0;
                    valorDiferenca = Math.Round(valorTotal - valorParcelas, 2);
                    if (valorDiferenca != 0 && tituloBaixa.TitulosNegociacao.Count() > sequencia)
                    {
                        tituloBaixa.TitulosNegociacao[sequencia + 1].Valor = tituloBaixa.TitulosNegociacao[sequencia + 1].Valor + valorDiferenca;
                        repTituloBaixaNegociacao.Atualizar(tituloBaixa.TitulosNegociacao[sequencia + 1]);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os valores da parcela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosConhecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);

                int codigoConhecimento = 0, codigoBaixaTitulo = 0;
                int.TryParse(Request.Params("Codigo"), out codigoConhecimento);
                int.TryParse(Request.Params("CodigoBaixaTitulo"), out codigoBaixaTitulo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento detalhe = repTituloBaixaDetalheConhecimento.BuscarPorCTeBaixa(codigoConhecimento, codigoBaixaTitulo);
                if (detalhe != null)
                {
                    var dynRetorno = new
                    {
                        Codigo = detalhe.Codigo,
                        CodigoCTe = detalhe.CTe.Codigo,
                        Numero = detalhe.CTe.Numero.ToString("n0"),
                        Serie = detalhe.CTe.Serie.Numero.ToString("n0"),
                        Tomador = detalhe.CTe.Tomador != null ? detalhe.CTe.Tomador.Nome : "",
                        ValorAReceber = detalhe.CTe.ValorAReceber.ToString("n2"),
                        ValorPago = detalhe.ValorPago.ToString("n2"),
                        ValorDesconto = detalhe.ValorDesconto.ToString("n2"),
                        ValorAcrescimo = detalhe.ValorAcrescimo.ToString("n2"),
                        JustificativaDesconto = new { Codigo = detalhe.JustificativaDesconto != null ? detalhe.JustificativaDesconto.Codigo : 0, Descricao = detalhe.JustificativaDesconto != null ? detalhe.JustificativaDesconto.Descricao : "" },
                        JustificativaAcrescimo = new { Codigo = detalhe.JustificativaAcrescimo != null ? detalhe.JustificativaAcrescimo.Codigo : 0, Descricao = detalhe.JustificativaAcrescimo != null ? detalhe.JustificativaAcrescimo.Descricao : "" },
                        Observacao = detalhe.Observacao
                    };

                    return new JsonpResult(dynRetorno);
                }
                else
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorCodigo(codigoConhecimento);
                    var dynRetorno = new
                    {
                        Codigo = 0,
                        CodigoCTe = cte.Codigo,
                        Numero = cte.Numero.ToString("n0"),
                        Serie = cte.Serie.Numero.ToString("n0"),
                        Tomador = cte.Tomador != null ? cte.Tomador.Nome : "",
                        ValorAReceber = cte.ValorAReceber.ToString("n2"),
                        ValorPago = cte.ValorAReceber.ToString("n2"),
                        ValorDesconto = 0.ToString("n2"),
                        ValorAcrescimo = 0.ToString("n2"),
                        JustificativaDesconto = new { Codigo = 0, Descricao = "" },
                        JustificativaAcrescimo = new { Codigo = 0, Descricao = "" },
                        Observacao = ""
                    };

                    return new JsonpResult(dynRetorno);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da parcela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDetalheConhecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int codigo = 0, codigoCTe = 0, codigoBaixa = 0, codigoDesconto = 0, codigoAcrescimo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoBaixaTitulo"), out codigoBaixa);
                int.TryParse(Request.Params("JustificativaDesconto"), out codigoDesconto);
                int.TryParse(Request.Params("JustificativaAcrescimo"), out codigoAcrescimo);

                decimal valorPago = 0, valorDesconto = 0, valorAcrescimo = 0;
                decimal.TryParse(Request.Params("ValorPago"), out valorPago);
                decimal.TryParse(Request.Params("ValorDesconto"), out valorDesconto);
                decimal.TryParse(Request.Params("ValorAcrescimo"), out valorAcrescimo);

                string observacao = Request.Params("Observacao");

                if (codigoCTe == 0)
                    return new JsonpResult(false, "Conhecimento não localizado.");
                if (codigoBaixa == 0)
                    return new JsonpResult(false, "Baixa de título não localizada.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento detalhe;
                if (codigo > 0)
                    detalhe = repTituloBaixaDetalheConhecimento.BuscarPorCodigo(codigo, true);
                else
                    detalhe = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento();

                unitOfWork.Start();

                detalhe.CTe = repCTe.BuscarPorCodigo(codigoCTe);
                detalhe.TituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (codigoAcrescimo > 0)
                    detalhe.JustificativaAcrescimo = repJustificativa.BuscarPorCodigo(codigoAcrescimo);
                else
                    detalhe.JustificativaAcrescimo = null;

                if (codigoDesconto > 0)
                    detalhe.JustificativaDesconto = repJustificativa.BuscarPorCodigo(codigoDesconto);
                else
                    detalhe.JustificativaDesconto = null;

                detalhe.Observacao = observacao;
                detalhe.ValorAcrescimo = valorAcrescimo;
                detalhe.ValorDesconto = valorDesconto;
                detalhe.ValorPago = valorPago;

                if (codigo > 0)
                {
                    repTituloBaixaDetalheConhecimento.Atualizar(detalhe);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, detalhe.TituloBaixa, detalhe.GetChanges(), "Atualizou detalhes do conhecimento " + detalhe.CTe.Descricao + ".", unitOfWork);
                }
                else
                {
                    repTituloBaixaDetalheConhecimento.Inserir(detalhe);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, detalhe.TituloBaixa, null, "Adicionou detalhes do conhecimento " + detalhe.CTe.Descricao + ".", unitOfWork);
                }

                int codigoTipoDePagamento = 0;
                int.TryParse(Request.Params("TipoDePagamento"), out codigoTipoDePagamento);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
                if (codigoTipoDePagamento > 0)
                {
                    tituloBaixa.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento);
                    repTituloBaixa.Atualizar(tituloBaixa);
                }
                else
                {
                    tituloBaixa.TipoPagamentoRecebimento = null;
                    repTituloBaixa.Atualizar(tituloBaixa);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o detalhe do conhecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FecharBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceber_PermiteFecharBaixa)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para fechar a baixa do título a receber.");

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoTipoDePagamento = Request.GetIntParam("TipoDePagamento");
                SituacaoBaixaTitulo etapa = Request.GetEnumParam<SituacaoBaixaTitulo>("Etapa");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.EmNegociacao && tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, "A situação da baixa não permite que a mesma seja finalizada.");

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto);
                decimal acrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo);
                descontos = descontos + repTituloBaixaDetalheConhecimento.TotalDescontoPorTituloBaixa(codigo);
                acrescimos = acrescimos + repTituloBaixaDetalheConhecimento.TotalAcrescimoPorTituloBaixa(codigo);

                decimal valorPendente = tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos;

                if (valorPendente > 0)
                {
                    if (tituloBaixa.Pessoa != null && (tituloBaixa.TitulosNegociacao == null || tituloBaixa.TitulosNegociacao.Count() <= 0))
                        return new JsonpResult(false, "Por favor gere as parcelas de negociação com o saldo pendente antes de finalizar a baixa.");
                    else if (tituloBaixa.Pessoa == null)
                        return new JsonpResult(false, "Por favor verifique o saldo pendente na baixa de títulos agrupada com diferentes clientes.");

                    decimal valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() : 0;
                    decimal valorCTeRemovidos = tituloBaixa.ConhecimentosRemovidos != null ? (from p in tituloBaixa.ConhecimentosRemovidos select p.CTe.ValorAReceber).Sum() : 0;
                    decimal valorParcelasComCTe = 0;// valorCTeRemovidos;
                    decimal valorDiferenca = Math.Round(valorPendente - valorParcelasComCTe, 2);
                    if (valorDiferenca != 0 && valorParcelasComCTe > 0)
                        return new JsonpResult(false, "Existe diferença entre o valor pendente com as parcelas/conhecimentos removidos. Valor pendente R$ " + valorPendente.ToString("n2") + " Valor das parcelas/conhecimentos removidos R$ " + valorParcelasComCTe.ToString("n2"));
                }
                else if (valorPendente < 0)
                    return new JsonpResult(false, "Por favor verifique o valor pendente pois não pode ser negativo.");

                if (repTitulo.ContemTitulosPagosBaixaTitulo(codigo))
                    return new JsonpResult(false, "Esta baixa de títulos já possui títulos quitados, impossível de fechar a mesma.");

                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorio = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                decimal valorTotalCheques = repositorio.BuscarPorTituloBaixa(tituloBaixa.Codigo).Sum(o => o.Cheque.Valor);

                if (valorTotalCheques > valorABaixar)
                    return new JsonpResult(false, "O valor total dos cheques deve ser menor ou igual ao valor total a receber.");

                unitOfWork.Start();

                tituloBaixa.SituacaoBaixaTitulo = etapa;

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(codigo);
                for (int i = 0; i < listaTitulos.Count(); i++)
                    repTitulo.Deletar(listaTitulos[i]);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTituloBaixa.BuscarTitulosPorCodigo(codigo);
                string observacaoAnterior = "";
                string observacaoInternaAnterior = "";
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(listaTitulo[i].Observacao))
                        observacaoAnterior += " " + listaTitulo[i].Observacao;
                    if (!string.IsNullOrWhiteSpace(listaTitulo[i].ObservacaoInterna))
                        observacaoInternaAnterior += " " + listaTitulo[i].ObservacaoInterna;
                }

                if (tituloBaixa.TitulosNegociacao == null || tituloBaixa.TitulosNegociacao.Count() > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoPadrao = repTituloBaixa.BuscarTipoMovimentoPadrao(codigo);
                    for (int i = 0; i < tituloBaixa.TitulosNegociacao.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao tituloBaixaNegociacao = tituloBaixa.TitulosNegociacao[i];

                        Dominio.Entidades.Embarcador.Financeiro.Titulo novoTitulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                        novoTitulo.Acrescimo = tituloBaixaNegociacao.Acrescimo;
                        novoTitulo.DataEmissao = tituloBaixaNegociacao.DataEmissao;
                        novoTitulo.DataVencimento = tituloBaixaNegociacao.DataVencimento;
                        novoTitulo.DataProgramacaoPagamento = tituloBaixaNegociacao.DataVencimento;
                        novoTitulo.Desconto = tituloBaixaNegociacao.Desconto;
                        novoTitulo.TituloBaixaNegociacao = tituloBaixaNegociacao;
                        novoTitulo.Usuario = this.Usuario;
                        novoTitulo.DataLancamento = DateTime.Now;
                        if (tituloBaixa.GrupoPessoas != null)
                            novoTitulo.GrupoPessoas = tituloBaixa.GrupoPessoas;

                        novoTitulo.Historico = "TITULO GERADO PELO USUÁRIO " + this.Usuario.Nome + " A PARTIR DA NEGOCIAÇÃO DO TÍTULO " + tituloBaixa.CodigosTitulos;

                        if (tituloBaixa.Pessoa != null)
                            novoTitulo.Pessoa = tituloBaixa.Pessoa;

                        if (novoTitulo.GrupoPessoas == null && novoTitulo.Pessoa != null && novoTitulo.Pessoa.GrupoPessoas != null)
                            novoTitulo.GrupoPessoas = novoTitulo.Pessoa.GrupoPessoas;

                        novoTitulo.Sequencia = tituloBaixaNegociacao.Sequencia;
                        novoTitulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                        novoTitulo.DataAlteracao = DateTime.Now;
                        novoTitulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                        novoTitulo.ValorOriginal = tituloBaixaNegociacao.Valor;
                        novoTitulo.ValorPago = 0;

                        if (repTituloBaixa.CodigoFaturaBaixaAReceber(codigo) > 0)
                            novoTitulo.FaturaParcela = repFaturaParcela.BuscarPorFatura(repTituloBaixa.CodigoFaturaBaixaAReceber(codigo))?.FirstOrDefault();
                        else
                            novoTitulo.FaturaParcela = null;

                        novoTitulo.ValorPendente = tituloBaixaNegociacao.Valor;
                        novoTitulo.Empresa = this.Usuario.Empresa;
                        novoTitulo.ValorTituloOriginal = tituloBaixa.ValorTituloOriginal;
                        novoTitulo.TipoDocumentoTituloOriginal = tituloBaixa.TipoDocumentoTituloOriginal;

                        if (!string.IsNullOrWhiteSpace(novoTitulo.TipoDocumentoTituloOriginal) && novoTitulo.TipoDocumentoTituloOriginal.Length > 500)
                            novoTitulo.TipoDocumentoTituloOriginal = novoTitulo.TipoDocumentoTituloOriginal.Substring(0, 499);

                        novoTitulo.NumeroDocumentoTituloOriginal = tituloBaixa.NumeroDocumentoTituloOriginal;

                        if (!string.IsNullOrWhiteSpace(novoTitulo.NumeroDocumentoTituloOriginal) && novoTitulo.NumeroDocumentoTituloOriginal.Length > 4000)
                            novoTitulo.NumeroDocumentoTituloOriginal = novoTitulo.NumeroDocumentoTituloOriginal.Substring(0, 3999);

                        if (!string.IsNullOrWhiteSpace(observacaoInternaAnterior) && observacaoInternaAnterior.Length >= 299)
                            observacaoInternaAnterior = observacaoInternaAnterior.Substring(0, 200);

                        if (!string.IsNullOrWhiteSpace(observacaoAnterior) && observacaoAnterior.Length >= 999)
                            observacaoAnterior = observacaoAnterior.Substring(0, 999);

                        novoTitulo.Observacao = observacaoAnterior;
                        novoTitulo.FormaTitulo = tituloBaixaNegociacao.FormaParcela;
                        novoTitulo.TipoMovimento = tipoMovimentoPadrao;
                        novoTitulo.ObservacaoInterna = !string.IsNullOrWhiteSpace(observacaoInternaAnterior) ? observacaoInternaAnterior : string.Empty;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            novoTitulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                        repTitulo.Inserir(novoTitulo);
                    }
                }

                decimal valorDesconto = Math.Round(descontos, 2), valorAcrescimo = Math.Round(acrescimos, 2);
                valorABaixar = Math.Round(valorABaixar, 2);

                decimal valorTotalTitulos = listaTitulo.Sum(o => o.Saldo);

                decimal totalValorPago = 0, totalDesconto = 0, totalAcrescimo = 0;
                int codigoUltimoTitulo = 0;
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                    if (titulo.DataEmissao.Value.Date > tituloBaixa.DataBaixa.Value.Date)
                        throw new ControllerException("O título " + listaTitulo[i].Codigo.ToString() + " possui a data de emissão maior que a data da baixa.");

                    titulo.DataLiquidacao = tituloBaixa.DataBaixa;
                    titulo.DataBaseLiquidacao = tituloBaixa.DataBase;
                    titulo.Provisao = false;

                    if (valorABaixar > 0)
                    {
                        titulo.ValorPago = Math.Round(((valorABaixar * titulo.Saldo) / valorTotalTitulos), 2);
                        totalValorPago += titulo.ValorPago;
                    }
                    else
                        titulo.ValorPago = 0;

                    if (valorDesconto > 0)
                    {
                        titulo.Desconto = Math.Round(((valorDesconto * titulo.Saldo) / valorTotalTitulos), 2);
                        totalDesconto += titulo.Desconto;
                    }
                    else
                        titulo.Desconto = 0;

                    if (valorAcrescimo > 0)
                    {
                        titulo.Acrescimo = Math.Round(((valorAcrescimo * titulo.Saldo) / valorTotalTitulos), 2);
                        totalAcrescimo += titulo.Acrescimo;
                    }
                    else
                        titulo.Acrescimo = 0;

                    titulo.ValorPendente = 0;

                    if (titulo.ValorPendente == 0)
                        titulo.StatusTitulo = StatusTitulo.Quitada;
                    else if (valorPendente > 0)
                    {
                        titulo.ValorPendente = 0;
                        titulo.StatusTitulo = StatusTitulo.Quitada;
                    }

                    titulo.DataAlteracao = DateTime.Now;
                    codigoUltimoTitulo = titulo.Codigo;
                    repTitulo.Atualizar(titulo);

                    servicoTitulo.RemoverTituloBloqueioFinanceiroPessoa(titulo, TipoServicoMultisoftware);
                }

                if (valorABaixar > (decimal)0 && totalValorPago != valorABaixar)
                {
                    decimal valorDiferenca = Math.Round((valorABaixar - totalValorPago), 2);
                    if (valorDiferenca != (decimal)0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.ValorPago = Math.Round((titulo.ValorPago + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }
                if (valorDesconto > (decimal)0 && totalDesconto != valorDesconto)
                {
                    decimal valorDiferenca = Math.Round((valorDesconto - totalDesconto), 2);
                    if (valorDiferenca != (decimal)0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.Desconto = Math.Round((titulo.Desconto + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }
                if (valorAcrescimo > (decimal)0 && totalAcrescimo != valorAcrescimo)
                {
                    decimal valorDiferenca = Math.Round((valorAcrescimo - totalAcrescimo), 2);
                    if (valorDiferenca != (decimal)0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.Acrescimo = Math.Round((titulo.Acrescimo + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa2 = repTituloBaixa.BuscarPorCodigo(codigo);
                tituloBaixa2.TipoPagamentoRecebimento = codigoTipoDePagamento > 0 ? repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento) : null;
                repTituloBaixa.Atualizar(tituloBaixa2);

                if (tituloBaixa2.TipoPagamentoRecebimento != null || (tituloBaixa2.TipoPagamentoRecebimento == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                {
                    string erro = "";
                    if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                    {
                        if (!servBaixaTituloReceber.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, tituloBaixa2.TipoPagamentoRecebimento?.PlanoConta))
                            throw new ControllerException(erro);
                    }
                    else if (!servBaixaTituloReceber.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, tituloBaixa2.TipoPagamentoRecebimento?.PlanoConta))
                        throw new ControllerException(erro);
                }

                GerarReverterVinculoTituloDocumentoAcrescimoDesconto(tituloBaixa, unitOfWork);
                SalvarListaCTesRemovidos(tituloBaixa, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Fechou Baixa.", unitOfWork);

                unitOfWork.CommitChanges();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && this.Usuario != null && this.Usuario.Empresa != null)
                    servBaixaTituloReceber.GeraIntegracaoBaixaTituloReceber(codigo, this.Usuario.Nome, this.Usuario.Empresa.EmailAdministrativo, unitOfWork, false);

                dynamic dynRetorno = servBaixaTituloReceber.RetornaObjetoCompletoTitulo(codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fechar baixa do título. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
                if (codigo <= 0)
                    return new JsonpResult(false, "Não é possível cancelar uma baixa sem ter iniciado a mesma.");

                if (repTituloBaixa.ContemParcelaQuitada(codigo))
                    return new JsonpResult(false, "Esta baixa já possui parcela de negociação quitada.");
                else
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Cancelada)
                    return new JsonpResult(false, "Esta baixa já se encontra cancelada.");

                if (repTituloBaixa.ContemTitulosNegociadosEmOutraBaixa(codigo))
                    return new JsonpResult(false, "Há parcelas de negociação em outra baixa.");

                unitOfWork.Start();

                if (tituloBaixa.TipoPagamentoRecebimento != null || (tituloBaixa.TipoPagamentoRecebimento == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                {
                    string erro = "";
                    if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada)
                    {
                        if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                        {
                            if (!servBaixaTituloReceber.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta))
                                throw new ControllerException(erro);
                        }
                        else if (!servBaixaTituloReceber.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta))
                            throw new ControllerException(erro);
                    }
                }

                GerarReverterVinculoTituloDocumentoAcrescimoDesconto(tituloBaixa, unitOfWork, true);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTituloBaixa.BuscarTitulosPorCodigo(tituloBaixa.Codigo);
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                    titulo.DataLiquidacao = null;
                    titulo.DataBaseLiquidacao = null;
                    titulo.Desconto = 0;
                    titulo.Acrescimo = 0;
                    titulo.ValorPago = 0;
                    titulo.ValorPendente = listaTitulo[i].Saldo;
                    titulo.StatusTitulo = StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;

                    repTitulo.Atualizar(titulo);

                    if (titulo.FaturaParcela?.Fatura != null)
                    {
                        if (!repTitulo.ContemTitulosPagosFatura(titulo.FaturaParcela.Fatura.Codigo))
                        {
                            titulo.FaturaParcela.Fatura.Situacao = SituacaoFatura.Fechado;
                            repFatura.Atualizar(titulo.FaturaParcela.Fatura);

                            serCargaDadosSumarizados.AtualizarDadosCTesFaturados(titulo.FaturaParcela.Fatura.Codigo, unitOfWork);
                        }
                    }
                }

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(codigo);
                for (int i = 0; i < listaTitulos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulos[i];

                    titulo.DataCancelamento = DateTime.Now.Date;
                    titulo.StatusTitulo = StatusTitulo.Cancelado;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TituloBaixaNegociacao = null;

                    repTitulo.Atualizar(titulo);

                    servicoTitulo.RemoverTituloBloqueioFinanceiroPessoa(titulo, TipoServicoMultisoftware);
                }

                tituloBaixa.SituacaoBaixaTitulo = SituacaoBaixaTitulo.Cancelada;
                repTituloBaixa.Atualizar(tituloBaixa);
                

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Cancelou baixa.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                string observacao = Request.Params("Observacao");
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
                if (codigo > 0)
                {
                    unitOfWork.Start();

                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);
                    tituloBaixa.Observacao = observacao;
                    repTituloBaixa.Atualizar(tituloBaixa, Auditado);

                    unitOfWork.CommitChanges();
                }
                else
                    return new JsonpResult(false, "Não é possível salvar a observação de uma baixa sem ter iniciado a mesma.");

                var dynRetorno = servBaixaTituloReceber.RetornaObjetoCompletoTitulo(codigo, unitOfWork);
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar observacai baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaConhecimentosRemovidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> listaTituloBaixaConhecimentoRemovido = repTituloBaixaConhecimentoRemovido.BuscarPorTituloBaixa(codigo);

                var lista = new List<dynamic>();
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido removido in listaTituloBaixaConhecimentoRemovido)
                {
                    var cteRemovido = new
                    {
                        Codigo = removido.Codigo,
                        CodigoCTe = removido.CTe.Codigo,
                    };
                    lista.Add(cteRemovido);
                }

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os conhecimentos removidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionaConhecimentoRemovido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                int codigo, codigoCTe = 0;
                int.TryParse(Request.Params("CodigoBaixaTitulo"), out codigo);
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                if (codigo == 0 || codigoCTe == 0)
                    return new JsonpResult(false, "Por favor inicie uma baixa e selecione um conhecimento.");

                if (repTituloBaixaConhecimentoRemovido.ContemConhecimentoRemovido(codigo, codigoCTe))
                    return new JsonpResult(false, "Este conhecimento já está adicionado nesta baixa.");

                if (repTituloBaixaConhecimentoRemovido.ContemConhecimentoRemovido(codigoCTe))
                    return new JsonpResult(false, "Este conhecimento já está adicionado em outra baixa.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido cteRemovido = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido();
                cteRemovido.CTe = repCTe.BuscarPorCodigo(codigoCTe);
                cteRemovido.TituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                repTituloBaixaConhecimentoRemovido.Inserir(cteRemovido);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cteRemovido.TituloBaixa, null, "Adicionou o Documento " + cteRemovido.CTe.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir conhecimento removido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverConhecimentoRemovido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido cteRemovido = repTituloBaixaConhecimentoRemovido.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cteRemovido.TituloBaixa, null, "Removeu o Documento " + cteRemovido.CTe.Descricao + ".", unitOfWork);

                repTituloBaixaConhecimentoRemovido.Deletar(cteRemovido);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o conhecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosCheque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorio = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaCheque = repositorio.BuscarPorTituloBaixa(codigo);

                var listaChequeRetornar = (
                    from cheque in listaCheque
                    select new
                    {
                        cheque.Codigo,
                        cheque.Cheque.NumeroCheque,
                        Banco = cheque.Cheque.Banco.Descricao,
                        Pessoa = cheque.Cheque.Pessoa.Descricao,
                        Status = cheque.Cheque.Status.ObterDescricao(),
                        Tipo = cheque.Cheque.Tipo.ObterDescricao(),
                        Valor = cheque.Cheque.Valor.ToString("n2")
                    }
                ).ToList();

                return new JsonpResult(listaChequeRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os cheques.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDadosCheque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixa repositorioTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repositorioTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa == null)
                    throw new ControllerException("Negociação da baixa de títulos não encontrada.");

                if (!IsPermitirGerenciarCheque(tituloBaixa))
                    throw new ControllerException("Situação da baixa de títulos não permite adicionar cheques.");

                List<int> listaCodigoCheque = Request.GetListParam<int>("Cheques");

                if (listaCodigoCheque.Count == 0)
                    throw new ControllerException("Nenhum cheque selecionado.");

                Repositorio.Embarcador.Financeiro.Cheque repositorioCheque = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaTituloBaixaChequeInserir = new List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>();

                foreach (int codigoCheque in listaCodigoCheque)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque()
                    {
                        TituloBaixa = tituloBaixa,
                        Cheque = repositorioCheque.BuscarPorCodigo(codigoCheque) ?? throw new ControllerException("Cheque não encontrado.")
                    };

                    listaTituloBaixaChequeInserir.Add(tituloBaixaCheque);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorioTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque in listaTituloBaixaChequeInserir)
                {
                    repositorioTituloBaixaCheque.Inserir(tituloBaixaCheque);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os cheques.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirChequePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorio = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque = repositorio.BuscarPorCodigo(codigo);

                if (tituloBaixaCheque == null)
                    throw new ControllerException("Cheque não encontrado.");

                if (!IsPermitirGerenciarCheque(tituloBaixaCheque.TituloBaixa))
                    throw new ControllerException("Situação da baixa de títulos não permite remover cheque.");

                repositorio.Deletar(tituloBaixaCheque);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o cheque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                decimal valorPendente = 0m;

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                int totalLinhas = linhas.Count;

                List<object> titulosRetornar = new List<object>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroTitulo = linha.Colunas?.Where(o => o.NomeCampo == "NumeroTitulo").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDocumento = linha.Colunas?.Where(o => o.NomeCampo == "NumeroDocumento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJPessoaCliente = linha.Colunas?.Where(o => o.NomeCampo == "CNPJPessoaCliente").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroFatura = linha.Colunas?.Where(o => o.NomeCampo == "NumeroFatura").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVencimento = linha.Colunas?.Where(o => o.NomeCampo == "Vencimento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = linha.Colunas?.Where(o => o.NomeCampo == "Valor").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroParcela = linha.Colunas?.Where(o => o.NomeCampo == "NumeroParcela").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazaoSocial = linha.Colunas?.Where(o => o.NomeCampo == "RazaoSocial").FirstOrDefault();

                        string razaoSocial = colRazaoSocial?.Valor;
                        string numeroDocumento = colNumeroDocumento?.Valor;

                        int.TryParse(colNumeroFatura?.Valor ?? "0", out int numeroFatura);
                        int.TryParse(colNumeroTitulo?.Valor ?? "0", out int numeroTitulo);
                        int.TryParse(colNumeroParcela?.Valor ?? "0", out int numeroParcela);
                        decimal.TryParse(colValor?.Valor ?? "0", out decimal valor);
                        double.TryParse(Utilidades.String.OnlyNumbers(colCNPJPessoaCliente?.Valor) ?? "0", out double cnpjPessoaCliente);
                        DateTime.TryParse(colVencimento?.Valor, out DateTime vencimento);

                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = null;
                        string mensagemErro = string.Empty;

                        if (numeroFatura > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorFatura(numeroFatura);
                        }
                        else if (numeroTitulo > 0)
                        {
                            titulos = repTitulo.BuscarTituloReceberPendentePorCodigo(numeroTitulo);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente com o número {numeroTitulo}. ";
                        }

                        if ((titulos == null || titulos.Count == 0) && !string.IsNullOrWhiteSpace(numeroDocumento) && cnpjPessoaCliente > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorNumeroDocumento(cnpjPessoaCliente, numeroDocumento, valor, numeroParcela);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente para o cliente {cnpjPessoaCliente} com o número {numeroDocumento}. ";
                        }

                        if ((titulos == null || titulos.Count == 0) && !string.IsNullOrWhiteSpace(razaoSocial) && vencimento != DateTime.MinValue && valor > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorTitulo(razaoSocial, vencimento, valor);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente para o emissor {razaoSocial} com o valor {valor} e vencimento {vencimento}. ";
                        }

                        if (titulos != null && titulos.Count > 0)
                        {
                            valorPendente += titulos.Sum(o => o.ValorPendente);

                            titulosRetornar.AddRange(titulos.Select(o => ObterDetalhesTituloGrid(o)));

                            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                            contador++;
                        }
                        else
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(mensagemErro, i));

                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                        continue;
                    }
                }

                retornoImportacao.Retorno = new { ValorPendente = valorPendente.ToString("n2"), Titulos = titulosRetornar };
                retornoImportacao.Total = linhas.Count;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter as configurações para importação.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente ObterFiltrosPesquisaTituloPendente()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente()
            {
                DataVencimentoInicial = Request.GetDateTimeParam("DataInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataFinal"),
                DataProgramacaoPagamentoInicial = Request.GetDateTimeParam("DataProgramacaoPagamentoInicial"),
                DataProgramacaoPagamentoFinal = Request.GetDateTimeParam("DataProgramacaoPagamentoFinal"),
                CodigoBaixa = Request.GetIntParam("Codigo"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigoFatura = Request.GetIntParam("Fatura"),
                CodigoConhecimento = Request.GetIntParam("Conhecimento"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroOcorrencia = Request.GetStringParam("NumeroOcorrencia"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                SomenteTitulosDeNegociacao = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("TitulosDeAgrupamento"),
                FormaTitulo = Request.GetEnumParam<FormaTitulo>("FormaTitulo"),
                TipoTitulo = TipoTitulo.Receber,
                TipoServico = TipoServicoMultisoftware,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

        private bool IsPermitirGerenciarCheque(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa)
        {
            return (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada) || (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao);
        }

        private void SalvarListaCTesRemovidos(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> listaCTeRemovido = repTituloBaixaConhecimentoRemovido.BuscarPorBaixaTitulo(tituloBaixa.Codigo);
            for (int i = 0; i < listaCTeRemovido.Count(); i++)
                repTituloBaixaConhecimentoRemovido.Deletar(listaCTeRemovido[i]);

            dynamic listaCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CTesRemovidos"));
            if (listaCTe != null)
            {
                foreach (var cte in listaCTe)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido cteRemovido = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido();
                    cteRemovido.TituloBaixa = tituloBaixa;
                    cteRemovido.CTe = repCTe.BuscarPorCodigo((int)cte.Codigo.val);
                    repTituloBaixaConhecimentoRemovido.Inserir(cteRemovido);
                }
            }
        }

        private List<int> RetornaCodigosTitulos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private object ObterDetalhesTituloGrid(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            return new
            {
                DT_RowId = titulo.Codigo,
                titulo.Codigo,
                CNPJPessoa = titulo.Pessoa != null ? titulo.Pessoa.CPF_CNPJ : 0,
                CodigoTitulo = titulo.Codigo.ToString("n0"),
                Cargas = titulo.NumeroCargas,
                Fatura = titulo.FaturaParcela != null && titulo.FaturaParcela.Fatura != null ? titulo.FaturaParcela.Fatura.Numero.ToString("n0") : string.Empty,
                Conhecimentos = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? titulo.NumeroConhecimentos :
                                    titulo.ConhecimentoDeTransporteEletronico != null ? titulo.ConhecimentoDeTransporteEletronico.Numero + "-" + titulo.ConhecimentoDeTransporteEletronico.Serie?.Numero.ToString() : string.Empty,
                NotaFiscal = titulo.NotaFiscal != null ? titulo.NotaFiscal.Numero.ToString("n0") : string.Empty,
                NumeroParcela = titulo.Sequencia.ToString("n0"),
                titulo.NumeroDocumentoTituloOriginal,
                titulo.NossoNumero,
                DataEmissao = titulo.DataEmissao.Value.ToString("dd/MM/yyyy"),
                DataVencimento = titulo.DataVencimento.Value.ToString("dd/MM/yyyy"),
                Pessoa = titulo.Pessoa != null ? titulo.Pessoa.Nome : titulo.GrupoPessoas != null ? titulo.GrupoPessoas.Descricao : string.Empty,
                ValorOriginalMoedaEstrangeira = titulo.ValorOriginalMoedaEstrangeira.ToString("n2"),
                Valor = titulo.Saldo.ToString("n2"),
                DT_RowColor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                    titulo.DataVencimento.Value.Date < DateTime.Now.Date ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Red : titulo.DataVencimento.Value.Date == DateTime.Now.Date ?
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Orange : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco,
                DT_FontColor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && (titulo.DataVencimento.Value.Date < DateTime.Now.Date || titulo.DataVencimento.Value.Date == DateTime.Now.Date) ?
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza
            };
        }

        private void GerarReverterVinculoTituloDocumentoAcrescimoDesconto(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork, bool reverter = false)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

            if (reverter)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> listaTituloDocumentoAcrescimoDesconto = repTituloDocumentoAcrescimoDesconto.BuscarPorTituloBaixa(tituloBaixa.Codigo);
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto in listaTituloDocumentoAcrescimoDesconto)
                    repTituloDocumentoAcrescimoDesconto.Deletar(tituloDocumentoAcrescimoDesconto);

                return;
            }

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in listaTituloBaixaAcrescimo)
            {
                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = acrescimoDesconto.Justificativa;

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto()
                {
                    TituloBaixaAcrescimo = acrescimoDesconto,
                    Justificativa = justificativa,
                    Tipo = EnumTipoAcrescimoDescontoTituloDocumento.Baixa,
                    TipoJustificativa = justificativa.TipoJustificativa,
                    TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                    TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                    Valor = acrescimoDesconto.Valor,
                    Usuario = Usuario
                };

                repTituloDocumentoAcrescimoDesconto.Inserir(tituloDocumentoAcrescimoDesconto);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Título", Propriedade = "NumeroTitulo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número do Documento", Propriedade = "NumeroDocumento", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ Pessoa/Cliente", Propriedade = "CNPJPessoaCliente", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Número da Fatura", Propriedade = "NumeroFatura", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Vencimento", Propriedade = "Vencimento", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Valor", Propriedade = "Valor", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Parcela", Propriedade = "NumeroParcela", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Razão Social", Propriedade = "RazaoSocial", Tamanho = 100, Obrigatorio = false });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion
    }
}

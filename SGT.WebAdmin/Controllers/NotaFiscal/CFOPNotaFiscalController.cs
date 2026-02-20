using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/CFOP", "Financeiros/DocumentoEntrada")]
    public class CFOPNotaFiscalController : BaseController
    {
		#region Construtores

		public CFOPNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                string descricao = Request.Params("Descricao");
                string status = Request.Params("Status");
                string dentroEstado = Request.Params("DentroEstado");
                string extensao = Request.Params("Extensao");

                int indicadorPresenca = Request.GetIntParam("IndicadorPresenca");
                int numeroCFOP = Request.GetIntParam("NumeroCFOP");
                int codigoEmpresa = Request.GetIntParam("Empresa");
                int codigoNaturezaOperacao = Request.GetIntParam("NaturezaOperacao");
                int codigoLocalidadeInicioPrestacao = Request.GetIntParam("LocalidadeInicioPrestacao");
                int codigoLocalidadeTerminoPrestacao = Request.GetIntParam("LocalidadeTerminoPrestacao");

                double codigoPessoa = Request.GetDoubleParam("Pessoa");

                Dominio.Entidades.Cliente pessoa = codigoPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(codigoPessoa) : null;
                Dominio.Entidades.Empresa empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                Dominio.Entidades.Localidade localidadeInicioPrestacao = codigoLocalidadeInicioPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeInicioPrestacao) : null;
                Dominio.Entidades.Localidade localidadeTerminoPrestacao = codigoLocalidadeTerminoPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao) : null;

                if (localidadeInicioPrestacao != null && localidadeTerminoPrestacao != null)
                    dentroEstado = localidadeInicioPrestacao.Estado.Sigla == localidadeTerminoPrestacao.Estado.Sigla ? "S" : "N";
                else if (pessoa != null)
                {
                    if (indicadorPresenca == 1 && pessoa.IndicadorIE == IndicadorIE.NaoContribuinte)
                        dentroEstado = "S";
                    else
                        dentroEstado = pessoa.Localidade.Estado.Sigla == (empresa?.Localidade.Estado.Sigla ?? Usuario.Empresa.Localidade.Estado.Sigla) ? "S" : "N";
                }

                int codigoEmpresaPesquisa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresaPesquisa = this.Usuario.Empresa.Codigo;

                Dominio.Enumeradores.TipoCFOP? tipoCFOP = null;
                Dominio.Enumeradores.TipoCFOP tipoCFOPAux;
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoEmissao")))
                {
                    if (Enum.TryParse(Request.Params("TipoEmissao"), out tipoCFOPAux))
                    {
                        if ((int)tipoCFOPAux >= 0)
                            tipoCFOP = tipoCFOPAux;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(Request.Params("Tipo")))
                {
                    if (Enum.TryParse(Request.Params("Tipo"), out tipoCFOPAux))
                        tipoCFOP = tipoCFOPAux;
                }
                else if (!string.IsNullOrWhiteSpace(Request.Params("TipoEntradaSaida")))
                {
                    if (Enum.TryParse(Request.Params("TipoEntradaSaida"), out tipoCFOPAux))
                        tipoCFOP = tipoCFOPAux;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CFOP.Descricao, "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CFOP.Numero, "CodigoCFOP", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CFOP.Extensao, "Extensao", 10, Models.Grid.Align.right, true);

                List<Dominio.Entidades.CFOP> listaCFOP = repCFOP.Consultar(numeroCFOP, descricao, tipoCFOP, codigoEmpresaPesquisa, codigoNaturezaOperacao, dentroEstado, status, extensao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCFOP.ContarConsulta(numeroCFOP, descricao, tipoCFOP, codigoEmpresaPesquisa, codigoNaturezaOperacao, dentroEstado, status, extensao));

                var lista = (from p in listaCFOP
                             select new
                             {
                                 p.Codigo,
                                 Descricao = (!string.IsNullOrWhiteSpace(p.Extensao) ? (p.CodigoCFOP.ToString() + "." + p.Extensao) : p.CodigoCFOP.ToString()) + " - " + p.Descricao,
                                 p.CodigoCFOP,
                                 p.Extensao
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Dominio.Entidades.CFOP cfop = new Dominio.Entidades.CFOP();

                PreencherCFOP(cfop, unitOfWork);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    VerificarDuplicidade(repCFOP, cfop);
                }

                repCFOP.Inserir(cfop, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCodigo(codigo, true);

                PreencherCFOP(cfop, unitOfWork);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    VerificarDuplicidade(repCFOP, cfop);
                }

                repCFOP.Atualizar(cfop, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCodigo(codigo);

                if (cfop == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro da CFOP.");

                return new JsonpResult(new
                {
                    cfop.Codigo,
                    cfop.CodigoCFOP,
                    cfop.Extensao,
                    cfop.Tipo,
                    cfop.Descricao,
                    cfop.Status,
                    cfop.GeraEstoque,
                    cfop.RealizarRateioDespesaVeiculo,
                    cfop.RealizarRateioSomenteQuandoTiverOS,
                    cfop.IrrelevanteParaNaoConformidade,
                    cfop.GrupoPrioridade,
                    cfop.CSTICMS,
                    cfop.CSTIPI,
                    cfop.ReducaoBCIPI,
                    cfop.AliquotaIPI,
                    cfop.CSTPIS,
                    cfop.ReducaoBCPIS,
                    cfop.AliquotaPIS,
                    cfop.CSTCOFINS,
                    cfop.ReducaoBCCOFINS,
                    cfop.AliquotaCOFINS,
                    AliquotaInterna = cfop.AliquotaICMSInterna,
                    AliquotaInterestadual = cfop.AliquotaICMSInterestadual,
                    cfop.AliquotaDiferencial,
                    cfop.MVA,
                    cfop.ReducaoMVA,
                    Empresa = new { Codigo = cfop.Empresa?.Codigo ?? 0, Descricao = cfop.Empresa?.RazaoSocial ?? string.Empty },
                    cfop.BloqueioDocumentoEntrada,
                    cfop.GerarMovimentoAutomatico,
                    cfop.GerarMovimentoAutomaticoCOFINS,
                    cfop.GerarMovimentoAutomaticoDesconto,
                    cfop.GerarMovimentoAutomaticoDiferencial,
                    cfop.GerarMovimentoAutomaticoFrete,
                    cfop.GerarMovimentoAutomaticoICMS,
                    cfop.GerarMovimentoAutomaticoICMSST,
                    cfop.GerarMovimentoAutomaticoIPI,
                    cfop.GerarMovimentoAutomaticoOutrasDespesas,
                    cfop.GerarMovimentoAutomaticoPIS,
                    cfop.GerarMovimentoAutomaticoSeguro,
                    cfop.GerarMovimentoAutomaticoFreteFora,
                    cfop.GerarMovimentoAutomaticoOutrasFora,
                    cfop.GerarMovimentoAutomaticoDescontoFora,
                    cfop.GerarMovimentoAutomaticoImpostoFora,
                    cfop.GerarMovimentoAutomaticoDiferencialFreteFora,
                    cfop.GerarMovimentoAutomaticoICMSFreteFora,
                    cfop.GerarMovimentoAutomaticoCusto,
                    cfop.GerarMovimentoAutomaticoRetencaoCOFINS,
                    cfop.GerarMovimentoAutomaticoRetencaoCSLL,
                    cfop.GerarMovimentoAutomaticoRetencaoINSS,
                    cfop.GerarMovimentoAutomaticoRetencaoIPI,
                    cfop.GerarMovimentoAutomaticoRetencaoOutras,
                    cfop.GerarMovimentoAutomaticoRetencaoPIS,
                    cfop.GerarMovimentoAutomaticoRetencaoISS,
                    cfop.GerarMovimentoAutomaticoRetencaoIR,
                    cfop.ObrigarVincularAbastecimentoAoItemDocumentoEntrada,
                    cfop.ObrigarInformarLocalArmazenamento,
                    cfop.CreditoSobreTotalParaItensSujeitosICMSST,
                    cfop.CreditoSobreTotalParaProdutosUsoConsumo,
                    TipoMovimentoReversao = new { Descricao = cfop.TipoMovimentoReversao?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversao?.Codigo ?? 0 },
                    TipoMovimentoReversaoCOFINS = new { Descricao = cfop.TipoMovimentoReversaoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoCOFINS?.Codigo ?? 0 },
                    TipoMovimentoReversaoDesconto = new { Descricao = cfop.TipoMovimentoReversaoDesconto?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoDesconto?.Codigo ?? 0 },
                    TipoMovimentoReversaoDiferencial = new { Descricao = cfop.TipoMovimentoReversaoDiferencial?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoDiferencial?.Codigo ?? 0 },
                    TipoMovimentoReversaoFrete = new { Descricao = cfop.TipoMovimentoReversaoFrete?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoFrete?.Codigo ?? 0 },
                    TipoMovimentoReversaoICMS = new { Descricao = cfop.TipoMovimentoReversaoICMS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoICMS?.Codigo ?? 0 },
                    TipoMovimentoReversaoICMSST = new { Descricao = cfop.TipoMovimentoReversaoICMSST?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoICMSST?.Codigo ?? 0 },
                    TipoMovimentoReversaoIPI = new { Descricao = cfop.TipoMovimentoReversaoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoIPI?.Codigo ?? 0 },
                    TipoMovimentoReversaoOutrasDespesas = new { Descricao = cfop.TipoMovimentoReversaoOutrasDespesas?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoOutrasDespesas?.Codigo ?? 0 },
                    TipoMovimentoReversaoPIS = new { Descricao = cfop.TipoMovimentoReversaoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoPIS?.Codigo ?? 0 },
                    TipoMovimentoUso = new { Descricao = cfop.TipoMovimentoUso?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUso?.Codigo ?? 0 },
                    TipoMovimentoUsoCOFINS = new { Descricao = cfop.TipoMovimentoUsoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoCOFINS?.Codigo ?? 0 },
                    TipoMovimentoUsoDesconto = new { Descricao = cfop.TipoMovimentoUsoDesconto?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoDesconto?.Codigo ?? 0 },
                    TipoMovimentoUsoDiferencial = new { Descricao = cfop.TipoMovimentoUsoDiferencial?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoDiferencial?.Codigo ?? 0 },
                    TipoMovimentoUsoFrete = new { Descricao = cfop.TipoMovimentoUsoFrete?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoFrete?.Codigo ?? 0 },
                    TipoMovimentoUsoICMS = new { Descricao = cfop.TipoMovimentoUsoICMS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoICMS?.Codigo ?? 0 },
                    TipoMovimentoUsoICMSST = new { Descricao = cfop.TipoMovimentoUsoICMSST?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoICMSST?.Codigo ?? 0 },
                    TipoMovimentoUsoIPI = new { Descricao = cfop.TipoMovimentoUsoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoIPI?.Codigo ?? 0 },
                    TipoMovimentoUsoOutrasDespesas = new { Descricao = cfop.TipoMovimentoUsoOutrasDespesas?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoOutrasDespesas?.Codigo ?? 0 },
                    TipoMovimentoUsoPIS = new { Descricao = cfop.TipoMovimentoUsoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoPIS?.Codigo ?? 0 },
                    TipoMovimentoUsoSeguro = new { Descricao = cfop.TipoMovimentoUsoSeguro?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoSeguro?.Codigo ?? 0 },
                    TipoMovimentoReversaoSeguro = new { Descricao = cfop.TipoMovimentoReversaoSeguro?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoSeguro?.Codigo ?? 0 },
                    TipoMovimentoUsoFreteFora = new { Descricao = cfop.TipoMovimentoUsoFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoFreteFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoFreteFora = new { Descricao = cfop.TipoMovimentoReversaoFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoFreteFora?.Codigo ?? 0 },
                    TipoMovimentoUsoOutrasFora = new { Descricao = cfop.TipoMovimentoUsoOutrasFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoOutrasFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoOutrasFora = new { Descricao = cfop.TipoMovimentoReversaoOutrasFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoOutrasFora?.Codigo ?? 0 },
                    TipoMovimentoUsoDescontoFora = new { Descricao = cfop.TipoMovimentoUsoDescontoFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoDescontoFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoDescontoFora = new { Descricao = cfop.TipoMovimentoReversaoDescontoFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoDescontoFora?.Codigo ?? 0 },
                    TipoMovimentoUsoImpostoFora = new { Descricao = cfop.TipoMovimentoUsoImpostoFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoImpostoFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoImpostoFora = new { Descricao = cfop.TipoMovimentoReversaoImpostoFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoImpostoFora?.Codigo ?? 0 },
                    TipoMovimentoUsoDiferencialFreteFora = new { Descricao = cfop.TipoMovimentoUsoDiferencialFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoDiferencialFreteFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoDiferencialFreteFora = new { Descricao = cfop.TipoMovimentoReversaoDiferencialFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoDiferencialFreteFora?.Codigo ?? 0 },
                    TipoMovimentoUsoICMSFreteFora = new { Descricao = cfop.TipoMovimentoUsoICMSFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoICMSFreteFora?.Codigo ?? 0 },
                    TipoMovimentoReversaoICMSFreteFora = new { Descricao = cfop.TipoMovimentoReversaoICMSFreteFora?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoICMSFreteFora?.Codigo ?? 0 },
                    TipoMovimentoUsoCusto = new { Descricao = cfop.TipoMovimentoUsoCusto?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoCusto?.Codigo ?? 0 },
                    TipoMovimentoReversaoCusto = new { Descricao = cfop.TipoMovimentoReversaoCusto?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoCusto?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoCOFINS = new { Descricao = cfop.TipoMovimentoUsoRetencaoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoCOFINS?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoCOFINS = new { Descricao = cfop.TipoMovimentoReversaoRetencaoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoCOFINS?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoCSLL = new { Descricao = cfop.TipoMovimentoUsoRetencaoCSLL?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoCSLL?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoCSLL = new { Descricao = cfop.TipoMovimentoReversaoRetencaoCSLL?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoCSLL?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoINSS = new { Descricao = cfop.TipoMovimentoUsoRetencaoINSS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoINSS?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoINSS = new { Descricao = cfop.TipoMovimentoReversaoRetencaoINSS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoINSS?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoIPI = new { Descricao = cfop.TipoMovimentoUsoRetencaoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoIPI?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoIPI = new { Descricao = cfop.TipoMovimentoReversaoRetencaoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoIPI?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoOutras = new { Descricao = cfop.TipoMovimentoUsoRetencaoOutras?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoOutras?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoOutras = new { Descricao = cfop.TipoMovimentoReversaoRetencaoOutras?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoOutras?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoPIS = new { Descricao = cfop.TipoMovimentoUsoRetencaoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoPIS?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoPIS = new { Descricao = cfop.TipoMovimentoReversaoRetencaoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoPIS?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoISS = new { Descricao = cfop.TipoMovimentoUsoRetencaoISS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoISS?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoISS = new { Descricao = cfop.TipoMovimentoReversaoRetencaoISS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoISS?.Codigo ?? 0 },
                    TipoMovimentoUsoRetencaoIR = new { Descricao = cfop.TipoMovimentoUsoRetencaoIR?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoRetencaoIR?.Codigo ?? 0 },
                    TipoMovimentoReversaoRetencaoIR = new { Descricao = cfop.TipoMovimentoReversaoRetencaoIR?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoRetencaoIR?.Codigo ?? 0 },
                    cfop.ReduzValorLiquidoRetencaoPIS,
                    cfop.GerarGuiaPagarRetencaoPIS,
                    TipoMovimentoUsoTituloRetencaoPIS = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoPIS?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoPIS = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoPIS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoPIS?.Codigo ?? 0 },
                    FornecedorRetencaoPIS = new { Descricao = cfop.FornecedorRetencaoPIS?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoPIS?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoPIS,
                    CalcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS = cfop?.CalcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS ?? false,
                    cfop.ReduzValorLiquidoRetencaoCOFINS,
                    cfop.GerarGuiaPagarRetencaoCOFINS,
                    TipoMovimentoUsoTituloRetencaoCOFINS = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoCOFINS?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoCOFINS = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoCOFINS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoCOFINS?.Codigo ?? 0 },
                    FornecedorRetencaoCOFINS = new { Descricao = cfop.FornecedorRetencaoCOFINS?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoCOFINS?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoCOFINS,
                    cfop.ReduzValorLiquidoRetencaoINSS,
                    cfop.GerarGuiaPagarRetencaoINSS,
                    TipoMovimentoUsoTituloRetencaoINSS = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoINSS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoINSS?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoINSS = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoINSS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoINSS?.Codigo ?? 0 },
                    FornecedorRetencaoINSS = new { Descricao = cfop.FornecedorRetencaoINSS?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoINSS?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoINSS,
                    cfop.ReduzValorLiquidoRetencaoCSLL,
                    cfop.GerarGuiaPagarRetencaoCSLL,
                    TipoMovimentoUsoTituloRetencaoCSLL = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoCSLL?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoCSLL?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoCSLL = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoCSLL?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoCSLL?.Codigo ?? 0 },
                    FornecedorRetencaoCSLL = new { Descricao = cfop.FornecedorRetencaoCSLL?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoCSLL?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoCSLL,
                    cfop.ReduzValorLiquidoRetencaoIPI,
                    cfop.GerarGuiaPagarRetencaoIPI,
                    TipoMovimentoUsoTituloRetencaoIPI = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoIPI?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoIPI = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoIPI?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoIPI?.Codigo ?? 0 },
                    FornecedorRetencaoIPI = new { Descricao = cfop.FornecedorRetencaoIPI?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoIPI?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoIPI,
                    cfop.ReduzValorLiquidoRetencaoOutras,
                    cfop.GerarGuiaPagarRetencaoOutras,
                    TipoMovimentoUsoTituloRetencaoOutras = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoOutras?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoOutras?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoOutras = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoOutras?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoOutras?.Codigo ?? 0 },
                    FornecedorRetencaoOutras = new { Descricao = cfop.FornecedorRetencaoOutras?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoOutras?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoOutras,
                    cfop.ReduzValorLiquidoRetencaoISS,
                    cfop.GerarGuiaPagarRetencaoISS,
                    TipoMovimentoUsoTituloRetencaoISS = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoISS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoISS?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoISS = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoISS?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoISS?.Codigo ?? 0 },
                    FornecedorRetencaoISS = new { Descricao = cfop.FornecedorRetencaoISS?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoISS?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoISS,
                    cfop.ReduzValorLiquidoRetencaoIR,
                    cfop.GerarGuiaPagarRetencaoIR,
                    TipoMovimentoUsoTituloRetencaoIR = new { Descricao = cfop.TipoMovimentoUsoTituloRetencaoIR?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoUsoTituloRetencaoIR?.Codigo ?? 0 },
                    TipoMovimentoReversaoTituloRetencaoIR = new { Descricao = cfop.TipoMovimentoReversaoTituloRetencaoIR?.Descricao ?? string.Empty, Codigo = cfop.TipoMovimentoReversaoTituloRetencaoIR?.Codigo ?? 0 },
                    FornecedorRetencaoIR = new { Descricao = cfop.FornecedorRetencaoIR?.Nome ?? string.Empty, Codigo = cfop.FornecedorRetencaoIR?.CPF_CNPJ ?? 0 },
                    cfop.DiaGerencaoRetencaoIR,
                    cfop.AliquotaRetencaoPIS,
                    cfop.AliquotaRetencaoCOFINS,
                    cfop.AliquotaRetencaoINSS,
                    cfop.AliquotaRetencaoIPI,
                    cfop.AliquotaRetencaoCSLL,
                    cfop.AliquotaRetencaoOutras,
                    cfop.AliquotaRetencaoIR,
                    cfop.AliquotaRetencaoISS,
                    AliquotaParaCredito = cfop.CreditoSobreTotalParaProdutosUsoConsumo ? cfop.AliquotaParaCredito : 0,
                    NaturezaOperacaoCTe = new { Codigo = cfop.NaturezaDaOperacao?.Codigo ?? 0, Descricao = cfop.NaturezaDaOperacao?.Descricao }
                });
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

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCodigo(codigo, true);

                repCFOP.Deletar(cfop, Auditado);

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

        #region Métodos Privados

        private void PreencherCFOP(Dominio.Entidades.CFOP cfop, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

            int codigoCFOP, empresa, codigoTipoMovimentoUso, codigoTipoMovimentoReversao, codigoTipoMovimentoUsoDesconto, codigoTipoMovimentoReversaoDesconto, codigoTipoMovimentoUsoOutrasDespesas, codigoTipoMovimentoReversaoOutrasDespesas,
                    codigoTipoMovimentoReversaoFrete, codigoTipoMovimentoUsoICMS, codigoTipoMovimentoUsoFrete, codigoTipoMovimentoReversaoICMS, codigoTipoMovimentoUsoPIS, codigoTipoMovimentoReversaoPIS, codigoTipoMovimentoUsoCOFINS, codigoTipoMovimentoReversaoCOFINS,
                    codigoTipoMovimentoUsoIPI, codigoTipoMovimentoReversaoIPI, codigoTipoMovimentoUsoICMSST, codigoTipoMovimentoUsoDiferencial, codigoTipoMovimentoReversaoICMSST, codigoTipoMovimentoReversaoDiferencial, codigoTipoMovimentoUsoSeguro,
                    codigoTipoMovimentoReversaoSeguro, codigoTipoMovimentoUsoFreteFora, codigoTipoMovimentoReversaoFreteFora, codigoTipoMovimentoUsoOutrasFora, codigoTipoMovimentoRevesaoOutrasFora, codigoTipoMovimentoUsoDescontoFora, codigoTipoMovimentoReversaoDescontoFora,
                    codigoTipoMovimentoUsoImpostoFora, codigoTipoMovimentoReversaoImpostoFora, codigoTipoMovimentoUsoDiferencialFreteFora, codigoTipoMovimentoReversaoDiferencialFreteFora, codigoTipoMovimentoUsoICMSFreteFora, codigoTipoMovimentoReversaoICMSFreteFota, codigoTipoMovimentoUsoCusto, codigoTipoMovimentoReversaoCusto,
                    codigoTipoMovimentoUsoRetencaoPIS, codigoTipoMovimentoReversaoRetencaoPIS, codigoTipoMovimentoReversaoRetencaoCOFINS, codigoTipoMovimentoUsoRetencaoIPI, codigoTipoMovimentoReversaoRetencaoOutras, codigoTipoMovimentoUsoRetencaoOutras, codigoTipoMovimentoUsoRetencaoCSLL,
                    codigoTipoMovimentoReversaoRetencaoCSLL, codigoTipoMovimentoUsoRetencaoINSS, codigoTipoMovimentoReversaoRetencaoINSS, codigoTipoMovimentoReversaoRetencaoIPI, codigoTipoMovimentoUsoRetencaoCOFINS,
                    codigoTipoMovimentoReversaoRetencaoISS, codigoTipoMovimentoUsoRetencaoISS, codigoTipoMovimentoReversaoRetencaoIR, codigoTipoMovimentoUsoRetencaoIR;

            int codigoNaturezaOperacao = Request.GetIntParam("NaturezaOperacaoCTe");
            int grupoPrioridade = Request.GetIntParam("GrupoPrioridade");
            int.TryParse(Request.Params("CodigoCFOP"), out codigoCFOP);
            int.TryParse(Request.Params("Empresa"), out empresa);
            int.TryParse(Request.Params("TipoMovimentoUso"), out codigoTipoMovimentoUso);
            int.TryParse(Request.Params("TipoMovimentoReversao"), out codigoTipoMovimentoReversao);
            int.TryParse(Request.Params("TipoMovimentoUsoDesconto"), out codigoTipoMovimentoUsoDesconto);
            int.TryParse(Request.Params("TipoMovimentoReversaoDesconto"), out codigoTipoMovimentoReversaoDesconto);
            int.TryParse(Request.Params("TipoMovimentoUsoOutrasDespesas"), out codigoTipoMovimentoUsoOutrasDespesas);
            int.TryParse(Request.Params("TipoMovimentoReversaoOutrasDespesas"), out codigoTipoMovimentoReversaoOutrasDespesas);
            int.TryParse(Request.Params("TipoMovimentoUsoFrete"), out codigoTipoMovimentoUsoFrete);
            int.TryParse(Request.Params("TipoMovimentoReversaoFrete"), out codigoTipoMovimentoReversaoFrete);
            int.TryParse(Request.Params("TipoMovimentoUsoICMS"), out codigoTipoMovimentoUsoICMS);
            int.TryParse(Request.Params("TipoMovimentoReversaoICMS"), out codigoTipoMovimentoReversaoICMS);
            int.TryParse(Request.Params("TipoMovimentoUsoPIS"), out codigoTipoMovimentoUsoPIS);
            int.TryParse(Request.Params("TipoMovimentoReversaoPIS"), out codigoTipoMovimentoReversaoPIS);
            int.TryParse(Request.Params("TipoMovimentoUsoCOFINS"), out codigoTipoMovimentoUsoCOFINS);
            int.TryParse(Request.Params("TipoMovimentoReversaoCOFINS"), out codigoTipoMovimentoReversaoCOFINS);
            int.TryParse(Request.Params("TipoMovimentoUsoIPI"), out codigoTipoMovimentoUsoIPI);
            int.TryParse(Request.Params("TipoMovimentoReversaoIPI"), out codigoTipoMovimentoReversaoIPI);
            int.TryParse(Request.Params("TipoMovimentoUsoICMSST"), out codigoTipoMovimentoUsoICMSST);
            int.TryParse(Request.Params("TipoMovimentoReversaoICMSST"), out codigoTipoMovimentoReversaoICMSST);
            int.TryParse(Request.Params("TipoMovimentoUsoDiferencial"), out codigoTipoMovimentoUsoDiferencial);
            int.TryParse(Request.Params("TipoMovimentoReversaoDiferencial"), out codigoTipoMovimentoReversaoDiferencial);
            int.TryParse(Request.Params("TipoMovimentoUsoSeguro"), out codigoTipoMovimentoUsoSeguro);
            int.TryParse(Request.Params("TipoMovimentoReversaoSeguro"), out codigoTipoMovimentoReversaoSeguro);
            int.TryParse(Request.Params("TipoMovimentoUsoFreteFora"), out codigoTipoMovimentoUsoFreteFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoFreteFora"), out codigoTipoMovimentoReversaoFreteFora);
            int.TryParse(Request.Params("TipoMovimentoUsoOutrasFora"), out codigoTipoMovimentoUsoOutrasFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoOutrasFora"), out codigoTipoMovimentoRevesaoOutrasFora);
            int.TryParse(Request.Params("TipoMovimentoUsoDescontoFora"), out codigoTipoMovimentoUsoDescontoFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoDescontoFora"), out codigoTipoMovimentoReversaoDescontoFora);
            int.TryParse(Request.Params("TipoMovimentoUsoImpostoFora"), out codigoTipoMovimentoUsoImpostoFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoImpostoFora"), out codigoTipoMovimentoReversaoImpostoFora);
            int.TryParse(Request.Params("TipoMovimentoUsoDiferencialFreteFora"), out codigoTipoMovimentoUsoDiferencialFreteFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoDiferencialFreteFora"), out codigoTipoMovimentoReversaoDiferencialFreteFora);
            int.TryParse(Request.Params("TipoMovimentoUsoICMSFreteFora"), out codigoTipoMovimentoUsoICMSFreteFora);
            int.TryParse(Request.Params("TipoMovimentoReversaoICMSFreteFora"), out codigoTipoMovimentoReversaoICMSFreteFota);
            int.TryParse(Request.Params("TipoMovimentoUsoCusto"), out codigoTipoMovimentoUsoCusto);
            int.TryParse(Request.Params("TipoMovimentoReversaoCusto"), out codigoTipoMovimentoReversaoCusto);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoPIS"), out codigoTipoMovimentoUsoRetencaoPIS);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoPIS"), out codigoTipoMovimentoReversaoRetencaoPIS);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoCOFINS"), out codigoTipoMovimentoUsoRetencaoCOFINS);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoCOFINS"), out codigoTipoMovimentoReversaoRetencaoCOFINS);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoIPI"), out codigoTipoMovimentoUsoRetencaoIPI);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoIPI"), out codigoTipoMovimentoReversaoRetencaoIPI);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoINSS"), out codigoTipoMovimentoUsoRetencaoINSS);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoINSS"), out codigoTipoMovimentoReversaoRetencaoINSS);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoCSLL"), out codigoTipoMovimentoUsoRetencaoCSLL);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoCSLL"), out codigoTipoMovimentoReversaoRetencaoCSLL);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoOutras"), out codigoTipoMovimentoUsoRetencaoOutras);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoOutras"), out codigoTipoMovimentoReversaoRetencaoOutras);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoISS"), out codigoTipoMovimentoUsoRetencaoISS);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoISS"), out codigoTipoMovimentoReversaoRetencaoISS);
            int.TryParse(Request.Params("TipoMovimentoUsoRetencaoIR"), out codigoTipoMovimentoUsoRetencaoIR);
            int.TryParse(Request.Params("TipoMovimentoReversaoRetencaoIR"), out codigoTipoMovimentoReversaoRetencaoIR);

            bool geraEstoque, gerarMovimentoAutomatico, gerarMovimentoAutomaticoDesconto, gerarMovimentoAutomaticoOutrasDespesas, gerarMovimentoAutomaticoFrete, gerarMovimentoAutomaticoICMS, gerarMovimentoAutomaticoPIS, gerarMovimentoAutomaticoCOFINS, gerarMovimentoAutomaticoIPI, gerarMovimentoAutomaticoICMSST, gerarMovimentoAutomaticoDiferencial,
                 geraMovimentoSeguro, geraMovimentoFreteFora, geraMovimentoOutrasFora, geraMovimentoDescontoFora, geraMovimentoImpostoFora, geraMovimentoDiferencialFreteFora, geraMovimentoICMSFreteFora, geraMovimentoCusto,
                 gerarMovimentoAutomaticoRetencaoPIS, gerarMovimentoAutomaticoRetencaoCOFINS, gerarMovimentoAutomaticoRetencaoINSS, gerarMovimentoAutomaticoRetencaoIPI, gerarMovimentoAutomaticoRetencaoCSLL, gerarMovimentoAutomaticoRetencaoOutras,
                 gerarMovimentoAutomaticoRetencaoISS, gerarMovimentoAutomaticoRetencaoIR;
            bool.TryParse(Request.Params("GeraEstoque"), out geraEstoque);
            bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoDesconto"), out gerarMovimentoAutomaticoDesconto);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoOutrasDespesas"), out gerarMovimentoAutomaticoOutrasDespesas);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoFrete"), out gerarMovimentoAutomaticoFrete);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoICMS"), out gerarMovimentoAutomaticoICMS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoPIS"), out gerarMovimentoAutomaticoPIS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoCOFINS"), out gerarMovimentoAutomaticoCOFINS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoIPI"), out gerarMovimentoAutomaticoIPI);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoICMSST"), out gerarMovimentoAutomaticoICMSST);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoDiferencial"), out gerarMovimentoAutomaticoDiferencial);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoSeguro"), out geraMovimentoSeguro);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoFreteFora"), out geraMovimentoFreteFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoOutrasFora"), out geraMovimentoOutrasFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoDescontoFora"), out geraMovimentoDescontoFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoImpostoFora"), out geraMovimentoImpostoFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoDiferencialFreteFora"), out geraMovimentoDiferencialFreteFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoICMSFreteFora"), out geraMovimentoICMSFreteFora);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoCusto"), out geraMovimentoCusto);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoPIS"), out gerarMovimentoAutomaticoRetencaoPIS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoCOFINS"), out gerarMovimentoAutomaticoRetencaoCOFINS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoINSS"), out gerarMovimentoAutomaticoRetencaoINSS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoIPI"), out gerarMovimentoAutomaticoRetencaoIPI);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoCSLL"), out gerarMovimentoAutomaticoRetencaoCSLL);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoOutras"), out gerarMovimentoAutomaticoRetencaoOutras);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoISS"), out gerarMovimentoAutomaticoRetencaoISS);
            bool.TryParse(Request.Params("GerarMovimentoAutomaticoRetencaoIR"), out gerarMovimentoAutomaticoRetencaoIR);

            Dominio.Enumeradores.TipoCFOP tipoCFOP;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS cstICMS;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI cstIPI;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS cstPIS;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS cstCOFINS;

            decimal reducaoBCIPI, aliquotaIPI, reducaoBCPIS, aliquotaPIS, reducaoBCCOFINS, aliquotaCOFINS, aliquotaInterna, aliquotaInterestadual, mva, reducaoMVA, aliquotaDiferencial;

            string descricao = Request.Params("Descricao");
            string status = Request.Params("Status");
            string extensao = Request.Params("Extensao");

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                empresa = this.Usuario.Empresa.Codigo;

            Enum.TryParse(Request.Params("Tipo"), out tipoCFOP);
            Enum.TryParse(Request.Params("CSTICMS"), out cstICMS);
            Enum.TryParse(Request.Params("CSTIPI"), out cstIPI);
            Enum.TryParse(Request.Params("CSTPIS"), out cstPIS);
            Enum.TryParse(Request.Params("CSTCOFINS"), out cstCOFINS);

            decimal.TryParse(Request.Params("ReducaoBCIPI"), out reducaoBCIPI);
            decimal.TryParse(Request.Params("AliquotaIPI"), out aliquotaIPI);
            decimal.TryParse(Request.Params("ReducaoBCPIS"), out reducaoBCPIS);
            decimal.TryParse(Request.Params("AliquotaPIS"), out aliquotaPIS);
            decimal.TryParse(Request.Params("ReducaoBCCOFINS"), out reducaoBCCOFINS);
            decimal.TryParse(Request.Params("AliquotaCOFINS"), out aliquotaCOFINS);
            decimal.TryParse(Request.Params("AliquotaInterna"), out aliquotaInterna);
            decimal.TryParse(Request.Params("AliquotaInterestadual"), out aliquotaInterestadual);
            decimal.TryParse(Request.Params("MVA"), out mva);
            decimal.TryParse(Request.Params("ReducaoMVA"), out reducaoMVA);
            decimal.TryParse(Request.Params("AliquotaDiferencial"), out aliquotaDiferencial);

            if (cfop.Codigo == 0)
            {
                cfop.CodigoCFOP = codigoCFOP;
                cfop.Tipo = tipoCFOP;
            }

            cfop.AliquotaDiferencial = aliquotaDiferencial;
            cfop.AliquotaCOFINS = aliquotaCOFINS;
            cfop.AliquotaICMSInterestadual = aliquotaInterestadual;
            cfop.AliquotaICMSInterna = aliquotaInterna;
            cfop.AliquotaIPI = aliquotaIPI;
            cfop.AliquotaPIS = aliquotaPIS;
            cfop.Extensao = extensao;

            if ((int)cstCOFINS > 0)
                cfop.CSTCOFINS = cstCOFINS;
            else
                cfop.CSTCOFINS = null;

            if ((int)cstICMS > 0)
                cfop.CSTICMS = cstICMS;
            else
                cfop.CSTICMS = null;

            if ((int)cstIPI > 0)
                cfop.CSTIPI = cstIPI;
            else
                cfop.CSTIPI = null;

            if ((int)cstPIS > 0)
                cfop.CSTPIS = cstPIS;
            else
                cfop.CSTPIS = null;

            cfop.Descricao = descricao;

            if (empresa > 0)
                cfop.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;
            else
                cfop.Empresa = null;

            cfop.RealizarRateioDespesaVeiculo = Request.GetBoolParam("RealizarRateioDespesaVeiculo");
            cfop.RealizarRateioSomenteQuandoTiverOS = Request.GetBoolParam("RealizarRateioSomenteQuandoTiverOS");
            cfop.IrrelevanteParaNaoConformidade = Request.GetBoolParam("IrrelevanteParaNaoConformidade");
            cfop.GeraEstoque = geraEstoque;
            cfop.MVA = mva;
            cfop.ReducaoBCCOFINS = reducaoBCCOFINS;
            cfop.ReducaoBCIPI = reducaoBCIPI;
            cfop.ReducaoBCPIS = reducaoBCPIS;
            cfop.ReducaoMVA = reducaoMVA;
            cfop.Status = status;
            cfop.GrupoPrioridade = grupoPrioridade;
            cfop.BloqueioDocumentoEntrada = Request.GetEnumParam<BloqueioDocumentoEntrada>("BloqueioDocumentoEntrada");
            cfop.AliquotaRetencaoPIS = Request.GetDecimalParam("AliquotaRetencaoPIS");
            cfop.AliquotaRetencaoCOFINS = Request.GetDecimalParam("AliquotaRetencaoCOFINS");
            cfop.AliquotaRetencaoINSS = Request.GetDecimalParam("AliquotaRetencaoINSS");
            cfop.AliquotaRetencaoIPI = Request.GetDecimalParam("AliquotaRetencaoIPI");
            cfop.AliquotaRetencaoCSLL = Request.GetDecimalParam("AliquotaRetencaoCSLL");
            cfop.AliquotaRetencaoOutras = Request.GetDecimalParam("AliquotaRetencaoOutras");
            cfop.AliquotaRetencaoIR = Request.GetDecimalParam("AliquotaRetencaoIR");
            cfop.AliquotaRetencaoISS = Request.GetDecimalParam("AliquotaRetencaoISS");
            cfop.AliquotaParaCredito = Request.GetDecimalParam("AliquotaParaCredito");
            cfop.ObrigarVincularAbastecimentoAoItemDocumentoEntrada = Request.GetBoolParam("ObrigarVincularAbastecimentoAoItemDocumentoEntrada");
            cfop.ObrigarInformarLocalArmazenamento = Request.GetBoolParam("ObrigarInformarLocalArmazenamento");
            cfop.CreditoSobreTotalParaItensSujeitosICMSST = Request.GetBoolParam("CreditoSobreTotalParaItensSujeitosICMSST");
            cfop.CreditoSobreTotalParaProdutosUsoConsumo = Request.GetBoolParam("CreditoSobreTotalParaProdutosUsoConsumo");
            cfop.NaturezaDaOperacao = codigoNaturezaOperacao > 0 ? repNaturezaOperacao.BuscarPorCodigo(codigoNaturezaOperacao, false) : null;

            cfop.TipoMovimentoReversao = codigoTipoMovimentoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao) : null;
            cfop.TipoMovimentoReversaoCOFINS = codigoTipoMovimentoReversaoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoCOFINS) : null;
            cfop.TipoMovimentoReversaoDesconto = codigoTipoMovimentoReversaoDesconto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoDesconto) : null;
            cfop.TipoMovimentoReversaoDiferencial = codigoTipoMovimentoReversaoDiferencial > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoDiferencial) : null;
            cfop.TipoMovimentoReversaoFrete = codigoTipoMovimentoReversaoFrete > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoFrete) : null;
            cfop.TipoMovimentoReversaoICMS = codigoTipoMovimentoReversaoICMS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoICMS) : null;
            cfop.TipoMovimentoReversaoICMSST = codigoTipoMovimentoReversaoICMSST > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoICMSST) : null;
            cfop.TipoMovimentoReversaoIPI = codigoTipoMovimentoReversaoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoIPI) : null;
            cfop.TipoMovimentoReversaoOutrasDespesas = codigoTipoMovimentoReversaoOutrasDespesas > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoOutrasDespesas) : null;
            cfop.TipoMovimentoReversaoPIS = codigoTipoMovimentoReversaoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoPIS) : null;

            cfop.TipoMovimentoUso = codigoTipoMovimentoUso > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUso) : null;
            cfop.TipoMovimentoUsoCOFINS = codigoTipoMovimentoUsoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoCOFINS) : null;
            cfop.TipoMovimentoUsoDesconto = codigoTipoMovimentoUsoDesconto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoDesconto) : null;
            cfop.TipoMovimentoUsoDiferencial = codigoTipoMovimentoUsoDiferencial > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoDiferencial) : null;
            cfop.TipoMovimentoUsoFrete = codigoTipoMovimentoUsoFrete > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoFrete) : null;
            cfop.TipoMovimentoUsoICMS = codigoTipoMovimentoUsoICMS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoICMS) : null;
            cfop.TipoMovimentoUsoICMSST = codigoTipoMovimentoUsoICMSST > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoICMSST) : null;
            cfop.TipoMovimentoUsoIPI = codigoTipoMovimentoUsoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoIPI) : null;
            cfop.TipoMovimentoUsoOutrasDespesas = codigoTipoMovimentoUsoOutrasDespesas > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoOutrasDespesas) : null;
            cfop.TipoMovimentoUsoPIS = codigoTipoMovimentoUsoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoPIS) : null;

            cfop.TipoMovimentoUsoSeguro = codigoTipoMovimentoUsoSeguro > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoSeguro) : null;
            cfop.TipoMovimentoReversaoSeguro = codigoTipoMovimentoReversaoSeguro > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoSeguro) : null;
            cfop.TipoMovimentoUsoFreteFora = codigoTipoMovimentoUsoFreteFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoFreteFora) : null;
            cfop.TipoMovimentoReversaoFreteFora = codigoTipoMovimentoReversaoFreteFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoFreteFora) : null;
            cfop.TipoMovimentoUsoOutrasFora = codigoTipoMovimentoUsoOutrasFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoOutrasFora) : null;
            cfop.TipoMovimentoReversaoOutrasFora = codigoTipoMovimentoRevesaoOutrasFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoRevesaoOutrasFora) : null;
            cfop.TipoMovimentoUsoDescontoFora = codigoTipoMovimentoUsoDescontoFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoDescontoFora) : null;
            cfop.TipoMovimentoReversaoDescontoFora = codigoTipoMovimentoReversaoDescontoFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoDescontoFora) : null;
            cfop.TipoMovimentoUsoImpostoFora = codigoTipoMovimentoUsoImpostoFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoImpostoFora) : null;
            cfop.TipoMovimentoReversaoImpostoFora = codigoTipoMovimentoReversaoImpostoFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoImpostoFora) : null;
            cfop.TipoMovimentoUsoDiferencialFreteFora = codigoTipoMovimentoUsoDiferencialFreteFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoDiferencialFreteFora) : null;
            cfop.TipoMovimentoReversaoDiferencialFreteFora = codigoTipoMovimentoReversaoDiferencialFreteFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoDiferencialFreteFora) : null;
            cfop.TipoMovimentoUsoICMSFreteFora = codigoTipoMovimentoUsoICMSFreteFora > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoICMSFreteFora) : null;
            cfop.TipoMovimentoReversaoICMSFreteFora = codigoTipoMovimentoReversaoICMSFreteFota > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoICMSFreteFota) : null;
            cfop.TipoMovimentoUsoCusto = codigoTipoMovimentoUsoCusto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoCusto) : null;
            cfop.TipoMovimentoReversaoCusto = codigoTipoMovimentoReversaoCusto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoCusto) : null;

            cfop.TipoMovimentoUsoRetencaoCOFINS = codigoTipoMovimentoUsoRetencaoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoCOFINS) : null;
            cfop.TipoMovimentoUsoRetencaoCSLL = codigoTipoMovimentoUsoRetencaoCSLL > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoCSLL) : null;
            cfop.TipoMovimentoUsoRetencaoINSS = codigoTipoMovimentoUsoRetencaoINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoINSS) : null;
            cfop.TipoMovimentoUsoRetencaoIPI = codigoTipoMovimentoUsoRetencaoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoIPI) : null;
            cfop.TipoMovimentoUsoRetencaoOutras = codigoTipoMovimentoUsoRetencaoOutras > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoOutras) : null;
            cfop.TipoMovimentoUsoRetencaoPIS = codigoTipoMovimentoUsoRetencaoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoPIS) : null;
            cfop.TipoMovimentoReversaoRetencaoCOFINS = codigoTipoMovimentoReversaoRetencaoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoCOFINS) : null;
            cfop.TipoMovimentoReversaoRetencaoCSLL = codigoTipoMovimentoReversaoRetencaoCSLL > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoCSLL) : null;
            cfop.TipoMovimentoReversaoRetencaoINSS = codigoTipoMovimentoReversaoRetencaoINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoINSS) : null;
            cfop.TipoMovimentoReversaoRetencaoIPI = codigoTipoMovimentoReversaoRetencaoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoIPI) : null;
            cfop.TipoMovimentoReversaoRetencaoOutras = codigoTipoMovimentoReversaoRetencaoOutras > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoOutras) : null;
            cfop.TipoMovimentoReversaoRetencaoPIS = codigoTipoMovimentoReversaoRetencaoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoPIS) : null;
            cfop.TipoMovimentoUsoRetencaoISS = codigoTipoMovimentoUsoRetencaoISS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoISS) : null;
            cfop.TipoMovimentoReversaoRetencaoISS = codigoTipoMovimentoReversaoRetencaoISS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoISS) : null;
            cfop.TipoMovimentoUsoRetencaoIR = codigoTipoMovimentoUsoRetencaoIR > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoRetencaoIR) : null;
            cfop.TipoMovimentoReversaoRetencaoIR = codigoTipoMovimentoReversaoRetencaoIR > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoRetencaoIR) : null;

            cfop.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
            cfop.GerarMovimentoAutomaticoCOFINS = gerarMovimentoAutomaticoCOFINS;
            cfop.GerarMovimentoAutomaticoDiferencial = gerarMovimentoAutomaticoDiferencial;
            cfop.GerarMovimentoAutomaticoDesconto = gerarMovimentoAutomaticoDesconto;
            cfop.GerarMovimentoAutomaticoFrete = gerarMovimentoAutomaticoFrete;
            cfop.GerarMovimentoAutomaticoICMS = gerarMovimentoAutomaticoICMS;
            cfop.GerarMovimentoAutomaticoICMSST = gerarMovimentoAutomaticoICMSST;
            cfop.GerarMovimentoAutomaticoIPI = gerarMovimentoAutomaticoIPI;
            cfop.GerarMovimentoAutomaticoOutrasDespesas = gerarMovimentoAutomaticoOutrasDespesas;
            cfop.GerarMovimentoAutomaticoPIS = gerarMovimentoAutomaticoPIS;

            cfop.GerarMovimentoAutomaticoSeguro = geraMovimentoSeguro;
            cfop.GerarMovimentoAutomaticoFreteFora = geraMovimentoFreteFora;
            cfop.GerarMovimentoAutomaticoOutrasFora = geraMovimentoOutrasFora;
            cfop.GerarMovimentoAutomaticoDescontoFora = geraMovimentoDescontoFora;
            cfop.GerarMovimentoAutomaticoImpostoFora = geraMovimentoImpostoFora;
            cfop.GerarMovimentoAutomaticoDiferencialFreteFora = geraMovimentoDiferencialFreteFora;
            cfop.GerarMovimentoAutomaticoICMSFreteFora = geraMovimentoICMSFreteFora;
            cfop.GerarMovimentoAutomaticoCusto = geraMovimentoCusto;

            cfop.GerarMovimentoAutomaticoRetencaoCOFINS = gerarMovimentoAutomaticoRetencaoCOFINS;
            cfop.GerarMovimentoAutomaticoRetencaoCSLL = gerarMovimentoAutomaticoRetencaoCSLL;
            cfop.GerarMovimentoAutomaticoRetencaoINSS = gerarMovimentoAutomaticoRetencaoINSS;
            cfop.GerarMovimentoAutomaticoRetencaoIPI = gerarMovimentoAutomaticoRetencaoIPI;
            cfop.GerarMovimentoAutomaticoRetencaoOutras = gerarMovimentoAutomaticoRetencaoOutras;
            cfop.GerarMovimentoAutomaticoRetencaoPIS = gerarMovimentoAutomaticoRetencaoPIS;
            cfop.GerarMovimentoAutomaticoRetencaoISS = gerarMovimentoAutomaticoRetencaoISS;
            cfop.GerarMovimentoAutomaticoRetencaoIR = gerarMovimentoAutomaticoRetencaoIR;

            CamposRetencao(ref cfop, unitOfWork);

            if (gerarMovimentoAutomatico)
            {
                if (cfop.TipoMovimentoUso == null)
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso para a CFOP.");

                if (gerarMovimentoAutomaticoCOFINS && (cfop.TipoMovimentoReversaoCOFINS == null || cfop.TipoMovimentoUsoCOFINS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a COFINS.");

                if (gerarMovimentoAutomaticoDesconto && (cfop.TipoMovimentoReversaoDesconto == null || cfop.TipoMovimentoUsoDesconto == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o desconto.");

                if (gerarMovimentoAutomaticoDiferencial && (cfop.TipoMovimentoReversaoDiferencial == null || cfop.TipoMovimentoUsoDiferencial == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o diferencial.");

                if (gerarMovimentoAutomaticoFrete && (cfop.TipoMovimentoReversaoFrete == null || cfop.TipoMovimentoUsoFrete == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o frete.");

                if (gerarMovimentoAutomaticoICMS && (cfop.TipoMovimentoReversaoICMS == null || cfop.TipoMovimentoUsoICMS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o ICMS.");

                if (gerarMovimentoAutomaticoICMSST && (cfop.TipoMovimentoReversaoICMSST == null || cfop.TipoMovimentoUsoICMSST == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o ICMS ST.");

                if (gerarMovimentoAutomaticoIPI && (cfop.TipoMovimentoReversaoIPI == null || cfop.TipoMovimentoUsoIPI == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o IPI.");

                if (gerarMovimentoAutomaticoOutrasDespesas && (cfop.TipoMovimentoReversaoOutrasDespesas == null || cfop.TipoMovimentoUsoOutrasDespesas == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para outras despesas.");

                if (gerarMovimentoAutomaticoPIS && (cfop.TipoMovimentoReversaoPIS == null || cfop.TipoMovimentoUsoPIS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o PIS.");

                if (geraMovimentoSeguro && (cfop.TipoMovimentoReversaoSeguro == null || cfop.TipoMovimentoUsoSeguro == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Seguro.");

                if (geraMovimentoFreteFora && (cfop.TipoMovimentoReversaoFreteFora == null || cfop.TipoMovimentoUsoFreteFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Frete Fora.");

                if (geraMovimentoOutrasFora && (cfop.TipoMovimentoReversaoOutrasFora == null || cfop.TipoMovimentoUsoOutrasFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a Outras Despesas Fora.");

                if (geraMovimentoDescontoFora && (cfop.TipoMovimentoReversaoDescontoFora == null || cfop.TipoMovimentoUsoDescontoFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Desconto Fora.");

                if (geraMovimentoImpostoFora && (cfop.TipoMovimentoReversaoImpostoFora == null || cfop.TipoMovimentoUsoImpostoFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Imposto Fora.");

                if (geraMovimentoDiferencialFreteFora && (cfop.TipoMovimentoReversaoDiferencialFreteFora == null || cfop.TipoMovimentoUsoDiferencialFreteFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Diferencial do Frete Fora.");

                if (geraMovimentoICMSFreteFora && (cfop.TipoMovimentoReversaoICMSFreteFora == null || cfop.TipoMovimentoUsoICMSFreteFora == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o ICMS do Frete Fora.");

                if (geraMovimentoCusto && (cfop.TipoMovimentoReversaoCusto == null || cfop.TipoMovimentoUsoCusto == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para o Custo.");

                if (gerarMovimentoAutomaticoRetencaoCOFINS && (cfop.TipoMovimentoUsoRetencaoCOFINS == null || cfop.TipoMovimentoReversaoRetencaoCOFINS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção da COFINS.");

                if (gerarMovimentoAutomaticoRetencaoCSLL && (cfop.TipoMovimentoUsoRetencaoCSLL == null || cfop.TipoMovimentoReversaoRetencaoCSLL == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção da CSLL.");

                if (gerarMovimentoAutomaticoRetencaoINSS && (cfop.TipoMovimentoUsoRetencaoINSS == null || cfop.TipoMovimentoReversaoRetencaoINSS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção do INSS.");

                if (gerarMovimentoAutomaticoRetencaoIPI && (cfop.TipoMovimentoUsoRetencaoIPI == null || cfop.TipoMovimentoReversaoRetencaoIPI == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção do IPI.");

                if (gerarMovimentoAutomaticoRetencaoOutras && (cfop.TipoMovimentoUsoRetencaoOutras == null || cfop.TipoMovimentoReversaoRetencaoOutras == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para outras retenções.");

                if (gerarMovimentoAutomaticoRetencaoPIS && (cfop.TipoMovimentoUsoRetencaoPIS == null || cfop.TipoMovimentoReversaoRetencaoPIS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção do PIS.");

                if (gerarMovimentoAutomaticoRetencaoISS && (cfop.TipoMovimentoUsoRetencaoISS == null || cfop.TipoMovimentoReversaoRetencaoISS == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção do ISS.");

                if (gerarMovimentoAutomaticoRetencaoIR && (cfop.TipoMovimentoUsoRetencaoIR == null || cfop.TipoMovimentoReversaoRetencaoIR == null))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para a retenção do IR.");
            }
        }

        private void CamposRetencao(ref Dominio.Entidades.CFOP cfop, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            //PIS
            bool reduzValorLiquidoRetencaoPIS, gerarGuiaPagarRetencaoPIS, calcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoPIS"), out reduzValorLiquidoRetencaoPIS);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoPIS"), out gerarGuiaPagarRetencaoPIS);
            cfop.ReduzValorLiquidoRetencaoPIS = reduzValorLiquidoRetencaoPIS;
            cfop.GerarGuiaPagarRetencaoPIS = gerarGuiaPagarRetencaoPIS;

            int codigoTipoMovimentoUsoTituloRetencaoPIS, codigoTipoMovimentoReversaoTituloRetencaoPIS;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoPIS"), out codigoTipoMovimentoUsoTituloRetencaoPIS);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoPIS"), out codigoTipoMovimentoReversaoTituloRetencaoPIS);
            cfop.TipoMovimentoUsoTituloRetencaoPIS = codigoTipoMovimentoUsoTituloRetencaoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoPIS) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoPIS = codigoTipoMovimentoReversaoTituloRetencaoPIS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoPIS) : null;

            double cnpjFornecedorRetencaoPIS;
            double.TryParse(Request.Params("FornecedorRetencaoPIS"), out cnpjFornecedorRetencaoPIS);
            cfop.FornecedorRetencaoPIS = cnpjFornecedorRetencaoPIS > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoPIS) : null;

            int diaGerencaoRetencaoPIS;
            int.TryParse(Request.Params("DiaGerencaoRetencaoPIS"), out diaGerencaoRetencaoPIS);
            cfop.DiaGerencaoRetencaoPIS = diaGerencaoRetencaoPIS;

            calcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS = Request.GetBoolParam("calcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS");
            cfop.CalcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS = calcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS;

            //COFINS
            bool reduzValorLiquidoRetencaoCOFINS, gerarGuiaPagarRetencaoCOFINS;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoCOFINS"), out reduzValorLiquidoRetencaoCOFINS);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoCOFINS"), out gerarGuiaPagarRetencaoCOFINS);
            cfop.ReduzValorLiquidoRetencaoCOFINS = reduzValorLiquidoRetencaoCOFINS;
            cfop.GerarGuiaPagarRetencaoCOFINS = gerarGuiaPagarRetencaoCOFINS;

            int codigoTipoMovimentoUsoTituloRetencaoCOFINS, codigoTipoMovimentoReversaoTituloRetencaoCOFINS;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoCOFINS"), out codigoTipoMovimentoUsoTituloRetencaoCOFINS);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoCOFINS"), out codigoTipoMovimentoReversaoTituloRetencaoCOFINS);
            cfop.TipoMovimentoUsoTituloRetencaoCOFINS = codigoTipoMovimentoUsoTituloRetencaoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoCOFINS) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoCOFINS = codigoTipoMovimentoReversaoTituloRetencaoCOFINS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoCOFINS) : null;

            double cnpjFornecedorRetencaoCOFINS;
            double.TryParse(Request.Params("FornecedorRetencaoCOFINS"), out cnpjFornecedorRetencaoCOFINS);
            cfop.FornecedorRetencaoCOFINS = cnpjFornecedorRetencaoCOFINS > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoCOFINS) : null;

            int diaGerencaoRetencaoCOFINS;
            int.TryParse(Request.Params("DiaGerencaoRetencaoCOFINS"), out diaGerencaoRetencaoCOFINS);
            cfop.DiaGerencaoRetencaoCOFINS = diaGerencaoRetencaoCOFINS;

            //INSS
            bool reduzValorLiquidoRetencaoINSS, gerarGuiaPagarRetencaoINSS;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoINSS"), out reduzValorLiquidoRetencaoINSS);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoINSS"), out gerarGuiaPagarRetencaoINSS);
            cfop.ReduzValorLiquidoRetencaoINSS = reduzValorLiquidoRetencaoINSS;
            cfop.GerarGuiaPagarRetencaoINSS = gerarGuiaPagarRetencaoINSS;

            int codigoTipoMovimentoUsoTituloRetencaoINSS, codigoTipoMovimentoReversaoTituloRetencaoINSS;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoINSS"), out codigoTipoMovimentoUsoTituloRetencaoINSS);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoINSS"), out codigoTipoMovimentoReversaoTituloRetencaoINSS);
            cfop.TipoMovimentoUsoTituloRetencaoINSS = codigoTipoMovimentoUsoTituloRetencaoINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoINSS) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoINSS = codigoTipoMovimentoReversaoTituloRetencaoINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoINSS) : null;

            double cnpjFornecedorRetencaoINSS;
            double.TryParse(Request.Params("FornecedorRetencaoINSS"), out cnpjFornecedorRetencaoINSS);
            cfop.FornecedorRetencaoINSS = cnpjFornecedorRetencaoINSS > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoINSS) : null;

            int diaGerencaoRetencaoINSS;
            int.TryParse(Request.Params("DiaGerencaoRetencaoINSS"), out diaGerencaoRetencaoINSS);
            cfop.DiaGerencaoRetencaoINSS = diaGerencaoRetencaoINSS;

            //CSLL
            bool reduzValorLiquidoRetencaoCSLL, gerarGuiaPagarRetencaoCSLL;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoCSLL"), out reduzValorLiquidoRetencaoCSLL);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoCSLL"), out gerarGuiaPagarRetencaoCSLL);
            cfop.ReduzValorLiquidoRetencaoCSLL = reduzValorLiquidoRetencaoCSLL;
            cfop.GerarGuiaPagarRetencaoCSLL = gerarGuiaPagarRetencaoCSLL;

            int codigoTipoMovimentoUsoTituloRetencaoCSLL, codigoTipoMovimentoReversaoTituloRetencaoCSLL;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoCSLL"), out codigoTipoMovimentoUsoTituloRetencaoCSLL);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoCSLL"), out codigoTipoMovimentoReversaoTituloRetencaoCSLL);
            cfop.TipoMovimentoUsoTituloRetencaoCSLL = codigoTipoMovimentoUsoTituloRetencaoCSLL > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoCSLL) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoCSLL = codigoTipoMovimentoReversaoTituloRetencaoCSLL > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoCSLL) : null;

            double cnpjFornecedorRetencaoCSLL;
            double.TryParse(Request.Params("FornecedorRetencaoCSLL"), out cnpjFornecedorRetencaoCSLL);
            cfop.FornecedorRetencaoCSLL = cnpjFornecedorRetencaoCSLL > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoCSLL) : null;

            int diaGerencaoRetencaoCSLL;
            int.TryParse(Request.Params("DiaGerencaoRetencaoCSLL"), out diaGerencaoRetencaoCSLL);
            cfop.DiaGerencaoRetencaoCSLL = diaGerencaoRetencaoCSLL;

            //IPI
            bool reduzValorLiquidoRetencaoIPI, gerarGuiaPagarRetencaoIPI;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoIPI"), out reduzValorLiquidoRetencaoIPI);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoIPI"), out gerarGuiaPagarRetencaoIPI);
            cfop.ReduzValorLiquidoRetencaoIPI = reduzValorLiquidoRetencaoIPI;
            cfop.GerarGuiaPagarRetencaoIPI = gerarGuiaPagarRetencaoIPI;

            int codigoTipoMovimentoUsoTituloRetencaoIPI, codigoTipoMovimentoReversaoTituloRetencaoIPI;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoIPI"), out codigoTipoMovimentoUsoTituloRetencaoIPI);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoIPI"), out codigoTipoMovimentoReversaoTituloRetencaoIPI);
            cfop.TipoMovimentoUsoTituloRetencaoIPI = codigoTipoMovimentoUsoTituloRetencaoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoIPI) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoIPI = codigoTipoMovimentoReversaoTituloRetencaoIPI > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoIPI) : null;

            double cnpjFornecedorRetencaoIPI;
            double.TryParse(Request.Params("FornecedorRetencaoIPI"), out cnpjFornecedorRetencaoIPI);
            cfop.FornecedorRetencaoIPI = cnpjFornecedorRetencaoIPI > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoIPI) : null;

            int diaGerencaoRetencaoIPI;
            int.TryParse(Request.Params("DiaGerencaoRetencaoIPI"), out diaGerencaoRetencaoIPI);
            cfop.DiaGerencaoRetencaoIPI = diaGerencaoRetencaoIPI;

            //OUTRAS
            bool reduzValorLiquidoRetencaoOutras, gerarGuiaPagarRetencaoOutras;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoOutras"), out reduzValorLiquidoRetencaoOutras);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoOutras"), out gerarGuiaPagarRetencaoOutras);
            cfop.ReduzValorLiquidoRetencaoOutras = reduzValorLiquidoRetencaoOutras;
            cfop.GerarGuiaPagarRetencaoOutras = gerarGuiaPagarRetencaoOutras;

            int codigoTipoMovimentoUsoTituloRetencaoOutras, codigoTipoMovimentoReversaoTituloRetencaoOutras;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoOutras"), out codigoTipoMovimentoUsoTituloRetencaoOutras);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoOutras"), out codigoTipoMovimentoReversaoTituloRetencaoOutras);
            cfop.TipoMovimentoUsoTituloRetencaoOutras = codigoTipoMovimentoUsoTituloRetencaoOutras > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoOutras) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoOutras = codigoTipoMovimentoReversaoTituloRetencaoOutras > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoOutras) : null;

            double cnpjFornecedorRetencaoOutras;
            double.TryParse(Request.Params("FornecedorRetencaoOutras"), out cnpjFornecedorRetencaoOutras);
            cfop.FornecedorRetencaoOutras = cnpjFornecedorRetencaoOutras > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoOutras) : null;

            int diaGerencaoRetencaoOutras;
            int.TryParse(Request.Params("DiaGerencaoRetencaoOutras"), out diaGerencaoRetencaoOutras);
            cfop.DiaGerencaoRetencaoOutras = diaGerencaoRetencaoOutras;

            //ISS
            bool reduzValorLiquidoRetencaoISS, gerarGuiaPagarRetencaoISS;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoISS"), out reduzValorLiquidoRetencaoISS);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoISS"), out gerarGuiaPagarRetencaoISS);
            cfop.ReduzValorLiquidoRetencaoISS = reduzValorLiquidoRetencaoISS;
            cfop.GerarGuiaPagarRetencaoISS = gerarGuiaPagarRetencaoISS;

            int codigoTipoMovimentoUsoTituloRetencaoISS, codigoTipoMovimentoReversaoTituloRetencaoISS;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoISS"), out codigoTipoMovimentoUsoTituloRetencaoISS);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoISS"), out codigoTipoMovimentoReversaoTituloRetencaoISS);
            cfop.TipoMovimentoUsoTituloRetencaoISS = codigoTipoMovimentoUsoTituloRetencaoISS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoISS) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoISS = codigoTipoMovimentoReversaoTituloRetencaoISS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoISS) : null;

            double cnpjFornecedorRetencaoISS;
            double.TryParse(Request.Params("FornecedorRetencaoISS"), out cnpjFornecedorRetencaoISS);
            cfop.FornecedorRetencaoISS = cnpjFornecedorRetencaoISS > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoISS) : null;

            int diaGerencaoRetencaoISS;
            int.TryParse(Request.Params("DiaGerencaoRetencaoISS"), out diaGerencaoRetencaoISS);
            cfop.DiaGerencaoRetencaoISS = diaGerencaoRetencaoISS;

            //IR
            bool reduzValorLiquidoRetencaoIR, gerarGuiaPagarRetencaoIR;
            bool.TryParse(Request.Params("ReduzValorLiquidoRetencaoIR"), out reduzValorLiquidoRetencaoIR);
            bool.TryParse(Request.Params("GerarGuiaPagarRetencaoIR"), out gerarGuiaPagarRetencaoIR);
            cfop.ReduzValorLiquidoRetencaoIR = reduzValorLiquidoRetencaoIR;
            cfop.GerarGuiaPagarRetencaoIR = gerarGuiaPagarRetencaoIR;

            int codigoTipoMovimentoUsoTituloRetencaoIR, codigoTipoMovimentoReversaoTituloRetencaoIR;
            int.TryParse(Request.Params("TipoMovimentoUsoTituloRetencaoIR"), out codigoTipoMovimentoUsoTituloRetencaoIR);
            int.TryParse(Request.Params("TipoMovimentoReversaoTituloRetencaoIR"), out codigoTipoMovimentoReversaoTituloRetencaoIR);
            cfop.TipoMovimentoUsoTituloRetencaoIR = codigoTipoMovimentoUsoTituloRetencaoIR > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoTituloRetencaoIR) : null;
            cfop.TipoMovimentoReversaoTituloRetencaoIR = codigoTipoMovimentoReversaoTituloRetencaoIR > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTituloRetencaoIR) : null;

            double cnpjFornecedorRetencaoIR;
            double.TryParse(Request.Params("FornecedorRetencaoIR"), out cnpjFornecedorRetencaoIR);
            cfop.FornecedorRetencaoIR = cnpjFornecedorRetencaoIR > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedorRetencaoIR) : null;

            int diaGerencaoRetencaoIR;
            int.TryParse(Request.Params("DiaGerencaoRetencaoIR"), out diaGerencaoRetencaoIR);
            cfop.DiaGerencaoRetencaoIR = diaGerencaoRetencaoIR;
        }

        private void VerificarDuplicidade(Repositorio.CFOP repCFOP, Dominio.Entidades.CFOP cfop)
        {
            string msg;
            if (repCFOP.ExisteDuplicado(cfop.CodigoCFOP, cfop.Descricao, cfop.Codigo, cfop.GrupoPrioridade, out msg))
                throw new ControllerException(msg);
        }

        #endregion
    }
}

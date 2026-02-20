using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BaixaTituloReceberNovo")]
    public class BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Documento"), out int codigoTituloBaixaAgrupadoDocumento);

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixaAgrupadoDocumento.BuscarTituloBaixaPorTituloBaixaAgrupadoDocumento(codigoTituloBaixaAgrupadoDocumento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);

                if (tituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                {
                    grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 18, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Observação", "Observacao", 18, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Valor em Moeda", "ValorMoeda", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho("Justificativa", "Justificativa", 25, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 20, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.left, false);
                }

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Justificativa")
                    propOrdena += ".Descricao";
                else if (propOrdena == "DescricaoTipoJustificativa")
                    propOrdena = "TipoJustificativa";

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> listaAcrescimoDesconto = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Consultar(codigoTituloBaixaAgrupadoDocumento, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.ContarConsulta(codigoTituloBaixaAgrupadoDocumento));

                var lista = (from p in listaAcrescimoDesconto
                             select new
                             {
                                 p.Codigo,
                                 Justificativa = p.Justificativa.Descricao,
                                 p.DescricaoTipoJustificativa,
                                 p.Observacao,
                                 Valor = p.Valor.ToString("n2"),
                                 ValorMoeda = p.ValorMoeda.ToString("n2")
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
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
                if (!this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo ou desconto na baixa de título a receber.");

                int codigoTituloBaixaAgrupadoDocumento = Request.GetIntParam("Documento");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                decimal valor = Request.GetDecimalParam("Valor");
                decimal valorMoeda = Request.GetDecimalParam("ValorMoeda");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo na baixa de título a receber.");
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = repTituloBaixaAgrupadoDocumento.BuscarPorCodigo(codigoTituloBaixaAgrupadoDocumento);

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da baixa.");

                if (!justificativa.GerarMovimentoAutomatico)
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    valor = Math.Round(valorMoeda * tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.ValorMoedaCotacao, 2, MidpointRounding.ToEven);
                else
                    valorMoeda = 0m;

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa;
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado;

                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarValorAoDocumento(out string mensagem, tituloBaixaAgrupadoDocumento, justificativa, valor, observacao, unidadeTrabalho, Usuario, null, false, valorMoeda))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupadoDocumento, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupado, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa + " ao título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa + " ao título " + tituloBaixaAgrupado.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unidadeTrabalho)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
                if (!this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para atualizar acréscimo ou desconto na baixa de título a receber.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                decimal valor = Request.GetDecimalParam("Valor");
                decimal valorMoeda = Request.GetDecimalParam("ValorMoeda");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento;
                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa;
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado;

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto) && tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para atualizar desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo) && tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para atualizar acréscimo na baixa de título a receber.");
                }

                if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.VariacaoCambial)
                    return new JsonpResult(false, true, "Não é permitido alterar um acréscimo/desconto de variação cambial. Este registro é de controle do sistema.");

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da baixa.");

                if (!justificativa.GerarMovimentoAutomatico)
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    valor = Math.Round(valorMoeda * tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.ValorMoedaCotacao, 2, MidpointRounding.ToEven);
                else
                    valorMoeda = 0m;

                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarValorAoDocumento(out string mensagem, tituloBaixaAgrupadoDocumento, justificativa, valor, observacao, unidadeTrabalho, Usuario, tituloBaixaAgrupadoDocumentoAcrescimoDesconto, false, valorMoeda))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupadoDocumento, null, "Atualizou o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupado, null, "Atualizou o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " ao título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Atualizou o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " ao título " + tituloBaixaAgrupado.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unidadeTrabalho)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
                if (!this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo ou desconto na baixa de título a receber.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento;
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa;
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado;

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto) && tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo) && tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo na baixa de título a receber.");
                }

                if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.VariacaoCambial)
                    return new JsonpResult(false, true, "Não é permitido alterar um acréscimo/desconto de variação cambial. Este registro é de controle do sistema.");

                if (tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da baixa.");

                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.RemoverValorDoDocumento(out string mensagem, tituloBaixaAgrupadoDocumento, tituloBaixaAgrupadoDocumentoAcrescimoDesconto, unidadeTrabalho))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupadoDocumento, null, "Removeu o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupado, null, "Removeu o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " do título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Removeu o " + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " do título " + tituloBaixaAgrupado.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unidadeTrabalho)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unidadeTrabalho);

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Codigo,
                    TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.Codigo,
                    Valor = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor.ToString("n2"),
                    ValorMoeda = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda.ToString("n2"),
                    Justificativa = new
                    {
                        Codigo = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.Codigo,
                        Descricao = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.Descricao
                    },
                    tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Observacao
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarAcrescimoDescontoDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
                if (!this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDescontoAcrescimo))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo ou desconto na baixa de título a receber.");

                int codigoBaixa = Request.GetIntParam("Codigo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                decimal valor = Request.GetDecimalParam("Valor");
                decimal valorMoeda = Request.GetDecimalParam("ValorMoeda");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> listaTitulos = repTituloBaixaAgrupadoDocumento.BuscarPorBaixa(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da baixa.");

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar desconto na baixa de título a receber.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaReceberNovo_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo na baixa de título a receber.");
                }

                if (!justificativa.GerarMovimentoAutomatico)
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");

                bool moedaEstrangeira = tituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real;

                decimal valorTotal = listaTitulos.Sum(o => moedaEstrangeira ? o.ValorPagoMoeda : o.ValorPago);
                decimal percentual = (moedaEstrangeira ? valorMoeda : valor) / valorTotal;
                decimal valorTotalRateado = 0m;
                decimal cotacao = tituloBaixa.ValorMoedaCotacao;

                int countDocumentos = listaTitulos.Count();

                unidadeTrabalho.Start();

                for (int i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = listaTitulos[i];
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado;

                    decimal valorRateado = 0m;

                    if (countDocumentos == (i + 1))
                        valorRateado = (moedaEstrangeira ? valorMoeda : valor) - valorTotalRateado;
                    else
                        valorRateado = Math.Round(((moedaEstrangeira ? tituloBaixaAgrupadoDocumento.ValorPagoMoeda : tituloBaixaAgrupadoDocumento.ValorPago) * percentual * 100) / 100, 2, MidpointRounding.ToEven);

                    if (valorRateado <= 0m)
                        continue;

                    valorTotalRateado += valorRateado;

                    decimal valorMoedaRateado = 0m;

                    if (moedaEstrangeira)
                    {
                        valorMoedaRateado = valorRateado;
                        valorRateado = Math.Round(valorMoedaRateado * cotacao, 2, MidpointRounding.ToEven);
                    }

                    if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarValorAoDocumento(out string mensagem, tituloBaixaAgrupadoDocumento, justificativa, valorRateado, observacao, unidadeTrabalho, Usuario, null, false, valorMoedaRateado))
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, mensagem);
                    }

                    Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unidadeTrabalho);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupadoDocumento, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa.ToLower() + ".", unidadeTrabalho);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixaAgrupado, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa.ToLower() + " ao título.", unidadeTrabalho);
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um " + justificativa.DescricaoTipoJustificativa.ToLower() + "." + tituloBaixa.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unidadeTrabalho)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RatearValorPagoEntreDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoBaixa = Request.GetIntParam("Codigo");

                decimal valorTotalPago = Request.GetDecimalParam("Valor");
                decimal valorTotalPagoMoeda = Request.GetDecimalParam("ValorMoeda");

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Não é possível ratear o valor pago entre os documentos na situação atual da baixa.");

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaGeralTituloBaixaAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa).OrderBy(o => o.ValorTotalAPagar).ToList();
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> listaGeralTituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorBaixa(codigoBaixa);

                int countTituloBaixaAgrupados = listaGeralTituloBaixaAgrupados.Count;

                bool moedaEstrangeira = tituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real;
                decimal cotacao = tituloBaixa.ValorMoedaCotacao;

                unitOfWork.Start();

                for (int i = 0; i < countTituloBaixaAgrupados; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = listaGeralTituloBaixaAgrupados[i];

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = listaGeralTituloBaixaAgrupadoDocumentos.Where(o => o.TituloBaixaAgrupado.Codigo == tituloBaixaAgrupado.Codigo).ToList();

                    int countTituloBaixaAgrupadoDocumentos = tituloBaixaAgrupadoDocumentos.Count;

                    for (int j = 0; j < countTituloBaixaAgrupadoDocumentos; j++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentos[j];

                        Servicos.Embarcador.Financeiro.BaixaTituloReceber.AjustarValorPagoDocumento(tituloBaixaAgrupadoDocumento, moedaEstrangeira, ref valorTotalPago, ref valorTotalPagoMoeda, unitOfWork);
                    }

                    Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unitOfWork);
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unitOfWork)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao ratear o valor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

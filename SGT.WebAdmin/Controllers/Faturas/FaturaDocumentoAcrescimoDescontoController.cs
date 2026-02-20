using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaDocumentoAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public FaturaDocumentoAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Documento"), out int codigoDocumento);

                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("ValorMoedaCotacao", false);
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Justificativa")
                    propOrdena += ".Descricao";
                else if (propOrdena == "DescricaoTipoJustificativa")
                    propOrdena = "TipoJustificativa";

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> listaAcrescimoDesconto = repFaturaDocumentoAcrescimoDesconto.Consultar(codigoDocumento, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaDocumentoAcrescimoDesconto.ContarConsulta(codigoDocumento));

                var lista = (from p in listaAcrescimoDesconto
                             select new
                             {
                                 p.Codigo,
                                 Justificativa = p.Justificativa.Descricao,
                                 p.DescricaoTipoJustificativa,
                                 p.Observacao,
                                 Valor = p.Valor.ToString("n2"),
                                 p.MoedaCotacaoBancoCentral,
                                 DataBaseCRT = p.DataBaseCRT.HasValue ? p.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 ValorMoedaCotacao = p.ValorMoedaCotacao.ToString("n2"),
                                 ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2")
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");

                int.TryParse(Request.Params("Documento"), out int codigoDocumento);
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                decimal valorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                decimal valorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar desconto na fatura.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo na fatura.");
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoDocumento, true);

                if (faturaDocumento.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da fatura.");
                }

                if (faturaDocumento.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto()
                {
                    FaturaDocumento = faturaDocumento,
                    Observacao = observacao,
                    Justificativa = justificativa,
                    Valor = valor,
                    MoedaCotacaoBancoCentral = moeda,
                    DataBaseCRT = dataBaseCRT,
                    ValorMoedaCotacao = valorMoedaCotacao,
                    ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira,
                    Usuario = Usuario
                };

                if (!faturaDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");
                }

                faturaDocumentoAcrescimoDesconto.TipoJustificativa = faturaDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
                faturaDocumentoAcrescimoDesconto.TipoMovimentoUso = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
                faturaDocumentoAcrescimoDesconto.TipoMovimentoReversao = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;

                repFaturaDocumentoAcrescimoDesconto.Inserir(faturaDocumentoAcrescimoDesconto, Auditado);

                if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                {
                    faturaDocumento.ValorAcrescimo += faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorAcrescimoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }
                else
                {
                    faturaDocumento.ValorDesconto += faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorDescontoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }

                faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar + faturaDocumento.ValorAcrescimo - faturaDocumento.ValorDesconto;

                if (faturaDocumento.ValorTotalACobrar < 0m)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível adicionar este valor pois o total a cobrar ficará negativo.");
                }

                repFaturaDocumento.Atualizar(faturaDocumento, Auditado);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaDocumento.Fatura;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Adicionou Documento Acrescimo Desconto.", unidadeTrabalho);

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");

                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                decimal valorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                decimal valorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para atualizar desconto na fatura.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para atualizar acréscimo na fatura.");
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto = repFaturaDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = faturaDocumentoAcrescimoDesconto.FaturaDocumento;
                faturaDocumento.Initialize();

                if (faturaDocumento.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da fatura.");
                }

                if (faturaDocumento.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                {
                    faturaDocumento.ValorAcrescimo -= faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorAcrescimoMoeda -= faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }
                else
                {
                    faturaDocumento.ValorDesconto -= faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorDescontoMoeda -= faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }

                faturaDocumentoAcrescimoDesconto.Justificativa = justificativa;

                if (!faturaDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");
                }

                faturaDocumentoAcrescimoDesconto.TipoJustificativa = faturaDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
                faturaDocumentoAcrescimoDesconto.TipoMovimentoUso = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
                faturaDocumentoAcrescimoDesconto.TipoMovimentoReversao = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;
                faturaDocumentoAcrescimoDesconto.Valor = valor;
                faturaDocumentoAcrescimoDesconto.Observacao = observacao;
                faturaDocumentoAcrescimoDesconto.MoedaCotacaoBancoCentral = moeda;
                faturaDocumentoAcrescimoDesconto.DataBaseCRT = dataBaseCRT;
                faturaDocumentoAcrescimoDesconto.ValorMoedaCotacao = valorMoedaCotacao;
                faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira;

                repFaturaDocumentoAcrescimoDesconto.Atualizar(faturaDocumentoAcrescimoDesconto, Auditado);

                if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                {
                    faturaDocumento.ValorAcrescimo += faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorAcrescimoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }
                else
                {
                    faturaDocumento.ValorDesconto += faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorDescontoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }

                faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar + faturaDocumento.ValorAcrescimo - faturaDocumento.ValorDesconto;

                if (faturaDocumento.ValorTotalACobrar < 0m)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível atualizar este valor pois o total a cobrar ficará negativo.");
                }

                repFaturaDocumento.Atualizar(faturaDocumento, Auditado);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaDocumento.Fatura;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Atualizou Documento Acrescimo Desconto.", unidadeTrabalho);

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto = repFaturaDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto) && faturaDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir desconto na fatura.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo) && faturaDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo na fatura.");
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = faturaDocumentoAcrescimoDesconto.FaturaDocumento;
                faturaDocumento.Initialize();

                if (faturaDocumento.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual da fatura.");
                }

                if (faturaDocumento.Fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                {
                    faturaDocumento.ValorAcrescimo -= faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorAcrescimoMoeda -= faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }
                else
                {
                    faturaDocumento.ValorDesconto -= faturaDocumentoAcrescimoDesconto.Valor;
                    faturaDocumento.ValorDescontoMoeda -= faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                }

                faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar + faturaDocumento.ValorAcrescimo - faturaDocumento.ValorDesconto;

                if (faturaDocumento.ValorTotalACobrar < 0m)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não é possível remover este valor pois o total a cobrar ficará negativo.");
                }

                repFaturaDocumento.Atualizar(faturaDocumento);
                repFaturaDocumentoAcrescimoDesconto.Deletar(faturaDocumentoAcrescimoDesconto, Auditado);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaDocumento.Fatura;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Exclui Documento Acrescimo Desconto.", unidadeTrabalho);

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
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
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto = repFaturaDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    faturaDocumentoAcrescimoDesconto.Codigo,
                    Documento = faturaDocumentoAcrescimoDesconto.FaturaDocumento.Codigo,
                    Valor = faturaDocumentoAcrescimoDesconto.Valor.ToString("n2"),
                    Justificativa = new
                    {
                        Codigo = faturaDocumentoAcrescimoDesconto.Justificativa.Codigo,
                        Descricao = faturaDocumentoAcrescimoDesconto.Justificativa.Descricao
                    },
                    faturaDocumentoAcrescimoDesconto.Observacao,
                    ValorMoedaCotacao = faturaDocumentoAcrescimoDesconto.ValorMoedaCotacao.ToString("n2"),
                    ValorOriginalMoedaEstrangeira = faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    faturaDocumentoAcrescimoDesconto.MoedaCotacaoBancoCentral,
                    DataBaseCRT = faturaDocumentoAcrescimoDesconto.DataBaseCRT.HasValue ? faturaDocumentoAcrescimoDesconto.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:ss") : string.Empty
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}

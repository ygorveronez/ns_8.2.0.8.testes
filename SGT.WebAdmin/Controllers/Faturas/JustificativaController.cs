using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    public class JustificativaController : BaseController
    {
		#region Construtores

		public JustificativaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa> finalidadesJustificativa = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa>();

                int nTipoJustificativa;
                if (int.TryParse(Request.Params("FinalidadeJustificativa"), out nTipoJustificativa))
                    finalidadesJustificativa.Add((Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa)nTipoJustificativa);
                else if (!string.IsNullOrWhiteSpace(Request.Params("FinalidadeJustificativa")))
                    finalidadesJustificativa = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa>>(Request.Params("FinalidadeJustificativa"));

                if (finalidadesJustificativa != null && finalidadesJustificativa.Count > 0 && !finalidadesJustificativa.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.Todas))
                    finalidadesJustificativa.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.Todas);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa? tipoJustificativa = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativaAux;
                if (Enum.TryParse(Request.Params("TipoJustificativa"), out tipoJustificativaAux))
                    tipoJustificativa = tipoJustificativaAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Descricao, "Descricao", 80, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("DescricaoTipoJustificativa", false);
                    grid.AdicionarCabecalho("DescricaoFinalidadeJustificativa", false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Tipo, "DescricaoTipoJustificativa", 15, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Finalidade, "DescricaoFinalidadeJustificativa", 25, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("DescricaoAplicacaoValorContratoFrete", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "DescricaoTipoJustificativa")
                    propOrdenar = "TipoJustificativa";
                else if (propOrdenar == "DescricaoFinalidadeJustificativa")
                    propOrdenar = "FinalidadeJustificativa";

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Fatura.Justificativa> listaJustificativa = repJustificativa.Consultar(codigoEmpresa, descricao, ativo, tipoJustificativa, finalidadesJustificativa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repJustificativa.ContarConsulta(codigoEmpresa, descricao, ativo, tipoJustificativa, finalidadesJustificativa));

                var lista = (from p in listaJustificativa
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoFinalidadeJustificativa,
                                 p.DescricaoTipoJustificativa,
                                 DescricaoAplicacaoValorContratoFrete = p.AplicacaoValorContratoFrete.HasValue ? p.AplicacaoValorContratoFrete.Value.ObterDescricao() : string.Empty,
                                 TipoJustificativa = new { Codigo = p.TipoJustificativa,  Descricao = p.DescricaoTipoJustificativa } 
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
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo, gerarMovimentoAutomatico;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);

                int codigoTipoMovimentoUsoJustificativa, codigoTipoMovimentoReversaoUsoJustificativa;
                int.TryParse(Request.Params("TipoMovimentoUsoJustificativa"), out codigoTipoMovimentoUsoJustificativa);
                int.TryParse(Request.Params("TipoMovimentoReversaoUsoJustificativa"), out codigoTipoMovimentoReversaoUsoJustificativa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa finalidadeJustificativa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete? aplicacaoValorContratoFrete = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete>("AplicacaoValorContratoFrete");
                Enum.TryParse(Request.Params("TipoJustificativa"), out tipoJustificativa);
                Enum.TryParse(Request.Params("FinalidadeJustificativa"), out finalidadeJustificativa);
                bool usarDataAutorizacaoParaMovimentoAcrescimoDesconto = Request.GetBoolParam("UsarDataAutorizacaoParaMovimentoAcrescimoDesconto");

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                string codigoIntegracaoRepom = Request.GetStringParam("CodigoIntegracaoRepom");

                int.TryParse(Request.Params("MotivoAvaria"), out int codigoMotivoAvaria);

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = new Dominio.Entidades.Embarcador.Fatura.Justificativa()
                {
                    Ativo = ativo,
                    Descricao = descricao,
                    FinalidadeJustificativa = finalidadeJustificativa,
                    GerarMovimentoAutomatico = gerarMovimentoAutomatico,
                    TipoMovimentoReversaoUsoJustificativa = codigoTipoMovimentoReversaoUsoJustificativa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoUsoJustificativa) : null,
                    TipoJustificativa = tipoJustificativa,
                    TipoMovimentoUsoJustificativa = codigoTipoMovimentoUsoJustificativa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoJustificativa) : null,
                    Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Empresa : null,
                    CodigoIntegracaoRepom = codigoIntegracaoRepom,
                    MotivoAvaria = codigoMotivoAvaria > 0 ? repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria) : null,
                    UsarDataAutorizacaoParaMovimentoAcrescimoDesconto = usarDataAutorizacaoParaMovimentoAcrescimoDesconto,
                    CodigoIntegracao = codigoIntegracao,
                };

                if (finalidadeJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.ContratoFrete)
                    justificativa.AplicacaoValorContratoFrete = aplicacaoValorContratoFrete;
                else
                    justificativa.AplicacaoValorContratoFrete = null;

                if (gerarMovimentoAutomatico)
                {
                    if (justificativa.TipoMovimentoReversaoUsoJustificativa == null || justificativa.TipoMovimentoUsoJustificativa == null)
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                }

                repJustificativa.Inserir(justificativa, Auditado);

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
                bool ativo, gerarMovimentoAutomatico;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);

                int codigo, codigoTipoMovimentoUsoJustificativa, codigoTipoMovimentoReversaoUsoJustificativa;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("TipoMovimentoUsoJustificativa"), out codigoTipoMovimentoUsoJustificativa);
                int.TryParse(Request.Params("TipoMovimentoReversaoUsoJustificativa"), out codigoTipoMovimentoReversaoUsoJustificativa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa finalidadeJustificativa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete? aplicacaoValorContratoFrete = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete>("AplicacaoValorContratoFrete");
                Enum.TryParse(Request.Params("TipoJustificativa"), out tipoJustificativa);
                Enum.TryParse(Request.Params("FinalidadeJustificativa"), out finalidadeJustificativa);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                string codigoIntegracaoRepom = Request.GetStringParam("CodigoIntegracaoRepom");
                bool usarDataAutorizacaoParaMovimentoAcrescimoDesconto = Request.GetBoolParam("UsarDataAutorizacaoParaMovimentoAcrescimoDesconto");

                int.TryParse(Request.Params("MotivoAvaria"), out int codigoMotivoAvaria);

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigo, true);

                justificativa.Ativo = ativo;
                justificativa.Descricao = descricao;
                justificativa.FinalidadeJustificativa = finalidadeJustificativa;
                justificativa.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
                justificativa.TipoMovimentoReversaoUsoJustificativa = codigoTipoMovimentoReversaoUsoJustificativa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoUsoJustificativa) : null;
                justificativa.TipoJustificativa = tipoJustificativa;
                justificativa.TipoMovimentoUsoJustificativa = codigoTipoMovimentoUsoJustificativa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoJustificativa) : null;
                justificativa.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Empresa : null;
                justificativa.CodigoIntegracaoRepom = codigoIntegracaoRepom;
                justificativa.MotivoAvaria = codigoMotivoAvaria > 0 ? repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria) : null;
                justificativa.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto = usarDataAutorizacaoParaMovimentoAcrescimoDesconto;
                justificativa.CodigoIntegracao = codigoIntegracao;
                if (finalidadeJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.ContratoFrete)
                    justificativa.AplicacaoValorContratoFrete = aplicacaoValorContratoFrete;
                else
                    justificativa.AplicacaoValorContratoFrete = null;

                if (gerarMovimentoAutomatico)
                {
                    if (justificativa.TipoMovimentoReversaoUsoJustificativa == null || justificativa.TipoMovimentoUsoJustificativa == null)
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                }

                repJustificativa.Atualizar(justificativa, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    justificativa.Codigo,
                    justificativa.Ativo,
                    justificativa.Descricao,
                    justificativa.TipoJustificativa,
                    justificativa.FinalidadeJustificativa,
                    justificativa.AplicacaoValorContratoFrete,
                    justificativa.GerarMovimentoAutomatico,
                    justificativa.UsarDataAutorizacaoParaMovimentoAcrescimoDesconto,
                    TipoMovimentoUsoJustificativa = new
                    {
                        Descricao = justificativa.TipoMovimentoUsoJustificativa?.Descricao ?? string.Empty,
                        Codigo = justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0
                    },
                    TipoMovimentoReversaoUsoJustificativa = new
                    {
                        Descricao = justificativa.TipoMovimentoReversaoUsoJustificativa?.Descricao ?? string.Empty,
                        Codigo = justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0
                    },
                    MotivoAvaria = new
                    {
                        Descricao = justificativa.MotivoAvaria?.Descricao ?? string.Empty,
                        Codigo = justificativa.MotivoAvaria?.Codigo ?? 0
                    },
                    justificativa.CodigoIntegracaoRepom,
                    justificativa.CodigoIntegracao
                };

                return new JsonpResult(retorno);
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigo);

                repJustificativa.Deletar(justificativa, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
    }
}

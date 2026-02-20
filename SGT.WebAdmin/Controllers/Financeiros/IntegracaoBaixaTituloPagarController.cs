using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    public class IntegracaoBaixaTituloPagarController : BaseController
    {
		#region Construtores

		public IntegracaoBaixaTituloPagarController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> PesquisaIntegracao()
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
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoIntegracaoTituloBaixa", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Assunto", "Assunto", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatários", "Destinatarios", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Última Integração", "DataIntegracao", 15, Models.Grid.Align.center, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoTipoIntegracaoTituloBaixa")
                    propOrdenar = "TipoIntegracaoTituloBaixa";

                Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao> listaTituloBaixaIntegracao = repTituloBaixaIntegracao.BuscarPorTituloBaixa(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaIntegracao.ContarBuscarPorTituloBaixa(codigo));
                var dynRetorno = (from obj in listaTituloBaixaIntegracao
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoTituloBaixa = obj.TituloBaixa.Codigo,
                                      obj.DescricaoTipoIntegracaoTituloBaixa,
                                      obj.Assunto,
                                      obj.Destinatarios,
                                      DataIntegracao = obj.DataIntegracao.Value.ToString("dd/MM/yyyy")
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as integrações realizadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao integracao = repTituloBaixaIntegracao.BuscarPorCodigo(codigo);
                integracao.DataIntegracao = DateTime.Now;
                repTituloBaixaIntegracao.Atualizar(integracao);

                if (integracao.TipoIntegracaoTituloBaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoTituloBaixa.Email)
                {
                    Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                    Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unitOfWork);

                    if (email != null && !string.IsNullOrWhiteSpace(integracao.Destinatarios))
                    {
                        string mensagemErro = "";
                        Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, integracao.Destinatarios, null, null, integracao.Assunto, integracao.Mensagem, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
                        if (!string.IsNullOrEmpty(mensagemErro))
                            return new JsonpResult(false, mensagemErro);
                    }
                }

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}

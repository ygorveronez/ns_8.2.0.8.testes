using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TermoQuitacaoDocumento")]
    public class TermoQuitacaoDocumentoController : BaseController
    {
		#region Construtores

		public TermoQuitacaoDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Renderizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string caminhoArquivo = string.Empty;

                if (codigo == 0)
                    return new JsonpResult(false, "Necessário selecionar um registro!");

                Repositorio.Embarcador.Financeiro.TermoQuitacao repTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento repDocumento = new Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = repTermoQuitacao.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento documento = repDocumento.BuscarAnexoPorEntidade(codigo);

                Servicos.Embarcador.Financeiro.TermoQuitacaoControleDocumento svcDocumento = new Servicos.Embarcador.Financeiro.TermoQuitacaoControleDocumento(unitOfWork);

                string caminhoRaiz = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos;
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine( caminhoRaiz , "TermoQuitacaoFinanceiro", "ControleDocumentos", $"{documento.GuidArquivo}.pdf");

                byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);

                MemoryStream stream = new MemoryStream();
                stream.Write(pdf, 0, pdf.Length);
                stream.Position = 0;

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o termo de quitação!");

                return File(stream, "application/pdf", $"Termo de Quitação N° {termoQuitacao.Codigo}");

            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Financeiro.TermoQuitacao repTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");
                string justificativa = Request.GetStringParam("Justificativa");

                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo = repTermoQuitacao.BuscarPorCodigo(codigo, false);

                if (termo == null)
                    throw new ControllerException("Nenhum termo selecionado");

                if (termo.SituacaoTermoQuitacao != SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador)
                    throw new ControllerException("O termo não pode ser aprovado na situação atual");

                unitOfWork.Start();

                termo.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.AprovadoTransportador;
                termo.SituacaoAprovacaoTransportador = SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado;
                repTermoQuitacao.Atualizar(termo);

                Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork, Auditado, configuracaoFinanceiro);
                servicoProvisao.VerificarProvisoesPendentesAprovacaoTermo(termo, TipoServicoMultisoftware);

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
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TermoQuitacao repTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");
                string justificativa = Request.GetStringParam("Justificativa");

                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo = repTermoQuitacao.BuscarPorCodigo(codigo, false);
                if (termo == null)
                    throw new ControllerException("Nenhum termo selecionado");

                if (termo.SituacaoTermoQuitacao != SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador)
                    throw new ControllerException("O termo não pode ser rejeitado na situação atual");

                unitOfWork.Start();

                termo.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador;
                termo.SituacaoAprovacaoTransportador = SituacaoAprovacaoTermoQuitacaoTransportador.Reprovado;
                termo.JustificativaRejeicao = justificativa;
                repTermoQuitacao.Atualizar(termo);

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
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        #endregion
    }
}

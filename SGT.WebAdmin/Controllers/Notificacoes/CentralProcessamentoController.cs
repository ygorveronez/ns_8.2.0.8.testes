using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/CentralProcessamento")]
    public class CentralProcessamentoController : BaseController
    {
		#region Construtores

		public CentralProcessamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarProcessamentoPendentes()//todo: existem apenas downloads de relatório até o momento, se necessário gerenciar outros downloads controlar aqui.
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorioControleGeracaoArquivo = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);

                List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> relatoriosEmExecucao = repRelatorioControleGeracao.BuscarRelatoriosEmExecucao(this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo> arquivosEmGeracao = repositorioControleGeracaoArquivo.BuscarEmGeracaoPorUsuario(this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> comissoesEmGeracao = repComissaoFuncionario.BuscarPorSituacaoEUsuario(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.EmGeracao, this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Fatura.Fatura> listaFaturas = repFatura.BuscarPorEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas, this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> listaFaturamentoEmGeracaoAutorizacaoDocumento = repFaturamentoMensal.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoAutorizacaoDocumento);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> listaFaturamentoEmGeracaoEnvioEmail = repFaturamentoMensal.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoEnvioEmail);

                var processamentos = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioEmExecucao in relatoriosEmExecucao)
                {
                    var processo = new
                    {
                        relatorioEmExecucao.Codigo,
                        Descricao = string.Format(Localization.Resources.Notificacoes.CentralProcessamento.GerandoOArquivo, relatorioEmExecucao.DescricaoTipoArquivo),
                        Titulo = relatorioEmExecucao.Titulo
                    };
                    processamentos.Add(processo);
                }
                foreach (Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo arquivoEmGeracao in arquivosEmGeracao)
                {
                    var processo = new
                    {
                        arquivoEmGeracao.Codigo,
                        Descricao = string.Format(Localization.Resources.Notificacoes.CentralProcessamento.GerandoOArquivo, arquivoEmGeracao.TipoArquivo.ObterDescricao()),
                        Titulo = arquivoEmGeracao.Descricao
                    };
                    processamentos.Add(processo);
                }
                foreach (Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissao in comissoesEmGeracao)
                {
                    var processo = new
                    {
                        comissao.Codigo,
                        Descricao = string.Format(Localization.Resources.Notificacoes.CentralProcessamento.GerandoComissaoMotoristasDeAte, comissao.DataInicio.ToString("dd/MM/yyyy"), comissao.DataFim.ToString("dd/MM/yyyy")),
                        Titulo = Localization.Resources.Notificacoes.CentralProcessamento.ComissaoMotorista
                    };
                    processamentos.Add(processo);
                }
                foreach (Dominio.Entidades.Embarcador.Fatura.Fatura fatura in listaFaturas)
                {
                    var processo = new
                    {
                        fatura.Codigo,
                        Descricao = string.Format(Localization.Resources.Notificacoes.CentralProcessamento.GerandoFaturaDe, (fatura.GrupoPessoas?.Descricao ?? fatura.Cliente?.Nome ?? "")),
                        Titulo = Localization.Resources.Notificacoes.CentralProcessamento.GeracaoFatura
                    };
                    processamentos.Add(processo);
                }
                foreach (Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal fatura in listaFaturamentoEmGeracaoAutorizacaoDocumento)
                {
                    var processo = new
                    {
                        fatura.Codigo,
                        Descricao = Localization.Resources.Notificacoes.CentralProcessamento.AutorizandoDocumentos,
                        Titulo = Localization.Resources.Notificacoes.CentralProcessamento.FaturamentoMensal
                    };
                    processamentos.Add(processo);
                }
                foreach (Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal fatura in listaFaturamentoEmGeracaoEnvioEmail)
                {
                    var processo = new
                    {
                        fatura.Codigo,
                        Descricao = Localization.Resources.Notificacoes.CentralProcessamento.EnviandoEmails,
                        Titulo = Localization.Resources.Notificacoes.CentralProcessamento.FaturamentoMensal
                    };
                    processamentos.Add(processo);
                }

                var retorno = new
                {
                    NadaParaProcessar = processamentos.Count > 0 ? false : true,
                    Processamentos = processamentos,
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Notificacoes.CentralProcessamento.OcorreuFalhaConsultarProcessamentosPendentes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}

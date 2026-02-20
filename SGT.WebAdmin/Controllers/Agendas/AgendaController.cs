using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Agendas
{
    [CustomAuthorize(new string[] { "ObterInformacoesTarefas" }, "Agendas/Agenda")]
    public class AgendaController : BaseController
    {
		#region Construtores

		public AgendaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterInformacoesTarefas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string observacao = Request.Params("Observacao");

                DateTime.TryParse(Request.Params("DataAgenda"), out DateTime dataAgenda);

                Enum.TryParse(Request.Params("Status"), out StatusAgendaTarefa status);

                int.TryParse(Request.Params("Colaborador"), out int codigoColaborador);

                double.TryParse(Request.Params("Cliente"), out double cliente);

                int codigoEmpresa = this.Usuario.Empresa.Codigo;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    codigoEmpresa = 0;

                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa> listaAgendaTarefa = repAgendaTarefa.Consultar(observacao, dataAgenda, dataAgenda, status, codigoColaborador, cliente, codigoEmpresa, "", "", 0, 0);

                var retorno = (from agenda in listaAgendaTarefa select ObterDadosTarefaAgenda(agenda, unidadeDeTrabalho)).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private object ObterDadosTarefaAgenda(Dominio.Entidades.Embarcador.Agenda.AgendaTarefa tarefa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            return new
            {
                tarefa.Codigo,
                tarefa.Observacao,
                DataInicial = tarefa.DataInicial.ToString("dd/MM/yyyy HH:mm"),
                DataFinal = tarefa.DataFinal.ToString("dd/MM/yyyy HH:mm"),
                Colaborador = tarefa.Funcionario?.Nome ?? string.Empty,
                Cliente = tarefa.Cliente?.Descricao ?? string.Empty,
                DescricaoStatus = StatusAgendaTarefaHelper.ObterDescricao(tarefa.Status),
                DescricaoPrioridade = PrioridadeAtendimentoHelper.ObterDescricao(tarefa.Prioridade),
                DescricaoTipoAssinatura = TipoAssinaturaVendaDiretaHelper.ObterDescricao(tarefa.TipoAssinatura),
                Tempo = (tarefa.DataFinal - tarefa.DataInicial).TotalMinutes,
                tarefa.Status,
                Servico = tarefa.Servico?.Descricao ?? string.Empty
            };
        }

        #endregion
    }
}

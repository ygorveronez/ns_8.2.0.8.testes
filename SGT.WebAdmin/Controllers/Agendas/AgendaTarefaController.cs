using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Agendas
{
    [CustomAuthorize("Agendas/AgendaTarefa", "Agendas/Agenda")]
    public class AgendaTarefaController : BaseController
    {
		#region Construtores

		public AgendaTarefaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unitOfWork);

                string observacao = Request.Params("Observacao");

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                StatusAgendaTarefa status;
                Enum.TryParse(Request.Params("Status"), out status);

                int.TryParse(Request.Params("Usuario"), out int codigoUsuario);

                double.TryParse(Request.Params("Cliente"), out double cliente);

                int empresa = this.Usuario.Empresa.Codigo;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Colaborador", "Colaborador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Prioridade", "DescricaoPrioridade", 8, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa> listaAgendaTarefa = repAgendaTarefa.Consultar(observacao, dataInicial, dataFinal, status, codigoUsuario, cliente, empresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAgendaTarefa.ContarConsulta(observacao, dataInicial, dataFinal, status, codigoUsuario, cliente, empresa));
                var lista = (from p in listaAgendaTarefa
                             select new
                             {
                                 p.Codigo,
                                 p.Observacao,
                                 DataInicial = p.DataInicial.ToString("dd/MM/yyyy HH:mm"),
                                 DataFinal = p.DataFinal.ToString("dd/MM/yyyy HH:mm"),
                                 Colaborador = p.Funcionario?.Nome ?? string.Empty,
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 DescricaoStatus = StatusAgendaTarefaHelper.ObterDescricao(p.Status),
                                 DescricaoPrioridade = PrioridadeAtendimentoHelper.ObterDescricao(p.Prioridade)
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa = new Dominio.Entidades.Embarcador.Agenda.AgendaTarefa();

                try
                {
                    PreencherAgendaTarefa(agendaTarefa, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                repAgendaTarefa.Inserir(agendaTarefa, Auditado);
                unitOfWork.CommitChanges();

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
                unitOfWork.Start();
                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa = repAgendaTarefa.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                try
                {
                    PreencherAgendaTarefa(agendaTarefa, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                repAgendaTarefa.Atualizar(agendaTarefa, Auditado);
                unitOfWork.CommitChanges();

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa = repAgendaTarefa.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    agendaTarefa.Codigo,
                    DataInicial = agendaTarefa.DataInicial.ToString("dd/MM/yyyy HH:mm"),
                    DataFinal = agendaTarefa.DataFinal.ToString("dd/MM/yyyy HH:mm"),
                    agendaTarefa.Status,
                    agendaTarefa.Prioridade,
                    agendaTarefa.Observacao,
                    Usuario = agendaTarefa.Funcionario != null ? new { agendaTarefa.Funcionario.Codigo, Descricao = agendaTarefa.Funcionario.Nome } : null,
                    Cliente = agendaTarefa.Cliente != null ? new { agendaTarefa.Cliente.Codigo, Descricao = agendaTarefa.Cliente.Nome } : null,
                    Servico = agendaTarefa.Servico != null ? new { agendaTarefa.Servico.Codigo, Descricao = agendaTarefa.Servico.Descricao } : null
                };
                return new JsonpResult(dynProcessoMovimento);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa = repAgendaTarefa.BuscarPorCodigo(codigo);
                repAgendaTarefa.Deletar(agendaTarefa, Auditado);
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

        #endregion

        #region Métodos Privados

        private void PreencherAgendaTarefa(Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);

            int codigoUsuario = 0;
            int.TryParse(Request.Params("Usuario"), out codigoUsuario);

            int codigoServico = 0;
            int.TryParse(Request.Params("Servico"), out codigoServico);

            double cnpjCliente = 0;
            double.TryParse(Request.Params("Cliente"), out cnpjCliente);

            DateTime dataInicial, dataFinal;
            DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

            if (dataInicial >= dataFinal)
                throw new ControllerException("Data e Hora Final deve ser maior que a Inicial");

            string observacao = Request.Params("Observacao");

            StatusAgendaTarefa status;
            PrioridadeAtendimento prioridade;
            Enum.TryParse(Request.Params("Status"), out status);
            Enum.TryParse(Request.Params("Prioridade"), out prioridade);

            agendaTarefa.Cliente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
            agendaTarefa.DataFinal = dataFinal;
            agendaTarefa.DataInicial = dataInicial;
            agendaTarefa.Funcionario = codigoUsuario > 0 ? repUsuario.BuscarPorCodigo(codigoUsuario) : null;
            agendaTarefa.Servico = codigoServico > 0 ? repServico.BuscarPorCodigo(codigoServico) : null;
            agendaTarefa.Observacao = observacao;
            agendaTarefa.Prioridade = prioridade;
            agendaTarefa.Status = status;

            if (agendaTarefa.Codigo == 0)
                agendaTarefa.Empresa = this.Usuario.Empresa;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Newtonsoft.Json;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize(new string[] { "AtualizarDiaria", "InserirDiaria", "RemoverDiaria", "BuscarDiaria" }, "Acertos/AcertoViagem")]
    public class AcertoDiariaController : BaseController
    {
		#region Construtores

		public AcertoDiariaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AtualizarDiaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                acertoViagem.DiariaSalvo = true;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Diarias, this.Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as diárias.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarDiarias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarDiarias " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 75, Models.Grid.Align.left, true, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Situação colaborador", "SituacaoColaborador", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> listaAcertoDiaria = repAcertoDiaria.BuscarAcertoDiaria(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoDiaria.ContarBuscarAcertoDiaria(codigoAcerto));
                var retorno = (from obj in listaAcertoDiaria
                               select new
                               {
                                   obj.Codigo,
                                   Descricao = obj.Descricao,
                                   SituacaoColaborador = obj.SituacaoColaborador?.ObterDescricao() ?? "Lançamento não encontrado!",
                                   Valor = obj.Valor.ToString("n2")
                               }).ToList();

                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarDiarias " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirDiaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoJustificativa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDiaria diaria = new Dominio.Entidades.Embarcador.Acerto.AcertoDiaria();
                diaria.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                diaria.Descricao = descricao;
                diaria.LancadoManualmente = true;
                diaria.Data = Request.GetNullableDateTimeParam("Data");
                diaria.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                // Verifica se existe lançamento de situação do colaborador na data e ajusta o valor da diária caso esteja trabalhando ou não.
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamentoSituacaoColaboradorFiltradoPorData = repColaboradorLancamento.ConsultarLancamentoColaboradorPorData(diaria.AcertoViagem.Motorista.Codigo, diaria.Data);
                diaria.Valor = (lancamentoSituacaoColaboradorFiltradoPorData != null && lancamentoSituacaoColaboradorFiltradoPorData?.ColaboradorSituacao?.SituacaoColaborador == SituacaoColaborador.Trabalhando) ? valor : lancamentoSituacaoColaboradorFiltradoPorData == null ? valor : 0;
                diaria.SituacaoColaborador = lancamentoSituacaoColaboradorFiltradoPorData?.ColaboradorSituacao?.SituacaoColaborador;

                if (ConfiguracaoEmbarcador.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto)
                {
                    if (diaria.AcertoViagem.DataInicial > diaria.Data)
                    {
                        return new JsonpResult(true, false, "Não é possível lançar uma diária fora do período do acerto.");
                    }
                    if (diaria.AcertoViagem.DataFinal.HasValue && diaria.AcertoViagem.DataFinal.Value < diaria.Data)
                    {
                        return new JsonpResult(true, false, "Não é possível lançar uma diária fora do período do acerto.");
                    }
                }

                unitOfWork.Start();
                repAcertoDiaria.Inserir(diaria, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, diaria.AcertoViagem, null, "Inserido a diária " + diaria.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir nova diária. " + ex.Message);
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDiaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoDiaria diaria = repAcertoDiaria.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();
                Servicos.Auditoria.Auditoria.Auditar(Auditado, diaria.AcertoViagem, null, "Deletado a diária " + diaria.Descricao + ".", unitOfWork);
                repAcertoDiaria.Deletar(diaria, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remver a diária. " + ex.Message);
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDiariasSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverDiariasSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<int> codigosDiarias = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

                foreach (var codigo in codigosDiarias)
                {
                    Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);

                    Dominio.Entidades.Embarcador.Acerto.AcertoDiaria acertoDiaria = repAcertoDiaria.BuscarPorCodigo(codigo);

                    repAcertoDiaria.Deletar(acertoDiaria/*, Auditado*/);
                }

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar as diárias.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverDiariasSelecionados " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDiaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigo);
                bool limparDiarias = false;
                bool.TryParse(Request.Params("LimparRegistros"), out limparDiarias);

                DateTime.TryParse(Request.Params("DataHoraInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataHoraFinal"), out DateTime dataFinal);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
                Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);
                Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigo);                

                if (acerto == null)
                    return new JsonpResult(false, "Acerto não iniciado.");

                if (dataInicial == null || dataInicial == DateTime.MinValue)
                    return new JsonpResult(false, "Favor informe a data inicial.");

                acerto.DataInicial = dataInicial;
                if (dataFinal > DateTime.MinValue)
                    acerto.DataFinal = dataFinal;
                else
                    acerto.DataFinal = null;

                if (!acerto.DataFinal.HasValue || acerto.DataFinal == null || acerto.DataFinal.Value == DateTime.MinValue)
                    return new JsonpResult(false, "Favor informe a data final.");

                repAcertoViagem.Atualizar(acerto);

                List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> tabelaDiaria = null;
                Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodoDaVigencia = null;
                List<int> codigosTabelas = new List<int>();

                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.SegmentoVeiculo?.Codigo ?? 0, acerto.Cargas.Where(o => o.Carga.ModeloVeicularCarga != null).Select(o => o.Carga.ModeloVeicularCarga.Codigo).ToList(), acerto.Motorista.CentroResultado?.Codigo ?? 0);
                if ((tabelaDiaria == null || tabelaDiaria.Count == 0) && acerto.Motorista.CentroResultado != null)
                    tabelaDiaria = repTabelaDiaria.BuscarTabelaDiariaPorCentroResultado(acerto.Motorista.CentroResultado?.Codigo ?? 0);
                if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                    tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.SegmentoVeiculo?.Codigo ?? 0);
                if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                    tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.Cargas.Where(o => o.Carga.ModeloVeicularCarga != null).Select(o => o.Carga.ModeloVeicularCarga.Codigo).ToList());
                if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                    tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria();

                if (tabelaDiaria == null || tabelaDiaria.Count == 0)
                    return new JsonpResult(false, "Nenhuma tabela ativa cadastrada.");

                codigosTabelas = tabelaDiaria.Select(c => c.Codigo).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repTabelaDiariaPeriodo.BuscarPorTabela(codigosTabelas.FirstOrDefault());
                if (periodos == null || periodos.Count <= 0)
                    return new JsonpResult(false, "Tabela sem períodos cadastrado.");

                unitOfWork.Start();

                if (limparDiarias)
                {
                    foreach (var diaria in acerto.Diarias)
                        repAcertoDiaria.Deletar(diaria);
                }

                dataInicial = acerto.DataInicial;
                dataFinal = acerto.DataFinal.Value;
                DateTime dataFinalDia;
                bool encontrouRefeicao = false;
                while (dataInicial <= dataFinal)
                {
                    dataFinalDia = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 59, 59);
                    if (dataFinalDia > dataFinal)
                        dataFinalDia = dataFinal;

                    foreach (var periodo in periodos)
                    {
                        if (periodo.HoraInicial.Value.Hours == 0 && periodo.HoraFinal.HasValue)
                        {
                            if (dataInicial.TimeOfDay <= periodo.HoraFinal.Value)
                            {
                                periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                                if (periodoDaVigencia == null)
                                    periodoDaVigencia = periodo;
                                AdicionarDiariaTabela(unitOfWork, repAcertoDiaria, acerto, periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy"), periodoDaVigencia.Valor, periodoDaVigencia.Justificativa, periodoDaVigencia, dataInicial);
                                encontrouRefeicao = true;
                            }
                        }
                        //else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours > 0)
                        else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours < 23)
                        {
                            if ((dataInicial.TimeOfDay >= periodo.HoraInicial.Value || dataInicial.TimeOfDay <= periodo.HoraInicial.Value) && (dataFinalDia.TimeOfDay >= periodo.HoraFinal.Value || dataFinalDia.TimeOfDay <= periodo.HoraFinal.Value)
                                && (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value && periodo.HoraFinal.Value >= dataInicial.TimeOfDay))
                            {
                                periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                                if (periodoDaVigencia == null)
                                    periodoDaVigencia = periodo;
                                AdicionarDiariaTabela(unitOfWork, repAcertoDiaria, acerto, periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy"), periodoDaVigencia.Valor, periodoDaVigencia.Justificativa, periodoDaVigencia, dataInicial);
                                encontrouRefeicao = true;
                            }
                        }
                        else if (periodo.HoraInicial.HasValue && periodo.HoraFinal.Value.Hours == 23)
                        {
                            if (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value)
                            {
                                periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                                if (periodoDaVigencia == null)
                                    periodoDaVigencia = periodo;
                                AdicionarDiariaTabela(unitOfWork, repAcertoDiaria, acerto, periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy"), periodoDaVigencia.Valor, periodoDaVigencia.Justificativa, periodoDaVigencia, dataInicial);
                                encontrouRefeicao = true;
                            }
                        }
                    }

                    dataInicial = dataInicial.AddDays(1);
                    dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 5, 00, 00);
                }
                //para quando precisa pagar a refeição do período anterior
                if (!encontrouRefeicao)
                {
                    dataInicial = acerto.DataInicial;
                    Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodo = null;
                    periodo = periodos.Where(o => o.HoraInicial <= dataInicial.TimeOfDay && (!o.HoraFinal.HasValue || o.HoraFinal.Value.Hours == 0)).FirstOrDefault();//para buscar a janta
                    if (periodo == null)
                        periodo = periodos.Where(o => o.HoraInicial <= dataInicial.TimeOfDay && o.HoraFinal.Value.Hours > 0 && o.HoraInicial.Value.Hours > 0).FirstOrDefault();//para buscar o almoço
                    if (periodo == null)
                        periodo = periodos.Where(o => o.HoraFinal <= dataInicial.TimeOfDay && (!o.HoraInicial.HasValue || o.HoraInicial.Value.Hours == 0)).FirstOrDefault();//para buscar o café
                    if (periodo != null)
                    {
                        periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                        if (periodoDaVigencia == null)
                            periodoDaVigencia = periodo;
                        AdicionarDiariaTabela(unitOfWork, repAcertoDiaria, acerto, periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy"), periodoDaVigencia.Valor, periodoDaVigencia.Justificativa, periodoDaVigencia, dataInicial);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Buscado diárias automáticas.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remver a diária. " + ex.Message);
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverDiaria " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        private void AdicionarDiariaTabela(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, string descricao, decimal valor, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo tabelaDiariaPeriodo, DateTime dataDiaria)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamentoSituacaoColaboradorFiltradoPorData = repColaboradorLancamento.ConsultarLancamentoColaboradorPorData(acerto.Motorista.Codigo, dataDiaria);

            Dominio.Entidades.Embarcador.Acerto.AcertoDiaria diaria = new Dominio.Entidades.Embarcador.Acerto.AcertoDiaria();
            diaria.AcertoViagem = acerto;
            diaria.Descricao = descricao;
            diaria.Valor = (lancamentoSituacaoColaboradorFiltradoPorData != null && lancamentoSituacaoColaboradorFiltradoPorData?.ColaboradorSituacao?.SituacaoColaborador == SituacaoColaborador.Trabalhando) ? valor : lancamentoSituacaoColaboradorFiltradoPorData == null ? valor : 0;
            diaria.LancadoManualmente = false;
            diaria.Justificativa = justificativa;
            diaria.TabelaDiariaPeriodo = tabelaDiariaPeriodo;
            diaria.Data = dataDiaria;
            diaria.SituacaoColaborador = lancamentoSituacaoColaboradorFiltradoPorData?.ColaboradorSituacao?.SituacaoColaborador;

            repAcertoDiaria.Inserir(diaria, Auditado);
        }
    }
}



using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/GuaritaCheckList")]
    public class GuaritaCheckListServicoController : BaseController
    {
		#region Construtores

		public GuaritaCheckListServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais Veículo

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuaritaCheckList, codigoServico, tempoEstimado;
                int.TryParse(Request.Params("GuaritaCheckList"), out codigoGuaritaCheckList);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("TempoEstimado"), out tempoEstimado);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repServicoVeiculo.BuscarPorCodigo(codigoServico);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Guarita Check List não encontrada.");

                if (servico == null)
                    return new JsonpResult(false, true, "Serviço não encontrado.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(servico.Codigo, guaritaCheckList.Veiculo?.Codigo ?? 0, 0);

                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
                bool manutencaoCorretiva = false;

                if (servico.ExecucaoUnica)
                {
                    if (servico.ValidadeKM < guaritaCheckList.Veiculo.KilometragemAtual)
                        manutencaoCorretiva = true;
                }
                else if (ultimoRealizado != null)
                {
                    if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorDia || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
                    {
                        DateTime dataUltimaExecucao = ultimoRealizado.OrdemServico.DataProgramada;

                        if (dataUltimaExecucao.AddDays(servico.ValidadeDias).Date <= DateTime.Now.Date)
                            manutencaoCorretiva = true;
                    }

                    if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorKM || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.Todos))
                    {
                        int kmUltimaExecucao = ultimoRealizado.OrdemServico.QuilometragemVeiculo;

                        if ((kmUltimaExecucao + servico.ValidadeKM) <= guaritaCheckList.Veiculo.KilometragemAtual)
                            manutencaoCorretiva = true;
                    }
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servicoGuaritaCheckList = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo()
                {
                    CustoEstimado = custoMedio,
                    CustoMedio = custoMedio,
                    Observacao = observacao,
                    GuaritaCheckList = guaritaCheckList,
                    Servico = servico,
                    TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva,
                    UltimaManutencao = ultimoRealizado,
                    TempoEstimado = tempoEstimado
                };

                repServicoGuaritaCheckList.Inserir(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Adicionou serviço da Manutenção", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.GestaoPatio.GuaritaCheckList.ObterDetalhesServico(servicoGuaritaCheckList));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a manutenção.");
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servicoGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorCodigo(codigo, true);

                if (servicoGuaritaCheckList == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                repServicoGuaritaCheckList.Deletar(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Excluiu serviço da Manutenção", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover a manutenção do check list.");
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
                int codigo, tempoEstimado;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("TempoEstimado"), out tempoEstimado);

                decimal custoEstimado;
                decimal.TryParse(Request.Params("CustoEstimado"), out custoEstimado);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servicoGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorCodigo(codigo, true);

                if (servicoGuaritaCheckList == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                if (!servicoGuaritaCheckList.Servico.PermiteLancamentoSemValor && custoEstimado <= 0m)
                    return new JsonpResult(false, true, "É necessário informar o custo estimado do serviço.");

                servicoGuaritaCheckList.CustoEstimado = custoEstimado;
                servicoGuaritaCheckList.Observacao = observacao;
                servicoGuaritaCheckList.TempoEstimado = tempoEstimado;

                repServicoGuaritaCheckList.Atualizar(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Atualizou dados de Serviço da Manutenção", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover a manutenção da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosUltimaExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoServico, codigoGuaritaCheckList;
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("GuaritaCheckList"), out codigoGuaritaCheckList);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unidadeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Guarita Check List não encontrada.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(codigoServico, guaritaCheckList.Veiculo?.Codigo ?? 0, 0);

                return new JsonpResult(new
                {
                    Codigo = ultimoRealizado?.Codigo ?? 0,
                    Quilometragem = ultimoRealizado?.OrdemServico.QuilometragemVeiculo.ToString("n2") ?? string.Empty,
                    Data = ultimoRealizado?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter a data da ultima execução do serviço para o veículo.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorGuaritaCheckList()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuaritaCheckList;
                int.TryParse(Request.Params("GuaritaCheckList"), out codigoGuaritaCheckList);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(codigoGuaritaCheckList);

                return new JsonpResult((from obj in servicosGuaritaCheckList select Servicos.Embarcador.GestaoPatio.GuaritaCheckList.ObterDetalhesServico(obj)).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os serviços de manutenção do check list.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarServicosParaManutencao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoGuaritaCheckList;
                int.TryParse(Request.Params("GuaritaCheckList"), out codigoGuaritaCheckList);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList, true);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o check list.");

                Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarManutencoesVeiculo(guaritaCheckList, unitOfWork, Auditado);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(codigoGuaritaCheckList);

                unitOfWork.CommitChanges();

                if (servicosGuaritaCheckList.Count > 0)
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, "Nenhum serviço localizado!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar serviços automaticamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Equipamento

        public async Task<IActionResult> AdicionarEquipamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("GuaritaCheckList"), out int codigoGuaritaCheckList);
                int.TryParse(Request.Params("Servico"), out int codigoServico);
                int.TryParse(Request.Params("TempoEstimado"), out int tempoEstimado);
                int.TryParse(Request.Params("Equipamento"), out int codigoEquipamento);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unidadeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repServicoVeiculo.BuscarPorCodigo(codigoServico);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Guarita Check List não encontrada.");

                if (servico == null)
                    return new JsonpResult(false, true, "Serviço não encontrado.");

                if (equipamento == null)
                    return new JsonpResult(false, true, "Equipamento não foi informado!");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(servico.Codigo, 0, codigoEquipamento);

                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
                bool manutencaoCorretiva = false;

                if (servico.ExecucaoUnica)
                {
                    if (servico.ValidadeHorimetro < equipamento.Horimetro)
                        manutencaoCorretiva = true;
                }
                else if (ultimoRealizado != null)
                {
                    if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorDia || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
                    {
                        DateTime dataUltimaExecucao = ultimoRealizado.OrdemServico.DataProgramada;

                        if (dataUltimaExecucao.AddDays(servico.ValidadeDias).Date <= DateTime.Now.Date)
                            manutencaoCorretiva = true;
                    }

                    if (servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorHorimetro || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
                    {
                        int horimetroUltimaExecucao = ultimoRealizado.OrdemServico.Horimetro;

                        if ((horimetroUltimaExecucao + servico.ValidadeHorimetro) <= equipamento.Horimetro)
                            manutencaoCorretiva = true;
                    }
                }

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servicoGuaritaCheckList = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento()
                {
                    CustoEstimado = custoMedio,
                    CustoMedio = custoMedio,
                    Observacao = observacao,
                    GuaritaCheckList = guaritaCheckList,
                    Servico = servico,
                    TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva,
                    UltimaManutencao = ultimoRealizado,
                    TempoEstimado = tempoEstimado
                };

                repServicoGuaritaCheckList.Inserir(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Adicionou serviço da Manutenção de Equipamento", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.GestaoPatio.GuaritaCheckList.ObterDetalhesServicoEquipamento(servicoGuaritaCheckList));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a manutenção de equipamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirEquipamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servicoGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorCodigo(codigo, true);

                if (servicoGuaritaCheckList == null)
                    return new JsonpResult(false, true, "Manutenção Equipamento não encontrada.");

                repServicoGuaritaCheckList.Deletar(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Excluiu serviço da Manutenção de Equipamento", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover a manutenção de equipamento do check list.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarEquipamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TempoEstimado"), out int tempoEstimado);

                decimal.TryParse(Request.Params("CustoEstimado"), out decimal custoEstimado);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unidadeTrabalho);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servicoGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorCodigo(codigo, true);

                if (servicoGuaritaCheckList == null)
                    return new JsonpResult(false, true, "Manutenção Equipamento não encontrada.");

                if (!servicoGuaritaCheckList.Servico.PermiteLancamentoSemValor && custoEstimado <= 0m)
                    return new JsonpResult(false, true, "É necessário informar o custo estimado do serviço.");

                servicoGuaritaCheckList.CustoEstimado = custoEstimado;
                servicoGuaritaCheckList.Observacao = observacao;
                servicoGuaritaCheckList.TempoEstimado = tempoEstimado;

                repServicoGuaritaCheckList.Atualizar(servicoGuaritaCheckList);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoGuaritaCheckList.GuaritaCheckList, null, "Atualizou dados de Serviço da Manutenção de Equipamento", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o serviço de manutenção de equipamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosUltimaExecucaoEquipamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Servico"), out int codigoServico);
                int.TryParse(Request.Params("GuaritaCheckList"), out int codigoGuaritaCheckList);
                int.TryParse(Request.Params("Equipamento"), out int codigoEquipamento);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Guarita Check List não encontrada.");

                if (codigoEquipamento == 0)
                    return new JsonpResult(false, true, "Equipamento não foi informado!");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(codigoServico, 0, codigoEquipamento);

                return new JsonpResult(new
                {
                    Codigo = ultimoRealizado?.Codigo ?? 0,
                    Quilometragem = ultimoRealizado?.OrdemServico.QuilometragemVeiculo.ToString("n2") ?? string.Empty,
                    Data = ultimoRealizado?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter a data da ultima execução do serviço para o equipamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorGuaritaCheckListEquipamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("GuaritaCheckList"), out int codigoGuaritaCheckList);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(codigoGuaritaCheckList);

                return new JsonpResult((from obj in servicosGuaritaCheckList select Servicos.Embarcador.GestaoPatio.GuaritaCheckList.ObterDetalhesServicoEquipamento(obj)).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os serviços de manutenção de equipamento do check list.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarServicosParaManutencaoEquipamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("GuaritaCheckList"), out int codigoGuaritaCheckList);
                int.TryParse(Request.Params("Equipamento"), out int codigoEquipamento);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigoGuaritaCheckList, true);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o check list.");

                if (codigoEquipamento == 0)
                    return new JsonpResult(false, true, "Equipamento não foi informado!");

                Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarManutencoesEquipamento(guaritaCheckList, codigoEquipamento, unitOfWork, Auditado);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(codigoGuaritaCheckList);

                unitOfWork.CommitChanges();

                if (servicosGuaritaCheckList.Count > 0)
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, "Nenhum serviço de equipamento localizado!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar serviços de equipamento automaticamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

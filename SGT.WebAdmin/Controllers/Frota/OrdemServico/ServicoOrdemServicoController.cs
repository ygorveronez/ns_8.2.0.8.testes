using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize("Frota/OrdemServico")]
    public class ServicoOrdemServicoController : BaseController
    {
		#region Construtores

		public ServicoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("OrdemServico"), out int codigoOrdemServico);
                int.TryParse(Request.Params("Servico"), out int codigoServico);
                int.TryParse(Request.Params("TempoEstimado"), out int tempoEstimado);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repServicoVeiculo.BuscarPorCodigo(codigoServico);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (servico == null)
                    return new JsonpResult(false, true, "Serviço não encontrado.");

                if (ConfiguracaoEmbarcador.BloquearLancamentoServicoDuplicadoOrdemServico && repServicoOrdemServico.ServicoJaEstaLancado(codigoServico, codigoOrdemServico))
                    return new JsonpResult(false, true, "Não é possível lançar o mesmo serviço na OS.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(servico.Codigo, ordemServico.Veiculo?.Codigo ?? 0, ordemServico.Equipamento?.Codigo ?? 0);

                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
                bool manutencaoCorretiva = false;

                if (servico.ExecucaoUnica)
                {
                    if (ordemServico.Veiculo != null && servico.ValidadeKM < ordemServico.Veiculo.KilometragemAtual)
                        manutencaoCorretiva = true;
                    if (ordemServico.Equipamento != null && servico.ValidadeHorimetro < ordemServico.Equipamento.Horimetro)
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

                    if (ordemServico.Veiculo != null && servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorKM || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.Todos))
                    {
                        int kmUltimaExecucao = ultimoRealizado.OrdemServico.QuilometragemVeiculo;

                        if ((kmUltimaExecucao + servico.ValidadeKM) <= ordemServico.Veiculo.KilometragemAtual)
                            manutencaoCorretiva = true;
                    }

                    if (ordemServico.Equipamento != null && servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorHorimetro || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
                    {
                        int horimetroUltimaExecucao = ultimoRealizado.OrdemServico.Horimetro;

                        if ((horimetroUltimaExecucao + servico.ValidadeHorimetro) <= ordemServico.Equipamento.Horimetro)
                            manutencaoCorretiva = true;
                    }
                }

                unidadeTrabalho.Start();

                TipoManutencaoServicoVeiculoOrdemServicoFrota tipoManutencao = servico.TipoManutencao != TipoManutencaoServicoVeiculo.Outros ?
                    servico.TipoManutencao == TipoManutencaoServicoVeiculo.Corretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva :
                    manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo()
                {
                    CustoEstimado = custoMedio,
                    CustoMedio = custoMedio,
                    Observacao = observacao,
                    OrdemServico = ordemServico,
                    Servico = servico,
                    TipoManutencao = tipoManutencao,
                    UltimaManutencao = ultimoRealizado,
                    TempoEstimado = tempoEstimado,
                    TempoExecutado = 0
                };

                repServicoOrdemServico.Inserir(servicoOrdemServico);

                Servicos.Embarcador.Frota.OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServicoManutencao.ObterDetalhesServico(servicoOrdemServico));
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

        public async Task<IActionResult> AdicionarMultiplos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrdemServico = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                List<int> codigosServicos = Request.GetListParam<int>("Servicos");
                if (codigosServicos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum serviço selecionado.");

                unidadeTrabalho.Start();

                List<object> retorno = new List<object>();

                foreach (int codigoServico in codigosServicos)
                {
                    Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repServicoVeiculo.BuscarPorCodigo(codigoServico);

                    if (servico == null)
                        throw new ControllerException("Serviço não encontrado.");

                    if (ConfiguracaoEmbarcador.BloquearLancamentoServicoDuplicadoOrdemServico && repServicoOrdemServico.ServicoJaEstaLancado(codigoServico, codigoOrdemServico))
                        throw new ControllerException("Não é possível lançar o mesmo serviço na OS.");

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(servico.Codigo, ordemServico.Veiculo?.Codigo ?? 0, ordemServico.Equipamento?.Codigo ?? 0);

                    decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
                    bool manutencaoCorretiva = false;

                    if (servico.ExecucaoUnica)
                    {
                        if (ordemServico.Veiculo != null && servico.ValidadeKM < ordemServico.Veiculo.KilometragemAtual)
                            manutencaoCorretiva = true;
                        if (ordemServico.Equipamento != null && servico.ValidadeHorimetro < ordemServico.Equipamento.Horimetro)
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

                        if (ordemServico.Veiculo != null && servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorKM || servico.Tipo == TipoServicoVeiculo.Ambos || servico.Tipo == TipoServicoVeiculo.Todos))
                        {
                            int kmUltimaExecucao = ultimoRealizado.OrdemServico.QuilometragemVeiculo;

                            if ((kmUltimaExecucao + servico.ValidadeKM) <= ordemServico.Veiculo.KilometragemAtual)
                                manutencaoCorretiva = true;
                        }

                        if (ordemServico.Equipamento != null && servico.Tipo != TipoServicoVeiculo.Nenhum && (servico.Tipo == TipoServicoVeiculo.PorHorimetro || servico.Tipo == TipoServicoVeiculo.Todos || servico.Tipo == TipoServicoVeiculo.PorHorimetroDia))
                        {
                            int horimetroUltimaExecucao = ultimoRealizado.OrdemServico.Horimetro;

                            if ((horimetroUltimaExecucao + servico.ValidadeHorimetro) <= ordemServico.Equipamento.Horimetro)
                                manutencaoCorretiva = true;
                        }
                    }

                    TipoManutencaoServicoVeiculoOrdemServicoFrota tipoManutencao = servico.TipoManutencao != TipoManutencaoServicoVeiculo.Outros ?
                        servico.TipoManutencao == TipoManutencaoServicoVeiculo.Corretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva :
                        manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo()
                    {
                        CustoEstimado = custoMedio,
                        CustoMedio = custoMedio,
                        Observacao = string.Empty,
                        OrdemServico = ordemServico,
                        Servico = servico,
                        TipoManutencao = tipoManutencao,
                        UltimaManutencao = ultimoRealizado,
                        TempoEstimado = servico.TempoEstimado,
                        TempoExecutado = 0
                    };

                    repServicoOrdemServico.Inserir(servicoOrdemServico);

                    retorno.Add(Servicos.Embarcador.Frota.OrdemServicoManutencao.ObterDetalhesServico(servicoOrdemServico));
                }

                Servicos.Embarcador.Frota.OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os serviços.");
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

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrdemServicoFrotaOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrdemServicoFrotaOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = repServicoOrdemServico.BuscarPorCodigo(codigo);

                if (servicoOrdemServico == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico ordemServicoFrotaOrcamentoServico = repOrdemServicoFrotaOrcamentoServico.BuscarPorManutencao(codigo);
                if (ordemServicoFrotaOrcamentoServico != null)
                {
                    List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> listaProdutoOrcamento = repOrdemServicoFrotaOrcamentoServicoProduto.BuscarPorOrcamentoServico(ordemServicoFrotaOrcamentoServico.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto produtoOrcamento in listaProdutoOrcamento)
                        repOrdemServicoFrotaOrcamentoServicoProduto.Deletar(produtoOrcamento);

                    repOrdemServicoFrotaOrcamentoServico.Deletar(ordemServicoFrotaOrcamentoServico);
                }
                repServicoOrdemServico.Deletar(servicoOrdemServico);

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

        public async Task<IActionResult> ExcluirTodosServicos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrdemServico = Request.GetIntParam("OrdemServico");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicosOrdemServico = repServicoOrdemServico.BuscarPorOrdemServico(codigoOrdemServico);

                if (servicosOrdemServico.Count == 0)
                    return new JsonpResult(false, true, "Nenhum serviço encontrado.");

                unidadeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico in servicosOrdemServico)
                    repServicoOrdemServico.Deletar(servicoOrdemServico);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover a manutenção da ordem de serviço.");
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

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = repServicoOrdemServico.BuscarPorCodigo(codigo);

                if (servicoOrdemServico == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                if (!servicoOrdemServico.Servico.PermiteLancamentoSemValor && custoEstimado <= 0m)
                    return new JsonpResult(false, true, "É necessário informar o custo estimado do serviço.");

                servicoOrdemServico.CustoEstimado = custoEstimado;
                servicoOrdemServico.Observacao = observacao;
                servicoOrdemServico.TempoEstimado = tempoEstimado;

                repServicoOrdemServico.Atualizar(servicoOrdemServico);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar a manutenção da ordem de serviço.");
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
                int codigoServico, codigoOrdemServico;
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("OrdemServico"), out codigoOrdemServico);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(codigoServico, ordemServico.Veiculo?.Codigo ?? 0, ordemServico.Equipamento?.Codigo ?? 0);

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

        public async Task<IActionResult> BuscarPorOrdemServico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrdemServico;
                int.TryParse(Request.Params("OrdemServico"), out codigoOrdemServico);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicosOrdemServico = repServicoOrdemServico.BuscarPorOrdemServico(codigoOrdemServico);

                return new JsonpResult((from obj in servicosOrdemServico select Servicos.Embarcador.Frota.OrdemServicoManutencao.ObterDetalhesServico(obj)).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar as manutenções da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarNaoExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = repServicoOrdemServico.BuscarPorCodigo(codigo);

                if (servicoOrdemServico == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                if (servicoOrdemServico.NaoExecutado)
                    return new JsonpResult(false, true, "Já foi informado que o serviço não foi executado.");

                servicoOrdemServico.NaoExecutado = true;

                repServicoOrdemServico.Atualizar(servicoOrdemServico);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao informar a não execução da manutenção da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}

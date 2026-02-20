using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroManutencaoController : BaseController
    {
        #region Construtores

        public SinistroManutencaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSinistro = Request.GetIntParam("Sinistro");
                int codigoServico = Request.GetIntParam("Servico");
                int tempoEstimado = Request.GetIntParam("TempoEstimado");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.SinistroServico repSinistroServico = new Repositorio.Embarcador.Frota.SinistroServico(unidadeTrabalho);
                Repositorio.Embarcador.Frota.SinistroDados repSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro = repSinistroDados.BuscarPorCodigo(codigoSinistro);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico = repServicoVeiculo.BuscarPorCodigo(codigoServico);

                if (sinistro == null)
                    return new JsonpResult(false, true, "Sinistro não encontrado.");

                if (servico == null)
                    return new JsonpResult(false, true, "Serviço não encontrado.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(servico.Codigo, sinistro.Veiculo?.Codigo ?? 0, 0);

                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
                bool manutencaoCorretiva = false;

                if (servico.ExecucaoUnica)
                {
                    if (servico.ValidadeKM < sinistro.Veiculo.KilometragemAtual)
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

                        if ((kmUltimaExecucao + servico.ValidadeKM) <= sinistro.Veiculo.KilometragemAtual)
                            manutencaoCorretiva = true;
                    }
                }

                unidadeTrabalho.Start();

                TipoManutencaoServicoVeiculoOrdemServicoFrota tipoManutencao = servico.TipoManutencao != TipoManutencaoServicoVeiculo.Outros ?
                    servico.TipoManutencao == TipoManutencaoServicoVeiculo.Corretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva :
                    manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                Dominio.Entidades.Embarcador.Frota.SinistroServico servicoSinistro = new Dominio.Entidades.Embarcador.Frota.SinistroServico()
                {
                    CustoEstimado = custoMedio,
                    CustoMedio = custoMedio,
                    Observacao = observacao,
                    Sinistro = sinistro,
                    Servico = servico,
                    TipoManutencao = tipoManutencao,
                    UltimaManutencao = ultimoRealizado,
                    TempoEstimado = tempoEstimado
                };

                repSinistroServico.Inserir(servicoSinistro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoSinistro.Sinistro, null, $"Adicionou o serviço {servico.Descricao} na Manutenção", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                Servicos.Embarcador.Frota.SinistroDados srcSinistro = new Servicos.Embarcador.Frota.SinistroDados();
                return new JsonpResult(srcSinistro.ObterDetalhesServico(servicoSinistro));
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.SinistroServico repSinistroServico = new Repositorio.Embarcador.Frota.SinistroServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroServico servicoSinistro = repSinistroServico.BuscarPorCodigo(codigo, false);

                if (servicoSinistro == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                repSinistroServico.Deletar(servicoSinistro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, servicoSinistro.Sinistro, null, $"Excluiu o serviço {servicoSinistro.Servico.Descricao} da Manutenção", unidadeTrabalho);

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int tempoEstimado = Request.GetIntParam("TempoEstimado");

                decimal.TryParse(Request.Params("CustoEstimado"), out decimal custoEstimado);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Frota.SinistroServico repSinistroServico = new Repositorio.Embarcador.Frota.SinistroServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroServico servicoSinistro = repSinistroServico.BuscarPorCodigo(codigo, false);

                if (servicoSinistro == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                if (!servicoSinistro.Servico.PermiteLancamentoSemValor && custoEstimado <= 0m)
                    return new JsonpResult(false, true, "É necessário informar o custo estimado do serviço.");

                servicoSinistro.CustoEstimado = custoEstimado;
                servicoSinistro.Observacao = observacao;
                servicoSinistro.TempoEstimado = tempoEstimado;

                repSinistroServico.Atualizar(servicoSinistro);

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
                int codigoServico = Request.GetIntParam("Servico");
                int codigoSinistro = Request.GetIntParam("Sinistro");

                Repositorio.Embarcador.Frota.SinistroDados repSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro = repSinistroDados.BuscarPorCodigo(codigoSinistro);

                if (sinistro == null)
                    return new JsonpResult(false, true, "Sinistro não encontrado.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo ultimoRealizado = repServicoOrdemServico.BuscarUltimoRealizado(codigoServico, sinistro.Veiculo?.Codigo ?? 0, 0);

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

        public async Task<IActionResult> BuscarPorSinistro()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSinistro = Request.GetIntParam("Sinistro");

                Repositorio.Embarcador.Frota.SinistroServico repSinistroServico = new Repositorio.Embarcador.Frota.SinistroServico(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Frota.SinistroServico> servicos = repSinistroServico.BuscarPorSinistro(codigoSinistro);

                Servicos.Embarcador.Frota.SinistroDados srcSinistro = new Servicos.Embarcador.Frota.SinistroDados();

                return new JsonpResult((from obj in servicos select srcSinistro.ObterDetalhesServico(obj)).ToList());
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

        public async Task<IActionResult> CriarOrdemServico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSinistro = Request.GetIntParam("Sinistro");

                int codigoTipoOrdemServico = Request.GetIntParam("TipoOrdemServico");
                double cnpjLocalManutencao = Request.GetDoubleParam("LocalManutencao");

                Repositorio.Embarcador.Frota.SinistroDados repSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro = repSinistroDados.BuscarPorCodigo(codigoSinistro, true);

                if (sinistro == null)
                    return new JsonpResult(false, true, "Sinistro não encontrado.");

                if (sinistro.Etapa != EtapaSinistro.Manutencao)
                    return new JsonpResult(false, true, "Sinistro não está mais na etapa de manutenção!");

                if (sinistro.OrdemServico != null)//Validação para quando volta etapa, vai que habilitam o botão e tentam novamente
                    return new JsonpResult(false, true, "Ordem de serviço já foi gerada! Não sendo possível gerar novamente.");

                unidadeTrabalho.Start();

                sinistro.DataProgramada = Request.GetNullableDateTimeParam("DataProgramada");
                sinistro.ObservacaoOS = Request.GetStringParam("ObservacaoOS");
                sinistro.TipoOrdemServico = codigoTipoOrdemServico > 0 ? repTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico) : null;
                sinistro.LocalManutencao = cnpjLocalManutencao > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjLocalManutencao) : null;
                sinistro.Etapa = EtapaSinistro.IndicacaoPagador;

                SalvarOrdensServico(sinistro, unidadeTrabalho);

                repSinistroDados.Atualizar(sinistro, Auditado);

                InserirOrdemServico(sinistro, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new { sinistro.Codigo }, true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao criar a ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void InserirOrdemServico(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unitOfWork)
        {
            if (sinistro.TipoOrdemServico == null)
                return;

            Repositorio.Embarcador.Frota.SinistroDados repSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);
            Repositorio.Embarcador.Frota.SinistroServico repSinistroServico = new Repositorio.Embarcador.Frota.SinistroServico(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.SinistroServico> servicos = repSinistroServico.BuscarPorSinistro(sinistro.Codigo);
            if (servicos.Count == 0)
                return;

            Dominio.Entidades.Veiculo veiculo = sinistro.Veiculo;

            Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico()
            {
                DataProgramada = sinistro.DataProgramada?.Date ?? DateTime.Now.Date,
                LocalManutencao = sinistro.LocalManutencao,
                Motorista = sinistro.Motorista,
                Observacao = "GERADO A PARTIR DO FLUXO DE SINISTRO N°" + sinistro.Numero.ToString() + (!string.IsNullOrWhiteSpace(sinistro.ObservacaoOS) ? " - " + sinistro.ObservacaoOS : ""),
                Operador = Usuario,
                Veiculo = veiculo,
                QuilometragemVeiculo = veiculo?.KilometragemAtual ?? 0,
                TipoOrdemServico = sinistro.TipoOrdemServico,
                Servicos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos>()
            };

            List<Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos> objetoServicos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos>();
            foreach (Dominio.Entidades.Embarcador.Frota.SinistroServico servico in servicos)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos objetoServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos()
                {
                    Servico = servico.Servico,
                    CustoEstimado = servico.CustoEstimado,
                    CustoMedio = servico.CustoMedio,
                    Observacao = servico.Observacao,
                    TempoEstimado = servico.TempoEstimado,
                    TipoManutencao = servico.TipoManutencao,
                    UltimaManutencao = servico.UltimaManutencao
                };

                objetoServicos.Add(objetoServico);
            }

            objetoOrdemServico.Servicos.AddRange(objetoServicos);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = Servicos.Embarcador.Frota.OrdemServico.AbrirOrdemServico(objetoOrdemServico, Auditado, unitOfWork);

            sinistro.OrdemServico = ordemServico;
            repSinistroDados.Atualizar(sinistro);
        }

        private void SalvarOrdensServico(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.SinistroOrdemServico repositorioSinistroOrdemServico = new Repositorio.Embarcador.Frota.SinistroOrdemServico(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

            dynamic dynOrdensServico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OrdensServico"));

            List<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico> ordensServico = repositorioSinistroOrdemServico.BuscarPorFluxoSinistro(sinistro.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            if (ordensServico.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynOrdemServico in dynOrdensServico)
                    if (dynOrdemServico.Codigo != null)
                        codigos.Add((int)dynOrdemServico.Codigo);

                List<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico> ordensServicoDeletar = (from obj in ordensServico where !codigos.Contains(obj.OrdemServico.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico ordemServicoDeletar in ordensServicoDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "OrdensServico",
                        De = ordemServicoDeletar.OrdemServico.Descricao,
                        Para = ""
                    });

                    repositorioSinistroOrdemServico.Deletar(ordemServicoDeletar);
                }
            }

            foreach (var dynOrdemServico in dynOrdensServico)
            {
                int codigoOrdemServico = ((string)dynOrdemServico.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico sinistroOrdemServico = codigoOrdemServico > 0 ? repositorioSinistroOrdemServico.BuscarPorFluxoSinistroEOrdemServico(sinistro.Codigo, codigoOrdemServico) : null;

                if (sinistroOrdemServico == null)
                {
                    sinistroOrdemServico = new Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico()
                    {
                        Sinistro = sinistro,
                        OrdemServico = repositorioOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServico)
                    };
                    repositorioSinistroOrdemServico.Inserir(sinistroOrdemServico);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "OrdensServico",
                        De = "",
                        Para = sinistroOrdemServico.OrdemServico.Descricao
                    });
                }
            }

            sinistro.SetExternalChanges(alteracoes);
        }

        #endregion
    }
}

using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using System.Threading.Tasks;
using System.Threading;

namespace Servicos.Embarcador.Frota
{
    public class OrdemServicoManutencao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public OrdemServicoManutencao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static object ObterDetalhesServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico)
        {
            return new
            {
                servico.Codigo,
                Servico = new
                {
                    servico.Servico.Codigo,
                    servico.Servico.Descricao
                },
                servico.CustoEstimado,
                servico.CustoMedio,
                servico.Observacao,
                servico.TipoManutencao,
                servico.DescricaoTipoManutencao,
                DataUltimaManutencao = servico.UltimaManutencao?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty,
                QuilometragemUltimaManutencao = servico.UltimaManutencao?.OrdemServico.QuilometragemVeiculo,
                servico.TempoEstimado,
                servico.TempoExecutado,
                servico.NaoExecutado
            };
        }

        public static void IniciarManutencaoVeiculo(int codigoVeiculo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (!configuracaoTMS.NaoControlarSituacaoVeiculoOrdemServico)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    if (veiculo.SituacaoVeiculo != SituacaoVeiculo.EmManutencao)
                    {
                        veiculo.SituacaoVeiculo = SituacaoVeiculo.EmManutencao;
                        veiculo.DataHoraPrevisaoDisponivel = ordemServico.DataProgramada;
                        if (ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                            veiculo.LocalidadeAtual = ordemServico.LocalManutencao.Localidade;
                        veiculo.VeiculoVazio = false;
                        veiculo.AvisadoCarregamento = false;

                        repVeiculo.Atualizar(veiculo);

                        Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                        situacao.DataHoraEmissao = DateTime.Now;
                        situacao.DataHoraSituacao = ordemServico.DataProgramada;
                        situacao.Veiculo = veiculo;

                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                        if (veiculoMotorista != null)
                            situacao.Motorista = veiculoMotorista;

                        if (ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                            situacao.Localidade = ordemServico.LocalManutencao.Localidade;
                        situacao.DataHoraEntradaManutencao = DateTime.Now;
                        situacao.DataHoraPrevisaoSaidaManutencao = ordemServico.DataProgramada;
                        situacao.Situacao = SituacaoVeiculo.EmManutencao;

                        repSituacaoVeiculo.Inserir(situacao);

                        if (Auditado != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Início da Manutenção " + ordemServico.Numero.ToString("n0"), unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Início da Manutenção " + ordemServico.Numero.ToString("n0"), unitOfWork);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void AtualizarKMVeiculoPneu(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (ordemServico.QuilometragemVeiculo > 0 && ordemServico.Veiculo != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                //atualiza km do veículo e seus pneus
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(ordemServico.Veiculo.Codigo);
                int qtdKMRodado = 0;
                if (veiculo != null && veiculo.KilometragemAtual < ordemServico.QuilometragemVeiculo)
                {
                    qtdKMRodado = ordemServico.QuilometragemVeiculo - veiculo.KilometragemAtual;

                    if (veiculo.Pneus?.Count > 0 && qtdKMRodado > 0)
                    {
                        foreach (Dominio.Entidades.VeiculoPneu eixo in veiculo.Pneus)
                        {
                            Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                            if (pneu != null)
                            {
                                pneu.KmAnteriorRodado = pneu.KmAtualRodado;
                                pneu.KmAtualRodado = pneu.KmAtualRodado + qtdKMRodado;
                                if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                    pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                repPneu.Atualizar(pneu);
                            }
                        }
                    }

                    veiculo.KilometragemAnterior = veiculo.KilometragemAtual;
                    if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        veiculo.KilometragemAtual = ordemServico.QuilometragemVeiculo;

                    repVeiculo.Atualizar(veiculo, auditado, null, $"Atualizada a Quilometragem Atual do Veículo via finalização de Ordem de Serviço");
                }

                //atualiza km dos reboques e seus pneus
                if (veiculo != null && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    {
                        if (reboque != null && qtdKMRodado > 0)
                        {
                            if (reboque.Pneus?.Count > 0 && qtdKMRodado > 0)
                            {
                                foreach (Dominio.Entidades.VeiculoPneu eixo in reboque.Pneus)
                                {
                                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                    if (pneu != null)
                                    {
                                        pneu.KmAnteriorRodado = pneu.KmAtualRodado;
                                        pneu.KmAtualRodado = pneu.KmAtualRodado + qtdKMRodado;
                                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                        repPneu.Atualizar(pneu);
                                    }
                                }
                            }

                            reboque.KilometragemAnterior = reboque.KilometragemAtual;
                            if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita && qtdKMRodado > 0)
                                reboque.KilometragemAtual = reboque.KilometragemAtual + qtdKMRodado;
                            repVeiculo.Atualizar(reboque);

                            repVeiculo.Atualizar(reboque, auditado, null, $"Atualizada a Quilometragem Atual do Reboque via finalização de Ordem de Serviço");
                        }
                    }
                }
            }

            if (ordemServico.Equipamento != null)
            {
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

                //atualiza hodômetro e horímetro do equipamento
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(ordemServico.Equipamento.Codigo);
                if (equipamento != null && ordemServico.QuilometragemVeiculo > 0 && equipamento.Hodometro < ordemServico.QuilometragemVeiculo)
                {
                    equipamento.Hodometro = ordemServico.QuilometragemVeiculo;
                    repEquipamento.Atualizar(equipamento);
                }
                if (equipamento != null && ordemServico.Horimetro > 0 && equipamento.Horimetro < ordemServico.Horimetro)
                {
                    if (equipamento != null)
                    {
                        if (equipamento.TrocaHorimetro)
                        {
                            if (equipamento != null && equipamento.HorimetroAtual < ordemServico.Horimetro)
                            {
                                equipamento.Horimetro = equipamento.Horimetro + (ordemServico.Horimetro - equipamento.HorimetroAtual);
                                equipamento.HorimetroAtual = ordemServico.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                        else
                        {
                            if (equipamento != null && equipamento.Horimetro < ordemServico.Horimetro)
                            {
                                equipamento.Horimetro = ordemServico.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                    }
                }
            }
        }

        public static void ReverterAtualizacaoKMVeiculoPneu(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (ordemServico.QuilometragemVeiculo > 0 && ordemServico.Veiculo != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                //atualiza km do veículo e seus pneus              
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(ordemServico.Veiculo.Codigo);
                if (veiculo != null && veiculo.KilometragemAtual == ordemServico.QuilometragemVeiculo && veiculo.KilometragemAnterior > 0 && veiculo.KilometragemAnterior != veiculo.KilometragemAtual)
                {
                    if (veiculo.Pneus?.Count > 0)
                    {
                        foreach (Dominio.Entidades.VeiculoPneu eixo in veiculo.Pneus)
                        {
                            Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                            if (pneu != null && pneu.KmAnteriorRodado > 0 && pneu.KmAnteriorRodado != pneu.KmAtualRodado)
                            {
                                pneu.KmAtualRodado = pneu.KmAnteriorRodado;
                                if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                    pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                repPneu.Atualizar(pneu);
                            }
                        }
                    }
                    if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        veiculo.KilometragemAtual = veiculo.KilometragemAnterior;
                    repVeiculo.Atualizar(veiculo);

                    repVeiculo.Atualizar(veiculo, auditado, null, $"Atualizada a Quilometragem Atual do Veículo via cancelamento de Ordem de Serviço");
                }

                //atualiza km dos reboques e seus pneus
                if (veiculo != null && veiculo.VeiculosVinculados?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    {
                        if (reboque != null && reboque.KilometragemAtual == ordemServico.QuilometragemVeiculo && reboque.KilometragemAnterior > 0 && reboque.KilometragemAnterior != reboque.KilometragemAtual)
                        {
                            if (reboque.Pneus?.Count > 0)
                            {
                                foreach (Dominio.Entidades.VeiculoPneu eixo in reboque.Pneus)
                                {
                                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                    if (pneu != null && pneu.KmAnteriorRodado > 0 && pneu.KmAnteriorRodado != pneu.KmAtualRodado)
                                    {
                                        pneu.KmAtualRodado = pneu.KmAnteriorRodado;
                                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                        repPneu.Atualizar(pneu);
                                    }
                                }
                            }
                            if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                                reboque.KilometragemAtual = reboque.KilometragemAnterior;

                            repVeiculo.Atualizar(reboque, auditado, null, $"Atualizada a Quilometragem Atual do Reboque via finalização de Ordem de Serviço");
                        }
                    }
                }
            }
        }

        public static void FinalizarManutencaoVeiculo(int codigoVeiculo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool naoControlarSituacaoVeiculo)
        {
            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);

                if (!naoControlarSituacaoVeiculo)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    if (veiculo.SituacaoVeiculo == SituacaoVeiculo.EmManutencao)
                    {
                        veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                        veiculo.DataHoraPrevisaoDisponivel = null;
                        veiculo.VeiculoVazio = true;
                        if (ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                            veiculo.LocalidadeAtual = ordemServico.LocalManutencao.Localidade;
                        else
                            veiculo.LocalidadeAtual = null;

                        repVeiculo.Atualizar(veiculo);

                        Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = null;
                        List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> situacoes = repSituacaoVeiculo.BuscarHistoricoPorVeículo(veiculo.Codigo);
                        if (situacoes.Count > 0)
                            situacao = situacoes.OrderByDescending(situacao => situacao.Codigo).FirstOrDefault(situacao => situacao.Situacao == SituacaoVeiculo.EmManutencao && !situacao.DataHoraSaidaManutencao.HasValue);

                        situacao ??= new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                        situacao.Initialize();

                        situacao.DataHoraEmissao = DateTime.Now;
                        situacao.DataHoraSituacao = ordemServico.DataProgramada;
                        situacao.Veiculo = veiculo;

                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                        if (veiculoMotorista != null)
                            situacao.Motorista = veiculoMotorista;

                        if (ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                            situacao.Localidade = ordemServico.LocalManutencao.Localidade;
                        situacao.DataHoraSaidaManutencao = ordemServico.DataFechamento;
                        situacao.OrdemServicoFrota = ordemServico;
                        situacao.Situacao = SituacaoVeiculo.EmManutencao;

                        if (situacao.Codigo > 0)
                            repSituacaoVeiculo.Atualizar(situacao, Auditado);
                        else
                            repSituacaoVeiculo.Inserir(situacao, Auditado);

                        if (Auditado != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Fim da Manutenção " + ordemServico.Numero.ToString("n0"), unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Fim da Manutenção " + ordemServico.Numero.ToString("n0"), unitOfWork);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public void VerificarSeNecessariaAutorizacaoServicoVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga repVeiculoServicoAutorizacaoCarga = new Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga(_unitOfWork);

            if (!configuracao.ValidarServicoPendenteVeiculoExecucaoCarga)
                return;

            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

            if (carga.Veiculo != null)
                veiculos.Add(carga.Veiculo);

            if (carga.VeiculosVinculados?.Count > 0)
                veiculos.AddRange(carga.VeiculosVinculados);

            List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentes = ObterManutencoesObrigatoriasParaCarga(veiculos, configuracao);

            if (servicosPendentes == null || servicosPendentes.Count <= 0)
                return;

            StringBuilder mensagem = new StringBuilder();

            mensagem.Append("Existem manutenções obrigatórias pendentes para os veículos");

            List<string> placas = servicosPendentes.Select(o => o.Placa).Distinct().ToList();

            foreach (string placa in placas)
                mensagem.Append(" ").Append(placa.ToUpper()).Append(" (").Append(string.Join(", ", servicosPendentes.Where(o => o.Placa == placa).Select(o => o.DescricaoServico))).Append(")");

            mensagem.Append(", não sendo possível prosseguir com a emissão.");

            Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga veiculoServicoAutorizacaoCarga = new Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga()
            {
                Carga = carga,
                Mensagem = Utilidades.String.Left(mensagem.ToString(), 500),
                Situacao = SituacaoAutorizacaoServicoVeiculoCarga.AgLiberacao
            };

            repVeiculoServicoAutorizacaoCarga.Inserir(veiculoServicoAutorizacaoCarga);
        }

        public static void ExtornarAutorizacoesServicoVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string motivoExtorno)
        {
            Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga repVeiculoServicoAutorizacaoCarga = new Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga> autorizacoesExtornar = repVeiculoServicoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga autorizacaoExtornar in autorizacoesExtornar)
            {
                if (autorizacaoExtornar.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.Liberada)
                {
                    autorizacaoExtornar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.AutorizacaoExtornada;
                    autorizacaoExtornar.MotivoExtornoAutorizacao = motivoExtorno;

                    repVeiculoServicoAutorizacaoCarga.Atualizar(autorizacaoExtornar);
                }
                else
                    repVeiculoServicoAutorizacaoCarga.Deletar(autorizacaoExtornar);
            }
        }

        public static bool VerificarSeEnecessarioAutorizacaoServicoVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            mensagem = string.Empty;

            Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga repVeiculoServicoAutorizacaoCarga = new Repositorio.Embarcador.Frota.VeiculoServicoAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga> autorizacoes = repVeiculoServicoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga autorizacao in autorizacoes)
            {
                if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.AgLiberacao)
                {
                    mensagem = autorizacao.Mensagem;
                    return true;
                }
            }

            return false;
        }

        public static void GerarManutencoesVeiculo(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (ordemServico.Veiculo == null && ordemServico.Equipamento == null)
                return;

            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int codigoGrupoServico = ordemServico.GrupoServico?.Codigo ?? 0;
            if (ordemServico.Veiculo != null && (!ordemServico.LancarServicosManualmente.HasValue || !ordemServico.LancarServicosManualmente.Value))
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentes = repServicoVeiculo.BuscarPendentesPorVeiculo(ordemServico.Veiculo.Codigo,
                    ordemServico.QuilometragemVeiculo > 0 ? ordemServico.QuilometragemVeiculo : ordemServico.Veiculo.KilometragemAtual, codigoGrupoServico, configuracaoTMS.UtilizarValidadeServicoPeloGrupoServicoOrdemServico);

                for (int i = 0; i < servicosPendentes.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico servicoPendente = servicosPendentes[i];
                    decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.CodigoServico);

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                    servico.CustoMedio = custoMedio;
                    servico.CustoEstimado = custoMedio;
                    servico.TempoEstimado = servicoPendente.TempoEstimado;
                    servico.TempoExecutado = 0;
                    servico.Observacao = string.Empty;
                    servico.OrdemServico = ordemServico;
                    servico.Servico = repServicoVeiculo.BuscarPorCodigo(servicoPendente.CodigoServico);

                    bool manutencaoCorretiva = false;

                    if ((servicoPendente.TipoServico == TipoServicoVeiculo.Ambos || servicoPendente.TipoServico == TipoServicoVeiculo.PorDia || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                        servicoPendente.DataUltimaExecucao.AddDays(servicoPendente.ValidadeDiasServico) <= DateTime.Now.Date)
                        manutencaoCorretiva = true;

                    if ((servicoPendente.TipoServico == TipoServicoVeiculo.Ambos || servicoPendente.TipoServico == TipoServicoVeiculo.PorKM) &&
                        (servicoPendente.QuilometragemUltimaExecucao + servicoPendente.ValidadeQuilometrosServico) <= servicoPendente.QuilometragemAtual)
                        manutencaoCorretiva = true;

                    servico.TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                    servico.UltimaManutencao = repServicoOrdemServico.BuscarUltimoRealizado(servicoPendente.CodigoServico, ordemServico.Veiculo?.Codigo ?? 0, ordemServico.Equipamento?.Codigo ?? 0);

                    repServicoOrdemServico.Inserir(servico);
                }

                List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicosPendentesExecucaoUnica = repServicoVeiculo.BuscarPendentesExecucaoUnicaPorVeiculo(ordemServico.Veiculo,
                    ordemServico.QuilometragemVeiculo > 0 ? ordemServico.QuilometragemVeiculo : ordemServico.Veiculo.KilometragemAtual, codigoGrupoServico, configuracaoTMS.UtilizarValidadeServicoPeloGrupoServicoOrdemServico);

                for (int i = 0; i < servicosPendentesExecucaoUnica.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoPendente = servicosPendentesExecucaoUnica[i];
                    decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.Codigo);

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                    servico.CustoMedio = custoMedio;
                    servico.CustoEstimado = custoMedio;
                    servico.TempoEstimado = servicoPendente.TempoEstimado;
                    servico.TempoExecutado = 0;
                    servico.Observacao = string.Empty;
                    servico.OrdemServico = ordemServico;
                    servico.Servico = servicoPendente;

                    if (servicoPendente.ValidadeKM <= ordemServico.Veiculo.KilometragemAtual)
                        servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva;
                    else
                        servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;

                    repServicoOrdemServico.Inserir(servico);
                }
            }

            if (ordemServico.Equipamento != null && (!ordemServico.LancarServicosManualmente.HasValue || !ordemServico.LancarServicosManualmente.Value))
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentesEquipamento = repServicoVeiculo.BuscarPendentesPorEquipamento(ordemServico.Equipamento.Codigo,
                    (ordemServico.QuilometragemVeiculo > 0 ? ordemServico.QuilometragemVeiculo : ordemServico.Equipamento.Hodometro),
                    (ordemServico.Horimetro > 0 ? ordemServico.Horimetro : ordemServico.Equipamento.Horimetro), codigoGrupoServico, configuracaoTMS.UtilizarValidadeServicoPeloGrupoServicoOrdemServico);

                for (int i = 0; i < servicosPendentesEquipamento.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico servicoPendente = servicosPendentesEquipamento[i];
                    decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.CodigoServico);

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                    servico.CustoMedio = custoMedio;
                    servico.CustoEstimado = custoMedio;
                    servico.TempoEstimado = servicoPendente.TempoEstimado;
                    servico.TempoExecutado = 0;
                    servico.Observacao = string.Empty;
                    servico.OrdemServico = ordemServico;
                    servico.Servico = repServicoVeiculo.BuscarPorCodigo(servicoPendente.CodigoServico);

                    bool manutencaoCorretiva = false;

                    if ((servicoPendente.TipoServico == TipoServicoVeiculo.Todos || servicoPendente.TipoServico == TipoServicoVeiculo.Ambos ||
                        servicoPendente.TipoServico == TipoServicoVeiculo.PorDia || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                        servicoPendente.DataUltimaExecucao.AddDays(servicoPendente.ValidadeDiasServico) <= DateTime.Now.Date)
                        manutencaoCorretiva = true;

                    if ((servicoPendente.TipoServico == TipoServicoVeiculo.Todos || servicoPendente.TipoServico == TipoServicoVeiculo.Ambos || servicoPendente.TipoServico == TipoServicoVeiculo.PorKM) &&
                        (servicoPendente.QuilometragemUltimaExecucao + servicoPendente.ValidadeQuilometrosServico) <= servicoPendente.QuilometragemAtual)
                        manutencaoCorretiva = true;

                    if ((servicoPendente.TipoServico == TipoServicoVeiculo.Todos || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetro || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                        (servicoPendente.HorimetroUltimaExecucao + servicoPendente.ValidadeHorimetroServico) <= servicoPendente.HorimetroAtual)
                        manutencaoCorretiva = true;

                    servico.TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                    servico.UltimaManutencao = repServicoOrdemServico.BuscarUltimoRealizado(servicoPendente.CodigoServico, ordemServico.Veiculo?.Codigo ?? 0, ordemServico.Equipamento?.Codigo ?? 0);

                    repServicoOrdemServico.Inserir(servico);
                }

                List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicosPendentesExecucaoUnica = repServicoVeiculo.BuscarPendentesExecucaoUnicaPorEquipamento(ordemServico.Equipamento,
                    (ordemServico.QuilometragemVeiculo > 0 ? ordemServico.QuilometragemVeiculo : ordemServico.Equipamento.Hodometro), (ordemServico.Horimetro > 0 ? ordemServico.Horimetro : ordemServico.Equipamento.Horimetro),
                     codigoGrupoServico, configuracaoTMS.UtilizarValidadeServicoPeloGrupoServicoOrdemServico);

                for (int i = 0; i < servicosPendentesExecucaoUnica.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoPendente = servicosPendentesExecucaoUnica[i];
                    decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.Codigo);

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                    servico.CustoMedio = custoMedio;
                    servico.CustoEstimado = custoMedio;
                    servico.TempoEstimado = servicoPendente.TempoEstimado;
                    servico.TempoExecutado = 0;
                    servico.Observacao = string.Empty;
                    servico.OrdemServico = ordemServico;
                    servico.Servico = servicoPendente;

                    if (servicoPendente.ValidadeKM <= ordemServico.QuilometragemVeiculo || servicoPendente.ValidadeHorimetro <= ordemServico.Horimetro)
                        servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva;
                    else
                        servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;

                    repServicoOrdemServico.Inserir(servico);
                }
            }

            OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);
        }

        public static void GerarManutencoesGrupoServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (ordemServico.Veiculo == null && ordemServico.Equipamento == null)
                return;

            if (ordemServico.GrupoServico == null)
                throw new ServicoException("Grupo de Serviço não foi informado.");

            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicosGrupo = ordemServico.GrupoServico.ServicosVeiculo.Select(o => o.ServicoVeiculoFrota).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculo in servicosGrupo)
            {
                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoVeiculo.Codigo);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                servico.CustoMedio = custoMedio;
                servico.CustoEstimado = custoMedio;
                servico.TempoEstimado = servicoVeiculo.TempoEstimado;
                servico.TempoExecutado = 0;
                servico.Observacao = string.Empty;
                servico.OrdemServico = ordemServico;
                servico.Servico = servicoVeiculo;
                servico.TipoManutencao = servicoVeiculo.TipoManutencao == TipoManutencaoServicoVeiculo.Corretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                servico.UltimaManutencao = null;

                repServicoOrdemServico.Inserir(servico);
            }

            OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);
        }

        public static void GerarManutencaoDoServicoEspecifico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculo, decimal custoServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (servicoVeiculo == null)
                throw new ServicoException("Serviço não foi informado.");

            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

            decimal custoMedio = custoServico > 0 ? custoServico : repServicoOrdemServico.BuscarCustoMedio(servicoVeiculo.Codigo);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
            servico.CustoMedio = custoMedio;
            servico.CustoEstimado = custoMedio;
            servico.TempoEstimado = servicoVeiculo.TempoEstimado;
            servico.TempoExecutado = 0;
            servico.Observacao = string.Empty;
            servico.OrdemServico = ordemServico;
            servico.Servico = servicoVeiculo;
            servico.TipoManutencao = servicoVeiculo.TipoManutencao == TipoManutencaoServicoVeiculo.Corretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
            servico.UltimaManutencao = null;

            repServicoOrdemServico.Inserir(servico);

            OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);
        }

        public static void GerarManutencaoDeServicosEspecificos(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, List<Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos> objetoServicos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (objetoServicos == null || objetoServicos.Count == 0)
                throw new ServicoException("Serviços não foram informados.");

            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServicoServicos objetoServico in objetoServicos)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo();
                servico.CustoMedio = objetoServico.CustoMedio;
                servico.CustoEstimado = objetoServico.CustoEstimado;
                servico.TempoEstimado = objetoServico.TempoEstimado;
                servico.Observacao = objetoServico.Observacao;
                servico.OrdemServico = ordemServico;
                servico.Servico = objetoServico.Servico;
                servico.TipoManutencao = objetoServico.TipoManutencao;
                servico.UltimaManutencao = objetoServico.UltimaManutencao;

                repServicoOrdemServico.Inserir(servico);
            }

            OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unidadeTrabalho);
        }

        public static async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico>> RetornarCabecalhoRelatorioAuditoriaOrdemDeOs(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtrosPesquisa,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork, cancellationToken);

            return await repositorioOrdemServico.ConsultarRelatorioAuditoriaOrdemServicoAsync(filtrosPesquisa);
        }



        public static async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico>> RetornarServicosRelatorioAuditoriaOrdemDeOs(
            Repositorio.UnitOfWork unitOfWork,
            List<int> codigosOrdemServico,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork, cancellationToken);

            return await repositorioOrdemServico.ConsultarRelatorioAuditoriaOrdemServicoServicos(codigosOrdemServico);
        }

        public static async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto>> RetornarProdutosRelatorioAuditoriaOrdemDeOs(
            Repositorio.UnitOfWork unitOfWork,
            List<int> codigosOrdemServico,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork, cancellationToken);

            return await repositorioOrdemServico.ConsultarRelatorioAuditoriaOrdemServicoProdutos(codigosOrdemServico);
        }

        public static byte[] GerarRelatorioAuditoriaOrdemServico(
            IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico> listaReport,
            IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico> listaServicos,
            IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto> listaProdutos,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> listaParametros)
        {
            return ReportRequest.WithType(ReportType.AuditoriaDeOs)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("ListaReport", listaReport.ToJson())
                .AddExtraData("ListaServicos", listaServicos.ToJson())
                .AddExtraData("ListaProdutos", listaProdutos.ToJson())
                .AddExtraData("ListaParametros", listaParametros.ToJson())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> ObterManutencoesObrigatoriasParaCarga(List<Dominio.Entidades.Veiculo> veiculos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentesParaExecucaoDaCarga = new List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico>();

            for (int i = 0; i < veiculos.Count; i++)
            {
                Dominio.Entidades.Veiculo veiculo = veiculos[i];

                IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> ultimasExecucoes = repServicoVeiculo.BuscarManutencoesObrigatoriasParaCargaPorVeiculo(veiculo.Codigo, configuracao.UtilizarValidadeServicoPeloGrupoServicoOrdemServico);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico ultimaExecucaoServico in ultimasExecucoes)
                {
                    bool manutencaoCorretiva = false;

                    if ((ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.Ambos || ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.PorDia ||
                        ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.Todos || ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                        ultimaExecucaoServico.DataUltimaExecucao.AddDays(ultimaExecucaoServico.ValidadeDiasServico) <= DateTime.Now.Date)
                        manutencaoCorretiva = true;

                    if ((ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.Ambos || ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.PorKM || ultimaExecucaoServico.TipoServico == TipoServicoVeiculo.Todos) &&
                        (ultimaExecucaoServico.QuilometragemUltimaExecucao + ultimaExecucaoServico.ValidadeQuilometrosServico) <= ultimaExecucaoServico.QuilometragemAtual)
                        manutencaoCorretiva = true;

                    if (manutencaoCorretiva)
                        servicosPendentesParaExecucaoDaCarga.Add(ultimaExecucaoServico);
                }
            }

            return servicosPendentesParaExecucaoDaCarga;
        }

        #endregion
    }
}

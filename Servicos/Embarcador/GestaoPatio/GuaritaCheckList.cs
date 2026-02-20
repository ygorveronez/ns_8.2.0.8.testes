using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public class GuaritaCheckList
    {
        #region Métodos Públicos

        public static int GerarCheckList(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita, Repositorio.UnitOfWork unitOfWork, int kmAtual, TipoEntradaSaida tipoEntradaSaida, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServicoFrota, Dominio.Entidades.Veiculo veiculo, string observacao, int codigoCheckListTipo, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            int codigoGuaritaCheckList = InserirCheckList(guarita, unitOfWork, kmAtual, tipoEntradaSaida, carga, ordemServicoFrota, veiculo, observacao, codigoCheckListTipo, codigoEmpresa, Auditado);

            if (guarita != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in guarita.Reboques)
                    InserirCheckList(guarita, unitOfWork, kmAtual, tipoEntradaSaida, carga, ordemServicoFrota, reboque, observacao, codigoCheckListTipo, codigoEmpresa, Auditado);
            }
            else
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    InserirCheckList(null, unitOfWork, kmAtual, tipoEntradaSaida, carga, ordemServicoFrota, reboque, observacao, codigoCheckListTipo, codigoEmpresa, Auditado);
            }

            return codigoGuaritaCheckList;
        }

        public static bool AtualizarDataEntradaPedido(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                if (guarita == null || guarita.Carga == null || guarita.TipoEntradaSaida == TipoEntradaSaida.Saida)
                    return true;

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(guarita.Carga.Codigo);
                foreach (var ped in pedidos)
                {

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(ped.Codigo, true);
                    DateTime datahora = DateTime.Now;
                    string strDataHora = guarita.DataSaidaEntrada.ToString("dd/MM/yyyy") + " " + guarita.HoraSaidaEntrada.Hours.ToString().PadLeft(2, '0') + ":" + guarita.HoraSaidaEntrada.Minutes.ToString().PadLeft(2, '0') + ":" + guarita.HoraSaidaEntrada.Seconds.ToString().PadLeft(2, '0');
                    DateTime.TryParseExact(strDataHora, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out datahora);
                    pedido.DataFinalViagemExecutada = datahora;
                    pedido.DataFinalViagemFaturada = datahora;
                    //Processo necessário para a rodofly

                    repPedido.Atualizar(pedido, auditado);
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }

        }

        public static bool VerificarVencimentoCheckList(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repositorioGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);

            return repositorioGuaritaCheckList.BuscarCheckListVencida(guarita?.Veiculo?.Codigo ?? 0, DateTime.Now);
        }

        public static bool AtualizarDataSaidaPedido(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            try
            {
                if (guarita == null || guarita.Carga == null || guarita.TipoEntradaSaida == TipoEntradaSaida.Entrada)
                    return true;

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(guarita.Carga.Codigo);

                foreach (var ped in pedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(ped.Codigo, true);
                    DateTime datahora = DateTime.Now;
                    string strDataHora = guarita.DataSaidaEntrada.ToString("dd/MM/yyyy") + " " + guarita.HoraSaidaEntrada.Hours.ToString().PadLeft(2, '0') + ":" + guarita.HoraSaidaEntrada.Minutes.ToString().PadLeft(2, '0') + ":" + guarita.HoraSaidaEntrada.Seconds.ToString().PadLeft(2, '0');
                    DateTime.TryParseExact(strDataHora, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out datahora);

                    if (!configuracaoTMS.InformarDataViagemExecutadaPedido)
                    {
                        pedido.DataInicialViagemExecutada = datahora;
                        pedido.DataInicialViagemFaturada = datahora;
                    }

                    //Processo necessário para a rodofly

                    repPedido.Atualizar(pedido, auditado);
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        public static object ObterDetalhesServico(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servico)
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
                DescricaoTipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrotaHelper.ObterDescricao(servico.TipoManutencao),
                DataUltimaManutencao = servico.UltimaManutencao?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty,
                QuilometragemUltimaManutencao = servico.UltimaManutencao?.OrdemServico.QuilometragemVeiculo,
                servico.TempoEstimado
            };
        }

        public static void GerarManutencoesVeiculo(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (guaritaCheckList.Veiculo == null)
                return;

            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unidadeTrabalho);

            IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentes = repServicoVeiculo.BuscarPendentesPorVeiculo(guaritaCheckList.Veiculo.Codigo, guaritaCheckList.Veiculo.KilometragemAtual);

            for (int i = 0; i < servicosPendentes.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico servicoPendente = servicosPendentes[i];
                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.CodigoServico);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servico = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo();
                servico.CustoMedio = custoMedio;
                servico.CustoEstimado = custoMedio;
                servico.TempoEstimado = servicoPendente.TempoEstimado;
                servico.Observacao = string.Empty;
                servico.GuaritaCheckList = guaritaCheckList;
                servico.Servico = repServicoVeiculo.BuscarPorCodigo(servicoPendente.CodigoServico);

                bool manutencaoCorretiva = false;

                if ((servicoPendente.TipoServico == TipoServicoVeiculo.Ambos || servicoPendente.TipoServico == TipoServicoVeiculo.PorDia || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                    servicoPendente.DataUltimaExecucao.AddDays(servicoPendente.ValidadeDiasServico) <= DateTime.Now.Date)
                    manutencaoCorretiva = true;

                if ((servicoPendente.TipoServico == TipoServicoVeiculo.Ambos || servicoPendente.TipoServico == TipoServicoVeiculo.PorKM) &&
                    (servicoPendente.QuilometragemUltimaExecucao + servicoPendente.ValidadeQuilometrosServico) <= servicoPendente.QuilometragemAtual)
                    manutencaoCorretiva = true;

                servico.TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                servico.UltimaManutencao = repServicoOrdemServico.BuscarUltimoRealizado(servicoPendente.CodigoServico, guaritaCheckList.Veiculo.Codigo, 0);

                repServicoGuaritaCheckList.Inserir(servico);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Adicionou serviço automaticamente pela busca", unidadeTrabalho);
            }

            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicosPendentesExecucaoUnica = repServicoVeiculo.BuscarPendentesExecucaoUnicaPorVeiculo(guaritaCheckList.Veiculo, guaritaCheckList.Veiculo.KilometragemAtual);

            for (int i = 0; i < servicosPendentesExecucaoUnica.Count; i++)
            {
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoPendente = servicosPendentesExecucaoUnica[i];
                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.Codigo);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo servico = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo();
                servico.CustoMedio = custoMedio;
                servico.CustoEstimado = custoMedio;
                servico.TempoEstimado = servicoPendente.TempoEstimado;
                servico.Observacao = string.Empty;
                servico.GuaritaCheckList = guaritaCheckList;
                servico.Servico = servicoPendente;

                if (servicoPendente.ValidadeKM <= guaritaCheckList.Veiculo.KilometragemAtual)
                    servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva;
                else
                    servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;

                repServicoGuaritaCheckList.Inserir(servico);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Adicionou serviço de execução única automaticamente pela busca", unidadeTrabalho);
            }
        }

        public static bool GerarAbastecimentos(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, out string erro)
        {
            erro = null;

            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

            if (configuracaoAbastecimento == null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                erro = "Não possui configuração para lançamento de abastecimento automático.";
                return false;
            }

            if (guaritaCheckList.TipoAbastecimento == null)
            {
                erro = "Tipo Abastecimento é obrigatório.";
                return false;
            }

            int kilometragem = guaritaCheckList.KMAtual;
            int horimetro = guaritaCheckList.Horimetro;

            Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
            abastecimento.Veiculo = guaritaCheckList.Veiculo;
            abastecimento.Motorista = guaritaCheckList.Motorista;
            abastecimento.Posto = guaritaCheckList.Posto;
            abastecimento.Produto = guaritaCheckList.Produto;
            abastecimento.Kilometragem = kilometragem;
            abastecimento.TipoAbastecimento = guaritaCheckList.TipoAbastecimento.Value;
            abastecimento.Litros = guaritaCheckList.Litros;
            abastecimento.ValorUnitario = guaritaCheckList.ValorUnitario;
            abastecimento.Status = "A";
            abastecimento.Situacao = "A";
            abastecimento.Data = guaritaCheckList.Data;
            abastecimento.Documento = "CH " + guaritaCheckList.Codigo.ToString();
            abastecimento.Empresa = guaritaCheckList.Empresa;
            abastecimento.DataAlteracao = DateTime.Now;
            abastecimento.Equipamento = guaritaCheckList.Equipamento;
            abastecimento.Horimetro = horimetro;

            Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria;

            if (abastecimento.Veiculo == null)
            {
                erro = "Veículo é obrigatório.";
                return false;
            }
            else if ((abastecimento.Veiculo.Modelo != null) && (abastecimento.Veiculo.Modelo.PossuiArla32 == SimNao.Nao && abastecimento.Produto.CodigoNCM.StartsWith("310210")))
            {
                erro = "O modelo do veículo selecionado não permite o lançamento de ARLA 32.";
                return false;
            }

            if (abastecimento.Veiculo.Tipo == "T" && abastecimento.TipoMovimento == null)
            {
                erro = "Movimento Financeiro é obrigatório quando Veículo é de Terceiro.";
                return false;
            }

            if (repAbastecimento.ContemAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
            {
                erro = "Já existe um abastecimento lançado com a mesma quilometragem e litros.";
                return false;
            }

            if (abastecimento.Equipamento != null && repAbastecimento.ContemAbastecimentoEquipamento(abastecimento.Equipamento.Codigo, abastecimento.Kilometragem, abastecimento.Horimetro, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
            {
                erro = "Já existe um abastecimento lançado com a mesma quilometragem, litros e horímetro para este equipamento.";
                return false;
            }

            if (!ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada && repAbastecimento.ContemAbastecimentoKMMaior(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Data.Value, abastecimento.Equipamento?.Codigo ?? 0, abastecimento.TipoAbastecimento))
            {
                erro = "Existe um abastecimento lançado para este veículo com KM superior ao informado neste lançamento.";
                return false;
            }

            if (abastecimento.Equipamento != null && !ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada && repAbastecimento.ContemAbastecimentoEquipamentoHorimetroMaior(abastecimento.Equipamento.Codigo, abastecimento.Horimetro, abastecimento.Data.Value, abastecimento.TipoAbastecimento))
            {
                erro = "Existe um abastecimento lançado para este equipamento com horímetro superior ao informado neste lançamento.";
                return false;
            }

            repAbastecimento.Inserir(abastecimento);

            guaritaCheckList.Abastecimento = abastecimento;
            repGuaritaCheckList.Atualizar(guaritaCheckList);

            return true;
        }

        public static object ObterDetalhesServicoEquipamento(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servico)
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
                DescricaoTipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrotaHelper.ObterDescricao(servico.TipoManutencao),
                DataUltimaManutencao = servico.UltimaManutencao?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty,
                QuilometragemUltimaManutencao = servico.UltimaManutencao?.OrdemServico.QuilometragemVeiculo,
                servico.TempoEstimado
            };
        }

        public static void GerarManutencoesEquipamento(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, int codigoEquipamento, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculo = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);
            if (equipamento == null)
                return;

            IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> servicosPendentes = repServicoVeiculo.BuscarPendentesPorEquipamento(equipamento.Codigo, equipamento.Hodometro, equipamento.Horimetro);

            for (int i = 0; i < servicosPendentes.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico servicoPendente = servicosPendentes[i];
                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.CodigoServico);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servico = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento();
                servico.CustoMedio = custoMedio;
                servico.CustoEstimado = custoMedio;
                servico.TempoEstimado = servicoPendente.TempoEstimado;
                servico.Observacao = string.Empty;
                servico.GuaritaCheckList = guaritaCheckList;
                servico.Servico = repServicoVeiculo.BuscarPorCodigo(servicoPendente.CodigoServico);

                bool manutencaoCorretiva = false;

                if ((servicoPendente.TipoServico == TipoServicoVeiculo.Todos || servicoPendente.TipoServico == TipoServicoVeiculo.Ambos ||
                    servicoPendente.TipoServico == TipoServicoVeiculo.PorDia || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                    servicoPendente.DataUltimaExecucao.AddDays(servicoPendente.ValidadeDiasServico) <= DateTime.Now.Date)
                    manutencaoCorretiva = true;

                if ((servicoPendente.TipoServico == TipoServicoVeiculo.Todos || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetro || servicoPendente.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                        (servicoPendente.HorimetroUltimaExecucao + servicoPendente.ValidadeHorimetroServico) <= servicoPendente.HorimetroAtual)
                    manutencaoCorretiva = true;

                servico.TipoManutencao = manutencaoCorretiva ? TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva : TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;
                servico.UltimaManutencao = repServicoOrdemServico.BuscarUltimoRealizado(servicoPendente.CodigoServico, 0, equipamento.Codigo);

                repServicoGuaritaCheckList.Inserir(servico);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Adicionou serviço de equipamento automaticamente pela busca", unidadeTrabalho);
            }

            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicosPendentesExecucaoUnica = repServicoVeiculo.BuscarPendentesExecucaoUnicaPorEquipamento(equipamento, equipamento.Hodometro, equipamento.Horimetro);

            for (int i = 0; i < servicosPendentesExecucaoUnica.Count; i++)
            {
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoPendente = servicosPendentesExecucaoUnica[i];
                decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(servicoPendente.Codigo);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento servico = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento();
                servico.CustoMedio = custoMedio;
                servico.CustoEstimado = custoMedio;
                servico.TempoEstimado = servicoPendente.TempoEstimado;
                servico.Observacao = string.Empty;
                servico.GuaritaCheckList = guaritaCheckList;
                servico.Servico = servicoPendente;

                if (servicoPendente.ValidadeHorimetro <= equipamento.Horimetro)
                    servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva;
                else
                    servico.TipoManutencao = TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva;

                repServicoGuaritaCheckList.Inserir(servico);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Adicionou serviço de execução única de equipamento automaticamente pela busca", unidadeTrabalho);
            }
        }

        #endregion

        #region Métodos Privados

        private static int InserirCheckList(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita, Repositorio.UnitOfWork unitOfWork, int kmAtual, TipoEntradaSaida tipoEntradaSaida, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServicoFrota, Dominio.Entidades.Veiculo veiculo, string observacao, int codigoCheckListTipo, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas repGuaritaCheckListPerguntas = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa repGuaritaCheckListPerguntasAlternativa = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);


            CheckList servicoChecklist = new CheckList(unitOfWork, Auditado);

            // GUARITA
            Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList()
            {
                Guarita = guarita,
                KMAtual = kmAtual,
                TipoEntradaSaida = tipoEntradaSaida,
                CheckListTipo = codigoCheckListTipo > 0 ? repCheckListTipo.BuscarPorCodigo(codigoCheckListTipo) : null,
                Carga = carga,
                OrdemServicoFrota = ordemServicoFrota,
                Veiculo = veiculo,
                Observacao = observacao,
                Data = null,
                Operador = null,
                Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null,
                Situacao = SituacaoGuaritaCheckList.Aberto,
                Motorista = guarita?.Motorista ?? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo?.Codigo ?? 0) ?? repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga?.Codigo ?? 0) ?? null,
            };
            repGuaritaCheckList.Inserir(guaritaCheckList, Auditado);

            // OPCOES
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntasCheckList = repCheckListOpcoes.BuscarPerguntasPorOrdem(codigoCheckListTipo);

            perguntasCheckList = servicoChecklist.ObterPerguntasFiltradas(carga, perguntasCheckList);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes perguntaCheckList in perguntasCheckList)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas pergunta = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas()
                {
                    GuaritaCheckList = guaritaCheckList,
                    Categoria = perguntaCheckList.Categoria,
                    Descricao = servicoChecklist.ObterDescricaoPergunta(perguntaCheckList, carga),
                    Tipo = perguntaCheckList.Tipo,
                    Opcao = null,
                    Resposta = string.Empty,
                };

                repGuaritaCheckListPerguntas.Inserir(pergunta);

                // ALTERNATIVAS
                if (pergunta.Tipo.IsPossuiAlternativas())
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa> alternativasCheckList = perguntaCheckList.Alternativas.ToList();
                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa alternativaCheckList in alternativasCheckList)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa alterantiva = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa()
                        {
                            GuaritaCheckListPerguntas = pergunta,
                            Descricao = alternativaCheckList.Descricao,
                            Ordem = alternativaCheckList.Ordem,
                            Marcado = false,
                        };

                        repGuaritaCheckListPerguntasAlternativa.Inserir(alterantiva);
                    }
                }
            }

            return guaritaCheckList.Codigo;
        }

        #endregion
    }
}

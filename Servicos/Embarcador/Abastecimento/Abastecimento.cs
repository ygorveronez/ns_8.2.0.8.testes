using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class Abastecimento : ServicoBase
    {
        public Abastecimento(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public static void ValidarAbastecimentoInconsistente(ref Dominio.Entidades.Abastecimento abs, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (abs.Situacao == "F" || abs.Situacao == "G")
                return;

            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Abastecimento abastecimentoAnterior = null;
            Dominio.Entidades.Usuario veiculoMotorista = null;

            bool validarApenasAbastecimentoEquipamento = false;
            if (abs.Equipamento != null && abs.Equipamento.EquipamentoAceitaAbastecimento && abs.Horimetro > 0)
                validarApenasAbastecimentoEquipamento = true;

            bool utilizarPrecoDaTabelaDeValoresDoFornecedor = false;
            bool gerarContasAPagarParaAbastecimentoExternos = false;
            if (configuracao != null)
            {
                utilizarPrecoDaTabelaDeValoresDoFornecedor = configuracao.UtilizarPrecoDaTabelaDeValoresDoFornecedor;
                gerarContasAPagarParaAbastecimentoExternos = configuracao.GerarContasAPagarParaAbastecimentoExternos;
            }

            abs.MotivoInconsistencia = "";

            if (abs.Veiculo != null && !validarApenasAbastecimentoEquipamento && abs.Kilometragem > 0)
                abastecimentoAnterior = repAbastecimento.BuscarUltimoAbastecimento(abs.Veiculo.Codigo, 0, abs.Codigo, abs.TipoAbastecimento, abs.Kilometragem, 0);
            else if (abs.Equipamento != null && validarApenasAbastecimentoEquipamento && abs.Horimetro > 0)
                abastecimentoAnterior = repAbastecimento.BuscarUltimoAbastecimento(0, abs.Equipamento.Codigo, abs.Codigo, abs.TipoAbastecimento, 0, abs.Horimetro);
            if (abastecimentoAnterior != null)
            {
                if (abastecimentoAnterior.Kilometragem > 0 && abs.Kilometragem > 0 && abastecimentoAnterior.Kilometragem == abs.Kilometragem && abastecimentoAnterior.Litros == abs.Litros && !validarApenasAbastecimentoEquipamento)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " A quilometragem e litragem do abastecimento do Veículo é igual a do último abastecimento.";
                }
                if (abastecimentoAnterior.Horimetro > 0 && abs.Horimetro > 0 && abastecimentoAnterior.Horimetro == abs.Horimetro && abastecimentoAnterior.Litros == abs.Litros && validarApenasAbastecimentoEquipamento)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " O horímetro e litragem do abastecimento do Equipamento é igual a do último abastecimento.";
                }

                if (configuracaoEmbarcador.ValidarMesmoKMComLitrosDiferenteAbastecimento)
                {
                    if (abastecimentoAnterior.Kilometragem > 0 && abs.Kilometragem > 0 && abastecimentoAnterior.Kilometragem == abs.Kilometragem && abastecimentoAnterior.Litros != abs.Litros && !validarApenasAbastecimentoEquipamento)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " A quilometragem do abastecimento do Veículo é igual a do último abastecimento (Litros diferentes).";
                    }
                    if (abastecimentoAnterior.Horimetro > 0 && abs.Horimetro > 0 && abastecimentoAnterior.Horimetro == abs.Horimetro && abastecimentoAnterior.Litros != abs.Litros && validarApenasAbastecimentoEquipamento)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " O horímetro do abastecimento do Equipamento é igual a do último abastecimento (Litros diferentes).";
                    }
                }

                if (abastecimentoAnterior.Situacao == "I" && !string.IsNullOrWhiteSpace(abastecimentoAnterior.MotivoInconsistencia) && abastecimentoAnterior.MotivoInconsistencia.Contains("excede o limite cadastrado"))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " O abastecimento anterior a este está inconsistente, motivo: " + abastecimentoAnterior.MotivoInconsistencia;
                }
            }

            if (abs.Veiculo == null && abs.Equipamento != null)
                abs.Veiculo = repVeiculo.BuscarVeiculosPorEquipamento(abs.Equipamento.Codigo);

            if (abs.Veiculo == null && abs.Motorista != null)
                abs.Veiculo = repVeiculo.BuscarVeiculoPorMotorista(abs.Motorista.Codigo);

            if (configuracaoEmbarcador.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && abs.Veiculo != null && abs.Data.HasValue && abs.Data.Value > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaVeiculo = repCarga.BuscarUltimaCargaPorVeiculos(abs.Veiculo.Codigo, abs.Data.Value);
                if (cargaVeiculo != null)
                    abs.Motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(cargaVeiculo.Codigo);
            }

            if (abs.Veiculo != null)
                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(abs.Veiculo.Codigo);

            if (abs.Motorista == null && veiculoMotorista != null && !configuracaoEmbarcador.NaoPreencherMotoristaVeiculoAbastecimento)
                abs.Motorista = veiculoMotorista;

            if (abs.Veiculo == null && abs.Equipamento == null)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Veículo/Equipamento não cadastrado.";
            }

            if (veiculo == null || abs.Veiculo != null)
                veiculo = abs.Veiculo;

            if (abs.Codigo == 0 && abs.Veiculo != null && abs.Veiculo.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Veiculo.CentroResultado;
            if (abs.Codigo == 0 && abs.Equipamento != null && abs.Equipamento.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Equipamento.CentroResultado;
            if (abs.Codigo == 0 && abs.Motorista != null && abs.Motorista.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Motorista.CentroResultado;

            if (abs.Kilometragem <= 0 && abs.Horimetro <= 0)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Não foi informado o KM/Horímetro.";
            }

            if (abs.Motorista == null)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Motorista não informado.";
            }

            if (abs.Veiculo != null && abs.Veiculo.Tipo == "T" && !configuracaoEmbarcador.NaoDeixarAbastecimentoTerceiroInconsistente)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Veículo de terceiro.";
            }

            if (abs.Veiculo != null && abs.Kilometragem > 0 && !validarApenasAbastecimentoEquipamento && repAbastecimento.ContemAbastecimento(abs.Veiculo.Codigo, abs.Kilometragem, abs.Documento, abs.Litros, abs.TipoAbastecimento, abs.Codigo))
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Abastecimento duplicado (Veículo, km (diferença de 5 km), numero documento e litros).";
            }

            if (configuracaoEmbarcador.DeixarAbastecimentosMesmaDataHoraInconsistentes)
            {
                if (abs.Veiculo != null && !validarApenasAbastecimentoEquipamento && repAbastecimento.ContemAbastecimentoDataHoraVeiculo(abs.Veiculo.Codigo, abs.Data, abs.TipoAbastecimento, abs.Codigo))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Abastecimento duplicado (Veículo, data e hora).";
                }
            }

            if (!abs.Data.HasValue || abs.Data.Value == DateTime.MinValue)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Data não informada para o abastecimento.";
            }

            if (abs.Posto == null)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Posto não informado.";
            }

            if (abs.Litros <= 0)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Litros não informado.";
            }

            if (abs.ValorUnitario <= 0)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Valor Unitário não informado.";
            }

            if (abs.Produto == null)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Produto não informado.";
            }

            if (veiculo != null && !validarApenasAbastecimentoEquipamento)
            {
                if (veiculo.CapacidadeMaximaTanque > 0)
                {
                    if (abs.Litros > veiculo.CapacidadeMaximaTanque)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade máxima do tanque configurado (" + veiculo.CapacidadeMaximaTanque.ToString("n4") + ") no veículo.";
                    }
                }
                else if (abs.Litros > veiculo.CapacidadeTanque && veiculo.CapacidadeTanque > 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade do tanque configurado (" + veiculo.CapacidadeTanque.ToString("n4") + ") no veículo.";
                }
            }

            if (abs.Equipamento != null)
            {
                if (abs.Equipamento.CapacidadeMaximaTanque > 0)
                {
                    if (abs.Litros > abs.Equipamento.CapacidadeMaximaTanque)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade máxima do tanque configurado (" + abs.Equipamento.CapacidadeMaximaTanque.ToString("n4") + ") no equipamento.";
                    }
                }
                else if (abs.Litros > abs.Equipamento.CapacidadeTanque && abs.Equipamento.CapacidadeTanque > 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade do tanque configurado (" + abs.Equipamento.CapacidadeTanque.ToString("n4") + ") no equipamento.";
                }
            }

            if (abs.Veiculo != null && abs.Data.HasValue && !validarApenasAbastecimentoEquipamento)
            {
                decimal kmUltimoKMVeiculo = repAbastecimento.BuscarUltimoKMAbastecimento(abs.Veiculo.Codigo, abs.Data.Value, abs.Codigo, abs.TipoAbastecimento);
                if (abs.TipoAbastecimento != TipoAbastecimento.Arla && configuracaoEmbarcador.KMLimiteEntreAbastecimentos > 0 && kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo < abs.Kilometragem && abs.Kilometragem > 0)
                {
                    if ((abs.Kilometragem - kmUltimoKMVeiculo) > configuracaoEmbarcador.KMLimiteEntreAbastecimentos)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " A diferença de KM entre o último abastecimento (" + kmUltimoKMVeiculo.ToString("n0") + ") e o atual (" + abs.Kilometragem.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.KMLimiteEntreAbastecimentos.ToString("n0") + ")";
                    }
                }

                if (abs.TipoAbastecimento == TipoAbastecimento.Arla && configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla > 0 && kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo < abs.Kilometragem && abs.Kilometragem > 0)
                {
                    if ((abs.Kilometragem - kmUltimoKMVeiculo) > configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " A diferença de KM entre o último abastecimento (" + kmUltimoKMVeiculo.ToString("n0") + ") e o atual (" + abs.Kilometragem.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla.ToString("n0") + ")";
                    }
                }


                if (kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo > abs.Kilometragem && abs.Kilometragem > 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Existe um abastecimento lançado com o KM (" + kmUltimoKMVeiculo.ToString("n0") + ") maior que o informado (" + abs.Kilometragem.ToString("n0") + ").";
                }
                else if (abs.Veiculo != null && abs.Veiculo.Modelo != null && (abs.Veiculo.Modelo.MediaPadrao > 0 || abs.Veiculo.Modelo.MediaMinima > 0 || abs.Veiculo.Modelo.MediaMaxima > 0) &&
                    abs.Kilometragem > 0 && abs.Litros > 0 && !configuracaoEmbarcador.NaoValidarMediaIdealAbastecimento && (abs.TipoAbastecimento != TipoAbastecimento.Arla || !configuracaoEmbarcador.NaoValidarMediaIdealDeArlaAbastecimento))
                {
                    decimal kmTotal = abs.Kilometragem - kmUltimoKMVeiculo;
                    if (kmTotal > 0)
                    {
                        decimal mediaAbastecimento = Math.Round((kmTotal / abs.Litros), 4);

                        if (abs.Veiculo.Modelo.MediaMinima > 0 && mediaAbastecimento < Math.Round(abs.Veiculo.Modelo.MediaMinima, 4))
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média MÍNIMA configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaMinima.ToString("n4") + ") .";
                        }
                        else if (abs.Veiculo.Modelo.MediaMaxima > 0 && mediaAbastecimento > Math.Round(abs.Veiculo.Modelo.MediaMaxima, 4))
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média MÁXIMA configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaMaxima.ToString("n4") + ") .";
                        }
                        else if (abs.Veiculo.Modelo.MediaPadrao > 0 && mediaAbastecimento < Math.Round(abs.Veiculo.Modelo.MediaPadrao, 4) && abs.Veiculo.Modelo.MediaMaxima == 0 && abs.Veiculo.Modelo.MediaMinima == 0)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média PADRÃO configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaPadrao.ToString("n4") + ") .";
                        }
                    }
                }
            }

            if (abs.Equipamento != null && abs.Data.HasValue)
            {
                decimal kmUltimoHorimetroEquipamento = 0;
                if (abs.Equipamento.TrocaHorimetro)
                    kmUltimoHorimetroEquipamento = abs.Equipamento.HorimetroAtual;
                else
                    kmUltimoHorimetroEquipamento = repAbastecimento.BuscarUltimoHorimetroAbastecimento(abs.Equipamento.Codigo, abs.Data.Value, abs.Codigo, abs.TipoAbastecimento);

                if (configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos > 0 && kmUltimoHorimetroEquipamento > 0 && kmUltimoHorimetroEquipamento < abs.Horimetro && abs.Horimetro > 0)
                {
                    if ((abs.Horimetro - kmUltimoHorimetroEquipamento) > configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " A diferença de Horímetro entre o último abastecimento (" + kmUltimoHorimetroEquipamento.ToString("n0") + ") e o atual (" + abs.Horimetro.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos.ToString("n0") + ")";
                    }
                }
                if (kmUltimoHorimetroEquipamento > 0 && kmUltimoHorimetroEquipamento > abs.Horimetro && abs.Horimetro > 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Existe um abastecimento lançado com o Horímetro (" + kmUltimoHorimetroEquipamento.ToString("n0") + ") maior que o informado (" + abs.Horimetro.ToString("n0") + ").";
                }
            }

            if (abs.Produto != null && abs.Posto != null && abs.Data.HasValue && abs.ValorUnitario > 0)
            {
                (decimal ValorDe, decimal ValorAte) valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivelDeAte(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, abs.Data.Value);
                if (valorTabelaFornecedor.ValorDe > 0m && valorTabelaFornecedor.ValorAte > 0m)
                {
                    if ((Math.Round(valorTabelaFornecedor.ValorDe, 3) > Math.Round(abs.ValorUnitario, 3)) || (Math.Round(valorTabelaFornecedor.ValorAte, 3) < Math.Round(abs.ValorUnitario, 3)))
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " O preço do combustível não está de acordo com a tabela cadastrada no fornecedor." +
                                            $" Valor Tabela R$ {valorTabelaFornecedor.ValorDe.ToString("n3")} até R$ {valorTabelaFornecedor.ValorAte.ToString("n3")} Valor Abastecimento R$ {abs.ValorUnitario.ToString("n3")}";
                    }
                } 
                else if (valorTabelaFornecedor.ValorDe > 0 && (Math.Round(valorTabelaFornecedor.ValorDe, 3) != Math.Round(abs.ValorUnitario, 3)))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " O preço do combustível não está de acordo com a tabela cadastrada no fornecedor." +
                                        $" Valor Tabela R$ {valorTabelaFornecedor.ValorDe.ToString("n3")} Valor Abastecimento R$ {abs.ValorUnitario.ToString("n3")}";
                }
            }

            if (abs.Produto != null && abs.Posto != null && abs.Data.HasValue && abs.ValorUnitario > 0 && utilizarPrecoDaTabelaDeValoresDoFornecedor)
            {
                decimal valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivel(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, abs.Data.Value);
                if (valorTabelaFornecedor > 0)
                {
                    abs.ValorUnitario = valorTabelaFornecedor;
                    abs.UtilizarPrecoDaTabelaDeValoresDoFornecedor = utilizarPrecoDaTabelaDeValoresDoFornecedor;
                }
                else
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += "Abastecimento inconsistente devido não ter encontrado valor na tabela de valores do fornecedor.";
                }
            }

            if (gerarContasAPagarParaAbastecimentoExternos || (abs.ConfiguracaoAbastecimento != null && abs.ConfiguracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos))
            {
                abs.TipoMovimentoPagamentoExterno = configuracao?.TipoMovimento ?? abs.ConfiguracaoAbastecimento?.TipoMovimento ?? null;
                abs.GerarContasAPagarParaAbastecimentoExternos = true;

                if (abs.Posto != null && abs.Produto != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores modalidadeFornecedorPessoas = repPostoCombustivelTabelaValores.BuscarModalidadeFornecedor(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, DateTime.MinValue);
                    if (modalidadeFornecedorPessoas?.ModalidadeFornecedorPessoas != null)
                    {
                        if (modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.Oficina && modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.TipoOficina.HasValue && modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.TipoOficina.Value == TipoOficina.Interna)
                        {
                            abs.TipoMovimentoPagamentoExterno = null;
                            abs.GerarContasAPagarParaAbastecimentoExternos = false;
                        }
                    }
                }
            }

            if (configuracaoEmbarcador.UtilizaMoedaEstrangeira && abs.MoedaCotacaoBancoCentral.HasValue && abs.MoedaCotacaoBancoCentral.Value != MoedaCotacaoBancoCentral.Real)
            {
                if (abs.ValorMoedaCotacao <= 0 || abs.ValorOriginalMoedaEstrangeira <= 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Não foi localizado nenhuma cotação vigente da moeda estrangeira.";
                }
            }
            if (!string.IsNullOrWhiteSpace(abs.MotivoInconsistencia))
            {
                string dataAbastecimento = abs.Data?.ToString("dd/MM/yyyy HH:mm") ?? "";
                string placaVeiculo = abs.Veiculo?.Placa?.ToString() ?? "";
                abs.MotivoInconsistencia += $" Data: {dataAbastecimento} - Placa: {placaVeiculo}";

                if (abs.MotivoInconsistencia.Length > 2000)                
                    abs.MotivoInconsistencia = abs.MotivoInconsistencia.Substring(0, 1999);                
            }
        }

        public static void ValidarAbastecimentoRequisicao(ref Dominio.Entidades.Abastecimento abs, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuarioLogado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (abs.Situacao == "F" || abs.Situacao == "G")
                return;

            bool ValidarComoRequisicao = true;
            bool validarInformacaoTerceiro = false;

            if (abs.Data <= DateTime.MinValue)
                abs.Data = null;

            if (abs.Codigo > 0)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    abs.Situacao = "A";
                    abs.Requisicao = false;
                    ValidarComoRequisicao = false;
                    validarInformacaoTerceiro = true;
                }
                else
                {
                    abs.Situacao = "R";
                    ValidarComoRequisicao = true;
                }
            }
            else
            {
                ValidarComoRequisicao = true;
                abs.Situacao = "R";
            }
            

            var repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            var repVeiculo = new Repositorio.Veiculo(unitOfWork);
            var repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            var repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Abastecimento abastecimentoAnterior = null;
            Dominio.Entidades.Usuario veiculoMotorista = null;

            var validarApenasAbastecimentoEquipamento = false;
            
            if (abs.Equipamento != null && abs.Equipamento.EquipamentoAceitaAbastecimento && abs.Horimetro > 0)
                validarApenasAbastecimentoEquipamento = true;

            var utilizarPrecoDaTabelaDeValoresDoFornecedor = false;
            var gerarContasAPagarParaAbastecimentoExternos = false;

            if (configuracao != null)
            {
                utilizarPrecoDaTabelaDeValoresDoFornecedor = configuracao.UtilizarPrecoDaTabelaDeValoresDoFornecedor;
                gerarContasAPagarParaAbastecimentoExternos = configuracao.GerarContasAPagarParaAbastecimentoExternos;
            }

            abs.MotivoInconsistencia = "";

            var isRequisicaoDuplicada = repAbastecimento.AbastecimentoDuplicadoPorRequisicao(abs);
            if (isRequisicaoDuplicada)
                abs.MotivoInconsistencia += "Não foi possível gravar requisição duplicada";

            if (abs.Veiculo != null && !validarApenasAbastecimentoEquipamento && abs.Kilometragem > 0)
                abastecimentoAnterior = repAbastecimento.BuscarUltimoAbastecimento(abs.Veiculo.Codigo, 0, abs.Codigo, abs.TipoAbastecimento, abs.Kilometragem, 0);
            else if (abs.Equipamento != null && validarApenasAbastecimentoEquipamento && abs.Horimetro > 0)
                abastecimentoAnterior = repAbastecimento.BuscarUltimoAbastecimento(0, abs.Equipamento.Codigo, abs.Codigo, abs.TipoAbastecimento, 0, abs.Horimetro);
            
            if (abastecimentoAnterior != null)
            {
                if (abastecimentoAnterior.Kilometragem > 0 && abs.Kilometragem > 0 && abastecimentoAnterior.Kilometragem == abs.Kilometragem && abastecimentoAnterior.Litros == abs.Litros && !validarApenasAbastecimentoEquipamento)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " A quilometragem e litragem do abastecimento do Veículo é igual a do último abastecimento.";
                }
                if (abastecimentoAnterior.Horimetro > 0 && abs.Horimetro > 0 && abastecimentoAnterior.Horimetro == abs.Horimetro && abastecimentoAnterior.Litros == abs.Litros && validarApenasAbastecimentoEquipamento)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " O horímetro e litragem do abastecimento do Equipamento é igual a do último abastecimento.";
                }

                if (configuracaoEmbarcador.ValidarMesmoKMComLitrosDiferenteAbastecimento)
                {
                    if (abastecimentoAnterior.Kilometragem > 0 && abs.Kilometragem > 0 && abastecimentoAnterior.Kilometragem == abs.Kilometragem && abastecimentoAnterior.Litros != abs.Litros && !validarApenasAbastecimentoEquipamento)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " A quilometragem do abastecimento do Veículo é igual a do último abastecimento (Litros diferentes).";
                    }
                    if (abastecimentoAnterior.Horimetro > 0 && abs.Horimetro > 0 && abastecimentoAnterior.Horimetro == abs.Horimetro && abastecimentoAnterior.Litros != abs.Litros && validarApenasAbastecimentoEquipamento)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " O horímetro do abastecimento do Equipamento é igual a do último abastecimento (Litros diferentes).";
                    }
                }

                if (abastecimentoAnterior.Situacao == "I" && !string.IsNullOrWhiteSpace(abastecimentoAnterior.MotivoInconsistencia) && abastecimentoAnterior.MotivoInconsistencia.Contains("excede o limite cadastrado"))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " O abastecimento anterior a este está inconsistente, motivo: " + abastecimentoAnterior.MotivoInconsistencia;
                }
            }

            if (abs.Veiculo == null && abs.Equipamento != null)
                abs.Veiculo = repVeiculo.BuscarVeiculosPorEquipamento(abs.Equipamento.Codigo);

            if (abs.Veiculo == null && abs.Motorista != null)
                abs.Veiculo = repVeiculo.BuscarVeiculoPorMotorista(abs.Motorista.Codigo);

            if (configuracaoEmbarcador.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && abs.Veiculo != null && abs.Data.HasValue && abs.Data.Value > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaVeiculo = repCarga.BuscarUltimaCargaPorVeiculos(abs.Veiculo.Codigo, abs.Data.Value);
                if (cargaVeiculo != null)
                    abs.Motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(cargaVeiculo.Codigo);
            }

            if (abs.Veiculo != null)
                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(abs.Veiculo.Codigo);

            if (abs.Motorista == null && veiculoMotorista != null && !configuracaoEmbarcador.NaoPreencherMotoristaVeiculoAbastecimento)
                abs.Motorista = veiculoMotorista;

            if (abs.Veiculo == null && abs.Equipamento == null)
            {
                abs.Situacao = "I";
                abs.MotivoInconsistencia += " Veículo/Equipamento não cadastrado.";
            }

            if (veiculo == null || abs.Veiculo != null)
                veiculo = abs.Veiculo;

            if (abs.Codigo == 0 && abs.Veiculo != null && abs.Veiculo.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Veiculo.CentroResultado;
            if (abs.Codigo == 0 && abs.Equipamento != null && abs.Equipamento.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Equipamento.CentroResultado;
            if (abs.Codigo == 0 && abs.Motorista != null && abs.Motorista.CentroResultado != null && abs.CentroResultado == null)
                abs.CentroResultado = abs.Motorista.CentroResultado;

            if (!ValidarComoRequisicao)
            {
                if (abs.Kilometragem <= 0 && abs.Horimetro <= 0 && !ValidarComoRequisicao)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Não foi informado o KM/Horímetro.";
                }

                if (abs.Motorista == null)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Motorista não informado.";
                }

                if (abs.Veiculo != null && abs.Veiculo.Tipo == "T" && !configuracaoEmbarcador.NaoDeixarAbastecimentoTerceiroInconsistente)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Veículo de terceiro.";
                }

                if (abs.Veiculo != null && abs.Kilometragem > 0 && !validarApenasAbastecimentoEquipamento && repAbastecimento.ContemAbastecimento(abs.Veiculo.Codigo, abs.Kilometragem, abs.Documento, abs.Litros, abs.TipoAbastecimento, abs.Codigo))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Abastecimento duplicado (Veículo, km (diferença de 5 km), numero documento e litros).";
                }

                if (configuracaoEmbarcador.DeixarAbastecimentosMesmaDataHoraInconsistentes)
                {
                    if (abs.Veiculo != null && !validarApenasAbastecimentoEquipamento && repAbastecimento.ContemAbastecimentoDataHoraVeiculo(abs.Veiculo.Codigo, abs.Data, abs.TipoAbastecimento, abs.Codigo))
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Abastecimento duplicado (Veículo, data e hora).";
                    }
                }

                if ((!abs.Data.HasValue || abs.Data.Value == DateTime.MinValue))
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Data não informada para o abastecimento.";
                }

                if (abs.Posto == null)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Posto não informado.";
                }

                if (abs.Litros <= 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Litros não informado.";
                }

                if (abs.ValorUnitario <= 0)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Valor Unitário não informado.";
                }
                if (abs.Produto == null)
                {
                    abs.Situacao = "I";
                    abs.MotivoInconsistencia += " Produto não informado.";
                }

                if (veiculo != null && !validarApenasAbastecimentoEquipamento)
                {
                    if (veiculo.CapacidadeMaximaTanque > 0)
                    {
                        if (abs.Litros > veiculo.CapacidadeMaximaTanque)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade máxima do tanque configurado (" + veiculo.CapacidadeMaximaTanque.ToString("n4") + ") no veículo.";
                        }
                    }
                    else if (abs.Litros > veiculo.CapacidadeTanque && veiculo.CapacidadeTanque > 0)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade do tanque configurado (" + veiculo.CapacidadeTanque.ToString("n4") + ") no veículo.";
                    }
                }

                if (abs.Equipamento != null)
                {
                    if (abs.Equipamento.CapacidadeMaximaTanque > 0)
                    {
                        if (abs.Litros > abs.Equipamento.CapacidadeMaximaTanque)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade máxima do tanque configurado (" + abs.Equipamento.CapacidadeMaximaTanque.ToString("n4") + ") no equipamento.";
                        }
                    }
                    else if (abs.Litros > abs.Equipamento.CapacidadeTanque && abs.Equipamento.CapacidadeTanque > 0)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quantidade de litros (" + abs.Litros.ToString("n4") + ") maior que a capacidade do tanque configurado (" + abs.Equipamento.CapacidadeTanque.ToString("n4") + ") no equipamento.";
                    }
                }

                if (abs.Veiculo != null && abs.Data.HasValue && abs.Data.HasValue && !validarApenasAbastecimentoEquipamento)
                {
                    decimal kmUltimoKMVeiculo = repAbastecimento.BuscarUltimoKMAbastecimento(abs.Veiculo.Codigo, abs.Data.Value, abs.Codigo, abs.TipoAbastecimento);
                    if (abs.TipoAbastecimento != TipoAbastecimento.Arla && configuracaoEmbarcador.KMLimiteEntreAbastecimentos > 0 && kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo < abs.Kilometragem && abs.Kilometragem > 0)
                    {
                        if ((abs.Kilometragem - kmUltimoKMVeiculo) > configuracaoEmbarcador.KMLimiteEntreAbastecimentos)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A diferença de KM entre o último abastecimento (" + kmUltimoKMVeiculo.ToString("n0") + ") e o atual (" + abs.Kilometragem.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.KMLimiteEntreAbastecimentos.ToString("n0") + ")";
                        }
                    }

                    if (abs.TipoAbastecimento == TipoAbastecimento.Arla && configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla > 0 && kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo < abs.Kilometragem && abs.Kilometragem > 0)
                    {
                        if ((abs.Kilometragem - kmUltimoKMVeiculo) > configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A diferença de KM entre o último abastecimento (" + kmUltimoKMVeiculo.ToString("n0") + ") e o atual (" + abs.Kilometragem.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.KMLimiteEntreAbastecimentosArla.ToString("n0") + ")";
                        }
                    }


                    if (kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo > abs.Kilometragem && abs.Kilometragem > 0)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Existe um abastecimento lançado com o KM (" + kmUltimoKMVeiculo.ToString("n0") + ") maior que o informado (" + abs.Kilometragem.ToString("n0") + ").";
                    }
                    else if (abs.Veiculo != null && abs.Veiculo.Modelo != null && (abs.Veiculo.Modelo.MediaPadrao > 0 || abs.Veiculo.Modelo.MediaMinima > 0 || abs.Veiculo.Modelo.MediaMaxima > 0) &&
                        abs.Kilometragem > 0 && abs.Litros > 0 && !configuracaoEmbarcador.NaoValidarMediaIdealAbastecimento && (abs.TipoAbastecimento != TipoAbastecimento.Arla || !configuracaoEmbarcador.NaoValidarMediaIdealDeArlaAbastecimento))
                    {
                        decimal kmTotal = abs.Kilometragem - kmUltimoKMVeiculo;
                        if (kmTotal > 0)
                        {
                            decimal mediaAbastecimento = Math.Round((kmTotal / abs.Litros), 4);

                            if (abs.Veiculo.Modelo.MediaMinima > 0 && mediaAbastecimento < Math.Round(abs.Veiculo.Modelo.MediaMinima, 4))
                            {
                                abs.Situacao = "I";
                                abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média MÍNIMA configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaMinima.ToString("n4") + ") .";
                            }
                            else if (abs.Veiculo.Modelo.MediaMaxima > 0 && mediaAbastecimento > Math.Round(abs.Veiculo.Modelo.MediaMaxima, 4))
                            {
                                abs.Situacao = "I";
                                abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média MÁXIMA configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaMaxima.ToString("n4") + ") .";
                            }
                            else if (abs.Veiculo.Modelo.MediaPadrao > 0 && mediaAbastecimento < Math.Round(abs.Veiculo.Modelo.MediaPadrao, 4) && abs.Veiculo.Modelo.MediaMaxima == 0 && abs.Veiculo.Modelo.MediaMinima == 0)
                            {
                                abs.Situacao = "I";
                                abs.MotivoInconsistencia += " A média do abastecimento (" + mediaAbastecimento.ToString("n4") + ") ficou menor que a média PADRÃO configurada no modelo do veículo (" + abs.Veiculo.Modelo.MediaPadrao.ToString("n4") + ") .";
                            }
                        }
                    }
                }

                if (abs.Equipamento != null && abs.Data.HasValue)
                {                    
                    decimal kmUltimoHorimetroEquipamento = 0;
                    if (abs.Equipamento.TrocaHorimetro)
                        kmUltimoHorimetroEquipamento = abs.Equipamento.HorimetroAtual;
                    else
                        kmUltimoHorimetroEquipamento = repAbastecimento.BuscarUltimoHorimetroAbastecimento(abs.Equipamento.Codigo, abs.Data.Value, abs.Codigo, abs.TipoAbastecimento);

                    if (configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos > 0 && kmUltimoHorimetroEquipamento > 0 && kmUltimoHorimetroEquipamento < abs.Horimetro && abs.Horimetro > 0)
                    {
                        if ((abs.Horimetro - kmUltimoHorimetroEquipamento) > configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos)
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " A diferença de Horímetro entre o último abastecimento (" + kmUltimoHorimetroEquipamento.ToString("n0") + ") e o atual (" + abs.Horimetro.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.HorimetroLimiteEntreAbastecimentos.ToString("n0") + ")";
                        }
                    }
                    if (kmUltimoHorimetroEquipamento > 0 && kmUltimoHorimetroEquipamento > abs.Horimetro && abs.Horimetro > 0)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Existe um abastecimento lançado com o Horímetro (" + kmUltimoHorimetroEquipamento.ToString("n0") + ") maior que o informado (" + abs.Horimetro.ToString("n0") + ").";
                    }
                }

                if (abs.Produto != null && abs.Posto != null && abs.Data.HasValue && abs.ValorUnitario > 0)
                {
                    (decimal ValorDe, decimal ValorAte) valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivelDeAte(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, abs.Data.Value);
                    if (valorTabelaFornecedor.ValorDe > 0m && valorTabelaFornecedor.ValorAte > 0m)
                    {
                        if ((Math.Round(valorTabelaFornecedor.ValorDe, 3) > Math.Round(abs.ValorUnitario, 3)) || (Math.Round(valorTabelaFornecedor.ValorAte, 3) < Math.Round(abs.ValorUnitario, 3)))
                        {
                            abs.Situacao = "I";
                            abs.MotivoInconsistencia += " O preço do combustível não está de acordo com a tabela cadastrada no fornecedor." +
                                                $" Valor Tabela R$ {valorTabelaFornecedor.ValorDe.ToString("n3")} até R$ {valorTabelaFornecedor.ValorAte.ToString("n3")} Valor Abastecimento R$ {abs.ValorUnitario.ToString("n3")}";
                        }
                    }
                    else if (valorTabelaFornecedor.ValorDe > 0 && (Math.Round(valorTabelaFornecedor.ValorDe, 3) != Math.Round(abs.ValorUnitario, 3)))
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " O preço do combustível não está de acordo com a tabela cadastrada no fornecedor." +
                                            $" Valor Tabela R$ {valorTabelaFornecedor.ValorDe.ToString("n3")} Valor Abastecimento R$ {abs.ValorUnitario.ToString("n3")}";
                    }
                }

                if (abs.Produto != null && abs.Posto != null && abs.Data.HasValue && abs.ValorUnitario > 0 && utilizarPrecoDaTabelaDeValoresDoFornecedor)
                {
                    decimal valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivel(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, abs.Data.Value);
                    if (valorTabelaFornecedor > 0)
                    {
                        abs.ValorUnitario = valorTabelaFornecedor;
                        abs.UtilizarPrecoDaTabelaDeValoresDoFornecedor = utilizarPrecoDaTabelaDeValoresDoFornecedor;
                    }
                    else
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += "Abastecimento inconsistente devido não ter encontrado valor na tabela de valores do fornecedor.";
                    }
                }

                if (gerarContasAPagarParaAbastecimentoExternos || (abs.ConfiguracaoAbastecimento != null && abs.ConfiguracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos))
                {
                    abs.TipoMovimentoPagamentoExterno = configuracao?.TipoMovimento ?? abs.ConfiguracaoAbastecimento?.TipoMovimento ?? null;
                    abs.GerarContasAPagarParaAbastecimentoExternos = true;

                    if (abs.Posto != null && abs.Produto != null)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores modalidadeFornecedorPessoas = repPostoCombustivelTabelaValores.BuscarModalidadeFornecedor(abs.Produto.Codigo, abs.Posto.CPF_CNPJ, DateTime.MinValue);
                        if (modalidadeFornecedorPessoas?.ModalidadeFornecedorPessoas != null)
                        {
                            if (modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.Oficina && modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.TipoOficina.HasValue && modalidadeFornecedorPessoas.ModalidadeFornecedorPessoas.TipoOficina.Value == TipoOficina.Interna)
                            {
                                abs.TipoMovimentoPagamentoExterno = null;
                                abs.GerarContasAPagarParaAbastecimentoExternos = false;
                            }
                        }
                    }
                }

                if (configuracaoEmbarcador.UtilizaMoedaEstrangeira && abs.MoedaCotacaoBancoCentral.HasValue && abs.MoedaCotacaoBancoCentral.Value != MoedaCotacaoBancoCentral.Real)
                {
                    if (abs.ValorMoedaCotacao <= 0 || abs.ValorOriginalMoedaEstrangeira <= 0)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Não foi localizado nenhuma cotação vigente da moeda estrangeira.";
                    }
                }


                if (validarInformacaoTerceiro)
                {
                    if ((abs.Kilometragem - abs.KilometragemAnterior) > 3000)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quilometragem não permitida acima de 3.000.";
                    }
                    if (abs.Kilometragem < abs.KilometragemAnterior)
                    {
                        abs.Situacao = "I";
                        abs.MotivoInconsistencia += " Quilometragem não permitida abaixo do atual.";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(abs.MotivoInconsistencia) && abs.Data.HasValue)
            {
                string dataAbastecimento = abs.Data?.ToString("dd/MM/yyyy HH:mm") ?? "";
                string placaVeiculo = abs.Veiculo?.Placa?.ToString() ?? "";
                abs.MotivoInconsistencia += $" Data: {dataAbastecimento} - Placa: {placaVeiculo}";

                if (abs.MotivoInconsistencia.Length > 2000)
                    abs.MotivoInconsistencia = abs.MotivoInconsistencia.Substring(0, 1999);
            }
        }

        public static void GerarRequisicaoAutomatica(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Abastecimento abs)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();

            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            if (configuracaoAbastecimento?.GerarRequisicaoAutomaticaParaVeiculosVinculados ?? false)
            {
                if(abs.Veiculo != null)
                {
                    if(abs.Veiculo.VeiculosVinculados != null && abs.Veiculo.VeiculosVinculados.Count > 0)
                    {
                        foreach (var item in abs.Veiculo.VeiculosVinculados)
                        {
                            if(item.Equipamentos != null && item.Equipamentos.Count > 0)
                            {
                                foreach (var equipamento in item.Equipamentos)
                                {
                                    Dominio.Entidades.Abastecimento abastecimento = abs.Clonar();
                                    abastecimento.Veiculo = item;
                                    abastecimento.Equipamento = equipamento;
                                    abastecimento.Litros = equipamento.CapacidadeTanque;
                                    repAbastecimento.Inserir(abastecimento);
                                }
                            }
                        }
                    }

                    if (abs.Veiculo.CapacidadeTanqueArla > 0)
                    {
                        Dominio.Entidades.Abastecimento abastecimentoArla = abs.Clonar();
                        abastecimentoArla.TipoAbastecimento = TipoAbastecimento.Arla;
                        abastecimentoArla.Litros = abs.Veiculo.CapacidadeTanqueArla;
                        abastecimentoArla.Produto = repProduto.BuscarPorNCM("31021010");
                        repAbastecimento.Inserir(abastecimentoArla);
                    }
                }
            }
        }
        public static void ProcessarViradaKMHorimetro(Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento)
        {
            if (abastecimento.Codigo > 0)
                return;

            if (veiculo?.ViradaHodometro ?? false)
            {
                abastecimento.KilometragemOriginal = abastecimento.Kilometragem;
                abastecimento.Kilometragem += veiculo.KilometragemVirada;
            }

            if (equipamento?.ViradaHodometro ?? false)
            {
                abastecimento.HorimetroOriginal = abastecimento.Horimetro;
                abastecimento.Horimetro += equipamento.HorimetroVirada;
            }
        }

        public static void ReprocessarViradaKMHorimetro(Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, int codigoVeiculoAnterior, decimal kilometragemAnterior, int codigoEquipamentoAnterior, int horimetroAnterior)
        {
            if (abastecimento.Codigo == 0)
                return;

            if (kilometragemAnterior != abastecimento.Kilometragem || codigoVeiculoAnterior != (veiculo?.Codigo ?? 0))
            {
                if (veiculo?.ViradaHodometro ?? false)
                {
                    if ((abastecimento.Kilometragem - veiculo.KilometragemVirada) < 0)
                    {
                        abastecimento.KilometragemOriginal = abastecimento.Kilometragem;
                        abastecimento.Kilometragem += veiculo.KilometragemVirada;
                    }
                    else
                        abastecimento.KilometragemOriginal = abastecimento.Kilometragem - veiculo.KilometragemVirada;
                }
                else
                    abastecimento.KilometragemOriginal = 0;
            }

            if (horimetroAnterior != abastecimento.Horimetro || codigoEquipamentoAnterior != (equipamento?.Codigo ?? 0))
            {
                if (equipamento?.ViradaHodometro ?? false)
                {
                    if ((abastecimento.Horimetro - equipamento?.HorimetroVirada) < 0)
                    {
                        abastecimento.HorimetroOriginal = abastecimento.Horimetro;
                        abastecimento.Horimetro += equipamento.HorimetroVirada;
                    }
                    else
                        abastecimento.HorimetroOriginal = abastecimento.Horimetro - equipamento.HorimetroVirada;
                }
                else
                    abastecimento.HorimetroOriginal = 0;
            }
        }

        public static bool EstornarTituloPagarAbastecimento(Dominio.Entidades.Abastecimento abastecimento, out string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            msgErro = string.Empty;
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorAbastecimento(abastecimento.Codigo);

            if (titulos != null && titulos.Count > 0)
            {
                foreach (var titulo in titulos)
                {

                    if (titulo.TipoMovimento != null)
                    {
                        if (!servProcessoMovimento.GerarMovimentacao(out msgErro, null, titulo.DataEmissao.Value.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO PELA REVERSÃO DO ABASTECIMENTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null))
                        {
                            return false;
                        }
                    }

                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                    titulo.DataCancelamento = DateTime.Now.Date;
                    titulo.DataAlteracao = DateTime.Now;

                    Servicos.Auditoria.Auditoria.Auditar(auditado, titulo, null, "Cancelou o título.", unitOfWork);
                    repTitulo.Atualizar(titulo);
                }
            }

            return true;
        }

        public static void CancelarAbastecimentosPorPeriodo(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var configs = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();
                var diasAtras = -configs.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias; //<-- Coloco o sinal de subtração para definir a consulta retroativa na base

                if (configs != null && configs.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo && configs.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias > 0)
                    new Repositorio.Abastecimento(unitOfWork).CancelarAbastecimentosPorPeriodo(diasAtras);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }
        }

        public static Dominio.Entidades.Embarcador.Compras.OrdemCompra GerarOrdemCompraPeloAbastecimento(Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria produto = new Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria();

                ordem.Data = abastecimento?.Data ?? throw new Exception("É necessário informar uma data!");
                ordem.DataPrevisaoRetorno = abastecimento?.Data ?? throw new Exception("É necessário informar uma data!");
                ordem.MotivoCompra = configuracaoAbastecimento?.MotivoCompraAbastecimento ?? null;
                ordem.Fornecedor = abastecimento?.Posto ?? null;
                ordem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta;
                ordem.Numero = repOrdemCompra.BuscarProximoNumero(abastecimento.Empresa.Codigo);
                ordem.Usuario = usuario ?? null;
                ordem.Empresa = abastecimento?.Empresa ?? null;
                ordem.Veiculo = abastecimento?.Veiculo ?? null;
                ordem.Motorista = abastecimento?.Motorista ?? null;
                ordem.BloquearEdicaoOrdemCompraPorAbastecimento = true;
                ordem.Observacao = abastecimento?.Observacao ?? string.Empty;

                produto.OrdemCompra = ordem ?? null;
                produto.Produto = abastecimento?.Produto ?? null;
                produto.Quantidade = abastecimento.Litros;
                produto.ValorUnitario = abastecimento.ValorUnitario;

                repOrdemCompra.Inserir(ordem);
                repOrdemCompraMercadoria.Inserir(produto);

                return ordem;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        #endregion
    }
}

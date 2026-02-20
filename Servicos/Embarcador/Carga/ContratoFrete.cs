using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public static class ContratoFrete
    {
        public static string CalcularFretePorContratoFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametrosContratoFreteTransportadorValorFreteMinimo, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador, bool apenasVerificar, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool naoRatear = false)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete servicoComponetesFrete = new ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);

            if (calculoFreteFilialEmissora || (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.AutorizacaoEmissao))
                return "";

            carga.ValorFreteContratoFrete = 0;
            carga.ValorFreteContratoFreteExcedente = 0;

            bool contratoFretePossuiValorFranquia = (
                (contratoFreteTransportador.FranquiaValorKM > 0) ||
                (contratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui && contratoFreteTransportador.DeduzirValorPorCarga) ||
                (contratoFreteTransportador.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) ||
                (configEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
            );

            if (!contratoFretePossuiValorFranquia)
                return "Contrato de Frete não possui valor por franquia";

            if (contratoFreteTransportador.Situacao != SituacaoContratoFreteTransportador.Aprovado)
                return $"O contrato de frete ({contratoFreteTransportador.Descricao}) não foi aprovado para uso.";

            if (contratoFreteTransportador.Filiais.Count > 0 && (carga.Filial == null || !contratoFreteTransportador.Filiais.Any(obj => obj.Filial.Codigo == carga.Filial.Codigo)))
                return "A filial da carga não está autorizada a usar o contrato de frete (" + contratoFreteTransportador.Descricao + ").";

            DateTime dataBase = DateTime.Now.Date;

            if (contratoFreteTransportador.DataInicial > dataBase || contratoFreteTransportador.DataFinal < dataBase)
                return "O contrato de frete (" + contratoFreteTransportador.Descricao + ") está fora da sua vigencia";

            bool veiculoInformadoFazParteDoContrato = ValidarDisponibilidadeDoContrato(contratoFreteTransportador, carga);

            if (!veiculoInformadoFazParteDoContrato)
                return "O veiculo informado na Carga não faz parte do contrato de frete (" + contratoFreteTransportador.Descricao + ")";

            if ((configEmbarcador.TipoFechamentoFrete != TipoFechamentoFrete.FechamentoPorFaixaKm) && (contratoFreteTransportador.TipoEmissaoComplemento != TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista))
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo repositorioContratoFreteTransportadorValorFreteMinimo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo contratoFreteTransportadorValorFreteMinimo = repositorioContratoFreteTransportadorValorFreteMinimo.BuscarPorParametros(parametrosContratoFreteTransportadorValorFreteMinimo);
                decimal valorFreteMinimo = contratoFreteTransportadorValorFreteMinimo?.ValorMinimo ?? contratoFreteTransportador.ValorFreteMinimo;

                if (contratoFreteTransportador.FranquiaValorKM > 0)
                {
                    Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = repositorioFechamentoFrete.BuscarFechamentoExistente(contratoFreteTransportador.Codigo, DateTime.Now);

                    if (fechamentoFrete != null)
                        return $"Para calcular o frete é necessário cancelar o fechamento número {fechamentoFrete.Numero} ou aguardar o fim do período ({fechamentoFrete.DataFim.ToString("dd/MM/yyyy")}) para tentar calcular o frete novamente";

                    Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
                    int distanciaNormalUtilizada = (int)repositorioContratoSaldoMes.ConsultarDistanciaPorContratoFrete(contratoFreteTransportador.Codigo, dataBase.FirstDayOfMonth(), dataBase);
                    int distanciaCarga = (int)servicoCarga.ObterDistancia(carga, configEmbarcador, unitOfWork);
                    int distanciaTotal = (distanciaNormalUtilizada + distanciaCarga);
                    int distanciaNormal = distanciaCarga;
                    int distanciaExcedente = 0;

                    if (distanciaTotal > contratoFreteTransportador.FranquiaTotalKM)
                    {
                        int distanciaExcedida = distanciaTotal - contratoFreteTransportador.FranquiaTotalKM;

                        if (distanciaExcedida < distanciaCarga)
                        {
                            distanciaExcedente = distanciaExcedida;
                            distanciaNormal = (distanciaCarga - distanciaExcedente);
                        }
                        else
                        {
                            distanciaNormal = 0;
                            distanciaExcedente = distanciaCarga;
                        }
                    }

                    decimal valorFreteContratoFrete = Math.Round(distanciaNormal * contratoFreteTransportador.FranquiaValorKM, 2, MidpointRounding.AwayFromZero);
                    decimal valorFreteContratoFreteExcedente = distanciaExcedente * contratoFreteTransportador.FranquiaValorKmExcedente;
                    decimal valorFreteContratoFreteTotal = (valorFreteContratoFrete + valorFreteContratoFreteExcedente);
                    decimal valorFreteCompletarValorFreteMinimo = 0m;

                    carga.ContratoFreteFranquiaValorPorKm = contratoFreteTransportador.FranquiaValorKM;
                    carga.ContratoFreteFranquiaValorKmExcedente = contratoFreteTransportador.FranquiaValorKmExcedente;
                    carga.DistanciaExcedenteContrato = (valorFreteContratoFreteExcedente > 0m) ? distanciaExcedente : 0;
                    carga.ValorFreteContratoFrete = valorFreteContratoFrete;
                    carga.ValorFreteContratoFreteExcedente = valorFreteContratoFreteExcedente;

                    if (valorFreteContratoFreteTotal < valorFreteMinimo)
                    {
                        valorFreteCompletarValorFreteMinimo = valorFreteMinimo - valorFreteContratoFreteTotal;
                        carga.ValorFreteContratoFrete += valorFreteCompletarValorFreteMinimo;
                    }

                    if (!apenasVerificar)
                    {
                        if (valorFreteContratoFrete > 0m)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Distância * Valor Franquia KM Contrato", distanciaNormal.ToString() + " * " + contratoFreteTransportador.FranquiaValorKM.ToString("n6"), contratoFreteTransportador.FranquiaValorKM, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.AumentoValor, "Contrato de Frete Valor Franquia KM", 0, valorFreteContratoFrete);
                            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicao, unitOfWork, null);
                        }

                        if (valorFreteContratoFreteExcedente > 0m)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Distância Excedente * Valor Franquia KM Excedente Contrato", distanciaExcedente.ToString() + " * " + contratoFreteTransportador.FranquiaValorKmExcedente.ToString("n2"), contratoFreteTransportador.FranquiaValorKmExcedente, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.AumentoValor, "Contrato de Frete Valor Franquia KM Valor Excedente", 0, valorFreteContratoFreteExcedente);
                            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicao, unitOfWork, null);
                        }

                        if (valorFreteCompletarValorFreteMinimo > 0m)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de Frete Mínimo - Contrato de Frete Valor", valorFreteMinimo.ToString("n2") + " - " + valorFreteContratoFreteTotal.ToString("n2"), valorFreteMinimo, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.AumentoValor, "Complemento do Valor de Frete Mínimo do Contrato", 0, valorFreteCompletarValorFreteMinimo);
                            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicao, unitOfWork, null);
                        }
                    }
                }
                else
                {
                    //todo: ver para limitar valor pelo contrato mensal.
                    if ((contratoFreteTransportador.TipoOperacoes.Count > 0) && (carga.TipoOperacao == null || !contratoFreteTransportador.TipoOperacoes.Any(obj => obj.TipoOperacao.Codigo == carga.TipoOperacao.Codigo)))
                        return "";

                    DateTime dataInicio = new DateTime(dataBase.Year, dataBase.Month, 1);
                    DateTime dataFim = dataInicio.AddMonths(1).AddDays(-1);

                    Repositorio.Embarcador.Frete.ContratoSaldoMes repContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
                    decimal valorTotalJaPago = repContratoSaldoMes.ConsultarValorTotalPorContratoFrete(contratoFreteTransportador.Codigo, dataInicio, dataFim, somenteComCarga: false);

                    decimal valorContrato = 0;
                    string aplicacaoFormula = "";
                    string formula = "";

                    if (contratoFreteTransportador.UtilizarValorFixoModeloVeicular)
                    {
                        Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repContratoFreteTransportadorAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);
                        Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo contratoFreteTransportadorAcordo = repContratoFreteTransportadorAcordo.BuscarPorContratoEModelo(contratoFreteTransportador.Codigo, carga.ModeloVeicularCarga?.Codigo ?? 0);
                        if (contratoFreteTransportadorAcordo != null)
                        {
                            valorContrato = contratoFreteTransportadorAcordo.ValorAcordado;
                            aplicacaoFormula = carga.ModeloVeicularCarga?.Descricao + " = " + contratoFreteTransportadorAcordo.ValorAcordado.ToString("n2");
                            formula = "Valor indicado por modelo veícular";
                        }
                        else
                            return "Não foi informada um valor para o modelo veícular " + (carga.ModeloVeicularCarga?.Descricao ?? "") + " no contrato de frete " + contratoFreteTransportador.Descricao + ". ";
                    }
                    else
                    {
                        if (contratoFreteTransportador.QuantidadeMensalCargas == 0)
                            return "Não foi informada a quantidade cargas mensais no contrato de frete " + contratoFreteTransportador.Descricao + ". ";

                        valorContrato = Math.Round((contratoFreteTransportador.ValorMensal / contratoFreteTransportador.QuantidadeMensalCargas), 2, MidpointRounding.AwayFromZero);
                        aplicacaoFormula = contratoFreteTransportador.ValorMensal.ToString("n2") + " / " + contratoFreteTransportador.QuantidadeMensalCargas.ToString();
                        formula = "Valor Mensal / Número de viagens estimadas no mês";
                    }


                    decimal valorAtual = valorContrato + valorTotalJaPago;
                    decimal valorFreteCompletarValorFreteMinimo = 0m;

                    if (valorAtual > contratoFreteTransportador.ValorMensal)
                    {
                        if (valorTotalJaPago < contratoFreteTransportador.ValorMensal)
                        {
                            formula = "Total do Contrato - Total Já Pago";
                            aplicacaoFormula = contratoFreteTransportador.ValorMensal.ToString("n2") + " - " + valorTotalJaPago.ToString("n2");
                            valorContrato = contratoFreteTransportador.ValorMensal - valorTotalJaPago;
                        }
                        else
                            valorContrato = 0;
                    }

                    if (valorContrato > 0)
                    {
                        carga.ValorFreteContratoFrete = valorContrato;

                        if (valorContrato < valorFreteMinimo)
                        {
                            valorFreteCompletarValorFreteMinimo = valorFreteMinimo - valorContrato;
                            carga.ValorFreteContratoFrete += valorFreteCompletarValorFreteMinimo;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula, aplicacaoFormula, contratoFreteTransportador.ValorMensal, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.AumentoValor, "Dedução do valor do contrato de frete mensal", 0, carga.ValorFreteContratoFrete);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicao, unitOfWork, null);

                        if (valorFreteCompletarValorFreteMinimo > 0m)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoCompletarValorFreteMinimo = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de Frete Mínimo - Dedução do Valor do Contrato", valorFreteMinimo.ToString("n2") + " - " + valorContrato.ToString("n2"), valorFreteMinimo, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.AumentoValor, "Complemento do Valor de Frete Mínimo do Contrato", 0, valorFreteCompletarValorFreteMinimo);
                            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicaoCompletarValorFreteMinimo, unitOfWork, null);
                        }
                    }
                }

                if (contratoFreteTransportador.ComponenteFreteValorContrato == null)
                    carga.ValorFrete += carga.ValorFreteContratoFreteTotal;
                else
                {
                    bool incluirICMS = configEmbarcador.IncluirICMSFreteInformadoManualmente;
                    bool incluirIntegralmenteContratoFreteTerceiro = false;

                    if (carga.TabelaFrete != null)
                        incluirICMS = carga.TabelaFrete.IncluirICMSValorFrete;

                    servicoComponetesFrete.AdicionarComponenteFreteCarga(carga, contratoFreteTransportador.ComponenteFreteValorContrato, carga.ValorFreteContratoFreteTotal, 0, calculoFreteFilialEmissora, TipoCampoValorTabelaFrete.ValorFixo, contratoFreteTransportador.ComponenteFreteValorContrato.TipoComponenteFrete, null, incluirICMS, incluirIntegralmenteContratoFreteTerceiro, null, tipoServicoMultisoftware, null, unitOfWork, true, TipoCargaComponenteFrete.TabelaFrete, true);
                }

                carga.ValorFreteAPagar += carga.ValorFreteContratoFreteTotal;
            }

            carga.ContratoFreteTransportador = contratoFreteTransportador;

            if (!apenasVerificar)
            {
                carga.DataValorContrato = dataBase;
                carga.PossuiPendencia = false;
                carga.MotivoPendencia = "";

                if (!naoRatear)
                {
                    Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(unitOfWork);
                    serFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configEmbarcador, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
                }
            }

            repositorioCarga.Atualizar(carga);

            return "";
        }

        public static void AtualizarPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);

            if (tabelaFrete.ImprimirObservacaoCTe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (!string.IsNullOrWhiteSpace(tabelaFrete.Observacao))
                    {
                        if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                            cargaPedido.Pedido.ObservacaoCTe = tabelaFrete.Observacao;
                        else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(tabelaFrete.Observacao.ToLower()))
                            cargaPedido.Pedido.ObservacaoCTe += " / " + tabelaFrete.Observacao;

                        repPedido.Atualizar(cargaPedido.Pedido);
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }
            }
        }

        public static int ObterKmUtilizadoContratoFreteNoPeriodoAtual(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = new Fechamento.FechamentoFrete(unitOfWork).ObterFechamentoFretePeriodo(contratoFreteTransportador.PeriodoAcordo, DateTime.Now.Date);
            int totalDistanciaContrato = (int)repositorioContratoSaldoMes.ConsultarDistanciaTotalPorContratoFrete(contratoFreteTransportador.Codigo, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim);

            return totalDistanciaContrato;
        }

        public static int ObterKmUtilizadoContratoFreteNoPeriodoVigencia(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            int totalDistanciaContrato = (int)repositorioContratoSaldoMes.ConsultarDistanciaTotalPorContratoFrete(contratoFreteTransportador.Codigo, contratoFreteTransportador.DataInicial, contratoFreteTransportador.DataFinal);

            return totalDistanciaContrato;
        }

        public static decimal ObterValorPagoContratoFreteNoPeriodoAtual(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = new Fechamento.FechamentoFrete(unitOfWork).ObterFechamentoFretePeriodo(contratoFreteTransportador.PeriodoAcordo, DateTime.Now.Date);
            decimal valorPagoContrato = repositorioContratoSaldoMes.ConsultarValorTotalPorContratoFrete(contratoFreteTransportador.Codigo, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim, somenteComCarga: true);

            return valorPagoContrato;
        }

        public static decimal ObterValorPagoContratoFreteNoPeriodoVigencia(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            decimal valorPagoContrato = repositorioContratoSaldoMes.ConsultarValorTotalPorContratoFrete(contratoFreteTransportador.Codigo, contratoFreteTransportador.DataInicial, contratoFreteTransportador.DataFinal, somenteComCarga: true);

            return valorPagoContrato;
        }

        public static bool ValidarDisponibilidadeDoContrato(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (contratoFreteTransportador.TipoDisponibilidadeContratoFrete == TipoDisponibilidadeContratoFrete.TodosVeiculos)
                return true;

            List<int> veiculosVinculados = new List<int>();
            List<int> veiculosContrato = new List<int>();

            if (contratoFreteTransportador.Veiculos != null && contratoFreteTransportador.Veiculos.Count > 0)
                veiculosContrato = contratoFreteTransportador.Veiculos.Select(obj => obj.Veiculo.Codigo).ToList();

            if (veiculosContrato.Count == 0)
                return false;

            veiculosVinculados = ObterVeiculosConformeModeloContrato(contratoFreteTransportador.TipoDisponibilidadeContratoFrete, carga);

            if (contratoFreteTransportador.TipoDisponibilidadeContratoFrete == TipoDisponibilidadeContratoFrete.TracaoComOuSemReboque)
                return (carga.Veiculo != null && veiculosContrato.Contains(carga.Veiculo.Codigo)) && (veiculosVinculados.Count == 0 || veiculosContrato.Any(obj => veiculosVinculados.Contains(obj)));

            return (veiculosVinculados.Count > 0 && veiculosContrato.Any(obj => veiculosVinculados.Contains(obj)));
        }

        private static List<int> ObterVeiculosConformeModeloContrato(TipoDisponibilidadeContratoFrete tipo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.Entidades.Veiculo> veiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                veiculosVinculados = carga.VeiculosVinculados.ToList();

            if (carga.Veiculo != null && tipo != TipoDisponibilidadeContratoFrete.TracaoComOuSemReboque)
                veiculosVinculados.Add(carga.Veiculo);

            if (tipo == TipoDisponibilidadeContratoFrete.Tracao)
                return veiculosVinculados.Where(obj => obj.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Tracao).Select(obj => obj.Codigo).ToList();

            if (tipo == TipoDisponibilidadeContratoFrete.Reboque)
                return veiculosVinculados.Where(obj => obj.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Reboque).Select(obj => obj.Codigo).ToList();

            if (tipo == TipoDisponibilidadeContratoFrete.TracaoComCarroceria)
                return veiculosVinculados.Where(obj => obj.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Geral).Select(obj => obj.Codigo).ToList();

            if (tipo == TipoDisponibilidadeContratoFrete.TracaoComOuSemReboque)
                return veiculosVinculados.Select(obj => obj.Codigo).ToList();

            return new List<int>();
        }

    }
}

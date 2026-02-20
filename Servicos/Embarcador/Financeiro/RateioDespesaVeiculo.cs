using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class RateioDespesaVeiculo
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public RateioDespesaVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool RatearValorEntreVeiculos(out string erro, Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo)
        {
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia repRateioDespesaVeiculoLancamentoDia = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado repValorCentroResultado = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo repValorVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceira = repConfiguracaoFinanceira.BuscarConfiguracaoPadrao();

            int totalDias = (rateioDespesaVeiculo.DataFinal - rateioDespesaVeiculo.DataInicial).Days + 1;

            var listaDespesasVeiculos = repValorVeiculo.BuscarVeiculosPorDespesa(rateioDespesaVeiculo);
            var listaDespesasCentroResultado = repValorCentroResultado.BuscarCentroResultadoPorDespesa(rateioDespesaVeiculo);
            var possuiConfiguracaoUtilizarValorDesproporcional = configuracaoFinanceira.UtilizarValorDesproporcionalRateioDespesaVeiculo;

            IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio> veiculosParaRateio = null;

            if (!possuiConfiguracaoUtilizarValorDesproporcional)
                veiculosParaRateio = repRateioDespesaVeiculoLancamento.BuscarVeiculosParaRateio(listaDespesasVeiculos.Select(d => d.Veiculo.Codigo), rateioDespesaVeiculo.SegmentosVeiculos?.Select(o => o.Codigo), listaDespesasCentroResultado.Select(d => d.CentroResultado.Codigo), rateioDespesaVeiculo.DataInicial, rateioDespesaVeiculo.DataFinal, rateioDespesaVeiculo.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo);

            if (possuiConfiguracaoUtilizarValorDesproporcional)
                veiculosParaRateio = repRateioDespesaVeiculoLancamento.BuscarSomenteVeiculos(listaDespesasCentroResultado.Select(d => d.CentroResultado.Codigo), listaDespesasVeiculos.Select(d => d.Veiculo.Codigo));

            if (possuiConfiguracaoUtilizarValorDesproporcional)
            {
                if (listaDespesasVeiculos?.Count() > 0)
                    PreencherValoresNaListaVeiculo(veiculosParaRateio, listaDespesasVeiculos);

                if (listaDespesasCentroResultado?.Count() > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado centroResultadoSemVeiculos = listaDespesasCentroResultado.Where(centro => !veiculosParaRateio.Any(veic => veic.CodigoCentroResultado == centro.CentroResultado.Codigo)).FirstOrDefault();

                    if(centroResultadoSemVeiculos != null)
                    {
                        erro = $"O centro de resultado {centroResultadoSemVeiculos?.CentroResultado.Descricao} não possui veículos vinculados, não sendo possível realizar o rateio.";
                        return false;
                    }

                    PreencherValoresCentroResultadoNaListaVeiculo(veiculosParaRateio, listaDespesasCentroResultado);
                }
            }
           
            decimal valorRestanteRatear = rateioDespesaVeiculo.Valor;
            decimal valorReceberTotal = veiculosParaRateio.Sum(o => o.ValorReceber);

            int totalVeiculosRateio = veiculosParaRateio.Count;

            if (totalVeiculosRateio <= 0)
            {
                erro = "Não foi encontrado um veículo para realizar o rateio.";
                return false;
            }

            while (totalVeiculosRateio > 0)
            {
                int indiceVeiculo = totalVeiculosRateio - 1;

                Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio veiculoRateio = veiculosParaRateio[indiceVeiculo];

                decimal valorRateio = 0m;
                decimal percentualSobreFaturamentoTotal = 0m;

                if (possuiConfiguracaoUtilizarValorDesproporcional && veiculoRateio.ValorRateio != 0)
                {
                    valorRateio = veiculoRateio.ValorRateio;
                }
                else {
                    if (rateioDespesaVeiculo.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo)
                    {
                        percentualSobreFaturamentoTotal = Math.Floor(veiculoRateio.ValorReceber / valorReceberTotal * 1000000) / 1000000;
                        valorRateio = Math.Floor((percentualSobreFaturamentoTotal * rateioDespesaVeiculo.Valor) * 1000000) / 1000000;
                    }
                    else
                        valorRateio = Math.Round(valorRestanteRatear / totalVeiculosRateio, 6, MidpointRounding.ToEven); //Math.Floor((rateioDespesaVeiculo.Valor / totalVeiculosRateio) * 1000000) / 1000000;

                    if (indiceVeiculo == 0)
                        valorRateio = valorRestanteRatear;
                }               

                totalVeiculosRateio--;

                if (valorRateio <= 0m)
                    continue;

                valorRestanteRatear -= valorRateio;

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento lancamentoDespesaVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento()
                {
                    PercentualSobreFaturamentoTotal = percentualSobreFaturamentoTotal * 100,
                    RateioDespesa = rateioDespesaVeiculo,
                    Valor = valorRateio,
                    ValorFaturamento = veiculoRateio.ValorReceber,
                    Veiculo = new Dominio.Entidades.Veiculo() { Codigo = veiculoRateio.Codigo },
                    CentroResultado = rateioDespesaVeiculo.CentroResultado ?? (veiculoRateio.CodigoCentroResultado > 0 ? new Dominio.Entidades.Embarcador.Financeiro.CentroResultado() { Codigo = veiculoRateio.CodigoCentroResultado } : null)
                };

                repRateioDespesaVeiculoLancamento.Inserir(lancamentoDespesaVeiculo);

                if (rateioDespesaVeiculo.RatearDespesaUmaVezPorMes && rateioDespesaVeiculo.DiaMesRateio > 0 && rateioDespesaVeiculo.DiaMesRateio <= 31)
                {
                    decimal valorRestanteRatearMes = valorRateio;
                    DateTime dataInicialLantamento = rateioDespesaVeiculo.DataInicial.Date;
                    decimal mesesPercorridosCompleto = ((decimal)(rateioDespesaVeiculo.DataFinal.Date - rateioDespesaVeiculo.DataInicial.Date).Days / (decimal)(365.25 / 12)) + (decimal)1;

                    int mesesPercorridos = (int)mesesPercorridosCompleto;

                    while (mesesPercorridos > 0)
                    {
                        DateTime data = AlterarDiadoMes(dataInicialLantamento, rateioDespesaVeiculo.DiaMesRateio);

                        decimal valorRateioMes = Math.Round(valorRestanteRatearMes / mesesPercorridos, 6, MidpointRounding.ToEven);

                        valorRestanteRatearMes -= valorRateioMes;

                        Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia lancamentoDespesaVeiculoDia = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia()
                        {
                            Data = data,
                            Lancamento = lancamentoDespesaVeiculo,
                            Valor = valorRateioMes
                        };

                        repRateioDespesaVeiculoLancamentoDia.Inserir(lancamentoDespesaVeiculoDia);

                        dataInicialLantamento = dataInicialLantamento.AddMonths(1);
                        mesesPercorridos--;
                    }
                }
                else
                {
                    decimal valorRestanteRatearDia = valorRateio;
                    int diasPercorridos = totalDias;

                    while (diasPercorridos > 0)
                    {
                        DateTime data = rateioDespesaVeiculo.DataInicial.AddDays(diasPercorridos - 1);

                        decimal valorRateioDia = Math.Round(valorRestanteRatearDia / diasPercorridos, 6, MidpointRounding.ToEven);

                        valorRestanteRatearDia -= valorRateioDia;

                        Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia lancamentoDespesaVeiculoDia = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia()
                        {
                            Data = data,
                            Lancamento = lancamentoDespesaVeiculo,
                            Valor = valorRateioDia
                        };

                        repRateioDespesaVeiculoLancamentoDia.Inserir(lancamentoDespesaVeiculoDia);

                        diasPercorridos--;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }
        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao Importar(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            List<dynamic> listaVeiculos = new List<dynamic>();
            decimal total = 0;

            int veiculosProcessados = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPlacaVeiculo = (from obj in linha.Colunas where obj.NomeCampo == "PlacaVeiculo" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();

                    string existeCampoVazio = VeirificarCampoVazios(colunaPlacaVeiculo, colunaValor);

                    if (!string.IsNullOrEmpty(existeCampoVazio))
                        throw new ServicoException(existeCampoVazio);

                    string placa = (string)colunaPlacaVeiculo.Valor.Trim();
                    string valorString = (string)colunaValor.Valor.Trim();
                    decimal valor = 0;
                    Decimal.TryParse(valorString, out valor);

                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placa);
                    
                    if (veiculo == null)
                        throw new ServicoException("Veiculo não encontrado");

                    if (valor <= 0)
                        throw new ServicoException("O Valor informado é invalido");

                    var existeVeiculoRepetido = listaVeiculos.Where(v => v.Placa == colunaPlacaVeiculo.Valor).FirstOrDefault();

                    if (existeVeiculoRepetido != null)
                        throw new ServicoException("Veiculo Dublicado");

                    total += valor;
                    listaVeiculos.Add(new
                    {
                        veiculo.Codigo,
                        veiculo.Placa,
                        veiculo.NumeroFrota,
                        ModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        SegmentoVeiculo = veiculo.SegmentoVeiculo?.Descricao ?? string.Empty,
                        Valor = valor
                    });
                    veiculosProcessados++;
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i));
                    unitOfWork.CommitChanges();
                }
                catch (ServicoException exception)
                {
                    unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(exception.Message, i));
                }
                catch (Exception exception)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(exception);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = veiculosProcessados;
            retornoImportacao.Retorno = new
            {
                Total = total,
                ListaVeiculos = listaVeiculos
            };

            return retornoImportacao;
        }
        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCentroResultado(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            List<dynamic> listaCentroResultado = new List<dynamic>();
            decimal total = 0;

            int CentroResultadoProcessados = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    string retorno = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    if (i != 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCentroResultado = (from obj in linha.Colunas where obj.NomeCampo == "CentroResultado" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroCentro = (from obj in linha.Colunas where obj.NomeCampo == "NumeroCentro" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();

                        string existeCampoVazio = VeirificarCampoVaziosCentroResultado(colunaCentroResultado, colunaNumeroCentro, colunaValor);

                        if (!string.IsNullOrEmpty(existeCampoVazio))
                            retorno = existeCampoVazio;

                        if (string.IsNullOrEmpty(existeCampoVazio))
                        {
                            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = null;
                            decimal valor = 0;

                            if (!string.IsNullOrEmpty(colunaCentroResultado?.Valor ?? string.Empty) && string.IsNullOrEmpty(colunaNumeroCentro?.Valor ?? string.Empty))
                                centroResultado = repCentroResultado.BuscarPorDescricao(colunaCentroResultado.Valor);

                            if (!string.IsNullOrEmpty(colunaNumeroCentro?.Valor ?? string.Empty))
                                centroResultado = repCentroResultado.BuscarPorNumeroPlano(colunaNumeroCentro.Valor);

                            Decimal.TryParse(colunaValor.Valor, out valor);

                            if (centroResultado != null)
                            {
                                var existeCentroResutadoNaLista = listaCentroResultado.Where(c => c.Descricao == centroResultado.Descricao && c.Plano == centroResultado.Plano).FirstOrDefault();

                                if (existeCentroResutadoNaLista != null)
                                    retorno = "Centro De Resultado Duplicado";

                                if (existeCentroResutadoNaLista == null)
                                {
                                    total += valor;
                                    listaCentroResultado.Add(new
                                    {
                                        centroResultado.Codigo,
                                        Descricao = centroResultado.Descricao,
                                        centroResultado.Plano,
                                        Valor = valor
                                    });
                                }

                            }
                            if (centroResultado == null)
                                retorno = "Centro de Resultado Não encontrado";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                    }
                    else
                    {
                        CentroResultadoProcessados++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                        unitOfWork.CommitChanges();
                    }
                }
                catch (ServicoException excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = CentroResultadoProcessados;
            retornoImportacao.Retorno = new
            {
                Total = total,
                ListaCentroResultado = listaCentroResultado
            };

            return retornoImportacao;
        }

        #endregion

        #region Métodos Privados

        private DateTime AlterarDiadoMes(DateTime data, int dia)
        {
            DateTime? dataRetorno = null;

            while (dataRetorno == null)
            {
                try
                {
                    dataRetorno = new DateTime(data.Year, data.Month, dia);
                }
                catch
                {
                    dataRetorno = null;
                    dia--;
                }
            }

            return dataRetorno.Value;
        }
        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }
        private string VeirificarCampoVazios(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPlaca, Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValor)
        {
            if (colunaPlaca == null || string.IsNullOrEmpty(colunaPlaca.Valor))
                return "Precisa informar a placa do veiculo";

            if (colunaValor == null || string.IsNullOrEmpty(colunaValor.Valor))
                return "Precisa informar um valor valido";

            return string.Empty;
        }
        private string VeirificarCampoVaziosCentroResultado(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCentroResultado, Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroCentro, Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValor)
        {

            if ((colunaCentroResultado == null || string.IsNullOrEmpty(colunaCentroResultado.Valor)) && (colunaNumeroCentro == null || string.IsNullOrEmpty(colunaNumeroCentro.Valor)))
                return "Precisa informar o Centro de Resultado ou o Número do Centro";

            if (colunaValor == null || string.IsNullOrEmpty(colunaValor.Valor))
                return "Precisa informar um valor valido";

            return string.Empty;
        }
        private void PreencherValoresCentroResultadoNaListaVeiculo(IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio> listVeiculos, List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado> listaCentroResultaDespesa)
        {
            for (int i = 0; i < listaCentroResultaDespesa.Count; i++)
            {
                var centroResultao = listaCentroResultaDespesa[i];
                var veiculosVinculadosAoCentro = listVeiculos.Where(v => v.CodigoCentroResultado == centroResultao.CentroResultado.Codigo).ToList();
                var totalVeiculosVinculados = veiculosVinculadosAoCentro.Count;

                if (totalVeiculosVinculados == 0)
                    continue;

                decimal valorPorVeiculo = Math.Round(centroResultao.Valor / totalVeiculosVinculados, 6, MidpointRounding.ToEven); 
                decimal totalRateado = 0m;

                for (int j = 0; j < totalVeiculosVinculados; j++)
                { 
                    var veiculo = veiculosVinculadosAoCentro[j];

                    if (j == totalVeiculosVinculados - 1)
                        veiculo.ValorRateio = centroResultao.Valor - totalRateado;
                    else
                    {
                        veiculo.ValorRateio = valorPorVeiculo;
                        totalRateado += valorPorVeiculo;
                    }
                }

            }
        }
        private void PreencherValoresNaListaVeiculo(IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio> listVeiculos, List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo> listaVeiculoDespesa)
        {
            for (int i = 0; i < listVeiculos.Count; i++)
            {
                var existe = listaVeiculoDespesa.Find(v => v.Veiculo.Codigo == listVeiculos[i].Codigo);
                if (existe != null)
                    listVeiculos[i].ValorRateio = existe.Valor;
            }
        }

        #endregion
    }
}

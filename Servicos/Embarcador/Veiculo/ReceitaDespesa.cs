using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Veiculo
{
    public class ReceitaDespesa
    {
        public static void ProcessarReceitasEDespesas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa repVeiculoProcessamentoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa(unitOfWork);

            DateTime dataUltimoProcessamento = repVeiculoProcessamentoReceitaDespesa.BuscarUltimaDataRealizada() ?? new DateTime(1900, 01, 01);
            DateTime dataProcessamentoAtual = DateTime.Now;

            ProcessarDespesasAcertoViagem(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarDespesasDocumentoEntrada(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarDespesasOrdemServico(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarReceitasDespesasTitulos(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarDespesasAbastecimento(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarDespesasPedagio(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            ProcessarReceitasCTe(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);
            
            RegistrarProcessamento(dataProcessamentoAtual, unitOfWork);
        }

        private static void ProcessarReceitasCTe(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<int> codigosCTesAutorizados = repCTe.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual);

            foreach (int codigoCTe in codigosCTesAutorizados)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                repVeiculoReceitaDespesa.DeletarPorCTe(cte.Codigo);

                if (cte.Status == "A")
                {
                    int countVeiculos = cte.Veiculos.Count;

                    IEnumerable<decimal> valores = DistribuirValores(cte.ValorAReceber, countVeiculos);

                    for (var i = 0; i < countVeiculos; i++)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                        {
                            CTe = cte,
                            Data = cte.DataEmissao.Value,
                            Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.CTe,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Receita,
                            Valor = valores.ElementAt(i),
                            Veiculo = cte.Veiculos[i].Veiculo
                        };

                        repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                    }
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarDespesasDocumentoEntrada(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            List<int> codigosDocumentosEntrada = repDocumentoEntrada.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual);

            foreach (int codigoDocumentoEntrada in codigosDocumentosEntrada)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);

                repVeiculoReceitaDespesa.DeletarPorDocumentoEntrada(documentoEntrada.Codigo);

                if (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                {
                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in documentoEntrada.Itens)
                    {
                        if (item.Produto.ProdutoCombustivel.HasValue && item.Produto.ProdutoCombustivel.Value == true)
                            continue;

                        if (item.Veiculo != null)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                            {
                                ItemDocumentoEntrada = item,
                                Data = item.DocumentoEntrada.DataEmissao,
                                Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.DocumentoEntrada,
                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                                Valor = item.ValorTotal,
                                Veiculo = item.Veiculo
                            };

                            repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                        }
                        else
                        {
                            decimal valorTotalRateado = 0m;
                            int countLancamentosCentroResultado = item.DocumentoEntrada.LancamentosCentroResultado.Count;

                            for (int z = 0; z < countLancamentosCentroResultado; z++)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = item.DocumentoEntrada.LancamentosCentroResultado[z];

                                List<Dominio.Entidades.Veiculo> veiculos = null;

                                if (lancamentoCentroResultado.CentroResultado.Veiculos.Count > 0)
                                    veiculos = lancamentoCentroResultado.CentroResultado.Veiculos.ToList();
                                else if (lancamentoCentroResultado.CentroResultado.SegmentoVeiculo != null)
                                    veiculos = repVeiculo.BuscarPorSegmento(lancamentoCentroResultado.CentroResultado.SegmentoVeiculo.Codigo);

                                int countVeiculos = veiculos?.Count ?? 0;

                                if (countVeiculos <= 0)
                                    continue;

                                decimal valorRateado = 0m;

                                if ((z + 1) == countLancamentosCentroResultado)
                                    valorRateado = item.ValorTotal - valorTotalRateado;
                                else
                                    valorRateado = Math.Round(Math.Floor(item.ValorTotal / lancamentoCentroResultado.Percentual) * 100, 2, MidpointRounding.AwayFromZero);

                                if (valorRateado <= 0m)
                                    continue;

                                valorTotalRateado += valorRateado;

                                IEnumerable<decimal> valores = DistribuirValores(valorRateado, countVeiculos);

                                for (int i = 0; i < countVeiculos; i++)
                                {
                                    Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                                    {
                                        ItemDocumentoEntrada = item,
                                        Data = item.DocumentoEntrada.DataEmissao,
                                        Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.DocumentoEntrada,
                                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                                        Valor = valores.ElementAt(i),
                                        Veiculo = veiculos[i]
                                    };

                                    repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                                }
                            }
                        }
                    }
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarDespesasAbastecimento(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

            List<int> codigosAbastecimentos = repAbastecimento.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual);

            foreach (int codigoAbastecimento in codigosAbastecimentos)
            {
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento);

                repVeiculoReceitaDespesa.DeletarPorAbastecimento(abastecimento.Codigo);

                if (abastecimento.Veiculo != null && abastecimento.Status == "A" && abastecimento.Situacao == "F")
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                    {
                        Abastecimento = abastecimento,
                        Data = abastecimento.Data.Value,
                        Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.Abastecimento,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                        Valor = abastecimento.ValorTotal,
                        Veiculo = abastecimento.Veiculo
                    };

                    repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarDespesasPedagio(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

            List<int> codigosPedagios = repPedagio.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual);

            foreach (int codigoPedagio in codigosPedagios)
            {
                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(codigoPedagio);

                repVeiculoReceitaDespesa.DeletarPorPedagio(pedagio.Codigo);

                if (pedagio.Veiculo != null && pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado)
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                    {
                        Pedagio = pedagio,
                        Data = pedagio.Data,
                        Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.Pedagio,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                        Valor = pedagio.Valor,
                        Veiculo = pedagio.Veiculo
                    };

                    repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarReceitasDespesasTitulos(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<int> codigosTitulos = repTitulo.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual);

            foreach (int codigoTitulo in codigosTitulos)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                repVeiculoReceitaDespesa.DeletarPorTitulo(titulo.Codigo);

                if (titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo tipo = titulo.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Receita : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa;

                    if (titulo.Veiculos.Count > 0)
                    {
                        int countVeiculos = titulo.Veiculos.Count;

                        IEnumerable<decimal> valores = DistribuirValores(titulo.ValorOriginal, countVeiculos);

                        for (int i = 0; i < countVeiculos; i++)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                            {
                                Titulo = titulo,
                                Data = titulo.DataEmissao.Value,
                                Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.Titulo,
                                Tipo = tipo,
                                Valor = valores.ElementAt(i),
                                Veiculo = titulo.Veiculos.ElementAt(i)
                            };

                            repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                        }
                    }
                    else
                    {
                        decimal valorTotalRateado = 0m;
                        int countLancamentosCentroResultado = titulo.LancamentosCentroResultado.Count;

                        for (int z = 0; z < countLancamentosCentroResultado; z++)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = titulo.LancamentosCentroResultado[z];

                            List<Dominio.Entidades.Veiculo> veiculos = null;

                            if (lancamentoCentroResultado.CentroResultado.Veiculos.Count > 0)
                                veiculos = lancamentoCentroResultado.CentroResultado.Veiculos.ToList();
                            else if (lancamentoCentroResultado.CentroResultado.SegmentoVeiculo != null)
                                veiculos = repVeiculo.BuscarPorSegmento(lancamentoCentroResultado.CentroResultado.SegmentoVeiculo.Codigo);

                            int countVeiculos = veiculos?.Count ?? 0;

                            if (countVeiculos <= 0)
                                continue;

                            decimal valorRateado = 0m;

                            if ((z + 1) == countLancamentosCentroResultado)
                                valorRateado = titulo.ValorOriginal - valorTotalRateado;
                            else
                                valorRateado = Math.Round(Math.Floor(titulo.ValorOriginal * lancamentoCentroResultado.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                            if (valorRateado <= 0m)
                                continue;

                            valorTotalRateado += valorRateado;

                            IEnumerable<decimal> valores = DistribuirValores(valorRateado, countVeiculos);

                            for (int i = 0; i < countVeiculos; i++)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                                {
                                    Titulo = titulo,
                                    Data = titulo.DataEmissao.Value,
                                    Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.Titulo,
                                    Tipo = tipo,
                                    Valor = valores.ElementAt(i),
                                    Veiculo = veiculos[i]
                                };

                                repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                            }
                        }
                    }
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarDespesasOrdemServico(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unitOfWork);

            List<int> codigosOrdensServico = repOrdemServico.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);

            foreach(int codigoOrdemServico in codigosOrdensServico)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);

                repVeiculoReceitaDespesa.DeletarPorOrdemServico(ordemServico.Codigo);

                if(ordemServico.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.Finalizada && ordemServico.Veiculo != null)
                {
                    decimal valorTotalOrdemServico = repFechamentoProduto.BuscarValorDocumentoPorOrdemServico(ordemServico.Codigo) - ordemServico.Desconto;

                    Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                    {
                        OrdemServico = ordemServico,
                        Data = ordemServico.DataProgramada,
                        Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.OrdemServico,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                        Valor = valorTotalOrdemServico,
                        Veiculo = ordemServico.Veiculo
                    };

                    repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void ProcessarDespesasAcertoViagem(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

            List<int> codigosAcertosViagem = repAcertoViagem.BuscarCodigosPorDataAlteracao(dataUltimoProcessamento, dataProcessamentoAtual, unitOfWork);

            foreach (int codigoAcertoViagem in codigosAcertosViagem)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcertoViagem);

                repVeiculoReceitaDespesa.DeletarPorAcertoViagem(acertoViagem.Codigo);

                if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado)
                {
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado> resultados = repAcertoVeiculoResultado.BuscarPorAcerto(acertoViagem.Codigo);
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> diarias = repAcertoDiaria.BuscarPorAcerto(acertoViagem.Codigo);
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> outrasDespesas = repAcertoOutraDespesa.BuscarPorAcerto(acertoViagem.Codigo);

                    int countVeiculos = acertoViagem.Veiculos.Count;

                    IEnumerable<decimal> valoresDiarias = DistribuirValores(diarias.Sum(o => o.Valor), countVeiculos);
                    IEnumerable<decimal> valoresResultado = DistribuirValores(resultados.Sum(o => o.ValorComissao), countVeiculos);

                    for(int i = 0; i < countVeiculos; i ++)
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo acertoVeiculo = acertoViagem.Veiculos[i];

                        decimal valorTotalDespesaVeiculo = outrasDespesas.Where(o => o.Veiculo.Codigo == acertoVeiculo.Veiculo.Codigo).Sum(o => o.Valor) + valoresDiarias.ElementAt(i) + valoresResultado.ElementAt(i);

                        Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa receitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa()
                        {
                            AcertoViagem = acertoViagem,
                            Data = acertoViagem.DataAcerto,
                            Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemReceitaDespesaVeiculo.AcertoViagem,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoReceitaDespesaVeiculo.Despesa,
                            Valor = valorTotalDespesaVeiculo,
                            Veiculo = acertoVeiculo.Veiculo
                        };

                        repVeiculoReceitaDespesa.Inserir(receitaDespesa);
                    }
                }

                unitOfWork.FlushAndClear();
            }
        }

        private static void RegistrarProcessamento(DateTime dataProcessamentoAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa repVeiculoProcessamentoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa veiculoProcessamentoReceitaDespesa = new Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa()
            {
                Data = dataProcessamentoAtual
            };

            repVeiculoProcessamentoReceitaDespesa.Inserir(veiculoProcessamentoReceitaDespesa);
        }

        private static IEnumerable<decimal> DistribuirValores(decimal valor, int quantidade)
        {
            while (quantidade > 0)
            {
                decimal valorRateado = Math.Round(valor / quantidade, 2);
                valor -= valorRateado;
                quantidade--;
                yield return valorRateado;
            }
        }
    }
}

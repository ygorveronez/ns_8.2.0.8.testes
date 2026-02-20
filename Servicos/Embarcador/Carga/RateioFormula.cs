using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class RateioFormula : ServicoBase
    {        
        public RateioFormula() : base() { }

        public RateioFormula(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void DefinirFormulaRateio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedidos.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = ObterFormulaDeRateio(carga, unitOfWork, cargaPedidos.First());

            if (formulaRateio == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.FormulaRateio != null)
                    continue;

                cargaPedido.FormulaRateio = formulaRateio;

                if (cargaPedido.FormulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                {
                    if (cargaPedido.TipoUsoFatorCubagemRateioFormula == null)
                        cargaPedido.TipoUsoFatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.TipoUsoFatorCubagemRateioFormula ?? null;

                    if ((cargaPedido.FatorCubagemRateioFormula ?? 0) == 0)
                        cargaPedido.FatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.FatorCubagemRateioFormula ?? null;
                }

                repositorioCargaPedido.Atualizar(cargaPedido);
            }
        }

        public void SubstituirFormulaDeRateio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedidos.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = ObterFormulaDeRateio(carga, unitOfWork, cargaPedidos.First());

            if (formulaRateio == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.FormulaRateio == formulaRateio)
                    continue;

                cargaPedido.FormulaRateio = formulaRateio;

                if (cargaPedido.FormulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                {
                    cargaPedido.TipoUsoFatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.TipoUsoFatorCubagemRateioFormula ?? null;
                    cargaPedido.FatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.FatorCubagemRateioFormula ?? null;
                }
            }
        }

        public Dominio.Entidades.Embarcador.Rateio.RateioFormula ObterFormulaDeRateioSubContratante(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            //todo:reverRegra retorna Formula de Rateio

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidos.FirstOrDefault().Pedido;
            Dominio.Entidades.Cliente tomador = pedido.SubContratante;

            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = null;

            if (tomador?.GrupoPessoas != null && tomador?.GrupoPessoas?.RateioFormulaExclusivo != null)
                formulaRateio = tomador.GrupoPessoas.RateioFormulaExclusivo;
            if (formulaRateio == null && tomador?.RateioFormulaExclusivo != null)
                formulaRateio = tomador.RateioFormulaExclusivo;

            if (formulaRateio == null)
            {
                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                    formulaRateio = carga.TipoOperacao.RateioFormula;
                else if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                        formulaRateio = (from obj in cargaPedidos select tomador.RateioFormula).FirstOrDefault();
                    else
                        formulaRateio = (from obj in cargaPedidos select tomador.GrupoPessoas.RateioFormula).FirstOrDefault();
                }
            }

            return formulaRateio;
        }

        public Dominio.Entidades.Embarcador.Rateio.RateioFormula ObterFormulaDeRateio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            if (cargaPedido == null)
                cargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarPrimeiroPedidoPorCarga(carga.Codigo);

            if (cargaPedido == null)
                return null;

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (tomador != null)
            {
                if (tomador.GrupoPessoas?.RateioFormulaExclusivo != null)
                    return tomador.GrupoPessoas.RateioFormulaExclusivo;

                if (tomador.RateioFormulaExclusivo != null)
                    return tomador.RateioFormulaExclusivo;

                if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                    return carga.TipoOperacao.RateioFormula;

                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || (tomador.GrupoPessoas == null))
                    return tomador.RateioFormula;

                return tomador.GrupoPessoas?.RateioFormula;
            }
            else
            {
                if (cargaPedido.Pedido.GrupoPessoas?.RateioFormulaExclusivo != null)
                    return cargaPedido.Pedido.GrupoPessoas.RateioFormulaExclusivo;

                if (cargaPedido.Pedido.GrupoPessoas?.RateioFormula != null)
                    return cargaPedido.Pedido.GrupoPessoas.RateioFormula;

                if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                    return carga.TipoOperacao.RateioFormula;
            }

            return null;
        }

        public decimal AplicarFormulaRateio(Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, decimal valorFrete, int quantidadeNotaFiscal, int totalCTes, decimal pesoTotal, decimal pesoParaRateio, decimal valorNotaParaRateio, decimal valorTotalNotasFiscais, decimal percentual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor, int distanciaTotal, int distanciaParaRateio, ref decimal valorRateioOriginal, decimal valorCTe = 0m, decimal percentualTabeladoParaSubcontratacao = 0m, decimal valorTotalCTe = 0m, decimal metrosCubicos = 0m, decimal metrosCubicosTotais = 0m, decimal densidadeProduto = 0m, bool rateioPorComponente = false, decimal pesoLiquido = 0m, decimal pesoLiquidoTotal = 0m, int volume = 0, int volumeTotal = 0, Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso rateioPonderacaoDistanciaPeso = null, bool utilizarFormulaRateioCarga = false, decimal pesoParaRateioFatorCubagem = 0, decimal pesoTotalFatorCubagem = 0)
        {
            decimal valorRateado = 0;
            valorRateioOriginal = 0;

            if (tipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal || utilizarFormulaRateioCarga) //todo: ver quando o tipo for aumento percentual
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula parametroRateio = formulaRateio != null ? formulaRateio.ParametroRateioFormula : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso;//esse é o parâmetro padrão quando não se tem nenhuma formula definida para o cliente.

                if (rateioPorComponente && (parametroRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porFracionadaMetroCubico || parametroRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porFracionadaTonelada))
                    parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso;

                switch (parametroRateio)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso:

                        if (pesoTotal == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / pesoTotal) * pesoParaRateio;

                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PesoLiquido:

                        if (pesoLiquidoTotal == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / pesoLiquidoTotal) * pesoLiquido;

                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.distancia:

                        if (distanciaTotal == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / distanciaTotal) * distanciaParaRateio;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.ValorMercadoria:

                        if (valorTotalNotasFiscais == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / valorTotalNotasFiscais) * valorNotaParaRateio;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe:

                        if (totalCTes == 0)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = valorFrete / totalCTes;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porValorCTe:

                        if (valorTotalCTe == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / valorTotalCTe) * valorCTe;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.MetroCubico:

                        if (metrosCubicosTotais == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / metrosCubicosTotais) * metrosCubicos;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porFracionadaTonelada:

                        if (pesoTotal == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                        {
                            decimal pesoCubado = metrosCubicos * pesoParaRateio;
                            decimal pesoCubadoTotal = metrosCubicosTotais * pesoTotal;

                            if (pesoParaRateio > pesoCubado)
                                valorRateado = (valorFrete) * (pesoParaRateio / 1000);
                            else
                                valorRateado = (valorFrete) * (pesoCubado / 1000);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porFracionadaMetroCubico:

                        if (pesoTotal == 0m || metrosCubicos == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                        {
                            if (densidadeProduto == 0m)
                                densidadeProduto = 300;

                            decimal valorBaseCalculo = valorFrete * (densidadeProduto / 1000) * metrosCubicos;

                            valorRateado = (valorBaseCalculo * (pesoParaRateio / valorBaseCalculo));
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.Volume:

                        if (volume == 0m || volumeTotal == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                        {
                            valorRateado = (valorFrete / volumeTotal) * volume;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.FatorPonderacaoDistanciaPeso:
                        if (rateioPonderacaoDistanciaPeso?.Ponderacao > 0m && rateioPonderacaoDistanciaPeso?.SomaTotalPonderacao > 0m)
                        {
                            decimal porcentagemPonderacao = (rateioPonderacaoDistanciaPeso.Ponderacao * 100) / rateioPonderacaoDistanciaPeso.SomaTotalPonderacao;
                            valorRateado = valorFrete * (porcentagemPonderacao / 100);
                        }
                        else
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem:
                        if (pesoTotalFatorCubagem == 0m || pesoParaRateioFatorCubagem == 0m)
                            parametroRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal;
                        else
                            valorRateado = (valorFrete / pesoTotalFatorCubagem) * pesoParaRateioFatorCubagem;

                        break;

                    default:
                        break;
                }

                if (parametroRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal)
                    valorRateado = valorFrete / quantidadeNotaFiscal;
            }
            else
            {
                valorRateado = valorNotaParaRateio * (percentual / 100);
            }

            valorRateioOriginal = valorRateado;
            if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                return Math.Round(valorRateado, 3, MidpointRounding.ToEven);
            if (formulaRateio?.ArredondarParaNumeroParMaisProximo ?? false)
                return Math.Round(valorRateado, 2, MidpointRounding.ToEven);
            else
                return Math.Floor(valorRateado * 100) / 100;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso> CalcularRateioPonderacaoDistanciaPeso(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso> rateioPonderacaoDistanciaPesos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso>();

            decimal somaTotalPonderacoes = 0m;
            decimal pesoTotalCarga = cargaPedidos.Sum(o => o.Peso);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.FormulaRateio == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem = pontosPassagem.Where(o => o.Cliente != null && ((cargaPedido.Recebedor != null && cargaPedido.Recebedor.CPF_CNPJ == o.Cliente.CPF_CNPJ) || (cargaPedido.Recebedor == null && cargaPedido.Pedido.Destinatario.CPF_CNPJ == o.Cliente.CPF_CNPJ))).FirstOrDefault();
                if (pontoPassagem == null)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso rateio = new Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso();
                rateio.Distancia = (int)Math.Round(pontoPassagem.DistanciaDireta / 1000.0, MidpointRounding.AwayFromZero);

                if(rateio.Distancia == 0 && pontoPassagem.DistanciaDireta > 0 && pontoPassagem.DistanciaDireta < 500)
                    rateio.Distancia = 1;

                rateio.Cliente = pontoPassagem.Cliente;
                rateio.CargaPedido = cargaPedido;
                rateio.PesoEntrega = cargaPedido.Peso;
                rateio.PesoTotalCarga = pesoTotalCarga;
                rateio.PercentualAcrescimoPesoCarga = cargaPedido.FormulaRateio.PercentualAcrescentarPesoTotalCarga;
                rateio.Ponderacao = (rateio.PesoEntrega + (rateio.PesoTotalCarga * (rateio.PercentualAcrescimoPesoCarga / 100))) * rateio.Distancia;

                somaTotalPonderacoes += rateio.Ponderacao;

                rateioPonderacaoDistanciaPesos.Add(rateio);
            }

            for (int i = 0; i < rateioPonderacaoDistanciaPesos.Count; i++)
            {
                if (rateioPonderacaoDistanciaPesos[i].Ponderacao > 0)
                    rateioPonderacaoDistanciaPesos[i].SomaTotalPonderacao = somaTotalPonderacoes;
            }

            return rateioPonderacaoDistanciaPesos;
        }

        public decimal ObterPesoTotalCubadoFatorCubagem(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals)
        {
            if (pedidoXMLNotaFiscals == null || pedidoXMLNotaFiscals.Count() == 0)
                return 0;

            if (pedidoXMLNotaFiscals.FirstOrDefault().CargaPedido?.FormulaRateio?.ParametroRateioFormula != ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                return 0;

            decimal pesoTotalCubado = 0;

            foreach (var item in pedidoXMLNotaFiscals)
            {
                pesoTotalCubado += ObterPesoCubadoFatorCubagem(item.CargaPedido?.FormulaRateio?.ParametroRateioFormula, item.CargaPedido?.TipoUsoFatorCubagemRateioFormula, item.CargaPedido?.FatorCubagemRateioFormula ?? 0, item.XMLNotaFiscal.Volumes, item.XMLNotaFiscal.Peso, item.XMLNotaFiscal.MetrosCubicos);
            }

             return pesoTotalCubado;

        }


        public decimal ObterPesoCubadoFatorCubagem(ParametroRateioFormula? parametroRateioFormula, TipoUsoFatorCubagem? tipoUsoFatorCubagem, decimal fatorCubagem = 0, decimal volume = 0, decimal peso = 0, decimal metrosCubicos = 0)
        {
            if (parametroRateioFormula != ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                return peso;

            if (fatorCubagem == 0 || tipoUsoFatorCubagem == null || metrosCubicos <= 0)
                return peso;

            decimal pesoCubado = metrosCubicos * fatorCubagem;

            if (tipoUsoFatorCubagem == TipoUsoFatorCubagem.SempreUtilizarPesoConvertido)
                return pesoCubado;
            else
            {
                if (pesoCubado > peso)
                    return pesoCubado;
                else
                    return peso;
            }

        }
        #endregion
    }
}
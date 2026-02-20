using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class RateioCTeComplementar : ServicoBase
    {
        public RateioCTeComplementar(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public string RatearValorDoFrenteEntreCTesComplementares(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementosInfo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula? tipoRateio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote? tipoRateioOcorrenciaLote = null)
        {
            if (tipoRateioOcorrenciaLote.HasValue)
            {
                if (tipoRateioOcorrenciaLote == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote.Peso)
                    return RatearComplementarPeloPeso(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
                else if (tipoRateioOcorrenciaLote == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote.ValorMercadoria)
                    return RatearComplementarPeloValorMercadoria(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
                else
                    return RatearComplementarIgualmenteEntreCTes(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
            }
            else if (configuracaoTMS.RatearValorOcorrenciaPeloValorFreteCTeOriginal)
                return RatearComplementarPeloValorFrete(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
            else if (tipoRateio != null && tipoRateio.HasValue && tipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PesoLiquido)
                return RatearComplementarPeloPesoLiquido(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
            else if (tipoRateio != null && tipoRateio.HasValue && tipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso)
                return RatearComplementarPeloPeso(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
            else
                return RatearComplementarIgualmenteEntreCTes(valorComplemento, cargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, moeda, valorCotacaoMoeda, valorTotalMoeda);
        }

        #endregion

        #region Métodos Privados

        private string RatearComplementarIgualmenteEntreCTes(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementosInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);

            var ultimoComplemento = listaCargaCTeComplementosInfo.LastOrDefault();

            decimal valorTotalRateado = 0m;
            decimal valorTotalMoedaRateado = 0m;
            int countCTes = listaCargaCTeComplementosInfo.Count;

            for (int i = 0; i < listaCargaCTeComplementosInfo.Count; i++)
            {
                var cargaCTeComplementoInfo = listaCargaCTeComplementosInfo[i];

                decimal valorRateado = 0m, valorMoedaRateado = 0m;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorMoedaRateado = Math.Floor(valorTotalMoeda / countCTes * 100) / 100;

                    valorTotalMoedaRateado += valorMoedaRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorMoedaRateado += valorTotalMoeda - valorTotalMoedaRateado;

                    valorRateado = Math.Round(valorMoedaRateado * valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    valorRateado = Math.Round(valorComplemento / countCTes, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                    {
                        decimal diferenca = valorComplemento - valorTotalRateado;
                        valorRateado += diferenca;
                    }
                }

                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeComplementoInfo.ValorTotalMoeda = valorMoedaRateado;
                cargaCTeComplementoInfo.ValorComplemento = valorRateado;
                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);
                string mensagem = CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                if (cargaCTeComplementoInfo.ValorComplemento <= 0m)
                    return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
            }

            return "";
        }

        private string RatearComplementarPeloValorFrete(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementosInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);

            var ultimoComplemento = listaCargaCTeComplementosInfo.LastOrDefault();

            decimal valorTotalFrete = listaCargaCTeComplementosInfo.Sum(o => o.ValorAReceberComplementado);

            if (valorTotalFrete == 0m)
                return RatearComplementarIgualmenteEntreCTes(valorComplemento, listaCargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracao, moeda, valorCotacaoMoeda, valorTotalMoeda);

            decimal valorTotalRateado = 0m, valorTotalMoedaRateado = 0m;

            for (int i = 0; i < listaCargaCTeComplementosInfo.Count; i++)
            {
                var cargaCTeComplementoInfo = listaCargaCTeComplementosInfo[i];

                decimal valorFrete = cargaCTeComplementoInfo.ValorAReceberComplementado != 0 ? cargaCTeComplementoInfo.ValorAReceberComplementado : 1m;
                decimal fatorCalculo = valorFrete / valorTotalFrete;

                decimal valorRateado = 0m, valorMoedaRateado = 0m;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorMoedaRateado = Math.Floor(valorTotalMoeda * fatorCalculo * 100) / 100;

                    valorTotalMoedaRateado += valorMoedaRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorMoedaRateado += valorTotalMoedaRateado - valorTotalMoeda;

                    valorRateado = Math.Round(valorMoedaRateado * valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    valorRateado = Math.Round(valorComplemento * fatorCalculo, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorRateado += valorComplemento - valorTotalRateado;
                }

                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeComplementoInfo.ValorTotalMoeda = valorMoedaRateado;
                cargaCTeComplementoInfo.ValorComplemento = valorRateado;
                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);

                string mensagem = CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                if (cargaCTeComplementoInfo.ValorComplemento <= 0m)
                    return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
            }

            return "";
        }

        private string RatearComplementarPeloPesoLiquido(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementosInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(listaCargaCTeComplementosInfo.FirstOrDefault().CargaOcorrencia.Carga?.Codigo ?? 0);
            var ultimoComplemento = listaCargaCTeComplementosInfo.LastOrDefault();

            decimal pesoLiquidoTotal = cargaPedidos.Sum(o => o.PesoLiquido);

            if (pesoLiquidoTotal == 0m)
                return RatearComplementarIgualmenteEntreCTes(valorComplemento, listaCargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracao, moeda, valorCotacaoMoeda, valorTotalMoeda);

            decimal valorTotalRateado = 0m, valorTotalMoedaRateado = 0m;

            for (int i = 0; i < listaCargaCTeComplementosInfo.Count; i++)
            {
                var cargaCTeComplementoInfo = listaCargaCTeComplementosInfo[i];

                decimal pesoLiquidoCTe = 0;

                if (cargaCTeComplementoInfo.CargaCTeComplementado != null) // Obtém o peso líquido quando o complemento é de CargaCTe
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTeComplementoInfo.CargaCTeComplementado.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargasPedidos)
                        pesoLiquidoCTe = cargaPedido.NotasFiscais.Distinct().Sum(o => o.XMLNotaFiscal.PesoLiquido);
                }
                else if (cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null) // Obtém o peso líquido quando o complemento é de CargaDocumentoParaEmissaoNFSManualComplementado
                {
                    pesoLiquidoCTe = cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                }

                decimal fatorCalculo = (pesoLiquidoCTe > 0 ? pesoLiquidoCTe : 1) / pesoLiquidoTotal;

                decimal valorRateado = 0m, valorMoedaRateado = 0m;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorMoedaRateado = Math.Floor(valorTotalMoeda * fatorCalculo * 100) / 100;

                    valorTotalMoedaRateado += valorMoedaRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorMoedaRateado += valorTotalMoedaRateado - valorTotalMoeda;

                    valorRateado = Math.Round(valorMoedaRateado * valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    valorRateado = Math.Round(valorComplemento * fatorCalculo, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorRateado += valorComplemento - valorTotalRateado;
                }

                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeComplementoInfo.ValorTotalMoeda = valorMoedaRateado;
                cargaCTeComplementoInfo.ValorComplemento = valorRateado;

                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);

                string mensagem = CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                if (cargaCTeComplementoInfo.ValorComplemento <= 0m)
                    return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
            }

            return "";
        }

        private string RatearComplementarPeloPeso(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementosInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(listaCargaCTeComplementosInfo.FirstOrDefault().CargaOcorrencia.Carga?.Codigo ?? 0);
            var ultimoComplemento = listaCargaCTeComplementosInfo.LastOrDefault();

            decimal pesoTotalCTe = (from obj in listaCargaCTeComplementosInfo where obj.CTeComplementado != null select obj.CTeComplementado.Peso).Sum();
            pesoTotalCTe += (from obj in listaCargaCTeComplementosInfo where obj.CargaDocumentoParaEmissaoNFSManualComplementado != null && obj.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal != null select obj.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso).Sum();

            if (pesoTotalCTe == 0m)
                return RatearComplementarIgualmenteEntreCTes(valorComplemento, listaCargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracao, moeda, valorCotacaoMoeda, valorTotalMoeda);

            decimal valorTotalRateado = 0m, valorTotalMoedaRateado = 0m;

            for (int i = 0; i < listaCargaCTeComplementosInfo.Count; i++)
            {
                var cargaCTeComplementoInfo = listaCargaCTeComplementosInfo[i];

                decimal pesoCTe = 0;
                if (cargaCTeComplementoInfo.CargaCTeComplementado != null) // Obtém o peso quando o complemento é de CargaCTe
                    pesoCTe = cargaCTeComplementoInfo.CTeComplementado.Peso;
                else if (cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null) // Obtém o peso quando o complemento é de CargaDocumentoParaEmissaoNFSManualComplementado
                    pesoCTe = cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Peso ?? 0;

                decimal fatorCalculo = (pesoCTe > 0 ? pesoCTe : 1) / pesoTotalCTe;

                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);
                decimal valorRateado = 0m, valorMoedaRateado = 0m;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorMoedaRateado = Math.Floor(valorTotalMoeda * fatorCalculo * 100) / 100;

                    valorTotalMoedaRateado += valorMoedaRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorMoedaRateado += valorTotalMoedaRateado - valorTotalMoeda;

                    valorRateado = Math.Round(valorMoedaRateado * valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    valorRateado = Math.Round(valorComplemento * fatorCalculo, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorRateado += valorComplemento - valorTotalRateado;
                }

                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeComplementoInfo.ValorTotalMoeda = valorMoedaRateado;
                cargaCTeComplementoInfo.ValorComplemento = valorRateado;

                string mensagem = CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                if (cargaCTeComplementoInfo.ValorComplemento <= 0m)
                    return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
            }

            return "";
        }

        private string RatearComplementarPeloValorMercadoria(decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementosInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, decimal valorTotalMoeda)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(listaCargaCTeComplementosInfo.FirstOrDefault().CargaOcorrencia.Carga?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo ultimoComplemento = listaCargaCTeComplementosInfo.LastOrDefault();

            decimal valorTotalMercadoria = cargaPedidos.Sum(o => o.NotasFiscais.Sum(n => n.XMLNotaFiscal.Valor));

            if (valorTotalMercadoria == 0m)
                return RatearComplementarIgualmenteEntreCTes(valorComplemento, listaCargaCTeComplementosInfo, tipoServicoMultisoftware, unitOfWork, configuracao, moeda, valorCotacaoMoeda, valorTotalMoeda);

            decimal valorTotalRateado = 0m, valorTotalMoedaRateado = 0m;

            for (int i = 0; i < listaCargaCTeComplementosInfo.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = listaCargaCTeComplementosInfo[i];

                decimal valorMercadoria = 0;
                if (cargaCTeComplementoInfo.CargaCTeComplementado != null) // Obtém o valor quando o complemento é de CargaCTe
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTeComplementoInfo.CargaCTeComplementado.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargasPedidos)
                        valorMercadoria = cargaPedido.NotasFiscais.Distinct().Sum(o => o.XMLNotaFiscal.Valor);
                }
                else if (cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null) // Obtém o valor quando o complemento é de CargaDocumentoParaEmissaoNFSManualComplementado
                {
                    valorMercadoria = cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor;
                }

                decimal fatorCalculo = (valorMercadoria > 0 ? valorMercadoria : 1) / valorTotalMercadoria;

                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);
                decimal valorRateado = 0m, valorMoedaRateado = 0m;

                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorMoedaRateado = Math.Floor(valorTotalMoeda * fatorCalculo * 100) / 100;

                    valorTotalMoedaRateado += valorMoedaRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorMoedaRateado += valorTotalMoedaRateado - valorTotalMoeda;

                    valorRateado = Math.Round(valorMoedaRateado * valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    valorRateado = Math.Round(valorComplemento * fatorCalculo, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorRateado;

                    if (cargaCTeComplementoInfo.Equals(ultimoComplemento))
                        valorRateado += valorComplemento - valorTotalRateado;
                }

                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeComplementoInfo.ValorTotalMoeda = valorMoedaRateado;
                cargaCTeComplementoInfo.ValorComplemento = valorRateado;

                string mensagem = CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                if (cargaCTeComplementoInfo.ValorComplemento <= 0m)
                    return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
            }

            return "";
        }

        private string InformarDadosContabeisCargaCTeComplementoInfo(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return retorno;

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplemento = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao;
            Dominio.Entidades.RotaFrete rotaFrete = cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.Rota;

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCTeComplementoInfo.CargaCTeComplementado?.CTe;
            if (cteComplementado == null & cargaCTeComplementoInfo.CargaOcorrencia?.Cargas != null && cargaCTeComplementoInfo.CargaOcorrencia.Cargas.Count > 0)
            {
                var cteComLocalidaDoTomador = from o in cargaCTeComplementoInfo.CargaOcorrencia.Cargas.FirstOrDefault().CargaCTes
                                              where o.CTe != null && o.CargaCTeComplementoInfo == null
                                              select o.CTe;
                cteComplementado = cteComLocalidaDoTomador.FirstOrDefault();
            }

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(cteComplementado?.Remetente?.Cliente, cteComplementado?.Destinatario?.Cliente, cargaCTeComplementoInfo.TomadorPagador, null, cteComplementado?.Empresa, tipoOperacao, rotaFrete, null, cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia, unitOfWork);

            if ((configuracaoContaContabil != null) && (configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes?.Count > 0))
            {
                if (cargaCTeComplementoInfo.Codigo == 0)
                    repCargaCTeComplemento.Inserir(cargaCTeComplementoInfo);

                foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracaoContabilizacao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao cargaCTeComplementoInfoContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao();
                    cargaCTeComplementoInfoContaContabilContabilizacao.CargaCTeComplementoInfo = cargaCTeComplementoInfo;
                    if (configuracaoContabilizacao.CodigoPlanoConta > 0)
                        cargaCTeComplementoInfoContaContabilContabilizacao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoConta };
                    cargaCTeComplementoInfoContaContabilContabilizacao.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                    if (configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao > 0)
                        cargaCTeComplementoInfoContaContabilContabilizacao.PlanoContaContraPartida = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao };
                    cargaCTeComplementoInfoContaContabilContabilizacao.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                    repCargaCTeComplementoInfoContaContabilContabilizacao.Inserir(cargaCTeComplementoInfoContaContabilContabilizacao);
                }
            }
            else if ((cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                retorno = "Não foi localizada uma configuração contábil compatível para complementar o CT-e " + cteComplementado?.Numero ?? "";

            bool logisticaReversa = tipoOperacao?.LogisticaReversa ?? false;

            Dominio.Entidades.Cliente destinatario = !logisticaReversa ? cteComplementado?.Destinatario?.Cliente : cteComplementado?.Remetente?.Cliente;
            Dominio.Entidades.Cliente remetente = !logisticaReversa ? cteComplementado?.Remetente?.Cliente : cteComplementado?.Destinatario?.Cliente;
            Dominio.Entidades.Cliente expedidor = !logisticaReversa ? cteComplementado?.Expedidor?.Cliente : cteComplementado?.Recebedor?.Cliente;
            Dominio.Entidades.Cliente recebedor = !logisticaReversa ? cteComplementado?.Recebedor?.Cliente : cteComplementado?.Expedidor?.Cliente;
            Dominio.Entidades.Localidade origem = !logisticaReversa ? cteComplementado?.LocalidadeInicioPrestacao : cteComplementado?.LocalidadeTerminoPrestacao;
            if (configuracao.ArmazenarCentroCustoDestinatario)
                destinatario = null;

            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, cargaCTeComplementoInfo.TomadorPagador, null, null, cteComplementado?.Empresa, tipoOperacao, cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia, rotaFrete, cargaCTeComplementoInfo?.CargaOcorrencia?.Filial, origem, unitOfWork);
            if (configuracaoCentroResultado != null)
            {
                cargaCTeComplementoInfo.CentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao;
                cargaCTeComplementoInfo.ItemServico = configuracaoCentroResultado.ItemServico;
                cargaCTeComplementoInfo.ValorMaximoCentroContabilizacao = configuracaoCentroResultado.ValorMaximoCentroContabilizacao;
                cargaCTeComplementoInfo.CentroResultadoEscrituracao = configuracaoCentroResultado.CentroResultadoEscrituracao;
                cargaCTeComplementoInfo.CentroResultadoICMS = configuracaoCentroResultado.CentroResultadoICMS;
                cargaCTeComplementoInfo.CentroResultadoPIS = configuracaoCentroResultado.CentroResultadoPIS;
                cargaCTeComplementoInfo.CentroResultadoCOFINS = configuracaoCentroResultado.CentroResultadoCOFINS;
            }
            else
            {
                cargaCTeComplementoInfo.CentroResultado = null;
                cargaCTeComplementoInfo.CentroResultadoICMS = null;
                cargaCTeComplementoInfo.CentroResultadoPIS = null;
                cargaCTeComplementoInfo.CentroResultadoCOFINS = null;
                cargaCTeComplementoInfo.ItemServico = "";
                cargaCTeComplementoInfo.CentroResultadoEscrituracao = null;
                cargaCTeComplementoInfo.ValorMaximoCentroContabilizacao = 0;
                if ((cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                    retorno = "Não foi localizada uma configuração centro de resultado compatível para complementar o CT-e " + cteComplementado?.Numero ?? "";
            }


            if (configuracao.ArmazenarCentroCustoDestinatario)
            {
                destinatario = !logisticaReversa ? cteComplementado?.Destinatario?.Cliente : cteComplementado?.Remetente?.Cliente;

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoDestinatario = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, destinatario, null, null, cargaCTeComplementoInfo.TomadorPagador, null, null, cteComplementado?.Empresa, tipoOperacao, cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia, rotaFrete, cargaCTeComplementoInfo?.CargaOcorrencia?.Filial, origem, unitOfWork);
                if (configuracaoCentroResultadoDestinatario != null)
                    cargaCTeComplementoInfo.CentroResultadoDestinatario = configuracaoCentroResultadoDestinatario.CentroResultadoContabilizacao;
                else
                {
                    cargaCTeComplementoInfo.CentroResultadoDestinatario = null;
                    if ((cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false))
                        retorno = "Não foi localizada uma configuração centro de resultado do destinatário compatível para complementar o CT-e " + cteComplementado?.Numero ?? "";
                }
            }

            return retorno;
        }

        public string CalcularImpostosComplementoInfo(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string mensagem = "";
            Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar servicoCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = null;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Empresa empresa = null;

            if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
            {
                if (!cargaCteComplementarInfo.ComplementoFilialEmissora)
                {
                    if (((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cargaCteComplementarInfo.CargaCTeComplementado?.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe) && cargaCteComplementarInfo.CTeComplementado?.Empresa != null) || (cargaCteComplementarInfo.CargaOcorrencia.Usuario != null && cargaCteComplementarInfo.CargaOcorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro) || (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaTerceiros))
                        empresa = cargaCteComplementarInfo.CTeComplementado.Empresa;
                    else
                        empresa = cargaCteComplementarInfo.CargaOcorrencia.Carga.Empresa;
                }
                else
                    empresa = cargaCteComplementarInfo.CTeComplementado.Empresa;
            }
            else if (cargaCteComplementarInfo.FechamentoFrete != null)
            {
                if (cargaCteComplementarInfo.FechamentoFrete.Contrato.Transportador != null)
                    empresa = cargaCteComplementarInfo.FechamentoFrete.Contrato.Transportador;
                else
                    empresa = cargaCteComplementarInfo.CargaCTeComplementado.Carga.Empresa;
            }
            else
                empresa = cargaCteComplementarInfo.CargaOcorrencia.Emitente;

            if (cargaCteComplementarInfo.CargaCTeComplementado != null)
                cteComplementado = cargaCteComplementarInfo.CargaCTeComplementado.CTe;
            else if (cargaCteComplementarInfo.CargaOcorrencia.Cargas != null && cargaCteComplementarInfo.CargaOcorrencia.Cargas.Count > 0)
            {
                var cteComLocalidaDoTomador = (
                    from o in cargaCteComplementarInfo.CargaOcorrencia.Cargas.FirstOrDefault().CargaCTes
                    where o.CTe != null && o.CargaCTeComplementoInfo == null
                    select o.CTe
                );

                cteComplementado = cteComLocalidaDoTomador.FirstOrDefault();
            }

            Dominio.Enumeradores.TipoServico tipoServico = cteComplementado?.TipoServico ?? Dominio.Enumeradores.TipoServico.Normal;

            Dominio.Entidades.Localidade origem = cteComplementado?.LocalidadeInicioPrestacao;
            Dominio.Entidades.Localidade destino = cteComplementado?.LocalidadeTerminoPrestacao;

            Dominio.Entidades.Cliente tomador = cteComplementado != null && cteComplementado.TomadorPagador != null ? cteComplementado.TomadorPagador.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.TomadorPagador.CPF_CNPJ)) : null;
            Dominio.Enumeradores.TipoTomador tipoTomador = cteComplementado?.TipoTomador ?? Dominio.Enumeradores.TipoTomador.Remetente;
            Dominio.Enumeradores.TipoPagamento tipoPagamento = cteComplementado?.TipoPagamento ?? Dominio.Enumeradores.TipoPagamento.Pago;

            Dominio.Entidades.Cliente remetente = cteComplementado != null && cteComplementado.Remetente != null ? cteComplementado.Remetente.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Remetente.CPF_CNPJ)) : null;
            Dominio.Entidades.Cliente destinatario = null;
            if (cteComplementado != null && cteComplementado.Destinatario != null)
            {
                if (!string.IsNullOrWhiteSpace(cteComplementado.Destinatario.CPF_CNPJ))
                    destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Destinatario.CPF_CNPJ));
                else if (cteComplementado.Destinatario?.Cliente != null)
                    destinatario = cteComplementado.Destinatario.Cliente;
            }
            if (remetente == null && cteComplementado != null && cteComplementado.Expedidor != null)
                remetente = cteComplementado != null && cteComplementado.Expedidor != null ? cteComplementado.Expedidor.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Expedidor.CPF_CNPJ)) : null;
            if (destinatario == null && cteComplementado != null && cteComplementado.Recebedor != null)
                destinatario = cteComplementado != null && cteComplementado.Recebedor != null ? cteComplementado.Recebedor.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Recebedor.CPF_CNPJ)) : null;

            bool complementoICMS = false;

            if (cargaCteComplementarInfo.ComponenteFrete != null && cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                complementoICMS = true;

            if (tomador == null && cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.ContratoFrete != null && cargaCteComplementarInfo.CargaOcorrencia.ContratoFrete.ClienteTomador == null)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.ObterCargaPorContrato(cargaCteComplementarInfo.CargaOcorrencia.ContratoFrete.Codigo);

                if (carga == null)
                    return "Não foi gerada nenhuma carga para esse contrato de frete, sendo assim não é possível gerar improdutividade deste contrato.";

                tomador = carga.Pedidos.FirstOrDefault().ObterTomador();
            }

            if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.Responsavel.HasValue && cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value != Dominio.Enumeradores.TipoTomador.NaoInformado)
            {
                if (cteComplementado == null ||
                    cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value != cteComplementado.TipoTomador ||
                    (cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value == Dominio.Enumeradores.TipoTomador.Outros &&
                     cteComplementado.Tomador?.CPF_CNPJ_SemFormato != cargaCteComplementarInfo.CargaOcorrencia.Tomador?.CPF_CNPJ_SemFormato))
                {
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                    tipoServico = Dominio.Enumeradores.TipoServico.Normal;

                    tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                    if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Outros)
                        tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    else if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Remetente)
                        tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    else
                        tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;

                    if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    {
                        tipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                        tomador = remetente;
                    }
                    else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    {
                        tipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                        tomador = destinatario;
                    }
                    else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    {
                        tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                        tomador = cargaCteComplementarInfo.CargaOcorrencia.Tomador;
                    }
                }
            }

            bool adicionarPISCOFINS = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.AdicionarPISCOFINS ?? false;
            bool adicionarPISCOFINSBaseCalculoICMS = adicionarPISCOFINS && (cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.AdicionarPISCOFINSBaseCalculoICMS ?? false);
            if (adicionarPISCOFINS)
                cargaCteComplementarInfo.AliquotaPISCOFINS = (empresa.Configuracao?.AliquotaPIS ?? 0m) + (empresa.Configuracao?.AliquotaCOFINS ?? 0m);

            if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal != null)
            {
                if (cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    modeloDocumentoFiscal = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscalEmissaoMunicipal;
                else
                    modeloDocumentoFiscal = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal;

            }
            else
            {
                if (cargaCteComplementarInfo.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cargaCteComplementarInfo.CargaOcorrencia?.Carga != null && cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado == null)
                {
                    if (cargaCteComplementarInfo.CargaOcorrencia.Carga.CargaTransbordo && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                        Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                        bool possuiCTe = false;
                        bool possuiNFS = false;
                        bool possuiNFSManual = false;
                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTransbordo = cargaCteComplementarInfo.CargaOcorrencia.Carga.Pedidos.FirstOrDefault();
                        serCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaCteComplementarInfo.CargaOcorrencia.Carga, cargaPedidoTransbordo, cargaPedidoTransbordo.Origem, cteComplementado.LocalidadeTerminoPrestacao, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual);
                        if (modeloDocumentoIntramunicipal != null)
                            modeloDocumentoFiscal = modeloDocumentoIntramunicipal;
                        else if (possuiCTe)
                            modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
                        else if (possuiNFS)
                            modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
                        else if (possuiNFSManual)
                            modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS);
                    }
                    else
                        modeloDocumentoFiscal = cteComplementado?.ModeloDocumentoFiscal ?? new Dominio.Entidades.ModeloDocumentoFiscal() { TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.NFSe };//se não tem indica que a emissão será por contrato e isso é por nota.
                }
                else
                {
                    if (cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null)
                        modeloDocumentoFiscal = cteComplementado?.ModeloDocumentoFiscal ?? new Dominio.Entidades.ModeloDocumentoFiscal() { TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.NFS };//se não tem indica que a emissão será por contrato e isso é por nota.
                    else
                        modeloDocumentoFiscal = cteComplementado?.ModeloDocumentoFiscal ?? new Dominio.Entidades.ModeloDocumentoFiscal() { TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.NFSe };//se não tem indica que a emissão será por contrato e isso é por nota.
                }
            }

            if (modeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                if (empresa.Codigo != cteComplementado.Empresa.Codigo && cargaCteComplementarInfo.CargaOcorrencia?.Carga != null)
                {
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                    if (cargaCteComplementarInfo.CargaOcorrencia.Carga.CargaTransbordo && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        origem = cargaCteComplementarInfo.CargaOcorrencia.Carga.Pedidos.FirstOrDefault().Origem;
                }

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cteComplementado);
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                if (tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    if (complementoICMS)
                    {
                        regraICMS.IncluirICMSBC = false;

                        if ((cargaCteComplementarInfo.CargaOcorrencia?.ValorICMS ?? 0) > 0m)
                        {
                            regraICMS.ValorBaseCalculoICMS = cargaCteComplementarInfo.CargaOcorrencia.BaseCalculoICMS;
                            regraICMS.ValorBaseCalculoPISCOFINS = cargaCteComplementarInfo.CargaOcorrencia.BaseCalculoICMS;
                            regraICMS.Aliquota = cargaCteComplementarInfo.CargaOcorrencia.AliquotaICMS;
                            regraICMS.ValorICMS = cargaCteComplementarInfo.CargaOcorrencia.ValorICMS;
                            regraICMS.ValorICMSIncluso = cargaCteComplementarInfo.CargaOcorrencia.ValorICMS;
                            regraICMS.CST = ObterCSTICMSOcorrencia(cargaCteComplementarInfo.CargaOcorrencia);
                        }
                        else if ((cargaCteComplementarInfo.CargaOcorrencia?.AliquotaICMS ?? 0) > 0m)
                        {
                            regraICMS.ValorBaseCalculoICMS = cteComplementado?.BaseCalculoICMS ?? 0m;
                            regraICMS.ValorBaseCalculoPISCOFINS = cteComplementado?.BaseCalculoICMS ?? 0m;
                            regraICMS.Aliquota = cargaCteComplementarInfo.CargaOcorrencia.AliquotaICMS;
                            regraICMS.ValorICMS = Math.Round(regraICMS.ValorBaseCalculoICMS * (cargaCteComplementarInfo.CargaOcorrencia.AliquotaICMS / 100), 2, MidpointRounding.AwayFromZero);
                            regraICMS.ValorICMSIncluso = Math.Round(regraICMS.ValorBaseCalculoICMS * (cargaCteComplementarInfo.CargaOcorrencia.AliquotaICMS / 100), 2, MidpointRounding.AwayFromZero);
                            regraICMS.CST = ObterCSTICMSOcorrencia(cargaCteComplementarInfo.CargaOcorrencia);
                        }

                        if (regraICMS.CST == "40" || regraICMS.CST == "")
                        {
                            bool incluirBC = regraICMS.IncluirICMSBC;
                            decimal percentualInclusao = regraICMS.PercentualInclusaoBC;
                            decimal baseDeCalculo = cargaCteComplementarInfo.ValorComplemento;
                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMSCST = serICMS.BuscarRegraICMS(cargaCteComplementarInfo.CargaComplementado, null, empresa, remetente, destinatario, tomador, origem, destino, ref incluirBC, ref percentualInclusao, baseDeCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal, null, true, tipoCTe);
                            regraICMS.CST = regraICMSCST.CST;
                        }
                    }
                    else
                    {
                        decimal baseDeCalculo = cargaCteComplementarInfo.ValorComplemento;

                        if (cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.BuscarCSTQuandoDocumentoOrigemIsento ?? false)
                        {
                            regraICMS.IncluirICMSBC = configuracao.IncluirICMSFreteInformadoManualmente;
                            regraICMS.PercentualInclusaoBC = 100;

                            bool incluirBC = regraICMS.IncluirICMSBC;
                            decimal percentualInclusao = regraICMS.PercentualInclusaoBC;
                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMSCST = serICMS.BuscarRegraICMS(cargaCteComplementarInfo.CargaComplementado, null, empresa, remetente, destinatario, tomador, origem, destino, ref incluirBC, ref percentualInclusao, baseDeCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal, null, false, tipoCTe);
                            regraICMS.CST = regraICMSCST.CST;
                            regraICMS.Aliquota = regraICMSCST.Aliquota;
                        }
                        else if (cteComplementado.BaseCalculoICMS == 0)
                            baseDeCalculo = 0;

                        decimal aliquotaPisCofins = cargaCteComplementarInfo.AliquotaPISCOFINS;
                        if (regraICMS.AliquotaPis + regraICMS.AliquotaCofins > 0)
                            aliquotaPisCofins = regraICMS.AliquotaPis + regraICMS.AliquotaCofins;

                        if (cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia.TipoInclusaoImpostoComplemento.HasValue ?? false)
                        {
                            if (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.TipoInclusaoImpostoComplemento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoImpostoComplemento.SempreIncluir)
                                regraICMS.IncluirICMSBC = true;
                            else if (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.TipoInclusaoImpostoComplemento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoImpostoComplemento.NuncaIncluir)
                                regraICMS.IncluirICMSBC = false;
                        }

                        regraICMS.ValorICMSIncluso = Math.Round(serICMS.CalcularICMSInclusoNoFrete(regraICMS.CST, ref baseDeCalculo, regraICMS.Aliquota, regraICMS.PercentualInclusaoBC, regraICMS.PercentualReducaoBC, regraICMS.IncluirICMSBC, regraICMS.AliquotaInternaDifal, aliquotaPisCofins, adicionarPISCOFINSBaseCalculoICMS), 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorICMS = Math.Round(serICMS.CalcularInclusaoICMSNoFrete(regraICMS.CST, ref baseDeCalculo, regraICMS.Aliquota, regraICMS.PercentualInclusaoBC, regraICMS.PercentualReducaoBC, regraICMS.IncluirICMSBC, aliquotaPisCofins, adicionarPISCOFINSBaseCalculoICMS), 2, MidpointRounding.AwayFromZero);
                        regraICMS.ValorBaseCalculoPISCOFINS = baseDeCalculo;

                        if (regraICMS.AliquotaPis + regraICMS.AliquotaCofins > 0)
                        {
                            Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                            regraICMS.ValorPis = Math.Round(servicoPisCofins.CalcularValorPis(regraICMS.AliquotaPis, regraICMS.ValorBaseCalculoPISCOFINS), 2, MidpointRounding.AwayFromZero);
                            regraICMS.ValorCofins = Math.Round(servicoPisCofins.CalcularValorCofins(regraICMS.ValorCofins, regraICMS.ValorBaseCalculoPISCOFINS), 2, MidpointRounding.AwayFromZero);
                        }

                        regraICMS.ValorBaseCalculoICMS = baseDeCalculo;

                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cteComplementado, cargaCteComplementarInfo.ValorComplemento, cteComplementado?.OutrasAliquotas?.Codigo);
                    }

                    cargaCteComplementarInfo.CFOP = cteComplementado.CFOP;
                }
                else
                {
                    bool incluirBC = regraICMS.IncluirICMSBC;
                    decimal percentualInclusao = regraICMS.PercentualInclusaoBC;
                    decimal baseDeCalculo = cargaCteComplementarInfo.ValorComplemento;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                    if (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                        tipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                    regraICMS = serICMS.BuscarRegraICMS(cargaCteComplementarInfo.CargaCTeComplementado.Carga, null, empresa, remetente, destinatario, tomador, origem, destino, ref incluirBC, ref percentualInclusao, baseDeCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao, tipoContratacaoCarga, null, false, tipoCTe);
                    regraICMS.IncluirICMSBC = incluirBC;
                    regraICMS.PercentualInclusaoBC = percentualInclusao;
                    cargaCteComplementarInfo.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = cargaCteComplementarInfo.ValorComplemento, CodigoLocalidade = destinatario?.Localidade?.Codigo ?? destino.Codigo, SiglaUF = destinatario?.Localidade?.Estado?.Sigla ?? destino.Estado.Sigla, CodigoTipoOperacao = cargaCteComplementarInfo.CargaCTeComplementado.Carga?.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
                }

                cargaCteComplementarInfo.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                cargaCteComplementarInfo.CST = regraICMS.CST;
                cargaCteComplementarInfo.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                cargaCteComplementarInfo.PercentualIncluirBaseCalculo = regraICMS.PercentualInclusaoBC;
                cargaCteComplementarInfo.IncluirICMSFrete = regraICMS.IncluirICMSBC;
                cargaCteComplementarInfo.PercentualAliquota = regraICMS.Aliquota;
                cargaCteComplementarInfo.AliquotaPis = regraICMS.AliquotaPis;
                cargaCteComplementarInfo.AliquotaCofins = regraICMS.AliquotaCofins;
                cargaCteComplementarInfo.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                cargaCteComplementarInfo.ValorICMS = regraICMS.ValorICMS;
                cargaCteComplementarInfo.ValorPis = regraICMS.ValorPis;
                cargaCteComplementarInfo.ValorCofins = regraICMS.ValorCofins;
                cargaCteComplementarInfo.ValorCreditoPresumido = regraICMS.ValorCreditoPresumido;
                cargaCteComplementarInfo.SetarRegraICMS(regraICMS.CodigoRegra);

                if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.BaseCalculoICMS > 0 && empresa.Configuracao != null && !empresa.Configuracao.NaoIncluirICMSFreteDespesaOcorrencia
                    && ((cargaCteComplementarInfo.CargaOcorrencia.Usuario != null && cargaCteComplementarInfo.CargaOcorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro) || cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaTerceiros))
                    cargaCteComplementarInfo.ObservacaoCTe = "Frete Despesa: R$" + String.Format("{0:0.##}", cargaCteComplementarInfo.BaseCalculoICMS, cultura) + " / Tabela de Frete utilizada para o Calculo é a mesma da data da prestação do serviço;";

                servicoCTeComplementar.PreencherCamposImpostoIBSCBS(cargaCteComplementarInfo, impostoIBSCBS);
            }
            else if (modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = null;
                if (tomador == null && cargaCteComplementarInfo.CargaOcorrencia != null)
                {
                    tomador = cargaCteComplementarInfo.CargaOcorrencia.Tomador;
                    if (tomador == null && cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaDestinadaFranquias)
                    {
                        double.TryParse(empresa.CNPJ_SemFormato, out double cnpjEmpresa);
                        tomador = repCliente.BuscarPorCPFCNPJ(cnpjEmpresa);
                    }
                    if (origem == null)
                        origem = tomador?.Localidade;
                }

                if (tomador != null)
                {
                    if (origem != null)
                    {
                        cargaCteComplementarInfo.TomadorPagador = tomador;
                        regraISS = serCargaISS.BuscarRegraISS(empresa, cargaCteComplementarInfo.ValorComplemento, origem, cargaCteComplementarInfo.CargaOcorrencia?.Carga?.TipoOperacao, tomador, cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia ?? null, "", unitOfWork);

                        if (regraISS == null)
                        {
                            mensagem = "A transportadora não possui configuração para emissão de NFS-e na a localidade de " + origem.DescricaoCidadeEstado;
                        }
                        else
                        {
                            cargaCteComplementarInfo.BaseCalculoISS = regraISS.ValorBaseCalculoISS;
                            cargaCteComplementarInfo.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                            cargaCteComplementarInfo.PercentualAliquotaISS = regraISS.AliquotaISS;
                            cargaCteComplementarInfo.ValorISS = regraISS.ValorISS;
                            cargaCteComplementarInfo.ValorRetencaoISS = regraISS.ValorRetencaoISS;
                            cargaCteComplementarInfo.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;

                            cargaCteComplementarInfo.ReterIR = regraISS.ReterIR;
                            cargaCteComplementarInfo.AliquotaIR = regraISS.AliquotaIR;
                            cargaCteComplementarInfo.BaseCalculoIR = regraISS.BaseCalculoIR;
                            cargaCteComplementarInfo.ValorIR = regraISS.ValorIR;
                        }
                    }
                    else
                    {
                        mensagem = "Não existe uma localidade para calculo de ISS";
                    }
                }
                else
                {
                    mensagem = "Não existe um tomador para a ocorrência";
                }
            }
            else if (modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS && cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null)
                tomador = cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado.Tomador;

            if (adicionarPISCOFINS)
            {
                if (string.IsNullOrWhiteSpace(cargaCteComplementarInfo.CST) || cargaCteComplementarInfo.CST == "40" || cargaCteComplementarInfo.CST == "41" || cargaCteComplementarInfo.CST == "51" || adicionarPISCOFINSBaseCalculoICMS)
                    cargaCteComplementarInfo.ValorPISCOFINS = Math.Round(cargaCteComplementarInfo.ValorComplemento / ((100 - cargaCteComplementarInfo.AliquotaPISCOFINS) / 100) * (cargaCteComplementarInfo.AliquotaPISCOFINS / 100m), 2, MidpointRounding.AwayFromZero);
                else
                    cargaCteComplementarInfo.ValorPISCOFINS = Math.Round(cargaCteComplementarInfo.BaseCalculoICMS * (cargaCteComplementarInfo.AliquotaPISCOFINS / 100m), 2, MidpointRounding.AwayFromZero);
            }

            cargaCteComplementarInfo.TipoServico = tipoServico;
            cargaCteComplementarInfo.TipoCTE = tipoCTe;
            cargaCteComplementarInfo.TipoTomador = tipoTomador;
            cargaCteComplementarInfo.TomadorPagador = tomador;
            cargaCteComplementarInfo.TipoPagamento = tipoPagamento;

            if (cargaCteComplementarInfo.TomadorPagador == null)
                mensagem = "Não foi possível identificar o tomador para essa ocorrência.";

            if (!cargaCteComplementarInfo.ComplementoFilialEmissora)
                mensagem = InformarDadosContabeisCargaCTeComplementoInfo(cargaCteComplementarInfo, configuracao, tipoServicoMultisoftware, unitOfWork);

            return mensagem;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS
            {
                Aliquota = cteAnterior.AliquotaICMS,
                CST = cteAnterior.CST,
                PercentualReducaoBC = cteAnterior.PercentualReducaoBaseCalculoICMS,
                ObservacaoCTe = "",
                PercentualInclusaoBC = cteAnterior.PercentualICMSIncluirNoFrete,
                IncluirICMSBC = cteAnterior.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
            };

            if (cteAnterior.RegraICMS != null && cteAnterior.RegraICMS.IncluirPisConfisNaBC && !cteAnterior.RegraICMS.NaoIncluirPisConfisNaBCParaComplementos)
            {
                regraICMS.AliquotaPis = cteAnterior.AliquotaPIS;
                regraICMS.AliquotaCofins = cteAnterior.AliquotaCOFINS;
            }

            if (cteAnterior.ModeloDocumentoFiscal.Numero == "57")
                regraICMS.CFOP = cteAnterior.CFOP.CodigoCFOP;

            return regraICMS;
        }

        //private string RatearCteComplementarPesoEDistancia(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorComplemento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementosInfo, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

        //    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTes = (from obj in cargaCTeComplementosInfo select obj.CargaCTeComplementado).ToList();
        //    List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursos = serFreteRateio.ObterCargaPercurso(carga, unitOfWork);
        //    List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursoSumarizado = new List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
        //    decimal somaMediaPonderada = 0;
        //    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursos)
        //    {
        //        if (cargaPercurso.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento)
        //        {
        //            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTeDestinos = (from obj in CargaCTes where obj.CTe.LocalidadeTerminoPrestacao.Codigo == cargaPercurso.Destino.Codigo select obj).ToList();
        //            if (cargaCTeDestinos.Count > 0)
        //            {
        //                decimal pesoTotal = cargaCTeDestinos.Sum(p => p.CTe.QuantidadesCarga.Sum(obj => obj.Quantidade));
        //                decimal mediaPonderada = pesoTotal * cargaPercurso.DistanciaKM;
        //                somaMediaPonderada += mediaPonderada;
        //                cargaPercursoSumarizado.Add(cargaPercurso);
        //            }
        //        }
        //    }
        //    decimal valorComplementoCTe = 0;
        //    Dominio.Entidades.Embarcador.Cargas.CargaPercurso ultimoCargaPercurso = cargaPercursoSumarizado.Last();

        //    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursoSumarizado)
        //    {
        //        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTeDestinos = (from obj in CargaCTes where obj.CTe.LocalidadeTerminoPrestacao.Codigo == cargaPercurso.Destino.Codigo select obj).ToList();

        //        decimal pesoTotal = cargaCTeDestinos.Sum(p => p.CTe.QuantidadesCarga.Sum(obj => obj.Quantidade));
        //        decimal mediaPonderada = pesoTotal * cargaPercurso.DistanciaKM;

        //        decimal valorComplementoCidade = (valorComplemento / somaMediaPonderada) * mediaPonderada;

        //        Dominio.Entidades.Embarcador.Cargas.CargaCTe ultimoCargaCteDestino = cargaCTeDestinos.Last();
        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeDestino in cargaCTeDestinos)
        //        {
        //            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complementoRateio = (from obj in cargaCTeComplementosInfo where obj.CargaCTeComplementado.Codigo == cargaCTeDestino.Codigo select obj).FirstOrDefault();
        //            decimal valorComponenteRateado = Math.Round((valorComplementoCidade / pesoTotal) * complementoRateio.CargaCTeComplementado.CTe.QuantidadesCarga.Sum(obj => obj.Quantidade), 2, MidpointRounding.AwayFromZero);
        //            valorComplementoCTe += valorComponenteRateado;
        //            if (cargaCTeDestino.Equals(ultimoCargaCteDestino) && cargaPercurso.Equals(ultimoCargaPercurso))
        //            {
        //                decimal diferenca = valorComplemento - valorComplementoCTe;
        //                valorComponenteRateado += diferenca;
        //            }
        //            complementoRateio.ValorComplemento = valorComponenteRateado;
        //            repCargaCTeComplementoInfo.Atualizar(complementoRateio);

        //            if (complementoRateio.ValorComplemento <= 0)
        //            {
        //                return "O valor do complemento rateado entre os CT-es ficará zerado, por favor, selecione menos CT-es para complementá-los com esse valor.";
        //            }
        //        }
        //    }
        //    return "";
        //}

        private string ObterCSTICMSOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            if (string.IsNullOrWhiteSpace(cargaOcorrencia.CSTICMS))
                return string.Empty;

            if (cargaOcorrencia.CSTICMS.Equals("SN"))
                return string.Empty;

            return cargaOcorrencia.CSTICMS;
        }

        #endregion
    }
}

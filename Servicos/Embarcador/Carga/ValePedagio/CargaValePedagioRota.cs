using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.ValePedagio
{
    public class CargaValePedagioRota
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaValePedagioRota(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void RemoverValePedagioCargaSeOperadorOptarPorNaoComprar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.NaoComprarValePedagio)
                return;

            ExcluirValePedagioPorCarga(carga.Codigo);
        }

        public void RemoverValePedagioCargaSeInformadoManualmenteNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

            if (configuracaoTMS.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga && repositorioCargaValePedagio.ExistePorCarga(carga.Codigo))
                ExcluirValePedagioPorCarga(carga.Codigo);
        }

        public void RemoverValePedagioCargaSeTabelaIgnorarValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Tabela && carga.TabelaFrete != null && !(carga.Veiculo?.PossuiTagValePedagio ?? true))
            {
                Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(_unitOfWork);
                if (repComponenteFreteTabelaFrete.VerificarTabelaIgnoraValorPorTagValePedagio(carga.TabelaFrete.Codigo))
                    ExcluirValePedagioPorCarga(carga.Codigo);
            }
        }

        public static bool IsComprarValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Empresa == null)
                return false;

            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador = (carga.Filial != null) ? repositorioValePedagioTransportador.BuscarPorFilialETransportador(carga.Filial.Codigo, carga.Empresa.Codigo) : null;

            return valePedagioTransportador?.ComprarValePedagio ?? carga.Empresa.CompraValePedagio;
        }

        public static void SetarCargaIntegrandoConsultaValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositoriIntegracaoConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            if (repositoriIntegracaoConsultaValorPedagio.ContarConsultaIntegracaoAgIntegracaoPorCarga(carga.Codigo) > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                carga.IntegrandoValePedagio = true;
                repCarga.Atualizar(carga);
            }
        }
        //Async Method
        public static async Task SetarCargaIntegrandoConsultaValePedagioAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositoriIntegracaoConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            if (await repositoriIntegracaoConsultaValorPedagio.ContarConsultaIntegracaoAgIntegracaoPorCargaAsync(carga.Codigo) > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                carga.IntegrandoValePedagio = true;
                await repCarga.AtualizarAsync(carga);
            }
        }

        public static void ValidarCompraValePedagioComIntegracaoValorPedagioEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string StringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValorPedagioIntegracao = repCargaConsultaValorPedagioIntegracao.ConsultaIntegracaoPorCarga(carga.Codigo, SituacaoIntegracao.Integrado);
            List<Dominio.Entidades.CargaPracaPedagio> cargaPracasPedagio = repCargaPracaPedagio.BuscarPorCarga(carga.Codigo);

            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador && cargaValePedagios.Count > 0 && (cargaPracasPedagio.Count > 0 || cargaConsultaValorPedagioIntegracao != null))
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null, false);
                if (cargaComponenteFrete != null)
                {
                    if (carga.Veiculo != null && (carga.Veiculo.FormaDeducaoValePedagio != FormaDeducaoValePedagio.NaoAplicado && carga.Veiculo.FormaDeducaoValePedagio != null))
                    {
                        //tem vale pedagio; Remove os componentes do tipo pedagio da carga e carga pedido e recalcula valores da carga
                        repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, false, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null);
                        repCargaPedidoComponenteFrete.DeletarPorCargaETipoComponente(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

                        foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio integracaoValePedagio in cargaValePedagios)
                        {
                            integracaoValePedagio.ValidaCompraRemoveuComponentes = true;
                            repCargaValePedagio.Atualizar(integracaoValePedagio);
                        }

                        if (carga.Veiculo.FormaDeducaoValePedagio == FormaDeducaoValePedagio.AcrescentarValorFrete)
                        {
                            //somar o valor do frete com o componente de pedagio.

                            //decimal ValorCompraValePedagio = cargaPedidos.Sum(x => x.ValorPedagio);
                            decimal valorComponente = cargaComponenteFrete.ValorComponente;
                            carga.ValorFrete += valorComponente;
                        }
                        else if (carga.Veiculo.FormaDeducaoValePedagio == FormaDeducaoValePedagio.ReduzirValorFrete)
                        {
                            //verificar o valor de compra do vale pedagio e do valor do componente de pedagio, a diferenca tirar do valor do frete.
                            decimal valorContrato = carga.ValorFrete + cargaComponenteFrete.ValorComponente;
                            decimal ValorCompraValePedagio = cargaPedidos.Sum(x => x.ValorPedagio);

                            decimal diferencaValores = valorContrato - ValorCompraValePedagio;
                            carga.ValorFrete = diferencaValores;
                        }

                        carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;

                        Servicos.Embarcador.Carga.RateioFrete serCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                        serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);
                        return;
                    }

                    if (carga.Empresa != null && (carga.Empresa.FormaDeducaoValePedagio != FormaDeducaoValePedagio.NaoAplicado && carga.Empresa.FormaDeducaoValePedagio != null))
                    {
                        //tem vale pedagio; Remove os componentes do tipo pedagio da carga e carga pedido e recalcula valores da carga
                        repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, false, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null);
                        repCargaPedidoComponenteFrete.DeletarPorCargaETipoComponente(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

                        foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio integracaoValePedagio in cargaValePedagios)
                        {
                            integracaoValePedagio.ValidaCompraRemoveuComponentes = true;
                            repCargaValePedagio.Atualizar(integracaoValePedagio);
                        }

                        if (carga.Empresa.FormaDeducaoValePedagio == FormaDeducaoValePedagio.AcrescentarValorFrete)
                        {
                            //somar o valor do frete com o componente de pedagio e tambem o valor da sem parar.

                            //decimal ValorCompraValePedagio = cargaPedidos.Sum(x => x.ValorPedagio);
                            decimal valorComponente = cargaComponenteFrete.ValorComponente;
                            carga.ValorFrete += valorComponente; //ValorCompraValePedagio
                        }
                        else if (carga.Empresa.FormaDeducaoValePedagio == FormaDeducaoValePedagio.ReduzirValorFrete)
                        {
                            //verificar o valor de compra do vale pedagio e do valor do componente de pedagio, a diferenca tirar do valor do frete.
                            decimal valorContrato = carga.ValorFrete + cargaComponenteFrete.ValorComponente;
                            decimal ValorCompraValePedagio = cargaPedidos.Sum(x => x.ValorPedagio);

                            decimal diferencaValores = valorContrato - ValorCompraValePedagio;
                            carga.ValorFrete = diferencaValores;
                        }

                        carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;

                        Servicos.Embarcador.Carga.RateioFrete serCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                        serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);
                    }
                }
            }
        }

        public static void CriarCargaValePedagioPorRotaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool forcarCompra = false, bool consultouValePedagio = false)
        {
            if (carga.Empresa == null)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

            CargaValePedagioRota servicoCargaValePedagioRota = new CargaValePedagioRota(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador = carga.Filial != null ? repositorioValePedagioTransportador.BuscarPorFilialETransportador(carga.Filial.Codigo, carga.Empresa.Codigo) : null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { TipoIntegracao.Pamcard, TipoIntegracao.Repom };

            if (repCargaValePedagio.ExisteValePedagioPorCargaETipoIntegracao(carga.Codigo, tipoIntegracaos))
                return;

            decimal valorPedagioPedido = cargaPedidos?.Count > 0 ? (from o in cargaPedidos select o.ValorPedagio).Sum() : 0;//Solicitado pela Copacol quando tiver valor Pedágio no Pedido (Enviado via integração) nao comprar vale pedágio
            if (valorPedagioPedido > 0 && !forcarCompra)
                return;

            servicoCargaValePedagioRota.ExcluirValePedagioPorCarga(carga.Codigo);

            if (carga.CargaAgrupada && carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                servicoCargaValePedagioRota.ExcluirValePedagioPorCargaAgrupada(carga.Codigo);

            if (carga.NaoComprarValePedagio)
                return;

            if (configuracaoCargaEmissaoDocumento?.NaoComprarValePedagio ?? false)
                return;

            if (configuracao.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga && repositorioCargaValePedagio.ExistePorCarga(carga.Codigo))
                return;

            if ((!(valePedagioTransportador?.ComprarValePedagio ?? carga.Empresa.CompraValePedagio) && !(carga.Veiculo?.TiposIntegracaoValePedagio?.Count > 0)) ||
                (carga.TipoOperacao != null && (carga.TipoOperacao.NaoComprarValePedagio || carga.TipoOperacao.NaoExigeVeiculoParaEmissao)))
                return;

            if ((carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) && !(carga.TipoOperacao?.CompraValePedagioDocsEmitidosFora ?? false))
                return;

            if (carga.CargaTransbordo && (carga.Empresa.Configuracao?.NaoComprarValePedagioCargaTransbordo ?? false))
                return;

            // Primeiro passo é olhar para o tipo de operação da carga, e ela é ultimo ponto até a origem ou retorno vazio, e se ela algum dos trechos possui eixos suspensos
            TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = TipoUltimoPontoRoteirizacao.PontoMaisDistante;

            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoUltimoPontoRoteirizacao.HasValue)
                tipoUltimoPontoRoteirizacao = carga.TipoOperacao.TipoUltimoPontoRoteirizacao.Value;

            if (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.Retornando && (carga.Veiculo?.NaoComprarValePedagioRetorno ?? false))
                return;

            if (repositorioCargaPedido.NaoComprarValePedagioConfiguradoRemetentePedidoCarga(carga.Codigo))
                return;

            if (!carga.Empresa.CompraValePedagio && carga.Empresa.EmissaoDocumentosForaDoSistema)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio = servicoCargaValePedagioRota.ObterTiposIntegracaoValePedagio(carga, valePedagioTransportador);

            //Integração Sem Parar
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioSemParar(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, tipoUltimoPontoRoteirizacao, forcarCompra))
                return;

            //Integração Target
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioTarget(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração Repom
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioRepom(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração PagBem
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioPagBem(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração DBTrans
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioDBTrans(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, tipoUltimoPontoRoteirizacao))
                return;

            //Integração Pamcard
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioPamcard(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração QualP
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioQualP(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração e-Frete
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioEFrete(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração Extratta
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioExtratta(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração DigitalComm
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioDigitalCom(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, tipoUltimoPontoRoteirizacao))
                return;

            //Integração Ambipar
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioAmbipar(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração NDD Cargo
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioNDDCargo(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;
        }
        public static async Task CriarCargaValePedagioPorRotaFreteAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool forcarCompra = false, bool consultouValePedagio = false)
        {
            if (carga.Empresa == null)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

            CargaValePedagioRota servicoCargaValePedagioRota = new CargaValePedagioRota(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador = carga.Filial != null ? await repositorioValePedagioTransportador.BuscarPorFilialETransportadorAsync(carga.Filial.Codigo, carga.Empresa.Codigo) : null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = await repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadraoAsync();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { TipoIntegracao.Pamcard, TipoIntegracao.Repom };

            if (await repCargaValePedagio.ExisteValePedagioPorCargaETipoIntegracaoAsync(carga.Codigo, tipoIntegracaos))
                return;

            decimal valorPedagioPedido = cargaPedidos?.Count > 0 ? (from o in cargaPedidos select o.ValorPedagio).Sum() : 0;//Solicitado pela Copacol quando tiver valor Pedágio no Pedido (Enviado via integração) nao comprar vale pedágio
            if (valorPedagioPedido > 0 && !forcarCompra)
                return;

            servicoCargaValePedagioRota.ExcluirValePedagioPorCarga(carga.Codigo);

            if (carga.CargaAgrupada && carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                servicoCargaValePedagioRota.ExcluirValePedagioPorCargaAgrupada(carga.Codigo);

            if (carga.NaoComprarValePedagio)
                return;

            if (configuracaoCargaEmissaoDocumento?.NaoComprarValePedagio ?? false)
                return;

            if (configuracao.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga && repositorioCargaValePedagio.ExistePorCarga(carga.Codigo))
                return;

            if ((!(valePedagioTransportador?.ComprarValePedagio ?? carga.Empresa.CompraValePedagio) && !(carga.Veiculo?.TiposIntegracaoValePedagio?.Count > 0)) ||
                (carga.TipoOperacao != null && (carga.TipoOperacao.NaoComprarValePedagio || carga.TipoOperacao.NaoExigeVeiculoParaEmissao)))
                return;

            if ((carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) && !(carga.TipoOperacao?.CompraValePedagioDocsEmitidosFora ?? false))
                return;

            if (carga.CargaTransbordo && (carga.Empresa.Configuracao?.NaoComprarValePedagioCargaTransbordo ?? false))
                return;

            // Primeiro passo é olhar para o tipo de operação da carga, e ela é ultimo ponto até a origem ou retorno vazio, e se ela algum dos trechos possui eixos suspensos
            TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = TipoUltimoPontoRoteirizacao.PontoMaisDistante;

            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoUltimoPontoRoteirizacao.HasValue)
                tipoUltimoPontoRoteirizacao = carga.TipoOperacao.TipoUltimoPontoRoteirizacao.Value;

            if (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.Retornando && (carga.Veiculo?.NaoComprarValePedagioRetorno ?? false))
                return;

            if (repositorioCargaPedido.NaoComprarValePedagioConfiguradoRemetentePedidoCarga(carga.Codigo))
                return;

            if (!carga.Empresa.CompraValePedagio && carga.Empresa.EmissaoDocumentosForaDoSistema)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio = servicoCargaValePedagioRota.ObterTiposIntegracaoValePedagio(carga, valePedagioTransportador);

            //Integração Sem Parar
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioSemParar(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, tipoUltimoPontoRoteirizacao, forcarCompra))
                return;

            //Integração Target
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioTarget(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração Repom
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioRepom(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração PagBem
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioPagBem(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração DBTrans
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioDBTrans(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, tipoUltimoPontoRoteirizacao))
                return;

            //Integração Pamcard
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioPamcard(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração QualP
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioQualP(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração e-Frete
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioEFrete(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, cargaPedidos, configuracao, consultouValePedagio))
                return;

            //Integração Extratta
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioExtratta(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração DigitalComm
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioDigitalCom(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware, tipoUltimoPontoRoteirizacao))
                return;

            //Integração Ambipar
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioAmbipar(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;

            //Integração NDD Cargo
            if (servicoCargaValePedagioRota.GerarIntegracaoValePedagioNDDCargo(carga, tiposIntegracaoValePedagio, tipoServicoMultisoftware))
                return;
        }

        public void AtualizarConsultaValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<TipoIntegracao> tiposIntegracao = new List<TipoIntegracao> { TipoIntegracao.Pamcard, TipoIntegracao.Repom };
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> integracoes = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracao);

            if (integracoes.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = repositorioCargaConsultaValoresPedagio.BuscarPorCargaETipoIntegracao(carga.Codigo, integracoes);

            if (cargaConsultaValoresPedagio == null)
                return;

            cargaConsultaValoresPedagio.ProblemaIntegracao = "";
            cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

            int numeroEixos = 0;
            bool eixosSuspensosVeiculo = false;

            if (carga.TipoOperacao != null && carga.Rota != null)
            {
                if (carga.Rota.TipoRota == TipoRotaFrete.Ida && carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == RetornoCargaTipo.Vazio)
                    eixosSuspensosVeiculo = true;
            }

            if (carga.ModeloVeicularCarga != null)
            {
                numeroEixos = carga.ModeloVeicularCarga.NumeroEixos ?? 0;
                if (eixosSuspensosVeiculo)
                    numeroEixos -= carga.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
            }

            cargaConsultaValoresPedagio.QuantidadeEixos = numeroEixos;

            repositorioCargaConsultaValoresPedagio.Atualizar(cargaConsultaValoresPedagio);

            carga.IntegrandoValePedagio = true;
            repositorioCarga.Atualizar(carga);
        }

        #endregion Métodos Públicos

        #region Métodos Privados - Geração Vale Pedágio

        private bool GerarIntegracaoValePedagioSemParar(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, bool forcarCompra)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.SemParar))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);
            Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.SemParar);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(carga, tipoServicoMultisoftware);
            if (integracaoSemParar == null)
                return false;

            RetornoCargaTipo retornoCargaTipo = RetornoCargaTipo.Vazio;
            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoCarregamento.HasValue)
                retornoCargaTipo = carga.TipoOperacao.TipoCarregamento.Value;
            EixosSuspenso eixosSuspensos = EixosSuspenso.Nenhum;
            if (carga.TipoOperacao != null && carga.TipoOperacao.EixosSuspenso.HasValue)
                eixosSuspensos = carga.TipoOperacao.EixosSuspenso.Value;

            List<Dominio.Entidades.CargaPracaPedagio> cargaPracasPedagio = repCargaPracaPedagio.BuscarPorCarga(carga.Codigo);

            bool gerarPedagioValorZerado = cargaPracasPedagio.Count == 0 && integracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio;

            if (cargaPracasPedagio.Count > 0 || !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio) || integracaoSemParar.BuscarPracasNaGeracaoDaCarga ||
                (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false) || gerarPedagioValorZerado) //Sem Parar só compra se tiver praças de vale pedágio ou rota com código de integração
            {

                //caso o veiculo nao possui tag e deve consultar valor pedagio, criar entidade e consultar posteriormente na thread IntegracaoCarga -> ConsultarValoresPedagioPendente
                if (carga.Veiculo != null && carga.Veiculo.PossuiTagValePedagio == false && integracaoSemParar.NaoComprarValePedagioVeiculoSemTag && !forcarCompra)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                    repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);

                    if (integracaoSemParar.ConsultarValorPedagioParaRota || (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false))
                    {
                        //vamos sempre remover o registro de integracao para carga e zerar o valor vale pedagio dos pedidos;
                        Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
                        repCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

                        //criar classe para consulta de valores pedagio posteriormente na thread IntegracaoCarga -> ConsultarValoresPedagioPendente;
                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                        cargaConsultaValoresPedagio.Carga = carga;
                        cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                        cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                        cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                        cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        cargaConsultaValoresPedagio.TipoRota = integracaoSemParar.TipoRota;
                        cargaConsultaValoresPedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

                        bool eixosSuspensosVeiculo = false;
                        if (carga.TipoOperacao != null && carga.Rota != null)
                        {
                            if (carga.Rota.TipoRota == TipoRotaFrete.Ida && carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == RetornoCargaTipo.Vazio)
                                eixosSuspensosVeiculo = true;
                        }

                        int numeroEixos = 0;
                        if (!integracaoSemParar.UtilizarModeoVeicularCarga)
                        {
                            if (carga.Veiculo?.ModeloVeicularCarga != null)
                            {
                                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                                if (eixosSuspensosVeiculo)
                                    numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                            }

                            if (carga.VeiculosVinculados != null)
                            {
                                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                                {
                                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo?.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                                    {
                                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                                        if (eixosSuspensosVeiculo)
                                            numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (carga.ModeloVeicularCarga != null)
                            {
                                numeroEixos = carga.ModeloVeicularCarga.NumeroEixos ?? 0;
                                if (eixosSuspensosVeiculo)
                                    numeroEixos -= carga.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                            }
                        }

                        cargaConsultaValoresPedagio.QuantidadeEixos = numeroEixos;

                        if (cargaConsultaValoresPedagio.TipoRota == TipoRotaSemParar.RotaFixa)
                            cargaConsultaValoresPedagio.CodigoIntegracaoValePedagio = carga.Rota?.CodigoIntegracaoValePedagio;

                        if (gerarPedagioValorZerado)
                        {
                            cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            cargaConsultaValoresPedagio.ProblemaIntegracao = "Rota não possui vale pedágio. Informação do Embarcador";
                            cargaConsultaValoresPedagio.NumeroTentativas++;
                        }

                        repCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                        if (!gerarPedagioValorZerado)
                        {
                            carga.IntegrandoValePedagio = true;
                            repCarga.Atualizar(carga);
                        }

                        return true;
                    }

                    return false;
                }

                bool compra1ComEixosSuspensos = (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante ? false : (eixosSuspensos == EixosSuspenso.Ida ? true : false));

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, compra1ComEixosSuspensos);

                List<string> cargaPracasPedagioIda = (from obj in cargaPracasPedagio where obj.EixosSuspenso == EixosSuspenso.Ida || obj.EixosSuspenso == EixosSuspenso.Nenhum select obj.PracaPedagio.CodigoIntegracao).ToList();
                List<string> cargaPracasPedagioVolta = (from obj in cargaPracasPedagio where obj.EixosSuspenso == EixosSuspenso.Volta select obj.PracaPedagio.CodigoIntegracao).ToList();

                List<string> todasPracasPedagio = cargaPracasPedagioIda;
                if (eixosSuspensos == EixosSuspenso.Nenhum)
                    todasPracasPedagio.AddRange(cargaPracasPedagioVolta);

                bool gerarPedagioValorZeradoForaDoMesVigente = integracaoSemParar.ComprarSomenteNoMesVigente && !carga.DataCriacaoCarga.IsDateSameMonth(DateTime.Now) &&
                    !(carga.DataCriacaoCarga.IsLastDayOfMonth() && carga.DataCriacaoCarga.Hour >= 20 && DateTime.Now.IsFirstDayOfMonth());

                bool gerouValePedagio = false;
                if (cargaValePedagio == null && (integracaoSemParar.TipoRota == TipoRotaSemParar.RotaFixa ||
                    (integracaoSemParar.TipoRota == TipoRotaSemParar.RotaTemporaria && (todasPracasPedagio.Count > 0 || integracaoSemParar.BuscarPracasNaGeracaoDaCarga || gerarPedagioValorZerado))))
                {
                    cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                    cargaValePedagio.Carga = carga;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                    cargaValePedagio.TipoIntegracao = integracao;
                    cargaValePedagio.ProblemaIntegracao = "";
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    cargaValePedagio.TipoRota = integracaoSemParar.TipoRota;
                    // Se ~tipo ultimo ponto é o ponto mais distante.. Eixos Suspensos false.. Se estiver indo coletar... e retorna carregado.. Eixos suspensos de IDA = true
                    cargaValePedagio.CompraComEixosSuspensos = compra1ComEixosSuspensos;
                    cargaValePedagio.TipoPercursoVP = ObterTipoPercursoVP(tipoUltimoPontoRoteirizacao, compra1ComEixosSuspensos);

                    if (cargaValePedagio.TipoRota == TipoRotaSemParar.RotaFixa)
                        cargaValePedagio.CodigoIntegracaoValePedagio = carga.Rota?.CodigoIntegracaoValePedagio;

                    if (gerarPedagioValorZerado)
                    {
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                        cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaValePedagio.ProblemaIntegracao = "Rota não possui vale pedágio. Informação do Embarcador";
                        cargaValePedagio.NumeroTentativas++;
                    }
                    else if (gerarPedagioValorZeradoForaDoMesVigente)
                    {
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                        cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaValePedagio.ProblemaIntegracao = "Configurado para não comprar em mês diferente da criação da carga. Informação do Embarcador";
                        cargaValePedagio.NumeroTentativas++;
                    }

                    repCargaValePedagio.Inserir(cargaValePedagio);
                }

                if (cargaValePedagio != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagio.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRotaExiste in rotasValePedagio)
                        repCargaValePedagioRota.Deletar(cargaValePedagioRotaExiste);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRota = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota();
                    cargaValePedagioRota.CargaValePedagio = cargaValePedagio;
                    cargaValePedagioRota.DescricaoRota = Utilidades.String.Left(carga.Rota?.Descricao ?? carga.CodigoCargaEmbarcador, 64);

                    if (todasPracasPedagio.Count > 0)
                        cargaValePedagioRota.CodigosPracaSemParar = string.Join(", ", todasPracasPedagio);

                    repCargaValePedagioRota.Inserir(cargaValePedagioRota);

                    if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
                    {
                        carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }

                    cargaValePedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

                    if (cargaValePedagio.RotaFrete != null)
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                    gerouValePedagio = true;
                }

                //if (integracaoSemParar != null && integracaoSemParar.TipoRota == TipoRotaSemParar.RotaFixa && integracaoSemParar.ComprarRetornoVazioSeparado && carga.Rota != null && carga.Rota.TipoRota == TipoRotaFrete.IdaVolta && !string.IsNullOrWhiteSpace(carga.Rota.CodigoIntegracaoValePedagioRetorno))

                //
                // Também não é necessário separar em ida e volta a compra do vale pedágio (somente para RotaTemporaria) se no tipo de operação estiver marcado nenhum para a opção eixo suspenso
                //if (integracaoSemParar != null && cargaPracasPedagioVolta.Count > 0 && ((integracaoSemParar.TipoRota == TipoRotaSemParar.RotaFixa && integracaoSemParar.ComprarRetornoVazioSeparado) || eixosSuspensos != EixosSuspenso.Nenhum))

                //
                // Não estava comprando vale pedágio retorno quando tinha código de de integração fixo para retorno
                if ((integracaoSemParar.TipoRota == TipoRotaSemParar.RotaFixa && integracaoSemParar.ComprarRetornoVazioSeparado && !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
                                                   || (cargaPracasPedagioVolta.Count > 0 && eixosSuspensos != EixosSuspenso.Nenhum))
                {
                    bool compraComEixosSuspensos = (eixosSuspensos == EixosSuspenso.Volta ? true : false);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagioRetorno = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, compraComEixosSuspensos);

                    if (cargaValePedagioRetorno == null)
                    {
                        cargaValePedagioRetorno = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                        cargaValePedagioRetorno.Carga = carga;
                        cargaValePedagioRetorno.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                        cargaValePedagioRetorno.TipoIntegracao = integracao;
                        cargaValePedagioRetorno.ProblemaIntegracao = "";
                        cargaValePedagioRetorno.DataIntegracao = DateTime.Now;
                        cargaValePedagioRetorno.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        cargaValePedagioRetorno.TipoRota = integracaoSemParar.TipoRota;
                        // Se estiver voltando.. e vazio... e retorna carregado.. Eixos suspensos de IDA = true
                        cargaValePedagioRetorno.CompraComEixosSuspensos = compraComEixosSuspensos;
                        cargaValePedagioRetorno.TipoPercursoVP = TipoRotaFrete.Volta;

                        if (cargaValePedagioRetorno.TipoRota == TipoRotaSemParar.RotaFixa)
                            cargaValePedagioRetorno.CodigoIntegracaoValePedagio = carga.Rota?.CodigoIntegracaoValePedagioRetorno;

                        if (gerarPedagioValorZeradoForaDoMesVigente)
                        {
                            cargaValePedagioRetorno.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                            cargaValePedagioRetorno.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            cargaValePedagioRetorno.ProblemaIntegracao = "Configurado para não comprar em mês diferente da criação da carga. Informação do Embarcador";
                            cargaValePedagioRetorno.NumeroTentativas++;
                        }

                        repCargaValePedagio.Inserir(cargaValePedagioRetorno);
                    }

                    if (cargaValePedagioRetorno != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagioRetorno.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRotaExiste in rotasValePedagio)
                            repCargaValePedagioRota.Deletar(cargaValePedagioRotaExiste);

                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRota = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota();
                        cargaValePedagioRota.CargaValePedagio = cargaValePedagioRetorno;
                        cargaValePedagioRota.DescricaoRota = Utilidades.String.Left(carga.Rota?.Descricao ?? carga.CodigoCargaEmbarcador, 64);
                        if (cargaPracasPedagioVolta != null && cargaPracasPedagioVolta.Count > 0)
                            cargaValePedagioRota.CodigosPracaSemParar = string.Join(", ", cargaPracasPedagioVolta);

                        repCargaValePedagioRota.Inserir(cargaValePedagioRota);

                        if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
                        {
                            carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                            repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                        }

                        cargaValePedagioRetorno.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
                        if (cargaValePedagioRetorno.RotaFrete != null)
                            repCargaValePedagio.Atualizar(cargaValePedagio);
                    }

                    gerouValePedagio = true;
                }

                return gerouValePedagio;
            }

            return false;
        }

        private bool GerarIntegracaoValePedagioTarget(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool consultouValePedagio = false)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.Target))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Target);
            if (integracao == null)
                return false;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(carga, tipoServicoMultisoftware);
            if (integracaoTarget == null)
                return false;


            if (!consultouValePedagio && (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false)
                && servicoValePedagio.UtilizarConsultaValePedagioIntegracaoTarget(carga, tipoServicoMultisoftware))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

                repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);
                repositorioCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                cargaConsultaValoresPedagio.Carga = carga;
                cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaConsultaValoresPedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

                repositorioCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                carga.IntegrandoValePedagio = true;
                repositorioCarga.Atualizar(carga);

                return true;
            }

            cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
            cargaValePedagio.Carga = carga;
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
            cargaValePedagio.TipoIntegracao = integracao;
            cargaValePedagio.ProblemaIntegracao = "";
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            cargaValePedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
            repCargaValePedagio.Inserir(cargaValePedagio);

            if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
            {
                carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            return true;
        }

        private bool GerarIntegracaoValePedagioRepom(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool consultouValePedagio = false)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.Repom))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Repom);
            if (integracao == null)
                return false;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom integracaoRepom = servicoValePedagio.ObterIntegracaoRepom(carga, tipoServicoMultisoftware);
            if (integracaoRepom == null)
                return false;

            if (!consultouValePedagio && (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false)
                && servicoValePedagio.UtilizarConsultaValePedagioIntegracaoRepom(carga, tipoServicoMultisoftware)
                && integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

                repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);
                repositorioCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                cargaConsultaValoresPedagio.Carga = carga;
                cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaConsultaValoresPedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

                repositorioCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                carga.IntegrandoValePedagio = true;
                repositorioCarga.Atualizar(carga);

                return true;
            }

            cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
            cargaValePedagio.Carga = carga;
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
            cargaValePedagio.TipoIntegracao = integracao;
            cargaValePedagio.ProblemaIntegracao = "";
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repCargaValePedagio.Inserir(cargaValePedagio);

            if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
            {
                carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            if (integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
                cargaValePedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

            if (carga.DadosSumarizados != null && (cargaValePedagio.RotaFrete?.UtilizarDistanciaRotaCarga ?? false))
            {
                carga.Distancia = cargaValePedagio.RotaFrete.Quilometros;
                carga.DadosSumarizados.Distancia = cargaValePedagio.RotaFrete.Quilometros;

                repositorioCarga.Atualizar(carga);
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            if (cargaValePedagio.RotaFrete != null)
                repCargaValePedagio.Atualizar(cargaValePedagio);

            return true;
        }

        private bool GerarIntegracaoValePedagioPagBem(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.PagBem))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.PagBem);
            if (integracao == null)
                return false;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem = servicoValePedagio.ObterIntegracaoPagbem(carga.TipoOperacao, carga.Filial, carga.GrupoPessoaPrincipal, carga.FreteDeTerceiro, tipoServicoMultisoftware);
            if (integracaoPagbem == null)
                return false;

            cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
            cargaValePedagio.Carga = carga;
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
            cargaValePedagio.TipoIntegracao = integracao;
            cargaValePedagio.ProblemaIntegracao = "";
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repCargaValePedagio.Inserir(cargaValePedagio);

            if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
            {
                carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            return true;
        }

        private bool GerarIntegracaoValePedagioDBTrans(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.DBTrans))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.DBTrans);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(carga, tipoServicoMultisoftware);
            if (integracaoDBTrans == null)
                return false;

            bool geraValePedagio = true;
            if (integracaoDBTrans.TipoTomador.HasValue && integracaoDBTrans.TipoTomador.Value != Dominio.Enumeradores.TipoTomador.NaoInformado && cargaPedidos?.Count > 0 && cargaPedidos.Any(o => o.TipoTomador != integracaoDBTrans.TipoTomador.Value))
                geraValePedagio = false;

            bool possuiCodigoIntegracaoValePedagio = !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio) || !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno);
            if (!geraValePedagio || (!possuiCodigoIntegracaoValePedagio && integracaoDBTrans.TipoRota != TipoRotaDBTrans.RotaTemporaria))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);

                if (integracaoDBTrans.ConsultarValorPedagioParaRota || (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false))
                {
                    //vamos sempre remover o registro de integracao para carga e zerar o valor vale pedagio dos pedidos;
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
                    repCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

                    if (integracaoDBTrans.TipoRota == TipoRotaDBTrans.RotaFixa && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
                        return false;

                    //criar classe para consulta de valores pedagio posteriormente na thread IntegracaoCarga -> ConsultarValoresPedagioPendente;
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                    cargaConsultaValoresPedagio.Carga = carga;
                    cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                    cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                    cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                    cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    if (integracaoDBTrans.TipoRota == TipoRotaDBTrans.RotaFixa)
                        cargaConsultaValoresPedagio.CodigoIntegracaoValePedagio = carga.Rota?.CodigoIntegracaoValePedagio;

                    repCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                    carga.IntegrandoValePedagio = true;
                    repCarga.Atualizar(carga);

                    return true;
                }

                return false;
            }

            bool gerouValePedagio = false;
            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);

                if (cargaIntegracaoValePedagio == null)
                {
                    cargaIntegracaoValePedagio = GerarCargaIntegracaoValePedagio(carga, integracao, carga.Rota.CodigoIntegracaoValePedagio, false);
                    cargaIntegracaoValePedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
                    cargaIntegracaoValePedagio.TipoPercursoVP = ObterTipoPercursoVP(tipoUltimoPontoRoteirizacao, !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno));

                    if (carga.DadosSumarizados != null && (cargaIntegracaoValePedagio.RotaFrete?.UtilizarDistanciaRotaCarga ?? false))
                    {
                        carga.Distancia = cargaIntegracaoValePedagio.RotaFrete.Quilometros;
                        carga.DadosSumarizados.Distancia = cargaIntegracaoValePedagio.RotaFrete.Quilometros;

                        repositorioCarga.Atualizar(carga);
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }

                    if (cargaIntegracaoValePedagio.RotaFrete != null)
                        repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);
                }

                gerouValePedagio = true;
            }

            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagioRetorno = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, true);

                if (cargaIntegracaoValePedagioRetorno == null)
                {
                    cargaIntegracaoValePedagioRetorno = GerarCargaIntegracaoValePedagio(carga, integracao, carga.Rota.CodigoIntegracaoValePedagioRetorno, true);
                    cargaIntegracaoValePedagioRetorno.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
                    cargaIntegracaoValePedagioRetorno.TipoPercursoVP = TipoRotaFrete.Volta;

                    if (carga.DadosSumarizados != null && (cargaIntegracaoValePedagioRetorno.RotaFrete?.UtilizarDistanciaRotaCarga ?? false))
                    {
                        carga.Distancia = cargaIntegracaoValePedagioRetorno.RotaFrete.Quilometros;
                        carga.DadosSumarizados.Distancia = cargaIntegracaoValePedagioRetorno.RotaFrete.Quilometros;

                        repositorioCarga.Atualizar(carga);
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }

                    if (cargaIntegracaoValePedagioRetorno.RotaFrete != null)
                        repCargaValePedagio.Atualizar(cargaIntegracaoValePedagioRetorno);
                }

                gerouValePedagio = true;
            }

            if (integracaoDBTrans.TipoRota == TipoRotaDBTrans.RotaTemporaria && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio) && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
            {
                Dominio.Entidades.RotaFrete rotaFreteExclusiva = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
                Dominio.Entidades.RotaFrete rotaFrete = rotaFreteExclusiva ?? carga.Rota;

                bool compraIdaComEixoSuspensos = rotaFrete?.TipoRota == TipoRotaFrete.Ida && carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;
                bool compraVoltaComEixoSuspenso = VerificarCompraValePedagioVoltaEixoSuspenso(integracaoDBTrans, carga.TipoOperacao, rotaFrete, tipoUltimoPontoRoteirizacao);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, compraIdaComEixoSuspensos);

                if (cargaIntegracaoValePedagio == null)
                {
                    cargaIntegracaoValePedagio = GerarCargaIntegracaoValePedagio(carga, integracao, "", compraIdaComEixoSuspensos);
                    cargaIntegracaoValePedagio.RotaFrete = rotaFreteExclusiva;
                    cargaIntegracaoValePedagio.TipoPercursoVP = ObterTipoPercursoVP(tipoUltimoPontoRoteirizacao, compraVoltaComEixoSuspenso, CompraIdaVoltaMesmoSendoPontoMaisDistante(integracaoDBTrans, rotaFrete, tipoUltimoPontoRoteirizacao));

                    if (carga.DadosSumarizados != null && (cargaIntegracaoValePedagio.RotaFrete?.UtilizarDistanciaRotaCarga ?? false))
                    {
                        carga.Distancia = cargaIntegracaoValePedagio.RotaFrete.Quilometros;
                        carga.DadosSumarizados.Distancia = cargaIntegracaoValePedagio.RotaFrete.Quilometros;

                        repositorioCarga.Atualizar(carga);
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }

                    if (cargaIntegracaoValePedagio.RotaFrete != null)
                        repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);
                }

                if (compraVoltaComEixoSuspenso)
                {
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagioRetorno = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, compraVoltaComEixoSuspenso);
                    if (cargaIntegracaoValePedagioRetorno == null)
                    {
                        cargaIntegracaoValePedagioRetorno = GerarCargaIntegracaoValePedagio(carga, integracao, "", compraVoltaComEixoSuspenso);
                        cargaIntegracaoValePedagioRetorno.TipoPercursoVP = TipoRotaFrete.Volta;

                        cargaIntegracaoValePedagioRetorno.RotaFrete = rotaFreteExclusiva;

                        if (carga.DadosSumarizados != null && (cargaIntegracaoValePedagioRetorno.RotaFrete?.UtilizarDistanciaRotaCarga ?? false))
                        {
                            carga.Distancia = cargaIntegracaoValePedagioRetorno.RotaFrete.Quilometros;
                            carga.DadosSumarizados.Distancia = cargaIntegracaoValePedagioRetorno.RotaFrete.Quilometros;

                            repositorioCarga.Atualizar(carga);
                            repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                        }

                        if (cargaIntegracaoValePedagioRetorno.RotaFrete != null)
                            repCargaValePedagio.Atualizar(cargaIntegracaoValePedagioRetorno);
                    }
                }

                gerouValePedagio = true;
            }

            return gerouValePedagio;
        }

        private bool GerarIntegracaoValePedagioPamcard(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool consultouValePedagio = false)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.Pamcard))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Pamcard);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard = servicoValePedagio.ObterIntegracaoPamcard(carga, tipoServicoMultisoftware);
            if (integracaoPamcard == null)
                return false;

            bool geraRotaDinamica = integracaoPamcard.TipoRota == TipoRotaPamcard.RotaDinamica && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio) && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno);
            if (geraRotaDinamica && !consultouValePedagio && (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false) && (integracaoPamcard.AcoesPamcard == TipoAcaoPamcard.SomenteConsulta || integracaoPamcard.AcoesPamcard == TipoAcaoPamcard.ConsultaCompra))
                GerarRegistroIntegracaoConsulta(carga, tipoServicoMultisoftware, cargaPedidos, configuracao, integracao, integracaoPamcard);

            bool gerouValePedagio = false;
            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);

                if (cargaValePedagio == null)
                {
                    cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                    cargaValePedagio.Carga = carga;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                    cargaValePedagio.TipoIntegracao = integracao;
                    cargaValePedagio.ProblemaIntegracao = "";
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    cargaValePedagio.CompraComEixosSuspensos = false;
                    cargaValePedagio.CodigoIntegracaoValePedagio = carga.Rota.CodigoIntegracaoValePedagio;
                    repCargaValePedagio.Inserir(cargaValePedagio);
                    if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
                    {
                        carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }
                }

                gerouValePedagio = true;
            }

            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagioRetorno = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, true);

                if (cargaValePedagioRetorno == null)
                {
                    cargaValePedagioRetorno = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                    cargaValePedagioRetorno.Carga = carga;
                    cargaValePedagioRetorno.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                    cargaValePedagioRetorno.TipoIntegracao = integracao;
                    cargaValePedagioRetorno.ProblemaIntegracao = "";
                    cargaValePedagioRetorno.DataIntegracao = DateTime.Now;
                    cargaValePedagioRetorno.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    cargaValePedagioRetorno.CompraComEixosSuspensos = true;//Retorno
                    cargaValePedagioRetorno.CodigoIntegracaoValePedagio = carga.Rota.CodigoIntegracaoValePedagioRetorno;
                    repCargaValePedagio.Inserir(cargaValePedagioRetorno);
                    if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
                    {
                        carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }
                }

                gerouValePedagio = true;
            }

            if (geraRotaDinamica)
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);

                if (cargaValePedagio == null)
                {
                    cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                    cargaValePedagio.Carga = carga;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                    cargaValePedagio.TipoIntegracao = integracao;
                    cargaValePedagio.ProblemaIntegracao = "";
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    repCargaValePedagio.Inserir(cargaValePedagio);
                    if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
                    {
                        carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                        repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                    }
                }

                gerouValePedagio = true;
            }

            return gerouValePedagio;
        }

        private void GerarRegistroIntegracaoConsulta(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            repCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
            cargaConsultaValoresPedagio.Carga = carga;
            cargaConsultaValoresPedagio.TipoIntegracao = integracao;
            cargaConsultaValoresPedagio.ProblemaIntegracao = "";
            cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            cargaConsultaValoresPedagio.TipoRota = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar)integracaoPamcard.TipoRota;
            cargaConsultaValoresPedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

            int numeroEixos = 0;
            bool eixosSuspensosVeiculo = false;

            if (carga.TipoOperacao != null && carga.Rota != null)
            {
                if (carga.Rota.TipoRota == TipoRotaFrete.Ida && carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == RetornoCargaTipo.Vazio)
                    eixosSuspensosVeiculo = true;
            }

            if (carga.ModeloVeicularCarga != null)
            {
                numeroEixos = carga.ModeloVeicularCarga.NumeroEixos ?? 0;
                if (eixosSuspensosVeiculo)
                    numeroEixos -= carga.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
            }

            cargaConsultaValoresPedagio.QuantidadeEixos = numeroEixos;

            if (cargaConsultaValoresPedagio.TipoRota == TipoRotaSemParar.RotaFixa)
                cargaConsultaValoresPedagio.CodigoIntegracaoValePedagio = carga.Rota?.CodigoIntegracaoValePedagio;

            repCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);
            carga.IntegrandoValePedagio = true;
            repCarga.Atualizar(carga);
        }

        private bool GerarIntegracaoValePedagioQualP(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.QualP))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.QualP);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP integracaoQualP = servicoValePedagio.ObterIntegracaoQualP(carga, tipoServicoMultisoftware);
            if (integracaoQualP == null)
                return false;

            if (string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
                return false;

            //vamos sempre remover o registro de integracao para carga e zerar o valor vale pedagio dos pedidos;
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            repCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);
            repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);

            if (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false)//VAMOS USAR A MESMA FLAG.
            {
                //criar classe para consulta de valores pedagio posteriormente na thread IntegracaoCarga -> ConsultarValoresPedagioPendente;
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                cargaConsultaValoresPedagio.Carga = carga;
                cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaConsultaValoresPedagio.TipoRota = TipoRotaSemParar.RotaFixa;

                bool eixosSuspensosVeiculo = false;
                if (carga.TipoOperacao != null && carga.Rota != null)
                {
                    if (carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida && carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                        eixosSuspensosVeiculo = true;
                }

                int numeroEixos = 0;
                if (carga.ModeloVeicularCarga != null)
                {
                    numeroEixos = carga.ModeloVeicularCarga.NumeroEixos ?? 0;
                    if (eixosSuspensosVeiculo)
                        numeroEixos -= carga.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                }
                cargaConsultaValoresPedagio.QuantidadeEixos = numeroEixos;
                repCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                return true;
            }
            else
            {
                carga.IntegrandoValePedagio = false;
                repCarga.Atualizar(carga);

                return false;
            }
        }

        private bool GerarIntegracaoValePedagioEFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool consultouValePedagio = false)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.EFrete))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.EFrete);
            if (integracao == null)
                return false;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(carga, tipoServicoMultisoftware);
            if (integracaoEFrete == null)
                return false;

            if (!consultouValePedagio && (carga.TipoOperacao?.PermitirConsultaDeValoresPedagioSemParar ?? false))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValoresPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

                repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);
                repositorioCargaConsultaValoresPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
                cargaConsultaValoresPedagio.Carga = carga;
                cargaConsultaValoresPedagio.TipoIntegracao = integracao;
                cargaConsultaValoresPedagio.ProblemaIntegracao = "";
                cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValoresPedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaConsultaValoresPedagio.RotaFrete = SetarRotaFreteExclusivaCompraValePedagio(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

                repositorioCargaConsultaValoresPedagio.Inserir(cargaConsultaValoresPedagio);

                carga.IntegrandoValePedagio = true;
                repositorioCarga.Atualizar(carga);

                return true;
            }

            cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
            cargaValePedagio.Carga = carga;
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
            cargaValePedagio.TipoIntegracao = integracao;
            cargaValePedagio.ProblemaIntegracao = "";
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repCargaValePedagio.Inserir(cargaValePedagio);

            if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
            {
                carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            return true;
        }

        private bool GerarIntegracaoValePedagioExtratta(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.Extratta))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Extratta);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio integracaoExtratta = servicoValePedagio.ObterIntegracaoExtratta(carga, tipoServicoMultisoftware);
            if (integracaoExtratta == null)
                return false;

            bool gerouValePedagio = false;
            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
                if (cargaValePedagio == null)
                    GerarCargaIntegracaoValePedagio(carga, integracao, carga.Rota.CodigoIntegracaoValePedagio);

                gerouValePedagio = true;
            }

            if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagioRetorno = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, true);
                if (cargaValePedagioRetorno == null)
                    GerarCargaIntegracaoValePedagio(carga, integracao, carga.Rota.CodigoIntegracaoValePedagioRetorno, true);//De retorno

                gerouValePedagio = true;
            }

            if (integracaoExtratta.TipoRota == TipoRotaExtratta.RotaDinamica && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio) && string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno))
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
                if (cargaValePedagio == null)
                    GerarCargaIntegracaoValePedagio(carga, integracao);

                gerouValePedagio = true;
            }

            return gerouValePedagio;
        }

        private bool GerarIntegracaoValePedagioDigitalCom(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.DigitalCom))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.DigitalCom);
            if (integracao == null)
                return false;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom integracaoDigitalCom = servicoValePedagio.ObterIntegracaoDigitalCom(carga, tipoServicoMultisoftware)?.IntegracaoDigitalCom;
            if (integracaoDigitalCom == null)
                return false;

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = GerarCargaIntegracaoValePedagio(carga, integracao);

            cargaIntegracaoValePedagio.TipoPercursoVP = ObterTipoPercursoVP(tipoUltimoPontoRoteirizacao, compraVoltaComEixoSuspenso: false);
            repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);

            return true;
        }

        private bool GerarIntegracaoValePedagioAmbipar(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.Ambipar))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Ambipar);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio integracaoAmbipar = servicoValePedagio.ObterIntegracaoAmbipar(carga, tipoServicoMultisoftware);
            if (integracaoAmbipar == null)
                return false;

            GerarCargaIntegracaoValePedagio(carga, integracao);

            return true;
        }

        private bool GerarIntegracaoValePedagioNDDCargo(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tiposIntegracaoValePedagio.Count > 0 && !tiposIntegracaoValePedagio.Any(o => o.Tipo == TipoIntegracao.NDDCargo))
                return false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.NDDCargo);
            if (integracao == null)
                return false;

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo, false);
            if (cargaValePedagio != null)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo integracaoNDDCargo = servicoValePedagio.ObterIntegracaoNDDCargo(carga, tipoServicoMultisoftware);
            if (integracaoNDDCargo == null)
                return false;

            GerarCargaIntegracaoValePedagio(carga, integracao);

            return true;
        }

        #endregion Métodos Privados - Geração Vale Pedágio

        #region Métodos Privados

        private bool CompraIdaVoltaMesmoSendoPontoMaisDistante(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.RotaFrete rotaFrete, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            if (tipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                return false;

            if (CompraVoltaMesmoSendoPontoMaisDistante(integracaoDBTrans, rotaFrete, tipoUltimoPontoRoteirizacao))
                return false;

            if (integracaoDBTrans.TipoRotaFrete == TipoRotaFreteDBTrans.IdaVolta && rotaFrete?.TipoRota != TipoRotaFrete.IdaVolta)
                return true;

            if (integracaoDBTrans.TipoRotaFrete == TipoRotaFreteDBTrans.NaoEspecificado && rotaFrete?.TipoRota == TipoRotaFrete.IdaVolta)
                return true;

            return false;
        }

        private bool CompraVoltaMesmoSendoPontoMaisDistante(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.RotaFrete rotaFrete, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            return tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante && integracaoDBTrans.TipoRotaFrete == TipoRotaFreteDBTrans.IdaVolta && rotaFrete?.TipoRota == TipoRotaFrete.IdaVolta;
        }

        private void ExcluirValePedagioPorCarga(int carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga);
            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio in cargaValePedagios)
            {
                //só deleta se não teve tentativa de compra
                if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && cargaValePedagio.SituacaoValePedagio == SituacaoValePedagio.Pendete)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagio.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRotaExiste in rotasValePedagio)
                        repCargaValePedagioRota.Deletar(cargaValePedagioRotaExiste);

                    repCargaValePedagio.Deletar(cargaValePedagio);
                }
            }
        }
        private async Task ExcluirValePedagioPorCargaAsync(int carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = await repCargaValePedagio.BuscarPorCargaAsync(carga);
            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio in cargaValePedagios)
            {
                //só deleta se não teve tentativa de compra
                if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && cargaValePedagio.SituacaoValePedagio == SituacaoValePedagio.Pendete)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = await repCargaValePedagioRota.BuscarPorCargaValePedagioAsync(cargaValePedagio.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRotaExiste in rotasValePedagio)
                        await repCargaValePedagioRota.DeletarAsync(cargaValePedagioRotaExiste);

                    await repCargaValePedagio.DeletarAsync(cargaValePedagio);
                }
            }
        }

        private void ExcluirValePedagioPorCargaAgrupada(int cargaAgrupada)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(cargaAgrupada);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga CargaOriginal in cargasOriginais)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(CargaOriginal.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio in cargaValePedagios)
                {
                    //só deleta se não teve tentativa de compra
                    if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && cargaValePedagio.SituacaoValePedagio == SituacaoValePedagio.Pendete)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagio.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRotaExiste in rotasValePedagio)
                            repCargaValePedagioRota.Deletar(cargaValePedagioRotaExiste);

                        repCargaValePedagio.Deletar(cargaValePedagio);
                    }

                }
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio GerarCargaIntegracaoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao, string codigoIntegracaoValePedagio = "", bool compraComEixosSuspensos = false)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio()
            {
                Carga = carga,
                SituacaoValePedagio = SituacaoValePedagio.Pendete,
                TipoIntegracao = integracao,
                ProblemaIntegracao = "",
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                CodigoIntegracaoValePedagio = codigoIntegracaoValePedagio,
                CompraComEixosSuspensos = compraComEixosSuspensos
            };

            repositorioCargaIntegracaoValePedagio.Inserir(cargaIntegracaoValePedagio);

            if (carga.DadosSumarizados != null && !carga.DadosSumarizados.PossuiIntegracaoValePedagio)
            {
                carga.DadosSumarizados.PossuiIntegracaoValePedagio = true;
                repositorioCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            return cargaIntegracaoValePedagio;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> ObterTiposIntegracaoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador)
        {
            if (carga.Veiculo?.TiposIntegracaoValePedagio?.Count > 0)
                return carga.Veiculo.TiposIntegracaoValePedagio.ToList();

            if (valePedagioTransportador != null)
                return new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>() { valePedagioTransportador.TipoIntegracaoValePedagio };

            return carga.Empresa.TiposIntegracaoValePedagio.ToList();
        }

        private TipoRotaFrete ObterTipoPercursoVP(TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, bool compraVoltaComEixoSuspenso, bool compraIdaVoltaMesmoSendoPontoMaisDistante = false)
        {
            if (tipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante && compraVoltaComEixoSuspenso)
                return TipoRotaFrete.Ida;

            if (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante && !compraIdaVoltaMesmoSendoPontoMaisDistante)
                return TipoRotaFrete.Ida;

            return TipoRotaFrete.IdaVolta;
        }

        private Dominio.Entidades.RotaFrete SetarRotaFreteExclusivaCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

            if (!repRotaFrete.PossuiRotaFreteExclusivaCompraValePedagio())
                return null;

            RotaFrete servicoRotaFrete = new RotaFrete(_unitOfWork);

            List<Dominio.Entidades.RotaFrete> rotasPedagio = servicoRotaFrete.ObterRotaFreteCarga(carga, cargaPedidos, configuracao, tipoServicoMultisoftware, true);

            if (rotasPedagio.Count == 0)
                return null;

            return rotasPedagio.FirstOrDefault();
        }

        private bool VerificarCompraValePedagioVoltaEixoSuspenso(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.RotaFrete rotaFrete, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            if (CompraVoltaMesmoSendoPontoMaisDistante(integracaoDBTrans, rotaFrete, tipoUltimoPontoRoteirizacao))
                return true;

            if (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                return false;

            if (rotaFrete != null)
                return rotaFrete.TipoRota == TipoRotaFrete.IdaVolta && rotaFrete.TipoCarregamentoVolta == RetornoCargaTipo.Vazio;

            if (integracaoDBTrans.TipoRotaFrete != TipoRotaFreteDBTrans.NaoEspecificado)
                return integracaoDBTrans.TipoRotaFrete == TipoRotaFreteDBTrans.IdaVolta;

            if (tipoOperacao != null)
                return tipoOperacao.TipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.AteOrigem && tipoOperacao.TipoCarregamento == RetornoCargaTipo.Vazio && tipoOperacao.EixosSuspenso == EixosSuspenso.Volta;

            return false;
        }

        #endregion Métodos Privados
    }
}

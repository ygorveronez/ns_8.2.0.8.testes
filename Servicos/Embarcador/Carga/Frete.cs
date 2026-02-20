using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Entidades.Embarcador.Frete;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using Dominio.ObjetosDeValor.Relatorios;
using Repositorio;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class Frete : ServicoBase
    {
        #region Propriedades Protegidas

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly CancellationToken _cancellationToken;

        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware;

        #endregion

        #region Construtores

        public Frete(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork)
        {
            TipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        public Frete(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base()
        {
            TipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        public Frete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Frete(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public void LimparTabelaFreteRotaParaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteRota repCargaTabelaFreteRota = new Repositorio.Embarcador.Cargas.CargaTabelaFreteRota(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota> cargaTabelasFreteRota = repCargaTabelaFreteRota.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota cargaTabelaFreteRota in cargaTabelasFreteRota)
            {
                repCargaTabelaFreteRota.Deletar(cargaTabelaFreteRota);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete RecalcularFreteTabelaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, int tabelaFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool atualizarInformacoesPagamentoPedido = true)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            if (tabelaFrete > 0) //Quando o frete for por rota e ouver mais que uma tabela para a mesma rota o usuário informa qual a tabela deseja utilizar.
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRota() { Codigo = tabelaFrete };

                Repositorio.Embarcador.Cargas.CargaTabelaFreteRota repCargaTabelaFreteRota = new Repositorio.Embarcador.Cargas.CargaTabelaFreteRota(unitOfWork);

                repCargaTabelaFreteRota.DeletarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota cargaTabelaFreteRota = new Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota();
                cargaTabelaFreteRota.Carga = carga;
                cargaTabelaFreteRota.TabelaFreteRota = tabelaFreteRota;
                repCargaTabelaFreteRota.Inserir(cargaTabelaFreteRota);
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = null;
            if (!carga.CalcularFreteSemEstornarComplemento)
                retorno = CalcularFreteExtornandoComplementos(ref carga, unitOfWork, configuracao, configuracaoPedido, atualizarInformacoesPagamentoPedido);
            else
                retorno = CalcularFreteSemExtornarComplementos(ref carga, unitOfWork, configuracao, configuracaoPedido);

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
            {
                //carga = repCarga.BuscarPorCodigo(carga.Codigo);
                if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                    serRateioFrete.ZerarValoresDaCarga(carga, false, unitOfWork);

                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela;

                //repCarga.Atualizar(carga);
            }

            return retorno;
        }

        public string DefinirEtapaEFreteCargas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            if (cargaPedidos == null || cargaPedidos.Count <= 0)
                return retorno;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            LimparTabelaFreteRotaParaCarga(carga, unitOfWork);

            if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Cliente;

            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador && carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente &&
                (!carga.CargaTransbordo || (carga.CargaTransbordo && carga.TipoFreteEscolhido != TipoFreteEscolhido.Operador)))
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Tabela;

            repCarga.Atualizar(carga);

            if (carga.PossuiPendencia)
                retorno = "Os dados foram atualizados, porém, não foi possível calcular o frete. Por favor, verifique a pendência.";

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            { //Quando a carga exige as notas antes de calcular o frete, e já enviou as notas anteriormente.
                bool pedidosAutorizados = repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo);

                if (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS &&
                    (carga.DataEnvioUltimaNFe.HasValue || pedidosAutorizados) &&
                    (carga.TipoOperacao == null || !carga.TipoOperacao.PermiteImportarDocumentosManualmente || carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao) &&
                    !carga.CargaDePreCargaEmFechamento
                )
                {
                    if (!carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false)
                    {
                        if (!carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false)
                        {

                            if (configuracaoEmbarcador.IncluirCargaCanceladaProcessarDT && pedidosAutorizados)
                            {
                                //se arcelor e todas as notas estao enviadas precisamos passar pelo finalizar envio das notas
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

                                decimal pesoNaNFspesoNaNFs = repPedidoXMLnotaFiscal.BuscarPesoPorCarga(carga.Codigo);
                                int volumes = repPedidoXMLnotaFiscal.BuscarVolumesPorCarga(carga.Codigo);
                                string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFspesoNaNFs, volumes, null, null, null, null, null, configuracaoEmbarcador, TipoServicoMultisoftware, null, null, unitOfWork);
                            }
                            else
                                servicoCarga.AvancarEtapaAgNFe(carga);

                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador && carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                            {
                                Servicos.Log.TratarErro($"Atualizou a situação para calculo frete, CALCULANDO FRETE Consolidacao.AutorizacaoEmissao Carga {carga.CodigoCargaEmbarcador} - EnvioUltimaNota {carga.DataEnvioUltimaNFe.HasValue} - Todos Pedidos Autorizados {pedidosAutorizados} ", "AtualizouSituacaoCalculoFrete");
                                carga.CalculandoFrete = true;
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                                carga.DataInicioCalculoFrete = DateTime.Now;
                            }

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro($"Atualizou a situação para calculo frete 32 Carga {carga.CodigoCargaEmbarcador} - EnvioUltimaNota {carga.DataEnvioUltimaNFe.HasValue} - Todos Pedidos Autorizados {pedidosAutorizados} ", "AtualizouSituacaoCalculoFrete");

                            if (!carga.CargaTransbordo && (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || carga.ExigeConfirmacaoAntesEmissao))
                            {
                                carga.DataEnvioUltimaNFe = null; //volta a situacao a data de envio da nota para poder calcular o frete e ver se pode emitir.
                                carga.DataInicioEmissaoDocumentos = null;
                            }
                        }
                    }
                    else
                        servicoCarga.AvancarEtapaAgNFe(carga);
                }
                else
                {
                    if (!carga.AguardarIntegracaoDadosTransporte)
                    {
                        if (carga.TipoOperacao?.ConfiguracaoCarga?.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento ?? false)
                        {
                            if ((cargaPedidos.Count > 0) && (cargaPedidos.First().Pedido.DataInicialColeta.HasValue) && (carga.TipoOperacao.ConfiguracaoCarga?.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento > 0))
                            {
                                int horasConfiguradas = carga.TipoOperacao.ConfiguracaoCarga.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento;
                                DateTime horaLimite = cargaPedidos.First().Pedido.DataInicialColeta.Value.AddHours(-horasConfiguradas);
                                if (DateTime.Now < horaLimite)
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                                else
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                            }
                        }
                        else
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                    }
                }
                repCarga.Atualizar(carga);
            }
            else
            {
                if ((carga.ModeloVeicularCarga != null || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao) && carga.TipoDeCarga != null))
                {
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";

                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn)
                    {
                        carga.CalculandoFrete = true;
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                        carga.DataInicioCalculoFrete = DateTime.Now;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 30 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }
                    else
                    {
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                        bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false) && repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao();

                        if (!existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    }

                    repCarga.Atualizar(carga);
                }
            }

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                if (carga.TipoOperacao != null && !carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    repCarga.Atualizar(carga);
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                        {
                            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
                            repCargaPedido.Atualizar(cargaPedido);
                        }
                    }
                    if (!carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        Servicos.Embarcador.Carga.RateioFrete serCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                        serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                        if (carga.EmpresaFilialEmissora != null)
                            serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, true, unitOfWork, TipoServicoMultisoftware);
                    }
                }
            }

            if (carga.TipoOperacao != null && carga.ExigeNotaFiscalParaCalcularFrete && ((carga.TipoOperacao.ExigePlacaTracao && (carga.Veiculo == null || carga.Veiculo.TipoVeiculo != "0")) || carga.TipoOperacao.ExigirConfirmacaoDadosTransportadorAvancarCarga))
            {
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                carga.CalculandoFrete = false;

                if (!carga.DataEnvioUltimaNFe.HasValue)
                {
                    if ((!carga.TipoOperacao.PermiteImportarDocumentosManualmente || carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao) && repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                }
            }

            return retorno;
        }

        public async Task<string> DefinirEtapaEFreteCargasAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            if (cargaPedidos == null || cargaPedidos.Count <= 0)
                return retorno;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();

            LimparTabelaFreteRotaParaCarga(carga, unitOfWork);

            if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Cliente;

            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador && carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente &&
                (!carga.CargaTransbordo || (carga.CargaTransbordo && carga.TipoFreteEscolhido != TipoFreteEscolhido.Operador)))
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Tabela;

            await repCarga.AtualizarAsync(carga);

            if (carga.PossuiPendencia)
                retorno = "Os dados foram atualizados, porém, não foi possível calcular o frete. Por favor, verifique a pendência.";

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            { //Quando a carga exige as notas antes de calcular o frete, e já enviou as notas anteriormente.
                bool pedidosAutorizados = repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo);

                if (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS &&
                    (carga.DataEnvioUltimaNFe.HasValue || pedidosAutorizados) &&
                    (carga.TipoOperacao == null || !carga.TipoOperacao.PermiteImportarDocumentosManualmente || carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao) &&
                    !carga.CargaDePreCargaEmFechamento
                )
                {
                    if (!carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false)
                    {
                        if (!carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false)
                        {

                            if (configuracaoEmbarcador.IncluirCargaCanceladaProcessarDT && pedidosAutorizados)
                            {
                                //se arcelor e todas as notas estao enviadas precisamos passar pelo finalizar envio das notas
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

                                decimal pesoNaNFspesoNaNFs = repPedidoXMLnotaFiscal.BuscarPesoPorCarga(carga.Codigo);
                                int? volumesNullable = await repPedidoXMLnotaFiscal.BuscarVolumesPorCargaAsync(carga.Codigo);
                                int volumes = volumesNullable ?? 0;
                                string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFspesoNaNFs, volumes, null, null, null, null, null, configuracaoEmbarcador, TipoServicoMultisoftware, null, null, unitOfWork);
                            }

                            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador && carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                            {
                                Servicos.Log.TratarErro($"Atualizou a situação para calculo frete, CALCULANDO FRETE Consolidacao.AutorizacaoEmissao Carga {carga.CodigoCargaEmbarcador} - EnvioUltimaNota {carga.DataEnvioUltimaNFe.HasValue} - Todos Pedidos Autorizados {pedidosAutorizados} ", "AtualizouSituacaoCalculoFrete");
                                carga.CalculandoFrete = true;
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                                carga.DataInicioCalculoFrete = DateTime.Now;
                            }

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro($"Atualizou a situação para calculo frete 32 Carga {carga.CodigoCargaEmbarcador} - EnvioUltimaNota {carga.DataEnvioUltimaNFe.HasValue} - Todos Pedidos Autorizados {pedidosAutorizados} ", "AtualizouSituacaoCalculoFrete");

                            if (!carga.CargaTransbordo && (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || carga.ExigeConfirmacaoAntesEmissao))
                            {
                                carga.DataEnvioUltimaNFe = null; //volta a situacao a data de envio da nota para poder calcular o frete e ver se pode emitir.
                                carga.DataInicioEmissaoDocumentos = null;
                            }
                        }
                    }
                    else
                    {
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                        carga.ProcessandoDocumentosFiscais = true;
                        carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    }
                }
                else
                {
                    if (!carga.AguardarIntegracaoDadosTransporte)
                    {
                        if (carga.TipoOperacao?.ConfiguracaoCarga?.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento ?? false)
                        {
                            if ((cargaPedidos.Count > 0) && (cargaPedidos.First().Pedido.DataInicialColeta.HasValue) && (carga.TipoOperacao.ConfiguracaoCarga?.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento > 0))
                            {
                                int horasConfiguradas = carga.TipoOperacao.ConfiguracaoCarga.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento;
                                DateTime horaLimite = cargaPedidos.First().Pedido.DataInicialColeta.Value.AddHours(-horasConfiguradas);
                                if (DateTime.Now < horaLimite)
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                                else
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                            }
                        }
                        else
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                    }
                }
                await repCarga.AtualizarAsync(carga);
            }
            else
            {
                if ((carga.ModeloVeicularCarga != null || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao) && carga.TipoDeCarga != null))
                {
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";

                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn)
                    {
                        carga.CalculandoFrete = true;
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                        carga.DataInicioCalculoFrete = DateTime.Now;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 30 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }
                    else
                    {
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                        bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false) && repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao();

                        if (!existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    }

                    await repCarga.AtualizarAsync(carga);
                }
            }

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                if (carga.TipoOperacao != null && !carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    await repCarga.AtualizarAsync(carga);
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                        {
                            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
                            await repCargaPedido.AtualizarAsync(cargaPedido);
                        }
                    }
                    if (!carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        Servicos.Embarcador.Carga.RateioFrete serCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                        serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                        if (carga.EmpresaFilialEmissora != null)
                            serCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, true, unitOfWork, TipoServicoMultisoftware);
                    }
                }
            }

            if (carga.TipoOperacao != null && carga.ExigeNotaFiscalParaCalcularFrete && ((carga.TipoOperacao.ExigePlacaTracao && (carga.Veiculo == null || carga.Veiculo.TipoVeiculo != "0")) || carga.TipoOperacao.ExigirConfirmacaoDadosTransportadorAvancarCarga))
            {
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                carga.CalculandoFrete = false;

                if (!carga.DataEnvioUltimaNFe.HasValue)
                {
                    if ((!carga.TipoOperacao.PermiteImportarDocumentosManualmente || carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao) && repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                }
            }

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFreteSemExtornarComplementos(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteSempreExtornar = repCargaComponentesFrete.BuscarPorCargaSempreExtornar(carga.Codigo, true);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFreteSempreExtornar)
            {
                if (componente.CargaComplementoFrete != null)
                    serComplementoFrete.ExtornarComplementoDeFrete(componente.CargaComplementoFrete, TipoServicoMultisoftware, unitOfWork);

                repCargaComponentesFrete.Deletar(componente);
            }

            if (carga.CargaAgrupada)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteSempreExtornarOrigem = repCargaComponentesFrete.BuscarPorCargaSempreExtornar(cargaOrigem.Codigo, true);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFreteSempreExtornarOrigem)
                        repCargaComponentesFrete.Deletar(componente);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = ProcessarFrete(ref carga, unitOfWork, false, true, configuracaoTMS, configuracaoPedido, true, false, false);

            if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFilialEmissora = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, false, true, unitOfWork, TipoServicoMultisoftware, configuracaoTMS, configuracaoPedido);
                if (retornoFilialEmissora != null)
                    retorno.DadosFreteFilialEmissora = retornoFilialEmissora;
            }
            else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
            {
                Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, TipoServicoMultisoftware, unitOfWork, configuracaoTMS);
            }

            NotificarAlteracaoCargaAoOperador(retorno, carga, unitOfWork, configuracaoTMS);

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFreteExtornandoComplementos(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool atualizarInformacoesPagamentoPedido = true)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete repCargaTabelaFreteComponenteFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete(unitOfWork);

            bool cteEmitidoNoEmbarcador = repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo);

            if (!cteEmitidoNoEmbarcador && (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao))
            {
                Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarComplementosPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFrete)
                    serComplementoFrete.ExtornarComplementoDeFrete(componente.CargaComplementoFrete, TipoServicoMultisoftware, unitOfWork);

                repCargaComponentesFrete.DeletarPorCarga(carga.Codigo);

                if (carga.CargaAgrupada)
                    repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, false);

                repCargaTabelaFreteComponenteFrete.DeletarPorCarga(carga.Codigo);
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = ProcessarFrete(ref carga, unitOfWork, false, true, configuracao, configuracaoPedido, atualizarInformacoesPagamentoPedido);

            if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFilialEmissora = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, true, atualizarInformacoesPagamentoPedido, unitOfWork, TipoServicoMultisoftware, configuracao, configuracaoPedido);
                if (retornoFilialEmissora != null)
                    retornoFrete.DadosFreteFilialEmissora = retornoFilialEmissora;
            }
            else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
            {
                Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, TipoServicoMultisoftware, unitOfWork, configuracao);
            }

            NotificarAlteracaoCargaAoOperador(retornoFrete, carga, unitOfWork, configuracao);

            return retornoFrete;
        }

        public (decimal Percentual, decimal ValorTabela, decimal PercentualInverso) CalcularPercentualFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            decimal percentual = 0;
            decimal valorTabela = 0;
            decimal percentualInverso = 0;

            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.todos && carga.TipoFreteEscolhido != TipoFreteEscolhido.Tabela && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                decimal valorFrete = 0;

                if (carga.TipoFreteEscolhido != TipoFreteEscolhido.todos && carga.TipoFreteEscolhido != TipoFreteEscolhido.Tabela)
                {

                    valorTabela = carga.ValorFreteTabelaFrete;

                    if (configuracaoEmbarcador.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga)
                    {
                        valorTabela -= carga.ValorICMS - carga.ValorISS;

                        valorTabela -= BuscarValorTotalComponentes(carga);
                    }

                    if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                        valorFrete = carga.ValorFreteEmbarcador;
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                        valorFrete = carga.ValorFreteOperador;
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Leilao)
                        valorFrete = carga.ValorFreteLeilao;
                }

                if (valorTabela > 0 && valorFrete > 0)
                {

                    if (configuracaoEmbarcador.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga)
                    {
                        percentual = ((valorFrete - valorTabela) * 100) / valorTabela;
                        percentualInverso = ((valorFrete - valorTabela) * 100) / valorFrete;
                    }
                    else
                    {
                        decimal maiorValor = 0;
                        decimal menorValor = 0;
                        var fator = 1;

                        if (valorFrete >= valorTabela)
                        {
                            fator = -1;
                            maiorValor = valorFrete;
                            menorValor = valorTabela;
                        }
                        else
                        {
                            maiorValor = valorTabela;
                            menorValor = valorFrete;
                        }

                        percentual = (((menorValor * 100) / maiorValor) - 100) * fator;
                        percentualInverso = (((maiorValor * 100) / menorValor) - 100) * (-1 * fator);
                    }
                }
            }

            return ValueTuple.Create(percentual, valorTabela, percentualInverso);

        }

        public decimal BuscarValorTotalComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            decimal valorTotalComponentes = 0;

            if (carga.Componentes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in carga.Componentes)
                {
                    if (componente.TipoComponenteFrete != TipoComponenteFrete.ISS && componente.TipoComponenteFrete != TipoComponenteFrete.ICMS)
                    {
                        valorTotalComponentes += componente.ValorComponente;
                    }
                }
            }

            return valorTotalComponentes;
        }

        public decimal BuscarValorTotalDescontarComponentesTabelaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete)
        {
            decimal valorTotalComponentes = 0;

            if (carga.Componentes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in carga.Componentes)
                {
                    if (componenteFrete == componente.ComponenteFrete)
                        valorTotalComponentes += componente.ValorComponente;
                }
            }

            return valorTotalComponentes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete VerificarFrete(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool executarPreCalculoFrete = false, Dominio.Entidades.Usuario usuario = null)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = ProcessarFrete(ref carga, unitOfWork, true, false, configuracao, configuracaoPedido, precalculoFrete: executarPreCalculoFrete, usuario: usuario);

            if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFilialEmissora = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, true, false, true, unitOfWork, TipoServicoMultisoftware, configuracao, configuracaoPedido);
                if (retornoFilialEmissora != null)
                    retornoFrete.DadosFreteFilialEmissora = retornoFilialEmissora;
            }

            NotificarAlteracaoCargaAoOperador(retornoFrete, carga, unitOfWork, configuracao);

            return retornoFrete;
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente ProcessarTabelasPorMaiorValor(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga, List<TabelaFrete> tabelasFrete, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool cargaOrigem, out decimal maiorValorEncontrado)
        {
            //fixo Regra maior Valor.
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaMaiorValor = null;
            decimal maiorValor = 0;
            maiorValorEncontrado = 0;
            foreach (TabelaFrete tabelaFrete in tabelasFrete)
            {
                StringBuilder mensagemRetorno = new StringBuilder();

                switch (tabelaFrete.TipoTabelaFrete)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
                        if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos;
                            if (cargaOrigem)
                                cargaPedidos = (from obj in cargaPedidosCarga where obj.CargaOrigem.Codigo == carga.Codigo select obj).ToList();
                            else
                                cargaPedidos = (from obj in cargaPedidosCarga where obj.Carga.Codigo == carga.Codigo select obj).ToList();

                            int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
                            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
                            if (carga.Carregamento != null)
                                carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                            if (carga.Carregamento != null)
                                carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);


                            for (int i = 0; i < cargaPedidos.Count; i++)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, carregamentoPedidoNotaFiscals, 0);
                                Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                                if (tabelasCliente.Count == 1)
                                {
                                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelasCliente[0];
                                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                                    dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                                    if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                                        svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                                    else
                                        svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);

                                    decimal valorTotal = dados.ValorFrete + dados.ValorTotalComponentes;

                                    if (valorTotal > maiorValor)
                                    {
                                        maiorValor = valorTotal;
                                        tabelaMaiorValor = tabelaFreteCliente;
                                        maiorValorEncontrado = maiorValor;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return tabelaMaiorValor;
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente ProcessarTabelas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga, List<TabelaFrete> tabelasFrete, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, bool cargaOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, out decimal maiorValorEncontrado)
        {
            //fixo Regra maior Valor.
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaMaiorValor = null;
            decimal maiorValor = 0;
            maiorValorEncontrado = 0;
            foreach (TabelaFrete tabelaFrete in tabelasFrete)
            {
                StringBuilder mensagemRetorno = new StringBuilder();

                switch (tabelaFrete.TipoTabelaFrete)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
                        if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                        //|| tabelasFrete[0].TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos;
                            if (cargaOrigem)
                                cargaPedidos = (from obj in cargaPedidosCarga where obj.CargaOrigem.Codigo == carga.Codigo select obj).ToList();
                            else
                                cargaPedidos = (from obj in cargaPedidosCarga where obj.Carga.Codigo == carga.Codigo select obj).ToList();

                            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, calculoFreteFilialEmissora, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao);
                            if (parametrosCalculo == null)
                                return null;

                            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                            if (tabelasCliente.Count == 1)
                            {
                                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelasCliente[0];
                                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                                dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                                    svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                                else
                                    svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);

                                decimal valorTotal = dados.ValorFrete + dados.ValorTotalComponentes;

                                if (valorTotal > maiorValor)
                                {
                                    maiorValor = valorTotal;
                                    tabelaMaiorValor = tabelaFreteCliente;
                                    maiorValorEncontrado = maiorValor;
                                }
                            }
                        }
                        break;
                }
            }
            return tabelaMaiorValor;
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente retornarTabelaMaiorValor(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaMaiorValor = null;
            decimal maiorValor = 0;
            //fixo Regra maior Valor.
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasOriginais)
            {
                StringBuilder mensagemRetorno = new StringBuilder();
                List<TabelaFrete> tabelasFrete = ObterTabelasFrete(carga, unitOfWork, TipoServicoMultisoftware, configuracao, ref mensagemRetorno, false, null, false);

                if (tabelasFrete.Count > 0)
                {
                    decimal valor = 0;
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaMaiorValorCarga = ProcessarTabelas(carga, cargaPedidos, tabelasFrete, false, unitOfWork, true, configuracao, out valor);
                    if (tabelaMaiorValorCarga != null && valor > maiorValor)
                    {
                        tabelaMaiorValor = tabelaMaiorValorCarga;
                        maiorValor = valor;
                    }
                }
            }
            //se não encontrou tenta verificar se existe tabela por maior valor.
            if (tabelaMaiorValor == null)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasOriginais)
                {
                    StringBuilder mensagemRetorno = new StringBuilder();
                    List<TabelaFrete> tabelasFrete = ObterTabelasFrete(carga, unitOfWork, TipoServicoMultisoftware, configuracao, ref mensagemRetorno, false, null, false);

                    if (tabelasFrete.Count > 0)
                    {
                        decimal valor = 0;
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaMaiorValorCarga = ProcessarTabelasPorMaiorValor(carga, cargaPedidos, tabelasFrete, false, unitOfWork, configuracao, true, out valor);
                        if (tabelaMaiorValorCarga != null && valor > maiorValor)
                        {
                            tabelaMaiorValor = tabelaMaiorValorCarga;
                            maiorValor = valor;
                        }
                    }
                }

            }
            return tabelaMaiorValor;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete ProcessarFrete(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool adicionarComponentesCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool atualizarInformacoesPagamentoPedido = true, bool tabelaFreteMinima = false, bool estornarComponentes = true, bool precalculoFrete = false, Dominio.Entidades.Usuario usuario = null)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao repCargaTabelaFreteSubContratacao = new Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFreteExistente = carga.TabelaFrete;

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { };

            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                if (!repCargaPedido.ExisteDiferenteSubcontratacaoPorCarga(carga.Codigo))
                    cargaTabelaFreteSubContratacao = repCargaTabelaFreteSubContratacao.BuscarPorCarga(carga.Codigo);

            if (!apenasVerificar && (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || !carga.CalculandoFrete))
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.ExcluirComposicoesFrete(carga, unitOfWork);

            if (!apenasVerificar && !carga.FixarUtilizacaoContratoTransportador)
            {
                if (!ValidarVeiculoVinculadoContratoFrete(retorno, carga, configuracaoTMS, unitOfWork))
                    return retorno;

                carga.ValorFreteContratoFrete = 0;
                carga.ValorFreteContratoFreteExcedente = 0;

                Servicos.Log.GravarInfo($"Removeu o contrato {carga.ContratoFreteTransportador?.Codigo ?? 0} da Carga '{carga.Codigo}' Frete.ProcessarFrete", "RemoverContratoFreteTransportador");
                carga.ContratoFreteTransportador = null;
                repContratoSaldoMes.DeletarPorCarga(carga.Codigo);
            }

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = ObterDadosRotas(carga, unitOfWork, configuracaoPedido);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);

            new Servicos.Embarcador.Carga.RateioFormula().DefinirFormulaRateio(carga, cargaPedidos, unitOfWork);

            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao && carga.DadosSumarizados?.CargaTrecho != CargaTrechoSumarizada.SubCarga)
            {
                new Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador, configuracaoTMS).ProcessarFreteEGerarStagesAgrupadas(carga, apenasVerificar);
                if (carga.ValorFrete > 0)
                {
                    SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, false, carga.TipoFreteEscolhido, usuario); //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela);
                    retorno.situacao = SituacaoRetornoDadosFrete.FreteValido;
                    return retorno;
                }
            }

            if (!ValidarTipoOperacaoCarga(ref retorno, carga, unitOfWork, configuracaoTMS) ||
                VerificarTipoFreteCliente(ref retorno, ref carga, apenasVerificar, unitOfWork) ||
                VerificarTipoFreteOperador(ref retorno, ref carga, cargaPedidos, configuracaoTMS, cargaTabelaFreteSubContratacao, apenasVerificar, estornarComponentes, false, unitOfWork, usuario) ||
                VerificarFreteNegociadoNoPedido(ref retorno, carga, cargaPedidos, configuracaoTMS, cargaTabelaFreteSubContratacao, apenasVerificar, adicionarComponentesCarga, false, unitOfWork) ||
                VerificarTipoFreteEmbarcador(ref retorno, carga, apenasVerificar, false, unitOfWork) ||
                VerificarTabelaFreteExistente(ref retorno, carga, cargaPedidos, configuracaoTMS, cargaTabelaFreteSubContratacao, tabelaFreteExistente, apenasVerificar, false, unitOfWork))
                return retorno;

            StringBuilder mensagemRetorno = new StringBuilder();
            List<TabelaFrete> tabelasFrete = new List<TabelaFrete>();

            if ((!apenasVerificar || precalculoFrete) && !carga.FixarUtilizacaoContratoTransportador)
            {
                tabelasFrete = ObterTabelasFrete(carga, unitOfWork, TipoServicoMultisoftware, configuracaoTMS, ref mensagemRetorno, false, null, false, tabelaFreteMinima);

                //Se não achar nenhuma tabela normal, tenta buscar a tabela de frete configurada como mínima
                if (tabelasFrete.Count == 0 && !tabelaFreteMinima)
                    tabelasFrete = ObterTabelasFrete(carga, unitOfWork, TipoServicoMultisoftware, configuracaoTMS, ref mensagemRetorno, false, null, false, true);
            }
            else if (tabelaFreteExistente != null)
                tabelasFrete.Add(tabelaFreteExistente);

            if (tabelasFrete.Where(obj => obj.ContratoFreteTransportador != null).Count() > 1)
            {
                bool localizouPlaca = false;
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelaFreteCompativelPlaca = new List<TabelaFrete>();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaCompara = carga;
                List<int> reboques = new List<int>();
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count() > 0)
                    reboques = (from o in carga.VeiculosVinculados select o.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete in tabelasFrete)
                {
                    if (tabelaFrete.ContratoFreteTransportador != null)
                    {
                        if (carga.Veiculo != null && ((tabelaFrete.ContratoFreteTransportador.Veiculos.Any(obj => obj.Veiculo.Codigo == cargaCompara.Veiculo.Codigo) && (reboques.Count() == 0))
                            || (reboques.Count() > 0 && tabelaFrete.ContratoFreteTransportador.Veiculos.Any(obj => reboques.Contains(obj.Veiculo.Codigo)))))
                        {
                            localizouPlaca = true;
                            tabelaFreteCompativelPlaca.Add(tabelaFrete);
                        }
                    }
                    else
                        tabelaFreteCompativelPlaca.Add(tabelaFrete);
                }
                if (localizouPlaca)
                    tabelasFrete = tabelaFreteCompativelPlaca;
            }

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClientePadrao = null;

            //todo: criar uma opçao por ora está apenas como embarcador.
            if (configuracaoTMS.CompararTabelasDeFreteParaCalculo && !carga.FixarUtilizacaoContratoTransportador)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(carga.Codigo);

                if (tabelasFrete.Count > 0)
                {
                    decimal valor = 0;
                    tabelaFreteClientePadrao = ProcessarTabelas(carga, cargaPedidos, tabelasFrete, false, unitOfWork, false, configuracaoTMS, out valor);
                    if (cargasOriginais.Count == 0 && tabelaFreteClientePadrao == null)
                        tabelaFreteClientePadrao = ProcessarTabelasPorMaiorValor(carga, cargaPedidos, tabelasFrete, false, unitOfWork, configuracaoTMS, false, out valor);
                }

                if (tabelaFreteClientePadrao != null)
                {
                    tabelasFrete = new List<TabelaFrete>();
                    tabelasFrete.Add(tabelaFreteClientePadrao.TabelaFrete);
                }
                else
                {   //regraFixa, pega tabela de maior valor.


                    if (cargasOriginais.Count > 1)
                        tabelaFreteClientePadrao = retornarTabelaMaiorValor(cargasOriginais, cargaPedidos, false, unitOfWork, configuracaoTMS);

                    if (tabelaFreteClientePadrao != null)
                    {
                        tabelasFrete = new List<TabelaFrete>();
                        tabelasFrete.Add(tabelaFreteClientePadrao.TabelaFrete);
                    }
                }
            }

            bool valorValidadoPeloEmbarcador = false;
            if (!ValidarQuantidadeTabelasFreteDisponivel(ref retorno, ref carga, tabelasFrete, mensagemRetorno, apenasVerificar, unitOfWork, configuracaoTMS))
                valorValidadoPeloEmbarcador = true;


            if (!valorValidadoPeloEmbarcador)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];

                if (!ValidarPermiteTabelaPorDistancia(ref retorno, ref carga, tabelaFrete, mensagemRetorno, apenasVerificar, unitOfWork, configuracaoTMS))
                {
                    if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                        if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                            return retornoTabelaMinima;
                        else
                            return retorno;
                    }
                    else
                        return retorno;
                }


                switch (tabelaFrete.TipoTabelaFrete)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
                        if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga || tabelaFreteClientePadrao != null)
                        {
                            if (CalcularFretePorCliente(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false, configuracaoTMS, tabelaFreteClientePadrao))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;
                                }
                                else
                                    return retorno;
                            }

                            if ((carga.ValorFrete <= 0) && tabelaFrete.ValorParametroBaseObrigatorioParaCalculo)
                            {
                                if (servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                                {
                                    carga.PossuiPendencia = true;
                                    if (!configuracaoTMS.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 29 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                                    repCarga.Atualizar(carga);
                                }

                                return new RetornoDadosFrete()
                                {
                                    mensagem = "O valor do parâmetro base está zerado",
                                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete,
                                    tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente
                                };
                            }
                        }
                        else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido)
                        {
                            if (CalcularFretePorClientePedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false, configuracaoTMS))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;
                                }
                                else
                                    return retorno;
                            }
                        }
                        else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                        {
                            Servicos.Embarcador.Carga.FretePedidoAgrupado svcFretePedidoAgrupado = new FretePedidoAgrupado(configuracaoTMS, unitOfWork, TipoServicoMultisoftware);

                            if (svcFretePedidoAgrupado.CalcularFrete(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;
                                }
                                else
                                    return retorno;
                            }
                        }
                        else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido)
                        {
                            if (CalcularFretePorDocumentoEmitido(ref retorno, ref carga, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false, configuracaoTMS))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;
                                }
                                else
                                    return retorno;
                            }

                            if (!apenasVerificar)
                            {
                                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                                cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);
                            }
                        }
                        else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido || tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedidoAgrupados)
                        {
                            if (CalcularFretePorClienteMaiorValorPedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false, configuracaoTMS))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;

                                }
                                else
                                    return retorno;
                            }
                        }
                        else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados)
                        {
                            if (CalcularFretePorClienteMaiorDistanciaPedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false, configuracaoTMS))
                            {
                                if (!tabelaFrete.TabelaFreteMinima && tabelaFrete.UtilizaTabelaFreteMinima && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoTabelaMinima = ProcessarFrete(ref carga, unitOfWork, apenasVerificar, adicionarComponentesCarga, configuracaoTMS, configuracaoPedido, atualizarInformacoesPagamentoPedido, true);
                                    if (retornoTabelaMinima.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                                        return retornoTabelaMinima;
                                    else
                                        return retorno;

                                }
                                else
                                    return retorno;
                            }
                        }

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota:
                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                            CalcularFretePorRota(ref retorno, ref carga, cargaPedidos, configuracaoTMS, dynRota, tabelaFrete, apenasVerificar, adicionarComponentesCarga, false, unitOfWork);
                        else
                            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto:
                        CalcularFretePorComissaoProduto(ref retorno, ref carga, cargaPedidos, configuracaoTMS, tabelaFrete, apenasVerificar, false, unitOfWork);
                        break;
                }

            }

            if (!apenasVerificar)
            {
                //Todo: Rever esse ponto, não sei se atende todos os cenário, provavelmente não.
                if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);
                    serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoTMS, false, unitOfWork, TipoServicoMultisoftware);
                }
                else
                {
                    Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracaoTMS, TipoServicoMultisoftware, unitOfWork);

                    if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                    {
                        Seguro.Seguro svcSeguro = new Seguro.Seguro(unitOfWork);
                        svcSeguro.VerificarSeNecessariaAutorizacaoSeguro(carga, cargaPedidos, unitOfWork);
                    }
                }

                ValidarValoresFrete(carga, cargaPedidos, unitOfWork);

                if (!ValidarSaldoContratoPrestacaoServico(retorno, carga, configuracaoTMS, unitOfWork))
                    return retorno;

                ProcessarRegraInclusaoICMSComponenteFrete(carga, cargaPedidos, configuracaoTMS, unitOfWork);
                CalcularValorFreteComICMSIncluso(carga, unitOfWork);
            }

            SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, false, carga.TipoFreteEscolhido, usuario); //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela);

            return retorno;
        }

        public void ValidarValoresFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                return;

            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


            Dominio.Entidades.Cliente tomador = cargaPedidos.FirstOrDefault()?.ObterTomador() ?? null;

            bool compararValores = false;
            bool gerarOcorrenciaDiferenca = false;
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaGerar = null;

            decimal percentualDiferencaBloquear = 0m;

            if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                compararValores = carga.TipoOperacao.BloquearDiferencaValorFreteEmbarcador;
                percentualDiferencaBloquear = carga.TipoOperacao.PercentualBloquearDiferencaValorFreteEmbarcador;
                gerarOcorrenciaDiferenca = carga.TipoOperacao.EmitirComplementoDiferencaFreteEmbarcador;
                tipoOcorrenciaGerar = carga.TipoOperacao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador;

            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    compararValores = tomador.BloquearDiferencaValorFreteEmbarcador;
                    percentualDiferencaBloquear = tomador.PercentualBloquearDiferencaValorFreteEmbarcador;
                    gerarOcorrenciaDiferenca = tomador.EmitirComplementoDiferencaFreteEmbarcador;
                    tipoOcorrenciaGerar = tomador.TipoOcorrenciaComplementoDiferencaFreteEmbarcador;
                }
                else if (tomador.GrupoPessoas != null)
                {
                    compararValores = tomador.GrupoPessoas.BloquearDiferencaValorFreteEmbarcador;
                    percentualDiferencaBloquear = tomador.GrupoPessoas.PercentualBloquearDiferencaValorFreteEmbarcador;
                    gerarOcorrenciaDiferenca = tomador.GrupoPessoas.EmitirComplementoDiferencaFreteEmbarcador;
                    tipoOcorrenciaGerar = tomador.GrupoPessoas.TipoOcorrenciaComplementoDiferencaFreteEmbarcador;
                }
            }

            if (compararValores)
            {
                decimal valorComponentes = repCargaComponenteFrete.BuscarValorTotalPorCargaSemImpostos(carga.Codigo);
                decimal valorTotalFrete = carga.ValorFrete + valorComponentes;
                decimal valorDiferencaPermitir = valorTotalFrete * (percentualDiferencaBloquear / 100m);
                decimal valorDiferencaFrete = 0m;

                if (carga.ValorFreteTabelaFrete > valorTotalFrete)
                {
                    valorDiferencaFrete = carga.ValorFreteTabelaFrete - valorTotalFrete;

                    if (valorDiferencaFrete > 0m)
                    {
                        carga.BloqueadaDiferencaValorFrete = false;
                        carga.ValorDiferencaValorFrete = valorDiferencaFrete;

                        if (gerarOcorrenciaDiferenca && tipoOcorrenciaGerar != null)
                        {
                            carga.GerarOcorrenciaDiferencaValorFrete = true;
                            carga.TipoOcorrenciaDiferencaValorFrete = tipoOcorrenciaGerar;
                        }
                        else
                        {
                            carga.GerarOcorrenciaDiferencaValorFrete = false;
                            carga.TipoOcorrenciaDiferencaValorFrete = null;
                        }
                    }
                }
                else
                {
                    valorDiferencaFrete = valorTotalFrete - carga.ValorFreteTabelaFrete;

                    if (valorDiferencaFrete > 0m && valorDiferencaFrete > valorDiferencaPermitir)
                    {
                        carga.BloqueadaDiferencaValorFrete = true;
                        carga.ValorDiferencaValorFrete = valorDiferencaFrete;
                        carga.GerarOcorrenciaDiferencaValorFrete = false;
                        carga.TipoOcorrenciaDiferencaValorFrete = null;
                    }
                }
            }
            else
            {
                carga.BloqueadaDiferencaValorFrete = false;
                carga.ValorDiferencaValorFrete = 0m;
                carga.GerarOcorrenciaDiferencaValorFrete = false;
                carga.TipoOcorrenciaDiferencaValorFrete = null;
            }

            repCarga.Atualizar(carga);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga RecalcularFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, decimal valorFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool freteFilialEmissora = false)
        {
            Servicos.Embarcador.Carga.RateioFrete serCargaRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            serCargaRateioFrete.ZerarValoresDaCarga(carga, freteFilialEmissora, unitOfWork);

            if (freteFilialEmissora)
                carga.ValorFreteFilialEmissora = valorFrete;
            else
                carga.ValorFrete = valorFrete;

            serCargaRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, freteFilialEmissora, unitOfWork, TipoServicoMultisoftware);

            if (!freteFilialEmissora)
                if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, false, true, unitOfWork, TipoServicoMultisoftware, configuracao, configuracaoPedido);

                    NotificarAlteracaoCargaAoOperador(retornoFrete, carga, unitOfWork, configuracao);
                }
                else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
                {
                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, TipoServicoMultisoftware, unitOfWork, configuracao);
                }

            if (freteFilialEmissora)
            {
                carga.TabelaFreteFilialEmissora = null;
                carga.TabelaFrete = null;
            }

            return carga;
        }

        public static bool DestacarComponenteTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteOriginal)
        {
            if (componenteFreteOriginal == null
                || tabelaFrete == null
                || tabelaFrete.ComponenteFreteDestacar == null
                || !tabelaFrete.DestacarComponenteFrete)
                return false;

            if (tabelaFrete.ComponenteFreteDestacar.Codigo != componenteFreteOriginal.Codigo)
                return false;

            return true;
        }
        public static async Task<bool> DestacarComponenteTabelaFreteAsync(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteOriginal)
        {
            if (componenteFreteOriginal == null
                || tabelaFrete == null
                || tabelaFrete.ComponenteFreteDestacar == null
                || !tabelaFrete.DestacarComponenteFrete)
                return await Task.FromResult(false);

            if (tabelaFrete.ComponenteFreteDestacar.Codigo != componenteFreteOriginal.Codigo)
                return await Task.FromResult(false);

            return await Task.FromResult(true);
        }

        public static decimal CalcularValorFreteAdicionalPorModeloCarroceriaVeiculo(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorFrete)
        {
            if (valorFrete <= 0m)
                return 0m;

            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> modelos = new List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            if (veiculo?.ModeloCarroceria != null)
                modelos.Add(veiculo.ModeloCarroceria);

            if (veiculosVinculados != null && veiculosVinculados.ToList().Count > 0)
                modelos.AddRange(from obj in veiculosVinculados where obj.ModeloCarroceria != null select obj.ModeloCarroceria);

            modelos = (from obj in modelos select obj).Distinct().ToList();

            decimal percentualAdicionalFrete = 0;
            foreach (Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modelo in modelos)
                percentualAdicionalFrete += modelo.PercentualAdicionalFrete;

            if (percentualAdicionalFrete <= 0m)
                return 0m;

            decimal valorAdicionar = valorFrete * (percentualAdicionalFrete / 100);

            composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Percentual adicional pelo Modelo * Valor Frete ", percentualAdicionalFrete.ToString("n2") + "% * " + valorFrete.ToString("n2"), percentualAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Adicional pelo modelo de Carroceria " + string.Join(",", (from obj in modelos select obj.Descricao)), componenteFrete?.Codigo ?? 0, valorAdicionar);

            if (componenteFrete == null)
                composicaoFrete.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido;

            return valorAdicionar;
        }

        public static List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> ObterTabelasFrete(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, bool tabelaFreteMinima, out StringBuilder mensagem, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int tipoOcorrencia)
        {
            return ObterTabelasFrete(parametros, tabelaFreteMinima, out mensagem, unidadeTrabalho, tipoServicoMultisoftware, tipoOcorrencia, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="tabelaFreteMinima"></param>
        /// <param name="mensagem"></param>
        /// <param name="unidadeTrabalho"></param>
        /// <param name="tipoServicoMultisoftware"></param>
        /// <param name="tipoOcorrencia"></param>
        /// <param name="primeiraValida">Parametro para retormar apenas a primeira tabela de frete válida, quando false,... irá retornar todas as tabelas válidas</param>
        /// <returns></returns>
        public static List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> ObterTabelasFrete(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, bool tabelaFreteMinima, out StringBuilder mensagem, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int tipoOcorrencia, bool primeiraValida)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFreteValidas = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = parametros.TransportadorTerceiro?.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = null;

            if (modalidade != null)
                modalidadeTransportador = repModalidadeTransportador.BuscarPorModalidade(modalidade.Codigo);

            mensagem = new StringBuilder();

            ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas = new ParametrosRetornarTabelasValidas()
            {
                CodigoEmpresa = parametros.Empresa?.Codigo ?? 0,
                CodigoFilial = parametros.Filial?.Codigo ?? 0,
                CodigoGrupoPessoaTomador = parametros.GrupoPessoas?.Codigo ?? 0,
                CodigoTipoCarga = parametros.TipoCarga?.Codigo ?? 0,
                CodigoTipoOcorrencia = tipoOcorrencia,
                CodigoTipoOperacao = parametros.TipoOperacao?.Codigo ?? 0,
                CodigoVeiculo = parametros.Veiculo?.Codigo ?? 0,
                CpfCnpjTerceiro = parametros.TransportadorTerceiro?.CPF_CNPJ ?? 0,
                CpfCnpjTomador = parametros.Tomador?.CPF_CNPJ ?? 0,
                DataVigencia = DateTime.Now.Date,
                PagamentoTerceiro = parametros.PagamentoTerceiro,
                RetornarPrimeiraValida = primeiraValida,
                CodigoModeloVeicularDaCarga = parametros.ModeloVeiculo?.Codigo ?? 0,
                CodigoModeloVeicularDoVeiculo = parametros.Veiculo?.ModeloVeicularCarga?.Codigo ?? 0,
                CalcularVariacoes = parametros.CalcularVariacoes,
                CodigoRotaFrete = parametros.Rota?.Codigo ?? 0,
            };

            if (parametros.Empresa != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorEmpresa(parametros.Empresa.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0 && primeiraValida)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
            }

            if (tabelasFreteValidas?.Count == 0 && parametros.Filial != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorFilial(parametros.Filial.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
            }

            if (tabelasFreteValidas?.Count == 0 && parametros.TipoOperacao != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorTipoOperacao(parametros.TipoOperacao.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
            }

            if (tabelasFreteValidas?.Count == 0 && parametros.GrupoPessoas != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorGrupoPessoas(parametros.GrupoPessoas.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
            }

            if (tabelasFreteValidas?.Count == 0 && parametros.PagamentoTerceiro)
            {
                tabelasFrete = repTabelaFrete.BuscarPorTransportadorTerceiro(tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
            }

            if (tabelasFreteValidas?.Count == 0)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPadrao(parametros.PagamentoTerceiro, parametros.TransportadorTerceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelaFrete != null)
                {
                    tabelasFrete.Add(tabelaFrete);
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeTrabalho);
                }
            }

            //if (tabelasFreteValidas?.Count <= 0)
            //{
            //    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = null;
            //    if (parametros.GrupoPessoas != null)
            //    {
            //        if (parametros.Filial != null)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoaFilial(parametros.GrupoPessoas.Codigo, parametros.Filial.Codigo, parametros.TipoOperacao, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, parametros.Tomador?.CPF_CNPJ);

            //        if (tabelaFrete == null)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoa(parametros.GrupoPessoas.Codigo, parametros.TipoOperacao, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, parametros.Tomador?.CPF_CNPJ);

            //        if (tabelaFrete == null)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoa(parametros.GrupoPessoas.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false);
            //    }

            //    if (tabelaFrete == null && parametros.TipoOperacao != null)
            //        tabelaFrete = repTabelaFrete.BuscarSemGrupoPessoaETipoOperacao(parametros.TipoOperacao.Codigo, parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, parametros.Tomador?.CPF_CNPJ);

            //    if (tabelaFrete == null)
            //        tabelaFrete = repTabelaFrete.BuscarSemGrupoPessoaESemTipoOperacao(parametros.PagamentoTerceiro, tabelaFreteMinima, false, parametros.TransportadorTerceiro?.CPF_CNPJ, parametros.Tomador?.CPF_CNPJ);

            //    if (tabelaFrete == null)
            //    {
            //        if (parametros.Tomador != null)
            //            mensagem.Append("O tomador do serviço (" + parametros.Tomador.Nome + ") não possui uma tabela de frete configurada para ele.");
            //        else
            //            mensagem.Append("O tomador do serviço não foi identificado.");
            //    }
            //    else
            //    {
            //        tabelasFreteValidas.Add(tabelaFrete);
            //    }
            //}

            return tabelasFreteValidas;
        }

        public static Dominio.Entidades.Embarcador.Frete.TabelaFrete RetornarTabelaValida(List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelas, ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasValida = RetornarTabelasValidas(tabelas, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unitOfWork);

            if (tabelasValida?.Count > 0)
                return tabelasValida[0];

            return null;
        }

        public static List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> RetornarTabelasValidas(List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelas, ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasValidas = new List<TabelaFrete>();

            tabelas = tabelas.OrderByDescending(obj => obj.PossuiVeiculos).ToList();

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabela in tabelas)
            {
                DateTime dataValidarVigencia;

                if (parametrosRetornarTabelasValidas.DataVigencia.HasValue)
                    dataValidarVigencia = parametrosRetornarTabelasValidas.DataVigencia.Value;
                else if (tabela.ValidarPorDataCarregamento && parametrosRetornarTabelasValidas.DataCarregamento.HasValue)
                    dataValidarVigencia = parametrosRetornarTabelasValidas.DataCarregamento.Value;
                else
                    dataValidarVigencia = parametrosRetornarTabelasValidas.DataCriacaoCarga.Value;

                if (!parametrosRetornarTabelasValidas.CalcularVariacoes && (tabela.Transportadores != null && tabela.Transportadores.Count > 0 && !tabela.Transportadores.Any(t => t.Codigo == parametrosRetornarTabelasValidas.CodigoEmpresa)))
                    continue;

                if (tabela.TiposOperacao != null && tabela.TiposOperacao.Count > 0 && !tabela.TiposOperacao.Any(tp => tp.Codigo == parametrosRetornarTabelasValidas.CodigoTipoOperacao))
                    continue;

                if (tabela.TiposCarga != null && tabela.TiposCarga.Count > 0 && !tabela.TiposCarga.Any(tp => tp.Codigo == parametrosRetornarTabelasValidas.CodigoTipoCarga) && parametrosRetornarTabelasValidas.CodigoTipoCarga > 0)
                    continue;

                if (tabela.Filiais != null && tabela.Filiais.Count > 0 && !tabela.Filiais.Any(f => f.Codigo == parametrosRetornarTabelasValidas.CodigoFilial) && parametrosRetornarTabelasValidas.CodigoFilial > 0)
                    continue;

                if (tabela.GrupoPessoas != null && tabela.GrupoPessoas.Codigo != parametrosRetornarTabelasValidas.CodigoGrupoPessoaTomador)
                    continue;

                // INÍCIO - filtros novos para os métodos que não passam o parametro RetornoCarga

                if (!parametrosRetornarTabelasValidas.CalcularVariacoes)
                {
                    if (tabela.ModelosReboque != null && tabela.ModelosReboque.Count > 0 && !tabela.UtilizaModeloVeicularVeiculo && !tabela.ModelosReboque.Any(rb => rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDaCarga))
                        continue;

                    if (tabela.ModelosReboque != null && tabela.ModelosReboque.Count > 0 && tabela.UtilizaModeloVeicularVeiculo && !tabela.ModelosReboque.Any(rb => (rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo || rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDaCarga)))
                        continue;

                    if (tabela.UtilizarModeloVeicularDaCargaParaCalculo)
                    {
                        if (tabela.ModelosTracao != null && tabela.ModelosTracao.Count > 0 && !tabela.ModelosTracao.Any(rb => rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDaCarga))
                            continue;
                    }
                    else
                    {
                        if (tabela.ModelosTracao != null && tabela.ModelosTracao.Count > 0 && !tabela.UtilizaModeloVeicularVeiculo && !tabela.ModelosTracao.Any(rb => rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDaCarga))
                            continue;

                        if (tabela.ModelosTracao != null && tabela.ModelosTracao.Count > 0 && tabela.UtilizaModeloVeicularVeiculo && !tabela.ModelosTracao.Any(rb => (rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo || rb.Codigo == parametrosRetornarTabelasValidas.CodigoModeloVeicularDaCarga)))
                            continue;
                    }
                }

                if (parametrosRetornarTabelasValidas.CodigoContratoFreteCliente > 0 && (tabela.ContratoFreteCliente == null || tabela.ContratoFreteCliente?.Codigo != parametrosRetornarTabelasValidas.CodigoContratoFreteCliente))
                    continue;

                if (tabela.CanalEntrega != null && tabela.CanalEntrega.Codigo != parametrosRetornarTabelasValidas.CodigoCanalEntrega)
                    continue;

                if (tabela.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas)
                {
                    if (tabela.Pallets != null && tabela.Pallets.Count > 0 && tabela.Pallets.Any(obj => obj.Tipo == TipoNumeroPalletsTabelaFrete.PorFaixaPallets))
                    {
                        Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete palletTabelaFrete = (from obj in tabela.Pallets where obj.Tipo == TipoNumeroPalletsTabelaFrete.PorFaixaPallets && obj.NumeroInicialPallet <= parametrosRetornarTabelasValidas.Pallets && obj.NumeroFinalPallet >= parametrosRetornarTabelasValidas.Pallets select obj).FirstOrDefault();
                        if (palletTabelaFrete == null && !tabela.PermiteValorAdicionalPorPalletExcedente)
                            continue;
                    }
                    //parametrosRetornarTabelasValidas.numero
                }

                if (tabela.Fronteiras?.Count > 0)
                {
                    if (parametrosRetornarTabelasValidas.CodigoFronteira > 0)
                    {
                        if (!tabela.Fronteiras.Any(f => f.CPF_CNPJ == parametrosRetornarTabelasValidas.CodigoFronteira))
                            continue;
                    }
                    else
                    {
                        Repositorio.RotaFreteFronteira repositorioRotaFreteFronteira = new Repositorio.RotaFreteFronteira(unitOfWork);
                        List<Dominio.Entidades.RotaFreteFronteira> rotaFreteFronteiras = repositorioRotaFreteFronteira.BuscarPorRotaFrete(parametrosRetornarTabelasValidas.CodigoRotaFrete);

                        if ((rotaFreteFronteiras.Count == 0) || !rotaFreteFronteiras.All(o => o.Cliente != null && tabela.Fronteiras.Any(f => f.CPF_CNPJ == o.Cliente.CPF_CNPJ)))
                            continue;
                    }
                }

                // FIM - filtros novos para os métodos que não passam o parametro RetornoCarga

                if (!parametrosRetornarTabelasValidas.CalcularVariacoes && tabela.PossuiVeiculos)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteVeiculo repositorioTabelaFreteVeiculo = new Repositorio.Embarcador.Frete.TabelaFreteVeiculo(unitOfWork);

                    List<int> codigosVeiculos = parametrosRetornarTabelasValidas.CodigosReboque != null ? parametrosRetornarTabelasValidas.CodigosReboque : new List<int>();
                    codigosVeiculos.Add(parametrosRetornarTabelasValidas.CodigoVeiculo);

                    if (!repositorioTabelaFreteVeiculo.ExistePorTabelaEVeiculo(tabela.Codigo, codigosVeiculos))
                        continue;
                }

                if (parametrosRetornarTabelasValidas.CodigoTipoOcorrencia > 0)
                {
                    if (tabela.AplicacaoTabela == AplicacaoTabela.Carga)
                        continue;

                    if (tabela.TiposDeOcorrencia != null && tabela.TiposDeOcorrencia.Count > 0 && !tabela.TiposDeOcorrencia.Any(f => f.Codigo == parametrosRetornarTabelasValidas.CodigoTipoOcorrencia))
                        continue;
                }
                else
                {
                    //Checando também se não tem um FreeTIme, já que agora as aplicações por ocorrência podem também servir para gerar DiariaAutomatica
                    if (tabela.AplicacaoTabela == AplicacaoTabela.Ocorrencia && tabela.LocalFreeTime == LocalFreeTime.Nenhum)
                        continue;
                }

                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                {
                    if (parametrosRetornarTabelasValidas.CpfCnpjTomador > 0d)
                    {
                        if (tabela.ContratoFreteTransportador != null && !tabela.ContratoFreteTransportador.Clientes.Any(c => c.Cliente.CPF_CNPJ == parametrosRetornarTabelasValidas.CpfCnpjTomador))
                            continue;
                    }

                    if (parametrosRetornarTabelasValidas.PagamentoTerceiro && parametrosRetornarTabelasValidas.CpfCnpjTerceiro > 0d)
                    {
                        if (!tabela.TransportadoresTerceiros.Any(c => c.CPF_CNPJ == parametrosRetornarTabelasValidas.CpfCnpjTerceiro) && tabela.TransportadoresTerceiros.Any())
                            continue;
                    }
                }
                else
                {
                    // INÍCIO - filtros novos para os métodos que não passam o parametro RetornoCarga

                    if (tabela.RotasFreteEmbarcador != null && tabela.RotasFreteEmbarcador.Count > 0 && !tabela.RotasFreteEmbarcador.Any(rf => rf.RotaFrete.Codigo == parametrosRetornarTabelasValidas.CodigoRotaFrete))
                        continue;

                    // FIM - filtros novos para os métodos que não passam o parametro RetornoCarga
                }

                if (tabela.ContratoFreteTransportador != null)
                {
                    if (tabela.ContratoFreteTransportador.Transportador != null && tabela.ContratoFreteTransportador.Transportador.Codigo != parametrosRetornarTabelasValidas.CodigoEmpresa && !tabela.ContratoFreteTransportador.Transportador.Filiais.Any(fil => fil.Codigo == parametrosRetornarTabelasValidas.CodigoEmpresa))
                        continue;

                    if (tabela.ContratoFreteTransportador.Filiais != null && tabela.ContratoFreteTransportador.Filiais.Count > 0 && !tabela.ContratoFreteTransportador.Filiais.Any(obj => obj.Filial.Codigo == parametrosRetornarTabelasValidas.CodigoFilial))
                        continue;

                    if (tabela.ContratoFreteTransportador.DataInicial > dataValidarVigencia.Date || tabela.ContratoFreteTransportador.DataFinal < dataValidarVigencia.Date)
                        continue;

                    if (tabela.ContratoFreteTransportador.CanaisEntrega != null && tabela.ContratoFreteTransportador.CanaisEntrega.Count > 0 && tabela.ContratoFreteTransportador.CanaisEntrega.Any(obj => obj.Codigo == parametrosRetornarTabelasValidas.CodigoCanalEntrega))
                        continue;

                    //todo: regra para walmart
                    if (tabela.ContratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui)
                    {
                        if (parametrosRetornarTabelasValidas.RetornoCarga != null && parametrosRetornarTabelasValidas.RetornoCarga.Carga.ContratoFreteTransportador == null && (parametrosRetornarTabelasValidas.RetornoCarga.Carga.TabelaFrete == null || (parametrosRetornarTabelasValidas.RetornoCarga.Carga.TabelaFrete.ContratoFreteTransportador == null && parametrosRetornarTabelasValidas.RetornoCarga.Carga.TabelaFrete.Ativo)))
                            continue;
                    }
                }
                else
                {
                    //todo: regra para walmart
                    if (parametrosRetornarTabelasValidas.RetornoCarga != null && parametrosRetornarTabelasValidas.RetornoCarga.Carga.ContratoFreteTransportador != null && parametrosRetornarTabelasValidas.RetornoCarga.Carga.ContratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui)
                        continue;
                }

                if (parametrosRetornarTabelasValidas.LocalFreeTime.HasValue)
                {
                    if (tabela.LocalFreeTime != LocalFreeTime.Todos && tabela.LocalFreeTime != parametrosRetornarTabelasValidas.LocalFreeTime.Value)
                        continue;
                }

                tabelasValidas.Add(tabela);

                if (parametrosRetornarTabelasValidas.RetornarPrimeiraValida && !parametrosRetornarTabelasValidas.CalcularVariacoes)
                    break;
            }

            return tabelasValidas;
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> ObterTabelasFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, ref StringBuilder mensagem, bool calculoFreteFilialEmissora, Dominio.Entidades.Cliente tomador = null, bool pagamentoTerceiro = false, bool tabelaFreteMinima = false, int tipoOcorrencia = 0, LocalFreeTime? localFreeTime = null, double fronteira = 0)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeDeTrabalho);

            decimal quantidadePallets = repCargaPedido.BuscarQuantidadePalletsPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = null;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPadrao = null;

            int codigoFilial = carga.Filial?.Codigo ?? 0;
            //if (carga.FilialOrigem != null)//regra fixa danone se outro cliente usar podemos criar uma configuração (carga origem só é gerada lá)
            //    codigoFilial = carga.FilialOrigem.Codigo;

            if (carga.CargaAgrupamento == null)
                cargaPedidoPadrao = repCargaPedido.BuscarPorCargaUnica(carga.Codigo);
            else
                cargaPedidoPadrao = repCargaPedido.BuscarPrimeiraPorCargaOrigem(carga.Codigo);

            //if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            //{
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFreteValidas = new List<TabelaFrete>();

            if (tomador == null)
                tomador = cargaPedidoPadrao.ObterTomador();

            bool exclusivaCalculoCliente = (calculoFreteFilialEmissora && carga.CalcularFreteCliente);
            Dominio.Entidades.Empresa empresa = carga.Empresa;

            if (calculoFreteFilialEmissora && !exclusivaCalculoCliente)
                empresa = carga.EmpresaFilialEmissora;

            List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntrega = repCargaPedido.BuscarCanaisEntregaPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repRetornoCarga.BuscarPorCargaRetorno(carga.Codigo);
            int codigoCanalEntrega = (canaisEntrega.Count == 1) ? canaisEntrega.FirstOrDefault().Codigo : 0;
            int codigoContratoFreteCliente = repContratoFreteCliente.BuscarContratoPorCarga(carga.Codigo)?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = carga.Terceiro?.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = null;

            if (modalidade != null)
                modalidadeTransportador = repModalidadeTransportador.BuscarPorModalidade(modalidade.Codigo);

            ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas = new ParametrosRetornarTabelasValidas()
            {
                CodigoCanalEntrega = codigoCanalEntrega,
                CodigoEmpresa = empresa?.Codigo ?? 0,
                CodigoFilial = codigoFilial,
                CodigoModeloVeicularDaCarga = carga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoGrupoPessoaTomador = tomador?.GrupoPessoas?.Codigo ?? 0,
                CodigoRotaFrete = carga.Rota?.Codigo ?? 0,
                CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                CodigoTipoOcorrencia = tipoOcorrencia,
                CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                CodigoVeiculo = carga.Veiculo?.Codigo ?? 0,
                CpfCnpjTerceiro = carga.Terceiro?.CPF_CNPJ ?? 0d,
                CpfCnpjTomador = tomador?.CPF_CNPJ ?? 0d,
                DataCarregamento = carga.DataCarregamentoCarga,
                DataCriacaoCarga = configuracao.ValidarTabelaFreteComDataAtual ? DateTime.Now : carga.DataCriacaoCarga,
                LocalFreeTime = localFreeTime,
                PagamentoTerceiro = pagamentoTerceiro,
                RetornoCarga = retornoCarga,
                Pallets = quantidadePallets,
                CodigoFronteira = fronteira,
                CodigosReboque = carga.VeiculosVinculados != null ? (from obj in carga.VeiculosVinculados select obj.Codigo).ToList() : new List<int>(),
                CodigoContratoFreteCliente = codigoContratoFreteCliente
            };

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.Codigo;
            else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo = carga.Veiculo.ModeloVeicularCarga.Codigo;

            if (empresa != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorEmpresa(empresa.Codigo, pagamentoTerceiro, tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }

            if (tabelasFreteValidas.Count <= 0 && codigoFilial > 0)
            {
                tabelasFrete = repTabelaFrete.BuscarPorFilial(codigoFilial, pagamentoTerceiro, tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);

            }

            if (tabelasFreteValidas.Count <= 0 && carga.TipoOperacao != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorTipoOperacao(carga.TipoOperacao.Codigo, pagamentoTerceiro, tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }

            if (tabelasFreteValidas.Count <= 0 && tomador?.GrupoPessoas != null)
            {
                tabelasFrete = repTabelaFrete.BuscarPorGrupoPessoas(tomador.GrupoPessoas.Codigo, pagamentoTerceiro, tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }

            if (tabelasFreteValidas.Count <= 0 && codigoCanalEntrega > 0)
            {
                tabelasFrete = repTabelaFrete.BuscarPorCanalEntrega(canaisEntrega[0].Codigo, pagamentoTerceiro, tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }

            if (tabelasFreteValidas.Count <= 0 && pagamentoTerceiro)
            {
                tabelasFrete = repTabelaFrete.BuscarPorTransportadorTerceiro(tabelaFreteMinima, exclusivaCalculoCliente, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }

            if (tabelasFreteValidas.Count <= 0 && pagamentoTerceiro)
            {
                tabelasFrete = repTabelaFrete.BuscarPorTransportadorTerceiro(tabelaFreteMinima, exclusivaCalculoCliente, null, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelasFrete.Count > 0)
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
            }
            if (tabelasFreteValidas.Count <= 0)
            {
                tabelaFrete = repTabelaFrete.BuscarPadrao(pagamentoTerceiro, carga.Terceiro?.CPF_CNPJ, modalidadeTransportador?.TipoTerceiro?.Codigo);
                if (tabelaFrete != null)
                {
                    tabelasFrete.Clear();
                    tabelasFrete.Add(tabelaFrete);
                    tabelasFreteValidas = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
                }
            }

            if (tabelasFreteValidas.Count > 0)
            {
                if (carga.Rota != null && tabelasFreteValidas.Any(obj => obj.RotasFreteEmbarcador != null && obj.RotasFreteEmbarcador.Count > 0))
                    tabelasFreteValidas = tabelasFreteValidas.OrderByDescending(obj => obj.RotasFreteEmbarcador.Any(o => o.RotaFrete.Codigo == carga.Rota.Codigo)).ToList();

                if (configuracao.CompararTabelasDeFreteParaCalculo)
                    tabelasFrete = tabelasFreteValidas;
                else
                {
                    tabelasFrete.Clear();
                    tabelasFrete.Add(tabelasFreteValidas.FirstOrDefault());
                }
            }
            else
            {
                if (!exclusivaCalculoCliente)
                    tabelasFrete.Clear();
                else
                {
                    if (carga.TabelaFrete != null)
                        tabelasFrete.Add(carga.TabelaFrete);
                    else
                        tabelasFrete.Clear();
                }
            }

            //}
            //else
            //{
            //    if (tomador == null)
            //        tomador = cargaPedidoPadrao.ObterTomador();

            //    if (tomador != null && tomador.GrupoPessoas != null)
            //    {
            //        if (codigoFilial > 0)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoaFilial(tomador.GrupoPessoas.Codigo, codigoFilial, carga.TipoOperacao, pagamentoTerceiro, tabelaFreteMinima, false, carga.Terceiro?.CPF_CNPJ , tomador.CPF_CNPJ);

            //        if (tabelaFrete == null)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoa(tomador.GrupoPessoas.Codigo, carga.TipoOperacao, pagamentoTerceiro, tabelaFreteMinima, false, carga.Terceiro?.CPF_CNPJ, tomador.CPF_CNPJ);

            //        if (tabelaFrete == null)
            //            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoaSemTipoOperacao(tomador.GrupoPessoas.Codigo, pagamentoTerceiro, tabelaFreteMinima, false, carga.Terceiro?.CPF_CNPJ, tomador.CPF_CNPJ);
            //    }

            //    if (tabelaFrete == null && carga.TipoOperacao != null)
            //        tabelaFrete = repTabelaFrete.BuscarSemGrupoPessoaETipoOperacao(carga.TipoOperacao.Codigo, pagamentoTerceiro, tabelaFreteMinima, false, carga.Terceiro?.CPF_CNPJ, tomador?.CPF_CNPJ);

            //    if (tabelaFrete == null)
            //        tabelaFrete = repTabelaFrete.BuscarSemGrupoPessoaESemTipoOperacao(pagamentoTerceiro, tabelaFreteMinima, false, carga.Terceiro?.CPF_CNPJ, tomador?.CPF_CNPJ);

            //    if (tabelaFrete == null)
            //    {
            //        if (tomador != null)
            //            mensagem.Append("O tomador do serviço (" + tomador.Nome + ") não possui uma tabela de frete configurada para ele.");
            //        else
            //            mensagem.Append("O tomador do serviço não identificado.");
            //    }
            //    else
            //    {
            //        tabelasFrete.Add(tabelaFrete);
            //    }

            //}

            return tabelasFrete;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete ObterTabelaFreteJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if ((TipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador) && (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiCTe))
                return null;

            Dominio.Entidades.Empresa empresa = cargaJanelaCarregamentoTransportador.Transportador;
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = repositorioTabelaFrete.BuscarPorEmpresa(empresa.Codigo, false, false, false);

            if (tabelasFrete.Count == 0)
                return null;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPadrao = repositorioCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
            Dominio.Entidades.Cliente tomador = cargaPedidoPadrao?.ObterTomador();
            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCargaRetorno(carga.Codigo);

            ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas = new ParametrosRetornarTabelasValidas()
            {
                CodigoEmpresa = empresa?.Codigo ?? 0,
                CodigoFilial = carga.Filial?.Codigo ?? 0,
                CodigoGrupoPessoaTomador = tomador.GrupoPessoas?.Codigo ?? 0,
                CodigoModeloVeicularDaCarga = carga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoRotaFrete = carga.Rota?.Codigo ?? 0,
                CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                CodigoVeiculo = carga.Veiculo?.Codigo ?? 0,
                DataCarregamento = carga.DataCarregamentoCarga,
                DataCriacaoCarga = configuracao.ValidarTabelaFreteComDataAtual ? DateTime.Now : carga.DataCriacaoCarga,
                RetornoCarga = retornoCarga
            };

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.Codigo;
            else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                parametrosRetornarTabelasValidas.CodigoModeloVeicularDoVeiculo = carga.Veiculo.ModeloVeicularCarga.Codigo;

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelaFretes = RetornarTabelasValidas(tabelasFrete, parametrosRetornarTabelasValidas, TipoServicoMultisoftware, unidadeDeTrabalho);

            if (tabelaFretes.Count > 0)
                return tabelaFretes.FirstOrDefault();

            return null;
        }

        public static void CalcularFretePreCargasPendentes(bool controlePorLote, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);

            Servicos.Embarcador.PreCarga.FretePreCarga serFretePreCarga = new PreCarga.FretePreCarga(unitOfWork, tipoServicoMultisoftware);

            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = repPreCarga.BuscarCargasAguardandoCalculoFrete(3);

            for (int i = 0; i < preCargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = preCargas[i];
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serFretePreCarga.ProcessarFrete(ref preCarga, unitOfWork, tipoServicoMultisoftware, configuracao);
                preCarga.CalculandoFrete = false;
                if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                {
                    preCarga.PendenciaCalculoFrete = true;
                    preCarga.MotivoPendencia = retorno.mensagem;
                }
                repPreCarga.Atualizar(preCarga);
            }
        }

        public static void CalcularFreteCargasPendentes(Dominio.Enumeradores.LoteCalculoFrete controleCalculoFretePorLote, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, List<int> cargas = null)
        {

            Servicos.Embarcador.Carga.Frete servicoCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadraoCalculoFrete();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            bool utilizarAlcadaAprovacaoAlteracaoValorFrete = configuracao.UtilizarAlcadaAprovacaoAlteracaoValorFrete;
            int quantidadeRegistros = 5;

            if (cargas == null)
                cargas = repositorioCarga.BuscarCargasAguardandoCalculoFrete(controleCalculoFretePorLote, configuracao.ExigirCargaRoteirizada, quantidadeRegistros,
                    tipoServicoMultisoftware, configuracao.CalcularFreteInicioCarga);

            for (int i = 0; i < cargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoFetch(cargas[i]);

                carga.PosicaoFilaProcessamento++;

                repositorioCarga.Atualizar(carga);

                try
                {
                    if (!carga.CalculandoFrete)
                    {
                        Log.GravarAdvertencia($"Carga não estava na situação para calcular o frete - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");
                        return;
                    }

                    if (!carga.CargaTransbordo || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                    {
                        if (utilizarAlcadaAprovacaoAlteracaoValorFrete && (carga.SituacaoAlteracaoFreteCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.Aprovada))
                        {
                            carga.CalculandoFrete = false;
                            carga.DataInicioCalculoFrete = null;
                            if (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgTransportador && carga.DataInicioGeracaoCTes.HasValue && carga.DataInicioGeracaoCTes.Value > DateTime.MinValue && (carga.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false))
                                carga.SituacaoCarga = SituacaoCarga.AgNFe;
                            repositorioCarga.Atualizar(carga);
                        }
                        else
                        {
                            Log.GravarInfo($"Calculando frete carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");

                            AjustarValoresCargasCargoX(carga, tipoServicoMultisoftware, configuracao, unitOfWork);

                            repositorioCargaPedido.SetarPedidoEncaixeParaNulo(carga.Codigo);

                            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = servicoCargaFrete.RecalcularFreteTabelaFrete(carga, carga.TabelaFreteRota?.Codigo ?? 0, unitOfWork, configuracao, configuracaoPedido);
                            carga = repositorioCarga.BuscarPorCodigo(carga.Codigo);
                            servicoCargaFrete.SetarDadosFreteCarga(retorno, carga, tipoServicoMultisoftware, configuracao, stringConexao, unitOfWork, clienteMultisoftware);
                            if (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgTransportador && carga.DataInicioGeracaoCTes.HasValue && carga.DataInicioGeracaoCTes.Value > DateTime.MinValue && (carga.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false))
                                carga.SituacaoCarga = SituacaoCarga.AgNFe;
                            repositorioCarga.Atualizar(carga);

                            Log.GravarInfo($"Finalizado calculo frete carga  - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");
                        }

                    }
                    else
                    {
                        if (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                            carga.SituacaoCarga = SituacaoCarga.AgTransportador;

                        carga.DataInicioCalculoFrete = null;
                        carga.CalculandoFrete = false;
                        repositorioCarga.Atualizar(carga);
                    }
                }
                catch (ServicoException ex)
                {
                    unitOfWork.Rollback();
                    Log.TratarErro(ex, "CalcularFreteCargasPendentes");
                    gerarErroProcessamentoFrete(carga.Codigo, ex.Message.ToString(), MotivoPendenciaFrete.ProblemaCalculoFrete, unitOfWork);

                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Log.TratarErro(excecao, "CalcularFreteCargasPendentes");
                    gerarErroProcessamentoFrete(carga.Codigo, "Erro ao calcular frete, por favor entrar em contato com suporte técnico", MotivoPendenciaFrete.ProblemaCalculoFrete, unitOfWork);
                }
                finally
                {
                    unitOfWork.FlushAndClear();
                }
            }

        }

        public static void CalcularFreteCargasPendentesReprocessamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Carga.Frete servicoCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                bool utilizarAlcadaAprovacaoAlteracaoValorFrete = configuracao?.UtilizarAlcadaAprovacaoAlteracaoValorFrete ?? false;
                int quantidadeRegistros = 5;

                List<int> cargas = repositorioCarga.BuscarCargasAguardandoCalculoFrete(Dominio.Enumeradores.LoteCalculoFrete.Reprocessamento, configuracao.ExigirCargaRoteirizada, quantidadeRegistros, tipoServicoMultisoftware, configuracao.CalcularFreteInicioCarga);


                for (int i = 0; i < cargas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoFetch(cargas[i]);

                    if (!carga.CargaTransbordo || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                    {

                        if (utilizarAlcadaAprovacaoAlteracaoValorFrete && (carga.SituacaoAlteracaoFreteCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.Aprovada))
                        {
                            carga.CalculandoFrete = false;
                            carga.CalcularFreteLote = null;
                            carga.DataInicioCalculoFrete = null;
                            repositorioCarga.Atualizar(carga);
                        }
                        else
                        {
                            Log.TratarErro($"Calculando frete Reprocessamento carga - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentesReprocessamento");

                            AjustarValoresCargasCargoX(carga, tipoServicoMultisoftware, configuracao, unitOfWork);

                            repositorioCargaPedido.SetarPedidoEncaixeParaNulo(carga.Codigo);

                            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = servicoCargaFrete.RecalcularFreteTabelaFrete(carga, carga.TabelaFreteRota?.Codigo ?? 0, unitOfWork, configuracao, configuracaoPedido);
                            carga = repositorioCarga.BuscarPorCodigoFetch(carga.Codigo);
                            carga.CalcularFreteLote = null;
                            servicoCargaFrete.SetarDadosFreteCarga(retorno, carga, tipoServicoMultisoftware, configuracao, stringConexao, unitOfWork, clienteMultisoftware);

                            Log.TratarErro($"Finalizado calculo frete Reprocessamento carga  - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentesReprocessamento");
                        }

                    }
                    else
                    {
                        carga.DataInicioCalculoFrete = null;
                        carga.CalcularFreteLote = null;
                        carga.CalculandoFrete = false;
                        repositorioCarga.Atualizar(carga);
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "CalcularFreteCargasPendentesReprocessamento");
            }
        }

        private static void AjustarValoresCargasCargoX(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Integracoes != null && carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX))
            {
                var cargaPedido = carga.Pedidos.FirstOrDefault();
                if (cargaPedido.ValorFrete == 0 && cargaPedido.ValorFreteAPagar > 0)
                {
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                    Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(unitOfWork);

                    bool incluirBase = false;
                    decimal percentualIncluir = 100;
                    cargaPedido.BaseCalculoICMS = cargaPedido.ValorFreteAPagar;
                    decimal valorComponentes = repCargaPedidoComponentesFrete.BuscarSomaPorCargaPedido(cargaPedido.Codigo, true);
                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);
                    if (regraICMS.CST == "60")
                        cargaPedido.ValorFrete = cargaPedido.ValorFreteAPagar - valorComponentes;
                    else
                        cargaPedido.ValorFrete = cargaPedido.ValorFreteAPagar - regraICMS.ValorICMS - valorComponentes;

                    if (regraICMS.CodigoRegra > 0)
                        cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);
                    cargaPedido.Pedido.ValorFreteNegociado = cargaPedido.ValorFrete;
                    repPedido.Atualizar(cargaPedido.Pedido);
                }
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosPedidoCarregamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFreteCarregamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                DataVigencia = DateTime.Now,
                Empresa = parametrosCalculoFreteCarregamento.Empresa,
                ModeloVeiculo = parametrosCalculoFreteCarregamento.ModeloVeiculo,
                NumeroPallets = pedido.NumeroPaletes + pedido.NumeroPaletesFracionado,
                Peso = pedido.PesoTotal,
                PesoLiquido = pedido.PesoLiquidoTotal,
                NecessarioAjudante = pedido.Ajudante,
                NumeroEntregas = 1,
                PesoCubado = pedido.PesoCubado,
                PesoPaletizado = pedido.PesoTotalPaletes,
                Quantidades = new List<ParametrosCalculoFreteQuantidade>()
                {
                    new ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = pedido.PesoTotal,
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    }
                },
                TipoCarga = parametrosCalculoFreteCarregamento.TipoCarga,
                TipoOperacao = parametrosCalculoFreteCarregamento.TipoOperacao,
                ValorNotasFiscais = pedido.ValorTotalNotasFiscais,
                Veiculo = parametrosCalculoFreteCarregamento.Veiculo,
                Reboques = parametrosCalculoFreteCarregamento.Reboques,
                Volumes = (from obj in pedidoProdutos where obj.Pedido.Codigo == pedido.Codigo select obj.Quantidade).Sum(),
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                PagamentoTerceiro = false,
                Distancia = parametrosCalculoFreteCarregamento.Distancia,
                Destinatarios = new List<Dominio.Entidades.Cliente>(),
                Destinos = new List<Dominio.Entidades.Localidade>(),
                Remetentes = new List<Dominio.Entidades.Cliente>(),
                Origens = new List<Dominio.Entidades.Localidade>(),
                DataBaseCRT = pedido.DataBaseCRT,
                Rota = parametrosCalculoFreteCarregamento.Rota,
                Cubagem = pedido.CubagemTotal,
                MaiorAlturaProdutoEmCentimetros = pedido.MaiorAlturaProdutoEmCentimetros,
                MaiorLarguraProdutoEmCentimetros = pedido.MaiorLarguraProdutoEmCentimetros,
                MaiorComprimentoProdutoEmCentimetros = pedido.MaiorComprimentoProdutoEmCentimetros,
                MaiorVolumeProdutoEmCentimetros = pedido.MaiorVolumeProdutoEmCentimetros
            };

            if (parametrosCalculo.Cubagem > 0)
                parametrosCalculo.Quantidades.Add(new ParametrosCalculoFreteQuantidade()
                {
                    Quantidade = parametrosCalculo.Cubagem,
                    UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3
                });

            parametrosCalculo.Remetentes.Add(pedido.Remetente);
            parametrosCalculo.Origens.Add(pedido.Origem);
            if (pedido.Destinatario != null)
                parametrosCalculo.Destinatarios.Add(pedido.Destinatario);
            if (pedido.Destino != null)
                parametrosCalculo.Destinos.Add(pedido.Destino);
            return parametrosCalculo;

        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete ObterDadosCalculoFreteMontagemCarga(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(parametros, false, out StringBuilder mensagem, unidadeTrabalho, tipoServicoMultisoftware, 0);

            if (ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];
                if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                {
                    if (tabelaFrete.CalcularQuantidadeEntregaPorNumeroDePedidos)
                        parametros.NumeroEntregas = pedidos.Count;

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametros, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();
                    if (tabelaFreteCliente == null)
                    {
                        dadosCalculoFrete.FreteCalculado = true;
                        dadosCalculoFrete.FreteCalculadoComProblemas = true;
                        dadosCalculoFrete.MensagemRetorno = mensagem.ToString();
                    }
                    else
                    {
                        if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);

                        dadosCalculoFrete.TabelaFreteCliente = tabelaFreteCliente;
                        dadosCalculoFrete.FreteCalculado = true;
                    }
                }
                else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                         tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadoCalculoFretePedidos = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametroPedido = ObterParametrosPedidoCarregamento(pedido, pedidoProdutos, parametros);

                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametroPedido, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();
                        if (tabelaFreteCliente == null)
                        {
                            dadosCalculoFrete.FreteCalculadoComProblemas = true;
                            dadosCalculoFrete.MensagemRetorno = mensagem.ToString();
                            if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                                tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                                break;
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosFretePedido = new DadosCalculoFrete();
                            if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                                svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosFretePedido, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);
                            else
                                svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosFretePedido, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);

                            dadosCalculoFrete.LeadTime += tabelaFreteCliente?.LeadTime ?? 0;
                            dadoCalculoFretePedidos.Add(dadosFretePedido);
                        }
                    }
                    if (dadoCalculoFretePedidos.Count > 0)
                    {
                        decimal valorTotal = 0;
                        if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete pedidoCalculado in dadoCalculoFretePedidos)
                            {
                                decimal valorCalculado = pedidoCalculado.ValorFrete + pedidoCalculado.ValorTotalComponentes;
                                if (valorTotal < valorCalculado)
                                {
                                    valorTotal = valorCalculado;
                                    dadosCalculoFrete = pedidoCalculado;
                                }
                            }
                        }
                        else if (!dadosCalculoFrete.FreteCalculadoComProblemas)
                        {
                            dadosCalculoFrete.Componentes = new List<DadosCalculoFreteComponente>();
                            dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete pedidoCalculado in dadoCalculoFretePedidos)
                            {
                                dadosCalculoFrete.ComposicaoFrete.AddRange(pedidoCalculado.ComposicaoFrete);
                                dadosCalculoFrete.Componentes.AddRange(pedidoCalculado.Componentes);
                                dadosCalculoFrete.ValorFrete += pedidoCalculado.ValorFrete;
                                dadosCalculoFrete.ValorFixo += pedidoCalculado.ValorFixo;
                            }
                        }
                    }
                    dadosCalculoFrete.FreteCalculado = true;
                }
                else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados
                    || tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadoCalculoFretePedidos = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametros, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();
                    if (tabelaFreteCliente == null)
                    {
                        dadosCalculoFrete.FreteCalculadoComProblemas = true;
                        dadosCalculoFrete.MensagemRetorno = mensagem.ToString();
                        return dadosCalculoFrete;
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosFretePedido = new DadosCalculoFrete();
                        if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosFretePedido, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosFretePedido, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);

                        dadoCalculoFretePedidos.Add(dadosFretePedido);
                    }
                    if (dadoCalculoFretePedidos.Count > 0)
                    {
                        if (!dadosCalculoFrete.FreteCalculadoComProblemas)
                        {
                            dadosCalculoFrete.LeadTime = tabelaFreteCliente?.LeadTime ?? 0;
                            dadosCalculoFrete.Componentes = new List<DadosCalculoFreteComponente>();
                            dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete pedidoCalculado in dadoCalculoFretePedidos)
                            {
                                dadosCalculoFrete.ComposicaoFrete.AddRange(pedidoCalculado.ComposicaoFrete);
                                dadosCalculoFrete.Componentes.AddRange(pedidoCalculado.Componentes);
                                dadosCalculoFrete.ValorFrete += pedidoCalculado.ValorFrete;
                                dadosCalculoFrete.ValorFixo += pedidoCalculado.ValorFixo;
                            }
                        }
                    }

                    dadosCalculoFrete.TabelaFreteCliente = tabelaFreteCliente;
                    dadosCalculoFrete.FreteCalculado = true;
                }
            }
            return dadosCalculoFrete;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete ObterDadosCalculoFrete(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new DadosCalculoFrete();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(parametros, false, out StringBuilder mensagem, unidadeTrabalho, tipoServicoMultisoftware, 0);

            if (parametros.CalcularVariacoes)
            {
                dadosCalculoFrete.Variacoes = new List<DadosCalculoFrete>() { };

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete in tabelasFrete)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete variacaoCalculo = new DadosCalculoFrete();
                    ObterDadosTabelaFrete(parametros, tipoServicoMultisoftware, unidadeTrabalho, configuracaoTMS, svcFreteCliente, ref variacaoCalculo, tabelaFrete, ref mensagem);
                    dadosCalculoFrete.Variacoes.Add(variacaoCalculo);
                }

                dadosCalculoFrete.FreteCalculado = dadosCalculoFrete.Variacoes.Exists(variacao => variacao.FreteCalculado);
            }
            else if (ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
            {
                ObterDadosTabelaFrete(parametros, tipoServicoMultisoftware, unidadeTrabalho, configuracaoTMS, svcFreteCliente, ref dadosCalculoFrete, tabelasFrete[0], ref mensagem);
            }

            return dadosCalculoFrete;
        }

        private static void ObterDadosTabelaFrete(ParametrosCalculoFrete parametros, TipoServicoMultisoftware tipoServicoMultisoftware, UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, FreteCliente svcFreteCliente, ref DadosCalculoFrete dadosCalculoFrete, TabelaFrete tabelaFrete, ref StringBuilder mensagem)
        {
            dadosCalculoFrete.TabelaFrete = tabelaFrete;
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametros, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();

            if (tabelaFreteCliente == null)
            {
                dadosCalculoFrete.FreteCalculado = true;
                dadosCalculoFrete.FreteCalculadoComProblemas = true;
                dadosCalculoFrete.MensagemRetorno = mensagem.ToString();

                if (string.IsNullOrWhiteSpace(dadosCalculoFrete.MensagemRetorno))
                    dadosCalculoFrete.MensagemRetorno = "Nenhuma tabela de valores encontrada para os parâmetros.";
            }
            else
            {
                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);
                else
                    svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);

                dadosCalculoFrete.FreteCalculado = true;
                dadosCalculoFrete.TabelaFreteCliente = tabelaFreteCliente;
            }
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> ObterDadosCalculosFrete(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> lista = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(parametros, false, out StringBuilder mensagem, unidadeTrabalho, tipoServicoMultisoftware, 0, false);

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete in tabelasFrete)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                dadosCalculoFrete.TabelaFrete = tabelaFrete;
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametros, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();

                if (tabelaFreteCliente == null)
                {
                    dadosCalculoFrete.FreteCalculado = true;
                    dadosCalculoFrete.FreteCalculadoComProblemas = true;
                    dadosCalculoFrete.MensagemRetorno = mensagem.ToString();
                }
                else
                {
                    if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                        svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);
                    else
                        svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracaoTMS);

                    dadosCalculoFrete.FreteCalculado = true;
                    dadosCalculoFrete.TabelaFreteCliente = tabelaFreteCliente;
                }
                lista.Add(dadosCalculoFrete);
            }

            return lista;
        }

        public bool ExecutarPreCalculo(ref Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo cargaPreCalculo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPreCalculo repositorioCargaPreCalculo = new Repositorio.Embarcador.Cargas.CargaPreCalculo(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            if (cargaPreCalculo == null)
                cargaPreCalculo = new Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo();

            RetornoDadosFrete retorno = this.VerificarFrete(ref carga, unitOfWork, configuracaoPedido, true);

            cargaPreCalculo.Carga = carga;
            cargaPreCalculo.Observacao = Utilidades.String.Left(retorno.mensagem, 150);
            cargaPreCalculo.Situacao = retorno.situacao;
            cargaPreCalculo.ValorTotal = retorno.valorFrete;

            if (cargaPreCalculo.Codigo > 0)
                repositorioCargaPreCalculo.Atualizar(cargaPreCalculo);
            else
                repositorioCargaPreCalculo.Inserir(cargaPreCalculo);

            return retorno.situacao != SituacaoRetornoDadosFrete.FreteValido ? false : true;
        }

        public void ProcessarRegraInclusaoICMSComponenteFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracaoTMS.GerarComponentesDeFreteComImpostoIncluso)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Frete.RegrasInclusaoICMS repositorioRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS(unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaPedidos.FirstOrDefault().ObterTomador();
            if (tomador == null)
                return;

            Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS = repositorioRegrasInclusaoICMS.BuscarRegraParaCarga(tomador.CPF_CNPJ, tomador.GrupoPessoas?.Codigo ?? 0, carga.TipoOperacao?.Codigo ?? 0);

            carga.PossuiComponenteFreteComImpostoIncluso = regrasInclusaoICMS != null;
            carga.RegraInclusaoICMS = regrasInclusaoICMS;

            repositorioCarga.Atualizar(carga);
        }

        public void CalcularValorFreteComICMSIncluso(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!carga.PossuiComponenteFreteComImpostoIncluso)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            repCargaPedido.AjustarValoresCargaComImpostoIncluso(carga.Codigo);
        }

        public decimal CalcularPercentualFreteSobreNotaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            decimal valorFrete = 0;

            if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                valorFrete = carga.ValorFreteEmbarcador;
            else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                valorFrete = carga.ValorFreteOperador;
            else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Leilao)
                valorFrete = carga.ValorFreteLeilao;

            if (valorFrete <= 0 || carga.DadosSumarizados.ValorTotalMercadoriaPedidos <= 0)
                return 0;

            return ((valorFrete / carga.DadosSumarizados.ValorTotalMercadoriaPedidos) * 100);
        }

        public void SumarizarValoresFreteSubTrechosPreChecking(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repconfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);

            Servicos.Embarcador.Carga.RateioFrete serCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete svcComponenteFrete = new ComponetesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamentoStageCarga = repStageAgrupamento.BuscarPrimeiroPorCargaGerada(carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repconfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            if (agrupamentoStageCarga == null)
                return;

            agrupamentoStageCarga.ValorFreteTotal = retorno.valorFrete;
            agrupamentoStageCarga.RetornoProcessamento = "";
            repStageAgrupamento.Atualizar(agrupamentoStageCarga);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaPai = agrupamentoStageCarga.CargaDT;

            if (cargaPai != null && cargaPai.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoCarga = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<int> codigosPedido = cargaPedidoCarga.Select(x => x.Pedido.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentosDemaisCargas = repStageAgrupamento.BuscarPorCargaDt(cargaPai.Codigo);
                List<int> codigosCargaGerada = agrupamentosDemaisCargas.Where(x => x.CargaGerada != null).Select(x => x.CargaGerada.Codigo).ToList();

                //aqui devemos ratear os valores da carga de transferencia, e cargas de entrega antes de sumarizar valores da pai
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTransferenciaEEntrega = agrupamentosDemaisCargas.Where(x => x.CargaGerada?.ExigeNotaFiscalParaCalcularFrete == false && x.CargaGerada?.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga).Select(x => x.CargaGerada).ToList();

                foreach (var cargaGerada in cargasTransferenciaEEntrega)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaGerada = repCargaPedido.BuscarPorCarga(cargaGerada.Codigo);
                    serCargaRateio.RatearValorDoFrenteEntrePedidos(cargaGerada, cargaPedidosCargaGerada, configuracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                }

                cargaPai.ValorFrete = agrupamentosDemaisCargas.Sum(x => x.CargaGerada.ValorFrete);
                cargaPai.ValorFreteLiquido = agrupamentosDemaisCargas.Sum(x => x.CargaGerada.ValorFreteLiquido);
                cargaPai.ValorFreteAPagar = agrupamentosDemaisCargas.Sum(x => x.CargaGerada.ValorFreteAPagar);

                svcComponenteFrete.RemoverComponentesCarga(cargaPai, unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoCargaPai = repCargaPedido.BuscarPorCargaEPedidos(cargaPai.Codigo, codigosPedido);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPai in cargaPedidoCargaPai)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDoPedido = repCargaPedido.BuscarPorCargasEPedido(codigosCargaGerada, cargaPedidoPai.Pedido.Codigo);

                    cargaPedidoPai.ValorFrete = cargaPedidosDoPedido.Where(x => x.Codigo != cargaPedidoPai.Codigo).Sum(x => x.ValorFrete); //somando valores dos fretes para o pedido das carga pedidos das filhos
                    cargaPedidoPai.ValorBaseFrete = cargaPedidosDoPedido.Where(x => x.Codigo != cargaPedidoPai.Codigo).Sum(x => x.ValorBaseFrete); //somando valores dos fretes para o pedido das carga pedidos das filhos
                    cargaPedidoPai.ValorFreteAPagar = cargaPedidosDoPedido.Where(x => x.Codigo != cargaPedidoPai.Codigo).Sum(x => x.ValorFreteAPagar); //somando valores dos fretes para o pedido das carga pedidos das filhos
                    cargaPedidoPai.Peso = cargaPedidosDoPedido.Where(x => x.Codigo != cargaPedidoPai.Codigo && x.Peso > 0).Select(x => x.Peso).FirstOrDefault(); //valores do peso
                    cargaPedidoPai.PesoLiquido = cargaPedidosDoPedido.Where(x => x.Codigo != cargaPedidoPai.Codigo && x.PesoLiquido > 0).Select(x => x.PesoLiquido).FirstOrDefault(); // valores do peso liquido

                    //criar/validar os componentes do frete do cargaPedidoFilho para o cargaPedidoPai
                    RecriarComponentesFreteCargaPedidoPreCheking(cargaPedidoPai, cargaPedidosDoPedido, unitOfWork);
                }

                RecriarComponentesFreteCargaPreCheking(cargaPai, agrupamentosDemaisCargas.Select(x => x.CargaGerada.Codigo).ToList(), unitOfWork);

                //quando todos estao calculados calcular imposto da carga mae
                bool NaoPodeCalcularImpostoCargaMae = agrupamentosDemaisCargas.Any(x => x.CargaGerada == null || x.CargaGerada.ValorFrete <= 0);

                if (!NaoPodeCalcularImpostoCargaMae)
                {
                    serCargaRateio.ZerarValoresDaCarga(cargaPai, false, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigemMae = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(cargaPai, unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosAgrupados = repCargaPedido.BuscarPorCarga(cargaPai.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(cargaPai.Codigo, false);
                    List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in pedidosAgrupados select obj.Origem.Estado.Sigla).Distinct().ToList());
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(cargaPai, unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
                    List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in pedidosAgrupados where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(cargaPai.Codigo);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                    serCargaRateio.CalcularImpostosAgrupados(ref cargaPai, cargasOrigemMae, pedidosAgrupados, false, TipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNotas, configuracaoTabelaFrete, configuracaoGeralCarga);
                }

                //if (!repStageAgrupamento.ExisteAgrupamentoComFreteCalculado(cargaPai.Codigo))
                //{
                //    Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao existeIntegracaoCargaPai = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaPai.Codigo, TipoIntegracao.Unilever);
                //    if (existeIntegracaoCargaPai != null && existeIntegracaoCargaPai.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                //    {
                //        existeIntegracaoCargaPai.ProblemaIntegracao = "";
                //        existeIntegracaoCargaPai.NumeroTentativas = 0;
                //        existeIntegracaoCargaPai.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                //        repCargaDadosTransporteIntegracao.Atualizar(existeIntegracaoCargaPai);
                //    }
                //}

                repCarga.Atualizar(cargaPai);

            }

        }

        public void SolicitarRecalculoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool recalculoComAjusteManual = false, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, configuracaoTMS);

            if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada && servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete())
                throw new ServicoException("O valor de frete está aprovado e não pode mais ser alterado.");

            if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                throw new ServicoException("A carga foi agrupada, sendo assim não é possível alterá-la.");

            if (!carga.ExigeNotaFiscalParaCalcularFrete && servicoCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                throw new ServicoException("A carga já recebeu o número de carga do Embarcador e não permite essa alteração.");

            string retornoVerificarOperador = servicoCarga.VerificarOperadorPodeConfigurarCarga(usuario, carga, tipoServicoMultisoftware);
            if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                throw new ServicoException(retornoVerificarOperador);

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                throw new ServicoException("Não é possível recalcular o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

            bool podeCalcular = carga.SituacaoCarga == SituacaoCarga.CalculoFrete || (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgTransportador);
            if (!podeCalcular)
                throw new ServicoException("Não é possível recalcular o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

            if (carga.CalculandoFrete)
                throw new ServicoException("Os valores do frete já estão sendo calculados.");

            carga.DadosPagamentoInformadosManualmente = false;
            carga.DataInicioCalculoFrete = DateTime.Now;
            carga.CalculandoFrete = true;
            carga.PendenciaEmissaoAutomatica = false;
            carga.TipoFreteEscolhido = TipoFreteEscolhido.Tabela;
            carga.ConfirmouConferenciaManualDeFrete = false;

            if (!recalculoComAjusteManual && carga.CargaOrigemPedidos.FirstOrDefault(o => (o.FormulaRateio?.ExigirConferenciaManual ?? false) == true) != null)
                repPedidoXMLNotaFiscal.UpdatePorCargaLimparAjusteConferenciaDeFrete(carga.Codigo);

            if (carga.FixarUtilizacaoContratoTransportador)
            {
                carga.FixarUtilizacaoContratoTransportador = false;
                carga.TabelaFrete = null;

                Servicos.Log.TratarErro($"Removeu o contrato {carga.ContratoFreteTransportador?.Codigo ?? 0} da Carga '{carga.Codigo}' Frete.SolicitarRecalculoFrete", "RemoverContratoFreteTransportador");
                carga.ContratoFreteTransportador = null;
                repContratoSaldoMes.DeletarPorCarga(carga.Codigo);
            }

            servicoCargaAprovacaoFrete.RemoverAprovacao(carga);

            if (configuracaoTMS.CalcularFreteCargaJanelaCarregamentoTransportador && carga.Empresa == null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargaJanelaCarregamentoTransportadores = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargaJanelaCarregamentoTransportadores)
                {
                    cargaJanelaCarregamentoTransportador.PendenteCalcularFrete = true;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                }
            }

            repositorioCarga.Atualizar(carga);

            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Solicitou o recálculo do frete.", unitOfWork);
        }

        public void SolicitarRecalculoFreteBID(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(_unitOfWork, configuracaoTMS);
            Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga);

            if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada && servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete())
                throw new ServicoException("O valor de frete está aprovado e não pode mais ser alterado.");

            if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                throw new ServicoException("A carga foi agrupada, sendo assim não é possível alterá-la.");

            if (!carga.ExigeNotaFiscalParaCalcularFrete && servicoCarga.RecebeuNumeroCargaEmbarcador(carga, _unitOfWork))
                throw new ServicoException("A carga já recebeu o número de carga do Embarcador e não permite essa alteração.");

            string retornoVerificarOperador = servicoCarga.VerificarOperadorPodeConfigurarCarga(usuario, carga, tipoServicoMultisoftware);
            if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                throw new ServicoException(retornoVerificarOperador);

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                throw new ServicoException("Não é possível recalcular o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

            bool podeCalcular = carga.SituacaoCarga == SituacaoCarga.CalculoFrete || (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgTransportador);
            if (!podeCalcular)
                throw new ServicoException("Não é possível recalcular o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

            if (carga.CalculandoFrete)
                throw new ServicoException("Os valores do frete já estão sendo calculados.");

            carga.DadosPagamentoInformadosManualmente = false;
            carga.DataInicioCalculoFrete = DateTime.Now;
            carga.CalculandoFrete = true;
            carga.PendenciaEmissaoAutomatica = false;
            carga.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;

            if (carga.FixarUtilizacaoContratoTransportador)
            {
                carga.FixarUtilizacaoContratoTransportador = false;
                carga.TabelaFrete = null;

                Servicos.Log.TratarErro($"Removeu o contrato {carga.ContratoFreteTransportador?.Codigo ?? 0} da Carga '{carga.Codigo}' Frete.SolicitarRecalculoFrete", "RemoverContratoFreteTransportador");
                carga.ContratoFreteTransportador = null;
                repositorioContratoSaldoMes.DeletarPorCarga(carga.Codigo);
            }

            servicoCargaAprovacaoFrete.RemoverAprovacao(carga);

            servicoPreCarga.CalculoFreteCotacaoPedido(carga, cargaPedidos, configuracaoTMS, TipoServicoMultisoftware, _unitOfWork, true);

            repositorioCarga.Atualizar(carga);

            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Solicitou o recálculo do frete pelo BID.", _unitOfWork);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorAgrupamentoStages(List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagePedido, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos, Dominio.Entidades.Embarcador.Cargas.Carga cargaDT)
        {
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega = listaStagePedido.Select(x => x.Stage?.CanalEntrega)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda = listaStagePedido.Select(x => x.Stage?.CanalVenda)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularStage = listaStagePedido.Select(x => x.Stage?.ModeloVeicularCarga)?.FirstOrDefault() ?? null;
            decimal distancia = listaStagePedido.Select(x => x.Stage.Distancia).Sum();


            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            foreach (var stageVeiculo in listaStagePedido)
            {
                if (stageVeiculo.Stage.TipoPercurso == Vazio.PercursoRegreso)
                    continue;

                if (stageVeiculo.Stage.Recebedor != null)
                    destinatarios.Add(stageVeiculo.Stage.Recebedor);
                else
                {
                    if (stageVeiculo.Pedido.Recebedor != null)
                        destinatarios.Add(stageVeiculo.Pedido.Recebedor);
                    else if (stageVeiculo.Pedido.Recebedor == null && stageVeiculo.Pedido.Destinatario != null)
                        destinatarios.Add(stageVeiculo.Pedido.Destinatario);
                }

                if (stageVeiculo.Stage.Expedidor != null)
                {
                    remetentes.Add(stageVeiculo.Stage.Expedidor);

                    if (cargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                        origens.Add(stageVeiculo.Stage.Expedidor.Localidade);
                    else
                    {
                        if (stageVeiculo.Pedido.EnderecoOrigem != null && stageVeiculo.Pedido.EnderecoOrigem.Localidade != stageVeiculo.Stage.Expedidor.Localidade)
                            origens.Add(stageVeiculo.Pedido.EnderecoOrigem.Localidade);
                        else
                            origens.Add(stageVeiculo.Stage.Expedidor.Localidade);
                    }

                }
                else
                {
                    remetentes.Add(stageVeiculo.Pedido.Remetente);

                    if (stageVeiculo.Pedido.EnderecoOrigem != null && stageVeiculo.Pedido.EnderecoOrigem.Localidade != stageVeiculo.Pedido.Remetente.Localidade)
                        origens.Add(stageVeiculo.Pedido.EnderecoOrigem.Localidade);
                    else
                        origens.Add(stageVeiculo.Pedido.Remetente.Localidade);
                }

                if (stageVeiculo.Pedido.RegiaoDestino != null)
                    regioesDestino.Add(stageVeiculo.Pedido.RegiaoDestino);

            }

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoUsadaCalculo = cargaDT.TipoOperacao;

            if (listaStagePedido.Any(x => x.Stage.TipoPercurso == Vazio.PercursoPreliminar) && cargaDT.TipoOperacao?.TipoOperacaoPrecheckin != null)
                TipoOperacaoUsadaCalculo = cargaDT.TipoOperacao?.TipoOperacaoPrecheckin;

            if (listaStagePedido.Any(x => x.Stage.TipoPercurso == Vazio.PercursoPrincipal || x.Stage.TipoPercurso == Vazio.PercursoSubSeQuente) && cargaDT.TipoOperacao?.TipoOperacaoPrecheckinTransferencia != null)
                TipoOperacaoUsadaCalculo = cargaDT.TipoOperacao?.TipoOperacaoPrecheckinTransferencia;

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
            {
                DataVigencia = DateTime.Now,
                Empresa = cargaDT.Empresa,
                ModeloVeiculo = modeloVeicularStage == null ? cargaDT.ModeloVeicularCarga : modeloVeicularStage,
                NumeroPallets = pedidos.Sum(o => o.NumeroPaletes + o.NumeroPaletesFracionado),
                Peso = pedidos.Sum(o => o.PesoTotal),
                PesoLiquido = pedidos.Sum(o => o.PesoLiquidoTotal),
                NumeroEntregas = destinatarios.Distinct().Count(),
                PesoCubado = pedidos.Sum(o => o.PesoCubado),
                PesoPaletizado = pedidos.Sum(o => o.PesoTotalPaletes),
                PesoTotalCarga = cargaDT?.DadosSumarizados?.PesoTotal ?? 0m,
                Quantidades = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = pedidos.Sum(o => o.PesoTotal),
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    }
                },
                TipoCarga = cargaDT.TipoDeCarga,
                TipoOperacao = TipoOperacaoUsadaCalculo,
                ValorNotasFiscais = pedidos.Sum(o => o.ValorTotalNotasFiscais),
                Volumes = pedidoProdutos.Sum(p => p.Quantidade),
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                PagamentoTerceiro = false,
                Distancia = distancia,
                Destinatarios = destinatarios.Distinct().ToList(),
                Destinos = destinatarios.Select(o => o.Localidade).Distinct().ToList(),
                Remetentes = remetentes.Distinct().ToList(),
                Origens = origens.Count > 0 ? origens.Distinct().ToList() : remetentes.Select(o => o.Localidade).Distinct().ToList(),
                DataBaseCRT = pedidos.Select(o => o.DataBaseCRT).FirstOrDefault(),
                Cubagem = pedidos.Sum(o => o.CubagemTotal),
                MaiorAlturaProdutoEmCentimetros = pedidos.Max(o => o.MaiorAlturaProdutoEmCentimetros),
                MaiorLarguraProdutoEmCentimetros = pedidos.Max(o => o.MaiorLarguraProdutoEmCentimetros),
                MaiorComprimentoProdutoEmCentimetros = pedidos.Max(o => o.MaiorComprimentoProdutoEmCentimetros),
                MaiorVolumeProdutoEmCentimetros = pedidos.Max(o => o.MaiorVolumeProdutoEmCentimetros),
                Filial = cargaDT.Filial,
                CanalEntrega = CanalEntrega,
                CanalVenda = CanalVenda,
                RegioesDestino = regioesDestino?.Distinct().ToList() ?? new List<Dominio.Entidades.Embarcador.Localidades.Regiao>(),
                CargaPerigosa = cargaDT?.CargaPerigosaIntegracaoLeilao ?? false,
            };

            if (parametrosCalculo.Cubagem > 0)
                parametrosCalculo.Quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
                {
                    Quantidade = parametrosCalculo.Cubagem,
                    UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3
                });

            return parametrosCalculo;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete AjustarValorFreteContratoFreteCargaTP(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagePedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repositorioPedidoProduto.BuscarPorPedidos(pedidos.Select(x => x.Codigo).ToList());
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros = ObterParametrosCalculoFretePorAgrupamentoStages(listaStagePedido, pedidos, pedidoProdutos, carga);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(parametros, false, out StringBuilder mensagem, _unitOfWork, tipoServicoMultisoftware, 0);

            if (ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
            {
                dadosCalculoFrete.FreteCalculado = true;
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = tabelaFrete.ContratoFreteTransportador;

                if ((carga?.TipoOperacao?.PermiteUtilizarEmContratoFrete ?? false) && contratoFreteTransportador == null)
                    contratoFreteTransportador = ObterContratoCombativel(tabelaFrete, carga, _unitOfWork);

                if (
                contratoFreteTransportador != null &&
                contratoFreteTransportador.Ativo &&
                    (
                        (contratoFreteTransportador.FranquiaValorKM > 0) ||
                        (contratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui && contratoFreteTransportador.DeduzirValorPorCarga) ||
                        (contratoFreteTransportador.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) ||
                        (configuracaoTMS.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
                    )
                )
                {

                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametrosContratoFreteTransportadorValorFreteMinimo = ObterParametrosContratoFreteTransportadorValorFreteMinimo(contratoFreteTransportador, parametros);
                    string retornoContrato = Servicos.Embarcador.Carga.ContratoFrete.CalcularFretePorContratoFrete(contratoFreteTransportador, parametrosContratoFreteTransportadorValorFreteMinimo, carga, cargaPedidos, configuracaoTMS, false, false, TipoServicoMultisoftware, _unitOfWork, true);
                    if (!string.IsNullOrWhiteSpace(retornoContrato))
                    {
                        dadosCalculoFrete.FreteCalculadoComProblemas = true;
                        dadosCalculoFrete.MensagemRetorno = retornoContrato;
                        carga.PossuiPendencia = true;

                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;
                            if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete AgrupamentoStages Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        carga.MotivoPendencia = dadosCalculoFrete.MensagemRetorno.Length < 2000 ? dadosCalculoFrete.MensagemRetorno : dadosCalculoFrete.MensagemRetorno.Substring(0, 1999);
                        carga.TabelaFrete = null;

                        repCarga.Atualizar(carga);
                    }
                }

            }

            return dadosCalculoFrete;
        }

        public void SumarizarComponentesFreteCargaTP(List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentos)
        {
            if (stagesAgrupamentos?.Count == 0)
                return;

            Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete repositorioStageAgrupamentoComponente = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> stageAgrupamentoComponenteFretes = repositorioStageAgrupamentoComponente.BuscarPorCodigosAgrupamentos(stagesAgrupamentos.Select(x => x.Codigo).ToList());

            if (stageAgrupamentoComponenteFretes.Count == 0)
                return;

            List<IGrouping<Dominio.Entidades.Embarcador.Frete.ComponenteFrete, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete>> componentes = stageAgrupamentoComponenteFretes.GroupBy(x => x.ComponenteFrete).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCarga = repositorioCargaComponentesFrete.BuscarPorCarga(stagesAgrupamentos.FirstOrDefault().CargaDT.Codigo);

            foreach (IGrouping<Dominio.Entidades.Embarcador.Frete.ComponenteFrete, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> componenteAgrupado in componentes)
            {
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete primeiroComponente = componenteAgrupado.FirstOrDefault();

                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente = componentesCarga.FirstOrDefault(x => x.ComponenteFrete == primeiroComponente.ComponenteFrete);

                if (componente == null)
                {
                    componente = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete()
                    {
                        AcrescentaValorTotalAReceber = primeiroComponente.AcrescentaValorTotalAReceber,
                        Carga = primeiroComponente.StageAgrupamento.CargaDT,
                        DescontarComponenteFreteLiquido = primeiroComponente.DescontarComponenteFreteLiquido,
                        DescontarValorTotalAReceber = primeiroComponente.DescontarValorTotalAReceber,
                        IncluirBaseCalculoICMS = primeiroComponente.IncluirBaseCalculoICMS,
                        IncluirIntegralmenteContratoFreteTerceiro = primeiroComponente.IncluirIntegralmenteContratoFreteTerceiro,
                        NaoSomarValorTotalAReceber = primeiroComponente.NaoSomarValorTotalAReceber,
                        NaoSomarValorTotalPrestacao = primeiroComponente.NaoSomarValorTotalPrestacao,
                        Percentual = primeiroComponente.Percentual,
                        TipoValor = primeiroComponente.TipoValor,
                        TipoComponenteFrete = primeiroComponente.TipoComponenteFrete,
                        ComponenteFrete = primeiroComponente.ComponenteFrete
                    };

                }

                componente.ValorComponente += componenteAgrupado.Sum(x => x.ValorComponente);

                if (componente.Codigo > 0)
                    repositorioCargaComponentesFrete.Atualizar(componente);
                else
                    repositorioCargaComponentesFrete.Inserir(componente);
            }

        }

        public void CriarComponentesFreteCargaPorStageAgrupamento(Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete repositorioStageAgrupamentoComponente = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> stageAgrupamentoComponenteFretes = repositorioStageAgrupamentoComponente.BuscarPorAgrupamento(stageAgrupamento.Codigo);

            if (stageAgrupamentoComponenteFretes?.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete stageAgrupamentoComponente in stageAgrupamentoComponenteFretes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete()
                {
                    AcrescentaValorTotalAReceber = stageAgrupamentoComponente.AcrescentaValorTotalAReceber,
                    Carga = carga,
                    DescontarComponenteFreteLiquido = stageAgrupamentoComponente.DescontarComponenteFreteLiquido,
                    DescontarValorTotalAReceber = stageAgrupamentoComponente.DescontarValorTotalAReceber,
                    IncluirBaseCalculoICMS = stageAgrupamentoComponente.IncluirBaseCalculoICMS,
                    IncluirIntegralmenteContratoFreteTerceiro = stageAgrupamentoComponente.IncluirIntegralmenteContratoFreteTerceiro,
                    NaoSomarValorTotalAReceber = stageAgrupamentoComponente.NaoSomarValorTotalAReceber,
                    NaoSomarValorTotalPrestacao = stageAgrupamentoComponente.NaoSomarValorTotalPrestacao,
                    Percentual = stageAgrupamentoComponente.Percentual,
                    TipoValor = stageAgrupamentoComponente.TipoValor,
                    TipoComponenteFrete = stageAgrupamentoComponente.TipoComponenteFrete,
                    ComponenteFrete = stageAgrupamentoComponente.ComponenteFrete,
                    ValorComponente = stageAgrupamentoComponente.ValorComponente
                };

                repositorioCargaComponentesFrete.Inserir(componente);
            }
        }

        #endregion

        #region Métodos Privados

        private decimal ObterPesoCubado(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidosNotasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNf = repositorioCarregamentoPedidoNotaFiscal.BuscarPorPedidoECarregamento(cargaPedido.Pedido.Codigo, (carregamento?.Codigo ?? 0));

            if (carregamentoPedidoNf == null)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCarga in cargaPedido.Carga.Pedidos)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in cargaPedidoCarga.NotasFiscais)
                    {
                        notasFiscais.Add(pedidoXmlNotaFiscal.XMLNotaFiscal);
                    }
                }

                notasFiscais = notasFiscais.Distinct().ToList();

                return notasFiscais?.Sum(obj => obj.PesoCubado) ?? 0m;
            }

            return carregamentoPedidoNf.NotasFiscais.Sum(x => x.PesoCubado);
        }

        //private decimal ObterPesoCubado(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repositorioCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

        //    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaCarregamentosPedidoNf = repositorioCarregamentoPedidoNotaFiscal.BuscarPorPedidoECarregamento((carregamento?.Codigo ?? 0), cargasPedido.Select(x => x.Pedido.Codigo).ToList());

        //    if (listaCarregamentosPedidoNf.Count == 0)
        //    {
        //        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
        //        {
        //            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in cargaPedido.NotasFiscais)
        //            {
        //                notasFiscais.Add(pedidoXmlNotaFiscal.XMLNotaFiscal);
        //            }
        //        }

        //        notasFiscais = notasFiscais.Distinct().ToList();

        //        return notasFiscais?.Sum(obj => obj.PesoCubado) ?? 0m;
        //    }

        //    decimal pesoCubado = 0m;

        //    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoNf in listaCarregamentosPedidoNf)
        //        pesoCubado += carregamentoNf.NotasFiscais.Sum(x => x.PesoCubado);

        //    return pesoCubado;
        //}

        private void NotificarAlteracaoCargaAoOperador(Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            if (configuracaoTMS.NotificarAlteracaoCargaAoOperador && (retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
                new Carga(unitOfWork).NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.NaoFoiPossivelCalcularFreteCarga, carga.CodigoCargaEmbarcador), unitOfWork);
        }

        private static bool ValidarSaldoContratoPrestacaoServico(Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Embarcador.Frete.ContratoPrestacaoServicoSaldo servicoSaldo = new Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork);

            if (configuracao.UtilizarContratoPrestacaoServico)
            {
                ContratoPrestacaoServicoSaldoDados dados = new ContratoPrestacaoServicoSaldoDados()
                {
                    CodigoFilial = carga.Filial?.Codigo ?? 0,
                    CodigoTransportador = carga.Empresa?.Codigo ?? 0,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoContratoPrestacaoServico.Saida,
                    Valor = carga.ValorFrete + carga.ValorFreteContratoFreteTotal
                };

                if (!servicoSaldo.IsPossuiSaldoDisponivel(dados))
                {
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    retorno.mensagem = "Não existe contrato de prestação de serviço com saldo disponível";

                    carga.PossuiPendencia = true;
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                    if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                    carga.MotivoPendencia = "Não existe contrato de prestação de serviço com saldo disponível";

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 28 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                    return false;
                }
            }

            return true;
        }

        private static bool ValidarVeiculoVinculadoContratoFrete(Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if ((configuracaoTMS?.ValidarVeiculoVinculadoContratoDeFrete ?? false) && (carga.Veiculo != null))
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repositorioContratoFreteTransportador.BuscarContratosPorVeiculo(DateTime.Now, carga.Veiculo.Codigo);

                if (contratoFreteTransportador != null && (carga.ContratoFreteTransportador?.Codigo != contratoFreteTransportador?.Codigo) && !(carga.TipoOperacao?.PermitirUtilizarPlacaContrato ?? false))
                {
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    retorno.mensagem = "O veículo informado pertence a um contrato de frete e não pode ser utilizado nesta carga.";
                    retorno.VeiculoPossuiContratoFrete = true;

                    carga.PossuiPendencia = true;
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                    if (!configuracaoTMS.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                    carga.MotivoPendencia = "O veículo informado pertence a um contrato de frete e não pode ser utilizado nesta carga.";

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 27 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    return false;
                }
            }

            return true;
        }

        public void SetarDadosFreteCarga(Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, string stringConexao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.NFSe.NFSe serNFSe = new Servicos.Embarcador.NFSe.NFSe(unitOfWork);
            Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracao);
            Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente servicoContratoFreteCliente = new Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete repConfiguracaoCargaCalculoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete configuracaoCargaCalculoFrete = repConfiguracaoCargaCalculoFrete.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);

            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            serCargaPedido.VerificarSePossuiPedidoFilialEmissora(ref carga, unitOfWork);
            serFrete.ValidarMensagemAlertaGrupoPessoa(carga, unitOfWork, configuracao);
            serFrete.ValidarConfiguracaoFaturamentoTomador(carga, unitOfWork, configuracao);
            serFrete.ValidarEntidadesSemCadastro(carga, unitOfWork, configuracao);
            serFrete.ValidarEntidadesSemCodigoDocumentacao(carga, unitOfWork, configuracao);

            unitOfWork.Start();

            Log.GravarInfo($"Inciou Commit finalização do Calculo - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");

            carga.DataInicioCalculoFrete = null;
            carga.CalculandoFrete = false;
            carga.CalcularFreteSemEstornarComplemento = false;

            if (configuracao.CalcularFreteInicioCarga)
            {
                if (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe)
                {
                    if (carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Integracao)
                        carga.CalcularFreteLote = Dominio.Enumeradores.LoteCalculoFrete.Padrao;
                    repCarga.Atualizar(carga);
                    unitOfWork.CommitChanges();
                    Log.GravarInfo($"Finalizou pré calculo - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");
                    return;
                }
            }

            if (cargaJanelaCarregamento != null)
                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador &&
                (!carga.ExigeConfirmacaoAntesEmissao || carga.LiberadaEmissaoERP) &&
                carga.ExigeNotaFiscalParaCalcularFrete && !carga.DataEnvioUltimaNFe.HasValue)
            {
                if (retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                {
                    bool averbacaoLiberada = true;
                    if (configuracao.NaoPermiteEmitirCargaSemAverbacao && !(carga.TipoOperacao?.PermitirCargaSemAverbacao ?? false) && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente && !carga.CargaDePreCarga)
                    {
                        Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);
                        IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);

                        averbacaoLiberada = false;
                        Dominio.Entidades.Empresa empresa = carga.ObterEmpresaEmissora;
                        if (empresa == null || !empresa.LiberarEmissaoSemAverbacao)
                            averbacaoLiberada = carga.DadosSumarizados != null ? carga.DadosSumarizados.PossuiAverbacaoCTe || (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0) : false;
                        else
                            averbacaoLiberada = true;
                    }
                    if (averbacaoLiberada)
                    {
                        if (carga.Rota == null && (
                            (carga.GrupoPessoaPrincipal != null && carga.GrupoPessoaPrincipal.ExigirRotaParaEmissaoDocumentos) ||
                            (configuracao.ExigirRotaParaEmissaoDocumentos && Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.IsComprarValePedagio(carga, unitOfWork) && !cargaPedidos.Any(o => o.ValorPedagio > 0))
                        ))
                        {
                            carga.DataInicioEmissaoDocumentos = null;
                            carga.DataEnvioUltimaNFe = null;
                            carga.DataInicioGeracaoCTes = null;
                            carga.PendenciaEmissaoAutomatica = true;

                            AlertarRotaNaoCadastrada(carga, unitOfWork, tipoServicoMultisoftware);
                        }
                        else
                        {
                            //valida se pode emitir NFS automaticamente
                            bool liberadaEmissaoNFS = true;
                            if (carga.DadosSumarizados.PossuiNFS)
                            {
                                List<Dominio.Entidades.Localidade> localidades = repCargaPedido.BuscarLocalidadesEmissaoNFsPorCarga(carga.Codigo);

                                foreach (Dominio.Entidades.Localidade localidade in localidades)
                                {
                                    Dominio.Entidades.Cliente tomador = carga.Pedidos.FirstOrDefault()?.ObterTomador() ?? null;
                                    Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(carga.Empresa.Codigo, localidade.Codigo, localidade.Estado?.Sigla ?? "", carga.GrupoPessoaPrincipal?.Codigo ?? 0, localidade.Codigo, carga?.TipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, 0, unitOfWork);
                                    if (configNFSe == null)
                                    {
                                        carga.DataInicioEmissaoDocumentos = null;
                                        carga.DataEnvioUltimaNFe = null;
                                        carga.DataInicioGeracaoCTes = null;
                                        carga.PendenciaEmissaoAutomatica = true;
                                        liberadaEmissaoNFS = false;
                                        break;
                                    }
                                }
                            }

                            if (liberadaEmissaoNFS && configuracao.NaoEmitirCargaComValorZerado)
                            {
                                if (repCargaPedido.VerificarPorCargaSePossuiValorZerado(carga.Codigo) > 0)
                                {
                                    carga.DataInicioEmissaoDocumentos = null;
                                    carga.DataEnvioUltimaNFe = null;
                                    carga.DataInicioGeracaoCTes = null;
                                    carga.PendenciaEmissaoAutomatica = true;
                                    liberadaEmissaoNFS = false;
                                }
                            }

                            if (liberadaEmissaoNFS)
                            {
                                if (serCTe.VerificarSePodeEmitirAutomaticamente(tipoServicoMultisoftware, carga, configuracao.AtivarAutorizacaoAutomaticaDeTodasCargas))
                                    carga.CTesEmDigitacao = false;
                                else
                                    carga.CTesEmDigitacao = true;

                                if (configuracaoCargaCalculoFrete == null ||
                                    configuracaoCargaCalculoFrete.ValorMaximoCalculoFrete <= 0m ||
                                    carga.ValorFreteAPagar <= configuracaoCargaCalculoFrete.ValorMaximoCalculoFrete)
                                {
                                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                                    carga.DataEnvioUltimaNFe = DateTime.Now;
                                    carga.DataInicioGeracaoCTes = DateTime.Now;

                                    string retornoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracao, tipoServicoMultisoftware, unitOfWork);
                                    if (!string.IsNullOrWhiteSpace(retornoCIOT))
                                    {
                                        carga.DataInicioEmissaoDocumentos = null;
                                        carga.DataEnvioUltimaNFe = null;
                                        carga.DataInicioGeracaoCTes = null;
                                        carga.PendenciaEmissaoAutomatica = true;
                                    }
                                    else
                                    {
                                        if (carga.Empresa != null && carga.Empresa.TempoDelayHorasParaIniciarEmissao > 0 && carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao)
                                        {
                                            carga.DataInicioEmissaoDocumentos = DateTime.Now;
                                            carga.DataEnvioUltimaNFe = DateTime.Now.AddHours(carga.Empresa.TempoDelayHorasParaIniciarEmissao);
                                            carga.DataInicioGeracaoCTes = DateTime.Now.AddHours(carga.Empresa.TempoDelayHorasParaIniciarEmissao);
                                        }
                                    }

                                    if (carga.PossuiPendenciaConfiguracaoContabil)
                                    {
                                        carga.DataInicioEmissaoDocumentos = null;
                                        carga.DataEnvioUltimaNFe = null;
                                        carga.DataInicioGeracaoCTes = null;
                                        carga.PendenciaEmissaoAutomatica = true;
                                    }
                                }
                                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);
                            }
                        }
                    }
                    else
                    {
                        carga.problemaAverbacaoCTe = true;
                    }
                }
                else
                {
                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao, null, clienteMultisoftware);
                }
            }
            else
            {
                var avancarAutomatico = carga.TipoOperacao != null && carga.TipoOperacao?.ConfiguracaoDocumentoEmissao?.MinutosAvancarParaEmissaoseInformadosDadosTransporte > 0;

                if (carga.ExigeNotaFiscalParaCalcularFrete && carga.ExigeConfirmacaoAntesEmissao && carga.DataEnvioUltimaNFe.HasValue && !avancarAutomatico)
                {
                    carga.DataInicioEmissaoDocumentos = null;
                    carga.DataEnvioUltimaNFe = null;
                }
            }

            if (!carga.ExigeNotaFiscalParaCalcularFrete && !carga.PossuiPendencia)
            {
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> listaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);

                bool averbacaoLiberada = true;
                if (configuracao.NaoPermiteEmitirCargaSemAverbacao && !(carga.TipoOperacao?.PermitirCargaSemAverbacao ?? false) && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente)
                {
                    Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);

                    averbacaoLiberada = false;
                    Dominio.Entidades.Empresa empresa = carga.ObterEmpresaEmissora;
                    if (empresa == null || !empresa.LiberarEmissaoSemAverbacao)
                        averbacaoLiberada = carga.DadosSumarizados != null ? carga.DadosSumarizados.PossuiAverbacaoCTe || (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0) : false;
                    else
                        averbacaoLiberada = true;
                }

                if (carga.Empresa != null && averbacaoLiberada && ((carga.Veiculo != null && listaMotoristas.Count > 0) || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao)) && (carga.DataEnvioUltimaNFe.HasValue || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao)))
                {

                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                    repCargaPedido.SetarAguardandoNotas(carga.Codigo);

                }
                else
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                    bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false) && repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao();

                    if (!existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                }
            }

            if (!carga.PossuiPendencia)
                servicoContratoFreteCliente.ConsultarSaldoEConsome(carga);

            if (carga.ExigeNotaFiscalParaCalcularFrete && carga.PossuiPendencia && carga.DataEnvioUltimaNFe.HasValue)
            {
                carga.DataEnvioUltimaNFe = null;
                carga.DataInicioEmissaoDocumentos = null;
            }

            if (carga.CargaPossuiPreCalculoFrete && carga.CargaDePreCarga) // volta para etapa 1
            {
                if (carga.DataSalvamentoDadosTransporte.HasValue && carga.ExigeNotaFiscalParaCalcularFrete)
                    carga.SituacaoCarga = SituacaoCarga.AgNFe;
                else
                    carga.SituacaoCarga = SituacaoCarga.Nova;
            }
            else
                ValidarDivergenciaPreCalculoValorFrete(carga, unitOfWork);

            if (!carga.PossuiPendencia && retorno.situacao == SituacaoRetornoDadosFrete.FreteValido && carga.LiberadaEmissaoERP)
                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, tipoServicoMultisoftware).AdicionarIntegracaoIndividual(carga, EtapaCarga.FreteEmbarcador, "Sucesso no cálculo de frete.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal }, true);

            if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga && retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                SumarizarValoresFreteSubTrechosPreChecking(carga, retorno, unitOfWork);

            bool naoCriarAprovacao = carga.TipoOperacao?.NaoCriarAprovacaoCargaConfirmarDocumento ?? false;

            if ((configuracao.UtilizaEmissaoMultimodal || configuracaoAprovacao.CriarAprovacaoCargaAoConfirmarDocumentos) && !naoCriarAprovacao)
            {
                new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork).CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.Outros, tipoServicoMultisoftware);
            }

            repCarga.Atualizar(carga);

            // Verifica se existe frete para ser alterado manualmente
            if (carga != null && carga.TabelaFrete != null && carga.TabelaFrete.PermiteAlterarValor)
                PrepararAlteracaoValorFreteInformadoManualNoPedido(carga, cargaPedidos, unitOfWork, configuracao, true, false);

            if (carga != null && carga.TabelaFreteFilialEmissora != null && carga.TabelaFreteFilialEmissora.PermiteAlterarValor)
                PrepararAlteracaoValorFreteInformadoManualNoPedido(carga, cargaPedidos, unitOfWork, configuracao, false, true);

            unitOfWork.CommitChanges();
            Log.GravarInfo($"Finalizou Commit finalização do Calculo - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");

            if (cargaJanelaCarregamento != null)
                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

            serHubCarga.InformarRetornoCalculoFrete(carga.Codigo, retorno, clienteMultisoftware);

            new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, clienteMultisoftware)
                .ReenviarIntegracoesCargaDadosTransportePosFreteAsync(carga, unitOfWork)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

        private void ValidarDivergenciaPreCalculoValorFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCarga?.ValidaValorPreCalculoValorFrete ?? false))
                return;

            Repositorio.Embarcador.Cargas.CargaPreCalculoFrete repCargaPreCalculoFrete = new Repositorio.Embarcador.Cargas.CargaPreCalculoFrete(unitOfWork);

            if (!repCargaPreCalculoFrete.ExistePorCarga(carga.Codigo))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete cargaDadosPreCalculoFrete = repCargaPreCalculoFrete.BuscarPorCarga(carga.Codigo);

            if (cargaDadosPreCalculoFrete.Carga.ValorFrete != cargaDadosPreCalculoFrete.ValorFrete)
            {
                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                carga.PossuiPendencia = true;
                carga.MotivoPendencia = "Pré-cálculo diferente do cálculo do frete. Por favor validar o processo novamente";
                carga.MotivoPendenciaFrete = MotivoPendenciaFrete.DivergenciaPreCalculoFrete;
            }
        }

        private void PrepararAlteracaoValorFreteInformadoManualNoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool AlteraValorFrete, bool AlteraValorFreteFilialEmissora)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete> lstCargaPedidoValoresDeFrete = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete>();
            foreach (var cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Pedido.ValorCobrancaFreteCombinado == null) // não é boa estratégia qualquer automação pode deichar esse valor diferente de null
                    continue;

                int codigoCargaPedido = (int)cargaPedido.Codigo;
                int codigoPedido = (int)cargaPedido.Pedido.Codigo;
                string numeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador ?? "";
                decimal valorFrete = cargaPedido.ValorFrete;
                decimal valorFreteFilialEmissora = cargaPedido.ValorFreteFilialEmissora;
                decimal valorFreteAntesDaAlteracaoManual = 0;
                decimal valorFreteFilialEmissoraAntesDaAlteracaoManual = 0;
                decimal valorFreteDatabase = 0;
                decimal valorFreteFilialEmissoraDatabase = 0;

                if (AlteraValorFreteFilialEmissora)
                    valorFreteFilialEmissora = (decimal)cargaPedido.Pedido.ValorCobrancaFreteCombinado;
                else
                    valorFrete = (decimal)cargaPedido.Pedido.ValorCobrancaFreteCombinado;


                lstCargaPedidoValoresDeFrete.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete()
                {
                    CodigoCargaPedido = cargaPedido.Codigo,
                    CodigoPedido = cargaPedido.Pedido.Codigo,
                    //NomeRemetente = nomeRemetente,
                    //NomeDestinatario = nomeDestinatario,
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    ValorFrete = valorFrete,
                    ValorFreteFilialEmissora = valorFreteFilialEmissora,
                    ValorFreteAntesDaAlteracaoManual = cargaPedido.ValorFreteTabelaFrete,
                    ValorFreteFilialEmissoraAntesDaAlteracaoManual = cargaPedido.ValorFreteFilialEmissora,
                    ValorFreteDatabase = cargaPedido.ValorFrete,
                    ValorFreteFilialEmissoraDatabase = cargaPedido.ValorFreteFilialEmissora
                });
            }
            if (lstCargaPedidoValoresDeFrete.Count <= 0)
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);
            Servicos.Embarcador.Carga.Frete srvFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);

            srvFrete.SalvarValorManualFretePorPorPedido(lstCargaPedidoValoresDeFrete, carga, cargasOrigem, cargaPedidos, unitOfWork, AlteraValorFrete, AlteraValorFreteFilialEmissora, TipoServicoMultisoftware, configuracaoTMS);

            repCarga.Atualizar(carga);
            Log.TratarErro($"Finalizou Commit finalização do Calculo - {carga.CodigoCargaEmbarcador} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CalcularFreteCargasPendentes");
            //Servicos.Auditoria.Auditoria.Auditar(null, carga, null, "Alterado valor do frete manualmente.", unitOfWork);
        }

        private bool ValidarTipoOperacaoCarga(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (carga.TipoOperacao != null)
            {
                if (carga.TipoOperacao.ExigeRecebedor)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                    if (repCargaPedido.VerificarSePossuiPedidoSemRecebedor(carga.Codigo))
                    {
                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                        //retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ConfiguracaoFaltando;
                        retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                        retorno.mensagem = "A operação da carga exige que todos os pedidos tenham um recebedor diferente do destinatário do pedido.";
                        carga.PossuiPendencia = true;
                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                        if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                        carga.MotivoPendencia = "A operação da carga exige que todos os pedidos tenham um recebedor diferente do destinatário do pedido.";

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 26 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                        return false;
                    }
                }
            }

            return true;
        }

        public void SetarDadosGeraisRetornoFrete(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido tipoFreteEscolhido, Dominio.Entidades.Usuario usuario = null)
        {
            SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, tipoFreteEscolhido, null, usuario);
        }

        public void SetarDadosGeraisRetornoFrete(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido tipoFreteEscolhido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario = null)
        {
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> complementosFrete = repCargaComplementoFrete.BuscarPorCargaSemComponenteCompoeFreteValor(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponenteFrete.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga = new OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

            bool possuiOcultarInformacoesCarga = usuario != null ? serOcultarInformacoesCarga.PossuiOcultarInformacoesCarga(usuario.Codigo) : false;
            Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = null;
            if (possuiOcultarInformacoesCarga)
                ocultarInformacoesCarga = serOcultarInformacoesCarga.ObterOcultarInformacoesCarga(usuario.Codigo);

            retorno.complementosDoFrete = (from obj in complementosFrete
                                           select new
                                           {
                                               obj.Codigo,
                                               ComponenteFrete = obj.ComponenteFrete.Descricao,
                                               Data = obj.DataAlteracao.ToString("dd/MM/yyyy HH:mm"),
                                               Operador = obj.Usuario.Nome,
                                               ValorComplemento = obj.ValorComplemento.ToString("n2"),
                                               obj.SituacaoComplementoFrete,
                                               obj.DescricaoSituacao
                                           }).ToList();

            if (!apenasVerificar)
            {
                if (retorno.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                {
                    Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                    serFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, tipoFreteEscolhido, unitOfWork, apenasVerificar, TipoServicoMultisoftware, StringConexao);
                }
            }


            if (carga.FreteDeTerceiro)
            {
                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serFreteSubcontratacaoTerceiro = new FreteSubcontratacaoTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = repContratoFreteTerceiro.BuscarPorCarga(carga.Codigo);

                retorno.freteSubContratacao = serFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFreteTerceiro);
            }

            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
            bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;
            decimal valorFrete = carga.ValorFrete;
            decimal valorFretePagar = (configuracao?.VisualizarValorNFSeDescontandoISSRetido ?? false) ? carga.ValorFreteAPagar - (carga.ValorRetencaoISS > 0 ? carga.ValorRetencaoISS : carga.ValorISS) : carga.ValorFreteAPagar;
            decimal valorFreteTabelaFrete = (configuracao?.VisualizarValorNFSeDescontandoISSRetido ?? false) ? carga.ValorFreteTabelaFrete - (carga.ValorRetencaoISS > 0 ? carga.ValorRetencaoISS : carga.ValorISS) : carga.ValorFreteTabelaFrete;
            decimal valorFreteAPagarComICMSeISS = (configuracao?.VisualizarValorNFSeDescontandoISSRetido ?? false) ? carga.ValorTotalAReceberComICMSeISS - (carga.ValorRetencaoISS > 0 ? carga.ValorRetencaoISS : carga.ValorISS) : carga.ValorTotalAReceberComICMSeISS;
            decimal valorFreteLiquido = 0;
            if (carga.TabelaFrete == null && carga.ContratoFreteTransportador != null)
                valorFreteLiquido = carga.ValorFreteContratoFreteTotal;
            else
                valorFreteLiquido = carga.ValorFreteLiquido + ((!(carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);

            if (carga.TabelaFrete?.IncluirICMSValorFreteNaCarga ?? false)
            {
                decimal valorICMS = carga.ValorICMS;

                if (valorICMS != 0)
                    valorFreteLiquido = (valorFreteLiquido + valorICMS);
            }

            if (carga.TabelaFrete?.DescontarComponenteFreteLiquido ?? false)
                valorFreteLiquido += BuscarValorTotalComponentes(carga);

            decimal valorDescontar = 0;
            if (carga.TabelaFrete?.NaoSomarValorTotalPrestacao ?? false)
                valorDescontar = BuscarValorTotalDescontarComponentesTabelaFrete(carga, carga.TabelaFrete.ComponenteFreteDestacar);

            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS retornoIBSCBS = repCargaPedido.BuscarImpostoIBSCBSPorCarga(carga.Codigo);

            if (!calculoFreteFilialEmissora)
            {
                retorno.valorFreteContratoFrete = carga.ValorFreteContratoFreteTotal;
                retorno.valorFrete = cargaComponentesFrete.Any(o => o.ComponenteFrete?.DescontarComponenteNotaFiscalServico ?? false) ? valorFretePagar : (possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, valorFrete) : valorFrete);
                retorno.valorFreteAPagar = cargaComponentesFrete.Any(o => o.ComponenteFrete?.DescontarComponenteNotaFiscalServico ?? false) ? valorFrete : ObterValorFretePorCarga(carga, serOcultarInformacoesCarga, possuiOcultarInformacoesCarga, ocultarInformacoesCarga, valorFrete, valorFretePagar, valorDescontar);
                retorno.ValorFreteNegociado = carga.ValorFreteNegociado;
                retorno.valorFreteTabelaFrete = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, valorFreteTabelaFrete) : valorFreteTabelaFrete;
                retorno.valorFreteAPagarComICMSeISS = ObterValorFretePorCarga(carga, serOcultarInformacoesCarga, possuiOcultarInformacoesCarga, ocultarInformacoesCarga, valorFrete, valorFreteAPagarComICMSeISS, valorDescontar);
                retorno.valorFreteOperador = carga.ValorFreteOperador;
                retorno.valorFreteLeilao = carga.ValorFreteLeilao;
                retorno.valorFreteEmbarcador = carga.ValorFreteEmbarcador;

                retorno.CodigoClassificacaoTributaria = retornoIBSCBS.ClassificacaoTributaria;

                retorno.AliquotaIBSUF = retornoIBSCBS.AliquotaIBSEstadual;
                retorno.ValorIBSUF = carga.ValorIBSEstadual;
                retorno.ReducaoIBSUF = retornoIBSCBS.PercentualReducaoIBSEstadual;

                retorno.AliquotaIBSMunicipio = retornoIBSCBS.AliquotaIBSMunicipal;
                retorno.ValorIBSMunicipio = carga.ValorIBSMunicipal;
                retorno.ReducaoIBSMunicipio = retornoIBSCBS.PercentualReducaoIBSMunicipal;

                retorno.AliquotaCBS = retornoIBSCBS.AliquotaCBS;
                retorno.ValorCBS = carga.ValorCBS;
                retorno.ReducaoCBS = retornoIBSCBS.PercentualReducaoCBS;

                retorno.ValorFreteLiquido = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, valorFreteLiquido) : valorFreteLiquido;
                retorno.valorICMS = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, carga.ValorICMS) : carga.ValorICMS;
                retorno.valorISS = carga.ValorISS;
                retorno.ValorRetencaoISS = carga.ValorRetencaoISS;
                retorno.aliquotaICMS = repCargaPedido.BuscarMediaAliquotaICMSdaCarga(carga.Codigo);
                retorno.csts = repCargaPedido.BuscarCSTICMSdaCarga(carga.Codigo);
                retorno.taxaDocumentacao = RetornarTaxaDocumental(carga);
                retorno.aliquotaISS = repCargaPedido.BuscarMediaAliquotaISSdaCarga(carga.Codigo);
                retorno.valorMercadoria = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo)) : repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
                retorno.peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
                retorno.BloqueadaDiferencaValorFrete = carga.BloqueadaDiferencaValorFrete;
                retorno.GerarOcorrenciaDiferencaValorFrete = carga.GerarOcorrenciaDiferencaValorFrete;
                retorno.ValorDiferencaValorFrete = carga.ValorDiferencaValorFrete;
                retorno.ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m;
                retorno.Moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                retorno.ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
                retorno.ValorTotalMoedaPagar = carga.ValorTotalMoedaPagar ?? 0m;
                retorno.CustoFrete = carga.DadosSumarizados?.CustoFrete ?? string.Empty;
                retorno.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ?? false;
                retorno.PercentualBonificacaoTransportador = carga.PercentualBonificacaoTransportador;
                retorno.DescricaoBonificacaoTransportador = (carga.PercentualBonificacaoTransportador != 0m) ? carga.BonificacaoTransportador?.ComponenteFrete?.Descricao ?? string.Empty : string.Empty;
                retorno.TipoOcorrenciaDiferencaValorFrete = new
                {
                    Codigo = carga.TipoOcorrenciaDiferencaValorFrete?.Codigo ?? 0,
                    Descricao = carga.TipoOcorrenciaDiferencaValorFrete?.Descricao ?? string.Empty
                };
            }
            else
            {
                retorno.valorFreteContratoFrete = carga.ValorFreteContratoFreteTotal;
                retorno.valorFrete = carga.ValorFreteFilialEmissora;
                retorno.valorFreteAPagar = carga.ValorFreteAPagarFilialEmissora;
                retorno.valorFreteTabelaFrete = carga.ValorFreteTabelaFreteFilialEmissora;
                retorno.valorFreteAPagarComICMSeISS = carga.ValorTotalAReceberComICMSeISSFilialEmissora;
                retorno.valorFreteOperador = carga.ValorFreteOperador;
                retorno.ValorFreteLiquido = carga.ValorFreteAPagarFilialEmissora + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);
                retorno.valorICMS = carga.ValorICMSFilialEmissora;
            }

            ComponetesFrete serComponentes = new ComponetesFrete(unitOfWork);
            serComponentes.BuscarComponentesDeFreteDaCarga(ref retorno, carga, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware, possuiOcultarInformacoesCarga, ocultarInformacoesCarga);
        }

        private static decimal ObterValorFretePorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga, bool possuiOcultarInformacoesCarga, Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, decimal valorFrete, decimal valorFretePagarComOuSemImpostos, decimal valorDescontar)
        {
            bool naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ?? false;

            if (naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador && carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                return valorFrete;

            decimal valorFretePorCarga = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, valorFretePagarComOuSemImpostos) : valorFretePagarComOuSemImpostos;

            return (carga.TabelaFrete?.NaoSomarValorTotalPrestacao ?? false) ? (valorFretePorCarga - valorDescontar) : valorFretePorCarga;
        }

        private bool VerificarTipoFreteCliente(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                if (!apenasVerificar)
                {
                    carga.PossuiPendencia = false;
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    serCarga.InformarSituacaoCargaFreteValido(ref carga, TipoServicoMultisoftware, unitOfWork);
                }

                return true;
            }

            return false;
        }

        public bool VerificarFreteNegociadoNoPedido(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao, bool apenasVerificar, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS || carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                return false;

            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            decimal valorFreteNegociado = 0m;
            if (configuracao.SolicitarValorFretePorTonelada)
            {
                valorFreteNegociado = cargaPedidos.Sum(o => o.Pedido.ValorFreteToneladaNegociado);
                if (valorFreteNegociado > 0)
                {
                    decimal pesoNotas = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
                    if (pesoNotas > 0)
                        valorFreteNegociado = (valorFreteNegociado * (pesoNotas / 1000));
                }
            }
            else
                valorFreteNegociado = cargaPedidos.Sum(o => o.Pedido.ValorFreteNegociado);

            bool possuiComponentePedido = repPedidoComponenteFrete.ExisteComponenteNoPedido(carga.Codigo, calculoFreteFilialEmissora);

            if (valorFreteNegociado <= 0m && (!possuiComponentePedido || (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarComponentesManuaisPedidoComTabelaFrete ?? false)))
                return false;

            if (!apenasVerificar)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicoesFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                Servicos.Embarcador.Carga.ComponetesFrete svcCargaComponenteFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete serCargaRateioFrete = new RateioFrete(unitOfWork);

                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;

                if (configuracao.UtilizaEmissaoMultimodal && carga.TipoOperacao != null && carga.TipoOperacao.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = cargaPedidos?.Select(o => o.Pedido).FirstOrDefault() ?? null;
                    if (primeiroPedido != null)
                    {
                        if (primeiroPedido.TipoCalculoCargaFracionada == null || primeiroPedido.TipoCalculoCargaFracionada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCargaFracionada.Tonelada)
                            valorFreteNegociado = CalcularValorFreteFracionadaTonelada(valorFreteNegociado, carga, unitOfWork);
                        else if (primeiroPedido.TipoCalculoCargaFracionada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCargaFracionada.MetroCubito)
                        {
                            decimal densidadeProduto = primeiroPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;
                            valorFreteNegociado = CalcularValorFreteFracionadaMetroCubico(valorFreteNegociado, densidadeProduto, carga, unitOfWork);
                        }
                    }
                }

                carga.ValorFreteOperador = valorFreteNegociado;
                carga.ValorFrete = valorFreteNegociado;

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteAdicionarCarga = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> componentesPedido = repPedidoComponenteFrete.BuscarPorPedido(cargaPedido.Pedido.Codigo, calculoFreteFilialEmissora);

                    decimal valorNotasFiscais = componentesPedido.Any(o => o.TipoValor == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) ? repPedidoXMLNotaFiscal.BuscarValorTotalPorCargaPedido(cargaPedido.Codigo) : 0m;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componentePedido in componentesPedido)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteDinamico = componentePedido.ConvertarParaComponenteDinamico();

                        int idxComponente = componentesFreteAdicionarCarga.FindIndex(o => o.ComponenteFrete.Codigo == componenteFreteDinamico.ComponenteFrete.Codigo);

                        if (componenteFreteDinamico.TipoValor == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal && valorNotasFiscais > 0m && componenteFreteDinamico.Percentual > 0m)
                            componenteFreteDinamico.ValorComponente = Math.Round((valorNotasFiscais * (componenteFreteDinamico.Percentual / 100)), 2, MidpointRounding.ToEven);

                        if (idxComponente > -1)
                            componentesFreteAdicionarCarga[idxComponente].ValorComponente += componenteFreteDinamico.ValorComponente;
                        else
                            componentesFreteAdicionarCarga.Add(componenteFreteDinamico);
                    }
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteAdicionar in componentesFreteAdicionarCarga)
                {
                    svcCargaComponenteFrete.AdicionarComponenteFreteCarga(carga, componenteFreteAdicionar.ComponenteFrete, componenteFreteAdicionar.ValorComponente, componenteFreteAdicionar.Percentual, calculoFreteFilialEmissora, componenteFreteAdicionar.TipoValor, componenteFreteAdicionar.TipoComponenteFrete, null, true, componenteFreteAdicionar.IncluirIntegralmenteContratoFreteTerceiro, null, TipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true);

                    composicoesFrete.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Componente adicionado pelo Pedido", " Valor do Componente = " + componenteFreteAdicionar.ValorComponente.ToString("n2"), componenteFreteAdicionar.ValorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, componenteFreteAdicionar.ComponenteFrete?.DescricaoComponente ?? "", componenteFreteAdicionar.ComponenteFrete?.Codigo ?? 0, componenteFreteAdicionar.ValorComponente));
                }

                serCargaRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware);

                composicoesFrete.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor do Frete Negociado pelo Pedido", " Valor Informado = " + carga.ValorFreteOperador.ToString("n2"), carga.ValorFreteOperador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFreteOperador));

                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicoesFrete, unitOfWork, null);

                ProcessarRegraInclusaoICMSComponenteFrete(carga, cargaPedidos, configuracao, unitOfWork);
                CalcularValorFreteComICMSIncluso(carga, unitOfWork);
            }

            retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete
            {
                tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.freteSemTabela,
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido
            };

            SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador);

            return true;
        }

        public bool VerificarTipoFreteOperador(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao, bool apenasVerificar, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario = null)
        {
            if ((carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador || carga.CargaSVM) && carga.TabelaFrete == null && apenasVerificar && cargaTabelaFreteSubContratacao == null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.freteSemTabela;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

                SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador, usuario);

                return true;
            }
            else if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && !apenasVerificar && !adicionarComponentesCarga)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicaoFretes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                Servicos.Embarcador.Carga.ComponetesFrete serCargaComponentesFrete = new ComponetesFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = carga.Veiculo?.ModeloCarroceria?.ComponenteFrete;

                if (componente == null && carga.VeiculosVinculados != null)
                    componente = (from obj in carga.VeiculosVinculados where obj.ModeloCarroceria != null && obj.ModeloCarroceria.ComponenteFrete != null select obj.ModeloCarroceria.ComponenteFrete).FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFreteAdicionarCarroceria = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                decimal valorAdicionalCarroceria = Frete.CalcularValorFreteAdicionalPorModeloCarroceriaVeiculo(carga.Veiculo, carga.VeiculosVinculados, ref composicaoFreteAdicionarCarroceria, componente, carga.ValorFrete);

                composicaoFretes.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", " Valor Informado = " + carga.ValorFreteOperador.ToString("n2"), carga.ValorFreteOperador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFreteOperador));

                if (componente != null && valorAdicionalCarroceria > 0)
                {
                    serCargaComponentesFrete.AdicionarComponenteFreteCarga(carga, componente, valorAdicionalCarroceria, 0, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, componente.TipoComponenteFrete, null, true, false, null, TipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true);
                    composicaoFretes.Add(composicaoFreteAdicionarCarroceria);
                }

                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicaoFretes, unitOfWork, null);

                Servicos.Embarcador.Carga.RateioFrete serCargaRateioFrete = new RateioFrete(unitOfWork);
                serCargaRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware);

                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.freteSemTabela;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

                SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador);

                return true;
            }

            return false;
        }

        public bool VerificarTipoFreteEmbarcador(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool apenasVerificar, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                if (!apenasVerificar)
                {
                    AtualizarFreteEmbarcadorCargaComponentes(carga, calculoFreteFilialEmissora, unitOfWork);
                }

                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador);

                return true;
            }

            return false;
        }

        public void AtualizarFreteEmbarcadorCargaComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Embarcador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Embarcador", 0, carga.ValorFrete);
            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicaoFrete, unitOfWork, null);

            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponente = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentes = repCargaComponente.BuscarPorCargaFilialEmissora(carga.Codigo, calculoFreteFilialEmissora);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in componentes)
                repCargaComponente.Deletar(componente);

            if (carga.CargaAgrupada && carga.CargasAgrupamento?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteAgrupamento = repCargaComponente.BuscarPorCarga(cargaOrigem.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFreteAgrupamento)
                        repCargaComponente.Deletar(componente);
                }
            }

            CriarCargaComponentes(carga, TipoServicoMultisoftware, calculoFreteFilialEmissora, unitOfWork);
        }

        public void CalcularFretePorComissaoProduto(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            FreteComissao serFreteComissao = new FreteComissao(unitOfWork);
            retorno = serFreteComissao.CalcularFretePorComissao(ref carga, cargaPedidos, configuracao, tabelaFrete, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, TipoServicoMultisoftware);
            retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto;

            if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
            {
                carga.PossuiPendencia = true;
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 25 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
            }
            else
            {
                if (!apenasVerificar)
                {
                    carga.PossuiPendencia = false;
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    serCarga.InformarSituacaoCargaFreteValido(ref carga, TipoServicoMultisoftware, unitOfWork);
                }
            }

            carga.ValorFrete = retorno != null ? retorno.valorFrete : 0;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete ObterDadosCalculoFreteParaSubcontratacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            StringBuilder mensagem = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(carga, unitOfWork, tipoServicoMultisoftware, configuracao, ref mensagem, false, null, true);

            if (tabelasFrete.Count > 1)
                return new DadosCalculoFrete() { MensagemRetorno = "Foi encontrada mais de uma configuração de tabela de frete para esta subcontratação." };

            if (tabelasFrete.Count == 0)
                return new DadosCalculoFrete() { MensagemRetorno = "Não foi encontrada uma configuração de tabela de frete para esta subcontratação." };

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, false, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracao);
            if (parametrosCalculo == null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFreteSemParametro = new DadosCalculoFrete();
                dadosCalculoFreteSemParametro.FreteCalculado = false;
                return dadosCalculoFreteSemParametro;
            }

            parametrosCalculo.TransportadorTerceiro = carga.Terceiro;
            parametrosCalculo.PagamentoTerceiro = true;

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = ObterDadosCalculoFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracao);

            dadosCalculoFrete.ObservacaoContratoFrete = tabelaFrete.ObservacaoContratoFrete;
            dadosCalculoFrete.TextoAdicionalContratoFrete = tabelaFrete.TextoAdicionalContratoFrete;
            dadosCalculoFrete.ReterImpostosContratoFrete = tabelaFrete.ReterImpostosContratoFrete;
            dadosCalculoFrete.DiasVencimentoAdiantamentoContratoFrete = tabelaFrete.DiasVencimentoAdiantamentoContratoFrete;
            dadosCalculoFrete.DiasVencimentoSaldoContratoFrete = tabelaFrete.DiasVencimentoSaldoContratoFrete;

            return dadosCalculoFrete;
        }

        public static decimal CalcularFretePorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            if (cte.Remetente == null || cte.Destinatario == null)
                return 0m;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new InformacaoCargaCTE(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCTe(cte.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

            if (cargaPedido == null || cargaPedido.Pedido == null)
                return 0m;

            int qtdAjudantesTerceiros = 0;

            if ((tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS) && (cargaPedido.Carga.Ajudantes?.Count > 0))
                qtdAjudantesTerceiros = cargaPedido.Carga.Ajudantes.Where(o => o.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro).Count();

            List<ParametrosCalculoFreteQuantidade> listaQuantidade = (from obj in cte.QuantidadesCarga
                                                                      select new ParametrosCalculoFreteQuantidade()
                                                                      {
                                                                          Quantidade = obj.Quantidade,
                                                                          UnidadeMedida = obj.EnumUnidadeDeMedida
                                                                      }).ToList();

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                DataColeta = cargaPedido.Pedido.DataInicialColeta,
                DataFinalViagem = cargaPedido.Pedido.DataFinalViagemFaturada,//cargaPedido.Pedido.DataFinalViagemFaturada,
                DataInicialViagem = cargaPedido.Pedido.DataInicialViagemFaturada,//cargaPedido.Pedido.DataInicialViagemFaturada,
                DataVigencia = cte.DataEmissao.Value,
                DespachoTransitoAduaneiro = cargaPedido.Pedido.DespachoTransitoAduaneiro,
                Empresa = cte.Empresa,
                EscoltaArmada = cargaPedido.Pedido.EscoltaArmada,
                QuantidadeEscolta = cargaPedido.Pedido.QtdEscolta,
                Filial = cargaPedido.Pedido.Filial,
                GerenciamentoRisco = cargaPedido.Pedido.GerenciamentoRisco,
                ModeloVeiculo = cargaPedido.Carga.ModeloVeicularCarga,
                PesoTotalCarga = cargaPedido.Carga?.DadosSumarizados?.PesoTotal ?? 0m,
                NecessarioReentrega = cargaPedido.Pedido.NecessarioReentrega,
                NumeroAjudantes = qtdAjudantesTerceiros,
                NumeroEntregas = 1,//fixo por cte
                NumeroPallets = cte.XMLNotaFiscais.Sum(obj => obj.QuantidadePallets),
                Peso = cte.QuantidadesCarga.Where(obj => obj.UnidadeMedida == "01").Sum(o => o.Quantidade),
                PesoCubado = cte.XMLNotaFiscais.Sum(obj => obj.PesoCubado),
                PesoPaletizado = cte.XMLNotaFiscais.Sum(obj => obj.PesoPaletizado),
                PossuiRestricaoTrafego = (cte.Remetente?.Cliente?.PossuiRestricaoTrafego ?? false) || (cte.Destinatario?.Cliente?.PossuiRestricaoTrafego ?? false) || (cte.Expedidor?.Cliente?.PossuiRestricaoTrafego ?? false) || (cte.Recebedor?.Cliente?.PossuiRestricaoTrafego ?? false),
                QuantidadeNotasFiscais = cte.XMLNotaFiscais.Count(),
                Quantidades = listaQuantidade,
                Rastreado = cargaPedido.Pedido.Rastreado,
                TipoCarga = cargaPedido.Carga.TipoDeCarga,
                TipoOperacao = cargaPedido.Carga.TipoOperacao,
                ValorNotasFiscais = cte.ValorTotalMercadoria,
                Veiculo = cargaPedido.Carga.Veiculo,
                Reboques = cargaPedido.Carga.VeiculosVinculados?.ToList(),
                Volumes = cte.QuantidadesCarga.Where(obj => obj.UnidadeMedida == "03").Sum(o => o.Quantidade),
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { cte.ModeloDocumentoFiscal },
                PagamentoTerceiro = true,
                DataBaseCRT = cargaPedido.Pedido.DataBaseCRT,
                FreteTerceiro = cargaPedido.Carga.FreteDeTerceiro,
                CargaPerigosa = cargaPedido.Carga?.CargaPerigosaIntegracaoLeilao ?? false,
                NumeroPacotes = repositorioCargaPedidoPacote.BuscarQuantidadePacoteCarga(cargaPedido.Carga.Codigo),
            };

            parametrosCalculo.QuantidadeEmissoesPorModeloDocumento.Add(cte.ModeloDocumentoFiscal, 1);

            if (cte.Recebedor != null)
            {
                parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>() { cte.Recebedor.Cliente };
                parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>() { cte.Recebedor.Localidade };
            }
            else if (cte.Destinatario != null)
            {
                parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>() { cte.Destinatario.Cliente };
                parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>() { cte.Destinatario.Localidade };
            }

            if (cte.Expedidor != null)
            {
                parametrosCalculo.Remetentes = new List<Dominio.Entidades.Cliente>() { cte.Expedidor.Cliente };
                parametrosCalculo.Origens = new List<Dominio.Entidades.Localidade>() { cte.Expedidor.Localidade };
            }
            else if (cte.Remetente != null)
            {
                parametrosCalculo.Remetentes = new List<Dominio.Entidades.Cliente>() { cte.Remetente.Cliente };
                parametrosCalculo.Origens = new List<Dominio.Entidades.Localidade>() { cte.Remetente.Localidade };
            }

            parametrosCalculo.Tomador = cte.TomadorPagador.Cliente;
            parametrosCalculo.GrupoPessoas = grupoPessoa;

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = ObterDadosCalculoFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracaoTMS);

            decimal valorTotalFrete = dadosCalculo.ValorFrete;

            if (dadosCalculo.Componentes != null)
                valorTotalFrete += (from obj in dadosCalculo.Componentes select obj.ValorComponente).Sum();

            return valorTotalFrete;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete CalcularFretePorCotacaoPedido(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            DateTime? dataColeta, DateTime? dataFinalViagem, DateTime? dataInicialViagem, DateTime dataVigencia, double cnpjClienteAtivo, double cnpjClienteInativo, int codigoLocalidadeDestino, int codigoEmpresa,
            int codigoLocalidadeOrigem, int codigoGrupoPessoa, int codigoModeloVeicular,
            decimal distancia, bool escoltaArmada, bool gerenciamentoRisco, decimal numeroAjudantes, int numeroEntregas, decimal numeroDeslocamento, decimal numeroDiarias, decimal numeroPallets, int numeroPedidos,
            decimal peso, decimal pesoCubado, int quantidadeNotasFiscais, bool rastreado, double cnpjDestinatario, int codigoTipoDeCarga, int codigoTipoOperacao,
            decimal valorNotasFiscais, decimal volumes, decimal pesoTotal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool pagamentoTerceiro)
        {

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Localidade(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                CodigosRotasFixas = null,
                DataColeta = dataColeta,
                DataFinalViagem = dataFinalViagem,
                DataInicialViagem = dataInicialViagem,
                DataVigencia = dataVigencia,
                Desistencia = false,
                DespachoTransitoAduaneiro = false,
                Destinatarios = null,
                Destinos = null,
                Distancia = distancia,
                Empresa = null,
                EscoltaArmada = escoltaArmada,
                Filial = null,
                FormulaRateio = null,
                GerenciamentoRisco = gerenciamentoRisco,
                GrupoPessoas = null,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                ModeloVeiculo = null,
                NecessarioReentrega = false,
                NumeroAjudantes = numeroAjudantes,
                NumeroEntregas = numeroEntregas,
                NumeroDeslocamento = numeroDeslocamento,
                NumeroDiarias = numeroDiarias,
                NumeroPallets = numeroPallets,
                NumeroPedidos = numeroPedidos,
                Origens = null,
                PagamentoTerceiro = pagamentoTerceiro,
                ParametrosCarga = null,
                PercentualDesistencia = 0,
                Peso = peso,
                PesoCubado = pesoCubado,
                PesoPaletizado = 0,
                PossuiRestricaoTrafego = false,
                QuantidadeNotasFiscais = quantidadeNotasFiscais,
                Quantidades = new List<ParametrosCalculoFreteQuantidade>() { new ParametrosCalculoFreteQuantidade() { Quantidade = pesoTotal, UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG } },
                Rastreado = rastreado,
                Remetentes = null,
                RementesEDestinatariosOpcionaisQuandoExistirLocalidade = true,
                Rota = null,
                RotasDinamicas = null,
                TipoCarga = null,
                TipoOperacao = null,
                Tomador = null,
                ValorNotasFiscais = valorNotasFiscais,
                Veiculo = null,
                Volumes = volumes
            };

            if (cnpjDestinatario > 0)
                parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>() { repCliente.BuscarPorCPFCNPJ(cnpjDestinatario) };

            if (codigoLocalidadeDestino > 0)
                parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>() { repLocalidade.BuscarPorCodigo(codigoLocalidadeDestino) };
            else if (parametrosCalculo.Destinatarios != null && parametrosCalculo.Destinatarios.Count > 0)
                parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>() { parametrosCalculo.Destinatarios.FirstOrDefault().Localidade };

            if (codigoEmpresa > 0)
                parametrosCalculo.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (codigoGrupoPessoa > 0)
                parametrosCalculo.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);

            if (codigoModeloVeicular > 0)
                parametrosCalculo.ModeloVeiculo = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular);

            if (cnpjClienteAtivo > 0)
                parametrosCalculo.Remetentes = new List<Dominio.Entidades.Cliente>() { repCliente.BuscarPorCPFCNPJ(cnpjClienteAtivo) };

            if (cnpjClienteInativo > 0)
                parametrosCalculo.Remetentes = new List<Dominio.Entidades.Cliente>() { repCliente.BuscarPorCPFCNPJ(cnpjClienteInativo) };

            if (codigoLocalidadeOrigem > 0)
                parametrosCalculo.Origens = new List<Dominio.Entidades.Localidade>() { repLocalidade.BuscarPorCodigo(codigoLocalidadeOrigem) };
            else if (parametrosCalculo.Remetentes != null && parametrosCalculo.Remetentes.Count > 0)
                parametrosCalculo.Origens = new List<Dominio.Entidades.Localidade>() { parametrosCalculo.Remetentes.FirstOrDefault().Localidade };

            if (codigoTipoDeCarga > 0)
                parametrosCalculo.TipoCarga = repTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);

            if (codigoTipoOperacao > 0)
                parametrosCalculo.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

            int codigoDestino = parametrosCalculo.Destinos != null && parametrosCalculo.Destinos.Count > 0 ? parametrosCalculo.Destinos.FirstOrDefault().Codigo : 0;
            int codigoOrigem = parametrosCalculo.Origens != null && parametrosCalculo.Origens.Count > 0 ? parametrosCalculo.Origens.FirstOrDefault().Codigo : 0;

            parametrosCalculo.Tomador = parametrosCalculo.Remetentes != null && parametrosCalculo.Remetentes.Count > 0 ? parametrosCalculo.Remetentes.FirstOrDefault() : null;

            if (parametrosCalculo.GrupoPessoas == null)
            {
                if (parametrosCalculo.Tomador != null && parametrosCalculo.Tomador.GrupoPessoas != null)
                    parametrosCalculo.GrupoPessoas = parametrosCalculo.Tomador.GrupoPessoas;
                else if (parametrosCalculo.Remetentes != null && parametrosCalculo.Remetentes.Count > 0 && parametrosCalculo.Remetentes.FirstOrDefault() != null && parametrosCalculo.Remetentes.FirstOrDefault().GrupoPessoas != null)
                    parametrosCalculo.GrupoPessoas = parametrosCalculo.Remetentes.FirstOrDefault().GrupoPessoas;
                else if (parametrosCalculo.Destinatarios != null && parametrosCalculo.Destinatarios.Count > 0 && parametrosCalculo.Destinatarios.FirstOrDefault() != null && parametrosCalculo.Destinatarios.FirstOrDefault().GrupoPessoas != null)
                    parametrosCalculo.GrupoPessoas = parametrosCalculo.Destinatarios.FirstOrDefault().GrupoPessoas;
            }

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

            if (codigoOrigem == codigoDestino && codigoOrigem > 0 && codigoDestino > 0)
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
            else
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");

            parametrosCalculo.QuantidadeEmissoesPorModeloDocumento.Add(modeloDocumentoFiscal, 1);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = ObterDadosCalculoFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracaoTMS);

            if (dadosCalculo?.Componentes != null)
            {
                return dadosCalculo;
            }

            return null;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete SimularFretePorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            if (pedido.Remetente == null || pedido.Destinatario == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(pedido, unitOfWork);

            return ObterDadosCalculoFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracaoTMS);
        }

        public static decimal SimularFretePorPedidos(Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacaoEspecial, UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, ConfiguracaoTMS configuracaoTMS)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cotacaoEspecial.Pedidos.ToList();
            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = pedidos.OrderBy(obj => obj.TipoDeCarga.PrioridadeCarga).FirstOrDefault();

            decimal valorTotalFrete = 0;

            if (primeiroPedido.Remetente == null || primeiroPedido.Destinatario == null)
                return 0;

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                Empresa = cotacaoEspecial.Transportador,
                Filial = primeiroPedido.Filial,
                TipoCarga = primeiroPedido.TipoDeCarga,
                TipoOperacao = cotacaoEspecial.TipoOperacao,
                Tomador = primeiroPedido.Tomador,
                ModeloVeiculo = cotacaoEspecial.ModeloVeicularCarga,
                CalcularVariacoes = false,
                Rota = primeiroPedido.RotaFrete
            };

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = ObterTabelasFrete(parametrosCalculo, false, out StringBuilder mensagem, unitOfWork, tipoServicoMultisoftware, 0).FirstOrDefault();
            //Obtem a tabela frete pai para identificar a forma de calculo

            if (tabelaFrete == null)
                throw new ServicoException("Não foi possível encontrar uma tabela de frete para os parâmetros enviados.");

            switch (tabelaFrete.TipoCalculo)
            {
                case TipoCalculoTabelaFrete.PorPedidosAgrupados:

                    List<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> pedidosAgrupados = pedidos.GroupBy(pedido => new { ClienteOutroEndereco = pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0, Remetente = pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = pedido.Destinatario?.CPF_CNPJ ?? 0D, pedido.TipoTomador, Destino = pedido.Destino?.Codigo }).Select(grupo => grupo.ToList()).ToList();

                    if (tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado)
                    {
                        pedidosAgrupados = pedidos.Where(obj => obj.Recebedor != null).GroupBy(pedido => new { Recebedor = pedido.Recebedor }).Select(grupo => grupo.ToList()).ToList();
                        pedidosAgrupados.AddRange(pedidos.Where(obj => obj.Recebedor == null).GroupBy(pedido => new { ClienteOutroEndereco = pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0, Remetente = pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = pedido.Destinatario?.CPF_CNPJ ?? 0D, pedido.TipoTomador, Destino = pedido.Destino?.Codigo }).Select(grupo => grupo.ToList()).ToList());
                    }

                    foreach (var pedidoAgrupado in pedidosAgrupados)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                        parametrosCalculo = ObterParametrosCalculoFretePorPedidoAgrupado(tabelaFrete, pedidoAgrupado, cotacaoEspecial, unitOfWork);
                        ObterDadosTabelaFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, svcFreteCliente, ref dadosCalculo, tabelaFrete, ref mensagem);

                        valorTotalFrete += dadosCalculo.ValorFrete;

                        if (dadosCalculo.Componentes != null)
                            valorTotalFrete += (from obj in dadosCalculo.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum();
                    }

                    return valorTotalFrete;


                case TipoCalculoTabelaFrete.PorPedido:

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                        parametrosCalculo = ObterParametrosCalculoFretePorPedido(pedido, unitOfWork);
                        ObterDadosTabelaFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, svcFreteCliente, ref dadosCalculo, tabelaFrete, ref mensagem);

                        valorTotalFrete += dadosCalculo.ValorFrete;

                        if (dadosCalculo.Componentes != null)
                            valorTotalFrete += (from obj in dadosCalculo.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum();
                    }

                    return valorTotalFrete;

                default:
                    return 0;
            }
        }

        public static decimal CalcularFretePorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = SimularFretePorPedido(pedido, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracaoTMS);

            if (dadosCalculo == null)
            {
                return 0m;
            }

            decimal valorTotalFrete = dadosCalculo.ValorFrete;

            if (dadosCalculo.Componentes != null)
                valorTotalFrete += (from obj in dadosCalculo.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum();

            return valorTotalFrete;
        }

        public static decimal CalcularFretePorPedidos(Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacaoEspecial, UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, ConfiguracaoTMS configuracaoTMS)
        {
            decimal valorTotalFrete = 0;

            valorTotalFrete = SimularFretePorPedidos(cotacaoEspecial, unitOfWork, tipoServicoMultisoftware, configuracaoTMS);

            if (valorTotalFrete == 0)
            {
                throw new ServicoException("Não foi possível calcular o frete.");
            }
            if (cotacaoEspecial.Usuario?.Cliente?.CotacaoEspecial > 0)
                valorTotalFrete += valorTotalFrete * (cotacaoEspecial.Usuario.Cliente.CotacaoEspecial / 100);

            return valorTotalFrete;
        }

        public static decimal CalcularFretePorCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento, Dominio.Entidades.RotaFrete rotaFrete, out string msgFrete, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, ref int leadTime, out Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo)
        {
            msgFrete = "";
            dadosCalculo = null;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCodigos(cotacaoFreteCarregamento.Pedidos);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repPedidoProduto.BuscarPorPedidos((from obj in pedidos select obj.Codigo).ToList());

            if (pedidos.Count == 0)
                return 0m;

            decimal volumesProdutos = pedidoProdutos.Sum(p => p.Quantidade);
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();

            if (cotacaoFreteCarregamento.CarregamentoRedespacho)
            {
                destinatarios.AddRange((from pedido in pedidos where pedido.Destinatario != null select pedido.Destinatario).Distinct().ToList());
                remetentes.AddRange((from pedido in pedidos where pedido.Expedidor != null select pedido.Expedidor).Distinct().ToList());
            }
            else
            {
                destinatarios.AddRange((from pedido in pedidos where pedido.Recebedor == null && pedido.Destinatario != null select pedido.Destinatario).Distinct().ToList());
                destinatarios.AddRange((from pedido in pedidos where pedido.Recebedor != null select pedido.Recebedor).Distinct().ToList());
                remetentes.AddRange((from pedido in pedidos where pedido.Remetente != null select pedido.Remetente).Distinct().ToList());
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                DataVigencia = DateTime.Now,
                Empresa = repEmpresa.BuscarPorCodigo(cotacaoFreteCarregamento.Transportador),
                ModeloVeiculo = repModeloVeicularCarga.BuscarPorCodigo(cotacaoFreteCarregamento.ModeloVeicularCarga),
                NumeroPallets = pedidos.Sum(o => o.NumeroPaletes + o.NumeroPaletesFracionado),
                Peso = pedidos.Sum(o => o.PesoTotal),
                PesoLiquido = pedidos.Sum(o => o.PesoLiquidoTotal),
                NumeroEntregas = destinatarios.Distinct().Count(),
                PesoCubado = pedidos.Sum(o => o.PesoCubado),
                PesoPaletizado = pedidos.Sum(o => o.PesoTotalPaletes),
                Quantidades = new List<ParametrosCalculoFreteQuantidade>()
                {
                    new ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = pedidos.Sum(o => o.PesoTotal),
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    }
                },
                TipoCarga = repTipoDeCarga.BuscarPorCodigo(cotacaoFreteCarregamento.TipoDeCarga),
                TipoOperacao = repTipoOperacao.BuscarPorCodigo(cotacaoFreteCarregamento.TipoOperacao),
                ValorNotasFiscais = pedidos.Sum(o => o.ValorTotalNotasFiscais),
                Veiculo = repVeiculo.BuscarPorCodigo(cotacaoFreteCarregamento.Veiculo),
                Volumes = volumesProdutos,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                PagamentoTerceiro = false,
                Distancia = cotacaoFreteCarregamento.Distancia,
                Destinatarios = destinatarios,
                Destinos = destinatarios.Select(o => o.Localidade).ToList(),
                Remetentes = remetentes,
                Origens = remetentes.Select(o => o.Localidade).ToList(),
                DataBaseCRT = pedidos.Select(o => o.DataBaseCRT).FirstOrDefault(),
                Cubagem = pedidos.Sum(o => o.CubagemTotal),
                MaiorAlturaProdutoEmCentimetros = pedidos.Max(o => o.MaiorAlturaProdutoEmCentimetros),
                MaiorLarguraProdutoEmCentimetros = pedidos.Max(o => o.MaiorLarguraProdutoEmCentimetros),
                MaiorComprimentoProdutoEmCentimetros = pedidos.Max(o => o.MaiorComprimentoProdutoEmCentimetros),
                MaiorVolumeProdutoEmCentimetros = pedidos.Max(o => o.MaiorVolumeProdutoEmCentimetros),
                Rota = rotaFrete,
                Filial = (cotacaoFreteCarregamento.Filial == 0 ? null : repositorioFilial.BuscarPorCodigo(cotacaoFreteCarregamento.Filial))
            };

            if (parametrosCalculo.Cubagem > 0)
                parametrosCalculo.Quantidades.Add(new ParametrosCalculoFreteQuantidade()
                {
                    Quantidade = parametrosCalculo.Cubagem,
                    UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3
                });

            dadosCalculo = ObterDadosCalculoFreteMontagemCarga(parametrosCalculo, pedidos, pedidoProdutos, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracaoTMS);

            decimal valorTotalFrete = dadosCalculo.ValorFrete + dadosCalculo.ValorTotalComponentes;
            leadTime = dadosCalculo.LeadTime;

            if (!dadosCalculo.FreteCalculado || dadosCalculo.FreteCalculadoComProblemas)
                msgFrete = dadosCalculo.MensagemRetorno;

            return valorTotalFrete;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> CalcularFreteSimulacao(Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFrete, out string msgFrete, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            msgFrete = "";

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(cotacaoFrete.ModeloVeicularCarga);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                Peso = cotacaoFrete.PesoBruto,
                Filial = (cotacaoFrete.Filial > 0 ? repositorioFilial.BuscarPorCodigo(cotacaoFrete.Filial) : null),
                DataVigencia = DateTime.Now,
                TipoOperacao = (cotacaoFrete.TipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(cotacaoFrete.TipoOperacao) : null),
                Origens = new List<Dominio.Entidades.Localidade>() { repositorioLocalidade.BuscarPorCodigo(cotacaoFrete.Origem) },
                Destinos = repositorioLocalidade.BuscarPorCodigos(cotacaoFrete.Destinos),
                ModeloVeiculo = repositorioModeloVeicularCarga.BuscarPorCodigo(cotacaoFrete.ModeloVeicularCarga),
                Distancia = cotacaoFrete.Distancia,
                RementesEDestinatariosOpcionaisQuandoExistirLocalidade = true,
                Veiculo = new Dominio.Entidades.Veiculo() { ModeloVeicularCarga = modeloVeicularCarga }
            };

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadosCalculos = ObterDadosCalculosFrete(parametrosCalculo, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracaoTMS);

            return dadosCalculos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete CalcularFretePorJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);


            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            ParametrosCalculoFrete parametrosCalculoFrete = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, false, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, calcularFretePorJanelaCarregamentoTransportador: true);
            if (parametrosCalculoFrete == null)
                return null;
            DadosCalculoFrete dadosCalculo = null;
            if (tabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorCarga)
            {
                // Força o set da empresa, sobrescrevendo a informação que vinha da carga
                parametrosCalculoFrete.Empresa = cargaJanelaCarregamentoTransportador.Transportador;

                dadosCalculo = ObterDadosCalculoFrete(parametrosCalculoFrete, tipoServicoMultisoftware, unitOfWork, stringConexao, configuracao);
            }
            else if (tabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedido || tabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorMaiorValorPedido)
            {
                Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
                int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
                if (carga.Carregamento != null)
                    carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);
                StringBuilder mensagemRetorno = new StringBuilder();
                List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadosCalculoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                if (carga.Carregamento != null)
                    carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);

                for (int i = 0; i < cargaPedidos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, false, carregamentoPedidoNotaFiscals, 0);
                    parametrosCalculo.ParametrosCarga = parametrosCalculoFrete;
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                    dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                    if (tabelasCliente.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelasCliente[0];
                        if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                        dadosCalculoFrete.Add(dados);
                    }
                    else
                    {
                        return null;
                        //dadosCalculo.FreteCalculado = true;
                        //dadosCalculo.FreteCalculadoComProblemas = true;
                        //dadosCalculo.MensagemRetorno = mensagemRetorno.ToString();
                        //return dadosCalculo;
                    }
                }

                if (tabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedido)
                {
                    dadosCalculo = dadosCalculoFrete.FirstOrDefault();
                    dadosCalculo.FreteCalculado = true;
                    for (int i = 1; i < dadosCalculoFrete.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = dadosCalculoFrete[i];
                        dadosCalculo.ValorFrete += dados.ValorFrete;
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componenteExiste = (from obj in dadosCalculo.Componentes where obj.ComponenteFrete?.Codigo == componente.ComponenteFrete?.Codigo select obj).FirstOrDefault();
                            if (componenteExiste != null)
                                componenteExiste.ValorComponente += componente.ValorComponente;
                            else
                                dadosCalculo.Componentes.Add(componente);
                        }
                    }
                }
                else
                    dadosCalculo = (from obj in dadosCalculoFrete orderby obj.ValorTotal descending select obj).FirstOrDefault();

            }

            return dadosCalculo;
        }

        public void CalcularFretePorRota(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dadosRota, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            if (dadosRota.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.NaoEncontrada)
            {
                Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

                carga.PossuiPendencia = true;
                if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 23 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                retorno = serCargaRota.retornarDadosRotaNaoEncontrada(dadosRota);
            }
            else
            {
                Servicos.Embarcador.Carga.FreteRota serCargaFreteRota = new Servicos.Embarcador.Carga.FreteRota(unitOfWork);
                retorno = serCargaFreteRota.CalcularFretePorRota(carga, cargaPedidos, configuracao, tabelaFrete, unitOfWork, apenasVerificar, adicionarComponentesCarga, calculoFreteFilialEmissora, TipoServicoMultisoftware);
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota;

                if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                {
                    carga.PossuiPendencia = true;
                    if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                    //carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 22 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }
                else
                {
                    if (!apenasVerificar)
                    {
                        carga.PossuiPendencia = false;
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                        serCarga.InformarSituacaoCargaFreteValido(ref carga, TipoServicoMultisoftware, unitOfWork);
                    }
                }
            }

            if (!calculoFreteFilialEmissora)
                carga.ValorFrete = retorno != null ? retorno.valorFrete : 0;
            else
                carga.ValorFreteFilialEmissora = retorno != null ? retorno.valorFrete : 0;
        }

        public void AjustarCargaPedidoTabelaNaoExiste(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            //Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                cargaPedido.IncluirICMSBaseCalculo = true;

            if (!cargaPedido.Pedido.AdicionadaManualmente && !cargaPedido.Carga.DadosPagamentoInformadosManualmente && cargaPedido.RegraTomador == null && !(cargaPedido.Carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente ?? false))
            {
                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                    cargaPedido.Pedido.UsarTipoPagamentoNF = true;
                else
                    cargaPedido.Pedido.UsarTipoPagamentoNF = false;

                if ((from obj in pedidoXMLNotasFiscais where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).Count() > 0) //if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = (from obj in pedidoXMLNotasFiscais where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).FirstOrDefault()?.XMLNotaFiscal?.ModalidadeFrete; //repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);

                    if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                        cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;
                    else
                    {
                        cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    }
                }
                else
                {
                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                }
            }

            repCargaPedido.Atualizar(cargaPedido);
            repPedido.Atualizar(cargaPedido.Pedido);
        }

        public void SalvarValorManualFretePorPorPedido(List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete> lisCargaPedidoValoresDeFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, bool alteraValorFrete, bool alteraValorFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete();
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                throw new ServicoException(Localization.Resources.Cargas.Frete.NaoEPossivelAlterarValorDeFreteNaSituacaoAtualDaCarga + " (" + carga.DescricaoSituacaoCarga + ").");

            if (alteraValorFrete && (carga.TabelaFrete == null || !carga.TabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete))
                throw new ServicoException(Localization.Resources.Cargas.Frete.TabelaDeFretePrecisaPermitirAlteracaoDeValorDefretePorPedido);

            if (alteraValorFreteFilialEmissora && (carga.TabelaFreteFilialEmissora == null || !carga.TabelaFreteFilialEmissora.PermiteAlterarValorFretePedidoPosCalculoFrete))
                throw new ServicoException(Localization.Resources.Cargas.Frete.TabelaDeFreteDaFilialEmissoraPrecisaPermitirAlteracaoDeValorDefretePorPedido);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoNaoAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete cargaPedidoParaAlterar in lisCargaPedidoValoresDeFrete)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos.Where(x => x.Codigo == cargaPedidoParaAlterar.CodigoCargaPedido))
                {
                    cargaPedidoParaAlterar.ValorFreteAntesDaAlteracaoManual = cargaPedido.ValorFrete;
                    cargaPedidoParaAlterar.ValorFreteFilialEmissoraAntesDaAlteracaoManual = cargaPedido.ValorFreteFilialEmissora;
                    bool alterou = false;

                    if (cargaPedidoParaAlterar.FreteAlteradoManualmente())
                    {
                        cargaPedido.ValorFreteAntesAlteracaoManual = cargaPedido.ValorFrete;
                        cargaPedido.ValorFrete = cargaPedidoParaAlterar.ValorFrete;
                        cargaPedido.ValorFreteAPagar = cargaPedidoParaAlterar.ValorFrete;
                        cargaPedido.ValorFreteTabelaFrete = cargaPedidoParaAlterar.ValorFrete;
                        alterou = true;
                    }

                    if (cargaPedidoParaAlterar.FreteFilialEmissoraAlteradoManualmente())
                    {
                        cargaPedido.ValorFreteFilialEmissoraAntesAlteracaoManual = cargaPedido.ValorFreteFilialEmissora;
                        cargaPedido.ValorFreteFilialEmissora = cargaPedidoParaAlterar.ValorFreteFilialEmissora;
                        cargaPedido.ValorFreteAPagarFilialEmissora = cargaPedidoParaAlterar.ValorFreteFilialEmissora;
                        cargaPedido.ValorFreteTabelaFreteFilialEmissora = cargaPedidoParaAlterar.ValorFreteFilialEmissora;
                        alterou = true;
                    }

                    if (alterou)
                        repCargaPedido.Atualizar(cargaPedido);

                    if (carga.ValorBaseFrete < cargaPedido.ValorBaseFrete)
                        carga.ValorBaseFrete = cargaPedido.ValorBaseFrete;

                    if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                       || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado
                       || cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                        cargaPedidoAgrupados.Add(cargaPedido);
                    else
                        cargaPedidoNaoAgrupados.Add(cargaPedido);

                    if (cargaPedidoParaAlterar.FreteAlteradoManualmente() || cargaPedidoParaAlterar.FreteFilialEmissoraAlteradoManualmente())
                        repPedidoComponenteFrete.DeletarPorPedido(cargaPedido.Pedido.Codigo, alteraValorFreteFilialEmissora);
                }
            }

            // Recalcular impostos
            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> lstPedidoComponenteFrete = repPedidoComponenteFrete.BuscarPorCodigosPedido(cargaPedidos.Select(x => x.Pedido.Codigo).ToList(), alteraValorFreteFilialEmissora);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
            List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, alteraValorFreteFilialEmissora);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            svcRateioFrete.ZerarValoresDaCarga(carga, alteraValorFreteFilialEmissora, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoNaoAgrupados)
            {
                svcFreteCliente.RecalculaImpostoValorFreteManualPedidoNaoAgrupados(carga, cargasOrigem, cargaPedido, null, alteraValorFreteFilialEmissora, TipoServicoMultisoftware,
                    configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
            }

            if (cargaPedidoAgrupados.Count > 0)
            {
                svcRateioFrete.CalcularImpostosAgrupados(ref carga, cargasOrigem, cargaPedidoAgrupados, alteraValorFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora,
                    cargaPedidosValoresNotas, configuracaoTabelaFrete, configuracaoGeralCarga);
            }

            //Alterar composição do frete
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorCarga(carga.Codigo, alteraValorFreteFilialEmissora);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicaoFrete in cargaComposicaoFretes.Where(x => x.CargaPedido != null && x.CargaPedido.Pedido != null && x.CargaPedido.Pedido.Codigo == cargaPedido.Pedido.Codigo && x.ComponenteFrete == null))
                {
                    if (cargaComposicaoFrete.ComposicaoFreteFilialEmissora)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor do frete informado pelo operador.", " Valor Informado = " + cargaPedido.ValorFrete.ToString("n2"), cargaPedido.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor Manual informado pelo Operador", 0, cargaPedido.ValorFrete);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.AtualizarComposicaoFrete(carga, cargaComposicaoFrete, null, null, null, false, composicaoFrete, unitOfWork);
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor do frete informado pelo operador.", " Valor Informado = " + cargaPedido.ValorFreteFilialEmissora.ToString("n2"), cargaPedido.ValorFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor manual informado pelo Operador", 0, cargaPedido.ValorFreteFilialEmissora);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.AtualizarComposicaoFrete(carga, cargaComposicaoFrete, null, null, null, false, composicaoFrete, unitOfWork);
                    }
                }
            }

            if (alteraValorFrete)
            {
                carga.ValorICMS = cargaPedidos.Sum(obj => obj.ValorICMS);
                carga.ValorFrete = cargaPedidos.Sum(obj => obj.ValorFrete);
                carga.ValorFreteAPagar = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);
                carga.ValorFreteLiquido = carga.ValorFrete;
                carga.ValorFreteEmbarcador = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);

                carga.ValorIBSEstadual = cargaPedidos.Sum(obj => obj.ValorIBSEstadual);
                carga.ValorIBSMunicipal = cargaPedidos.Sum(obj => obj.ValorIBSMunicipal);
                carga.ValorCBS = cargaPedidos.Sum(obj => obj.ValorCBS);
            }

            if (alteraValorFreteFilialEmissora)
            {
                carga.ValorICMSFilialEmissora = cargaPedidos.Sum(obj => obj.ValorICMSFilialEmissora);
                carga.ValorFreteFilialEmissora = cargaPedidos.Sum(obj => obj.ValorFreteFilialEmissora);
                carga.ValorFreteAPagarFilialEmissora = cargaPedidos.Sum(obj => obj.ValorFreteAPagarFilialEmissora);

                carga.ValorIBSEstadualFilialEmissora = cargaPedidos.Sum(obj => obj.ValorIBSEstadualFilialEmissora);
                carga.ValorIBSMunicipalFilialEmissora = cargaPedidos.Sum(obj => obj.ValorIBSMunicipalFilialEmissora);
                carga.ValorCBSFilialEmissora = cargaPedidos.Sum(obj => obj.ValorCBSFilialEmissora);
            }

            carga.PossuiPendencia = false;
        }

        public bool CalcularFretePorClientePedido(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioPontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);

            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.FreteCTeSubcontratacao serFreteCTeSubcontratacao = new FreteCTeSubcontratacao(unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete svcCargaAprovacaoFrete = new CargaAprovacaoFrete(unitOfWork, configuracao);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && !apenasVerificar)//zera o valor, pois o rateio é feio nesse momento quando calculo é por pedido.
                svcRateioFrete.ZerarValoresDaCarga(carga, calculoFreteFilialEmissora, unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCarga = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, calculoFreteFilialEmissora, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao);
            if (parametrosCarga == null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = mensagemRetorno.Insert(0, "Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet)").ToString();
                return true;
            }
            parametrosCarga.NumeroPedidos = cargaPedidos.Count;
            parametrosCarga.FormulaRateio = cargaPedidos.FirstOrDefault().FormulaRateio;

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga = new List<DadosCalculoFreteComponente>();
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                repCargaPedidoComponenteFrete.DeletarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
            List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);

            int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
            if (carga.Carregamento != null)
                carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = null;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido> fretesCargaPedido = new List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido>();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelaFreteClientes = new List<TabelaFreteCliente>();

            if (!Servicos.Embarcador.Carga.CTeSimplificado.ValidarCTeSimplificado(cargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracao))
                Servicos.Embarcador.Carga.CTeGlobalizado.ValidarCTeGlobalizadoPorDestinatario(cargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracao);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
            if (carga.Carregamento != null)
                carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);

            bool cargaPossuiAjudante = cargaPedidos.Any(obj => obj.Pedido.Ajudante);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePedagio = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteDescarga = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValorPedagioIntegracao = repCargaConsultaValorPedagioIntegracao.ConsultaIntegracaoPorCarga(carga.Codigo, SituacaoIntegracao.Integrado);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga.Codigo);
            bool removeuComponentesAnteriormente = false;
            if (cargaValePedagios != null && cargaValePedagios.Count > 0)
                removeuComponentesAnteriormente = cargaValePedagios.Any(x => x.ValidaCompraRemoveuComponentes == true);//caso ja tenha removido componentes de vale pedagio, nao será adicionado novamente.

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                bool ultimoPedido = (i == cargaPedidos.Count - 1);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, carregamentoPedidoNotaFiscals, 0);

                if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos && cargaPossuiAjudante)
                    parametrosCalculo.ParametrosCarga.NecessarioAjudante = true;

                parametrosCalculo.ParametrosCarga = parametrosCarga;

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                if (!svcFreteCliente.PermiteCalcularFrete(tabelasCliente) && (cargaPedido.TipoContratacaoCarga != TipoContratacaoCarga.SubContratada))
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                        retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

                        if (tabelasCliente.Count <= 0)
                            retorno.mensagem = mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete para a tabela de frete " + tabelaFrete.Descricao + " compatível com as configurações do pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + ".\n").ToString();
                        else if (tabelasCliente.Count > 1)
                            retorno.mensagem = mensagemRetorno.Insert(0, "Foi encontrada mais configuração de frete disponível para o pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + ". na tabela de frete " + tabelaFrete.Descricao + ".").ToString();
                        else if (tabelasCliente[0].Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao)
                            retorno.mensagem = mensagemRetorno.Insert(0, "A tabela de frete " + tabelaFrete.Descricao + " ainda não foi aprovada e não pode ser utilizada no pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + ".").ToString();


                        if (!apenasVerificar)
                        {
                            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                            carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                            if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                            {
                                carga.PossuiPendencia = true;
                                if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 21 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                            }

                            carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);
                            if (!calculoFreteFilialEmissora)
                                carga.TabelaFrete = null;
                            else
                                carga.TabelaFreteFilialEmissora = null;

                            repCarga.Atualizar(carga);


                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaPedido.Codigo, calculoFreteFilialEmissora);
                            if (cargaPedidoTabelaCliente != null)
                                repCargaPedidoTabelaFreteCliente.Deletar(cargaPedidoTabelaCliente);

                            if (!calculoFreteFilialEmissora)
                            {
                                if (pedidoNotasFiscais == null)
                                    pedidoNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                                AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoNotasFiscais, unitOfWork);

                                Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
                            }
                        }

                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                            return true;
                    }
                }
                else if ((cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada) && ((tabelasCliente.Count == 0) || (tabelasCliente[0] == null)))
                {
                    retorno = serFreteCTeSubcontratacao.BuscarTabelaFreteSubcontratado(carga, cargaPedidos, configuracao, apenasVerificar, TipoServicoMultisoftware, unitOfWork);
                    if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                    {
                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaSubContratacao;
                        return true;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                    dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = (from obj in tabelasCliente where obj.Codigo == tabelasCliente[0].Codigo select obj).FirstOrDefault();
                    if (tabelaFreteCliente == null)
                    {
                        tabelaFreteCliente = tabelasCliente[0];
                        tabelasCliente.Add(tabelaFreteCliente);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido();
                    freteCargaPedido.cargaPedido = cargaPedido;
                    freteCargaPedido.parametrosCalculo = parametrosCalculo;
                    freteCargaPedido.tabelaFreteCliente = tabelaFreteCliente;

                    if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                        svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                    else
                        svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);

                    freteCargaPedido.dadosCalculoFrete = dados;
                    freteCargaPedido.ultimoPedido = ultimoPedido;
                    fretesCargaPedido.Add(freteCargaPedido);
                }

                if (cargaConsultaValorPedagioIntegracao != null && cargaConsultaValorPedagioIntegracao.ValorValePedagio > 0)
                    serCargaPedido.ValidarValorPedagioPorCargaPedido(cargaPedido, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware, componenteFretePedagio, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos, removeuComponentesAnteriormente, tabelaFrete);

                serCargaPedido.ValidarValorDescargaPorCargaPedido(carga, cargaPedido, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware, componenteFreteDescarga, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos);

            }

            if (!calculoFreteFilialEmissora)
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    carga.TabelaFrete = tabelaFrete;
            }
            else
                carga.TabelaFreteFilialEmissora = tabelaFrete;

            if (tabelaFrete != null && (tabelaFrete.UtilizaModeloVeicularVeiculo || tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo))
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = null;
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
                if (modeloVeicularCalculo != null)
                    carga.ModeloVeicularCarga = modeloVeicularCalculo;
                else
                    modeloVeicularCalculo = carga.ModeloVeicularCarga;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelaCargaPedidoCargas = repCargaPedidoTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosModalidades = repPedidoXMLNotaFiscal.BuscarModalidadesDeFretePadraoCargaPedidoPorCarga(carga.Codigo);

            bool abriuTransacao = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(unitOfWork);
            decimal maiorValorBaseFreteDosPedidos = 0m;

            decimal pesoTotalParaCalculoFatorCubagem = svcRateio.ObterPesoTotalCubadoFatorCubagem(pedidoNotasFiscais);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = repositorioPontosPassagem.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso> rateiosPonderacaoPesoDistancia = svcRateio.CalcularRateioPonderacaoDistanciaPeso(cargaPedidos, pontosPassagem);

            if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido> fretesPedidosPagos = (from obj in fretesCargaPedido where obj.cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente select obj).ToList();
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = (from obj in fretesCargaPedido select obj.dadosCalculoFrete).OrderByDescending(obj => obj.ValorBase).FirstOrDefault();

                maiorValorBaseFreteDosPedidos = dadosCalculoFrete?.ValorBase ?? 0;

                decimal valorTotalFOB = (from obj in fretesCargaPedido where obj.cargaPedido.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente select (obj.dadosCalculoFrete.ValorFrete + obj.dadosCalculoFrete.ValorTotalComponentes)).Sum();
                decimal diferenca = maiorValorBaseFreteDosPedidos - valorTotalFOB;
                if (diferenca < 0)
                    diferenca = 0;

                if (fretesPedidosPagos.Count > 0)
                {
                    decimal valorTotalRateado = 0;
                    Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido ultimoFreteCargaPedido = fretesPedidosPagos.LastOrDefault();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesPedidosPagos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = freteCargaPedido.cargaPedido;
                        decimal valorRateioOriginal = 0;
                        freteCargaPedido.dadosCalculoFrete.ValorFixo = 0;
                        Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso rateioPonderacaoDistanciaPeso = rateiosPonderacaoPesoDistancia.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).FirstOrDefault();

                        decimal pesoParaCalculoFatorCubagem = 0;

                        if (cargaPedido.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                            pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(cargaPedido.FormulaRateio?.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, cargaPedido.Pedido.QtVolumes, cargaPedido.Peso, repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo)?.Sum(x => x.XMLNotaFiscal.MetrosCubicos) ?? 0);

                        freteCargaPedido.dadosCalculoFrete.ValorFrete = svcRateio.AplicarFormulaRateio(cargaPedido.FormulaRateio, diferenca, fretesPedidosPagos.Count(), 1, fretesPedidosPagos.Sum(obj => obj.cargaPedido.Peso), cargaPedido.Peso, cargaPedido.Pedido.ValorTotalNotasFiscais, fretesPedidosPagos.Sum(obj => obj.cargaPedido.Pedido.ValorTotalNotasFiscais), 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.PesoLiquido, fretesPedidosPagos.Sum(obj => obj.cargaPedido.PesoLiquido), cargaPedido.Pedido.QtVolumes, fretesPedidosPagos.Sum(obj => obj.cargaPedido.Pedido.QtVolumes), rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        if ((maiorValorBaseFreteDosPedidos - valorTotalFOB) < 0 && tabelaFrete.ValorMinimoDiferencaFreteNegativo > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoMin = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                            Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicaoMin, "Diferença negativa (" + freteCargaPedido.dadosCalculoFrete.ValorFrete.ToString("n2") + "), aplicado frete mínimo (" + tabelaFrete.ValorMinimoDiferencaFreteNegativo.ToString("n2") + ")", tabelaFrete.ValorMinimoDiferencaFreteNegativo.ToString("n2"), tabelaFrete.ValorMinimoDiferencaFreteNegativo);
                            freteCargaPedido.dadosCalculoFrete.ValorFrete = tabelaFrete.ValorMinimoDiferencaFreteNegativo;
                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicaoMin);
                            diferenca += freteCargaPedido.dadosCalculoFrete.ValorFrete;
                        }

                        valorTotalRateado += freteCargaPedido.dadosCalculoFrete.ValorFrete;

                        freteCargaPedido.dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                        if (freteCargaPedido.cargaPedido.Codigo == ultimoFreteCargaPedido.cargaPedido.Codigo)
                        {
                            decimal diferencao = diferenca - valorTotalRateado;
                            freteCargaPedido.dadosCalculoFrete.ValorFrete = freteCargaPedido.dadosCalculoFrete.ValorFrete + diferencao;

                            if (freteCargaPedido.dadosCalculoFrete.ValorFrete < 0)
                            {
                                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                                retorno.mensagem = mensagemRetorno.Insert(0, "O valor do frete residual não pode ser superior ao valor do frete pois neste caso o CT-e será gerado com valor negativo oque fiscalmente não é possível (" + tabelaFrete.Descricao + ")").ToString();
                                return true;
                            }
                        }
                        freteCargaPedido.dadosCalculoFrete.Componentes = new List<DadosCalculoFreteComponente>();
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Diferença do maior valor (" + maiorValorBaseFreteDosPedidos.ToString("n2") + ") base entre as tabelas para fretes CIF menos o total de FOB (" + valorTotalFOB.ToString("n2") + ") rateado por " + (freteCargaPedido.cargaPedido.FormulaRateio?.Descricao ?? "Peso"), diferenca + " / Formula de Rateio = " + freteCargaPedido.dadosCalculoFrete.ValorFrete, freteCargaPedido.dadosCalculoFrete.ValorFrete);
                        //SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.Valor, 0m, null, tabelaFreteCliente, unitOfWork);
                        if (dadosCalculoFrete?.ComposicaoValorBaseFrete != null && dadosCalculoFrete.ComposicaoValorBaseFrete.Count > 0)
                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.AddRange(dadosCalculoFrete.ComposicaoValorBaseFrete);

                        freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicao);
                    }
                }
                else
                {
                    if (diferenca != (decimal)0)
                    {
                        string decontoAliquota = "";
                        if (carga.Empresa.AliquotaICMSNegociado > 0)
                        {
                            diferenca = diferenca * ((100 - carga.Empresa.AliquotaICMSNegociado) / 100);
                            decontoAliquota = " (valor com o desconto da alíquota de ICMS de " + carga.Empresa.AliquotaICMSNegociado.ToString("n2") + ")";
                        }


                        decimal valorTotalRateado = 0;
                        Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido ultimoFreteCargaPedido = fretesCargaPedido.LastOrDefault();
                        foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesCargaPedido)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = freteCargaPedido.cargaPedido;
                            freteCargaPedido.dadosCalculoFrete.ValorFixo = 0;
                            decimal valorRateioOriginal = 0;
                            Dominio.ObjetosDeValor.Embarcador.Carga.RateioPonderacaoDistanciaPeso rateioPonderacaoDistanciaPeso = rateiosPonderacaoPesoDistancia.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).FirstOrDefault();
                            decimal pesoParaCalculoFatorCubagem = 0;

                            if (cargaPedido.FormulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(cargaPedido.FormulaRateio?.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, cargaPedido.Pedido.QtVolumes, cargaPedido.Peso, repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo)?.Sum(x => x.XMLNotaFiscal.MetrosCubicos) ?? 0);

                            freteCargaPedido.dadosCalculoFrete.ValorFreteResidual = svcRateio.AplicarFormulaRateio(cargaPedido.FormulaRateio, diferenca, fretesCargaPedido.Count(), 1, fretesCargaPedido.Sum(obj => obj.cargaPedido.Peso), cargaPedido.Peso, cargaPedido.Pedido.ValorTotalNotasFiscais, fretesCargaPedido.Sum(obj => obj.cargaPedido.Pedido.ValorTotalNotasFiscais), 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.PesoLiquido, fretesCargaPedido.Sum(obj => obj.cargaPedido.PesoLiquido), cargaPedido.Pedido.QtVolumes, fretesCargaPedido.Sum(obj => obj.cargaPedido.Pedido.QtVolumes), rateioPonderacaoDistanciaPeso, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                            valorTotalRateado += freteCargaPedido.dadosCalculoFrete.ValorFreteResidual;
                            if (freteCargaPedido.cargaPedido.Codigo == ultimoFreteCargaPedido.cargaPedido.Codigo)
                            {
                                decimal diferencao = diferenca - valorTotalRateado;
                                freteCargaPedido.dadosCalculoFrete.ValorFreteResidual = freteCargaPedido.dadosCalculoFrete.ValorFreteResidual + diferencao;
                            }
                            //freteCargaPedido.dadosCalculoFrete.Componentes = new List<DadosCalculoFreteComponente>();
                            //freteCargaPedido.dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();

                            Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "(Lote Residual) Diferença do maior valor (" + maiorValorBaseFreteDosPedidos.ToString("n2") + ") base entre as tabelas para fretes CIF menos o total de FOB (" + valorTotalFOB.ToString("n2") + ") rateado por " + (freteCargaPedido.cargaPedido.FormulaRateio?.Descricao ?? "Peso") + decontoAliquota, diferenca + " / Formula de Rateio = " + freteCargaPedido.dadosCalculoFrete.ValorFrete, freteCargaPedido.dadosCalculoFrete.ValorFrete);
                            //SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.Valor, 0m, null, tabelaFreteCliente, unitOfWork);
                            if (dadosCalculoFrete?.ComposicaoValorBaseFrete != null && dadosCalculoFrete.ComposicaoValorBaseFrete.Count > 0)
                                freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.AddRange(dadosCalculoFrete.ComposicaoValorBaseFrete);

                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicao);
                        }
                    }
                }
            }

            if (!apenasVerificar)
                carga.MaiorValorBaseFreteDosPedidos = maiorValorBaseFreteDosPedidos;

            if (!calculoFreteFilialEmissora)
            {
                bool utilizaContaRazao = carga.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false;
                if (!utilizaContaRazao)
                {
                    repCargaPedidoContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);
                    carga.PossuiPendenciaConfiguracaoContabil = false;
                }
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesCargaPedido)
            {
                svcFreteCliente.SetarTabelaFreteCargaPedido(carga, cargasOrigem, freteCargaPedido.cargaPedido, freteCargaPedido.parametrosCalculo, freteCargaPedido.dadosCalculoFrete, freteCargaPedido.tabelaFreteCliente, apenasVerificar, TipoServicoMultisoftware, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, ref componentesPorCarga, freteCargaPedido.ultimoPedido, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, tabelaCargaPedidoCargas, cargaPedidosModalidades, configuracaoGeralCarga);

                if (carga.ValorBaseFrete < freteCargaPedido.cargaPedido.ValorBaseFrete)
                    carga.ValorBaseFrete = freteCargaPedido.cargaPedido.ValorBaseFrete;

                if (freteCargaPedido.cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                   || freteCargaPedido.cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado
                   || freteCargaPedido.cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                    pedidosAgrupados.Add(freteCargaPedido.cargaPedido);
            }

            if (abriuTransacao)
                unitOfWork.CommitChanges();

            if (pedidosAgrupados.Count > 0)
            {
                abriuTransacao = false;
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork.Start();
                    abriuTransacao = true;
                }
                serRateioFrete.CalcularImpostosAgrupados(ref carga, cargasOrigem, pedidosAgrupados, calculoFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNotas, configuracaoTabelaFrete, configuracaoGeralCarga);

                if (abriuTransacao)
                    unitOfWork.CommitChanges();
            }

            serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, calculoFreteFilialEmissora, cargaPedidosComponentesFreteCarga, unitOfWork);
            serRateioFrete.AcrescentarValoresDaCargaAgrupada(carga, calculoFreteFilialEmissora, cargaPedidos, unitOfWork);

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                carga.PossuiPendencia = false;

                Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, calculoFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork, configuracao);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(carga.Codigo, calculoFreteFilialEmissora);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteISS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePisCONFIS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);

                abriuTransacao = false;
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork.Start();
                    abriuTransacao = true;
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    svcRateioFrete.GerarComponenteICMS(cargaPedido, calculoFreteFilialEmissora, componenteFreteICMS, cargaPedidosComponentesFreteCargaImpostos, unitOfWork);
                    if (!calculoFreteFilialEmissora)
                        svcRateioFrete.GerarComponenteISS(cargaPedido, componenteFreteISS, cargaPedidosComponentesFreteCargaImpostos, false, unitOfWork);

                    if (!calculoFreteFilialEmissora)
                        svcRateioFrete.GerarComponentePisCofins(cargaPedido, componenteFretePisCONFIS, cargaPedidosComponentesFreteCargaImpostos, unitOfWork);
                }

                if (abriuTransacao)
                    unitOfWork.CommitChanges();

                if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                    bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                    if (!carga.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    else
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.GravarInfo("Atualizou a situação para calculo frete 20 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }

                carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                carga.MotivoPendencia = "";

                svcFreteCliente.CriarTabelaFreteCliente(carga, cargaPedidos, apenasVerificar, calculoFreteFilialEmissora, TipoServicoMultisoftware);

                CriarCargaComponentes(carga, TipoServicoMultisoftware, cargaPedidosComponentesFreteCarga, cargaPedidos.FirstOrDefault().ObterTomador(), calculoFreteFilialEmissora, unitOfWork);

                svcRateioFrete.GerarComponenteICMS(carga, cargaPedidos, false, calculoFreteFilialEmissora, unitOfWork);
                if (!calculoFreteFilialEmissora)
                    svcRateioFrete.GerarComponenteISS(carga, cargaPedidos, unitOfWork);

                if (!calculoFreteFilialEmissora)
                    svcRateioFrete.GerarComponentePisCofins(carga, cargaPedidos, false, unitOfWork);

                retorno = svcFreteCliente.ObterDadosTabelaFreteClientePorPedido(carga, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware);

                svcCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.TabelaFrete, TipoServicoMultisoftware);

            }
            else
            {
                if (!calculoFreteFilialEmissora)
                    carga.ValorFreteTabelaFrete = cargaPedidos.Sum(o => o.ValorFreteTabelaFrete);
                else
                    carga.ValorFreteTabelaFreteFilialEmissora = cargaPedidos.Sum(o => o.ValorFreteTabelaFreteFilialEmissora);

                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete()
                {
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido
                };
            }

            repCarga.Atualizar(carga);

            return false;
        }

        public bool CalcularFretePorClienteMaiorValorPedido(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.FreteCTeSubcontratacao serFreteCTeSubcontratacao = new FreteCTeSubcontratacao(unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && !apenasVerificar && !calculoFreteFilialEmissora)//zera o valor, pois o rateio é feio nesse momento quando calculo é por pedido.
                svcRateioFrete.ZerarValoresDaCarga(carga, calculoFreteFilialEmissora, unitOfWork);


            int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
            if (carga.Carregamento != null)
                carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = new List<TabelaFreteCliente>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
            if (carga.Carregamento != null)
                carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteMaiorValor = null;
            decimal maiorvalor = 0;

            if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
            {
                for (int i = 0; i < cargaPedidos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                    decimal pesoCalculo = 0;
                    if (tabelaFrete.ParametroBase != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso && tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                        pesoCalculo = 1; //Quando a tabela de frete não for por Parametro Base Peso e o tipo for Por Maior valor dos pedidos busca a tabela de desconsiderando o peso do pedido

                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, carregamentoPedidoNotaFiscals, pesoCalculo);

                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                    if (tabelasCliente.Count > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                        dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                        dados.CodigoCargaPedido = cargaPedido.Codigo;

                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = (from obj in tabelasFreteCliente where obj.Codigo == tabelasCliente[0].Codigo select obj).FirstOrDefault();
                        if (tabelaFreteCliente == null)
                        {
                            tabelaFreteCliente = tabelasCliente[0];
                            tabelasFreteCliente.Add(tabelasCliente[0]);
                        }

                        if (tabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);

                        decimal valorTotal = dados.ValorFrete + dados.ValorTotalComponentes;
                        if (valorTotal > maiorvalor)
                        {
                            tabelaFreteMaiorValor = tabelaFreteCliente;
                            maiorvalor = valorTotal;
                        }
                    }
                }
            }
            else
            {
                Servicos.Embarcador.Carga.FretePedidoAgrupado serFretePedidoAgrupado = new FretePedidoAgrupado(configuracao, unitOfWork, TipoServicoMultisoftware);
                List<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> cargaPedidosAgrupados = cargaPedidos.GroupBy(cargaPedido => new { Remetente = cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0D, cargaPedido.TipoTomador }).Select(grupo => grupo.ToList()).ToList();
                for (int i = 0; i < cargaPedidosAgrupados.Count; i++)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCalculo = cargaPedidosAgrupados[i];
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = serFretePedidoAgrupado.ObterParametrosCalculoFrete(tabelaFrete, carga, cargaPedidosCalculo, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, 0m, null);

                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

                    if (tabelasCliente.Count > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                        dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                        dados.CodigoCargaPedido = cargaPedidosCalculo.FirstOrDefault().Codigo;

                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = (from obj in tabelasFreteCliente where obj.Codigo == tabelasCliente[0].Codigo select obj).FirstOrDefault();
                        if (tabelaFreteCliente == null)
                        {
                            tabelaFreteCliente = tabelasCliente[0];
                            tabelasFreteCliente.Add(tabelasCliente[0]);
                        }

                        if (tabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, TipoServicoMultisoftware, configuracao);

                        decimal valorTotal = dados.ValorFrete + dados.ValorTotalComponentes;
                        if (valorTotal > maiorvalor)
                        {
                            tabelaFreteMaiorValor = tabelaFreteCliente;
                            maiorvalor = valorTotal;
                        }
                    }
                }
            }

            if (!svcFreteCliente.PermiteCalcularFrete(tabelaFreteMaiorValor))
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                    retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    if (tabelaFreteMaiorValor == null)
                        retorno.mensagem = mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete para a tabela de frete " + tabelaFrete.Descricao + " compatível com as configurações do pedido.\n").ToString();
                    else if (tabelaFreteMaiorValor.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao)
                        retorno.mensagem = mensagemRetorno.Insert(0, "A tabela de frete " + tabelaFrete.Descricao + " ainda não foi aprovada e não pode ser utilizada nesta carga.").ToString();

                    if (!apenasVerificar)
                    {
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;
                            if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 19 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = null;
                        else
                            carga.TabelaFreteFilialEmissora = null;

                        repCarga.Atualizar(carga);
                    }

                    return true;
                }
            }
            else
            {
                if (CalcularFretePorCliente(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, configuracao, tabelaFreteMaiorValor))
                    return true;
            }

            return false;
        }

        public bool CalcularFretePorClienteMaiorDistanciaPedido(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioPontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && !apenasVerificar && !calculoFreteFilialEmissora)//zera o valor, pois o rateio é feio nesse momento quando calculo é por pedido.
                svcRateioFrete.ZerarValoresDaCarga(carga, calculoFreteFilialEmissora, unitOfWork);


            int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
            if (carga.Carregamento != null)
                carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = new List<TabelaFreteCliente>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
            if (carga.Carregamento != null)
                carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteMaiorDistancia = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem = repositorioPontosPassagem.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontoPassagem = cargaRotaFretePontosPassagem?.Count > 0 ? cargaRotaFretePontosPassagem.Where(obj => obj.TipoPontoPassagem == TipoPontoPassagem.Entrega).OrderByDescending(obj => obj.Ordem).FirstOrDefault() : null;

            if (cargaRotaFretePontoPassagem != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = ObterPedidoMasDistante(cargaPedidos, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, carregamentoPedidoNotaFiscals, 0m);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);
                if (tabelasCliente?.Count > 0)
                    tabelaFreteMaiorDistancia = tabelasCliente.FirstOrDefault();
            }

            if (!svcFreteCliente.PermiteCalcularFrete(tabelaFreteMaiorDistancia))
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                    retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    if (tabelaFreteMaiorDistancia == null)
                        retorno.mensagem = mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete para a tabela de frete " + tabelaFrete.Descricao + " compatível com as configurações do pedido.\n").ToString();
                    else if (tabelaFreteMaiorDistancia.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao)
                        retorno.mensagem = mensagemRetorno.Insert(0, "A tabela de frete " + tabelaFrete.Descricao + " ainda não foi aprovada e não pode ser utilizada nesta carga.").ToString();

                    if (!apenasVerificar)
                    {
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;
                            if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 17 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = null;
                        else
                            carga.TabelaFreteFilialEmissora = null;

                        repCarga.Atualizar(carga);
                    }

                    return true;
                }
            }
            else
            {
                if (CalcularFretePorCliente(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, configuracao, tabelaFreteMaiorDistancia))
                    return true;
            }

            return false;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete CalcularFreteParaCargaFechamento(Dominio.Entidades.Embarcador.Cargas.CargaFechamento cargaFechamento, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(cargaFechamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorCarga(carga.Codigo);

            StringBuilder mensagemRetorno = new StringBuilder();
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = new List<TabelaFrete>();

            if (carga.TabelaFrete != null)
                tabelasFrete.Add(carga.TabelaFrete);

            if (ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargasPedido, false, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao);
                if (parametrosCalculo == null)
                {
                    dadosCalculoFrete.FreteCalculadoComProblemas = true;
                    dadosCalculoFrete.MensagemRetorno = "Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis  (exemplo, somente pedidos de pallet)";
                }

                if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                {

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();
                    if (tabelaFreteCliente == null)
                    {
                        dadosCalculoFrete.FreteCalculadoComProblemas = true;
                        dadosCalculoFrete.MensagemRetorno = mensagemRetorno.ToString();
                    }
                    else
                    {
                        if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                            svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                        else
                            svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                        dadosCalculoFrete.FreteCalculado = true;
                    }
                }
                else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                         tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido ||
                         tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                {

                    parametrosCalculo.NumeroPedidos = cargasPedido.Count;
                    parametrosCalculo.FormulaRateio = cargasPedido.FirstOrDefault().FormulaRateio;

                    List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadoCalculoFretePedidos = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();

                    for (int i = 0; i < cargasPedido.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargasPedido[i];


                        int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
                        if (carga.Carregamento != null)
                            carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

                        List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                        if (carga.Carregamento != null)
                            carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);


                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoPedido = ObterParametrosCalculoFretePorPedido(tabelaFrete, carga, cargaPedido, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, false, carregamentoPedidoNotaFiscals, 0);
                        parametrosCalculoPedido.ParametrosCarga = parametrosCalculo;

                        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculoPedido, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();

                        if (tabelaCliente == null)
                        {
                            dadosCalculoFrete.FreteCalculadoComProblemas = true;
                            dadosCalculoFrete.MensagemRetorno = mensagemRetorno.ToString();
                            if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                                tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                                break;
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

                            if (tabelaCliente.TabelaFrete.ParametroBase.HasValue)
                                svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaCliente, tipoServicoMultisoftware, configuracao);
                            else
                                svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaCliente, tipoServicoMultisoftware, configuracao);

                            dadoCalculoFretePedidos.Add(dados);
                        }
                    }

                    if (dadoCalculoFretePedidos.Count > 0)
                    {
                        decimal valorTotal = 0;
                        if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete pedidoCalculado in dadoCalculoFretePedidos)
                            {
                                decimal valorCalculado = pedidoCalculado.ValorFrete + pedidoCalculado.ValorTotalComponentes;
                                if (valorTotal < valorCalculado)
                                {
                                    valorTotal = valorCalculado;
                                    dadosCalculoFrete = pedidoCalculado;
                                }
                            }
                        }
                        else if (!dadosCalculoFrete.FreteCalculadoComProblemas)
                        {
                            dadosCalculoFrete.Componentes = new List<DadosCalculoFreteComponente>();
                            dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete pedidoCalculado in dadoCalculoFretePedidos)
                            {
                                dadosCalculoFrete.ComposicaoFrete.AddRange(pedidoCalculado.ComposicaoFrete);
                                dadosCalculoFrete.Componentes.AddRange(pedidoCalculado.Componentes);
                                dadosCalculoFrete.ValorFrete += pedidoCalculado.ValorFrete;
                                dadosCalculoFrete.ValorFixo += pedidoCalculado.ValorFixo;
                            }
                        }
                    }
                    dadosCalculoFrete.FreteCalculado = true;
                }

            }

            return dadosCalculoFrete;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorCTeTerceiroIndividual(Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, string descricaoItemPeso, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, calculoFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, calculoFreteFilialEmissora);

            List<int> codigosPedidoCTesParaSubcontratacao = repPedidoCTeParaSubcontratacao.BuscarCodigosPorCargaPedido(cargaPedido.Codigo);

            foreach (int codigoPedidoCTeParaSubcontratacao in codigosPedidoCTesParaSubcontratacao)
            {
                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCodigoComFetch(codigoPedidoCTeParaSubcontratacao);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>() { pedidoCTeParaSubContratacao };

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = CalcularFretePorCTesTerceiro(pedidoCTesParaSubcontratacao, pedidoNotasFiscais, descricaoItemPeso, valoresCargaPedido, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, cargaPedidoContaContabilContabilizacaos, unitOfWork, configuracao, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);

                if (retorno != null) //ocorreu algum problema no cálculo do frete (não achou tabela, etc...)
                    return retorno;

                unitOfWork.FlushAndClear();
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorCTeTerceiroAgrupado(Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, string descricaoItemPeso, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, calculoFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, calculoFreteFilialEmissora);


            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(carga.Codigo);


            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete> gruposParticipantes = repPedidoCTeParaSubcontratacao.BuscarParticipantesPorCargaPedido(cargaPedido);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete grupoParticipante in gruposParticipantes)
            {
                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubcontratacao.BuscarPorCargaPedidoEParticipantes(cargaPedido.Codigo, grupoParticipante.Tomador, grupoParticipante.Remetente, grupoParticipante.Expedidor, grupoParticipante.Recebedor, grupoParticipante.Destinatario, cargaPedido.TipoEmissaoCTeParticipantes);

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = CalcularFretePorCTesTerceiro(pedidoCTesParaSubcontratacao, pedidoNotasFiscais, descricaoItemPeso, valoresCargaPedido, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, cargaPedidoContaContabilContabilizacaos, unitOfWork, configuracao, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);

                if (retorno != null) //ocorreu algum problema no cálculo do frete (não achou tabela, etc...)
                    return retorno;

                unitOfWork.FlushAndClear();
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorNotaFiscalAgrupada(Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosRateioFrete parametrosGeraisRateioFrete)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete> gruposParticipantes = null;

            if (!cargaPedido.Pedido.UsarTipoPagamentoNF)
                gruposParticipantes = repPedidoXMLNotaFiscal.BuscarParticipantesPorCargaPedido(cargaPedido.Codigo, cargaPedido.TipoEmissaoCTeParticipantes);
            else
                gruposParticipantes = repPedidoXMLNotaFiscal.BuscarParticipantesEModalidadesPorCargaPedido(cargaPedido.Codigo, cargaPedido.TipoEmissaoCTeParticipantes);

            int countGrupos = gruposParticipantes.Count;

            for (int i = 0; i < countGrupos; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete grupoParticipante = gruposParticipantes[i];

                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoEParticipantes(cargaPedido.Codigo, grupoParticipante.Remetente, grupoParticipante.Expedidor, grupoParticipante.Recebedor, grupoParticipante.Destinatario, grupoParticipante.Modalidade, cargaPedido.TipoEmissaoCTeParticipantes);

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = CalcularFretePorNotasFiscais(pedidoXMLNotasFiscais, cargaPedidos, valoresCargaPedido, carga, cargaPedido, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoModalidadesPagamento, ref parametrosGeraisRateioFrete, i == (countGrupos - 1));

                if (retorno != null) //ocorreu algum problema no cálculo do frete (não achou tabela, etc...)
                    return retorno;

                unitOfWork.FlushAndClear();
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorNotaFiscalIndividual(Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosRateioFrete parametrosGeraisRateioFrete)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<int> codigosNotasFiscais = repPedidoXMLNotaFiscal.BuscarCodigosPorCargaPedido(cargaPedido.Codigo);

            int codigoUltimoPedidoXMLNotaFiscal = codigosNotasFiscais.LastOrDefault();

            foreach (int codigoPedidoXMLNotaFiscal in codigosNotasFiscais)
            {
                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigoComFetch(codigoPedidoXMLNotaFiscal);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>() { pedidoXMLNotaFiscal };

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = CalcularFretePorNotasFiscais(pedidoXMLNotasFiscais, cargaPedidos, valoresCargaPedido, carga, cargaPedido, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoModalidadesPagamento, ref parametrosGeraisRateioFrete, codigoPedidoXMLNotaFiscal == codigoUltimoPedidoXMLNotaFiscal);

                if (retorno != null) //ocorreu algum problema no cálculo do frete (não achou tabela, etc...)
                    return retorno;

                unitOfWork.FlushAndClear();
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorNotasFiscais(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, ref Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosRateioFrete parametrosGeraisRateioFrete, bool ultimoRegistro)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorNotasFiscais(tabelaFrete, carga, cargaPedido, pedidoXMLNotasFiscais, unitOfWork, TipoServicoMultisoftware);

            System.Text.StringBuilder mensagemRetorno = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

            if ((tabelasCliente.Count <= 0 || tabelasCliente.Count > 1))
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                {
                    pedidoXMLNotaFiscal.ValorFreteTabelaFrete = 0m;
                    pedidoXMLNotaFiscal.ValorFreteTabelaFreteFilialEmissora = 0m;

                    repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                }

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    string mensagem = string.Empty;

                    if (tabelasCliente.Count <= 0)
                        mensagem = mensagemRetorno.Insert(0, $"Não foi localizada uma tabela de frete para a configuração {tabelaFrete?.Descricao} compatível com a(s) nota(s) fiscal(is) {string.Join(", ", pedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal.Numero))}.\n").ToString();
                    else
                        mensagem = $"Foi encontrada mais de uma tabela de frete para a configuração {tabelaFrete?.Descricao} disponível para a(s) nota(s) fiscal(is) {string.Join(", ", pedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal.Numero))}.";

                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete
                    {
                        tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente,
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete,
                        mensagem = mensagem
                    };

                    if (svcCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    {
                        carga.PossuiPendencia = true;
                        if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 16 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }

                    carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                    if (!calculoFreteFilialEmissora)
                        carga.TabelaFrete = null;
                    else
                        carga.TabelaFreteFilialEmissora = null;

                    repCarga.Atualizar(carga);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaPedido.Codigo, calculoFreteFilialEmissora);

                    if (cargaPedidoTabelaCliente != null)
                        repCargaPedidoTabelaFreteCliente.Deletar(cargaPedidoTabelaCliente);

                    if (!calculoFreteFilialEmissora)
                    {
                        AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoXMLNotasFiscais, unitOfWork);
                        Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
                    }

                    return retorno;
                }
            }
            else
            {
                svcFreteCliente.SetarTabelaFreteNotasFiscais(ref cargaPedido, parametrosCalculo, pedidoXMLNotasFiscais, tabelasCliente.FirstOrDefault(), apenasVerificar, TipoServicoMultisoftware, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, cargaPedido.FormulaRateio, configuracao, cargaPedidoModalidadesPagamento, ref parametrosGeraisRateioFrete, ultimoRegistro);
            }

            if (!calculoFreteFilialEmissora)
                valoresCargaPedido.totalFreteTabelaFrete += pedidoXMLNotasFiscais.Sum(o => o.ValorFreteTabelaFrete);
            else
                valoresCargaPedido.totalFreteTabelaFrete += pedidoXMLNotasFiscais.Sum(o => o.ValorFreteTabelaFreteFilialEmissora);

            if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                if (!calculoFreteFilialEmissora)
                {
                    valoresCargaPedido.totalFrete += pedidoXMLNotasFiscais.Sum(o => o.ValorFrete);
                    valoresCargaPedido.totalBaseCalculoISS += pedidoXMLNotasFiscais.Sum(o => o.BaseCalculoISS);
                    valoresCargaPedido.totalISS += pedidoXMLNotasFiscais.Sum(o => o.ValorISS);
                    valoresCargaPedido.totalRetencaoISS += pedidoXMLNotasFiscais.Sum(o => o.ValorRetencaoISS);
                    valoresCargaPedido.totalBaseCalculoICMS += pedidoXMLNotasFiscais.Sum(o => o.BaseCalculoICMS);
                    valoresCargaPedido.totalICMS += pedidoXMLNotasFiscais.Sum(o => o.ValorICMS);
                    valoresCargaPedido.totalCreditoPresumido += pedidoXMLNotasFiscais.Sum(o => o.ValorCreditoPresumido);
                    valoresCargaPedido.totalICMSSomarValorFrete += pedidoXMLNotasFiscais.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMS);
                    valoresCargaPedido.totalPisCofinsSomarValorFrete += pedidoXMLNotasFiscais.Sum(o => o.ValorPis + o.ValorCofins);
                    valoresCargaPedido.totalICMSInclusoSomarValorFrete += pedidoXMLNotasFiscais.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMSIncluso);
                    valoresCargaPedido.totalISSSomarValorFrete += pedidoXMLNotasFiscais.Where(o => o.IncluirISSBaseCalculo).Sum(o => o.ValorISS);

                    valoresCargaPedido.totalBaseCalculoIBSCBS += pedidoXMLNotasFiscais.Sum(o => o.BaseCalculoIBSCBS);
                    valoresCargaPedido.totalIBSEstadual += pedidoXMLNotasFiscais.Sum(o => o.ValorIBSEstadual);
                    valoresCargaPedido.totalIBSMunicipal += pedidoXMLNotasFiscais.Sum(o => o.ValorIBSMunicipal);
                    valoresCargaPedido.totalCBS += pedidoXMLNotasFiscais.Sum(o => o.ValorCBS);
                }
                else
                {
                    valoresCargaPedido.totalFrete += pedidoXMLNotasFiscais.Sum(o => o.ValorFreteFilialEmissora);
                    valoresCargaPedido.totalBaseCalculoICMS += pedidoXMLNotasFiscais.Sum(o => o.BaseCalculoICMSFilialEmissora);
                    valoresCargaPedido.totalICMS += pedidoXMLNotasFiscais.Sum(o => o.ValorICMSFilialEmissora);
                    valoresCargaPedido.totalCreditoPresumido += pedidoXMLNotasFiscais.Sum(o => o.ValorCreditoPresumidoFilialEmissora);
                    valoresCargaPedido.totalICMSSomarValorFrete += pedidoXMLNotasFiscais.Where(o => o.IncluirICMSBaseCalculoFilialEmissora && o.CSTFilialEmissora != "60").Sum(o => o.ValorICMSFilialEmissora);
                    valoresCargaPedido.totalICMSInclusoSomarValorFrete += pedidoXMLNotasFiscais.Where(o => o.IncluirICMSBaseCalculoFilialEmissora && o.CSTFilialEmissora != "60").Sum(o => o.ValorICMSFilialEmissora);

                    valoresCargaPedido.totalBaseCalculoIBSCBS += pedidoXMLNotasFiscais.Sum(o => o.BaseCalculoIBSCBSFilialEmissora);
                    valoresCargaPedido.totalIBSEstadual += pedidoXMLNotasFiscais.Sum(o => o.ValorIBSEstadualFilialEmissora);
                    valoresCargaPedido.totalIBSMunicipal += pedidoXMLNotasFiscais.Sum(o => o.ValorIBSMunicipalFilialEmissora);
                    valoresCargaPedido.totalCBS += pedidoXMLNotasFiscais.Sum(o => o.ValorCBSFilialEmissora);
                }
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorCTesTerceiro(List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais, string descricaoItemPeso, Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCTesTerceiro(tabelaFrete, carga, cargaPedido, pedidoCTesParaSubcontratacao, descricaoItemPeso, unitOfWork, TipoServicoMultisoftware);

            System.Text.StringBuilder mensagemRetorno = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

            if ((tabelasCliente.Count <= 0 || tabelasCliente.Count > 1))
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    string mensagem = string.Empty;

                    if (tabelasCliente.Count <= 0)
                        mensagem = mensagemRetorno.Insert(0, $"Não foi localizada uma tabela de frete para a configuração {tabelaFrete?.Descricao} compatível com o(s) CT-e(s) {string.Join(", ", pedidoCTesParaSubcontratacao.Select(o => o.CTeTerceiro.Numero))}.\n").ToString();
                    else
                        mensagem = $"Foi encontrada mais de uma tabela de frete para a configuração {tabelaFrete?.Descricao} disponível para o(s) CT-e(s) {string.Join(", ", pedidoCTesParaSubcontratacao.Select(o => o.CTeTerceiro.Numero))}.";

                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete
                    {
                        tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente,
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete,
                        mensagem = mensagem
                    };

                    if (svcCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    {
                        carga.PossuiPendencia = true;
                        if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 15 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }

                    carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                    if (!calculoFreteFilialEmissora)
                        carga.TabelaFrete = null;
                    else
                        carga.TabelaFreteFilialEmissora = null;

                    repCarga.Atualizar(carga);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaPedido.Codigo, calculoFreteFilialEmissora);

                    if (cargaPedidoTabelaCliente != null)
                        repCargaPedidoTabelaFreteCliente.Deletar(cargaPedidoTabelaCliente);

                    if (!calculoFreteFilialEmissora)
                    {
                        if (pedidoNotasFiscais == null)
                            pedidoNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                        AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoNotasFiscais, unitOfWork);
                        Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
                    }

                    return retorno;
                }
            }
            else
            {
                svcFreteCliente.SetarTabelaFreteCTesTerceiro(ref cargaPedido, parametrosCalculo, pedidoCTesParaSubcontratacao, tabelasCliente.FirstOrDefault(), apenasVerificar, TipoServicoMultisoftware, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, cargaPedido.FormulaRateio, configuracao, cargaPedidoContaContabilContabilizacaos, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
            }

            if (!calculoFreteFilialEmissora)
                valoresCargaPedido.totalFreteTabelaFrete += pedidoCTesParaSubcontratacao.Sum(o => o.ValorFreteTabelaFrete);

            if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                if (!calculoFreteFilialEmissora)
                {
                    valoresCargaPedido.totalFrete += pedidoCTesParaSubcontratacao.Sum(o => o.ValorFrete);
                    valoresCargaPedido.totalBaseCalculoISS += pedidoCTesParaSubcontratacao.Sum(o => o.BaseCalculoISS);
                    valoresCargaPedido.totalISS += pedidoCTesParaSubcontratacao.Sum(o => o.ValorISS);
                    valoresCargaPedido.totalRetencaoISS += pedidoCTesParaSubcontratacao.Sum(o => o.ValorRetencaoISS);
                    valoresCargaPedido.totalBaseCalculoICMS += pedidoCTesParaSubcontratacao.Sum(o => o.BaseCalculoICMS);
                    valoresCargaPedido.totalICMS += pedidoCTesParaSubcontratacao.Sum(o => o.ValorICMS);
                    valoresCargaPedido.totalPisCofinsSomarValorFrete += pedidoCTesParaSubcontratacao.Sum(o => o.ValorPis + o.ValorCofins);
                    //valoresCargaPedido.totalCreditoPresumido += pedidoCTesParaSubcontratacao.Sum(o => o.ValorCreditoPresumido);
                    valoresCargaPedido.totalICMSSomarValorFrete += pedidoCTesParaSubcontratacao.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMS);
                    valoresCargaPedido.totalICMSInclusoSomarValorFrete += pedidoCTesParaSubcontratacao.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMSIncluso);
                    valoresCargaPedido.totalISSSomarValorFrete += pedidoCTesParaSubcontratacao.Where(o => o.IncluirISSBaseCalculo).Sum(o => o.ValorISS);

                    valoresCargaPedido.totalBaseCalculoIBSCBS += pedidoCTesParaSubcontratacao.Sum(o => o.BaseCalculoIBSCBS);
                    valoresCargaPedido.totalIBSEstadual += pedidoCTesParaSubcontratacao.Sum(o => o.ValorIBSEstadual);
                    valoresCargaPedido.totalIBSMunicipal += pedidoCTesParaSubcontratacao.Sum(o => o.ValorIBSMunicipal);
                    valoresCargaPedido.totalCBS += pedidoCTesParaSubcontratacao.Sum(o => o.ValorCBS);
                }
            }

            return null;
        }

        public bool CalcularFretePorDocumentoEmitido(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);

            if (!apenasVerificar)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
                Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete repRotaEmbarcadorTabelaFrete = new Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);


                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = null;

                Servicos.Embarcador.Carga.CTeSubContratacao svcCTeSubcontratacao = new CTeSubContratacao(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);
                Servicos.Embarcador.Carga.CargaAprovacaoFrete svcCargaAprovacaoFrete = new CargaAprovacaoFrete(unitOfWork, configuracao);
                Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento = repPedidoXMLNotaFiscal.BuscarModalidadesDeFretePadraoCargaPedidoPorCarga(carga.Codigo);

                StringBuilder mensagemRetorno = new StringBuilder();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    serRateioFrete.ZerarValoresDaCarga(carga, calculoFreteFilialEmissora, unitOfWork);
                    if (!calculoFreteFilialEmissora)
                    {
                        if (tabelaFrete.Componentes.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && o.TipoDocumentoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido))
                            Servicos.Embarcador.Carga.CargaPedido.CriarPreviaDocumentoCarga(carga, unitOfWork, TipoServicoMultisoftware, configuracao);
                    }
                }
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
                bool tipoContratacaoCargaInvalido = cargaPedidos.Any(o => o.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada);
                bool tipoRateioDocumentosInvalido = cargaPedidos.Any(o => o.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado && o.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada && o.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos && o.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual);

                if (tipoContratacaoCargaInvalido || tipoRateioDocumentosInvalido)
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                        retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

                        if (tipoContratacaoCargaInvalido)
                            retorno.mensagem = "Não é possível calcular o frete por documento para uma carga que possua subcontratação/redespacho e notas fiscais.";
                        else if (tipoRateioDocumentosInvalido)
                            retorno.mensagem = "Não é possível calcular o frete por documento para uma carga configurada para ratear os documentos por pedido.";

                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;

                            if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 14 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                        carga.MotivoPendencia = Utilidades.String.Left(retorno.mensagem, 2000);

                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = null;
                        else
                            carga.TabelaFreteFilialEmissora = null;

                        repCarga.Atualizar(carga);

                        bool abriuTransacao = false;
                        if (!unitOfWork.IsActiveTransaction())
                        {
                            unitOfWork.Start();
                            abriuTransacao = true;
                        }

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaPedido.Codigo, calculoFreteFilialEmissora);
                            if (cargaPedidoTabelaCliente != null)
                                repCargaPedidoTabelaFreteCliente.Deletar(cargaPedidoTabelaCliente);

                            if (!calculoFreteFilialEmissora)
                            {
                                if (pedidoNotasFiscais == null)
                                    pedidoNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                                AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoNotasFiscais, unitOfWork);
                            }

                        }

                        if (abriuTransacao)
                            unitOfWork.CommitChanges();

                        if (!calculoFreteFilialEmissora)
                            Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
                    }

                    return true;
                }

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    repCargaComponenteFrete.DeletarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

                carga.ConfiguracaoTabelaFretePorPedido = carga.TipoOperacao?.ConfiguracaoTabelaFretePorPedido ?? false;

                repCarga.Atualizar(carga);

                Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete rotaEmbarcadorTabelaFrete = null;

                if (!carga.ConfiguracaoTabelaFretePorPedido && carga.Rota != null)
                    rotaEmbarcadorTabelaFrete = repRotaEmbarcadorTabelaFrete.BuscarPorRotaFixa(carga.Rota.Codigo, tabelaFrete.Codigo);

                decimal totalFreteCarga = 0m;
                decimal totalISSCarga = 0m;
                decimal totalRetencaoISSCarga = 0m;
                decimal totalBaseCalculoICMSCarga = 0m;
                decimal totalICMSCarga = 0m;
                decimal totalFreteTabelaFreteCarga = 0m;
                decimal totalFreteAPagarCarga = 0m;

                decimal totalValorIBSEstadualCarga = 0m;
                decimal totalValorIBSMunicipalCarga = 0m;
                decimal totalValorCBSCarga = 0m;

                decimal totalICMSComponenteCarga = 0m;
                decimal totalPISCONFISComponenteCarga = 0m;
                decimal totalISSComponenteCarga = 0m;

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosRateioFrete parametrosGeraisRateioFrete = null;

                if (rotaEmbarcadorTabelaFrete != null && rotaEmbarcadorTabelaFrete.ComponenteFrete != null)
                {
                    bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, rotaEmbarcadorTabelaFrete.ComponenteFrete);

                    parametrosGeraisRateioFrete = new ParametrosRateioFrete()
                    {
                        FormulaRateio = cargaPedidos.FirstOrDefault()?.FormulaRateio,
                        MetrosCubicos = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCarga(carga.Codigo),
                        Peso = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo),
                        QuantidadeNotasFiscais = repPedidoXMLNotaFiscal.ContarPorCarga(carga.Codigo),
                        PesoCubado = repPedidoXMLNotaFiscal.BuscarPesoCubadoPorCarga(carga.Codigo),
                        PesoLiquido = repPedidoXMLNotaFiscal.BuscarPesoLiquidoPorCarga(carga.Codigo),
                        Componentes = new List<DadosCalculoFreteComponente>()
                        {
                            new DadosCalculoFreteComponente()
                            {
                                 ComponenteFrete = rotaEmbarcadorTabelaFrete.ComponenteFrete,
                                 ValorComponente = rotaEmbarcadorTabelaFrete.ValorFixoRota,
                                 AcrescentaValorTotalAReceber = rotaEmbarcadorTabelaFrete.ComponenteFrete.AcrescentaValorTotalAReceber,
                                 IncluirBaseCalculoICMS = true,
                                 NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : rotaEmbarcadorTabelaFrete.ComponenteFrete.NaoSomarValorTotalAReceber) ?? false,
                                 DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : rotaEmbarcadorTabelaFrete.ComponenteFrete?.DescontarValorTotalAReceber) ?? false,
                                 DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                                 ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0,
                                 DescontarValorTotalAReceber = rotaEmbarcadorTabelaFrete.ComponenteFrete?.DescontarValorTotalAReceber ?? false,
                                 NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : rotaEmbarcadorTabelaFrete.ComponenteFrete.NaoSomarValorTotalPrestacao) ?? false,
                                 SomarComponenteFreteLiquido = rotaEmbarcadorTabelaFrete.ComponenteFrete?.SomarComponenteFreteLiquido ?? false,
                                 DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : rotaEmbarcadorTabelaFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false,
                                 TipoComponenteFrete = rotaEmbarcadorTabelaFrete.ComponenteFrete.TipoComponenteFrete,
                                 TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor
                            }
                        }
                    };
                }

                for (int i = 0; i < cargaPedidos.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete valoresCargaPedido = new Dominio.ObjetosDeValor.Embarcador.Frete.ValoresGeraisCalculoFrete();

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                    if (carga.ConfiguracaoTabelaFretePorPedido)
                    {
                        List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = ObterTabelasFrete(carga, unitOfWork, TipoServicoMultisoftware, configuracao, ref mensagemRetorno, false, null, false, false);

                        if (!ValidarQuantidadeTabelasFreteDisponivel(ref retorno, ref carga, tabelasFrete, mensagemRetorno, apenasVerificar, unitOfWork, configuracao, cargaPedido))
                            return true;

                        tabelaFrete = tabelasFrete[0];

                        if (calculoFreteFilialEmissora)
                            cargaPedido.TabelaFreteFilialEmissora = tabelaFrete;
                        else
                            cargaPedido.TabelaFrete = tabelaFrete;
                    }
                    else
                    {
                        if (calculoFreteFilialEmissora)
                            cargaPedido.TabelaFreteFilialEmissora = null;
                        else
                            cargaPedido.TabelaFrete = null;
                    }

                    string descricaoItemPeso = null;

                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                        descricaoItemPeso = svcCTeSubcontratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

                    if (!calculoFreteFilialEmissora)
                    {
                        cargaPedido.BaseCalculoICMS = 0m;
                        cargaPedido.ValorICMS = 0m;
                        cargaPedido.ValorISS = 0m;
                        cargaPedido.ValorRetencaoISS = 0m;
                        cargaPedido.BaseCalculoISS = 0m;
                        cargaPedido.ValorCreditoPresumido = 0m;
                        cargaPedido.DescontarICMSDoValorAReceber = false;

                        servicoCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedido, true);
                    }
                    else
                    {
                        cargaPedido.BaseCalculoICMSFilialEmissora = 0m;
                        cargaPedido.ValorICMSFilialEmissora = 0m;
                        cargaPedido.ValorCreditoPresumidoFilialEmissora = 0m;
                        cargaPedido.DescontarICMSDoValorAReceber = false;

                        servicoCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedido, true);
                    }

                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        repCargaPedidoComponenteFrete.DeletarPorCargaPedido(cargaPedido.Codigo, calculoFreteFilialEmissora);

                    if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                        || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                    {
                        if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                        {
                            retorno = CalcularFretePorNotaFiscalAgrupada(valoresCargaPedido, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoModalidadesPagamento, parametrosGeraisRateioFrete);

                            if (retorno != null) //ocorreu algum problema no cálculo (não achou tabela, etc...)
                                return true;
                        }
                        else //Calcula por CT-e para subcontratação/redespacho
                        {
                            retorno = CalcularFretePorCTeTerceiroAgrupado(valoresCargaPedido, descricaoItemPeso, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoModalidadesPagamento, pedidoNotasFiscais);

                            if (retorno != null) //ocorreu algum problema no cálculo (não achou tabela, etc...)
                                return true;
                        }
                    }
                    else
                    {
                        if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                        {
                            retorno = CalcularFretePorNotaFiscalIndividual(valoresCargaPedido, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoModalidadesPagamento, parametrosGeraisRateioFrete);

                            if (retorno != null) //ocorreu algum problema no cálculo (não achou tabela, etc...)
                                return true;
                        }
                        else //Calcula por CT-e para subcontratação/redespacho
                        {
                            retorno = CalcularFretePorCTeTerceiroIndividual(valoresCargaPedido, descricaoItemPeso, carga, cargaPedido, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, unitOfWork, configuracao, cargaPedidoContaContabilContabilizacaos, pedidoNotasFiscais, componenteICMS, componentePisCofins, pedidoCTeParaSubContratacaoNotasFiscais);

                            if (retorno != null) //ocorreu algum problema no cálculo (não achou tabela, etc...)
                                return true;
                        }
                    }

                    cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                    if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        if (!calculoFreteFilialEmissora)
                            cargaPedido.ValorFreteTabelaFrete = valoresCargaPedido.totalFreteTabelaFrete;
                        else
                            cargaPedido.ValorFreteTabelaFreteFilialEmissora = valoresCargaPedido.totalFreteTabelaFrete;
                    }
                    else
                    {
                        decimal valorTotalComponentesPedidoFreteLiquido = repCargaPedidoComponenteFrete.BuscarValorComponentesFreteLiquido(cargaPedido.Codigo, calculoFreteFilialEmissora);
                        decimal valorTotalComponentesPedido = repCargaPedidoComponenteFrete.BuscarValorComponentes(cargaPedido.Codigo, calculoFreteFilialEmissora);

                        if (!calculoFreteFilialEmissora)
                        {
                            cargaPedido.ValorFreteTabelaFrete = valoresCargaPedido.totalFrete + (valoresCargaPedido.totalICMSInclusoSomarValorFrete > 0m ? valoresCargaPedido.totalICMSInclusoSomarValorFrete : valoresCargaPedido.totalICMSSomarValorFrete) + valoresCargaPedido.totalISSSomarValorFrete + valorTotalComponentesPedido;
                            cargaPedido.ValorFreteAPagar = cargaPedido.ValorFreteTabelaFrete;
                            cargaPedido.ValorFrete = valoresCargaPedido.totalFrete + valorTotalComponentesPedidoFreteLiquido;

                            cargaPedido.BaseCalculoICMS = valoresCargaPedido.totalBaseCalculoICMS;
                            cargaPedido.ValorICMS = valoresCargaPedido.totalICMS;
                            cargaPedido.ValorCreditoPresumido = valoresCargaPedido.totalCreditoPresumido;

                            cargaPedido.BaseCalculoISS = valoresCargaPedido.totalBaseCalculoISS;
                            cargaPedido.ValorISS = valoresCargaPedido.totalISS;
                            cargaPedido.ValorRetencaoISS = valoresCargaPedido.totalRetencaoISS;

                            servicoCargaPedido.PreencherValoresImpostoIBSCBS(cargaPedido, valoresCargaPedido.totalBaseCalculoIBSCBS, valoresCargaPedido.totalIBSEstadual, valoresCargaPedido.totalIBSMunicipal, valoresCargaPedido.totalCBS);
                        }
                        else
                        {
                            cargaPedido.ValorFreteTabelaFreteFilialEmissora = valoresCargaPedido.totalFrete + (valoresCargaPedido.totalICMSInclusoSomarValorFrete > 0m ? valoresCargaPedido.totalICMSInclusoSomarValorFrete : valoresCargaPedido.totalICMSSomarValorFrete) + valoresCargaPedido.totalISSSomarValorFrete + valorTotalComponentesPedido;
                            cargaPedido.ValorFreteAPagarFilialEmissora = cargaPedido.ValorFreteTabelaFrete;
                            cargaPedido.ValorFreteFilialEmissora = valoresCargaPedido.totalFrete + valorTotalComponentesPedidoFreteLiquido;

                            cargaPedido.BaseCalculoICMSFilialEmissora = valoresCargaPedido.totalBaseCalculoICMS;
                            cargaPedido.ValorICMSFilialEmissora = valoresCargaPedido.totalICMS;
                            cargaPedido.ValorCreditoPresumidoFilialEmissora = valoresCargaPedido.totalCreditoPresumido;

                            servicoCargaPedido.PreencherValoresImpostoIBSCBSFilialEmissora(cargaPedido, valoresCargaPedido.totalBaseCalculoIBSCBS, valoresCargaPedido.totalIBSEstadual, valoresCargaPedido.totalIBSMunicipal, valoresCargaPedido.totalCBS);
                        }

                        repCargaPedido.Atualizar(cargaPedido);

                        svcRateioFrete.GerarComponenteICMS(cargaPedido, valoresCargaPedido.totalICMSSomarValorFrete, calculoFreteFilialEmissora, unitOfWork, valoresCargaPedido.totalICMSInclusoSomarValorFrete);

                        if (!calculoFreteFilialEmissora)
                            svcRateioFrete.GerarComponenteISS(cargaPedido, valoresCargaPedido.totalISSSomarValorFrete, unitOfWork);

                        if (!calculoFreteFilialEmissora)
                            svcRateioFrete.GerarComponentePisCofins(cargaPedido, valoresCargaPedido.totalPisCofinsSomarValorFrete, unitOfWork);

                        if (valoresCargaPedido.totalICMSInclusoSomarValorFrete > 0)
                            totalICMSComponenteCarga += valoresCargaPedido.totalICMSInclusoSomarValorFrete;
                        else
                            totalICMSComponenteCarga += valoresCargaPedido.totalICMSSomarValorFrete;

                        totalPISCONFISComponenteCarga += valoresCargaPedido.totalPisCofinsSomarValorFrete;
                        totalISSComponenteCarga += valoresCargaPedido.totalISSSomarValorFrete;
                        totalFreteCarga += cargaPedido.ValorFrete;
                        totalFreteAPagarCarga += cargaPedido.ValorFreteAPagar;
                        totalBaseCalculoICMSCarga += cargaPedido.BaseCalculoICMS;
                        totalICMSCarga += cargaPedido.ValorICMS;
                        totalISSCarga += cargaPedido.ValorISS;
                        totalRetencaoISSCarga += cargaPedido.ValorRetencaoISS;

                        totalValorIBSEstadualCarga += cargaPedido.ValorIBSEstadual;
                        totalValorIBSMunicipalCarga += cargaPedido.ValorIBSMunicipal;
                        totalValorCBSCarga += cargaPedido.ValorCBS;
                    }

                    totalFreteTabelaFreteCarga += cargaPedido.ValorFreteTabelaFrete;
                }

                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                if (!calculoFreteFilialEmissora)
                    carga.ValorFreteTabelaFrete = totalFreteTabelaFreteCarga;
                else
                    carga.ValorFreteTabelaFreteFilialEmissora = totalFreteTabelaFreteCarga;

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    svcRateioFrete.GerarComponenteICMS(carga, totalICMSComponenteCarga, calculoFreteFilialEmissora, unitOfWork, 0m);

                    if (!calculoFreteFilialEmissora)
                        svcRateioFrete.GerarComponentePisCofins(carga, totalPISCONFISComponenteCarga, unitOfWork);

                    if (!calculoFreteFilialEmissora)
                    {
                        svcRateioFrete.GerarComponenteISS(carga, totalISSComponenteCarga, unitOfWork);

                        carga.ValorFrete = totalFreteCarga;
                        carga.ValorFreteLiquido = totalFreteCarga;
                        carga.ValorFreteAPagar = totalFreteAPagarCarga;
                        carga.ValorICMS = totalICMSCarga;
                        carga.ValorISS = totalISSCarga;
                        carga.ValorRetencaoISS = totalRetencaoISSCarga;
                        carga.ValorIBSEstadual = totalValorIBSEstadualCarga;
                        carga.ValorIBSMunicipal = totalValorIBSMunicipalCarga;
                        carga.ValorCBS = totalValorCBSCarga;

                        carga.TabelaFrete = tabelaFrete;
                        carga.PossuiPendencia = false;
                    }
                    else
                    {
                        carga.ValorFreteFilialEmissora = totalFreteCarga;
                        carga.ValorFreteAPagarFilialEmissora = totalFreteAPagarCarga;
                        carga.ValorICMSFilialEmissora = totalICMSCarga;

                        carga.ValorIBSEstadualFilialEmissora = totalValorIBSEstadualCarga;
                        carga.ValorIBSMunicipalFilialEmissora = totalValorIBSMunicipalCarga;
                        carga.ValorCBSFilialEmissora = totalValorCBSCarga;

                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = tabelaFrete;
                        else
                            carga.TabelaFreteFilialEmissora = tabelaFrete;

                        carga.PossuiPendencia = false;
                    }

                    if (tabelaFrete != null && (tabelaFrete.UtilizaModeloVeicularVeiculo || tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = null;
                        if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                            modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                        else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                            modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
                        if (modeloVeicularCalculo != null)
                            carga.ModeloVeicularCarga = modeloVeicularCalculo;

                        if (modeloVeicularCalculo == null)
                            modeloVeicularCalculo = carga.ModeloVeicularCarga;
                    }

                    if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                    {
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                        bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                        if (!carga.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                        else
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 13 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }

                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.MotivoPendencia = "";

                    svcFreteCliente.CriarTabelaFreteCliente(carga, cargaPedidos, apenasVerificar, calculoFreteFilialEmissora, TipoServicoMultisoftware);
                    svcCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.TabelaFrete, TipoServicoMultisoftware);
                }

                repCarga.Atualizar(carga);
            }

            if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete()
                {
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido
                };
            }
            else
                retorno = svcFreteCliente.ObterDadosTabelaFreteClientePorPedido(carga, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware);

            return false;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo ObterParametrosContratoFreteTransportadorValorFreteMinimo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete)
        {
            List<Dominio.Entidades.Cliente> remetentes = parametrosCalculoFrete.Remetentes ?? new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = parametrosCalculoFrete.Destinatarios ?? new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Localidade> origens = parametrosCalculoFrete.Origens ?? new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = parametrosCalculoFrete.Destinos ?? new List<Dominio.Entidades.Localidade>();

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametros = new ParametrosContratoFreteTransportadorValorFreteMinimo();

            parametros.CodigoContratoFreteTransportador = contratoFreteTransportador.Codigo;
            parametros.CodigoModeloVeicularCarga = parametrosCalculoFrete.ModeloVeiculo?.Codigo ?? 0;
            parametros.CodigoTipoCarga = parametrosCalculoFrete.TipoCarga?.Codigo ?? 0;
            parametros.ListaCodigoLocalidadeDestino = (from o in destinos select o.Codigo).Distinct().ToList();
            parametros.ListaCodigoLocalidadeOrigem = (from o in origens select o.Codigo).Distinct().ToList();
            parametros.ListaCpfCnpjClienteDestino = destinatarios.Where(o => o != null).Select(o => o.CPF_CNPJ).Distinct().ToList();
            parametros.ListaCpfCnpjClienteOrigem = remetentes.Where(o => o != null).Select(o => o.CPF_CNPJ).Distinct().ToList();
            parametros.ListaUfDestino = (from o in destinos where o.Estado != null select o.Estado.Sigla).Distinct().ToList();
            parametros.ListaUfOrigem = (from o in origens where o.Estado != null select o.Estado.Sigla).Distinct().ToList();

            return parametros;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorCTesTerceiro(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, string descricaoItemPeso, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao svcSubcontratacao = new CTeSubContratacao(_unitOfWork);

            bool possuiComponentePorQuantidadeDocumentos = tabelaFrete.Componentes.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && o.TipoDocumentoQuantidadeDocumentos.HasValue && o.TipoDocumentoQuantidadeDocumentos.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido);

            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;

            if (possuiComponentePorQuantidadeDocumentos)
                quantidadesDocumentosEmitir = Servicos.Embarcador.Carga.CargaPedido.ObterQuantidadeDocumentosEmitir(null, null, null, pedidoCTesParaSubcontratacao, unidadeTrabalho, tipoServicoMultisoftware);

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();

            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();

            if (tabelaFrete.UtilizarParticipantePedidoParaCalculo)
            {
                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido.Expedidor != null && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                {
                    remetentes.Add(cargaPedido.Expedidor);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Expedidor.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }
                else if (cargaPedido.Pedido.Remetente != null)
                {
                    remetentes.Add(cargaPedido.Pedido.Remetente);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? cargaPedido.Pedido.Remetente.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }

                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                {
                    destinatarios.Add(cargaPedido.Recebedor);

                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Recebedor.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }
                else if (cargaPedido.Pedido.Destinatario != null)
                {
                    destinatarios.Add(cargaPedido.Pedido.Destinatario);

                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? cargaPedido.Pedido.Destinatario.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }

                if (cargaPedido.Pedido.Origem != null)
                    origens.Add(cargaPedido.Pedido.Origem);

                if (cargaPedido.Pedido.Destino != null)
                    destinos.Add(cargaPedido.Pedido.Destino);
            }
            else
            {
                Dominio.Entidades.Cliente remetente = pedidoCTesParaSubcontratacao.Where(o => o.CTeTerceiro.Expedidor != null).Select(o => o.CTeTerceiro.Expedidor.Cliente).FirstOrDefault();
                Dominio.Entidades.Cliente destinatario = pedidoCTesParaSubcontratacao.Where(o => o.CTeTerceiro.Recebedor != null).Select(o => o.CTeTerceiro.Recebedor.Cliente).FirstOrDefault();

                if (remetente == null && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor))
                    remetente = cargaPedido.Expedidor;

                if (destinatario == null && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                    destinatario = cargaPedido.Recebedor;

                if (remetente == null)
                {
                    remetente = pedidoCTesParaSubcontratacao.Where(o => o.CTeTerceiro.Remetente != null).Select(o => o.CTeTerceiro.Remetente.Cliente).FirstOrDefault();

                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? remetente.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }
                else
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(remetente.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }

                if (destinatario == null)
                {
                    destinatario = pedidoCTesParaSubcontratacao.Where(o => o.CTeTerceiro.Destinatario != null).Select(o => o.CTeTerceiro.Destinatario.Cliente).FirstOrDefault();

                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? destinatario.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }
                else
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(destinatario.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }

                if (remetente != null)
                {
                    remetentes.Add(remetente);
                    origens.Add(remetente.Localidade);
                }

                if (destinatario != null)
                {
                    destinatarios.Add(destinatario);
                    destinos.Add(destinatario.Localidade);
                }
            }

            decimal peso = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.Peso);
            decimal pesoCubado = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.PesoCubado);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
            {
                DataColeta = cargaPedido.Pedido.DataInicialColeta,
                DataFinalViagem = cargaPedido.Pedido.DataFinalViagemFaturada,
                DataInicialViagem = cargaPedido.Pedido.DataInicialViagemFaturada,
                DataVigencia = (tabelaFrete.ValidarPorDataCarregamento && carga.DataCarregamentoCarga.HasValue) ? carga.DataCarregamentoCarga.Value : (tabelaFrete.UsarComoDataBaseVigenciaDataAtual ? DateTime.Now : carga.DataCriacaoCarga.Date),
                Desistencia = carga.Desistencia,
                DespachoTransitoAduaneiro = cargaPedido.Pedido.DespachoTransitoAduaneiro,
                Destinatarios = destinatarios,
                Destinos = destinos,
                Empresa = carga.Empresa,
                PesoTotalCarga = carga?.DadosSumarizados?.PesoTotal ?? 0m,
                EscoltaArmada = cargaPedido.Pedido.EscoltaArmada,
                QuantidadeEscolta = cargaPedido.Pedido.QtdEscolta,
                Filial = carga.Filial,
                GerenciamentoRisco = cargaPedido.Pedido.GerenciamentoRisco,
                CargaPerigosa = cargaPedido.Carga?.CargaPerigosaIntegracaoLeilao ?? false,
                GrupoPessoas = carga.GrupoPessoaPrincipal,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>()
                {
                    cargaPedido.ModeloDocumentoFiscal
                },
                ModeloVeiculo = carga.ModeloVeicularCarga,
                NecessarioReentrega = cargaPedido.Pedido.NecessarioReentrega,
                NumeroAjudantes = cargaPedido.Pedido.QtdAjudantes,
                NumeroDeslocamento = cargaPedido.Pedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = cargaPedido.Pedido.ValorDiaria ?? 0m,
                NumeroEntregas = cargaPedido.Pedido.QtdEntregas,
                NumeroPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unidadeTrabalho).BuscarQuantidadePacoteCarga(cargaPedido.Carga.Codigo),
                NumeroPallets = 0, //pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.QuantidadePallets),
                Origens = origens,
                PercentualDesistencia = carga.PercentualDesistencia,
                Peso = peso,
                PesoCubado = pesoCubado,
                PesoPaletizado = 0m, //pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoPaletizado),
                PossuiRestricaoTrafego = remetentes.Any(o => o.PossuiRestricaoTrafego) || destinatarios.Any(o => o.PossuiRestricaoTrafego),
                QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
                QuantidadeNotasFiscais = pedidoCTesParaSubcontratacao.Count,
                Quantidades = new List<ParametrosCalculoFreteQuantidade> {
                                    new ParametrosCalculoFreteQuantidade() {
                                        Quantidade = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.CTeTerceiroQuantidades.Where(q => q.Unidade == Dominio.Enumeradores.UnidadeMedida.UN).Sum(q => q.Quantidade)),
                                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.UN
                                    },
                                    new ParametrosCalculoFreteQuantidade() {
                                        Quantidade = peso,
                                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                                    },
                                    new ParametrosCalculoFreteQuantidade()
                                    {
                                        Quantidade = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.CTeTerceiroQuantidades.Where(q => q.Unidade == Dominio.Enumeradores.UnidadeMedida.M3).Sum(q => q.Quantidade)),
                                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3
                                    }
                                },
                Rastreado = cargaPedido.Pedido.Rastreado,
                Remetentes = remetentes,
                Rota = carga.Rota,
                TipoCarga = carga.TipoDeCarga,
                TipoOperacao = carga.TipoOperacao,
                Tomador = cargaPedido.ObterTomador(),
                ValorNotasFiscais = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.ValorTotalMercadoria),
                Veiculo = carga.Veiculo,
                Reboques = carga.VeiculosVinculados?.ToList(),
                Volumes = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.CTeTerceiroQuantidades.Where(q => q.Unidade == Dominio.Enumeradores.UnidadeMedida.UN).Sum(q => q.Quantidade)),
                DataBaseCRT = cargaPedido.Pedido.DataBaseCRT,
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                Fronteiras = carga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                FreteTerceiro = carga.FreteDeTerceiro
            };

            return parametrosCalculoFrete;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorNotasFiscais(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool possuiComponentePorQuantidadeDocumentos = tabelaFrete.Componentes.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && o.TipoDocumentoQuantidadeDocumentos.HasValue && o.TipoDocumentoQuantidadeDocumentos.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido);
            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;

            if (possuiComponentePorQuantidadeDocumentos)
                quantidadesDocumentosEmitir = Servicos.Embarcador.Carga.CargaPedido.ObterQuantidadeDocumentosEmitir(null, null, pedidoXMLNotasFiscais, null, unidadeTrabalho, tipoServicoMultisoftware);

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();

            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();

            if (tabelaFrete.UtilizarParticipantePedidoParaCalculo)
            {
                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido.Expedidor != null && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                {
                    remetentes.Add(cargaPedido.Expedidor);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Expedidor.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }
                else if (cargaPedido.Pedido.Remetente != null)
                {
                    remetentes.Add(cargaPedido.Pedido.Remetente);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? cargaPedido.Pedido.Remetente.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);

                }

                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                {
                    destinatarios.Add(cargaPedido.Recebedor);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Recebedor.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }
                else if (cargaPedido.Pedido.Destinatario != null)
                {
                    destinatarios.Add(cargaPedido.Pedido.Destinatario);
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? cargaPedido.Pedido.Destinatario.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }

                if (cargaPedido.Pedido.Origem != null)
                    origens.Add(cargaPedido.Pedido.Origem);

                if (cargaPedido.Pedido.Destino != null)
                    destinos.Add(cargaPedido.Pedido.Destino);
            }
            else
            {
                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;

                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor))
                {
                    remetente = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Expedidor != null).Select(o => o.XMLNotaFiscal.Expedidor).FirstOrDefault();

                    if (remetente == null)
                        remetente = cargaPedido.Expedidor;

                    if (remetente != null)
                    {
                        int.TryParse(Utilidades.String.OnlyNumbers(remetente.CEP), out int cepRemetente);
                        cepsRemetentes.Add(cepRemetente);
                    }
                }

                if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor))
                {
                    destinatario = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Recebedor != null).Select(o => o.XMLNotaFiscal.Recebedor).FirstOrDefault();

                    if (destinatario == null)
                        destinatario = cargaPedido.Recebedor;

                    if (destinatario != null)
                    {
                        int.TryParse(Utilidades.String.OnlyNumbers(destinatario.CEP), out int cepDestinatario);
                        cepsDestinatarios.Add(cepDestinatario);
                    }
                }

                if (remetente == null)
                {
                    remetente = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Emitente != null).Select(o => o.XMLNotaFiscal.Emitente).FirstOrDefault();

                    //Quando é mesma por nota sempre considera o CEP da nota
                    //int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? remetente.CEP), out int cepRemetente);
                    int.TryParse(Utilidades.String.OnlyNumbers(remetente.CEP), out int cepRemetente);
                    cepsRemetentes.Add(cepRemetente);
                }

                if (destinatario == null)
                {
                    destinatario = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Destinatario != null).Select(o => o.XMLNotaFiscal.Destinatario).FirstOrDefault();

                    //Quando é mesma por nota sempre considera o CEP da nota
                    //int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? destinatario.CEP), out int cepDestinatario);
                    int.TryParse(Utilidades.String.OnlyNumbers(destinatario.CEP), out int cepDestinatario);
                    cepsDestinatarios.Add(cepDestinatario);
                }

                if (remetente != null)
                {
                    remetentes.Add(remetente);
                    origens.Add(remetente.Localidade);
                }

                if (destinatario != null)
                {
                    destinatarios.Add(destinatario);
                    destinos.Add(destinatario.Localidade);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
            {
                DataColeta = cargaPedido.Pedido.DataInicialColeta,
                DataFinalViagem = cargaPedido.Pedido.DataFinalViagemFaturada,
                DataInicialViagem = cargaPedido.Pedido.DataInicialViagemFaturada,
                DataVigencia = (tabelaFrete.ValidarPorDataCarregamento && carga.DataCarregamentoCarga.HasValue) ? carga.DataCarregamentoCarga.Value : (tabelaFrete.UsarComoDataBaseVigenciaDataAtual ? DateTime.Now : carga.DataCriacaoCarga.Date),
                Desistencia = carga.Desistencia,
                DespachoTransitoAduaneiro = cargaPedido.Pedido.DespachoTransitoAduaneiro,
                Destinatarios = destinatarios,
                Destinos = destinos,
                PesoTotalCarga = carga?.DadosSumarizados?.PesoTotal ?? 0m,
                Empresa = carga.Empresa,
                EscoltaArmada = cargaPedido.Pedido.EscoltaArmada,
                QuantidadeEscolta = cargaPedido.Pedido.QtdEscolta,
                Filial = carga.Filial,
                GerenciamentoRisco = cargaPedido.Pedido.GerenciamentoRisco,
                GrupoPessoas = carga.GrupoPessoaPrincipal,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>()
                {
                    cargaPedido.ModeloDocumentoFiscal
                },
                ModeloVeiculo = carga.ModeloVeicularCarga,
                NecessarioReentrega = cargaPedido.Pedido.NecessarioReentrega,
                NumeroAjudantes = cargaPedido.Pedido.QtdAjudantes,
                NumeroDeslocamento = cargaPedido.Pedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = cargaPedido.Pedido.ValorDiaria ?? 0m,
                NumeroEntregas = cargaPedido.Pedido.QtdEntregas,
                NumeroPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unidadeTrabalho).BuscarQuantidadePacoteCarga(cargaPedido.Carga.Codigo),
                NumeroPallets = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.QuantidadePallets),
                Origens = origens,
                PercentualDesistencia = carga.PercentualDesistencia,
                Peso = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Peso),
                PesoLiquido = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoLiquido),
                PesoCubado = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoCubado),
                PesoPaletizado = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoPaletizado),
                PossuiRestricaoTrafego = remetentes.Any(o => o.PossuiRestricaoTrafego) || destinatarios.Any(o => o.PossuiRestricaoTrafego),
                QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
                QuantidadeNotasFiscais = pedidoXMLNotasFiscais.Count,
                Quantidades = new List<ParametrosCalculoFreteQuantidade> {
                    new ParametrosCalculoFreteQuantidade() {
                        Quantidade = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Volumes),
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.UN
                    },
                    new ParametrosCalculoFreteQuantidade() {
                        Quantidade = pedidoXMLNotasFiscais.Sum(o => o.Peso),
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    },
                    new ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.MetrosCubicos),
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3
                    }
                },
                Rastreado = cargaPedido.Pedido.Rastreado,
                Remetentes = remetentes,
                Rota = carga.Rota,
                TipoCarga = carga.TipoDeCarga,
                TipoOperacao = carga.TipoOperacao,
                Tomador = cargaPedido.ObterTomador(),
                ValorNotasFiscais = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Valor),
                Veiculo = carga.Veiculo,
                Reboques = carga.VeiculosVinculados?.ToList(),
                Volumes = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Volumes),
                DataBaseCRT = cargaPedido.Pedido.DataBaseCRT,
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                Fronteiras = carga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                FreteTerceiro = carga.FreteDeTerceiro,
                CargaPerigosa = carga?.CargaPerigosaIntegracaoLeilao ?? false,
            };

            return parametrosCalculoFrete;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorPedido(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, int distanciaPercurso, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete, bool calculoFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidosNotasFiscais, decimal pesoCalculo)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unidadeTrabalho);

            decimal distancia = 0;

            if (cargaPedido.Pedido.Distancia > 0)
                distancia = cargaPedido.Pedido.Distancia;
            else
            {
                if (carga.Rota?.Quilometros > 0)
                    distancia = carga.Rota.Quilometros;
                else
                    distancia = distanciaPercurso;

                if ((carregamentoRoteirizacao != null) && (configuracao?.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga ?? true) && carregamentoRoteirizacao.DistanciaKM > 0)
                    distancia = carregamentoRoteirizacao.DistanciaKM;

                if (carga.DeslocamentoQuilometros > 0)
                    distancia += carga.DeslocamentoQuilometros;


                if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();

                    if ((configuracaoCarga.UtilizarDistanciaRoteirizacaoNaCarga ?? false))
                    {
                        Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unidadeTrabalho);
                        int distanciaEmMetros = repositorioCargaRotaFretePontosPassagem.BuscarDistanciaPorCarga(carga.Codigo);
                        distancia = (distanciaEmMetros / 1000);
                    }
                }
            }


            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = (from obj in cargaPedidoQuantidadesCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

            decimal quantidadePallets = cargaPedido.Pedido.NumeroPaletes + cargaPedido.Pedido.NumeroPaletesFracionado;

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                decimal quantidadePalletsNotas = (from obj in cargaPedidosValoresNotas where obj.Codigo == cargaPedido.Codigo select obj.QuantidadePallets).Sum();

                if (quantidadePalletsNotas > 0m)
                    quantidadePallets = quantidadePalletsNotas;
            }

            decimal peso = pesoCalculo;
            decimal pesoLiquido = cargaPedido.PesoLiquido;

            if (peso <= 0)
            {
                if (tabelaFrete.UtilizarPesoLiquido)
                    peso = cargaPedido.PesoLiquido;
                else
                {
                    peso = svcFreteCliente.ObterQuilosTotaisParaQuilos(cargaPedidoQuantidades);

                    if (peso <= 0m)
                        peso = cargaPedido.Peso;
                }
            }

            peso -= cargaPedido.PesoMercadoriaDescontar;

            int quantidadeNotasFiscais = (from obj in cargaPedidosValoresNotas where obj.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.TotalNotasFiscais).Sum();
            decimal valorTotalNotasFiscais = 0m;

            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                valorTotalNotasFiscais = (from obj in cargaPedidosValoresNotas where obj.Codigo == cargaPedido.Codigo select obj.ValorTotalNotaFiscal).Sum();
            else if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                valorTotalNotasFiscais = (from obj in cargaPedidosValoresCTesSubcontratacao where obj.Codigo == cargaPedido.Codigo select obj.ValorTotalNotaFiscal).Sum();

            valorTotalNotasFiscais -= cargaPedido.ValorMercadoriaDescontar;

            bool possuiComponentePorQuantidadeDocumentos = tabelaFrete.Componentes.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && o.TipoDocumentoQuantidadeDocumentos.HasValue && o.TipoDocumentoQuantidadeDocumentos.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido);

            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;

            if (possuiComponentePorQuantidadeDocumentos)
            {
                Servicos.Embarcador.Carga.CargaPedido.CriarPreviaDocumentoCarga(carga, unidadeTrabalho, tipoServicoMultisoftware, configuracao);

                quantidadesDocumentosEmitir = Servicos.Embarcador.Carga.CargaPedido.ObterQuantidadeDocumentosEmitir(null, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, null, null, unidadeTrabalho, tipoServicoMultisoftware);
            }

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false) || cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos)
            {
                if (cargaPedido.Pedido.Origem != null)
                    origens.Add(cargaPedido.Pedido.Origem);

                if (cargaPedido.Pedido.Destino != null)
                    destinos.Add(cargaPedido.Pedido.Destino);
            }
            else
            {
                if (cargaPedido.Origem != null)
                    origens.Add(cargaPedido.Origem);

                if (cargaPedido.Destino != null)
                    destinos.Add(cargaPedido.Destino);
            }

            if (cargaPedido.Pedido.RegiaoDestino != null)
                regioesDestino.Add(cargaPedido.Pedido.RegiaoDestino);

            if (cargaPedido.Carga.OrigemTrocaNota != null)
            {
                origens.Clear();
                origens.Add(cargaPedido.Carga.OrigemTrocaNota);
            }

            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();

            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();

            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                destinatarios.Add(cargaPedido.Recebedor);

                int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Recebedor.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }
            else if (cargaPedido.Pedido.Destinatario != null)
            {
                destinatarios.Add(cargaPedido.Pedido.Destinatario);

                int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? cargaPedido.Pedido.Destinatario.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }

            if (cargaPedido.Pedido.ClienteDeslocamento != null)
            {
                remetentes.Add(cargaPedido.Pedido.ClienteDeslocamento);

                int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.ClienteDeslocamento.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Expedidor != null && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                remetentes.Add(cargaPedido.Expedidor);

                int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Expedidor.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if (cargaPedido.Pedido.Remetente != null)
            {
                remetentes.Add(cargaPedido.Pedido.Remetente);

                int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? cargaPedido.Pedido.Remetente.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = carga.ModeloVeicularCarga;
            if (tabelaFrete.UtilizaModeloVeicularVeiculo)
            {
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
            }

            if (modeloVeicularCalculo != null)
            {
                if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeMinimaGarantidaModeloVeicular && modeloVeicularCalculo.ToleranciaPesoMenor > 0)
                {
                    if (peso < modeloVeicularCalculo.ToleranciaPesoMenor)
                        peso = modeloVeicularCalculo.ToleranciaPesoMenor;
                }
                else if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeModeloVeicular && modeloVeicularCalculo.CapacidadePesoTransporte > 0)
                    peso = modeloVeicularCalculo.CapacidadePesoTransporte;
            }

            decimal pesoCubado = 0m;

            //if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            //{
            //    pesoCubado = (from obj in cargaPedidosValoresNotas where obj.Codigo == cargaPedido.Codigo select obj.Cubagem).Sum();

            //    if (pesoCubado <= 0m)
            //        pesoCubado = cargaPedido.Pedido.PesoCubado;
            //}
            //else
            //    pesoCubado = ObterPesoCubado(cargaPedido, cargaPedido.Carga.Carregamento, unidadeTrabalho);

            if (carregamentoPedidosNotasFiscais.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoPedidoNf = (from obj in carregamentoPedidosNotasFiscais where obj.CarregamentoPedido.Pedido.Codigo == cargaPedido.Pedido.Codigo select obj).FirstOrDefault();
                if (carregamentoPedidoNf != null)
                    pesoCubado = carregamentoPedidoNf.NotasFiscais.Sum(x => x.PesoCubado);
            }
            else
                pesoCubado = (from obj in cargaPedidosValoresNotas where obj.Codigo == cargaPedido.Codigo select obj.Cubagem).Sum();

            if (pesoCubado <= 0m)
                pesoCubado = cargaPedido.Pedido.PesoCubado;

            if (pesoCubado <= 0 && tabelaFrete.CalcularFretePorPesoCubado)
                pesoCubado = cargaPedido.Pedido.CubagemTotal * tabelaFrete.FatorCubagem;

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem> paramtrosTipoEmbalagem = new List<ParametroTipoEmbalagem>();
            if (tabelaFrete.TipoEmbalagens?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeTrabalho);
                // todo: se ficar lento aqui cenário para poucos produtos, tratar para passar por parametro a lista de produtos da carga e filtrar aqui por cargapedido (dúvidas falar com o Rodrigo).
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    if (cargaPedidoProduto.Produto?.TipoEmbalagem != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in paramtrosTipoEmbalagem where obj.TipoEmbalagem.Codigo == cargaPedidoProduto.Produto.TipoEmbalagem.Codigo select obj).FirstOrDefault();
                        if (parametroTipoEmbalagem == null)
                        {
                            parametroTipoEmbalagem = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem();
                            parametroTipoEmbalagem.TipoEmbalagem = cargaPedidoProduto.Produto.TipoEmbalagem;
                            parametroTipoEmbalagem.Quantidade = cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso = cargaPedidoProduto.PesoTotal;
                            paramtrosTipoEmbalagem.Add(parametroTipoEmbalagem);
                        }
                        else
                        {
                            parametroTipoEmbalagem.Quantidade += cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso += cargaPedidoProduto.PesoTotal;
                        }

                    }
                }
            }


            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
            {
                DataColeta = cargaPedido.Pedido.DataInicialColeta,
                DataFinalViagem = cargaPedido.Pedido.DataFinalViagemFaturada,
                DataInicialViagem = cargaPedido.Pedido.DataInicialViagemFaturada,
                DataVigencia = (tabelaFrete.ValidarPorDataCarregamento && carga.DataCarregamentoCarga.HasValue) ? carga.DataCarregamentoCarga.Value : (tabelaFrete.UsarComoDataBaseVigenciaDataAtual ? DateTime.Now : carga.DataCriacaoCarga.Date),
                Desistencia = carga.Desistencia,
                DespachoTransitoAduaneiro = cargaPedido.Pedido.DespachoTransitoAduaneiro,
                NecessarioAjudante = cargaPedido.Pedido.Ajudante,
                Destinatarios = destinatarios,
                Destinos = destinos,
                RegioesDestino = regioesDestino,
                Distancia = distancia,
                PesoTotalCarga = carga?.DadosSumarizados?.PesoTotal ?? 0m,
                Empresa = !calculoFilialEmissora ? cargaPedido.CargaOrigem.Empresa : cargaPedido.CargaOrigem.EmpresaFilialEmissora,
                EscoltaArmada = cargaPedido.Pedido.EscoltaArmada,
                QuantidadeEscolta = cargaPedido.Pedido.QtdEscolta,
                Filial = carga.Filial,
                GerenciamentoRisco = cargaPedido.Pedido.GerenciamentoRisco,
                GrupoPessoas = carga.GrupoPessoaPrincipal,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>()
                {
                    cargaPedido.ModeloDocumentoFiscal
                },
                ModeloVeiculo = modeloVeicularCalculo,
                NecessarioReentrega = cargaPedido.Pedido.NecessarioReentrega,
                NumeroAjudantes = cargaPedido.Pedido.QtdAjudantes,
                NumeroDeslocamento = cargaPedido.Pedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = cargaPedido.Pedido.ValorDiaria ?? 0m,
                NumeroEntregas = cargaPedido.Pedido.QtdEntregas,
                NumeroPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unidadeTrabalho).BuscarQuantidadePacoteCarga(cargaPedido.Carga.Codigo),
                NumeroPallets = quantidadePallets,
                Origens = origens,
                PercentualDesistencia = carga.PercentualDesistencia,
                Peso = peso,
                PesoLiquido = pesoLiquido,
                PesoCubado = pesoCubado,
                PesoPaletizado = cargaPedido.Pedido.PesoTotalPaletes,
                PossuiRestricaoTrafego = (cargaPedido.Pedido.Remetente != null && cargaPedido.Pedido.Remetente.PossuiRestricaoTrafego) ||
                                         (cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.PossuiRestricaoTrafego) ||
                                         ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && (cargaPedido.Expedidor?.PossuiRestricaoTrafego ?? false)) ||
                                         ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && (cargaPedido.Recebedor?.PossuiRestricaoTrafego ?? false)),
                QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
                QuantidadeNotasFiscais = quantidadeNotasFiscais,
                Quantidades = (from obj in cargaPedidoQuantidades
                               select new ParametrosCalculoFreteQuantidade()
                               {
                                   Quantidade = obj.Quantidade,
                                   UnidadeMedida = obj.Unidade
                               }).ToList(),
                Rastreado = cargaPedido.Pedido.Rastreado,
                Remetentes = remetentes,
                Rota = (cargaPedido.Pedido.RotaFrete != null && tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador) ? cargaPedido.Pedido.RotaFrete : carga.Rota,
                RotasDinamicas = cargaPedido.Pedido.RotaFrete != null ? new List<Dominio.Entidades.RotaFrete>() { cargaPedido.Pedido.RotaFrete } : null,
                CodigosRotasFixas = (from obj in cargaPedidoRotasFrete select obj.Codigo).ToList(),
                TipoCarga = carga.TipoOperacao?.UtilizarTipoCargaPedidoCalculoFrete ?? false ? cargaPedido.Pedido.TipoDeCarga : carga.TipoDeCarga,
                TipoOperacao = tabelaFrete.UtilizarTipoOperacaoPedido ? cargaPedido.Pedido.TipoOperacao ?? carga.TipoOperacao : carga.TipoOperacao,
                Tomador = cargaPedido.ObterTomador(),
                ValorNotasFiscais = valorTotalNotasFiscais,
                Veiculo = carga.Veiculo,
                Reboques = carga.VeiculosVinculados?.ToList(),
                Volumes = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum(),
                DataBaseCRT = cargaPedido.Pedido.DataBaseCRT,
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                Cubagem = cargaPedido.Pedido.CubagemTotal,
                MaiorAlturaProdutoEmCentimetros = cargaPedido.Pedido.MaiorAlturaProdutoEmCentimetros,
                MaiorLarguraProdutoEmCentimetros = cargaPedido.Pedido.MaiorLarguraProdutoEmCentimetros,
                MaiorComprimentoProdutoEmCentimetros = cargaPedido.Pedido.MaiorComprimentoProdutoEmCentimetros,
                MaiorVolumeProdutoEmCentimetros = cargaPedido.Pedido.MaiorVolumeProdutoEmCentimetros,
                Fronteiras = cargaPedido.Carga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                TiposEmbalagem = paramtrosTipoEmbalagem,
                FreteTerceiro = carga.FreteDeTerceiro,
                CanalEntrega = cargaPedido.CanalEntrega != null ? cargaPedido.CanalEntrega : cargaPedido.Pedido.CanalEntrega,
                CanalVenda = cargaPedido.CanalVenda != null ? cargaPedido.CanalVenda : cargaPedido.Pedido.CanalVenda,
                DataPrevisaoEntrega = cargaPedido.Pedido?.PrevisaoEntrega ?? null,
                CargaPerigosa = cargaPedido.Carga?.CargaPerigosaIntegracaoLeilao ?? false
            };

            return parametrosCalculoFrete;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorCarga(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            return ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, calculoFreteFilialEmissora, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, configuracao, calcularFretePorJanelaCarregamentoTransportador: false);
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorCarga(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool calcularFretePorJanelaCarregamentoTransportador)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unidadeTrabalho);
            Servicos.Embarcador.Carga.CTe svcCargaCTe = new CTe(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unidadeTrabalho);
            Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade repDestinoPrioritarioCalculoFreteLocalidade = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Frete.PesoTabelaFrete repPesoTabelaFrete = new Repositorio.Embarcador.Frete.PesoTabelaFrete(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);

            if ((cargaLocaisPrestacao.Count <= 0) && !calcularFretePorJanelaCarregamentoTransportador)
                return null;

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regiaosDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosParametro = (from obj in cargaPedidos where !obj.PedidoPallet && obj.Pedido.TipoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega select obj).ToList();

            if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false) || cargaPedidosParametro.Exists(cargaPedido => cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos))
            {
                origens = (from obj in cargaPedidosParametro where obj.Pedido?.Origem != null select obj.Pedido.Origem).Distinct().ToList();
                destinos = (from obj in cargaPedidosParametro where obj.Pedido?.Destino != null select obj.Pedido.Destino).Distinct().ToList();
                regiaosDestino = (from obj in cargaPedidosParametro where obj.Pedido?.RegiaoDestino != null select obj.Pedido.RegiaoDestino).Distinct().ToList();
            }
            else
            {
                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                {
                    origens = (from obj in cargaLocaisPrestacao select obj.LocalidadeInicioPrestacao).Distinct().ToList();
                    destinos = (from obj in cargaLocaisPrestacao select obj.LocalidadeTerminoPrestacao).Distinct().ToList();
                }
                else
                {
                    origens = (from obj in cargaPedidos select obj.Origem).Distinct().ToList();
                    destinos = (from obj in cargaPedidos select obj.Destino).Distinct().ToList();
                    regiaosDestino = (from obj in cargaPedidos where obj.Pedido?.RegiaoDestino != null select obj.Pedido.RegiaoDestino).Distinct().ToList();
                }
            }

            if (tabelaFrete.CalcularFreteDestinoPrioritario)
            {
                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade localidadePrioritaria = repDestinoPrioritarioCalculoFreteLocalidade.ValidarPorTabelaFreteELocalidades(tabelaFrete.Codigo, (from o in destinos select o.Codigo).ToList());
                if (localidadePrioritaria != null)
                    destinos = new List<Dominio.Entidades.Localidade>() { localidadePrioritaria.Localidade };
            }

            if (carga.OrigemTrocaNota != null)
            {
                origens.Clear();
                origens.Add(carga.OrigemTrocaNota);
            }

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();

            bool possuiComponentePorQuantidadeDocumentos = repComponenteFreteTabelaFrete.VerificarSePossuiComponentePorDocumento(tabelaFrete.Codigo);

            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;

            if (possuiComponentePorQuantidadeDocumentos)
            {
                Servicos.Embarcador.Carga.CargaPedido.CriarPreviaDocumentoCarga(carga, unidadeTrabalho, tipoServicoMultisoftware, configuracao);

                quantidadesDocumentosEmitir = Servicos.Embarcador.Carga.CargaPedido.ObterQuantidadeDocumentosEmitir(carga, null, null, null, unidadeTrabalho, tipoServicoMultisoftware);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosParametro)
            {
                if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (!svcCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes, tipoServicoMultisoftware) ||
                    cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado))
                {
                    if (cargaPedido.Recebedor != null && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                    {
                        destinatarios.Add(cargaPedido.Recebedor);

                        int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Recebedor.CEP), out int cepDestinatario);
                        cepsDestinatarios.Add(cepDestinatario);
                    }
                    else if (cargaPedido.Pedido.Destinatario != null)
                    {
                        destinatarios.Add(cargaPedido.Pedido.Destinatario);

                        int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoDestino?.CEP ?? cargaPedido.Pedido.Destinatario.CEP), out int cepDestinatario);
                        cepsDestinatarios.Add(cepDestinatario);
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                    destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && !destinatarios.Any(o => o.CPF_CNPJ == obj.XMLNotaFiscal.Destinatario.CPF_CNPJ) select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());
                    destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && !destinatarios.Any(o => o.CPF_CNPJ == obj.XMLNotaFiscal.Emitente.CPF_CNPJ) select obj.XMLNotaFiscal.Emitente).Distinct().ToList());

                    cepsDestinatarios = (from obj in destinatarios where !string.IsNullOrWhiteSpace(obj.CEP) select int.Parse(Utilidades.String.OnlyNumbers(obj.CEP))).Distinct().ToList();
                }

                if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (!svcCargaCTe.VerificarSePercursoOrigemSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes) ||
                    cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado))
                {
                    if (cargaPedido.Pedido.ClienteDeslocamento != null)
                    {
                        remetentes.Add(cargaPedido.Pedido.ClienteDeslocamento);

                        int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.ClienteDeslocamento.CEP), out int cepRemetente);
                        cepsRemetentes.Add(cepRemetente);
                    }
                    else if (cargaPedido.Expedidor != null && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
                    {
                        remetentes.Add(cargaPedido.Expedidor);

                        int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Expedidor.CEP), out int cepRemetente);
                        cepsRemetentes.Add(cepRemetente);
                    }
                    else if (cargaPedido.Pedido.Remetente != null)
                    {
                        remetentes.Add(cargaPedido.Pedido.Remetente);

                        int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.EnderecoOrigem?.CEP ?? cargaPedido.Pedido.Remetente.CEP), out int cepRemetente);
                        cepsRemetentes.Add(cepRemetente);
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                    remetentes.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && !remetentes.Any(o => o.CPF_CNPJ == obj.XMLNotaFiscal.Emitente.CPF_CNPJ) select obj.XMLNotaFiscal.Emitente).Distinct().ToList());
                    remetentes.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && !remetentes.Any(o => o.CPF_CNPJ == obj.XMLNotaFiscal.Destinatario.CPF_CNPJ) select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());

                    cepsRemetentes = (from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CEP) select int.Parse(Utilidades.String.OnlyNumbers(obj.CEP))).Distinct().ToList();
                }
            }

            decimal distancia = carga.Distancia;

            if (distancia == 0)
            {
                if (carga.Rota != null && carga.Rota.Quilometros > 0)
                    distancia = (int)carga.Rota.Quilometros;
                else
                {
                    if (carga.DadosSumarizados != null && carga.DadosSumarizados.Distancia <= 1)
                        distancia = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
                    else
                        distancia = carga.DadosSumarizados?.Distancia ?? 0;
                }

                if ((carga.Carregamento != null) && (configuracao?.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga ?? true))
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

                    if (carregamentoRoteirizacao != null && carregamentoRoteirizacao.DistanciaKM > 0)
                        distancia = (int)carregamentoRoteirizacao.DistanciaKM;
                }

                if (carga.DeslocamentoQuilometros > 0)
                    distancia += (int)carga.DeslocamentoQuilometros;

                if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                {

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();

                    if ((configuracaoCarga.UtilizarDistanciaRoteirizacaoNaCarga ?? false))
                    {
                        Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unidadeTrabalho);
                        distancia = (repositorioCargaRotaFretePontosPassagem.BuscarDistanciaPorCarga(carga.Codigo) / 1000);
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> pesosTabelaFrete = repPesoTabelaFrete.BuscarPorTabelaFrete(tabelaFrete.Codigo);

            if (carga.TipoOperacao != null && !carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete && cargaPedidoQuantidades.Count() == 0 && pesosTabelaFrete.Any(x => x.UnidadeMedida.UnidadeMedida == Dominio.Enumeradores.UnidadeMedida.UN))
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosParametro)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                    {
                        Quantidade = cargaPedido.Pedido.QtVolumes,
                        Unidade = Dominio.Enumeradores.UnidadeMedida.UN,
                        CargaPedido = cargaPedido
                    };
                    repCargaPedidoQuantidades.Inserir(cargaPedidoQuantidade);
                    cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                }
            }

            decimal quantidadePallets = cargaPedidosParametro.Sum(o => o.Pallet);
            if (quantidadePallets == 0)
                quantidadePallets = cargaPedidosParametro.Sum(o => o.Pedido.NumeroPaletes + o.Pedido.NumeroPaletesFracionado);

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                decimal quantidadePalletsNotas = repPedidoXMLNotaFiscal.BuscarPalletsPorCarga(carga.Codigo);

                if (quantidadePalletsNotas > 0m)
                    quantidadePallets = quantidadePalletsNotas;
            }

            decimal peso = 0m;
            decimal pesoLiquido = cargaPedidos.Sum(o => o.PesoLiquido);

            if (!configuracao.UsarPesoProdutoSumarizacaoCarga)
                peso = tabelaFrete.UtilizarPesoLiquido ? cargaPedidos.Sum(o => o.PesoLiquido) : cargaPedidos.Sum(o => o.Peso);

            if (peso <= 0m)
            {
                if (tabelaFrete.UtilizarPesoLiquido)
                    peso = cargaPedidos.Sum(o => o.PesoLiquido);
                else
                {
                    peso = svcFreteCliente.ObterQuilosTotaisParaQuilos(cargaPedidoQuantidades);

                    if (peso <= 0m)
                        peso = cargaPedidos.Sum(o => o.Peso);
                }
            }

            int quantidadeNotasFiscais = repPedidoXMLNotaFiscal.ContarPorCargaParaCalculo(carga.Codigo);

            decimal valorTotalNotasFiscais = 0;
            decimal valorTotalNotasFiscaisSemPallets = 0;
            decimal valorMercadoriaDescontar = cargaPedidos.Sum(obj => obj.ValorMercadoriaDescontar);

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                if (carga.TipoContratacaoCarga == TipoContratacaoCarga.Normal || carga.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (carga.Internacional)
                        valorTotalNotasFiscais = repPedidoXMLNotaFiscal.BuscarTotalFacturaPorCarga(carga.Codigo, carga.Moeda ?? MoedaCotacaoBancoCentral.DolarVenda);
                    else
                        valorTotalNotasFiscais = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);

                    valorTotalNotasFiscaisSemPallets = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo, true);
                }
                else
                    valorTotalNotasFiscais = repPedidoCTeParaSubContratacao.BuscarValorTotalMercadoriaPorCarga(carga.Codigo);
            }
            else
                valorTotalNotasFiscais = repCargaPedido.BuscarValorTotalPedidos(carga.Codigo);

            if (valorTotalNotasFiscais == 0 && (carga.TipoContratacaoCarga == TipoContratacaoCarga.Normal || carga.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador))
                valorTotalNotasFiscais = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);

            valorTotalNotasFiscais -= valorMercadoriaDescontar;

            if (valorTotalNotasFiscaisSemPallets > 0)
                valorTotalNotasFiscaisSemPallets -= valorMercadoriaDescontar;

            if ((valorTotalNotasFiscais <= 0m) && calcularFretePorJanelaCarregamentoTransportador && (carga.Filial?.ValorMedioMercadoria > 0m))
            {
                valorTotalNotasFiscais = carga.Filial.ValorMedioMercadoria;
                valorTotalNotasFiscaisSemPallets = carga.Filial.ValorMedioMercadoria;
            }

            if ((carga.TipoOperacao?.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false) && valorTotalNotasFiscais <= 0)
                valorTotalNotasFiscais = 0;

            int quantidadeEntregas = 0;
            if (!tabelaFrete.CalcularQuantidadeEntregaPorNumeroDePedidos)
            {
                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                {
                    if (tabelaFrete.CalcularQuantidadeEntregaPorParticipantesPedido)
                        quantidadeEntregas = cargaPedidosParametro.GroupBy(o => new { CPFCNPJRemetente = o.Pedido.Remetente?.CPF_CNPJ ?? 0D, CPFCNPJExpedidor = o.Expedidor?.CPF_CNPJ ?? 0D, CPFCNPJRecebedor = o.Recebedor?.CPF_CNPJ ?? 0D, CPFCNPJDestinatario = o.Pedido.Destinatario?.CPF_CNPJ }).Count();
                    else
                        quantidadeEntregas = cargaPedidosParametro.Sum(o => o.Pedido.QtdEntregas);
                }
                else
                {

                    if (carga.DadosSumarizados != null)
                    {
                        quantidadeEntregas = carga.DadosSumarizados?.NumeroEntregas ?? 1;
                    }
                    else
                    {
                        Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
                        quantidadeEntregas = serCargaDadosSumarizados.BuscarNumeroDeEntregasPorPedido(cargaPedidosParametro, unidadeTrabalho);
                    }
                }
            }
            else
                quantidadeEntregas = cargaPedidosParametro.Count();

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = carga.ModeloVeicularCarga;
            if (tabelaFrete.UtilizaModeloVeicularVeiculo)
            {
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
            }

            if (modeloVeicularCalculo != null)
            {
                if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeMinimaGarantidaModeloVeicular && modeloVeicularCalculo.ToleranciaPesoMenor > 0)
                {
                    if (peso < modeloVeicularCalculo.ToleranciaPesoMenor)
                        peso = modeloVeicularCalculo.ToleranciaPesoMenor;
                }
                else if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeModeloVeicular && modeloVeicularCalculo.CapacidadePesoTransporte > 0)
                    peso = modeloVeicularCalculo.CapacidadePesoTransporte;
            }

            peso -= cargaPedidos.Sum(obj => obj.PesoMercadoriaDescontar);

            decimal pesoCubado = 0m;
            decimal isencaoCubagem = 0m;
            bool calcularFretePorPesoCubado = tabelaFrete.CalcularFretePorPesoCubado;
            bool aplicarMaiorValorEntrePesoEPesoCubado = tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado;

            if (calcularFretePorPesoCubado)
            {
                decimal cubagemPedidos = cargaPedidosParametro.Sum(o => o.Pedido.CubagemTotal);
                if (tabelaFrete.FatorCubagem > 0 && cubagemPedidos > 0)
                {
                    pesoCubado = cubagemPedidos * tabelaFrete.FatorCubagem;
                    isencaoCubagem = tabelaFrete.IsencaoCubagem;
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> carregamentoPedidoNotaFiscals = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                if (carga.Carregamento != null)
                    carregamentoPedidoNotaFiscals = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(carga.Carregamento.Codigo);

                if (carregamentoPedidoNotaFiscals.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal carregamentoNf in carregamentoPedidoNotaFiscals)
                        pesoCubado += carregamentoNf.NotasFiscais.Sum(x => x.PesoCubado);
                }
                else
                {
                    pesoCubado = repPedidoXMLNotaFiscal.BuscarPesoCubadoPorCarga(carga.Codigo);

                    if (pesoCubado <= 0m)
                        pesoCubado = cargaPedidosParametro.Sum(o => o.Pedido.PesoCubado);
                }
            }

            List<int> codigosRotaFrete = repCargaPedidoRotaFrete.ObterCodigosRotasFretePorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem> paramtrosTipoEmbalagem = new List<ParametroTipoEmbalagem>();
            if (tabelaFrete.TipoEmbalagens?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    if (cargaPedidoProduto.Produto?.TipoEmbalagem != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in paramtrosTipoEmbalagem where obj.TipoEmbalagem.Codigo == cargaPedidoProduto.Produto.TipoEmbalagem.Codigo select obj).FirstOrDefault();
                        if (parametroTipoEmbalagem == null)
                        {
                            parametroTipoEmbalagem = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem();
                            parametroTipoEmbalagem.TipoEmbalagem = cargaPedidoProduto.Produto.TipoEmbalagem;
                            parametroTipoEmbalagem.Quantidade = cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso = cargaPedidoProduto.PesoTotal;
                            paramtrosTipoEmbalagem.Add(parametroTipoEmbalagem);
                        }
                        else
                        {
                            parametroTipoEmbalagem.Quantidade += cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso += cargaPedidoProduto.PesoTotal;
                        }

                    }
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
            {
                DataColeta = cargaPedidosParametro.Select(o => o.Pedido.DataInicialColeta).Min(),
                DataFinalViagem = cargaPedidosParametro.Select(o => o.Pedido.DataFinalViagemFaturada).Max(),
                DataInicialViagem = cargaPedidosParametro.Select(o => o.Pedido.DataInicialViagemFaturada).Min(),
                DataVigencia = (tabelaFrete.ValidarPorDataCarregamento && carga.DataCarregamentoCarga.HasValue) ? carga.DataCarregamentoCarga.Value : (tabelaFrete.UsarComoDataBaseVigenciaDataAtual ? DateTime.Now : carga.DataCriacaoCarga.Date),
                Desistencia = carga.Desistencia,
                DespachoTransitoAduaneiro = cargaPedidosParametro.Any(o => o.Pedido.DespachoTransitoAduaneiro),
                Destinatarios = destinatarios,
                Destinos = destinos,
                Distancia = distancia,
                PesoTotalCarga = carga?.DadosSumarizados?.PesoTotal ?? 0,
                Empresa = !calculoFreteFilialEmissora ? carga.Empresa : carga.EmpresaFilialEmissora,
                EscoltaArmada = cargaPedidosParametro.Any(o => o.Pedido.EscoltaArmada),
                QuantidadeEscolta = cargaPedidosParametro.Where(o => o.Pedido.EscoltaArmada).Sum(o => o.Pedido.QtdEscolta),
                NecessarioAjudante = cargaPedidosParametro.Any(o => o.Pedido.Ajudante),
                Filial = carga.Filial,
                GerenciamentoRisco = cargaPedidosParametro.Any(o => o.Pedido.GerenciamentoRisco),
                GrupoPessoas = carga.GrupoPessoaPrincipal,
                ModelosUtilizadosEmissao = cargaPedidosParametro.Select(o => o.ModeloDocumentoFiscal).Distinct().ToList(),
                ModeloVeiculo = modeloVeicularCalculo,
                NecessarioReentrega = cargaPedidosParametro.Any(o => o.Pedido.NecessarioReentrega),
                NumeroAjudantes = cargaPedidosParametro.Sum(o => o.Pedido.QtdAjudantes),
                NumeroDeslocamento = cargaPedidosParametro.Sum(o => o.Pedido.ValorDeslocamento ?? 0m),
                NumeroDiarias = cargaPedidosParametro.Sum(o => o.Pedido.ValorDiaria ?? 0m),
                NumeroEntregas = quantidadeEntregas,
                NumeroPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unidadeTrabalho).BuscarQuantidadePacoteCarga(carga.Codigo),
                NumeroPallets = quantidadePallets,
                Origens = origens,
                PercentualDesistencia = carga.PercentualDesistencia,
                Peso = peso,
                PesoLiquido = pesoLiquido,
                PesoCubado = pesoCubado,
                PesoPaletizado = cargaPedidosParametro.Sum(o => o.Pedido.PesoTotalPaletes),
                PossuiRestricaoTrafego = remetentes.Any(o => o.PossuiRestricaoTrafego) || destinatarios.Any(o => o.PossuiRestricaoTrafego),
                QuantidadeNotasFiscais = quantidadeNotasFiscais,
                QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
                RegioesDestino = regiaosDestino,
                Quantidades = (from obj in cargaPedidoQuantidades
                               select new ParametrosCalculoFreteQuantidade()
                               {
                                   Quantidade = obj.Quantidade,
                                   UnidadeMedida = obj.Unidade
                               }).ToList(),
                Rastreado = cargaPedidosParametro.Any(o => o.Pedido.Rastreado),
                Remetentes = remetentes,
                Rota = carga.Rota,
                RotasDinamicas = cargaPedidosParametro.Where(o => o.Pedido.RotaFrete != null).Select(o => o.Pedido.RotaFrete).ToList(),
                CodigosRotasFixas = codigosRotaFrete,
                TipoCarga = carga.TipoDeCarga,
                TipoOperacao = carga.TipoOperacao,
                Tomador = cargaPedidosParametro.FirstOrDefault()?.ObterTomador(),
                ValorNotasFiscais = valorTotalNotasFiscais,
                ValorNotasFiscaisSemPallets = valorTotalNotasFiscaisSemPallets,
                Veiculo = carga.Veiculo,
                Reboques = carga.VeiculosVinculados?.ToList(),
                Volumes = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum(),
                DataBaseCRT = cargaPedidosParametro.Where(o => o.Pedido.DataBaseCRT.HasValue).Select(o => o.Pedido.DataBaseCRT).FirstOrDefault(),
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                CalcularFretePorPesoCubado = calcularFretePorPesoCubado,
                AplicarMaiorValorEntrePesoEPesoCubado = aplicarMaiorValorEntrePesoEPesoCubado,
                IsencaoCubagem = isencaoCubagem,
                Cubagem = cargaPedidosParametro.Sum(obj => obj.Pedido.CubagemTotal),
                MaiorAlturaProdutoEmCentimetros = cargaPedidosParametro.Max(obj => obj.Pedido.MaiorAlturaProdutoEmCentimetros),
                MaiorLarguraProdutoEmCentimetros = cargaPedidosParametro.Max(obj => obj.Pedido.MaiorLarguraProdutoEmCentimetros),
                MaiorComprimentoProdutoEmCentimetros = cargaPedidosParametro.Max(obj => obj.Pedido.MaiorComprimentoProdutoEmCentimetros),
                MaiorVolumeProdutoEmCentimetros = cargaPedidosParametro.Max(obj => obj.Pedido.MaiorVolumeProdutoEmCentimetros),
                Fronteiras = carga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                TiposEmbalagem = paramtrosTipoEmbalagem,
                FreteTerceiro = carga.FreteDeTerceiro,
                CanalEntrega = cargaPedidosParametro.FirstOrDefault()?.CanalEntrega != null ? cargaPedidosParametro.FirstOrDefault()?.CanalEntrega : cargaPedidosParametro.Select(o => o.Pedido.CanalEntrega).FirstOrDefault(),
                CanalVenda = cargaPedidosParametro.FirstOrDefault()?.CanalVenda != null ? cargaPedidosParametro.FirstOrDefault()?.CanalVenda : cargaPedidosParametro.Select(o => o.Pedido.CanalVenda).FirstOrDefault(),
                DataPrevisaoEntrega = cargaPedidosParametro.FirstOrDefault()?.Pedido?.PrevisaoEntrega ?? null,
                CargaPerigosa = carga.CargaPerigosaIntegracaoLeilao,
                CargaInternacional = carga.Internacional,
            };

            return parametrosCalculoFrete;
        }

        public void CriarCargaComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, Dominio.Entidades.Cliente tomador, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ComponetesFrete serComponenteFrete = new ComponetesFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = (from obj in cargaPedidoComponentesFretes select obj.ComponenteFrete).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente in componentes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesCarga = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.Carga.Codigo == carga.Codigo && obj.ComponenteFrete == componente && obj.ComponenteFilialEmissora == calculoFreteFilialEmissora select obj).ToList();
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = cargaPedidoComponentesFretesCarga.FirstOrDefault();
                serComponenteFrete.AdicionarComponenteFreteCargaUnicoPorTipo(carga, componente, cargaPedidoComponentesFretesCarga.Sum(obj => obj.ValorComponente), cargaPedidoComponentesFrete.Percentual, calculoFreteFilialEmissora, cargaPedidoComponentesFrete.TipoValor, cargaPedidoComponentesFrete.TipoComponenteFrete, null, cargaPedidoComponentesFrete.IncluirBaseCalculoICMS, cargaPedidoComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro, cargaPedidoComponentesFrete.ModeloDocumentoFiscal, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, false, false, null, 0, null, tomador);

                if (carga.TabelaFrete == null || carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de " + cargaPedidoComponentesFrete.ComponenteFrete.Descricao + " Informado Pelo Embarcador", " Valor Informado = " + cargaPedidoComponentesFretesCarga.Sum(obj => obj.ValorComponente).ToString("n2"), cargaPedidoComponentesFrete.ValorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Valor de " + componente.Descricao + " informado pelo Embarcador", componente.Codigo, cargaPedidoComponentesFretesCarga.Sum(obj => obj.ValorComponente));

                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);
                }
            }
        }

        public void CriarCargaComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ComponetesFrete serComponenteFrete = new ComponetesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFrete = repCargaPedidoComponentesFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            for (int i = 0; i < componentesFrete.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componentePedido = componentesFrete[i];
                serComponenteFrete.AdicionarComponenteFreteCargaUnicoPorTipo(carga, componentePedido.ComponenteFrete, componentePedido.ValorComponente, componentePedido.Percentual, calculoFreteFilialEmissora, componentePedido.TipoValor, componentePedido.TipoComponenteFrete, null, componentePedido.IncluirBaseCalculoICMS, componentePedido.IncluirIntegralmenteContratoFreteTerceiro, componentePedido.ModeloDocumentoFiscal, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete);
                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de " + componentePedido.ComponenteFrete.Descricao + " Informado Pelo Embarcador", " Valor Informado = " + componentePedido.ValorComponente.ToString("n2"), componentePedido.ValorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Valor de " + componentePedido.ComponenteFrete.Descricao + " informado pelo Embarcador", componentePedido.ComponenteFrete.Codigo, componentePedido.ValorComponente);
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);
            }
        }

        public bool CalcularFretePorCliente(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteUtilizar = null)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);

            Servicos.Embarcador.Carga.FreteCTeSubcontratacao serFreteCTeSubcontratacao = new FreteCTeSubcontratacao(unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete svcCargaAprovacaoFrete = new CargaAprovacaoFrete(unitOfWork, configuracao);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = null;

            StringBuilder mensagemRetorno = new StringBuilder();

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, calculoFreteFilialEmissora, unitOfWork, StringConexao, TipoServicoMultisoftware, configuracao);
            if (parametrosCalculo == null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = mensagemRetorno.Insert(0, "Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet)").ToString();
                return true;
            }

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = new List<TabelaFreteCliente>();

            if (tabelaFreteClienteUtilizar != null)
                tabelasCliente.Add(tabelaFreteClienteUtilizar);
            else
                tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

            bool possuiTipoContratoNormal = repCargaPedido.PossuiTipoContratoNormalPorCarga(carga.Codigo);

            if (!svcFreteCliente.PermiteCalcularFrete(tabelasCliente) && (possuiTipoContratoNormal || (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)))
            {

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = tabelaFrete.ContratoFreteTransportador;
                if ((carga?.TipoOperacao?.PermiteUtilizarEmContratoFrete ?? false) && contratoFreteTransportador == null)
                    contratoFreteTransportador = ObterContratoCombativel(tabelaFrete, carga, unitOfWork);

                if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador)
                {
                    if (
                        contratoFreteTransportador != null &&
                        contratoFreteTransportador.Ativo &&
                        !contratoFreteTransportador.ExigeTabelaFreteComValor &&
                        (
                            (contratoFreteTransportador.FranquiaValorKM > 0) ||
                            (contratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui && contratoFreteTransportador.DeduzirValorPorCarga) ||
                            (contratoFreteTransportador.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) ||
                            (configuracao.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
                        )
                    )
                    {
                        carga.ValorFrete = 0;
                        carga.ValorFreteAPagar = 0;

                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametrosContratoFreteTransportadorValorFreteMinimo = ObterParametrosContratoFreteTransportadorValorFreteMinimo(contratoFreteTransportador, parametrosCalculo);

                        string retornoContrato = Servicos.Embarcador.Carga.ContratoFrete.CalcularFretePorContratoFrete(contratoFreteTransportador, parametrosContratoFreteTransportadorValorFreteMinimo, carga, cargaPedidos, configuracao, apenasVerificar, calculoFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork);
                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                        if (!string.IsNullOrWhiteSpace(retornoContrato))
                        {
                            SetarRetornoPendenciaContratoFrete(ref retorno, ref carga, retornoContrato, unitOfWork, configuracao);
                            return true;
                        }
                        else
                        {
                            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                                carga.TabelaFrete = tabelaFrete;
                            Servicos.Embarcador.Carga.FreteCliente serFreteCliente = new FreteCliente(unitOfWork);
                            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

                            if (!apenasVerificar)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

                                if (cargaTabelaCliente != null)
                                    repCargaTabelaFreteCliente.Deletar(cargaTabelaCliente);
                            }

                            Servicos.Embarcador.Carga.ContratoFrete.AtualizarPedidos(carga, tabelaFrete, unitOfWork);
                            retorno = serFreteCliente.ObterDadosTabelaFreteCliente(null, contratoFreteTransportador, carga, calculoFreteFilialEmissora, TipoServicoMultisoftware);
                        }
                    }
                    else
                    {

                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                        retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

                        if (tabelasCliente.Count <= 0)
                            retorno.mensagem = mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete para a tabela de frete " + tabelaFrete.Descricao + " compatível com as configurações da carga.\n").ToString();
                        else if (tabelasCliente.Count > 1)
                            retorno.mensagem = mensagemRetorno.Insert(0, "Foi encontrada mais configuração de frete disponível para a carga para a tabela de frete " + tabelaFrete.Descricao + ".").ToString();
                        else if (tabelasCliente[0].Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao)
                            retorno.mensagem = mensagemRetorno.Insert(0, "A tabela de frete " + tabelaFrete.Descricao + " ainda não foi aprovada e não pode ser utilizada nesta carga.").ToString();

                        if (!apenasVerificar)
                        {
                            carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                            if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                            {
                                carga.PossuiPendencia = true;
                                if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 12 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                            }

                            carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                            if (!calculoFreteFilialEmissora)
                                carga.TabelaFrete = null;
                            else
                                carga.TabelaFreteFilialEmissora = null;

                            repCarga.Atualizar(carga);

                            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

                            if (cargaTabelaCliente != null)
                                repCargaTabelaFreteCliente.Deletar(cargaTabelaCliente);

                            if (!calculoFreteFilialEmissora)
                            {
                                bool abriuTransacao = false;
                                if (!unitOfWork.IsActiveTransaction())
                                {
                                    unitOfWork.Start();
                                    abriuTransacao = true;
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                {
                                    if (pedidoNotasFiscais == null)
                                        pedidoNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                                    AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoNotasFiscais, unitOfWork);
                                }

                                if (abriuTransacao)
                                    unitOfWork.CommitChanges();

                                Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
                            }
                        }

                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                            return true;

                    }
                }
            }
            else if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && !possuiTipoContratoNormal && (tabelasCliente.Count == 0 || tabelasCliente[0] == null))
            {
                retorno = serFreteCTeSubcontratacao.BuscarTabelaFreteSubcontratado(carga, cargaPedidos, configuracao, apenasVerificar, TipoServicoMultisoftware, unitOfWork);
                if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                {
                    retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaSubContratacao;
                    return true;
                }
            }
            else
            {
                retorno = svcFreteCliente.SetarTabelaFreteCarga(ref carga, cargaPedidos, parametrosCalculo, tabelasCliente[0], apenasVerificar, TipoServicoMultisoftware, configuracao, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora);

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = tabelaFrete.ContratoFreteTransportador;

                if ((carga?.TipoOperacao?.PermiteUtilizarEmContratoFrete ?? false) && contratoFreteTransportador == null)
                    contratoFreteTransportador = ObterContratoCombativel(tabelaFrete, carga, unitOfWork);

                if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador)
                {
                    if (
                    contratoFreteTransportador != null &&
                    contratoFreteTransportador.Ativo &&
                    (
                        (contratoFreteTransportador.FranquiaValorKM > 0) ||
                        (contratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui && contratoFreteTransportador.DeduzirValorPorCarga) ||
                        (contratoFreteTransportador.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) ||
                        (configuracao.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
                    )
                )
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametrosContratoFreteTransportadorValorFreteMinimo = ObterParametrosContratoFreteTransportadorValorFreteMinimo(contratoFreteTransportador, parametrosCalculo);
                        string retornoContrato = Servicos.Embarcador.Carga.ContratoFrete.CalcularFretePorContratoFrete(contratoFreteTransportador, parametrosContratoFreteTransportadorValorFreteMinimo, carga, cargaPedidos, configuracao, apenasVerificar, calculoFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(retornoContrato))
                        {
                            SetarRetornoPendenciaContratoFrete(ref retorno, ref carga, retornoContrato, unitOfWork, configuracao);
                            return true;
                        }
                    }
                }

            }

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                svcCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.TabelaFrete, TipoServicoMultisoftware);

            if (retorno.situacao != SituacaoRetornoDadosFrete.ProblemaCalcularFrete)
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

            return false;
        }

        private void SetarRetornoPendenciaContratoFrete(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, string retornoContrato, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
            retorno.mensagem = retornoContrato;

            if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
            {
                carga.PossuiPendencia = true;
                if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 11 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
            }

            carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);
            carga.TabelaFrete = null;

            repCarga.Atualizar(carga);
        }

        private bool ValidarPermiteTabelaPorDistancia(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, StringBuilder mensagemRetorno, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia = null)
        {
            if (tabelaFrete.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga && carga.Distancia == 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    retorno.tipoTabelaFrete = tabelaFrete.TipoTabelaFrete;
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    retorno.mensagem = "A tabela de Frete (" + tabelaFrete.Descricao + ") pode ser utilizada somente quando a distância da carga for informada via integração.";

                    if (!apenasVerificar)
                    {
                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;
                            if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 10 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }
                        carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);
                        repCarga.Atualizar(carga);
                    }
                }
                else
                {
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                }
                return false;
            }
            return true;
        }

        public void ValidarConfiguracaoFaturamentoTomador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            try
            {
                RetornoDadosMensagemAlerta retorno = new RetornoDadosMensagemAlerta();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                string msgAlerta = serCarga.BuscarPendenciaConfiguracaoFaturamentoTomador(carga, unitOfWork, configuracao);

                if (!string.IsNullOrWhiteSpace(msgAlerta))
                {
                    retorno.mensagem = msgAlerta;
                    new Servicos.Embarcador.Hubs.Carga().InformarMensagemAlerta(carga.Codigo, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValidarConfiguracaoFaturamentoTomador");
            }
        }

        public void ValidarEntidadesSemCadastro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            try
            {
                if (!configuracao.UtilizaEmissaoMultimodal)
                    return;

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                if (integracaoIntercab?.PossuiIntegracaoIntercab ?? false)
                    return;

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                RetornoDadosMensagemAlerta retorno = new RetornoDadosMensagemAlerta();
                string msgAlerta = "";

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notas = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                if (notas != null && notas.Count > 0)
                {
                    foreach (var nota in notas)
                    {
                        if (nota.XMLNotaFiscal != null)
                        {
                            if (nota.XMLNotaFiscal.Destinatario != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Destinatario.CodigoIntegracao))
                                msgAlerta += "Destinatário " + nota.XMLNotaFiscal.Destinatario.Descricao + " não possui o código de integração cadastrado <br />";
                            if (nota.XMLNotaFiscal.Emitente != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Emitente.CodigoIntegracao))
                                msgAlerta += "Emitente " + nota.XMLNotaFiscal.Emitente.Descricao + " não possui o código de integração cadastrado <br />";
                            if (nota.XMLNotaFiscal.Recebedor != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Recebedor.CodigoIntegracao))
                                msgAlerta += "Recebedor " + nota.XMLNotaFiscal.Recebedor.Descricao + " não possui o código de integração cadastrado <br />";
                            if (nota.XMLNotaFiscal.Expedidor != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Expedidor.CodigoIntegracao))
                                msgAlerta += "Expedidor " + nota.XMLNotaFiscal.Expedidor.Descricao + " não possui o código de integração cadastrado <br />";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(msgAlerta))
                {
                    retorno.mensagem = msgAlerta;
                    new Servicos.Embarcador.Hubs.Carga().InformarMensagemAlerta(carga.Codigo, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValidarEntidadesSemCadastro");
            }
        }

        public string ValidarEntidadesSemCodigoDocumentacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            try
            {
                if (!configuracao.UtilizaEmissaoMultimodal)
                    return "";

                if (carga.Pedidos == null || carga.Pedidos.Count == 0 || !carga.Pedidos.Any(p => p.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS) ||
                    !carga.Pedidos.Any(p => p.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTeMultimodal || p.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTeRodoviario))
                    return "";

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                RetornoDadosMensagemAlerta retorno = new RetornoDadosMensagemAlerta();
                string msgAlerta = "";

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notas = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                if (notas != null && notas.Count > 0)
                {
                    foreach (var nota in notas)
                    {
                        if (nota.XMLNotaFiscal != null)
                        {
                            if (nota.XMLNotaFiscal.Destinatario != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Destinatario.CodigoDocumento))
                                msgAlerta += "Destinatário " + nota.XMLNotaFiscal.Destinatario.Descricao + " não possui o código de documentação cadastrado <br />";
                            if (nota.XMLNotaFiscal.Emitente != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Emitente.CodigoDocumento))
                                msgAlerta += "Emitente " + nota.XMLNotaFiscal.Emitente.Descricao + " não possui o código de documentação cadastrado <br />";
                            if (nota.XMLNotaFiscal.Recebedor != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Recebedor.CodigoDocumento))
                                msgAlerta += "Recebedor " + nota.XMLNotaFiscal.Recebedor.Descricao + " não possui o código de documentação cadastrado <br />";
                            if (nota.XMLNotaFiscal.Expedidor != null && string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Expedidor.CodigoDocumento))
                                msgAlerta += "Expedidor " + nota.XMLNotaFiscal.Expedidor.Descricao + " não possui o código de documentação cadastrado <br />";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(msgAlerta))
                {
                    retorno.mensagem = msgAlerta;
                    new Servicos.Embarcador.Hubs.Carga().InformarMensagemAlerta(carga.Codigo, retorno);
                }
                return msgAlerta;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValidarEntidadesSemCodigoDocumentacao");
                return "";
            }
        }

        public void ValidarMensagemAlertaGrupoPessoa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            try
            {
                RetornoDadosMensagemAlerta retorno = new RetornoDadosMensagemAlerta();

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta repGrupoPessoaMensagemAlerta = new Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta(unitOfWork);

                if (configuracao.UtilizaEmissaoMultimodal && carga.GrupoPessoaPrincipal != null && carga.GrupoPessoaPrincipal.MensagemAlerta != null && carga.GrupoPessoaPrincipal.MensagemAlerta.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta> mensagensAlerta = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta>();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                    if (notasCarga != null)
                    {
                        foreach (var nota in notasCarga)
                        {
                            if (nota.XMLNotaFiscal != null && !string.IsNullOrWhiteSpace(nota.XMLNotaFiscal.Observacao))
                            {
                                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta> alertas = repGrupoPessoaMensagemAlerta.BuscarAlertasPorGrupoPessoas(carga.GrupoPessoaPrincipal.Codigo, nota.XMLNotaFiscal.Observacao);
                                if (alertas != null && alertas.Count > 0)
                                    mensagensAlerta.AddRange(alertas);
                            }
                        }
                        if (mensagensAlerta != null && mensagensAlerta.Count > 0)
                            mensagensAlerta = mensagensAlerta.Select(o => o).Distinct().ToList();
                        if (mensagensAlerta != null && mensagensAlerta.Count > 0)
                        {
                            retorno.mensagem = string.Join("<br /> ", mensagensAlerta.Select(o => o.MensagemAlerta).ToList());
                            new Servicos.Embarcador.Hubs.Carga().InformarMensagemAlerta(carga.Codigo, retorno);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValidarMensagemAlertaGrupoPessoa");
            }
        }

        public bool ValidarQuantidadeTabelasFreteDisponivel(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete, StringBuilder mensagemRetorno, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia = null)
        {
            if (tabelasFrete.Count <= 0 || tabelasFrete.Count > 1)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

                if (!carga.CargaSVM && carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    retorno.tipoTabelaFrete = tabelasFrete.Count > 0 ? tabelasFrete.Select(o => o.TipoTabelaFrete).First() : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    //retorno.dadosRetornoTipoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteCliente.TabelaFreteNaoCadastrada };

                    string tabelas = "";
                    if (tabelasFrete.Count > 0)
                        tabelas = string.Join(", ", from obj in tabelasFrete select obj.Descricao);

                    if (configuracao.CompararTabelasDeFreteParaCalculo && tabelasFrete.Count > 1)
                        retorno.mensagem = mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete para as tabelas de frete " + string.Join(", ", tabelasFrete.Select(obj => obj.Descricao)) + " compatível com as configurações da carga.\n").ToString();
                    else
                    {
                        if (cargaPedidoReferencia != null)
                            retorno.mensagem = tabelasFrete.Count <= 0 ? mensagemRetorno.Insert(0, $"Não foi localizada uma tabela de frete compatível com as configurações do pedido {cargaPedidoReferencia.Pedido?.Numero} - {cargaPedidoReferencia.Pedido?.NumeroPedidoEmbarcador}.\n").ToString() : $"Foi encontrada mais de uma tabela de frete disponível para a carga ({tabelas}).";
                        else
                            retorno.mensagem = tabelasFrete.Count <= 0 ? mensagemRetorno.Insert(0, "Não foi localizada uma tabela de frete compatível com as configurações da carga.\n").ToString() : $"Foi encontrada mais de uma tabela de frete disponível para a carga ({tabelas}).";
                    }



                    if (!apenasVerificar)
                    {
                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        if (carga.TabelaFrete != null && tabelasFrete.Count <= 0)//tinha tabela frete porem nao a encontrou mais
                            carga.TabelaFrete = null;

                        if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        {
                            carga.PossuiPendencia = true;
                            if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 9 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }
                        carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                        repCarga.Atualizar(carga);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            cargaPedido.IncluirICMSBaseCalculo = true;

                            if (!cargaPedido.Pedido.AdicionadaManualmente && cargaPedido.RegraTomador == null && !(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente ?? false))
                            {
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                                    cargaPedido.Pedido.UsarTipoPagamentoNF = true;
                                else
                                    cargaPedido.Pedido.UsarTipoPagamentoNF = false;

                                if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) > 0)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);
                                    if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                                        cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;
                                    else
                                    {
                                        cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                    }
                                }
                                else
                                {
                                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                }

                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && !cargaPedido.Pedido.UsarTipoTomadorPedido && !carga.DadosPagamentoInformadosManualmente && cargaPedido.RegraTomador == null && !carga.EmitirCTeComplementar)
                                {
                                    if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                    else
                                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                }

                                repCargaPedido.Atualizar(cargaPedido);
                                repPedido.Atualizar(cargaPedido.Pedido);
                            }
                        }
                        //Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, TipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                }

                return false;
            }

            return true;
        }

        public static bool ValidarQuantidadeTabelaFreteDisponivel(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete, List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete)
        {
            if (tabelasFrete.Count == 0 || tabelasFrete.Count > 1)
            {
                if (dadosCalculoFrete.MensagemRetorno == null)
                    dadosCalculoFrete.MensagemRetorno = string.Empty;

                dadosCalculoFrete.FreteCalculado = false;
                dadosCalculoFrete.MensagemRetorno = tabelasFrete.Count == 0 ? dadosCalculoFrete.MensagemRetorno.Insert(0, "Não foi localizada uma tabela de frete compatível com as configurações da carga.\n").ToString() : "Foi encontrada mais de uma tabela de frete disponível para a carga (" + string.Join(", ", (from obj in tabelasFrete select obj.Descricao).ToList()) + ").";

                return false;
            }

            return true;
        }

        public bool VerificarTabelaFreteExistente(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            if (apenasVerificar && ((tabelaFrete != null && tabelaFrete.TipoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente) || cargaTabelaFreteSubContratacao != null))
            {
                Servicos.Embarcador.Carga.FreteCTeSubcontratacao serFreteCTeSubcontratacao = new FreteCTeSubcontratacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);
                Servicos.Embarcador.Carga.FreteCliente serFreteCliente = new FreteCliente(unitOfWork);

                if (tabelaFrete == null || tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

                    if (cargaTabelaFreteCliente != null)
                    {
                        retorno = serFreteCliente.ObterDadosTabelaFreteCliente(cargaTabelaFreteCliente, null, carga, calculoFreteFilialEmissora, TipoServicoMultisoftware);
                    }
                    else if (cargaPedidos.All(obj => obj.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada))
                    {
                        retorno = serFreteCTeSubcontratacao.BuscarTabelaFreteSubcontratado(carga, cargaPedidos, configuracao, apenasVerificar, TipoServicoMultisoftware, unitOfWork);
                    }
                    else
                    {
                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                        Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente freteCliente = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente();
                        if (carga.TabelaFrete?.ContratoFreteTransportador != null)
                        {
                            freteCliente.ContratoFrete = "nº " + carga.TabelaFrete.ContratoFreteTransportador.Numero;
                            freteCliente.ContratoFrete += " - " + carga.TabelaFrete.ContratoFreteTransportador.Descricao;
                        }
                        retorno.dadosRetornoTipoFrete = freteCliente;
                    }

                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                }
                else
                {
                    retorno = serFreteCliente.ObterDadosTabelaFreteClientePorPedido(carga, calculoFreteFilialEmissora, unitOfWork, TipoServicoMultisoftware, apenasVerificar);
                }

                SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela, configuracao);

                return true;
            }
            else if (apenasVerificar && tabelaFrete == null && carga.ContratoFreteTransportador != null)
            {
                Servicos.Embarcador.Carga.FreteCliente serFreteCliente = new FreteCliente(unitOfWork);
                retorno = serFreteCliente.ObterDadosTabelaFreteCliente(null, carga.ContratoFreteTransportador, carga, calculoFreteFilialEmissora, TipoServicoMultisoftware);
                SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela, configuracao);
                return true;
            }

            return false;
        }

        public static string DescricaoTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabela)
        {
            string tabelaDescricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(tabela?.CodigoIntegracao))
                tabelaDescricao = tabela?.CodigoIntegracao + " - ";

            tabelaDescricao += tabela?.Descricao ?? string.Empty;

            if ((tabela?.ContratoFreteTransportador ?? null) != null)
            {
                tabelaDescricao += " - nº " + tabela.ContratoFreteTransportador.Numero;
                tabelaDescricao += " - " + tabela.ContratoFreteTransportador.Descricao;
            }

            return tabelaDescricao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga ObterDadosRotas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dadosRota = null;

            if (repCargaPercurso.ContarPorCarga(carga.Codigo) <= 0)
            {
                dadosRota = serCargaRota.CriarRota(carga, TipoServicoMultisoftware, unitOfWork, configuracaoPedido);
            }
            else
            {
                dadosRota = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga();
                dadosRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida;
            }

            return dadosRota;
        }

        public static void AlertarRotaNaoCadastrada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConfiguracaoAlerta.RotaNaoCadastrada);

                if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                    return;

                Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
                string mensagem = string.Format(Localization.Resources.Cargas.Frete.CargaFilialNaoPossuiRotaValida, carga.CodigoCargaEmbarcador, (carga.Filial == null ? "" : string.Format(Localization.Resources.Cargas.Frete.DaFilial, carga.Filial.Descricao)));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: carga.Codigo,
                        URLPagina: "Cargas/Carga",
                        titulo: Localization.Resources.Cargas.Frete.RotaNaoCadastrada,
                        nota: mensagem,
                        icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.atencao,
                        tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                        unitOfWork: unitOfWork
                    );
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        //public static void AlertarRotaNaoCadastradaPorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        //{
        //    try
        //    {
        //        if (pedido == null)
        //            return;

        //        Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
        //        Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
        //        Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConfiguracaoAlerta.RotaNaoCadastrada);

        //        if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
        //            return;

        //        List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
        //        if (pedido.Destinatario != null)
        //            destinatarios.Add(pedido.Destinatario);
        //        List<Dominio.Entidades.Localidade> destinos = destinatarios.Select(o => o.Localidade).Distinct().ToList();
        //        List<Dominio.Entidades.Estado> estadosDestino = destinos.Select(o => o.Estado).Distinct().ToList();

        //        List<Dominio.Entidades.RotaFrete> rotas = repRotaFrete.BuscarPorOrigemEDestinos(null, pedido.Remetente?.Localidade, pedido.Remetente, destinatarios, destinos, estadosDestino, null);

        //        if (rotas == null || rotas.Count == 0)
        //        {
        //            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
        //            string mensagem = $"Pedido {pedido.NumeroPedidoEmbarcador} não possui uma rota válida.";

        //            foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
        //            {
        //                servicoNotificacao.GerarNotificacaoEmail(
        //                    usuario: usuarioNotificar,
        //                    usuarioGerouNotificacao: null,
        //                    codigoObjeto: pedido.Codigo,
        //                    URLPagina: string.Empty,
        //                    titulo: "Rota não Cadastrada",
        //                    nota: mensagem,
        //                    icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.atencao,
        //                    tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
        //                    tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
        //                    unitOfWork: unitOfWork
        //                );
        //            }
        //        }
        //    }
        //    catch (Exception excecao)
        //    {
        //        Log.TratarErro(excecao);
        //    }
        //}

        //public static void AlertarFaltaTabelaFretePorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        //{
        //    try
        //    {
        //        if (pedido == null)
        //            return;

        //        Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
        //        Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
        //        Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConfiguracaoAlerta.PedidoSemTabelaFrete);

        //        if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
        //            return;

        //        bool possuiTabela = VerificarTabelaFretePedido(pedido, tipoServicoMultisoftware, unitOfWork);

        //        if (!possuiTabela)
        //        {
        //            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
        //            string mensagem = $"Pedido {pedido.NumeroPedidoEmbarcador} não possui uma tabela de frete para sua Origem e Destino.";

        //            foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
        //            {
        //                servicoNotificacao.GerarNotificacaoEmail(
        //                    usuario: usuarioNotificar,
        //                    usuarioGerouNotificacao: null,
        //                    codigoObjeto: pedido.Codigo,
        //                    URLPagina: string.Empty,
        //                    titulo: "Tabela de frete não não Cadastrada",
        //                    nota: mensagem,
        //                    icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.atencao,
        //                    tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
        //                    tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
        //                    unitOfWork: unitOfWork
        //                );
        //            }
        //        }
        //    }
        //    catch (Exception excecao)
        //    {
        //        Log.TratarErro(excecao);
        //    }
        //}

        //private static bool VerificarTabelaFretePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        //{
        //    if (pedido == null)
        //        return false;

        //    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
        //    {
        //        DataColeta = pedido.DataInicialColeta,
        //        DataFinalViagem = pedido.DataFinalViagemFaturada,
        //        DataInicialViagem = pedido.DataInicialViagemExecutada,
        //        DataVigencia = DateTime.Now,
        //        DespachoTransitoAduaneiro = pedido.DespachoTransitoAduaneiro,
        //        Empresa = null,
        //        EscoltaArmada = pedido.EscoltaArmada,
        //        QuantidadeEscolta = pedido.QtdEscolta,
        //        Filial = pedido.Filial,
        //        GerenciamentoRisco = pedido.GerenciamentoRisco,
        //        ModeloVeiculo = pedido.ModeloVeicularCarga,
        //        NecessarioReentrega = pedido.NecessarioReentrega,
        //        NumeroAjudantes = pedido.QtdAjudantes,
        //        NumeroEntregas = pedido.QtdEntregas,
        //        NumeroPallets = pedido.TotalPallets,
        //        Peso = pedido.PesoTotal,
        //        PesoLiquido = pedido.PesoLiquidoTotal,
        //        PesoCubado = pedido.PesoCubado,
        //        PesoPaletizado = 0,
        //        PossuiRestricaoTrafego = false,
        //        QuantidadeNotasFiscais = 1,
        //        Quantidades = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade>()
        //        {
        //            new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
        //            {
        //                Quantidade = pedido.PesoTotal,
        //                UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
        //            }
        //        },
        //        Rastreado = pedido.Rastreado,
        //        Rota = pedido.RotaFrete,
        //        TipoCarga = pedido.TipoDeCarga,
        //        TipoOperacao = pedido.TipoOperacao,
        //        ValorNotasFiscais = pedido.ValorTotalNotasFiscais,
        //        Veiculo = null,
        //        Volumes = pedido.QtVolumes,
        //        ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
        //        PagamentoTerceiro = false,
        //        DataBaseCRT = pedido.DataBaseCRT
        //    };

        //    parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>();
        //    parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>();

        //    if (pedido.Destinatario != null)
        //    {
        //        parametrosCalculo.Destinatarios.Add(pedido.Destinatario);

        //        if (pedido.Destinatario.Localidade != null)
        //            parametrosCalculo.Destinos.Add(pedido.Destinatario.Localidade);
        //    }

        //    parametrosCalculo.Origens = pedido.Remetente != null ? new List<Dominio.Entidades.Localidade>() { pedido.Remetente.Localidade } : new List<Dominio.Entidades.Localidade>();
        //    parametrosCalculo.Remetentes = pedido.Remetente != null ? new List<Dominio.Entidades.Cliente>() { pedido.Remetente } : new List<Dominio.Entidades.Cliente>();

        //    Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
        //    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

        //    modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");
        //    parametrosCalculo.QuantidadeEmissoesPorModeloDocumento.Add(modeloDocumentoFiscal, 1);

        //    if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
        //    {
        //        parametrosCalculo.GrupoPessoas = pedido.Remetente?.GrupoPessoas;
        //        parametrosCalculo.Tomador = pedido.Remetente;
        //    }
        //    else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
        //    {
        //        parametrosCalculo.GrupoPessoas = pedido.Destinatario?.GrupoPessoas;
        //        parametrosCalculo.Tomador = pedido.Destinatario;
        //    }
        //    else
        //    {
        //        parametrosCalculo.GrupoPessoas = pedido.Tomador?.GrupoPessoas;
        //        parametrosCalculo.Tomador = pedido.Tomador;
        //    }

        //    bool possuiTabela = false;
        //    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

        //    List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = Servicos.Embarcador.Carga.Frete.ObterTabelasFrete(parametrosCalculo, false, out StringBuilder mensagem, unitOfWork, tipoServicoMultisoftware, 0);
        //    if (Servicos.Embarcador.Carga.Frete.ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
        //    {
        //        Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork.StringConexao);
        //        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametrosCalculo, tabelasFrete[0], unitOfWork, tipoServicoMultisoftware).FirstOrDefault();

        //        if (tabelaFreteCliente != null)
        //            possuiTabela = true;
        //    }

        //    return possuiTabela;
        //}

        public static decimal CalcularValorFreteFracionadaTonelada(decimal valorFreteNegociado, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            decimal valorFrete = valorFreteNegociado;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            if (notasCarga != null && notasCarga.Count > 0)
            {
                decimal pesoTotal = notasCarga.Sum(o => o.XMLNotaFiscal.Peso);
                decimal metroCubicoTotal = notasCarga.Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                decimal totalPesoCubado = pesoTotal * metroCubicoTotal;
                valorFrete = 0;
                foreach (var nota in notasCarga)
                {
                    decimal pesoCubado = nota.XMLNotaFiscal.MetrosCubicos * nota.XMLNotaFiscal.Peso;

                    if (nota.XMLNotaFiscal.Peso > pesoCubado)
                        valorFrete += (valorFreteNegociado) * (nota.XMLNotaFiscal.Peso / 1000);
                    else
                        valorFrete += (valorFreteNegociado) * (pesoCubado / 1000);
                }
                if (valorFrete == 0)
                    valorFrete = valorFreteNegociado;
            }
            return valorFrete;
        }

        public static decimal CalcularValorFreteFracionadaMetroCubico(decimal valorFreteNegociado, decimal densidadeProduto, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            decimal valorFrete = valorFreteNegociado;
            decimal valorBaseCalculoTotal = valorFreteNegociado;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            if (notasCarga != null && notasCarga.Count > 0)
            {
                decimal pesoTotal = notasCarga.Sum(o => o.XMLNotaFiscal.Peso);
                decimal metroCubicoTotal = notasCarga.Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                decimal totalPesoCubado = pesoTotal * metroCubicoTotal;
                valorFrete = 0;
                valorBaseCalculoTotal = 0;

                foreach (var nota in notasCarga)
                {
                    if (densidadeProduto == 0m)
                        densidadeProduto = 300;

                    decimal valorBaseCalculo = valorFreteNegociado * (densidadeProduto / 1000) * nota.XMLNotaFiscal.MetrosCubicos;
                    valorBaseCalculoTotal += valorBaseCalculo;

                    valorFrete += (valorBaseCalculo * (nota.XMLNotaFiscal.Peso / pesoTotal));
                }
                if (valorBaseCalculoTotal == 0)
                    valorBaseCalculoTotal = valorFreteNegociado;
            }
            return valorBaseCalculoTotal;
        }

        public static string RetornarTaxaDocumental(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string retorno = "";

            if (carga.Pedidos != null && carga.Pedidos.Count > 0)
            {
                foreach (var pedido in carga.Pedidos)
                {
                    if (pedido.Pedido != null && pedido.Pedido.RealizarCobrancaTaxaDocumentacao)
                    {
                        retorno = "R$ " + pedido.Pedido.ValorTaxaDocumento.ToString("n2") + " a partir de " + pedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao.ToString("D") + " documentos.";
                        break;
                    }
                }
            }

            return retorno;
        }

        public static void AjustarValorDoFreteDescontandoComponenteFreteLiquido(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados)
        {
            bool temComponenteDesconto = false;

            foreach (var item in dados.Componentes.Where(x => x.DescontarComponenteFreteLiquido))
            {
                dados.ValorFrete += item.ValorComponente * -1;
                temComponenteDesconto = true;
            }

            if (dados.ValorFrete < 0 && temComponenteDesconto)
                dados.ValorFrete = 0;
        }

        public byte[] GeraComposicaoDeFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool filialEmissora)
        {
            var report = ReportRequest.WithType(ReportType.CargaComposicaoFrete)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoCarga", carga.Codigo)
                    .AddExtraData("filialEmissora", filialEmissora)
                    .CallReport()
                    .GetContentFile();
            return report;
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ObterContratoCombativel(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Empresa == null)
                return null;

            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repositorioStage.BuscarStagesPorCarga(carga.Codigo);

            List<int> codigosModelosVeiculares;
            List<int> codigosCanalEntrega;

            if (stages.Count > 0)
            {
                codigosCanalEntrega = stages.Where(stage => stage.CanalEntrega != null).Select(stage => stage.CanalEntrega.Codigo).Distinct().ToList();
                codigosModelosVeiculares = stages.Where(stage => stage.ModeloVeicularCarga != null).Select(stage => stage.ModeloVeicularCarga.Codigo).Distinct().ToList();
            }
            else
            {
                codigosCanalEntrega = repositorioCargaPedido.BuscarCodigosCanalEntregaPorCarga(carga);
                codigosModelosVeiculares = carga.ModeloVeicularCarga != null ? new List<int>() { carga.ModeloVeicularCarga.Codigo } : new List<int>();
            }

            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);

            servicoMensagemAlerta.Remover(carga, TipoMensagemAlerta.ContratoFreteTransportadorNaoAprovado);

            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratosPorTabelaFreteETransportador = repositorioContratoFreteTransportador.BuscarContratosPorTabelaFreteETransportador(tabelaFrete.Codigo, carga.Empresa.Codigo);

            if (contratosPorTabelaFreteETransportador.Count == 0)
                return null;

            Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repositorioContratoFreteTransportadorAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorFilial repositorioContratoFreteTransportadorFiliais = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga repositorioContratoFreteTransportadorTipoCarga = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ContratoTransportadorFrete> contratosDisponiveis = new List<ContratoTransportadorFrete>();
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ContratoTransportadorFrete> contratosCompativeis = new List<ContratoTransportadorFrete>();

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato in contratosPorTabelaFreteETransportador)
                contratosDisponiveis.Add(new ContratoTransportadorFrete()
                {
                    CodigoContrato = contrato.Codigo,
                    CodigosTabelaFrete = contrato.TabelasFrete.Select(tabela => tabela.Codigo).ToList(),
                    CodigosFiliais = repositorioContratoFreteTransportadorFiliais.BuscarCodigosFiliaisPorContrato(contrato.Codigo),
                    CodigosModelosVeiculares = repositorioContratoFreteTransportadorAcordo.BuscarCodigosModeloVeicularPorContrato(contrato.Codigo),
                    CodigosTipoCarga = repositorioContratoFreteTransportadorTipoCarga.BuscarCodigosTipoCargasPorContrato(contrato.Codigo),
                    CodigosCanalEntrega = contrato.CanaisEntrega.Select(canalEntrega => canalEntrega.Codigo).ToList(),
                    CodigoTransportador = contrato.Transportador.Codigo
                });

            Dominio.ObjetosDeValor.Embarcador.Frete.ContratoTransportadorFrete contratoComparar = new ContratoTransportadorFrete()
            {
                CodigosCanalEntrega = codigosCanalEntrega,
                CodigosModelosVeiculares = codigosModelosVeiculares,
                CodigosTipoCarga = new List<int>() { carga.TipoDeCarga.Codigo },
                CodigosFiliais = new List<int>() { carga.Filial.Codigo },
                CodigosTabelaFrete = new List<int>() { tabelaFrete.Codigo },
                CodigoTransportador = carga.Empresa.Codigo
            };

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ContratoTransportadorFrete contrato in contratosDisponiveis)
            {
                if (contrato.Equals(contratoComparar))
                    contratosCompativeis.Add(contrato);
            }

            if (contratosCompativeis.Count == 0)
                return null;

            List<(int CodigoContrato, int Compatibilidade)> contratosPorCompatibilidade = contratosCompativeis.Select(contrato => ValueTuple.Create(contrato.CodigoContrato, contrato.ObterQuantidadeFiltrosCompativeis(contratoComparar))).ToList();
            int maiorCompatibilidade = contratosPorCompatibilidade.Max(contrato => contrato.Compatibilidade);
            List<int> codigosContratosMaiorCompatibilidade = contratosPorCompatibilidade.Where(contrato => contrato.Compatibilidade == maiorCompatibilidade).Select(contrato => contrato.CodigoContrato).ToList();
            List<string> mensagensContratosNaoAprovados = new List<string>();

            foreach (int codigoContrato in codigosContratosMaiorCompatibilidade)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = contratosPorTabelaFreteETransportador.Where(contrato => contrato.Codigo == codigoContrato).FirstOrDefault();

                if (contratoFreteTransportador.Situacao == SituacaoContratoFreteTransportador.Aprovado)
                    return contratoFreteTransportador;

                mensagensContratosNaoAprovados.Add($"O contrato de frete ({contratoFreteTransportador.Descricao}) não foi aprovado para uso.");
            }

            servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.ContratoFreteTransportadorNaoAprovado, mensagensContratosNaoAprovados, confirmada: true);

            return null;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaPedido ObterPedidoMasDistante(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedidos.Count == 0)
                return null;

            Repositorio.Embarcador.Pedidos.PedidoStage repositorioStagePedido = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(x => x.Pedido).ToList();

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                pedidos = repositorioStagePedido.BuscarPedidosRelevanteParaCusto((from obj in pedidos select obj.Codigo).ToList());

            List<Dominio.Entidades.Localidade> destinatarioLocalidade = pedidos.Select(x => x.Destinatario.Localidade).Distinct().ToList();
            bool destinoUnico = destinatarioLocalidade.Count == 1;
            List<(int codigoPedido, double distancia)> distanciasPorPedidos = new List<(int codigoPedido, double destinos)>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                double latOrigem = Decimal.ToDouble(pedido.Remetente.Localidade.Latitude ?? 0m);
                double lngOrigem = Decimal.ToDouble(pedido.Remetente.Localidade.Longitude ?? 0m);
                double latDestino = destinoUnico
                    ? Decimal.ToDouble(destinatarioLocalidade[0].Latitude ?? 0m)
                    : Decimal.ToDouble(pedido.Destinatario.Localidade.Latitude ?? 0m);
                double lngDestino = destinoUnico
                    ? Decimal.ToDouble(destinatarioLocalidade[0].Longitude ?? 0m)
                    : Decimal.ToDouble(pedido.Destinatario.Localidade.Longitude ?? 0m);

                double distancia = CalcularDistanciaPorRoteirizacao(latOrigem, lngOrigem, latDestino, lngDestino, pedido, configuracaoIntegracao.ServidorRouteOSM);
                distanciasPorPedidos.Add((pedido.Codigo, distancia));
            }

            if (distanciasPorPedidos.Count == 0)
                return cargaPedidos.FirstOrDefault();

            int codigoPedido = distanciasPorPedidos.OrderByDescending(x => x.distancia).First().codigoPedido;
            return cargaPedidos.FirstOrDefault(x => x.Pedido.Codigo == codigoPedido);
        }

        private double CalcularDistanciaPorRoteirizacao(double latOrigem, double lngOrigem, double latDestino, double lngDestino, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string servidorRouteOSM)
        {
            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(servidorRouteOSM);
            rota.Clear();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
            wayPoints.AddRange(new[]
            {
                rota.ObterWaypoint(latOrigem, lngOrigem, pedido.Remetente.Localidade.Descricao, 1, TipoPontoPassagem.Coleta),
                rota.ObterWaypoint(latDestino, lngDestino, pedido.Destinatario.Localidade.Descricao, 2, TipoPontoPassagem.Entrega)
            });

            rota.Add(wayPoints);

            Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
            {
                AteOrigem = false,
                Ordenar = true,
                PontosNaRota = false
            };

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = rota.Roteirizar(opcoes);
            return (double)respostaRoteirizacao.Distancia;
        }

        private void RecriarComponentesFreteCargaPedidoPreCheking(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPai, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidosDoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            //com o carga Pedido da carga Pai, vamos buscar os compontenes dos cargapedidos da filho e atualizar ou criar o componente para a carga pai
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete svcComponenteFrete = new ComponetesFrete(unitOfWork);

            svcComponenteFrete.RemoverComponentesCargaPedido(cargaPedidoPai, unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesOutrasCargas = repCargaPedidoComponenteFrete.BuscarPorListaCargaPedidos(listaCargaPedidosDoPedido.Select(x => x.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFreteOutraCarga in cargaPedidoComponentesFretesOutrasCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteCargaPedidoFreteCargaPai = repCargaPedidoComponenteFrete.BuscarPorCompomente(cargaPedidoPai.Codigo, cargaPedidoComponenteFreteOutraCarga.TipoComponenteFrete, null, false);

                if (componenteCargaPedidoFreteCargaPai != null)
                {
                    componenteCargaPedidoFreteCargaPai.ValorComponente += cargaPedidoComponenteFreteOutraCarga.ValorComponente;
                    componenteCargaPedidoFreteCargaPai.ValorComponenteComICMSIncluso += cargaPedidoComponenteFreteOutraCarga.ValorComponenteComICMSIncluso;
                    repCargaPedidoComponenteFrete.Atualizar(componenteCargaPedidoFreteCargaPai);
                }
                else
                {
                    componenteCargaPedidoFreteCargaPai = cargaPedidoComponenteFreteOutraCarga.Clonar(); //new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    componenteCargaPedidoFreteCargaPai.CargaPedido = cargaPedidoPai;
                    repCargaPedidoComponenteFrete.Inserir(componenteCargaPedidoFreteCargaPai);
                }
            }
        }

        private void RecriarComponentesFreteCargaPreCheking(Dominio.Entidades.Embarcador.Cargas.Carga CargaPai, List<int> codigoCargasGeradas, Repositorio.UnitOfWork unitOfWork)
        {
            //com o carga Pedido da carga Pai, vamos buscar os compontenes dos cargapedidos da filho e atualizar ou criar o componente para a carga pai
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteCargasGeradas = repCargaComponenteFrete.BuscarPorCodigosCargas(codigoCargasGeradas);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFreteGerado in cargaComponentesFreteCargasGeradas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteFreteCarga = repCargaComponenteFrete.BuscarPorCargaETipo(CargaPai.Codigo, cargaComponenteFreteGerado.TipoComponenteFrete, null, false);

                if (componenteFreteCarga != null)
                {
                    componenteFreteCarga.ValorComponente += cargaComponenteFreteGerado.ValorComponente;
                    repCargaComponenteFrete.Atualizar(componenteFreteCarga);
                }
                else
                {
                    componenteFreteCarga = cargaComponenteFreteGerado.Clonar(); //new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    componenteFreteCarga.Carga = CargaPai;
                    repCargaComponenteFrete.Inserir(componenteFreteCarga);
                }
            }
        }

        private static void gerarErroProcessamentoFrete(int codigoCarga, String erro, MotivoPendenciaFrete motivo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            unitOfWork.Start();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoFetch(codigoCarga);
            carga.PossuiPendencia = true;
            carga.MotivoPendenciaFrete = motivo;
            carga.MotivoPendencia = erro;
            carga.DataInicioCalculoFrete = null;
            carga.CalculandoFrete = false;
            carga.CalcularFreteSemEstornarComplemento = false;
            carga.TabelaFrete = null;
            repositorioCarga.Atualizar(carga);
            unitOfWork.CommitChanges();
        }

        private static ParametrosCalculoFrete ObterParametrosCalculoFretePorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repPedidoProduto.BuscarPorPedido(pedido.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new ParametrosCalculoFrete()
            {
                DataColeta = pedido.DataInicialColeta,
                DataFinalViagem = pedido.DataFinalViagemFaturada,
                DataInicialViagem = pedido.DataInicialViagemFaturada,
                DataVigencia = DateTime.Now,
                DespachoTransitoAduaneiro = pedido.DespachoTransitoAduaneiro,
                Empresa = pedido.Empresa,
                EscoltaArmada = pedido.EscoltaArmada,
                QuantidadeEscolta = pedido.QtdEscolta,
                Filial = pedido.Filial,
                GerenciamentoRisco = pedido.GerenciamentoRisco,
                ModeloVeiculo = pedido.ModeloVeicularCarga,
                NecessarioReentrega = pedido.NecessarioReentrega,
                NumeroAjudantes = pedido.QtdAjudantes,
                NumeroDeslocamento = pedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = pedido.ValorDiaria ?? 0m,
                NumeroEntregas = pedido.QtdEntregas,
                NumeroPallets = pedido.NumeroPaletes + pedido.NumeroPaletesFracionado,
                Peso = pedido.PesoTotal,
                PesoLiquido = pedido.PesoLiquidoTotal,
                PesoCubado = pedido.PesoCubado,
                PesoPaletizado = pedido.PesoTotalPaletes,
                PossuiRestricaoTrafego = (pedido.Remetente != null && pedido.Remetente.PossuiRestricaoTrafego) || (pedido.Destinatario != null && pedido.Destinatario.PossuiRestricaoTrafego),
                QuantidadeNotasFiscais = 1,
                Quantidades = new List<ParametrosCalculoFreteQuantidade>()
                                      {
                                           new ParametrosCalculoFreteQuantidade()
                                           {
                                                Quantidade = pedido.PesoTotal,
                                                 UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                                           }
                                      },
                Rastreado = pedido.Rastreado,
                Rota = pedido.RotaFrete,
                TipoCarga = pedido.TipoDeCarga,
                TipoOperacao = pedido.TipoOperacao,
                ValorNotasFiscais = pedido.ValorTotalNotasFiscais,
                Veiculo = pedido.Veiculos.FirstOrDefault(),
                Volumes = pedidoProdutos.Sum(o => o.Quantidade),
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                PagamentoTerceiro = false,
                DataBaseCRT = pedido.DataBaseCRT,
                Cubagem = pedido.CubagemTotal,
                MaiorAlturaProdutoEmCentimetros = pedido.MaiorAlturaProdutoEmCentimetros,
                MaiorLarguraProdutoEmCentimetros = pedido.MaiorLarguraProdutoEmCentimetros,
                MaiorComprimentoProdutoEmCentimetros = pedido.MaiorComprimentoProdutoEmCentimetros,
                MaiorVolumeProdutoEmCentimetros = pedido.MaiorVolumeProdutoEmCentimetros,
                Fronteiras = pedido.RotaFrete?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
            };

            parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>() { pedido.Destinatario };
            parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>() { pedido.Destinatario.Localidade };
            int codigoDestino = pedido.Destinatario.Localidade.Codigo;

            parametrosCalculo.Remetentes = new List<Dominio.Entidades.Cliente>() { pedido.Remetente };
            parametrosCalculo.Origens = new List<Dominio.Entidades.Localidade>() { pedido.Remetente.Localidade };
            int codigoOrigem = pedido.Remetente.Localidade.Codigo;

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

            if (codigoOrigem == codigoDestino)
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
            else
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");

            parametrosCalculo.QuantidadeEmissoesPorModeloDocumento.Add(modeloDocumentoFiscal, 1);

            if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
            {
                parametrosCalculo.GrupoPessoas = pedido.Remetente?.GrupoPessoas;
                parametrosCalculo.Tomador = pedido.Remetente;
            }
            else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
            {
                parametrosCalculo.GrupoPessoas = pedido.Destinatario?.GrupoPessoas;
                parametrosCalculo.Tomador = pedido.Destinatario;
            }
            else
            {
                parametrosCalculo.GrupoPessoas = pedido.Tomador?.GrupoPessoas;
                parametrosCalculo.Tomador = pedido.Tomador;
            }

            return parametrosCalculo;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorPedidoAgrupado(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacaoEspecial, UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = pedidos.OrderBy(obj => obj.TipoDeCarga.PrioridadeCarga).FirstOrDefault();

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cotacaoEspecial.TipoOperacao;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = cotacaoEspecial.ModeloVeicularCarga;
            Dominio.Entidades.Empresa empresa = cotacaoEspecial.Transportador;

            int distancia = (int)pedidos.Sum(o => o.Distancia);

            decimal quantidadePallets = pedidos.Sum(o => o.NumeroPaletes + o.NumeroPaletesFracionado);

            decimal peso = 0;
            decimal pesoLiquido = pedidos.Sum(o => o.PesoLiquidoTotal);

            if (peso <= 0)
            {
                if (tabelaFrete.UtilizarPesoLiquido)
                    peso = pedidos.Sum(o => o.PesoLiquidoTotal);
                else
                    peso = pedidos.Sum(o => o.PesoTotal);
            }

            if (primeiroPedido.RegiaoDestino != null)
                regioesDestino.Add(primeiroPedido.RegiaoDestino);

            origens.Add(primeiroPedido.Origem);
            destinos.Add(primeiroPedido.Destino);

            if (primeiroPedido.Recebedor != null && !(tipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                destinatarios.Add(primeiroPedido.Recebedor);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroPedido.Recebedor.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }
            else if (primeiroPedido.Destinatario != null)
            {
                destinatarios.Add(primeiroPedido.Destinatario);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroPedido.EnderecoDestino?.CEP ?? primeiroPedido.Destinatario.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }

            if (primeiroPedido.ClienteDeslocamento != null)
            {
                remetentes.Add(primeiroPedido.ClienteDeslocamento);
                int.TryParse(Utilidades.String.OnlyNumbers(primeiroPedido.ClienteDeslocamento.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if (primeiroPedido.Expedidor != null && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                remetentes.Add(primeiroPedido.Expedidor);
                int.TryParse(Utilidades.String.OnlyNumbers(primeiroPedido.Expedidor.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if (primeiroPedido.Remetente != null)
            {
                remetentes.Add(primeiroPedido.Remetente);
                int.TryParse(Utilidades.String.OnlyNumbers(primeiroPedido.EnderecoOrigem?.CEP ?? primeiroPedido.Remetente.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }


            if (modeloVeicularCalculo != null)
            {
                if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeMinimaGarantidaModeloVeicular && modeloVeicularCalculo.ToleranciaPesoMenor > 0)
                {
                    if (peso < modeloVeicularCalculo.ToleranciaPesoMenor)
                        peso = modeloVeicularCalculo.ToleranciaPesoMenor;
                }
                else if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeModeloVeicular && modeloVeicularCalculo.CapacidadePesoTransporte > 0)
                    peso = modeloVeicularCalculo.CapacidadePesoTransporte;
            }

            decimal pesoCubado = pedidos.Sum(o => o.PesoCubado);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem> parametrosTipoEmbalagem = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem>();
            if (tabelaFrete.TipoEmbalagens?.Count > 0)
            {

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos.Where(p => p.Produtos != null))
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedido.Produtos)
                    {

                        if (pedidoProduto.TipoEmbalagem != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in parametrosTipoEmbalagem where obj.TipoEmbalagem.Codigo == pedidoProduto.TipoEmbalagem.Codigo select obj).FirstOrDefault();
                            if (parametroTipoEmbalagem == null)
                            {
                                parametroTipoEmbalagem = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem();
                                parametroTipoEmbalagem.TipoEmbalagem = pedidoProduto.TipoEmbalagem;
                                parametroTipoEmbalagem.Quantidade = pedidoProduto.Quantidade;
                                parametroTipoEmbalagem.Peso = pedidoProduto.PesoTotal;
                                parametrosTipoEmbalagem.Add(parametroTipoEmbalagem);
                            }
                            else
                            {
                                parametroTipoEmbalagem.Quantidade += pedidoProduto.Quantidade;
                                parametroTipoEmbalagem.Peso += pedidoProduto.PesoTotal;
                            }
                        }
                    }
                }
            }


            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
            {
                DataColeta = primeiroPedido.DataInicialColeta,
                DataFinalViagem = primeiroPedido.DataFinalViagemFaturada,
                DataInicialViagem = primeiroPedido.DataInicialViagemFaturada,
                DataVigencia = tabelaFrete.ValidarPorDataCarregamento && primeiroPedido.DataCarregamentoCarga.HasValue ? primeiroPedido.DataCarregamentoCarga.Value : primeiroPedido.DataCriacao.Value,
                DespachoTransitoAduaneiro = primeiroPedido.DespachoTransitoAduaneiro,
                NecessarioAjudante = primeiroPedido.Ajudante,
                Destinatarios = destinatarios,
                Destinos = destinos,
                PesoTotalCarga = peso,
                RegioesDestino = regioesDestino,
                Distancia = distancia,
                Empresa = empresa,
                EscoltaArmada = primeiroPedido.EscoltaArmada,
                QuantidadeEscolta = primeiroPedido.QtdEscolta,
                Filial = primeiroPedido.Filial,
                GerenciamentoRisco = primeiroPedido.GerenciamentoRisco,
                GrupoPessoas = primeiroPedido.GrupoPessoas,
                ModeloVeiculo = modeloVeicularCalculo,
                NecessarioReentrega = primeiroPedido.NecessarioReentrega,
                NumeroAjudantes = primeiroPedido.QtdAjudantes,
                NumeroDeslocamento = primeiroPedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = primeiroPedido.ValorDiaria ?? 0m,
                NumeroEntregas = primeiroPedido.QtdEntregas,
                NumeroPallets = quantidadePallets,
                Origens = origens,
                Peso = peso,
                PesoLiquido = pesoLiquido,
                PesoCubado = pesoCubado,
                Cubagem = pedidos.Sum(o => o.CubagemTotal),
                MaiorAlturaProdutoEmCentimetros = pedidos.Max(o => o.MaiorAlturaProdutoEmCentimetros),
                MaiorLarguraProdutoEmCentimetros = pedidos.Max(o => o.MaiorLarguraProdutoEmCentimetros),
                MaiorComprimentoProdutoEmCentimetros = pedidos.Max(o => o.MaiorComprimentoProdutoEmCentimetros),
                MaiorVolumeProdutoEmCentimetros = pedidos.Max(o => o.MaiorVolumeProdutoEmCentimetros),
                PesoPaletizado = pedidos.Sum(o => o.PesoTotalPaletes),
                PossuiRestricaoTrafego = (primeiroPedido.Remetente != null && primeiroPedido.Remetente.PossuiRestricaoTrafego) ||
                                         (primeiroPedido.Destinatario != null && primeiroPedido.Destinatario.PossuiRestricaoTrafego),
                Quantidades = new List<ParametrosCalculoFreteQuantidade>()
                                      {
                                           new ParametrosCalculoFreteQuantidade()
                                           {
                                                Quantidade = peso,
                                                 UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                                           }
                                      },
                Rastreado = primeiroPedido.Rastreado,
                Remetentes = remetentes,
                Rota = primeiroPedido.RotaFrete,
                TipoCarga = primeiroPedido.TipoDeCarga,
                TipoOperacao = tipoOperacao,
                Tomador = primeiroPedido.ObterTomador(),
                ValorNotasFiscais = pedidos.Sum(o => o.ValorTotalNotasFiscais),
                DataBaseCRT = primeiroPedido.DataBaseCRT,
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                TiposEmbalagem = parametrosTipoEmbalagem,
                CanalEntrega = primeiroPedido.CanalEntrega,
                CanalVenda = primeiroPedido.CanalVenda,
                DataPrevisaoEntrega = primeiroPedido.PrevisaoEntrega ?? null,
            };

            return parametrosCalculoFrete;
        }

        #endregion
    }
}

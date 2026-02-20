using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CargaDistribuidor
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public CargaDistribuidor(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaProximoTrecho(Dominio.ObjetosDeValor.Embarcador.Carga.CargaDistribuidor cargaDistribuidor)
        {
            return GerarCargaProximoTrecho(cargaDistribuidor.CargaAntiga, cargaDistribuidor.TipoOperacao, cargaDistribuidor.Distancia, cargaDistribuidor.UsarTipoOperacao, cargaDistribuidor.Expedidor, cargaDistribuidor.CargaPedidos, cargaDistribuidor.Empresa, cargaDistribuidor.ConfiguracaoTMS, cargaDistribuidor.VincularTrechos, cargaDistribuidor.Redespacho, cargaDistribuidor.Veiculo, _tipoServicoMultisoftware, _unitOfWork, cargaDistribuidor.RedespachoContainer, cargaDistribuidor.ModeloVeicularCarga, cargaDistribuidor.Recebedor, cargaDistribuidor.Motorista, cargaDistribuidor.VeiculosVinculados, cargaDistribuidor.CodigoCargaEmbarcador);
        }

        public void VerificarCargasPendentesGerarDistribuidor()
        {
            try
            {
                _unitOfWork.FlushAndClear();

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaPedido.BuscarCargaPendentesDistribuidor();

                for (int i = 0; i < cargas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];
                    Servicos.Log.TratarErro("Iniciou carga distribuidor " + carga.Codigo, "CargaDistribuidor");

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidosPendentesDistribuidor(carga.Codigo);
                    List<Dominio.Entidades.Cliente> distribuidores = (from obj in cargaPedidos select obj.Recebedor).Distinct().ToList();
                    for (int j = 0; j < distribuidores.Count; j++)
                    {
                        _unitOfWork.Start();
                        Dominio.Entidades.Cliente distribuidor = distribuidores[j];
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDistribuidores = (from obj in cargaPedidos where obj.Recebedor.CPF_CNPJ == distribuidor.CPF_CNPJ select obj).ToList();
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(distribuidor.CPF_CNPJ_SemFormato);

                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor;
                        GerarCargaProximoTrecho(carga, tipoOperacao, 0m, false, distribuidor, cargaPedidosDistribuidores, empresa, configuracaoTMS, true, null, null, _tipoServicoMultisoftware, _unitOfWork);
                        _unitOfWork.CommitChanges();
                    }

                    AtualizarCargaDistribuidor(carga);

                    Servicos.Log.TratarErro("Finalizou carga distribuidor " + carga.Codigo, "CargaDistribuidor");
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "CargaDistribuidor");
                _unitOfWork.Rollback();
            }
        }

        public static Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaProximoTrecho(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, decimal distancia, bool usarTipoOperacao, Dominio.Entidades.Cliente expedidor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool vincularTrechos, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho, Dominio.Entidades.Veiculo veiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool redespachoContainer = false, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null, Dominio.Entidades.Cliente recebedor = null, Dominio.Entidades.Usuario motorista = null, List<Dominio.Entidades.Veiculo> veiculosVinculados = null, string codigoCargaEmbarcador = null)
        {
            Servicos.Log.TratarErro($"Iniciou carga proximo trecho {cargaAntiga.Codigo}", "CargaDistribuidor");

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao repCargaPedidoDistribuicao = new Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula();
            Servicos.Embarcador.Carga.FilialEmissora serFilialEmissora = new Embarcador.Carga.FilialEmissora();
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = cargaPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao cargaPedidoDistribuicao = null;

            if (primeiroCargaPedido != null)
                cargaPedidoDistribuicao = repCargaPedidoDistribuicao.BuscarPorCargaPedido(primeiroCargaPedido.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = modeloVeicularCarga;

            if (cargaPedidoDistribuicao != null && (!usarTipoOperacao || tipoOperacao == null))
            {
                tipoOperacao = cargaPedidoDistribuicao.TipoOperacao;
                if (cargaPedidoDistribuicao.Empresa != null)
                    empresa = cargaPedidoDistribuicao.Empresa;

                if (cargaPedidoDistribuicao.ModeloVeicularCarga != null)
                    modeloVeicular = cargaPedidoDistribuicao.ModeloVeicularCarga;
            }

            Random rnd = new Random();

            if (tipoOperacao == null && cargaAntiga.TipoOperacao?.TipoOperacaoRedespacho != null)
            {
                tipoOperacao = cargaAntiga.TipoOperacao.TipoOperacaoRedespacho;
                empresa = null;
            }

            if (empresa == null && tipoOperacao != null && tipoOperacao.UtilizarExpedidorComoTransportador && expedidor != null)
                empresa = repEmpresa.BuscarPorCNPJ(expedidor.CPF_CNPJ_SemFormato);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = cargaAntiga.Filial;
            Dominio.Entidades.Embarcador.Filiais.Filial filialOrigem = null;
            if (expedidor != null && filial != null)
            {
                if (filial.CNPJ != expedidor.CPF_CNPJ_SemFormato)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filialExpedidor = repFilial.BuscarPorCNPJ(expedidor.CPF_CNPJ_SemFormato);
                    if (filialExpedidor != null)
                    {
                        filialOrigem = cargaAntiga.Filial;
                        filial = filialExpedidor;
                    }
                }
            }

            DateTime? datacarregamentoredespacho = redespachoContainer ? (DateTime?)DateTime.Now.Date.AddDays(2).Add(cargaAntiga.DataCarregamentoCarga?.TimeOfDay ?? new TimeSpan()) : null;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaRedespacho = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                AgConfirmacaoUtilizacaoCredito = cargaAntiga.AgConfirmacaoUtilizacaoCredito,
                AgImportacaoCTe = false,
                AgImportacaoMDFe = false,
                AguardandoEmissaoDocumentoAnterior = false,
                AutorizouTodosCTes = false,
                CargaCancelamento = cargaAntiga.CargaCancelamento,
                CargaFechada = true,
                CargaIntegradaEmbarcador = cargaAntiga.CargaIntegradaEmbarcador,
                CargaTransbordo = cargaAntiga.CargaTransbordo,
                CalcularFreteLote = cargaAntiga.CalcularFreteLote,
                ControleNumeracao = rnd.Next(),
                CodigoCargaEmbarcador = !string.IsNullOrWhiteSpace(codigoCargaEmbarcador) ? codigoCargaEmbarcador : cargaAntiga.CodigoCargaEmbarcador,
                ControlaTempoParaEmissao = cargaAntiga.ControlaTempoParaEmissao,
                DataCarregamentoCarga = datacarregamentoredespacho ?? cargaAntiga.DataCarregamentoCarga,
                DataEnvioUltimaNFe = cargaAntiga.DataEnvioUltimaNFe.HasValue ? DateTime.Now : cargaAntiga.DataEnvioUltimaNFe,
                DataFinalPrevisaoCarregamento = cargaAntiga.DataFinalPrevisaoCarregamento,
                DataInicialPrevisaoCarregamento = cargaAntiga.DataInicialPrevisaoCarregamento,
                DataPrevisaoTerminoCarga = cargaAntiga.DataPrevisaoTerminoCarga,
                Distancia = distancia,
                Empresa = empresa,
                ExigeNotaFiscalParaCalcularFrete = tipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? cargaAntiga.ExigeNotaFiscalParaCalcularFrete,
                Filial = filial,
                FilialOrigem = filialOrigem,
                FreteDeTerceiro = !(tipoOperacao?.NaoExigeVeiculoParaEmissao ?? false) ? cargaAntiga.FreteDeTerceiro : false,
                GrupoPessoaPrincipal = cargaAntiga.GrupoPessoaPrincipal,
                MotivoPendencia = cargaAntiga.MotivoPendencia,
                MotivoPendenciaFrete = cargaAntiga.MotivoPendenciaFrete,
                NaoExigeVeiculoParaEmissao = tipoOperacao?.NaoExigeVeiculoParaEmissao ?? cargaAntiga.NaoExigeVeiculoParaEmissao,
                Operador = cargaAntiga.Operador,
                PossuiPendencia = cargaAntiga.PossuiPendencia,
                PrioridadeEnvioIntegracao = cargaAntiga.PrioridadeEnvioIntegracao,
                problemaCTE = cargaAntiga.problemaCTE,
                problemaAverbacaoCTe = cargaAntiga.problemaAverbacaoCTe,
                problemaMDFe = cargaAntiga.problemaMDFe,
                NaoGerarMDFe = tipoOperacao?.NaoEmitirMDFe ?? cargaAntiga.NaoGerarMDFe,
                problemaNFS = cargaAntiga.problemaNFS,
                Rota = cargaAntiga.Rota,
                OrdemRoteirizacaoDefinida = cargaAntiga.OrdemRoteirizacaoDefinida,
                SegmentoGrupoPessoas = cargaAntiga.SegmentoGrupoPessoas,
                SegmentoModeloVeicularCarga = cargaAntiga.SegmentoModeloVeicularCarga,
                SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                TabelaFrete = null,
                TipoDeCarga = cargaAntiga.TipoDeCarga,
                TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela,
                TipoOperacao = tipoOperacao,
                ValorFrete = 0,
                ValorFreteAPagar = 0,
                ValorFreteEmbarcador = 0,
                ValorFreteLeilao = 0,
                ValorFreteLiquido = 0,
                ValorFreteOperador = 0,
                ValorFreteTabelaFrete = 0,
                ValorICMS = 0,
                ValorISS = 0,
                ValorRetencaoISS = 0m,
                VeiculoIntegradoEmbarcador = cargaAntiga.VeiculoIntegradoEmbarcador,
                Veiculo = null,
                LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                EmpresaFilialEmissora = null,
                Redespacho = redespacho,
                ModeloVeicularCarga = redespachoContainer ? cargaAntiga.ModeloVeicularCarga : modeloVeicular
            };

            if ((tipoOperacao?.GerarNovoNumeroCargaNoRedespacho ?? false) && string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                ObterNumeroCargaRedespacho(cargaRedespacho, tipoServicoMultisoftware, unitOfWork);

            if (veiculo != null)
            {
                if (veiculo.TipoVeiculo == "1")
                {
                    if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                        veiculo = veiculo.VeiculosTracao.FirstOrDefault();
                }

                cargaRedespacho.Veiculo = veiculo;
                if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                    cargaRedespacho.VeiculosVinculados = veiculo.VeiculosVinculados.ToList();

                if (veiculosVinculados != null && !(cargaRedespacho.VeiculosVinculados?.Count > 0))
                    cargaRedespacho.VeiculosVinculados = veiculosVinculados;
            }

            bool aguardandoEmissaoDocumentoAnterior = false;

            repCarga.Inserir(cargaRedespacho);

            Servicos.Log.TratarErro($"Inseriu a carga {cargaRedespacho.Codigo} a partir da carga {cargaAntiga.Codigo}", "CargaDistribuidor");

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaRedespacho, "Adicionada carga de redespacho com protocolo " + cargaRedespacho.Codigo.ToString(), unitOfWork);

            if (tipoOperacao?.GerarNovoNumeroCargaNoRedespacho ?? false)
                SalvarCodigosEmbarcador(cargaAntiga, cargaRedespacho, unitOfWork);

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(cargaRedespacho.Rota);

            // Replicar as fronteiras da carga antiga pra nova
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
            CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteirasCargaAntiga = serCargaFronteira.ObterFronteirasPorCarga(cargaAntiga);
            repCargaFronteira.CopiarFronteirasParaCarga(fronteirasCargaAntiga, cargaRedespacho);

            Dominio.Entidades.Usuario veiculoMotorista = veiculo != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo) : null;

            servicoCargaMotorista.AdicionarMotorista(cargaRedespacho, veiculoMotorista);

            if (veiculoMotorista == null && motorista != null)
                servicoCargaMotorista.AdicionarMotorista(cargaRedespacho, motorista);

            cargaRedespacho.Protocolo = cargaRedespacho.Codigo;

            bool gerarRedespachoAposEmissaoDocumentos = cargaAntiga.TipoOperacao?.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos ?? false;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaRedespachoPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];
                Servicos.Log.TratarErro($"Iniciou Criar Carga Pedido {cargaPedido.Codigo} a partir da carga {cargaAntiga.Codigo}", "CargaDistribuidor");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRedespacho = cargaPedido.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoRedespacho);
                cargaPedidoRedespacho.Carga = cargaRedespacho;
                cargaPedidoRedespacho.CargaOrigem = cargaRedespacho;
                cargaPedidoRedespacho.Pedido = cargaPedido.Pedido;
                cargaPedidoRedespacho.Recebedor = recebedor != null ? recebedor : tipoOperacao?.Recebedor;
                cargaPedidoRedespacho.Expedidor = expedidor;
                cargaPedidoRedespacho.CargaPedidoFilialEmissora = false;
                cargaPedidoRedespacho.Origem = expedidor != null ? expedidor.Localidade : cargaPedido.Origem;
                cargaPedidoRedespacho.Destino = cargaPedidoRedespacho.Recebedor != null ? cargaPedidoRedespacho.Recebedor.Localidade : cargaPedido.Pedido.Destino;
                cargaPedidoRedespacho.ValorFrete = 0;
                cargaPedidoRedespacho.PendenteGerarCargaDistribuidor = false;
                cargaPedidoRedespacho.ValorFreteAPagar = 0;
                cargaPedidoRedespacho.CargaRedespacho = null;
                cargaPedidoRedespacho.ValorFreteAPagarFilialEmissora = 0;
                cargaPedidoRedespacho.ValorFreteTabelaFrete = 0;
                cargaPedidoRedespacho.ValorFreteTabelaFreteFilialEmissora = 0;
                cargaPedidoRedespacho.ImpostoInformadoPeloEmbarcador = false;
                cargaPedidoRedespacho.ValorFreteFilialEmissora = 0;
                cargaPedidoRedespacho.BaseCalculoICMS = 0;
                cargaPedidoRedespacho.CargaPedidoProximoTrecho = null;
                cargaPedidoRedespacho.CargaPedidoTrechoAnterior = null;
                cargaPedidoRedespacho.ValorICMS = 0;
                cargaPedidoRedespacho.ValorAdValorem = 0;
                cargaPedidoRedespacho.ValorDescarga = 0;
                cargaPedidoRedespacho.PedidoEncaixado = false;
                cargaPedidoRedespacho.FormulaRateio = serRateioFormula.ObterFormulaDeRateio(cargaRedespacho, unitOfWork, cargaPedido);
                cargaPedidoRedespacho.Redespacho = gerarRedespachoAposEmissaoDocumentos;

                serCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedidoRedespacho, true);
                serCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedidoRedespacho, true);

                if (redespacho != null && redespacho.TipoRedespacho == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRedespacho.Reentrega)
                    cargaPedido.Pedido.ReentregaSolicitada = true;

                if (cargaPedido.Pedido.ReentregaSolicitada && configuracaoTMS.NaoEmitirDocumentosEmCargasDeReentrega)
                {
                    cargaPedidoRedespacho.PedidoSemNFe = true;
                    cargaPedidoRedespacho.CTesEmitidos = true;
                }

                bool atualizarPedido = false;

                if (cargaPedido.Pedido.ReentregaSolicitada)
                {
                    cargaPedidoRedespacho.ReentregaSolicitada = true;
                    cargaPedido.Pedido.ReentregaSolicitada = false;
                    atualizarPedido = true;
                }

                if (cargaPedidoRedespacho.Recebedor != null && cargaPedidoRedespacho.Expedidor != null)
                    cargaPedidoRedespacho.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                else if (cargaPedidoRedespacho.Expedidor != null)
                    cargaPedidoRedespacho.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                else if (cargaPedidoRedespacho.Recebedor != null)
                    cargaPedidoRedespacho.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                else
                    cargaPedidoRedespacho.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal;


                if (cargaPedido.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto)
                {
                    cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                    atualizarPedido = true;
                }

                if (atualizarPedido)
                    repPedido.Atualizar(cargaPedido.Pedido);

                bool possuiCTe = false;
                bool possuiNFS = false;
                bool possuiNFSManual = false;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;
                serCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaRedespacho, cargaPedidoRedespacho, cargaPedidoRedespacho.Origem, cargaPedidoRedespacho.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual);
                cargaPedidoRedespacho.PossuiCTe = possuiCTe;
                cargaPedidoRedespacho.PossuiNFS = possuiNFS;
                cargaPedidoRedespacho.PossuiNFSManual = possuiNFSManual;
                cargaPedidoRedespacho.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;

                //todo: rever regra de redespacho
                if (vincularTrechos || (cargaPedido.CargaPedidoProximoTrecho != null && cargaPedido.CargaPedidoFilialEmissora))
                {
                    cargaPedidoRedespacho.CargaPedidoTrechoAnterior = cargaPedido;
                    cargaPedido.CargaPedidoProximoTrecho = cargaPedidoRedespacho;
                    if (cargaPedido.CargaPedidoFilialEmissora)
                    {
                        cargaPedidoRedespacho.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                        cargaPedidoRedespacho.TipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                        if (cargaPedidoRedespacho.CargaPedidoProximoTrecho != null)
                            cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                        else if (cargaPedidoRedespacho.CargaPedidoTrechoAnterior != null)
                            cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                        else
                            cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                        aguardandoEmissaoDocumentoAnterior = true;
                    }
                    else
                    {
                        if (cargaPedidoRedespacho.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario && (cargaPedidoRedespacho.Recebedor == null || cargaPedidoRedespacho.Expedidor == null))
                            cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    }
                }
                else
                {
                    if (cargaPedidoRedespacho.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario && (cargaPedidoRedespacho.Recebedor == null || cargaPedidoRedespacho.Expedidor == null))
                        cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;

                    if (cargaRedespacho.Filial != null && cargaRedespacho.TipoOperacao != null && cargaRedespacho.Filial.EmpresaEmissora != null && cargaRedespacho.TipoOperacao.EmiteCTeFilialEmissora)
                    {
                        if (!cargaPedido.PedidoEncaixado)
                        {
                            if (cargaPedidoRedespacho.PossuiCTe)
                                cargaPedidoRedespacho.CargaPedidoFilialEmissora = true;

                            cargaPedidoRedespacho.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
                        }

                        if (cargaPedidoRedespacho.PossuiCTe)
                        {
                            Dominio.Entidades.Empresa empresaFilialEmissoraOrigem = repEmpresa.BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(expedidor?.Localidade.Estado ?? cargaPedido.Origem.Estado);
                            cargaRedespacho.EmpresaFilialEmissora = empresaFilialEmissoraOrigem != null ? empresaFilialEmissoraOrigem : cargaRedespacho.Filial.EmpresaEmissora;
                            repCarga.Atualizar(cargaRedespacho);
                        }
                    }
                }

                if (cargaRedespacho.EmpresaFilialEmissora != null && configuracaoTMS.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem && cargaRedespacho.EmpresaFilialEmissora.Localidade.Estado.Sigla != cargaPedidoRedespacho.Origem.Estado.Sigla)
                {
                    cargaPedidoRedespacho.EmitirComplementarFilialEmissora = true;
                    cargaPedidoRedespacho.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    if (cargaPedido.Recebedor != null)
                        cargaPedidoRedespacho.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;

                    if (!cargaPedidoRedespacho.PossuiNFSManual)
                        cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                }

                if ((cargaAntiga.TipoOperacao?.ConfiguracaoCarga?.UtilizarDistribuidorPorRegiaoNaRegiaoDestino ?? false))
                {
                    cargaPedidoRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    cargaRedespacho.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                }

                if (cargaAntiga.ExigeNotaFiscalParaCalcularFrete
                && (cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                || cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                || cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                || cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                ) && !cargaRedespacho.DataEnvioUltimaNFe.HasValue)
                {
                    cargaRedespacho.DataEnvioUltimaNFe = DateTime.Now;
                    repCarga.Atualizar(cargaRedespacho);
                }

                repCargaPedido.Inserir(cargaPedidoRedespacho);
                cargaRedespachoPedidos.Add(cargaPedidoRedespacho);

                Servicos.Log.TratarErro($"Inseriu carga pedido {cargaPedidoRedespacho.Codigo} a partir do carga pedido {cargaPedido.Codigo} da carga {cargaAntiga.Codigo}", "CargaDistribuidor");

                if (vincularTrechos)
                    serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

                cargaPedido.PendenteGerarCargaDistribuidor = false;
                repCargaPedido.Atualizar(cargaPedido);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProdutoTransbordo = cargaPedidoProduto.Clonar();
                    cargaPedidoProdutoTransbordo.CargaPedido = cargaPedidoRedespacho;
                    repCargaPedidoProduto.Inserir(cargaPedidoProdutoTransbordo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                if ((!cargaPedido.CargaPedidoFilialEmissora || cargaPedido.CargaPedidoProximoTrecho != null || cargaPedidoRedespacho.EmitirComplementarFilialEmissora) && pedidosCTesParaSubContratacao.Count > 0)
                {
                    if (cargaPedidoRedespacho.CargaPedidoFilialEmissora)
                    {
                        if (!cargaPedidoRedespacho.EmitirComplementarFilialEmissora)
                            cargaPedidoRedespacho.CargaPedidoFilialEmissora = false;
                        else
                        {
                            cargaPedidoRedespacho.PercentualAliquotaFilialEmissora = cargaPedido.PercentualAliquotaFilialEmissora;
                            cargaPedidoRedespacho.PercentualAliquotaFilialEmissoraInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
                            cargaPedidoRedespacho.PercentualReducaoBCFilialEmissora = cargaPedido.PercentualReducaoBCFilialEmissora;
                            cargaPedidoRedespacho.PercentualIncluirBaseCalculoFilialEmissora = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                            cargaPedidoRedespacho.IncluirICMSBaseCalculoFilialEmissora = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                            cargaPedidoRedespacho.CSTFilialEmissora = cargaPedido.CSTFilialEmissora;
                            cargaPedidoRedespacho.CFOPFilialEmissora = cargaPedido.CFOPFilialEmissora;
                        }
                        cargaPedidoRedespacho.Tomador = pedidosCTesParaSubContratacao.FirstOrDefault().CTeTerceiro.Emitente.Cliente;
                        cargaPedidoRedespacho.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        repCargaPedido.Atualizar(cargaPedidoRedespacho);
                    }


                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTesParaSubContratacao)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoClone = pedidoCTeParaSubContratacao.Clonar();

                        if (cargaPedidoRedespacho.EmitirComplementarFilialEmissora)
                        {
                            pedidoCTeParaSubContratacaoClone.CteSubContratacaoFilialEmissora = true;
                            pedidoCTeParaSubContratacaoClone.PercentualAliquota = cargaPedido.PercentualAliquotaFilialEmissora;
                            pedidoCTeParaSubContratacaoClone.PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
                            pedidoCTeParaSubContratacaoClone.PercentualReducaoBC = cargaPedido.PercentualReducaoBCFilialEmissora;
                            pedidoCTeParaSubContratacaoClone.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                            pedidoCTeParaSubContratacaoClone.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                            pedidoCTeParaSubContratacaoClone.CST = cargaPedido.CSTFilialEmissora;
                            pedidoCTeParaSubContratacaoClone.CFOP = cargaPedido.CFOPFilialEmissora;

                            servicoPedidoCTeParaSubContratacao.PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(pedidoCTeParaSubContratacaoClone, cargaPedido);
                        }

                        pedidoCTeParaSubContratacaoClone.ValorFrete = 0;
                        pedidoCTeParaSubContratacaoClone.ValorFreteTabelaFrete = 0;
                        pedidoCTeParaSubContratacaoClone.BaseCalculoICMS = 0;
                        pedidoCTeParaSubContratacaoClone.ValorICMS = 0;

                        servicoPedidoCTeParaSubContratacao.ZerarCamposImpostoIBSCBS(pedidoCTeParaSubContratacaoClone, true);

                        pedidoCTeParaSubContratacaoClone.CargaPedido = cargaPedidoRedespacho;
                        Utilidades.Object.DefinirListasGenericasComoNulas(pedidoCTeParaSubContratacaoClone);
                        repPedidoCTeParaSubContratacao.Inserir(pedidoCTeParaSubContratacaoClone);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalClone = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.Clonar();
                            pedidoXMLNotaFiscalClone.ValorFrete = 0;
                            pedidoXMLNotaFiscalClone.ValorFreteFilialEmissora = 0;
                            pedidoXMLNotaFiscalClone.ValorFreteTabelaFrete = 0;
                            pedidoXMLNotaFiscalClone.ValorFreteTabelaFreteFilialEmissora = 0;
                            pedidoXMLNotaFiscalClone.BaseCalculoICMS = 0;
                            pedidoXMLNotaFiscalClone.ValorICMS = 0;
                            if (!cargaPedidoRedespacho.EmitirComplementarFilialEmissora)
                                pedidoXMLNotaFiscalClone.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao;
                            else
                                pedidoXMLNotaFiscalClone.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;

                            servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBS(pedidoXMLNotaFiscalClone, true);
                            servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBSFilialEmissora(pedidoXMLNotaFiscalClone, true);

                            pedidoXMLNotaFiscalClone.CargaPedido = cargaPedidoRedespacho;
                            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalClone);
                            repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalClone);

                            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscalClone = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal();
                            pedidoCTeParaSubContratacaoPedidoNotaFiscalClone.PedidoXMLNotaFiscal = pedidoXMLNotaFiscalClone;
                            pedidoCTeParaSubContratacaoPedidoNotaFiscalClone.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacaoClone;
                            repPedidoCTeParaSubContratacaoNotaFiscal.Inserir(pedidoCTeParaSubContratacaoPedidoNotaFiscalClone);

                            serCanhoto.SalvarCanhotoNota(pedidoXMLNotaFiscalClone.XMLNotaFiscal, cargaPedidoRedespacho, cargaRedespacho.FreteDeTerceiro ? cargaRedespacho.Veiculo?.Proprietario ?? cargaRedespacho.ProvedorOS : null, cargaRedespacho.Motoristas != null ? cargaRedespacho.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, configuracaoTMS, unitOfWork, configuracaoCanhoto);
                        }
                    }

                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidosXMLNotaFiscal in pedidosXMLNotasFiscais)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalClone = pedidosXMLNotaFiscal.Clonar();
                        pedidoXMLNotaFiscalClone.ValorFrete = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteFilialEmissora = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteTabelaFrete = 0;
                        pedidoXMLNotaFiscalClone.ValorFreteTabelaFreteFilialEmissora = 0;
                        pedidoXMLNotaFiscalClone.BaseCalculoICMS = 0;
                        pedidoXMLNotaFiscalClone.ValorICMS = 0;
                        pedidoXMLNotaFiscalClone.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;

                        servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBS(pedidoXMLNotaFiscalClone, true);
                        servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBSFilialEmissora(pedidoXMLNotaFiscalClone, true);

                        pedidoXMLNotaFiscalClone.CargaPedido = cargaPedidoRedespacho;
                        Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalClone);
                        repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalClone);

                        serCanhoto.SalvarCanhotoNota(pedidoXMLNotaFiscalClone.XMLNotaFiscal, cargaPedidoRedespacho, cargaRedespacho.FreteDeTerceiro ? cargaRedespacho.Veiculo?.Proprietario ?? cargaRedespacho.ProvedorOS : null, cargaRedespacho.Motoristas != null ? cargaRedespacho.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, configuracaoTMS, unitOfWork, configuracaoCanhoto);

                        if ((cargaAntiga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && cargaAntiga.TipoOperacao?.TipoOperacaoRedespacho != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                            if (pedido != null)
                            {
                                if (pedido.NotasFiscais == null)
                                    pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                                pedido.NotasFiscais.Add(pedidoXMLNotaFiscalClone.XMLNotaFiscal);
                                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;

                                repPedido.Atualizar(pedido);
                                repCargaPedido.Atualizar(cargaPedido);
                            }
                        }
                    }
                }

                if (cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos)
                    serFilialEmissora.GerarCTesAnterioresDaFilialEmissoraRedespacho(cargaPedido, cargaPedidoRedespacho, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);

                Servicos.Log.TratarErro($"Finalizou Criar Carga Pedido {cargaPedido.Codigo} a partir da carga {cargaAntiga.Codigo}", "CargaDistribuidor");
            }

            if ((serCarga.VerificarSeCargaEstaNaLogistica(cargaAntiga, tipoServicoMultisoftware) ||
                cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                cargaAntiga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos) && aguardandoEmissaoDocumentoAnterior)
            {
                cargaRedespacho.AguardandoEmissaoDocumentoAnterior = true;
                repCarga.Atualizar(cargaRedespacho);
            }

            if (configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
            {
                Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                serIntegracaoCarga.InformarIntegracaoCarga(cargaRedespacho, configuracaoTMS.SistemaIntegracaoPadraoCarga, unitOfWork);
            }

            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
            {
                DiasLimiteParaDefinicaoHorarioCarregamento = configuracaoTMS.BloquearGeracaoCargaComJanelaCarregamentoExcedente ? 6 : 0
            };

            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaRedespacho, (gerarRedespachoAposEmissaoDocumentos ? cargaRedespachoPedidos : cargaPedidos), configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Log.TratarErro("Iniciou Fechar Carga " + cargaRedespacho.Codigo, "CargaDistribuidor");
            svcCarga.FecharCarga(cargaRedespacho, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware: null, recriarRotas: true, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true, propriedades);
            Servicos.Log.TratarErro("Finalizou Fechar Carga " + cargaRedespacho.Codigo, "CargaDistribuidor");
            Servicos.Log.TratarErro("7 - Fechou Carga (" + cargaRedespacho.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

            Servicos.Log.TratarErro($"Finalizou carga proximo trecho {cargaAntiga.Codigo}, gerou a carga {cargaRedespacho.Codigo}", "CargaDistribuidor");

            return cargaRedespacho;
        }

        public void GerarCargaProximoTrechoIndividualPorPedido()
        {
            try
            {
                _unitOfWork.FlushAndClear();

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaPedido.BuscarCargaPendentesGeracaoCargaSegundoTechoPorPedido();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidosPendentesGeracaoCargaSegundoTechoPorPedido(carga.Codigo);
                    Dominio.Entidades.Empresa empresa = carga.Empresa;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        _unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor;
                        if ((tipoOperacao?.UtilizarExpedidorComoTransportador ?? false) && cargaPedido.Recebedor != null)
                            empresa = null;

                        Dominio.Entidades.Embarcador.Cargas.Carga cargaRefespachada = GerarCargaProximoTrecho(carga, tipoOperacao, 0m, false, cargaPedido.Recebedor, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, empresa, configuracaoTMS, true, null, null, _tipoServicoMultisoftware, _unitOfWork);
                        if (configuracaoFinanceiro.GerarDoumentoProvisaoAoReceberNotaFiscal ?? false)
                            Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisaoPorCargaPedido(repositorioPedidoXMLNotaFiscal.BuscarPorCarga(cargaRefespachada.Codigo).FirstOrDefault(), false, _tipoServicoMultisoftware, _unitOfWork);

                        _unitOfWork.CommitChanges();
                    }

                    AtualizarCargaDistribuidor(carga);
                }
            }
            catch (Exception e)
            {
                Log.TratarErro(e, "GerarCargaProximoTrechoIndividualPorPedido");
                _unitOfWork.Rollback();
            }
        }

        public void GerarCargaProximoTrechoAposEmissao()
        {
            try
            {
                _unitOfWork.FlushAndClear();
                int quantidadeDeCargas = 10;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaPedido.BuscarCargaPendentesGeracaoCargaSegundoTrechoAposEmissao(quantidadeDeCargas);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidosPendentesDistribuidor(cargas.Select(carga => carga.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    List<Dominio.Entidades.Cliente> distribuidores = (from obj in cargaPedidos where obj.Carga.Codigo == carga.Codigo select obj.Recebedor).Distinct().ToList();

                    foreach (Dominio.Entidades.Cliente distribuidor in distribuidores)
                    {
                        _unitOfWork.Start();

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDistribuidores = (from obj in cargaPedidos where obj.Recebedor.CPF_CNPJ == distribuidor.CPF_CNPJ select obj).ToList();
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(distribuidor.CPF_CNPJ_SemFormato);

                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor;
                        GerarCargaProximoTrecho(carga, tipoOperacao, 0m, false, distribuidor, cargaPedidosDistribuidores, empresa, configuracaoTMS, true, null, null, _tipoServicoMultisoftware, _unitOfWork);

                        _unitOfWork.CommitChanges();
                    }

                    AtualizarCargaDistribuidor(carga);
                }
            }
            catch (Exception e)
            {
                Log.TratarErro(e, "GerarCargaProximoTrechoAposEmissao");
                _unitOfWork.Rollback();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarCargaDistribuidor(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            _unitOfWork.Start();

            carga.PendenteGerarCargaDistribuidor = false;
            repositorioCarga.Atualizar(carga);

            int codigoCarga = carga.Codigo;
            if (carga.CargaAgrupamento != null)
            {
                carga.CargaAgrupamento.PendenteGerarCargaDistribuidor = repositorioCarga.VerificarSePendenteGerarCargaDistribuidorPorCargaAgrupada(carga.CargaAgrupamento.Codigo);
                if (!carga.CargaAgrupamento.PendenteGerarCargaDistribuidor)
                {
                    repositorioCarga.Atualizar(carga.CargaAgrupamento);
                    codigoCarga = carga.CargaAgrupamento.Codigo;
                }
            }

            _unitOfWork.CommitChanges();

            servicoHubCarga.InformarCargaAlterada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada);
        }

        private static void ObterNumeroCargaRedespacho(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                carga.CodigoCargaEmbarcador = carga.NumeroSequenciaCarga.ToString();
            }
            else if (configuracaoEmbarcador.NumeroCargaSequencialUnico || (carga.Carregamento == null))
            {
                carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                carga.CodigoCargaEmbarcador = carga.NumeroSequenciaCarga.ToString();
            }
            else
            {
                //#58808-DECATHLON- Cargas sendo gerada com o mesmo número por importação de arquivo e montagem carga.
                if (repCarga.ExisteCarga(carga.Carregamento.NumeroCarregamento, carga.Filial?.CodigoFilialEmbarcador ?? string.Empty, false))
                {
                    carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                    carga.CodigoCargaEmbarcador = carga.NumeroSequenciaCarga.ToString();
                }
                else
                {
                    carga.CodigoCargaEmbarcador = carga.Carregamento.NumeroCarregamento;
                    carga.NumeroSequenciaCarga = carga.Carregamento.AutoSequenciaNumero;
                }
            }
        }

        private static void SalvarCodigosEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Embarcador.Cargas.Carga cargaRedespacho, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (cargaAntiga.CodigosAgrupados != null)
                foreach (string codigo in cargaAntiga.CodigosAgrupados)
                    cargaRedespacho.CodigosAgrupados?.Add(codigo);

            if (cargaRedespacho.CodigosAgrupados != null)
                cargaRedespacho.CodigosAgrupados.Add(cargaAntiga.CodigoCargaEmbarcador);

            cargaAntiga.CodigosAgrupados.Add(cargaRedespacho.CodigoCargaEmbarcador);

            repCarga.Atualizar(cargaAntiga);
            repCarga.Atualizar(cargaRedespacho);
        }

        #endregion
    }
}

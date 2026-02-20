using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CargaAgrupada
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido _configuracaoPedido;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaAgrupada(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoGeralCarga: null) { }

        public CargaAgrupada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga: null) { }

        public CargaAgrupada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoGeralCarga = configuracaoGeralCarga;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.Carga CriarCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Empresa empresa, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Filiais.Filial filial, string numeroCarga, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga, Dominio.Entidades.Empresa empresaFilialEmissora, bool cargaDeComplemento, bool cargaDePreCarga, string numeroDoca, string numeroDocaEncosta, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);

            Repositorio.Embarcador.Cargas.CargaFronteira repositorioCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork);
            Servicos.Embarcador.Carga.CargaFronteira servicoCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = ObterTipoCargaPrioritario(tiposDeCarga);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = DuplicarCarga(cargaAntiga, empresa, empresaFilialEmissora, filial, tipoCarga, rotaFrete, faixaTemperatura, numeroCarga, cargaDeComplemento, cargaDePreCarga, numeroDoca, numeroDocaEncosta, tipoServicoMultisoftware);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteirasCargaAntiga = servicoCargaFronteira.ObterFronteirasPorCarga(cargaAntiga);

            cargaAgrupada.Protocolo = cargaAgrupada.Codigo;
            cargaAgrupada.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            if (cargaAntiga.VeiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in cargaAntiga.VeiculosVinculados.ToList())
                    cargaAgrupada.VeiculosVinculados.Add(reboque);
            }

            servicoCargaMotorista.AdicionarMotoristas(cargaAntiga, cargaAgrupada);
            repositorioCargaFronteira.CopiarFronteirasParaCarga(fronteirasCargaAntiga, cargaAgrupada);

            return cargaAgrupada;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga DuplicarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Empresa empresaFilialEmissora, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, string numeroCarga, bool cargaDeComplemento, bool cargaDePreCarga, string numeroDoca, string numeroDocaEncosta, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            int proximoNumero = 0;

            if (string.IsNullOrWhiteSpace(numeroCarga))
                proximoNumero = configuracaoEmbarcador.NumeroCargaSequencialUnico ? Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork) : Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork, filial?.Codigo ?? 0);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaNova = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                AgConfirmacaoUtilizacaoCredito = cargaAntiga.AgConfirmacaoUtilizacaoCredito,
                AgImportacaoCTe = cargaAntiga.AgImportacaoCTe,
                AgImportacaoMDFe = cargaAntiga.AgImportacaoMDFe,
                AguardandoEmissaoDocumentoAnterior = cargaAntiga.AguardandoEmissaoDocumentoAnterior,
                AutorizouTodosCTes = cargaAntiga.AutorizouTodosCTes,
                CargaCancelamento = cargaAntiga.CargaCancelamento,
                CargaDeComplemento = cargaDeComplemento,
                CargaDePreCarga = cargaDePreCarga,
                CargaFechada = false,
                CargaIntegradaEmbarcador = cargaAntiga.CargaIntegradaEmbarcador,
                CargaTransbordo = cargaAntiga.CargaTransbordo,
                NumeroSequenciaCarga = proximoNumero,
                CodigoCargaEmbarcador = proximoNumero > 0 ? proximoNumero.ToString() : numeroCarga,
                CargaAgrupada = true,
                ControlaTempoParaEmissao = cargaAntiga.ControlaTempoParaEmissao,
                DataCarregamentoCarga = cargaAntiga.DataCarregamentoCarga,
                Redespacho = cargaAntiga.Redespacho,
                Rota = rotaFrete,
                DataEnvioUltimaNFe = cargaAntiga.DataEnvioUltimaNFe,
                DataFinalPrevisaoCarregamento = cargaAntiga.DataFinalPrevisaoCarregamento,
                DataInicialPrevisaoCarregamento = cargaAntiga.DataInicialPrevisaoCarregamento,
                DataPrevisaoTerminoCarga = cargaAntiga.DataPrevisaoTerminoCarga,
                Empresa = empresa,
                ExigeNotaFiscalParaCalcularFrete = cargaAntiga.ExigeNotaFiscalParaCalcularFrete,
                Filial = filial,
                FreteDeTerceiro = cargaAntiga.FreteDeTerceiro,
                GrupoPessoaPrincipal = cargaAntiga.GrupoPessoaPrincipal,
                MotivoPendencia = cargaAntiga.MotivoPendencia,
                MotivoPendenciaFrete = cargaAntiga.MotivoPendenciaFrete,
                NaoExigeVeiculoParaEmissao = cargaAntiga.NaoExigeVeiculoParaEmissao,
                Operador = cargaAntiga.Operador,
                PossuiPendencia = cargaAntiga.PossuiPendencia,
                PrioridadeEnvioIntegracao = cargaAntiga.PrioridadeEnvioIntegracao,
                problemaCTE = cargaAntiga.problemaCTE,
                problemaAverbacaoCTe = cargaAntiga.problemaAverbacaoCTe,
                problemaMDFe = cargaAntiga.problemaMDFe,
                NaoGerarMDFe = cargaAntiga.NaoGerarMDFe,
                problemaNFS = cargaAntiga.problemaNFS,
                SegmentoGrupoPessoas = cargaAntiga.SegmentoGrupoPessoas,
                SegmentoModeloVeicularCarga = cargaAntiga.SegmentoModeloVeicularCarga,
                SituacaoCarga = cargaAntiga.SituacaoCarga,
                TabelaFrete = cargaAntiga.TabelaFrete,
                TipoDeCarga = tipoCarga,
                ModeloVeicularCarga = cargaAntiga.ModeloVeicularCarga,
                TipoFreteEscolhido = cargaAntiga.TipoFreteEscolhido,
                CalcularFreteLote = cargaAntiga.CalcularFreteLote,
                TipoOperacao = cargaAntiga.TipoOperacao,
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
                Veiculo = cargaAntiga.Veiculo,
                EmpresaFilialEmissora = empresaFilialEmissora,
                PedidoViagemNavio = cargaAntiga.PedidoViagemNavio,
                TerminalOrigem = cargaAntiga.TerminalOrigem,
                TerminalDestino = cargaAntiga.TerminalDestino,
                PortoOrigem = cargaAntiga.PortoOrigem,
                ProvedorOS = cargaAntiga.ProvedorOS,
                TipoServicoCarga = cargaAntiga.TipoServicoCarga,
                PortoDestino = cargaAntiga.PortoDestino,
                DescontoSeguro = cargaAntiga.DescontoSeguro,
                PercentualDescontoSeguro = cargaAntiga.PercentualDescontoSeguro,
                DescontoFilial = cargaAntiga.DescontoFilial,
                NumeroDoca = numeroDoca,
                NumeroDocaEncosta = numeroDocaEncosta,
                FaixaTemperatura = faixaTemperatura,
                ProcedimentoEmbarque = faixaTemperatura?.ProcedimentoEmbarque,
                OrdemRoteirizacaoDefinida = cargaAntiga.OrdemRoteirizacaoDefinida,
                CargaBloqueadaParaEdicaoIntegracao = cargaAntiga.CargaBloqueadaParaEdicaoIntegracao
            };

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                if ((integracaoIntercab?.PossuiIntegracaoIntercab ?? false) && (integracaoIntercab?.TipoOperacao != null) && (integracaoIntercab?.SelecionarTipoOperacao ?? false))
                    cargaNova.TipoOperacao = integracaoIntercab.TipoOperacao;

            repositorioCarga.Inserir(cargaNova);

            return cargaNova;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido ObterConfiguracaoPedido()
        {
            if (_configuracaoPedido == null)
                _configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoPedido;
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoCargaPrioritario(List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = tiposCarga.FirstOrDefault();

            if (tiposCarga.Count > 1)
            {
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repositorioTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = repositorioTipoCargaModeloVeicularAutoConfig.Buscar();

                if (tipoCargaModeloVeicularAutoConfig != null)
                {
                    Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig repositorioTipoCargaPrioridadeCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> tipoCargaPrioridadeCargaAutoConfigs = repositorioTipoCargaPrioridadeCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig tipoCargaPrioridadeCargaAutoConfig in tipoCargaPrioridadeCargaAutoConfigs)
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaPrioritario = (from obj in tiposCarga where obj.Codigo == tipoCargaPrioridadeCargaAutoConfig.TipoDeCarga.Codigo select obj).FirstOrDefault();

                        if (tipoCargaPrioritario != null)
                        {
                            tipoCarga = tipoCargaPrioritario;
                            break;
                        }
                    }
                }
            }

            return tipoCarga;
        }

        private void VerificarSePossuiTodasCargas(Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            if (!repCarga.VerificarSeCargaAgrupamentoPossuiPreCarga(cargaAgrupada.Codigo))
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(cargaAgrupada.Codigo);
                cargaAgrupada.TipoDeCarga = ObterTipoCargaPrioritario((from obj in cargasOriginais where obj.TipoDeCarga != null select obj.TipoDeCarga).Distinct().ToList());
                cargaAgrupada.TipoOperacao = cargasOriginais.FirstOrDefault().TipoOperacao;
                cargaAgrupada.EmpresaFilialEmissora = cargasOriginais.FirstOrDefault().EmpresaFilialEmissora;
                cargaAgrupada.ExigeNotaFiscalParaCalcularFrete = cargasOriginais.FirstOrDefault().ExigeNotaFiscalParaCalcularFrete;
                cargaAgrupada.PendenteGerarCargaDistribuidor = cargasOriginais.Any(obj => obj.PendenteGerarCargaDistribuidor);
                if (cargasOriginais.Any(obj => !obj.DataEnvioUltimaNFe.HasValue))
                    cargaAgrupada.DataEnvioUltimaNFe = null;
                else
                    cargaAgrupada.DataEnvioUltimaNFe = cargasOriginais.FirstOrDefault().DataEnvioUltimaNFe;

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                svcCarga.FecharCarga(cargaAgrupada, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, true);
                cargaAgrupada.CargaDePreCarga = false;
                cargaAgrupada.CargaFechada = true;
                repCarga.Atualizar(cargaAgrupada);
            }
            else if (cargaAgrupada.DataEnvioUltimaNFe.HasValue)
            {
                cargaAgrupada.DataEnvioUltimaNFe = null;
                repCarga.Atualizar(cargaAgrupada);
            }
        }

        private void RemoverRetiradaContainerCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.RetiradaContainer repRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retirada = repRetiradaContainer.BuscarPorCarga(carga.Codigo);

            if (retirada != null)
                repRetiradaContainer.Deletar(retirada);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Carga AgruparCargas(Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.Entidades.Embarcador.Filiais.Filial filial = null)
        {
            return AgruparCargas(cargaAgrupada, cargas, preAgrupamentoCargas: null, numeroCarga: "", filial, cargaTroca: null, empresa: null, tipoServicoMultisoftware, ClienteMultisoftware, limparOrdemColetaEntrega: true, carregamento: null);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga AgruparCargas(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas, string numeroCarga, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.Carga cargaTroca, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            return AgruparCargas(null, cargas, preAgrupamentoCargas, numeroCarga, filial, cargaTroca, empresa: null, tipoServicoMultisoftware, ClienteMultisoftware, limparOrdemColetaEntrega: true, carregamento: null);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga AgruparCargas(Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas, string numeroCarga, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.Carga cargaTroca, Dominio.Entidades.Empresa empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, bool limparOrdemColetaEntrega, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMdfe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoCarregamento repositorioTipoDecarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarziados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(_unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = ObterConfiguracaoPedido();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = carregamento != null ? repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro() : null;
            Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamentoBoxAgrupada = repositorioTipoDecarregamento.BuscarTipoPadraoCargaAgrupada();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoDeCargas = (from obj in cargas where obj.TipoDeCarga != null select obj.TipoDeCarga).Distinct().ToList();
            List<Dominio.Entidades.Empresa> empresas = (from obj in cargas where obj.Empresa != null select obj.Empresa).Distinct().ToList();
            Dominio.Entidades.Empresa empresaFilialEmissora = (from obj in cargas where obj.EmpresaFilialEmissora != null select obj.EmpresaFilialEmissora).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from obj in cargas select obj.TipoOperacao).Distinct().ToList();

            bool manterUnicaCarga = false;
            if (tiposOperacao.Exists(obj => obj.ManterUnicaCargaNoAgrupamento))
                manterUnicaCarga = true;

            if (cargas.Count < 2)
            {
                if (cargaAgrupada == null)
                    throw new ServicoException("É obrigatório informar ao menos duas cargas para realizar o agrupamento.");

                if (cargas.Count == 1)
                {
                    if (cargas.FirstOrDefault().CargaAgrupamento == null || cargas.FirstOrDefault().CargaAgrupamento.Codigo != cargaAgrupada.Codigo)
                        throw new ServicoException($"A carga {cargas.FirstOrDefault().CodigoCargaEmbarcador} não pertence a esse carregamento para ser removida.");
                }
            }

            bool cargaDeComplemento = cargas.Exists(obj => obj.CargaDeComplemento);
            bool cargaDePreCarga = cargas.Exists(obj => obj.CargaDePreCarga);
            bool pendenteGerarCargaDistribuidor = cargas.Exists(obj => obj.PendenteGerarCargaDistribuidor);

            Dominio.Entidades.RotaFrete rotaFrete = null;
            List<Dominio.Entidades.RotaFrete> rotasFrete = (from obj in cargas where obj.Rota != null select obj.Rota).Distinct().ToList();
            if (rotasFrete.Count == 1 && (!configuracaoEmbarcador.ExigirRotaRoteirizadaNaCarga || tiposOperacao.Exists(obj => obj.NaoExigeRotaRoteirizada)))
                rotaFrete = rotasFrete.FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in cargas where obj.Filial != null select obj.Filial).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filialVerificar in filiais)
            {
                if (filialVerificar.NaoPermitirAgruparCargaMesmaFilial && cargas.Where(obj => obj.Filial?.Codigo == filialVerificar.Codigo).Count() > 1)
                    throw new ServicoException($"Não é permitir agrupar cargas da mesma filial ({filialVerificar.Descricao}).");
            }

            Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = (from obj in cargas where obj.FaixaTemperatura != null select obj.FaixaTemperatura).FirstOrDefault();

            string numeroDoca = "";
            string numeroDocaEncosta = "";
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                if (!string.IsNullOrWhiteSpace(carga.NumeroDoca))
                    numeroDoca = carga.NumeroDoca;

                if (!string.IsNullOrWhiteSpace(carga.NumeroDocaEncosta))
                    numeroDocaEncosta = carga.NumeroDocaEncosta;
            }

            bool atualizarCargaAgrupada = false;

            Servicos.Log.TratarErro($"AGPCTS-0 - carga: {cargaAgrupada?.Codigo}, qtde: {cargas.Count} codigos: {(cargas.Count > 0 ? string.Join(",", cargas.Select(o => o.Codigo)) : string.Empty)}", "AgrupamentosCanhotos");
            if (!manterUnicaCarga)
            {
                if (filial == null)
                {
                    if (filiais.Count > 1)
                        filial = (from obj in filiais orderby obj.Codigo select obj).FirstOrDefault();
                    else if (filiais.Count > 0)
                        filial = filiais.FirstOrDefault();
                }

                if (cargaAgrupada == null)
                    cargaAgrupada = CriarCargaAgrupada(cargas.FirstOrDefault(), empresa ?? empresas?.FirstOrDefault(), rotaFrete, filial, numeroCarga, tipoDeCargas, empresaFilialEmissora, cargaDeComplemento, cargaDePreCarga, numeroDoca, numeroDocaEncosta, faixaTemperatura, tipoServicoMultisoftware);
                else
                    atualizarCargaAgrupada = true;

                if (cargas.Count > 0)
                {
                    //#68553 - Comentado e atribuido o carregamento somente no processo de agrupar pelo carregamento
                    // aonde todas as cargas geradas (uma para cada filial) possuem o mesmo carregamento/roteirização.
                    //cargaAgrupada.Carregamento = cargas.FirstOrDefault().Carregamento;
                    cargaAgrupada.CodigosAgrupados = new List<string>();
                }
            }
            else
            {

                if (tiposOperacao.Count > 1 || empresas.Count > 1 || filiais.Count > 1)
                    throw new ServicoException($"Cargas com o tipo de operação {tiposOperacao.FirstOrDefault().Descricao} permitem que somente cargas com a mesma configuração sejam agrupadas.");

                cargaAgrupada = cargas.FirstOrDefault();
                cargaAgrupada.CargaDePreCarga = cargaDePreCarga;
                cargaAgrupada.CargaDeComplemento = cargaDeComplemento;

                if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaAgrupada, tipoServicoMultisoftware))
                    throw new ServicoException($"A atual situação da carga {cargaAgrupada.CodigoCargaEmbarcador} não permite que ela seja agrupada.");

                if (!cargaAgrupada.CargaDeVinculo || cargaAgrupada.CodigosAgrupados == null)
                {
                    cargaAgrupada.CodigosAgrupados = new List<string>();
                    if (manterUnicaCarga)
                        cargaAgrupada.CodigosAgrupados.Add(cargaAgrupada.CodigoCargaEmbarcador);
                }

                cargaAgrupada.CargaDeVinculo = true;

                if (string.IsNullOrEmpty(numeroCarga))
                {
                    int proximoNumero;
                    if (configuracaoEmbarcador.NumeroCargaSequencialUnico)
                        proximoNumero = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);
                    else
                        proximoNumero = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork, filiais.FirstOrDefault()?.Codigo ?? 0);

                    cargaAgrupada.NumeroSequenciaCarga = proximoNumero;
                    cargaAgrupada.CodigoCargaEmbarcador = proximoNumero.ToString();
                }
                else
                    cargaAgrupada.CodigoCargaEmbarcador = numeroCarga;

                cargaAgrupada.Rota = rotaFrete;
                cargaAgrupada.DataCriacaoCarga = DateTime.Now;
                cargaAgrupada.CalcularFreteLote = Dominio.Enumeradores.LoteCalculoFrete.Integracao;
                cargaAgrupada.CarregamentoIntegradoERP = false;

                new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);
            }

            if (carregamento != null)
                cargaAgrupada.Carregamento = carregamento;

            if (tipoCarregamentoBoxAgrupada != null)
                cargaAgrupada.TipoCarregamento = tipoCarregamentoBoxAgrupada;

            if (empresas.Count > 1)
            {
                string raiz = empresas.FirstOrDefault().RaizCnpj;

                foreach (Dominio.Entidades.Empresa empresaRaiz in empresas)
                {
                    if (!empresaRaiz.RaizCnpj.Contains(raiz))
                        throw new ServicoException("Não é possível agrupar cargas de diferentes transportadores.");
                }
            }

            bool enviouTodasNotas = true;

            int index = 0;
            if (manterUnicaCarga || (cargas.Count == 1 && atualizarCargaAgrupada))
                index = 1;

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            if (atualizarCargaAgrupada)
                cargasOriginais = repCarga.BuscarCargasOriginais(cargaAgrupada.Codigo);

            List<int> codigosCargas = (from o in cargas select o.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorCargasFetchBasico(codigosCargas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargasLacres = repCargaLacre.BuscarPorCargas(codigosCargas);

            Servicos.Embarcador.Monitoramento.Monitoramento.ExcluirMonitoriaPorCargas(codigosCargas, configuracaoEmbarcador, _unitOfWork);

            if (cargas != null && cargas.Count() > 0 && cargas.FirstOrDefault().Integracoes != null)
            {
                if (cargas.FirstOrDefault().AgruparCargaAutomaticamente)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao integracao in cargas.FirstOrDefault().Integracoes)
                    {
                        Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(_unitOfWork);
                        serIntegracaoCarga.InformarIntegracaoCarga(cargaAgrupada, integracao.TipoIntegracao.Tipo, _unitOfWork);
                    }
                }
                else if (cargas.FirstOrDefault().Integracoes.Any(i => i.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo))
                {
                    Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(_unitOfWork);
                    serIntegracaoCarga.InformarIntegracaoCarga(cargaAgrupada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo, _unitOfWork);
                }
            }

            Servicos.Log.TratarErro("Iniciou agrupamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");
            for (int i = index; i < cargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];

                if (carga.CargaAgrupamento != null && carga.CargaAgrupamento.Codigo != cargaAgrupada?.Codigo)
                    throw new ServicoException($"A carga {carga.CodigoCargaEmbarcador} já está em outro agrupamento.");

                if (carga.CargaAgrupada)
                    throw new ServicoException("A carga já é uma carga agrupada e não pode mais ser modificada, se necessário cancele a mesma e refaça o processo.");

                if (!carga.AgruparCargaAutomaticamente && !serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                    throw new ServicoException($"A atual situação da carga {carga.CodigoCargaEmbarcador} não permite que ela seja agrupada.");

                if (carga.CalculandoFrete)
                    throw new ServicoException($"A carga {carga.CodigoCargaEmbarcador} está em calculo de frete.");

                if (carga.ContratoFreteTransportador != null)
                {
                    repContratoSaldoMes.DeletarPorCarga(carga.Codigo);

                    carga.ContratoFreteTransportador = null;
                    carga.ValorFreteContratoFrete = 0;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargasPedidos where obj.Carga.Codigo == carga.Codigo select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    cargaPedido.Carga = cargaAgrupada;

                    if (manterUnicaCarga)
                        cargaPedido.CargaOrigem = cargaAgrupada;

                    if (preAgrupamentoCargas != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = (from obj in preAgrupamentoCargas where obj.Carga.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                        if (preAgrupamentoCarga != null && cargaPedido.Recebedor == null && !string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjRecebedor))
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                            double recebedor = 0;
                            double.TryParse(preAgrupamentoCarga.CnpjRecebedor, out recebedor);
                            cargaPedido.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);

                            if (cargaPedido.Recebedor != null)
                                cargaPedido.Destino = cargaPedido.Recebedor.Localidade;

                            if (cargaPedido.Expedidor != null)
                                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                            else
                                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                        }
                    }

                    if (limparOrdemColetaEntrega)
                    {
                        cargaPedido.OrdemEntrega = 0;
                        cargaPedido.OrdemColeta = 0;
                    }

                    repCargaPedido.Atualizar(cargaPedido);
                }

                bool agrupandoComDocumentos = configuracaoMontagemCarga?.ExibirListagemNotasFiscais ?? false;
                if ((carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte && configuracaoEmbarcador.RetornarCargaDocumentoEmitido) || agrupandoComDocumentos)
                {
                    if (!agrupandoComDocumentos)
                        cargaAgrupada.AgrupadaPosEmissaoDocumento = true;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCte = repCargaCte.BuscarPorCarga(carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargasCte)
                    {
                        cargaCte.Carga = cargaAgrupada;
                        repCargaCte.Atualizar(cargaCte);
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> CargasMdfe = repCargaMdfe.BuscarPorCarga(carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMdfe in CargasMdfe)
                    {
                        cargaMdfe.Carga = cargaAgrupada;
                        repCargaMdfe.Atualizar(cargaMdfe);
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = (from obj in cargasLacres where obj.Carga.Codigo == carga.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre in cargaLacres)
                {
                    cargaLacre.Carga = cargaAgrupada;
                    repCargaLacre.Atualizar(cargaLacre);
                }

                if (configuracaoEmbarcador.ManterOperacaoUnicaEmCargasAgrupadas)
                    carga.TipoOperacao = cargaAgrupada.TipoOperacao;

                if (configuracaoGeralCarga.ManterTransportadorUnicoEmCargasAgrupadas)
                    carga.Empresa = cargaAgrupada.Empresa;

                carga.OcultarNoPatio = !configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada;
                carga.CargaFechada = false;
                carga.AgruparCargaAutomaticamente = false;

                if (!manterUnicaCarga)
                    carga.CargaAgrupamento = cargaAgrupada;
                else
                    carga.CargaVinculada = cargaAgrupada;

                if (carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    if (!carga.DataEnvioUltimaNFe.HasValue && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        enviouTodasNotas = false;
                }
                else
                    enviouTodasNotas = false;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                    cargaAgrupada.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                else if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && cargaAgrupada.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                    cargaAgrupada.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;

                carga.DataEnvioUltimaNFe = null;

                if (!manterUnicaCarga)
                {
                    carga.Veiculo = cargaAgrupada.Veiculo;
                    carga.VeiculosVinculados = cargaAgrupada.VeiculosVinculados?.ToList();

                    string raizCNPJEmpresaAgrup = cargaAgrupada.Empresa != null ? Utilidades.String.OnlyNumbers(cargaAgrupada.Empresa.CNPJ).Remove(8, 6) : "";
                    string raizEmpresa = carga.Empresa != null ? Utilidades.String.OnlyNumbers(carga.Empresa.CNPJ).Remove(8, 6) : "";

                    if (carga.Empresa == null)
                        carga.Empresa = cargaAgrupada.Empresa;

                    List<Dominio.Entidades.Usuario> motoristasCargaAgrupada = repCargaMotorista.BuscarMotoristasPorCarga(cargaAgrupada.Codigo);

                    servicoCargaMotorista.AtualizarMotoristas(carga, motoristasCargaAgrupada);
                }

                servicoCargaJanelaCarregamento.RemoverPorCargaEmAgrupamento(carga);
                servicoCargaJanelaDescarregamento.RemoverPorCargaEmAgrupamento(carga);

                if (configuracaoEmbarcador.PermiteInformarModeloVeicularCargaOrigem && carga.ModeloVeicularOrigem == null)
                    carga.ModeloVeicularOrigem = carga.ModeloVeicularCarga;

                repCarga.Atualizar(carga);

                if (!carga.CargaDeVinculo || carga.CodigosAgrupados == null)
                {
                    if (!cargaAgrupada.CodigosAgrupados.Contains(carga.CodigoCargaEmbarcador))
                        cargaAgrupada.CodigosAgrupados.Add(carga.CodigoCargaEmbarcador);
                }
                else
                {
                    foreach (string codigoCargaAnterior in carga.CodigosAgrupados)
                        cargaAgrupada.CodigosAgrupados.Add(codigoCargaAnterior);
                }

                if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    cargaAgrupada.ValorFrete += carga.ValorFrete;
                    cargaAgrupada.ValorFreteAPagar += carga.ValorFreteAPagar;
                }

                RemoverRetiradaContainerCargaAgrupada(carga);

            }

            Servicos.Log.TratarErro("Finalizou agrupamentos" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");


            if (enviouTodasNotas)
                cargaAgrupada.DataEnvioUltimaNFe = DateTime.Now;
            else
                cargaAgrupada.DataEnvioUltimaNFe = null;

            cargaAgrupada.PendenteGerarCargaDistribuidor = pendenteGerarCargaDistribuidor;
            cargaAgrupada.AgIntegracaoAgrupamentoCarga = true;
            cargaAgrupada.DataPrevisaoChegadaOrigem = cargas != null && cargas.Count() > 0 ? cargas.OrderBy(obj => obj.DataPrevisaoChegadaOrigem)?.FirstOrDefault()?.DataPrevisaoChegadaOrigem : null;

            if (limparOrdemColetaEntrega)
            {
                repCargaPedido.removerOrdemColeta(cargaAgrupada.Codigo);
                repCargaPedido.RemoverOrdemEntrega(cargaAgrupada.Codigo);
            }

            Servicos.Log.TratarErro($"AGPCTS-1 - carga: {cargaAgrupada.Codigo}, qtde: {cargas?.Count}, manter única: {manterUnicaCarga}, atualizar: {atualizarCargaAgrupada}", "AgrupamentosCanhotos");
            if (!manterUnicaCarga)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedadesGeracaoCarga = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                {
                    PermitirHorarioCarregamentoComLimiteAtingido = true
                };

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                //List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxoGestaoPatioCargaAgrupada = new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();
                if (atualizarCargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDesagrupar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                    if (cargas.Count < 2)
                    {
                        cargasDesagrupar = cargasOriginais;
                    }
                    else
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOriginais)
                        {
                            if (!cargas.Contains(cargaOrigem))
                                cargasDesagrupar.Add(cargaOrigem);
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaDesagrupar in cargasDesagrupar)
                    {
                        repCargaPedido.SetarCargaOrigem(cargaDesagrupar.Codigo, cargaDesagrupar.Codigo);
                        cargaDesagrupar.CargaAgrupamento = null;
                        cargaDesagrupar.CargaFechada = true;
                        cargaDesagrupar.Veiculo = null;
                        cargaDesagrupar.OcultarNoPatio = false;
                        cargaDesagrupar.VeiculosVinculados.Clear();
                        List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaDesagrupar.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                            repCargaMotorista.Deletar(cargaMotorista);

                        repCarga.Atualizar(cargaDesagrupar);
                        svcCarga.FecharCarga(cargaDesagrupar, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: true, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true, propriedadesGeracaoCarga);

                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(cargaDesagrupar.Codigo);

                        if (cargaJanelaCarregamento != null)
                            servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
                    }

                    if (cargas.Count < 2)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaManter = cargas.FirstOrDefault();
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupadaTemporaria = DuplicarCarga(cargaAgrupada, cargaAgrupada.Empresa, cargaAgrupada.EmpresaFilialEmissora, cargaAgrupada.Filial, cargaAgrupada.TipoDeCarga, cargaAgrupada.Rota, cargaAgrupada.FaixaTemperatura, cargaAgrupada.CodigoCargaEmbarcador, cargaAgrupada.CargaDeComplemento, cargaAgrupada.CargaDePreCarga, cargaAgrupada.NumeroDoca, cargaAgrupada.NumeroDocaEncosta, tipoServicoMultisoftware);

                        Servicos.Embarcador.Carga.Carga.TrocarCarga(cargaManter, cargaAgrupadaTemporaria, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: false);
                        Servicos.Embarcador.Carga.Carga.TrocarCarga(cargaAgrupada, cargaManter, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: false);
                        Servicos.Embarcador.Carga.Carga.TrocarCarga(cargaAgrupadaTemporaria, cargaAgrupada, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: false);

                        cargaManter.CargaFechada = true;
                        cargaAgrupada.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada;
                        cargaAgrupada.CargaFechada = false;
                        cargaAgrupada.OcultarNoPatio = true;

                        repCarga.Atualizar(cargaManter);
                        repCarga.Atualizar(cargaAgrupada);
                        repCarga.Deletar(cargaAgrupadaTemporaria);
                    }
                }

                _unitOfWork.Flush();

                if (cargas.Count > 1 || !atualizarCargaAgrupada)
                {
                    Servicos.Log.TratarErro($"AGPCTS-2 - cargaTroca: {cargaTroca?.Codigo ?? 0}", "AgrupamentosCanhotos");
                    if (cargaTroca != null)
                    {
                        Servicos.Embarcador.Carga.Carga.TrocarCarga(cargaTroca, cargaAgrupada, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: false);
                        repCanhoto.SetarTransportadorCanhotosPorCargaAgrupadaOuCargaNova(cargaAgrupada.Codigo, cargaAgrupada.Empresa?.Codigo ?? 0, cargaTroca.Codigo);
                        Servicos.Log.TratarErro($"AGPCTS-3 - TrocaCargaTroca - emp: {cargaAgrupada.Empresa?.Codigo ?? 0}", "AgrupamentosCanhotos");
                        svcCarga.FecharCarga(cargaAgrupada, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: true, adicionarJanelaDescarregamento: false, adicionarJanelaCarregamento: false, validarDados: false, gerarAgendamentoColeta: true, propriedadesGeracaoCarga);
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamentoCargaAgrupada = svcCarga.AdicionarCargaJanelaCarregamento(cargaAgrupada, configuracaoEmbarcador, tipoServicoMultisoftware, _unitOfWork);

                        if (configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada)
                        {
                            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisCargasFilhas = (from o in cargas where o.Filial != null && o.Filial.Codigo != cargaAgrupada.Filial.Codigo select o.Filial).Distinct().ToList();

                            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filialCargaFilha in filiaisCargasFilhas)
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga cargaFilha = (from o in cargas where o.Filial.Codigo == filialCargaFilha.Codigo select o).FirstOrDefault();
                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoFillha = repCargaJanelaCarregamento.BuscarPorCarga(cargaFilha.Codigo);

                                if (cargaJanelaCarregamentoFillha != null)
                                {
                                    svcCarga.AdicionarCargaJanelaDescarregamento(cargaAgrupada, cargaJanelaCarregamentoFillha, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                                    break;
                                }
                            }
                        }
                        else
                            svcCarga.AdicionarCargaJanelaDescarregamento(cargaAgrupada, janelaCarregamentoCargaAgrupada, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                    }
                    else
                    {
                        repCanhoto.SetarTransportadorCanhotosPorCargaAgrupadaOuCargaNova(cargaAgrupada.Codigo, cargaAgrupada.Empresa?.Codigo ?? 0);
                        Servicos.Log.TratarErro($"AGPCTS-3 - TrocaSemCargaTroca - emp: {cargaAgrupada.Empresa?.Codigo ?? 0}", "AgrupamentosCanhotos");
                        svcCarga.FecharCarga(cargaAgrupada, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: true, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true, propriedadesGeracaoCarga);
                    }

                    if (limparOrdemColetaEntrega)
                        cargaAgrupada.OrdemRoteirizacaoDefinida = false;

                    cargaAgrupada.CargaFechada = true;
                    Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(cargaAgrupada, cargasPedidos, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);

                    Servicos.Log.TratarErro("6 - Fechou Carga (" + cargaAgrupada.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                    repCarga.Atualizar(cargaAgrupada);
                }
                //else if (cargas.Count == 1 && fluxoGestaoPatioCargaAgrupada.Count > 0)
                //{
                //    Dominio.Entidades.Embarcador.Cargas.Carga cargaManter = cargas.FirstOrDefault();
                //    List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxoGestaoesPatioCarga = repFluxoGestaoPatio.BuscarPorCarga(cargaManter.Codigo);
                //    foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioCarga in fluxoGestaoesPatioCarga)
                //    {
                //        fluxoGestaoPatioCarga.Carga = cargaAgrupada;
                //        fluxoGestaoPatioCarga.SituacaoEtapaFluxoGestaoPatio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Cancelado;
                //        repFluxoGestaoPatio.Atualizar(fluxoGestaoPatioCarga);
                //    }

                //    foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio in fluxoGestaoPatioCargaAgrupada)
                //    {
                //        fluxoGestaoPatio.Carga = cargaManter;
                //        repFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
                //    }
                //}
            }
            else
            {
                if (limparOrdemColetaEntrega)
                    cargaAgrupada.OrdemRoteirizacaoDefinida = false;

                repCanhoto.SetarTransportadorCanhotosPorCargaVinculada(cargaAgrupada.Codigo, cargaAgrupada.Empresa?.Codigo ?? 0);
                Servicos.Log.TratarErro($"AGPCTS-3 - TrocaSemManterUnica - emp: {cargaAgrupada.Empresa?.Codigo ?? 0}", "AgrupamentosCanhotos");

                if (cargaAgrupada.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando)
                    cargaAgrupada.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;

                if (cargaAgrupada.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                {
                    cargaAgrupada.CalculandoFrete = true;
                    cargaAgrupada.DataInicioCalculoFrete = DateTime.Now;
                }

                if (cargas.Any(x => x.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador))
                {
                    cargaAgrupada.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    cargaAgrupada.ValorFrete = cargas.Sum(obj => obj.ValorFrete);

                    // vamos inserir componentes frete das cargas origem para a carga agrupada;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponenteFreteCargaOrigem = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
                    foreach (var cargaOrigem in cargas)
                        cargaComponenteFreteCargaOrigem.AddRange(repCargaComponentesFrete.BuscarPorCarga(cargaOrigem.Codigo));

                    if (cargaComponenteFreteCargaOrigem.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargas)
                        {
                            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = (from obj in cargaComponenteFreteCargaOrigem select obj.ComponenteFrete).Distinct().ToList();
                            foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente in componentes)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesCargaAgrupada = (from obj in cargaComponenteFreteCargaOrigem where obj.Carga.Codigo == cargaOrigem.Codigo && obj.ComponenteFrete == componente select obj).ToList();
                                if (cargaComponentesFretesCargaAgrupada != null)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = cargaComponentesFretesCargaAgrupada.FirstOrDefault();
                                    if (cargaComponentesFrete != null)
                                        serComponetesFrete.AdicionarComponenteFreteCarga(cargaAgrupada, componente, cargaComponentesFretesCargaAgrupada.Sum(obj => obj.ValorComponente), cargaComponentesFrete.Percentual, cargaComponentesFrete.ComponenteFilialEmissora, cargaComponentesFrete.TipoValor, cargaComponentesFrete.TipoComponenteFrete, null, cargaComponentesFrete.IncluirBaseCalculoICMS, cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro, cargaComponentesFrete.ModeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, _unitOfWork, false);
                                }
                            }
                        }
                    }
                }

                repCarga.Atualizar(cargaAgrupada);

                List<Dominio.Entidades.Estado> estadosDestino = (from obj in cargasPedidos select obj.Destino.Estado).Distinct().ToList();

                if (estadosDestino.Count > 0)
                {
                    Servicos.Log.TratarErro("inciou ajustes locais prestacao" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEstado = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                    foreach (Dominio.Entidades.Estado estadoDestino in estadosDestino)
                        cargaPedidosEstado.Add((from obj in cargasPedidos where obj.Destino.Estado.Sigla == estadoDestino.Sigla select obj).FirstOrDefault());

                    serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(cargaAgrupada, cargaPedidosEstado, _unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                    Servicos.Log.TratarErro("finalizou ajustes locais prestacao" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");
                }

                Servicos.Log.TratarErro("iniciou ajustes rota frete" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");
                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(cargaAgrupada, cargasPedidos, configuracaoEmbarcador, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                Servicos.Log.TratarErro("finalizou rota Frete" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");

                Servicos.Log.TratarErro("iniciou dados sumarizados" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");
                serCargaDadosSumarziados.AlterarDadosSumarizadosCarga(ref cargaAgrupada, cargasPedidos, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                Servicos.Log.TratarErro("finalizou dados sumarizados" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Agrupamentos");

                Servicos.Embarcador.Carga.CargaPedido.SumarizarDadosZonaTransporte(cargaAgrupada, _unitOfWork);
            }

            return cargaAgrupada;
        }

        public void AjustarCargasAgrupadasAtualizadas()
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasComDivergencia(10);
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                carga.SituacaoCarga = carga.CargaAgrupamento.SituacaoCarga;
                carga.MotivoSolicitacaoFrete = carga.CargaAgrupamento.MotivoSolicitacaoFrete;
                carga.TipoFreteEscolhido = carga.CargaAgrupamento.TipoFreteEscolhido;
                carga.TipoContratacaoCarga = carga.CargaAgrupamento.TipoContratacaoCarga;
                carga.TabelaFreteFilialEmissora = carga.CargaAgrupamento.TabelaFreteFilialEmissora;
                carga.ContratoFreteTransportador = carga.CargaAgrupamento.ContratoFreteTransportador;
                carga.DataFinalizacaoEmissao = carga.CargaAgrupamento.DataFinalizacaoEmissao;
                carga.TabelaFrete = carga.CargaAgrupamento.TabelaFrete;
                carga.Operador = carga.CargaAgrupamento.Operador;
                carga.SituacaoAlteracaoFreteCarga = carga.CargaAgrupamento.SituacaoAlteracaoFreteCarga;
                carga.ModeloVeicularCarga = carga.CargaAgrupamento.ModeloVeicularCarga;
                carga.DataInicioEmissaoDocumentos = carga.CargaAgrupamento.DataInicioEmissaoDocumentos;
                carga.DataFinalizacaoEmissao = carga.CargaAgrupamento.DataFinalizacaoEmissao;
                carga.CalculandoFrete = carga.CargaAgrupamento.CalculandoFrete;
                repCarga.Atualizar(carga);

                if (carga.DadosSumarizados != null && carga.CargaAgrupamento.DadosSumarizados != null && carga.DadosSumarizados.Distancia != carga.CargaAgrupamento.DadosSumarizados.Distancia)
                {
                    carga.DadosSumarizados.Distancia = carga.CargaAgrupamento.DadosSumarizados.Distancia;
                    repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                }
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            if (cargaAtual.CargaAgrupamento == null)
                return;

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            cargaNova.CargaAgrupamento = cargaAtual.CargaAgrupamento;
            cargaNova.CargaFechada = false;

            if (cargaAtual.CargaAgrupamento.Empresa != null)//todo valida se durante o agrupamento da pré carga não ouve alguma alteração no transportador, se teve e ele não é da mesma raiz faz a troca.
            {
                string raizCNPJEmpresaAgrup = Utilidades.String.OnlyNumbers(cargaAtual.CargaAgrupamento.Empresa.CNPJ).Remove(8, 6);
                string raizEmpresa = cargaNova.Empresa != null ? Utilidades.String.OnlyNumbers(cargaNova.Empresa.CNPJ).Remove(8, 6) : "";
                if (raizCNPJEmpresaAgrup != raizEmpresa)
                    cargaNova.Empresa = cargaAtual.CargaAgrupamento.Empresa;
            }

            _unitOfWork.Flush();

            cargaAtual.CargaAgrupamento = null;
            repCanhoto.SetarTransportadorCanhotosPorCargaAgrupadaOuCargaNova(cargaNova.CargaAgrupamento.Codigo, cargaNova.CargaAgrupamento.Empresa?.Codigo ?? 0, cargaNova.Codigo);
            repCargaPedido.SetarCargaOrigem(cargaAtual.Codigo, cargaAtual.Codigo);
            repCargaPedido.SetarCargaOrigem(cargaNova.CargaAgrupamento.Codigo, cargaNova.Codigo);
            repCarga.Atualizar(cargaNova);
            VerificarSePossuiTodasCargas(cargaNova.CargaAgrupamento, tipoServicoMultisoftware, ClienteMultisoftware);
            serHubCarga.InformarCargaAtualizada(cargaNova.CargaAgrupamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
        }

        #endregion Métodos Públicos
    }
}

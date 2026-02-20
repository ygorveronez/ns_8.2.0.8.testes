using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    public class BaseControleEntregaController : BaseController
    {
        #region Construtores

        public BaseControleEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        protected Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigocargaEmbarcador"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                CodigoResponsavelEntrega = Request.GetIntParam("ResponsavelEntrega"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculos"),
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                CodigosSupervisor = Request.GetListParam<int>("Supervisor"),
                CodigosGerente = Request.GetListParam<int>("Gerente"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CpfCnpjRemetentes = Request.GetListParam<double>("Remetente"),
                CpfCnpjEmitentes = Request.GetListParam<double>("Emitente"),
                CpfCnpjExpedidores = Request.GetListParam<double>("Expedidor"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataLimite = Request.GetDateTimeParam("DataFinal"),
                ExibirEntregaAntesEtapaTransporte = ConfiguracaoEmbarcador.ExibirEntregaAntesEtapaTransporte,
                ExibirSomenteCargasComVeiculo = Request.GetBoolParam("ExibirSomenteCargasComVeiculo"),
                NumeroPedido = Request.GetIntParam("Pedido"),
                NumeroPedidosEmbarcador = ObterParametroListaString(Request.GetStringParam("Pedido")),
                Placa = Request.GetStringParam("Placa"),
                StatusViagem = Request.GetNullableEnumParam<StatusViagemControleEntrega>("StatusViagemControleEntrega"),
                NumeroNotasFiscais = Request.GetListParam<int>("NumeroNotaFiscal"),
                ExibirSomenteCargasComChamadoAberto = Request.GetBoolParam("ExibirSomenteCargasComChamadoAberto"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                ExibirSomenteCargasComChatNaoLido = Request.GetBoolParam("ExibirSomenteCargasComChatNaoLido"),
                ExibirSomenteCargasComReentrega = Request.GetBoolParam("ExibirSomenteCargasComReentrega"),
                ExibirSomenteCargasComMotoristaAppDesatualizado = Request.GetBoolParam("ExibirSomenteCargasComMotoristaAppDesatualizado"),
                SomenteCargaComEstadiaConfiguraca = Request.GetBoolParam("SomenteCargaComEstadiaConfiguraca"),
                DataEntregaPedidoInicial = Request.GetDateTimeParam("DataEntregaPedidoInicial"),
                DataEntregaPedidoFinal = Request.GetDateTimeParam("DataEntregaPedidoFinal"),
                DataPrevisaoEntregaPedidoInicial = Request.GetDateTimeParam("DataPrevisaoEntregaPedidoInicial"),
                DataPrevisaoEntregaPedidoFinal = Request.GetDateTimeParam("DataPrevisaoEntregaPedidoFinal"),
                SerieNota = Request.GetStringParam("SerieNota"),
                DataEmissaoNotaDe = Request.GetDateTimeParam("DataEmissaoNotaDe"),
                DataEmissaoNotaAte = Request.GetDateTimeParam("DataEmissaoNotaAte"),
                NumeroCTeDe = Request.GetIntParam("NumeroCTeDe"),
                NumeroCTeAte = Request.GetIntParam("NumeroCTeAte"),
                SerieCTe = Request.GetIntParam("SerieCTe"),
                DataEmissaoCTeDe = Request.GetDateTimeParam("DataEmissaoCTeDe"),
                DataEmissaoCTeAte = Request.GetDateTimeParam("DataEmissaoCTeAte"),
                DataCarregamentoCargaInicial = Request.GetDateTimeParam("DataCarregamentoCargaInicial"),
                DataCarregamentoCargaFinal = Request.GetDateTimeParam("DataCarregamentoCargaFinal"),
                DataInicioAbate = Request.GetDateTimeParam("DataInicioAbate"),
                DataFimAbate = Request.GetDateTimeParam("DataFimAbate"),
                EmpresaDestino = Request.GetIntParam("EmpresaDestino"),
                CidadeOrigem = Request.GetIntParam("CidadeOrigem"),
                CidadeDestino = Request.GetIntParam("CidadeDestino"),
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                NumeroSolicitacao = Request.GetStringParam("NumeroSolicitacao"),
                NumeroPedidoCF = Request.GetStringParam("NumeroPedidoCF"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigoCargaEmbarcadorMulti = Request.GetListParam<string>("CodigoCargaEmbarcadorMulti"),
                Recebedor = Request.GetListParam<double>("Recebedor"),
                CargasComEstadiasGeradas = Request.GetNullableBoolParam("CargasComEstadiasGeradas"),
                ExibirSomenteCargasComRecebedor = Request.GetNullableBoolParam("PossuiRecebedor"),
                ExibirSomenteCargasComExpedidor = Request.GetNullableBoolParam("PossuiExpedidor"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                RetornarInformacoesMonitoramento = Request.GetBoolParam("RetornarInformacoesMonitoramento"),
                RetornarCargasQueMonitoro = Request.GetBoolParam("RetornarCargasQueMonitoro"),
                DataProgramadaDescargaInicial = Request.GetDateTimeParam("DataProgramadaDescargaInicial"),
                DataProgramadaDescargaFinal = Request.GetDateTimeParam("DataProgramadaDescargaFinal"),
                DataPrevisaoInicioViagemFinal = Request.GetDateTimeParam("DataPrevisaoInicioViagemFinal"),
                DataPrevisaoInicioViagemInicial = Request.GetDateTimeParam("DataPrevisaoInicioViagemInicial"),
                CanaisEntrega = Request.GetListParam<int>("CanalEntrega"),
                CanaisVenda = Request.GetListParam<int>("CanalVenda"),
                CodigoSap = Request.GetStringParam("CodigoSap"),
                StatusViagens = Request.GetListEnumParam<StatusViagemControleEntrega>("StatusViagem"),
                CentrosCarregamentos = Request.GetListParam<int>("CentroCarregamento"),
                CentrosResultados = Request.GetListParam<int>("CentroResultado"),
                CodigoResponsavelVeiculo = Request.GetIntParam("ResponsavelVeiculo"),
                DataAgendamentoInicial = Request.GetDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetDateTimeParam("DataAgendamentoFinal"),
                DataProgramadaColetaInicial = Request.GetDateTimeParam("DataProgramadaColetaInicial"),
                DataProgramadaColetaFinal = Request.GetDateTimeParam("DataProgramadaColetaFinal"),
                OrdernarResultadosPorDataCriacao = Request.GetBoolParam("OrdernarResultadosPorDataCriacao"),
                SomenteCargasCriticas = Request.GetBoolParam("SomenteCargasCriticas"),
                DataInicioViagemInicial = Request.GetDateTimeParam("DataInicioViagemInicial"),
                DataInicioViagemFinal = Request.GetDateTimeParam("DataInicioViagemFinal"),
                DataInicioEmissao = Request.GetDateTimeParam("DataInicioEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                CnpjTomador = Request.GetDoubleParam("Tomador"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CnpjRemetente = Request.GetDoubleParam("Remetente"),
                CodigosCarga = Request.GetListParam<int>("NumeroCarga"),
                TiposTrecho = Request.GetListParam<int>("TipoTrecho"),
                DataInicioCriacaoCarga = Request.GetDateTimeParam("DataInicioCriacaoCarga"),
                DataFinalCriacaoCarga = Request.GetDateTimeParam("DataFinalCriacaoCarga"),
                CodigosOrigem = Request.GetListParam<int>("Origens"),
                CodigosTiposOperacao = Request.GetListParam<int>("TiposOperacao"),
                Filial = Request.GetIntParam("Filial"),
                SituacoesCarga = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                CodigosStatusViagem = Request.GetListParam<int>("StatusDaViagem"),
                CodigosTipoOcorrencia = Request.GetListParam<int>("OcorrenciaEntrega"),
                TendenciaEntrega = Request.GetEnumParam<TendenciaEntrega>("TendenciaEntrega"),
                TendenciaColeta = Request.GetEnumParam<TendenciaEntrega>("TendenciaColeta"),
                ClienteComplementar = Request.GetListParam<double>("ClienteComplementar"),
                NumeroOrdemPedido = Request.GetStringParam("NumeroOrdemPedido"),
                VeiculosNoRaio = Request.GetBoolParam("VeiculosNoRaio"),
                ModalTransporte = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum),
                MonitoramentoStatus = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                TipoAlerta = Request.GetListParam<int>("TipoAlerta"),
                EquipeVendas = Request.GetStringParam("EquipeVendas"),
                TipoMercadoria = Request.GetStringParam("TipoMercadoria"),
                EscritorioVenda = Request.GetStringParam("EscritorioVenda"),
                RotaFrete = Request.GetStringParam("RotaFrete"),
                MesoRegiao = Request.GetListParam<int>("MesoRegiao"),
                Regiao = Request.GetListParam<int>("Regiao"),
                Matriz = Request.GetStringParam("Matriz"),
                Parqueada = Request.GetNullableBoolParam("Parqueada"),
                SomenteCargasComPesquisaRecebedorPendenteResposta = Request.GetBoolParam("SomenteCargasComPesquisaRecebedorPendenteResposta"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNota"),
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            List<int> codigosTipoCarga = Request.GetListParam<int>("TipoCarga");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && this.Usuario != null && this.Usuario.Cliente != null && this.Usuario.Cliente.CompartilharAcessoEntreGrupoPessoas && this.Usuario.Cliente.GrupoPessoas != null)
                filtrosPesquisa.GrupoPessoa = this.Usuario.Cliente.GrupoPessoas.Codigo;

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.Recebedor = filtrosPesquisa.Recebedor.Count == 0 ? ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork) : filtrosPesquisa.Recebedor;
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : codigosFilialVenda;
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;
            filtrosPesquisa.CodigosTipoCarga = codigosTipoCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoCarga;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                filtrosPesquisa.NumeroPedidosEmbarcador = new List<string>();
            else
                filtrosPesquisa.NumeroPedido = 0;

            if (new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(TipoIntegracao.Unilever))
                filtrosPesquisa.ExibirSomenteCargasSubTrecho = true;

            return filtrosPesquisa;
        }

        private List<int> ObterParametroListaInt(string parametro)
        {
            List<string> itens = ObterParametroListaString(parametro);

            return itens.Select(i => i.ToInt()).Where(i => i > 0).ToList();
        }

        private List<string> ObterParametroListaString(string parametro)
        {
            List<string> itens = parametro.Split(',').ToList();

            return itens.Select(i => i.Trim()).Where(i => i.Length > 0).ToList();
        }
    }
}

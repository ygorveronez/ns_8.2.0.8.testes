var _tabelaValores = {};
var _indiceAtual;

function loadValores() {
    $("#btnTagCNPJTomador").click(function () {
        InserirTag("txtObservacao", "#CNPJTomador");
    });

    $("#btnTagNomeTomador").click(function () {
        InserirTag("txtObservacao", "#NomeTomador");
    });

    $("#btnTagCNPJRemetente").click(function () {
        InserirTag("txtObservacao", "#CNPJRemetente");
    });

    $("#btnTagNomeRemetente").click(function () {
        InserirTag("txtObservacao", "#NomeRemetente");
    });

    $("#btnTagCNPJDestinatario").click(function () {
        InserirTag("txtObservacao", "#CNPJDestinatario");
    });

    $("#btnTagNomeDestinatario").click(function () {
        InserirTag("txtObservacao", "#NomeDestinatario");
    });

    $("#btnTagNumeroCTe").click(function () {
        InserirTag("txtObservacao", "#NumeroCTe");
    });

    $("#btnTagSerieCTe").click(function () {
        InserirTag("txtObservacao", "#SerieCTe");
    });

    $("#btnTagValorTotalPrestacao").click(function () {
        InserirTag("txtObservacao", "#ValorTotalPrestacao");
    });

    $("#btnTagNumeroPedido").click(function () {
        InserirTag("txtObservacao", "#NumeroPedido");
    });

    $("#btnTagRota").click(function () {
        InserirTag("txtObservacao", "#Rota");
    });

    $("#btnTagRotaPedido").click(function () {
        InserirTag("txtObservacao", "#RotaPedido");
    });

    $("#btnTagRotaPedidoComValor").click(function () {
        InserirTag("txtObservacao", "#RotaPedidoComValor");
    });

    $("#btnTagPlacas").click(function () {
        InserirTag("txtObservacao", "#Placas");
    });

    $("#btnTagCPFMotorista").click(function () {
        InserirTag("txtObservacao", "#CPFMotorista");
    });

    $("#btnTagNomeMotorista").click(function () {
        InserirTag("txtObservacao", "#NomeMotorista");
    });

    $("#btnTagCodigoTabelaFrete").click(function () {
        InserirTag("txtObservacao", "#CodigoTabelaFrete");
    });

    $("#btnTagCPFOperador").click(function () {
        InserirTag("txtObservacao", "#CPFOperador");
    });

    $("#btnTagNomeOperador").click(function () {
        InserirTag("txtObservacao", "#NomeOperador");
    });

    $("#btnTagModeloVeicularCarga").click(function () {
        InserirTag("txtObservacao", "#ModeloVeicularCarga");
    });

    $("#btnTagSeguradora").click(function () {
        InserirTag("txtObservacao", "#Seguradora");
    });

    $("#btnTagApoliceSeguro").click(function () {
        InserirTag("txtObservacao", "#ApoliceSeguro");
    });

    $("#btnTagCNPJEmitente").click(function () {
        InserirTag("txtObservacao", "#CNPJEmitente");
    });

    $("#btnTagNomeEmitente").click(function () {
        InserirTag("txtObservacao", "#NomeEmitente");
    });

    $("#btnTagMarcaVeiculo").click(function () {
        InserirTag("txtObservacao", "#MarcaVeiculo");
    });

    $("#btnTagChaveDocumentoTransporteAnterior").click(function () {
        InserirTag("txtObservacao", "#ChaveDocumentoTransporteAnterior");
    });

    $("#btnTagNumeroOCADocumentoTransporteAnterior").click(function () {
        InserirTag("txtObservacao", "#NumeroOCADocumentoTransporteAnterior");
    });

    $("#btnTagNumeroNotaFiscal").click(function () {
        InserirTag("txtObservacao", "#NumeroNotaFiscal");
    });

    $("#btnTagObservacaoNotaFiscal").click(function () {
        InserirTag("txtObservacao", "#ObservacaoNotaFiscal");
    });

    $("#btnTagNumeroContainer").click(function () {
        InserirTag("txtObservacao", "#NumeroContainer");
    });

    $("#btnTagOrdem").click(function () {
        InserirTag("txtObservacao", "#OrdemPedido");
    });

    $("#btnTagTipoOperacao").click(function () {
        InserirTag("txtObservacao", "#TipoOperacao");
    });

    $("#btnTagDataAgendamento").click(function () {
        InserirTag("txtObservacao", "#DataAgendamento");
    });

    $("#btnTagFaixaTemperatura").click(function () {
        InserirTag("txtObservacao", "#FaixaTemperatura");
    });

    $("#btnTagNumeroCarga").click(function () {
        InserirTag("txtObservacao", "#NumeroCarga");
    });

    $("#btnTagCPFCNPJProprietarioVeiculo").click(function () {
        InserirTag("txtObservacaoTerceiro", "#CPFCNPJProprietarioVeiculo");
    });

    $("#btnTagIEProprietarioVeiculo").click(function () {
        InserirTag("txtObservacaoTerceiro", "#IEProprietarioVeiculo");
    });

    $("#btnTagNomeProprietarioVeiculo").click(function () {
        InserirTag("txtObservacaoTerceiro", "#NomeProprietarioVeiculo");
    });

    $("#btnTagEnderecoProprietarioVeiculo").click(function () {
        InserirTag("txtObservacaoTerceiro", "#EnderecoProprietarioVeiculo");
    });

    $("#btnTagRNTRCProprietarioVeiculo").click(function () {
        InserirTag("txtObservacaoTerceiro", "#RNTRCProprietarioVeiculo");
    });

    $("#btnTagPlacaVeiculoTerceiro").click(function () {
        InserirTag("txtObservacaoTerceiro", "#Placa");
    });

    $("#btnTagRENAVAMVeiculoTerceiro").click(function () {
        InserirTag("txtObservacaoTerceiro", "#RENAVAM");
    });

    $("#btnTagMarcaVeiculoTerceiro").click(function () {
        InserirTag("txtObservacaoTerceiro", "#MarcaVeiculo");
    });

    $("#btnSalvarObservacaoTabelaValores").click(function () {
        salvarObservacaoTabelaValores();
    });

    $("#btnSalvarObservacaoTabelaValores").prop("disabled", _FormularioSomenteLeitura);
    $("#txtObservacao").prop("disabled", _FormularioSomenteLeitura);
    $("#txtObservacaoTerceiro").prop("disabled", _FormularioSomenteLeitura);
    $("#chkImprimirObservacaoCTe").prop("disabled", _FormularioSomenteLeitura);

    $("#btnTagCNPJTomador").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeTomador").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCNPJRemetente").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeRemetente").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCNPJDestinatario").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeDestinatario").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNumeroCTe").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagSerieCTe").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagValorTotalPrestacao").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNumeroPedido").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagRota").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagRotaPedido").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagRotaPedidoComValor").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagPlacas").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCPFMotorista").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeMotorista").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCodigoTabelaFrete").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCPFOperador").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeOperador").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagModeloVeicularCarga").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagSeguradora").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagApoliceSeguro").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagCPFCNPJProprietarioVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagIEProprietarioVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNomeProprietarioVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagEnderecoProprietarioVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagRNTRCProprietarioVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagPlacaVeiculoTerceiro").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagRENAVAMVeiculoTerceiro").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagMarcaVeiculoTerceiro").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagMarcaVeiculo").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagFaixaTemperatura").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNumeroCarga").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagDataAgendamento").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagTipoOperacao").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagOrdem").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagObservacaoNotaFiscal").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNumeroNotaFiscal").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagNumeroOCADocumentoTransporteAnterior").prop("disabled", _FormularioSomenteLeitura);
    $("#btnTagChaveDocumentoTransporteAnterior").prop("disabled", _FormularioSomenteLeitura);

    $("#btnSalvarObservacaoTabelaValores").prop("disabled", _FormularioSomenteLeitura);
}

function obterValoresTabelaValores(possuiIntegracaoLBC) {
    var valorArray = new Array();
    let valores = new Array();
    let observacoes = new Array();
    let valoresMinimosGarantidos = new Array();
    let valoresMaximos = new Array();
    let valoresBases = new Array();
    let valoresExcedentes = new Array();
    let percentuaisPagamentoAgregados = new Array();
    let existeAlgumValorPreenchido = false;

    if (_tabelaValores.Body != null) {
        for (var i = 0; i < _tabelaValores.Body.length; i++) {
            for (var j = 0; j < _tabelaValores.Body[i].length; j++) {
                let itemTabelaValores = _tabelaValores.Body[i][j];

                if (itemTabelaValores.Tipo == "objeto") {
                    var valorDoFrete = $("#txtValores_" + j.toString() + "_" + i.toString()).val();
                    valores.push({
                        Tipo: itemTabelaValores.Data.Tipo,
                        TipoValor: itemTabelaValores.TipoValor,
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        Codigo: getProperty(itemTabelaValores.Data.Objeto, itemTabelaValores.PropCodigo),
                        Valor: valorDoFrete
                    });
                    valorArray.push(valorDoFrete);
                }
                else if (itemTabelaValores.Tipo == "observacao") {
                    observacoes.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        Observacao: itemTabelaValores.Data.Observacao,
                        ObservacaoTerceiro: itemTabelaValores.Data.ObservacaoTerceiro,
                        ImprimirObservacaoCTe: itemTabelaValores.Data.ImprimirObservacaoCTe
                    });
                }
                else if (itemTabelaValores.Tipo == "valorminimogarantido") {
                    var valorDoFrete = $("#txtValorMinimoGarantido_" + j.toString() + "_" + i.toString()).val();
                    valoresMinimosGarantidos.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        ValorMinimoGarantido: valorDoFrete
                    });
                }
                else if (itemTabelaValores.Tipo == "valormaximo") {
                    var valorDoFrete = $("#txtValorMaximo_" + j.toString() + "_" + i.toString()).val();
                    valoresMaximos.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        ValorMaximo: valorDoFrete
                    });
                }
                else if (itemTabelaValores.Tipo == "valorbase") {
                    var valorDoFrete = $("#txtValorBase_" + j.toString() + "_" + i.toString()).val();
                    valoresBases.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        ValorBase: valorDoFrete
                    });
                    valorArray.push(valorDoFrete);
                }
                else if (itemTabelaValores.Tipo == "PagamentoAgregado") {
                    var valorDoFrete = $("#txtValores_" + j.toString() + "_" + i.toString()).val();
                    percentuaisPagamentoAgregados.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        Tipo: itemTabelaValores.Tipo,
                        Valor: valorDoFrete
                    });
                }
                else if (itemTabelaValores.Tipo.lastIndexOf("Excedente") > 0) {
                    var valorDoFrete = $("#txtValores_" + j.toString() + "_" + i.toString()).val();
                    valoresExcedentes.push({
                        CodigoItemBase: itemTabelaValores.Data.CodigoItem,
                        Tipo: itemTabelaValores.Tipo,
                        Valor: valorDoFrete
                    });
                }
            }
        }

    }

    if (possuiIntegracaoLBC) {
        valorArray.forEach((item) => {
            if (item.Valor != "" || item.Valor > 0) {
                existeAlgumValorPreenchido = true;
                return;
            }
        });
    } else {
        existeAlgumValorPreenchido = true;
    }

    _tabelaFreteCliente.Valores.val(JSON.stringify(valores));
    _tabelaFreteCliente.Observacoes.val(JSON.stringify(observacoes));
    _tabelaFreteCliente.ValoresMinimosGarantidos.val(JSON.stringify(valoresMinimosGarantidos));
    _tabelaFreteCliente.ValoresMaximos.val(JSON.stringify(valoresMaximos));
    _tabelaFreteCliente.ValoresBases.val(JSON.stringify(valoresBases));

    _tabelaFreteCliente.ValoresExcedentes.val(JSON.stringify(valoresExcedentes));
    _tabelaFreteCliente.PercentuaisPagamentoAgregados.val(JSON.stringify(percentuaisPagamentoAgregados));

    return existeAlgumValorPreenchido;
}

function montarTabelaValores() {
    if (_tabelaFrete == null) {
        $("#divValoresTabelaFrete").hide();
        return;
    }

    _tabelaValores.Header = new Array();
    _tabelaValores.Body = new Array();
    _indiceAtual = 0;

    switch (_tabelaFrete.ParametroBase) {
        case EnumTipoParametroBaseTabelaFrete.ComponenteFrete:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Componente, cssClass: "" });
            setarColunasTabelaValores("ComponentesFrete", "Componente.Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.ComponenteFrete);
            break;
        case EnumTipoParametroBaseTabelaFrete.Distancia:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Distancia, cssClass: "" });
            setarColunasTabelaValores("Distancias", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Distancia);
            break;
        case EnumTipoParametroBaseTabelaFrete.ModeloReboque:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Reboque, cssClass: "" });
            setarColunasTabelaValores("ModelosReboque", "Modelo.Descricao", "Modelo.Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.ModeloReboque);
            break;
        case EnumTipoParametroBaseTabelaFrete.ModeloTracao:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Tracao, cssClass: "" });
            setarColunasTabelaValores("ModelosTracao", "Modelo.Descricao", "Modelo.Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.ModeloTracao);
            break;
        case EnumTipoParametroBaseTabelaFrete.Pallets:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Pallets, cssClass: "" });
            setarColunasTabelaValores("Pallets", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Pallets);
            break;
        case EnumTipoParametroBaseTabelaFrete.NumeroEntrega:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Entrega, cssClass: "" });
            setarColunasTabelaValores("NumeroEntregas", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.NumeroEntrega);
            break;
        case EnumTipoParametroBaseTabelaFrete.Pacote:
            _tabelaValores.Header.push({ text: "Pacote", cssClass: "" });
            setarColunasTabelaValores("Pacotes", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Pacote);
            break;
        case EnumTipoParametroBaseTabelaFrete.Peso:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Peso, cssClass: "" });
            setarColunasTabelaValores("PesosTransportados", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Peso);
            break;
        case EnumTipoParametroBaseTabelaFrete.TipoCarga:
            if (!_tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga) {
                _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Carga, cssClass: "" });
                setarColunasTabelaValores("TiposCargas", "Tipo.Descricao", "Tipo.Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.TipoCarga);
            }
            break;
        case EnumTipoParametroBaseTabelaFrete.TipoEmbalagem:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.TipoEmbalagem, cssClass: "" });
            setarColunasTabelaValores("TipoEmbalagens", "TipoEmbalagem.Descricao", "TipoEmbalagem.Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.TipoEmbalagem);
            break;
        case EnumTipoParametroBaseTabelaFrete.Rota:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Rota, cssClass: "" });
            setarColunasTabelaValores("RotasFreteEmbarcador", "RotaFrete.Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Rota);
            break;
        case EnumTipoParametroBaseTabelaFrete.Tempo:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Tempo, cssClass: "" });
            setarColunasTabelaValores("Tempos", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Tempo);
            break;
        case EnumTipoParametroBaseTabelaFrete.Ajudante:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Ajudante, cssClass: "" });
            setarColunasTabelaValores("Ajudantes", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Ajudante);
            break;
        case EnumTipoParametroBaseTabelaFrete.Hora:
            _tabelaValores.Header.push({ text: Localization.Resources.Fretes.TabelaFreteCliente.Hora, cssClass: "" });
            setarColunasTabelaValores("Horas", "Descricao", "Codigo", 0, true, EnumTipoParametroBaseTabelaFrete.Hora);
            break;
        default:
            if (_tabelaFrete.PossuiValorBase)
                setarColunaValorBaseTabelaValores(0);

            if (!_tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga)
                setarColunasTabelaValores("TiposCargas", "Tipo.Descricao", "Tipo.Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.TipoCarga);

            setarColunasTabelaValores("ModelosTracao", "Modelo.Descricao", "Modelo.Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.ModeloTracao);
            //setarColunasTabelaValores("TipoEmbalagens", "TipoEmbalagem.Descricao", "TipoEmbalagem.Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.TipoEmbalagem);
            setarColunasTabelaValores("ModelosReboque", "Modelo.Descricao", "Modelo.Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.ModeloReboque);
            setarColunasTabelaValores("NumeroEntregas", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.NumeroEntrega);
            setarColunasTabelaValores("Pacotes", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Pacote);

            if (_tabelaFrete.PermiteValorAdicionalEntregaExcedente === true && _tabelaFrete.NumeroEntregas.length > 0)
                setarColunaExcedenteTabelaValores(0, "Entrega", Localization.Resources.Fretes.TabelaFreteCliente.PorEntregaExcedente);

            if (_tabelaFrete.PermiteValorAdicionalPacoteExcedente === true && _tabelaFrete.Pacotes.length > 0)
                setarColunaExcedenteTabelaValores(0, "Pacote", Localization.Resources.Fretes.TabelaFreteCliente.PorPacoteExcedente);

            setarColunasTabelaValores("Pallets", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Pallets);

            if (_tabelaFrete.PermiteValorAdicionalPalletExcedente === true && _tabelaFrete.Pallets.length > 0)
                setarColunaExcedenteTabelaValores(0, "Pallet", Localization.Resources.Fretes.TabelaFreteCliente.PorPalletExcedente);

            setarColunasTabelaValores("PesosTransportados", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Peso);

            if (_tabelaFrete.PermiteValorAdicionalPesoExcedente === true && Globalize.parseFloat(_tabelaFrete.PesoExcedente) > 0 && _tabelaFrete.PesosTransportados.length > 0)
                setarColunaExcedenteTabelaValores(0, "Peso", Localization.Resources.Fretes.TabelaFreteCliente.ACadaExcedente.format(_tabelaFrete.PesoExcedente, _tabelaFrete.PesosTransportados[0].UnidadeMedida.Sigla));

            setarColunasTabelaValores("Distancias", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Distancia);

            if (_tabelaFrete.PermiteValorAdicionalQuilometragemExcedente === true && Globalize.parseFloat(_tabelaFrete.QuilometragemExcedente) > 0 && _tabelaFrete.Distancias.length > 0)
                setarColunaExcedenteTabelaValores(0, "Quilometragem", Localization.Resources.Fretes.TabelaFreteCliente.ACadaKmExcedente.format(_tabelaFrete.QuilometragemExcedente));

            setarColunasTabelaValores("Tempos", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Tempo);
            setarColunasTabelaValores("Ajudantes", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Ajudante);
            setarColunasTabelaValores("Horas", "Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Hora);

            if (_tabelaFrete.PermiteValorAdicionalHoraExcedente === true && _tabelaFrete.Horas.length > 0)
                setarColunaExcedenteTabelaValores(0, "Horas", Localization.Resources.Fretes.TabelaFreteCliente.PorHoraExcedente);

            setarColunasTabelaValores("ComponentesFrete", "Componente.Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.ComponenteFrete);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                setarColunasTabelaValores("RotasFreteEmbarcador", "RotaFrete.Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Rota, null, exibirRota);

            if (_tabelaFrete.PossuiMinimoGarantido)
                setarColunaValorMinimoGarantidoTabelaValores(0);

            if (_tabelaFrete.PossuiValorMaximo)
                setarColunaValorMaximoTabelaValores(0);

            setarColunaPercentualPagamentoAgregadoTabelaValores(0);
            setarColunaObservacaoTabelaValores(0);

            break;
    };

    renderizarTabelaValores();
}

function renderizarTabelaValores() {
    let tabela = document.getElementById("tblValores");

    if (_tabelaValores.Header.length <= 0 || _tabelaValores.Body.length <= 0) {
        tabela.innerHTML = "<tr><td><b> " + Localization.Resources.Fretes.TabelaFreteCliente.NenhumParametroCadastradoParaAConfiguracaoDosValores + "</b></td></tr>";
        return;
    }

    tabela.style.width = (_tabelaValores.Header.length * 150).toString() + "px";

    tabela.innerHTML = "";

    let header = tabela.createTHead();
    let headerRow = document.createElement("tr");
    header.appendChild(headerRow);

    let body = document.createElement("tbody");
    tabela.appendChild(body);

    for (let i = 0; i < _tabelaValores.Header.length; i++) {
        let itemHeader = _tabelaValores.Header[i];

        let title = document.createElement("th");

        headerRow.appendChild(title);

        title.className = itemHeader.cssClass;
        title.innerHTML = itemHeader.text;
        title.title = itemHeader.text;
    }

    for (let i = 0; i < _tabelaValores.Body.length; i++) {
        for (let j = 0; j < _tabelaValores.Body[i].length; j++) {
            let itemTabelaValores = _tabelaValores.Body[i][j];

            itemTabelaValores.X = j;
            itemTabelaValores.Y = i;

            let row = document.getElementById("rowValores_" + j.toString());

            if (row == null) {
                row = document.createElement("tr");
                row.id = "rowValores_" + j.toString();
                body.appendChild(row);
            }

            let col = document.createElement("td");

            row.appendChild(col);

            col.className = _tabelaValores.Header[i].cssClass; //seta a classe, por hora utilizado para esconder as colunas somente
            col.id = "colValores_" + j.toString() + "_" + i.toString();

            let precisao = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 4;

            if (itemTabelaValores.Tipo == "string") {
                col.innerHTML = itemTabelaValores.Texto;
                col.title = itemTabelaValores.Texto;
                col.style.textOverflow = "ellipsis";
                col.style.whiteSpace = "normal";
                col.style.overflow = "hidden";
            }
            else if (itemTabelaValores.Tipo == "valorminimogarantido") {

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValorMinimoGarantido_" + j.toString() + "_" + i.toString();
                inputValor.disabled = _FormularioSomenteLeitura ? "disabled" : "";

                col.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.ValoresMinimosGarantidos.val(), function (indice, valorMinimoExiste) {
                    if (valorMinimoExiste.CodigoItem == itemTabelaValores.Data.CodigoItem) {
                        inputValor.value = valorMinimoExiste.ValorMinimoGarantido > 0 ? Globalize.format(valorMinimoExiste.ValorMinimoGarantido, "n" + precisao) : "";
                        return false;
                    }
                });
            }
            else if (itemTabelaValores.Tipo == "valormaximo") {

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValorMaximo_" + j.toString() + "_" + i.toString();
                inputValor.disabled = _FormularioSomenteLeitura ? "disabled" : "";

                col.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.ValoresMaximos.val(), function (indice, valorMaximoExiste) {
                    if (valorMaximoExiste.CodigoItem == itemTabelaValores.Data.CodigoItem) {
                        inputValor.value = valorMaximoExiste.ValorMaximo > 0 ? Globalize.format(valorMaximoExiste.ValorMaximo, ("n" + precisao)) : "";
                        return false;
                    }
                });
            }
            else if (itemTabelaValores.Tipo == "valorbase") {

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValorBase_" + j.toString() + "_" + i.toString();
                inputValor.disabled = _FormularioSomenteLeitura ? "disabled" : "";

                col.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.ValoresBases.val(), function (indice, valorBaseExiste) {
                    if (valorBaseExiste.CodigoItem == itemTabelaValores.Data.CodigoItem) {
                        inputValor.value = valorBaseExiste.ValorBase > 0 ? Globalize.format(valorBaseExiste.ValorBase, ("n" + precisao)) : "";
                        return false;
                    }
                });
            }
            else if (itemTabelaValores.Tipo == "observacao") {
                let divGrid = document.createElement("div");
                divGrid.className = "d-grid";

                let button = document.createElement("button");
                button.id = "btnObservacaoValores_" + j.toString() + "_" + i.toString();
                button.type = "button";
                button.className = "btn btn-default waves-effect waves-themed";
                button.onclick = function () { abrirTelaObservacaoTabelaValores(this); };
                button.innerHTML = Localization.Resources.Fretes.TabelaFreteCliente.Observacao;

                divGrid.appendChild(button);

                col.appendChild(divGrid);

                $.each(_tabelaFreteCliente.Observacoes.val(), function (indice, observacaoExistente) {
                    if (observacaoExistente.CodigoItem == itemTabelaValores.Data.CodigoItem) {
                        itemTabelaValores.Data.Observacao = observacaoExistente.Observacao;
                        itemTabelaValores.Data.ObservacaoTerceiro = observacaoExistente.ObservacaoTerceiro;
                        itemTabelaValores.Data.ImprimirObservacaoCTe = observacaoExistente.ImprimirObservacaoCTe;
                        return false;
                    }
                });
            }
            else if (itemTabelaValores.Tipo == "PagamentoAgregado") {
                precisao = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 2;

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValores_" + j.toString() + "_" + i.toString();
                inputValor.disabled = _FormularioSomenteLeitura ? "disabled" : "";
                inputValor.maxLength = 5;

                col.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.PercentuaisPagamentoAgregados.val(), function (indice, percentualPagamentoAgregado) {
                    if (percentualPagamentoAgregado.CodigoItem == itemTabelaValores.Data.CodigoItem) {
                        inputValor.value = percentualPagamentoAgregado.Valor > 0 ? Globalize.format(percentualPagamentoAgregado.Valor, "n" + precisao) : "";
                        return false;
                    }
                });
            }
            else if (itemTabelaValores.Tipo.lastIndexOf("Excedente") > 0) {
                precisao = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 6;

                let divInputGroup = document.createElement("div");
                divInputGroup.className = "input-group";

                col.appendChild(divInputGroup);

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValores_" + j.toString() + "_" + i.toString();
                inputValor.disabled = _FormularioSomenteLeitura ? "disabled" : "";

                let button = document.createElement("button");
                button.id = "btnValores_" + j.toString() + "_" + i.toString();
                button.type = "button";
                button.className = "btn btn-default dropdown-toggle waves-effect waves-themed";
                button.dataset.bsToggle = "dropdown";
                button.tabIndex = -1;
                button.disabled = _FormularioSomenteLeitura ? "disabled" : "";

                let dropDown = document.createElement("ul");
                dropDown.className = "dropdown-menu";

                let liAumentoValor = document.createElement("li");

                dropDown.appendChild(liAumentoValor);

                let aAumentoValor = document.createElement("a");
                aAumentoValor.href = "javascript:void(0);";
                aAumentoValor.className = "dropdown-item";
                aAumentoValor.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.AumentoValor); };
                aAumentoValor.innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp; ' + Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor + '';

                liAumentoValor.appendChild(aAumentoValor);

                button.innerHTML = '<span class="fal fa-plus"></span>';
                button.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;

                inputValor.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;
                inputValor.maxLength = "13";

                divInputGroup.appendChild(button);
                divInputGroup.appendChild(dropDown);
                divInputGroup.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.ValoresExcedentes.val(), function (indice, valorExcedenteExistente) {
                    if (valorExcedenteExistente.CodigoItem == itemTabelaValores.Data.CodigoItem && valorExcedenteExistente.Tipo == itemTabelaValores.Tipo) {
                        inputValor.value = valorExcedenteExistente.Valor > 0 ? Globalize.format(valorExcedenteExistente.Valor, ("n" + precisao)) : "";
                        return false;
                    }
                });

            }
            else if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.ComponenteFrete && itemTabelaValores.Data.Objeto.Tipo == EnumTipoComponenteTabelaFrete.ValorCalculado && itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Tempo) {
                let divGrid = document.createElement("div");
                divGrid.className = "d-grid";

                let button = document.createElement("button");
                button.id = "btnVisualizarValoresComponenteFreteTempo_" + j.toString() + "_" + i.toString();
                button.type = "button";
                button.className = "btn btn-default waves-effect waves-themed";
                button.onclick = function () { AbrirTelaValoresComponenteFreteTempoTabelaFrete(this); };
                button.innerHTML = Localization.Resources.Fretes.TabelaFreteCliente.Visualizar;
                button.dataset.ComponenteFrete = itemTabelaValores.Data.Objeto.Codigo;

                divGrid.append(button);

                col.appendChild(divGrid);
            }
            else {
                let divInputGroup = document.createElement("div");
                divInputGroup.className = "input-group";
                col.appendChild(divInputGroup);

                let campoDesabilitado = false;
                let botaoDesabilitado = false;
                let valorPadrao = "";

                if (!_FormularioSomenteLeitura && itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.ComponenteFrete && itemTabelaValores.Data.Objeto.Tipo == EnumTipoComponenteTabelaFrete.ValorCalculado) {
                    botaoDesabilitado = true;

                    if (itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Tempo)
                        campoDesabilitado = true;
                    else if (!itemTabelaValores.Data.Objeto.ValorInformadoNaTabela) {
                        campoDesabilitado = true;

                        if (itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Percentual)
                            valorPadrao = Globalize.format(itemTabelaValores.Data.Objeto.Percentual.val, "n3");
                        else
                            valorPadrao = Globalize.format(itemTabelaValores.Data.Objeto.ValorFormula.val, "n" + precisao);
                    }
                }

                let inputValor = document.createElement("input");
                inputValor.type = "text";
                inputValor.className = "form-control";
                inputValor.id = "txtValores_" + j.toString() + "_" + i.toString();
                inputValor.disabled = campoDesabilitado || _FormularioSomenteLeitura ? "disabled" : "";
                inputValor.value = valorPadrao;

                let button = document.createElement("button");
                button.id = "btnValores_" + j.toString() + "_" + i.toString();
                button.type = "button";
                button.className = "btn btn-default dropdown-toggle waves-effect waves-themed";
                button.dataset.bsToggle = "dropdown";
                button.tabIndex = -1;
                button.disabled = botaoDesabilitado || _FormularioSomenteLeitura ? "disabled" : "";

                let dropDown = document.createElement("ul");
                dropDown.className = "dropdown-menu";


                if (itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.ComponenteFrete && itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.Rota) {
                    let liValorFixo = document.createElement("li");
                    dropDown.appendChild(liValorFixo);

                    let aValorFixo = document.createElement("a");
                    aValorFixo.href = "javascript:void(0);";
                    aValorFixo.className = "dropdown-item";
                    aValorFixo.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.ValorFixo); };
                    aValorFixo.innerHTML = '<i class="fal fa-dollar-sign"></i>&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo + '';
                    liValorFixo.appendChild(aValorFixo);
                }


                if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Distancia || itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Peso || itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Hora)
                    precisao = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 6;

                if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Pallets) {
                    let liArredondadoParaCima = document.createElement("li");
                    dropDown.appendChild(liArredondadoParaCima);

                    let aliArredondadoParaCima = document.createElement("a");
                    aliArredondadoParaCima.href = "javascript:void(0);";
                    aliArredondadoParaCima.className = "dropdown-item";
                    aliArredondadoParaCima.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima); };
                    aliArredondadoParaCima.innerHTML = '<i class="fal fa-level-up"></i>&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.ValorFixoArredondado + '';
                    liArredondadoParaCima.appendChild(aliArredondadoParaCima);
                }

                if (itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.ComponenteFrete ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.PEDAGIO ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.DESCARGA ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.FRETE ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.OUTROS) {

                    let liAumentoValor = document.createElement("li");
                    dropDown.appendChild(liAumentoValor);

                    let aAumentoValor = document.createElement("a");
                    aAumentoValor.href = "javascript:void(0);";
                    aAumentoValor.className = "dropdown-item";
                    aAumentoValor.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.AumentoValor); };
                    aAumentoValor.innerHTML = '<i class="fal fa-plus"></i>&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor + '';
                    liAumentoValor.appendChild(aAumentoValor);
                }

                if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.ComponenteFrete &&
                    itemTabelaValores.Data.Objeto.Tipo == EnumTipoComponenteTabelaFrete.ValorCalculado &&
                    itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Percentual) {

                    let liAumentoPercentual = document.createElement("li");
                    dropDown.appendChild(liAumentoPercentual);

                    let aAumentoPercentual = document.createElement("a");
                    aAumentoPercentual.href = "javascript:void(0);";
                    aAumentoPercentual.className = "dropdown-item";
                    aAumentoPercentual.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.AumentoPercentual); };
                    aAumentoPercentual.innerHTML = '%&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.AumentoPercentual + ''
                    liAumentoPercentual.appendChild(aAumentoPercentual);
                }

                if (itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.ComponenteFrete ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.OUTROS ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.ADVALOREM ||
                    (itemTabelaValores.Data.Objeto.Tipo == EnumTipoComponenteTabelaFrete.ValorCalculado && itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Percentual)) {

                    let liPercentualNF = document.createElement("li");
                    dropDown.appendChild(liPercentualNF);

                    let aPercentualNF = document.createElement("a");
                    aPercentualNF.href = "javascript:void(0);";
                    aPercentualNF.className = "dropdown-item";
                    aPercentualNF.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal); };
                    aPercentualNF.innerHTML = '<i class="fal fa-barcode"></i>&nbsp;&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.PercentualSobreOValorDaNotaFiscal + '';
                    liPercentualNF.appendChild(aPercentualNF);
                }

                if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Peso ||
                    itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Distancia
                ) {

                    let liDesabilitar = document.createElement("li");
                    dropDown.appendChild(liDesabilitar);

                    let aDesabilitar = document.createElement("a");
                    aDesabilitar.href = "javascript:void(0);";
                    aDesabilitar.className = "dropdown-item";
                    aDesabilitar.onclick = function () { trocarTipoValorCampoTabelaValores(this, EnumTipoCampoValorTabelaFrete.Desabilitado); };
                    aDesabilitar.innerHTML = '<i class="fal fa-ban"></i>&nbsp;&nbsp;' + Localization.Resources.Fretes.TabelaFreteCliente.Desabilitar + '';

                    liDesabilitar.appendChild(aDesabilitar);
                }

                if (itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.ComponenteFrete && itemTabelaValores.Data.Tipo != EnumTipoParametroBaseTabelaFrete.Rota) {

                    inputValor.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;
                    inputValor.maxLength = "15";
                    button.innerHTML = '<span class="fal fa-dollar-sign"></span>';
                    button.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;

                }
                else if (itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.PEDAGIO ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.DESCARGA ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.FRETE ||
                    itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.OUTROS ||
                    itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.Rota) {

                    itemTabelaValores.TipoValor = EnumTipoCampoValorTabelaFrete.AumentoValor;

                    inputValor.maxLength = "10";
                    button.innerHTML = '<span class="fal fa-plus"></span>';

                    inputValor.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;
                    button.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;

                }
                else if (itemTabelaValores.Data.Objeto.Componente.Tipo == EnumTipoComponenteFrete.ADVALOREM) {

                    itemTabelaValores.TipoValor = EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                    inputValor.maxLength = "7";
                    button.innerHTML = '<span class="fal fa-barcode"></span>';

                    inputValor.title = Localization.Resources.Fretes.TabelaFreteCliente.PercentualSobreOValorDaNotaFiscal;
                    button.title = Localization.Resources.Fretes.TabelaFreteCliente.PercentualSobreOValorDaNotaFiscal;

                }
                else {
                    inputValor.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;
                    inputValor.maxLength = "15";
                    button.innerHTML = '<span class="fal fa-dollar-sign"></span>';
                    button.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;
                }

                divInputGroup.appendChild(button);
                divInputGroup.appendChild(dropDown);
                divInputGroup.appendChild(inputValor);

                $(inputValor).maskMoney(ConfigDecimal({ precision: precisao }));

                $.each(_tabelaFreteCliente.Valores.val(), function (indice, itemExistente) {
                    if (itemExistente.TipoObjeto == itemTabelaValores.Data.Tipo &&
                        itemExistente.Codigo == getProperty(itemTabelaValores.Data.Objeto, itemTabelaValores.PropCodigo) &&
                        itemExistente.CodigoItem == itemTabelaValores.Data.CodigoItem) {

                        if (itemTabelaValores.Data.Tipo == EnumTipoParametroBaseTabelaFrete.ComponenteFrete && itemTabelaValores.Data.Objeto.Tipo == EnumTipoComponenteTabelaFrete.ValorCalculado) {

                            let permiteInformarValor = itemTabelaValores.Data.Objeto.ValorInformadoNaTabela && !_FormularioSomenteLeitura;

                            if (itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Percentual) {
                                let tipoPercentual = EnumTipoCampoValorTabelaFrete.AumentoPercentual;

                                if (itemTabelaValores.Data.Objeto.TipoPercentual == EnumTipoPercentualComponenteTabelaFrete.SobreNotaFiscal)
                                    tipoPercentual = EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                                if (itemTabelaValores.Data.Objeto.ValorInformadoNaTabela === true)
                                    inputValor.value = itemExistente.Valor > 0 ? Globalize.format(itemExistente.Valor, ("n" + precisao)) : "";
                                else
                                    inputValor.value = itemTabelaValores.Data.Objeto.Percentual.val > 0 ? Globalize.format(itemTabelaValores.Data.Objeto.Percentual.val, ("n" + precisao)) : "";

                                trocarHtmlTipoValorCampoTabelaValores(button, inputValor, tipoPercentual, permiteInformarValor);
                                itemTabelaValores.TipoValor = tipoPercentual;
                            }
                            else if (itemTabelaValores.Data.Objeto.TipoCalculo == EnumTipoCalculoComponenteTabelaFrete.Tempo) {
                                inputValor.value = "";

                                trocarHtmlTipoValorCampoTabelaValores(button, inputValor, EnumTipoCampoValorTabelaFrete.AumentoValor, permiteInformarValor);
                                itemTabelaValores.TipoValor = EnumTipoCampoValorTabelaFrete.AumentoValor;
                            }
                            else {
                                if (itemTabelaValores.Data.Objeto.ValorInformadoNaTabela === true)
                                    inputValor.value = itemExistente.Valor > 0 ? Globalize.format(itemExistente.Valor, ("n" + precisao)) : "";
                                else
                                    inputValor.value = itemTabelaValores.Data.Objeto.ValorFormula.val > 0 ? Globalize.format(itemTabelaValores.Data.Objeto.ValorFormula.val, ("n" + precisao)) : "";

                                trocarHtmlTipoValorCampoTabelaValores(button, inputValor, EnumTipoCampoValorTabelaFrete.AumentoValor, permiteInformarValor);
                                itemTabelaValores.TipoValor = EnumTipoCampoValorTabelaFrete.AumentoValor;
                            }
                        } else {
                            inputValor.value = itemExistente.Valor > 0 ? Globalize.format(itemExistente.Valor, ("n" + precisao)) : "";
                            trocarHtmlTipoValorCampoTabelaValores(button, inputValor, itemExistente.TipoValor, !_FormularioSomenteLeitura);
                            itemTabelaValores.TipoValor = itemExistente.TipoValor;
                        }

                        return false;
                    }
                });
            }

            _tabelaValores.Body[i][j] = itemTabelaValores;

            $(col).data("data", itemTabelaValores);
        }
    }

    $("#divValoresTabelaFrete").fadeIn();
    preencherValoresObtidosBidding();
}

function preencherValoresObtidosBidding() {
    const parametroBase = parseInt(sessionStorage.getItem('parametroBase'));
    const tipoBidding = sessionStorage.getItem('tipoBiddingCli');
    const tipoOferta = parseInt(sessionStorage.getItem('tipoOferta'));
    const dadosOferta = sessionStorage.getItem('oferta');
    const ofertasSemAjudante = sessionStorage.getItem('ofertasSemAjudante');
    const ofertasComAjudantes = sessionStorage.getItem('ofertasComAjudantes');

    var precisao = _CONFIGURACAO_TMS.TabelaFretePrecisaoDinheiroDois ? 2 : 4;

    if (!parametroBase || !tipoOferta || !dadosOferta) return;

    var oferta = new Object();
    var dadoFormatado = dadosOferta.split("-");

    if (ofertasSemAjudante && ofertasComAjudantes && tipoOferta == EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        oferta.ModeloVeicular = dadoFormatado[0];

        var dadosSemAjudante = ofertasSemAjudante.split("-");
        var dadosComAjudante = ofertasComAjudantes.split("-");
        var linha = 0;

        for (let i = 0; i < _tabelaValores.Body.length; i++) {
            for (let j = 0; j < _tabelaValores.Body[i].length; j++) {
                let itemTabelaValores = _tabelaValores.Body[i][j];
                if (itemTabelaValores.Tipo == "string" && itemTabelaValores.Texto == oferta.ModeloVeicular) {
                    linha = itemTabelaValores.X;
                }
                if (linha == itemTabelaValores.X && itemTabelaValores.Tipo == "objeto" && itemTabelaValores.Data.Objeto.ComAjudante == false) {
                    $("#txtValores_" + linha + "_" + i).val(Globalize.format(parseFloat(dadosSemAjudante[itemTabelaValores.Data.Objeto.NumeroFinalEntrega - 1]), ("n" + precisao)));
                }
                if (linha == itemTabelaValores.X && itemTabelaValores.Tipo == "objeto" && itemTabelaValores.Data.Objeto.ComAjudante == true) {
                    $("#txtValores_" + linha + "_" + i).val(Globalize.format(parseFloat(dadosComAjudante[itemTabelaValores.Data.Objeto.NumeroFinalEntrega - 1]), ("n" + precisao)));
                }
            }
        }
    } else {
        switch (tipoOferta) {
            case EnumTipoLanceBidding.LancePorEquipamento:
                oferta.Valor = dadoFormatado[2];
                break;

            case EnumTipoLanceBidding.LanceFrotaFixaKmRodado:
                oferta.Valor = dadoFormatado[5];
                break;

            case EnumTipoLanceBidding.LancePorcentagemNota:
                oferta.Valor = dadoFormatado[3];
                break;

            case EnumTipoLanceBidding.LanceViagemAdicional:
                oferta.Valor = dadoFormatado[8];
                break;

            case EnumTipoLanceBidding.LancePorPeso:
                oferta.Valor = dadoFormatado[9];
                break;

            case EnumTipoLanceBidding.LancePorCapacidade:
                oferta.Valor = dadoFormatado[10];
                break;

            case EnumTipoLanceBidding.LancePorFreteViagem:
                oferta.Valor = dadoFormatado[7];
                break;

            case EnumTipoLanceBidding.LanceFrotaFixaFranquia:
                oferta.Valor = dadoFormatado[1];
                break;

            case EnumTipoLanceBidding.LancePorViagemEntregaAjudante:
                oferta.Valor = dadoFormatado[13];
                break;
        }

        oferta.ModeloVeicular = dadoFormatado[0];
        var itemEncontrado = false;

        for (let i = 0; i < _tabelaValores.Body.length; i++) {
            for (let j = 0; j < _tabelaValores.Body[i].length; j++) {
                let itemTabelaValores = _tabelaValores.Body[i][j];
                if (!itemEncontrado && itemTabelaValores.Tipo == "string" || itemTabelaValores.Tipo == "valorbase" && itemTabelaValores.Texto == oferta.ModeloVeicular) {
                    itemEncontrado = true;
                }
                if (itemEncontrado && itemTabelaValores.Tipo == "valorbase") {
                    $("#txtValorBase_" + j + "_" + i).val(oferta.Valor);
                    itemEncontrado = false;
                }
            }
        }
    }

    /*
    switch (parametroBase) {
        case EnumTipoParametroBaseTabelaFrete.ModeloTracao:
        case EnumTipoParametroBaseTabelaFrete.ModeloReboque:
        case EnumTipoParametroBaseTabelaFrete.Peso:
        case EnumTipoParametroBaseTabelaFrete.Capacidade:
            break;
        default:
            break;
    }
    */

    sessionStorage.removeItem('parametroBase');
    sessionStorage.removeItem('tipoBiddingCli');
    sessionStorage.removeItem('tipoOferta');
    sessionStorage.removeItem('oferta');
    sessionStorage.removeItem('ofertasSemAjudante');
    sessionStorage.removeItem('ofertasComAjudantes');
}

function trocarHtmlTipoValorCampoTabelaValores(btn, txt, tipo, permiteInformarValor) {
    switch (tipo) {
        case EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.PercentualSobreOValorDaNotaFiscal;
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.PercentualSobreOValorDaNotaFiscal;

            btn.innerHTML = '<span class="fal fa-barcode" style="width: 14px;"></span>';

            if (permiteInformarValor)
                txt.removeAttribute("disabled");
            txt.setAttribute("maxlength", "8");

            if (txt.value.length > 8)
                txt.value = "";

            break;
        case EnumTipoCampoValorTabelaFrete.AumentoPercentual:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoPercentual;
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoPercentual;

            btn.innerHTML = '%';

            if (permiteInformarValor)
                txt.removeAttribute("disabled");
            txt.setAttribute("maxlength", "8");

            if (txt.value.length > 8)
                txt.value = "";

            break;
        case EnumTipoCampoValorTabelaFrete.AumentoValor:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.AumentoDeValor;

            btn.innerHTML = '<span class="fal fa-plus" style="width: 14px;"></span>';

            if (permiteInformarValor)
                txt.removeAttribute("disabled");

            txt.setAttribute("maxlength", "15");

            if (txt.value.length > 15)
                txt.value = "";

            break;
        case EnumTipoCampoValorTabelaFrete.Desabilitado:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.Desabilitado;
            btn.innerHTML = '<span class="fal fa-ban" style="width: 14px;"></span>';
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.Desabilitado;
            txt.value = "";
            txt.setAttribute("disabled", "disabled");

            break;
        case EnumTipoCampoValorTabelaFrete.ValorFixo:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixo;

            btn.innerHTML = '<span class="fal fa-dollar-sign" style="width: 14px;"></span>';

            if (permiteInformarValor)
                txt.removeAttribute("disabled");
            txt.setAttribute("maxlength", "15");

            if (txt.value.length > 15)
                txt.value = "";

            break;
        case EnumTipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
            btn.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixoArredondado;
            txt.title = Localization.Resources.Fretes.TabelaFreteCliente.ValorFixoArredondado;

            btn.innerHTML = '<span class="fal fa-level-up" style="width: 14px;"></span>';

            if (permiteInformarValor)
                txt.removeAttribute("disabled");
            txt.setAttribute("maxlength", "15");

            if (txt.value.length > 15)
                txt.value = "";

            break;
        default:
            break;
    }
}

function trocarTipoValorCampoTabelaValores(btn, tipo) {
    let col = $(btn).closest("td");
    let dados = $(col).data("data");

    if (dados != null) {
        let txt = document.getElementById("txtValores_" + dados.X.toString() + "_" + dados.Y.toString());
        let btn = document.getElementById("btnValores_" + dados.X.toString() + "_" + dados.Y.toString());

        dados.TipoValor = tipo;
        $(col).data("data", dados);

        trocarHtmlTipoValorCampoTabelaValores(btn, txt, tipo);
    }
}

function setarColunaValorMinimoGarantidoTabelaValores(codigoItem) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: Localization.Resources.Fretes.TabelaFreteCliente.ValorMinimoGarantido, cssClass: "" };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: "valorminimogarantido",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function setarColunaValorBaseTabelaValores(codigoItem) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: Localization.Resources.Fretes.TabelaFreteCliente.ValorBase, cssClass: "" };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: "valorbase",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function setarColunaValorMaximoTabelaValores(codigoItem) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: Localization.Resources.Fretes.TabelaFreteCliente.ValorMaximo, cssClass: "" };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: "valormaximo",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function setarColunaPercentualPagamentoAgregadoTabelaValores(codigoItem) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        let cssClass = "";

        if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro != EnumTipoContratoFreteTerceiro.PorPagamentoAgregado)
            cssClass = "d-none";

        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: Localization.Resources.Fretes.TabelaFreteCliente.PorcentagemPagamentoAoAgregado, cssClass: cssClass };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: "PagamentoAgregado",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function setarColunaObservacaoTabelaValores(codigoItem) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: Localization.Resources.Fretes.TabelaFreteCliente.Observacao, cssClass: "" };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: "observacao",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function setarColunaExcedenteTabelaValores(codigoItem, tipo, descricao) {
    if (_tabelaValores.Body[_indiceAtual] == null) {
        _tabelaValores.Body[_indiceAtual] = new Array();
        _tabelaValores.Header[_indiceAtual] = { text: descricao, cssClass: "" };
    }

    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
        Tipo: tipo + "Excedente",
        Data: {
            CodigoItem: codigoItem
        }
    };

    _indiceAtual++;
}

function exibirRota(rota) {
    if (rota.ValorAdicionalFixoPorRota)
        return false;
    else
        return true;
}

function setarColunasTabelaValores(propLista, propDescricao, propCodigo, codigoItem, isSideHeaderCol, type, parentType, adicionarValorCallback) {
    if (parentType != type) {
        for (var i = 0; i < _tabelaFrete[propLista].length; i++) {
            if (isSideHeaderCol) {
                _indiceAtual = 0;

                if (_tabelaValores.Body[_indiceAtual] == null)
                    _tabelaValores.Body[_indiceAtual] = new Array();

                _tabelaValores.Body[_indiceAtual][i] = {
                    Tipo: "string",
                    Texto: getProperty(_tabelaFrete[propLista][i], propDescricao)
                };

                _indiceAtual++;

                let codigo = getProperty(_tabelaFrete[propLista][i], propCodigo);

                if (_tabelaFrete.PossuiValorBase)
                    setarColunaValorBaseTabelaValores(codigo);

                if (!_tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga)
                    setarColunasTabelaValores("TiposCargas", "Tipo.Descricao", "Tipo.Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.TipoCarga, type);

                setarColunasTabelaValores("ModelosTracao", "Modelo.Descricao", "Modelo.Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.ModeloTracao, type);
                //setarColunasTabelaValores("TipoEmbalagem", "TipoEmbalagem.Descricao", "TipoEmbalagem.Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.TipoEmbalagem, type);
                setarColunasTabelaValores("TipoEmbalagens", "TipoEmbalagem.Descricao", "TipoEmbalagem.Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.TipoEmbalagem, type);
                setarColunasTabelaValores("ModelosReboque", "Modelo.Descricao", "Modelo.Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.ModeloReboque, type);
                setarColunasTabelaValores("NumeroEntregas", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.NumeroEntrega, type);

                if (_tabelaFrete.PermiteValorAdicionalEntregaExcedente === true && _tabelaFrete.NumeroEntregas.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Entrega", Localization.Resources.Fretes.TabelaFreteCliente.PorEntregaExcedente);

                setarColunasTabelaValores("Pacotes", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Pacote, type);

                if (_tabelaFrete.PermiteValorAdicionalPacoteExcedente === true && _tabelaFrete.Pacotes.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Pacote", Localization.Resources.Fretes.TabelaFreteCliente.PorPacoteExcedente);

                setarColunasTabelaValores("Pallets", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Pallets, type);

                if (_tabelaFrete.PermiteValorAdicionalPalletExcedente === true && _tabelaFrete.Pallets.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Pallet", Localization.Resources.Fretes.TabelaFreteCliente.PorPalletExcedente);

                setarColunasTabelaValores("PesosTransportados", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Peso, type);

                if (_tabelaFrete.PermiteValorAdicionalPesoExcedente === true && Globalize.parseFloat(_tabelaFrete.PesoExcedente) > 0 && _tabelaFrete.PesosTransportados.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Peso", Localization.Resources.Fretes.TabelaFreteCliente.ACadaExcedente.format(_tabelaFrete.PesoExcedente, _tabelaFrete.PesosTransportados[0].UnidadeMedida.Sigla));

                setarColunasTabelaValores("Distancias", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Distancia, type);

                if (_tabelaFrete.PermiteValorAdicionalQuilometragemExcedente === true && Globalize.parseFloat(_tabelaFrete.QuilometragemExcedente) > 0 && _tabelaFrete.Distancias.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Quilometragem", Localization.Resources.Fretes.TabelaFreteCliente.ACadaKmExcedente.format(_tabelaFrete.QuilometragemExcedente));

                setarColunasTabelaValores("Tempos", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Tempo, type);
                setarColunasTabelaValores("Ajudantes", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Ajudante, type);
                setarColunasTabelaValores("Horas", "Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.Hora, type);

                if (_tabelaFrete.PermiteValorAdicionalHoraExcedente === true && _tabelaFrete.Horas.length > 0)
                    setarColunaExcedenteTabelaValores(codigo, "Hora", Localization.Resources.Fretes.TabelaFreteCliente.PorHoraExcedente);

                setarColunasTabelaValores("ComponentesFrete", "Componente.Descricao", "Codigo", codigo, false, EnumTipoParametroBaseTabelaFrete.ComponenteFrete, type);
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                    setarColunasTabelaValores("RotasFreteEmbarcador", "RotaFrete.Descricao", "Codigo", 0, false, EnumTipoParametroBaseTabelaFrete.Rota, type, exibirRota);

                if (_tabelaFrete.PossuiMinimoGarantido)
                    setarColunaValorMinimoGarantidoTabelaValores(codigo);

                if (_tabelaFrete.PossuiValorMaximo)
                    setarColunaValorMaximoTabelaValores(codigo);

                setarColunaPercentualPagamentoAgregadoTabelaValores(codigo);
                setarColunaObservacaoTabelaValores(codigo);
            } else {

                let adicionar = true;

                if (adicionarValorCallback != null)
                    adicionar = adicionarValorCallback(_tabelaFrete[propLista][i]);

                if (adicionar) {
                    if (_tabelaValores.Body[_indiceAtual] == null) {
                        _tabelaValores.Body[_indiceAtual] = new Array();
                        _tabelaValores.Header[_indiceAtual] = { text: getProperty(_tabelaFrete[propLista][i], propDescricao), cssClass: "" };
                    }

                    _tabelaValores.Body[_indiceAtual][_tabelaValores.Body[_indiceAtual].length] = {
                        Tipo: "objeto",
                        TipoValor: EnumTipoCampoValorTabelaFrete.ValorFixo,
                        PropLista: propLista,
                        PropDescricao: propDescricao,
                        PropCodigo: propCodigo,
                        Data: {
                            Objeto: _tabelaFrete[propLista][i],
                            CodigoItem: codigoItem || 0,
                            Tipo: type
                        }
                    };

                    _indiceAtual++;
                }

            }
        }
    }
}

function getProperty(obj, prop) {

    if (typeof obj === 'undefined')
        return null;

    if (typeof prop === 'undefined')
        return null;

    let _index = prop.indexOf('.');

    if (_index > -1)
        return getProperty(obj[prop.substring(0, _index)], prop.substr(_index + 1));

    return obj[prop];
}

function abrirTelaObservacaoTabelaValores(btn) {
    let dados = $(btn).closest("td").data("data");

    if (dados.Data.Observacao == null)
        $("#txtObservacao").val(_tabelaFrete.Observacao);
    else
        $("#txtObservacao").val(dados.Data.Observacao);

    if (dados.Data.ObservacaoTerceiro == null)
        $("#txtObservacaoTerceiro").val(_tabelaFrete.ObservacaoTerceiro);
    else
        $("#txtObservacaoTerceiro").val(dados.Data.ObservacaoTerceiro);

    if (dados.Data.ImprimirObservacaoCTe == null)
        $("#chkImprimirObservacaoCTe").prop("checked", _tabelaFrete.ImprimirObservacaoCTe);
    else
        $("#chkImprimirObservacaoCTe").prop("checked", dados.Data.ImprimirObservacaoCTe);

    $("body").data("dadosObservacao", dados);
    Global.abrirModal('divModalSolicitarCancelamentoCarga');
}

function salvarObservacaoTabelaValores() {
    let dados = $("body").data("dadosObservacao");

    dados.Data.Observacao = $("#txtObservacao").val();
    dados.Data.ObservacaoTerceiro = $("#txtObservacaoTerceiro").val();
    dados.Data.ImprimirObservacaoCTe = $("#chkImprimirObservacaoCTe").prop("checked");

    $("#colValores_" + dados.X + "_" + dados.Y).data("data", dados);

    Global.fecharModal('divModalSolicitarCancelamentoCarga');
}

function AbrirTelaValoresComponenteFreteTempoTabelaFrete(btn) {
    for (var i = 0; i < _tabelaFrete.ComponentesFrete.length; i++) {
        let componenteFrete = _tabelaFrete.ComponentesFrete[i];

        if (btn.dataset.ComponenteFrete == componenteFrete.Codigo) {

            $("#tblComponenteFreteTempoTabelaFrete tbody").html("");

            let html = "";

            for (var j = 0; j < componenteFrete.Tempos.length; j++) {
                let componenteFreteTempo = componenteFrete.Tempos[j];

                html += "<tr><td>" + Localization.Resources.Fretes.TabelaFreteCliente.DasAs.format(componenteFreteTempo.HoraInicialTempo, componenteFreteTempo.HoraFinalTempo) + "</td>";

                html += "<td>";

                if (componenteFrete.PossuiHorasMinimasCobrancaTempo)
                    html += Localization.Resources.Fretes.TabelaFreteCliente.DasAs.format(componenteFreteTempo.HoraInicialCobrancaMinimaTempo, componenteFreteTempo.HoraFinalCobrancaMinimaTempo);

                html += "</td><td>" + componenteFreteTempo.ValorTempo + "</td></tr>";
            }

            if (string.IsNullOrWhiteSpace(html))
                html = '<tr><td colspan="3">' + Localization.Resources.Fretes.TabelaFreteCliente.NenhumRegistroEncontrado + '</td></tr>';

            $("#tblComponenteFreteTempoTabelaFrete tbody").html(html);

            Global.abrirModal('divModalComponenteFreteTempoTabelaFrete');
        }
    }
}

function limparCamposTabelaValores() {

    $("#divValoresTabelaFrete").find('input:text').val('');

    $("#txtObservacao").val("");
    $("#txtObservacaoTerceiro").val("");
    $("#chkImprimirObservacaoCTe").prop("checked", false);

    if (_tabelaValores.Body != null) {
        for (var i = 0; i < _tabelaValores.Body.length; i++) {
            for (var j = 0; j < _tabelaValores.Body[i].length; j++) {
                if (_tabelaValores.Body[i][j].Tipo == "observacao") {
                    _tabelaValores.Body[i][j].Data.Observacao = "";
                    _tabelaValores.Body[i][j].Data.ObservacaoTerceiro = "";
                    _tabelaValores.Body[i][j].Data.ImprimirObservacaoCTe = false;

                    $("#colValores_" + j.toString() + "_" + i.toString()).data("data", _tabelaValores.Body[i][j]);
                } else if (_tabelaValores.Body[i][j].Tipo == "valorminimogarantido") {
                    _tabelaValores.Body[i][j].Data.ValorMinimoGarantido = "";
                } else if (_tabelaValores.Body[i][j].Tipo == "valormaximo") {
                    _tabelaValores.Body[i][j].Data.ValorMaximo = "";
                } else if (_tabelaValores.Body[i][j].Tipo == "valorbase") {
                    _tabelaValores.Body[i][j].Data.ValorBase = "";
                }
            }
        }
    }
}

jQuery(function ($) {
    $.fn.hScroll = function (amount) {
        amount = amount || 120;

        $(this).unbind("DOMMouseScroll mousewheel");

        $(this).bind("DOMMouseScroll mousewheel", function (event) {
            var oEvent = event.originalEvent,
                direction = oEvent.detail ? oEvent.detail * -amount : oEvent.wheelDelta,
                position = $(this).scrollLeft();
            position += direction > 0 ? -amount : amount;
            $(this).scrollLeft(position);
            event.preventDefault();
        });
    };
});


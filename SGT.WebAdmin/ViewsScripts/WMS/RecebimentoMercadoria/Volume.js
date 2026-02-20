/// <reference path="../../Enumeradores/EnumSituacaoRecebimentoMercadoria.js" />
/// <reference path="RecebimentoMercadoria.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _volume;
var _adicionarVolume;
var _gridVolume;
var editandoVolume = false;
var _modalAdicionarVolume;

var Volume = function () {
    this.Grid = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoBarras = PropertyEntity({ type: types.map, text: "Código de Barras:", required: false, eventClick: ConferirVolumeClick, enable: ko.observable(true) });
    this.LerCodigoBarrasCamera = PropertyEntity({ type: types.map, text: "Abrir câmera:", required: false, eventClick: AbrirCamera, enable: ko.observable(true), visible: ko.observable(window.mobileAndTabletCheck()) });

    this.AdicionarVolume = PropertyEntity({ eventClick: AdicionarVolumeClick, type: types.event, text: "Adicionar Volume", visible: ko.observable(true), enable: ko.observable(true) });

    this.ImprimirEtiquetas = PropertyEntity({ eventClick: ImprimirEtiquetaClick(ETIQUETA.MULTIPLAS), type: types.event, text: "Imprimir Etiquetas", visible: ko.observable(true) });

    this.AutorizarVolumes = PropertyEntity({ eventClick: AutorizarVolumesClick, type: types.event, text: "Autorizar Volumes Faltantes", visible: ko.observable(false), enable: ko.observable(true) });
    this.VolumesFaltantes = PropertyEntity({ eventClick: VolumesFaltantesClick, type: types.event, text: "Volumes Faltantes", visible: ko.observable(true) });

    this.ImprimirEtiquetaPallet = PropertyEntity({ eventClick: ImprimirEtiquetaClick(ETIQUETA.ETIQUETAPALLET), type: types.event, text: "Master Pallet", visible: ko.observable(true) });
}

var AdicionarVolume = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoEmissora = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoBarras = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Identificacao = PropertyEntity({ val: ko.observable(""), def: "" });

    this.ChaveNFe = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CNPJRemetente = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CNPJDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NomeDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoDepositoPosicao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoRecebimento = PropertyEntity({ val: ko.observable(""), def: "" });
    this.CodigoProdutoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "" });

    this.FilialEmissora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial Emissora:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Remetente:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destinatário:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Descricao = PropertyEntity({ text: "*NS Entrega: ", maxlength: 150, visible: true, required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "*Número NF: ", maxlength: 150, visible: true, required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: "*Série: ", maxlength: 150, visible: true, required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.DataEmissaoNF = PropertyEntity({ text: "Data NF: ", visible: true, required: false, val: ko.observable(""), enable: ko.observable(true), getType: typesKnockout.date });
    this.DepositoPosicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local de Armanzenamento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.QuantidadeLote = PropertyEntity({ text: "*Qtd. Volume:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(true) });
    this.QuantidadeConferida = PropertyEntity({ text: "*Qtd. Embarcado:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(false) });
    this.QuantidadeFaltante = PropertyEntity({ text: "*Falta(m):", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false }, val: ko.observable("0,000"), maxlength: 11, def: "0,000", enable: ko.observable(false) });
    this.Peso = PropertyEntity({ text: "*Peso Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 11, def: "0,00", enable: ko.observable(true) });

    this.PesoBruto = PropertyEntity({ text: "Peso Bruto:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 11, def: "0,00", enable: ko.observable(true) });
    this.PesoLiquido = PropertyEntity({ text: "Peso Líquido:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 11, def: "0,00", enable: ko.observable(true) });
    this.ValorNF = PropertyEntity({ text: "Valor NF:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 11, def: "0,00", enable: ko.observable(true) });
    this.ValorMercadoria = PropertyEntity({ text: "Valor Mercadoria:", required: false, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 11, def: "0,00", enable: ko.observable(true) });
    this.TipoUnidade = PropertyEntity({ text: "Tipo Volume: ", maxlength: 150, visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });

    this.Altura = PropertyEntity({
        text: "Altura:",
        required: false,
        getType: typesKnockout.decimal,
        configDecimal: { precision: 2, allowZero: false },
        val: ko.observable("0,00"),
        maxlength: 11,
        def: "0,00",
        enable: ko.observable(true)
    });
    this.Largura = PropertyEntity({
        text: "Largura:",
        required: false,
        getType: typesKnockout.decimal,
        configDecimal: { precision: 2, allowZero: false },
        val: ko.observable("0,00"),
        maxlength: 11,
        def: "0,00",
        enable: ko.observable(true)
    });
    this.Comprimento = PropertyEntity({
        text: "Comprimento:",
        required: false,
        getType: typesKnockout.decimal,
        configDecimal: { precision: 2, allowZero: false },
        val: ko.observable("0,00"),
        maxlength: 11,
        def: "0,00",
        enable: ko.observable(true)
    });
    this.MetroCubico = PropertyEntity({
        text: "Metros Cúbicos",
        required: false,
        getType: typesKnockout.decimal,
        configDecimal: { precision: 2, allowZero: false },
        val: ko.observable("0,00"),
        maxlength: 11,
        def: "0,00",
        enable: ko.observable(false)
    });

    this.QuantidadeLote.val.subscribe(function () {
        AjustarMetrosCubicos();
    });

    this.Altura.val.subscribe(function () {
        AjustarMetrosCubicos();
    });

    this.Largura.val.subscribe(function () {
        AjustarMetrosCubicos();
    });

    this.Comprimento.val.subscribe(function () {
        AjustarMetrosCubicos();
    });


    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarVolumeNovaClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadVolume() {
    _volume = new Volume();
    KoBindings(_volume, "knockoutVolume");

    _adicionarVolume = new AdicionarVolume();
    KoBindings(_adicionarVolume, "knoutAdicionarVolume");

    new BuscarEmpresa(_adicionarVolume.FilialEmissora);
    new BuscarClientes(_adicionarVolume.Remetente);
    new BuscarClientes(_adicionarVolume.Destinatario);
    new BuscarDepositoPosicao(_adicionarVolume.DepositoPosicao);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _adicionarVolume.FilialEmissora.visible(false);
        _adicionarVolume.FilialEmissora.required(false);
    }
    CarregarGridVolume();

    _volume.CodigoBarras.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key === ENTER_KEY)
                ConferirVolumeClick();
        });
    _modalAdicionarVolume = new bootstrap.Modal(document.getElementById("divAdicionarVolume"), { backdrop: true, keyboard: true });
}

function AjustarMetrosCubicos() {
    var altura = Globalize.parseFloat(_adicionarVolume.Altura.val());
    var largura = Globalize.parseFloat(_adicionarVolume.Largura.val());
    var comprimento = Globalize.parseFloat(_adicionarVolume.Comprimento.val());
    var volumes = Globalize.parseFloat(_adicionarVolume.QuantidadeLote.val());

    if (isNaN(volumes))
        volumes = 0;
    if (isNaN(altura))
        altura = 0;
    if (isNaN(largura))
        largura = 0;
    if (isNaN(comprimento))
        comprimento = 0;

    _adicionarVolume.MetroCubico.val(Globalize.format((altura * largura * comprimento * volumes), "n2"));
}

function CarregarGridVolume() {
    var excluir = { descricao: "Remover", id: "clasRemoverVolume", evento: "onclick", metodo: ExcluirVolumeClick, tamanho: "15", icone: "" };
    var editar = { descricao: "Editar", id: "clasEditarVolume", evento: "onclick", metodo: EditarVolumeClick, tamanho: "15", icone: "" };
    var etiqueta = { descricao: "Imprimir Etiqueta", id: "clasEtiquetaVolume", evento: "onclick", metodo: ImprimirEtiquetaClick(ETIQUETA.UNICA), tamanho: "15", icone: "" };
    var etiquetaLRMaster = { descricao: "Etiqueta LR Master", id: "clasEtiquetaVolumeLRMaster", evento: "onclick", metodo: ImprimirEtiquetaClick(ETIQUETA.LRMASTER), tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [excluir, editar, etiqueta, etiquetaLRMaster] };

    _gridVolume = new GridView(_volume.Grid.idGrid, "RecebimentoMercadoria/PesquisaVolume", _recebimentoMercadoria, menuOpcoes, null);
    _gridVolume.CarregarGrid();
    //_gridVolume.CarregarGrid().then(() => {
    //    const grid = document.getElementById(_pesquisaRecebimentoMercadoria.Pesquisar.idGrid)
    //    grid.tBodies[0].firstChild.lastChild.firstElementChild.click()
    //})
}


function ConferirVolumeClick(e, sender) {
    setTimeout(function () {
        var erro = false;
        var codigoBarrasLocalizar = _volume.CodigoBarras.val();
        var numeroNS = "";
        var numeroSerie = "";
        var cnpjRemetente = "";
        var numeroNota = "";
        var serieNota = "";
        var volume = "";

        if (codigoBarrasLocalizar.length === 34) {
            cnpjRemetente = codigoBarrasLocalizar.substring(0, 14);
            cnpjRemetente = Globalize.parseFloat(cnpjRemetente);

            numeroNota = codigoBarrasLocalizar.substring(14, 20);
            numeroNota = Globalize.parseFloat(numeroNota);

            serieNota = codigoBarrasLocalizar.substring(20, 22);
            serieNota = Globalize.parseFloat(serieNota);

            volume = codigoBarrasLocalizar.substring(22, 26);
        }
        else if (codigoBarrasLocalizar.length === 13) {
            numeroNS = codigoBarrasLocalizar.substring(0, 10);
            volume = codigoBarrasLocalizar.substring(10, 13);
        } else if (codigoBarrasLocalizar.length === 31) {
            cnpjRemetente = codigoBarrasLocalizar.substring(0, 14);
            cnpjRemetente = Globalize.parseFloat(cnpjRemetente);

            numeroNota = codigoBarrasLocalizar.substring(14, 23);
            numeroNota = Globalize.parseFloat(numeroNota);

            volume = codigoBarrasLocalizar.substring(23, 27);

            serieNota = codigoBarrasLocalizar.substring(27, 29);
            serieNota = Globalize.parseFloat(serieNota);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Código de barras inválido.");
            _volume.CodigoBarras.val("");
            _volume.CodigoBarras.get$().focus();
            erro = true;
            return;
        }

        if (!erro) {
            var data = {
                CodigoBarrasLocalizar: codigoBarrasLocalizar,
                CodigoRecebimento: _recebimentoMercadoria.Codigo.val(),
                NumeroNS: numeroNS,
                Volume: volume,
                CNPJRemetente: cnpjRemetente,
                NumeroNota: numeroNota,
                SerieNota: serieNota
            };

            executarReST("RecebimentoMercadoria/ConferirVolume", data, function (arg) {
                if (arg.Success) {
                    console.log("codigo merc ConferirVolume ", _recebimentoMercadoria.Codigo.val());
                    _gridVolume.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Volume conferido com sucesso");

                    _volume.CodigoBarras.val("");
                    _volume.CodigoBarras.get$().focus();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    _volume.CodigoBarras.val("");
                    _volume.CodigoBarras.get$().focus();
                }
            });
        }
    }, 100);
}

const ETIQUETA = {
    UNICA: "UNICA",
    MULTIPLAS: "MULTIPLAS",
    LRMASTER: "LRMASTER",
    ETIQUETAPALLET: "ETIQUETAPALLET"
}

const dataEtiqueta = (data, tipoEtiqueta) => {
    return dataEnvio = {
        Etiqueta: tipoEtiqueta,
        Codigo: 0,
        CodigoEmissora: data.CodigoEmissora,
        NomeEmissora: data.NomeEmissora,
        CNPJRemetente: data.CNPJRemetente,
        CNPJDestinatario: data.CNPJDestinatario,
        NomeDestinatario: data.NomeDestinatario,
        ChaveNFe: data.ChaveNFe,
        CodigoDepositoPosicao: data.CodigoDepositoPosicao,
        DataEmissaoNF: data.DataEmissaoNF,
        Descricao: data.Descricao,
        Numero: data.Numero,
        Serie: data.Serie,
        QuantidadeLote: data.QuantidadeLote,
        QuantidadeConferida: data.QuantidadeConferida,
        QuantidadeFaltante: data.QuantidadeFaltante,
        NomeRemetente: data.NomeRemetente,
        DescricaoDepositoPosicao: data.DescricaoDepositoPosicao,
        CodigoCarga: _recebimentoMercadoria.Carga.codEntity(),
        Item: _recebimentoMercadoria.ProdutoEmbarcador.codEntity(),
        CodigoRecebimento: _recebimentoMercadoria.Codigo.val(),
        Peso: data.Peso,
        MetroCubico: data.MetroCubico
    };
}

function ImprimirEtiquetaClick(tipoEtiqueta) {
    return (data) => {
        const dataEnvio = dataEtiqueta(data, tipoEtiqueta);
        executarReST("RecebimentoMercadoria/ImprimirEtiqueta", dataEnvio, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do arquivo da impressão de etiqueta.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}


function VolumesFaltantesClick(e, sender) {
    var data = { RecebimentoVolume: true, CodigoRecebimento: _recebimentoMercadoria.Codigo.val() };
    executarReST("RecebimentoMercadoria/VolumesFaltantes", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do arquivo da impressão dos volumes faltantes.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function AutorizarVolumesClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja autorizar os volumes faltantes?", function () {
        var data = { Codigo: _recebimentoMercadoria.Codigo.val() };
        executarReST("RecebimentoMercadoria/AutorizarProdutosFaltantes", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Recebimento com volumes faltantes autorizado.");
                _volume.AutorizarVolumes.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function AdicionarVolumeClick(e, sender) {
    LimparCampos(_adicionarVolume);
    editandoVolume = false;

    _adicionarVolume.Codigo.val(guid());
    _adicionarVolume.Adicionar.text("Adicionar Volume");
    _modalAdicionarVolume.show();
}


function AdicionarVolumeNovaClick(e, sender) {
    if (ValidarCamposObrigatorios(_adicionarVolume)) {
        var volumeGrid = new Object();

        volumeGrid.Codigo = _adicionarVolume.Codigo.val();
        volumeGrid.CodigoEmissora = _adicionarVolume.FilialEmissora.codEntity();
        volumeGrid.NomeEmissora = _adicionarVolume.FilialEmissora.val();
        volumeGrid.CNPJRemetente = _adicionarVolume.Remetente.codEntity();
        volumeGrid.CNPJDestinatario = _adicionarVolume.Destinatario.codEntity();
        volumeGrid.NomeDestinatario = _adicionarVolume.Destinatario.val();
        volumeGrid.ChaveNFe = _adicionarVolume.ChaveNFe.val();
        volumeGrid.CodigoDepositoPosicao = _adicionarVolume.DepositoPosicao.codEntity();
        volumeGrid.CodigoRecebimento = _recebimentoMercadoria.Codigo.val();
        volumeGrid.CodigoProdutoEmbarcador = _recebimentoMercadoria.ProdutoEmbarcador.codEntity();
        volumeGrid.CodigoBarras = "";
        volumeGrid.Identificacao = "";
        volumeGrid.DataEmissaoNF = _adicionarVolume.DataEmissaoNF.val();
        volumeGrid.Descricao = _adicionarVolume.Descricao.val();
        volumeGrid.Numero = _adicionarVolume.Numero.val();
        volumeGrid.Serie = _adicionarVolume.Serie.val();
        volumeGrid.QuantidadeLote = _adicionarVolume.QuantidadeLote.val();
        volumeGrid.QuantidadeConferida = _adicionarVolume.QuantidadeConferida.val();
        volumeGrid.QuantidadeFaltante = _adicionarVolume.QuantidadeFaltante.val();
        volumeGrid.NomeRemetente = _adicionarVolume.Remetente.val();
        volumeGrid.DescricaoDepositoPosicao = _adicionarVolume.DepositoPosicao.val();
        volumeGrid.Peso = _adicionarVolume.Peso.val();

        volumeGrid.Altura = _adicionarVolume.Altura.val();
        volumeGrid.Largura = _adicionarVolume.Largura.val();
        volumeGrid.Comprimento = _adicionarVolume.Comprimento.val();

        volumeGrid.PesoBruto = _adicionarVolume.PesoBruto.val();
        volumeGrid.PesoLiquido = _adicionarVolume.PesoLiquido.val();
        volumeGrid.ValorNF = _adicionarVolume.ValorNF.val();
        volumeGrid.ValorMercadoria = _adicionarVolume.ValorMercadoria.val();
        volumeGrid.TipoUnidade = _adicionarVolume.TipoUnidade.val();

        var quantidade = Globalize.parseFloat(volumeGrid.QuantidadeLote);
        var quantidadeConferida = Globalize.parseFloat(volumeGrid.QuantidadeConferida);
        var quantidadeFaltante = quantidade - quantidadeConferida;
        volumeGrid.QuantidadeFaltante = quantidadeFaltante.toFixed(3).replace(".", ",");

        executarReST("RecebimentoMercadoria/SalvarMercadoriaVolume", volumeGrid, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _recebimentoMercadoria.Codigo.val(arg.Data.CodigoRecebimento)
                    console.log("codigo merc SalvarMercadoriaVolume ", _recebimentoMercadoria.Codigo.val());
                    _gridVolume.CarregarGrid();

                    LimparCampos(_adicionarVolume);
                    editandoVolume = false;

                    _adicionarVolume.Codigo.val(guid());
                    _adicionarVolume.Adicionar.text("Adicionar Volume");

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    }
}

function EditarVolumeClick(data) {
    _adicionarVolume.Codigo.val(data.Codigo);
    _adicionarVolume.FilialEmissora.codEntity(data.CodigoEmissora);
    _adicionarVolume.FilialEmissora.val(data.NomeEmissora);
    _adicionarVolume.Remetente.codEntity(data.CNPJRemetente);
    _adicionarVolume.Destinatario.codEntity(data.CNPJDestinatario);
    _adicionarVolume.Destinatario.val(data.NomeDestinatario);
    _adicionarVolume.ChaveNFe.val(data.ChaveNFe);
    _adicionarVolume.DepositoPosicao.codEntity(data.CodigoDepositoPosicao);
    _adicionarVolume.DataEmissaoNF.val(data.DataEmissaoNF);
    _adicionarVolume.Descricao.val(data.Descricao);
    _adicionarVolume.Numero.val(data.Numero);
    _adicionarVolume.Serie.val(data.Serie);
    _adicionarVolume.QuantidadeLote.val(data.QuantidadeLote);
    _adicionarVolume.QuantidadeConferida.val(data.QuantidadeConferida);
    _adicionarVolume.QuantidadeFaltante.val(data.QuantidadeFaltante);
    _adicionarVolume.Remetente.val(data.NomeRemetente);
    _adicionarVolume.DepositoPosicao.val(data.DescricaoDepositoPosicao);
    _adicionarVolume.Peso.val(data.Peso);

    _adicionarVolume.Altura.val(data.Altura);
    _adicionarVolume.Largura.val(data.Largura);
    _adicionarVolume.Comprimento.val(data.Comprimento);
    _adicionarVolume.MetroCubico.val(data.MetroCubico);

    _adicionarVolume.PesoBruto.val(data.PesoBruto);
    _adicionarVolume.PesoLiquido.val(data.PesoLiquido);
    _adicionarVolume.ValorNF.val(data.ValorNF);
    _adicionarVolume.ValorMercadoria.val(data.ValorMercadoria);
    _adicionarVolume.TipoUnidade.val(data.TipoUnidade);

    editandoVolume = true;
    _adicionarVolume.Adicionar.text("Atualizar Volume");
    _modalAdicionarVolume.show();
}

function ExcluirVolumeClick(e) {
    if (_recebimentoMercadoria.Situacao.val() !== EnumSituacaoRecebimentoMercadoria.Iniciado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possivel remover o volume.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja remover a volume selecionado?", function () {
        var data = { Codigo: e.Codigo };
        executarReST("RecebimentoMercadoria/ExcluirMercadoriaVolume", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    console.log("codigo merc ExcluirMercadoriaVolume ", _recebimentoMercadoria.Codigo.val());
                    _gridVolume.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function LimparCamposVolume() {
    _volume.AutorizarVolumes.visible(false);
    LimparCampos(_volume);
    console.log("codigo merc LimparCamposVolume ", _recebimentoMercadoria.Codigo.val());
    _gridVolume.CarregarGrid();
}


function AbrirCamera() {
    var etiquetaUsaQRCode = _CONFIGURACAO_TMS.UtilizarEtiquetaDetalhadaWMS;
    if (etiquetaUsaQRCode) {
        LerQRCodeCamera();
    } else {
        LerCodigoBarrasCamera();
    }
}

window.mobileAndTabletCheck = function () {
    let check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};
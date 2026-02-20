/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Consultas/Usuario.js" />

var _dadosTransporte;
var _cameraDadosTransporte;
var _opcoesCameraDadosTransporte;

var DadosTransporte = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SolicitacaoVeiculo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tração (Cavalo):", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarAjudantes = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: "Informar Ajudantes", visible: ko.observable(true), enable: ko.observable(true), idGrid: guid(), enable: ko.observable(true) });

    this.ArquivoFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable("Arquivo:"), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.ArquivoFotoRemover = PropertyEntity({ eventClick: removerFotoMotoristaDadosTransporte, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Remover), visible: ko.observable(true), enable: ko.observable(true) });
    this.FotoMotorista = PropertyEntity({});
    this.FotoMotoristaArquivo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.AbrirCamera = PropertyEntity({ eventClick: AbrirCamaraDadosTransporteClick, type: types.event, text: "Abrir Camara", visible: ko.observable(true), enable: ko.observable(true) });

    this.ArquivoFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            enviarFotoMotoristaDadosTransporte();
    });

    this.Atualizar = PropertyEntity({ eventClick: atualizarDadosTransporteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true), enable: ko.observable(true) });
}

var OpcoesCameraDadosTransporte = function () {
    this.TirarFoto = PropertyEntity({ eventClick: TirarFotoDadosTransporteClick, type: types.event, text: "Tirar Foto", visible: ko.observable(true) });
    this.FecharCamera = PropertyEntity({ eventClick: FecharModalCameraDadosTransporteClick, type: types.event, visible: ko.observable(true) });
}

function loadDadosTransporte() {
    _dadosTransporte = new DadosTransporte();
    KoBindings(_dadosTransporte, "knockoutDadosTransporte");

    BuscarMotoristas(_dadosTransporte.Motorista, null, null, null, true);
    BuscarVeiculos(_dadosTransporte.Veiculo, null, null, null, null, null, null, null, true, null, null, "0");

    carregarGridDadosTransporteAjudante();
    loadCameraDadosTransporte();
}

function buscarDadosTransporte(codigoSolicitacaoVeiculo) {
    LimparCampos(_dadosTransporte);
    executarReST("SolicitacaoVeiculo/BuscarDadosTransporte", { Codigo: codigoSolicitacaoVeiculo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_dadosTransporte, retorno);

                atualizarGridDadosTransporteAjudante(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarDadosTransporteClick() {
    if (!ValidarCamposObrigatorios(_dadosTransporte)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    if (string.IsNullOrWhiteSpace(_dadosTransporte.FotoMotoristaArquivo.val())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A imagem do Motorista é obrigatória");
        return;
    }

    let dados = {
        Codigo: _dadosTransporte.Codigo.val(),
        SolicitacaoVeiculo: _dadosTransporte.SolicitacaoVeiculo.val(),
        Motorista: _dadosTransporte.Motorista.codEntity(),
        Veiculo: _dadosTransporte.Veiculo.codEntity(),
        Ajudantes: JSON.stringify(_dadosTransporte.AdicionarAjudantes.basicTable.BuscarRegistros()),
        FotoMotoristaArquivo: _dadosTransporte.FotoMotoristaArquivo.val()
    }

    executarReST("SolicitacaoVeiculo/SalvarDadosTransporte", dados , function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados Salvos com Sucesso!");
                Global.fecharModal("divModalDadosTransporte");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

}

function abrirModalDadosTransporte() {
    Global.abrirModal("divModalDadosTransporte");  
}

function SetarEnableCamposDadosTransporte(permiteEditar) {
    SetarEnableCamposKnockout(_dadosTransporte, permiteEditar);
}

// #region Ajudantes
function removerAjudanteDadosTransporteClick(data) {

    let ajudanteGrid = _dadosTransporte.AdicionarAjudantes.basicTable.BuscarRegistros();
    for (let i = 0; i < ajudanteGrid.length; i++) {
        if (data.Codigo == ajudanteGrid[i].Codigo) {
            ajudanteGrid.splice(i, 1);
            break;
        }
    }
    _dadosTransporte.AdicionarAjudantes.basicTable.CarregarGrid(ajudanteGrid);
}

function atualizarGridDadosTransporteAjudante(data) {
    let ajudantes = new Array();

    if (data.Ajudantes != null) {
        $.each(data.Ajudantes, function (i, ajudante) {
            ajudantes.push({ Codigo: ajudante.Codigo, CPF: ajudante.CPF, Nome: ajudante.Descricao });
        });
    }

    if (_dadosTransporte.AdicionarAjudantes?.basicTable != null)
        _dadosTransporte.AdicionarAjudantes.basicTable.CarregarGrid(ajudantes);
}

function carregarGridDadosTransporteAjudante() {
    let remover = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: function (datagrid) {
            removerAjudanteDadosTransporteClick(datagrid);
        }, icone: ""
    };
    let menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [remover] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.Carga.CPF, width: "15%", className: "text-align-center", orderable: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "70%", className: "text-align-left", orderable: false }
    ];

    let _gridDadosTransporteAjudantes = new BasicDataTable(_dadosTransporte.AdicionarAjudantes.idGrid, header, menuOpcoes);

    _gridDadosTransporteAjudantes.CarregarGrid(new Array());

    _dadosTransporte.AdicionarAjudantes.basicTable = _gridDadosTransporteAjudantes;
    BuscarMotoristas(_dadosTransporte.AdicionarAjudantes, null, null, _gridDadosTransporteAjudantes, true);
}

// #endregion

// #region Foto Motorista

function obterFormDataFotoMotoristaDadosTransporte() {
    let arquivo = document.getElementById(_dadosTransporte.ArquivoFoto.id);

    if (arquivo.files.length > 0) {
        let formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function enviarFotoMotoristaDadosTransporte() {
    let formData = obterFormDataFotoMotoristaDadosTransporte();
    
    if (formData) {
        enviarArquivo("SolicitacaoVeiculo/AdicionarFoto?callback=?", { Codigo: _dadosTransporte.SolicitacaoVeiculo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _dadosTransporte.FotoMotorista.val(retorno.Data.FotoMotorista);
                    _dadosTransporte.FotoMotoristaArquivo.val(retorno.Data.FotoMotoristaArquivo);
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                    _dadosTransporte.FotoMotorista.val("");
                    _dadosTransporte.ArquivoFoto.val("");
                    _dadosTransporte.FotoMotoristaArquivo.val("");
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function removerFotoMotoristaDadosTransporte() {
    executarReST("SolicitacaoVeiculo/ExcluirFoto", { FotoMotoristaArquivo: _dadosTransporte.FotoMotoristaArquivo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _dadosTransporte.FotoMotorista.val("");
                _dadosTransporte.ArquivoFoto.val("");
                _dadosTransporte.FotoMotoristaArquivo.val("");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}


function loadCameraDadosTransporte() {
    const webcamElement = document.getElementById('webcam');
    const canvasElement = document.getElementById('canvas');
    _cameraDadosTransporte = new Webcam(webcamElement, 'user', canvasElement);

    _opcoesCameraDadosTransporte = new OpcoesCameraDadosTransporte();
    KoBindings(_opcoesCameraDadosTransporte, "CardCameraDadosTransporte");
}

async function TirarFotoDadosTransporteClick() {
    let imagemBase64 = _cameraDadosTransporte.snap();

    const data = await ObterFormDataCamera(imagemBase64);

    if (data == null)
        return;

    enviarArquivo("SolicitacaoVeiculo/AdicionarFoto?callback=?", { Codigo: _dadosTransporte.SolicitacaoVeiculo.val() }, data, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);

        _dadosTransporte.FotoMotorista.val(retorno.Data.FotoMotorista);
        _dadosTransporte.FotoMotoristaArquivo.val(retorno.Data.FotoMotoristaArquivo);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Imagem Carregada");
    });

    _cameraDadosTransporte.stop();
    Global.fecharModal("#divCardCameraDadosTransporte");
}

function AbrirCamaraDadosTransporteClick() {
    _cameraDadosTransporte.start().then(result => {

    }).catch(err => {
        exibirMensagem(tipoMensagem.falha, "Error", "Não foi possivel Abrir a Camera");
    });
    Global.abrirModal("#divCardCameraDadosTransporte");
}

async function ObterFormDataCamera(img) {
    if (img == null)
        return null;

    const formData = new FormData();
    formData.append("ArquivoFoto", await ConvertBase64ToFile(img));
    return formData
}

async function ConvertBase64ToFile(imgBase64) {
    const regex = new RegExp(/^data:(.+);base64/);
    const [, typeImage] = imgBase64.match(regex);
    const res = await fetch(imgBase64);
    const blob = await res.blob();

    let [, extension] = typeImage.split("/");
    return new File([blob], `${guid()}.${extension}`, { type: typeImage });
}

function FecharModalCameraDadosTransporteClick() {
    _cameraDadosTransporte.stop();
    Global.fecharModal("#divCardCameraDadosTransporte");
}
    // #endregion
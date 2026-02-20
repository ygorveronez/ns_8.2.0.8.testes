/// <reference path="../../../Consultas/Container.js" />
/// <reference path="ColetaContainerAnexo.js" />

var _HTMLCargaContainer = "";
var _containerCarga;
var _anexoColetaContainerCarga;


//PARA CADASTRAR UM NOVO CONTAINER
var CadastroContainer = function () {
    this.Numero = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getRequiredFieldDescription(), maxlength: 20, required: ko.observable(true), val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoIntegracao.getFieldDescription(), required: ko.observable(false), maxlength: 50, val: ko.observable("") });
    this.Tara = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Tara.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoDeContainer.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeContainer.Proprio), options: EnumTipoPropriedadeContainer.obterOpcoes(), def: EnumTipoPropriedadeContainer.Proprio, text: Localization.Resources.Cargas.Carga.Propriedade.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

var ColetaContainerCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataColetaContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataColetaContainer.getRequiredFieldDescription(), getType: typesKnockout.date });
    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: false, enable: ko.observable(true), val: ko.observable("") }); /*Utilizado para carga de transferencia Container.. buscar containers apenas de um local.*/

    this.Anexos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(recarregarGridColetaContainerAnexo);
    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoColetaContainerModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    // ***** OCR RIC
    this.TipoContainerRic = PropertyEntity({ text: "Tipo:", getType: typesKnockout.string, val: ko.observable('') });
    this.TaraContainer = PropertyEntity({ text: "Tara:", getType: typesKnockout.int, val: ko.observable(0) });
    this.ArmadorBooking = PropertyEntity({ text: "Armador/Booking:", getType: typesKnockout.string, val: ko.observable('') });
    this.Transportadora = PropertyEntity({ text: "Transportadora:", getType: typesKnockout.string, val: ko.observable('') });
    this.Motorista = PropertyEntity({ text: "Motorista:", getType: typesKnockout.string, val: ko.observable('') });
    this.Placa = PropertyEntity({ text: "Placa:", getType: typesKnockout.placa, val: ko.observable('') });

    this.MostrarAnexoRic = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ArquivoRic = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "RIC:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivoRic = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ArquivoRic.val.subscribe(function (novoValor) { _containerCarga.NomeArquivoRic.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.ArquivoRicEventChange = function () { adicionarColetaContainerAnexoRicClick(); };
    //*****

    this.Salvar = PropertyEntity({ eventClick: salvarColetaContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.Salvar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverContainer = PropertyEntity({ eventClick: removerContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverContainer, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
}

function adicionarColetaContainerAnexoRicClick() {

    var arquivo = document.getElementById(_containerCarga.ArquivoRic.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Descricao: "RIC",
        OcrRic: true,
        NomeArquivo: _containerCarga.NomeArquivoRic.val(),
        Arquivo: arquivo.files[0]
    };

    enviarColetaContainerAnexosRic(_containerCarga.Codigo.val(), [anexo]);

}

function enviarColetaContainerAnexosRic(codigo, anexos) {
    var formData = obterFormDataColetaContainerAnexoRic(anexos);

    if (formData) {
        enviarArquivo("ColetaContainerAnexo/AnexarArquivos", { Codigo: codigo, CodigoCarga: _containerCarga.Carga.val() }, formData, function (retorno) {
            if (!retorno.Success) {
                let tipo = retorno.Msg === "Ocorreu uma falha ao anexar o(s) arquivo(s)." ? tipoMensagem.falha : tipoMensagem.atencao;

                exibirMensagem(tipo, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

                if (tipo !== tipoMensagem.falha)
                    ConfirmacaoQueSalvaOArquivoMesmoError(codigo, anexos);
                return;
            }
            if (retorno.Data && retorno.Data.Ric) {
                let ric = retorno.Data.Ric;

                if (ric.CodigoContainer > 0) {
                    _containerCarga.Container.codEntity(ric.CodigoContainer);
                    _containerCarga.Container.val(ric.Container);
                }

                PreencherObjetoKnout(_containerCarga, retorno);

                let exibirMsgPreencherCampo = !(ric.CodigoContainer && ric.TipoContainer && ric.TaraContainer && ric.ArmadorBooking &&
                    ric.Transportadora && ric.Motorista && ric.Placa && ric.DataDeColeta);

                _containerCarga.TipoContainerRic.val(ric.TipoContainer);
                _containerCarga.TaraContainer.val(ric.TaraContainer);
                _containerCarga.ArmadorBooking.val(ric.ArmadorBooking);
                _containerCarga.Transportadora.val(ric.Transportadora);
                _containerCarga.Motorista.val(ric.Motorista);
                _containerCarga.Placa.val(ric.Placa);
                _containerCarga.DataColetaContainer.val(ric.DataDeColeta);

                _containerCarga.Anexos.val(retorno.Data.Anexos);

                if (exibirMsgPreencherCampo)
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha,
                        "Nem todos os campos puderam ser preenchidos. Por favor, informe manualmente.");
                else
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ArquivoAnexadoComSucesso);

                Salvar(_containerCarga, "ColetaContainer/VincularContainerCarga", function (retorno) {
                    if (retorno.Success) {
                        if (!retorno.Data) {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 200000);
                            BuscarColetaContainerCarga(_cargaAtual);
                        }

                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Não foi possível vincular a Carga ao Container.", retorno.Msg);
                });
            }
            else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.NaoFoiPossivelAnexarArquivo, retorno.Msg);
                ConfirmacaoQueSalvaOArquivoMesmoError(codigo, anexos);
            }

        });
    }
}

function ConfirmacaoQueSalvaOArquivoMesmoError(codigo, anexos) {
    exibirConfirmacao("Erro ao processar", "Deseja Adicionar o aquivo como um anexo da mesma forma? ", () => {
        enviarColetaContainerAnexos(codigo, anexos);
    })
}

function obterFormDataColetaContainerAnexoRic(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("OcrRic", anexo.OcrRic);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function carregarDadosRic() {
    let p = new promise.Promise();
    var codigoColetaContainer = _containerCarga.Codigo.val();
    executarReST("ColetaContainerAnexo/BuscarRicPelaColetaContainer", { CodigoColetaContainer: codigoColetaContainer }, function (retorno) {
        if (retorno && retorno.Success) {
            if (retorno.Data && retorno.Data.Ric) {
                let ric = retorno.Data.Ric;

                _containerCarga.TipoContainerRic.val(ric.TipoContainer);
                _containerCarga.TaraContainer.val(ric.TaraContainer);
                _containerCarga.ArmadorBooking.val(ric.ArmadorBooking);
                _containerCarga.Transportadora.val(ric.Transportadora);
                _containerCarga.Motorista.val(ric.Motorista);
                _containerCarga.Placa.val(ric.Placa);

                if (!_containerCarga.DataColetaContainer.val() || _containerCarga.DataColetaContainer.val().trim().length == 0) {
                    _containerCarga.DataColetaContainer.val(ric.DataDeColeta);
                }
                if (ric.CodigoContainer > 0) {
                    _containerCarga.Container.codEntity(ric.CodigoContainer);
                    _containerCarga.Container.val(ric.Container);
                }
            }
        }
        p.done();
    });
    return p;
};

function loadCargaContainer(carga) {
    let idDivContainerCarga;

    if (_cargaAtual.TransferenciaContainer.val()) {
        $("#liTabColetaContainer_" + _cargaAtual.EtapaInicioTMS.idGrid).removeClass("d-none");
        idDivContainerCarga = "tabCargaDadosTransporteColetaContainer_" + _cargaAtual.EtapaInicioTMS.idGrid;
    } else {
        idDivContainerCarga = "divContainer_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";
    }

    let divCargaContainer = $("#" + idDivContainerCarga);
    CarregarHTMLContainer().then(function () {

        divCargaContainer.html(_HTMLCargaContainer.replace(/#knoutDocumentosParaEmissao/g, (_cargaAtual.EtapaInicioTMS.idGrid)));

        _containerCarga = new ColetaContainerCarga();
        KoBindings(_containerCarga, "knoutContainer_" + _cargaAtual.EtapaInicioTMS.idGrid);

        _containerCarga.MostrarAnexoRic.visible(_cargaAtual.ObrigarInformarRICnaColetaDeConteiner.val());

        _containerCarga.Carga.val(_cargaAtual.Codigo.val());

        LocalizeCurrentPage();

        divCargaContainer.removeClass("d-none");

        if (!_cargaAtual.TransferenciaContainer.val())
            $("#knoutContainer_" + _cargaAtual.EtapaInicioTMS.idGrid).removeClass("row")

        if (_cargaAtual.TransferenciaContainer.val()) {
            _containerCarga.Local.codEntity(_cargaAtual.RemetenteTrasferencia.codEntity())
            _containerCarga.Local.val(_cargaAtual.RemetenteTrasferencia.val())

            new BuscarContainers(_containerCarga.Container, null, null, true, cadastrarNovoContainerClick, null, _containerCarga.Local, EnumStatusColetaContainer.EmAreaEsperaVazio);
        }
        else
            new BuscarContainers(_containerCarga.Container, null, null, true, cadastrarNovoContainerClick);

        BuscarColetaContainerCarga(_cargaAtual).then(function () {
            loadColetaContainerAnexo(_cargaAtual, _containerCarga);
            carregarDadosRic();
        });

    });
}

function CarregarHTMLContainer() {
    let p = new promise.Promise();

    if (_HTMLCargaContainer.length == 0) {
        $.get("Content/Static/Carga/CargaContainer.html?dyn=" + guid(), function (data) {
            _HTMLCargaContainer = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function cadastrarNovoContainerClick() {
    var idDiv = guid();
    var knoutCadastro = idDiv + "_knockoutCadastroContainer";
    var modalCadastro = idDiv + "divModalCadastrarContainer";

    var fnPreecherRetornoSelecao = function (knout, e, idDiv) {
        knout.codEntity(e.Codigo);
        knout.entityDescription(e.Numero);
        knout.val(e.Numero);
        Global.fecharModal(modalCadastro);
    }

    $.get("Content/Static/Consultas/Cadastros/Container.html?dyn=" + guid(), function (data) {
        var html = data.replace(/#knockoutCadastroContainer/g, knoutCadastro).replace(/#divModalCadastrarContainer/g, modalCadastro);

        $('body #js-page-content').append(html);
        var container = new CadastroContainer();

        if (_containerCarga && _containerCarga.Container.val()) {
            container.Numero.val(_containerCarga.Container.val());
        }

        container.Cancelar.eventClick = function (e) {
            Global.fecharModal(modalCadastro);
        };

        container.Adicionar.eventClick = function (e) {
            Salvar(e, "Container/Adicionar", function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        Global.fecharModal(modalCadastro);
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                        fnPreecherRetornoSelecao(_containerCarga.Container, arg.Data, idDiv);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        };

        KoBindings(container, knoutCadastro, false);

        new BuscarTiposContainer(container.TipoContainer);

        Global.abrirModal(modalCadastro);
    });
}

function salvarColetaContainerClick() {
    Salvar(_containerCarga, "ColetaContainer/VincularContainerCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                BuscarColetaContainerCarga(_cargaAtual);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 200000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerContainerClick() {
    Salvar(_containerCarga, "ColetaContainer/RemoverContainerCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                BuscarColetaContainerCarga(_cargaAtual).then(function () {
                    recarregarGridColetaContainerAnexo();
                    carregarDadosRic();
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 200000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function BuscarColetaContainerCarga(carga) {
    let p = new promise.Promise();
    _containerCarga.RemoverContainer.visible(false);

    executarReST("ColetaContainer/BuscarContainerCarga", { Carga: carga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_containerCarga, retorno);

                if (retorno.Data.PodeRemoverContainer)
                    _containerCarga.RemoverContainer.visible(true);

                p.done();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            p.done();
        }
    });

    return p;
}

function adicionarAnexoColetaContainerModalClick() {
    adicionarColetaContainerAnexo();
}

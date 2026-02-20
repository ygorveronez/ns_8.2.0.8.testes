
var _file;
var _exibirCanhotos;
var _pesquisaCanhoto;

var PesquisaCanhoto = function () {
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            Confirmar();
        }, type: types.event, text: "Vincular Canhotos", idGrid: guid(), visible: ko.observable(false)
    });

    this.VincularOutraCanhoteira = PropertyEntity({
        eventClick: function (e) {
            LimparCamposCanhoto();
        }, type: types.event, text: "Vincular Outra Canhoteira", idGrid: guid(), visible: ko.observable(false)
    });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.png,.bmp,.jpeg,.gif, .pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });
}

function LimparCamposCanhoto() {
    _exibirCanhotos.ImagensConferencia.val([]);
    _pesquisaCanhoto.Confirmar.visible(false);
    _pesquisaCanhoto.VincularOutraCanhoteira.visible(false);

    $("#" + _pesquisaCanhoto.Upload.idFile).val("");

    _pesquisaCanhoto.Carga.codEntity('');
    _pesquisaCanhoto.Carga.val('');
};

var ItemArray = function () {
    this.PendenteAprovacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Rejeitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisibilidadeRodape = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Numero = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), });
    this.Id = PropertyEntity({ getType: typesKnockout.string, val: ko.observable('') });
    this.Y = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Miniatura = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(''), def: '' });
}

var getItemArray = function (itemKnockout) {
    let item = {
        PendenteAprovacao: itemKnockout.PendenteAprovacao.val(),
        Rejeitado: itemKnockout.Rejeitado.val(),
        VisibilidadeRodape: itemKnockout.VisibilidadeRodape.val(),
        Numero: itemKnockout.Numero.val(),
        Miniatura: itemKnockout.Miniatura.val(),
        Id: itemKnockout.Id.val(),
        Y: itemKnockout.Y.val()
    };
    return item;
}

var ExibirCanhotos = function () {

    this.ImagensConferencia = PropertyEntity({ val: ko.observableArray([]), });
    this.TipoServicoMultiCTe = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisualizarPDF = PropertyEntity({
        eventClick: function (e) {
            //exibirModalVisualizarPDFClick({ Codigo: e.Codigo.val() })
        }
    });

}

var dragg;

function loadEnviarCanhotos() {
    _exibirCanhotos = new ExibirCanhotos();
    _pesquisaCanhoto = new PesquisaCanhoto();

    KoBindings(_pesquisaCanhoto, "knockoutPesquisaCanhoto", false);
    KoBindings(_exibirCanhotos, "knockoutExibirCanhotos");

    _pesquisaCanhoto.Upload.file = document.getElementById(_pesquisaCanhoto.Upload.idFile);

    new BuscarCargas(_pesquisaCanhoto.Carga);

    dragg = $.draggImagem({
        container: ".DivConferencia",
        image: ".container-drag img"
    });

}

function Confirmar() {
    if (!_pesquisaCanhoto.Carga.codEntity()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Selecione uma carga.");
        return;
    }
    executarReST("EnviarCanhotos/VincularCanhoto", { Key: _antiForgeryKey, Carga: _pesquisaCanhoto.Carga.codEntity() }, function (arg) {
        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);
            return;
        }
        if (arg.Data) {
            if (arg.Data.TeveFalha) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.Falha);
            }
            if (arg.Data.TeveSucesso) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.Sucesso);
            }
            _pesquisaCanhoto.Confirmar.visible(false);
            _pesquisaCanhoto.VincularOutraCanhoteira.visible(true);
        }
    });
}

function UploadChange() {
    if (!_pesquisaCanhoto.Carga.codEntity()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Selecione uma carga.");
        $("#" + _pesquisaCanhoto.Upload.idFile).val("");
        return;
    }

    if (_pesquisaCanhoto.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _pesquisaCanhoto.Upload.file.files.length; i < s; i++) {
            data.append("Imagem", _pesquisaCanhoto.Upload.file.files[i]);
        }
        //$('#' + _pesquisaCanhoto.idFile)[0].files[0];
        _file = document.querySelector('input[type="file"]').files[0];

        enviarArquivo("EnviarCanhotos/UploadImagens", { Key: _antiForgeryKey, Carga: _pesquisaCanhoto.Carga.codEntity() }, data, function (arg) {
            if (!arg.Success) {
                _pesquisaCanhoto.Confirmar.visible(false);
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                return;
            }
            if (arg.Success && arg.Data) {
                if (arg.Data.length == 0) {
                    _pesquisaCanhoto.Confirmar.visible(false);
                    exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhuma nota fiscal encontrada para a carga selecionada.");
                    return;
                }
                _pesquisaCanhoto.Confirmar.visible(true);
                var objetos = [];
                for (let i = 0; i < arg.Data.length; i++) {
                    let obj = {
                        Y: arg.Data[i].Y,
                        Height: arg.Data[i].Height,
                        NumeroNFe: arg.Data[i].NumeroNFe,
                        Base64: arg.Data[i].Base64
                    }
                    objetos.push(obj);
                }
                SetCanhotoScroll(objetos);
            } else {
                _pesquisaCanhoto.Confirmar.visible(false);
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            $("#" + _pesquisaCanhoto.Upload.idFile).val("");
        });
    }
}

function SetCanhotoScroll(objetos) {

    var arr = [];
    for (let i = 0; i < objetos.length; i++) {
        let obj = objetos[i];
        //obj.Y; obj.Height; obj.NumeroNFe;

        var item = new ItemArray();
        item.Id.val('imagem_' + obj.Y);
        item.Y.val(obj.Y);
        item.Numero.val(obj.NumeroNFe);
        item.Miniatura.val(obj.Base64);
        arr.push(getItemArray(item));

    }
    _exibirCanhotos.ImagensConferencia.val(arr);

    dragg = $.draggImagem({
        container: ".DivConferencia",
        image: ".container-drag img"
    });

}

function setarScrollDeCadaCanhoto() {
    $('#tabImagensAgrupadas .img-canhoto').each(function () {
        let id = $(this).find('img').attr('id');
        var y = id.replace("imagem_", "");
        $(this)[0].scrollTop = y;
    });
}

function getBase64(file, callback) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        callback(reader.result);
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

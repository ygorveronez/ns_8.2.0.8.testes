//*******MAPEAMENTO KNOUCKOUT*******

var _gridLacresMDFeManual, _lacresMDFeManual;

var LacreMDFeManual = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ text: "*Número:", required: true, maxlength: 30, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarLacreClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadLacre() {

    _lacresMDFeManual = new LacreMDFeManual();
    KoBindings(_lacresMDFeManual, "tabLacre");
    
    RecarregarGridLacres();
}

function RecarregarGridLacres() {
    if (_gridLacresMDFeManual != null) {
        _gridLacresMDFeManual.Destroy();
        _gridLacresMDFeManual = null;
    }

    var excluir = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverLacreClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    if (_cargaMDFeManual.Situacao.val() != EnumSituacaoMDFeManual.EmDigitacao)
        menuOpcoes = null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "85%", className: "text-align-left" }
    ];

    _gridLacresMDFeManual = new BasicDataTable(_lacresMDFeManual.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridLacresMDFeManual.CarregarGrid(_cargaMDFeManual.ListaLacres.val());
}

function RemoverLacreClick(data) {
    var lacres = _cargaMDFeManual.ListaLacres.val();

    for (var i = 0; i < lacres.length; i++) {
        if (data.Codigo == lacres[i].Codigo) {
            lacres.splice(i, 1);
            break;
        }
    }

    _cargaMDFeManual.ListaLacres.val(lacres);
    _gridLacresMDFeManual.CarregarGrid(_cargaMDFeManual.ListaLacres.val());
}

function AdicionarLacreClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_lacresMDFeManual);

    if (valido) {

        var lacres = _cargaMDFeManual.ListaLacres.val();

        for (var i = 0; i < lacres.length; i++) {
            if (_lacresMDFeManual.Numero.val() == lacres[i].Numero) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "O lacre informado (" + lacres[i].Numero + ") já existe.");
                return;
            }
        }

        _lacresMDFeManual.Codigo.val(guid());

        lacres.push(RetornarObjetoPesquisa(_lacresMDFeManual));

        _cargaMDFeManual.ListaLacres.val(lacres);
        _gridLacresMDFeManual.CarregarGrid(_cargaMDFeManual.ListaLacres.val());
        LimparCamposLacres();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposLacres() {
    LimparCampos(_lacresMDFeManual);
}
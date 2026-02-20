
//*******MAPEAMENTO KNOUCKOUT*******

var AlterarCentroResultado = function () {
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), required: true, multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.CentroResultadoAtual = PropertyEntity({ text: "Centro de Resultado Atual:", visible: ko.observable(true) });
    this.CentroResultadoNovo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado Novo:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
};


//*******EVENTOS*******

var CRUDAlterarCentroResultado = function () {
    this.Alterar = PropertyEntity({ eventClick: AlterarClick, type: types.event, text: "Alterar" });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        UrlImportacao: "AlterarCentroResultado/Importar",
        UrlConfiguracao: "AlterarCentroResultado/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O086_AlteracaoCentroResultado,
        CallbackImportacao: function () {}
    });
};

function loadPesquisaCentroResultado() {
    _alterarCentroResultado = new AlterarCentroResultado();
    _crudAlterarCentroResultado = new CRUDAlterarCentroResultado();

    KoBindings(_alterarCentroResultado, "knockoutAlterarCentroResultado");
    KoBindings(_crudAlterarCentroResultado, "knockoutCRUDAlterarCentroResultado");

    new BuscarCargas(_alterarCentroResultado.Carga, retornoCarga, null, null, null, null, null, null, null, null, null, null, null, null, [EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.EmTransporte]);
    new BuscarCentroResultado(_alterarCentroResultado.CentroResultadoNovo);
}

function AlterarClick() {
    Salvar(_alterarCentroResultado, "AlterarCentroResultado/AlterarCentroResultadoCarga", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Centro de resultado da carga alterado com sucesso!");
                LimparCampos(_alterarCentroResultado);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function PreencherCentroResultado(codigoCarga) {
    executarReST("AlterarCentroResultado/PesquisarCentroResultadoAtual", { CodigoCarga: codigoCarga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _alterarCentroResultado.CentroResultadoAtual.val(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function retornoCarga(carga) {
    _alterarCentroResultado.Carga.codEntity(carga.Codigo);
    _alterarCentroResultado.Carga.val(carga.CodigoCargaEmbarcador);
    
    PreencherCentroResultado(carga.Codigo);
}
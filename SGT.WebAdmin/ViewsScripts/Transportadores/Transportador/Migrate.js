/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


var _migrate;

var Migrate = function () {
    this.TokenMigrate = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TokenMigrate.getFieldDescription(), visible: ko.observable(false), required: ko.observable(true) });
    this.IntegracaoMigrateRegimeTributario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Transportador.IntegracaoMigrateRegimeTributario.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.EnviarObservacaoNaDiscriminacaoServicoMigrate = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.EnviarObservacaoNaDiscriminacaoServicoMigrate });

    this.PossuiIntegracaoMigrate = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: true, text: Localization.Resources.Transportadores.Transportador.PossuiIntegracaoMigrate});

    this.PossuiIntegracaoMigrate.val.subscribe(function (valor) {
        _migrate.TokenMigrate.visible(valor);
        _migrate.TokenMigrate.required(valor);

        _migrate.IntegracaoMigrateRegimeTributario.visible(valor);
        _migrate.IntegracaoMigrateRegimeTributario.required(valor);

        _migrate.EnviarObservacaoNaDiscriminacaoServicoMigrate.visible(valor);
    });
}


//*******EVENTOS*******
function LoadMigrate() {
    _migrate = new Migrate();
    KoBindings(_migrate, "knockoutMigrate");

    new BuscarIntegracaoMigrateRegimeTributario(_migrate.IntegracaoMigrateRegimeTributario);


}
function preencherMigrate(dados) {
    if (dados) {
        _migrate.PossuiIntegracaoMigrate.val(dados.PossuiIntegracaoMigrate);
        _migrate.TokenMigrate.val(dados.TokenMigrate);
        _migrate.EnviarObservacaoNaDiscriminacaoServicoMigrate.val(dados.EnviarObservacaoNaDiscriminacaoServicoMigrate);

    }
}
function LimparMigrate() {
    LimparCampos(_migrate);
}


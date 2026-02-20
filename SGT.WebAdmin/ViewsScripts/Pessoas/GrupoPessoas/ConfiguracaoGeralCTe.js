var _configuracaoGeralCTe;

var ConfiguracaoGeralCTe = function () {
    var self = this;

    this.NomeNomenclaturaArquivosDownloadCTe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NomenclaturaArquivosXMLePDF.getFieldDescription(), issue: 0, required: false, maxLength: 150, enable: ko.observable(true), visible: ko.observable(true) });

    this.TagNumeroCTe = PropertyEntity({ eventClick: function (e) { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#NumeroCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroDoCTe, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagSerieCTe = PropertyEntity({ eventClick: function (e) { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#SerieCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.SerieCTe, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagChaveCTe = PropertyEntity({ eventClick: function (e) { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#ChaveCTe"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.ChaveCTe, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCNPJEmissor = PropertyEntity({ eventClick: function () { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#CNPJEmissor"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CNPJEmissor, visible: ko.observable(true), enable: ko.observable(true) });
    this.TagCNPJTomador = PropertyEntity({ eventClick: function () { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#CNPJTomador"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.CNPJTomador, visible: ko.observable(true), enable: ko.observable(true) });
    this.TagNumeroBooking = PropertyEntity({ eventClick: function () { InserirTag(self.NomeNomenclaturaArquivosDownloadCTe.id, "#NumeroBooking"); }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.NumeroBooking, visible: ko.observable(true), enable: ko.observable(true) });

};

//*******EVENTOS*******

function LoadConfiguracaoGeralCTe() {

    _configuracaoGeralCTe = new ConfiguracaoGeralCTe();
    KoBindings(_configuracaoGeralCTe, "knockoutConfiguracaoGeralCTe");

    _grupoPessoas.NomeNomenclaturaArquivosDownloadCTe = _configuracaoGeralCTe.NomeNomenclaturaArquivosDownloadCTe;

}
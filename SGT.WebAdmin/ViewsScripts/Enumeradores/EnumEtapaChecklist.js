var EnumEtapaChecklistHelper = function () {
    this.Checklist = 0;
    this.AvaliacaoDescarga = 1;
};

EnumEtapaChecklistHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Checklist,
                text: "Checklist"
            }, {
                value: this.AvaliacaoDescarga,
                text: "Avaliação Descarga"
            }
        ];
    }
}

var EnumEtapaChecklist = Object.freeze(new EnumEtapaChecklistHelper());


const EnumModoValidacaoImagemEtapaChecklistSuperAppHelper = function () {
    this.Camera = "CAMERA";
    this.DocumentScanner = "DOCUMENT_SCANNER";
};

EnumModoValidacaoImagemEtapaChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Camera), value: this.Camera });
        opcoes.push({ text: this.obterDescricao(this.DocumentScanner), value: this.DocumentScanner });

        return opcoes;
    },
    obterOpcoesCadastroChecklists: function () {
        const opcoes = [];

        opcoes.push({ text: "Nenhum", value: "" });
        opcoes.push({ text: this.obterDescricao(this.Camera), value: this.Camera });
        opcoes.push({ text: this.obterDescricao(this.DocumentScanner), value: this.DocumentScanner });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Camera: return "Camera";
            case this.DocumentScanner: return "Scanner de documento (Máscara Dinâmica)";
            default: return "";
        }
    }
};

const EnumModoValidacaoImagemEtapaChecklistSuperApp = Object.freeze(new EnumModoValidacaoImagemEtapaChecklistSuperAppHelper());
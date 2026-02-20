

const EnumTipoEtapaChecklistSuperAppHelper = function () {
    this.Checkbox = 1;
    this.RadioGroup = 2;
    this.Combobox = 3;
    this.Date = 4;
    this.DateTime = 5;
    this.Number = 6;
    this.Range = 7;
    this.Text = 8;
    this.Image = 9;
    this.ImageValidator = 10;
    this.Video = 11;
    this.InfoCheck = 12;
    this.ScanCode = 13;
    this.Signature = 14;
    this.Terms = 15;
    this.Timer = 16;
    this.LoadDetail = 17;
    this.Document = 18;
    this.Location = 19;
    this.ItemValidator = 20;
};

EnumTipoEtapaChecklistSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        // opcoes.push({ text: this.obterDescricao(this.Checkbox), value: this.Checkbox });
        // opcoes.push({ text: this.obterDescricao(this.RadioGroup), value: this.RadioGroup });
        // opcoes.push({ text: this.obterDescricao(this.Combobox), value: this.Combobox });
        opcoes.push({ text: this.obterDescricao(this.Date), value: this.Date });
        opcoes.push({ text: this.obterDescricao(this.DateTime), value: this.DateTime });
        opcoes.push({ text: this.obterDescricao(this.Number), value: this.Number });
        // opcoes.push({ text: this.obterDescricao(this.Range), value: this.Range });
        opcoes.push({ text: this.obterDescricao(this.Text), value: this.Text });
        opcoes.push({ text: this.obterDescricao(this.Image), value: this.Image });
        opcoes.push({ text: this.obterDescricao(this.ImageValidator), value: this.ImageValidator });
        // opcoes.push({ text: this.obterDescricao(this.Video), value: this.Video });
        // opcoes.push({ text: this.obterDescricao(this.InfoCheck), value: this.InfoCheck });
        // opcoes.push({ text: this.obterDescricao(this.ScanCode), value: this.ScanCode });
        opcoes.push({ text: this.obterDescricao(this.Signature), value: this.Signature });
        // opcoes.push({ text: this.obterDescricao(this.Terms), value: this.Terms });
        opcoes.push({ text: this.obterDescricao(this.Timer), value: this.Timer });
        // opcoes.push({ text: this.obterDescricao(this.LoadDetail), value: this.LoadDetail });
        // opcoes.push({ text: this.obterDescricao(this.Document), value: this.Document });
        opcoes.push({ text: this.obterDescricao(this.Location), value: this.Location });
        // opcoes.push({ text: this.obterDescricao(this.ItemValidator), value: this.ItemValidator });

        return opcoes;
    },
    obterOpcoesCadastroChecklists: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        // opcoes.push({ text: this.obterDescricao(this.Checkbox), value: this.Checkbox });
        // opcoes.push({ text: this.obterDescricao(this.RadioGroup), value: this.RadioGroup });
        // opcoes.push({ text: this.obterDescricao(this.Combobox), value: this.Combobox });
        opcoes.push({ text: this.obterDescricao(this.Date), value: this.Date });
        opcoes.push({ text: this.obterDescricao(this.DateTime), value: this.DateTime });
        opcoes.push({ text: this.obterDescricao(this.Number), value: this.Number });
        // opcoes.push({ text: this.obterDescricao(this.Range), value: this.Range });
        opcoes.push({ text: this.obterDescricao(this.Text), value: this.Text });
        opcoes.push({ text: this.obterDescricao(this.Image), value: this.Image });
        opcoes.push({ text: this.obterDescricao(this.ImageValidator), value: this.ImageValidator });
        // opcoes.push({ text: this.obterDescricao(this.Video), value: this.Video });
        // opcoes.push({ text: this.obterDescricao(this.InfoCheck), value: this.InfoCheck });
        // opcoes.push({ text: this.obterDescricao(this.ScanCode), value: this.ScanCode });
        opcoes.push({ text: this.obterDescricao(this.Signature), value: this.Signature });
        // opcoes.push({ text: this.obterDescricao(this.Terms), value: this.Terms });
        opcoes.push({ text: this.obterDescricao(this.Timer), value: this.Timer });
        // opcoes.push({ text: this.obterDescricao(this.LoadDetail), value: this.LoadDetail });
        // opcoes.push({ text: this.obterDescricao(this.Document), value: this.Document });
        opcoes.push({ text: this.obterDescricao(this.Location), value: this.Location });
        // opcoes.push({ text: this.obterDescricao(this.ItemValidator), value: this.ItemValidator });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Checkbox: return "Caixa de Seleção";
            case this.RadioGroup: return "Grupo de Opções";
            case this.Combobox: return "Lista Suspensa";
            case this.Date: return "Data";
            case this.DateTime: return "Data e Hora";
            case this.Number: return "Número";
            case this.Range: return "Intervalo";
            case this.Text: return "Texto";
            case this.Image: return "Imagem";
            case this.ImageValidator: return "Imagem com Validação";
            case this.Video: return "Vídeo";
            case this.InfoCheck: return "Confirmação de Informação";
            case this.ScanCode: return "Leitura de Código";
            case this.Signature: return "Assinatura";
            case this.Terms: return "Termos de Aceite";
            case this.Timer: return "Temporizador";
            case this.LoadDetail: return "Detalhes da Carga";
            case this.Document: return "Documento";
            case this.Location: return "Localização";
            case this.ItemValidator: return "Validador de Item";
            default: return "";
        }
    }
};

const EnumTipoEtapaChecklistSuperApp = Object.freeze(new EnumTipoEtapaChecklistSuperAppHelper());
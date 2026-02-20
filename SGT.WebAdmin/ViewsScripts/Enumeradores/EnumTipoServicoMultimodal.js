var EnumTipoServicoMultimodalHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.Normal = 1;
    this.Subcontratacao = 2;
    this.RedespachoIntermediario = 3;
    this.VinculadoMultimodalTerceiro = 4;
    this.VinculadoMultimodalProprio = 5;
    this.Redespacho = 6;
};

EnumTipoServicoMultimodalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.TipoServicoMultimodal.Nenhum;
            case this.Normal: return Localization.Resources.Enumeradores.TipoServicoMultimodal.UmNormal;
            case this.Subcontratacao: return Localization.Resources.Enumeradores.TipoServicoMultimodal.DoisSubcontratacao;
            case this.RedespachoIntermediario: return Localization.Resources.Enumeradores.TipoServicoMultimodal.TresRedespachoIntermediario;
            case this.VinculadoMultimodalTerceiro: return Localization.Resources.Enumeradores.TipoServicoMultimodal.QuatroVinculadoMultimodalTerceiro;
            case this.VinculadoMultimodalProprio: return Localization.Resources.Enumeradores.TipoServicoMultimodal.CincoVinculadoMultimodalProprio;
            case this.Redespacho: return Localization.Resources.Enumeradores.TipoServicoMultimodal.SeisRedespacho;
            case this.Todos: return Localization.Resources.Enumeradores.TipoServicoMultimodal.Todos;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.UmNormal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.DoisSubcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.TresRedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.QuatroVinculadoMultimodalTerceiro, value: this.VinculadoMultimodalTerceiro },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.CincoVinculadoMultimodalProprio, value: this.VinculadoMultimodalProprio },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.SeisRedespacho, value: this.Redespacho }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.UmNormal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.DoisSubcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.TresRedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.QuatroVinculadoMultimodalTerceiro, value: this.VinculadoMultimodalTerceiro },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.CincoVinculadoMultimodalProprio, value: this.VinculadoMultimodalProprio },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.SeisRedespacho, value: this.Redespacho }
        ];
    },
    obterOpcoesSemNumero: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Subcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.RedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro, value: this.VinculadoMultimodalTerceiro },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio, value: this.VinculadoMultimodalProprio },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Redespacho, value: this.Redespacho }
        ];
    },
    obterOpcoesImpressaoLote: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Subcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.RedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro, value: this.VinculadoMultimodalTerceiro },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Redespacho, value: this.Redespacho }
        ];
    },
    obterOpcoesSVMTerceiro: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.Subcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro, value: this.VinculadoMultimodalTerceiro }
        ];
    }
};

var EnumTipoServicoMultimodal = Object.freeze(new EnumTipoServicoMultimodalHelper());
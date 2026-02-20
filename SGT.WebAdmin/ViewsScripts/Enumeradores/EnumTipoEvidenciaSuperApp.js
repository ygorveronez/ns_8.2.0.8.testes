

const EnumTipoEvidenciaSuperAppHelper = function () {
    this.FotoCanhotoComValidacao = 1;
    this.FotoCanhotoSemValidacao = 2;
    this.FotoAdicional = 3;
    this.FotoCanhotoComentario = 4;
    this.AssinaturaConfirmacao = 5;
    this.SolicitacaoPacotesColeta = 6;
    this.NomeRecebedorConfirmacao = 7;
    this.DocumentoConfirmacao = 8;
    this.FotoCanhotoPallet = 9;
    this.FotoCanhotoValePallet = 10;
    this.FotoCanhotoMercadoria = 11;
    this.DataRetroativa = 12;
    this.LocalizacaoCliente = 13;
    this.DetalheCarga = 14;
};

EnumTipoEvidenciaSuperAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoComValidacao), value: this.FotoCanhotoComValidacao});
        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoSemValidacao), value: this.FotoCanhotoSemValidacao});
        opcoes.push({ text: this.obterDescricao(this.FotoAdicional), value: this.FotoAdicional});
        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoComentario), value: this.FotoCanhotoComentario});
        opcoes.push({ text: this.obterDescricao(this.AssinaturaConfirmacao), value: this.AssinaturaConfirmacao});
        opcoes.push({ text: this.obterDescricao(this.SolicitacaoPacotesColeta), value: this.SolicitacaoPacotesColeta});
        opcoes.push({ text: this.obterDescricao(this.NomeRecebedorConfirmacao), value: this.NomeRecebedorConfirmacao});
        opcoes.push({ text: this.obterDescricao(this.DocumentoConfirmacao), value: this.DocumentoConfirmacao});
        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoPallet), value: this.FotoCanhotoPallet});
        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoValePallet), value: this.FotoCanhotoValePallet});
        opcoes.push({ text: this.obterDescricao(this.FotoCanhotoMercadoria), value: this.FotoCanhotoMercadoria });
        opcoes.push({ text: this.obterDescricao(this.DataRetroativa), value: this.DataRetroativa });
        opcoes.push({ text: this.obterDescricao(this.LocalizacaoCliente), value: this.LocalizacaoCliente });
        opcoes.push({ text: this.obterDescricao(this.DetalheCarga), value: this.DetalheCarga });

        return opcoes;
    },

    obterOpcoesPorTipoEtapaChecklist: function (tipoEtapaChecklist) {
        const opcoes = [];

        switch (tipoEtapaChecklist) {
            case EnumTipoEtapaChecklistSuperApp.Number:
                opcoes.push({ text: this.obterDescricao(this.SolicitacaoPacotesColeta), value: this.SolicitacaoPacotesColeta });
                break;
            case EnumTipoEtapaChecklistSuperApp.Text:
                opcoes.push({ text: this.obterDescricao(this.FotoCanhotoComentario), value: this.FotoCanhotoComentario });
                opcoes.push({ text: this.obterDescricao(this.NomeRecebedorConfirmacao), value: this.NomeRecebedorConfirmacao });
                opcoes.push({ text: this.obterDescricao(this.DocumentoConfirmacao), value: this.DocumentoConfirmacao });
                break;
            case EnumTipoEtapaChecklistSuperApp.Image:
                opcoes.push({ text: this.obterDescricao(this.FotoAdicional), value: this.FotoAdicional });
                opcoes.push({ text: this.obterDescricao(this.FotoCanhotoPallet), value: this.FotoCanhotoPallet });
                opcoes.push({ text: this.obterDescricao(this.FotoCanhotoValePallet), value: this.FotoCanhotoValePallet });
                break;
            case EnumTipoEtapaChecklistSuperApp.ImageValidator:
                opcoes.push({ text: this.obterDescricao(this.FotoCanhotoComValidacao), value: this.FotoCanhotoComValidacao });
                opcoes.push({ text: this.obterDescricao(this.FotoCanhotoSemValidacao), value: this.FotoCanhotoSemValidacao });
                break;
            case EnumTipoEtapaChecklistSuperApp.Signature:
                opcoes.push({ text: this.obterDescricao(this.AssinaturaConfirmacao), value: this.AssinaturaConfirmacao });
                break;
            case EnumTipoEtapaChecklistSuperApp.Date:
            case EnumTipoEtapaChecklistSuperApp.DateTime:
                opcoes.push({ text: this.obterDescricao(this.DataRetroativa), value: this.DataRetroativa });
                break;
            case EnumTipoEtapaChecklistSuperApp.Location:
                opcoes.push({ text: this.obterDescricao(this.LocalizacaoCliente), value: this.LocalizacaoCliente });
                break;
            case EnumTipoEtapaChecklistSuperApp.LoadDetail:
                opcoes.push({ text: this.obterDescricao(this.DetalheCarga), value: this.DetalheCarga });
            case EnumTipoEtapaChecklistSuperApp.Timer:
            default:
                break;
        }
        return opcoes;
    },

    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.FotoCanhotoComValidacao: return "Foto do Canhoto Com Validacao";
            case this.FotoCanhotoSemValidacao: return "Foto do Canhoto Sem Validacao";
            case this.FotoAdicional: return "Foto Adicional";
            case this.FotoCanhotoComentario: return "Observação da Foto do Canhoto";
            case this.AssinaturaConfirmacao: return "Assinatura do Recebedor";
            case this.NomeRecebedorConfirmacao: return "Nome do Recebedor";
            case this.DocumentoConfirmacao: return "Documento do Recebedor";
            case this.SolicitacaoPacotesColeta: return "Solicitação de Pacotes na Coleta";
            case this.FotoCanhotoPallet: return "Foto do Canhoto do tipo Pallet";
            case this.FotoCanhotoValePallet: return "Foto do Canhoto do tipo ValePallet";
            case this.DataRetroativa: return "Data Retroativa";
            case this.LocalizacaoCliente: return "Localização do Cliente";
            case this.DetalheCarga: return "Detalhe de carga";
            default: return "";
        }
    }
};

const EnumTipoEvidenciaSuperApp = Object.freeze(new EnumTipoEvidenciaSuperAppHelper());
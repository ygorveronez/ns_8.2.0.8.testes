var EnumMotivoInconsistenciaGestaoDocumentoHelper = function () {
    this.Todos = 0;
    this.SemCarga = 1;
    this.ValorFretePrestacao = 2;
    this.AliquotaICMS = 3;
    this.BaseCalculo = 4;
    this.ICMS = 5;
    this.CST = 6;
    this.CFOP = 7;
    this.Tomador = 8;
    this.Remetente = 9;
    this.Destinatario = 10;
    this.Expedidor = 11;
    this.Recebedor = 12;
    this.Origem = 13;
    this.Destino = 14;
    this.Emissor = 15;
    this.ValorTotalReceber = 16;
    this.TipoAmbiente = 17;
    this.TipoCTe = 18;
    this.CTeAnterior = 19;
    this.NotasFiscais = 20;
    this.EnvioPosteriorCarga = 21;
    this.CargaJaPossuiCTe = 22;
    this.ComponentesFreteDivergentes = 23;
    this.AprovacaoObrigatoria = 24;
};

EnumMotivoInconsistenciaGestaoDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Carga", value: this.SemCarga },
            { text: "Valor da Prestação", value: this.ValorFretePrestacao },
            { text: "Valor Total a Receber", value: this.ValorTotalReceber },
            { text: "Alíquota ICMS", value: this.AliquotaICMS },
            { text: "Base de Cálculo ICMS", value: this.BaseCalculo },
            { text: "Valor do ICMS", value: this.ICMS },
            { text: "CST", value: this.CST },
            { text: "CFOP", value: this.CFOP },
            { text: "Tomador", value: this.Tomador },
            { text: "Remetente", value: this.Remetente },
            { text: "Destinatário", value: this.Destinatario },
            { text: "Expedidor", value: this.Expedidor },
            { text: "Recebedor", value: this.Recebedor },
            { text: "Origem", value: this.Origem },
            { text: "Destino", value: this.Destino },
            { text: "Emissor", value: this.Emissor },
            { text: "Tipo do Ambiente", value: this.TipoAmbiente },
            { text: "Tipo do CT-e", value: this.TipoCTe },
            { text: "CT-e Anterior", value: this.CTeAnterior },
            { text: "Notas Fiscais", value: this.NotasFiscais },
            { text: "Envio Posterior Carga", value: this.EnvioPosteriorCarga },
            { text: "Carga já possui CTe", value: this.CargaJaPossuiCTe },
            { text: "Componentes de Frete Divergentes", value: this.ComponentesFreteDivergentes },
            { text: "Aprovação Obrigatória", value: this.AprovacaoObrigatoria }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumMotivoInconsistenciaGestaoDocumento = Object.freeze(new EnumMotivoInconsistenciaGestaoDocumentoHelper());
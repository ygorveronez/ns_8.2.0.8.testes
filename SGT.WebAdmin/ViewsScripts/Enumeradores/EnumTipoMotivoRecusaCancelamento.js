var EnumTipoMotivoRecusaCancelamentoHelper = function() {
    this.Todos = 0;
    this.Recusa = 1;
    this.Cancelamento = 2;
};

EnumTipoMotivoRecusaCancelamentoHelper.prototype = {
    obterOpcoes: function(){
        return [
            { text: "Todos", value: this.Todos },
            { text: "Recusa", value: this.Recusa },            
            { text: "Cancelamento", value: this.Cancelamento },
        ];
    },
    obterOpcoesCancelamento: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Cancelamento", value: this.Cancelamento },
        ];
    },
    obterOpcoesRecusa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Recusa", value: this.Recusa },       
        ];
    },
    obterOpcoesPesquisa: function()
{
    return this.obterOpcoes();
}};

var EnumTipoMotivoRecusaCancelamento = Object.freeze(new EnumTipoMotivoRecusaCancelamentoHelper());
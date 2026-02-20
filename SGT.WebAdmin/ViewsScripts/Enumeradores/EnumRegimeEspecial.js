var EnumRegimeEspecialHelper = function () {
    this.Nenhum = 0;
    this.MicroempresaMunicipal = 1;
    this.Estimativa = 2;
    this.SociedadeProfissionais = 3;
    this.Cooperativa = 4;
    this.MicroempresarioIndividual = 5;
    this.MicroempresarioEmpresaPP = 6;
    this.LucroReal = 7;
    this.LucroPresumido = 8;
    this.SimplesNacional = 9;
    this.Imune = 10;
    this.EmpresaIndividualEireli = 11;
    this.EmpresaPP = 12;
    this.MicroEmpresario = 13;
    this.Outros = 14;
    this.MovimentoMensal = 15;
    this.ISSQNAutonomos = 16;
    this.ISSQNSociedade = 17;
    this.NotarioRegistrador = 18;
    this.TribFaturamentoVariavel = 19;
    this.Fixo = 20;
    this.Isencao = 21;
    this.ExigibilidadeSuspensaoJudicial = 22;
    this.ExigibilidadeSuspensaAdm = 23;

};

EnumRegimeEspecialHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Microempresa Municipal", value: this.MicroempresaMunicipal },
            { text: "Estimativa", value: this.Estimativa },
            { text: "Sociedade Profissionais", value: this.SociedadeProfissionais },
            { text: "Cooperativa", value: this.Cooperativa },
            { text: "Microempresario Individual", value: this.MicroempresarioIndividual },
            { text: "Microempresario Empresa P.P.", value: this.MicroempresarioEmpresaPP },
            { text: "Lucro Real", value: this.LucroReal },
            { text: "Lucro Presumido", value: this.LucroPresumido },
            { text: "Simples Nacional", value: this.SimplesNacional },
            { text: "Imune", value: this.Imune },
            { text: "Empresa Individual Eireli", value: this.EmpresaIndividualEireli },
            { text: "Empresa PP", value: this.EmpresaPP },
            { text: "Micro Empresário", value: this.MicroEmpresario },
            { text: "Outros", value: this.Outros },
            { text: "Movimento Mensal", value: this.MovimentoMensal },
            { text: "ISSQN Autonomos", value: this.ISSQNAutonomos },
            { text: "ISSQN Sociedade", value: this.ISSQNSociedade },
            { text: "Notario Registrador", value: this.NotarioRegistrador },
            { text: "Trib. Faturamento Variável", value: this.TribFaturamentoVariavel },
            { text: "Fixo", value: this.Fixo },
            { text: "Isencao", value: this.Isencao },
            { text: "Exigibilidade Suspensão Judicial", value: this.ExigibilidadeSuspensaoJudicial },
            { text: "Exigibilidade Suspensa Adm", value: this.ExigibilidadeSuspensaAdm }

        ];
    },

    obterOpcoesNaoSelecionado: function () {
        return [{ text: "Nenhum", value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumRegimeEspecial = Object.freeze(new EnumRegimeEspecialHelper());
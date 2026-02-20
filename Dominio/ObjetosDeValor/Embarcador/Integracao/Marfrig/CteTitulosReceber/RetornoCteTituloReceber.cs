namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber
{
    public class RetornoCteTituloReceber
    {
        public int status { get; set; }
        public string mensagem { get; set; }
        public string qtdPaginas { get; set; }
        public string qtdRegistros { get; set; }
        public RetornoCteTitulo[] titulos { get; set; }

    }
    public class RetornoCteTitulo
    {
        public string cpfCnpj { get; set; }
        public string contratoFatImp { get; set; }
        public string contratoFatExp { get; set; }
        public string vencSemDesconto { get; set; }
        public string depositoOuBoleto { get; set; }
        public string baixa { get; set; }
        public string faturamentoPref { get; set; }
        public string filial { get; set; }
        public string parcela { get; set; }
        public string[] emissao { get; set; }
        public string empresa { get; set; }
        public bool envBanco { get; set; }
        public bool existeTipoValor { get; set; }
        public string fornecedor { get; set; }
        public string historico { get; set; }
        public string idCnab { get; set; }
        public string loja { get; set; }
        public string nossoNumeroAp { get; set; }
        public RetornoNotasfiscais[] notasFiscais { get; set; }
        public string numeroProcesso { get; set; }
        public string numeroTitulo { get; set; }
        public string origem { get; set; }
        public string pedido { get; set; }
        public string polo { get; set; }
        public string serie { get; set; }
        public string tipo { get; set; }
        public string vencimento { get; set; }
        public float valorLiquido { get; set; }
        public string idPortal { get; set; }
        public bool adiantamentoDevolucao { get; set; }
        public bool acrescimoDecrescimo { get; set; }
        public string acrescimo { get; set; }
        public string decrescimo { get; set; }
        public string agenciaFavorecido { get; set; }
        public string bancoFavorecido { get; set; }
        public string contaFavorecido { get; set; }
        public string dvAgenciaFavorecido { get; set; }
        public string dvContaFavorecido { get; set; }
        public string idIntegracao { get; set; }
        public string chaveFatura { get; set; }
        public string idMulti { get; set; }

    }

    public class RetornoNotasfiscais
    {
        public string numero { get; set; }
        public float valor { get; set; }
        public string dataEmissao { get; set; }

        public string chaveDocumento { get; set; }
    }
}

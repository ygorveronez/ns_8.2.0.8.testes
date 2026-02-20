using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro
{
    public class TituloFinanceiroEnvio
    {
        public string InscricaoEmpresa { get; set; }
        public string TipoDocumento { get; set; }
        public string Emissao { get; set; }
        public int NumeroParcela { get; set; }
        public string Serie { get; set; }
        public string Vencimento { get; set; }
        public string Sistema { get; set; }
        public string Usuario { get; set; }
        public string Entrada { get; set; }
        public string InscricaoFornecedor { get; set; }
        public int CodigoInternoFornecedor { get; set; }
        public string NumeroDocumento { get; set; }
        public string CodigoModelo { get; set; }
        public decimal IdEscrituracaoFiscal { get; set; }
        public decimal ValorCSL { get; set; }
        public decimal ValorIRRF { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorSestSenat { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal Desconto { get; set; }
        public string Observacao { get; set; }
        public bool IntegrarContabilidade { get; set; }
        public string TipoPagamento { get; set; }
        public List<DespesaTitulo> Despesas { get; set; }
        public ValoresComplementares ValoresComplementares { get; set; }
    }

    public class DespesaTitulo
    {
        public string Despesa { get; set; }
        public decimal Valor { get; set; }
        public int CustoFinanceiro { get; set; }
        public int PlanoContabil { get; set; }
        public int ContaContabil { get; set; }
        public int CustoContabil { get; set; }
        public string Observacao { get; set; }
    }

    public class ValoresComplementares
    {
        public int Grupo { get; set; }
        public List<ValorComplementar> Valores { get; set; }
    }

    public class ValorComplementar
    {
        public int Codigo { get; set; }
        public decimal Valor { get; set; }
    }
}

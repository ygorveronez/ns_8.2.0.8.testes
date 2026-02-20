using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.DocumentoEntrada
{
    public class DocumentoNFEnvio
    {
        public string InscricaoEmpresa { get; set; }
        public string InscricaoParticipante { get; set; }
        public string NumeroDocumento { get; set; }
        public string CodigoTipoDocumento { get; set; }
        public string Serie { get; set; }
        public string Usuario { get; set; }
        public string DataEmissao { get; set; }
        public string DataEntradaSaida { get; set; }
        public string DataVencimento { get; set; }
        public string Observacao { get; set; }
        public int CodigoIbgeMunicipio { get; set; }
        public bool IntegrarContabilidade { get; set; }
        public bool IntegrarFinanceiro { get; set; }
        public bool ISSRetido { get; set; }
        public decimal ISSBaseCalculo { get; set; }
        public decimal ISSValor { get; set; }
        public List<Item> Itens { get; set; }
    }

    public class Item
    {
        public string CodigoServico { get; set; }
        public string ContaPlanoFinanceiro { get; set; }
        public decimal Valor { get; set; }
        public int ContaContabil { get; set; }
        public int CustoContabil { get; set; }
        public ValorComplementar ValoresComplementares { get; set; }


    }

    public class ValorComplementar
    {
        public int? Grupo { get; set; }
       // public List<ValorUnico> Valores { get; set; }

    }

    public class ValorUnico
    {
        public int Codigo { get; set; }
        public decimal BaseCalculo { get; set; }
        public decimal Valor { get; set; }
    }

}

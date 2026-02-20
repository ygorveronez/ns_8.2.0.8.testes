using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Fatura
{
    public class FaturaEnvio
    {
        public string InscricaoCliente { get; set; }
        public int CodigoInternoCliente { get; set; }
        public string CodigoTipoDocumento { get; set; }
        public string Serie { get; set; }
        public string NumeroDocumento { get; set; }
        public string Emissao { get; set; }
        public string Vencimento { get; set; }
        public string Observacao { get; set; }
        public string Usuario { get; set; }
        public bool IntegrarContabilidade { get; set; }
        public int CodigoHistoricoContabil { get; set; }
        public ItemDocumento ItemDocumento { get; set; }
        public List<DocumentoSubstituido> DocumentoSubstituido { get; set; }
    }

    public class ItemDocumento
    {
        public string CodigoTipoReceita { get; set; }
        public string Observacao { get; set; }
    }

    public class DocumentoSubstituido
    {
        public string Codigo { get; set; }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class GrupoPessoa
    {
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoEmissaoCTeDocumentosExclusivo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula ParametroRateioFormulaExclusivo { get; set; }
        public bool ExigirNumeroControleCliente { get; set; }
        public bool ExigirNumeroNumeroReferenciaCliente { get; set; }
        public List<RaizCNPJ> RaizCNPJ { get; set; }
        public bool InativarCadastro { get; set; }
        public List<CNPJsDoGrupo> CNPJsDoGrupo { get; set; }
        public string CodigoDocumento { get; set; }
    }

    public class RaizCNPJ
    {
        public string Raiz { get; set; }
    }

    public class CNPJsDoGrupo
    {
        public string CNPJ { get; set; }
    }
}

using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class ManifestoEletronicoDocumentosFiscais
    {
        public string Chave { get; set; }

        public int Numero { get; set; }

        public DateTime? DataEmissao { get; set; }

        public DateTime? DataAutorizacao { get; set; }
    }
}

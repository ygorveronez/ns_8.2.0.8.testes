using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus
{
    public class DocumentoCTeFaturaIntegracao
    {
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string ChaveCTe { get; set; }
        public DateTime? DataEmissaoCTe { get; set; }
        public decimal ValorFrete { get; set; }
    }
}
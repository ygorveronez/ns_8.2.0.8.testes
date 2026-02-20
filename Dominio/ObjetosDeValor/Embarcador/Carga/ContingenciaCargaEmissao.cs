using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaContingenciaEmissao
    {
        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public bool ContingenciaEmissao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public SituacaoCarga Situacao { get; set; }
        public string TipoOperacao { get; set; }
        public string CnpjTransportador { get; set; }
        public string RazaoTransportador { get; set; }
        public string Filial { get; set; }
    }

}
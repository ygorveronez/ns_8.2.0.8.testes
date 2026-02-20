using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class AdicionaProvisao
    {
        public int Carga { get; set; }
        public int Filial { get; set; }
        public int Ocorrencia { get; set; }
        public int RegraEscrituracao { get; set; }
        public int TipoOperacao { get; set; }
        public double Tomador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao TipoLocalPrestacao { get; set; }
        public List<int> CodigosTransportadores { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao TipoProvisao { get; set; }
        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> DocumentosProvisoes { get; set; }
        public bool GerarLoteIndividualPorNfe { get; set; }
    }
}

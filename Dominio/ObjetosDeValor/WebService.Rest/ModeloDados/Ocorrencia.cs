using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Ocorrencia
    {
        public int Protocolo { get; set; }

        public int ProtocoloCarga { get; set; }

        public List<int> ProtocolosChamados { get; set; }

        public int NumeroOcorrencia { get; set; }

        public DateTime DataCriacao { get; set; }

        public decimal Valor { get; set; }

        public TipoOcorrencia Tipo {  get; set; }

        public List<ConhecimentoTransporteEletronico> Ctes { get; set; }

        public List<ConhecimentoTransporteEletronico> CtesComplemento { get; set; }
    }
}

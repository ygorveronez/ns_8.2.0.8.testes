using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Ocorrencia
{
    public class OcorrenciaIntegracaoMulti
    {
        public int Protocolo { get; set; }

        public int NumeroOcorrencia { get; set; }

        public string Descricao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia TipoOcorrencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public int ProtocoloCarga { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public decimal ValorOcorrencia { get; set; }
        public string Observacao { get; set; }
        public DateTime DataOcorrencia { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> Conhecimentos { get; set; }
    }
}

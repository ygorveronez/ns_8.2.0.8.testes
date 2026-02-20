using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Ocorrencia
{
    public class Ocorrencia
    {
        public int Protocolo;
        public string CodigoIntegracao { get; set; }
        public string Descricao;
        public int ProtocoloCarga { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int ProtocoloOcorrencia { get; set; }
        public string CPFResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public List<Pedido> Pedidos { get; set; }
    }
}

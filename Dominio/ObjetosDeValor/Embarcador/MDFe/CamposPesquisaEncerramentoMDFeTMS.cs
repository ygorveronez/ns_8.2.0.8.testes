using System;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe
{
    public class CamposPesquisaEncerramentoMDFeTMS
    {
        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? MaiorDataEmissao { get; set; }
        public string LocalidadeOrigem { get; set; }
        public string LocalidadeDestino { get; set; }
        public string Carga { get; set; }
        public string Motorista { get; set; }
    }
}

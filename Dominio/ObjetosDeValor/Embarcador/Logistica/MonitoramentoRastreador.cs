using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoRastreador
    {
        public long CodigoPosicao { get; set; }
        public string Tecnologia { get; set; }
        public string Descricao { get; set; }

        public DateTime DataUltimaPosicaoProcessada { get; set; }
        public DateTime DataUltimaPosicaoRecebida { get; set; }

        public Enumeradores.EnumTecnologiaRastreador Rastreador { get; set; }
        public Enumeradores.EnumTecnologiaGerenciadora Gerenciadora { get; set; }
    }
}

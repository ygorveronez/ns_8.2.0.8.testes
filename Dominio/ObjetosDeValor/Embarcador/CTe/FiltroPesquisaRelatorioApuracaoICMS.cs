using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioApuracaoICMS
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public double CnpjCpfRemetente { get; set; }
        public double CnpjCpfDestinatario { get; set; }
        public double CnpjCpfTomador { get; set; }
        public double CnpjCpfRecebedor { get; set; }
        public double CnpjCpfExpedidor { get; set; }
    }
}

using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioAFRMMControlMercante
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoViagem { get; set; }
    }
}

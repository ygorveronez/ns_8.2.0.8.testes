using System;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class DataConfirmacaoCanhoto
    {
        public int CodigoCargaEntrega { get; set; }
        public int CodigoCanhoto { get; set; }
        public DateTime DataConfirmacao { get; set; }

        public string DataConfirmacaoFormatada
        {
            get { return DataConfirmacao.ToString("g"); }
        }
    }
}

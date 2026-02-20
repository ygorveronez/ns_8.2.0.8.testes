using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaImportacaoPedagio
    {
        public int CodigoUsuario { get; set; }
        public string Planilha { get; set; }
        public DateTime? DataImportacaoInicial { get; set; }
        public DateTime? DataImportacaoFinal { get; set; }
        public Enumeradores.SituacaoImportacaoPedagio Situacao { get; set; }
        public string Mensagem { get; set; }
    }
}

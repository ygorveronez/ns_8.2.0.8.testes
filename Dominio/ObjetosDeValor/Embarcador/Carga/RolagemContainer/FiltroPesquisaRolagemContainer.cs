using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer
{
    public sealed class FiltroPesquisaRolagemContainer
    {
        public int CodigoUsuario { get; set; }
        public string Planilha { get; set; }
        public DateTime? DataImportacaoInicial { get; set; }
        public DateTime? DataImportacaoFinal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }
        public string Mensagem { get; set; }
        public bool? EntidadePai { get; set; }
    }
}

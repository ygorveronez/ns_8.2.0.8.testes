using System;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class FiltroPesquisaHistoricoMovimentacaoCanhoto
    {
        public DateTime DataUpload { get; set; }
        public DateTime DataAprovacao { get; set; }
        public DateTime DataRejeicao { get; set; }
        public DateTime DataReversao { get; set; }
        public DateTime DataRecebimentoFisico { get; set; }
        public DateTime DataConfirmacaoEntrega { get; set; }
        public int Usuario { get; set; }
        public int MotivoRejeicao { get; set; }
        public string CodigoEmitente { get; set; }
        public int NumeroCanhoto { get; set; }

    }
}

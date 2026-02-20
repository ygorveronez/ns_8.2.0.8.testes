using System;

namespace Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil
{
    public class ConfiguracaoCentroResultado
    {
        public long Codigo { get; set; }
        private double Remetente { get; set; }
        private double Destinatario { get; set; }
        private double Tomador { get; set; }
        public string Transportador { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoOcorrencia { get; set; }
        public string GrupoProduto { get; set; }
        public string RotaFrete { get; set; }
        private bool Situacao { get; set; }
        public string CentroResultadoContabilizacao { get; set; }
        public string CentroResultadoEscrituracao { get; set; }
        public string CentroResultadoICMS { get; set; }
        public string CentroResultadoPIS { get; set; }
        public string CentroResultadoCOFINS { get; set; }
        public string ItemServico { get; set; }

        public virtual string RemetenteFormatado
        {
            get
            {
                return Remetente > 0 ? Remetente.ToString().ObterCpfOuCnpjFormatado() : string.Empty;
            }
        }

        public virtual string DestinatarioFormatado
        {
            get
            {
                return Destinatario > 0 ? Destinatario.ToString().ObterCpfOuCnpjFormatado() : string.Empty;
            }
        }

        public virtual string TomadorFormatado
        {
            get
            {
                return Tomador > 0 ? Tomador.ToString().ObterCpfOuCnpjFormatado() : string.Empty;
            }
        }

        public virtual string SituacaoDescricao
        {
            get
            {
                return Situacao.ObterDescricaoAtivo();
            }
        }
    }
}

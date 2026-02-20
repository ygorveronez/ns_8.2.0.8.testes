using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioOcorrenciasCTe
    {
        public int Codigo { get; set; }

        public int CodigoCTe { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public string CPFCNPJRemetente { get; set; }

        public string NomeRemetente { get; set; }

        public string CPFCNPJDestinatario { get; set; }

        public string NomeDestinatario { get; set; }

        public string NumeroNF { get; set; }

        public DateTime DataOcorrencia { get; set; }

        public string TipoOcorrencia { get; set; }

        public string Ocorrencia { get; set; }

        public string Observacao { get; set; }
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaCancelamentoProvisao
    {
        public int Transportador { get; set; }
        public int Filial { get; set; }
        public int Carga { get; set; }
        public int Ocorrencia { get; set; }
        public int Numero { get; set; }
        public int NumeroDoc { get; set; }
        public string NumeroFolha { get; set; }
        public int LocalPrestacao { get; set; }
        public double Tomador { get; set; }
        public SituacaoCancelamentoProvisao Situacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public bool? CancelamentoProvisaoContraPartida { get; set; }
    }
}

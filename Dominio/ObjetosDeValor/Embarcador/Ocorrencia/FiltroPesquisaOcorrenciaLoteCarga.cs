using System;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class FiltroPesquisaOcorrenciaLoteCarga
    {
        public DateTime DataCriacaoInicial { get; set; }
        public DateTime DataCriacaoFinal { get; set; }
        public int CodigoOcorrenciaLote { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public double CnpjCpfRemetente { get; set; }
    }
}

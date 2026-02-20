using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas
{
    public sealed class FiltroPesquisaPagamentoMotoristaTMS
    {
        public int NumeroPagamento { get; set; }
        public int NumeroCarga { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataEfetivacaoInicial { get; set; }
        public DateTime? DataEfetivacaoFinal { get; set; }
        public Enumeradores.SituacaoPagamentoMotorista? Situacao { get; set; }
        public Enumeradores.EtapaPagamentoMotorista? Etapa { get; set; }
        public int CodigoOperador { get; set; }
        public List<int> CodigosTipoPagamento { get; set; }
        public int CodigoMotorista { get; set; }
        public bool PagamentosSemAcertoViagem { get; set; }
        public double CpfCnpjFavorecido { get; set; }
        public int NumeroDocumento { get; set; }
        public double Tomador { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioDespesaAcertoViagem
    {
        public int CodigoMotorista { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public int CodigoVeiculoTracao { get; set; }
        public int CodigoVeiculoReboque { get; set; }
        public int CodigoAcertoViagem { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoJustificativa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Enumeradores.SituacaoAcertoViagem Situacao { get; set; }
        public List<int> CodigoPais { get; set; }
    }
}

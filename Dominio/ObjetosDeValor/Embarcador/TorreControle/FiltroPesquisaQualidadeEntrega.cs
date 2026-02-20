using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaQualidadeEntrega
    {
        public List<int> Filiais { get; set; }
        public string Carga { get; set; }
        public int NumeroNF { get; set; }
        public DateTime DataInicioCriacaoCarga { get; set; }
        public DateTime DataFimCriacaoCarga { get; set; }
        public DateTime DataInicioEmissaoNF { get; set; }
        public DateTime DataFimEmissaoNF { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public bool? DisponivelParaConsulta { get; set; }
    }
}

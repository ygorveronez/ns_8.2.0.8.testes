using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga
{
    public sealed class FiltroPesquisaComprovanteCarga
    {
        public int Codigo { get; set; }
        public int Carga { get; set; }
        public SituacaoComprovanteCarga? Situacao { get; set; }
        public int TipoComprovante { get; set; }
        public List<int> MotoristaCarga { get; set; }
        public List<int> VeiculosCarga { get; set; }
        public DateTime DataCarga { get; set; }
    }
}

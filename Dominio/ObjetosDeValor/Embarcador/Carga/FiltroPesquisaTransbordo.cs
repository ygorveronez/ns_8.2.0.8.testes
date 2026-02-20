using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaTransbordo
    {
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public int LocalidadeTransbordo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo SituacaoTransbordo { get; set; }
        public int NumeroTransbordo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class LoteEscrituracaoMiroFiltro
    {
        public int Carga { get; set; }
        public int NumeroLote { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Chave { get; set; }
        public int Empresa { get; set; }
        public SituacaoLoteEscrituracaoMiro Situacao { get; set; }
    }
}

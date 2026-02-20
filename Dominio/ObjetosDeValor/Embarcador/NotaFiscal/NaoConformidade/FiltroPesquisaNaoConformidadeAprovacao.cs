using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class FiltroPesquisaNaoConformidadeAprovacao
    {
        public int CodigoUsuario { get; set; }

        public int NumeroNotaFiscal { get; set; }
        public DateTime? DataInicialGeracaoNC { get; set; }
        public DateTime? DataFinalGeracaoNC { get; set; }
        public DateTime? DataFinalEmissaoNotaFiscal { get; set; }
        public DateTime? DataInicialEmissaoNotaFiscal { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int Transportador { get; set; }
        public string Filial { get; set; }
        public double Destino { get; set; }
        public string ItemNC { get; set; }
        public List<int> NumeroNotas { get; set; }
        public List<string> Motorista { get; set; }
        public double Fornecedor { get; set; }
        public string NumeroOrdem { get; set; }
        public Enumeradores.SituacaoNaoConformidade? Situacao { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class EntregaDetalhes
    {
        public string Cliente { get; set; }
        public double ClienteCPFCNPJ { get; set; }
        public int SequenciaPrevista { get; set; }
        public int SequenciaRealizada { get; set; }
        public string EnderecoEntrega { get; set; }
        public string LocalidadeEntrega { get; set; }
        public DateTime? DataEntrega { get; set; }
        public DateTime? DataEntradaRaio { get; set; }
        public DateTime? DataSaidaRaio { get; set; }
        public decimal Peso { get; set; }
        public string NotasFiscais { get; set; }
        public List<string> ChavesNotas { get; set; }
        public string DescricaoSituacao { get; set; }
        public DateTime? DataInicioEntrega { get; set; }
        public DateTime? DataFimEntrega { get; set; }
        public string TempoEntrega { get; set; }
        public string JustificativaEntregaForaRaio { get; set; }
        public string CPFRecebedor { get; set; }
        public string NomeRecebedor { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public string ObservacaoEntrega { get; set; }
        public bool Coleta { get; set; }
        public string CodigoIntegracaoCliente { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public List<string> DataEmissaoNota { get; set; }
        public DateTime? DataRejeicaoEntrega { get; set; }
    }
}

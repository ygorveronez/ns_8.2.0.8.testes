using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class DespesaAcertoViagem
    {
        public DateTime Data { get; set; }
        public string Fornecedor { get; set; }
        public int NumeroAcerto { get; set; }
        public string Observacao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Placa { get; set; }
        public DateTime DataAcerto { get; set; }
        public DateTime DataInicialAcerto { get; set; }
        public DateTime DataFinalAcerto { get; set; }
        public string Situacao { get; set; }
        public string Motorista { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Justificativa { get; set; }
        public int? MoedaCotacaoBancoCentral { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public decimal ValorOriginalMoedaEstrangeira { get; set; }
        public string Moeda { get; set; }
        public string PaisFornecedor { get; set; }
        //{
        //    get { return !MoedaCotacaoBancoCentral.HasValue ? "Reais" : ((Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral)MoedaCotacaoBancoCentral.Value).ObterDescricao(); }
        //    set;
        //}
    }
}

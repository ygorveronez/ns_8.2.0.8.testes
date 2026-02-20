using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Produto
    {
        public int Protocolo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }

        // Mudado para dynamic para garantir que strings vazias não quebrem a requisição. Pode voltar a ser 
        // um decimal quando os erros não acontecerem mais. 07/07/2021
        public dynamic Quantidade { get; set; }
        public int? QuantidadeCaixa { get; set; }
        public int? QuantidadeCaixasVazias { get; set; }
        public decimal QuantidadePlanejada { get; set; }
        public decimal QuantidadeDevolucao { get; set; }
        public decimal Temperatura { get; set; }
        public bool InformarTemperatura { get; set; }
        public bool ObrigatorioGuiaTransporteAnimal { get; set; }
        public bool ObrigatorioNFProdutor { get; set; }
        public bool InformarDadosColeta { get; set; }
        public int motivoTemperatura { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota nota { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida UnidadeDeMedida { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade> ProdutoDivisoesCapacidade { get; set; }
        public decimal PesoUnitario { get; set; }
        public int? Imuno { get; set; }
        public int? ImunoRealizado { get; set; }
        public int? QuantidadePorCaixaRealizada { get; set; }
        public int? QuantidadeCaixasVaziasRealizada { get; set; }
        public string Lote { get; set; }
        public DateTime? DataCritica { get; set; }
        public decimal ValorDevolucao { get; set; }
        public int NFDevolucao { get; set; }
        public int CodigoMotivoDaDevolucao { get; set; }
    }
}

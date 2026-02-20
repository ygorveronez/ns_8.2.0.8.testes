using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class TituloQuitado
    {
        public int Codigo { get; set; }
        public DateTime DataEmissao { get; set; }
        public Pessoas.Empresa Empresa { get; set; }
        public string NumeroDocumento { get; set; }
        public string ObservacaoMovimento { get; set; }
        public string Atribuicao { get; set; }
        public string Conta { get; set; }
        public Pessoas.Pessoa Pessoa { get; set; }
        public TipoTitulo TipoTitulo { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorPago { get; set; }
        public string ObservacaoBaixa { get; set; }
        public PlanoDeConta PlanoRecebimento { get; set; }
        public List<ObjetosDeValor.WebService.CTe.CTe> CTes { get; set; }
    }
}

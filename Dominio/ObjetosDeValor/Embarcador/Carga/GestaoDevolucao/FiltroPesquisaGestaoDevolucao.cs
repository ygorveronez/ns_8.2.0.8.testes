using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao
{
    public class FiltroPesquisaGestaoDevolucao
    {
        public string Carga { get; set; }
        public string CargaDevolucao { get; set; }
        public int NFOrigem { get; set; }
        public int NFDevolucao { get; set; }
        public int CodigoNF { get; set; }
        public double Cliente { get; set; }
        public List<int> Transportadores { get; set; }
        public List<int> Filiais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao? OrigemRecebimento { get; set; }
        public TipoFluxoGestaoDevolucao? TipoFluxoGestaoDevolucao { get; set; }
        public bool? DevolucaoGerada { get; set; }
        public DateTime DataEmissaoNFInicial { get; set; }
        public DateTime DataEmissaoNFFinal { get; set; }
        public TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public List<TipoNotasGestaoDevolucao> TipoNotasGestaoDevolucao { get; set; }
        public List<TipoNotaFiscalIntegrada> TipoNotasFiscais { get; set; }
        public List<EtapaGestaoDevolucao> Etapas { get; set; } = new List<EtapaGestaoDevolucao>();
        public List<SituacaoGestaoDevolucao> SituacaoDevolucao { get; set; }
        public string EscritorioVendas { get; set; }
        public string EquipeVendas { get; set; }
        public List<TipoGestaoDevolucao> TipoGestaoDevolucao { get; set; }
    }
}
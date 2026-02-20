using Dominio.ObjetosDeValor.WebService.Pedido;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao
{
    public class ItemGridGestaoDevolucao
    {
        public string Codigo { get; set; }
        public string OrigemRecebimento { get; set; }
        public string OrigemGeracao { get; set; }
        public string NFOrigem { get; set; }
        public string NFDevolucao { get; set; }
        public string DataEmissaoNFDevolucao { get; set; }
        public string Transportador { get; set; }
        public string CargaOrigem { get; set; }
        public string CargaDevolucao { get; set; }
        public string Filial { get; set; }
        public string Tomadores { get; set; }
        public string Aprovado { get; set; }
        public string Laudo { get; set; }
        public string TipoDevolucaoDescricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao TipoDevolucao { get; set; }
        public string TipoFluxoGestaoDevolucaoDescricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao TipoFluxoGestaoDevolucao { get; set; }
        public string PosEntrega { get; set; }
        public string ComPendenciaFinanceira { get; set; }
        public string Atendimentos { get; set; }
        public string Etapas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao MovimentarEtapa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao EtapaAtual { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDevolucao SituacaoDevolucao { get; set; }
        public string TipoNotasDevolucao { get; set; }
        public string EtapaAtualDescricao { get; set; }
        public string SituacaoDevolucaoDescricao { get; set; }
        public string PrazoEscolhaTipoDevolucao { get; set; }
        public string ControleFinalizacaoDevolucaoDescricao { get; set; }
        public string Valor { get; set; }
        public string StatusAtendimento { get; set; }
        public string MotivoAtendimento { get; set; }
        public string DataAtendimento { get; set; }
        public string Ordem { get; set; }
        public string Remessa { get; set; }
        public long Volume { get; set; }
        public string DataAgendamento { get; set; }
        public string NLaudo { get; set; }
        public string DataNFD { get; set; }
        public string DataCanhoto { get; set; }
        public string ClienteCPFCNPJ { get; set; }
        public string ClienteNome { get; set; }
        public string EscritorioVendas { get; set; }
        public string EquipeVendas { get; set; }
        public string DocContabil { get; set; }
        public string TipoRecusa { get; set; }
        public string Aprovacao { get; set; }
        public string Custo { get; set; }
        public string DT_RowId
        {
            get
            {
                return Codigo.ToString();
            }
        }
        public bool DT_Enable
        {
            get
            {
                return true;
            }
        }
        public string DT_FontColor { get; set; }
        public string DT_RowClass { get; set; }
        public string DT_RowColor { get; set; }
    }
    public class ItemGridGestaoDevolucaoEtapa
    {
        public int Etapa { get; set; }
        public string Descricao { get; set; }
        public int SituacaoEtapa { get; set; }
        public int Ordem { get; set; }
        public string Observacao { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
    }
}


using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRotaFrete
    {
        public FiltroPesquisaRotaFrete()
        {
            CodigosDestinatario = new List<double>();
        }

        public string FilialDistribuidora { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public bool SomenteGrupo { get; set; }
        public double Remetente { get; set; }
        public List<double> CodigosDestinatario { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public List<int> CodigosCidadeRemetente { get; set; }
        public List<int> CodigosCidadeDestinatario { get; set; }
        public int CodigoTransportador { get; set; }
        public bool? RotaExclusivaCompraValePedagio { get; set; }
        public int CEPDestino { get; set; }
        public SituacaoRoteirizacao SituacaoRoteirizacao { get; set; }
        public int CodigoTipoOperacao { get; set; }
    }
}

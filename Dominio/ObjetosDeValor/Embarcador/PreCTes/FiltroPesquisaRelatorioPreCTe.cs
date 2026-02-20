using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ObjetosDeValor.Embarcador.PreCTes
{
    public sealed class FiltroPesquisaRelatorioPreCTe
    {
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public string NumeroNFe { get; set; }
        public SituacaoRelatorioPreCTe? Situacao { get; set; } 
        public List<TipoTomador> TipoTomador { get; set; }
        public bool? PossuiFRS { get; set; }
        public List<double> CodigosRemetentes {  get; set; }
        public List<double> CodigosDestinatarios {  get; set; }
        public List<double> CodigosRecebedores {  get; set; }
        public List<double> CodigosExpedidores {  get; set; }
        public List<double> CodigosTomadores {  get; set; }
        public List<int> CodigosCargas {  get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosTransportadores { get; set; }
        public List<int> CodigosTiposDeCarga { get; set; }
        public List<int> CodigosModelosVeiculos { get; set; }
        public List<int> CodigosTiposOperacao {  get; set; }
        public List<int> CodigosTiposOcorrencia {  get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino {  get; set; }
        public string CodigoEstadoOrigem {  get; set; }
        public string CodigoEstadoDestino {  get; set; }

    }
}

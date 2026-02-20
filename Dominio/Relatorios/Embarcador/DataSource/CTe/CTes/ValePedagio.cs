using System;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace Dominio.Relatorios.Embarcador.DataSource.CTe.CTes
{
    public class ValePedagio
    {
        public string NumeroCarga { get; set; }
        public DateTime DataCarga { get; set; }
        public string DataCargaFormatada
        {
            get
            {
                return DataCarga > DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string Filial { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string TipoCarga { get; set; }
        public string ModeloVeicular { get; set; }
        public string TipoOperacao { get; set; }
        public string Transportador { get; set; }
        public string Motoristas { get; set; }
        public string NumeroValePedagio { get; set; }
        public SituacaoValePedagio SituacaoValePedagio { get; set; }
        public string SituacaoValePedagioDescricao
        {
            get
            {
                return SituacaoValePedagio.ObterDescricao();
            }
        }

        public SituacaoCarga SituacaoCarga { get; set; }
        public string SituacaoCargaDescricao
        {
            get
            {
                return SituacaoCarga.ObterDescricao();
            }
        }

        public decimal ValorValePedagio { get; set; }

        public SituacaoIntegracao SituacaoIntegracaoValePedagio { set; get; }
        public string SituacaoIntegracaoValePedagioDescricao
        {
            get
            {
                return SituacaoIntegracaoValePedagio.ObterDescricao();
            }
        }
        public DateTime DataRetornoValePedagio { get; set; }
        public string DataRetornoValePedagioFormatada
        {
            get
            {
                return DataRetornoValePedagio > DateTime.MinValue ? DataRetornoValePedagio.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string RetornoIntegracao { get; set; }

        public string NumeroCargaAgrupada { get; set; }
        public decimal PesoCarga { get; set; }
        public string VeiculosCarga { get; set; }
        public string CNPJFilial { get; set; }
        public string CNPJFilialFormatado
        {
            get
            {
                return CNPJFilial.ObterCnpjFormatado();
            }
        }
        public string CNPJTransportador { get; set; }
        public string CNPJTransportadorFormatado
        {
            get
            {
                return CNPJTransportador.ObterCnpjFormatado();
            }
        }

        public string NumValePedagio { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Integradora { set; get; }
        public string TipoIntegracaoDescricao
        {
            get
            {
                return Integradora.ObterDescricao();
            }
        }

        public TipoCompraValePedagio TipoCompraValePedagio { set; get; }
        public string TipoCompraValePedagioDescricao
        {
            get
            {
                return TipoCompraValePedagio.ObterDescricao();
            }
        }

        public int TipoPercursoVP { get; set; }

        public string TipoPercursoVPDescricao => TipoPercursoVP >= 0 ? TipoPercursoVP.ToString().ToEnum<TipoRotaFrete>().ObterDescricao() : string.Empty;

        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public string RotaFrete { get; set; }
        
        public ModoCompraValePedagioTarget ModoCompraValePedagioTarget { get; set; }
        
        public string ModoCompraValePedagioTargetDescricao
        {
            get
            {
                return ModoCompraValePedagioTarget.ObterDescricao();
            }
        }
    }
}

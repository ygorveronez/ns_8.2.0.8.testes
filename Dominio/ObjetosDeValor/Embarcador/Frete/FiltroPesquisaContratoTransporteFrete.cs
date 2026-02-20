using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaContratoTransporteFrete
    {
        public int NumeroContrato { get; set; }
        public int ContratoExternoId { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaContratoTransporte? Categoria { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaContratoTransporte? SubCategoria { get; set; }
        public int CodigoTransportador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.PessoaJuridicaContratoTransporte? PessoaJuridica { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAprovacaoTransportador? StatusAprovacaoTransportador { get; set; }
        public int StatusAssinaturaContrato { get; set; }
        public bool Situacao { get; set; }
        public string NomeContrato { get; set; }
        public int TransportadorPesquisa { get; set; }
        public int CodigoTabelaFrete { get; set; }
        public bool FiltrarPorTransportadorContrato { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; } 
    }
}

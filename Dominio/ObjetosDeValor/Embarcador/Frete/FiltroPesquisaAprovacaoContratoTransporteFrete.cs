using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaAprovacaoContratoTransporteFrete
    {

        public int NumeroContrato { get; set; }
        public int ContratoExternoID { get; set; }
        public Enumeradores.CategoriaContratoTransporte? Categoria { get; set; }
        public Enumeradores.SubCategoriaContratoTransporte? SubCategoria { get; set; }
        public int Transportador { get; set; }
        public Enumeradores.PessoaJuridicaContratoTransporte? PessoaJuridica { get; set; }
        public DateTime? DatatInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public Enumeradores.StatusAprovacaoTransportador? StatusAprovacaoTransportador { get; set; }
        public int StatusAssinaturaContrato { get; set; }
        public bool? Ativo { get; set; }
    }
}

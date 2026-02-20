using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class FiltroPesquisaTaxaTerceiro
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public double CnpjCpfTerceiro { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoJustificativa { get; set; }
        public int CodigoCarga { get; set; }
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class FiltroPesquisaContratoNotaFiscal
    {
        public string Contrato { get; set; }
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}

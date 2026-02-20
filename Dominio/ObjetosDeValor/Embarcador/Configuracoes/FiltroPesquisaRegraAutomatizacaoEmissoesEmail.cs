using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class FiltroPesquisaRegraAutomatizacaoEmissoesEmail
    {
        public string Descricao { get; set; }

        public string EmailDestino { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public double CodigoRemetente { get; set; }

        public double CodigoDestinatario { get; set; }

        public SituacaoAtivoPesquisa Ativo { get; set; }
    }
}
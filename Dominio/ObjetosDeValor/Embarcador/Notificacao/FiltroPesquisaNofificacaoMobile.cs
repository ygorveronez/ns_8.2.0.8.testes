namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class FiltroPesquisaNofificacaoMobile
    {
        public string Assunto { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoUsuario { get; set; }

        public System.DateTime? DataInicio { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public Enumeradores.TipoLancamentoNotificacaoMobile? TipoLancamento { get; set; }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum InformacoesAdicionaisEntregaTrizy
    {
        ObservacoesDoCliente = 1,
        TelefoneDoCliente = 2,
        NumeroDosPedidos = 3,
        Fardos = 4,
        RegrasPallet = 5,
        NotasFiscais = 6,
        NomeFantasia = 7,
        RazaoSocial = 8,
    }

    public static class InformacoesAdicionaisEntregaTrizyHelper
    {
        public static string ObterDescricao(this InformacoesAdicionaisEntregaTrizy InformacoesAdicionaisEntregaTrizy)
        {
            switch (InformacoesAdicionaisEntregaTrizy)
            {
                case InformacoesAdicionaisEntregaTrizy.ObservacoesDoCliente: return Localization.Resources.Pedidos.TipoOperacao.ObservacoesDoCliente;
                case InformacoesAdicionaisEntregaTrizy.TelefoneDoCliente: return Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente;
                case InformacoesAdicionaisEntregaTrizy.NumeroDosPedidos: return Localization.Resources.Pedidos.TipoOperacao.NumeroDosPedidos;
                case InformacoesAdicionaisEntregaTrizy.Fardos: return Localization.Resources.Pedidos.TipoOperacao.Fardos;
                case InformacoesAdicionaisEntregaTrizy.RegrasPallet: return Localization.Resources.Pedidos.TipoOperacao.RegrasPallet;
                case InformacoesAdicionaisEntregaTrizy.NotasFiscais: return Localization.Resources.Pedidos.TipoOperacao.NotasFiscais;
                case InformacoesAdicionaisEntregaTrizy.NomeFantasia: return Localization.Resources.Pedidos.TipoOperacao.NomeFantasia;
                case InformacoesAdicionaisEntregaTrizy.RazaoSocial: return Localization.Resources.Pedidos.TipoOperacao.RazaoSocial;
                default: return string.Empty;
            }
        }
    }
}
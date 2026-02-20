using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEntrega
    {
        NaoEntregue = 0,
        EmCliente = 1,
        Entregue = 2,
        Rejeitado = 3,
        Revertida = 4,
        Reentergue = 5,
        AgAtendimento = 6,
        EntregarEmOutroCliente = 7,
        DescartarMercadoria = 8,
        QuebraPeso = 9,
        ReentregarMesmaCarga = 10,
        EmFinalizacao = 11
    }


    public static class SituacaoEntregaHelper
    {
        public static bool ObterSituacaoEntregaEmAberto(SituacaoEntrega situacaoEnterga)
        {
            return (situacaoEnterga == SituacaoEntrega.NaoEntregue) || (situacaoEnterga == SituacaoEntrega.EmCliente) || (situacaoEnterga == SituacaoEntrega.Revertida);
        } 
        
        public static List<SituacaoEntrega> ObterListaSituacaoEntregaEmAberto()
        {
            return new List<SituacaoEntrega>() { SituacaoEntrega.NaoEntregue, SituacaoEntrega.EmCliente, SituacaoEntrega.Revertida };
        }

        public static bool ObterSituacaoEntregaFinalizada(SituacaoEntrega situacaoEnterga)
        {
            return (situacaoEnterga == SituacaoEntrega.Entregue) 
                || (situacaoEnterga == SituacaoEntrega.Reentergue) 
                || (situacaoEnterga == SituacaoEntrega.Rejeitado) 
                || (situacaoEnterga == SituacaoEntrega.QuebraPeso) 
                || (situacaoEnterga == SituacaoEntrega.DescartarMercadoria) 
                || (situacaoEnterga == SituacaoEntrega.EntregarEmOutroCliente) 
                || (situacaoEnterga == SituacaoEntrega.ReentregarMesmaCarga)
                || (situacaoEnterga == SituacaoEntrega.EmFinalizacao);
        }

        public static List<SituacaoEntrega> ObterListaSituacaoEntregaFinalizada()
        {
            return new List<SituacaoEntrega> {

            SituacaoEntrega.Entregue,
            SituacaoEntrega.Reentergue,
            SituacaoEntrega.Rejeitado,
            SituacaoEntrega.AgAtendimento,
            SituacaoEntrega.EntregarEmOutroCliente,
            SituacaoEntrega.DescartarMercadoria,
            SituacaoEntrega.QuebraPeso,
            SituacaoEntrega.ReentregarMesmaCarga,
            SituacaoEntrega.EmFinalizacao};
        }

        public static string ObterDescricao(this SituacaoEntrega situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case SituacaoEntrega.NaoEntregue: return Localization.Resources.Enumeradores.SituacaoEntrega.NaoEntregue;
                case SituacaoEntrega.EmCliente: return Localization.Resources.Enumeradores.SituacaoEntrega.NoLocal;
                case SituacaoEntrega.Entregue: return Localization.Resources.Enumeradores.SituacaoEntrega.Realizado;
                case SituacaoEntrega.Rejeitado: return Localization.Resources.Enumeradores.SituacaoEntrega.Rejeitado;
                case SituacaoEntrega.Revertida: return Localization.Resources.Enumeradores.SituacaoEntrega.Revertida;
                case SituacaoEntrega.Reentergue: return Localization.Resources.Enumeradores.SituacaoEntrega.Reentregue;
                case SituacaoEntrega.AgAtendimento: return Localization.Resources.Enumeradores.SituacaoEntrega.AgAtendimento;
                case SituacaoEntrega.EntregarEmOutroCliente: return Localization.Resources.Enumeradores.SituacaoEntrega.EntregarEmOutroCliente;
                case SituacaoEntrega.DescartarMercadoria: return Localization.Resources.Enumeradores.SituacaoEntrega.DescartarMercadoria;
                case SituacaoEntrega.QuebraPeso: return Localization.Resources.Enumeradores.SituacaoEntrega.QuebraPeso;
                case SituacaoEntrega.ReentregarMesmaCarga: return Localization.Resources.Enumeradores.SituacaoEntrega.ReentregarMesmaCarga;
                case SituacaoEntrega.EmFinalizacao: return Localization.Resources.Enumeradores.SituacaoEntrega.EmFinalizacao;
                default: return "";
            }
        }

        public static string ObterDescricaoPortalEntrega(this SituacaoEntrega situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case SituacaoEntrega.NaoEntregue: return Localization.Resources.Enumeradores.SituacaoEntrega.Transito;
                case SituacaoEntrega.EmCliente: return Localization.Resources.Enumeradores.SituacaoEntrega.NoLocal;
                case SituacaoEntrega.Entregue: return Localization.Resources.Enumeradores.SituacaoEntrega.Realizado;
                case SituacaoEntrega.Rejeitado: return Localization.Resources.Enumeradores.SituacaoEntrega.Rejeitado;
                case SituacaoEntrega.Revertida: return Localization.Resources.Enumeradores.SituacaoEntrega.Revertida;
                case SituacaoEntrega.Reentergue: return Localization.Resources.Enumeradores.SituacaoEntrega.Reentregue;
                case SituacaoEntrega.AgAtendimento: return Localization.Resources.Enumeradores.SituacaoEntrega.AgAtendimento;
                case SituacaoEntrega.EntregarEmOutroCliente: return Localization.Resources.Enumeradores.SituacaoEntrega.EntregarEmOutroCliente;
                case SituacaoEntrega.DescartarMercadoria: return Localization.Resources.Enumeradores.SituacaoEntrega.DescartarMercadoria;
                case SituacaoEntrega.QuebraPeso: return Localization.Resources.Enumeradores.SituacaoEntrega.QuebraPeso;
                case SituacaoEntrega.ReentregarMesmaCarga: return Localization.Resources.Enumeradores.SituacaoEntrega.ReentregarMesmaCarga;
                case SituacaoEntrega.EmFinalizacao: return Localization.Resources.Enumeradores.SituacaoEntrega.EmFinalizacao;
                default: return "";
            }
        }

        public static string ObterDescricaoTratativaDevolucao(this SituacaoEntrega situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case SituacaoEntrega.Rejeitado: return Localization.Resources.Enumeradores.SituacaoEntrega.AceitarDevolucao;
                case SituacaoEntrega.Revertida: return Localization.Resources.Enumeradores.SituacaoEntrega.ReverterDevolucao;
                case SituacaoEntrega.EntregarEmOutroCliente: return Localization.Resources.Enumeradores.SituacaoEntrega.EntregarEmOutroCliente;
                case SituacaoEntrega.DescartarMercadoria: return Localization.Resources.Enumeradores.SituacaoEntrega.DescartarMercadoria;
                case SituacaoEntrega.QuebraPeso: return Localization.Resources.Enumeradores.SituacaoEntrega.QuebraPeso;
                case SituacaoEntrega.Reentergue: return Localization.Resources.Enumeradores.SituacaoEntrega.ReentregarOutroMomento;
                case SituacaoEntrega.ReentregarMesmaCarga: return Localization.Resources.Enumeradores.SituacaoEntrega.ReentregarMesmaCarga;
                default: return Localization.Resources.Enumeradores.SituacaoEntrega.AceitarDevolucao;
            }
        }

    }
}

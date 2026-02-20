using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DiaSemana
    {
        Domingo = 1,
        Segunda = 2,
        Terca = 3,
        Quarta = 4,
        Quinta = 5,
        Sexta = 6,
        Sabado = 7
    }

    public static class DiaSemanaHelper
    {
        public static string ObterDescricao(this DiaSemana diaSemana)
        {
            switch (diaSemana)
            {
                case DiaSemana.Domingo: return Localization.Resources.Pedidos.RetiradaProduto.Domingo;
                case DiaSemana.Segunda: return Localization.Resources.Pedidos.RetiradaProduto.SegundaFeira;
                case DiaSemana.Terca: return Localization.Resources.Pedidos.RetiradaProduto.TercaFeira;
                case DiaSemana.Quarta: return Localization.Resources.Pedidos.RetiradaProduto.QuartaFeira;
                case DiaSemana.Quinta: return Localization.Resources.Pedidos.RetiradaProduto.QuintaFeira;
                case DiaSemana.Sexta: return Localization.Resources.Pedidos.RetiradaProduto.SextaFeira;
                case DiaSemana.Sabado: return Localization.Resources.Pedidos.RetiradaProduto.Sabado;
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(DateTime data)
        {
            DiaSemana diaSemana = ObterDiaSemana(data);

            return diaSemana.ObterDescricao();
        }

        public static string ObterDescricaoResumida(this DiaSemana diaSemana)
        {
            switch (diaSemana)
            {
                case DiaSemana.Domingo: return Localization.Resources.Pedidos.RetiradaProduto.Domingo;
                case DiaSemana.Segunda: return Localization.Resources.Pedidos.RetiradaProduto.Segunda;
                case DiaSemana.Terca: return Localization.Resources.Pedidos.RetiradaProduto.Terca;
                case DiaSemana.Quarta: return Localization.Resources.Pedidos.RetiradaProduto.Quarta;
                case DiaSemana.Quinta: return Localization.Resources.Pedidos.RetiradaProduto.Quinta;
                case DiaSemana.Sexta: return Localization.Resources.Pedidos.RetiradaProduto.Sexta;
                case DiaSemana.Sabado: return Localization.Resources.Pedidos.RetiradaProduto.Sabado;
                default: return string.Empty;
            }
        }

        public static DiaSemana ObterDiaSemana(DateTime data)
        {
            int dia = (int)data.DayOfWeek + 1;

            return (DiaSemana)dia;
        }
    }
}

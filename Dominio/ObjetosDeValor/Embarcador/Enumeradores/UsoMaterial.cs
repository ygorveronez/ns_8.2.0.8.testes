namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum UsoMaterial
    {
        Revenda = 0,
        ContinuacaoProcessamento = 1,
        Consumo = 2,
        Imobilizado = 3
    }

    public static class UsoMaterialHelper
    {
        public static string ObterDescricao(this UsoMaterial opcao)
        {
            switch (opcao)
            {
                case UsoMaterial.Revenda: return "Revenda";
                case UsoMaterial.ContinuacaoProcessamento: return "Continuação Processamento";
                case UsoMaterial.Consumo: return "Consumo";
                case UsoMaterial.Imobilizado: return "Imobilizado";
                default: return string.Empty;
            }
        }
    }
}

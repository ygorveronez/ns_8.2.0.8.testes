namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ClassificacaoNFe
    {
        Todos = -1,
        SemClassificacao = 0,
        Revenda = 1,
        NaoRevenda = 2,
        NFEletronicos = 3,
        GrandesVolumes = 4,
        MateriaPrima = 5,
        Retira = 6,
        VM = 7,
        Remessa = 8,
        Venda = 9
    }

    public static class ClassificacaoNFeHelper
    {
        public static string ObterDescricao(this ClassificacaoNFe classificacao)
        {
            switch (classificacao)
            {
                case ClassificacaoNFe.Revenda: return "Revenda";
                case ClassificacaoNFe.NaoRevenda: return "Não Revenda";
                case ClassificacaoNFe.SemClassificacao: return "Sem Classificação";
                case ClassificacaoNFe.NFEletronicos: return "NF Eletrônicos";
                case ClassificacaoNFe.GrandesVolumes: return "Grandes Volumes";
                case ClassificacaoNFe.MateriaPrima: return "Matéria Prima";
                case ClassificacaoNFe.Retira: return "Retira";
                case ClassificacaoNFe.VM: return "VM";
                case ClassificacaoNFe.Remessa: return "Remessa";
                case ClassificacaoNFe.Venda: return "Venda";
                default: return string.Empty;
            }
        }
    }
}

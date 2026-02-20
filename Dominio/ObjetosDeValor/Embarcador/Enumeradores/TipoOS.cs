namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOS
    {
        CargaCheia = 0,
        Armazenagem = 1,
        Estadia = 2,
        OSAvulsa = 3,
        Nenhum = 4,
        NaoInformado = 9
    }

    public static class EnumTipoOSeHelper
    {
        public static string ObterDescricao(this TipoOS situacao)
        {
            switch (situacao)
            {
                case TipoOS.CargaCheia: return "Carga Cheia";
                case TipoOS.Armazenagem: return "Armazenagem";
                case TipoOS.Estadia: return "Estadia";
                case TipoOS.OSAvulsa: return "OS Avulsa";
                case TipoOS.Nenhum: return "Nenhum";
                case TipoOS.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoManobraAcao
    {
        NaoInformado = 0,
        InicioCarregamento = 1,
        Checklist = 2,
        Higienizacao = 3
    }

    public static class TipoManobraAcaoHelper
    {
        public static string ObterDescricao(this TipoManobraAcao tipo)
        {
            switch (tipo)
            {
                case TipoManobraAcao.Checklist: return "Checklist";
                case TipoManobraAcao.Higienizacao: return "Higienização";
                case TipoManobraAcao.InicioCarregamento: return "Início de Carregamento";
                case TipoManobraAcao.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }
    }
}

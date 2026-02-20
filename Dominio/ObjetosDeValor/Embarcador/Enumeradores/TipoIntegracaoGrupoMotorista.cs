namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoGrupoMotorista
    {
        Criar = 0,
        Atualizar = 1,
    }

    public static class TipoIntegracaoGrupoMotoristaHelper
    {
        public static string ObterDescricao(this TipoIntegracaoGrupoMotorista tipo)
        {
            switch (tipo)
            {
                case TipoIntegracaoGrupoMotorista.Criar: return "Criar";
                case TipoIntegracaoGrupoMotorista.Atualizar: return "Atualizar";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoSetorFuncionario
    {
        NaoInformado = 0,
        Planejamento = 1,
    }

    public static class TipoSetorFuncionarioHelper
    {
        public static string ObterDescricao(this TipoSetorFuncionario tipoSetor)
        {
            switch (tipoSetor)
            {
                case TipoSetorFuncionario.NaoInformado: return "Não informado";
                case TipoSetorFuncionario.Planejamento: return "Planejamento";
                default: return string.Empty;
            }
        }
    }
}
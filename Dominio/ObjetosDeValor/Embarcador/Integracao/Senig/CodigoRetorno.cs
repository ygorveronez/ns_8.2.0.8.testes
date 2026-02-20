
namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senig
{
    public enum CodigoRetorno
    {
        Sucesso = 100,
        Erro = 110,
        Invalido = 154
    }

    public static class CodigoRetornoHelper
    {
        public static string ObterDetalhe(this CodigoRetorno codigo)
        {
            switch (codigo)
            {
                case CodigoRetorno.Sucesso: return "Recebido com Sucesso";
                case CodigoRetorno.Erro: return "Erro no processamento";
                case CodigoRetorno.Invalido: return "Segurado não possui apólice 540 para gerar protocolo";
                default: return string.Empty;
            }
        }
    }
}

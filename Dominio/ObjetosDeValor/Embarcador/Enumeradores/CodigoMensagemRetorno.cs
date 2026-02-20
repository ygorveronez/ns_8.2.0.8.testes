namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public sealed class CodigoMensagemRetorno
    {
        public static readonly int Sucesso = 200;
        public static readonly int DadosInvalidos = 300;
        public static readonly int RegistroIndisponivel = 301;
        public static readonly int FalhaGenerica = 400;
        public static readonly int DuplicidadeDaRequisicao = 500;
        public static readonly int SessaoExpirada = 600;
    }
}
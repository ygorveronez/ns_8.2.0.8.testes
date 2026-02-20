using System;

namespace SGT.WebServiceCargoX
{
    public class Retorno<T>
    {
        #region Propriedades

        public T Objeto { get; set; }

        public bool Status { get; set; }

        public string DataRetorno { get; set; }

        public string Mensagem { get; set; }

        public int CodigoMensagem { get; set; }

        #endregion

        #region Construtores

        public static Retorno<T> CriarRetornoDadosInvalidos(string mensagem, int codigoMensagem)
        {
            return new Retorno<T>
            {
                CodigoMensagem = codigoMensagem,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = mensagem,
                Status = false
            };
        }

        public static Retorno<T> CriarRetornoDadosInvalidos(string mensagem, T objeto = default(T))
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = mensagem,
                Objeto = objeto,
                Status = false
            };
        }

        public static Retorno<T> CriarRetornoDuplicidadeRequisicao(string mensagem, T objeto = default(T))
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = mensagem,
                Objeto = objeto,
                Status = false
            };
        }

        public static Retorno<T> CriarRetornoExcecao(string mensagem)
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = mensagem,
                Status = false
            };
        }

        public static Retorno<T> CriarRetornoSucesso(T objeto)
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Objeto = objeto,
                Status = true
            };
        }

        #endregion

    }
}
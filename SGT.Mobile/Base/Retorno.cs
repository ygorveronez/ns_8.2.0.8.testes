using System;

namespace SGT.Mobile
{
    public class Retorno<T>
    {
        #region Propriedades

        public int CodigoMensagem { get; set; }

        public string DataRetorno { get; set; }

        public string Mensagem { get; set; }

        public T Objeto { get; set; }

        public bool Status { get; set; }

        public string VersaoAplicativoMobile { get; set; }

        #endregion

        #region Construtores

        public Retorno()
        {            
            VersaoAplicativoMobile = Startup.appSettingsAD["AppSettings:VersaoAplicativoMobile"]?.ToString() ?? "";
        }

        public static Retorno<T> CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware(T objeto = default(T))
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = "Sua sessão não permite consultar dados deste cliente",
                Objeto = objeto,
                Status = false
            };
        }

        public static Retorno<T> CriarRetornoRegistroIndisponivel(string mensagem, T objeto = default(T))
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = mensagem,
                Objeto = objeto,
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

        public static Retorno<T> CriarRetornoSessaoExpirada()
        {
            return new Retorno<T>
            {
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.SessaoExpirada,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = "Sessão inválida ou expirada",
                Status = false
            };
        }

        #endregion
    }
}
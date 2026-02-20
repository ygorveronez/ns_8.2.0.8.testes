using Dominio.Excecoes.Embarcador;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.SystemSat
{

    public class IntegracaoSystemSat
    {
        #region Atributos privados

        private static IntegracaoSystemSat Instance;
        private Servicos.SystemSat.PosicoesSoapClient client;
        private Servicos.InspectorBehavior inspector;

        private string url;
        private string login;
        private string senha;
        private string empCliente;

        #endregion

        #region Construtor privado

        private IntegracaoSystemSat() { }

        #endregion

        #region Métodos públicos

        public static IntegracaoSystemSat GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoSystemSat();
            return Instance;
        }

        public void DefinirConfiguracoes(string url, string login, string senha, string empCliente)
        {
            this.url = url;
            this.login = login;
            this.senha = senha;
            this.empCliente = empCliente;
        }

        public List<Servicos.SystemSat.Posicao> BuscarUltimasPosicoes(Repositorio.UnitOfWork unitOfWork)
        {
            VerificarPreparar(unitOfWork);
            Servicos.SystemSat.Lista_UltimasPosicoesResponse ultimasPosicoesResult = this.client.Lista_UltimasPosicoesAsync(this.empCliente, this.login, this.senha, true).Result;
            List<Servicos.SystemSat.Posicao> ultimasPosicoes = ultimasPosicoesResult?.Body?.Lista_UltimasPosicoesResult?.ToList() ?? new List<Servicos.SystemSat.Posicao>();
            return ultimasPosicoes;
        }

        #endregion

        #region Métodos privados

        private void VerificarPreparar(Repositorio.UnitOfWork unitOfWork)
        {
            VerificarConfiguracoes();
            PrepararWSClient(unitOfWork);
        }

        private void PrepararWSClient(Repositorio.UnitOfWork unitOfWork)
        {
            if (this.client == null)
            {
                this.client = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.SystemSat.PosicoesSoapClient, Servicos.SystemSat.PosicoesSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SystemSat_PosicoesSoap, url);
            }
        }

        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.url)) throw new ServicoException("URL SystemSat não definida");
            if (string.IsNullOrWhiteSpace(this.login)) throw new ServicoException("Login SystemSat não definido");
            if (string.IsNullOrWhiteSpace(this.senha)) throw new ServicoException("Senha SystemSat não definida");
            if (string.IsNullOrWhiteSpace(this.empCliente)) throw new ServicoException("EmpCliente SystemSat não definida");
        }

        #endregion

    }

}

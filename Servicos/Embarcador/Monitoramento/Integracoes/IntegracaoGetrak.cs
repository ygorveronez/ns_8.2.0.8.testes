using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoGetrak : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoGetrak Instance;
        private static readonly string nameConfigSection = "Getrak";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private Servicos.Embarcador.Integracao.Getrak.IntegracaoGetrak ServicoIntegracaoGetrak;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_URI_CHAVE = "Chave";
        private const string KEY_URI_TOKEN = "URIToken";
        private const string KEY_URI_LOCALIZACOES = "URILocalizacao";

        #endregion

        #region Construtor privado

        private IntegracaoGetrak(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Getrak, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoGetrak GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoGetrak(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {

        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {

        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        override protected void Preparar()
        {
            this.ServicoIntegracaoGetrak = Servicos.Embarcador.Integracao.Getrak.IntegracaoGetrak.GetInstance();
        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                this.ServicoIntegracaoGetrak.SetURL(Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta));
                this.ServicoIntegracaoGetrak.SetUsuario(this.conta.Usuario);
                this.ServicoIntegracaoGetrak.SetSenha(this.conta.Senha);
                this.ServicoIntegracaoGetrak.SetChave(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_URI_CHAVE, this.conta.ListaParametrosAdicionais));
                this.ServicoIntegracaoGetrak.SetURIToken(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_URI_TOKEN, this.conta.ListaParametrosAdicionais));
                this.ServicoIntegracaoGetrak.SetURILocalizacoes(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_URI_LOCALIZACOES, this.conta.ListaParametrosAdicionais));

                // Autenticação
                Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Token token = this.ServicoIntegracaoGetrak.ObterToken();
                if (this.ServicoIntegracaoGetrak.VerificarValidadeToken(token))
                {
                    // Busca os eventos normais, que contém as posições dos veículos
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak.Veiculo> veiculos = this.ServicoIntegracaoGetrak.BuscarLocalizacoes(token);
                    int total = veiculos.Count;
                    Log($"Recebidas {total} veiculos", 3);

                    for (int i = 0; i < total; i++)
                    {
                        string[] placas = veiculos[i].placa.Trim().Split(' ');
                        int totalPlacas = placas.Length;
                        for (int j = 0; j < totalPlacas; j++)
                        {
                            string placa = placas[j].Trim().ToUpper().Replace("-", "");
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = veiculos[i].dataServidor.date,
                                DataVeiculo = veiculos[i].datastatus.date,
                                IDEquipamento = veiculos[i].modulo,
                                Placa = placa,
                                Latitude = veiculos[i].lat ?? 0,
                                Longitude = veiculos[i].lon ?? 0,
                                Velocidade = veiculos[i].velocidade,
                                Ignicao = veiculos[i].lig,
                                SensorTemperatura = false,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Getrak
                            });
                        }
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                }
                else
                {
                    Log("Token invalido", 3);
                }

            }
            catch (Exception ex)
            {
                Log("Erro ObterPacotePosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        #endregion

    }

}

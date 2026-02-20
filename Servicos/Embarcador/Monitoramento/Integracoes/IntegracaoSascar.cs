using System;
using System.Collections.Generic;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoSascar : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoSascar Instance;
        private static readonly string nameConfigSection = "Sascar";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private bool _possuiSensorDesengate;

        #endregion

        #region Atributos privados específicos para a Sascar

        Servicos.Sascar.SasIntegraWSClient sascarWSClient;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_LIMITE_REGISTROS = "LimiteRegistros";

        #endregion

        #region Construtor privado

        private IntegracaoSascar(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sascar, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoSascar GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSascar(cliente);
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

        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            InicializarWSSascar();
            if (this.configuracao.ProcessarSensores)
                ObterGrupoAtuadores();

            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        private void ObterGrupoAtuadores()
        {

            Log($"Buscando grupo atuadores ativos", 2);

            try
            {
                Servicos.Sascar.grupoAtuador[] respostas = sascarWSClient.obterGrupoAtuadores(conta.Usuario, conta.Senha);
                if (respostas != null)
                {

                    foreach (var grupo in respostas)
                    {
                        if (grupo.idAtuador == (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoAtuadoresSensorSascar.Desengate)
                            _possuiSensorDesengate = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Erro ObterGrupoAtuadores " + e.Message, 3);
            }

        }


        private void IntegrarPosicao()
        {

            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                Servicos.Sascar.pacotePosicao[] respostas = sascarWSClient.obterPacotePosicoes(conta.Usuario, conta.Senha, ObterLimiteRegistros());
                if (respostas != null)
                {

                    foreach (var posicao in respostas)
                    {
                        string endereco = posicao.cidade + '/' + posicao.uf;
                        if (!string.IsNullOrWhiteSpace(posicao.rua)) endereco = posicao.rua + ". " + endereco;

                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            ID = posicao.codigoMacro,
                            Data = posicao.dataPosicao,
                            DataVeiculo = posicao.dataPacote,
                            Placa = posicao.placa,
                            IDEquipamento = posicao.idVeiculo.ToString(),
                            Velocidade = posicao.velocidade,
                            Latitude = posicao.latitude,
                            Longitude = posicao.longitude,
                            Ignicao = posicao.ignicao,
                            Descricao = endereco,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Sascar
                        };

                        // Se apresentar o valor -125, indica que não há valor disponível ou válido para esse sensor
                        if (posicao.temperatura1 > -125) novaPosicao.Temperatura = posicao.temperatura1;

                        if (_possuiSensorDesengate && posicao.eventos != null && posicao.eventos.Length > 0)
                        {
                            foreach (var evento in posicao.eventos)
                            {
                                if (evento.codigo == (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoAtuadoresSensorSascar.Desengate)//ativou sensor desengate (FOI DESENGATADO)
                                    novaPosicao.SensorDeDesengate = true;
                                else if (evento.codigo == -(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoAtuadoresSensorSascar.Desengate)//desativou sensor desengate (ESTA ENGATADO)
                                    novaPosicao.SensorDeDesengate = false;
                            }
                        }

                        posicoes.Add(novaPosicao);
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Erro obterPacotePosicoes " + e.Message, 3);
            }
            return posicoes;
        }

        private void InicializarWSSascar()
        {
            Log("Inicializando WS", 2);

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            this.sascarWSClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient< Servicos.Sascar.SasIntegraWSClient,  Servicos.Sascar.SasIntegraWS>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Sascar_SasIntegraWS, url);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private int ObterLimiteRegistros()
        {
            try
            {
                string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_LIMITE_REGISTROS, this.conta.ListaParametrosAdicionais);
                return Int32.Parse(value);
            }
            catch
            {
                return 1000;
            }
        }

        #endregion

    }

}

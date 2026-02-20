using MiX.Integrate.API.Client;
using MiX.Integrate.Shared.Entities.Assets;
using MiX.Integrate.Shared.Entities.Carriers;
using MiX.Integrate.Shared.Entities.Groups;
using MiX.Integrate.Shared.Entities.Positions;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    /**
     * Murillo Rodrigues
     * Analista de Suporte
     * murillo@rotagyn.com.br
     * (062) 98231-0084
     * 
     * Rotagyn (Revendedor MixTelematics)
     * (062) 3942-2122
     * 0800 787 3020
     * 
     * https://identity.us.mixtelematics.com/core
     * https://integrate.us.mixtelematics.com/
     * 
     * https://identity.uat.mixtelematics.com/core
     * https://integrate.uat.mixtelematics.com/
     * 
     * Identificação do veículo:
     * Podem ser usados dois campos da aplicação Web MixTelematics: "Número de registro" e "ID do ativo".
     * Na MixTelematics não existe o conceito de espelhamento/replicação de sinal de veículos para outras contas.
     * Para integrar mais novas transportadoras, deverão ser configuradas novas contas.
     * Como o ID do ativo é único dentro de cada conta mas repete entre contas distintas, deve ser usado o 
     * campo "Número de registro" para identificação do veículo. Este campo é único e pode ser alterado pela transportadora. 
     * O Murillo sugere que este campo deve ser preenchido com a placa do veículo, mas pode ser informado outro valor, 
     * como o nome/apelido do motorista, chassi ou número de série do motor.
     * 
     * Limites de requisições:
     * - Calls per min: 50
     * - Calls per hour: 500
     * - Fair use per month: 50GB
     * 403 - Too Many Requests
     * 
     */
    public class IntegracaoMixTelematics : Abstract.AbstractIntegracaoWebService
    {
        #region Atributos privados

        private static IntegracaoMixTelematics Instance;
        private static readonly string nameConfigSection = "MixTelematics";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados MixTelematics

        private DateTime ultimaDataConsultada;
        private DateTime dataAtual;
        private string idBaseUrl;
        private string apiBaseUrl;
        private DateTime validadeGroups;
        private string ultimoSinceToken;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string KEY_ULTIMO_SINCE_TOKEN = "SinceToken";
        private const string KEY_ID_BASE_URL = "IDBaseUrl";
        private const string KEY_API_BASE_URL = "ApiBaseUrl";
        private const string KEY_CLIENT_APPLICATION_NAME = "ClientApplicationName";
        private const string KEY_CLIENT_ID = "ClientID";
        private const string KEY_CLIENT_SECRET = "ClientSecret";
        private const string KEY_SCOPES = "Scopes";

        private List<Group> _groups = new List<Group>();

        #endregion

        #region Construtor privado

        private IntegracaoMixTelematics(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MixTelematics, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoMixTelematics GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoMixTelematics(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {
            try
            {
                this.idBaseUrl = ObterValorOpcao(KEY_ID_BASE_URL);
                this.apiBaseUrl = ObterValorOpcao(KEY_API_BASE_URL);
            }
            catch (Exception e)
            {
                Log($"Erro ao ComplementarConfiguracoes {e.Message}", 2);
            }
        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
            string msg;
            if (string.IsNullOrWhiteSpace(this.idBaseUrl))
            {
                msg = $"URL de identificacao ({KEY_ID_BASE_URL}) nao informada";
                LogErro(msg);
                throw new Exception(msg);
            }
            if (string.IsNullOrWhiteSpace(this.apiBaseUrl))
            {
                msg = $"URL da API ({KEY_API_BASE_URL}) nao informada";
                LogErro(msg);
                throw new Exception(msg);
            }
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
            this.dataAtual = DateTime.Now;
            //ObterUltimaDataDoArquivo();
            ObtemOuRevalidaGroups();
            ObterUltimaSinceTokenDoArquivo();
            IntegrarPosicao();
            //SalvarUltimaDataNoArquivo();
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
            IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings = Login();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes(idServerResourceOwnerClientSettings);

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }

        private void ObtemOuRevalidaGroups()
        {
            if (!GroupsEstaValido())
            {
                // Objeto para autenticação
                IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings = Login();

                _groups = GetGroups(idServerResourceOwnerClientSettings);
            }
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes(IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {

                // Grupos da organização
                //groups = GetGroups(idServerResourceOwnerClientSettings);

                // Consulta as posições
                List<Position> positions = GetPositions(_groups, idServerResourceOwnerClientSettings);
                int total = positions?.Count ?? 0;
                if (total > 0)
                {

                    // Consulta todos os veículos dos grupos para buscar a identificação
                    List<Asset> assets = GetAssets(_groups, idServerResourceOwnerClientSettings);

                    for (int i = 0; i < total; i++)
                    {

                        // Busca a identificação do veículo na lista de assets
                        string idEquipamento = IdentificarVeiculoNosAssets(positions[i].AssetId, assets);
                        if (!string.IsNullOrWhiteSpace(idEquipamento))
                        {
                            // Adiciona na lista de posições recebidas
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = positions[i].Timestamp.AddHours(-3),
                                Placa = idEquipamento.Trim(),
                                DataVeiculo = positions[i].Timestamp.AddHours(-3),
                                IDEquipamento = idEquipamento,
                                Descricao = positions[i].FormattedAddress,
                                Latitude = positions[i].Latitude,
                                Longitude = positions[i].Longitude,
                                Velocidade = (int)positions[i].SpeedKilometresPerHour,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.MixTelematics
                            });
                        }
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                    // Extrai a maior data para iniciar por ela na próxima requisição
                    //AtualizarUltimaDataConsultada(posicoes);

                }

                SalvarUltimaSinceTokenNoArquivo();
            }
            catch (Exception ex)
            {
                Log("Erro GetPositions " + ex.Message, 3);
            }
            return posicoes;
        }

        private IdServerResourceOwnerClientSettings Login()
        {
            IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings = new IdServerResourceOwnerClientSettings()
            {
                BaseAddress = this.idBaseUrl,
                ClientId = ObterClientID(),
                ClientSecret = ObterClientSecret(),
                UserName = this.conta.Usuario,
                Password = this.conta.Senha,
                Scopes = ObterScopes()
            };
            return idServerResourceOwnerClientSettings;
        }

        private List<Group> GetGroups(IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            GroupsClient groupsClient = new GroupsClient(this.apiBaseUrl, idServerResourceOwnerClientSettings);
            List<Group> groups = groupsClient.GetAvailableOrganisations();
            Log($"Recebidos \"{groups?.Count ?? 0} groups\"", 3);

            this.validadeGroups = DateTime.Now.AddSeconds(36000);
            return groups;
        }

        private bool GroupsEstaValido()
        {
            return (_groups.Count > 0 && this.validadeGroups > DateTime.Now);
        }

        private List<Position> GetPositions(List<Group> groups, IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            //#31312 TELEMATIC, DELAY NO RETORNO, QUANDO SOLICITADO SEMPRE PEDIR 3 HORAS A FRENTE PARA CONVERSAO INTERNA DA TECNLOGIA CONVERTER PARA HORA CORRENTE;
            List<long> groupIds = new List<long>();
            int total = groups?.Count ?? 0;
            if (total > 0)
            {
                List<Position> positions = new List<Position>();

                for (int i = 0; i < total; i++)
                {
                    groupIds.Add(groups[i].GroupId);
                }

                PositionsClient positionsClient = new PositionsClient(this.apiBaseUrl, idServerResourceOwnerClientSettings);

                //List<Position> positions = positionsClient.GetByDateRangeByGroupIds(groupIds, this.ultimaDataConsultada.AddMinutes(165), this.dataAtual.AddHours(3));
                //Log($"Recebidas \"{positions?.Count ?? 0} positions\"", 3);
                //return positions;

                CreatedSinceResult<Position> result = positionsClient.GetCreatedSinceForGroups(groupIds, "driver", this.ultimoSinceToken, 1000);

                if (result.Items != null && result.Items.Count > 0)
                    positions.AddRange(result.Items);

                while (result.HasMoreItems)
                {
                    result = positionsClient.GetCreatedSinceForGroups(groupIds, "driver", result.GetSinceToken.Substring(0, 14), 1000);
                    positions.AddRange(result.Items);
                }
                AtualizarUltimoSinceToken(result);

                Log($"Recebidas \"{positions?.Count ?? 0} positions\"", 3);

                return positions;
            }

            return null;
        }

        private GroupSummary GetGroupSummary(long organisationGroupId, IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            var groupsClient = new GroupsClient(this.apiBaseUrl, idServerResourceOwnerClientSettings);
            var group = groupsClient.GetSubGroups(organisationGroupId);
            return group;
        }

        private Asset GetAsset(long assetId, IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            AssetsClient assetsClient = new AssetsClient(this.apiBaseUrl, idServerResourceOwnerClientSettings);
            Asset asset = assetsClient.Get(assetId);
            return asset;
        }

        private List<Asset> GetAssets(List<Group> groups, IdServerResourceOwnerClientSettings idServerResourceOwnerClientSettings)
        {
            List<Asset> assets = new List<Asset>();
            AssetsClient assetsClient = new AssetsClient(this.apiBaseUrl, idServerResourceOwnerClientSettings);
            int total = groups?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                List<Asset> assetsGroup = assetsClient.GetAll(groups[i].GroupId);
                assets.AddRange(assetsGroup);
            }
            Log($"Encontrados \"{assets.Count} assets\" em \"{total}\" groups", 3);
            return assets;
        }

        private string IdentificarVeiculoNosAssets(long assetId, List<Asset> assets)
        {
            int total = assets?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (assetId == assets[i].AssetId)
                {

                    // Campo "Número de registro" da aplicação MixTelematics
                    return assets[i].RegistrationNumber;

                    // Campo "ID do ativo" da aplicação MixTelematics
                    //return assets[i].FmVehicleId.ToString();
                }
            }
            return null;
        }

        private string ObterClientApplicationName()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CLIENT_APPLICATION_NAME, this.conta.ListaParametrosAdicionais);
            return value;
        }

        private string ObterClientID()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CLIENT_ID, this.conta.ListaParametrosAdicionais);
        }

        private string ObterClientSecret()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CLIENT_SECRET, this.conta.ListaParametrosAdicionais);
        }

        private string ObterScopes()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_SCOPES, this.conta.ListaParametrosAdicionais);
        }

        private void AtualizarUltimaDataConsultada(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            if (cont > 0)
            {
                this.ultimaDataConsultada = posicoes[0].Data;
                for (int i = 1; i < cont; i++)
                {
                    if (posicoes[i].Data > this.ultimaDataConsultada)
                    {
                        this.ultimaDataConsultada = posicoes[i].Data.AddSeconds(1);
                    }
                }
            }
        }

        private void AtualizarUltimoSinceToken(CreatedSinceResult<Position> result)
        {
            this.ultimoSinceToken = result.GetSinceToken.Substring(0, 14);
        }

        private void ObterUltimaDataDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMA_DATA, dadosControle);
            this.ultimaDataConsultada = (String.IsNullOrWhiteSpace(value)) ? DateTime.Now : DateTime.Parse(value);
            Log($"Ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        private void SalvarUltimaDataNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMA_DATA, this.ultimaDataConsultada.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        private void ObterUltimaSinceTokenDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SINCE_TOKEN, dadosControle);
            this.ultimoSinceToken = (String.IsNullOrWhiteSpace(value)) ? DateTime.Now.AddMinutes(-190).ToString("yyyyMMddHHmmssff") : value;
            Log($"Ultimo SinceToken {this.ultimoSinceToken}", 2);
        }

        private void SalvarUltimaSinceTokenNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SINCE_TOKEN, this.ultimoSinceToken));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo SinceToken {this.ultimoSinceToken}", 2);
        }

        #endregion

    }

}

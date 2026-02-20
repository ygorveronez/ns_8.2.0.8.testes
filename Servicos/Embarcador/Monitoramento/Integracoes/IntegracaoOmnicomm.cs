using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoOmnicomm : Abstract.AbstractIntegracaoMonitoramentoREST
    {
        private static readonly string nameConfigSection = "Omnicomm";
        private static IntegracaoOmnicomm Instance;
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.TokenJWT jwt = new();

        private IntegracaoOmnicomm(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Omnicomm, 
            nameConfigSection, 
            cliente,
            "JWT"
        ) {}

        public static IntegracaoOmnicomm GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoOmnicomm(cliente);
            return Instance;
        }

        protected override void ComplementarConfiguracoes()
        {
        }

        protected override void Executar(ContaIntegracao configuracao)
        {
            this.conta = configuracao;
            IntegrarPosicao();
        }

        protected override void Preparar()
        {
        }

        protected override void Validar()
        {
        }

        #region Métodos privados 

        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes().GetAwaiter().GetResult();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RequisicaoConsultaPosicoesRastreSat MontarPayload()
        {
            List<int> ids = new();
            for (int i = 0; i < monitorar.Count; i++)
            {
                if (int.TryParse(monitorar[i].Value, out int id))
                    ids.Add(id);
            }
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RequisicaoConsultaPosicoesRastreSat(ids);
        }

        private void AdicionarPosicao(ref List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes, Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaConsultaPosicoesRastreSat rastreio)
        {
            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
            {
                Data = DateTimeOffset.FromUnixTimeMilliseconds(rastreio.Data).LocalDateTime,
                DataCadastro = DateTime.Now,
                DataVeiculo = DateTimeOffset.FromUnixTimeMilliseconds(rastreio.Data).LocalDateTime,
                IDEquipamento = rastreio.IdVeiculo.ToString(),
                Placa = rastreio.NumeroRegistro?.Split(new[] { " - " }, StringSplitOptions.None)[1].Replace("-", string.Empty),
                Latitude = rastreio.Latitude,
                Longitude = rastreio.Longitude,
                Velocidade = Convert.ToInt32(rastreio.Velocidade),
                Temperatura = 0,
                SensorTemperatura = false,
                Descricao = rastreio.Endereco,
                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Omnicomm,
                Ignicao = rastreio.Ignicao ? 1 : 0,
            });
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>> ObterPosicoes()
        {
            var posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                await Login();

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaConsultaPosicoesRastreSat> rastreios = await PostJson<
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RequisicaoConsultaPosicoesRastreSat, 
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaConsultaPosicoesRastreSat>,
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaErroRastreSat
                >(
                    conta.URI,
                    MontarPayload(),
                    jwt.AccessToken,
                    1
                );

                for ( int i = 0;  i < rastreios.Count;  i++ )
                {
                    AdicionarPosicao(ref posicoes, rastreios[i]);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes: " + ex.Message, 3);
            }

            return posicoes;
        }

        private async Task Login()
        {
            if (!string.IsNullOrWhiteSpace(jwt.AccessToken) && jwt.DataExpiracaoAccessToken > DateTime.UtcNow.AddMinutes(1))
                return;
            else if (!string.IsNullOrWhiteSpace(jwt.RefreshToken) && jwt.DataExpiracaoRefreshToken > DateTime.UtcNow.AddMinutes(1))
            {
                try
                {
                    AtualizarCredenciais(
                        await PostJson<
                            object,
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaRefresh,
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaErroRastreSat
                        >("auth/refresh?jwt=1", null, jwt.RefreshToken, 1, true)
                    );
                } catch (Exception ex)
                {
                    Log("Falha ao atualizar token via refresh token: " + ex.Message, 3);
                    LimparCredenciais();
                    throw;
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RequisicaoLogin payload = new()
            {
                Usuario = conta.Usuario,
                Senha = conta.Senha,
            };

            try
            {
                AtualizarCredenciais(
                    await PostJson<
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RequisicaoLogin,
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaLogin,
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaErroRastreSat
                    >("auth/login?jwt=1", payload, null, 1, true)
                );
            }
            catch (Exception ex)
            {
                Log("Falha ao realizar login: " + ex.Message, 3);
                LimparCredenciais();
                throw;
            }
        }

        private void AtualizarCredenciais(Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm.RespostaLogin result)
        {
            jwt.AccessToken = result.AccessToken;
            jwt.RefreshToken = result.RefreshToken;

            JwtSecurityToken access = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
            JwtSecurityToken refresh = new JwtSecurityTokenHandler().ReadJwtToken(result.RefreshToken);

            jwt.DataExpiracaoAccessToken = access.ValidTo;
            jwt.DataExpiracaoRefreshToken = refresh.ValidTo;
        }

        private void LimparCredenciais()
        {
            jwt = new Dominio.ObjetosDeValor.Embarcador.Integracao.TokenJWT();
        }
        
        #endregion
    }
}

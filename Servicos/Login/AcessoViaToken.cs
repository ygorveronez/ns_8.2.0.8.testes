using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Servicos.Login
{
    public class AcessoViaToken
    {
        #region Propriedades

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken _configuracaoAcessoViaToken;

        #endregion

        #region Construtores

        public AcessoViaToken(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoAcessoViaToken = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken(unitOfWork).Buscar();
        }

        #endregion

        #region Métodos Públicos

        public string GerarUrlAcesso(Dominio.Entidades.Usuario usuario, string url)
        {
            string token = GerarToken(usuario);
            url += token;

            return url;
        }

        public Dominio.Entidades.Usuario Autenticar(string token)
        {
            return ValidarToken(token);
        }

        #endregion

        #region Métodos Privados

        private string GerarToken(Dominio.Entidades.Usuario usuario)
        {
            SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracaoAcessoViaToken.ChaveSecreta));
            SigningCredentials credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.ClienteFornecedor.CodigoIntegracao.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuracaoAcessoViaToken.Emissor,
                audience: _configuracaoAcessoViaToken.Audiencia,
                claims: claims,
                notBefore: DateTime.Now,
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Dominio.Entidades.Usuario ValidarToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracaoAcessoViaToken.ChaveSecreta));

                TokenValidationParameters parametrosValidacao = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuracaoAcessoViaToken.Emissor,
                    ValidAudience = _configuracaoAcessoViaToken.Audiencia,
                    ValidateLifetime = false,
                    IssuerSigningKey = chave
                };

                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                SecurityToken tokenValidado;

                jwtHandler.ValidateToken(token, parametrosValidacao, out tokenValidado);

                var jwtToken = tokenValidado as JwtSecurityToken;
                var sub = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
                Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigoIntegracaoClienteFornecedor(sub);

                if ((usuario == null) || (usuario.ClienteFornecedor == null))
                    return null;

                return usuario;
            }
            catch (SecurityTokenException)
            {
                Servicos.Log.TratarErro($"Falha ao Autenticar: {token}", "AcessoViaToken");
                return null;
            }
        }


        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Base
{
    public class IntegracaoHUBOfertasBase
    {
        protected Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Endereco PopularEndereco(Dominio.Entidades.Localidade localidade, string rua, string numero, string complemento, string cep)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Endereco
            {
                Tipo = 1,
                CEP = cep,
                Rua = rua,
                Complemento = complemento,
                Latitude = localidade.Latitude.ToString(),
                Longitude = localidade.Longitude.ToString(),
                Numero = numero,
                Cidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Cidade
                {
                    NomeCidade = localidade.Descricao,
                    CodigoIBGE = localidade.CodigoIBGE.ToString(),
                    Estado = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Estado
                    {
                        NomeEstado = localidade.Estado.Descricao,
                        CodigoEstado = localidade.Estado.CodigoIBGE.ToString(),
                        Pais = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Pais
                        {
                            NomePais = localidade.Estado.Pais.Descricao,
                            CodigoPais = "076"
                        }
                    }
                }
            };
        }
    }
}

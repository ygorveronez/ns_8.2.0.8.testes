using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao.HUB.Base;
using Servicos.Embarcador.Integracao.HUB.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Tranportador
{
    public class IntegracaoTransportadorHUBOferta : IntegracaoHUBOfertasBase
    {
        #region Propriedades Protegidas
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly HubHttpClient _hubHttp;
        #endregion

        #region Construtores
        public IntegracaoTransportadorHUBOferta(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base()
        {
            _hubHttp = new HubHttpClient(new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unitOfWork).BuscarPrimeiroRegistro());
        }
        #endregion

        #region Metodos Publicos

        public async Task<HttpRequisicaoResposta> GerarIntegracaoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.Entidades.Empresa empresaMatriz = cargaIntegracaoHUB.Empresa.Matriz.FirstOrDefault() == null ? cargaIntegracaoHUB.Empresa : cargaIntegracaoHUB.Empresa.Matriz.FirstOrDefault();

            DadosEnvioTransportador dadosEnvioTransportador = new DadosEnvioTransportador();

            dadosEnvioTransportador.Transportadores = new() { PopularDadosTransportador(cargaIntegracaoHUB.Empresa, empresaMatriz) };

            respostaHttp = await _hubHttp.PostAsync("/api/Carriers/Queue", dadosEnvioTransportador);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador.RetornoIntegracao>(respostaHttp.conteudoResposta);

            if (respostaHttp == null && respostaHttp.httpStatusCode != HttpStatusCode.OK || !retornoIntegracao.Sucesso)
                throw new ServicoException(!string.IsNullOrEmpty(retornoIntegracao.StatusMensagem) ? retornoIntegracao.StatusMensagem : "Problema ao tentar integrar com Hub de Ofertas.");

            cargaIntegracaoHUB.Protocolo = retornoIntegracao.StatusMensagem;

            return respostaHttp;
        }
        #endregion

        #region Metodos Popular Envio Transportador
        private Transportador PopularTransportador(Dominio.Entidades.Empresa empresa)
        {
            return new Transportador
            {
                Email = empresa.Email,
                NumeroTelefone = empresa.Telefone,
                PessoaJuridica = new EntidadeLegal
                {
                    RazaoSocial = empresa.RazaoSocial,
                    NomeFantasia = empresa.NomeFantasia,
                    TelefoneEmpresa = empresa.Telefone,
                },
                Enderecos = new List<Endereco>
                {
                    PopularEndereco(empresa.Localidade, empresa.Endereco, empresa.Numero, empresa.Complemento, empresa.CEP)
                },
                Documentos = new List<Documento>
                {
                    new Documento
                    {
                        NumeroDocumento = empresa.CNPJ,
                        Tipo = new Tipo { Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a" },
                    }
                }
            };
        }

        protected DadosTransportador PopularDadosTransportador(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Empresa matriz)
        {
            return new DadosTransportador
            {
                Transportador = PopularTransportador(empresa),
                Matriz = PopularMatriz(matriz),
                Ativo = empresa.Status == "A"
            };
        }

        private Matriz PopularMatriz(Dominio.Entidades.Empresa matriz)
        {
            return new Matriz
            {
                PessoaJuridica = new EntidadeLegal
                {
                    RazaoSocial = matriz.RazaoSocial,
                    NomeFantasia = matriz.NomeFantasia,
                },
                Documentos = new List<Documento>
                {
                    new Documento
                    {
                        NumeroDocumento = matriz.CNPJ,
                        Tipo = new Tipo { Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a" },
                    }
                },
                Enderecos = new List<Endereco>
                {
                    PopularEndereco(matriz.Localidade, matriz.Endereco, matriz.Numero, matriz.Complemento, matriz.CEP)
                }
            };
        }

        #endregion
    }
}

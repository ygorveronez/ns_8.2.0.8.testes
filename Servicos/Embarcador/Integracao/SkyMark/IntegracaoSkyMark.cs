using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.SkyMark
{
    public class IntegracaoSkyMark
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSkyMark _configuracaoIntegracaoSkyMark;
        protected readonly CancellationToken _cancellationToken;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoSkyMark(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }

        #endregion Construtores

        #region Métodos Públicos

        public async Task IntegrarCargaDadosTransporteAsync(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;
            string requisicao = null;
            string resposta = null;
            string mensagem = null;

            try
            {
                ObterConfiguracaoIntegracao();

                Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte integracaoCarga = await ObterIntegracaoCargaAsync(cargaIntegracao.Carga);
                (requisicao, resposta, Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.RetornoIntegracaoAtualizacaoPerfilAutonomo respostaDeserializada) = await EnviarRequisicaoAsync(integracaoCarga);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao;

                if (respostaDeserializada.Resultado)
                {
                    mensagem = "Integração realizada com sucesso.";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    mensagem = $"{(string.IsNullOrWhiteSpace(respostaDeserializada?.Mensagem) ? "Sem conteúdo na mensagem de retorno" : respostaDeserializada.Mensagem)}.";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                AtualizarSituacaoIntegracao(cargaIntegracao, situacao, mensagem, requisicao, resposta);
            }
            catch (ServicoException excecao)
            {
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, excecao.Message, requisicao, resposta);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, "Ocorreu uma falha ao enviar a requisição.", requisicao, resposta);
            }
            finally
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
                await repCargaCargaIntegracao.AtualizarAsync(cargaIntegracao);
            }
        }

        #endregion

        #region Métodos Privados        

        private void ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark repositorioSkyMark = new Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark(_unitOfWork);
            _configuracaoIntegracaoSkyMark = repositorioSkyMark.BuscarPrimeiroRegistro();
        }

        private async Task<(string requisicao, string resposta, Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.RetornoIntegracaoAtualizacaoPerfilAutonomo respostaDeserializada)> EnviarRequisicaoAsync(Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte integracaoCarga)
        {
            string requisicao = _configuracaoIntegracaoSkyMark.Url + ObterRequestParams(integracaoCarga);

            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_configuracaoIntegracaoSkyMark.Url);
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(requisicao);
            string resposta = await httpResponseMessage.Content?.ReadAsStringAsync();
            Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.RetornoIntegracaoAtualizacaoPerfilAutonomo respostaDeserializada = null;

            if (!string.IsNullOrWhiteSpace(resposta))
            {
                XmlSerializer serializer = new(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.RetornoIntegracaoAtualizacaoPerfilAutonomo));

                using (StringReader reader = new(resposta))
                {
                    respostaDeserializada = (Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.RetornoIntegracaoAtualizacaoPerfilAutonomo)serializer.Deserialize(reader);
                }
            }

            return (requisicao, resposta, respostaDeserializada);
        }

        public string ObterRequestParams(Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte integracaoCarga)
        {
            if (integracaoCarga == null)
                throw new ArgumentNullException(nameof(integracaoCarga));

            PropertyInfo[] properties = typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string[] placasAdicionais = new[] { nameof(integracaoCarga.PlacaCarreta), nameof(integracaoCarga.PlacaBitrem), nameof(integracaoCarga.PlacaTreminhao) };

            IEnumerable<string> queryParams = properties
                .Select(p =>
                {
                    string campo = $"{p.Name}=";
                    object valor = p.GetValue(integracaoCarga);

                    if (placasAdicionais.Contains(p.Name) && valor == null)
                        return campo;

                    if (p.PropertyType == typeof(decimal))
                        return campo + (decimal)valor;

                    return campo + (valor?.ToString() ?? "\"\"");
                });

            return string.Join("&", queryParams);
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte> ObterIntegracaoCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark repositorioIntegracaoSkyMark = new Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte integracaoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.IntegracaoCargaDadosTransporte();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSkyMark integracaoSkyMark = await repositorioIntegracaoSkyMark.BuscarPrimeiroRegistroAsync();
            Dominio.ObjetosDeValor.Embarcador.Integracao.SkyMark.Percurso percurso = await repCargaPedido.BuscarPercursoPorCargaAsync(carga.Codigo, _cancellationToken);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.VeiculoVinculadoMaisVeiculaDaCarga> VeiculosCargas = await repCarga.BuscarVeiculosDaCargaAsync(new List<int> { carga.Codigo });

            integracaoCarga.Integracao = integracaoSkyMark.Integracao;
            integracaoCarga.Contrato = integracaoSkyMark.Contrato;
            integracaoCarga.Chave1 = integracaoSkyMark.ChaveUm;
            integracaoCarga.Chave2 = integracaoSkyMark.ChaveDois;
            integracaoCarga.Cpf = carga.CPFPrimeiroMotorista;
            integracaoCarga.Placa = VeiculosCargas.Count > 0 ? VeiculosCargas[0].Placa : null;
            integracaoCarga.PlacaCarreta = VeiculosCargas.Count > 1 ? VeiculosCargas[1].Placa : null;
            integracaoCarga.PlacaBitrem = VeiculosCargas.Count > 2 ? VeiculosCargas[2].Placa : null;
            integracaoCarga.PlacaTreminhao = VeiculosCargas.Count > 3 ? VeiculosCargas[3].Placa : null;
            integracaoCarga.TipoMercadoria = null;
            integracaoCarga.ValorCarga = carga.ValorFrete;
            integracaoCarga.CidadeOrigem = percurso.CidadeOrigem;
            integracaoCarga.UfOrigem = percurso.UfOrigem;
            integracaoCarga.CidadeDestino = percurso.CidadeDestino;
            integracaoCarga.UfDestino = percurso.UfDestino;

            return integracaoCarga;
        }

        private void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string requisicao, string resposta)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracao.SituacaoIntegracao = situacao;
            cargaIntegracao.ProblemaIntegracao = mensagem;

            servicoArquivoTransacao.Adicionar(cargaIntegracao, requisicao, resposta, "xml");
        }

        #endregion
    }
}
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.CTASmart
{
    public class IntegracaoCTASmart
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoCTASmart(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void SincronizarAbastecimentos(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart repIntegracaoCTASmart = new Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart>  configuracaoIntegracoesCTASmart = repIntegracaoCTASmart.BuscarTodos();

            if (!(configuracaoIntegracao?.PossuiIntegracaoCTASmart ?? false))
                return;

            try
            {
                // Código do produto (tipo_produto)
                // Parâmetro opcional. Consiste do identificador do tipo de produto e sua função é filtrar os abastecimentos somente do tipo informado.
                // Caso não informado serão recuperados abastecimentos de tipo "1"(Combustível).
                // 0 - Todos os tipos de produtos
                // 1 - Combustíveis
                // 2 - Aditivos

                var tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;

                for (int i = 1; i <= 2; i++)
                {
                    if (i == 1)
                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                    else if (i == 2)
                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                    foreach (var configuracaoIntegracaoCTASmart in configuracaoIntegracoesCTASmart)
                    {
                        string urlConsulta = $"{configuracaoIntegracaoCTASmart.URL}/SvWebSincronizaAbastecimentos?token={configuracaoIntegracaoCTASmart.Token}&tipo_produto={i}&formato=json&max_linhas=50"
                                             + (configuracaoIntegracaoCTASmart.DataInicio.HasValue ? $"&data_inicio={configuracaoIntegracaoCTASmart.DataInicio.Value:dd/MM/yyyy}" : string.Empty)
                                             + (!string.IsNullOrEmpty(configuracaoIntegracaoCTASmart.CodigoEmpresa) ? $"&empresa={configuracaoIntegracaoCTASmart.CodigoEmpresa}" : string.Empty);

                        HttpClient requisicao = CriarRequisicao(urlConsulta);
                        HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlConsulta).Result;
                        string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                        if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsulta retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsulta>(jsonRetorno);

                            if (retorno.Status.Codigo.Equals("001"))
                            {
                                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsultaAbastecimento objetoAbastecimento in retorno.Abastecimentos)
                                    GerarAbastecimento(tipoAbastecimento, objetoAbastecimento, configuracao, auditado, configuracaoIntegracaoCTASmart);
                            }
                            else
                                Log.TratarErro($"Retorno WS CTASmart: {retorno.Status.Codigo} - {retorno.Status.Mensagem}");
                        }
                        else
                            Log.TratarErro($"Falha ao conectar no WS CTASmart: {retornoRequisicao.StatusCode}");

                    }

                }
            }
            catch (Exception ex)
            {
                Log.TratarErro("Falha CTASmart: " + ex);
            }
        }

        #endregion

        #region Métodos Privados

        private void InformaSincronismoAbastecimento(int codigoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart configuracaoIntegracaoCTASmart)
        {
            try
            {
                string urlEnvio = $"{configuracaoIntegracaoCTASmart.URL}/SvWebInformaSincronismo?token={configuracaoIntegracaoCTASmart.Token}&formato=json";
                HttpClient requisicao = CriarRequisicao(urlEnvio);

                Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismo informaSincronismo = ObterDadosInformaSincronismo(codigoIntegracao);
                string jsonRequisicao = JsonConvert.SerializeObject(informaSincronismo, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlEnvio, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsulta retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsulta>(jsonRetorno);

                    if (!retorno.Status.Codigo.Equals("001"))
                        Log.TratarErro($"Retorno WS CTASmart - InformaSincronismo: {retorno.Status.Codigo} - {retorno.Status.Mensagem}");
                }
                else
                    Log.TratarErro($"Falha ao conectar no WS CTASmart - InformaSincronismo: {retornoRequisicao.StatusCode}");
            }
            catch (Exception ex)
            {
                Log.TratarErro("Falha CTASmart - InformaSincronismo: " + ex);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismo ObterDadosInformaSincronismo(int codigoIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismoAbastecimento> listaAbastecimentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismoAbastecimento>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismoAbastecimento abastecimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismoAbastecimento
            {
                Codigo = codigoIntegracao,
                Status = "SUCESSO"
            };

            listaAbastecimentos.Add(abastecimento);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.InformaSincronismo()
            {
                Abastecimentos = listaAbastecimentos.ToArray()
            };
        }

        private void GerarAbastecimento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart.RetornoConsultaAbastecimento objetoAbastecimento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart configuracaoIntegracaoCTASmart)
        {
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(_unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);

            if (repAbastecimento.AbastecimentoDuplicadoIntegracao(objetoAbastecimento.Codigo.ToString(), $"{objetoAbastecimento.DataInicio} {objetoAbastecimento.HoraInicio}".ToDateTime(), objetoAbastecimento.Volume.ToDecimal()))
            {
                InformaSincronismoAbastecimento(objetoAbastecimento.Codigo, configuracaoIntegracaoCTASmart);
                return;
            }

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(objetoAbastecimento.Veiculo.Placa);
            Dominio.Entidades.Cliente posto = !string.IsNullOrWhiteSpace(objetoAbastecimento.Posto.Cnpj) ? repCliente.BuscarPorCPFCNPJ(objetoAbastecimento.Posto.Cnpj.ToDouble()) : null;
            Dominio.Entidades.Usuario motorista = !string.IsNullOrWhiteSpace(objetoAbastecimento.Motorista.Cpf) ? repUsuario.BuscarMotoristaPorCPF(objetoAbastecimento.Motorista.Cpf) : null;            
            Dominio.Entidades.Produto produto = null;
            if (posto != null && !string.IsNullOrWhiteSpace(objetoAbastecimento.Combustivel.Codigo))
                produto = repPostoCombustivelTabelaValores.BuscarProdutoPorPessoa(objetoAbastecimento.Combustivel.Codigo, posto.CPF_CNPJ);
            if (produto == null)
                produto = !string.IsNullOrWhiteSpace(objetoAbastecimento.Combustivel.Codigo) ? repProduto.BuscarPorCodigoProduto(objetoAbastecimento.Combustivel.Codigo.ToString()) : null;

            Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento()
            {
                CodigoIntegracao = objetoAbastecimento.Codigo.ToString(),
                Data = $"{objetoAbastecimento.DataInicio} {objetoAbastecimento.HoraInicio}".ToDateTime(),
                DataAlteracao = DateTime.Now,
                Situacao = "A",
                Status = "A",
                Documento = "INTEGRAÇÃO " + objetoAbastecimento.Codigo,
                TipoAbastecimento = tipoAbastecimento,
                TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.Integracao,
                Litros = objetoAbastecimento.Volume.ToDecimal(),
                ValorUnitario = objetoAbastecimento.CustoUnitario.ToDecimal(),
                Kilometragem = objetoAbastecimento.Odometro.ToDecimal(),
                Horimetro = (int)objetoAbastecimento.Horimetro.ToDecimal(),
                Veiculo = veiculo,
                Posto = posto,
                NomePosto = posto?.Nome ?? "",
                Motorista = motorista,
                Produto = produto,
                Equipamento = null
            };

            if (abastecimento.Veiculo != null && abastecimento.Veiculo.TipoVeiculo == "1" && abastecimento.Veiculo.Equipamentos != null && abastecimento.Veiculo.Equipamentos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = abastecimento.Veiculo.Equipamentos.Where(c => c.EquipamentoAceitaAbastecimento == true)?.FirstOrDefault() ?? null;
                abastecimento.Equipamento = equipamento;
                if (abastecimento.Horimetro <= 0 && abastecimento.Kilometragem > 0)
                {
                    abastecimento.Horimetro = (int)abastecimento.Kilometragem;
                    abastecimento.Kilometragem = 0;
                }
            }
            //if (abastecimento.Equipamento != null && abastecimento.Horimetro > 0)
            //{
            //    abastecimento.Veiculo = null;
            //    abastecimento.Kilometragem = 0;
            //}

            if (produto != null)
            {
                if (produto.CodigoNCM.StartsWith("310210"))
                    abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                else
                    abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
            }

            Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);
            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, _unitOfWork, veiculo, null, configuracao);

            repAbastecimento.Inserir(abastecimento);
            Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, "Adicionado via integração CTASmart", _unitOfWork);

            InformaSincronismoAbastecimento(abastecimento.CodigoIntegracao.ToInt(), configuracaoIntegracaoCTASmart);
        }

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCTASmart));

            requisicao.BaseAddress = new Uri(url);

            return requisicao;
        }

        #endregion
    }
}

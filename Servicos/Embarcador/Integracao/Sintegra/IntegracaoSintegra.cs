using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Integracao.Sintegra
{
    public class IntegracaoSintegra
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoSintegra(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void ConsultarSimplesNacional(Dominio.Entidades.EmpresaIntegracao empresaIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.EmpresaIntegracao repEmpresaIntegracao = new Repositorio.EmpresaIntegracao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao?.URLSintegra) || string.IsNullOrWhiteSpace(configuracaoIntegracao?.TokenSintegra))
            {
                empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                empresaIntegracao.ProblemaIntegracao = "Não foi configurada a integração com o Sintegra, por favor verifique.";
                return;
            }

            empresaIntegracao.NumeroTentativas += 1;
            empresaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Empresa empresa = empresaIntegracao.Empresa;

                //CNPJ sem gasto de crédito: 06990590000123
                string urlConsulta = $"{configuracaoIntegracao.URLSintegra}?token={configuracaoIntegracao.TokenSintegra}&cnpj={empresa.CNPJ}&plugin=SN";

                HttpClient requisicao = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlConsulta).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Sintegra.RetornoConsultaSimplesNacional retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Sintegra.RetornoConsultaSimplesNacional>(jsonRetorno);

                if (retorno.Codigo == 0)
                {
                    bool simplesNacional = !retorno.SituacaoSimplesNacional.StartsWith("NÃO");

                    empresa.Initialize();
                    empresa.DataUltimaConsultaSintegra = DateTime.Now.Date;
                    empresa.DataProximaConsultaSintegra = DateTime.Now.Date.AddMonths(configuracaoIntegracao.IntervaloConsultaSintegra);

                    bool atualizouCadastro = false;
                    if (empresa.OptanteSimplesNacional && !simplesNacional)
                    {
                        empresa.OptanteSimplesNacional = false;
                        empresa.OptanteSimplesNacionalComExcessoReceitaBruta = false;
                        empresa.DataAtualizacao = DateTime.Now;
                        empresa.UsuarioAtualizacao = auditado.Usuario;
                        repEmpresa.Atualizar(empresa);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = empresa.GetChanges();
                        Servicos.Auditoria.Auditoria.Auditar(auditado, empresa, alteracoes, "Alterado do Simples Nacional para o Normal automaticamente via consulta no Sintegra", _unitOfWork);
                        atualizouCadastro = true;
                    }
                    else if (!empresa.OptanteSimplesNacional && simplesNacional)
                    {
                        empresa.OptanteSimplesNacional = true;
                        empresa.DataAtualizacao = DateTime.Now;
                        empresa.UsuarioAtualizacao = auditado.Usuario;
                        repEmpresa.Atualizar(empresa);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = empresa.GetChanges();
                        Servicos.Auditoria.Auditoria.Auditar(auditado, empresa, alteracoes, "Alterado do Normal para o Simples Nacional automaticamente via consulta no Sintegra", _unitOfWork);
                        atualizouCadastro = true;
                    }

                    if (!atualizouCadastro)
                        repEmpresa.Atualizar(empresa, auditado);

                    empresaIntegracao.SimplesNacional = simplesNacional;
                    empresaIntegracao.AtualizouCadastro = atualizouCadastro;
                    empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    empresaIntegracao.ProblemaIntegracao = "Consulta realizada com sucesso.";
                }
                else
                {
                    empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    empresaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                empresaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com o Sintegra";
            }

            servicoArquivoTransacao.Adicionar(empresaIntegracao, string.Empty, jsonRetorno, "json");

            repEmpresaIntegracao.Atualizar(empresaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSintegra));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return requisicao;
        }

        #endregion
    }
}

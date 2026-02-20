using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Servicos
{
    public class Trafegus
    {
        public static void EnviarViagem(ref Dominio.Entidades.SMViagemMDFe smViagemMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.SMViagemMDFe repSMViagemMDFe = new Repositorio.SMViagemMDFe(unidadeDeTrabalho);

            smViagemMDFe.DataIntegracao = DateTime.Now;

            if (smViagemMDFe.MDFe.Empresa.Configuracao == null && smViagemMDFe.MDFe.Empresa.EmpresaPai?.Configuracao != null)
            {
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
                smViagemMDFe.Mensagem = "Empresas sem configuração para integração com a Trafegus";

                repSMViagemMDFe.Atualizar(smViagemMDFe);
            }

            string usuario = string.Empty;
            string senha = string.Empty;

            if (!string.IsNullOrWhiteSpace(smViagemMDFe.MDFe.Empresa.Configuracao?.TrafegusUsuario))
            {
                usuario = smViagemMDFe.MDFe.Empresa.Configuracao?.TrafegusUsuario;
                senha = smViagemMDFe.MDFe.Empresa.Configuracao?.TrafegusSenha;
            }
            else if (!string.IsNullOrWhiteSpace(smViagemMDFe.MDFe.Empresa.EmpresaPai.Configuracao?.TrafegusUsuario))
            {
                usuario = smViagemMDFe.MDFe.Empresa.EmpresaPai.Configuracao?.TrafegusUsuario;
                senha = smViagemMDFe.MDFe.Empresa.EmpresaPai.Configuracao?.TrafegusSenha;
            }

            string url = !string.IsNullOrWhiteSpace(smViagemMDFe.MDFe.Empresa.Configuracao?.TrafegusUsuario) ? smViagemMDFe.MDFe.Empresa.Configuracao?.TrafegusURL : smViagemMDFe.MDFe.Empresa.EmpresaPai.Configuracao?.TrafegusURL;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(url))
            {
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
                smViagemMDFe.Mensagem = "Sem configuração para integração com a Trafegus";

                repSMViagemMDFe.Atualizar(smViagemMDFe);
            }

            string urlViagem = "viagem";
            string barra = url.Substring(url.Length - 1) == "/" ? "" : "/";
            string urlWebServiceViagem = url + barra + urlViagem;

            Servicos.Log.TratarErro("URL: "+ urlWebServiceViagem, "EnviarViagemTrafegus");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem integracaoViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem();

            viagem.documento_transportador = smViagemMDFe.MDFe.Empresa.CNPJ;
            viagem.viag_codigo_externo = smViagemMDFe.MDFe.Numero.ToString() + "_" + smViagemMDFe.MDFe.Serie.Numero.ToString();

            viagem.veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo>
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo { placa = smViagemMDFe.MDFe.Veiculos.FirstOrDefault().Placa }
            };

            foreach (var reboque in smViagemMDFe.MDFe.Reboques)
            {
                viagem.veiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo { placa = reboque.Placa });
            }

            //Motoristas
            viagem.motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista>();

            foreach (var motoristaCarga in smViagemMDFe.MDFe.Motoristas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista
                {
                    cpf_moto = motoristaCarga.CPF,
                };

                viagem.motoristas.Add(motorista);
            }

            //Rota
            //viagem.coordenadas = cargaRotaFrete.PolilinhaRota;
            //viagem.viag_distancia = Convert.ToDouble(carga.DadosSumarizados.Distancia);

            //viagem.origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Origem()
            //{
            //    vloc_descricao = origem.Descricao,
            //    refe_latitude = origem.Latitude,
            //    refe_longitude = origem.Longitude
            //};

            //viagem.destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Destino()
            //{
            //    vloc_descricao = destino.Descricao,
            //    refe_latitude = destino.Latitude,
            //    refe_longitude = destino.Longitude,
            //    conhecimentos = ObterConhecimento(carga, destino?.Cliente?.CPF_CNPJ ?? 0)
            //};

            viagem.locais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais>();
            foreach (var municipioDescarregamento in smViagemMDFe.MDFe.MunicipiosDescarregamento)
            {
                foreach (var documento in municipioDescarregamento.Documentos)
                {
                    var locais = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais
                    {
                        vloc_descricao = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Descricao : documento.CTe.Destinatario.Descricao,
                        cep = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.CEP_SemFormato : documento.CTe.Destinatario.CEP_SemFormato,
                        logradouro = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Endereco : documento.CTe.Destinatario.Endereco,
                        bairro = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Bairro : documento.CTe.Destinatario.Bairro,
                        numero = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Numero : documento.CTe.Destinatario.Numero,
                        complemento = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Complemento : documento.CTe.Destinatario.Complemento,
                        cida_descricao_ibge = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Localidade.CodigoIBGE.ToString() : documento.CTe.Destinatario.Localidade.CodigoIBGE.ToString(),
                        sigla_estado = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.Localidade.Estado.Sigla : documento.CTe.Destinatario.Localidade.Estado.Sigla
                    };

                    locais.conhecimentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento>();
                    var conhecimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento
                    {
                        vlco_cpf_cnpj = documento.CTe.Recebedor != null ? documento.CTe.Recebedor.CPF_CNPJ : documento.CTe.Destinatario.CPF_CNPJ,
                        vlco_numero = documento.CTe.Numero.ToString(),
                        vlco_valor = Convert.ToDouble(documento.CTe.ValorAReceber)
                    };
                    locais.conhecimentos.Add(conhecimento);

                    viagem.locais.Add(locais);
                }
            }

            //viagem.viag_pgpg_codigo = configuracaoIntegracaoTrafegus.PGR;

            viagem.viag_numero_manifesto = smViagemMDFe.MDFe.Descricao;
            viagem.viag_peso_total = Convert.ToDouble(smViagemMDFe.MDFe.PesoBrutoMercadoria);

            //if (carga?.TipoDeCarga?.ControlaTemperatura ?? false)
            //{
            //    viagem.temperatura = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Temperatura
            //    {
            //        descricao = carga?.TipoDeCarga?.FaixaDeTemperatura?.Descricao ?? string.Empty,
            //        de = carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial != null ? Convert.ToInt32(carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0) : 0,
            //        ate = carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal != null ? Convert.ToInt32(carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0) : 0
            //    };

            //}

            viagem.viag_valor_carga = Convert.ToDouble(smViagemMDFe.MDFe.ValorTotalMercadoria); ;
            //viagem.viag_descricao_carga = carga?.TipoDeCarga?.Descricao;

            viagem.viag_previsao_inicio = smViagemMDFe.MDFe.DataEmissao.Value.ToString();
            viagem.viag_previsao_fim = smViagemMDFe.MDFe.DataEmissao.Value.AddDays(5).ToString();

            //int.TryParse(carga.Rota?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty, out int codigoIntegracaoGerenciadora);
            //viagem.rota_codigo = codigoIntegracaoGerenciadora;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem IntegracaoViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem
            {
                viagem = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem>()
            };

            IntegracaoViagem.viagem.Add(viagem);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemFalha = string.Empty;
            
            try
            {
                jsonRequest = JsonConvert.SerializeObject(IntegracaoViagem);
                Servicos.Log.TratarErro("Request: " + jsonRequest, "EnviarViagemTrafegus");
                jsonResponse = ExecutarMetodo("POST", usuario, senha, urlWebServiceViagem, jsonRequest);
                Servicos.Log.TratarErro("Response: " + jsonResponse, "EnviarViagemTrafegus");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarViagemTrafegus");
                mensagemFalha = ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(mensagemFalha))
            {
                SalvarLogIntegracao(jsonRequest, mensagemFalha, smViagemMDFe, unidadeDeTrabalho);

                smViagemMDFe.Mensagem = "Não foi possível enviar integração para a Trafegus, consulte o log.";
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
            }
            else
            {
                SalvarLogIntegracao(jsonRequest, jsonResponse, smViagemMDFe, unidadeDeTrabalho);

                if (jsonResponse.Contains("Fatal error"))
                {
                    smViagemMDFe.Mensagem = "Não foi possível enviar integração para a Trafegus, consulte o log.";
                    smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
                }
                else
                {
                    var retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);

                    bool sucesso = (retorno?.error == null || retorno.error.Count == 0) || retorno?.sucesso?.Count > 0;
                    smViagemMDFe.Mensagem = sucesso ? "Integração gerada com sucesso" : (retorno?.error != null ? String.Join(",", (from erro in retorno.error select erro.mensagem)) : "erro ao integrar viagem");

                    if (sucesso)
                    {
                        smViagemMDFe.CodigoIntegracaoViagem = retorno?.sucesso?.FirstOrDefault().cod_viagem;
                        smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Sucesso;
                    }
                    else
                    {
                        smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
                    }
                }
            }

            repSMViagemMDFe.Atualizar(smViagemMDFe);           
        }

        private static string ExecutarMetodo(string Metodo, string usuario, string senha, string url, string json)
        {
            WebRequest request = HttpWebRequest.Create(url);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{usuario}:{senha}"));
            request.ContentType = "application/json";
            request.Method = Metodo;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static void SalvarLogIntegracao(string request, string response, Dominio.Entidades.SMViagemMDFe smViagemMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.SMViagemMDFeLog repSMViagemMDFeLog = new Repositorio.SMViagemMDFeLog(unidadeDeTrabalho);

            Dominio.Entidades.SMViagemMDFeLog log = new Dominio.Entidades.SMViagemMDFeLog()
            {
                SMViagemMDFe = smViagemMDFe,
                DataHora = DateTime.Now,
                Requisicao = request,
                Resposta = response
            };

            repSMViagemMDFeLog.Inserir(log);
        }
    }
}

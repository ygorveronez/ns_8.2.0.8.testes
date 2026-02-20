using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.AngelLira
{
    public class Rota
    {
        public static void ObterRotasAngelLira(Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";

            InspectorBehavior inspector = new InspectorBehavior();

            WSAngelLira.ValidationSoapHeader validationSoapHeader = IntegrarCargaAngelLira.ObterCabecalho(unitOfWork, out url);
            WSAngelLira.WSImportSoapClient svcAngelLira = IntegrarCargaAngelLira.ObterWSImportClient(url);

            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.RotaFreteDestinatarios repRotaFreteDestinatarios = new Repositorio.RotaFreteDestinatarios(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            svcAngelLira.Endpoint.EndpointBehaviors.Add(inspector);

            WSAngelLira.RetornoRota[] retorno = svcAngelLira.GetRotas(validationSoapHeader, 0, null, null, false);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

            int codigoUltimaRota = 0;

            Servicos.Log.TratarErro("Iniciou a integração de rotas com a AngelLira.");

            do
            {
                if (retorno.Count() > 0)
                    codigoUltimaRota = retorno.Max(o => o.codigo);
                else
                    codigoUltimaRota = 0;

                foreach (WSAngelLira.RetornoRota retornoRota in retorno)
                {
                    bool alterouRota = false;

                    Dominio.Entidades.RotaFrete rotaFrete = repRota.BuscarPorCodigoIntegracaoETipoIntegracao(retornoRota.codigo.ToString(), tipoIntegracao.Codigo, true);

                    unitOfWork.Start();

                    List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosExistentes = new List<RotaFreteDestinatarios>();

                    if (rotaFrete == null)
                    {
                        rotaFrete = new Dominio.Entidades.RotaFrete()
                        {
                            CodigoIntegracao = retornoRota.codigo.ToString(),
                            TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                            TipoRota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida,
                            AdicionadoViaIntegracao = true,
                            TipoIntegracao = tipoIntegracao
                        };
                    }
                    else
                    {
                        rotaFrete.Initialize();

                        destinatariosExistentes = repRotaFreteDestinatarios.BuscarPorRocartaFrete(rotaFrete.Codigo);
                    }

                    rotaFrete.Ativo = retornoRota.status == 0;
                    rotaFrete.Descricao = Utilidades.String.Left(retornoRota.descricao, 100);
                    rotaFrete.Quilometros = (decimal)retornoRota.distancia;
                    rotaFrete.TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : (int)Math.Round(retornoRota.tempo, 0, MidpointRounding.AwayFromZero);

                    if (double.TryParse(Utilidades.String.OnlyNumbers(retornoRota.origem), out double cpfCnpjOrigem) && cpfCnpjOrigem > 0D)
                    {
                        Dominio.Entidades.Cliente clienteOrigem = repCliente.BuscarPorCPFCNPJ(cpfCnpjOrigem);

                        if (clienteOrigem != null)
                        {
                            if (rotaFrete.Remetente == null || rotaFrete.Remetente.CPF_CNPJ != clienteOrigem.CPF_CNPJ)
                                alterouRota = true;

                            rotaFrete.Remetente = clienteOrigem;
                        }
                    }

                    if (rotaFrete.Codigo > 0)
                        repRota.Atualizar(rotaFrete, auditado);
                    else
                        repRota.Inserir(rotaFrete, auditado);

                    List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
                    bool possuiDestinatarioNaoEncontrado = false;

                    double.TryParse(Utilidades.String.OnlyNumbers(retornoRota.destino), out double cpfCnpjDestino);

                    Dominio.Entidades.Cliente clienteDestino = cpfCnpjDestino > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjDestino) : null;

                    if (clienteDestino != null)
                    {
                        destinatarios.Add(clienteDestino);

                        if (retornoRota.entregas != null && retornoRota.entregas.Length > 0)
                        {
                            foreach (string entrega in retornoRota.entregas)
                            {
                                double.TryParse(Utilidades.String.OnlyNumbers(entrega), out double cpfCnpjEntrega);

                                Dominio.Entidades.Cliente clienteEntrega = cpfCnpjEntrega > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEntrega) : null;

                                if (clienteEntrega != null && !destinatarios.Any(o => o.CPF_CNPJ == clienteEntrega.CPF_CNPJ))
                                    destinatarios.Add(clienteEntrega);
                                else
                                {
                                    possuiDestinatarioNaoEncontrado = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        possuiDestinatarioNaoEncontrado = true;
                    }

                    if (!possuiDestinatarioNaoEncontrado)
                    {
                        if (destinatariosExistentes.Count != destinatarios.Count)
                            alterouRota = true;
                        else
                        {
                            foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
                            {
                                if (!destinatariosExistentes.Any(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ))
                                    alterouRota = true;
                            }
                        }

                        if (destinatariosExistentes.Count > 0)
                        {
                            List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosDeletar = (from obj in destinatariosExistentes where !destinatarios.Any(o => o.CPF_CNPJ == obj.Cliente.CPF_CNPJ) select obj).ToList();

                            for (var i = 0; i < destinatariosDeletar.Count; i++)
                            {
                                repRotaFreteDestinatarios.Deletar(destinatariosDeletar[i]);
                            }

                        }

                        int ordem = 0;
                        foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
                        {
                            Dominio.Entidades.RotaFreteDestinatarios destinatarioExistente = destinatariosExistentes.Where(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ).FirstOrDefault();

                            if (destinatarioExistente != null)
                                continue;

                            destinatarioExistente = new RotaFreteDestinatarios()
                            {
                                Cliente = destinatario,
                                Ordem = ordem,
                                RotaFrete = rotaFrete
                            };

                            repRotaFreteDestinatarios.Inserir(destinatarioExistente);

                            ordem += 1;
                        }
                    }

                    if (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && rotaFrete.Remetente != null && rotaFrete.Destinatarios.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(retornoRota.polilinha) && rotaFrete.PolilinhaRota != retornoRota.polilinha)
                        {
                            rotaFrete.PolilinhaRota = retornoRota.polilinha;
                            rotaFrete.RotaRoteirizada = false;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                            rotaFrete.ApenasObterPracasPedagio = true;

                            repRota.Atualizar(rotaFrete);
                        }
                        else if (alterouRota)
                        {
                            rotaFrete.RotaRoteirizada = false;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;

                            repRota.Atualizar(rotaFrete);
                        }
                    }

                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }

                if (codigoUltimaRota > 0)
                    retorno = svcAngelLira.GetRotas(validationSoapHeader, codigoUltimaRota, null, null, false);

            } while (codigoUltimaRota > 0);

            Servicos.Log.TratarErro("Finalizou a integração de rotas com a AngelLira.");
        }
    }
}

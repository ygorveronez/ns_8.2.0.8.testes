using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarColetasEntregas(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out List<int> idColetasEntregas, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idColetasEntregas = new List<int>();

            //Agrupar Registros por Rementete e Destinatário
            var queryGroup = cargaCTes.GroupBy(x => new { x.CTe.Remetente, x.CTe.Destinatario }).Select(y => new { Remetente = y.Key.Remetente, Destinatario = y.Key.Destinatario, listDoc = y });

            foreach (var item in queryGroup)
            {
                int idClienteOrigem = 0;
                int idClienteOrigemEndereco = 0;
                int idClienteDestino = 0;
                int idClienteDestinoEndereco = 0;
                int idColetaEntrega = 0;

                if (IntegrarCliente(item.Remetente.Cliente, null, out idClienteOrigem, out idClienteOrigemEndereco, out mensagemErro, logs) &&
                    IntegrarCliente(item.Destinatario.Cliente, null, out idClienteDestino, out idClienteDestinoEndereco, out mensagemErro, logs) &&
                    IntegrarColetaEntrega(item.listDoc.ToList(), cargaIntegracao, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco, out idColetaEntrega, out mensagemErro, logs)
                    )
                    idColetasEntregas.Add(idColetaEntrega);
                else
                    return false;
            }

            return true;
        }

        private bool IntegrarColetaEntrega(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco, out int idColetaEntrega, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idColetaEntrega = 0;
            string mensagemLog = string.Empty;
            try
            {
                bool sucesso = false;

                object envioWS = ObterColetaEntrega(cargaCTes, cargaIntegracao, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco);

                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "coletas-entregas", this.tokenAutenticacao);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de erro de coleta/entrega A52 (primeira): {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da coleta/entrega; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retColetaEntrega retColetaEntrega = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if (count == 2 && retorno.statusCode == "409")
                                retColetaEntrega = message.ToString().FromJson<retColetaEntrega>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da coleta/entrega.";
                            sucesso = false;
                        }
                        else if (retColetaEntrega == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da coleta/entrega; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idColetaEntrega = (int)retColetaEntrega.id;
                            sucesso = true;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da coleta/entrega.";
                    mensagemLog = mensagemErro;
                    sucesso = false;
                }
                else
                {
                    retColetaEntrega retColetaEntrega = retornoWS.jsonRetorno.ToString().FromJson<retColetaEntrega>();
                    idColetaEntrega = (int)retColetaEntrega.id;
                    mensagemLog = "IntegrarColetaEntrega: Coleta/entrega Cadastrado com sucesso.";
                    sucesso = true;
                }

                logs?.Add(new LogIntegracao
                {
                    NomeEtapa = $"IntegrarColetaEntrega",
                    JsonEnvio = retornoWS.jsonEnvio,
                    JsonRetorno = retornoWS.jsonRetorno
                });
               
                SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados da coleta/entrega.";
                return false;
            }
        }

        private bool IntegrarColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco, out string jsonEnvio, out string jsonRetorno, out int idColetaEntrega, out string mensagemErro)
        {
            mensagemErro = null;
            jsonEnvio = null;
            jsonRetorno = null;
            idColetaEntrega = 0;

            try
            {
                bool sucesso = false;
                bool primeiroEnvio = string.IsNullOrWhiteSpace(pedidoIntegracao.CodigoIntegracaoIntegradora);

                object envioWS = ObterColetaEntrega(pedidoIntegracao, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco);

                //Transmite o arquivo
                retornoWebService retornoWS = null;

                if (primeiroEnvio)
                    retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "coletas-entregas", this.tokenAutenticacao);
                else
                    retornoWS = this.TransmitirRepom(enumTipoWS.PUT, envioWS, $"coletas-entregas/{pedidoIntegracao.CodigoIntegracaoIntegradora}", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de erro de coleta/entrega A52 (PUT): {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da coleta/entrega; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retColetaEntrega retColetaEntrega = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if (count == 2 && retorno.statusCode == "409")
                                retColetaEntrega = message.ToString().FromJson<retColetaEntrega>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da coleta/entrega.";
                            sucesso = false;
                        }
                        else if (retColetaEntrega == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da coleta/entrega; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idColetaEntrega = (int)retColetaEntrega.id;
                            sucesso = true;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da coleta/entrega.";
                    sucesso = false;
                }
                else
                {
                    retColetaEntrega retColetaEntrega = retornoWS.jsonRetorno.ToString().FromJson<retColetaEntrega>();
                    idColetaEntrega = (int)retColetaEntrega.id;

                    sucesso = true;
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados da coleta/entrega.";
                return false;
            }
        }

        private envColetaEntrega ObterColetaEntrega(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco)
        {
            envColetaEntrega retorno = new envColetaEntrega();

            retorno.identificador = null;

            // 0 - Entrega, 1 - Coleta
            retorno.tipo = 0;

            // 0 - Vazio, 1 - Carregado
            retorno.tipoTransporte = 1;
            retorno.dataPrevisaoChegadaOrigem = null;
            retorno.dataPrevisaoSaidaOrigem = null;
            retorno.dataPrevisaoChegadaDestino = null;
            retorno.dataPrevisaoSaidaDestino = null;
            retorno.idClienteOrigem = idClienteOrigem;
            retorno.clienteOrigem = null;
            retorno.idClienteOrigemEndereco = idClienteOrigemEndereco;
            retorno.idClienteDestino = idClienteDestino;
            retorno.clienteDestino = null;
            retorno.idClienteDestinoEndereco = idClienteDestinoEndereco;
            retorno.ordem = null;
            retorno.ativo = true;
            retorno.documentos = this.ObterColetaEntregaDocumentos(cargaCTes, cargaIntegracao);
            retorno.conjuntos = null;

            return retorno;
        }

        private List<envColetaEntregaDocumentos> ObterColetaEntregaDocumentos(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            List<envColetaEntregaDocumentos> retorno = new List<envColetaEntregaDocumentos>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                envColetaEntregaDocumentos documento = new envColetaEntregaDocumentos();

                documento.chaveCte = cargaCTe.CTe.ChaveAcesso;
                documento.chaveNf = null;
                documento.produto = cargaIntegracao.Carga.TipoDeCarga.Descricao;
                documento.peso = cargaCTe.CTe.Peso;
                documento.volumes = cargaCTe.CTe.Volumes;
                documento.identificador = null;
                documento.numeroCte = cargaCTe.CTe.Numero;
                documento.numeroNf = null;
                documento.serieCte = cargaCTe.CTe.Serie.Numero;
                documento.serieNf = null;
                documento.dataEmissaoNf = null;
                documento.valorCarga = cargaCTe.CTe.ValorTotalMercadoria;
                documento.valorICMS = cargaCTe.CTe.ValorICMS;
                documento.freteLiquido = cargaCTe.CTe.ValorAReceber;
                documento.freteBruto = cargaCTe.CTe.ValorFrete;
                documento.ativo = true;

                retorno.Add(documento);
            }

            return retorno;
        }

        private envColetaEntrega ObterColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco)
        {
            envColetaEntrega retorno = new envColetaEntrega();

            retorno.identificador = null;
            retorno.idOperacaoLogistico = null;
            retorno.idRota = null;
            retorno.operacaoLogistico = null;

            // 0 - Entrega, 1 - Coleta
            retorno.tipo = 1;

            // 0 - Vazio, 1 - Carregado
            retorno.tipoTransporte = 1;
            retorno.dataChegadaOrigem = null;
            retorno.dataPrevisaoChegadaOrigem = null;
            retorno.dataSaidaOrigem = null;
            retorno.dataPrevisaoSaidaOrigem = pedidoIntegracao.Pedido.DataPrevisaoInicioViagem != null ? ((DateTime)pedidoIntegracao.Pedido.DataPrevisaoInicioViagem).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataPrevisaoChegadaDestino = pedidoIntegracao.Pedido.DataPrevisaoChegadaDestinatario != null ? ((DateTime)pedidoIntegracao.Pedido.DataPrevisaoChegadaDestinatario).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataChegadaDestino = null;
            retorno.dataPrevisaoSaidaDestino = null;
            retorno.dataSaidaDestino = null;
            retorno.idClienteOrigem = idClienteOrigem;
            retorno.clienteOrigem = null;
            retorno.idClienteOrigemEndereco = idClienteOrigemEndereco;
            retorno.enderecoClienteOrigem = null;
            retorno.idClienteDestino = idClienteDestino;
            retorno.clienteDestino = null;
            retorno.idClienteDestinoEndereco = idClienteDestinoEndereco;
            retorno.enderecoClienteDestino = null;
            retorno.ordem = null;
            retorno.ativo = true;
            retorno.documentos = null;
            retorno.conjuntos = null;

            return retorno;
        }

        #endregion Métodos Privados

    }
}

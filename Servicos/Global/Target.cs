using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos
{
    public class Target : ServicoBase
    {        
        public Target(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool ComprarValePedagioMDFe(ref Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
            Repositorio.ReboqueMDFe repReboquesMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);

            try
            {
                valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
                valePedagioMDFeCompra.TentativaReenvio += 1;

                int quantidadeEixosConjunto = 0;
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                Dominio.Entidades.Veiculo veiculoTracao = null;
                if (veiculoMDFe == null)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = "Veículo do MDFe não informado.";
                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }
                else
                {
                    veiculoTracao = repVeiculo.BuscarPorPlaca(valePedagioMDFeCompra.MDFe.Empresa.Codigo, veiculoMDFe.Placa);
                    if (veiculoTracao == null)
                    {
                        valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                        valePedagioMDFeCompra.Mensagem = "Veículo " + veiculoTracao.Placa + " não cadastrado.";
                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                        return false;
                    }
                    else if (veiculoTracao.TipoDoVeiculo == null)
                    {
                        valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                        valePedagioMDFeCompra.Mensagem = "Veículo " + veiculoTracao.Placa + " sem tipo configurado.";
                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                        return false;
                    }
                    quantidadeEixosConjunto = veiculoTracao.TipoDoVeiculo.NumeroEixos;
                }

                Dominio.Entidades.Veiculo veiculoReboque = null;
                List<Dominio.Entidades.ReboqueMDFe> reboquesMDFe = repReboquesMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                if (reboquesMDFe != null && reboquesMDFe.Count > 0)
                {

                    for (var i = 0; i < reboquesMDFe.Count(); i++)
                    {
                        veiculoReboque = repVeiculo.BuscarPorPlaca(valePedagioMDFeCompra.MDFe.Empresa.Codigo, reboquesMDFe[i].Placa);
                        if (veiculoReboque == null)
                        {
                            valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                            valePedagioMDFeCompra.Mensagem = "Veículo Reboque " + veiculoReboque.Placa + " não cadastrado.";
                            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                            return false;
                        }
                        else if (veiculoReboque.TipoDoVeiculo == null)
                        {
                            valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                            valePedagioMDFeCompra.Mensagem = "Veículo Reboque " + veiculoReboque.Placa + " sem tipo configurado.";
                            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                            return false;
                        }
                        quantidadeEixosConjunto += veiculoReboque.TipoDoVeiculo.NumeroEixos;
                    }

                }

                valePedagioMDFeCompra.QuantidadeEixos = quantidadeEixosConjunto;

                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = new ServicoTarget.ValePedagio.AutenticacaoRequest()
                {
                    Usuario = valePedagioMDFeCompra.IntegracaoUsuario,
                    Senha = valePedagioMDFeCompra.IntegracaoSenha,
                    Token = valePedagioMDFeCompra.IntegracaoToken // Target retornou que parametro não esta sendo utilizado, o Token é configurado/enviado na senha
                };

                string mensagemRetorno = string.Empty;

                List<Dominio.Entidades.MotoristaMDFe> listaMotoristaMDFe = repMotoristaMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                Dominio.Entidades.Usuario motorista = null;
                if (listaMotoristaMDFe != null && listaMotoristaMDFe.Count > 0)
                    motorista = repMotorista.BuscarMotoristaPorCPF(valePedagioMDFeCompra.MDFe.Empresa.Codigo, listaMotoristaMDFe.FirstOrDefault().CPF);

                //if (motorista == null)
                //{
                //    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                //    valePedagioMDFeCompra.Mensagem = "Motorista CPF " + listaMotoristaMDFe.FirstOrDefault().CPF + " não cadastrado.";
                //    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                //    return false;
                //}

                if (string.IsNullOrWhiteSpace(veiculoTracao.TipoDoVeiculo.CodigoIntegracao))
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = "Tipo " + veiculoTracao.TipoDoVeiculo.Descricao + " sem código para integração configurado.";
                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                int idModoCompraValePedagio = 2; //1 == Cartão // 2 == ViaFacil // 5 == Veloe // 6 == ConectCar // 7 == MoveMais // 9 == Taggy                

                if (veiculoTracao.ModoCompraValePedagioTarget.HasValue)
                {
                    switch (veiculoTracao.ModoCompraValePedagioTarget.Value)
                    {
                        case ModoCompraValePedagioTarget.PedagioTagViaFacil:
                            idModoCompraValePedagio = 2;
                            break;
                        case ModoCompraValePedagioTarget.PedagioTagVeloe:
                            idModoCompraValePedagio = 5;
                            break;
                        case ModoCompraValePedagioTarget.PedagioConectCar:
                            idModoCompraValePedagio = 6;
                            break;
                        case ModoCompraValePedagioTarget.PedagioTagMoveMais:
                            idModoCompraValePedagio = 7;
                            break;
                        case ModoCompraValePedagioTarget.PedagioTagTaggy:
                            idModoCompraValePedagio = 9;
                            break;
                        default:
                            idModoCompraValePedagio = 2;
                            break;
                    }
                }

                //Buscar rota pelos IBGE
                ServicoTarget.ValePedagio.ListarRotaClienteRequest consultaRotaIBGE = new ServicoTarget.ValePedagio.ListarRotaClienteRequest()
                {
                    CodigoIBGEOrigem = valePedagioMDFeCompra.IBGEInicio,
                    CodigoIBGEDestino = valePedagioMDFeCompra.IBGEFim
                };

                //Buscar Rota por IBGE
                mensagemRetorno = string.Empty;

                Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                ServicoTarget.ValePedagio.RotaResponse rota = BuscarRotaIBGE(autenticacao, consultaRotaIBGE, unidadeDeTrabalho, ref mensagemRetorno, ref inspectorBuscarRota);
                SalvarXMLIntegracao(inspectorBuscarRota, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.BuscarRotaIBGE, unidadeDeTrabalho);

                if (rota != null)
                {
                    //int.TryParse(veiculo.TipoDoVeiculo.CodigoIntegracao, out int categoriaVeiculo);
                    int categoriaVeiculo = this.ObterCategoriaPorEixos(quantidadeEixosConjunto, veiculoTracao.ModeloVeicularCarga?.PadraoEixos);

                    //Buscar Custo da Rota
                    ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest consultaCustoRota = new ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest()
                    {
                        CategoriaVeiculo = categoriaVeiculo,
                        IdRotaModelo = rota.IdRotaCliente,
                        ModoPagamentoRota = 2
                    };


                    mensagemRetorno = string.Empty;
                    Servicos.Models.Integracao.InspectorBehavior inspectorCustoRota = new Servicos.Models.Integracao.InspectorBehavior();
                    ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse custoRota = BuscarCustoRota(autenticacao, consultaCustoRota, unidadeDeTrabalho, ref mensagemRetorno, ref inspectorCustoRota);
                    SalvarXMLIntegracao(inspectorCustoRota, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.BuscarCustoRota, unidadeDeTrabalho);

                    if (custoRota != null)
                    {
                        if (custoRota.ValorPedagioTotal > 0)
                        {
                            valePedagioMDFeCompra.Valor = custoRota.ValorPedagioTotal;

                            ServicoTarget.ValePedagio.CompraValePedagioRequest compraValePedagio = new ServicoTarget.ValePedagio.CompraValePedagioRequest();

                            compraValePedagio.IdModoCompraValePedagio = idModoCompraValePedagio;
                            compraValePedagio.IdRotaModelo = rota.IdRotaCliente;
                            compraValePedagio.CodigoCategoriaVeiculo = categoriaVeiculo;
                            compraValePedagio.MunicipioOrigemCodigoIBGE = valePedagioMDFeCompra.IBGEInicio;
                            compraValePedagio.MunicipioDestinoCodigoIBGE = valePedagioMDFeCompra.IBGEFim;
                            compraValePedagio.Placa = veiculoTracao.Placa;

                            compraValePedagio.NumeroCartao = string.Empty;
                            compraValePedagio.MotoristaNome = listaMotoristaMDFe.FirstOrDefault().Nome;
                            compraValePedagio.MotoristaCPF = listaMotoristaMDFe.FirstOrDefault().CPF;
                            compraValePedagio.MotoristaRNTRC = veiculoTracao.RNTRC > 0 ? veiculoTracao.RNTRC.ToString() : veiculoTracao.Empresa.RegistroANTT;

                            string transportadoraCpfCnpj = string.Empty;

                            if (veiculoTracao?.Tipo == "T" && veiculoTracao.Proprietario != null)
                                transportadoraCpfCnpj = veiculoTracao.Proprietario.CPF_CNPJ_SemFormato;
                            else
                                transportadoraCpfCnpj = valePedagioMDFeCompra.MDFe.Empresa?.CNPJ_SemFormato ?? string.Empty;

                            compraValePedagio.CpfCnpjTransportador = transportadoraCpfCnpj;

                            compraValePedagio.IdIntegrador = string.Empty;
                            //compraValePedagio.CodigoCentroDeCusto = null;
                            compraValePedagio.NumeroDocumentoEmbarque = string.Empty;

                            string itemFinanceiro = valePedagioMDFeCompra.MDFe.Codigo.ToString();
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCTesPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                            if (listaCTes.Count > 0 && listaCTes.FirstOrDefault().Documentos.Count > 0)
                                itemFinanceiro = listaCTes.FirstOrDefault().Documentos.FirstOrDefault().Numero.ToString();

                            if (valePedagioMDFeCompra.ComprarRetorno == Dominio.Enumeradores.OpcaoSimNao.Nao)
                                itemFinanceiro = string.Concat(itemFinanceiro, " (ida)");
                            else
                                itemFinanceiro = string.Concat(itemFinanceiro, " (retorno)");

                            compraValePedagio.ItemFinanceiro = itemFinanceiro;

                            if (compraValePedagio.IdModoCompraValePedagio != 1)
                            {
                                compraValePedagio.InicioVigencia = DateTime.Today; //veiculo.DataInicioVigenciaTagValePedagio;
                                compraValePedagio.FimVigencia = DateTime.Today.AddDays(15); ////veiculo.DataInicioVigenciaTagValePedagio;
                            }

                            compraValePedagio.ValorPrevioCalculado = custoRota.ValorPedagioTotal;

                            mensagemRetorno = string.Empty;
                            Servicos.Models.Integracao.InspectorBehavior inspectorCompraRota = new Servicos.Models.Integracao.InspectorBehavior();
                            ServicoTarget.ValePedagio.CompraValePedagioResponse retornoCompraValePedagio = ComprarValePedagio(autenticacao, compraValePedagio, unidadeDeTrabalho, ref mensagemRetorno, ref inspectorCompraRota);
                            SalvarXMLIntegracao(inspectorCompraRota, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unidadeDeTrabalho);

                            if (retornoCompraValePedagio != null)
                            {
                                valePedagioMDFeCompra.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso;
                                valePedagioMDFeCompra.Mensagem = retornoCompraValePedagio.Mensagem;
                                valePedagioMDFeCompra.NumeroComprovante = retornoCompraValePedagio.IdCompraValePedagio.ToString();

                                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                                if (valePedagioMDFeCompra.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag)
                                {
                                    ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest confirmacaoPedagioTag = new ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest
                                    {
                                        IdCompraValePedagioViaFacil = retornoCompraValePedagio.IdCompraValePedagio
                                    };

                                    mensagemRetorno = string.Empty;
                                    Servicos.Models.Integracao.InspectorBehavior inspectorConfirmacaoTag = new Servicos.Models.Integracao.InspectorBehavior();
                                    ServicoTarget.ValePedagio.ConfirmarPedagioResponse retornoConfirmarPedagioTag = ConfirmarPedagioTag(autenticacao, confirmacaoPedagioTag, unidadeDeTrabalho, ref mensagemRetorno, ref inspectorCompraRota);
                                    SalvarXMLIntegracao(inspectorCompraRota, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ConfirmacaoPedagioTAG, unidadeDeTrabalho);

                                    if (retornoConfirmarPedagioTag == null)
                                    {
                                        //Se não conseguir confirmar a Compra precisa Cancelar.
                                        int.TryParse(valePedagioMDFeCompra.NumeroComprovante, out int idCompraValePedagio);
                                        ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio = new ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest()
                                        {
                                            IdCompraValePedagio = idCompraValePedagio,
                                            ViaFacil = valePedagioMDFeCompra.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? true : false
                                        };

                                        string mensagemRetornoCancelamento = string.Empty;
                                        Servicos.Models.Integracao.InspectorBehavior inspectorCancelamento = new Servicos.Models.Integracao.InspectorBehavior();
                                        ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagioResponse = CancelarCompraValePedagio(autenticacao, cancelaCompraValePedagio, unidadeDeTrabalho, ref mensagemRetornoCancelamento, ref inspectorCancelamento);
                                        SalvarXMLIntegracao(inspectorCancelamento, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.CancelarCompraValePedagio, unidadeDeTrabalho);

                                        valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                                        valePedagioMDFeCompra.Mensagem = mensagemRetorno;
                                        valePedagioMDFeCompra.NumeroComprovante = string.Empty;
                                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                                        return false;
                                    }

                                    valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT = retornoConfirmarPedagioTag.IdVpoAntt;
                                }

                                return true;
                            }
                            else
                            {
                                valePedagioMDFeCompra.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                                valePedagioMDFeCompra.Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Falha ao efetuar compra";
                                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                                return false;
                            }

                        }
                        else
                        {
                            valePedagioMDFeCompra.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                            valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RotaSemCusto;
                            valePedagioMDFeCompra.Mensagem = "Rota sem custo de pedágio.";
                            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                            return true;
                        }
                    }
                    else
                    {
                        valePedagioMDFeCompra.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                        valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RotaSemCusto;
                        valePedagioMDFeCompra.Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Nenhuma custo para rota encontrado";
                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                        return true;
                    }
                }
                else
                {
                    valePedagioMDFeCompra.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Nenhuma rota valida encontrada";
                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ComprarValePedagioMDFe Target: " + ex);

                valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = "9999 - " + (ex.Message.Length > 900 ? ex.Message.Substring(0, 900) : ex.Message);
                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }
        }

        public bool CancelarCompraValePedagioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);

            List<Dominio.Entidades.ValePedagioMDFeCompra> listaValePedagioMDFeCompra = repValePedagioMDFeCompra.BuscarPorMDFeTipoStatus(mdfe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao, Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso);

            bool retornoCancelmento = false;

            if (listaValePedagioMDFeCompra != null && listaValePedagioMDFeCompra.Count > 0)
            {
                foreach (Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra in listaValePedagioMDFeCompra)
                {
                    Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCancelamentoCompra = new Dominio.Entidades.ValePedagioMDFeCompra();
                    try
                    {
                        if (valePedagioMDFeCompra.Integradora != Dominio.Enumeradores.IntegradoraValePedagio.Target)
                        {
                            return false;
                        }

                        valePedagioMDFeCancelamentoCompra.MDFe = mdfe;
                        valePedagioMDFeCancelamentoCompra.NumeroComprovante = valePedagioMDFeCompra.NumeroComprovante;
                        valePedagioMDFeCancelamentoCompra.CodigoEmissaoValePedagioANTT = valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT;
                        valePedagioMDFeCancelamentoCompra.CNPJFornecedor = valePedagioMDFeCompra.CNPJFornecedor;
                        valePedagioMDFeCancelamentoCompra.CNPJResponsavel = valePedagioMDFeCompra.CNPJResponsavel;
                        valePedagioMDFeCancelamentoCompra.IBGEInicio = valePedagioMDFeCompra.IBGEInicio;
                        valePedagioMDFeCancelamentoCompra.IBGEFim = valePedagioMDFeCompra.IBGEFim;
                        valePedagioMDFeCancelamentoCompra.Integradora = valePedagioMDFeCompra.Integradora;
                        valePedagioMDFeCancelamentoCompra.Tipo = Dominio.Enumeradores.TipoIntegracaoValePedagio.Cancelamento;
                        valePedagioMDFeCancelamentoCompra.IntegracaoUsuario = valePedagioMDFeCompra.IntegracaoUsuario;
                        valePedagioMDFeCancelamentoCompra.IntegracaoSenha = valePedagioMDFeCompra.IntegracaoSenha;
                        valePedagioMDFeCancelamentoCompra.IntegracaoToken = valePedagioMDFeCompra.IntegracaoToken;
                        valePedagioMDFeCancelamentoCompra.TipoCompra = valePedagioMDFeCompra.TipoCompra;
                        valePedagioMDFeCancelamentoCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente;
                        repValePedagioMDFeCompra.Inserir(valePedagioMDFeCancelamentoCompra);

                        retornoCancelmento = this.CancelarCompraValePedagioMDFe(ref valePedagioMDFeCancelamentoCompra, unidadeDeTrabalho);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Falha CancelarCompraValePedagioMDFe Target: " + ex);

                        valePedagioMDFeCancelamentoCompra.MDFe = mdfe;
                        valePedagioMDFeCancelamentoCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                        valePedagioMDFeCancelamentoCompra.Mensagem = "9999 - " + ex;
                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCancelamentoCompra);

                        retornoCancelmento = false;
                    }
                }
                return retornoCancelmento;
            }
            else return true;
        }

        public bool CancelarCompraValePedagioMDFe(ref Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
            try
            {
                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = new ServicoTarget.ValePedagio.AutenticacaoRequest()
                {
                    Usuario = valePedagioMDFeCompra.IntegracaoUsuario,
                    Senha = valePedagioMDFeCompra.IntegracaoSenha,
                    Token = valePedagioMDFeCompra.IntegracaoToken // Target retornou que parametro não esta sendo utilizado, o Token é configurado/enviado na senha
                };

                int.TryParse(valePedagioMDFeCompra.NumeroComprovante, out int idCompraValePedagio);
                ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio = new ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest()
                {
                    IdCompraValePedagio = idCompraValePedagio,
                    ViaFacil = valePedagioMDFeCompra.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? true : false
                };

                string mensagemRetorno = string.Empty;
                Servicos.Models.Integracao.InspectorBehavior inspectorCancelamento = new Servicos.Models.Integracao.InspectorBehavior();
                ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagioResponse = CancelarCompraValePedagio(autenticacao, cancelaCompraValePedagio, unidadeDeTrabalho, ref mensagemRetorno, ref inspectorCancelamento);
                SalvarXMLIntegracao(inspectorCancelamento, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.CancelarCompraValePedagio, unidadeDeTrabalho);

                if (retornoCancelaCompraValePedagioResponse != null)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Cancelado;
                    valePedagioMDFeCompra.Mensagem = !string.IsNullOrWhiteSpace(retornoCancelaCompraValePedagioResponse.Mensagem) ? retornoCancelaCompraValePedagioResponse.Mensagem : "Cancelado com sucesso.";
                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);
                }
                else
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                    valePedagioMDFeCompra.Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Não foi possúvel efetuar o cancelamento";
                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha CancelarCompraValePedagioMDFe Target: " + ex);

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                valePedagioMDFeCompra.Mensagem = "9999 - " + ex;
                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }
        }

        public bool ObterDocumento(Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, ref byte[] documento, ref string mensagemRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = new ServicoTarget.ValePedagio.AutenticacaoRequest()
                {
                    Usuario = valePedagioMDFeCompra.IntegracaoUsuario,
                    Senha = valePedagioMDFeCompra.IntegracaoSenha,
                    Token = valePedagioMDFeCompra.IntegracaoToken // Target retornou que parametro não esta sendo utilizado, o Token é configurado/enviado na senha
                };

                int.TryParse(valePedagioMDFeCompra.NumeroComprovante, out int idTarget);

                ServicoTarget.ValePedagio.EmissaoDocumentoRequest emissaoDocumento = new ServicoTarget.ValePedagio.EmissaoDocumentoRequest
                {
                    IdEntidade = idTarget,
                    Tipo = valePedagioMDFeCompra.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? 4 : 3
                };

                mensagemRetorno = string.Empty;
                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                ServicoTarget.ValePedagio.EmissaoDocumentoResponse retornoEmissaoDocumentoResponse = EmitirDocumentoTarget(autenticacao, emissaoDocumento, unidadeDeTrabalho, ref mensagemRetorno, ref inspector);
                SalvarXMLIntegracao(inspector, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.BuscarDocumento, unidadeDeTrabalho);

                if (retornoEmissaoDocumentoResponse != null)
                {
                    documento = retornoEmissaoDocumentoResponse.DocumentoBinario;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ObterDocumento Target: " + ex);
                mensagemRetorno = "Falha ObterDocumento Target: " + ex;

                return false;
            }
        }

        private ServicoTarget.ValePedagio.ConfirmarPedagioResponse ConfirmarPedagioTag(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest confirmacaoPedagioTag, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ConfirmarPedagioTAGRequest confirmarPedagioTAGRequest = new ServicoTarget.ValePedagio.ConfirmarPedagioTAGRequest()
            {
                auth = autenticacao,
                confirmacaoRequest = confirmacaoPedagioTag
            };

            ServicoTarget.ValePedagio.ConfirmarPedagioResponse retornoConfirmarPedagio = svcTarget.ConfirmarPedagioTAG(confirmarPedagioTAGRequest).ConfirmarPedagioTAGResult;

            if (retornoConfirmarPedagio != null)
            {
                if (retornoConfirmarPedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoConfirmarPedagio.Erro.CodigoErro.ToString(), " - ", retornoConfirmarPedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoConfirmarPedagio;
            }
            else
            {
                mensagemRetorno = "ObterCustoRota não retornou rotas.";
                return null;
            }

        }

        private ServicoTarget.ValePedagio.CompraValePedagioResponse ComprarValePedagio(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.CompraValePedagioRequest compraValePedagio, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ComprarPedagioAvulsoRequest comprarPedagioAvulsoRequest = new ServicoTarget.ValePedagio.ComprarPedagioAvulsoRequest
            {
                auth = autenticacao,
                compraRequest = compraValePedagio
            };

            ServicoTarget.ValePedagio.CompraValePedagioResponse retornoCompraValePedagio = svcTarget.ComprarPedagioAvulso(comprarPedagioAvulsoRequest).ComprarPedagioAvulsoResult;

            if (retornoCompraValePedagio != null)
            {
                if (retornoCompraValePedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoCompraValePedagio.Erro.CodigoErro.ToString(), " - ", retornoCompraValePedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoCompraValePedagio;
            }
            else
            {
                mensagemRetorno = "ComprarValePedagio não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse BuscarCustoRota(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest consultaCustoRota, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ObterCustoRotaRequest obterCustoRotaRequest = new ServicoTarget.ValePedagio.ObterCustoRotaRequest()
            {
                auth = autenticacao,
                custoRotaRequest = consultaCustoRota
            };

            ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse retornoCustoRota = svcTarget.ObterCustoRota(obterCustoRotaRequest).ObterCustoRotaResult;

            if (retornoCustoRota != null)
            {
                if (retornoCustoRota.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoCustoRota.Erro.CodigoErro.ToString(), " - ", retornoCustoRota.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoCustoRota;
            }
            else
            {
                mensagemRetorno = "ObterCustoRota não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.RotaResponse BuscarRotaIBGE(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ListarRotaClienteRequest consultaRotaIBGE, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ListarRotasRequest listarRotasRequest = new ServicoTarget.ValePedagio.ListarRotasRequest()
            {
                auth = autenticacao,
                listarRotasRequest = consultaRotaIBGE
            };

            ServicoTarget.ValePedagio.ResultadoPaginadoListarRotasClienteResponse retornoRota = svcTarget.ListarRotas(listarRotasRequest).ListarRotasResult;

            if (retornoRota != null)
            {
                if (retornoRota.Erro != null && string.IsNullOrWhiteSpace(retornoRota.Erro.MensagemErro))
                {
                    mensagemRetorno = string.Concat(retornoRota.Erro.CodigoErro.ToString(), " - ", retornoRota.Erro.MensagemErro);
                    return null;
                }

                if (retornoRota.Itens != null && retornoRota.Itens.Count() > 0)
                {
                    string erroItem = string.Empty;
                    string erroRota = string.Empty;
                    for (var i = 0; i < retornoRota.Itens.Count(); i++)
                    {
                        if (retornoRota.Itens[i].Erro != null && String.IsNullOrWhiteSpace(retornoRota.Itens[i].Erro.MensagemErro))
                            erroItem = string.Concat(retornoRota.Itens[i].Erro.CodigoErro.ToString(), " - ", retornoRota.Itens[i].Erro.MensagemErro);
                        else
                        {
                            erroItem = string.Empty;
                            if (retornoRota.Itens[i].Rotas == null || retornoRota.Itens[i].Rotas.Count() == 0)
                                erroRota = "Sem rota cadastrada para IBGE Origem e IBGE Destino.";
                            else
                            {
                                erroRota = string.Empty;
                                for (var j = 0; i < retornoRota.Itens[j].Rotas.Count(); j++)
                                {
                                    if (retornoRota.Itens[j].Rotas[j].Erro != null)
                                        erroRota = string.Concat(retornoRota.Itens[j].Rotas[j].Erro.CodigoErro.ToString(), " - ", retornoRota.Itens[j].Rotas[j].Erro.MensagemErro);
                                    else
                                    {
                                        mensagemRetorno = string.Empty;
                                        return retornoRota.Itens[j].Rotas[j];
                                    }

                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(erroItem))
                    {
                        mensagemRetorno = erroItem;

                        return null;
                    }
                    if (!string.IsNullOrWhiteSpace(erroRota))
                    {
                        mensagemRetorno = erroRota;

                        return null;
                    }
                }

                mensagemRetorno = "Nenhuma rota valida encontrada.";
                return null;
            }
            else
            {
                mensagemRetorno = "ListarRotasIBGE não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse CancelarCompraValePedagio(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.CancelarCompraValePedagioRequest cancelarCompraValePedagioRequest = new ServicoTarget.ValePedagio.CancelarCompraValePedagioRequest()
            {
                auth = autenticacao,
                cancelaVPRequest = cancelaCompraValePedagio
            };

            ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagio = svcTarget.CancelarCompraValePedagio(cancelarCompraValePedagioRequest).CancelarCompraValePedagioResult;

            if (retornoCancelaCompraValePedagio != null)
            {
                if (retornoCancelaCompraValePedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoCancelaCompraValePedagio.Erro.CodigoErro.ToString(), " - ", retornoCancelaCompraValePedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoCancelaCompraValePedagio;
            }
            else
            {
                mensagemRetorno = "CancelarCompraValePedagio não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.ItemBuscarCartoesResponse BuscarCartaoMotorista(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.BuscaCartoesRequest buscarCartoes, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.BuscarCartoesPortadorRequest buscarCartoesPortadorRequest = new ServicoTarget.ValePedagio.BuscarCartoesPortadorRequest()
            {
                auth = autenticacao,
                buscaRequest = buscarCartoes
            };

            ServicoTarget.ValePedagio.BuscarCartoesResponse retornoBuscarCartoesResponse = svcTarget.BuscarCartoesPortador(buscarCartoesPortadorRequest).BuscarCartoesPortadorResult;

            if (retornoBuscarCartoesResponse != null)
            {
                if (retornoBuscarCartoesResponse.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoBuscarCartoesResponse.Erro.CodigoErro.ToString(), " - ", retornoBuscarCartoesResponse.Erro.MensagemErro);
                    return null;
                }
                else
                {
                    if (retornoBuscarCartoesResponse.ListaCartoesAtivos != null && retornoBuscarCartoesResponse.ListaCartoesAtivos.Count() > 0)
                    {
                        for (var i = 0; i < retornoBuscarCartoesResponse.ListaCartoesAtivos.Count(); i++)
                        {
                            if (retornoBuscarCartoesResponse.ListaCartoesAtivos[i].Ativo && retornoBuscarCartoesResponse.ListaCartoesAtivos[i].LiberacaoCarga == ServicoTarget.ValePedagio.LiberacaoCarga.Liberado)
                            {
                                mensagemRetorno = string.Empty;
                                return retornoBuscarCartoesResponse.ListaCartoesAtivos[i];
                            }
                            else
                            {
                                mensagemRetorno = "BuscarCartaoMotorista não retornou cartão Ativo e Liberado.";
                                return null;
                            }
                        }
                    }

                    mensagemRetorno = "BuscarCartaoMotorista não retornou cartão.";
                    return null;
                }
            }
            else
            {
                mensagemRetorno = "BuscarCartaoMotorista não retornou cartão.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.EmissaoDocumentoResponse EmitirDocumentoTarget(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.EmissaoDocumentoRequest emissaoDocumento, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.EmitirDocumentoRequest emitirDocumentoRequest = new ServicoTarget.ValePedagio.EmitirDocumentoRequest()
            {
                auth = autenticacao,
                emissaoDocumento = emissaoDocumento
            };

            ServicoTarget.ValePedagio.EmissaoDocumentoResponse retornoEmissaoDocumentos = svcTarget.EmitirDocumento(emitirDocumentoRequest).EmitirDocumentoResult;

            if (retornoEmissaoDocumentos != null)
            {
                if (retornoEmissaoDocumentos.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoEmissaoDocumentos.Erro.CodigoErro.ToString(), " - ", retornoEmissaoDocumentos.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoEmissaoDocumentos;
            }
            else
            {
                mensagemRetorno = "BuscarDocumentoCompra não retornou cartão.";
                return null;
            }
        }

        private void SalvarXMLIntegracao(Servicos.Models.Integracao.InspectorBehavior inspector, Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompraXML repValePedagioMDFeCompraXML = new Repositorio.ValePedagioMDFeCompraXML(unidadeDeTrabalho);

            Dominio.Entidades.ValePedagioMDFeCompraXML log = new Dominio.Entidades.ValePedagioMDFeCompraXML()
            {
                ValePedagioMDFeCompra = valePedagioMDFeCompra,
                Tipo = tipo,
                DataHora = DateTime.Now,
                Requisicao = inspector.LastRequestXML,
                Resposta = inspector.LastResponseXML
            };

            repValePedagioMDFeCompraXML.Inserir(log);
        }

        private int ObterCategoriaPorEixos(int quantidadeEixos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo? padraoEixos)
        {
            switch (quantidadeEixos)
            {
                case 0:
                    return 1; //Motocicletas, motonetas e bicicletas
                case 1:
                    return 1; //Motocicletas, motonetas e bicicletas
                case 2:
                    return padraoEixos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo.Simples ? 2 : 7; //Automóvel, caminhoneta e furgão (dois eixos simples)
                case 3:
                    return 8; //Caminhão, caminhão trator e cavalo mecânico com semireboque (tres eixos duplos)
                case 4:
                    return 9; //Caminhão com reboque e cavalo mecânico com semi reboque (quatro eixos duplos)
                case 5:
                    return 10; //Caminhão com reboque e cavalo mecânico com semireboque (cinco eixos duplos)
                case 6:
                    return 11; //Caminhão com reboque e cavalo mecânico com semireboque (seis eixos duplos)
                case 7:
                    return 12; //Caminhão com reboque e cavalo mecânico com semireboque (sete eixos duplos)
                case 8:
                    return 13; //Caminhão com reboque e cavalo mecânico com semireboque (oito eixos duplos)
                case 9:
                    return 14; //Caminhão com reboque e cavalo mecânico com semireboque (nove eixos duplos)
                default:
                    return 2; //Automóvel, caminhoneta e furgão (dois eixos simples)
            }
        }

    }
}

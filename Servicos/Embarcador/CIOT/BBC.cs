using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public class BBC
    {
        #region Propriedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public BBC(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoBBC repConfiguracaoIntegracaoBBC = new Repositorio.Embarcador.Configuracoes.IntegracaoBBC(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC = repConfiguracaoIntegracaoBBC.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao = ObterConfiguracaoBBC(ciot.ConfiguracaoCIOT);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            ciot.Operadora = OperadoraCIOT.BBC;

            if (ciot.Contratante == null)
                ciot.Contratante = cargaCIOT.Carga.Empresa;

            if (ciot.Motorista == null)
            {
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

                Dominio.Entidades.Usuario veiculoMotorista = null;

                if (cargaCIOT.Carga.Veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(cargaCIOT.Carga.Veiculo.Codigo);

                ciot.Motorista = cargaCIOT.Carga.Motoristas != null && cargaCIOT.Carga.Motoristas.Count > 0 ? cargaCIOT.Carga.Motoristas.FirstOrDefault() : veiculoMotorista ?? null;
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, _unitOfWork);

            bool sucesso = false;
            string mensagemErro = string.Empty;
            if (configuracaoIntegracaoBBC?.PossuiIntegracaoViagem ?? false)
            {
                if (!ciot.SituacaoIntegracaoPagamentoBBC.HasValue)
                {
                    sucesso = DeclararOperacaoTransporte(out mensagemErro, cargaCIOT, modalidadeTerceiro, configuracao, configuracaoIntegracaoBBC);
                    if (sucesso)
                        ciot.SituacaoIntegracaoPagamentoBBC = SituacaoIntegracaoPagamentoBBC.DeclarouOperacaoTransporte;
                }

                if (ciot.SituacaoIntegracaoPagamentoBBC == SituacaoIntegracaoPagamentoBBC.DeclarouOperacaoTransporte && cargaCIOT.ContratoFrete.ValorAdiantamento > 0)
                {
                    sucesso = IntegrarPagamentoViagem(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC, true);
                    if (sucesso)
                    {
                        ciot.Situacao = SituacaoCIOT.Aberto;
                        ciot.SituacaoIntegracaoPagamentoBBC = SituacaoIntegracaoPagamentoBBC.DeclarouEIntegrouPagamentoViagem;
                        ciot.DataIntegracaoPagamentoBBC = DateTime.Now;
                    }
                }
            }
            else
                sucesso = DeclararOperacaoTransporte(out mensagemErro, cargaCIOT, modalidadeTerceiro, configuracao, configuracaoIntegracaoBBC);

            if (!sucesso)
            {
                ciot.Situacao = SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoBBC repConfiguracaoIntegracaoBBC = new Repositorio.Embarcador.Configuracoes.IntegracaoBBC(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC = repConfiguracaoIntegracaoBBC.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, _unitOfWork);

            bool sucesso = false;
            bool pagamentoIntegrado = ciot.SituacaoIntegracaoPagamentoBBC == SituacaoIntegracaoPagamentoBBC.EncerrouEIntegrouPagamentoViagem;
            mensagemErro = string.Empty;

            if (!pagamentoIntegrado)
            {
                sucesso = IntegrarPagamentoViagem(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC, false);
                if (sucesso)
                {
                    ciot.SituacaoIntegracaoPagamentoBBC = SituacaoIntegracaoPagamentoBBC.EncerrouEIntegrouPagamentoViagem;
                    repCIOT.Atualizar(ciot);
                    pagamentoIntegrado = true;
                    cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
                }
                else
                {
                    ciot.Mensagem = mensagemErro;
                    repCIOT.Atualizar(ciot);
                }

            }

            if (pagamentoIntegrado)
            {
                if (modalidadeTerceiro.TipoTransportador != TipoProprietarioVeiculo.TACAgregado)
                {
                    sucesso = EncerrarOperacaoTransportePelaRegra(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC);
                }
                else if (configuracaoIntegracaoBBC?.PossuiIntegracaoViagem ?? false)
                {
                    if (!ciot.SituacaoIntegracaoPagamentoBBC.HasValue || ciot.SituacaoIntegracaoPagamentoBBC != SituacaoIntegracaoPagamentoBBC.EncerrouOperacaoTransporte)
                    {
                        sucesso = EncerrarOperacaoTransporte(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC, modalidadeTerceiro);
                        //if (sucesso)
                        //    ciot.SituacaoIntegracaoPagamentoBBC = SituacaoIntegracaoPagamentoBBC.EncerrouOperacaoTransporte;
                    }

                    repCIOT.Atualizar(ciot);
                }
                else
                {
                    if (ciot.SituacaoIntegracaoPagamentoBBC != SituacaoIntegracaoPagamentoBBC.EncerrouOperacaoTransporte)
                    {
                        sucesso = EncerrarOperacaoTransporte(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC, modalidadeTerceiro);
                    }
                }

                return sucesso;
            }
            else
            {
                mensagemErro = "Não foi possivel integrar pagamento !";
                sucesso = false;

                GravarArquivoIntegracao(ciot, "Não foi possivel integrar pagamento !", "Não foi possivel integrar pagamento !", "xml");
            }

            return sucesso;
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro)
        {
            return CancelarOperacaoTransporte(out mensagemErro, ciot);
        }

        #endregion

        #region Métodos Privados

        private ServicoBBC.TEncerrarOperacaoTransporteViagem[] ObterViagensOperacaoTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);

            ObterNCMFrete(out string ncm, cargaCIOT);

            List<ServicoBBC.TEncerrarOperacaoTransporteViagem> viagensOperacaoTransporte = new List<ServicoBBC.TEncerrarOperacaoTransporteViagem>();

            var pesoCarga = ObterPesoCarga(cargaCIOT);

            viagensOperacaoTransporte.Add(new ServicoBBC.TEncerrarOperacaoTransporteViagem()
            {
                CodigoMunicipioDestino = cargaPedido.Destino.CodigoIBGE,
                CodigoMunicipioOrigem = cargaPedido.Origem.CodigoIBGE,
                CodigoNaturezaCarga = ncm,
                PesoCarga = pesoCarga > 0 ? pesoCarga : 1,
                QuantidadeViagens = 1
            });

            return viagensOperacaoTransporte.ToArray();
        }

        private ServicoBBC.TFrete ObterFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao)
        {
            ObterDatasInicioTerminoFrete(out DateTime dataInicioFrete, out DateTime dataTerminoFrete, cargaCIOT, configuracao);
            ObterNCMFrete(out string ncm, cargaCIOT);
            var retorno = new ServicoBBC.TFrete()
            {
                CodigoNaturezaCarga = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : ncm,
                CodigoMunicipioOrigem = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : (long?)cargaPedido.Origem.CodigoIBGE,
                CodigoMunicipioDestino = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : (long?)cargaPedido.Destino.CodigoIBGE,
                DataInicioFrete = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : (DateTime?)dataInicioFrete,
                DataTerminoFrete = dataTerminoFrete,
                Motorista = ObterMotorista(cargaCIOT),
                Proprietario = ObterProprietario(cargaCIOT, modalidadeTerceiro),
                TipoViagem = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? 3 : 1
            };

            if (modalidadeTerceiro.TipoTransportador != TipoProprietarioVeiculo.TACAgregado)
                retorno.PesoCarga = (decimal?)ObterPesoCarga(cargaCIOT);

            return retorno;
        }

        private decimal ObterPesoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            return Math.Round(repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo), 2);
        }

        private void ObterNCMFrete(out string ncm, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            ncm = "0000";
            if (!string.IsNullOrWhiteSpace(cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros()) && cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Length > 3)
                ncm = cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Substring(0, 4) ?? "0000";
        }

        private void ObterDatasInicioTerminoFrete(out DateTime dataInicioFrete, out DateTime dataTerminoFrete, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao)
        {
            dataInicioFrete = cargaCIOT.CIOT.DataAbertura ?? DateTime.Now;
            dataTerminoFrete = cargaCIOT.CIOT.DataFinalViagem;

            if (configuracao.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                dataInicioFrete = DateTime.Now;
                dataTerminoFrete = dataInicioFrete.AddDays(configuracao.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1);
            }
        }

        private ServicoBBC.TProprietario ObterProprietario(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            var _endereco = new ServicoBBC.TEndereco()
            {
                Bairro = cargaCIOT.CIOT.Transportador.Bairro,
                CEP = cargaCIOT.CIOT.Transportador.CEP,
                CodigoMunicipio = cargaCIOT.CIOT.Transportador.Localidade.CodigoIBGE,
                Complemento = cargaCIOT.CIOT.Transportador.Complemento,
                Logradouro = cargaCIOT.CIOT.Transportador.Endereco,
                Numero = cargaCIOT.CIOT.Transportador.Numero,
                Telefone = Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Transportador.Telefone1)
            };
            if (!string.IsNullOrEmpty(cargaCIOT?.CIOT?.Transportador?.Email ?? ""))
                _endereco.Email = cargaCIOT.CIOT.Transportador.Email;

            return new ServicoBBC.TProprietario()
            {
                CpfCnpj = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                Endereco = _endereco,
                NomeFantasia = cargaCIOT.CIOT.Transportador.NomeFantasia,
                NomeRazaoSocial = cargaCIOT.CIOT.Transportador.Nome,
                RNTRC = modalidadeTerceiro.RNTRC,
                TipoPessoa = cargaCIOT.CIOT.Transportador.Tipo
            };
        }

        private ServicoBBC.TMotorista ObterMotorista(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            if (cargaCIOT.CIOT.Motorista == null)
                return null;

            return new ServicoBBC.TMotorista()
            {
                CpfCnpj = cargaCIOT.CIOT.Motorista.CPF,
                Nome = cargaCIOT.CIOT.Motorista.Nome,
                NumeroCNH = cargaCIOT.CIOT.Motorista.NumeroHabilitacao
            };
        }

        private ServicoBBC.TConsignatario ObterConsignatario(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {

            var _endereco = new ServicoBBC.TEndereco()
            {
                Bairro = cargaCIOT.CIOT.Transportador.Bairro,
                CEP = cargaCIOT.CIOT.Transportador.CEP,
                CodigoMunicipio = cargaCIOT.CIOT.Transportador.Localidade.CodigoIBGE,
                Complemento = cargaCIOT.CIOT.Transportador.Complemento,
                Logradouro = cargaCIOT.CIOT.Transportador.Endereco,
                Numero = cargaCIOT.CIOT.Transportador.Numero,
                Telefone = Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Transportador.Telefone1)
            };
            if (!string.IsNullOrEmpty(cargaCIOT?.CIOT?.Transportador?.Email ?? ""))
                _endereco.Email = cargaCIOT.CIOT.Transportador.Email;


            return new ServicoBBC.TConsignatario()
            {
                CpfCnpj = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                Endereco = _endereco,
                NomeFantasia = cargaCIOT.CIOT.Transportador.NomeFantasia,
                NomeRazaoSocial = cargaCIOT.CIOT.Transportador.Nome,
                TipoPessoa = cargaCIOT.CIOT.Transportador.Tipo
            };
        }

        private ServicoBBC.TRemetente ObterRemetente(Dominio.Entidades.Cliente remetente)
        {

            var _endereco = new ServicoBBC.TEndereco()
            {
                Bairro = remetente.Bairro,
                CEP = remetente.CEP,
                CodigoMunicipio = remetente.Localidade.CodigoIBGE,
                Complemento = remetente.Complemento,
                Logradouro = remetente.Endereco,
                Numero = remetente.Numero,
                Telefone = Utilidades.String.OnlyNumbers(remetente.Telefone1)
            };
            if (!string.IsNullOrEmpty(remetente?.Email ?? ""))
                _endereco.Email = remetente.Email;


            return new ServicoBBC.TRemetente()
            {
                CpfCnpj = remetente.CPF_CNPJ_SemFormato,
                NomeFantasia = remetente.NomeFantasia,
                NomeRazaoSocial = remetente.Nome,
                TipoPessoa = remetente.Tipo,
                Endereco = _endereco
            };
        }

        private ServicoBBC.TDestinatario ObterDestinatario(Dominio.Entidades.Cliente destinatario)
        {
            var _endereco = new ServicoBBC.TEndereco()
            {
                Bairro = destinatario.Bairro,
                CEP = destinatario.CEP,
                CodigoMunicipio = destinatario.Localidade.CodigoIBGE,
                Complemento = destinatario.Complemento,
                Logradouro = destinatario.Endereco,
                Numero = destinatario.Numero,
                Telefone = Utilidades.String.OnlyNumbers(destinatario.Telefone1)
            };
            if (!string.IsNullOrEmpty(destinatario?.Email ?? ""))
                _endereco.Email = destinatario.Email;

            return new ServicoBBC.TDestinatario()
            {
                CpfCnpj = destinatario.CPF_CNPJ_SemFormato,
                NomeFantasia = destinatario.NomeFantasia,
                NomeRazaoSocial = destinatario.Nome,
                TipoPessoa = destinatario.Tipo,
                Endereco = _endereco
            };
        }

        private ServicoBBC.Contratante ObterContratante(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            var _endereco = new ServicoBBC.TEndereco()
            {
                Bairro = cargaCIOT.CIOT.Contratante.Bairro,
                CEP = cargaCIOT.CIOT.Contratante.CEP,
                CodigoMunicipio = cargaCIOT.CIOT.Contratante.Localidade.CodigoIBGE,
                Complemento = cargaCIOT.CIOT.Contratante.Complemento,
                Logradouro = cargaCIOT.CIOT.Contratante.Endereco,
                Numero = cargaCIOT.CIOT.Contratante.Numero,
                Telefone = Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Contratante.Telefone)
            };
            if (!string.IsNullOrEmpty(cargaCIOT?.CIOT?.Contratante?.Email ?? ""))
                _endereco.Email = cargaCIOT.CIOT.Contratante.Email;

            return new ServicoBBC.Contratante()
            {
                CpfCnpj = cargaCIOT.CIOT.Contratante.CNPJ,
                Endereco = _endereco,
                NomeFantasia = cargaCIOT.CIOT.Contratante.NomeFantasia,
                NomeRazaoSocial = cargaCIOT.CIOT.Contratante.RazaoSocial,
                RNTRC = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? cargaCIOT.CIOT.Contratante.RegistroANTT : null,
                TipoPessoa = "J"
            };
        }

        private Dominio.Entidades.Cliente ObterRemetentePedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Expedidor != null)
                return cargaPedido.Expedidor;
            else
                return cargaPedido.Pedido.Remetente;
        }

        private Dominio.Entidades.Cliente ObterDestinatarioPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                return cargaPedido.Recebedor;
            else
                return cargaPedido.Pedido.Destinatario;
        }

        private ServicoBBC.TVeiculo[] ObterVeiculos(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<ServicoBBC.TVeiculo> veiculos = new List<ServicoBBC.TVeiculo>();

            if (cargaCIOT.CIOT.Veiculo != null)
            {
                veiculos.Add(new ServicoBBC.TVeiculo()
                {
                    Placa = cargaCIOT.CIOT.Veiculo.Placa,
                    RNTRC = cargaCIOT.CIOT.Veiculo.RNTRC > 0 ? $"{cargaCIOT.CIOT.Veiculo.RNTRC:00000000}" : null
                });
            }

            //foreach (Dominio.Entidades.Veiculo veiculo in cargaCIOT.CIOT.VeiculosVinculados)
            //{
            //    veiculos.Add(new ServicoBBC.TVeiculo()
            //    {
            //        Placa = veiculo.Placa,
            //        RNTRC = veiculo.RNTRC > 0 ? $"{veiculo.RNTRC:00000000}" : null
            //    });
            //}

            return veiculos.ToArray();
        }

        private int ObterTipoPagamento(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            //FormaPagamento - Indica o tipo de pagamento do valor do frete.
            //1 - IPEF
            //2 - Conta corrente
            //3 - Conta poupança
            //4 - Conta de pagamento
            //5 - Outros

            return 2;
        }

        private ServicoBBC.TParcelaPagamento[] ObterParcelasPagamento(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            List<ServicoBBC.TParcelaPagamento> parcelasPagamento = new List<ServicoBBC.TParcelaPagamento>();

            decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
            decimal valorSaldo = cargaCIOT.ContratoFrete.SaldoAReceber;

            if (valorAdiantamento > 0m)
            {
                if (!cargaCIOT.CIOT.DataVencimentoAdiantamento.HasValue)
                    cargaCIOT.CIOT.DataVencimentoAdiantamento = DateTime.Now.AddDays(cargaCIOT.ContratoFrete.DiasVencimentoAdiantamento);

                parcelasPagamento.Add(new ServicoBBC.TParcelaPagamento()
                {
                    CodigoParcela = cargaCIOT.CIOT.Codigo.ToString() + "1",
                    ValorParcela = valorAdiantamento,
                    Vencimento = cargaCIOT.CIOT.DataVencimentoAdiantamento.Value
                });
            }

            if (valorSaldo > 0m)
            {
                if (!cargaCIOT.CIOT.DataVencimentoSaldo.HasValue)
                    cargaCIOT.CIOT.DataVencimentoSaldo = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(cargaCIOT.ContratoFrete);

                parcelasPagamento.Add(new ServicoBBC.TParcelaPagamento()
                {
                    CodigoParcela = cargaCIOT.CIOT.Codigo.ToString() + "2",
                    ValorParcela = valorSaldo,
                    Vencimento = cargaCIOT.CIOT.DataVencimentoSaldo.Value
                });
            }

            return parcelasPagamento.ToArray();
        }

        private ServicoBBC.TPagamento ObterPagamento(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            return new ServicoBBC.TPagamento()
            {
                AgenciaPagamento = cargaCIOT.CIOT.Transportador.Agencia,
                BancoPagamento = cargaCIOT.CIOT.Transportador.Banco?.Numero.ToString(),
                ContaPagamento = cargaCIOT.CIOT.Transportador.NumeroConta,
                FormaPagmento = ObterTipoPagamento(modalidadeTerceiro),
                Parcelas = ObterParcelasPagamento(cargaCIOT),
                ParcelaUnica = cargaCIOT.ContratoFrete.ValorAdiantamento <= 0
            };
        }

        private ServicoBBC.TValoresFrete ObterValores(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            return new ServicoBBC.TValoresFrete()
            {
                TotalImposto = cargaCIOT.ContratoFrete.ValorImpostosReter,
                TotalPegadio = cargaCIOT.ContratoFrete.ValorPedagio,
                ValorCombustivel = cargaCIOT.ContratoFrete.ValorAbastecimento,
                ValorFrete = cargaCIOT.ContratoFrete.ValorFreteSubcontratacao,
                ValorDespesas = cargaCIOT.ContratoFrete.ValoresAdicionais.Where(o => o.TipoJustificativa == TipoJustificativa.Desconto).Sum(o => o.Valor),
                QuantidadeTarifas = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? (int?)0 : null,
                ValorTarifas = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? (decimal?)0m : null
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.Autenticacao ObterAutenticacaoViagem(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC)
        {
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.Autenticacao()
            {
                cnpj = configuracaoIntegracaoBBC.CnpjEmpresaViagem,
                senhaApi = configuracaoIntegracaoBBC.SenhaViagem,
                clientSecret = configuracaoIntegracaoBBC.ClientSecret
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.IntegrarPagamentoViagem ObterPagamentoViagem(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string token, bool adiantamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);
            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = cargaCIOT.CIOT;


            var retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.IntegrarPagamentoViagem();
            retorno.viagemExternoId = cargaCIOT.ContratoFrete.NumeroContrato;
            retorno.tipo = adiantamento ? "Adiantamento" : "Saldo";
            retorno.formaPagamento = string.IsNullOrEmpty(ciot.Transportador.ChavePix) ? "Deposito" : "Pix";
            retorno.cpfCnpjContratado = ciot.Transportador.CPF_CNPJ_SemFormato;
            retorno.nomeContratado = ciot.Transportador.Nome;
            retorno.cpfMotorista = ciot.Motorista.CPF;
            retorno.nomeMotorista = ciot.Motorista.Nome;
            retorno.valor = adiantamento ? cargaCIOT.ContratoFrete.ValorAdiantamento : cargaCIOT.ContratoFrete.SaldoAReceber;
            retorno.tipoBanco = ciot.Transportador?.Banco?.Descricao;
            retorno.pagamentoExternoId = Convert.ToInt32((cargaCIOT.CIOT.Codigo.ToString() + (adiantamento ? "1" : "2")));
            retorno.agencia = ciot.Transportador?.Banco?.Numero == 655 ? null : ciot.Transportador.Agencia.ToNullableInt();
            retorno.conta = ciot.Transportador?.Banco?.Numero == 655 ? null : ciot.Transportador.NumeroConta;
            retorno.tipoConta = ciot.Transportador.TipoContaBanco.HasValue ? ciot.Transportador.TipoContaBanco.ObterDescricaoAbreviada() : "Corrente";
            retorno.ibgeOrigem = cargaPedido.Origem.CodigoIBGE;
            retorno.ibgeDestino = cargaPedido.Destino.CodigoIBGE;
            retorno.chavePix = string.IsNullOrEmpty(ciot.Transportador.ChavePix) ? null : ciot.Transportador.ChavePix;
            retorno.hashValidacao = Servicos.Criptografia.GerarHashMD5($"{token}{retorno.cpfCnpjContratado}{retorno.valor.ToString().Replace(".", ",")}").ToLower();
            retorno.filialId = cargaPedido?.Carga?.Empresa?.CodigoCentroCusto ?? " ";

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.CancelarPagamentoViagem ObterCancelamentoPagamentoViagem(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string token, bool adiantamento)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.CancelarPagamentoViagem();

            retorno.pagamentoExternoId = Convert.ToInt32((cargaCIOT.CIOT.Codigo.ToString() + (adiantamento ? "1" : "2"))); ;
            retorno.viagemExternoId = cargaCIOT.ContratoFrete.NumeroContrato;
            retorno.dataRequisicao = cargaCIOT.CIOT.DataIntegracaoPagamentoBBC?.ToString("MM-dd-yyyy");
            retorno.valor = adiantamento ? cargaCIOT.ContratoFrete.ValorAdiantamento : cargaCIOT.ContratoFrete.SaldoAReceber;
            retorno.Evento = "Cancelamento";
            retorno.motivo = string.Empty;

            return retorno;
        }

        private bool DeclararOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC)
        {
            mensagemErro = null;
            bool sucesso = false;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);

                ServicoBBC.CiotServiceClient svcCIOTBBC = ObterClientCIOT(configuracaoIntegracaoBBC?.URL ?? string.Empty);

                svcCIOTBBC.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.Entidades.Cliente remetente = ObterRemetentePedido(cargaPedido);
                Dominio.Entidades.Cliente destinatario = ObterDestinatarioPedido(cargaPedido);

                ServicoBBC.DeclararOperacaoTransporteReq requisicao = new ServicoBBC.DeclararOperacaoTransporteReq()
                {
                    Frete = ObterFrete(cargaCIOT, cargaPedido, modalidadeTerceiro, configuracao),
                    Consignatario = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : ObterConsignatario(cargaCIOT),
                    Contratante = ObterContratante(cargaCIOT, modalidadeTerceiro),
                    Remetente = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : ObterRemetente(remetente),
                    Destinatario = modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? null : ObterDestinatario(destinatario),
                    Veiculos = ObterVeiculos(cargaCIOT),
                    Pagamento = ObterPagamento(cargaCIOT, modalidadeTerceiro),
                    Valores = ObterValores(cargaCIOT, modalidadeTerceiro),
                };

                ServicoBBC.DeclararOperacaoTransporteResp retorno = svcCIOTBBC.DeclararOperacaoTransporte(requisicao);

                if (retorno.Sucesso)
                {
                    sucesso = true;

                    cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                    cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                    cargaCIOT.CIOT.Mensagem = "CIOT integrado com sucesso.";
                    cargaCIOT.CIOT.Numero = retorno.CIOT;
                    cargaCIOT.CIOT.Digito = retorno.CodigoVerificador;
                    cargaCIOT.CIOT.ProtocoloAutorizacao = retorno.SenhaAlteracao;

                    repCIOT.Atualizar(cargaCIOT.CIOT);
                }
                else
                {
                    mensagemErro = retorno.Excecao != null ? $"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem} (protocolo {retorno.ProtocoloErro})" : retorno.AvisoTransportador;

                    cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                    cargaCIOT.CIOT.Mensagem = mensagemErro;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                cargaCIOT.CIOT.Mensagem = "Ocorreu uma falha ao integrar o CIOT.";
            }

            repCIOT.Atualizar(cargaCIOT.CIOT);

            GravarArquivoIntegracao(cargaCIOT.CIOT, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return sucesso;
        }

        private bool CancelarOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Configuracoes.IntegracaoBBC repConfiguracaoIntegracaoBBC = new Repositorio.Embarcador.Configuracoes.IntegracaoBBC(_unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC = repConfiguracaoIntegracaoBBC.BuscarPrimeiroRegistro();

            if (ciot.SituacaoIntegracaoPagamentoBBC == SituacaoIntegracaoPagamentoBBC.DeclarouEIntegrouPagamentoViagem)
            {
                bool retornoCancelamento = CancelarPagamentoViagem(out mensagemErro,cargaCIOT, configuracaoIntegracaoBBC,true);

                if (!string.IsNullOrEmpty(mensagemErro))
                {
                    ciot.Mensagem = mensagemErro;
                    return retornoCancelamento;
                }
                else
                {
                    mensagemErro = "Pagamento cancelado na BBC - ";
                }

            }

            ServicoBBC.CiotServiceClient svcCIOTBBC = ObterClientCIOT(configuracaoIntegracaoBBC?.URL ?? string.Empty);

            InspectorBehavior inspector = new InspectorBehavior();

            svcCIOTBBC.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoBBC.CancelarOperacaoTransporteReq requisicao = new ServicoBBC.CancelarOperacaoTransporteReq()
            {
                CIOT = ciot.Numero,
                MotivoCancelamento = !string.IsNullOrWhiteSpace(ciot.MotivoCancelamento) ? ciot.MotivoCancelamento : "CIOT gerado incorretamente.",
                SenhaAlteracao = ciot.ProtocoloAutorizacao
            };

            ServicoBBC.CancelarOperacaoTransporteResp retorno = svcCIOTBBC.CancelarOperacaoTransporte(requisicao);

            if (retorno.Sucesso)
            {
                mensagemErro += " Cancelamento realizado com sucesso.";
                ciot.Mensagem = mensagemErro;
                ciot.ProtocoloCancelamento = retorno.ProtocoloCancelamento;
                ciot.DataCancelamento = retorno.DataCancelamento ?? DateTime.Now;
                ciot.Situacao = SituacaoCIOT.Cancelado;
            }
            else
            {
                mensagemErro = retorno.Excecao != null ? $"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}" : "Ocorreu uma falha na BBC ao enviar a tentativa de cancelamento do CIOT.";

                ciot.Mensagem = mensagemErro;
            }

            repCIOT.Atualizar(ciot);

            GravarArquivoIntegracao(ciot, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return retorno.Sucesso;
        }

        private bool EncerrarOperacaoTransportePelaRegra(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            mensagemErro = "";
            bool sucesso = false;

            if (configuracaoIntegracaoBBC?.PossuiIntegracaoViagem ?? false && cargaCIOT.CIOT.SituacaoIntegracaoPagamentoBBC != SituacaoIntegracaoPagamentoBBC.EncerrouEIntegrouPagamentoViagem)
            {
                sucesso = IntegrarPagamentoViagem(out mensagemErro, cargaCIOT, configuracaoIntegracaoBBC, false);
                if (sucesso)
                {
                    cargaCIOT.CIOT.SituacaoIntegracaoPagamentoBBC = SituacaoIntegracaoPagamentoBBC.EncerrouEIntegrouPagamentoViagem;
                    repCIOT.Atualizar(cargaCIOT.CIOT);
                }
            }
            if (sucesso)
            {
                cargaCIOT.CIOT.Mensagem = "Encerramento por regra de negocio (Tipo Transportador diferente de TACAgregado encerar automático).";
                cargaCIOT.CIOT.ProtocoloEncerramento = "Regra de negocio padrão BBC";
                cargaCIOT.CIOT.DataEncerramento = DateTime.Now;
                cargaCIOT.CIOT.Situacao = SituacaoCIOT.Encerrado;
                GravarArquivoIntegracao(cargaCIOT.CIOT, "Encerrado sem necessidade de integração com BBC.", "Encerrado sem necessidade de integração com BBC.", "xml");
                sucesso = true;
            }
            repCIOT.Atualizar(cargaCIOT.CIOT);
            return sucesso;
        }


        private bool EncerrarOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            ServicoBBC.CiotServiceClient svcCIOTBBC = ObterClientCIOT(configuracaoIntegracaoBBC?.URL ?? string.Empty);

            InspectorBehavior inspector = new InspectorBehavior();

            svcCIOTBBC.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoBBC.EncerrarOperacaoTransporteReq requisicao = new ServicoBBC.EncerrarOperacaoTransporteReq()
            {
                CIOT = cargaCIOT.CIOT.Numero,
                SenhaAlteracao = cargaCIOT.CIOT.ProtocoloAutorizacao,
                ValoresEfetivos = ObterValores(cargaCIOT, modalidadeTerceiro),
                ViagensOperacaoTransporte = ObterViagensOperacaoTransporte(cargaCIOT)
            };

            ServicoBBC.EncerrarOperacaoTransporteResp retorno = svcCIOTBBC.EncerrarOperacaoTransporte(requisicao);

            if (retorno.Sucesso)
            {
                mensagemErro = "Encerramento realizado com sucesso.";
                cargaCIOT.CIOT.Mensagem = mensagemErro;
                cargaCIOT.CIOT.ProtocoloEncerramento = retorno.ProtocoloEncerramento;
                cargaCIOT.CIOT.DataEncerramento = retorno.DataEncerramento ?? DateTime.Now;
                cargaCIOT.CIOT.Situacao = SituacaoCIOT.Encerrado;
            }
            else
            {
                mensagemErro = retorno.Excecao != null ? $"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}" : "Ocorreu uma falha na BBC ao enviar a tentativa de cancelamento do CIOT.";

                cargaCIOT.CIOT.Mensagem = mensagemErro;
            }

            repCIOT.Atualizar(cargaCIOT.CIOT);

            GravarArquivoIntegracao(cargaCIOT.CIOT, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return retorno.Sucesso;
        }

        private bool IntegrarPagamentoViagem(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC, bool adiantamento)
        {
            mensagemErro = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                string token = ObterAutenticacaoViagem(configuracaoIntegracaoBBC, cargaCIOT);
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.IntegrarPagamentoViagem objetoIntegrarPagamentoViagem = ObterPagamentoViagem(cargaCIOT, token, adiantamento);

                string url = $"{configuracaoIntegracaoBBC.URLViagem}/Viagem/IntegrarPagamentoViagem";
                HttpClient requisicao = CriarRequisicao(url, token);

                jsonRequisicao = JsonConvert.SerializeObject(objetoIntegrarPagamentoViagem, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Rest BBC ao integrar pagamento. Status: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoIntegrarPagamentoViagem retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoIntegrarPagamentoViagem>(jsonRetorno);

                if (retorno.Sucesso)
                {
                    sucesso = (retorno?.Dados?.Pagamento?.statusPagamento ?? 0) == 0 ? true : false;

                    if (!sucesso)
                    {
                        switch ((retorno?.Dados?.Pagamento?.statusPagamento ?? -1))
                        {
                            case 0:
                                mensagemErro = "Status pagamento Fechado - Pagamento realizado com sucesso e fechado.";
                                break;
                            case 1:
                                mensagemErro = "Status pagamento Aberto - Pagamento com status inicial ao integrar a viagem.";
                                break;
                            case 2:
                                mensagemErro = "Status pagamento Pendente - Esperando resposta da dock na realização do pagamento. Ex: A dock retornou pendente ou esperando a resposta.";
                                break;
                            case 3:
                                mensagemErro = "Status pagamento Erro - Erro de comunicação com a dock ou algum erro no código que impossibilitou o pagamento. Ex: a Dock caiu e não houve resposta.";
                                break;
                            case 4:
                                mensagemErro = "Status pagamento Cancelado - Pagamento atualizado para cancelado devido o cancelamento.";
                                break;
                            case 5:
                                mensagemErro = "Status pagamento Processando - Status do momento que o código está processando o pagamento.";
                                break;
                            case 6:
                                mensagemErro = "Status pagamento NaoExecutado - Pagamento não foi aprovado mas teve comunicação com a dock e não deu erro, ex: Saldo insuficiente.";
                                break;
                            default:
                                mensagemErro = "Status pagamento inválido.";
                                break;
                        }
                    }
                }
                else
                {
                    sucesso = false;
                    mensagemErro = string.Concat("Erro: ", retorno?.Mensagem);
                }                
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração de pagamento da BBC";
            }

            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = cargaCIOT.CIOT;
            ciot.Mensagem = sucesso ? "CIOT e Pagamento Viagem integrados com sucesso." : mensagemErro;

            GravarArquivoIntegracao(ciot, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        private bool CancelarPagamentoViagem(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC, bool adiantamento)
        {
            mensagemErro = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                string token = ObterAutenticacaoViagem(configuracaoIntegracaoBBC, cargaCIOT);
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.CancelarPagamentoViagem objetoCancelarPagamentoViagem = ObterCancelamentoPagamentoViagem(cargaCIOT, token, adiantamento);

                string url = $"{configuracaoIntegracaoBBC.URLViagem}/Viagem/CancelamentoPagamentoViagem";
                HttpClient requisicao = CriarRequisicao(url, token);

                jsonRequisicao = JsonConvert.SerializeObject(objetoCancelarPagamentoViagem, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Rest BBC ao cancelar pagamento. Status: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoCancelarPagamentoViagem retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoCancelarPagamentoViagem>(jsonRetorno);
                sucesso = retorno?.Sucesso ?? false;

            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração de cancelamento do pagamento da BBC";
            }

            GravarArquivoIntegracao(cargaCIOT.CIOT, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        private string ObterAutenticacaoViagem(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            string mensagemErro;
            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoBBC?.URLViagem))
                    throw new ServicoException("Não possui configuração para integração de viagem da BBC.");

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.Autenticacao objeto = ObterAutenticacaoViagem(configuracaoIntegracaoBBC);

                string url = $"{configuracaoIntegracaoBBC.URLViagem}/AuthSession/GerarToken";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objeto, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Rest BBC ao efetuar login. Status: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoAutenticacao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.BBC.RetornoAutenticacao>(jsonRetorno);

                if (retorno.Sucesso)
                {
                    return retorno?.Dados?.Token;
                }
                else
                {
                    mensagemErro = "Autenticação Viagem: " + (retorno.Mensagem ?? "Não possível obter o token de autenticação.");
                }
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a autenticação de viagem da BBC";
            }

            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = cargaCIOT.CIOT;
            ciot.Mensagem = mensagemErro;

            GravarArquivoIntegracao(ciot, jsonRequisicao, jsonRetorno, "json");

            return null;
        }

        #endregion

        #region Métodos Privados - Configurações

        private HttpClient CriarRequisicao(string url, string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(BBC));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Add("x-web-auth-token", accessToken);

            return requisicao;
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTBBC ObterConfiguracaoBBC(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT)
        {
            Repositorio.Embarcador.CIOT.CIOTBBC repCIOTBBC = new Repositorio.Embarcador.CIOT.CIOTBBC(_unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao = repCIOTBBC.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private void GravarArquivoIntegracao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, string request, string response, string extensaoArquivo)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request ?? string.Empty, extensaoArquivo, _unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response ?? string.Empty, extensaoArquivo, _unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Mensagem = ciot.Mensagem
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);
        }

        private ServicoBBC.CiotServiceClient ObterClientCIOT(string url)
        {
            if (!url.EndsWith("/"))
                url += "/";

            url += "CiotService.svc";

            ServicoBBC.CiotServiceClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoBBC.CiotServiceClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                binding.SendTimeout = new TimeSpan(0, 20, 0);

                client = new ServicoBBC.CiotServiceClient(binding, endpointAddress);
            }

            return client;
        }

        #endregion
    }
}

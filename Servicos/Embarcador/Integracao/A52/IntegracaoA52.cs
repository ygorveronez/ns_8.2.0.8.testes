using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.A52
{
    public class IntegracaoA52
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 _configuracaoIntegracao;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoA52(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Repositorio.Embarcador.Configuracoes.IntegracaoA52 repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(_unitOfWork);
            _configuracaoIntegracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao100 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao100.IntegrarCarga(cargaIntegracao);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao170 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao170.IntegrarCarga(cargaIntegracao);
                    break;

                default:
                    cargaIntegracao.NumeroTentativas += 1;
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Versão de integração informada disponível para a A52.";
                    repCargaIntegracao.Atualizar(cargaIntegracao);
                    break;
            }
        }

        public void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao10 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao10.CancelarCarga(cargaCancelamentoIntegracao);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao17 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao17.CancelarCarga(cargaCancelamentoIntegracao);
                    break;

                default:
                    cargaCancelamentoIntegracao.NumeroTentativas += 1;
                    cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Versão de integração informada disponível para a A52.";
                    repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                    break;
            }
        }

        public void IntegrarTrocaMotorista(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao10 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao10.IntegrarTrocaMotorista(veiculoIntegracao);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao17 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao17.IntegrarTrocaMotorista(veiculoIntegracao);
                    break;

                default:
                    veiculoIntegracao.NumeroTentativas += 1;
                    veiculoIntegracao.DataIntegracao = DateTime.Now;
                    veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    veiculoIntegracao.ProblemaIntegracao = "Versão de integração informada disponível para a A52.";
                    repVeiculoIntegracao.Atualizar(veiculoIntegracao);
                    break;
            }
        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao10 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao10.IntegrarCargaDadosTransporte(cargaDadosTransporteIntegracao);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao17 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao17.IntegrarCargaDadosTransporte(cargaDadosTransporteIntegracao);
                    break;

                default:
                    cargaDadosTransporteIntegracao.NumeroTentativas += 1;
                    cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Versão de integração informada disponível para a A52.";
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                    break;
            }
        }

        public void IntegrarSituacaoColaborador(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao10 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao, _unitOfWork);
                    integracaoA53Versao10.IntegrarSituacaoColaborador(situacaoColaborador, veiculo);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao17 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao17.IntegrarSituacaoColaborador(situacaoColaborador, veiculo);
                    break;

                default:
                    situacaoColaborador.NumeroTentativas += 1;
                    situacaoColaborador.DataIntegracao = DateTime.Now;
                    situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    situacaoColaborador.ProblemaIntegracao = "Versão de integração informada não está disponível para a A52.";
                    repColaboradorSituacaoLancamentoIntegracao.Atualizar(situacaoColaborador);
                    break;
            }
        }

        public void IntegrarPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);

            switch (_configuracaoIntegracao.VersaoIntegracao ?? VersaoA52Enum.Versao10)
            {
                case VersaoA52Enum.Versao10:
                    Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52 integracaoA53Versao10 = new Servicos.Embarcador.Integracao.A52.V100.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao10.IntegrarPedido(pedidoIntegracao);
                    break;

                case VersaoA52Enum.Versao17:
                    Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52 integracaoA53Versao17 = new Servicos.Embarcador.Integracao.A52.V170.IntegracaoA52(_configuracaoIntegracao,_unitOfWork);
                    integracaoA53Versao17.IntegrarPedido(pedidoIntegracao);
                    break;

                default:
                    pedidoIntegracao.Tentativas += 1;
                    pedidoIntegracao.DataEnvio = DateTime.Now;
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pedidoIntegracao.ProblemaIntegracao = "Versão de integração informada não está disponível para a A52.";
                    repPedidoIntegracao.Atualizar(pedidoIntegracao);
                    break;
            }
        }

        #endregion Métodos Públicos

    }
}

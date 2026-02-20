using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class IntegracaoFrotaDaCarga
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion
        public IntegracaoFrotaDaCarga(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void IntegrarFrotaDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao IntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            IntegracaoPendente.NumeroTentativas++;
            IntegracaoPendente.DataIntegracao = DateTime.Now;

            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;


            //string teste = IntegracaoPendente.Carga.GrupoPessoaPrincipal.URLIntegracaoMultiEmbarcador;

            if (IntegracaoPendente.Carga == null)
            {
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = $"Número da carga vazia, integração ({IntegracaoPendente.Codigo}) está desabilitada.";
                repCargaIntegracao.Atualizar(IntegracaoPendente);
                return;
            }
            if (IntegracaoPendente.Carga.GrupoPessoaPrincipal == null || !(IntegracaoPendente.Carga.GrupoPessoaPrincipal.UtilizaMultiEmbarcador ?? false) || !(IntegracaoPendente.Carga.GrupoPessoaPrincipal.HabilitarIntegracaoVeiculoMultiEmbarcador ?? false))
            {
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = $"A integração do veículo com este embarcador ({IntegracaoPendente.Carga.GrupoPessoaPrincipal?.Descricao ?? string.Empty}) está desabilitada.";
                repCargaIntegracao.Atualizar(IntegracaoPendente);
                return;
            }


            if (string.IsNullOrEmpty(IntegracaoPendente.Carga.GrupoPessoaPrincipal.URLIntegracaoMultiEmbarcador))
            {
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = $"A URL de integração esta vazia. Grupo pessoa:({IntegracaoPendente.Carga.GrupoPessoaPrincipal?.Descricao ?? string.Empty}).";
                repCargaIntegracao.Atualizar(IntegracaoPendente);
                return;
            }

            if (string.IsNullOrEmpty(IntegracaoPendente.Carga.GrupoPessoaPrincipal.TokenIntegracaoMultiEmbarcador))
            {
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = $"O token de integração esta vazio. Grupo pessoa:({IntegracaoPendente.Carga.GrupoPessoaPrincipal?.Descricao ?? string.Empty}).";
                repCargaIntegracao.Atualizar(IntegracaoPendente);
                return;
            }



            if (IntegracaoPendente.Carga.Veiculo == null)
            {
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = $"Veiculo não informado na carga ({IntegracaoPendente.Carga.Codigo}) .";
                repCargaIntegracao.Atualizar(IntegracaoPendente);
                return;
            }





            InspectorBehavior inspector = new InspectorBehavior();
            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(IntegracaoPendente.Carga.GrupoPessoaPrincipal.URLIntegracaoMultiEmbarcador, IntegracaoPendente.Carga.GrupoPessoaPrincipal.TokenIntegracaoMultiEmbarcador);
            svcEmpresa.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem = null;
            bool sucesso = false;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegrar = ConverterCargaEmObjetoValor(IntegracaoPendente, _unitOfWork);
                ServicoSGT.Empresa.RetornoOfboolean retorno = svcEmpresa.SalvarVeiculo(veiculoIntegrar);
                sucesso = retorno.Status;
                if (sucesso)
                    mensagem = "Integração realizada com sucesso.";
                else
                    mensagem = retorno.Mensagem;
            }
            catch (ServicoException excecao)
            {
                IntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Problema ao tentar integrar.";
            }

            servicoArquivoTransacao.Adicionar(IntegracaoPendente, xmlRequisicao, xmlRetorno, "xml");
            
            IntegracaoPendente.ProblemaIntegracao = mensagem;
            
            if (!sucesso)
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            else
                IntegracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            
            repCargaIntegracao.Atualizar(IntegracaoPendente);
        }
        #endregion

        #region Métodos Privados
        private ServicoSGT.Empresa.EmpresaClient ObterClientEmpresa(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Empresa.svc";

            ServicoSGT.Empresa.EmpresaClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }


        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterCargaEmObjetoValor(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao IntegracaoPendente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista();
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = null;

            if (IntegracaoPendente.Carga.Veiculo != null)
                veiculoIntegracao = this.ConverterVeiculoEmObjetoValor(IntegracaoPendente.Carga.Veiculo, IntegracaoPendente.Carga.GrupoPessoaPrincipal, unitOfWork);


            if (IntegracaoPendente.Carga.VeiculosVinculados != null)
            {
                foreach (var reboque in IntegracaoPendente.Carga.VeiculosVinculados)
                {
                    if (veiculoIntegracao == null)
                        veiculoIntegracao = ConverterVeiculoEmObjetoValor(reboque, IntegracaoPendente.Carga.GrupoPessoaPrincipal, unitOfWork);
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao = ConverterVeiculoEmObjetoValor(reboque, IntegracaoPendente.Carga.GrupoPessoaPrincipal, unitOfWork);
                        reboqueIntegracao.Reboques = null;
                        veiculoIntegracao.Reboques.Add(reboqueIntegracao);
                    }
                }
            }
            if (veiculoIntegracao != null && IntegracaoPendente.Carga.Motoristas != null)
                foreach (var motorista in IntegracaoPendente.Carga.Motoristas)
                    veiculoIntegracao.Motoristas.Add(serMotorista.ConverterObjetoMotorista(motorista));

            return veiculoIntegracao;
        }


        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterVeiculoEmObjetoValor(Dominio.Entidades.Veiculo Veiculo, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);
            //Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista();
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
            if (Veiculo.ModeloVeicularCarga != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.BuscarPorGrupoPessoasEModeloVeicular(GrupoPessoa.Codigo, Veiculo.ModeloVeicularCarga.Codigo);
                if (grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga != null)
                {
                    veiculoIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
                    {
                        CodigoIntegracao = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicularEmbarcador.CodigoModeloVeicularEmbarcador,
                        Descricao = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicularEmbarcador.Descricao,
                        TipoModeloVeicular = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicular.Tipo
                    };
                }
            }

            veiculoIntegracao.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
            veiculoIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

            veiculoIntegracao.AnoFabricacao = Veiculo.AnoFabricacao;
            veiculoIntegracao.AnoModelo = Veiculo.AnoModelo;
            veiculoIntegracao.Ativo = Veiculo.Ativo;
            veiculoIntegracao.CapacidadeKG = Veiculo.CapacidadeKG;
            veiculoIntegracao.CapacidadeM3 = Veiculo.CapacidadeM3;
            veiculoIntegracao.DataAquisicao = Veiculo.DataCompra.HasValue ? Veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : "";

            veiculoIntegracao.Placa = Veiculo.Placa;
            veiculoIntegracao.TipoPropriedadeVeiculo = Veiculo.Tipo == "P" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros;
            if (veiculoIntegracao.TipoPropriedadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros)
            {
                veiculoIntegracao.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                veiculoIntegracao.Proprietario.TipoTACVeiculo = Veiculo.TipoProprietario;
                if (Veiculo.Proprietario != null)
                    veiculoIntegracao.Proprietario.TransportadorTerceiro = serEmpresa.ConverterObjetoEmpresa(Veiculo.Proprietario);
                veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC = Veiculo.RNTRC.ToString();
                veiculoIntegracao.Proprietario.CIOT = Veiculo.CIOT;
            }

            veiculoIntegracao.Renavam = Veiculo.Renavam;
            veiculoIntegracao.RNTC = Veiculo.RNTRC.ToString();
            veiculoIntegracao.Tara = Veiculo.Tara;
            veiculoIntegracao.TipoCarroceria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria)int.Parse(Veiculo.TipoCarroceria);
            veiculoIntegracao.TipoRodado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado)int.Parse(Veiculo.TipoRodado);
            veiculoIntegracao.TipoVeiculo = Veiculo.TipoVeiculo == "0" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;
            veiculoIntegracao.UF = Veiculo.Estado.Sigla;

            return veiculoIntegracao;
        }


        #endregion
    }
}

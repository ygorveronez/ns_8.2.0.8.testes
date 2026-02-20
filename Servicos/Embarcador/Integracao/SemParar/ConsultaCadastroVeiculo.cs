using Dominio.Excecoes.Embarcador;
using System;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.SemParar
{
    public class ConsultaCadastroVeiculo : IntegracaoClientBase<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public ConsultaCadastroVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            this._unitOfWork = unitOfWork;
            this._tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();

            try
            {

                if (integracaoSemParar == null || !integracaoSemParar.ConsultarSeVeiculoPossuiCadastro)
                {
                    credencial.Autenticado = false;
                    credencial.Retorno = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";
                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                    SemPararValePedagio.Identificador identifador = valePedagioClient.autenticarUsuario(integracaoSemParar.CNPJ, integracaoSemParar.Usuario, integracaoSemParar.Senha);
                    string request = inspector.LastRequestXML;
                    string response = inspector.LastResponseXML;

                    Servicos.Log.TratarErro(request, "IntegracaoSemParar");
                    Servicos.Log.TratarErro(response, "IntegracaoSemParar");

                    if (identifador.status == 0)
                    {
                        credencial.Autenticado = true;
                        credencial.Retorno = "Autenticado com sucesso";
                        credencial.Sessao = identifador.sessao;
                    }
                    else
                    {
                        credencial.Autenticado = false;
                        credencial.Retorno = ValePedagio.ObterMensagemRetorno(identifador.status);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                credencial.Autenticado = false;
                credencial.Retorno = "O WS da sem parar não está disponivel no momento.";
            }
            return credencial;
        }


        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao GerarIntegracaoCadastroVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return null;

            if (veiculo.NaoComprarValePedagio)
                return null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoGeralEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento janelaCarregamento = repositorioJanelaCarregamento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = configuracaoIntegracaoGeralEFrete.BuscarPrimeiroRegistro();

            if (!(janelaCarregamento.BloquearVeiculoSemTagValePedagioAtiva ?? false) && !(configuracaoIntegracaoEFrete?.ConsultarTagAoIncluirVeiculoNaCarga ?? false))
                return null;

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);

            if (veiculo.TiposIntegracaoValePedagio?.Count == 0)
                return null;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = veiculo.TiposIntegracaoValePedagio.FirstOrDefault();

            if (tipoIntegracao == null)
                return null;

            Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = repositorioVeiculoIntegracao.BuscarPorVeiculoETipo(veiculo.Codigo, tipoIntegracao.Tipo);

            if (integracao == null)
            {
                integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                {
                    DataIntegracao = DateTime.Now,
                    Veiculo = veiculo,
                    ProblemaIntegracao = "",
                    TipoIntegracao = tipoIntegracao
                };

                repositorioVeiculoIntegracao.Inserir(integracao);
            }

            IntegrarCadastroVeiculo(integracao);

            return integracao;
        }

        private void IntegrarCadastroVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao)
        {
            Servicos.Embarcador.Integracao.EFrete.ValePedagio servicoValePedagioEFrete = new Servicos.Embarcador.Integracao.EFrete.ValePedagio(_unitOfWork);

            switch (integracao.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar:
                    IntegrarCadastroVeiculoSemParar(integracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete:
                    servicoValePedagioEFrete.IntegrarCadastroVeiculo(integracao);
                    break;
            }
        }

        private void IntegrarCadastroVeiculoSemParar(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string erro = "";
            string response = "";
            string request = "";

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemPararParaAutenticacao(_tipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);

                if (tipoIntegracao == null)
                    return;

                if (integracaoSemParar == null)
                    return;

                if (!integracaoSemParar.ConsultarSeVeiculoPossuiCadastro)
                    return;

                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = Autenticar(integracaoSemParar);

                if (!credencial.Autenticado)
                    throw new ServicoException(credencial.Retorno);


                SemPararValePedagio.Veiculo retorno = buscarCadastroVeiculoSemParar(integracao.Veiculo, credencial, out erro, out request, out response);

                if (string.IsNullOrEmpty(erro))
                {
                    if (retorno?.status == 0)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        integracao.Veiculo.PossuiTagValePedagio = true;
                    }
                    else if (retorno != null)
                        throw new ServicoException(ValePedagio.ObterMensagemRetorno(retorno.status));

                    throw new ServicoException("Erro ao consultar cadastro veiculo na SemParar");
                }
            }
            catch (ServicoException e)
            {
                Servicos.Log.TratarErro(e.Message);

                integracao.ProblemaIntegracao = e.Message;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.Veiculo.PossuiTagValePedagio = false;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                integracao.ProblemaIntegracao = "Erro ao consultar cadastro veiculo na SemParar";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.Veiculo.PossuiTagValePedagio = false;
            }

            repVeiculoIntegracao.Atualizar(integracao);
            repVeiculo.Atualizar(integracao.Veiculo);
            servicoArquivoTransacao.Adicionar(integracao, request, response, "xml", erro);
        }

        private SemPararValePedagio.Veiculo buscarCadastroVeiculoSemParar(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, out string erro, out string request, out string response)
        {

            erro = "";
            request = "";
            response = "";

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                var retorno = valePedagioClient.obterStatusVeiculo(veiculo.Placa, credencial.Sessao);

                request = inspector.LastRequestXML;
                response = inspector.LastResponseXML;

                if (retorno.Length > 0)
                    return retorno[0];
                else
                {
                    erro = "Sem retorno na SemParar";
                    return null;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = e.Message;
            }

            return null;

        }

        #endregion

    }
}

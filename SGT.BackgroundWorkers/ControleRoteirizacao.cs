using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]
    public class ControleRoteirizacao : LongRunningProcessBase<ControleRoteirizacao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            GerarIntegracaoCoordenadasCidades(unitOfWork);
            GerarIntegracaoCoordenadasClientes(unitOfWork);
            GerarIntegracaoCoordenadasClientesOutroEndereco(unitOfWork);
            ValidacaoCoordenadasClientesLocalidade(unitOfWork);
        }

        private double ObterLatitudeOuLongitude(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            return double.Parse(value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        }

        private void ValidacaoCoordenadasClientesLocalidade(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (configuracao.RaioMaximoGeoLocalidadeGeoCliente > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesValidarRaioLocalidade(100);
                foreach (var cliente in clientes)
                {
                    double latCliente = ObterLatitudeOuLongitude(cliente.Latitude);
                    double lngCliente = ObterLatitudeOuLongitude(cliente.Longitude);
                    if ((cliente?.Localidade?.Latitude.HasValue ?? false) && (cliente?.Localidade?.Latitude.HasValue ?? false))
                    {
                        double latLocalidade = (double)(cliente?.Localidade?.Latitude.Value ?? 0);
                        double lngLocalidade = (double)(cliente?.Localidade?.Longitude.Value ?? 0);
                        double distancia = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(latLocalidade, lngLocalidade, latCliente, lngCliente) / 1000;
                        // Se a distãncia for superior ao raio máximo.. vamos marcar o cliente..
                        if (distancia > configuracao.RaioMaximoGeoLocalidadeGeoCliente)
                            cliente.GeoLocalizacaoRaioLocalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoRaioLocalidade.ForaRaio;
                        else
                            cliente.GeoLocalizacaoRaioLocalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoRaioLocalidade.OK;
                        cliente.DataUltimaAtualizacao = DateTime.Now;
                        cliente.Integrado = false;
                        repCliente.Atualizar(cliente);
                    }
                }
            }
        }

        private void GerarIntegracaoCoordenadasClientes(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesSemCoordenada(10);

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> pessoas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>();
            foreach (var cliente in clientes)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto tipoPonto = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();
                tipoPonto.Cliente = cliente;
                pessoas.Add(tipoPonto);
            }

            Servicos.Embarcador.Carga.CargaRotaFrete.AtualizarCoordenadas(pessoas, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
        }

        private void GerarIntegracaoCoordenadasClientesOutroEndereco(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> enderecos = repClienteOutroEndereco.BuscarEnderecosSemCoordenada(10);

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.CargaRotaFrete.AtualizarCoordenadasOutroEndereco(enderecos, unidadeTrabalho, configuracaoIntegracao, false, configuracao);
        }

        private void GerarIntegracaoCoordenadasCidades(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

            List<Dominio.Entidades.Localidade> localidades = repLocalidade.BuscarLocalidadeSemCoordenada(10);

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            foreach (Dominio.Entidades.Localidade localidade in localidades)
            {
                try
                {
                    Servicos.Embarcador.Carga.CargaRotaFrete.AtualizarCoordenadasCidade(localidade, configuracaoIntegracao, unidadeTrabalho);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }


    }
}

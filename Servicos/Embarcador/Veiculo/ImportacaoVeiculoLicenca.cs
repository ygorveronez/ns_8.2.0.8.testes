using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class VeiculoLicencaImportar
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtores

        public VeiculoLicencaImportar(Dictionary<string, dynamic> dados, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            _dados = dados;
            _usuario = usuario;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Veiculo ObterVeiculo()
        {
            string placaVeiculoBuscar = string.Empty;

            if (_dados.TryGetValue("Veiculo", out var placaVeiculo))
                placaVeiculoBuscar = (string)placaVeiculo;

            if (string.IsNullOrWhiteSpace(placaVeiculoBuscar))
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.PlacaVeiculoNaoInformada);

            int codigoEmpresa = 0;
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = _usuario.Empresa?.Codigo ?? 0;
            else
                codigoEmpresa = ObterTransportador()?.Codigo ?? 0;

            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorPlaca(codigoEmpresa, placaVeiculoBuscar.Trim());

            if (veiculo == null)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.VeiculoNaoEncontrado);

            return veiculo;
        }

        private Dominio.Entidades.Empresa ObterTransportador()
        {
            var cnpjTransportadorBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjTransportador", out var cnpjTransportador))
                cnpjTransportadorBuscar = (string)cnpjTransportador;

            if (string.IsNullOrWhiteSpace(cnpjTransportadorBuscar))
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    throw new ImportacaoException("CNPJ do transportador não informado");

                return null;
            }

            Repositorio.Empresa repositorio = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa tranportador = repositorio.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportadorBuscar));

            if (tranportador == null)
                throw new ImportacaoException("Transportador não encontrado");

            return tranportador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Licenca ObterLicenca()
        {
            string descricaoLicencaBuscar = string.Empty;

            if (_dados.TryGetValue("Licenca", out var descricaoLicenca))
                descricaoLicencaBuscar = (string)descricaoLicenca;

            if (string.IsNullOrWhiteSpace(descricaoLicencaBuscar))
                return null;

            Repositorio.Embarcador.Configuracoes.Licenca repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = repositorioLicenca.BuscarPorDescricao(descricaoLicencaBuscar);

            if (licenca == null)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.LicencaNaoEncontrada);

            return licenca;
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial()
        {
            string codigoIntegracaoFilialBuscar = string.Empty;

            if (_dados.TryGetValue("Filial", out var codigoFilial))
                codigoIntegracaoFilialBuscar = (string)codigoFilial;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoFilialBuscar))
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.FilialNaoInformada);

                return null;
            }

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(codigoIntegracaoFilialBuscar);

            if (filial == null)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.FilialNaoEncontrada);

            return filial;
        }

        private DateTime ObterDataEmissao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissao", out var dataEmissao))
                data = ((string)dataEmissao).ToNullableDateTime();

            if (data == null || data.Value == DateTime.MinValue)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.DataEmissaoNaoInformada);

            return data.Value;
        }

        private DateTime ObterDataVencimento()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataVencimento", out var dataVencimento))
                data = ((string)dataVencimento).ToNullableDateTime();

            if (data == null || data.Value == DateTime.MinValue)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.DataVencimentoNaoInformada);

            return data.Value;
        }

        private string ObterDescricao()
        {
            string descricaoRetornar = string.Empty;

            if (_dados.TryGetValue("Descricao", out var descricao))
                descricaoRetornar = (string)descricao;

            return string.IsNullOrWhiteSpace(descricaoRetornar) ? string.Empty : descricaoRetornar.Trim();
        }

        private string ObterNumero()
        {
            string numeroRetornar = string.Empty;

            if (_dados.TryGetValue("Numero", out var numero))
                numeroRetornar = (string)numero;

            return string.IsNullOrWhiteSpace(numeroRetornar) ? string.Empty : numeroRetornar.Trim();
        }

        private StatusLicenca ObterStatusLicenca()
        {
            string statusLicencaRetornar = string.Empty;

            if (_dados.TryGetValue("Status", out var status))
                statusLicencaRetornar = (string)status;

            statusLicencaRetornar = !string.IsNullOrWhiteSpace(statusLicencaRetornar) ? statusLicencaRetornar.Trim() : string.Empty;

            return StatusLicencaHelper.ObterStatusLicenca(statusLicencaRetornar, _tipoServicoMultisoftware);
        }

        private void AdicionarFaixasTemperatura(Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo veiculoLicenca)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            string codigosIntegracaoFaixasTemperaturaBuscar = string.Empty;

            if (_dados.TryGetValue("FaixasDeTemperatura", out var codigosFaixasTemperatura))
                codigosIntegracaoFaixasTemperaturaBuscar = (string)codigosFaixasTemperatura;

            if (string.IsNullOrWhiteSpace(codigosIntegracaoFaixasTemperaturaBuscar))
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.FavorInformarAlgumaFaixaDeTemperatura);

            List<string> codigosFaixas = codigosIntegracaoFaixasTemperaturaBuscar.Split(',').Select(o => o.Trim()).ToList();
            if (codigosFaixas.Count == 0)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.FavorInformarAlgumaFaixaDeTemperatura);

            Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repositorioFaixaTemperatura.BuscarPorCodigosIntegracao(codigosFaixas);

            if (faixasTemperatura.Count == 0)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.FavorInformarAlgumaFaixaDeTemperatura);
            if (codigosFaixas.Count() != faixasTemperatura.Count)
                throw new ImportacaoException(string.Format(Localization.Resources.Veiculos.VeiculoLicenca.FaixaTemperaturaNaoEncontrada, codigosFaixas.Where(o => !faixasTemperatura.Select(f => f.CodigoIntegracao).Contains(o)).FirstOrDefault()));

            veiculoLicenca.FaixasTemperatura = new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            foreach (Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura in faixasTemperatura)
                veiculoLicenca.FaixasTemperatura.Add(faixaTemperatura);
        }

        #endregion

        #region Métodos Validação

        private void ValidarDados(Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo veiculoLicenca)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);

            if (veiculoLicenca.DataEmissao > veiculoLicenca.DataVencimento)
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.DataEmissaoMaiorDataVencimento);

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repLicencaVeiculo.ContemLicencaVeiculoDuplicada(0, veiculoLicenca.Veiculo.Codigo, veiculoLicenca.Licenca?.Codigo ?? 0, veiculoLicenca.Filial?.Codigo ?? 0, veiculoLicenca.DataEmissao.Value, veiculoLicenca.DataVencimento.Value))
                throw new ImportacaoException(Localization.Resources.Veiculos.VeiculoLicenca.LicencaDuplicada);
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo ObterVeiculoLicencaImportar()
        {
            Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo veiculoLicenca = new Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo();

            veiculoLicenca.Veiculo = ObterVeiculo();
            veiculoLicenca.Licenca = ObterLicenca();
            veiculoLicenca.Filial = ObterFilial();

            veiculoLicenca.DataEmissao = ObterDataEmissao();
            veiculoLicenca.DataVencimento = ObterDataVencimento();
            veiculoLicenca.Descricao = ObterDescricao();
            veiculoLicenca.Numero = ObterNumero();
            veiculoLicenca.Status = ObterStatusLicenca();

            ValidarDados(veiculoLicenca);

            AdicionarFaixasTemperatura(veiculoLicenca);

            veiculoLicenca.UsuarioAlteracao = _usuario;
            veiculoLicenca.DataAlteracao = DateTime.Now;

            return veiculoLicenca;
        }

        #endregion
    }
}

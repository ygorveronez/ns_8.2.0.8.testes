using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class TaxaOcupacaoVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioTaxaOcupacaoVeiculo, Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaOcupacaoVeiculo.TaxaOcupacaoVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public TaxaOcupacaoVeiculo(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaOcupacaoVeiculo.TaxaOcupacaoVeiculo> ConsultarRegistros(FiltroPesquisaRelatorioTaxaOcupacaoVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultarRelatorioTaxaOcupacaoVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioTaxaOcupacaoVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarConsultaRelatorioTaxaOcupacaoVeiculo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/TaxaOcupacaoVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(FiltroPesquisaRelatorioTaxaOcupacaoVeiculo filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Logistica.Rota repositorioRota = new Repositorio.Embarcador.Logistica.Rota(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repositorioCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoDestino > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filial = filtrosPesquisa.CodigosFilial.Count > 0 ? repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = filtrosPesquisa.CodigoModeloVeiculo > 0 ? repositorioModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeiculo) : null;
            Dominio.Entidades.Embarcador.Logistica.Rota rota = filtrosPesquisa.CodigoRota > 0 ? repositorioRota.BuscarPorCodigo(filtrosPesquisa.CodigoRota) : null;
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoCarga = filtrosPesquisa.CodigosTipoCarga.Count > 0 ? repositorioTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCarga) : new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataJanelaCarregamentoInicial", filtrosPesquisa.DataJanelaCarregamentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataJanelaCarregamentoFinal", filtrosPesquisa.DataJanelaCarregamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoInicial", filtrosPesquisa.DataCriacaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoFinal", filtrosPesquisa.DataCriacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa != null ? (empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", centroCarregamento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario != null ? (destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", string.Join(", ", filial.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", rota != null ? rota.Origem.DescricaoCidadeEstado + " -> " + rota.Destino.DescricaoCidadeEstado : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", string.Join(", ", tipoCarga.Select(x => x.Descricao))));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoCarga")
                return "SituacaoCarga";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}

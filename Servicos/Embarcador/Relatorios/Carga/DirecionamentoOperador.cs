using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class DirecionamentoOperador : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador, Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaJanelaCarregamento _repositorioCargaJanelaCarregamento;

        #endregion

        #region Construtores

        public DirecionamentoOperador(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaJanelaCarregamento.ConsultarRelatorioDirecionamentoOperador(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repositorioCargaJanelaCarregamento.ConsultarRelatorioDirecionamentoOperadorAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaJanelaCarregamento.ContarConsultaRelatorioDirecionamentoOperador(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/DirecionamentoOperador";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = filtrosPesquisa.CodigoModeloVeiculo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeiculo) : null;
            Dominio.Entidades.RotaFrete rota = filtrosPesquisa.CodigoRota > 0 ? repRotaFrete.BuscarPorCodigo(filtrosPesquisa.CodigoRota) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigosFilial.Count == 1 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigosFilial.FirstOrDefault()) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = filtrosPesquisa.CodigosTipoCarga.Count == 1 ? repTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigosTipoCarga.FirstOrDefault()) : null;

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal));

            parametros.Add(new Parametro("Transportador", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : null));
            parametros.Add(new Parametro("Operador", operador?.Nome));
            parametros.Add(new Parametro("CentroCarregamento", centroCarregamento?.Descricao));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Destinatario", destinatario != null ? destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome : null));

            parametros.Add(new Parametro("TipoCarga", tipoCarga?.Descricao));
            parametros.Add(new Parametro("Rota", rota?.Descricao));
            parametros.Add(new Parametro("ModeloVeiculo", modeloVeiculo?.Descricao));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataCarregamentoFormatada")
                return "DataCarregamento";
            if (propriedadeOrdenarOuAgrupar == "DataCriacaoCargaFormatada")
                return "DataCriacaoCarga";
            if (propriedadeOrdenarOuAgrupar == "DataInteresseFormatada")
                return "DataInteresse";
            if (propriedadeOrdenarOuAgrupar == "DataCargaContratadaFormatada")
                return "DataCargaContratada";
            if (propriedadeOrdenarOuAgrupar == "CNPJTransportadorFormatado")
                return "CNPJTransportador";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}

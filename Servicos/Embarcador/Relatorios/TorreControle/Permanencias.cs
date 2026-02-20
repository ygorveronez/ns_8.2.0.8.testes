using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.TorreControle
{
    public class Permanencias : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias, Dominio.Relatorios.Embarcador.DataSource.TorreControle.Permanencias>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public Permanencias(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.Permanencias> ConsultarRegistros(FiltroPesquisaRelatorioPermanencias filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultaRelatorioPermanencias(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPermanencias filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarConsultaRelatorioPermanencias(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/TorreControle/Permanencias";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPermanencias filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas RepositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

            var filial = filtrosPesquisa.CodigoFilial.HasValue ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial.Value) : null;
            var transportador = filtrosPesquisa.CodigoTransportador.HasValue ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador.Value) : null;
            var cliente = filtrosPesquisa.CodigoCliente.HasValue ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoCliente.Value) : null;
            var grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas.HasValue ? RepositorioGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas.Value) : null;

            parametros.Add(new Parametro("Carga", filtrosPesquisa.Carga));
            parametros.Add(new Parametro("Placa", filtrosPesquisa.Placa));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));
            parametros.Add(new Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Cliente", cliente?.Descricao));
            parametros.Add(new Parametro("TipoParada", filtrosPesquisa.CodigoTipoParada.HasValue ? (filtrosPesquisa.CodigoTipoParada.Value ? "Coleta" : "Entrega") : string.Empty));
            parametros.Add(new Parametro("DataCarregamentoInicial", filtrosPesquisa.DataCarregamentoInicial));
            parametros.Add(new Parametro("DataCarregamentoFinal", filtrosPesquisa.DataCarregamentoFinal));
            parametros.Add(new Parametro("DataCriacaoCargaInicial", filtrosPesquisa.DataCriacaoCargaInicial));
            parametros.Add(new Parametro("DataCriacaoCargaFinal", filtrosPesquisa.DataCriacaoCargaFinal));
            parametros.Add(new Parametro("DataAgendamentoColetaInicial", filtrosPesquisa.DataAgendamentoColetaInicial));
            parametros.Add(new Parametro("DataAgendamentoColetaFinal", filtrosPesquisa.DataAgendamentoColetaFinal));
            parametros.Add(new Parametro("DataAgendamentoEntregaInicial", filtrosPesquisa.DataAgendamentoEntregaInicial));
            parametros.Add(new Parametro("DataAgendamentoEntregaFinal", filtrosPesquisa.DataAgendamentoEntregaFinal));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
    }
}

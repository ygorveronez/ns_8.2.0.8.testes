using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class AbastecimentoTicketLog : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog, Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog>
    {
        #region Atributos

        private readonly Repositorio.AbastecimentoTicketLog _repositorioAbastecimentoTicketLog;

        #endregion

        #region Construtores

        public AbastecimentoTicketLog(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimentoTicketLog = new Repositorio.AbastecimentoTicketLog(_unitOfWork);
        }

        public AbastecimentoTicketLog(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimentoTicketLog = new Repositorio.AbastecimentoTicketLog(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioAbastecimentoTicketLog.ConsultarRelatorioAbastecimentoTicketLogAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Públicos Protegidos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAbastecimentoTicketLog.ConsultarRelatorioAbastecimentoTicketLog(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAbastecimentoTicketLog.ContarConsultaRelatorioAbastecimentoTicketLog(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/AbastecimentoTicketLog";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Cliente fornecedor = filtrosPesquisa.CNPJFornecedor > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CNPJFornecedor) : null;


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataTransacao", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", fornecedor?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoTransacao", filtrosPesquisa.CodigoTransacao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Descricao"))
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
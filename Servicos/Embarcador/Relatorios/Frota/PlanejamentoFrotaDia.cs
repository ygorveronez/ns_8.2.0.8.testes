using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class PlanejamentoFrotaDia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia, Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia>
    {
        #region Propriedades

        private readonly Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia _repositorioPlanejamentoFrotaDia;

        #endregion

        #region Construtores

        public PlanejamentoFrotaDia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork);
        }

        public PlanejamentoFrotaDia(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repositorioPlanejamentoFrotaDia.ConsultarRelatorioPlanejamentoFrotaDiaAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPlanejamentoFrotaDia.ConsultarRelatorioPlanejamentoFrotaDia(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPlanejamentoFrotaDia.ContarConsultaRelatorioPlanejamentoFrotaDia(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/PlanejamentoFrotaDia";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportador.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculo.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculo) : new List<Dominio.Entidades.Veiculo>();

            parametros.Add(new Parametro("Filial", string.Join(", ", filiais.Select(obj => obj.Descricao))));
            parametros.Add(new Parametro("Transportador", string.Join(", ", transportadores.Select(obj => obj.Descricao))));
            parametros.Add(new Parametro("Periodo", filtrosPesquisa.PeriodoInicio, filtrosPesquisa.PeriodoFim));
            parametros.Add(new Parametro("Veiculo", string.Join(", ", veiculos.Select(obj => obj.Descricao))));
            parametros.Add(new Parametro("Placa", filtrosPesquisa.Placa));
            parametros.Add(new Parametro("Roteirizado", filtrosPesquisa.Roteirizado));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}

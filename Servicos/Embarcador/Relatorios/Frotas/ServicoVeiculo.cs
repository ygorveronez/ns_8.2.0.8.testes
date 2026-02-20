using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class ServicoVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo, Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.ServicoVeiculoFrota _repositorioMovimentoFrota;

        #endregion

        #region Construtores

        public ServicoVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
        }

        public ServicoVeiculo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMovimentoFrota.ConsultarRelatorioServicoVeiculoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoFrota.ConsultarRelatorioServicoVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoFrota.ContarConsultaRelatorioServicoVeiculo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/ServicoVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
            Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicos = filtrosPesquisa.CodigosServico.Count > 0 ? repServicoVeiculoFrota.BuscarPorCodigo(filtrosPesquisa.CodigosServico) : new List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = filtrosPesquisa.CodigoGrupoServico > 0 ? repGrupoServico.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoServico, false) : null;


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", string.Join(", ", servicos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoManutencao", filtrosPesquisa.TipoManutencao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motivo", filtrosPesquisa.Motivo?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoServico", grupoServico?.Descricao));

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
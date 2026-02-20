using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frota;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class Pneu : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu, Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu>
    {

        #region Propriedades

        private readonly Repositorio.Embarcador.Frota.Pneu _repositorioPneu;

        #endregion

        #region Construtores

        public Pneu(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
        }

        public Pneu(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu>> ConsultarRegistrosAsync(FiltroPesquisaRelatorioPneu filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repositorioPneu.ConsultarRelatorioPneuAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu> ConsultarRegistros(FiltroPesquisaRelatorioPneu filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPneu.ConsultarRelatorioPneu(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPneu filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPneu.ContarConsultaRelatorioPneu(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/Pneu";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPneu filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork);
            Repositorio.Embarcador.Frota.ModeloPneu repositorioModeloPneu = new Repositorio.Embarcador.Frota.ModeloPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.MarcaPneu repositorioMarcaPneu = new Repositorio.Embarcador.Frota.MarcaPneu(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Frota.Pneu pneu = filtrosPesquisa.CodigoPneu > 0 ? repositorioPneu.BuscarPorCodigo(filtrosPesquisa.CodigoPneu) : null;
            Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = filtrosPesquisa.CodigoModeloPneu > 0 ? repositorioModeloPneu.BuscarPorCodigo(filtrosPesquisa.CodigoModeloPneu) : null;
            Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = filtrosPesquisa.CodigoMarcaPneu > 0 ? repositorioMarcaPneu.BuscarPorCodigo(filtrosPesquisa.CodigoMarcaPneu) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            parametros.Add(new Parametro("Pneu", pneu?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("ModeloPneu", modeloPneu?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("MarcaPneu", marcaPneu?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa_Formatada ?? string.Empty));
            parametros.Add(new Parametro("TipoBandaRodagem", string.Join(", ", filtrosPesquisa.TiposBandaRodagem.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("Movimentacao", filtrosPesquisa.Movimentacao.HasValue ? filtrosPesquisa.Movimentacao.Value.ObterDescricao() : string.Empty));
            parametros.Add(new Parametro("VidaUtil", filtrosPesquisa.VidaUtil.HasValue ? filtrosPesquisa.VidaUtil.Value.ObterDescricao() : string.Empty));
            parametros.Add(new Parametro("StatusPneu", filtrosPesquisa.StatusPneu.HasValue && filtrosPesquisa.StatusPneu.Value > 0 ? filtrosPesquisa.StatusPneu.Value.ObterDescricao() : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "TipoBandaRodagemDescricao")
                return "TipoBandaRodagem";

            if (propriedadeOrdenarOuAgrupar == "StatusPneuDescricao")
                return "StatusPneu";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}

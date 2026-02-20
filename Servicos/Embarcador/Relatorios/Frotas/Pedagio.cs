using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class Pedagio : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioPedagio, Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedagio.Pedagio _repositorioPedagio;

        #endregion

        #region Construtores

        public Pedagio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(_unitOfWork);
        }

        public Pedagio(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioPedagio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioPedagio.ConsultarRelatorioPedagioAsync(filtrosPesquisa.TipoPedagio, propriedadesAgrupamento, filtrosPesquisa.Veiculo, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.ValorInicial, filtrosPesquisa.ValorFinal, filtrosPesquisa.Situacoes, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioPedagio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPedagio.ConsultarRelatorioPedagio(filtrosPesquisa.TipoPedagio,propriedadesAgrupamento,filtrosPesquisa.Veiculo, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.ValorInicial, filtrosPesquisa.ValorFinal, filtrosPesquisa.Situacoes, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioPedagio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPedagio.ContarConsultaRelatorioPedagio(filtrosPesquisa.TipoPedagio, propriedadesAgrupamento, filtrosPesquisa.Veiculo, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.ValorInicial, filtrosPesquisa.ValorFinal, filtrosPesquisa.Situacoes);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/Pedagio";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioPedagio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (filtrosPesquisa.Veiculo > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

            if (filtrosPesquisa.ValorInicial > 0m)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorInicial", filtrosPesquisa.ValorInicial.ToString("n2"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorInicial", false));

            if (filtrosPesquisa.ValorFinal > 0m)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorFinal", filtrosPesquisa.ValorFinal.ToString("n2"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorFinal", false));

            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o == SituacaoPedagio.Fechado ? "Fechado" : o == SituacaoPedagio.Lancado ? "Lançado" : "Inconsistente")), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.TipoPedagio == TipoPedagio.Credito)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPedagio", "Crédito"));
            else if (filtrosPesquisa.TipoPedagio == TipoPedagio.Debito)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPedagio", "Débito"));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPedagio", "Todos"));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoPedagio")
                return "TipoPedagio";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
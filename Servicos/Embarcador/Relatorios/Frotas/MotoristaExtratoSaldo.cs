using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class MotoristaExtratoSaldo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotoristaExtratoSaldo, Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista _repositorioMotoristaExtratoSaldo;

        #endregion

        #region Construtores

        public MotoristaExtratoSaldo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMotoristaExtratoSaldo = new Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista(_unitOfWork);
        }

        public MotoristaExtratoSaldo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMotoristaExtratoSaldo = new Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotoristaExtratoSaldo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMotoristaExtratoSaldo.RelatorioMotoristaExtratoSaldoAsync(filtrosPesquisa.Motorista, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotoristaExtratoSaldo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMotoristaExtratoSaldo.RelatorioMotoristaExtratoSaldo(filtrosPesquisa.Motorista,filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotoristaExtratoSaldo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMotoristaExtratoSaldo.ContarMotoristaExtratoSaldo(filtrosPesquisa.Motorista, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/MotoristaExtratoSaldo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotoristaExtratoSaldo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            if (filtrosPesquisa.Motorista > 0)
            {
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", usuario.Nome + " (" + usuario.CPF_Formatado + ")", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyy"), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyy"), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Data")
                return "Data";

            if (propriedadeOrdenarOuAgrupar == "DataLancamento")
                return "DataLancamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
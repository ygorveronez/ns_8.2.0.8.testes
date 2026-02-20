using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class TempoDeViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioTempoDeViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.TempoDeViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoViagem _repositorioTempoDeViagem;

        #endregion

        #region Construtores

        public TempoDeViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTempoDeViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.TempoDeViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioTempoDeViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTempoDeViagem.RelatorioTempoDeViagem(filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal ,filtrosPesquisa.Situacao, filtrosPesquisa.Veiculo, filtrosPesquisa.Motorista, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioTempoDeViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTempoDeViagem.ContarRelatorioTempoDeViagem(filtrosPesquisa.Situacao, filtrosPesquisa.Veiculo, filtrosPesquisa.Motorista);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/TempoDeViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioTempoDeViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (filtrosPesquisa.Motorista > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", repMotorista.BuscarPorCodigo(filtrosPesquisa.Motorista).Nome, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.Veiculo > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo).Placa, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataSaidaFormatada")
                return "DataSaida";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
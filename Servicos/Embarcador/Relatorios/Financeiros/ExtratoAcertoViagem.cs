using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class ExtratoAcertoViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem, Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.MovimentoFinanceiro _repositorioFinanceiros;
        private bool _naoDescontarValorSaldoMotorista;

        #endregion

        #region Construtores

        public ExtratoAcertoViagem(bool naoDescontarValorSaldoMotorista, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioFinanceiros = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(_unitOfWork);
            _naoDescontarValorSaldoMotorista = naoDescontarValorSaldoMotorista;
        }

        #endregion  

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioFinanceiros.RelatorioExtratoAcertoViagem(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, _naoDescontarValorSaldoMotorista, true).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioFinanceiros.ContarRelatorioExtratoAcertoViagem(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/ExtratoAcertoViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();


            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal != DateTime.MinValue ? filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : ""));

            Dominio.Entidades.Usuario motorista = repFuncionario.BuscarPorCodigo(filtrosPesquisa.Motorista);
            string nome = motorista != null ? $"{motorista.Nome} - {motorista.CPF_CNPJ_Formatado}" : "";
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", nome));

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Descricao ?? ""));

            Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.SegmentoVeiculo);
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", segmentoVeiculo?.Descricao ?? ""));

            string situacaoAcerto = "";
            if (filtrosPesquisa.SituacaoAcerto != null && filtrosPesquisa.SituacaoAcerto.Contains(1))
                situacaoAcerto += "Em Andamento, ";
            if (filtrosPesquisa.SituacaoAcerto != null && filtrosPesquisa.SituacaoAcerto.Contains(2))
                situacaoAcerto += "Fechado, ";
            if (filtrosPesquisa.SituacaoAcerto != null && filtrosPesquisa.SituacaoAcerto.Contains(3))
                situacaoAcerto += "Cancelado, ";
            if (string.IsNullOrWhiteSpace(situacaoAcerto))
                situacaoAcerto = "Todos";

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAcerto", situacaoAcerto, true));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLancamento", filtrosPesquisa?.TipoLancamento ?? ""));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Justificativa", ""));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Data")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class FaturamentoPorGrupoPessoas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas, Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioCTe;

        #endregion

        #region Construtores

        public FaturamentoPorGrupoPessoas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCTe.ConsultarRelatorioFaturamentoPorGrupoPessoas(false, filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCTe.ContarConsultaRelatorioFaturamentoPorGrupoPessoas(true, filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/FaturamentoPorGrupoPessoas";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupos = filtrosPesquisa.CodigosGruposPessoas?.Count > 0 ? repGrupoPessoa.BuscarPorCodigo(filtrosPesquisa.CodigosGruposPessoas.ToArray()) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentosFiscais = filtrosPesquisa.CodigosModeloDocumentoFiscal?.Count > 0 ? repModeloDocumentoFiscal.BuscarPorCodigo(filtrosPesquisa.CodigosModeloDocumentoFiscal.ToArray()) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.DataInicialEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialAutorizacao", filtrosPesquisa.DataInicialAutorizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalAutorizacao", filtrosPesquisa.DataFinalAutorizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", !string.IsNullOrWhiteSpace(filtrosPesquisa.PropriedadeVeiculo) ? (filtrosPesquisa.PropriedadeVeiculo == "T" ? "Terceiros" : filtrosPesquisa.PropriedadeVeiculo == "P" ? "Próprio" : filtrosPesquisa.PropriedadeVeiculo == "O" ? "Outros" : "Todos") : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", string.Join(", ", from obj in grupos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumentoFiscal", string.Join(", ", from obj in modelosDocumentosFiscais select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DocumentoFaturavel", filtrosPesquisa.DocumentoFaturavel));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VinculoCarga", filtrosPesquisa.VinculoCarga));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
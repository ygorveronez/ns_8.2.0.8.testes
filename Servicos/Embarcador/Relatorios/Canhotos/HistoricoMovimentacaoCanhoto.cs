using Dominio.ObjetosDeValor.Embarcador.Canhoto;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Canhotos
{
    public class HistoricoMovimentacaoCanhoto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto, Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto>
    {

        #region Atributos

        private readonly Repositorio.Embarcador.Canhotos.CanhotoHistorico _repositorioHistoricoMovimentacaoCanhoto;
        #endregion

        #region Construtores
        public HistoricoMovimentacaoCanhoto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioHistoricoMovimentacaoCanhoto = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(_unitOfWork);
        }

        #endregion

        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto> ConsultarRegistros(FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioHistoricoMovimentacaoCanhoto.ConsultarRelatorioHistoricoMovimentacaoCanhoto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioHistoricoMovimentacaoCanhoto.ContarConsultaRelatorioHistoricoMovimentacaoCanhoto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Canhotos/HistoricoMovimentacaoCanhoto";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Canhotos.CanhotoHistorico repositorioHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(_unitOfWork);
            Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Usuario funcionario = filtrosPesquisa.Usuario > 0 ? repUsuario.BuscarMotoristaPorCodigo(filtrosPesquisa.Usuario) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmitente.ToLong() > 0 ? repositorioEmpresa.BuscarEmpresaPorCNPJ(filtrosPesquisa.CodigoEmitente) : null;

            Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoAuditoria = filtrosPesquisa.MotivoRejeicao > 0 ? repositorioMotivoRejeicaoAuditoria.BuscarPorCodigo(filtrosPesquisa.MotivoRejeicao) : null;

            parametros.Add(new Parametro("DataUpload", filtrosPesquisa.DataUpload));
            parametros.Add(new Parametro("DataAprovacao", filtrosPesquisa.DataAprovacao));
            parametros.Add(new Parametro("DataRejeicao", filtrosPesquisa.DataRejeicao));
            parametros.Add(new Parametro("DataConfirmacaoEntrega", filtrosPesquisa.DataConfirmacaoEntrega));
            parametros.Add(new Parametro("DataReversao", filtrosPesquisa.DataReversao));
            parametros.Add(new Parametro("DataRecebimentoFisico", filtrosPesquisa.DataRecebimentoFisico));
            parametros.Add(new Parametro("Usuario", funcionario?.Nome ?? string.Empty));
            parametros.Add(new Parametro("MotivoRejeicao", motivoAuditoria?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("NumeroCanhoto", filtrosPesquisa?.NumeroCanhoto > 0 ? filtrosPesquisa?.NumeroCanhoto : null ));
            parametros.Add(new Parametro("Emitente", empresa?.Descricao ?? string.Empty));


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
    }
}

using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.CTe;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class AuditoriaCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe, Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe>
    {
        #region Atributos 
        
        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repAuditoriaCTe;

        #endregion

        #region Construtores

        public AuditoriaCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) :base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware) 
        {
            _repAuditoriaCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        public AuditoriaCTe(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repAuditoriaCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
        }

        #endregion
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe>> ConsultarRegistrosAsync(FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repAuditoriaCTe.ConsultarRelatorioAuditoriaCTeAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #region

        #endregion


        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe> ConsultarRegistros(FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repAuditoriaCTe.ConsultarRelatorioAuditoriaCTe(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repAuditoriaCTe.ContarConsultaRelatorioAuditoriaCTe(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/AuditoriaCTe";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Parametro("Documento", filtrosPesquisa.NumeroDocumentoInicial, filtrosPesquisa.NumeroDocumentoFinal));
            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("Tomador", tomador?.Descricao));

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

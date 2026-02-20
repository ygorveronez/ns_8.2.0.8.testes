using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Dominio.ObjetosDeValor.EDI.Notfis;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class MonitoramentoAlerta: RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta, Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.Monitoramento _repMonitoramentoAlerta;

        #endregion

        #region Construtores

        public MonitoramentoAlerta(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repMonitoramentoAlerta = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
        }

        #endregion        
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> ConsultarRegistros(FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repMonitoramentoAlerta.ConsultarRelatorioAlerta(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repMonitoramentoAlerta.ContarRelatorioAlerta(filtrosPesquisa, propriedadesAgrupamento, null);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/MonitoramentoAlerta";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.Motorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista) : null;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.Transportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.Transportador) : null;
            
            parametros.Add(new Parametro("Tipo", filtrosPesquisa.TipoAlerta.ObterDescricao()));
            parametros.Add(new Parametro("CodigoCargaEmbarcador", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("Placa", filtrosPesquisa.Placa));
            parametros.Add(new Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));
            parametros.Add(new Parametro("Status", filtrosPesquisa.AlertaMonitorStatus.ObterDescricao()));
            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial?.ToString("dd/MM/yyyy HH:mm") : string.Empty));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal != DateTime.MinValue ? filtrosPesquisa.DataFinal?.ToString("dd/MM/yyyy HH:mm") : string.Empty));

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

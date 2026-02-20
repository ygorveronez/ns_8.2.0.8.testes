using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pedido
{
    public class PedidoDadosTransporteMaritimo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo, Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDadosTransporteMaritimo>
    {
        #region Atributos Privados 
        
        private readonly Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo _repositorioPedidoDadosTransporteMaritimo;

        #endregion

        #region Construtores
        
        public PedidoDadosTransporteMaritimo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDadosTransporteMaritimo> ConsultarRegistros(FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPedidoDadosTransporteMaritimo.ConsultarRelatorioPedidoDadosTransporteMaritimo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPedidoDadosTransporteMaritimo.ContarConsultaRelatorioPedidoDadosTransporteMaritimo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pedidos/Booking";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.Filial > 0 ? repositorioFilial.BuscarPorCodigo(filtrosPesquisa.Filial) : null;
            Dominio.Entidades.Localidade origem = filtrosPesquisa.Origem > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.Origem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.Destino > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.Destino) : null;

            string status = string.Empty;
            if (filtrosPesquisa.Status.HasValue)
                status = filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo ? "Ativo" : "Cancelado";

            parametros.Add(new Parametro("DataBooking", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFim));
            parametros.Add(new Parametro("NumeroEXP", filtrosPesquisa.NumeroEXP));
            parametros.Add(new Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCargaEmbarcador));
            parametros.Add(new Parametro("Status", status));
            parametros.Add(new Parametro("Filial", filial?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Origem", origem?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Destino", destino?.Descricao ?? string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "TipoEnvioDescricao")
                return "TipoEnvio";

            if (propriedadeOrdenarOuAgrupar == "StatusDescricao")
                return "Status";

            if (propriedadeOrdenarOuAgrupar == "TipoProbeDescricao")
                return "TipoProbe";

            if (propriedadeOrdenarOuAgrupar == "StatusEXPDescricao")
                return "StatusEXP";

            if (propriedadeOrdenarOuAgrupar == "FretePrepaidDescricao")
                return "FretePrepaid";

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}

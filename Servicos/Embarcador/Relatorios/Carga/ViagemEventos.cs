using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaViagemEventos : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaViagemEventos>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaViagemEventos _repCargaViagemEventos;

        #endregion

        #region Construtores

        public CargaViagemEventos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repCargaViagemEventos = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaViagemEventos(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaViagemEventos> ConsultarRegistros(FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repCargaViagemEventos.ConsultarRelatorioCargaViagemEventos(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repCargaViagemEventos.ContarConsultarRelatorioCargaViagemEventos(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaViagemEventos";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente clienteOrigem = filtrosPesquisa.CodigoClienteOrigem > 0 ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoClienteOrigem) : null;
            Dominio.Entidades.Cliente clienteDestino = filtrosPesquisa.CodigoClienteDestino > 0 ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoClienteDestino) : null;
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidadeOrigem = filtrosPesquisa.CodigoLocalidadeOrigem > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeOrigem) : null;
            Dominio.Entidades.Localidade localidadeDestino = filtrosPesquisa.CodigoLocalidadeDestino > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeDestino) : null;
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = filtrosPesquisa.CodigoCargaEmbarcador.Count > 0 ? repositorioCarga.BuscarPorCodigos(filtrosPesquisa.CodigoCargaEmbarcador) : null;

            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("CodigoFilial", filial?.Descricao ?? ""));
            parametros.Add(new Parametro("CodigoClienteOrigem", clienteOrigem?.Descricao ?? ""));
            parametros.Add(new Parametro("CodigoClienteDestino", clienteDestino?.Descricao ?? ""));
            parametros.Add(new Parametro("CodigoLocalidadeOrigem", localidadeOrigem?.DescricaoCidadeEstado ?? ""));
            parametros.Add(new Parametro("CodigoLocalidadeDestino", localidadeDestino?.DescricaoCidadeEstado ?? ""));
            parametros.Add(new Parametro("CodigoCargaEmbarcador",  carga?.Count > 0 ? string.Join(",",carga.Select(x => x.CodigoCargaEmbarcador)) : ""));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            //if (propriedadeOrdenarOuAgrupar == "SituacaoEntregaDescricao")
            //    return "SituacaoEntrega";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}

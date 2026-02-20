using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class ValePedagio : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio, Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioConhecimentoDeTransporteEletronico;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioConhecimentoDeTransporteEletronico.ConsultarRelatorioValePedagio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConhecimentoDeTransporteEletronico.ContarConsultaRelatorioValePedagio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/ValePedagio";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            Dominio.Entidades.Cliente recebedor = filtrosPesquisa.Recebedor > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Recebedor) : null;
            Dominio.Entidades.Cliente expedidor = filtrosPesquisa.Expedidor > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Expedidor) : null;

            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("DataCarga", filtrosPesquisa.DataCargaInicial, filtrosPesquisa.DataCargaFinal));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("Filial", filiais != null ? (from o in filiais select o.Descricao) : null));
            parametros.Add(new Parametro("NumeroValePedagio", string.Join(", ", filtrosPesquisa.NumeroValePedagio)));
            parametros.Add(new Parametro("SituacaoValePedagioDescricao", filtrosPesquisa.SituacaoValePedagio.Select(o => o.ObterDescricao())));
            parametros.Add(new Parametro("SituacaoIntegracaoValePedagioDescricao", filtrosPesquisa.SituacaoIntegracaoValePedagio.Select(o => o.ObterDescricao())));
            parametros.Add(new Parametro("Expedidor", expedidor?.Descricao));
            parametros.Add(new Parametro("Recebedor", recebedor?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataCargaFormatada")
                return "DataCarga";
            if (propriedadeOrdenarOuAgrupar == "DataRetornoValePedagioFormatada")
                return "DataRetornoValePedagio";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}

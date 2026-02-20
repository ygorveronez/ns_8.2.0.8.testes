using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.NFes
{
    public class NFes : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe, Dominio.Relatorios.Embarcador.DataSource.NFe.NFes>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repNotaFiscal;

        #endregion

        #region Construtores

        public NFes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repNotaFiscal.RelatorioNFesCargas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repNotaFiscal.ContarConsultaRelatorioNFesCargas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/NFes";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = filtrosPesquisa.CodigosCarga.Count > 0 ? repCarga.BuscarPorCodigos(filtrosPesquisa.CodigosCarga) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Cliente> remetentes = filtrosPesquisa.CpfCnpjsRemetente.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsRemetente) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = filtrosPesquisa.CpfCnpjsDestinatario.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsDestinatario) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> expedidores = filtrosPesquisa.CpfCnpjsExpedidor.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsExpedidor) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportador.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportador) : new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Estado> UForigem = filtrosPesquisa.EstadosOrigem.Count > 0 ? repEstado.BuscarPorSiglas(filtrosPesquisa.EstadosOrigem) : new List<Dominio.Entidades.Estado>();
            List<Dominio.Entidades.Estado> UFDestino = filtrosPesquisa.EstadosDestino.Count > 0 ? repEstado.BuscarPorSiglas(filtrosPesquisa.EstadosDestino) : new List<Dominio.Entidades.Estado>();
            List<Dominio.Entidades.Localidade> origem = filtrosPesquisa.CodigosOrigem.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosOrigem) : new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destino = filtrosPesquisa.CodigosDestino.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosDestino) : new List<Dominio.Entidades.Localidade>();
            List<string> descricoesGrupoPessoas = filtrosPesquisa.CodigosGrupoPessoas.Count > 0 ? repGrupoPessoas.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigosGrupoPessoas) : new List<string>();
            List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricoesEntregas = filtrosPesquisa.CodigosRestricoes.Count > 0 ? repRestricaoEntrega.BuscarPorCodigos(filtrosPesquisa.CodigosRestricoes) : new List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial?.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao?.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = filtrosPesquisa.CodigosTipoCarga?.Count > 0 ? repTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCarga) : new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            List<Dominio.Entidades.Usuario> motoristas = filtrosPesquisa.CodigosMotorista?.Count > 0 ? repUsuario.BuscarMotoristaPorCodigo(filtrosPesquisa.CodigosMotorista) : new List<Dominio.Entidades.Usuario>();

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Parametro("DataEmissaoCTe", filtrosPesquisa.DataInicialEmissaoCTe, filtrosPesquisa.DataFinalEmissaoCTe));
            parametros.Add(new Parametro("DataEmissaoCarga", filtrosPesquisa.DataInicialEmissaoCarga, filtrosPesquisa.DataFinalEmissaoCarga));
            parametros.Add(new Parametro("DataPrevisaoEntregaPedido", filtrosPesquisa.DataInicialPrevisaoEntregaPedido, filtrosPesquisa.DataFinalPrevisaoEntregaPedido, true));
            parametros.Add(new Parametro("DataInicioViagemPlanejada", filtrosPesquisa.DataInicialInicioViagemPlanejada, filtrosPesquisa.DataFinalInicioViagemPlanejada));
            parametros.Add(new Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("QuantidadeVolumes", filtrosPesquisa.QuantidadeVolumesInicial, filtrosPesquisa.QuantidadeVolumesFinal));
            parametros.Add(new Parametro("ClassificacaoNFe", filtrosPesquisa.ClassificacaoNFe != ClassificacaoNFe.Todos ? filtrosPesquisa.ClassificacaoNFe.ObterDescricao() : ""));
            parametros.Add(new Parametro("Carga", from o in cargas select o.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("Remetente", from o in remetentes select o.Nome));
            parametros.Add(new Parametro("Destinatario", from o in destinatarios select o.Nome));
            parametros.Add(new Parametro("Expedidor", from o in expedidores select o.Nome));
            parametros.Add(new Parametro("Transportador", from o in transportadores select o.RazaoSocial));
            parametros.Add(new Parametro("Filial", filiais.Select(o => o.Descricao)));
            parametros.Add(new Parametro("SituacaoFatura", filtrosPesquisa.SituacaoFatura?.ObterDescricao()));
            parametros.Add(new Parametro("Origem", origem.Select(o => o.Descricao)));
            parametros.Add(new Parametro("EstadoOrigem", UForigem.Select(o => o.Nome)));
            parametros.Add(new Parametro("Destino", destino.Select(o => o.Descricao)));
            parametros.Add(new Parametro("EstadoDestino", UFDestino.Select(o => o.Nome)));
            parametros.Add(new Parametro("TipoLocalPrestacao", filtrosPesquisa.TipoLocalPrestacao.ObterDescricao()));
            parametros.Add(new Parametro("GrupoPessoas", descricoesGrupoPessoas));
            parametros.Add(new Parametro("Restricao", restricoesEntregas.Select(o => o.Descricao)));
            parametros.Add(new Parametro("TipoCTe", string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("NotasFiscaisSemCarga", filtrosPesquisa.NotasFiscaisSemCarga.HasValue ? filtrosPesquisa.NotasFiscaisSemCarga.Value ? "Sim" : "Não" : ""));
            parametros.Add(new Parametro("CargaTransbordo", filtrosPesquisa.CargaTransbordo.HasValue ? filtrosPesquisa.CargaTransbordo.Value ? "Sim" : "Não" : ""));
            parametros.Add(new Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                parametros.Add(new Parametro("Situacao", string.Join(", ", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao()))));
            else
                parametros.Add(new Parametro("Situacao", string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("SituacaoEntrega", string.Join(", ", filtrosPesquisa.SituacoesEntrega.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("TipoOperacao", tiposOperacao.Select(o => o.Descricao)));
            parametros.Add(new Parametro("TipoCarga", tiposCarga.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Motorista", motoristas.Select(o => o.Descricao)));
            parametros.Add(new Parametro("PossuiExpedidor", filtrosPesquisa.PossuiExpedidor != 9 ? filtrosPesquisa.PossuiExpedidor == 1 ? "Sim" : "Não" : string.Empty));
            parametros.Add(new Parametro("PossuiRecebedor", filtrosPesquisa.PossuiRecebedor != 9 ? filtrosPesquisa.PossuiRecebedor == 1 ? "Sim" : "Não" : string.Empty));
            parametros.Add(new Parametro("DataPrevisaoCargaEntrega", filtrosPesquisa.DataPrevisaoCargaEntregaInicial, filtrosPesquisa.DataPrevisaoCargaEntregaFinal, true));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CNPJEmpresa")
                return "CNPJEmpresaSemFormato";

            if (propriedadeOrdenarOuAgrupar == "CNPJRemetente")
                return "CNPJRemetenteSemFormato";

            if (propriedadeOrdenarOuAgrupar == "CNPJDestinatario")
                return "CNPJDestinatarioSemFormato";

            if ((propriedadeOrdenarOuAgrupar == "DataPedido") || (propriedadeOrdenarOuAgrupar == "HoraPedido"))
                return "DataHoraPedido";

            return propriedadeOrdenarOuAgrupar;
        }

        public bool ExisteCargaSemNotasFiscais(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa)
        {
            return _repNotaFiscal.ContarConsultaRelatorioNFesCargas(filtrosPesquisa, new List<PropriedadeAgrupamento>()) == 0;
        }

        #endregion
    }
}
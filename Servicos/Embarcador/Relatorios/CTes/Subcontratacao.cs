using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class Subcontratacao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao, Dominio.Relatorios.Embarcador.DataSource.CTe.Subcontratacao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.CTe.CTeTerceiro _repositorioCTeTerceiro;

        #endregion

        #region Construtores

        public Subcontratacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.Subcontratacao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCTeTerceiro.ConsultarRelatorioSubcontratacao(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCTeTerceiro.ContarConsultaRelatorioSubcontratacao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/Subcontratacao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(filtrosPesquisa.CodigosCarga);
            List<Dominio.Entidades.Cliente> remetentes = repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsRemetente);
            List<Dominio.Entidades.Cliente> destinatarios = repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsDestinatario);
            List<Dominio.Entidades.Empresa> transportadores = repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportador);
            List<Dominio.Entidades.Estado> UForigem = repEstado.BuscarPorSiglas(filtrosPesquisa.EstadosOrigem);
            List<Dominio.Entidades.Estado> UFDestino = repEstado.BuscarPorSiglas(filtrosPesquisa.EstadosDestino);
            List<Dominio.Entidades.Localidade> origem = repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosOrigem);
            List<Dominio.Entidades.Localidade> destino = repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosDestino);
            List<string> descricoesGrupoPessoas = repGrupoPessoas.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigosGrupoPessoas);
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCarga", filtrosPesquisa.DataInicialEmissaoCarga, filtrosPesquisa.DataFinalEmissaoCarga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalizacaoEmissao", filtrosPesquisa.DataInicialFinalizacaoEmissao, filtrosPesquisa.DataFinalFinalizacaoEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", string.Join(", ", from o in cargas select o.CodigoCargaEmbarcador)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCargaEmbarcador", filtrosPesquisa.NumeroCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", string.Join(", ", from o in remetentes select o.Nome)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", string.Join(", ", from o in destinatarios select o.Nome)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", string.Join(", ", from o in transportadores select o.RazaoSocial)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", string.Join(", ", from o in filiais select o.Descricao)));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", string.Join(", ", origem.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", string.Join(", ", UForigem.Select(o => o.Nome.Trim()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", string.Join(", ", destino.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", string.Join(", ", UFDestino.Select(o => o.Nome.Trim()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", string.Join(", ", descricoesGrupoPessoas)));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCTe", string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ObterDescricao()))));

            if (filtrosPesquisa.TiposServicos != null && filtrosPesquisa.TiposServicos.Count > 0)
            {
                List<string> servicos = new List<string>();
                foreach (int tipoServico in filtrosPesquisa.TiposServicos)
                    servicos.Add(((Dominio.Enumeradores.TipoServico)tipoServico).ObterDescricao());
                string descricaoTipoServico = string.Join(", ", servicos.Select(o => o));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", descricaoTipoServico, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", false));

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacoesCargaMercante != null ? string.Join(",", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())) : null));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCarga.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoSEFAZ", filtrosPesquisa.SituacaoSEFAZ.Select(o => o.ObterDescricao())));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataCargaFormatada")
                return "DataCarga";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoCTe")
                return "TipoCTe";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoServico")
                return "TipoServico";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoTomador")
                return "TipoTomador";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
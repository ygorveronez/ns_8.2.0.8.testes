using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace EmissaoCTe.WebApp
{
    public partial class EmissaoCTe : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

                this.PreencherDDLNaturezaOperacao(unitOfWork);
                this.PreencherDDLModalTransporte(unitOfWork);
                this.PreencherDDLsUF(unitOfWork);
                this.PreencherDDLsMunicipio(unitOfWork);
                this.PreencherDDLModelo(unitOfWork);
                this.PreencherDadosEmitente(unitOfWork);
                this.PreencherDDLSerie(unitOfWork);
                this.PreencherDDLsPais(unitOfWork);
                if(!this.EmpresaUsuario.PermiteEmissaoDocumentosDestinados)
                    this.spnNFesDestinadas.Style["display"] = "none";
                if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao?.IDMercadoLivre))
                    this.spnMercadoLivre.Style["display"] = "none";

            }


            //List<string> filias = new List<string>();
            //filias.Add("T990");
            //filias.Add("T113369");

            //Servicos.EDI.StartupEDINatura serStartupEDINatura = new Servicos.EDI.StartupEDINatura(Conexao.StringConexao);
            //serStartupEDINatura.Iniciar(this.EmpresaUsuario.Codigo, 10, @"D:\Arquivos\Alianca", filias);

        }

        #endregion

        #region Metodos

        private void PreencherDDLSerie(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.EmpresaSerie> listaSeries = new List<Dominio.Entidades.EmpresaSerie>();

            if (this.Usuario.Series.Count > 0)
                listaSeries = this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe && o.Status == "A").ToList();
            else
            {
                Repositorio.EmpresaSerie repositorioSerie = new Repositorio.EmpresaSerie(unitOfWork);
                listaSeries = repositorioSerie.BuscarTodosPorEmpresa(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoSerie.CTe, "A");
            }

            foreach (Dominio.Entidades.EmpresaSerie serie in listaSeries)
            {
                this.ddlSerie.Items.Add(new ListItem(serie.Numero.ToString(), serie.Codigo.ToString()));
                this.ddlSerieFiltro.Items.Add(new ListItem(serie.Numero.ToString(), serie.Codigo.ToString()));
            }
        }

        private void PreencherDDLsPais(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork);
            List<Dominio.Entidades.Pais> listaPaises = repositorioPais.BuscarTodos();
            foreach (Dominio.Entidades.Pais pais in listaPaises)
            {
                this.ddlPaisTomador.Items.Add(new ListItem(pais.Nome, pais.Sigla.ToUpper()));
                this.ddlPaisRemetente.Items.Add(new ListItem(pais.Nome, pais.Sigla.ToUpper()));
                this.ddlPaisExpedidor.Items.Add(new ListItem(pais.Nome, pais.Sigla.ToUpper()));
                this.ddlPaisRecebedor.Items.Add(new ListItem(pais.Nome, pais.Sigla.ToUpper()));
                this.ddlPaisDestinatario.Items.Add(new ListItem(pais.Nome, pais.Sigla.ToUpper()));
            }
        }

        private void PreencherDDLsUF(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            List<Dominio.Entidades.Estado> listaUF = repositorioEstado.BuscarTodos();
            foreach (Dominio.Entidades.Estado uf in listaUF)
            {
                this.ddlUFInicioPrestacao.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlUFLocalEmissaoCTe.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlUFTerminoPrestacao.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlEstadoTomador.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlEstadoRemetente.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlEstadoExpedidor.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlEstadoRecebedor.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlEstadoDestinatario.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
                this.ddlUFLocalEntregaDiferenteDestinatario.Items.Add(new ListItem(string.Concat(uf.Sigla, " - ", uf.Nome), uf.Sigla));
            }
        }

        private void PreencherDDLsMunicipio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            List<Dominio.Entidades.Localidade> listaMunicipios = repositorioLocalidade.BuscarPorUF(this.EmpresaUsuario.Localidade.Estado.Sigla, this.EmpresaUsuario.Codigo);
            foreach (Dominio.Entidades.Localidade municipio in listaMunicipios)
            {
                this.ddlMunicipioInicioPrestacao.Items.Add(new ListItem(municipio.Descricao, municipio.Codigo.ToString()));
                this.ddlMunicipioLocalEmissaoCTe.Items.Add(new ListItem(municipio.Descricao, municipio.Codigo.ToString()));
                this.ddlMunicipioTerminoPrestacao.Items.Add(new ListItem(municipio.Descricao, municipio.Codigo.ToString()));
            }
        }

        private void PreencherDDLNaturezaOperacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NaturezaDaOperacao repositorioNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            var listaNaturezaOperacao = repositorioNaturezaOperacao.BuscarTodosPorTipoCFOP(Dominio.Enumeradores.TipoCFOP.Saida);

            foreach (Dominio.Entidades.NaturezaDaOperacao natureza in listaNaturezaOperacao)
            {
                if (natureza != null)
                    this.ddlNaturezaOperacao.Items.Add(new ListItem(natureza.Descricao, natureza.Codigo.ToString()));
            }
        }

        private void PreencherDDLModalTransporte(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModalTransporte repositorioModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            var listaModais = repositorioModalTransporte.BuscarTodos();
            foreach (Dominio.Entidades.ModalTransporte modal in listaModais)
                this.ddlModalTransporte.Items.Add(new ListItem(modal.Descricao, modal.Codigo.ToString()));
        }

        private void PreencherDDLModelo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            var listaModelos = repositorioModeloDocumentoFiscal.BuscarTodos();
            foreach (Dominio.Entidades.ModeloDocumentoFiscal modelo in listaModelos)
            {
                //this.ddlModelo.Items.Add(new ListItem(modelo.Numero, modelo.Codigo.ToString()));
                this.ddlModeloNFouCTEmitidoTomador.Items.Add(new ListItem(modelo.Numero, modelo.Codigo.ToString()));
                //if (modelo.Numero.Equals("57"))
                //{
                //    this.ddlModelo.SelectedValue = modelo.Codigo.ToString();
                //    this.ddlModelo.Enabled = false;
                //}
                if (modelo.Numero.Equals("0") || modelo.Numero.Equals("00") || modelo.Numero.Equals("99"))
                    this.ddlTipoDocumentoOutrosRemetente.Items.Add(new ListItem(string.Concat(modelo.Numero, " - ", modelo.Descricao), modelo.Codigo.ToString()));
                if (modelo.Numero.Equals("1") || modelo.Numero.Equals("01") || modelo.Numero.Equals("4") || modelo.Numero.Equals("04"))
                    this.ddlModeloNotaFiscaiRemetente.Items.Add(new ListItem(string.Concat(modelo.Numero, " - ", modelo.Descricao), modelo.Codigo.ToString()));
            }
        }

        private void PreencherDadosEmitente(Repositorio.UnitOfWork unitOfWork)
        {
            this.txtBairroEmitente.Text = this.EmpresaUsuario.Bairro;
            this.txtCEPEmitente.Text = this.EmpresaUsuario.CEP;
            this.txtCidadeEmitente.Text = this.EmpresaUsuario.Localidade.Descricao;
            this.txtCNPJEmitente.Text = this.EmpresaUsuario.CNPJ;
            this.txtComplementoEmitente.Text = this.EmpresaUsuario.Complemento;
            this.txtEmailsContadorEmitente.Text = this.EmpresaUsuario.EmailContador;
            this.txtEmailsAdministrativosEmitente.Text = this.EmpresaUsuario.EmailAdministrativo;
            this.txtEmailsEmitente.Text = this.EmpresaUsuario.Email;
            this.txtEnderecoEmitente.Text = this.EmpresaUsuario.Endereco;
            this.txtIEEmitente.Text = this.EmpresaUsuario.InscricaoEstadual;
            this.txtNomeFantasiaEmitente.Text = this.EmpresaUsuario.NomeFantasia;
            this.txtNumeroEmitente.Text = this.EmpresaUsuario.Numero;
            this.txtRazaoSocialEmitente.Text = this.EmpresaUsuario.RazaoSocial;
            this.txtTelefoneEmitente.Text = this.EmpresaUsuario.Telefone;
            this.txtUFEmitente.Text = this.EmpresaUsuario.Localidade.Estado.Sigla;
            this.chkEmailsAdministrativosStatusEmitente.Checked = string.IsNullOrEmpty(this.EmpresaUsuario.StatusEmailAdministrativo) || this.EmpresaUsuario.StatusEmailAdministrativo.Equals("I") ? false : this.EmpresaUsuario.StatusEmailAdministrativo.Equals("A");
            this.chkEmailsContadorStatusEmitente.Checked = string.IsNullOrEmpty(this.EmpresaUsuario.StatusEmailContador) || this.EmpresaUsuario.StatusEmailContador.Equals("I") ? false : this.EmpresaUsuario.StatusEmailContador.Equals("A");
            this.chkEmailsStatusEmitente.Checked = string.IsNullOrEmpty(this.EmpresaUsuario.Status) || this.EmpresaUsuario.Status.Equals("I") ? false : this.EmpresaUsuario.Status.Equals("A");
            this.ddlMunicipioInicioPrestacao.SelectedValue = this.Usuario.Localidade.Codigo.ToString();
            this.ddlMunicipioLocalEmissaoCTe.SelectedValue = this.Usuario.Localidade.Codigo.ToString();
            this.ddlMunicipioTerminoPrestacao.SelectedValue = this.Usuario.Localidade.Codigo.ToString();
            this.ddlUFInicioPrestacao.SelectedValue = this.Usuario.Localidade.Estado.Sigla;
            this.ddlUFLocalEmissaoCTe.SelectedValue = this.Usuario.Localidade.Estado.Sigla;
            this.ddlUFTerminoPrestacao.SelectedValue = this.Usuario.Localidade.Estado.Sigla;
            this.txtRNTRC.Text = this.EmpresaUsuario.RegistroANTT;
            this.txtRNTRCEmpresa.Text = this.EmpresaUsuario.RegistroANTT;
        }

        #endregion
    }
}
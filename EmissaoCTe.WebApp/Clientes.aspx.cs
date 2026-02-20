using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace EmissaoCTe.WebApp
{
    public partial class Clientes : PaginaBaseSegura
    {
        #region Variáveis Globais

        Repositorio.DadosCliente _RepositorioDadosCliente;
        Repositorio.Cliente _RepositorioCliente;
        Repositorio.Atividade _RepositorioAtividade;
        Repositorio.Localidade _RepositorioLocalidade;
        Repositorio.Banco _RepositorioBanco;
        Repositorio.Estado _RepositorioEstado;
        Dominio.Entidades.Cliente _Cliente;
        Dominio.Entidades.DadosCliente _DadosCliente;

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("clientes.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {
            this.InicializarRepositorios();
            if (!IsPostBack)
            {
                this.CarregarDDLUF();
                this.LimparCampos();
            }
        }

        protected void btnSalvarASP_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ValidarDados())
                {
                    if (this._RepositorioCliente.BuscarPorCPFCNPJ(this._Cliente.CPF_CNPJ) == null)
                    {
                        if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptAcessoNegado", "$('#divMensagemAlerta .mensagem').text('Permissão para inclusão de cliente negada!'); $('#divMensagemAlerta').slideDown();", true);
                        }
                        else
                        {
                            this._Cliente.Ativo = true;
                            this._RepositorioCliente.Inserir(this._Cliente, Auditado);

                            if (this._DadosCliente.Codigo > 0)
                                this._RepositorioDadosCliente.Atualizar(this._DadosCliente, Auditado);
                            else
                                this._RepositorioDadosCliente.Inserir(this._DadosCliente);

                            this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptSucesso", "$('#divMensagemSucesso').slideDown();", true);
                            this.LimparCampos();
                        }
                    }
                    else
                    {
                        if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptAcessoNegado", "$('#divMensagemAlerta .mensagem').text('Permissão para alteração de cliente negada!'); $('#divMensagemAlerta').slideDown();", true);
                        }
                        else
                        {
                            Dominio.Entidades.Auditoria.HistoricoObjeto historico = null;
                            historico = this._RepositorioCliente.Atualizar(this._Cliente, Auditado);

                            if (this._DadosCliente.Codigo > 0)
                                this._RepositorioDadosCliente.Atualizar(this._DadosCliente, Auditado, historico);
                            else
                                this._RepositorioDadosCliente.Inserir(this._DadosCliente);

                            this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptSucesso", "$('#divMensagemSucesso').slideDown();", true);
                            this.LimparCampos();
                        }
                    }

                }
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptDadosIncorretos", "$('#divMensagemAlerta .mensagem').text('Os campos com asterísco (*) ou em vermelho são obrigatórios.'); $('#divMensagemAlerta').slideDown();", true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                this.ClientScript.RegisterStartupScript(this.GetType(), "ScriptDadosIncorretos", "$('#divMensagemErro').slideDown();", true);
            }
        }

        #endregion

        #region Métodos

        private void InicializarRepositorios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            this._RepositorioAtividade = new Repositorio.Atividade(unitOfWork);
            this._RepositorioCliente = new Repositorio.Cliente(unitOfWork);
            this._RepositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            this._RepositorioEstado = new Repositorio.Estado(unitOfWork);
            this._RepositorioDadosCliente = new Repositorio.DadosCliente(unitOfWork);
            this._RepositorioBanco = new Repositorio.Banco(unitOfWork);
        }

        private void CarregarDDLUF()
        {
            var listaUF = this._RepositorioEstado.BuscarTodos();

            foreach (Dominio.Entidades.Estado uf in listaUF)
            {
                this.ddlUF.Items.Add(new ListItem(uf.Sigla + " - " + uf.Nome, uf.Sigla));
                this.ddlUFRG.Items.Add(new ListItem(uf.Sigla + " - " + uf.Nome, uf.Sigla));
            }
        }

        private bool ValidarDados()
        {
            int.TryParse(this.hddIdBanco.Value, out int codigoBanco);
            int.TryParse(this.hddIdAtividade.Value, out int idAtividade);
            int.TryParse(this.hddIdLocalidade.Value, out int idLocalidade);

            double cpfCnpj;
            double.TryParse(this.txtCPFCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", ""), out cpfCnpj);

            if (cpfCnpj <= 0)
                return false;
            if (idAtividade <= 0)
                return false;
            if (idLocalidade <= 0)
                return false;
            if (string.IsNullOrWhiteSpace(this.txtBairro.Text))
                return false;
            if (string.IsNullOrWhiteSpace(this.txtCEP.Text))
                return false;
            if (string.IsNullOrWhiteSpace(this.txtEndereco.Text))
                return false;
            if (string.IsNullOrWhiteSpace(this.txtNumero.Text))
                return false;
            if (string.IsNullOrWhiteSpace(this.txtRazaoSocial.Text))
                return false;

            this._Cliente = this._RepositorioCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (this._Cliente == null)
            {
                this._Cliente = new Dominio.Entidades.Cliente
                {
                    DataCadastro = DateTime.Now
                };
            }
            else
                this._Cliente.Initialize();

            this._Cliente.CPF_CNPJ = cpfCnpj;
            this._Cliente.Atividade = this._RepositorioAtividade.BuscarPorCodigo(idAtividade);
            this._Cliente.Bairro = this.txtBairro.Text;
            this._Cliente.CEP = this.txtCEP.Text.Replace(".", "").Replace("-", "");
            this._Cliente.Complemento = this.txtComplemento.Text;
            this._Cliente.Email = this.txtEmails.Text;
            this._Cliente.EmailContador = this.txtEmailsContador.Text;
            this._Cliente.EmailContato = this.txtEmailsContato.Text;
            this._Cliente.EmailStatus = this.chkEmailsStatus.Checked ? "A" : "I";
            this._Cliente.EmailContadorStatus = this.chkEmailsContadorStatus.Checked ? "A" : "I";
            this._Cliente.EmailContatoStatus = this.chkEmailsContatoStatus.Checked ? "A" : "I";
            this._Cliente.Endereco = this.txtEndereco.Text;
            this._Cliente.IE_RG = (this.ddlTipo.SelectedValue == "J" ? (this.txtRGIE.Text == string.Empty ? "ISENTO" : this.txtRGIE.Text) : this.txtRGIE.Text);
            this._Cliente.Localidade = this._RepositorioLocalidade.BuscarPorCodigo(idLocalidade);
            this._Cliente.Nome = this.txtRazaoSocial.Text;
            this._Cliente.NomeFantasia = this.txtNomeFantasia.Text;
            this._Cliente.Numero = this.txtNumero.Text;
            this._Cliente.Telefone1 = this.txtTelefone1.Text;
            this._Cliente.Telefone2 = this.txtTelefone2.Text;
            this._Cliente.Tipo = this.ddlTipo.SelectedValue;
            this._Cliente.EstadoRG = this._RepositorioEstado.BuscarPorSigla(this.ddlUFRG.SelectedValue);
            this._Cliente.InscricaoST = this.txtInscricaoST.Text;
            this._Cliente.InscricaoSuframa = this.txtSuframa.Text;

            if (DateTime.TryParseExact(this.txtDataNascimento.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataNascimento))
                this._Cliente.DataNascimento = dataNascimento;
            else
                this._Cliente.DataNascimento = null;

            Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG orgaoEmissorRG;
            Dominio.ObjetosDeValor.Enumerador.Sexo sexo;

            if (Enum.TryParse(this.ddlOrgaoEmissorRG.SelectedValue, out orgaoEmissorRG))
                this._Cliente.OrgaoEmissorRG = orgaoEmissorRG;
            else
                this._Cliente.OrgaoEmissorRG = null;

            if (Enum.TryParse(this.ddlSexo.SelectedValue, out sexo))
                this._Cliente.Sexo = sexo;
            else
                this._Cliente.OrgaoEmissorRG = null;

            this._DadosCliente = this._RepositorioDadosCliente.Buscar(this.EmpresaUsuario.Codigo, this._Cliente.CPF_CNPJ);

            if (this._DadosCliente == null)
            {
                this._DadosCliente = new Dominio.Entidades.DadosCliente
                {
                    Cliente = this._Cliente,
                    Empresa = this.EmpresaUsuario
                };
            }
            else
            {
                this._DadosCliente.Initialize();
            }

            this._DadosCliente.Agencia = this.txtAgencia.Text;
            this._DadosCliente.DigitoAgencia = this.txtDigitoAgencia.Text;
            this._DadosCliente.NumeroConta = this.txtNumeroConta.Text;
            this._DadosCliente.NumeroCartao = this.txtNumeroCartao.Text;
            decimal percentualRetencaoICMSST = 0m;
            decimal.TryParse(this.txtPercentualRetencaoICMSST.Text, out percentualRetencaoICMSST);
            this._DadosCliente.PercentualRetencaoICMSST = percentualRetencaoICMSST;
            this._DadosCliente. PIS = this.txtPIS.Text;
            this._DadosCliente.Email = this.txtEmailsTransportador.Text;
            this._DadosCliente.EmailStatus = this.chkEmailsTransportadorStatus.Checked ? "A" : "I";
            this._DadosCliente.NomeFantasia = this.txtNomeFantasiaTransportador.Text;
            this._DadosCliente.ArmazenaNotasParaGerarPorPeriodo = this.chkArmazenaNotasParaGerarPorPeriodo.Checked;
            this._DadosCliente.NaoAverbarQuandoTerceiro = this.chkNaoAverbarQuandoTerceiro.Checked;

            Dominio.ObjetosDeValor.Enumerador.TipoConta tipoConta;

            if (Enum.TryParse<Dominio.ObjetosDeValor.Enumerador.TipoConta>(this.ddlTipoConta.SelectedValue, out tipoConta))
                this._DadosCliente.TipoConta = tipoConta;
            else
                this._DadosCliente.TipoConta = null;

            this._DadosCliente.Banco = this._RepositorioBanco.BuscarPorCodigo(codigoBanco);

            return true;
        }

        private void LimparCampos()
        {
            this.ddlTipo.SelectedValue = "J";
            this.txtBairro.Text = "";
            this.txtCEP.Text = "";
            this.txtComplemento.Text = "";
            this.txtCPFCNPJ.Text = "";
            this.txtDataCadastro.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            this.txtEmails.Text = "";
            this.txtEmailsContador.Text = "";
            this.txtEmailsContato.Text = "";
            this.txtEndereco.Text = "";
            this.txtNomeFantasia.Text = "";
            this.txtNumero.Text = "";
            this.txtRazaoSocial.Text = "";
            this.txtRGIE.Text = "";
            this.txtTelefone1.Text = "";
            this.txtTelefone2.Text = "";
            this.chkEmailsContadorStatus.Checked = false;
            this.chkEmailsContatoStatus.Checked = false;
            this.chkEmailsStatus.Checked = false;
            this.txtAtividade.Text = "";
            this.ddlUF.SelectedIndex = 0;
            this.ddlUFRG.SelectedIndex = 0;
            this.ddlSexo.SelectedIndex = 0;
            this.ddlOrgaoEmissorRG.SelectedIndex = 0;
            this.txtDataNascimento.Text = "";
            this.txtInscricaoST.Text = "";
            this.txtSuframa.Text = "";

            this.txtAgencia.Text = "";
            this.txtDigitoAgencia.Text = "";
            this.txtNumeroConta.Text = "";
            this.ddlTipo.SelectedIndex = 0;
            this.hddIdBanco.Value = "";
            this.txtBanco.Text = "";
            this.txtNumeroCartao.Text = "";
            this.txtPercentualRetencaoICMSST.Text = "0,00";
            this.txtPIS.Text = "";
            this.txtEmailsTransportador.Text = "";
            this.chkEmailsTransportadorStatus.Checked = false;
            this.txtNomeFantasiaTransportador.Text = "";
            this.chkArmazenaNotasParaGerarPorPeriodo.Checked = false;
            this.chkNaoAverbarQuandoTerceiro.Checked = false;
        }

        #endregion
    }
}
using System;

namespace Servicos
{
    public class NDDigital : ServicoBase
    {        

        public NDDigital(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Cliente ObterRemetente(Dominio.NDDigital.v104.Emissao.Registro12100 rem, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rem == null || codigoEmpresa <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(rem.CPF_CNPJ));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            bool inserir = false;

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }

            cliente.Bairro = rem.enderReme.xBairro;
            cliente.CEP = rem.enderReme.CEP;
            cliente.Complemento = rem.enderReme.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = rem.enderReme.xLgr;
            cliente.IE_RG = rem.IE;
            cliente.InscricaoMunicipal = null;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(rem.enderReme.cMun);
            cliente.Nome = rem.xNome;
            cliente.NomeFantasia = rem.xFant;
            cliente.Numero = rem.enderReme.nro;
            cliente.Telefone1 = rem.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(rem.CPF_CNPJ).Length == 14 ? "J" : "F";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, rem.ativ, unidadeDeTrabalho);

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
                repCliente.Atualizar(cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterExpedidor(Dominio.NDDigital.v104.Emissao.Registro12200 exp, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (exp == null || codigoEmpresa <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(exp.CPF_CNPJ));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            bool inserir = false;

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }

            cliente.Bairro = exp.enderExped.xBairro;
            cliente.CEP = exp.enderExped.CEP;
            cliente.Complemento = exp.enderExped.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = exp.enderExped.xLgr;
            cliente.IE_RG = exp.IE;
            cliente.InscricaoMunicipal = null;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(exp.enderExped.cMun);
            cliente.Nome = exp.xNome;
            cliente.NomeFantasia = exp.xNome;
            cliente.Numero = exp.enderExped.nro;
            cliente.Telefone1 = exp.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(exp.CPF_CNPJ).Length == 14 ? "J" : "F";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, exp.ativ, unidadeDeTrabalho);

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
                repCliente.Atualizar(cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterRecebedor(Dominio.NDDigital.v104.Emissao.Registro12300 rec, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rec == null || codigoEmpresa <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(rec.CPF_CNPJ));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            bool inserir = false;

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }

            cliente.Bairro = rec.enderReceb.xBairro;
            cliente.CEP = rec.enderReceb.CEP;
            cliente.Complemento = rec.enderReceb.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = rec.enderReceb.xLgr;
            cliente.IE_RG = rec.IE;
            cliente.InscricaoMunicipal = null;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(rec.enderReceb.cMun);
            cliente.Nome = rec.xNome;
            cliente.NomeFantasia = rec.xNome;
            cliente.Numero = rec.enderReceb.nro;
            cliente.Telefone1 = rec.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(rec.CPF_CNPJ).Length == 14 ? "J" : "F";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, rec.ativ, unidadeDeTrabalho);

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
                repCliente.Atualizar(cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterDestinatario(Dominio.NDDigital.v104.Emissao.Registro12400 dest, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dest == null || codigoEmpresa <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(dest.CPF_CNPJ));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            bool inserir = false;

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }

            cliente.Bairro = dest.enderDest.xBairro;
            cliente.CEP = dest.enderDest.CEP;
            cliente.Complemento = dest.enderDest.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = dest.enderDest.xLgr;
            cliente.IE_RG = dest.IE;
            cliente.InscricaoMunicipal = null;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(dest.enderDest.cMun);
            cliente.Nome = dest.xNome;
            cliente.NomeFantasia = dest.xNome;
            cliente.Numero = dest.enderDest.nro;
            cliente.Telefone1 = dest.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(dest.CPF_CNPJ).Length == 14 ? "J" : "F";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, dest.ativ, unidadeDeTrabalho);

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
                repCliente.Atualizar(cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterTomador(Dominio.NDDigital.v104.Emissao.Registro11120 toma, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (toma == null || codigoEmpresa <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(toma.CPF_CNPJ));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            bool inserir = false;

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }

            cliente.Bairro = toma.enderToma.xBairro;
            cliente.CEP = toma.enderToma.CEP;
            cliente.Complemento = toma.enderToma.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = toma.enderToma.xLgr;
            cliente.IE_RG = toma.IE;
            cliente.InscricaoMunicipal = null;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(toma.enderToma.cMun);
            cliente.Nome = toma.xNome;
            cliente.NomeFantasia = toma.xNome;
            cliente.Numero = toma.enderToma.xNum;
            cliente.Telefone1 = toma.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(toma.CPF_CNPJ).Length == 14 ? "J" : "F";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, toma.ativ, unidadeDeTrabalho);

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
                repCliente.Atualizar(cliente);

            return cliente;
        }

        public Dominio.NDDigital.v104.Emissao.Retorno GerarRetornoEmissao(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                Dominio.NDDigital.v104.Emissao.Retorno retorno = new Dominio.NDDigital.v104.Emissao.Retorno();

                retorno.AliquotaICMS = cte.AliquotaICMS;
                retorno.CFOP = string.Format("{0:0000}", cte.CFOP.CodigoCFOP);
                retorno.chCTe = cte.Chave;
                retorno.chCTe2 = string.Empty;
                retorno.CST = cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? "001" : cte.CST;
                retorno.cStat = cte.MensagemStatus != null ? string.Format("{0:000}", cte.MensagemStatus.CodigoDoErro) : string.Empty;
                retorno.dhRecbto = cte.DataRetornoSefaz.HasValue ? cte.DataRetornoSefaz.Value : DateTime.MinValue;
                retorno.digVal = string.Empty;
                retorno.id = string.Concat("ID", cte.Codigo);
                retorno.nProt = cte.Protocolo;
                retorno.PercentualReducaoBaseCalculo = cte.PercentualReducaoBaseCalculoICMS;
                retorno.tpAmb = (int)cte.TipoAmbiente;
                retorno.tpEmis = 1;
                retorno.ValorAPagar = cte.ValorAReceber;
                retorno.ValorBaseCalculoICMS = cte.BaseCalculoICMS;
                retorno.ValorICMS = cte.ValorICMS;
                retorno.ValorReducaoBaseCalculo = 0;
                retorno.verAplic = "MultiCTe";
                retorno.xMotivo = cte.MensagemStatus != null ? cte.MensagemStatus.MensagemDoErro : cte.MensagemRetornoSefaz;

                return retorno;
            }
            else
            {
                return null;
            }
        }

        public Dominio.NDDigital.v104.Cancelamento.Retorno GerarRetornoCancelamento(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                Dominio.NDDigital.v104.Cancelamento.Retorno retorno = new Dominio.NDDigital.v104.Cancelamento.Retorno();

                retorno.chCTe = cte.Chave;
                retorno.chCTe2 = string.Empty;
                retorno.cStat = cte.MensagemStatus != null ? string.Format("{0:000}", cte.MensagemStatus.CodigoDoErro) : string.Empty;
                retorno.dhRecbto = cte.DataRetornoSefaz.HasValue ? cte.DataRetornoSefaz.Value : DateTime.MinValue;
                retorno.digVal = string.Empty;
                retorno.id = string.Concat("ID", cte.Codigo);
                retorno.nProt = cte.Protocolo;
                retorno.tpAmb = (int)cte.TipoAmbiente;
                retorno.tpEmis = 1;
                retorno.verAplic = "MultiCTe";
                retorno.xMotivo = cte.MensagemStatus != null ? cte.MensagemStatus.MensagemDoErro : cte.MensagemRetornoSefaz;

                return retorno;
            }
            else
            {
                return null;
            }
        }
    }
}

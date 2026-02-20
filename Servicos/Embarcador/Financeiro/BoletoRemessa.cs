using System;
using System.Collections.Generic;
using Repositorio;

namespace Servicos.Embarcador.Financeiro
{
    public class BoletoRemessa : ServicoBase
    {
        public BoletoRemessa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public bool GerarRemessaDeCancelamento(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Empresa empresa)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            listaTitulo.Add(titulo);

            return MontarRemessaCancelamento(listaTitulo, unitOfWork, Auditado, empresa);
        }

        public bool GerarRemessaDeCancelamento(List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Empresa empresa)
        {
            return MontarRemessaCancelamento(listaTitulo, unitOfWork, Auditado, empresa);
        }

        #endregion

        #region Métodos Privados

        private bool MontarRemessaCancelamento(List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo repBoletoRemessaCancelamentoTitulo = new Repositorio.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo(unitOfWork);

            if (listaTitulo.Count == 0)
                return false;

            Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa = new Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa();
            remessa.DataGeracao = DateTime.Now;
            remessa.Empresa = empresa;
            remessa.NumeroSequencial = 0;
            remessa.RemessaDeCancelamento = true;

            repBoletoRemessa.Inserir(remessa, Auditado);

            Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = null;
            foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in listaTitulo)
            {
                Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo remessaCancelamento = new Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo();
                remessaCancelamento.NossoNumeroAnterior = titulo.NossoNumero;
                remessaCancelamento.Titulo = titulo;
                remessaCancelamento.BoletoRemessa = remessa;

                repBoletoRemessaCancelamentoTitulo.Inserir(remessaCancelamento);
                boletoConfiguracao = titulo.BoletoConfiguracao;
            }

            remessa.BoletoConfiguracao = boletoConfiguracao;
            repBoletoRemessa.Atualizar(remessa);

            return true;
        }

        #endregion
    }
}

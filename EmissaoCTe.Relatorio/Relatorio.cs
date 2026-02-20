using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmissaoCTe.Relatorio
{
    public partial class Relatorio : Form
    {
        public Relatorio()
        {
            try
            {
                

                InitializeComponent();
                this.GerarDACTE();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Application.Exit();
            }
        }

        public void GerarDACTE()
        {
            Servicos.DACTE svcDACTE = new Servicos.DACTE();
            svcDACTE.Gerar(1);



            Application.Exit();
        }
    }
}

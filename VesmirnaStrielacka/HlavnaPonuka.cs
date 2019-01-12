using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VesmirnaStrielacka
{
    public partial class HlavnaPonuka : UserControl
    {
        PolozkyPonuky zvolenaPolozka;
        const int sirkaPolozky = 400;
        const int vyskaPolozky = 100;
        Rectangle obdlznikNovaHra;
        Rectangle obdlznikKoniec;
        Image obrazokNovaHra = Properties.Resources.NovaHraNeoznacene;
        Image obrazokKoniec = Properties.Resources.KonecNeoznacene;

        public delegate void DelegatVyberPolozky(PolozkyPonuky p);
        public event DelegatVyberPolozky PolozkaVybrana;

        public HlavnaPonuka()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.Paint += ObsluhaUdalostiPaint;
            this.MouseDoubleClick += HlavnaPonuka_MouseClick;
        }

        private void HlavnaPonuka_Load(object sender, EventArgs e)
        {
            obdlznikNovaHra = new Rectangle((Parent.ClientRectangle.Width - sirkaPolozky) / 2, (Parent.ClientRectangle.Height - vyskaPolozky) / 2, sirkaPolozky,vyskaPolozky);
            obdlznikKoniec = new Rectangle((Parent.ClientRectangle.Width - sirkaPolozky) / 2, (Parent.ClientRectangle.Height - vyskaPolozky) / 2 + vyskaPolozky*2, sirkaPolozky, vyskaPolozky);
        }

        private void ObsluhaUdalostiPaint(object sender, PaintEventArgs e)
        {
            switch (zvolenaPolozka)
            {
                case PolozkyPonuky.NovaHra:
                    obrazokNovaHra = Properties.Resources.NovaHraOznacene;
                    obrazokKoniec = Properties.Resources.KonecNeoznacene;
                    break;
                case PolozkyPonuky.Koniec:
                    obrazokKoniec = Properties.Resources.KonecOznacene;
                    obrazokNovaHra = Properties.Resources.NovaHraNeoznacene;
                    break;
            }
            e.Graphics.DrawImage(obrazokNovaHra, obdlznikNovaHra);
            e.Graphics.DrawImage(obrazokKoniec, obdlznikKoniec);
        }

        private void HlavnaPonuka_MouseClick(object sender, MouseEventArgs e)
        {
            int pocetKlikov = e.Clicks;

            //moja uprava
            if (zvolenaPolozka == PolozkyPonuky.NovaHra && obdlznikNovaHra.Contains(e.Location))
            {
                PolozkaVybrana(zvolenaPolozka);
            }
            else if (zvolenaPolozka == PolozkyPonuky.Koniec && obdlznikKoniec.Contains(e.Location))
            {
                PolozkaVybrana(zvolenaPolozka);
            }
            //


            if (obdlznikNovaHra.Contains(e.Location))
            {
                zvolenaPolozka = PolozkyPonuky.NovaHra;
            }
            else if (obdlznikKoniec.Contains(e.Location))
            {
                zvolenaPolozka = PolozkyPonuky.Koniec;
            }

            /*if (pocetKlikov == 2)
            {
                PolozkaVybrana(zvolenaPolozka);
            }*/

            this.Invalidate();
        }
    }
}

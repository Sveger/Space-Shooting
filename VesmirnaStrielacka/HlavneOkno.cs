using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VesmirnaStrielacka
{
    public enum PolozkyPonuky
    {
        NovaHra, Koniec
    }
    


    public partial class HlavneOkno : Form
    {
        HlavnaPonuka ponuka;

        public HlavneOkno()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            ponuka = new HlavnaPonuka();
            ponuka.Parent = this;
            ponuka.Dock = DockStyle.Fill;
            ponuka.Show();
            ponuka.PolozkaVybrana += Ponuka_PolozkaVybrana;
        }

        private void Ponuka_PolozkaVybrana(PolozkyPonuky p)
        {
            switch (p)
            {
                case PolozkyPonuky.NovaHra:
                    ponuka.Dispose();
                    ponuka = null;
                    NastartujHru();
                    break;
                case PolozkyPonuky.Koniec:
                    Application.Exit();
                    break;
            }
        }

        private void NastartujHru()
        {
            this.BackColor = Color.Black;
            HernySvet hra = new HernySvet(this);
        }

    }

    public class HernySvet
    {
        Timer casovac = new Timer();
        List<HernyObjekt> zoznamHernychObjektov = new List<HernyObjekt>();
        Form hlavneOkno;
        Timer generatorNepriatelov = new Timer();
        Hrac lodHraca;
        

        public HernySvet(Form f)
        {
            MessageBox.Show("Hra bude spustena", "Sprava hry");
            hlavneOkno = f;
            casovac.Interval = 1;
            casovac.Tick += AktualizaciaHry;
            hlavneOkno.Paint += HlavneOkno_Paint;
            hlavneOkno.KeyDown += HlavneOkno_KeyDown;
            hlavneOkno.KeyUp += HlavneOkno_KeyUp;
            casovac.Start();
            lodHraca = new Hrac(this);
            Nepriatel nepriatel = new Nepriatel(this, "Grafika\\Nepritel1.bmp");
            nepriatel.Prienik += PrienikNepriatela;
            zoznamHernychObjektov.Add(lodHraca);
            zoznamHernychObjektov.Add(nepriatel);
            generatorNepriatelov.Interval = 2000;
            generatorNepriatelov.Tick += GeneratorNepriatelov_Tick;
            generatorNepriatelov.Start();


        }

        private void AktualizaciaHry(object sender, EventArgs e)
        {
            for (int i = 0; i < zoznamHernychObjektov.Count; i++)
            {
                zoznamHernychObjektov[i].AktualizujPohyb();
            }

            switch (lodHraca.PocetZasahov)
            {
                case int n when n <= 10:
                    generatorNepriatelov.Interval = 2000;
                    break;
                case int n when n <= 20:
                    generatorNepriatelov.Interval = 1000;
                    break;
                case int n when n <= 30:
                    generatorNepriatelov.Interval = 500;
                    break;
            }
            ZkontrolujKolizie();
            hlavneOkno.Invalidate();
        }

        private void HlavneOkno_Paint(object sender, PaintEventArgs e)
        {
            foreach(HernyObjekt o in zoznamHernychObjektov)
            {
                o.Vykresli(e.Graphics);
            }
            Font pismo = new Font("Arial", 9);
            e.Graphics.DrawString("Pocet zasahov : " + lodHraca.PocetZasahov, pismo, Brushes.White, HernyObjekt.min_X + 10, HernyObjekt.max_Y + 30);
        }

        private void HlavneOkno_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < zoznamHernychObjektov.Count; i++)
            {
                zoznamHernychObjektov[i].VstupHraca(sender, e);
            }
        }
        private void HlavneOkno_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < zoznamHernychObjektov.Count; i++)
            {
                zoznamHernychObjektov[i].Strielanie(sender, e);
            }
        }
      

        public void PridajObjekt(HernyObjekt h)
        {
            zoznamHernychObjektov.Add(h);
        }
        public void OdoberObjekt(HernyObjekt h)
        {
            zoznamHernychObjektov.Remove(h);
        }

        private void ZkontrolujKolizie()
        {
            
            for (int i = 0; i < zoznamHernychObjektov.Count; i++)
            {
                for (int j = i + 1; j < zoznamHernychObjektov.Count; j++)
                {
                    HernyObjekt prvy = zoznamHernychObjektov[i];
                    HernyObjekt druhy = zoznamHernychObjektov[j];

                    if ((prvy is Strela) && (druhy is Nepriatel) || (prvy is Nepriatel) && (druhy is Strela)) 
                    {
                        if (ObjektyKoliduju(prvy, druhy))
                        {
                            OdoberObjekt(prvy);
                            OdoberObjekt(druhy);
                            lodHraca.PocetZasahov++;
                            if(lodHraca.PocetZasahov == 100)
                            {
                                casovac.Stop();
                                generatorNepriatelov.Stop();
                                MessageBox.Show("Porazil si 100 nepriatelov si majster", "Sprava hry");
                            }
                            
                        }
                    }
                    else if ((prvy is Hrac) && (druhy is Nepriatel) || (prvy is Nepriatel) && (druhy is Hrac))
                    {
                        if (ObjektyKoliduju(prvy, druhy))
                        {
                            if(prvy is Hrac)
                            {
                                OdoberObjekt(druhy);
                                lodHraca.Zivoty--;
                                if (lodHraca.Zivoty == 0)
                                {
                                    OdoberObjekt(prvy);
                                    generatorNepriatelov.Stop();
                                    MessageBox.Show("Dosli ti zivoty, Prehral si!", "Sprava hry");
                                    Application.Exit();
                                }
                            }
                            else if (druhy is Hrac)
                            {
                                OdoberObjekt(prvy);
                                lodHraca.Zivoty--;
                                if (lodHraca.Zivoty == 0)
                                {
                                    OdoberObjekt(prvy);
                                    generatorNepriatelov.Stop();
                                    MessageBox.Show("Dosli ti zivoty, Prehral si!", "Sprava hry");
                                    Application.Exit();
                                }
                            }

                        }
                    }
                }
            }
        }

        private bool ObjektyKoliduju(HernyObjekt prvy, HernyObjekt druhy)
        {
            RectangleF obdPrvy = prvy.ZistiOkrajeObjektu();
            RectangleF obdDruhy = druhy.ZistiOkrajeObjektu();

            return obdPrvy.IntersectsWith(obdDruhy);
        }

        private void GeneratorNepriatelov_Tick(object sender, EventArgs e)
        {
            Random rndcis = new Random();
            Nepriatel nepriatel = null;

            if(rndcis.Next(2) == 1)
            {
                nepriatel = new Nepriatel(this, "Grafika\\Nepritel1.bmp");
            }
            else
            {
                nepriatel = new Nepriatel(this, "Grafika\\Nepritel2.bmp");
            }
            nepriatel.X = rndcis.Next(HernyObjekt.min_X, HernyObjekt.max_X - 100);

            for (int i = 0; i < zoznamHernychObjektov.Count; i++)
            {
                if (ObjektyKoliduju(zoznamHernychObjektov[i], nepriatel))
                {
                    return;
                }
            }
            nepriatel.Prienik += PrienikNepriatela;
            zoznamHernychObjektov.Add(nepriatel);
        }

        public void PrienikNepriatela()
        {
            lodHraca.Zivoty--;
            if (lodHraca.Zivoty == 0)
            {
                generatorNepriatelov.Stop();
                casovac.Stop();
                MessageBox.Show("Dosli ti zivoty, Prehral si!", "Sprava hry");
                Application.Exit();
            }
        }



    }


    class Strela : HernyObjekt
    {
        protected int rychlost = 5;


        public Strela(HernySvet hernSvet) : base(hernSvet)
        {
            obrazok = new Bitmap("Grafika\\Strela.bmp");
        }

        public override void AktualizujPohyb()
        {
            base.AktualizujPohyb();
            Y -= rychlost;

            if (Y <= 0)
            {
                instancHernehoSveta.OdoberObjekt(this);
            }
        }

    }

    class Nepriatel : HernyObjekt
    {
        public delegate void DelegatNepriatelPrenikol();
        public event DelegatNepriatelPrenikol Prienik;

        protected int rychlostHorizontalne = 10;
        protected int rychlostVertikalne = 1;

        public Nepriatel(HernySvet hernySvet, string cesta) : base(hernySvet)
        {
            Bitmap b = new Bitmap(cesta);
            b.MakeTransparent(Color.Black);
            obrazok = b;
        }

        public override void AktualizujPohyb()
        {
            base.AktualizujPohyb();
            Y += rychlostVertikalne;

            if (Y > max_Y)
            {
                Prienik();
                instancHernehoSveta.OdoberObjekt(this);
               
            }
        }


    }

    class Hrac : HernyObjekt
    {
        Image lod = Properties.Resources.Hrac;
        public int Zivoty
        {
            get;
            set;
        }
        Image zivotyObrazok;
        Rectangle zivotyObdlznik;
        public int PocetZasahov
        {
            get;
            set;
        }


        public Hrac(HernySvet hernySvet) : base(hernySvet)
        {
            Bitmap b = (Bitmap)lod;
            b.MakeTransparent(Color.Black);
            obrazok = b;
            this.X = (max_X / 2) - (obrazok.Width / 2);
            this.Y = max_Y - obrazok.Height;
            Zivoty = 3;
            PocetZasahov = 0;
            
        }

        public override void VstupHraca(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    X += 25;
                    break;
                case Keys.Left:
                    X -= 25;
                    break;
                
            }
        }
        public override void Strielanie(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                Strela strela = new Strela(instancHernehoSveta);
                strela.X = this.X + obrazok.Width / 2;
                strela.Y = this.Y;
                instancHernehoSveta.PridajObjekt(strela);
            }
        }



        public override void Vykresli(Graphics g)
        {
            base.Vykresli(g);
            switch (Zivoty)
            {
                case 3:
                    zivotyObrazok = new Bitmap("Grafika\\Zivoty3.bmp");
                    break;
                case 2:
                    zivotyObrazok = new Bitmap("Grafika\\Zivoty2.bmp");
                    break;
                case 1:
                    zivotyObrazok = new Bitmap("Grafika\\Zivoty1.bmp");
                    break;
            }
            zivotyObdlznik = new Rectangle(this.X, this.Y + obrazok.Height + 10, obrazok.Width, 20);
            g.DrawImage(zivotyObrazok, zivotyObdlznik);

        }
    }



    public abstract class HernyObjekt
    {
        protected Image obrazok;
        public const int max_X = 800;
        public const int min_X = 0;
        public const int max_Y = 550;
        protected HernySvet instancHernehoSveta;

        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get;
            set;
        }

        public HernyObjekt(HernySvet hernySvet)
        {
            this.instancHernehoSveta = hernySvet;
        }

        public virtual void AktualizujPohyb()
        {

        }

        public virtual void Vykresli(Graphics g)
        {
            g.DrawImage(obrazok, X, Y);
        }

        public virtual void VstupHraca(object sender, KeyEventArgs e)
        {

        }
        public virtual void Strielanie(object sender, KeyEventArgs e)
        {

        }
        
        
        
        public RectangleF ZistiOkrajeObjektu()                   //nechapem???
        {
            GraphicsUnit jednotka = GraphicsUnit.Pixel;
            RectangleF relativneOkraje = obrazok.GetBounds(ref jednotka);
            relativneOkraje.Offset(X, Y);
            return relativneOkraje;
        }


    }
}

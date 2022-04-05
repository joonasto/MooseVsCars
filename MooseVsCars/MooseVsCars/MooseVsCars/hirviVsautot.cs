using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

namespace Program
{
    /// <summary>
    /// @author Joonas Tolvanen
    /// @version 1.0.0.0
    /// </summary>
    public class hirviVsautot : PhysicsGame
    
    
    {
        /// <summary>
        /// T‰ss‰ esitell‰‰n pelille muuttujia
        /// ruutujen koolle, hyppynopeudelle,
        /// liikkumisnopeudelle sek‰ ladataan
        /// ohjelmaan k‰ytett‰v‰t grafiikat sek‰
        /// esitell‰‰n pelaajahahmo.
        /// Lopussa esitell‰‰n myˆs pelin 
        /// pistelaskuri ja pistelista.
        /// </summary>
        private const double NOPEUS = 200;
        private const double HYPPYNOPEUS = 500;
        private const int RUUDUN_KOKO = 40;
        const double RUUDUN_LEVEYS = 40;
        private Image kentanTausta = LoadImage("tausta1.jpg");
        private Image hirviKuva = LoadImage("hirvimodel.png");
        private Image riisikippoKuva = LoadImage("riisikippo.png");
        private PlatformCharacter pelaaja;
        private IntMeter pistelaskuri;
        private EasyHighScore topLista = new EasyHighScore();

        DoubleMeter alaspainlaskuri;
        Timer aikalaskuri;
        
        // T‰ss‰ aloitetaan peli
        // ja kutsutaan alkuvalikko
        // funktiota.

        public override void Begin()
       
        
        {
            AlkuValikko();
        }


        /// <summary>
        /// T‰m‰ funktio luo peliin
        /// kent‰n, lis‰‰ viholliset
        /// ja pelaajan, luo n‰pp‰imet,
        /// luo pistelaskurin sek‰ aika-
        /// laskurin.
        /// </summary>
        private void LuoKentta()
        {
            ClearAll();
            TileMap kentta = TileMap.FromLevelAsset("kentta.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('*', LisaaRiisikippo, 3, "riisikippo");
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
            Level.Size = new Vector(7680, 1080);
            Level.Background.Image = kentanTausta;
            Level.Background.TileToLevel();

            LisaaNappaimet();
            Level.CreateBorders();
            SetWindowSize(1920, 1080);

            Gravity = new Vector(0, -1000);

            Camera.Follow(pelaaja);
            Camera.ZoomFactor = 1.2;
            Camera.StayInLevel = true;
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, AlkuValikko, "N‰yt‰ alkuvalikko");

            LuoPistelaskuri();
            LuoAikalaskuri();
        }


        private void LisaaRiisikippo(Vector paikka, double leveys, double korkeus, int liikemaara, string kuva)

        // T‰ss‰ lis‰t‰‰n peliin vihollinen
        // ja luodaan sille attribuutit.
        {
            PlatformCharacter riisikippo = new PlatformCharacter(leveys, korkeus);
            riisikippo.Position = paikka;
            riisikippo.Image = riisikippoKuva;
            riisikippo.Tag = "riisikippo";
            Add(riisikippo);

            PathFollowerBrain pfb = new PathFollowerBrain();
            List<Vector> AjoReitti = new List<Vector>();
            AjoReitti.Add(riisikippo.Position);
            Vector seuraavaPiste = riisikippo.Position + new Vector(liikemaara * RUUDUN_LEVEYS, 0);
            AjoReitti.Add(seuraavaPiste);
            pfb.Path = AjoReitti;
            pfb.Loop = true;
            pfb.Speed = 60;
            riisikippo.Brain = pfb;
        }


        private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)

        // t‰ss‰ lis‰t‰‰n peliin pelaaja.
        {
            pelaaja = new PlatformCharacter(leveys, korkeus);
            pelaaja.Position = paikka;
            pelaaja.Mass = 4.0;
            pelaaja.Image = hirviKuva;
            AddCollisionHandler(pelaaja, "riisikippo", Tormays);
            Add(pelaaja);
        }


        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        //T‰ss‰ lis‰t‰‰n peliin
        // tasohyppely tasot.
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
            taso.Position = paikka;
            taso.Color = Color.Green;
            Add(taso);
        }


        private void LisaaNappaimet()
        // T‰ss‰ lis‰t‰‰n peliin n‰pp‰imet
        {
            Keyboard.Listen(Key.Up, ButtonState.Down, Hyppaa, "", pelaaja, HYPPYNOPEUS);
            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "", pelaaja, -NOPEUS);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "", pelaaja, NOPEUS);


            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        }


        public void Tormays(PhysicsObject pelaaja, PhysicsObject kohde)
        /// <summary>
        /// T‰m‰ aliohjelma m‰‰rittelee mit‰ tapahtuu,
        /// kun auto tˆrm‰‰ hirveen.
        /// </summary>
        /// <param name="pelaaja"></param>
        /// <param name="kohde"></param>
        {


            kohde.Destroy();
            pistelaskuri.Value += 1;
        }


        private void AlkuValikko()
        // T‰ss‰ m‰‰ritell‰‰n
        // pelille alkuvalikko.
        // Valikosta voit aloittaa
        // pelin uudelleen, tai tarkistella
        // parhaiden pisteiden listaa.
        {
            MultiSelectWindow alkuvalikko = new MultiSelectWindow("MooseVsCars", "Aloita peli", "Parhaat pisteet", "Lopeta");
            Add(alkuvalikko);
            alkuvalikko.AddItemHandler(0, LuoKentta);
            alkuvalikko.AddItemHandler(1, ParhaatPisteet);
            alkuvalikko.AddItemHandler(2, Exit);

            alkuvalikko.Color = Color.Green;
            alkuvalikko.SetButtonColor(Color.GreenYellow);

            alkuvalikko.SetButtonTextColor(Color.Black);
        }


        private void Liikuta(PlatformCharacter pelaaja, double nopeus)
        //T‰ss‰ m‰‰ritell‰‰n pelaajan
        // liikkuminen 
        {
            pelaaja.Walk(nopeus);
        }


        private void Hyppaa(PlatformCharacter pelaaja, double nopeus)
        // T‰ss‰ m‰‰ritell‰‰n
        // pelaajalle hyppy.
        {
            pelaaja.Jump(nopeus);
        }


        private void ParhaatPisteet()
        // T‰m‰ aliohjelma
        // n‰ytt‰‰ pelin top 10
        // pistetaulukon
        {


            topLista.Show();
            topLista.HighScoreWindow.Closed += AloitaUudelleen;
        }


        private void LuoPistelaskuri()
        // T‰ss‰ luodaan pelille pistelaskuri 
        // joka laskee ker‰ttyjen autojen m‰‰r‰n.
        {
            pistelaskuri = new IntMeter(0);

            Label pistenaytto = new Label();
            pistenaytto.X = Screen.Left + 50;
            pistenaytto.Y = Screen.Top - 50;
            pistenaytto.TextColor = Color.Black;
            pistenaytto.Color = Color.White;

            pistenaytto.BindTo(pistelaskuri);
            Add(pistenaytto);
        }


        private void LuoAikalaskuri()
        // T‰ss‰ luodaan aikalaskuri
        // joka mittaa aikaa 30 sekunnista 
        // alasp‰in
        {
            alaspainlaskuri = new DoubleMeter(30);

            aikalaskuri = new Timer();
            aikalaskuri.Interval = 0.1;
            aikalaskuri.Timeout += LaskeAlaspain;
            aikalaskuri.Start();

            Label aikanaytto = new Label();
            aikanaytto.TextColor = Color.White;
            aikanaytto.DecimalPlaces = 1;
            aikanaytto.BindTo(alaspainlaskuri);
            aikanaytto.Y = pelaaja.Y +100;
            Add(aikanaytto);
        }


        private void LaskeAlaspain()

        //T‰m‰ on aikalaskurin funktio
        // jolla aikaa lasketaan alasp‰in
        {
            alaspainlaskuri.Value -= 0.1;

            if (alaspainlaskuri.Value <= 0)
            {
                MessageDisplay.Add("Hienoa, tuhosit kaikki autot!");
                aikalaskuri.Stop();

                int lopullinenTulos = pistelaskuri.Value;
                
                PeliLoppui(lopullinenTulos);
            }            
        }


        private void PeliLoppui(double pelaajanTulos)

        // T‰ss‰ on pelin lopettamiseen liittyv‰ funktio
        // joka syˆtt‰‰ samalla pelaajan pisteet pistetaulukkoon
        {
            double lopullinenTulos = pelaajanTulos;
            topLista.EnterAndShow(lopullinenTulos);
            topLista.HighScoreWindow.Closed += AloitaUudelleen;
        }


        private void AloitaUudelleen(Window sender)

        // T‰ll‰ peli saadaan alkamaan uudelleen

        {
            ClearAll();
            AlkuValikko();
        }
        // TODO: silmukka, ks: https://tim.jyu.fi/answers/kurssit/tie/ohj1/2022k/demot/demo9?answerNumber=10&task=d9t1&user=tolvanjo
        // TODO: Taulukko, lista ks: https://tim.jyu.fi/answers/kurssit/tie/ohj1/2022k/demot/demo10?answerNumber=9&task=lottod10&user=tolvanjo
    }


}
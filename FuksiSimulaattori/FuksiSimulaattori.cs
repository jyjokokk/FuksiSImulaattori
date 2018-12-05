using System;
using System.Text;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class FuksiSimulaattori : PhysicsGame
{
    private PlatformCharacter fuksi;
    private BoundingRectangle spawnAlue;
    private IntMeter pisteLaskuri;
    private Timer esteAjastin;
    private DoubleMeter humalataso;
    private List<PhysicsObject> esteet = new List<PhysicsObject>();
    private double hyppyKorkeus = 1500.0;
    private double liikeNopeus = 800.0;

    public override void Begin()
    {
        AloitusValikko();
    }


    /// <summary>
    /// Tyhjentaa kentan ja aloittaa uuden pelin.
    /// </summary>
    public void AloitaPeli()
    {
        ClearAll();
        Gravity = new Vector(0.0, -1000.0);
        LuoKentta();
        LuoPistelaskuri();
        LuoHumalaPalkki();
        esteAjastin = new Timer();
        esteAjastin.Interval = 1.5;
        esteAjastin.Timeout += LuoEste;
        esteAjastin.Start();
        for (int i = 0; i < 3; i++)
        {
            LuoJuoma(this, spawnAlue);
        }
        AsetaOhjaimet();
        AddCollisionHandler<PlatformCharacter, PhysicsObject>(fuksi, "juoma", TormasiJuomaan);
        AddCollisionHandler<PlatformCharacter, PhysicsObject>(fuksi, "este", TormasiEsteeseen);
    }


    /// <summary>
    /// Asettaa peliin ohjaimet pelihahmon liikuttamiseksi.
    /// </summary>
    public void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Up, ButtonState.Pressed, HahmoHyppaa, "Hahmo hyppaa", fuksi);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, HahmoVasemalle, "Hahmo liikkuu vasemmalle", fuksi);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, HahmoOikealle, "Hahmo liikkuu oikealle", fuksi);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.K, ButtonState.Pressed, Katkaisuhoito, "Katkaisuhoito");
        Keyboard.Listen(Key.P, ButtonState.Pressed, PauseMenu, "Pysayttaa pelin");
    }


    /// <summary>
    /// Luo kentan ja lisaa sinne pelihahmon.
    /// Maarittaa samalla spawnAlueen sijainnin ja koon.
    /// </summary>
    public void LuoKentta()
    {
        Level.CreateBorders();
        Camera.ZoomToLevel();
        Vector ylakulma = new Vector(Screen.Left + 80.0, Screen.Top - Screen.Height / 1.8);
        Vector alakulma = new Vector(Screen.Right - Screen.Width / 3, Screen.Bottom + 80.0);
        spawnAlue = new BoundingRectangle(ylakulma, alakulma);
        fuksi = new PlatformCharacter(40.0, 80.0);
        fuksi.X = Screen.Left + 80.0;
        fuksi.Y = Screen.Bottom + 10.0;
        fuksi.Shape = Shape.Rectangle;
        fuksi.Color = Color.Black;
        Add(fuksi);
    }


    /// <summary>
    /// Luo pistelaskurin oikeaan ylakulmaan.
    /// </summary>
    public void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Right - 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.IntFormatString = "Pisteita: {0:D2}";
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }
    

    /// <summary>
    /// Luo humalapalkin, joka sidotaan nayttamaan humalamittarin tila.
    /// </summary>
    public void LuoHumalaPalkki()
    {
        humalataso = new DoubleMeter(0);
        humalataso.MaxValue = 2.0;
        ProgressBar humalaPalkki = new ProgressBar(200, 30);
        humalaPalkki.BindTo(humalataso);
        humalaPalkki.X = Screen.Left + 170;
        humalaPalkki.Y = Screen.Top - 40;
        humalaPalkki.BarColor = Color.Red;
        humalaPalkki.BorderColor = Color.Black;
        Add(humalaPalkki);
    }


    /// <summary>
    /// Lopettaa pelin, tulostaa syyn havioon, ja avaa monivalintaikkunan.
    /// </summary>
    /// <param name="syy">Syy.</param>
    public void PeliPaattyi(string syy)
    {
        fuksi.Stop();
        esteAjastin.Stop();
        for (int i = 0; i < esteet.Count; i++)
        {
            esteet[i].Stop();
        }
        StringBuilder viesti = new StringBuilder(syy + "\nTuloksesi: ");
        viesti.Append(pisteLaskuri.Value);
        MultiSelectWindow valikko = new MultiSelectWindow(viesti.ToString(),
                                                          "Aloita alusta",
                                                          "Lopeta");
        valikko.ItemSelected += ValikonNappi;
        Add(valikko);
    }


    /// <summary>
    /// Nayttaa pelin alussa aloitusvalikon.
    /// </summary>
    public void AloitusValikko()
    {
        MultiSelectWindow valikko = new MultiSelectWindow("Tervetuloa FuksiSimulaattoriin!",
                                                          "Aloita peli",
                                                          "Lopeta");
        valikko.ItemSelected += ValikonNappi;
        Add(valikko);
    }


    /// <summary>
    /// Aliohjelma valikkojen suorittamiselle.
    /// </summary>
    /// <param name="valinta">Valinta.</param>
    public void ValikonNappi(int valinta)
    {
        switch (valinta)
        {
            case 0:
                AloitaPeli();
                break;
            case 1:
                Exit();
                break;
        }
    }


    /// <summary>
    /// Pysayttaa pelin, ja nayttaa monivalintaikkunan.
    /// </summary>
    public void PauseMenu()
    {
        IsPaused = true;
        MultiSelectWindow valikko = new MultiSelectWindow("Pause", "Jatka", "Aloita alusta");
        valikko.ItemSelected += PauseValitse;
        Add(valikko);
        // Nestataan PauseValitse, ettei tarvitse luoda uutta julkista aliohjelmaa.
        void PauseValitse(int valinta)
        {
            switch (valinta)
            {
                case 0:
                    IsPaused = false;
                    Remove(valikko);
                    break;
                case 1:
                    AloitaPeli();
                    break;
            }
        }
    }


    /// <summary>
    /// Poistaa pelista juoman ja lisaa pisteen laskuriin.
    /// </summary>
    /// <param name="hahmo">Pelaaja.</param>
    /// <param name="juoma">Juoma.</param>
    public void TormasiJuomaan(PlatformCharacter hahmo, PhysicsObject juoma)
    {
        juoma.IgnoresCollisionResponse = true;
        juoma.Destroy();
        LuoJuoma(this, spawnAlue);
        pisteLaskuri.Value += 1;
        humalataso.Value += 0.2;
        hyppyKorkeus = HumalaKerroin(hyppyKorkeus, humalataso);
        liikeNopeus = HumalaKerroin(liikeNopeus, humalataso);
        if (humalataso.Value >= 1.25) PeliPaattyi("Joit itsesi hengilta!");
        if (humalataso.Value >= 0.875)
        {
            Label viesti = new Label(Level.Width / 3, 80.0, "Varo! Juoppuhulluus lahestyy!");
            viesti.Color = Color.White;
            viesti.LifetimeLeft = TimeSpan.FromSeconds(3.0);
            Add(viesti);
        }
    }


    /// <summary>
    /// Palauttaa humalakertoimen avulla kerrotun arvon. Arvo voi kasvaa,
    /// pienentya, tai pysya ennallaan.
    /// </summary>
    /// <returns>Muutettu arvo.</returns>
    /// <param name="arvo">Arvo jota muututaan.</param>
    public static double HumalaKerroin(double arvo, DoubleMeter taso)
    {
        int mitenMuuttuu = RandomGen.NextInt(-1, 1);
        double muutettuArvo = arvo + taso.Value * mitenMuuttuu * arvo;
        return muutettuArvo;
    }


    /// <summary>
    /// Lopettaa pelin pelaajan tormatessa esteeseen.
    /// </summary>
    /// <param name="hahmo">Pelaaja.</param>
    /// <param name="este">Este.</param>
    public void TormasiEsteeseen(PlatformCharacter hahmo, PhysicsObject este)
    {
        PeliPaattyi("Opinnot jyrasivat sinut alleensa!");
    }


    /// <summary>
    /// Pelihahmo hyppaa ylospain annetun impulssin voimakkuudella.
    /// </summary>
    /// <param name="hahmo">Pelihahmo.</param>
    /// <param name="impulssi">Impulssi.</param>
    public void HahmoHyppaa(PlatformCharacter hahmo)
    {
        hahmo.Jump(hyppyKorkeus);
    }


    /// <summary>
    /// Pelihahmo liikkuu annettuun suuntaan.
    /// </summary>
    /// <param name="hahmo">Pelihahmo.</param>
    public void HahmoOikealle(PlatformCharacter hahmo)
    {
        hahmo.Walk(liikeNopeus);
    }


    /// <summary>
    /// Pelihahmo liikkuu annettuun suuntaan.
    /// </summary>
    /// <param name="hahmo">Pelihahmo.</param>
    public void HahmoVasemalle(PlatformCharacter hahmo)
    {
        hahmo.Walk(-liikeNopeus);
    }


    /// <summary>
    /// Nollaa humalamittarin, ja vahentaa 5 pistetta.
    /// </summary>
    public void Katkaisuhoito()
    {
        if (pisteLaskuri.Value > 0)
        {
            humalataso.Value = 0;
            pisteLaskuri.Value -= 1;
        }
        hyppyKorkeus = 1500.0;
        liikeNopeus = 800.0;
        Label viesti = new Label(Level.Width / 4, 80.0, "Katkolle!");
        viesti.Color = Color.White;
        viesti.LifetimeLeft = TimeSpan.FromSeconds(3.0);
        Add(viesti);
    }


    /// <summary>
    /// Luo peliin juoman satunnaiseen sijaintiin maaratylla spawnAlueelle.
    /// </summary>
    /// <returns>juoma.</returns>
    /// <param name="peli">Peli johon juoma luodaan.</param>
    /// <param name="rect">Maaratty alue, johon juoma luodaan.</param>
    public static void LuoJuoma(PhysicsGame peli, BoundingRectangle rect)
    {
        PhysicsObject juoma = PhysicsObject.CreateStaticObject(15.0, 30.0);
        juoma.Shape = Shape.Rectangle;
        juoma.Color = Color.Red;
        juoma.Position = RandomGen.NextVector(rect);
        juoma.Tag = "juoma";
        peli.Add(juoma);
    }


    /// <summary>
    /// Luo peliin esteen. Poistaa samalla pelista naytolla nakymattomat esteet.
    /// </summary>
    public void LuoEste()
    {
        int korkeus = RandomGen.NextInt(100, 500);
        PhysicsObject este = PhysicsObject.CreateStaticObject(30.0, korkeus);
        este.Shape = Shape.Rectangle;
        este.Color = Color.Brown;
        este.X = Level.Right;
        este.Y = Level.Bottom;
        este.Velocity = new Vector(-200.0, 0);
        este.Tag = "este";
        esteet.Add(este);
        //this.Add(este);
        if (esteet.Count > 3)
        {
            for (int i = 0; i < esteet.Count; i++)
            {
                if (esteet[i].X <= Level.Left) esteet[i].Destroy();
            }
        }
    }
}
